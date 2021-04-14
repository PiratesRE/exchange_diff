using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	[DebuggerDisplay("Type: {ContentType} Name: {FileName}")]
	public class Attachment
	{
		internal AttachmentCookie Cookie
		{
			get
			{
				if (this.cookie.MessageImplementation == null)
				{
					throw new InvalidOperationException(EmailMessageStrings.AttachmentRemovedFromMessage);
				}
				return this.cookie;
			}
			set
			{
				this.cookie = value;
			}
		}

		internal Attachment(EmailMessage message)
		{
			this.message = message;
		}

		public string ContentType
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetContentType(this.cookie);
			}
			set
			{
				this.ThrowIfInvalid();
				this.message.Attachment_SetContentType(this.cookie, value);
			}
		}

		public bool IsOleAttachment
		{
			get
			{
				this.ThrowIfInvalid();
				return AttachmentMethod.AttachOle == this.message.Attachment_GetAttachmentMethod(this.cookie);
			}
		}

		public AttachmentType AttachmentType
		{
			get
			{
				this.ThrowIfInvalid();
				InternalAttachmentType internalAttachmentType = this.message.Attachment_GetAttachmentType(this.cookie);
				if (internalAttachmentType != InternalAttachmentType.Regular)
				{
					return AttachmentType.Inline;
				}
				return AttachmentType.Regular;
			}
			internal set
			{
				this.ThrowIfInvalid();
				InternalAttachmentType attachmentType = (value == AttachmentType.Regular) ? InternalAttachmentType.Regular : InternalAttachmentType.Related;
				this.message.Attachment_SetAttachmentType(this.cookie, attachmentType);
			}
		}

		public EmailMessage EmbeddedMessage
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetEmbeddedMessage(this.cookie);
			}
			set
			{
				this.ThrowIfInvalid();
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!this.IsEmbeddedMessage)
				{
					throw new InvalidOperationException(EmailMessageStrings.CannotSetEmbeddedMessageForNonMessageRfc822Attachment);
				}
				this.message.Attachment_SetEmbeddedMessage(this.cookie, value);
			}
		}

		public MimePart MimePart
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetMimePart(this.cookie);
			}
		}

		public string FileName
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetFileName(this.cookie);
			}
			set
			{
				this.ThrowIfInvalid();
				this.message.Attachment_SetFileName(this.cookie, value);
			}
		}

		internal bool IsEmbeddedMessage
		{
			get
			{
				this.ThrowIfInvalid();
				return this.AttachmentMethod == AttachmentMethod.EmbeddedMessage && null != this.EmbeddedMessage;
			}
		}

		internal string ContentDisposition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetContentDisposition(this.cookie);
			}
			set
			{
				this.ThrowIfInvalid();
				this.message.Attachment_SetContentDisposition(this.cookie, value);
			}
		}

		internal AttachmentMethod AttachmentMethod
		{
			get
			{
				return this.message.Attachment_GetAttachmentMethod(this.cookie);
			}
		}

		internal bool IsAppleDouble
		{
			get
			{
				return this.message.Attachment_IsAppleDouble(this.cookie);
			}
		}

		internal int RenderingPosition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetRenderingPosition(this.cookie);
			}
		}

		internal byte[] AttachRendering
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetAttachRendering(this.cookie);
			}
		}

		internal string AttachContentID
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetAttachContentID(this.cookie);
			}
		}

		internal string AttachContentLocation
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetAttachContentLocation(this.cookie);
			}
		}

		internal int AttachmentFlags
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetAttachmentFlags(this.cookie);
			}
		}

		internal bool AttachHidden
		{
			get
			{
				this.ThrowIfInvalid();
				return this.message.Attachment_GetAttachHidden(this.cookie);
			}
		}

		internal string AltContentID
		{
			get
			{
				return this.altContentID;
			}
			set
			{
				this.altContentID = value;
			}
		}

		public override int GetHashCode()
		{
			return this.message.Attachment_GetHashCode(this.cookie);
		}

		public Stream GetContentReadStream()
		{
			this.ThrowIfInvalid();
			return this.message.Attachment_GetContentReadStream(this.cookie);
		}

		public bool TryGetContentReadStream(out Stream result)
		{
			this.ThrowIfInvalid();
			return this.message.Attachment_TryGetContentReadStream(this.cookie, out result);
		}

		public Stream GetContentWriteStream()
		{
			this.ThrowIfInvalid();
			return this.message.Attachment_GetContentWriteStream(this.cookie);
		}

		internal void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				this.message.Attachment_Dispose(this.cookie);
			}
		}

		private void ThrowIfInvalid()
		{
			if (this.message == null)
			{
				throw new InvalidOperationException(EmailMessageStrings.AttachmentRemovedFromMessage);
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException("Attachment");
			}
		}

		internal const string DefaultSubject = "No Subject";

		private EmailMessage message;

		private bool disposed;

		private AttachmentCookie cookie;

		private string altContentID;

		internal static class FileNameGenerator
		{
			internal static string GenerateFileName(ref int sequenceNumber)
			{
				int num = Interlocked.Increment(ref sequenceNumber);
				return string.Format(CultureInfo.InvariantCulture, Attachment.FileNameGenerator.fileNameFormat, new object[]
				{
					num
				});
			}

			internal static bool IsGeneratedFileName(string fileName)
			{
				if (fileName == null)
				{
					return false;
				}
				if (fileName.Length != 8)
				{
					return false;
				}
				if (!fileName.StartsWith("ATT", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				for (int i = "ATT".Length; i < fileName.Length; i++)
				{
					if (!char.IsNumber(fileName[i]))
					{
						return false;
					}
				}
				return true;
			}

			private const string FileNamePrefix = "ATT";

			private const int FileNameLength = 8;

			private static readonly int fileNameSequenceNumberLength = 8 - "ATT".Length;

			private static string fileNameFormat = "ATT{0:d" + Attachment.FileNameGenerator.fileNameSequenceNumberLength.ToString() + "}";
		}
	}
}
