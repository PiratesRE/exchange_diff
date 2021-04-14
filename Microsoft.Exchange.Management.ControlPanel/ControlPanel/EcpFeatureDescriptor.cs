using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpFeatureDescriptor
	{
		public EcpFeatureDescriptor(EcpFeature id, string serverPath, string url, bool useAbsoluteUrl = false)
		{
			this.Id = id;
			this.Name = id.GetName();
			this.ServerPath = serverPath;
			this.url = url;
			this.UseAbsoluteUrl = useAbsoluteUrl;
		}

		public EcpFeature Id { get; private set; }

		public string Name { get; private set; }

		public bool UseAbsoluteUrl { get; private set; }

		public Uri AbsoluteUrl
		{
			get
			{
				if (this.Url.IsAbsoluteUri)
				{
					return this.Url;
				}
				Uri uri = new Uri(HttpContext.Current.GetRequestUrl(), this.Url);
				return EcpUrl.ResolveClientUrl(uri);
			}
		}

		public string ServerPath { get; private set; }

		public Uri Url
		{
			get
			{
				if (this.calculatedUrl == null)
				{
					this.CalculateUrl();
				}
				return this.calculatedUrl;
			}
		}

		private void CalculateUrl()
		{
			Uri uri = new Uri(this.url, UriKind.RelativeOrAbsolute);
			if (!uri.IsAbsoluteUri)
			{
				string text = EcpUrl.GetLeftPart(this.url, UriPartial.Path);
				string text2 = this.url.Substring(text.Length);
				text = VirtualPathUtility.ToAbsolute(text);
				if (!string.IsNullOrEmpty(text2))
				{
					text += text2;
				}
				text = EcpUrl.AppendQueryParameter(text, "exsvurl", "1");
				uri = new Uri(text, UriKind.Relative);
			}
			this.calculatedUrl = uri;
		}

		private readonly string url;

		private Uri calculatedUrl;
	}
}
