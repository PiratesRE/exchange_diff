using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ClassificationApplicationAgent : StoreDriverDeliveryAgent
	{
		public ClassificationApplicationAgent()
		{
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			if (this.msgClassifications == null)
			{
				this.msgClassifications = ClassificationUtils.ExtractClassifications(storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem.RootPart.Headers);
			}
			if (this.IsJournalReport(storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem))
			{
				this.ProcessClassificationsForJournalReport(storeDriverDeliveryEventArgsImpl);
				return;
			}
			this.ProcessClassifications(storeDriverDeliveryEventArgsImpl);
		}

		private bool IsJournalReport(MbxTransportMailItem mailItem)
		{
			if (mailItem != null && mailItem.RootPart != null)
			{
				Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Journal-Report");
				return header != null;
			}
			return false;
		}

		private void ProcessClassifications(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			ClassificationSummary classificationSummary = this.GetClassificationSummary(argsImpl);
			if (classificationSummary != null)
			{
				this.SetBanner(classificationSummary, argsImpl);
			}
		}

		private void ProcessClassificationsForJournalReport(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			ClassificationSummary classificationSummary = this.GetClassificationSummary(argsImpl);
			if (classificationSummary != null && classificationSummary.IsValid && classificationSummary.IsClassified)
			{
				using (ItemAttachment itemAttachment = this.TryOpenFirstAttachment(argsImpl.ReplayItem) as ItemAttachment)
				{
					if (itemAttachment != null)
					{
						using (MessageItem itemAsMessage = itemAttachment.GetItemAsMessage(StoreObjectSchema.ContentConversionProperties))
						{
							if (itemAsMessage != null)
							{
								ClassificationApplicationAgent.diag.TraceDebug<string>(0L, "Promote banner for recipient {0} on embedded message of journal report", argsImpl.MailRecipient.Email.ToString());
								itemAsMessage[ItemSchema.IsClassified] = classificationSummary.IsClassified;
								itemAsMessage[ItemSchema.Classification] = classificationSummary.DisplayName;
								itemAsMessage[ItemSchema.ClassificationDescription] = classificationSummary.RecipientDescription;
								itemAsMessage[ItemSchema.ClassificationGuid] = classificationSummary.ClassificationID.ToString();
								itemAsMessage[ItemSchema.ClassificationKeep] = classificationSummary.RetainClassificationEnabled;
								itemAsMessage.Save(SaveMode.NoConflictResolution);
								itemAttachment.Save();
							}
						}
					}
				}
			}
		}

		private void SetBanner(ClassificationSummary summary, StoreDriverDeliveryEventArgsImpl args)
		{
			if (summary.IsValid)
			{
				if (summary.IsClassified)
				{
					ClassificationApplicationAgent.diag.TraceDebug<string>(0L, "Promote banner for recipient {0}", args.MailRecipient.Email.ToString());
					args.ReplayItem[ItemSchema.IsClassified] = summary.IsClassified;
					args.ReplayItem[ItemSchema.Classification] = summary.DisplayName;
					args.ReplayItem[ItemSchema.ClassificationDescription] = summary.RecipientDescription;
					args.ReplayItem[ItemSchema.ClassificationGuid] = summary.ClassificationID.ToString();
					args.ReplayItem[ItemSchema.ClassificationKeep] = summary.RetainClassificationEnabled;
					return;
				}
				ClassificationApplicationAgent.diag.TraceDebug<string>(0L, "Clear banner for recipient {0}", args.MailRecipient.Email.ToString());
				args.ReplayItem.DeleteProperties(new PropertyDefinition[]
				{
					ItemSchema.IsClassified,
					ItemSchema.Classification,
					ItemSchema.ClassificationDescription,
					ItemSchema.ClassificationGuid,
					ItemSchema.ClassificationKeep
				});
			}
		}

		private ClassificationSummary GetClassificationSummary(StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			if (this.msgClassifications == null || 0 >= this.msgClassifications.Count)
			{
				return null;
			}
			CultureInfo cultureInfo = argsImpl.IsPublicFolderRecipient ? CultureInfo.InvariantCulture : argsImpl.MailboxSession.PreferedCulture;
			if (cultureInfo.Equals(this.previousLocale))
			{
				return null;
			}
			this.previousLocale = cultureInfo;
			return Components.ClassificationConfig.Summarize(argsImpl.MailItemDeliver.MbxTransportMailItem.OrganizationId, this.msgClassifications, cultureInfo);
		}

		private Attachment TryOpenFirstAttachment(MessageItem messageItem)
		{
			using (IEnumerator<AttachmentHandle> enumerator = messageItem.AttachmentCollection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AttachmentHandle handle = enumerator.Current;
					return messageItem.AttachmentCollection.Open(handle, AttachmentType.EmbeddedMessage);
				}
			}
			return null;
		}

		private static readonly Trace diag = ExTraceGlobals.MapiDeliverTracer;

		private List<string> msgClassifications;

		private CultureInfo previousLocale;
	}
}
