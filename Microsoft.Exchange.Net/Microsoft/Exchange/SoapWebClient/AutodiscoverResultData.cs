using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;

namespace Microsoft.Exchange.SoapWebClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AutodiscoverResultData
	{
		public AutodiscoverResult Type { get; internal set; }

		public Uri Url { get; internal set; }

		public Exception Exception { get; internal set; }

		public Uri RedirectUrl { get; internal set; }

		public StringList SslCertificateHostnames { get; internal set; }

		public AutodiscoverResponse Response { get; internal set; }

		public AutodiscoverResultData Alternate { get; internal set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(400);
			stringBuilder.Append("Type=" + this.Type + ";");
			stringBuilder.Append("Url=" + this.Url + ";");
			if (this.Exception != null)
			{
				stringBuilder.Append("Exception=" + this.Exception.Message + ";");
			}
			if (this.RedirectUrl != null)
			{
				stringBuilder.Append("RedirectUrl=" + this.RedirectUrl + ";");
			}
			if (this.SslCertificateHostnames != null)
			{
				stringBuilder.Append("SslCertificateHostnames=" + this.SslCertificateHostnames.ToString() + ";");
			}
			if (this.Alternate != null)
			{
				stringBuilder.Append("Alternate=(" + this.Alternate.ToString() + ");");
			}
			return stringBuilder.ToString();
		}

		internal AutodiscoverResultData()
		{
		}
	}
}
