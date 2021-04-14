using System;

namespace Microsoft.Exchange.SoapWebClient
{
	[Serializable]
	public enum AutodiscoverResult
	{
		Success,
		Failure,
		UnsecuredRedirect,
		InvalidSslHostname
	}
}
