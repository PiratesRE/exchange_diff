using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class MessageEventHandler : ItemEventHandler
	{
		protected static void ClearRecipients(MessageItem message, params RecipientItemType[] recipientTypes)
		{
			int num = message.Recipients.Count;
			message.Load(new PropertyDefinition[]
			{
				MessageItemSchema.IsResend
			});
			object obj = message.TryGetProperty(MessageItemSchema.IsResend);
			int i = 0;
			while (i < num)
			{
				if (obj is bool && (bool)obj && message.Recipients[i].Submitted)
				{
					i++;
				}
				else
				{
					bool flag = true;
					if (recipientTypes != null && recipientTypes.Length > 0)
					{
						flag = false;
						foreach (RecipientItemType recipientItemType in recipientTypes)
						{
							if (message.Recipients[i].RecipientItemType == recipientItemType)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						num--;
						message.Recipients.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		protected bool AddMessageRecipients(RecipientCollection recipients, RecipientItemType recipientItemType, string wellName)
		{
			bool flag = false;
			this.Writer.Write("<div id=\"");
			this.Writer.Write(wellName);
			this.Writer.Write("\">");
			RecipientInfo[] array = (RecipientInfo[])base.GetParameter(wellName);
			if (array == null)
			{
				this.Writer.Write("</div>");
				return false;
			}
			List<Participant> list = new List<Participant>();
			foreach (RecipientInfo recipientInfo in array)
			{
				flag |= base.GetExchangeParticipantsFromRecipientInfo(recipientInfo, list);
			}
			for (int j = 0; j < list.Count; j++)
			{
				recipients.Add(list[j], recipientItemType);
			}
			this.Writer.Write("</div>");
			return flag;
		}

		protected Participant FromParticipant
		{
			get
			{
				return this.fromParticipant;
			}
			set
			{
				this.fromParticipant = value;
			}
		}

		protected bool AddFromRecipient(MessageItem message)
		{
			bool flag = false;
			bool flag2 = false;
			List<Participant> list = new List<Participant>();
			RecipientInfo recipientInfo = (RecipientInfo)base.GetParameter("From");
			if (recipientInfo != null)
			{
				flag |= base.GetExchangeParticipantsFromRecipientInfo(recipientInfo, list);
				if (list.Count == 1)
				{
					message.From = list[0];
					SubscriptionCacheEntry subscriptionCacheEntry;
					if (RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
					{
						SubscriptionCache.GetCache(base.UserContext);
					}, this.GetHashCode()) && base.UserContext.SubscriptionCache.TryGetEntry(message.From, out subscriptionCacheEntry))
					{
						flag2 = true;
						message[MessageItemSchema.SharingInstanceGuid] = subscriptionCacheEntry.Id;
						message[ItemSchema.SentRepresentingEmailAddress] = subscriptionCacheEntry.Address;
						message[ItemSchema.SentRepresentingDisplayName] = subscriptionCacheEntry.DisplayName;
					}
					if (!flag2)
					{
						this.fromParticipant = message.From;
					}
				}
			}
			else
			{
				message.From = null;
			}
			return flag;
		}

		protected RecipientInfoCacheEntry GetFromRecipientEntry(string routingAddress)
		{
			RecipientInfoAC[] array = (RecipientInfoAC[])base.GetParameter("Recips");
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].RoutingAddress == routingAddress)
					{
						return AutoCompleteCacheEntry.ParseClientEntry(array[i]);
					}
				}
			}
			return null;
		}

		protected bool UpdateRecipientsOnAutosave()
		{
			bool result = true;
			object parameter = base.GetParameter("UpdRcpAs");
			if (parameter != null)
			{
				result = (bool)parameter;
			}
			return result;
		}

		protected void UpdateReadMessage(MessageItem message)
		{
			this.UpdateSubject(message);
			object parameter = base.GetParameter("AudioNotes");
			if (parameter != null)
			{
				message[MessageItemSchema.MessageAudioNotes] = (string)parameter;
			}
			parameter = base.GetParameter("AlWbBcn");
			if (parameter != null)
			{
				if (Utilities.IsPublic(message))
				{
					throw new OwaEventHandlerException("Allow web beacon parameter not valid in public folder");
				}
				message[ItemSchema.BlockStatus] = BlockStatus.NoNeverAgain;
			}
			parameter = base.GetParameter("StLnkEnbl");
			if (parameter != null && parameter is bool && (bool)parameter)
			{
				message[ItemSchema.LinkEnabled] = true;
			}
		}

		protected bool UpdateMessage(MessageItem message, StoreObjectType storeObjectType)
		{
			this.UpdateSubject(message);
			this.UpdateCommonMessageProperties(message);
			this.UpdateComplianceAction(message);
			this.UpdateBody(message, storeObjectType);
			return this.UpdateRecipients(message, storeObjectType);
		}

		protected bool UpdateMessageForAutoSave(MessageItem message, StoreObjectType storeObjectType)
		{
			bool result = false;
			this.TryUpdateSubject(message);
			this.UpdateCommonMessageProperties(message);
			this.UpdateComplianceAction(message);
			this.UpdateBody(message, storeObjectType);
			if (this.UpdateRecipientsOnAutosave())
			{
				result = this.UpdateRecipients(message, storeObjectType);
			}
			return result;
		}

		protected void UpdateComplianceAction(MessageItem message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			object parameter = base.GetParameter("CmpAc");
			if (parameter == null)
			{
				return;
			}
			string text = (string)parameter;
			if (text == "0")
			{
				message[ItemSchema.IsClassified] = false;
				message[ItemSchema.ClassificationGuid] = string.Empty;
				message[ItemSchema.ClassificationDescription] = string.Empty;
				message[ItemSchema.Classification] = string.Empty;
				if (Utilities.IsIrmDecrypted(message))
				{
					((RightsManagedMessageItem)message).SetRestriction(null);
				}
				return;
			}
			Guid empty = Guid.Empty;
			ComplianceType complianceType = ComplianceType.Unknown;
			if (GuidHelper.TryParseGuid(text, out empty))
			{
				complianceType = OwaContext.Current.UserContext.ComplianceReader.GetComplianceType(empty, base.UserContext.UserCulture);
			}
			switch (complianceType)
			{
			case ComplianceType.MessageClassification:
			{
				ClassificationSummary classificationSummary = base.UserContext.ComplianceReader.MessageClassificationReader.LookupMessageClassification(empty, base.UserContext.UserCulture);
				if (classificationSummary == null)
				{
					throw new OwaEventHandlerException("Invalid classification being set from client", LocalizedStrings.GetNonEncoded(-1799006479), OwaEventHandlerErrorCode.ComplianceLabelNotFoundError);
				}
				message[ItemSchema.IsClassified] = true;
				message[ItemSchema.ClassificationGuid] = classificationSummary.ClassificationID.ToString();
				message[ItemSchema.ClassificationDescription] = classificationSummary.SenderDescription;
				message[ItemSchema.Classification] = classificationSummary.DisplayName;
				message[ItemSchema.ClassificationKeep] = classificationSummary.RetainClassificationEnabled;
				if (Utilities.IsIrmDecrypted(message))
				{
					((RightsManagedMessageItem)message).SetRestriction(null);
					return;
				}
				return;
			}
			case ComplianceType.RmsTemplate:
				if (Utilities.IsIrmDecrypted(message))
				{
					RmsTemplate rmsTemplate = base.UserContext.ComplianceReader.RmsTemplateReader.LookupRmsTemplate(empty);
					if (rmsTemplate == null)
					{
						throw new OwaEventHandlerException("Invalid RMS template was sent from client.", LocalizedStrings.GetNonEncoded(-1799006479), OwaEventHandlerErrorCode.ComplianceLabelNotFoundError);
					}
					((RightsManagedMessageItem)message).SetRestriction(rmsTemplate);
					if (message.Sender == null)
					{
						message.Sender = new Participant(base.UserContext.MailboxSession.MailboxOwner);
					}
				}
				message[ItemSchema.IsClassified] = false;
				message[ItemSchema.ClassificationGuid] = string.Empty;
				message[ItemSchema.ClassificationDescription] = string.Empty;
				message[ItemSchema.Classification] = string.Empty;
				return;
			}
			if (!OwaContext.Current.UserContext.ComplianceReader.RmsTemplateReader.IsInternalLicensingEnabled)
			{
				throw new OwaEventHandlerException("Unable to determine compliance type because licensing against internal RMS server has been disabled.", LocalizedStrings.GetNonEncoded(-27910813), OwaEventHandlerErrorCode.ComplianceLabelNotFoundError, true);
			}
			if (OwaContext.Current.UserContext.ComplianceReader.RmsTemplateReader.TemplateAcquisitionFailed)
			{
				throw new OwaEventHandlerException("Unable to determine compliance type because there was an error loading templates from the RMS server.", LocalizedStrings.GetNonEncoded(1084956906), OwaEventHandlerErrorCode.ComplianceLabelNotFoundError, true);
			}
			throw new OwaEventHandlerException("Invalid compliance label was sent from client.", LocalizedStrings.GetNonEncoded(-1799006479), OwaEventHandlerErrorCode.ComplianceLabelNotFoundError);
		}

		private string GetSubject(MessageItem message)
		{
			return (string)base.GetParameter("Subj");
		}

		private void UpdateSubject(MessageItem message)
		{
			string subject = this.GetSubject(message);
			if (subject == null)
			{
				return;
			}
			if (subject.Length <= 255)
			{
				message.Subject = subject;
				return;
			}
			throw new OwaEventHandlerException("The subject exceeds the max length " + 255);
		}

		private void TryUpdateSubject(MessageItem message)
		{
			string subject = this.GetSubject(message);
			if (subject != null && subject.Length <= 255)
			{
				message.Subject = subject;
			}
		}

		private void UpdateBody(MessageItem message, StoreObjectType storeObjectType)
		{
			string text = (string)base.GetParameter("Body");
			object parameter = base.GetParameter("Text");
			if (text != null && parameter != null)
			{
				Markup markup = ((bool)parameter) ? Markup.PlainText : Markup.Html;
				BodyConversionUtilities.SetBody(message, text, markup, storeObjectType, base.UserContext);
			}
		}

		protected bool UpdateRecipients(MessageItem message, StoreObjectType storeObjectType)
		{
			bool flag = this.UpdateRecipients(message);
			if (storeObjectType == StoreObjectType.Message)
			{
				flag |= this.AddFromRecipient(message);
			}
			return flag;
		}

		protected bool UpdateRecipients(MessageItem message)
		{
			bool flag = false;
			MessageEventHandler.ClearRecipients(message, new RecipientItemType[0]);
			flag |= this.AddMessageRecipients(message.Recipients, RecipientItemType.To, "To");
			flag |= this.AddMessageRecipients(message.Recipients, RecipientItemType.Cc, "Cc");
			return flag | this.AddMessageRecipients(message.Recipients, RecipientItemType.Bcc, "Bcc");
		}

		private void UpdateCommonMessageProperties(MessageItem message)
		{
			object parameter = base.GetParameter("Imp");
			if (parameter != null)
			{
				message.Importance = (Importance)parameter;
			}
			parameter = base.GetParameter("Sensitivity");
			if (parameter != null)
			{
				message.Sensitivity = (Sensitivity)parameter;
			}
			parameter = base.GetParameter("AudioNotes");
			if (parameter != null)
			{
				message[MessageItemSchema.MessageAudioNotes] = (string)parameter;
			}
			parameter = base.GetParameter("DeliveryRcpt");
			if (parameter != null)
			{
				message.IsDeliveryReceiptRequested = (bool)parameter;
			}
			parameter = base.GetParameter("ReadRcpt");
			if (parameter != null)
			{
				message.IsReadReceiptRequested = (bool)parameter;
			}
		}

		protected void RenderErrorForAutoSave(Exception exception)
		{
			ExTraceGlobals.CoreTracer.TraceDebug<string>((long)this.GetHashCode(), "MessageEventHandler.RenderErrorForAutoSave. Exception {0} thrown", exception.Message);
			try
			{
				ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(exception, base.OwaContext.MailboxIdentity);
				Exception ex = (exception.InnerException == null) ? exception : exception.InnerException;
				StringBuilder stringBuilder = new StringBuilder();
				using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
				{
					OwaEventHttpHandler.RenderError(base.OwaContext, stringWriter, exceptionHandlingInformation.Message, exceptionHandlingInformation.MessageDetails, exceptionHandlingInformation.OwaEventHandlerErrorCode, exceptionHandlingInformation.HideDebugInformation ? null : ex);
				}
				this.Writer.Write(stringBuilder.ToString());
				base.OwaContext.ErrorSent = true;
			}
			finally
			{
				Utilities.HandleException(base.OwaContext, exception);
			}
		}

		public const string Subject = "Subj";

		public const string Importance = "Imp";

		public const string Private = "Sensitivity";

		public const string To = "To";

		public const string Cc = "Cc";

		public const string Bcc = "Bcc";

		public const string From = "From";

		public const string Body = "Body";

		public const string Text = "Text";

		public const string Recipients = "Recips";

		public const string AudioNotes = "AudioNotes";

		public const string IsDeliveryReceiptRequested = "DeliveryRcpt";

		public const string IsReadReceiptRequested = "ReadRcpt";

		public const string ComplianceAction = "CmpAc";

		public const string AllowWebBeacon = "AlWbBcn";

		public const string SetLinkEnabled = "StLnkEnbl";

		public const string AutosaveUpdateRecipients = "UpdRcpAs";

		private Participant fromParticipant;
	}
}
