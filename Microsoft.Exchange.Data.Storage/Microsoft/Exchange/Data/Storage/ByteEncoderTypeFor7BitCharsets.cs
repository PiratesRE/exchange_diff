using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ByteEncoderTypeFor7BitCharsets
	{
		Use7Bit,
		UseQP,
		UseBase64,
		UseQPHtmlDetectTextPlain = 5,
		UseBase64HtmlDetectTextPlain,
		UseQPHtml7BitTextPlain = 13,
		UseBase64Html7BitTextPlain
	}
}
