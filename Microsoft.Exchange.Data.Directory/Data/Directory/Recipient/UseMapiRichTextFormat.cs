using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	public enum UseMapiRichTextFormat
	{
		[LocDescription(DirectoryStrings.IDs.Never)]
		Never,
		[LocDescription(DirectoryStrings.IDs.Always)]
		Always,
		[LocDescription(DirectoryStrings.IDs.UseDefaultSettings)]
		UseDefaultSettings
	}
}
