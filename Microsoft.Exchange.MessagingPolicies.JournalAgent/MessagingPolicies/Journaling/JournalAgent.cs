using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.RecipientAPI;
using Microsoft.Exchange.Transport.RightsManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal class JournalAgent : RoutingAgent
	{
		public JournalAgent(SmtpServer server, JournalPerfCountersWrapper perfCountersWrapper)
		{
			this.server = server;
			base.OnSubmittedMessage += this.OnSubmitEvent;
			base.OnRoutedMessage += this.OnRoutedEventAndPerf;
			this.perfCountersWrapper = perfCountersWrapper;
		}

		private static bool IsJournalNdrWithSkipRulesStamped(MailItem mailItem)
		{
			return null != mailItem.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-JournalNdr-Skip-TransportMailboxRules");
		}

		private static bool IsRecipientTypeMailbox(MailItem mailItem, string email)
		{
			IADRecipientCache cache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
			ADRawEntry adrawEntry;
			object obj;
			if (Utils.TryGetADRawEntryByEmailAddress(cache, email, out adrawEntry) && adrawEntry.TryGetValueWithoutDefault(ADRecipientSchema.RecipientType, out obj) && obj != null && obj is Microsoft.Exchange.Data.Directory.Recipient.RecipientType)
			{
				Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)obj;
				if (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsJournalFilterEnabled()
		{
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\BCM", "DisableJournalingToDCMailboxFilter", 0);
				if (value is int && (int)value != 0)
				{
					return false;
				}
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			return true;
		}

		private static void TagRecipientTypes(RecipientP2Type recipientListType, List<RoutingAddress> recipientAddressList, List<EnvelopeRecipient> envelopeRecipients, EmailRecipientCollection p2Recipients)
		{
			foreach (EmailRecipient emailRecipient in p2Recipients)
			{
				if (RoutingAddress.IsValidAddress(emailRecipient.SmtpAddress))
				{
					RoutingAddress routingAddress = new RoutingAddress(emailRecipient.SmtpAddress);
					EnvelopeRecipient envelopeRecipient = JournalAgent.FindUntaggedRecipient(routingAddress, recipientAddressList, envelopeRecipients);
					if (envelopeRecipient != null)
					{
						envelopeRecipient.Properties["Microsoft.Exchange.Transport.RecipientP2Type"] = (int)recipientListType;
					}
					else
					{
						ExTraceGlobals.JournalingTracer.TraceDebug<RoutingAddress>(0L, "Misbehaving MUA. Recipient {0} was listed in P2, but is not a P1 recipient", routingAddress);
					}
				}
			}
		}

		private static EnvelopeRecipient FindUntaggedRecipient(RoutingAddress searchAddress, List<RoutingAddress> recipientAddressList, List<EnvelopeRecipient> envelopeRecipients)
		{
			int num = recipientAddressList.BinarySearch(searchAddress, MessageChecker.AddressComparer);
			if (num < 0)
			{
				return null;
			}
			int count = envelopeRecipients.Count;
			object obj;
			int num2 = envelopeRecipients[num].Properties.TryGetValue("Microsoft.Exchange.Transport.RecipientP2Type", out obj) ? 1 : -1;
			EnvelopeRecipient envelopeRecipient;
			for (;;)
			{
				int num3 = num + num2;
				if (num3 < 0 || num3 >= count)
				{
					goto IL_98;
				}
				envelopeRecipient = envelopeRecipients[num + num2];
				if (envelopeRecipient.Address != searchAddress)
				{
					goto IL_98;
				}
				bool flag = envelopeRecipient.Properties.TryGetValue("Microsoft.Exchange.Transport.RecipientP2Type", out obj);
				if (num2 == 1 && !flag)
				{
					break;
				}
				if (num2 == -1 && flag)
				{
					goto Block_9;
				}
				num += num2;
			}
			return envelopeRecipient;
			Block_9:
			return envelopeRecipients[num];
			IL_98:
			if (!envelopeRecipients[num].Properties.TryGetValue("Microsoft.Exchange.Transport.RecipientP2Type", out obj))
			{
				return envelopeRecipients[num];
			}
			return null;
		}

		private static void NdrAllRecipients(MailItem mailItem, SmtpResponse smtpResponse)
		{
			int count = mailItem.Recipients.Count;
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			for (int i = count - 1; i >= 0; i--)
			{
				recipients.Remove(recipients[i], DsnType.Failure, smtpResponse);
			}
		}

		private void OnSubmitEvent(SubmittedMessageEventSource source, QueuedMessageEventArgs args)
		{
			MailItem mailItem = args.MailItem;
			string messageId = mailItem.Message.MessageId;
			this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
			{
				messageId
			});
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			OrganizationId organizationId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
			using (JournalLogContext journalLogContext = new JournalLogContext("JA", "OnSubmittedMessage", messageId, mailItem))
			{
				this.configuration = Configuration.GetConfig(organizationId);
				if (this.configuration == null)
				{
					this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
					{
						messageId,
						"Failed to load configuration."
					});
					ExTraceGlobals.JournalingTracer.TraceError(0L, "OnSubmitted: Putting this message into retry, as configuration could not be loaded.");
					source.Defer(JournalingGlobals.RetryIntervalOnError);
					journalLogContext.LogAsSkipped("Cfg", new object[0]);
				}
				else
				{
					Exception ex = null;
					Exception ex2 = null;
					SmtpResponse smtpResponse = SmtpResponse.Empty;
					try
					{
						object obj;
						if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.ProcessedOnSubmitted", out obj))
						{
							this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
							{
								messageId,
								"Message was already processed."
							});
							ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Message was already processed OnSubmitted, skipping: {0}", messageId);
							journalLogContext.LogAsSkipped("Microsoft.Exchange.Journaling.ProcessedOnSubmitted", new object[]
							{
								obj
							});
							return;
						}
						if (this.IsProcessedByUnjournal(mailItem))
						{
							this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
							{
								messageId,
								"Message was already processed by unjournal agent."
							});
							ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Message was already processed by unjournal agent, skipping: {0}", messageId);
							journalLogContext.LogAsSkipped("IsProcUnJR", new object[0]);
							return;
						}
						if (this.IsDestinationJournalNdrDC(mailItem))
						{
							this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
							{
								messageId,
								"Skip message destined to journal NDR mailbox to avoid loop."
							});
							mailItem.FromAddress = RoutingAddress.NullReversePath;
							this.MarkJournalNdrMessagesShouldSkipRules(mailItem);
							ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Message is destined to journal ndr mailbox. We dont journal messages to journal ndr mailbox as there is a potential of a loop PS 685340, skipping: {0}", messageId);
							journalLogContext.LogAsSkipped("IsDestJNdrDC", new object[0]);
							return;
						}
						this.messageChecker = new MessageChecker(mailItem, this.configuration);
						CheckStatus checkStatus = this.messageChecker.CheckJournalReport();
						this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
						{
							messageId,
							"Calling CheckJournalReport and check status returned is ",
							checkStatus.ToString()
						});
						journalLogContext.AddParameter("MC", new object[]
						{
							checkStatus
						});
						if (checkStatus == CheckStatus.TransportJournalReport)
						{
							ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "No need to process journal-report");
							journalLogContext.LogAsSkipped("MC", new object[]
							{
								CheckStatus.TransportJournalReport
							});
						}
						else if (checkStatus == CheckStatus.MailboxJournalReport)
						{
							this.PatchJournalReportFromMailbox(mailItem);
							journalLogContext.LogAsSkipped("PJRFM", new object[0]);
						}
						else if (this.messageChecker.CheckResubmittedMessage())
						{
							this.InferRecipientTypesForMapiResubmit(mailItem);
							journalLogContext.LogAsSkipped("IRTFMR", new object[0]);
						}
						else
						{
							List<EnvelopeRecipient> envelopeRecipients = null;
							List<RoutingAddress> list = null;
							this.GetSortedRecipientList(mailItem, out list, out envelopeRecipients);
							ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Saving original recipient list before categorization. If recipients are added to this list, we will need to re-run journaling rules");
							mailItem.Properties["Microsoft.Exchange.Journaling.OriginalRecipientInfo"] = list;
							if (this.messageChecker.CheckMuaSubmission())
							{
								this.InferRecipientTypesForMuaSubmit(mailItem, list, envelopeRecipients);
							}
						}
						mailItem.Properties["Microsoft.Exchange.Journaling.ProcessedOnSubmitted"] = true;
						string value = string.Format("@{0}>", "journal.report.generator");
						if (mailItem.InternetMessageId != null && mailItem.InternetMessageId.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
						{
							this.TrackAgentInfoForJournalReportMessage(source, mailItem);
						}
					}
					catch (TransientException ex3)
					{
						ex2 = ex3;
					}
					catch (ExchangeDataException ex4)
					{
						ex = ex4;
						smtpResponse = SmtpResponse.InvalidContent;
					}
					catch (UnauthorizedSubmitterException ex5)
					{
						ex = ex5;
						smtpResponse = JournalAgent.JournalReportFromUnauthorizedSender;
					}
					if (ex != null)
					{
						this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
						{
							messageId,
							"Fatal error: ",
							ex
						});
						ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "NDR'ing all recipients in OnSubmitted because of a fatal error: {0}", ex.ToString());
						JournalAgent.NdrAllRecipients(mailItem, smtpResponse);
						journalLogContext.LogAsFatalError(new object[]
						{
							ex
						});
					}
					else if (ex2 != null)
					{
						this.SetBreadCrumbInfo("OnSubmitEvent", new object[]
						{
							messageId,
							"Retryable error: ",
							ex2
						});
						ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Deferring message due to retryable error: {0}", ex2.ToString());
						source.Defer(JournalingGlobals.RetryIntervalOnError);
						journalLogContext.LogAsRetriableError(new object[]
						{
							ex2
						});
					}
				}
			}
		}

		private bool IsProcessedByUnjournal(MailItem mailItem)
		{
			object obj = null;
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.ProcessedByUnjournal.Enabled;
			if (enabled && this.configuration != null && (this.configuration.LegacyArchiveJournalingEnabled || this.configuration.LegacyArchiveLiveJournalingEnabled) && (mailItem.Properties.TryGetValue("Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmitted", out obj) || mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Unjournal-Processed") != null || mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Unjournal-ProcessedNdr") != null))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "This is a message generated by unjournal agent as part of unjournaling process , journal agent should skip it.");
				return true;
			}
			return false;
		}

		private bool IsDestinationJournalNdrDC(MailItem mailItem)
		{
			RoutingAddress journalReportNdrTo = this.configuration.JournalReportNdrTo;
			RoutingAddress journalReportNdrToForGcc = this.configuration.JournalReportNdrToForGcc;
			if (mailItem.Recipients.Count == 1 && (mailItem.Recipients[0].Address.CompareTo(journalReportNdrTo) == 0 || mailItem.Recipients[0].Address.CompareTo(journalReportNdrToForGcc) == 0) && JournalAgent.IsRecipientTypeMailbox(mailItem, mailItem.Recipients[0].Address.ToString()))
			{
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "This message is destined to journal ndr mailbox, journal agent should skip it.");
				return true;
			}
			return false;
		}

		private void PatchJournalReportFromMailbox(MailItem mailItem)
		{
			string messageId = mailItem.Message.MessageId;
			string text = this.configuration.MSExchangeRecipient.ToString();
			mailItem.FromAddress = this.configuration.JournalReportNdrTo;
			mailItem.Message.Sender = new EmailRecipient(null, text);
			AttachmentCollection attachments = mailItem.Message.Attachments;
			if (attachments[0].MimePart.IsEmbeddedMessage)
			{
				this.SetBreadCrumbInfo("PatchJournalReportFromMailbox", new object[]
				{
					messageId,
					"Attachment was embedded message, rewriting From."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Attachment was embedded message, rewriting From:");
				mailItem.Message.From = ((attachments[0].EmbeddedMessage.From == null) ? null : new EmailRecipient(attachments[0].EmbeddedMessage.From.DisplayName, attachments[0].EmbeddedMessage.From.SmtpAddress));
			}
			((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.PrepareJournalReport();
			this.SetBreadCrumbInfo("PatchJournalReportFromMailbox", new object[]
			{
				messageId,
				"Sender was reset to: ",
				text
			});
			ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "Patched up resent/ELC journal report. Sender was reset to: {0}", text);
		}

		private void OnRoutedEventAndPerf(RoutedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (source == null || args == null)
			{
				throw new ArgumentException("internal transport error");
			}
			this.timer.Reset();
			this.timer.Start();
			MailItem mailItem = args.MailItem;
			string messageId = mailItem.Message.MessageId;
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			OrganizationId organizationId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
			this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
			{
				messageId
			});
			using (JournalLogContext journalLogContext = new JournalLogContext("JA", "OnRoutedMessage", messageId, mailItem))
			{
				this.RemoveMailboxRecipientsForJournalReportForDataCenter(mailItem, organizationId);
				this.configuration = Configuration.GetConfig(organizationId);
				if (this.configuration == null)
				{
					this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
					{
						messageId,
						"Failed to load configuration."
					});
					ExTraceGlobals.JournalingTracer.TraceError(0L, "OnRouted: Putting this message into retry, as configuration could not be loaded.");
					source.Defer(JournalingGlobals.RetryIntervalOnError);
					journalLogContext.LogAsSkipped("Cfg", new object[0]);
				}
				else if (this.IsProcessedByUnjournal(mailItem))
				{
					this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
					{
						messageId,
						"Message was already processed by unjournal agent."
					});
					ExTraceGlobals.JournalingTracer.TraceDebug(0L, "This is a message generated by unjournal agent as part of unjournaling process , journal agent should skip it.");
					journalLogContext.LogAsSkipped("UnJR", new object[0]);
				}
				else
				{
					bool flag = this.IsDestinationJournalNdrDC(mailItem);
					bool flag2 = JournalAgent.IsJournalNdrWithSkipRulesStamped(mailItem);
					if (flag || flag2)
					{
						this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
						{
							messageId,
							"Skip message destined to journal NDR mailbox to avoid loop."
						});
						ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Message is destined to journal ndr mailbox. We dont journal messages to journal ndr mailbox as there is a potential of a loop PS 685340, skipping: {0}", messageId);
						journalLogContext.LogAsSkipped("IsDestJNdrDCORIsJNdrSRS", new object[]
						{
							flag,
							flag2
						});
					}
					else
					{
						this.messageChecker = new MessageChecker(args.MailItem, this.configuration);
						if (organizationId != OrganizationId.ForestWideOrgId && !this.IsAlreadyProcessedByGcc() && this.ProcessForGccJournaling(source, args, journalLogContext) == JournalAgent.ProcessingStatus.TransientError)
						{
							this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
							{
								messageId,
								"Process for GCC Journaling returns transient error."
							});
							journalLogContext.LogAsSkipped("GccJR", new object[]
							{
								JournalAgent.ProcessingStatus.TransientError
							});
						}
						else
						{
							object obj = null;
							if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.ProcessedOnRouted", out obj))
							{
								this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
								{
									messageId,
									"Skip message because it is already processed."
								});
								ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Message was already processed OnRouted, skipping");
								journalLogContext.LogAsSkipped("OnRt", new object[]
								{
									obj
								});
							}
							else
							{
								PerfCounters.MessagesProcessed.Increment();
								JournalAgent.ProcessingStatus processingStatus;
								if (this.configuration.LegacyJournalingMigrationEnabled && this.messageChecker.IsLegacyJournalReport())
								{
									processingStatus = this.ProcessForLegacyJournaling(source, args, journalLogContext);
								}
								else
								{
									processingStatus = this.ProcessForJournaling(source, args, journalLogContext);
								}
								this.SetBreadCrumbInfo("OnRoutedEventAndPerf", new object[]
								{
									messageId,
									"Processing status: ",
									processingStatus.ToString()
								});
								journalLogContext.AddParameter("PSt", new object[]
								{
									processingStatus
								});
								switch (processingStatus)
								{
								case JournalAgent.ProcessingStatus.PermanentError:
								case JournalAgent.ProcessingStatus.Success:
									this.MarkProcessingDone(args.MailItem);
									this.UpdateProcessingTime();
									break;
								case JournalAgent.ProcessingStatus.TransientError:
									this.UpdateProcessingTime();
									break;
								}
							}
						}
					}
				}
			}
		}

		private void RemoveMailboxRecipientsForJournalReportForDataCenter(MailItem mailItem, OrganizationId orgId)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.RemoveMailboxFromJournalRecipients.Enabled;
			if (!enabled || JournalAgent.DisableJournalReportFilterToDCMailbox)
			{
				return;
			}
			string messageId = mailItem.Message.MessageId;
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			if (headers.FindFirst("X-MS-Journal-Report") == null || headers.FindFirst("X-MS-Gcc-Journal-Report") != null)
			{
				this.SetBreadCrumbInfo("RemoveMailboxRecipientsForJournalReportForDataCenter", new object[]
				{
					messageId,
					"Not a journal report OR it is a GCC journal"
				});
				return;
			}
			if (headers.FindFirst("X-MS-Exchange-Organization-Unjournal-ProcessedNdr") != null)
			{
				return;
			}
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(mailItem.Recipients.Count);
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				if (envelopeRecipient.OutboundDeliveryMethod == DeliveryMethod.Mailbox)
				{
					list.Add(envelopeRecipient);
				}
			}
			if (list.Count > 0)
			{
				RoutingAddress address = list[0].Address;
				foreach (EnvelopeRecipient envelopeRecipient2 in list)
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<RoutingAddress, string>(0L, "Dropping recipient {0} from message {1} because it is a journal report for a datacenter mailbox", envelopeRecipient2.Address, mailItem.InternetMessageId);
					mailItem.Recipients.Remove(envelopeRecipient2);
				}
				JournalingGlobals.Logger.LogEvent(orgId, MessagingPoliciesEventLogConstants.Tuple_JournalingDroppingJournalReportToDCMailbox, string.Empty, address);
				string text = string.Format("Some journal reports could not be delivered because the destination is a mailbox in the datacenter. Using a datacenter mailbox as the target address of a journal rule is not supported. One of your organization's journal rules is using {0} as the target address.", address);
				this.SetBreadCrumbInfo("RemoveMailboxRecipientsForJournalReportForDataCenter", new object[]
				{
					messageId,
					text
				});
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, text);
			}
		}

		private bool IsAlreadyProcessedByGcc()
		{
			return this.messageChecker.ShouldSkipGccJournaling();
		}

		private void MarkMessageAsProcessedByFirstHop(MailItem mailItem)
		{
			string messageId = mailItem.Message.MessageId;
			try
			{
				this.SetBreadCrumbInfo("MarkMessageAsProcessedByFirstHop", new object[]
				{
					messageId,
					"Check first Exchange hop."
				});
				if (this.messageChecker.CheckFirstExchangeHop())
				{
					HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
					headers.AppendChild(new TextHeader("X-MS-Exchange-Organization-Processed-By-Journaling", "Journal Agent"));
				}
			}
			catch (ExchangeDataException ex)
			{
				this.SetBreadCrumbInfo("MarkMessageAsProcessedByFirstHop", new object[]
				{
					messageId,
					"NDR recipients after getting exception: ",
					ex
				});
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to mark message as processed by first-hop, CTS Exception: {0}. NDR'ing recipients.", ex.ToString());
				JournalAgent.NdrAllRecipients(mailItem, SmtpResponse.InvalidContent);
			}
		}

		private JournalAgent.ProcessingStatus ProcessForGccJournaling(RoutedMessageEventSource source, QueuedMessageEventArgs args, JournalLogContext logContext)
		{
			this.savedMessageId = args.MailItem.Message.MessageId;
			this.SetBreadCrumbInfo("ProcessForGccJournaling", new object[]
			{
				this.savedMessageId
			});
			List<GccRuleEntry> gccrules = this.configuration.GCCRules;
			logContext.AddParameter("GRN", new object[]
			{
				(gccrules == null) ? 0 : gccrules.Count
			});
			if (gccrules == null || gccrules.Count == 0)
			{
				this.SetBreadCrumbInfo("ProcessForGccJournaling", new object[]
				{
					this.savedMessageId,
					"Skip GCC journaling since there are no GCC rules."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Skipping GCC journaling because there are no GCC rules.");
				this.MarkGccProcessingDone(args.MailItem);
				return JournalAgent.ProcessingStatus.Success;
			}
			Exception ex = null;
			try
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				List<string> list4 = new List<string>();
				List<string> list5 = new List<string>();
				List<string> list6 = new List<string>();
				MailItem mailItem = args.MailItem;
				foreach (GccRuleEntry gccRuleEntry in gccrules)
				{
					DateTime? expiryDate = gccRuleEntry.ExpiryDate;
					if (expiryDate != null)
					{
						DateTime date = DateTime.UtcNow.Date;
						DateTime? expiryDate2 = gccRuleEntry.ExpiryDate;
						if (date > expiryDate2.Value.Date)
						{
							this.SetBreadCrumbInfo("ProcessForGccJournaling", new object[]
							{
								this.savedMessageId,
								"Skip GCC rule because it passed expiration date: ",
								gccRuleEntry.Name
							});
							ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Skipping GCC rule with name '{0}' because it is passed the expiry date.", gccRuleEntry.Name);
							continue;
						}
					}
					UserComparer userComparer = new UserComparer(this.server.AddressBook);
					if (Utils.IsSmtpAddressSenderOrRecipientOnMessage(gccRuleEntry.Recipient, mailItem, userComparer))
					{
						this.SetBreadCrumbInfo("ProcessForGccJournaling", new object[]
						{
							this.savedMessageId,
							"Recipient is on the message."
						});
						logContext.AddParameter("GR", new object[]
						{
							gccRuleEntry.ImmutableId,
							gccRuleEntry.Name,
							gccRuleEntry.Recipient
						});
						ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
						ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
						OrganizationId orgId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
						SmtpAddress recipient = gccRuleEntry.Recipient;
						RoutingAddress address = (RoutingAddress)recipient.ToString();
						if (this.IsInternal(address, orgId))
						{
							if (gccRuleEntry.FullReport)
							{
								SmtpAddress journalEmailAddress = gccRuleEntry.JournalEmailAddress;
								Utils.AddRecipSortedToList(journalEmailAddress.ToString(), ref list);
								list3.Add(gccRuleEntry.Name);
								List<string> list7 = list5;
								Guid immutableId = gccRuleEntry.ImmutableId;
								list7.Add(immutableId.ToString("D"));
							}
							else
							{
								SmtpAddress journalEmailAddress2 = gccRuleEntry.JournalEmailAddress;
								Utils.AddRecipSortedToList(journalEmailAddress2.ToString(), ref list2);
								list4.Add(gccRuleEntry.Name);
								List<string> list8 = list6;
								Guid immutableId2 = gccRuleEntry.ImmutableId;
								list8.Add(immutableId2.ToString("D"));
							}
						}
					}
				}
				logContext.AddParameter("GRFRR", new object[]
				{
					list3
				});
				logContext.AddParameter("GRFRRId", new object[]
				{
					list5
				});
				if (list.Count != 0)
				{
					logContext.AddParameter("GRFR", new object[]
					{
						list
					});
					this.CreateAndSubmitGccReport(list, list3, mailItem, true, list5, source);
				}
				if (list2.Count != 0)
				{
					logContext.AddParameter("GRPRTT", new object[]
					{
						list2
					});
					this.CreateAndSubmitGccReport(list2, list4, mailItem, false, list6, source);
				}
				this.MarkGccProcessingDone(args.MailItem);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (TransientException ex3)
			{
				ex = ex3;
			}
			catch (RuleInvalidOperationException ex4)
			{
				ex = ex4;
			}
			catch (JournalReport.ReportGenerationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				this.SetBreadCrumbInfo("ProcessForGccJournaling", new object[]
				{
					this.savedMessageId,
					"Putting message into retry after getting retriable exception: ",
					ex
				});
				ExTraceGlobals.JournalingTracer.TraceError<long, Exception>((long)this.GetHashCode(), "Putting {0} message into retry, as there was an error during GCC journaling: {1}.", this.originalMailItem.InternalMessageId, ex);
				this.DeferMessageAndUpdateCounter(source, true);
				logContext.LogAsRetriableError(new object[]
				{
					"ProcessForGccJournaling",
					ex
				});
				return JournalAgent.ProcessingStatus.TransientError;
			}
			return JournalAgent.ProcessingStatus.Success;
		}

		private void CreateAndSubmitGccReport(List<string> recipientsOfJournalReport, List<string> ruleNames, MailItem originalMailItem, bool fullReport, List<string> ruleIds, RoutedMessageEventSource source)
		{
			string messageId = originalMailItem.Message.MessageId;
			this.SetBreadCrumbInfo("CreateAndSubmitGccReport", new object[]
			{
				messageId,
				"Create GCC journal report."
			});
			ITransportMailItemFacade transportMailItemFacade = JournalReport.CreateReport(this.configuration, originalMailItem, recipientsOfJournalReport, null, true, fullReport, ruleNames, null);
			this.PassDataForJournalReportAgentInfo((TransportMailItem)transportMailItemFacade, messageId, true, ruleIds);
			this.CheckAndMarkInternalGeneratedJournalReport(transportMailItemFacade as TransportMailItem);
			transportMailItemFacade.CommitLazy();
			this.TrackAgentInfoForOriginalMessage(source, transportMailItemFacade.Message.MessageId, ruleIds, recipientsOfJournalReport, true);
			long recordId = ((ITransportMailItemWrapperFacade)originalMailItem).TransportMailItem.RecordId;
			TransportFacades.TrackReceiveByAgent(transportMailItemFacade, "Journaling", null, new long?(recordId));
			this.SetBreadCrumbInfo("CreateAndSubmitGccReport", new object[]
			{
				messageId,
				"Enqueue side effect message."
			});
			TransportFacades.CategorizerComponent.EnqueueSideEffectMessage(((ITransportMailItemWrapperFacade)originalMailItem).TransportMailItem, transportMailItemFacade, "Journal Agent");
		}

		private JournalAgent.ProcessingStatus ProcessForLegacyJournaling(RoutedMessageEventSource source, QueuedMessageEventArgs args, JournalLogContext logContext)
		{
			this.savedMessageId = args.MailItem.Message.MessageId;
			MailItem mailItem = args.MailItem;
			Exception ex = null;
			this.SetBreadCrumbInfo("ProcessForLegacyJournaling", new object[]
			{
				this.savedMessageId
			});
			List<string> list = new List<string>();
			List<EnvelopeRecipient> list2 = new List<EnvelopeRecipient>();
			try
			{
				if (ExTraceGlobals.JournalingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Journaling is re-formatting journal report with message-id {0}", this.savedMessageId);
				}
				foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
				{
					MailRecipientWrapper mailRecipientWrapper = envelopeRecipient as MailRecipientWrapper;
					if (mailRecipientWrapper.OutboundDeliveryMethod == DeliveryMethod.Mailbox)
					{
						Utils.AddRecipSortedToList(envelopeRecipient.Address.ToString(), ref list);
						list2.Add(envelopeRecipient);
						mailRecipientWrapper.MailRecipient.SmtpResponse = JournalAgent.JournalReportMigration;
					}
				}
				logContext.AddParameter("JRRec", new object[]
				{
					list
				});
				if (list2.Count == 0)
				{
					logContext.LogAsSkipped("NoLDRec", new object[0]);
					return JournalAgent.ProcessingStatus.Success;
				}
				if (list2.Count != mailItem.Recipients.Count)
				{
					source.Fork(list2);
				}
				List<LegacyRecipientRecord> legacyRecords = LegacyJournalInfo.ParseOriginalRecipientsFromExch50(mailItem);
				this.originalMailItem = mailItem;
				this.SetBreadCrumbInfo("ProcessForLegacyJournaling", new object[]
				{
					this.savedMessageId,
					"Create journal report."
				});
				this.journalReportItem = JournalReport.CreateReport(this.configuration, mailItem, list, null, false, false, null, legacyRecords);
				logContext.AddParameter("JRItem", new object[]
				{
					(this.journalReportItem.Message != null) ? this.journalReportItem.Message.MessageId : string.Empty
				});
				this.PassDataForJournalReportAgentInfo((TransportMailItem)this.journalReportItem, this.savedMessageId, false, null);
				this.TrackAgentInfoForOriginalMessage(source, this.journalReportItem.Message.MessageId, null, list, false);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (TransientException ex3)
			{
				ex = ex3;
			}
			catch (DataValidationException ex4)
			{
				ex = ex4;
			}
			catch (FormatException ex5)
			{
				ex = ex5;
			}
			catch (JournalReport.ReportGenerationException ex6)
			{
				ex = ex6;
			}
			catch (ExchangeDataException ex7)
			{
				ex = ex7;
			}
			catch (TransportPropertyException ex8)
			{
				ex = ex8;
			}
			catch (ArgumentException ex9)
			{
				ex = ex9;
			}
			if (ex != null)
			{
				this.SetBreadCrumbInfo("ProcessForLegacyJournaling", new object[]
				{
					this.savedMessageId,
					"Permanent exception: ",
					ex
				});
				ExTraceGlobals.JournalingTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception happened while converting legacy journal report. the report will deliver to journal mailbox without conversion: {0}", ex);
				mailItem.Properties.Remove("Microsoft.Exchange.LegacyJournalReport");
				logContext.LogAsFatalError(new object[]
				{
					"ProcessForLegacyJournaling",
					ex
				});
				return JournalAgent.ProcessingStatus.PermanentError;
			}
			this.agentAsyncContext = base.GetAgentAsyncContext();
			this.CommitJournalReportAsync(source);
			return JournalAgent.ProcessingStatus.IntermediateSuccess;
		}

		private JournalAgent.ProcessingStatus ProcessForJournaling(RoutedMessageEventSource source, QueuedMessageEventArgs args, JournalLogContext logContext)
		{
			this.savedMessageId = args.MailItem.Message.MessageId;
			MailItem mailItem = args.MailItem;
			Exception ex = null;
			Exception ex2 = null;
			try
			{
				this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
				{
					this.savedMessageId
				});
				if (ExTraceGlobals.JournalingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<string>(0L, "Journaling is processing message with message-id {0}", this.savedMessageId);
				}
				if (this.messageChecker.ShouldSkipJournaling())
				{
					this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
					{
						this.savedMessageId,
						"Should skip journaling."
					});
					logContext.LogAsSkipped("MCSkipJR", new object[0]);
					return JournalAgent.ProcessingStatus.Success;
				}
				List<string> list = null;
				this.journalRecipGuidList = new SortedList<Guid, Guid>();
				bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.LegacyJournaling.Enabled;
				if (enabled)
				{
					this.ProcessLegacyJournalConfig(mailItem, ref list, this.journalRecipGuidList);
				}
				if (Utils.IsNdr(mailItem) && this.IsNdrLoop(mailItem))
				{
					this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
					{
						this.savedMessageId,
						"Not processing after detecting loop exists."
					});
					logContext.AddParameter("IsNdrOrLp", new object[0]);
					return JournalAgent.ProcessingStatus.PermanentError;
				}
				this.ProcessNdrsOfJournaledMessages(mailItem, ref list);
				if (list != null)
				{
					mailItem.Properties.Add("Microsoft.Exchange.JournalTargetRecips", list);
				}
				this.SetMailItemScope(mailItem);
				ExecutionStatus executionStatus = this.configuration.Rules.Rules.Run(this.server, mailItem, source, false, null, null, null, null);
				logContext.AddParameter("RRun", new object[]
				{
					executionStatus
				});
				if (executionStatus == ExecutionStatus.TransientError)
				{
					this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
					{
						this.savedMessageId,
						"Run journaling rules returns transient error."
					});
					throw new TransientException(new LocalizedString("Rule runtime error"));
				}
				object obj;
				if (mailItem.Properties.TryGetValue("Microsoft.Exchange.JournalTargetRecips", out obj))
				{
					list = (obj as List<string>);
					this.SubstituteGroupRecipients(mailItem, ref list);
					this.RemoveDuplicateJournalRecipients(mailItem, list);
				}
				logContext.AddParameter("JRRec", new object[]
				{
					list
				});
				List<string> list2 = null;
				if ((list == null || list.Count == 0) && (list2 == null || list2.Count == 0))
				{
					this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
					{
						this.savedMessageId,
						"Not creating journal report because of missing journal recipients."
					});
					ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Not journaling this message.");
					return JournalAgent.ProcessingStatus.Success;
				}
				this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
				{
					this.savedMessageId,
					"Create journal report."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Attempting to generate journal-report");
				this.journalReportItem = JournalReport.CreateReport(this.configuration, mailItem, list, list2, false, false, null, null);
				logContext.AddParameter("JRItem", new object[]
				{
					(this.journalReportItem.Message != null) ? this.journalReportItem.Message.MessageId : string.Empty
				});
				List<string> list3 = null;
				if (mailItem.Properties.ContainsKey("Microsoft.Exchange.JournalRuleIds"))
				{
					list3 = (List<string>)mailItem.Properties["Microsoft.Exchange.JournalRuleIds"];
				}
				this.PassDataForJournalReportAgentInfo((TransportMailItem)this.journalReportItem, this.savedMessageId, false, list3);
				this.TrackAgentInfoForOriginalMessage(source, this.journalReportItem.Message.MessageId, list3, list, false);
				logContext.AddParameter("RId", new object[]
				{
					list3
				});
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (TransientException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex2 = ex5;
			}
			catch (ExchangeDataException ex6)
			{
				ex2 = ex6;
			}
			catch (RuleInvalidOperationException ex7)
			{
				ex = ex7;
			}
			catch (JournalReport.ReportGenerationException ex8)
			{
				ex = ex8;
			}
			catch (JournalingTargetDGEmptyException ex9)
			{
				ex = ex9;
			}
			catch (JournalingTargetDGNotFoundException ex10)
			{
				ex = ex10;
			}
			finally
			{
				mailItem.Properties.Remove("Microsoft.Exchange.JournalTargetRecips");
				mailItem.Properties.Remove("Microsoft.Exchange.JournalReconciliationAccounts");
			}
			if (ex != null)
			{
				this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
				{
					this.savedMessageId,
					"Got retriable exception: ",
					ex
				});
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Putting this message into retry, as there was an error during journaling: {0}.", ex.ToString());
				this.DeferMessageAndUpdateCounter(source, false);
				logContext.LogAsRetriableError(new object[]
				{
					"ProcessForJournaling",
					ex
				});
				return JournalAgent.ProcessingStatus.TransientError;
			}
			if (ex2 != null)
			{
				this.SetBreadCrumbInfo("ProcessForJournaling", new object[]
				{
					this.savedMessageId,
					"Got permanent exception: ",
					ex2
				});
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "NDR'ing this message as it is corrupt and could not be journaled. Got an exception during processing: {0}", ex2.ToString());
				JournalAgent.NdrAllRecipients(mailItem, SmtpResponse.InvalidContent);
				this.perfCountersWrapper.Increment(PerfCounters.JournalReportsThatCouldNotBeCreated, 1L);
				List<string> list4 = null;
				object obj2 = null;
				if (mailItem.Properties.TryGetValue("Microsoft.Exchange.JournalRuleIds", out obj2))
				{
					list4 = (List<string>)obj2;
				}
				this.PublishMonitoringResult("ProcessForJournaling", this.savedMessageId, this.configuration.OrganizationId, list4, ex2);
				logContext.LogAsFatalError(new object[]
				{
					"ProcessForJournaling",
					"PermanentException",
					list4,
					ex2
				});
				return JournalAgent.ProcessingStatus.PermanentError;
			}
			this.originalMailItem = mailItem;
			this.agentAsyncContext = base.GetAgentAsyncContext();
			this.CommitJournalReportAsync(source);
			return JournalAgent.ProcessingStatus.IntermediateSuccess;
		}

		private void CommitJournalReportAsync(RoutedMessageEventSource source)
		{
			this.SetBreadCrumbInfo("CommitJournalReportAsync", new object[]
			{
				this.savedMessageId
			});
			ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Journal report MailItem was generated, committing it asynchronously");
			this.journalReportItem.BeginCommitForReceive(new AsyncCallback(this.CompleteJournalReportCommit), source);
		}

		internal void CompleteJournalReportCommit(IAsyncResult result)
		{
			this.SetBreadCrumbInfo("CompleteJournalReportCommit", new object[]
			{
				this.savedMessageId
			});
			this.Resume();
			using (JournalLogContext journalLogContext = new JournalLogContext("JA", "CompleteJournalReportCommit", this.savedMessageId, this.originalMailItem))
			{
				Exception ex;
				if (!this.journalReportItem.EndCommitForReceive(result, out ex))
				{
					this.SetBreadCrumbInfo("CompleteJournalReportCommit", new object[]
					{
						this.savedMessageId,
						"Unable to commit journal report: ",
						ex
					});
					journalLogContext.LogAsFatalError(new object[]
					{
						"CompleteJournalReportCommit",
						ex
					});
					if (ex is ExchangeDataException)
					{
						ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to commit journal report, Fatal Exception: {0}. NDR'ing recipients.", ex.ToString());
						if (!this.messageChecker.IsLegacyJournalReport())
						{
							this.SetBreadCrumbInfo("CompleteJournalReportCommit", new object[]
							{
								this.savedMessageId,
								"NDR all recipients."
							});
							JournalAgent.NdrAllRecipients(this.originalMailItem, SmtpResponse.InvalidContent);
							this.perfCountersWrapper.Increment(PerfCounters.JournalReportsThatCouldNotBeCreated, 1L);
							List<string> ruleIds = null;
							object obj = null;
							if (this.originalMailItem.Properties.TryGetValue("Microsoft.Exchange.JournalRuleIds", out obj))
							{
								ruleIds = (List<string>)obj;
							}
							this.PublishMonitoringResult("CompleteJournalReportCommit", this.savedMessageId, this.configuration.OrganizationId, ruleIds, ex);
						}
					}
					else
					{
						this.SetBreadCrumbInfo("CompleteJournalReportCommit", new object[]
						{
							this.savedMessageId,
							"Deferring this message due to error while committing the report."
						});
						RoutedMessageEventSource source = (RoutedMessageEventSource)result.AsyncState;
						ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Deferring this message, as there was an error while committing the report: {0}.", ex.ToString());
						this.DeferMessageAndUpdateCounter(source, false);
					}
					this.journalReportItem = null;
				}
				else
				{
					this.SetBreadCrumbInfo("CompleteJournalReportCommit", new object[]
					{
						this.savedMessageId,
						"Journal report committed."
					});
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Journal report MailItem committed");
					long recordId = ((ITransportMailItemWrapperFacade)this.originalMailItem).TransportMailItem.RecordId;
					TransportFacades.TrackReceiveByAgent(this.journalReportItem, "Journaling", null, new long?(recordId));
					this.CheckAndMarkInternalGeneratedJournalReport(this.journalReportItem as TransportMailItem);
					TransportFacades.CategorizerComponent.EnqueueSideEffectMessage(((ITransportMailItemWrapperFacade)this.originalMailItem).TransportMailItem, this.journalReportItem, "Journal Agent");
					ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Enqueued report to Cat");
					this.journalReportItem = null;
					if (this.messageChecker.IsLegacyJournalReport())
					{
						this.originalMailItem.Recipients.Clear();
					}
					else
					{
						this.LegacyAddRecipGuidsToExch50(this.originalMailItem, this.journalRecipGuidList);
						this.perfCountersWrapper.Increment(PerfCounters.UsersJournaled, (long)this.originalMailItem.Recipients.Count);
						this.perfCountersWrapper.Increment(PerfCounters.ReportsGenerated, 1L);
						if (Utils.IsProtectedEmail(this.originalMailItem.Message))
						{
							PerfCounters.ReportsGeneratedWithRMSProtectedMessage.Increment();
						}
					}
					this.MarkProcessingDone(this.originalMailItem);
					this.UpdateProcessingTime();
				}
			}
			this.Completed();
		}

		private void Resume()
		{
			this.SetBreadCrumbInfo("Resume", new object[0]);
			if (this.agentAsyncContext != null)
			{
				this.SetBreadCrumbInfo("Resume", new object[]
				{
					this.savedMessageId,
					"Calling agentAsyncContext.Resume"
				});
				this.agentAsyncContext.Resume();
			}
		}

		private void Completed()
		{
			this.SetBreadCrumbInfo("Completed", new object[0]);
			if (this.agentAsyncContext != null)
			{
				this.SetBreadCrumbInfo("Completed", new object[]
				{
					this.savedMessageId,
					"Calling agentAsyncContext.Complete"
				});
				AgentAsyncContext agentAsyncContext = this.agentAsyncContext;
				this.agentAsyncContext = null;
				agentAsyncContext.Complete();
			}
		}

		private void CheckAndMarkInternalGeneratedJournalReport(TransportMailItem journalReport)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.InternalJournaling.Enabled;
			if (enabled && this.configuration != null && (this.configuration.LegacyArchiveLiveJournalingEnabled || this.configuration.LegacyArchiveJournalingEnabled))
			{
				journalReport.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Journaling.ProcessedOnRoutedInternalJournalReport", true);
				Header newChild = Header.Create("X-MS-InternalJournal");
				HeaderList headers = journalReport.Message.MimeDocument.RootPart.Headers;
				headers.AppendChild(newChild);
			}
		}

		private void MarkJournalNdrMessagesShouldSkipRules(MailItem mailItem)
		{
			this.SetBreadCrumbInfo("MarkJournalNdrMessagesShouldSkipRules", new object[]
			{
				mailItem.Message.MessageId
			});
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			headers.AppendChild(new TextHeader("X-MS-Exchange-Organization-JournalNdr-Skip-TransportMailboxRules", "Journal Agent"));
		}

		private void MarkProcessingDone(MailItem mailItem)
		{
			this.SetBreadCrumbInfo("MarkProcessingDone", new object[]
			{
				mailItem.Message.MessageId
			});
			this.MarkMessageAsProcessedByFirstHop(mailItem);
			mailItem.Properties["Microsoft.Exchange.Journaling.ProcessedOnRouted"] = true;
		}

		private void MarkGccProcessingDone(MailItem mailItem)
		{
			string messageId = mailItem.Message.MessageId;
			this.SetBreadCrumbInfo("MarkGccProcessingDone", new object[]
			{
				messageId
			});
			try
			{
				if (this.messageChecker.CheckFirstExchangeHopForGccProcessing())
				{
					HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
					headers.AppendChild(new TextHeader("X-MS-Exchange-Organization-Processed-By-Gcc-Journaling", "Journal Agent"));
				}
			}
			catch (ExchangeDataException ex)
			{
				ExTraceGlobals.JournalingTracer.TraceError<string>((long)this.GetHashCode(), "Unable to mark message as processed by GCC journaling. Exception details: {0}.", ex.ToString());
				this.SetBreadCrumbInfo("MarkGccProcessingDone", new object[]
				{
					messageId,
					ex
				});
			}
		}

		private void UpdateProcessingTime()
		{
			this.timer.Stop();
			long elapsedMilliseconds = this.timer.ElapsedMilliseconds;
			ExTraceGlobals.JournalingTracer.TraceDebug<long>((long)this.GetHashCode(), "{0} ms to journal message", elapsedMilliseconds);
			PerfCounters.ProcessingTime.IncrementBy(elapsedMilliseconds);
		}

		private void SetMailItemScope(MailItem mailItem)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
			ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
			OrganizationId orgId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = this.IsInternal(mailItem.FromAddress, orgId);
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			foreach (EnvelopeRecipient envelopeRecipient in recipients)
			{
				if (!this.IsInternal(envelopeRecipient.Address, orgId))
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
			}
			object obj = new object();
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.External", out obj))
			{
				mailItem.Properties.Remove("Microsoft.Exchange.Journaling.External");
			}
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.Internal", out obj))
			{
				mailItem.Properties.Remove("Microsoft.Exchange.Journaling.Internal");
			}
			mailItem.Properties.Add("Microsoft.Exchange.Journaling.External", (!flag3 || flag) ? 1 : 0);
			mailItem.Properties.Add("Microsoft.Exchange.Journaling.Internal", (flag3 && flag2) ? 1 : 0);
			ExTraceGlobals.JournalingTracer.TraceDebug<bool, bool, bool>((long)this.GetHashCode(), "Scope: InternalSender: {0}, SomeInternalRecipients: {1}, SomeExternalRecipients: {2}", flag3, flag2, flag);
		}

		private void ProcessLegacyJournalConfig(MailItem mailItem, ref List<string> journalRecipsSorted, SortedList<Guid, Guid> journalRecipientList)
		{
			this.SetBreadCrumbInfo("ProcessLegacyJournalConfig", new object[]
			{
				mailItem.Message.MessageId
			});
			string recipientEmail = null;
			Guid guid;
			if (LegacyConfig.Instance.ShouldJournal(mailItem.FromAddress, mailItem.Properties, out recipientEmail, out guid))
			{
				Utils.AddRecipSortedToList(recipientEmail, ref journalRecipsSorted);
				if (!journalRecipientList.ContainsKey(guid))
				{
					journalRecipientList.Add(guid, guid);
				}
			}
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				if (LegacyConfig.Instance.ShouldJournal(envelopeRecipient.Address, envelopeRecipient.Properties, out recipientEmail, out guid))
				{
					Utils.AddRecipSortedToList(recipientEmail, ref journalRecipsSorted);
					if (!journalRecipientList.ContainsKey(guid))
					{
						journalRecipientList.Add(guid, guid);
					}
				}
			}
		}

		private void ProcessNdrsOfJournaledMessages(MailItem mailItem, ref List<string> journalRecipsSorted)
		{
			this.SetBreadCrumbInfo("ProcessNdrsOfJournaledMessages", new object[]
			{
				mailItem.Message.MessageId
			});
			if (!Utils.IsNdr(mailItem))
			{
				return;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "This message is an NDR of a message that was journaled previously");
			string[] array = Utils.ParseJournaledToHeader(mailItem);
			if (array != null)
			{
				foreach (string recipientEmail in array)
				{
					Utils.AddRecipSortedToList(recipientEmail, ref journalRecipsSorted);
				}
			}
		}

		private bool IsNdrLoop(MailItem mailItem)
		{
			if (mailItem == null || mailItem.Message == null || mailItem.Message.MimeDocument == null)
			{
				return false;
			}
			EmailMessage emailMessage = EmailMessage.Create(mailItem.Message.MimeDocument.Clone());
			int num = 0;
			int num2 = 0;
			while (num2 < 3 && emailMessage != null)
			{
				num = ((Utils.IsJournalReport(emailMessage.RootPart) || Utils.IsNdr(emailMessage.MimeDocument.RootPart)) ? (num + 1) : num);
				AttachmentCollection attachments = emailMessage.Attachments;
				Attachment attachment = null;
				if (attachments != null && attachments.Count > 0)
				{
					for (int i = 0; i < attachments.Count; i++)
					{
						if (Utils.IsMessageAttachment(attachments[i]))
						{
							attachment = attachments[i];
							break;
						}
					}
				}
				if (attachment == null || attachment.MimePart == null)
				{
					break;
				}
				Stream stream = null;
				bool flag = attachment.TryGetContentReadStream(out stream);
				emailMessage.Dispose();
				emailMessage = null;
				if (flag)
				{
					try
					{
						emailMessage = EmailMessage.Create(stream);
					}
					finally
					{
						stream.Dispose();
					}
				}
				num2++;
			}
			bool result = num > 1;
			if (emailMessage != null)
			{
				emailMessage.Dispose();
				emailMessage = null;
			}
			return result;
		}

		private void LegacyAddRecipGuidsToExch50(MailItem mailItem, SortedList<Guid, Guid> guidList)
		{
			this.SetBreadCrumbInfo("LegacyAddRecipGuidsToExch50", new object[]
			{
				mailItem.Message.MessageId
			});
			object obj = null;
			Guid[] array = null;
			StringBuilder stringBuilder = null;
			bool flag = false;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.JournalRecipientList", out obj) && obj is Guid[])
			{
				array = (Guid[])obj;
				for (int i = 0; i < array.Length; i++)
				{
					if (!guidList.ContainsKey(array[i]))
					{
						guidList.Add(array[i], array[i]);
						flag = true;
					}
				}
			}
			else if (guidList.Count > 0)
			{
				flag = true;
			}
			if (guidList.Count == 0)
			{
				return;
			}
			stringBuilder = new StringBuilder(guidList.Count * 37);
			array = new Guid[guidList.Count];
			int num = 0;
			foreach (Guid guid in guidList.Keys)
			{
				array[num] = guid;
				if (num > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(guid.ToString());
				num++;
			}
			if (flag)
			{
				mailItem.Properties.Remove("Microsoft.Exchange.JournalRecipientList");
				mailItem.Properties.Add("Microsoft.Exchange.JournalRecipientList", array);
			}
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			headers.RemoveAll("X-MS-Exchange-Organization-JournalRecipientList");
			headers.AppendChild(new TextHeader("X-MS-Exchange-Organization-JournalRecipientList", stringBuilder.ToString()));
		}

		private void RemoveDuplicateJournalRecipients(MailItem mailItem, List<string> journalRecipientList)
		{
			string messageId = mailItem.Message.MessageId;
			this.SetBreadCrumbInfo("RemoveDuplicateJournalRecipients", new object[]
			{
				messageId
			});
			if (journalRecipientList.Count == 0)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Empty journal recip list, nothing to de-dup");
				return;
			}
			object obj;
			if (!mailItem.Properties.TryGetValue("Microsoft.Exchange.JournalRecipientList", out obj))
			{
				this.SetBreadCrumbInfo("RemoveDuplicateJournalRecipients", new object[]
				{
					messageId,
					"Message not journaled on a previous hop, nothing to dedup."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Message not journaled on a previous hop, nothing to de-duplicate");
				return;
			}
			if (this.messageChecker.CheckRecipientsChanged())
			{
				this.SetBreadCrumbInfo("RemoveDuplicateJournalRecipients", new object[]
				{
					messageId,
					"Recipients changed on this hop."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "Recipients have changed on this hop, we must re-journal this message");
				return;
			}
			Guid[] array = (Guid[])obj;
			foreach (Guid recipientGuid in array)
			{
				ADRecipient adrecipient = LegacyConfig.Instance.LookupJournalRecipientByGuidCached(recipientGuid);
				if (adrecipient == null)
				{
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "A journal-recipient specified in XEXCH50 was not found in the AD");
				}
				else
				{
					foreach (ProxyAddress proxyAddress in adrecipient.EmailAddresses)
					{
						if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && journalRecipientList.BinarySearch(proxyAddress.AddressString, StringComparer.OrdinalIgnoreCase) >= 0)
						{
							ExTraceGlobals.JournalingTracer.TraceDebug<string>((long)this.GetHashCode(), "A journal report was already sent to {0} by E2K3, we won't re-journal", proxyAddress.ToString());
							journalRecipientList.Remove(proxyAddress.AddressString);
							break;
						}
					}
				}
			}
		}

		private bool IsInternal(RoutingAddress address, OrganizationId orgId)
		{
			AddressBookImpl addressBookImpl = this.server.AddressBook as AddressBookImpl;
			if (addressBookImpl != null)
			{
				return addressBookImpl.IsInternalTo(address, orgId, false);
			}
			return this.server.AddressBook.IsInternal(address);
		}

		private void InferRecipientTypesForMapiResubmit(MailItem mailItem)
		{
			EnvelopeRecipientCollection recipients = mailItem.Recipients;
			string messageId = mailItem.Message.MessageId;
			this.SetBreadCrumbInfo("InferRecipientTypesForMapiResubmit", new object[]
			{
				messageId,
				"Number of recipients: ",
				recipients.Count
			});
			ExTraceGlobals.JournalingTracer.TraceDebug<int>((long)this.GetHashCode(), "Processing {0} MAPI_P1 recipients on resubmitted message", recipients.Count);
			List<string> recipientProxyList = null;
			object obj = null;
			List<string> recipientProxyList2 = null;
			object obj2 = null;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Transport.ResentMapiP2ToRecipients", out obj))
			{
				recipientProxyList = (obj as List<string>);
			}
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Transport.ResentMapiP2CcRecipients", out obj2))
			{
				recipientProxyList2 = (obj2 as List<string>);
			}
			List<EnvelopeRecipient> list = null;
			foreach (EnvelopeRecipient envelopeRecipient in recipients)
			{
				ResendRecipientInfo resendRecipientInfo = null;
				Exception ex = null;
				try
				{
					resendRecipientInfo = ResendRecipientInfo.Lookup(mailItem, envelopeRecipient.Address.ToString());
				}
				catch (DataValidationException ex2)
				{
					ex = ex2;
				}
				catch (ArgumentException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					string text = string.Format("There was an error processing this recipient. We cannot determine it's PR_EMAIL_ADDRESS. We will NDR it: {0}, Error: {1}", envelopeRecipient.Address, ex.ToString());
					this.SetBreadCrumbInfo("InferRecipientTypesForMapiResubmit", new object[]
					{
						messageId,
						text
					});
					ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), text);
					if (list == null)
					{
						list = new List<EnvelopeRecipient>();
					}
					list.Add(envelopeRecipient);
				}
				else
				{
					ExTraceGlobals.JournalingTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Trying to infer PR_RECIPIENT_TYPE of {0}", envelopeRecipient.Address);
					int num;
					if (resendRecipientInfo.BinarySearchAndRemove(recipientProxyList))
					{
						ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "PR_RECIPIENT_TYPE == To");
						num = 1;
					}
					else if (resendRecipientInfo.BinarySearchAndRemove(recipientProxyList2))
					{
						ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "PR_RECIPIENT_TYPE == Cc");
						num = 2;
					}
					else
					{
						ExTraceGlobals.JournalingTracer.TraceDebug((long)this.GetHashCode(), "PR_RECIPIENT_TYPE == Bcc");
						num = 3;
					}
					this.SetBreadCrumbInfo("InferRecipientTypesForMapiResubmit", new object[]
					{
						messageId,
						string.Format("Trying to infer PR_RECIPIENT_TYPE of '{0}', recipient type = {1}", envelopeRecipient.Address, num)
					});
					envelopeRecipient.Properties["Microsoft.Exchange.Transport.RecipientP2Type"] = num;
				}
			}
			if (list != null)
			{
				foreach (EnvelopeRecipient envelopeRecipient2 in list)
				{
					ExTraceGlobals.JournalingTracer.TraceError<RoutingAddress>((long)this.GetHashCode(), "NDR'ing bad recipient: {0}", envelopeRecipient2.Address);
					mailItem.Recipients.Remove(envelopeRecipient2, DsnType.Failure, SmtpResponse.InvalidRecipientAddress);
				}
			}
		}

		private void GetSortedRecipientList(MailItem mailItem, out List<RoutingAddress> routingAddresses, out List<EnvelopeRecipient> envelopeRecipients)
		{
			routingAddresses = new List<RoutingAddress>(mailItem.Recipients.Count);
			envelopeRecipients = new List<EnvelopeRecipient>();
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				int num = routingAddresses.BinarySearch(envelopeRecipient.Address, MessageChecker.AddressComparer);
				if (num < 0)
				{
					num = ~num;
				}
				routingAddresses.Insert(num, envelopeRecipient.Address);
				envelopeRecipients.Insert(num, envelopeRecipient);
			}
		}

		private void InferRecipientTypesForMuaSubmit(MailItem mailItem, List<RoutingAddress> routingAddresses, List<EnvelopeRecipient> envelopeRecipients)
		{
			this.SetBreadCrumbInfo("InferRecipientTypesForMuaSubmit", new object[]
			{
				mailItem.Message.MessageId
			});
			JournalAgent.TagRecipientTypes(RecipientP2Type.To, routingAddresses, envelopeRecipients, mailItem.Message.To);
			JournalAgent.TagRecipientTypes(RecipientP2Type.Cc, routingAddresses, envelopeRecipients, mailItem.Message.Cc);
			foreach (EnvelopeRecipient envelopeRecipient in envelopeRecipients)
			{
				object obj;
				if (!envelopeRecipient.Properties.TryGetValue("Microsoft.Exchange.Transport.RecipientP2Type", out obj))
				{
					envelopeRecipient.Properties["Microsoft.Exchange.Transport.RecipientP2Type"] = 3;
				}
			}
		}

		private void SetBreadCrumbInfo(string functionName, params object[] breadCrumbInfo)
		{
			this.breadCrumbs.Drop(new BreadCrumb
			{
				FunctionName = functionName,
				Info = breadCrumbInfo
			});
		}

		private void SubstituteGroupRecipients(MailItem mailItem, ref List<string> journalRecipsSorted)
		{
			if (!this.configuration.JournalReportDLMemberSubstitutionEnabled)
			{
				return;
			}
			List<string> list = new List<string>(journalRecipsSorted);
			foreach (string text in list)
			{
				IADRecipientCache iadrecipientCache = (IADRecipientCache)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.ADRecipientCacheAsObject;
				ADRawEntry adrawEntry;
				if (!Utils.TryGetADRawEntryByEmailAddress(iadrecipientCache, text, out adrawEntry))
				{
					if (Utils.IsAuthoritativeDomain(text, (OrganizationId)((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.OrganizationIdAsObject))
					{
						JournalingGlobals.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingTargetDGNotFoundError, null, new object[]
						{
							mailItem.Message.MessageId,
							text,
							JournalingGlobals.RetryIntervalOnError
						});
						throw new JournalingTargetDGNotFoundException(text);
					}
				}
				else
				{
					Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = (Microsoft.Exchange.Data.Directory.Recipient.RecipientType)adrawEntry[ADRecipientSchema.RecipientType];
					if (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup || recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup)
					{
						JournalingDistributionGroupCacheItem groupCacheItem = JournalAgentFactory.JournalingDistributionGroupCacheInstance.GetGroupCacheItem(iadrecipientCache, text);
						if (groupCacheItem != null)
						{
							string text2;
							if (!groupCacheItem.TryGetNextGroupMember(out text2))
							{
								JournalingGlobals.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingTargetDGEmptyError, null, new object[]
								{
									mailItem.Message.MessageId,
									text,
									JournalingGlobals.RetryIntervalOnError
								});
								throw new JournalingTargetDGEmptyException(text);
							}
							ExTraceGlobals.JournalingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "JournalReportDLMemberSubstitution is enabled. DL {0} is being replaced with one of its members: {1}", text, text2);
							journalRecipsSorted.Remove(text);
							Utils.AddRecipSortedToList(text2, ref journalRecipsSorted);
						}
					}
				}
			}
		}

		private void PassDataForJournalReportAgentInfo(TransportMailItem journalReportItem, string originalMessageId, bool isGcc, List<string> ruleIds)
		{
			journalReportItem.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Journaling.OriginalMessageId", originalMessageId);
			journalReportItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Journaling.IsGccFlag", isGcc);
			if (ruleIds != null && ruleIds.Count > 0)
			{
				journalReportItem.ExtendedProperties.SetValue<List<string>>("Microsoft.Exchange.Journaling.RuleIds", ruleIds);
			}
		}

		private void TrackAgentInfoForOriginalMessage(RoutedMessageEventSource source, string messageId, List<string> ruleIds, List<string> journalRecipients, bool isGcc)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (isGcc)
			{
				list.Add(new KeyValuePair<string, string>("type", "LI"));
			}
			else
			{
				list.Add(new KeyValuePair<string, string>("type", "tenant"));
			}
			if (ruleIds != null && ruleIds.Count > 0)
			{
				foreach (string value in ruleIds)
				{
					list.Add(new KeyValuePair<string, string>("ruleid", value));
				}
			}
			if (!string.IsNullOrEmpty(messageId))
			{
				list.Add(new KeyValuePair<string, string>("mid", messageId));
			}
			if (journalRecipients != null && journalRecipients.Count > 0)
			{
				foreach (string value2 in journalRecipients)
				{
					list.Add(new KeyValuePair<string, string>("dest", value2));
				}
			}
			source.TrackAgentInfo("JA", "ORIG", list);
		}

		private void TrackAgentInfoForJournalReportMessage(SubmittedMessageEventSource source, MailItem mailItem)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			object obj = null;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.OriginalMessageId", out obj))
			{
				list.Add(new KeyValuePair<string, string>("orig", (string)obj));
				mailItem.Properties.Remove("Microsoft.Exchange.Journaling.OriginalMessageId");
			}
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.IsGccFlag", out obj))
			{
				if ((bool)obj)
				{
					list.Add(new KeyValuePair<string, string>("type", "LI"));
				}
				else
				{
					list.Add(new KeyValuePair<string, string>("type", "tenant"));
				}
				mailItem.Properties.Remove("Microsoft.Exchange.Journaling.IsGccFlag");
			}
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Journaling.RuleIds", out obj))
			{
				List<string> list2 = obj as List<string>;
				if (list2 != null && list2.Count > 0)
				{
					foreach (string value in list2)
					{
						list.Add(new KeyValuePair<string, string>("ruleid", value));
					}
				}
				mailItem.Properties.Remove("Microsoft.Exchange.Journaling.RuleIds");
			}
			if (list.Count > 0)
			{
				source.TrackAgentInfo("JA", "JR", list);
			}
		}

		private void PublishMonitoringResult(string functionName, string messageId, OrganizationId orgId, IEnumerable<string> ruleIds, Exception ex)
		{
			string text = null;
			if (orgId != null && orgId.OrganizationalUnit != null)
			{
				text = orgId.OrganizationalUnit.Name;
			}
			string text2 = null;
			if (ruleIds != null)
			{
				text2 = string.Join(",", ruleIds);
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.Compliance.Name, "JournalAgent", string.Format("Journal agent failed to generate journal report for message: {0} in organization: {1} because of exception: {2} in function: {3}", new object[]
			{
				messageId,
				text,
				ex.GetType().FullName,
				functionName
			}), ResultSeverityLevel.Error);
			eventNotificationItem.StateAttribute1 = functionName;
			eventNotificationItem.StateAttribute2 = messageId;
			eventNotificationItem.StateAttribute3 = text;
			eventNotificationItem.StateAttribute4 = ex.StackTrace;
			eventNotificationItem.StateAttribute5 = text2;
			try
			{
				eventNotificationItem.Publish(false);
				this.SetBreadCrumbInfo("PublishMonitoringResult", new object[]
				{
					messageId,
					"Notification published."
				});
				ExTraceGlobals.JournalingTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Notification published for message id: {0} in organization: {1} with journal rule ids: {2}", messageId, text, text2);
			}
			catch (Exception ex2)
			{
				this.SetBreadCrumbInfo("PublishMonitoringResult", new object[]
				{
					messageId,
					"Failed to publish notification with exception:",
					ex2
				});
				ExTraceGlobals.JournalingTracer.TraceError((long)this.GetHashCode(), "Failed to publish notification for message id: {0} in organization: {1} with journal rule ids: {2} Exception: {3}", new object[]
				{
					messageId,
					text,
					text2,
					ex2
				});
			}
		}

		private void DeferMessageAndUpdateCounter(RoutedMessageEventSource source, bool isGCC)
		{
			source.Defer(JournalingGlobals.RetryIntervalOnError);
			if (isGCC)
			{
				this.perfCountersWrapper.Increment(PerfCounters.MessagesDeferredWithinJournalAgentLawfulIntercept, 1L);
				return;
			}
			this.perfCountersWrapper.Increment(PerfCounters.MessagesDeferredWithinJournalAgent, 1L);
		}

		private const string AgentName = "Journal Agent";

		private static readonly SmtpResponse JournalReportFromUnauthorizedSender = new SmtpResponse("550", "5.7.1", new string[]
		{
			"Sender is not authorized to submit journal reports."
		});

		private static readonly SmtpResponse JournalReportMigration = new SmtpResponse("500", null, new string[]
		{
			"Migrating journal report"
		});

		private static readonly bool DisableJournalReportFilterToDCMailbox = !JournalAgent.IsJournalFilterEnabled();

		private readonly JournalPerfCountersWrapper perfCountersWrapper;

		private Stopwatch timer = new Stopwatch();

		private SmtpServer server;

		private Configuration configuration;

		private MessageChecker messageChecker;

		private ITransportMailItemFacade journalReportItem;

		private MailItem originalMailItem;

		private SortedList<Guid, Guid> journalRecipGuidList;

		private AgentAsyncContext agentAsyncContext;

		private Breadcrumbs<BreadCrumb> breadCrumbs = new Breadcrumbs<BreadCrumb>(256);

		private string savedMessageId = "no-message-id";

		private enum ProcessingStatus
		{
			PermanentError,
			TransientError,
			Success,
			IntermediateSuccess
		}
	}
}
