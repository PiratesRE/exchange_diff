using System;
using System.IO;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class MimeTnefMessage : MessageImplementation, IBody
	{
		internal MimeTnefMessage(BodyFormat bodyFormat, bool createAlternative, string charsetName)
		{
			this.mimeMessage = new PureMimeMessage(bodyFormat, createAlternative, charsetName);
			this.accessToken = new MimeTnefMessage.MimeTnefMessageThreadAccessToken(this);
		}

		internal MimeTnefMessage(MimeDocument mimeDocument)
		{
			this.mimeMessage = new PureMimeMessage(mimeDocument);
			bool hasTnef = this.HasTnef;
			if (mimeDocument.IsReadOnly)
			{
				this.Synchronize();
			}
			this.accessToken = new MimeTnefMessage.MimeTnefMessageThreadAccessToken(this);
		}

		internal MimeTnefMessage(MimePart rootPart)
		{
			this.mimeMessage = new PureMimeMessage(rootPart);
			bool hasTnef = this.HasTnef;
			this.Synchronize();
			this.accessToken = new MimeTnefMessage.MimeTnefMessageThreadAccessToken(this);
		}

		internal override ObjectThreadAccessToken AccessToken
		{
			get
			{
				return this.accessToken;
			}
		}

		public override EmailRecipient From
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient from;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					from = this.mimeMessage.From;
				}
				return from;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.From = value;
					this.AdjustVersions(versions);
				}
			}
		}

		public override EmailRecipientCollection To
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection to;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					to = this.mimeMessage.To;
				}
				return to;
			}
		}

		public override EmailRecipientCollection Cc
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection cc;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					cc = this.mimeMessage.Cc;
				}
				return cc;
			}
		}

		public override EmailRecipientCollection Bcc
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection bcc;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bcc = this.mimeMessage.Bcc;
				}
				return bcc;
			}
		}

		public override EmailRecipientCollection ReplyTo
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection replyTo;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					replyTo = this.mimeMessage.ReplyTo;
				}
				return replyTo;
			}
		}

		public override EmailRecipient DispositionNotificationTo
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient dispositionNotificationTo;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					dispositionNotificationTo = this.mimeMessage.DispositionNotificationTo;
				}
				return dispositionNotificationTo;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.DispositionNotificationTo = value;
					this.AdjustVersions(versions);
				}
			}
		}

		public override EmailRecipient Sender
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipient sender;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					sender = this.mimeMessage.Sender;
				}
				return sender;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Sender = value;
					this.AdjustVersions(versions);
				}
			}
		}

		public override DateTime Date
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime date;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						DateTime? property = this.tnefMessage.GetProperty<DateTime>(TnefPropertyTag.ClientSubmitTime);
						if (property != null)
						{
							return property.Value;
						}
					}
					date = this.mimeMessage.Date;
				}
				return date;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Date = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Date = ((DateTime.MinValue != value) ? value.ToUniversalTime() : value);
					}
				}
			}
		}

		public override DateTime Expires
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime expires;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						DateTime? property = this.tnefMessage.GetProperty<DateTime>(TnefPropertyTag.ExpiryTime);
						if (property != null)
						{
							return property.Value;
						}
					}
					expires = this.mimeMessage.Expires;
				}
				return expires;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Expires = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Expires = ((DateTime.MinValue != value) ? value.ToUniversalTime() : value);
					}
				}
			}
		}

		public override DateTime ReplyBy
		{
			get
			{
				this.ThrowIfDisposed();
				DateTime replyBy;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						DateTime? property = this.tnefMessage.GetProperty<DateTime>(TnefPropertyTag.ReplyTime);
						if (property != null)
						{
							return property.Value;
						}
					}
					replyBy = this.mimeMessage.ReplyBy;
				}
				return replyBy;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.ReplyBy = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.ReplyBy = ((DateTime.MinValue != value) ? value.ToUniversalTime() : value);
					}
				}
			}
		}

		public override string Subject
		{
			get
			{
				this.ThrowIfDisposed();
				string subject2;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						string subject = this.tnefMessage.Subject;
						if (!string.IsNullOrEmpty(subject))
						{
							return subject;
						}
					}
					subject2 = this.mimeMessage.Subject;
				}
				return subject2;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Subject = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Subject = value;
					}
				}
			}
		}

		public override string MessageId
		{
			get
			{
				this.ThrowIfDisposed();
				string messageId2;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						string messageId = this.tnefMessage.MessageId;
						if (!string.IsNullOrEmpty(messageId))
						{
							return messageId;
						}
					}
					messageId2 = this.mimeMessage.MessageId;
				}
				return messageId2;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.MessageId = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.MessageId = value;
					}
				}
			}
		}

		public override Importance Importance
		{
			get
			{
				this.ThrowIfDisposed();
				Importance result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					Importance importance;
					if (this.HasTnef && this.tnefMessage.TryGetImportance(out importance))
					{
						result = importance;
					}
					else
					{
						result = this.mimeMessage.Importance;
					}
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Importance = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Importance = value;
					}
				}
			}
		}

		public override Priority Priority
		{
			get
			{
				this.ThrowIfDisposed();
				Priority result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					Priority priority;
					if (this.HasTnef && this.tnefMessage.TryGetPriority(out priority))
					{
						result = priority;
					}
					else
					{
						result = this.mimeMessage.Priority;
					}
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Priority = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Priority = value;
					}
				}
			}
		}

		public override Sensitivity Sensitivity
		{
			get
			{
				this.ThrowIfDisposed();
				Sensitivity result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					Sensitivity sensitivity;
					if (this.HasTnef && this.tnefMessage.TryGetSensitivity(out sensitivity))
					{
						result = sensitivity;
					}
					else
					{
						result = this.mimeMessage.Sensitivity;
					}
				}
				return result;
			}
			set
			{
				this.ThrowIfDisposed();
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Sensitivity = value;
					this.AdjustVersions(versions);
					if (this.HasTnef)
					{
						this.tnefMessage.Sensitivity = value;
					}
				}
			}
		}

		public override string MapiMessageClass
		{
			get
			{
				this.ThrowIfDisposed();
				string mapiMessageClass;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						mapiMessageClass = this.tnefMessage.MapiMessageClass;
					}
					else
					{
						mapiMessageClass = this.mimeMessage.MapiMessageClass;
					}
				}
				return mapiMessageClass;
			}
		}

		public override MimeDocument MimeDocument
		{
			get
			{
				this.ThrowIfDisposed();
				MimeDocument result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = ((this.mimeMessage == null) ? null : this.mimeMessage.MimeDocument);
				}
				return result;
			}
		}

		public override MimePart RootPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart rootPart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					rootPart = this.mimeMessage.RootPart;
				}
				return rootPart;
			}
		}

		public override MimePart CalendarPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart calendarPart;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					calendarPart = this.mimeMessage.CalendarPart;
				}
				return calendarPart;
			}
		}

		public override MimePart TnefPart
		{
			get
			{
				this.ThrowIfDisposed();
				MimePart result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (!this.HasTnef)
					{
						result = null;
					}
					else
					{
						result = this.tnefPart;
					}
				}
				return result;
			}
		}

		internal override void Synchronize()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.mimeMessage.Synchronize();
				this.TnefCheck();
			}
		}

		internal override int Version
		{
			get
			{
				return this.mimeMessage.Version;
			}
		}

		internal override EmailRecipientCollection BccFromOrgHeader
		{
			get
			{
				this.ThrowIfDisposed();
				EmailRecipientCollection bccFromOrgHeader;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bccFromOrgHeader = this.mimeMessage.BccFromOrgHeader;
				}
				return bccFromOrgHeader;
			}
		}

		public override bool IsInterpersonalMessage
		{
			get
			{
				bool isInterpersonalMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						isInterpersonalMessage = this.tnefMessage.IsInterpersonalMessage;
					}
					else
					{
						isInterpersonalMessage = this.mimeMessage.IsInterpersonalMessage;
					}
				}
				return isInterpersonalMessage;
			}
		}

		public override bool IsSystemMessage
		{
			get
			{
				bool isSystemMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						isSystemMessage = this.tnefMessage.IsSystemMessage;
					}
					else
					{
						isSystemMessage = this.mimeMessage.IsSystemMessage;
					}
				}
				return isSystemMessage;
			}
		}

		public override bool IsPublicFolderReplicationMessage
		{
			get
			{
				bool isPublicFolderReplicationMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						isPublicFolderReplicationMessage = this.tnefMessage.IsPublicFolderReplicationMessage;
					}
					else
					{
						isPublicFolderReplicationMessage = this.mimeMessage.IsPublicFolderReplicationMessage;
					}
				}
				return isPublicFolderReplicationMessage;
			}
		}

		public override bool IsOpaqueMessage
		{
			get
			{
				bool isOpaqueMessage;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						isOpaqueMessage = this.tnefMessage.IsOpaqueMessage;
					}
					else
					{
						isOpaqueMessage = this.mimeMessage.IsOpaqueMessage;
					}
				}
				return isOpaqueMessage;
			}
		}

		public override MessageSecurityType MessageSecurityType
		{
			get
			{
				MessageSecurityType messageSecurityType;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					if (this.HasTnef)
					{
						messageSecurityType = this.tnefMessage.MessageSecurityType;
					}
					else
					{
						messageSecurityType = this.mimeMessage.MessageSecurityType;
					}
				}
				return messageSecurityType;
			}
		}

		internal PureMimeMessage PureMimeMessage
		{
			get
			{
				this.ThrowIfDisposed();
				PureMimeMessage result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.mimeMessage;
				}
				return result;
			}
		}

		internal PureTnefMessage PureTnefMessage
		{
			get
			{
				this.ThrowIfDisposed();
				PureTnefMessage result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.tnefMessage;
				}
				return result;
			}
		}

		internal bool HasTnef
		{
			get
			{
				this.ThrowIfDisposed();
				bool result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					bool flag = this.TnefCheck();
					if (!flag && this.tnefMessage != null)
					{
						this.tnefMessage.Dispose();
						this.tnefMessage = null;
					}
					result = flag;
				}
				return result;
			}
		}

		internal override void Dispose(bool disposing)
		{
			if (disposing && this.mimeMessage != null)
			{
				this.mimeMessage.Dispose(disposing);
				this.mimeMessage = null;
				this.DisposeTnef();
			}
			base.Dispose(disposing);
		}

		public override void Normalize(bool allowUTF8 = false)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				this.mimeMessage.NormalizeStructure(false);
				MimeTnefVersions versions = this.SnapshotVersions();
				this.mimeMessage.Normalize((NormalizeOptions)65534, allowUTF8);
				this.AdjustVersions(versions);
			}
		}

		internal override void Normalize(NormalizeOptions normalizeOptions, bool allowUTF8)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if ((normalizeOptions & NormalizeOptions.NormalizeMimeStructure) != (NormalizeOptions)0)
				{
					this.mimeMessage.NormalizeStructure(false);
					normalizeOptions &= ~NormalizeOptions.NormalizeMimeStructure;
				}
				if ((normalizeOptions & NormalizeOptions.NormalizeMime) != (NormalizeOptions)0)
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					this.mimeMessage.Normalize(normalizeOptions, allowUTF8);
					this.AdjustVersions(versions);
				}
				if ((normalizeOptions & NormalizeOptions.NormalizeTnef) != (NormalizeOptions)0 && this.HasTnef)
				{
					this.tnefMessage.Normalize(normalizeOptions, allowUTF8);
				}
			}
		}

		internal override IMapiPropertyAccess MapiProperties
		{
			get
			{
				IMapiPropertyAccess result;
				using (ThreadAccessGuard.EnterPublic(this.accessToken))
				{
					result = this.tnefMessage;
				}
				return result;
			}
		}

		internal override void AddRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient newRecipient)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				this.mimeMessage.AddRecipient(recipientType, ref cachedHeader, newRecipient);
				this.AdjustVersions(versions);
				if (this.HasTnef)
				{
					this.tnefMessage.AddRecipient(recipientType, ref cachedHeader, newRecipient);
				}
			}
		}

		internal override void RemoveRecipient(RecipientType recipientType, ref object cachedHeader, EmailRecipient oldRecipient)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				this.mimeMessage.RemoveRecipient(recipientType, ref cachedHeader, oldRecipient);
				this.AdjustVersions(versions);
				if (this.HasTnef)
				{
					this.tnefMessage.RemoveRecipient(recipientType, ref cachedHeader, oldRecipient);
				}
			}
		}

		internal override void ClearRecipients(RecipientType recipientType, ref object cachedHeader)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				this.mimeMessage.ClearRecipients(recipientType, ref cachedHeader);
				this.AdjustVersions(versions);
				if (this.HasTnef)
				{
					this.tnefMessage.ClearRecipients(recipientType, ref cachedHeader);
				}
			}
		}

		internal override IBody GetBody()
		{
			return this;
		}

		internal IBody InternalGetBody()
		{
			IBody result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body;
				if (this.HasTnef)
				{
					body = this.tnefMessage;
					BodyFormat bodyFormat = body.GetBodyFormat();
					if (bodyFormat != BodyFormat.None)
					{
						return body;
					}
				}
				body = this.mimeMessage.GetBody();
				result = body;
			}
			return result;
		}

		bool IBody.ConversionNeeded(int[] validCodepages)
		{
			return this.InternalGetBody().ConversionNeeded(validCodepages);
		}

		BodyFormat IBody.GetBodyFormat()
		{
			BodyFormat bodyFormat;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bodyFormat = this.InternalGetBody().GetBodyFormat();
			}
			return bodyFormat;
		}

		string IBody.GetCharsetName()
		{
			string charsetName;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				charsetName = this.InternalGetBody().GetCharsetName();
			}
			return charsetName;
		}

		MimePart IBody.GetMimePart()
		{
			MimePart mimePart;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				mimePart = this.InternalGetBody().GetMimePart();
			}
			return mimePart;
		}

		Stream IBody.GetContentReadStream()
		{
			Stream contentReadStream;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				contentReadStream = this.InternalGetBody().GetContentReadStream();
			}
			return contentReadStream;
		}

		bool IBody.TryGetContentReadStream(out Stream stream)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = this.InternalGetBody().TryGetContentReadStream(out stream);
			}
			return result;
		}

		Stream IBody.GetContentWriteStream(Charset charset)
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				IBody body = this.InternalGetBody();
				MimeTnefVersions versions = this.SnapshotVersions();
				Stream contentWriteStream = body.GetContentWriteStream(charset);
				this.AdjustVersions(versions);
				result = contentWriteStream;
			}
			return result;
		}

		void IBody.SetNewContent(DataStorage storage, long start, long end)
		{
		}

		internal override AttachmentCookie AttachmentCollection_AddAttachment(Attachment attachment)
		{
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				AttachmentCookie attachmentCookie;
				if (this.HasTnef)
				{
					attachmentCookie = this.tnefMessage.AttachmentCollection_AddAttachment(attachment);
				}
				else
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					attachmentCookie = this.mimeMessage.AttachmentCollection_AddAttachment(attachment);
					this.AdjustVersions(versions);
				}
				result = attachmentCookie;
			}
			return result;
		}

		internal override bool AttachmentCollection_RemoveAttachment(AttachmentCookie cookie)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				bool flag;
				if (cookie.MessageImplementation is PureTnefMessage)
				{
					flag = this.tnefMessage.AttachmentCollection_RemoveAttachment(cookie);
				}
				else
				{
					MimeTnefVersions versions = this.SnapshotVersions();
					flag = this.mimeMessage.AttachmentCollection_RemoveAttachment(cookie);
					this.AdjustVersions(versions);
				}
				result = flag;
			}
			return result;
		}

		internal override void AttachmentCollection_ClearAttachments()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				this.mimeMessage.AttachmentCollection_ClearAttachments();
				this.AdjustVersions(versions);
				if (this.HasTnef)
				{
					this.tnefMessage.AttachmentCollection_ClearAttachments();
				}
			}
		}

		internal override int AttachmentCollection_Count()
		{
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				int num = this.mimeMessage.AttachmentCollection_Count();
				if (this.HasTnef)
				{
					int num2 = this.tnefMessage.AttachmentCollection_Count();
					num += num2;
				}
				result = num;
			}
			return result;
		}

		internal override object AttachmentCollection_Indexer(int publicIndex)
		{
			object result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				int num = this.mimeMessage.AttachmentCollection_Count();
				if (publicIndex < 0)
				{
					result = null;
				}
				else if (publicIndex < num)
				{
					object obj = this.mimeMessage.AttachmentCollection_Indexer(publicIndex);
					result = obj;
				}
				else
				{
					int num2 = this.tnefMessage.AttachmentCollection_Count();
					publicIndex -= num;
					object obj2 = this.tnefMessage.AttachmentCollection_Indexer(publicIndex);
					result = obj2;
				}
			}
			return result;
		}

		internal override AttachmentCookie AttachmentCollection_CacheAttachment(int publicIndex, object attachment)
		{
			AttachmentCookie result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				int num = this.mimeMessage.AttachmentCollection_Count();
				if (publicIndex < 0)
				{
					result = new AttachmentCookie(0, null);
				}
				else if (publicIndex < num)
				{
					AttachmentCookie attachmentCookie = this.mimeMessage.AttachmentCollection_CacheAttachment(publicIndex, attachment);
					result = attachmentCookie;
				}
				else
				{
					int num2 = this.tnefMessage.AttachmentCollection_Count();
					publicIndex -= num;
					if (publicIndex >= num2)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					AttachmentCookie attachmentCookie2 = this.tnefMessage.AttachmentCollection_CacheAttachment(publicIndex, attachment);
					result = attachmentCookie2;
				}
			}
			return result;
		}

		internal override string Attachment_GetContentType(AttachmentCookie cookie)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetContentType(cookie);
			}
			return result;
		}

		internal override void Attachment_SetContentType(AttachmentCookie cookie, string contentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				cookie.MessageImplementation.Attachment_SetContentType(cookie, contentType);
				this.AdjustVersions(versions);
			}
		}

		internal override AttachmentMethod Attachment_GetAttachmentMethod(AttachmentCookie cookie)
		{
			AttachmentMethod result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachmentMethod(cookie);
			}
			return result;
		}

		internal override InternalAttachmentType Attachment_GetAttachmentType(AttachmentCookie cookie)
		{
			InternalAttachmentType result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachmentType(cookie);
			}
			return result;
		}

		internal override void Attachment_SetAttachmentType(AttachmentCookie cookie, InternalAttachmentType attachmentType)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				cookie.MessageImplementation.Attachment_SetAttachmentType(cookie, attachmentType);
			}
		}

		internal override EmailMessage Attachment_GetEmbeddedMessage(AttachmentCookie cookie)
		{
			EmailMessage result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetEmbeddedMessage(cookie);
			}
			return result;
		}

		internal override void Attachment_SetEmbeddedMessage(AttachmentCookie cookie, EmailMessage embeddedMessage)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				cookie.MessageImplementation.Attachment_SetEmbeddedMessage(cookie, embeddedMessage);
			}
		}

		internal override MimePart Attachment_GetMimePart(AttachmentCookie cookie)
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetMimePart(cookie);
			}
			return result;
		}

		internal override string Attachment_GetFileName(AttachmentCookie cookie, ref int sequenceNumber)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetFileName(cookie, ref sequenceNumber);
			}
			return result;
		}

		internal override void Attachment_SetFileName(AttachmentCookie cookie, string fileName)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				cookie.MessageImplementation.Attachment_SetFileName(cookie, fileName);
				this.AdjustVersions(versions);
			}
		}

		internal override string Attachment_GetContentDisposition(AttachmentCookie cookie)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetContentDisposition(cookie);
			}
			return result;
		}

		internal override void Attachment_SetContentDisposition(AttachmentCookie cookie, string value)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				cookie.MessageImplementation.Attachment_SetContentDisposition(cookie, value);
				this.AdjustVersions(versions);
			}
		}

		internal override bool Attachment_IsAppleDouble(AttachmentCookie cookie)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_IsAppleDouble(cookie);
			}
			return result;
		}

		internal override Stream Attachment_GetContentReadStream(AttachmentCookie cookie)
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetContentReadStream(cookie);
			}
			return result;
		}

		internal override bool Attachment_TryGetContentReadStream(AttachmentCookie cookie, out Stream result)
		{
			bool result2;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result2 = cookie.MessageImplementation.Attachment_TryGetContentReadStream(cookie, out result);
			}
			return result2;
		}

		internal override Stream Attachment_GetContentWriteStream(AttachmentCookie cookie)
		{
			Stream result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefVersions versions = this.SnapshotVersions();
				Stream stream = cookie.MessageImplementation.Attachment_GetContentWriteStream(cookie);
				this.AdjustVersions(versions);
				result = stream;
			}
			return result;
		}

		internal override int Attachment_GetRenderingPosition(AttachmentCookie cookie)
		{
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetRenderingPosition(cookie);
			}
			return result;
		}

		internal override string Attachment_GetAttachContentID(AttachmentCookie cookie)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachContentID(cookie);
			}
			return result;
		}

		internal override string Attachment_GetAttachContentLocation(AttachmentCookie cookie)
		{
			string result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachContentLocation(cookie);
			}
			return result;
		}

		internal override byte[] Attachment_GetAttachRendering(AttachmentCookie cookie)
		{
			byte[] result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachRendering(cookie);
			}
			return result;
		}

		internal override int Attachment_GetAttachmentFlags(AttachmentCookie cookie)
		{
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachmentFlags(cookie);
			}
			return result;
		}

		internal override bool Attachment_GetAttachHidden(AttachmentCookie cookie)
		{
			bool result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetAttachHidden(cookie);
			}
			return result;
		}

		internal override int Attachment_GetHashCode(AttachmentCookie cookie)
		{
			int result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				result = cookie.MessageImplementation.Attachment_GetHashCode(cookie);
			}
			return result;
		}

		internal override void Attachment_Dispose(AttachmentCookie cookie)
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				cookie.MessageImplementation.Attachment_Dispose(cookie);
			}
		}

		internal override void SetReadOnly(bool makeReadOnly)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.tnefRelayStorage != null)
				{
					this.tnefRelayStorage.SetReadOnly(makeReadOnly);
				}
			}
		}

		internal override void CopyTo(MessageImplementation destination)
		{
			this.ThrowIfDisposed();
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimeTnefMessage mimeTnefMessage = (MimeTnefMessage)destination;
				using (ThreadAccessGuard.EnterPublic(mimeTnefMessage.accessToken))
				{
					if (mimeTnefMessage != this)
					{
						base.CopyTo(mimeTnefMessage);
						this.mimeMessage.CopyTo(mimeTnefMessage.mimeMessage);
						if (mimeTnefMessage.HasTnef)
						{
							mimeTnefMessage.DisposeTnef();
						}
						mimeTnefMessage.tnefCheckRootPartVersion = -1;
						mimeTnefMessage.tnefCheckTnefPartVersion = -1;
						mimeTnefMessage.tnefState = MimeTnefMessage.TnefState.NoTnef;
						mimeTnefMessage.tnefPart = null;
					}
				}
			}
		}

		internal void InvalidateTnefContent()
		{
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				if (this.tnefRelayStorage == null)
				{
					this.tnefRelayStorage = new RelayStorage(this.tnefMessage);
				}
				else
				{
					this.tnefRelayStorage.Invalidate();
				}
				this.SetTnefPartContent(this.tnefRelayStorage, 0L, long.MaxValue);
			}
		}

		internal Charset GetMessageCharsetFromMime()
		{
			Charset result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				string name = null;
				Charset charset = null;
				if (this.mimeMessage.RootPart.ContentType == "multipart/mixed")
				{
					MimePart mimePart = this.mimeMessage.RootPart.FirstChild as MimePart;
					if (mimePart != null && mimePart.ContentType == "text/plain")
					{
						name = Utility.GetParameterValue(mimePart, HeaderId.ContentType, "charset");
						charset = (Charset.TryGetCharset(name, out charset) ? charset : Charset.DefaultMimeCharset);
						return charset;
					}
				}
				foreach (Header header in this.mimeMessage.RootPart.Headers)
				{
					TextHeader textHeader = header as TextHeader;
					if (textHeader != null)
					{
						if (Utility.Get2047CharsetName(textHeader, out name))
						{
							charset = (Charset.TryGetCharset(name, out charset) ? charset : Charset.DefaultMimeCharset);
							return charset;
						}
					}
					else
					{
						AddressHeader addressHeader = header as AddressHeader;
						if (addressHeader != null)
						{
							foreach (AddressItem addressItem in addressHeader)
							{
								if (Utility.Get2047CharsetName(addressItem, out name))
								{
									charset = (Charset.TryGetCharset(name, out charset) ? charset : Charset.DefaultMimeCharset);
									return charset;
								}
								MimeGroup mimeGroup = addressItem as MimeGroup;
								if (mimeGroup != null)
								{
									foreach (MimeRecipient addressItem2 in mimeGroup)
									{
										if (Utility.Get2047CharsetName(addressItem2, out name))
										{
											charset = (Charset.TryGetCharset(name, out charset) ? charset : Charset.DefaultMimeCharset);
											return charset;
										}
									}
								}
							}
						}
					}
				}
				if (this.mimeMessage.MimeDocument != null && this.mimeMessage.MimeDocument.EffectiveHeaderDecodingOptions.Charset != null)
				{
					result = this.mimeMessage.MimeDocument.EffectiveHeaderDecodingOptions.Charset;
				}
				else
				{
					result = Charset.DefaultMimeCharset;
				}
			}
			return result;
		}

		internal MimePart GetLegacyPlainTextBody()
		{
			MimePart result;
			using (ThreadAccessGuard.EnterPublic(this.accessToken))
			{
				MimePart mimePart = this.mimeMessage.RootPart.FirstChild as MimePart;
				if (mimePart.ContentType == "text/plain")
				{
					result = mimePart;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private void SetTnefPartContent(DataStorage newContent, long start, long end)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				bool contentDirty = this.tnefPart.ContentDirty;
				MimeTnefVersions versions = this.SnapshotVersions();
				bool isSynchronized = this.mimeMessage.IsSynchronized;
				ContentTransferEncoding contentTransferEncoding = this.tnefPart.ContentTransferEncoding;
				if (ContentTransferEncoding.Binary != contentTransferEncoding && ContentTransferEncoding.Base64 != contentTransferEncoding)
				{
					Header header = this.tnefPart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
					if (header == null)
					{
						header = Header.Create(HeaderId.ContentTransferEncoding);
						this.tnefPart.Headers.AppendChild(header);
					}
					header.Value = "base64";
				}
				this.tnefPart.SetStorage(newContent, start, end);
				if (isSynchronized)
				{
					this.mimeMessage.UpdateMimeVersion();
				}
				this.AdjustVersions(versions);
				this.tnefPart.ContentDirty = contentDirty;
			}
		}

		private bool TnefCheck()
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (this.tnefCheckRootPartVersion != this.mimeMessage.Version)
				{
					MimePart rootPart = this.mimeMessage.RootPart;
					if (rootPart == null)
					{
						result = false;
					}
					else
					{
						this.tnefCheckRootPartVersion = this.mimeMessage.Version;
						if (this.TnefCheck(rootPart, true))
						{
							result = true;
						}
						else if (this.tnefState == MimeTnefMessage.TnefState.Invalid)
						{
							result = false;
						}
						else
						{
							if (rootPart.ContentType == "multipart/mixed")
							{
								for (MimePart mimePart = rootPart.FirstChild as MimePart; mimePart != null; mimePart = (mimePart.NextSibling as MimePart))
								{
									if (this.TnefCheck(mimePart, false))
									{
										return true;
									}
									if (this.tnefState == MimeTnefMessage.TnefState.Invalid)
									{
										return false;
									}
								}
							}
							this.tnefState = MimeTnefMessage.TnefState.NoTnef;
							result = false;
						}
					}
				}
				else
				{
					result = (null != this.tnefMessage);
				}
			}
			return result;
		}

		private bool TnefCheck(MimePart candidate, bool isRoot)
		{
			bool result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				if (candidate == this.tnefPart && candidate.Version == this.tnefCheckTnefPartVersion)
				{
					result = (this.tnefState == MimeTnefMessage.TnefState.Valid);
				}
				else
				{
					ContentTypeHeader contentTypeHeader = candidate.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
					if (contentTypeHeader == null)
					{
						result = false;
					}
					else
					{
						string headerValue = Utility.GetHeaderValue(contentTypeHeader);
						if (string.IsNullOrEmpty(headerValue))
						{
							result = false;
						}
						else
						{
							if (headerValue != "application/ms-tnef")
							{
								if (headerValue != "application/octet-stream" && !headerValue.StartsWith("application/x-openmail", StringComparison.Ordinal))
								{
									return false;
								}
								DecodingOptions decodingOptions = new DecodingOptions((DecodingFlags)131071);
								MimeParameter mimeParameter = contentTypeHeader["name"];
								DecodingResults decodingResults;
								string a;
								if (mimeParameter == null || !mimeParameter.TryGetValue(decodingOptions, out decodingResults, out a) || !string.Equals(a, "winmail.dat", StringComparison.OrdinalIgnoreCase))
								{
									return false;
								}
							}
							if (this.tnefRelayStorage != null)
							{
								if (candidate == this.tnefPart && !candidate.ContentDirty)
								{
									return MimeTnefMessage.TnefState.Valid == this.tnefState;
								}
								this.tnefRelayStorage.PermanentlyRelay();
								this.tnefRelayStorage.Release();
								this.tnefRelayStorage = null;
							}
							if (this.tnefMessage != null)
							{
								this.tnefMessage.Dispose();
								this.tnefMessage = null;
							}
							if (this.tnefPart != null && this.tnefPart != candidate && this.tnefState == MimeTnefMessage.TnefState.Valid)
							{
								this.mimeMessage.SetTnefPart(null);
								this.Normalize(false);
								this.tnefCheckRootPartVersion = this.mimeMessage.Version;
							}
							this.tnefPart = candidate;
							this.tnefCheckTnefPartVersion = candidate.Version;
							Stream stream;
							if (!candidate.TryGetContentReadStream(out stream))
							{
								this.tnefState = MimeTnefMessage.TnefState.Invalid;
								result = false;
							}
							else
							{
								Stream stream2 = stream;
								DataStorage storage;
								long tnefStart;
								long tnefEnd;
								if (candidate.BodyCte == ContentTransferEncoding.Binary)
								{
									storage = candidate.Storage;
									tnefStart = candidate.DataStart + candidate.BodyOffset;
									tnefEnd = candidate.DataEnd;
								}
								else
								{
									ForkToTempStorageReadStream forkToTempStorageReadStream = new ForkToTempStorageReadStream(stream);
									storage = forkToTempStorageReadStream.Storage;
									tnefStart = 0L;
									tnefEnd = long.MaxValue;
									stream2 = forkToTempStorageReadStream;
								}
								if (storage == null)
								{
									this.tnefState = MimeTnefMessage.TnefState.Invalid;
									if (stream2 != null)
									{
										stream2.Dispose();
									}
									result = false;
								}
								else
								{
									PureTnefMessage pureTnefMessage = new PureTnefMessage(this, candidate, storage, tnefStart, tnefEnd);
									if (pureTnefMessage.Load(stream2))
									{
										string correlator = pureTnefMessage.Correlator;
										string tnefCorrelator = this.mimeMessage.TnefCorrelator;
										if (tnefCorrelator == null)
										{
											tnefCorrelator = Utility.GetTnefCorrelator(candidate);
										}
										if (EmailMessage.TestabilityEnableBetterFuzzing || (string.IsNullOrEmpty(tnefCorrelator) && string.IsNullOrEmpty(correlator)) || string.Equals(tnefCorrelator, correlator, StringComparison.OrdinalIgnoreCase))
										{
											this.tnefMessage = pureTnefMessage;
											this.mimeMessage.SetTnefPart(this.tnefPart);
											this.tnefMessage.Stnef = isRoot;
											candidate.ContentDirty = false;
											this.tnefState = MimeTnefMessage.TnefState.Valid;
											return true;
										}
									}
									this.tnefState = MimeTnefMessage.TnefState.Invalid;
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private void DisposeTnef()
		{
			if (this.tnefMessage != null)
			{
				if (this.tnefRelayStorage != null)
				{
					this.tnefRelayStorage.Release();
					this.tnefRelayStorage = null;
				}
				this.tnefMessage.Dispose();
				this.tnefMessage = null;
			}
		}

		private MimeTnefVersions SnapshotVersions()
		{
			MimeTnefVersions result;
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				result = new MimeTnefVersions(this.mimeMessage, this.tnefPart);
			}
			return result;
		}

		private void AdjustVersions(MimeTnefVersions versions)
		{
			using (ThreadAccessGuard.EnterPrivate(this.accessToken))
			{
				this.tnefCheckRootPartVersion += this.mimeMessage.Version - versions.RootPartVersion;
				if (-1 != versions.TnefPartVersion)
				{
					this.tnefCheckTnefPartVersion += this.tnefPart.Version - versions.TnefPartVersion;
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.mimeMessage == null)
			{
				throw new ObjectDisposedException("EmailMessage");
			}
		}

		private MimeTnefMessage.MimeTnefMessageThreadAccessToken accessToken;

		private PureMimeMessage mimeMessage;

		private PureTnefMessage tnefMessage;

		private MimePart tnefPart;

		private RelayStorage tnefRelayStorage;

		private int tnefCheckRootPartVersion = -1;

		private int tnefCheckTnefPartVersion = -1;

		private MimeTnefMessage.TnefState tnefState;

		private class MimeTnefMessageThreadAccessToken : ObjectThreadAccessToken
		{
			internal MimeTnefMessageThreadAccessToken(MimeTnefMessage parent)
			{
			}
		}

		private enum TnefState
		{
			NoTnef,
			Valid,
			Invalid
		}
	}
}
