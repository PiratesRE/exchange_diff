using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum MimeTextFormat
	{
		[LocDescription(DirectoryStrings.IDs.TextOnly)]
		TextOnly,
		[LocDescription(DirectoryStrings.IDs.HtmlOnly)]
		HtmlOnly,
		[LocDescription(DirectoryStrings.IDs.HtmlAndTextAlternative)]
		HtmlAndTextAlternative,
		[LocDescription(DirectoryStrings.IDs.TextEnrichedOnly)]
		TextEnrichedOnly,
		[LocDescription(DirectoryStrings.IDs.TextEnrichedAndTextAlternative)]
		TextEnrichedAndTextAlternative,
		[LocDescription(DirectoryStrings.IDs.BestBodyFormat)]
		BestBodyFormat,
		[LocDescription(DirectoryStrings.IDs.Tnef)]
		Tnef
	}
}
