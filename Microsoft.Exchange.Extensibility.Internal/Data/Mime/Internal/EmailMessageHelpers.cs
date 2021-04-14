using System;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	internal sealed class EmailMessageHelpers
	{
		public static bool IsAppleDoubleAttachment(Attachment attachment)
		{
			return attachment.IsAppleDouble;
		}

		public static bool IsEmbeddedMessageAttachment(Attachment attachment)
		{
			return attachment.IsEmbeddedMessage;
		}

		public static int GetRenderingPosition(Attachment attachment)
		{
			return attachment.RenderingPosition;
		}

		public static byte[] GetAttachRendering(Attachment attachment)
		{
			return attachment.AttachRendering;
		}

		public static string GetAttachContentID(Attachment attachment)
		{
			return attachment.AttachContentID;
		}

		public static string GetAttachContentLocation(Attachment attachment)
		{
			return attachment.AttachContentLocation;
		}

		public static Charset GetTnefTextCharset(EmailMessage emailMessage)
		{
			return emailMessage.TnefTextCharset;
		}

		public static bool TryGetTnefBinaryCharset(EmailMessage emailMessage, out Charset charset)
		{
			return emailMessage.TryGetTnefBinaryCharset(out charset);
		}

		public static int GetAttachmentFlags(Attachment attachment)
		{
			return attachment.AttachmentFlags;
		}

		public static bool GetAttachHidden(Attachment attachment)
		{
			return attachment.AttachHidden;
		}

		public static string GetHeaderValue(Header header)
		{
			return Utility.GetHeaderValue(header);
		}

		public static string GenerateFileName(ref int sequenceNumber)
		{
			return Attachment.FileNameGenerator.GenerateFileName(ref sequenceNumber);
		}

		public static bool IsGeneratedFileName(string name)
		{
			return Attachment.FileNameGenerator.IsGeneratedFileName(name);
		}

		public static bool IsPublicFolderReplicationMessage(EmailMessage message)
		{
			return message.IsPublicFolderReplicationMessage;
		}

		public static string RemoveMimeHeaderComments(string headerValue)
		{
			return Utility.RemoveMimeHeaderComments(headerValue);
		}

		public const TnefComplianceStatus BannedTnefComplianceViolations = ~(TnefComplianceStatus.InvalidAttributeChecksum | TnefComplianceStatus.InvalidMessageCodepage | TnefComplianceStatus.InvalidDate);

		public static readonly TnefNameTag TnefNameTagIsClassified = TnefPropertyBag.TnefNameTagIsClassified;

		public static readonly TnefNameTag TnefNameTagClassification = TnefPropertyBag.TnefNameTagClassification;

		public static readonly TnefNameTag TnefNameTagClassificationDescription = TnefPropertyBag.TnefNameTagClassificationDescription;

		public static readonly TnefNameTag TnefNameTagClassificationGuid = TnefPropertyBag.TnefNameTagClassificationGuid;

		public static readonly TnefNameTag TnefNameTagClassificationKeep = TnefPropertyBag.TnefNameTagClassificationKeep;
	}
}
