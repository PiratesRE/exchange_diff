using System;
using System.Text;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class AutoDiscoverRequestResult
	{
		public Exception Exception { get; private set; }

		public string RedirectAddress { get; private set; }

		public string FrontEndServerName { get; set; }

		public string BackEndServerName { get; set; }

		public Uri Url { get; private set; }

		public Uri AutoDiscoverRedirectUri { get; private set; }

		public WebServiceUri WebServiceUri { get; private set; }

		public AutoDiscoverRequestResult(Uri url, string redirectAddress, Uri autoDiscoverRedirectUri, WebServiceUri webServiceUri, string frontEndServerName, string backEnderServerName)
		{
			this.Url = url;
			this.RedirectAddress = redirectAddress;
			this.AutoDiscoverRedirectUri = autoDiscoverRedirectUri;
			this.WebServiceUri = webServiceUri;
			this.FrontEndServerName = frontEndServerName;
			this.BackEndServerName = backEnderServerName;
		}

		public AutoDiscoverRequestResult(Uri url, Exception exception, string frontEndServerName, string backEnderServerName)
		{
			this.Url = url;
			this.Exception = exception;
			this.FrontEndServerName = frontEndServerName;
			this.BackEndServerName = backEnderServerName;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (this.RedirectAddress != null)
			{
				stringBuilder.Append("RedirectAddress=");
				stringBuilder.AppendLine(this.RedirectAddress);
			}
			if (this.AutoDiscoverRedirectUri != null)
			{
				stringBuilder.Append("AutoDiscoverRedirectUri=");
				stringBuilder.AppendLine(this.AutoDiscoverRedirectUri.ToString());
			}
			if (this.WebServiceUri != null)
			{
				stringBuilder.Append("WebServiceUri=");
				stringBuilder.AppendLine(this.WebServiceUri.ToString());
			}
			if (this.Exception != null)
			{
				stringBuilder.Append("Exception=");
				stringBuilder.AppendLine(this.Exception.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
