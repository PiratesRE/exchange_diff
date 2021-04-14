using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ForwardCreation : ReplyForwardCommon
	{
		internal ForwardCreation(Item originalItem, MessageItem message, ReplyForwardConfiguration parameters) : base(originalItem, message, parameters, true)
		{
			LastAction lastAction = LastAction.Forward;
			IconIndex iconIndex = IconIndex.MailForwarded;
			if (this.originalItemSigned)
			{
				iconIndex = IconIndex.MailSmimeSignedForwarded;
			}
			else if (this.originalItemEncrypted)
			{
				iconIndex = IconIndex.MailEncryptedForwarded;
			}
			else if (this.originalItemIrm)
			{
				iconIndex = IconIndex.MailIrmForwarded;
			}
			if (originalItem.Id != null && originalItem.Id.ObjectId != null && !originalItem.Id.ObjectId.IsFakeId && !(originalItem is PostItem))
			{
				this.newItem.SafeSetProperty(InternalSchema.ReplyForwardStatus, ReplyForwardUtils.EncodeReplyForwardStatus(lastAction, iconIndex, originalItem.Id));
			}
		}

		private bool IsPreservingSubject
		{
			get
			{
				return ForwardCreationFlags.PreserveSubject == (this.parameters.ForwardCreationFlags & ForwardCreationFlags.PreserveSubject);
			}
		}

		protected override void BuildSubject()
		{
			if (!this.IsPreservingSubject)
			{
				if (this.parameters.SubjectPrefix != null)
				{
					this.newItem[InternalSchema.SubjectPrefix] = this.parameters.SubjectPrefix;
				}
				else
				{
					this.newItem[InternalSchema.SubjectPrefix] = ClientStrings.ItemForward.ToString(base.Culture);
				}
			}
			this.newItem[InternalSchema.NormalizedSubject] = this.originalItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		protected override void BuildBody(BodyConversionCallbacks callbacks)
		{
			if (!base.IsResourceDelegationMessage && (this.newItem is MeetingMessage || base.TreatAsMeetingMessage) && !string.IsNullOrEmpty(this.parameters.BodyPrefix))
			{
				this.BuildMeetingMessageBody(callbacks);
				return;
			}
			ReplyForwardCommon.CopyBodyWithPrefix(this.originalItem.Body, this.newItem.Body, this.parameters, callbacks);
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			if ((this.parameters.ForwardCreationFlags & ForwardCreationFlags.PreserveSender) == ForwardCreationFlags.PreserveSender)
			{
				PostItem postItem = this.newItem as PostItem;
				if (postItem != null)
				{
					postItem.Sender = ((PostItem)this.originalItem).Sender;
				}
				else
				{
					((MessageItem)this.newItem).Sender = ((MessageItem)this.originalItem).Sender;
				}
			}
			this.newItem[InternalSchema.Importance] = (Importance)StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.Importance), typeof(Importance), Importance.Normal);
			this.newItem[InternalSchema.OriginalAuthorName] = string.Empty;
			if (this.originalItem is MeetingRequest || this.originalItem is MeetingCancellation || this.originalItem is CalendarItemBase || base.TreatAsMeetingMessage)
			{
				this.newItem.SetOrDeleteProperty(InternalSchema.From, this.originalItem.GetValueOrDefault<Participant>(InternalSchema.From));
				this.newItem.SetOrDeleteProperty(InternalSchema.AcceptLanguage, this.originalItem.GetValueOrDefault<string>(InternalSchema.AcceptLanguage));
				this.UpdateCalendarProperties();
			}
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
			base.CopyAttachments(callbacks, this.originalItem.AttachmentCollection, this.newItem.AttachmentCollection, false, this.parameters.TargetFormat == BodyFormat.TextPlain, optionsForSmime);
		}

		private void BuildMeetingMessageBody(BodyConversionCallbacks callbacks)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(this.originalItem.Body.RawFormat);
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(this.originalItem.Body.RawFormat);
			bodyWriteConfiguration.SetTargetFormat(this.parameters.TargetFormat, this.originalItem.Body.Charset);
			if (!string.IsNullOrEmpty(this.parameters.BodyPrefix))
			{
				bodyWriteConfiguration.AddInjectedText(this.parameters.BodyPrefix, null, this.parameters.BodyPrefixFormat);
			}
			if (callbacks.HtmlCallback != null)
			{
				bodyWriteConfiguration.HtmlCallback = callbacks.HtmlCallback;
				bodyWriteConfiguration.HtmlFlags = HtmlStreamingFlags.FilterHtml;
			}
			if (callbacks.RtfCallback != null)
			{
				bodyWriteConfiguration.RtfCallback = callbacks.RtfCallback;
			}
			using (TextReader textReader = this.originalItem.Body.OpenTextReader(configuration))
			{
				using (TextWriter textWriter = this.newItem.Body.OpenTextWriter(bodyWriteConfiguration))
				{
					Util.StreamHandler.CopyText(textReader, textWriter);
				}
			}
		}

		private void UpdateCalendarProperties()
		{
			for (int i = 0; i < this.parentPropDefinitions.Length; i++)
			{
				this.newItem.SafeSetProperty(this.parentPropDefinitions[i], this.parentPropValues[i]);
			}
			if (this.originalItem is MeetingRequest || this.originalItem is CalendarItemBase || base.TreatAsMeetingMessage)
			{
				BusyType? valueAsNullable = this.originalItem.GetValueAsNullable<BusyType>(InternalSchema.FreeBusyStatus);
				this.newItem[InternalSchema.FreeBusyStatus] = ((valueAsNullable != null && valueAsNullable.Value == BusyType.Free) ? BusyType.Free : BusyType.Tentative);
			}
		}

		private const string XsoHeader = "+~+~+~+~+~+~+~+~+~+";
	}
}
