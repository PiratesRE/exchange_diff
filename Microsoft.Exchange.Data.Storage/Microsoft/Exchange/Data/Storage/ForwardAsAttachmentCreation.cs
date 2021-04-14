using System;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ForwardAsAttachmentCreation : ReplyForwardCommon
	{
		internal ForwardAsAttachmentCreation(MessageItem originalItem, MessageItem message, ReplyForwardConfiguration configuration) : base(originalItem, message, configuration, true)
		{
		}

		protected override void BuildSubject()
		{
			this.newItem[InternalSchema.SubjectPrefix] = ClientStrings.ItemForward.ToString(base.Culture);
			this.newItem[InternalSchema.NormalizedSubject] = this.originalItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		protected override void BuildBody(BodyConversionCallbacks callbacks)
		{
			string charset = this.originalItem.Body.Charset;
			try
			{
				Charset charset2 = Charset.GetCharset(charset);
				this.newItem[InternalSchema.InternetCpid] = charset2.CodePage;
				this.newItem[InternalSchema.Codepage] = ConvertUtils.MapItemWindowsCharset(charset2).CodePage;
			}
			catch (InvalidCharsetException)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "ForwardAsAttachmentCreation::BuildBody. Original message had invalid charset {0}", charset);
			}
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
			using (ItemAttachment itemAttachment = this.newItem.AttachmentCollection.Create(AttachmentType.EmbeddedMessage) as ItemAttachment)
			{
				using (Item item = itemAttachment.GetItem(InternalSchema.ContentConversionProperties))
				{
					this.originalItem.Load(InternalSchema.ContentConversionProperties);
					Item.CopyItemContent(this.originalItem, item);
					item.DeleteProperties(new PropertyDefinition[]
					{
						InternalSchema.EntryId,
						InternalSchema.MessageStatus,
						InternalSchema.SubmitFlags,
						InternalSchema.ReceivedRepresenting,
						InternalSchema.UrlCompName
					});
					item[InternalSchema.Flags] = this.originalItem.GetValueOrDefault<MessageFlags>(InternalSchema.Flags);
					item[InternalSchema.HasBeenSubmitted] = false;
					item.SaveFlags |= this.newItem.SaveFlags;
					item.Save(SaveMode.NoConflictResolution);
				}
				itemAttachment.Save();
			}
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			this.newItem[InternalSchema.Importance] = (Importance)StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.Importance), typeof(Importance), Importance.Normal);
			this.newItem[InternalSchema.Codepage] = (int)StoreObject.SafePropertyValue(this.originalItem.TryGetProperty(InternalSchema.Codepage), typeof(int), 0);
			if (this.originalItem is MeetingRequest || this.originalItem is MeetingCancellation || this.originalItem is CalendarItemBase || base.TreatAsMeetingMessage)
			{
				this.newItem.SetOrDeleteProperty(InternalSchema.From, this.originalItem.GetValueOrDefault<Participant>(InternalSchema.From));
				this.newItem.SetOrDeleteProperty(InternalSchema.AcceptLanguage, this.originalItem.GetValueOrDefault<string>(InternalSchema.AcceptLanguage));
			}
			this.newItem[InternalSchema.OriginalAuthorName] = string.Empty;
		}
	}
}
