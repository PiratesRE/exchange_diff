using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class UrlAnchorMailbox : AnchorMailbox
	{
		public UrlAnchorMailbox(Uri url, IRequestContext requestContext) : base(AnchorSource.Url, url, requestContext)
		{
		}

		public Uri Url
		{
			get
			{
				return (Uri)base.SourceObject;
			}
		}

		public override BackEndServer TryDirectBackEndCalculation()
		{
			return new BackEndServer(this.Url.Host, Server.E15MinVersion);
		}
	}
}
