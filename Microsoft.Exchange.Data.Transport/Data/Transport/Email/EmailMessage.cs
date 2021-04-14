using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	public class EmailMessage : IDisposable
	{
		internal EmailMessage(MessageImplementation message)
		{
			this.message = message;
			this.Synchronize();
			this.accessToken = new EmailMessage.EmailMessageThreadAccessToken(this);
		}

		public static EmailMessage Create()
		{
			return EmailMessage.Create(BodyFormat.Text, false, Charset.DefaultMimeCharset.Name);
		}

		public static EmailMessage Create(BodyFormat bodyFormat)
		{
			return EmailMessage.Create(bodyFormat, false, Charset.DefaultMimeCharset.Name);
		}

		public static EmailMessage Create(BodyFormat bodyFormat, bool createAlternative)
		{
			return EmailMessage.Create(bodyFormat, createAlternative, Charset.DefaultMimeCharset.Name);
		}

		public static EmailMessage Create(BodyFormat bodyFormat, bool createAlternative, string charsetName)
		{
			if (bodyFormat != BodyFormat.Text && bodyFormat != BodyFormat.Html)
			{
				throw new ArgumentException(EmailMessageStrings.CannotCreateSpecifiedBodyFormat(bodyFormat.ToString()));
			}
			if (bodyFormat == BodyFormat.Text && createAlternative)
			{
				throw new ArgumentException(EmailMessageStrings.CannotCreateAlternativeBody);
			}
			Charset.GetCharset(charsetName);
			MimeTnefMessage mimeTnefMessage = new MimeTnefMessage(bodyFormat, createAlternative, charsetName);
			return new EmailMessage(mimeTnefMessage);
		}

		public static EmailMessage Create(MimeDocument mimeDocument)
		{
			if (mimeDocument == null)
			{
				throw new ArgumentNullException("document");
			}
			if (mimeDocument.RootPart == null)
			{
				throw new ArgumentException(EmailMessageStrings.MimeDocumentRootPartMustNotBeNull);
			}
			MimeTnefMessage mimeTnefMessage = new MimeTnefMessage(mimeDocument);
			return new EmailMessage(mimeTnefMessage);
		}

		public static EmailMessage Create(Stream source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.CanRead)
			{
				throw new ArgumentException("Stream must support Read", "source");
			}
			MimeDocument mimeDocument = new MimeDocument();
			mimeDocument.Load(source, CachingMode.Copy);
			MimeTnefMessage mimeTnefMessage = new MimeTnefMessage(mimeDocument);
			return new EmailMessage(mimeTnefMessage);
		}

		public EmailRecipient From
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient from;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					from = this.message.From;
				}
				return from;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_From");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.From = value;
				}
			}
		}

		public EmailRecipientCollection To
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection to;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					to = this.message.To;
				}
				return to;
			}
		}

		public EmailRecipientCollection Cc
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection cc;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					cc = this.message.Cc;
				}
				return cc;
			}
		}

		public EmailRecipientCollection Bcc
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection bcc;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bcc = this.message.Bcc;
				}
				return bcc;
			}
		}

		public EmailRecipientCollection ReplyTo
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection replyTo;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					replyTo = this.message.ReplyTo;
				}
				return replyTo;
			}
		}

		public EmailRecipient DispositionNotificationTo
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient dispositionNotificationTo;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					dispositionNotificationTo = this.message.DispositionNotificationTo;
				}
				return dispositionNotificationTo;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.DispositionNotificationTo = value;
				}
			}
		}

		public EmailRecipient Sender
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient sender;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					sender = this.message.Sender;
				}
				return sender;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Sender");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Sender = value;
				}
			}
		}

		public DateTime Date
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime date;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					date = this.message.Date;
				}
				return date;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Date");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Date = value;
				}
			}
		}

		public DateTime Expires
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime expires;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					expires = this.message.Expires;
				}
				return expires;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Expires");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Expires = value;
				}
			}
		}

		public DateTime ReplyBy
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime replyBy;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					replyBy = this.message.ReplyBy;
				}
				return replyBy;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_ReplyBy");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.ReplyBy = value;
				}
			}
		}

		public string Subject
		{
			get
			{
				this.ThrowIfDisposed();
				string subject;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					subject = this.message.Subject;
				}
				return subject;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Subject");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Subject = value;
				}
			}
		}

		public string MessageId
		{
			get
			{
				this.ThrowIfDisposed();
				string messageId;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					messageId = this.message.MessageId;
				}
				return messageId;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_MessageId");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.MessageId = value;
				}
			}
		}

		public Importance Importance
		{
			get
			{
				this.ThrowIfDisposed();
				Importance importance;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					importance = this.message.Importance;
				}
				return importance;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Importance");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Importance = value;
				}
			}
		}

		public Priority Priority
		{
			get
			{
				this.ThrowIfDisposed();
				Priority priority;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					priority = this.message.Priority;
				}
				return priority;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Priority");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Priority = value;
				}
			}
		}

		public Sensitivity Sensitivity
		{
			get
			{
				this.ThrowIfDisposed();
				Sensitivity sensitivity;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					sensitivity = this.message.Sensitivity;
				}
				return sensitivity;
			}
			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfReadOnly("set_Sensitivity");
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.message.Sensitivity = value;
				}
			}
		}

		public string MapiMessageClass
		{
			get
			{
				this.ThrowIfDisposed();
				string mapiMessageClass;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					mapiMessageClass = this.message.MapiMessageClass;
				}
				return mapiMessageClass;
			}
		}

		public MimeDocument MimeDocument
		{
			get
			{
				this.ThrowIfDisposed();
				MimeDocument mimeDocument;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					mimeDocument = this.message.MimeDocument;
				}
				return mimeDocument;
			}
		}

		public MimePart RootPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart rootPart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					rootPart = this.message.RootPart;
				}
				return rootPart;
			}
		}

		public MimePart CalendarPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart calendarPart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					calendarPart = this.message.CalendarPart;
				}
				return calendarPart;
			}
		}

		public MimePart TnefPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart tnefPart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					tnefPart = this.message.TnefPart;
				}
				return tnefPart;
			}
		}

		public Body Body
		{
			get
			{
				this.ThrowIfDisposed();
				Body result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					if (this.body == null)
					{
						this.body = new Body(this);
					}
					result = this.body;
				}
				return result;
			}
		}

		public AttachmentCollection Attachments
		{
			get
			{
				this.ThrowIfDisposed();
				AttachmentCollection result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					this.Synchronize();
					if (this.attachmentCollection == null)
					{
						this.attachmentCollection = new AttachmentCollection(this);
					}
					result = this.attachmentCollection;
				}
				return result;
			}
		}

		private void Synchronize()
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.message.Synchronize();
				if (this.version != this.message.Version)
				{
					this.version = this.message.Version;
					if (this.attachmentCollection != null)
					{
						this.attachmentCollection.InvalidateEnumerators();
					}
				}
			}
		}

		public bool IsInterpersonalMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool isInterpersonalMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					isInterpersonalMessage = this.message.IsInterpersonalMessage;
				}
				return isInterpersonalMessage;
			}
		}

		public bool IsSystemMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool isSystemMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					isSystemMessage = this.message.IsSystemMessage;
				}
				return isSystemMessage;
			}
		}

		public bool IsOpaqueMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool isOpaqueMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					isOpaqueMessage = this.message.IsOpaqueMessage;
				}
				return isOpaqueMessage;
			}
		}

		public MessageSecurityType MessageSecurityType
		{
			get
			{
				this.ThrowIfDisposed();
				MessageSecurityType messageSecurityType;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					messageSecurityType = this.message.MessageSecurityType;
				}
				return messageSecurityType;
			}
		}

		internal EmailRecipientCollection BccFromOrgHeader
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection bccFromOrgHeader;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bccFromOrgHeader = this.message.BccFromOrgHeader;
				}
				return bccFromOrgHeader;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeDocument mimeDocument = this.MimeDocument;
					if (mimeDocument != null)
					{
						result = mimeDocument.IsReadOnly;
					}
					else
					{
						MimePart mimePart = this.RootPart;
						MimePart mimePart2 = mimePart;
						while (mimePart != null)
						{
							mimePart2 = mimePart;
							mimePart = (mimePart.Parent as MimePart);
						}
						if (mimePart2 != null)
						{
							mimeDocument = mimePart2.ParentDocument;
							if (mimeDocument != null)
							{
								return mimeDocument.IsReadOnly;
							}
						}
						result = false;
					}
				}
				return result;
			}
		}

		internal void SetReadOnly(bool makeReadOnly)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeDocument mimeDocument = this.MimeDocument;
				if (mimeDocument == null)
				{
					throw new InvalidOperationException("An EmailMessage must be built on a MimeDocument in order to be read-only.");
				}
				if (makeReadOnly != mimeDocument.IsReadOnly)
				{
					this.SetReadOnly(mimeDocument, makeReadOnly, false);
					mimeDocument.SetReadOnlyInternal(makeReadOnly);
				}
			}
		}

		private void SetReadOnly(MimeDocument parentDocument, bool makeReadOnly, bool isEmbedded)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (makeReadOnly)
				{
					if (this.MimeDocument != null)
					{
						this.MimeDocument.CompleteParse();
					}
					else if (this.RootPart != null)
					{
						parentDocument.BuildDomAndCompleteParse(this.RootPart);
					}
					List<EmailMessage> list = new List<EmailMessage>(this.Attachments.Count);
					foreach (Attachment attachment in this.Attachments)
					{
						if (attachment.IsEmbeddedMessage)
						{
							EmailMessage embeddedMessage = attachment.EmbeddedMessage;
							embeddedMessage.SetReadOnly(parentDocument, makeReadOnly, true);
							list.Add(embeddedMessage);
						}
					}
					if (list.Count > 0)
					{
						this.readOnlyEmbeddedMessages = list;
					}
					string mapiMessageClass = this.MapiMessageClass;
					int num = this.To.Count;
					num += this.Cc.Count;
					num += this.Bcc.Count;
					num += this.ReplyTo.Count;
					Body body = this.Body;
					AttachmentCollection attachments = this.Attachments;
					this.Synchronize();
				}
				else
				{
					this.readOnlyEmbeddedMessages = null;
				}
				this.message.SetReadOnly(makeReadOnly);
			}
		}

		internal static bool IsDocumentReadOnly(MimeDocument document)
		{
			return document.IsReadOnly;
		}

		internal static void SetDocumentReadOnly(MimeDocument document, bool makeReadOnly)
		{
			document.SetReadOnly(makeReadOnly);
		}

		internal PureTnefMessage PureTnefMessage
		{
			get
			{
				this.ThrowIfDisposed();
				PureTnefMessage result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					PureTnefMessage pureTnefMessage = this.message as PureTnefMessage;
					result = pureTnefMessage;
				}
				return result;
			}
		}

		internal IMapiPropertyAccess MapiProperties
		{
			get
			{
				this.ThrowIfDisposed();
				IMapiPropertyAccess mapiProperties;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					mapiProperties = this.message.MapiProperties;
				}
				return mapiProperties;
			}
		}

		internal bool IsPublicFolderReplicationMessage
		{
			get
			{
				this.ThrowIfDisposed();
				bool isPublicFolderReplicationMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					isPublicFolderReplicationMessage = this.message.IsPublicFolderReplicationMessage;
				}
				return isPublicFolderReplicationMessage;
			}
		}

		internal BodyStructure BodyStructure
		{
			get
			{
				this.ThrowIfDisposed();
				BodyStructure result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					PureMimeMessage pureMimeMessage = (this.message as MimeTnefMessage).PureMimeMessage;
					if (pureMimeMessage == null)
					{
						result = BodyStructure.Undefined;
					}
					else
					{
						result = pureMimeMessage.BodyStructure;
					}
				}
				return result;
			}
		}

		public void Normalize()
		{
			this.Normalize(false);
		}

		public void Normalize(bool allowUTF8)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Normalize");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.attachmentCollection != null)
				{
					this.attachmentCollection.InvalidateEnumerators();
				}
				this.message.Normalize(allowUTF8);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ContentManager != null)
				{
					this.ContentManager.Dispose();
					this.ContentManager = null;
				}
				if (this.message != null)
				{
					this.message.Dispose();
					this.message = null;
				}
			}
		}

		internal void Normalize(NormalizeOptions normalizeOptions, bool allowUTF8 = false)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Normalize");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Normalize(normalizeOptions, allowUTF8);
			}
		}

		internal object GetMapiProperty(TnefPropertyTag tag)
		{
			this.ThrowIfDisposed();
			object mapiProperty;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				mapiProperty = this.message.GetMapiProperty(tag);
			}
			return mapiProperty;
		}

		internal bool TryGetMapiProperty<T>(TnefPropertyTag propertyTag, out T propValue)
		{
			this.ThrowIfDisposed();
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				propValue = default(T);
				if (this.message.MapiProperties == null)
				{
					result = false;
				}
				else
				{
					object property = this.message.MapiProperties.GetProperty(propertyTag);
					if (property is T)
					{
						propValue = (T)((object)property);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		internal void CopyTo(EmailMessage destination)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				using (ThreadAccessGuard.EnterPublic(destination.accessToken))
				{
					this.message.CopyTo(destination.message);
				}
			}
		}

		internal Charset TnefTextCharset
		{
			get
			{
				Charset textCharset;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					PureTnefMessage pureTnefMessage = this.message as PureTnefMessage;
					if (pureTnefMessage == null)
					{
						MimeTnefMessage mimeTnefMessage = this.message as MimeTnefMessage;
						if (mimeTnefMessage != null)
						{
							pureTnefMessage = mimeTnefMessage.PureTnefMessage;
						}
					}
					if (pureTnefMessage == null)
					{
						throw new NotSupportedException("Message does not contain TNEF data");
					}
					textCharset = pureTnefMessage.TextCharset;
				}
				return textCharset;
			}
		}

		internal bool TryGetTnefBinaryCharset(out Charset charset)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				PureTnefMessage pureTnefMessage = this.message as PureTnefMessage;
				if (pureTnefMessage == null)
				{
					MimeTnefMessage mimeTnefMessage = this.message as MimeTnefMessage;
					if (mimeTnefMessage != null && mimeTnefMessage.HasTnef)
					{
						pureTnefMessage = mimeTnefMessage.PureTnefMessage;
					}
				}
				if (pureTnefMessage != null)
				{
					charset = pureTnefMessage.BinaryCharset;
					result = true;
				}
				else
				{
					charset = null;
					result = false;
				}
			}
			return result;
		}

		internal BodyFormat Body_GetBodyFormat()
		{
			this.ThrowIfDisposed();
			BodyFormat bodyFormat;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				bodyFormat = body.GetBodyFormat();
			}
			return bodyFormat;
		}

		internal bool Body_ConversionNeeded(int[] validCodepages)
		{
			IBody body = this.message.GetBody();
			return body.ConversionNeeded(validCodepages);
		}

		internal string Body_GetCharsetName()
		{
			this.ThrowIfDisposed();
			string charsetName;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				charsetName = body.GetCharsetName();
			}
			return charsetName;
		}

		internal MimePart Body_GetMimePart()
		{
			this.ThrowIfDisposed();
			MimePart mimePart;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				mimePart = body.GetMimePart();
			}
			return mimePart;
		}

		internal Stream Body_GetContentReadStream()
		{
			this.ThrowIfDisposed();
			Stream contentReadStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				contentReadStream = body.GetContentReadStream();
			}
			return contentReadStream;
		}

		internal bool Body_TryGetContentReadStream(out Stream stream)
		{
			this.ThrowIfDisposed();
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				result = body.TryGetContentReadStream(out stream);
			}
			return result;
		}

		internal Stream Body_GetContentWriteStream(Charset charset)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Body_GetContentWriteStream");
			Stream contentWriteStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.message.GetBody();
				contentWriteStream = body.GetContentWriteStream(charset);
			}
			return contentWriteStream;
		}

		internal AttachmentCookie AttachmentCollection_AddAttachment(Attachment attachment)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("AttachmentCollection_AddAttachment");
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.AttachmentCollection_AddAttachment(attachment);
			}
			return result;
		}

		internal bool AttachmentCollection_RemoveAttachment(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("AttachmentCollection_RemoveAttachment");
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.AttachmentCollection_RemoveAttachment(cookie);
			}
			return result;
		}

		internal void AttachmentCollection_ClearAttachments()
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("AttachmentCollection_ClearAttachment");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.AttachmentCollection_ClearAttachments();
			}
		}

		internal int AttachmentCollection_Count()
		{
			this.ThrowIfDisposed();
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.AttachmentCollection_Count();
			}
			return result;
		}

		internal object AttachmentCollection_Indexer(int publicIndex)
		{
			this.ThrowIfDisposed();
			object result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.AttachmentCollection_Indexer(publicIndex);
			}
			return result;
		}

		internal AttachmentCookie AttachmentCollection_CacheAttachment(int publicIndex, object attachment)
		{
			this.ThrowIfDisposed();
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.AttachmentCollection_CacheAttachment(publicIndex, attachment);
			}
			return result;
		}

		internal string Attachment_GetContentType(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetContentType(cookie);
			}
			return result;
		}

		internal void Attachment_SetContentType(AttachmentCookie cookie, string contentType)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_SetContentType");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_SetContentType(cookie, contentType);
			}
		}

		internal AttachmentMethod Attachment_GetAttachmentMethod(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			AttachmentMethod result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachmentMethod(cookie);
			}
			return result;
		}

		internal InternalAttachmentType Attachment_GetAttachmentType(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			InternalAttachmentType result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachmentType(cookie);
			}
			return result;
		}

		internal void Attachment_SetAttachmentType(AttachmentCookie cookie, InternalAttachmentType attachmentType)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_SetAttachmentType");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_SetAttachmentType(cookie, attachmentType);
			}
		}

		internal EmailMessage Attachment_GetEmbeddedMessage(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			EmailMessage result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetEmbeddedMessage(cookie);
			}
			return result;
		}

		internal void Attachment_SetEmbeddedMessage(AttachmentCookie cookie, EmailMessage embeddedMessage)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_SetEmbeddedMessage");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_SetEmbeddedMessage(cookie, embeddedMessage);
			}
		}

		internal MimePart Attachment_GetMimePart(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			MimePart result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetMimePart(cookie);
			}
			return result;
		}

		internal string Attachment_GetFileName(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetFileName(cookie, ref this.sequenceNumber);
			}
			return result;
		}

		internal void Attachment_SetFileName(AttachmentCookie cookie, string fileName)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_SetFileName");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_SetFileName(cookie, fileName);
			}
		}

		internal string Attachment_GetContentDisposition(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetContentDisposition(cookie);
			}
			return result;
		}

		internal void Attachment_SetContentDisposition(AttachmentCookie cookie, string contentDisposition)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_SetContentDisposition");
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_SetContentDisposition(cookie, contentDisposition);
			}
		}

		internal bool Attachment_IsAppleDouble(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_IsAppleDouble(cookie);
			}
			return result;
		}

		internal Stream Attachment_GetContentReadStream(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetContentReadStream(cookie);
			}
			return result;
		}

		internal bool Attachment_TryGetContentReadStream(AttachmentCookie cookie, out Stream result)
		{
			this.ThrowIfDisposed();
			bool result2;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result2 = this.message.Attachment_TryGetContentReadStream(cookie, out result);
			}
			return result2;
		}

		internal Stream Attachment_GetContentWriteStream(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			this.ThrowIfReadOnly("Attachment_GetContentWriteStream");
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetContentWriteStream(cookie);
			}
			return result;
		}

		internal int Attachment_GetRenderingPosition(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetRenderingPosition(cookie);
			}
			return result;
		}

		internal string Attachment_GetAttachContentID(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachContentID(cookie);
			}
			return result;
		}

		internal string Attachment_GetAttachContentLocation(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachContentLocation(cookie);
			}
			return result;
		}

		internal byte[] Attachment_GetAttachRendering(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			byte[] result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachRendering(cookie);
			}
			return result;
		}

		internal int Attachment_GetAttachmentFlags(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachmentFlags(cookie);
			}
			return result;
		}

		internal bool Attachment_GetAttachHidden(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetAttachHidden(cookie);
			}
			return result;
		}

		internal int Attachment_GetHashCode(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.message.Attachment_GetHashCode(cookie);
			}
			return result;
		}

		internal void Attachment_Dispose(AttachmentCookie cookie)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.message.Attachment_Dispose(cookie);
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.message == null)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
		}

		private void ThrowIfReadOnly(string method)
		{
			if (this.IsReadOnly)
			{
				throw new ReadOnlyMimeException(method);
			}
		}

		private EmailMessage.EmailMessageThreadAccessToken accessToken;

		internal static bool TestabilityEnableBetterFuzzing;

		private MessageImplementation message;

		private Body body;

		private AttachmentCollection attachmentCollection;

		private List<EmailMessage> readOnlyEmbeddedMessages;

		private int version = -1;

		private int sequenceNumber;

		internal IDisposable ContentManager;

		private class EmailMessageThreadAccessToken : ObjectThreadAccessToken
		{
			internal EmailMessageThreadAccessToken(EmailMessage parent)
			{
			}
		}
	}
}
