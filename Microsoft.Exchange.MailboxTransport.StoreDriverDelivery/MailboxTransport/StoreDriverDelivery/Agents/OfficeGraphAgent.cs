using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OfficeGraph;
using Microsoft.Exchange.SharePointSignalStore;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OfficeGraphAgent : StoreDriverDeliveryAgent
	{
		static OfficeGraphAgent()
		{
			OfficeGraphLog.Start();
		}

		public OfficeGraphAgent()
		{
			this.traceId = this.GetHashCode();
			base.OnDeliveredMessage += this.DeliveredMessageEventHandler;
		}

		public void DeliveredMessageEventHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			try
			{
				StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = args as StoreDriverDeliveryEventArgsImpl;
				if (storeDriverDeliveryEventArgsImpl != null && this.IsInputValid(storeDriverDeliveryEventArgsImpl))
				{
					VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(storeDriverDeliveryEventArgsImpl.MailboxOwner.GetContext(null), null, null);
					if (snapshot.OfficeGraph.OfficeGraphAgent.Enabled)
					{
						OfficeGraph.ItemsSeen.Increment();
						bool flag = false;
						OfficeGraphSignalType signalType = OfficeGraphSignalType.Attachment;
						string signal = string.Empty;
						bool enabled = snapshot.OfficeGraph.OfficeGraphGenerateSignals.Enabled;
						DateTime utcNow = DateTime.UtcNow;
						storeDriverDeliveryEventArgsImpl.MessageItem.Load();
						string sharePointUrl;
						List<Dictionary<string, string>> attachmentsProperties;
						if (this.IsInterestingMailWithAttachments(storeDriverDeliveryEventArgsImpl.MessageItem, storeDriverDeliveryEventArgsImpl.MailboxSession, out sharePointUrl, out attachmentsProperties))
						{
							OfficeGraph.ItemsFilteredTotal.Increment();
							if (enabled)
							{
								signal = OfficeGraphAgentUtils.CreateAttachmentsSignal(storeDriverDeliveryEventArgsImpl.MessageItem, attachmentsProperties, storeDriverDeliveryEventArgsImpl.MailboxOwner.PrimarySmtpAddress.ToString(), storeDriverDeliveryEventArgsImpl.MailItem.TenantId);
								signalType = OfficeGraphSignalType.Attachment;
								flag = true;
							}
							else
							{
								OfficeGraphAgent.tracer.TraceDebug((long)this.traceId, "Skipped generating signal since feature disabled.");
							}
						}
						else
						{
							OfficeGraphAgent.tracer.TraceDebug((long)this.traceId, "Skipped item since it did not pass filter criteria.");
						}
						if (flag)
						{
							TimeSpan timeSpan = DateTime.UtcNow - utcNow;
							OfficeGraph.LastSignalCreationTime.RawValue = (long)timeSpan.TotalMilliseconds;
							OfficeGraph.AverageSignalCreationTime.IncrementBy((long)timeSpan.TotalMilliseconds);
							OfficeGraph.AverageSignalCreationTimeBase.Increment();
							utcNow = DateTime.UtcNow;
							byte[] bytes = storeDriverDeliveryEventArgsImpl.MailboxSession.OrganizationId.GetBytes(Encoding.ASCII);
							string organizationId = Convert.ToBase64String(bytes);
							OfficeGraphLog.LogSignal(signalType, signal, organizationId, sharePointUrl);
							timeSpan = DateTime.UtcNow - utcNow;
							OfficeGraph.LastSignalPersistingTime.RawValue = (long)timeSpan.TotalMilliseconds;
							OfficeGraph.AverageSignalPersistingTime.IncrementBy((long)timeSpan.TotalMilliseconds);
							OfficeGraph.AverageSignalPersistingTimeBase.Increment();
							OfficeGraph.SignalPersisted.Increment();
						}
					}
					else
					{
						OfficeGraphAgent.tracer.TraceDebug((long)this.traceId, "Skipped item since agent is disabled.");
					}
				}
			}
			catch (Exception ex)
			{
				OfficeGraph.TotalExceptions.Increment();
				OfficeGraphAgent.tracer.TraceError<Exception>((long)this.traceId, "OfficeGraphAgent.DeliveredMessageEventHandler encountered an exception: {0}", ex);
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_OfficeGraphAgentException, ex.Message, new object[]
				{
					ex
				});
			}
		}

		private bool IsInputValid(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			if (argsImpl.ReplayItem == null)
			{
				OfficeGraphAgent.tracer.TraceDebug((long)this.traceId, "Replay item is null; skipping agent");
				return false;
			}
			MailboxSession mailboxSession = argsImpl.StoreSession as MailboxSession;
			if (mailboxSession == null)
			{
				OfficeGraphAgent.tracer.TraceDebug((long)this.traceId, "Mailbox session is null; skipping agent");
				return false;
			}
			if (mailboxSession.MailboxOwner.RecipientTypeDetails != RecipientTypeDetails.UserMailbox && mailboxSession.MailboxOwner.RecipientTypeDetails != RecipientTypeDetails.LinkedMailbox)
			{
				OfficeGraphAgent.tracer.TraceDebug<Guid, string, RecipientTypeDetails>((long)this.traceId, "Skipping mailbox with guid {0} and display name {1} since this is a {2} and not a UserMailbox or a LinkedMailbox", mailboxSession.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.DisplayName, mailboxSession.MailboxOwner.RecipientTypeDetails);
				return false;
			}
			return true;
		}

		private bool IsInterestingMailWithAttachments(MessageItem messageItem, MailboxSession mailboxSession, out string sharePointUrl, out List<Dictionary<string, string>> attachmentsProperties)
		{
			if (this.IsEmailMessage(messageItem) && this.HasAttachments(messageItem) && this.HasSharePointUrl(mailboxSession, out sharePointUrl) && this.AreAttachmentsInteresting(messageItem, out attachmentsProperties))
			{
				this.IsFromFavoriteSender(messageItem);
				return true;
			}
			sharePointUrl = string.Empty;
			attachmentsProperties = new List<Dictionary<string, string>>();
			return false;
		}

		private bool IsEmailMessage(MessageItem messageItem)
		{
			if (messageItem.ClassName.Equals("IPM.Note", StringComparison.InvariantCultureIgnoreCase))
			{
				OfficeGraph.FilterCriteriaMessage.Increment();
				return true;
			}
			return false;
		}

		private bool HasAttachments(MessageItem messageItem)
		{
			if (messageItem.AttachmentCollection.Count > 0)
			{
				OfficeGraph.FilterCriteriaHasAttachment.Increment();
				return true;
			}
			return false;
		}

		private bool AreAttachmentsInteresting(MessageItem messageItem, out List<Dictionary<string, string>> attachmentsProperties)
		{
			bool flag = false;
			attachmentsProperties = new List<Dictionary<string, string>>();
			foreach (AttachmentHandle handle in messageItem.AttachmentCollection.GetAllHandles())
			{
				using (Attachment attachment = messageItem.AttachmentCollection.Open(handle))
				{
					if (this.supportedAttachmentTypes.Contains(attachment.FileExtension.ToLower(CultureInfo.InvariantCulture)))
					{
						attachmentsProperties.Add(new Dictionary<string, string>
						{
							{
								"AttachmentId",
								attachment.Id.ToBase64String()
							},
							{
								"AttachmentFileName",
								attachment.FileName
							},
							{
								"AttachmentExtension",
								attachment.FileExtension
							},
							{
								"AttachmentSize",
								attachment.Size.ToString(CultureInfo.InvariantCulture)
							}
						});
						flag = true;
					}
				}
			}
			if (flag)
			{
				OfficeGraph.FilterCriteriaInterestingAttachment.Increment();
			}
			return flag;
		}

		private bool IsFromFavoriteSender(MessageItem messageItem)
		{
			if (messageItem.IsFromFavoriteSender)
			{
				OfficeGraph.FilterCriteriaIsFromFavoriteSender.Increment();
				return true;
			}
			return false;
		}

		private bool HasSharePointUrl(MailboxSession mailboxSession, out string sharePointUrl)
		{
			sharePointUrl = this.GetSharePointUrl(mailboxSession);
			if (!string.IsNullOrEmpty(sharePointUrl))
			{
				OfficeGraph.FilterCriteriaHasSharePointUrl.Increment();
				return true;
			}
			return false;
		}

		private string GetSharePointUrl(MailboxSession mailboxSession)
		{
			DateTime utcNow = DateTime.UtcNow;
			string result;
			using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Inbox, new PropertyDefinition[]
			{
				FolderSchema.OfficeGraphLocation
			}))
			{
				result = (folder.TryGetProperty(FolderSchema.OfficeGraphLocation) as string);
			}
			TimeSpan timeSpan = DateTime.UtcNow - utcNow;
			OfficeGraph.LastSharePointUrlRetrievalTime.RawValue = (long)timeSpan.TotalMilliseconds;
			OfficeGraph.AverageSharePointUrlRetrievalTime.IncrementBy((long)timeSpan.TotalMilliseconds);
			OfficeGraph.AverageSharePointUrlRetrievalTimeBase.Increment();
			return result;
		}

		private static readonly Trace tracer = ExTraceGlobals.OfficeGraphAgentTracer;

		private string[] supportedAttachmentTypes = new string[]
		{
			".doc",
			".docx",
			".xls",
			".xlsx",
			".ppt",
			".pptx",
			".pdf"
		};

		private readonly int traceId;
	}
}
