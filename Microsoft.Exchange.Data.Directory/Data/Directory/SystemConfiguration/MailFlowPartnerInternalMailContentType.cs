using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum MailFlowPartnerInternalMailContentType
	{
		[LocDescription(DirectoryStrings.IDs.MailFlowPartnerInternalMailContentTypeNone)]
		None,
		[LocDescription(DirectoryStrings.IDs.MailFlowPartnerInternalMailContentTypeTNEF)]
		TNEF,
		[LocDescription(DirectoryStrings.IDs.MailFlowPartnerInternalMailContentTypeMimeHtmlText)]
		MimeHtmlText,
		[LocDescription(DirectoryStrings.IDs.MailFlowPartnerInternalMailContentTypeMimeText)]
		MimeText,
		[LocDescription(DirectoryStrings.IDs.MailFlowPartnerInternalMailContentTypeMimeHtml)]
		MimeHtml
	}
}
