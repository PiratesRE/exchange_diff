using System;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal abstract class MessageImplementation : IDisposable
	{
		internal abstract ObjectThreadAccessToken AccessToken { get; }

		public abstract EmailRecipient From { get; set; }

		public abstract EmailRecipientCollection To { get; }

		public abstract EmailRecipientCollection Cc { get; }

		public abstract EmailRecipientCollection Bcc { get; }

		public abstract EmailRecipientCollection ReplyTo { get; }

		public abstract EmailRecipient DispositionNotificationTo { get; set; }

		public abstract EmailRecipient Sender { get; set; }

		public abstract DateTime Date { get; set; }

		public abstract DateTime Expires { get; set; }

		public abstract DateTime ReplyBy { get; set; }

		public abstract string Subject { get; set; }

		public abstract string MessageId { get; set; }

		public abstract Importance Importance { get; set; }

		public abstract Priority Priority { get; set; }

		public abstract Sensitivity Sensitivity { get; set; }

		public abstract string MapiMessageClass { get; }

		public virtual MimeDocument MimeDocument
		{
			get
			{
				return null;
			}
		}

		public virtual MimePart RootPart
		{
			get
			{
				return null;
			}
		}

		public virtual MimePart CalendarPart
		{
			get
			{
				return null;
			}
		}

		public virtual MimePart TnefPart
		{
			get
			{
				return null;
			}
		}

		public abstract bool IsInterpersonalMessage { get; }

		public abstract bool IsPublicFolderReplicationMessage { get; }

		public abstract bool IsSystemMessage { get; }

		public abstract bool IsOpaqueMessage { get; }

		public abstract MessageSecurityType MessageSecurityType { get; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal virtual void Dispose(bool disposing)
		{
		}

		public abstract void Normalize(bool allowUTF8);

		internal abstract void Normalize(NormalizeOptions normalizeOptions, bool allowUTF8);

		internal abstract void Synchronize();

		internal abstract void SetReadOnly(bool makeReadOnly);

		internal abstract IMapiPropertyAccess MapiProperties { get; }

		internal abstract int Version { get; }

		internal abstract EmailRecipientCollection BccFromOrgHeader { get; }

		internal abstract void AddRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient newRecipient);

		internal abstract void RemoveRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient oldRecipient);

		internal abstract void ClearRecipients(RecipientType recipientType, ref object cachedHeader);

		internal abstract IBody GetBody();

		internal abstract AttachmentCookie AttachmentCollection_AddAttachment(Attachment attachment);

		internal abstract bool AttachmentCollection_RemoveAttachment(AttachmentCookie cookie);

		internal abstract void AttachmentCollection_ClearAttachments();

		internal abstract int AttachmentCollection_Count();

		internal abstract object AttachmentCollection_Indexer(int index);

		internal abstract AttachmentCookie AttachmentCollection_CacheAttachment(int publicIndex, object attachment);

		internal abstract MimePart Attachment_GetMimePart(AttachmentCookie cookie);

		internal abstract string Attachment_GetContentType(AttachmentCookie cookie);

		internal abstract void Attachment_SetContentType(AttachmentCookie cookie, string contentType);

		internal abstract AttachmentMethod Attachment_GetAttachmentMethod(AttachmentCookie cookie);

		internal abstract InternalAttachmentType Attachment_GetAttachmentType(AttachmentCookie cookie);

		internal abstract void Attachment_SetAttachmentType(AttachmentCookie cookie, InternalAttachmentType attachmentType);

		internal abstract EmailMessage Attachment_GetEmbeddedMessage(AttachmentCookie cookie);

		internal abstract void Attachment_SetEmbeddedMessage(AttachmentCookie cookie, EmailMessage embeddedMessage);

		internal abstract string Attachment_GetFileName(AttachmentCookie cookie, ref int sequenceNumber);

		internal abstract void Attachment_SetFileName(AttachmentCookie cookie, string fileName);

		internal abstract string Attachment_GetContentDisposition(AttachmentCookie cookie);

		internal abstract void Attachment_SetContentDisposition(AttachmentCookie cookie, string contentDisposition);

		internal abstract bool Attachment_IsAppleDouble(AttachmentCookie cookie);

		internal abstract Stream Attachment_GetContentReadStream(AttachmentCookie cookie);

		internal abstract bool Attachment_TryGetContentReadStream(AttachmentCookie cookie, out Stream result);

		internal abstract Stream Attachment_GetContentWriteStream(AttachmentCookie cookie);

		internal abstract int Attachment_GetRenderingPosition(AttachmentCookie cookie);

		internal abstract string Attachment_GetAttachContentID(AttachmentCookie cookie);

		internal abstract string Attachment_GetAttachContentLocation(AttachmentCookie cookie);

		internal abstract byte[] Attachment_GetAttachRendering(AttachmentCookie cookie);

		internal abstract int Attachment_GetAttachmentFlags(AttachmentCookie cookie);

		internal abstract bool Attachment_GetAttachHidden(AttachmentCookie cookie);

		internal abstract int Attachment_GetHashCode(AttachmentCookie cookie);

		internal abstract void Attachment_Dispose(AttachmentCookie cookie);

		internal virtual void CopyTo(MessageImplementation destination)
		{
		}

		internal virtual object GetMapiProperty(TnefPropertyTag tag)
		{
			return null;
		}
	}
}
