using System;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class AttachmentWellInfo
	{
		public AttachmentWellInfo(AttachmentCollection collection, Attachment attachment, bool isJunkOrPhishing)
		{
			this.collection = collection;
			this.attachmentId = attachment.Id;
			if (isJunkOrPhishing)
			{
				this.attachmentLevel = AttachmentPolicy.Level.Block;
			}
			else
			{
				this.attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(attachment, UserContextManager.GetUserContext());
			}
			this.attachmentType = attachment.AttachmentType;
			this.fileName = attachment.FileName;
			if (this.attachmentType == AttachmentType.EmbeddedMessage)
			{
				using (Item itemAsReadOnly = ((ItemAttachment)attachment).GetItemAsReadOnly(null))
				{
					this.displayName = AttachmentUtility.GetEmbeddedAttachmentDisplayName(itemAsReadOnly);
				}
				this.fileExtension = ".msg";
			}
			else
			{
				this.displayName = attachment.DisplayName;
				this.fileExtension = ((attachment.FileExtension == null) ? string.Empty : attachment.FileExtension);
			}
			this.isInline = attachment.IsInline;
			this.attachmentSize = attachment.Size;
			this.attachmentName = AttachmentUtility.CalculateAttachmentName(this.displayName, this.fileName);
			this.mimeType = AttachmentUtility.CalculateContentType(attachment);
			this.textCharset = attachment.TextCharset;
		}

		public AttachmentWellInfo(AttachmentCollection collection, AttachmentLink attachmentLink, bool isJunkOrPhishing)
		{
			this.collection = collection;
			this.attachmentId = attachmentLink.AttachmentId;
			if (isJunkOrPhishing)
			{
				this.attachmentLevel = AttachmentPolicy.Level.Block;
			}
			else
			{
				this.attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(attachmentLink, UserContextManager.GetUserContext());
			}
			this.attachmentType = attachmentLink.AttachmentType;
			this.fileName = attachmentLink.Filename;
			this.displayName = attachmentLink.DisplayName;
			this.isInline = attachmentLink.IsInline(true);
			this.attachmentSize = attachmentLink.Size;
			this.fileExtension = attachmentLink.FileExtension;
			this.attachmentName = AttachmentUtility.CalculateAttachmentName(attachmentLink.DisplayName, attachmentLink.Filename);
			this.mimeType = attachmentLink.ContentType;
		}

		public AttachmentWellInfo(OwaStoreObjectId owaConversationId, AttachmentInfo attachmentInfo, bool isJunkOrPhishing)
		{
			this.messageId = OwaStoreObjectId.CreateFromStoreObjectId(attachmentInfo.MessageId, owaConversationId);
			this.attachmentId = attachmentInfo.AttachmentId;
			if (isJunkOrPhishing)
			{
				this.attachmentLevel = AttachmentPolicy.Level.Block;
			}
			else
			{
				this.attachmentLevel = AttachmentLevelLookup.GetAttachmentLevel(attachmentInfo, UserContextManager.GetUserContext());
			}
			this.attachmentType = attachmentInfo.AttachmentType;
			this.fileName = attachmentInfo.FileName;
			this.displayName = attachmentInfo.DisplayName;
			this.isInline = attachmentInfo.IsInline;
			this.attachmentSize = attachmentInfo.Size;
			this.fileExtension = attachmentInfo.FileExtension;
			this.attachmentName = AttachmentUtility.CalculateAttachmentName(attachmentInfo.DisplayName, attachmentInfo.FileName);
			this.mimeType = attachmentInfo.ContentType;
		}

		public Attachment OpenAttachment()
		{
			if (this.collection == null)
			{
				throw new InvalidOperationException("Attachment collection is null, this attachment might have been generated from a conversation item part.  OpenAttachment is not supported for these.");
			}
			return this.collection.Open(this.attachmentId);
		}

		public OwaStoreObjectId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string MimeType
		{
			get
			{
				return this.mimeType;
			}
			set
			{
				this.mimeType = value;
			}
		}

		public AttachmentId AttachmentId
		{
			get
			{
				return this.attachmentId;
			}
		}

		public AttachmentType AttachmentType
		{
			get
			{
				return this.attachmentType;
			}
		}

		public bool IsInline
		{
			get
			{
				return this.isInline;
			}
		}

		public AttachmentPolicy.Level AttachmentLevel
		{
			get
			{
				return this.attachmentLevel;
			}
		}

		public long Size
		{
			get
			{
				return this.attachmentSize;
			}
		}

		public string FileExtension
		{
			get
			{
				return this.fileExtension;
			}
			set
			{
				this.fileExtension = value;
			}
		}

		public string AttachmentName
		{
			get
			{
				return this.attachmentName;
			}
		}

		public Charset TextCharset
		{
			get
			{
				return this.textCharset;
			}
		}

		private string attachmentName;

		private string fileExtension;

		private string fileName;

		private string displayName;

		private bool isInline;

		private long attachmentSize;

		private string mimeType;

		private AttachmentType attachmentType;

		private AttachmentCollection collection;

		private AttachmentId attachmentId;

		private AttachmentPolicy.Level attachmentLevel;

		private Charset textCharset;

		private OwaStoreObjectId messageId;
	}
}
