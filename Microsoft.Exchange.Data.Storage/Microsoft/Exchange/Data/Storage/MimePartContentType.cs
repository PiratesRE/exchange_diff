using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum MimePartContentType
	{
		TextPlain,
		TextHtml,
		TextEnriched,
		Tnef,
		Calendar,
		DsnReportBody,
		MdnReportBody,
		Attachment,
		ItemAttachment,
		AppleFile,
		JournalReportBody,
		JournalReportMsg,
		JournalReportTnef,
		FirstMultipartType,
		MultipartAlternative = 13,
		MultipartRelated,
		MultipartMixed,
		MultipartReportDsn,
		MultipartReportMdn,
		MultipartDigest,
		MultipartFormData,
		MultipartParallel,
		MultipartAppleDouble
	}
}
