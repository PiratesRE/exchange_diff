using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal enum GetLinkPreviewMetadata
	{
		[DisplayName("GLP", "URL")]
		Url,
		[DisplayName("GLP", "ERR")]
		Error,
		[DisplayName("GLP", "EMSG")]
		ErrorMessage,
		[DisplayName("GLP", "TWP")]
		ElapsedTimeToWebPageStepCompletion,
		[DisplayName("GLP", "TRE")]
		ElapsedTimeToRegExStepCompletion,
		[DisplayName("GLP", "CL")]
		WebPageContentLength,
		[DisplayName("GLP", "ITC")]
		ImageTagCount,
		[DisplayName("GLP", "DTC")]
		DescriptionTagCount,
		[DisplayName("GLP", "TL")]
		TitleLength,
		[DisplayName("GLP", "DL")]
		DescriptionLength,
		[DisplayName("GLP", "DSR")]
		DisabledResponse,
		[DisplayName("GLP", "YTF")]
		YouTubeLinkValidationFailed,
		[DisplayName("GLP", "WEU")]
		WebPageEncodingUsed,
		[DisplayName("GLP", "ERC")]
		EncodingRegExCount,
		[DisplayName("GLP", "TWE")]
		ElapsedTimeToGetWebPageEncoding,
		[DisplayName("GLP", "IURL")]
		InvalidImageUrl,
		[DisplayName("GLP", "UCN")]
		UserContextNull,
		[DisplayName("GLP", "AVC")]
		ActiveViewConvergenceEnabled
	}
}
