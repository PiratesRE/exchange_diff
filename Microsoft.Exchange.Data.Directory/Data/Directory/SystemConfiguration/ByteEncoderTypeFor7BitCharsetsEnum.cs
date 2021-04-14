using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum ByteEncoderTypeFor7BitCharsetsEnum
	{
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUse7Bit)]
		Use7Bit,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseQP)]
		UseQP,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseBase64)]
		UseBase64,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseQPHtmlDetectTextPlain)]
		UseQPHtmlDetectTextPlain = 5,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseBase64HtmlDetectTextPlain)]
		UseBase64HtmlDetectTextPlain,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseQPHtml7BitTextPlain)]
		UseQPHtml7BitTextPlain = 13,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUseBase64Html7BitTextPlain)]
		UseBase64Html7BitTextPlain,
		[LocDescription(DirectoryStrings.IDs.ByteEncoderTypeUndefined)]
		Undefined
	}
}
