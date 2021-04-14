using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class DeltaSyncCommon
	{
		internal static readonly string TextXmlContentType = "text/xml";

		internal static readonly string ApplicationXopXmlContentType = "application/xop+xml";

		internal static readonly string DefaultSyncKey = "0";

		internal static readonly string FolderCollectionName = "Folder";

		internal static readonly string EmailCollectionName = "Email";

		internal static readonly string SevenBit = "7bit";

		internal static readonly string Binary = "binary";

		internal static readonly string Charset = "charset";

		internal static readonly string MultipartRelated = "multipart/related";

		internal static readonly string Boundary = "boundary";

		internal static readonly string Type = "type";

		internal static readonly string Start = "start";

		internal static readonly string StartInfo = "start-info";

		internal static readonly string VersionOneDotZero = "1.0";

		internal static readonly string ApplicationRFC822 = "application/rfc822";

		internal static readonly string NormalMessageClass = "IPM.Note";

		internal static readonly string DraftMessageClass = "HM.Note.Draft";

		internal static readonly string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ";

		internal static readonly string DefaultStringCharset = "iso-8859-1";

		internal static readonly int DefaultEncodingVersion = 1;

		internal static readonly string[] SupportedMessageClasses = new string[]
		{
			"ipm.note",
			"hm.note.fax",
			"hm.note.voicemail",
			"hm.schedule.related",
			"ipm.schedule.meeting.request",
			"ipm.schedule.meeting.cancelled",
			"ipm.schedule.meeting.resp.pos",
			"ipm.schedule.meeting.resp.tent",
			"ipm.schedule.meeting.resp.neq",
			"hm.note.draft",
			"hm.note.photomail",
			"hm.msngr.oim.text",
			"hm.msngr.oim.sms",
			"hm.msngr.bubble",
			"hm.note.calllog"
		};
	}
}
