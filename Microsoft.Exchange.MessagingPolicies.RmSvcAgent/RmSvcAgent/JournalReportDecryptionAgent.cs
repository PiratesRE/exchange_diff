using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies.Journaling;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal sealed class JournalReportDecryptionAgent : RoutingAgent
	{
		public JournalReportDecryptionAgent(JournalReportDecryptionAgentFactory factory, SmtpServer server)
		{
			this.agentFactory = factory;
			base.OnCategorizedMessage += this.OnCategorizedEventHandler;
		}

		private static bool IsProcessedByJRDAgent(EmailMessage message)
		{
			bool result = false;
			Header xheader = Utils.GetXHeader(message, "X-MS-Exchange-Organization-JournalReportDecryption-Processed");
			if (xheader != null)
			{
				bool.TryParse(xheader.Value, out result);
			}
			return result;
		}

		private void OnCategorizedEventHandler(CategorizedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.breadcrumbs.Drop("JRD Agent Started");
			this.tenantId = base.MailItem.TenantId;
			this.source = source;
			this.embeddedMsg = this.GetEmbeddedMessageFromJournalReport();
			if (this.embeddedMsg == null)
			{
				return;
			}
			this.isE4eEncrypted = E4eHelper.IsHeaderSetToTrue(this.embeddedMsg, "X-MS-Exchange-Organization-E4eMessageEncrypted");
			OrganizationId orgId;
			if (!this.ShouldProcess(out orgId))
			{
				return;
			}
			Exception ex = null;
			try
			{
				if (!RmsClientManager.IRMConfig.IsJournalReportDecryptionEnabledForTenant(orgId))
				{
					this.TracePass("JournalReportDecryptionEnabled is set to FALSE for tenant {0} - skipping message.", new object[]
					{
						orgId
					});
					return;
				}
			}
			catch (ExchangeConfigurationException ex2)
			{
				ex = ex2;
			}
			catch (RightsManagementException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				this.TraceFail("Hit a transient exception while reading configuration for org {0}. Error : {1}", new object[]
				{
					orgId,
					ex
				});
				if (this.IncrementDeferralCountAndCheckCap())
				{
					string[] additionalInfo = new string[]
					{
						string.Format(CultureInfo.InvariantCulture, "A transient error occurred when reading configuration for {0} from AD.", new object[]
						{
							Utils.GetTenantString(this.tenantId)
						})
					};
					this.source.Defer(RmsClientManager.AppSettings.JrdTransientErrorDeferInterval, Utils.GetResponseForExceptionDeferral(ex, additionalInfo));
					JournalReportDecryptionAgentPerfCounters.TotalDeferrals.Increment();
					return;
				}
				JournalReportDecryptionAgentFactory.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_FailedToLoadIRMConfiguration, null, new object[]
				{
					RmsComponent.JournalReportDecryptionAgent,
					orgId.ToString(),
					ex
				});
				JournalReportDecryptionAgentPerfCounters.TotalJRFailed.Increment();
				DecryptionBaseComponent.UpdatePercentileCounters(false);
			}
			else
			{
				this.isActive = this.agentFactory.InstanceController.TryMakeActive(this.tenantId);
				if (!this.isActive)
				{
					this.TracePass("Unable to make active agent - deferring message.", new object[0]);
					this.source.Defer(RmsClientManager.AppSettings.ActiveAgentCapDeferInterval, Utils.GetResponseForDeferral(new string[]
					{
						"Already processing maximum number of messages."
					}));
					return;
				}
				string recipientAddressForDecryption = null;
				string key = this.isE4eEncrypted ? "Microsoft.Exchange.Encryption.DecryptionTokenRecipient" : "Microsoft.Exchange.RightsManagement.DecryptionTokenRecipient";
				object obj;
				if (base.MailItem.Properties.TryGetValue(key, out obj))
				{
					recipientAddressForDecryption = (string)obj;
					this.TracePass("The recipient address is set to {0}", new object[]
					{
						recipientAddressForDecryption
					});
				}
				else
				{
					this.TraceFail("The recipient address for the decryption token is not set", new object[0]);
				}
				bool flag = false;
				try
				{
					if (!this.isE4eEncrypted)
					{
						string publishingLicense;
						string useLicense;
						Utils.GetTransportDecryptionPLAndUL(base.MailItem, out publishingLicense, out useLicense);
						DrmEmailMessageContainer drmMsgContainer = RmsDecryptor.DrmEmailMessageContainerFromMessage(this.embeddedMsg);
						this.rmsDecryptor = new RmsDecryptor(Utils.CreateRmsContext(orgId, base.MailItem, base.MailItem.Message.MessageId, publishingLicense), this.embeddedMsg, drmMsgContainer, recipientAddressForDecryption, useLicense, Utils.GetOutboundConversionOptions(base.MailItem), this.breadcrumbs, true, false, true, this.ShouldSendTnef(base.MailItem.Message));
					}
					else
					{
						JournalReportDecryptionAgent.<>c__DisplayClass3 CS$<>8__locals2 = new JournalReportDecryptionAgent.<>c__DisplayClass3();
						CS$<>8__locals2.htmlAttachment = E4eDecryptionHelper.GetHtmlAttachment(this.embeddedMsg);
						if (CS$<>8__locals2.htmlAttachment == null)
						{
							ex = new E4eException("Message.html attachment not found.");
							return;
						}
						EnvelopeRecipientCollection recipients = base.MailItem.Recipients;
						OutboundConversionOptions outboundOptions = Utils.GetOutboundConversionOptions(base.MailItem);
						Exception ex4;
						bool flag2;
						E4eHelper.RunUnderExceptionHandler(base.MailItem.Message.MessageId, delegate
						{
							string text = string.Empty;
							using (Stream contentReadStream = CS$<>8__locals2.htmlAttachment.GetContentReadStream())
							{
								byte[] bytes = E4eHelper.ReadStreamToEnd(contentReadStream);
								text = Encoding.UTF8.GetString(bytes);
							}
							string version = E4eDecryptionHelper.GetVersion(ref text);
							E4eDecryptionHelper e4eDecryptionHelper = E4eHelper.GetE4eDecryptionHelper(version);
							string text2;
							OrganizationId originalSenderOrgId;
							string text3;
							string text4;
							EmailMessage rmsMessage;
							if (e4eDecryptionHelper.VerifyEncryptedAttachment(ref text, orgId, recipients, outboundOptions, this.MailItem.Message.MessageId, recipientAddressForDecryption, out text2, out originalSenderOrgId, out text3, out text4, out rmsMessage))
							{
								this.rmsDecryptor = e4eDecryptionHelper.CreateRmsDecryptor(originalSenderOrgId, this.MailItem.Message.MessageId, recipientAddressForDecryption, this.breadcrumbs, rmsMessage, outboundOptions, null, E4eLog.Instance, out CS$<>8__locals2.publishingLicense, false);
							}
						}, null, out ex4, out flag2);
						if (ex4 != null)
						{
							ex = ex4;
							flag = flag2;
							return;
						}
						if (this.rmsDecryptor == null)
						{
							ex = new E4eException("RMSDecryptor could not be created.");
							return;
						}
					}
				}
				catch (InvalidRpmsgFormatException ex5)
				{
					ex = ex5;
					return;
				}
				catch (RightsManagementException ex6)
				{
					ex = ex6;
					return;
				}
				catch (ExchangeConfigurationException ex7)
				{
					ex = ex7;
					flag = true;
					return;
				}
				finally
				{
					if (ex != null)
					{
						if (!flag || !this.IncrementDeferralCountAndCheckCap())
						{
							JournalReportDecryptionAgentFactory.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_FailedToLoadDrmMessage, null, new object[]
							{
								ex,
								this.embeddedMsg.MessageId
							});
							JournalReportDecryptionAgentPerfCounters.TotalJRFailed.Increment();
							DecryptionBaseComponent.UpdatePercentileCounters(false);
						}
						else
						{
							string[] additionalInfo2 = new string[]
							{
								string.Format(CultureInfo.InvariantCulture, "A transient error occurred when reading configuration for {0} from AD.", new object[]
								{
									Utils.GetTenantString(this.tenantId)
								})
							};
							this.source.Defer(RmsClientManager.AppSettings.JrdTransientErrorDeferInterval, Utils.GetResponseForExceptionDeferral(ex, additionalInfo2));
							JournalReportDecryptionAgentPerfCounters.TotalDeferrals.Increment();
						}
						this.CompleteProcess(null);
					}
				}
				this.rmsDecryptor.BeginDecrypt(new AsyncCallback(this.OnDecryptComplete), new AgentAsyncState(base.GetAgentAsyncContext()));
				return;
			}
		}

		private void OnDecryptComplete(IAsyncResult result)
		{
			this.breadcrumbs.Drop("JRD Agent OnDecryptComplete");
			ArgumentValidator.ThrowIfNull("result", result);
			ArgumentValidator.ThrowIfNull("result.AsyncState", result.AsyncState);
			((AgentAsyncState)result.AsyncState).Resume();
			try
			{
				AsyncDecryptionOperationResult<DecryptionResultData> asyncDecryptionOperationResult = (AsyncDecryptionOperationResult<DecryptionResultData>)this.rmsDecryptor.EndDecrypt(result);
				if (asyncDecryptionOperationResult.IsSucceeded)
				{
					this.TracePass("RpMsgDecryptor completed successfully.", new object[0]);
					if (this.isE4eEncrypted)
					{
						Utils.CopyHeadersDuringDecryption(this.embeddedMsg, asyncDecryptionOperationResult.Data.DecryptedMessage);
					}
					Attachment attachment = base.MailItem.Message.Attachments.Add(null, "message/rfc822");
					attachment.EmbeddedMessage = asyncDecryptionOperationResult.Data.DecryptedMessage;
					this.TracePass("Stamping message with X-Header.", new object[0]);
					Utils.StampXHeader(base.MailItem.Message, "X-MS-Exchange-Organization-JournalReportDecryption-Processed", "true");
					JournalReportDecryptionAgentPerfCounters.TotalJRDecrypted.Increment();
					DecryptionBaseComponent.UpdatePercentileCounters(true);
				}
				else if (asyncDecryptionOperationResult.IsKnownException)
				{
					this.TraceFail("Caught exception when trying to decrypt journal report {0} (message-id: {1}); is transient? {2}; exception: {3}", new object[]
					{
						base.MailItem.Message.MessageId,
						this.embeddedMsg.MessageId,
						asyncDecryptionOperationResult.IsTransientException,
						asyncDecryptionOperationResult.Exception
					});
					if (asyncDecryptionOperationResult.Exception.InnerException is RightsManagementException)
					{
						RmsEventLogHandler.LogException(JournalReportDecryptionAgentFactory.Logger, base.MailItem, RmsComponent.JournalReportDecryptionAgent, asyncDecryptionOperationResult.Exception.InnerException, !asyncDecryptionOperationResult.IsTransientException);
					}
					else
					{
						RmsEventLogHandler.LogException(JournalReportDecryptionAgentFactory.Logger, base.MailItem, RmsComponent.JournalReportDecryptionAgent, asyncDecryptionOperationResult.Exception, !asyncDecryptionOperationResult.IsTransientException);
					}
					if (!asyncDecryptionOperationResult.IsTransientException || !this.IncrementDeferralCountAndCheckCap())
					{
						JournalReportDecryptionAgentPerfCounters.TotalJRFailed.Increment();
						DecryptionBaseComponent.UpdatePercentileCounters(false);
						Utils.StampXHeader(base.MailItem.Message, "X-MS-Exchange-Organization-JournalReportDecryption-Processed", "true");
					}
					else
					{
						string[] additionalInfo = new string[]
						{
							string.Format(CultureInfo.InvariantCulture, "A transient error occurred during journal decryption when communicating with RMS server {0}.", new object[]
							{
								(asyncDecryptionOperationResult.Data.LicenseUri != null) ? asyncDecryptionOperationResult.Data.LicenseUri.OriginalString : null
							})
						};
						this.source.Defer(RmsClientManager.AppSettings.JrdTransientErrorDeferInterval, Utils.GetResponseForExceptionDeferral(asyncDecryptionOperationResult.Exception, additionalInfo));
						JournalReportDecryptionAgentPerfCounters.TotalDeferrals.Increment();
					}
				}
			}
			finally
			{
				this.CompleteProcess((AgentAsyncState)result.AsyncState);
			}
		}

		private bool IncrementDeferralCountAndCheckCap()
		{
			int num = Utils.IncrementDeferralCount(base.MailItem, "Microsoft.Exchange.JournalReportDecryptionAgent.DeferralCount");
			if (num == -1)
			{
				this.TraceFail("Deferral count of journal report {0} (message-id: {1}) is broken.", new object[]
				{
					base.MailItem.Message.MessageId,
					this.embeddedMsg.MessageId
				});
				return false;
			}
			if (num > 1)
			{
				this.TracePass("Journal report {0} (message-id: {1}) has been deferred {2} time(s).", new object[]
				{
					base.MailItem.Message.MessageId,
					this.embeddedMsg.MessageId,
					num - 1
				});
			}
			if (num > 6)
			{
				this.TraceFail("Deferral count of journal report id {0} (message-id: {1}) has reached threshold.", new object[]
				{
					base.MailItem.Message.MessageId,
					this.embeddedMsg.MessageId
				});
				return false;
			}
			this.TracePass("Message: {0}. Will be deferred.", new object[]
			{
				base.MailItem.Message.MessageId
			});
			return true;
		}

		private void CompleteProcess(AgentAsyncState agentAsyncState)
		{
			this.breadcrumbs.Drop("JRD Agent CleanedUp");
			if (this.isActive)
			{
				this.agentFactory.InstanceController.MakeInactive(this.tenantId);
				this.isActive = false;
			}
			if (agentAsyncState != null)
			{
				agentAsyncState.Complete();
			}
		}

		private bool ShouldProcess(out OrganizationId orgId)
		{
			orgId = null;
			if (JournalReportDecryptionAgent.IsProcessedByJRDAgent(base.MailItem.Message))
			{
				this.TracePass("Message is already processed - skipping message.", new object[0]);
				return false;
			}
			if (!Utils.IsSupportedMapiMessageClass(this.embeddedMsg))
			{
				ExTraceGlobals.RmSvcAgentTracer.TraceDebug<string>((long)this.GetHashCode(), "Skip Journal Report Decryption because message {0} is not IPM.Note.", this.embeddedMsg.MessageId);
				return false;
			}
			if (!Utils.IsProtectedEmail(this.embeddedMsg) && !this.isE4eEncrypted)
			{
				this.TracePass("Journal Report doesn't have RMS or E4E protected attachment - skipping message.", new object[0]);
				return false;
			}
			orgId = Utils.OrgIdFromMailItem(base.MailItem);
			JournalingRules config = JournalingRules.GetConfig(orgId);
			if (config != null)
			{
				EnvelopeRecipientCollection recipients = base.MailItem.Recipients;
				foreach (EnvelopeRecipient envelopeRecipient in recipients)
				{
					string text = envelopeRecipient.Address.ToString();
					if (!config.IsConfiguredJournalTargetAddress(text))
					{
						this.TracePass("Journal Report is targeted to malicious (not configured) recipient - skipping message.", new object[0]);
						JournalReportDecryptionAgentFactory.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_SkippedDecryptionForMaliciousTargetAddress, "Journal Report Decryption Agent", new object[]
						{
							text
						});
						return false;
					}
				}
			}
			this.TracePass("Journal Report will be decrypted by JRD agent.", new object[0]);
			return true;
		}

		private EmailMessage GetEmbeddedMessageFromJournalReport()
		{
			MailItem mailItem = base.MailItem;
			if (!((ITransportMailItemWrapperFacade)mailItem).TransportMailItem.IsJournalReport())
			{
				this.TracePass("Message is not a Journal Report - skipping message.", new object[0]);
				return null;
			}
			AttachmentCollection attachments = mailItem.Message.Attachments;
			if (attachments.Count != 1 || attachments[0] == null || attachments[0].MimePart == null || !attachments[0].MimePart.IsEmbeddedMessage)
			{
				this.TracePass("Journal Report has more or less attachments than 1 - skipping message.", new object[0]);
				return null;
			}
			return attachments[0].EmbeddedMessage;
		}

		private void TracePass(string formatString, params object[] args)
		{
			if (base.MailItem != null)
			{
				RmsClientManager.TracePass(this, base.MailItem.SystemProbeId, formatString, args);
			}
		}

		private void TraceFail(string formatString, params object[] args)
		{
			if (base.MailItem != null)
			{
				RmsClientManager.TraceFail(this, base.MailItem.SystemProbeId, formatString, args);
			}
		}

		private bool ShouldSendTnef(EmailMessage message)
		{
			if (message.RootPart == null)
			{
				return true;
			}
			HeaderList headers = message.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Organization-ContentConversionOptions");
			if (header != null)
			{
				BifInfo bifInfo = new BifInfo();
				bifInfo.CopyFromContentConversionOptionsString(header.Value);
				if (bifInfo.SendTNEF != null)
				{
					return bifInfo.SendTNEF.Value;
				}
			}
			return true;
		}

		private const int MaxDeferrals = 6;

		private const string DeferralCountProperty = "Microsoft.Exchange.JournalReportDecryptionAgent.DeferralCount";

		private readonly Breadcrumbs<string> breadcrumbs = new Breadcrumbs<string>(8);

		private readonly JournalReportDecryptionAgentFactory agentFactory;

		private bool isActive;

		private bool isE4eEncrypted;

		private CategorizedMessageEventSource source;

		private EmailMessage embeddedMsg;

		private Guid tenantId;

		private RmsDecryptor rmsDecryptor;
	}
}
