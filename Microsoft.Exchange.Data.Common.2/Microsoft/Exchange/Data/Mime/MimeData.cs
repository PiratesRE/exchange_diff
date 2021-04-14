using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal static class MimeData
	{
		public static int HashName(byte[] chars, int off, int len)
		{
			int num = len;
			while (len != 0)
			{
				num = ((num << 3) + (num >> 5) ^ (int)(chars[off] | 32));
				len--;
				off++;
			}
			return (num & int.MaxValue) % 121;
		}

		public static int HashName(string chars)
		{
			int num = chars.Length;
			for (int num2 = 0; num2 != chars.Length; num2++)
			{
				num = ((num << 3) + (num >> 5) ^ (int)(chars[num2] | ' '));
			}
			return (num & int.MaxValue) % 121;
		}

		public static int HashValue(string chars, int off, int len)
		{
			int num = 0;
			while (len != 0)
			{
				num = ((num << 3) + (num >> 5) ^ (int)(chars[off] | ' '));
				len--;
				off++;
			}
			return (num & int.MaxValue) % 201;
		}

		public static int HashValue(byte[] chars, int off, int len)
		{
			int num = 0;
			while (len != 0)
			{
				num = ((num << 3) + (num >> 5) ^ (int)(chars[off] | 32));
				len--;
				off++;
			}
			return (num & int.MaxValue) % 201;
		}

		public static int HashValue(string chars)
		{
			int num = 0;
			for (int num2 = 0; num2 != chars.Length; num2++)
			{
				num = ((num << 3) + (num >> 5) ^ (int)(chars[num2] | ' '));
			}
			return (num & int.MaxValue) % 201;
		}

		public static int HashValueAdd(int hash, string chars)
		{
			for (int num = 0; num != chars.Length; num++)
			{
				hash = ((hash << 3) + (hash >> 5) ^ (int)(chars[num] | ' '));
			}
			return hash;
		}

		public static int HashValueFinish(int hash)
		{
			return (hash & int.MaxValue) % 201;
		}

		public const short MIN_HEADER_NAME = 2;

		public const short MAX_HEADER_NAME = 31;

		public const short MIN_VALUE = 2;

		public const short MAX_VALUE = 32;

		public static MimeData.HeaderNameDef[] headerNames = new MimeData.HeaderNameDef[]
		{
			new MimeData.HeaderNameDef(0, null, HeaderType.Text, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(0, "Return-Path", HeaderType.AsciiText, HeaderId.ReturnPath, false),
			new MimeData.HeaderNameDef(0, "Resent-Date", HeaderType.Date, HeaderId.ResentDate, false),
			new MimeData.HeaderNameDef(3, "Message-ID", HeaderType.AsciiText, HeaderId.MessageId, false),
			new MimeData.HeaderNameDef(4, "Subject", HeaderType.Text, HeaderId.Subject, false),
			new MimeData.HeaderNameDef(5, "X-Mailer", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(7, "Summary", HeaderType.AsciiText, HeaderId.Summary, false),
			new MimeData.HeaderNameDef(10, "X-Priority", HeaderType.AsciiText, HeaderId.XPriority, false),
			new MimeData.HeaderNameDef(11, "Precedence", HeaderType.AsciiText, HeaderId.Precedence, false),
			new MimeData.HeaderNameDef(12, "List-Unsubscribe", HeaderType.AsciiText, HeaderId.ListUnsubscribe, false),
			new MimeData.HeaderNameDef(18, "Rr", HeaderType.AsciiText, HeaderId.RR, false),
			new MimeData.HeaderNameDef(20, "Comments", HeaderType.Text, HeaderId.Comments, false),
			new MimeData.HeaderNameDef(24, "Content-Location", HeaderType.AsciiText, HeaderId.ContentLocation, true),
			new MimeData.HeaderNameDef(26, "Resent-From", HeaderType.Address, HeaderId.ResentFrom, false),
			new MimeData.HeaderNameDef(27, "Content-Language", HeaderType.AsciiText, HeaderId.ContentLanguage, true),
			new MimeData.HeaderNameDef(27, "Content-MD5", HeaderType.AsciiText, HeaderId.ContentMD5, true),
			new MimeData.HeaderNameDef(28, "Newsgroups", HeaderType.AsciiText, HeaderId.NewsGroups, false),
			new MimeData.HeaderNameDef(28, "Importance", HeaderType.AsciiText, HeaderId.Importance, false),
			new MimeData.HeaderNameDef(29, "Resent-Cc", HeaderType.Address, HeaderId.ResentCc, false),
			new MimeData.HeaderNameDef(30, "Resent-Bcc", HeaderType.Address, HeaderId.ResentBcc, false),
			new MimeData.HeaderNameDef(34, "Resent-Sender", HeaderType.Address, HeaderId.ResentSender, false),
			new MimeData.HeaderNameDef(34, "Content-Class", HeaderType.AsciiText, HeaderId.ContentClass, true),
			new MimeData.HeaderNameDef(36, "List-Help", HeaderType.AsciiText, HeaderId.ListHelp, false),
			new MimeData.HeaderNameDef(36, "AdHoc", HeaderType.AsciiText, HeaderId.AdHoc, false),
			new MimeData.HeaderNameDef(36, "Content-Base", HeaderType.AsciiText, HeaderId.ContentBase, true),
			new MimeData.HeaderNameDef(37, "From", HeaderType.Address, HeaderId.From, false),
			new MimeData.HeaderNameDef(38, "Return-Receipt-To", HeaderType.Address, HeaderId.ReturnReceiptTo, false),
			new MimeData.HeaderNameDef(38, "NNTP-Posting-Host", HeaderType.AsciiText, HeaderId.NntpPostingHost, false),
			new MimeData.HeaderNameDef(40, "Lines", HeaderType.AsciiText, HeaderId.Lines, false),
			new MimeData.HeaderNameDef(42, "Content-Disposition", HeaderType.ContentDisposition, HeaderId.ContentDisposition, true),
			new MimeData.HeaderNameDef(42, "Thread-Topic", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(43, "Bytes", HeaderType.AsciiText, HeaderId.Bytes, false),
			new MimeData.HeaderNameDef(48, "Content-ID", HeaderType.AsciiText, HeaderId.ContentId, true),
			new MimeData.HeaderNameDef(48, "CC", HeaderType.Address, HeaderId.Cc, false),
			new MimeData.HeaderNameDef(49, "Followup-To", HeaderType.AsciiText, HeaderId.FollowUpTo, false),
			new MimeData.HeaderNameDef(52, "Reply-By", HeaderType.Date, HeaderId.ReplyBy, false),
			new MimeData.HeaderNameDef(55, "BCC", HeaderType.Address, HeaderId.Bcc, false),
			new MimeData.HeaderNameDef(59, "Resent-Message-ID", HeaderType.AsciiText, HeaderId.ResentMessageId, false),
			new MimeData.HeaderNameDef(60, "X-OriginalArrivalTime", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(61, "X-MSMail-Priority", HeaderType.AsciiText, HeaderId.XMSMailPriority, false),
			new MimeData.HeaderNameDef(64, "X-MS-Exchange-Organization-BCC", HeaderType.Address, HeaderId.XExchangeBcc, false),
			new MimeData.HeaderNameDef(66, "Deferred-Delivery", HeaderType.Date, HeaderId.DeferredDelivery, false),
			new MimeData.HeaderNameDef(67, "Expires", HeaderType.Date, HeaderId.Expires, false),
			new MimeData.HeaderNameDef(70, "Thread-Index", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(71, "Organization", HeaderType.AsciiText, HeaderId.Organization, false),
			new MimeData.HeaderNameDef(72, "Content-Transfer-Encoding", HeaderType.AsciiText, HeaderId.ContentTransferEncoding, true),
			new MimeData.HeaderNameDef(78, "Encoding", HeaderType.AsciiText, HeaderId.Encoding, false),
			new MimeData.HeaderNameDef(80, "X-MS-Exchange-CrossPremises-BCC", HeaderType.Address, HeaderId.XExchangeCrossPremisesBcc, false),
			new MimeData.HeaderNameDef(81, "Content-Type", HeaderType.ContentType, HeaderId.ContentType, true),
			new MimeData.HeaderNameDef(82, "Apparently-To", HeaderType.AsciiText, HeaderId.ApparentlyTo, false),
			new MimeData.HeaderNameDef(83, "Approved", HeaderType.AsciiText, HeaderId.Approved, false),
			new MimeData.HeaderNameDef(84, "X-Notes-Item", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(84, "Received", HeaderType.Received, HeaderId.Received, false),
			new MimeData.HeaderNameDef(85, "Path", HeaderType.AsciiText, HeaderId.Path, false),
			new MimeData.HeaderNameDef(86, "References", HeaderType.AsciiText, HeaderId.References, false),
			new MimeData.HeaderNameDef(86, "Sender", HeaderType.Address, HeaderId.Sender, false),
			new MimeData.HeaderNameDef(88, "Resent-To", HeaderType.Address, HeaderId.ResentTo, false),
			new MimeData.HeaderNameDef(88, "X-MS-TNEF-Correlator", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(88, "Supersedes", HeaderType.AsciiText, HeaderId.Supercedes, false),
			new MimeData.HeaderNameDef(89, "Resent-Reply-To", HeaderType.Address, HeaderId.ResentReplyTo, false),
			new MimeData.HeaderNameDef(89, "Keywords", HeaderType.AsciiText, HeaderId.Keywords, false),
			new MimeData.HeaderNameDef(91, "Control", HeaderType.AsciiText, HeaderId.Control, false),
			new MimeData.HeaderNameDef(94, "In-Reply-To", HeaderType.AsciiText, HeaderId.InReplyTo, false),
			new MimeData.HeaderNameDef(96, "List-Subscribe", HeaderType.AsciiText, HeaderId.ListSubscribe, false),
			new MimeData.HeaderNameDef(97, "Content-Description", HeaderType.Text, HeaderId.ContentDescription, true),
			new MimeData.HeaderNameDef(101, "Encrypted", HeaderType.AsciiText, HeaderId.Encrypted, false),
			new MimeData.HeaderNameDef(102, "X-MS-Has-Attach", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(103, "MIME-Version", HeaderType.AsciiText, HeaderId.MimeVersion, true),
			new MimeData.HeaderNameDef(103, "Article", HeaderType.AsciiText, HeaderId.Article, false),
			new MimeData.HeaderNameDef(106, "Disposition-Notification-To", HeaderType.Address, HeaderId.DispositionNotificationTo, false),
			new MimeData.HeaderNameDef(111, "Distribution", HeaderType.AsciiText, HeaderId.Distribution, false),
			new MimeData.HeaderNameDef(111, "Date", HeaderType.Date, HeaderId.Date, false),
			new MimeData.HeaderNameDef(117, "Xref", HeaderType.AsciiText, HeaderId.XRef, false),
			new MimeData.HeaderNameDef(117, "X-MimeOLE", HeaderType.AsciiText, HeaderId.Unknown, false),
			new MimeData.HeaderNameDef(118, "Sensitivity", HeaderType.AsciiText, HeaderId.Sensitivity, false),
			new MimeData.HeaderNameDef(118, "To", HeaderType.Address, HeaderId.To, false),
			new MimeData.HeaderNameDef(118, "Reply-To", HeaderType.Address, HeaderId.ReplyTo, false),
			new MimeData.HeaderNameDef(119, "Expiry-Date", HeaderType.Date, HeaderId.ExpiryDate, false),
			new MimeData.HeaderNameDef(120, "Priority", HeaderType.AsciiText, HeaderId.Priority, false),
			new MimeData.HeaderNameDef(121)
		};

		public static HeaderNameIndex[] nameHashTable = new HeaderNameIndex[]
		{
			HeaderNameIndex.ReturnPath,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.MessageId,
			HeaderNameIndex.Subject,
			(HeaderNameIndex)5,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Summary,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.XPriority,
			HeaderNameIndex.Precedence,
			HeaderNameIndex.ListUnsubscribe,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.RR,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Comments,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ContentLocation,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ResentFrom,
			HeaderNameIndex.ContentLanguage,
			HeaderNameIndex.NewsGroups,
			HeaderNameIndex.ResentCc,
			HeaderNameIndex.ResentBcc,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ResentSender,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ListHelp,
			HeaderNameIndex.From,
			HeaderNameIndex.ReturnReceiptTo,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Lines,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ContentDisposition,
			HeaderNameIndex.Bytes,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ContentId,
			HeaderNameIndex.FollowUpTo,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ReplyBy,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Bcc,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ResentMessageId,
			(HeaderNameIndex)38,
			HeaderNameIndex.XMSMailPriority,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.XExchangeBcc,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.DeferredDelivery,
			HeaderNameIndex.Expires,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			(HeaderNameIndex)43,
			HeaderNameIndex.Organization,
			HeaderNameIndex.ContentTransferEncoding,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Encoding,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.XExchangeCrossPremisesBcc,
			HeaderNameIndex.ContentType,
			HeaderNameIndex.ApparentlyTo,
			HeaderNameIndex.Approved,
			(HeaderNameIndex)51,
			HeaderNameIndex.Path,
			HeaderNameIndex.References,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ResentTo,
			HeaderNameIndex.ResentReplyTo,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Control,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.InReplyTo,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.ListSubscribe,
			HeaderNameIndex.ContentDescription,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Encrypted,
			(HeaderNameIndex)66,
			HeaderNameIndex.MimeVersion,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.DispositionNotificationTo,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Distribution,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Unknown,
			HeaderNameIndex.XRef,
			HeaderNameIndex.Sensitivity,
			HeaderNameIndex.ExpiryDate,
			HeaderNameIndex.Priority
		};

		public static HeaderNameIndex[] nameIndex = new HeaderNameIndex[]
		{
			HeaderNameIndex.Unknown,
			HeaderNameIndex.Received,
			HeaderNameIndex.Date,
			HeaderNameIndex.From,
			HeaderNameIndex.Subject,
			HeaderNameIndex.Sender,
			HeaderNameIndex.To,
			HeaderNameIndex.Cc,
			HeaderNameIndex.Bcc,
			HeaderNameIndex.MessageId,
			HeaderNameIndex.InReplyTo,
			HeaderNameIndex.References,
			HeaderNameIndex.ReturnPath,
			HeaderNameIndex.Comments,
			HeaderNameIndex.Keywords,
			HeaderNameIndex.Encrypted,
			HeaderNameIndex.ReplyBy,
			HeaderNameIndex.ReplyTo,
			HeaderNameIndex.ResentDate,
			HeaderNameIndex.ResentSender,
			HeaderNameIndex.ResentFrom,
			HeaderNameIndex.ResentBcc,
			HeaderNameIndex.ResentCc,
			HeaderNameIndex.ResentTo,
			HeaderNameIndex.ResentReplyTo,
			HeaderNameIndex.ResentMessageId,
			HeaderNameIndex.ApparentlyTo,
			HeaderNameIndex.Approved,
			HeaderNameIndex.Control,
			HeaderNameIndex.DeferredDelivery,
			HeaderNameIndex.Distribution,
			HeaderNameIndex.Encoding,
			HeaderNameIndex.Expires,
			HeaderNameIndex.ExpiryDate,
			HeaderNameIndex.FollowUpTo,
			HeaderNameIndex.Lines,
			HeaderNameIndex.Bytes,
			HeaderNameIndex.Article,
			HeaderNameIndex.Importance,
			HeaderNameIndex.Supercedes,
			HeaderNameIndex.NewsGroups,
			HeaderNameIndex.NntpPostingHost,
			HeaderNameIndex.Organization,
			HeaderNameIndex.Path,
			HeaderNameIndex.Precedence,
			HeaderNameIndex.Priority,
			HeaderNameIndex.ReturnReceiptTo,
			HeaderNameIndex.DispositionNotificationTo,
			HeaderNameIndex.RR,
			HeaderNameIndex.Sensitivity,
			HeaderNameIndex.Summary,
			HeaderNameIndex.XRef,
			HeaderNameIndex.AdHoc,
			HeaderNameIndex.ListHelp,
			HeaderNameIndex.ListSubscribe,
			HeaderNameIndex.ListUnsubscribe,
			HeaderNameIndex.MimeVersion,
			HeaderNameIndex.ContentBase,
			HeaderNameIndex.ContentClass,
			HeaderNameIndex.ContentDescription,
			HeaderNameIndex.ContentDisposition,
			HeaderNameIndex.ContentId,
			HeaderNameIndex.ContentLanguage,
			HeaderNameIndex.ContentLocation,
			HeaderNameIndex.ContentMD5,
			HeaderNameIndex.ContentTransferEncoding,
			HeaderNameIndex.ContentType,
			HeaderNameIndex.XPriority,
			HeaderNameIndex.XMSMailPriority,
			HeaderNameIndex.XExchangeBcc,
			HeaderNameIndex.XExchangeCrossPremisesBcc
		};

		public static int[] valueHashTable = new int[]
		{
			1,
			0,
			2,
			3,
			4,
			0,
			0,
			0,
			0,
			5,
			0,
			7,
			0,
			0,
			0,
			9,
			0,
			0,
			11,
			12,
			0,
			0,
			0,
			0,
			0,
			0,
			13,
			15,
			0,
			16,
			0,
			17,
			18,
			19,
			0,
			22,
			23,
			0,
			25,
			27,
			28,
			29,
			31,
			32,
			0,
			33,
			0,
			34,
			35,
			0,
			0,
			38,
			39,
			40,
			0,
			41,
			42,
			43,
			0,
			0,
			0,
			44,
			0,
			45,
			46,
			47,
			48,
			0,
			0,
			0,
			50,
			0,
			0,
			0,
			51,
			52,
			54,
			58,
			0,
			61,
			63,
			0,
			64,
			0,
			0,
			0,
			65,
			66,
			67,
			68,
			0,
			69,
			0,
			0,
			0,
			0,
			70,
			0,
			0,
			0,
			72,
			0,
			0,
			0,
			73,
			0,
			0,
			74,
			75,
			76,
			77,
			0,
			79,
			80,
			82,
			0,
			0,
			0,
			83,
			85,
			88,
			0,
			90,
			91,
			93,
			0,
			0,
			0,
			94,
			0,
			0,
			0,
			95,
			0,
			97,
			99,
			0,
			100,
			101,
			0,
			102,
			0,
			0,
			0,
			103,
			105,
			106,
			107,
			0,
			109,
			110,
			0,
			0,
			0,
			0,
			0,
			111,
			113,
			116,
			0,
			117,
			118,
			0,
			0,
			119,
			0,
			0,
			120,
			121,
			122,
			123,
			125,
			0,
			127,
			0,
			128,
			0,
			0,
			129,
			0,
			131,
			0,
			0,
			0,
			132,
			133,
			0,
			134,
			135,
			0,
			136,
			137,
			0,
			139,
			140,
			0,
			142,
			0,
			0,
			145,
			146
		};

		public static MimeData.HeaderValueDef[] values = new MimeData.HeaderValueDef[]
		{
			new MimeData.HeaderValueDef(0, null),
			new MimeData.HeaderValueDef(0, "text/enriched"),
			new MimeData.HeaderValueDef(2, "total"),
			new MimeData.HeaderValueDef(3, "inline"),
			new MimeData.HeaderValueDef(4, "message/disposition-notification"),
			new MimeData.HeaderValueDef(9, "mixed"),
			new MimeData.HeaderValueDef(9, "basic"),
			new MimeData.HeaderValueDef(11, "xml"),
			new MimeData.HeaderValueDef(11, "id"),
			new MimeData.HeaderValueDef(15, "uuencode"),
			new MimeData.HeaderValueDef(15, "msaccess"),
			new MimeData.HeaderValueDef(18, "number"),
			new MimeData.HeaderValueDef(19, "application/ms-tnef"),
			new MimeData.HeaderValueDef(26, "video/avi"),
			new MimeData.HeaderValueDef(26, "gif"),
			new MimeData.HeaderValueDef(27, "binary"),
			new MimeData.HeaderValueDef(29, "ms-publisher"),
			new MimeData.HeaderValueDef(31, "multipart/digest"),
			new MimeData.HeaderValueDef(32, "disposition-notification"),
			new MimeData.HeaderValueDef(33, "enriched"),
			new MimeData.HeaderValueDef(33, "boundary"),
			new MimeData.HeaderValueDef(33, "digest"),
			new MimeData.HeaderValueDef(35, "application/x-pkcs7-signature"),
			new MimeData.HeaderValueDef(36, "start-info"),
			new MimeData.HeaderValueDef(36, "charset"),
			new MimeData.HeaderValueDef(38, "avi"),
			new MimeData.HeaderValueDef(38, "msword"),
			new MimeData.HeaderValueDef(39, "ms-vsi"),
			new MimeData.HeaderValueDef(40, "schdpl32"),
			new MimeData.HeaderValueDef(41, "external-body"),
			new MimeData.HeaderValueDef(41, "text/xml"),
			new MimeData.HeaderValueDef(42, "x-pkcs7-mime"),
			new MimeData.HeaderValueDef(43, "base64"),
			new MimeData.HeaderValueDef(45, "access-type"),
			new MimeData.HeaderValueDef(47, "format"),
			new MimeData.HeaderValueDef(48, "ms-powerpoint"),
			new MimeData.HeaderValueDef(48, "padding"),
			new MimeData.HeaderValueDef(48, "application/pgp-signature"),
			new MimeData.HeaderValueDef(51, "pgp-signature"),
			new MimeData.HeaderValueDef(52, "server"),
			new MimeData.HeaderValueDef(53, "report-type"),
			new MimeData.HeaderValueDef(55, "bmp"),
			new MimeData.HeaderValueDef(56, "text"),
			new MimeData.HeaderValueDef(57, "application/octet-stream"),
			new MimeData.HeaderValueDef(61, "read-date"),
			new MimeData.HeaderValueDef(63, "multipart"),
			new MimeData.HeaderValueDef(64, "mpg"),
			new MimeData.HeaderValueDef(65, "multipart/parallel"),
			new MimeData.HeaderValueDef(66, "html"),
			new MimeData.HeaderValueDef(66, "encrypted"),
			new MimeData.HeaderValueDef(70, "mode"),
			new MimeData.HeaderValueDef(74, "octet-stream"),
			new MimeData.HeaderValueDef(75, "x-pkcs7-signature"),
			new MimeData.HeaderValueDef(75, "tiff"),
			new MimeData.HeaderValueDef(76, "multipart/form-data"),
			new MimeData.HeaderValueDef(76, "multipart/signed"),
			new MimeData.HeaderValueDef(76, "image/bmp"),
			new MimeData.HeaderValueDef(76, "permission"),
			new MimeData.HeaderValueDef(77, "partial"),
			new MimeData.HeaderValueDef(77, "richtext"),
			new MimeData.HeaderValueDef(77, "pkcs7-signature"),
			new MimeData.HeaderValueDef(79, "attachment"),
			new MimeData.HeaderValueDef(79, "mac-binhex40"),
			new MimeData.HeaderValueDef(80, "audio"),
			new MimeData.HeaderValueDef(82, "version"),
			new MimeData.HeaderValueDef(86, "report"),
			new MimeData.HeaderValueDef(87, "parallel"),
			new MimeData.HeaderValueDef(88, "multipart/alternative"),
			new MimeData.HeaderValueDef(89, "creation-date"),
			new MimeData.HeaderValueDef(91, "audio/mp3"),
			new MimeData.HeaderValueDef(96, "image"),
			new MimeData.HeaderValueDef(96, "application/x-pkcs7-mime"),
			new MimeData.HeaderValueDef(100, "7bit"),
			new MimeData.HeaderValueDef(104, "delivery-status"),
			new MimeData.HeaderValueDef(107, "application/pkcs7-mime"),
			new MimeData.HeaderValueDef(108, "mp3"),
			new MimeData.HeaderValueDef(109, "mp4"),
			new MimeData.HeaderValueDef(110, "text/richtext"),
			new MimeData.HeaderValueDef(110, "modification-date"),
			new MimeData.HeaderValueDef(112, "text/plain"),
			new MimeData.HeaderValueDef(113, "pkcs7-mime"),
			new MimeData.HeaderValueDef(113, "alternative"),
			new MimeData.HeaderValueDef(114, "expiration"),
			new MimeData.HeaderValueDef(118, "multipart/encrypted"),
			new MimeData.HeaderValueDef(118, "css"),
			new MimeData.HeaderValueDef(119, "image/png"),
			new MimeData.HeaderValueDef(119, "form-data"),
			new MimeData.HeaderValueDef(119, "message/delivery-status"),
			new MimeData.HeaderValueDef(120, "png"),
			new MimeData.HeaderValueDef(120, "smime-type"),
			new MimeData.HeaderValueDef(122, "message/external-body"),
			new MimeData.HeaderValueDef(123, "jpeg"),
			new MimeData.HeaderValueDef(123, "video"),
			new MimeData.HeaderValueDef(124, "postscript"),
			new MimeData.HeaderValueDef(128, "application/mac-binhex40"),
			new MimeData.HeaderValueDef(132, "text/calendar"),
			new MimeData.HeaderValueDef(132, "image/gif"),
			new MimeData.HeaderValueDef(134, "site"),
			new MimeData.HeaderValueDef(134, "plain"),
			new MimeData.HeaderValueDef(135, "message/rfc822"),
			new MimeData.HeaderValueDef(137, "type"),
			new MimeData.HeaderValueDef(138, "image/jpeg"),
			new MimeData.HeaderValueDef(140, "msvideo"),
			new MimeData.HeaderValueDef(144, "related"),
			new MimeData.HeaderValueDef(144, "subject"),
			new MimeData.HeaderValueDef(145, "pkcs10"),
			new MimeData.HeaderValueDef(146, "pdf"),
			new MimeData.HeaderValueDef(147, "midi"),
			new MimeData.HeaderValueDef(147, "message"),
			new MimeData.HeaderValueDef(149, "rfc822"),
			new MimeData.HeaderValueDef(150, "size"),
			new MimeData.HeaderValueDef(156, "audio/basic"),
			new MimeData.HeaderValueDef(156, "text/html"),
			new MimeData.HeaderValueDef(157, "ms-infopath.xml"),
			new MimeData.HeaderValueDef(157, "multipart/related"),
			new MimeData.HeaderValueDef(157, "mpeg4"),
			new MimeData.HeaderValueDef(158, "ms-excel"),
			new MimeData.HeaderValueDef(160, "application/postscript"),
			new MimeData.HeaderValueDef(161, "ms-tnef"),
			new MimeData.HeaderValueDef(164, "multipart/report"),
			new MimeData.HeaderValueDef(167, "x-uue"),
			new MimeData.HeaderValueDef(168, "directory"),
			new MimeData.HeaderValueDef(169, "x-vcard"),
			new MimeData.HeaderValueDef(170, "protocol"),
			new MimeData.HeaderValueDef(170, "video/mpeg"),
			new MimeData.HeaderValueDef(171, "delsp"),
			new MimeData.HeaderValueDef(171, "filename"),
			new MimeData.HeaderValueDef(173, "x-uuencode"),
			new MimeData.HeaderValueDef(175, "name"),
			new MimeData.HeaderValueDef(178, "signed"),
			new MimeData.HeaderValueDef(178, "rfc822-headers"),
			new MimeData.HeaderValueDef(180, "micalg"),
			new MimeData.HeaderValueDef(184, "text/rfc822-headers"),
			new MimeData.HeaderValueDef(185, "calendar"),
			new MimeData.HeaderValueDef(187, "message/partial"),
			new MimeData.HeaderValueDef(188, "application"),
			new MimeData.HeaderValueDef(190, "mid"),
			new MimeData.HeaderValueDef(191, "image/tiff"),
			new MimeData.HeaderValueDef(191, "8bit"),
			new MimeData.HeaderValueDef(193, "mpeg"),
			new MimeData.HeaderValueDef(194, "application/pkcs7-signature"),
			new MimeData.HeaderValueDef(194, "wav"),
			new MimeData.HeaderValueDef(196, "quoted-printable"),
			new MimeData.HeaderValueDef(196, "multipart/mixed"),
			new MimeData.HeaderValueDef(196, "ms-mediapackage"),
			new MimeData.HeaderValueDef(199, "pjpeg"),
			new MimeData.HeaderValueDef(200, "start"),
			new MimeData.HeaderValueDef(201)
		};

		public struct HeaderNameDef
		{
			public HeaderNameDef(int hash)
			{
				this.hash = (byte)hash;
				this.restricted = false;
				this.headerType = HeaderType.Text;
				this.name = null;
				this.publicHeaderId = HeaderId.Unknown;
			}

			public HeaderNameDef(int hash, string name, HeaderType headerType, HeaderId publicHeaderId, bool restricted)
			{
				this.hash = (byte)hash;
				this.restricted = restricted;
				this.headerType = headerType;
				this.name = name;
				this.publicHeaderId = publicHeaderId;
			}

			public string name;

			public byte hash;

			public bool restricted;

			public HeaderType headerType;

			public HeaderId publicHeaderId;
		}

		public struct HeaderValueDef
		{
			public HeaderValueDef(int hash)
			{
				this.hash = (byte)hash;
				this.value = null;
			}

			public HeaderValueDef(int hash, string value)
			{
				this.hash = (byte)hash;
				this.value = value;
			}

			public byte hash;

			public string value;
		}
	}
}
