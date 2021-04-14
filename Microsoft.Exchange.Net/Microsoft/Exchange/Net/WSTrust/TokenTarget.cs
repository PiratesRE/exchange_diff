using System;
using System.Text;

namespace Microsoft.Exchange.Net.WSTrust
{
	[Serializable]
	internal sealed class TokenTarget
	{
		public TokenTarget(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this.Uri = uri;
		}

		public TokenTarget(Uri uri, Uri[] tokenIssuerUris)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (tokenIssuerUris == null)
			{
				throw new ArgumentNullException("tokenIssuerUris");
			}
			this.TokenIssuerUris = tokenIssuerUris;
			this.Uri = uri;
		}

		public Uri Uri { get; private set; }

		public Uri[] TokenIssuerUris { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Uri=");
			stringBuilder.Append(this.Uri);
			if (this.TokenIssuerUris != null)
			{
				stringBuilder.Append(",TokenIssuerUris=");
				bool flag = true;
				foreach (Uri uri in this.TokenIssuerUris)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(uri.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public static Uri Fix(string domain)
		{
			Uri originalUri = new Uri("http://" + domain, UriKind.Absolute);
			return TokenTarget.Fix(originalUri);
		}

		public static Uri Fix(Uri originalUri)
		{
			if (originalUri.IsAbsoluteUri)
			{
				return originalUri;
			}
			return new Uri("http://" + originalUri.OriginalString, UriKind.Absolute);
		}

		private const string Prefix = "http://";
	}
}
