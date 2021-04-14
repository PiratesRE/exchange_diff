using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.Exchange.Autodiscover;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	internal class AutoDiscoverV2Handler : AutoDiscoverV2HandlerBase, IHttpHandler
	{
		public AutoDiscoverV2Handler(RequestDetailsLogger logger)
		{
			base.Logger = logger;
		}

		[ExcludeFromCodeCoverage]
		public AutoDiscoverV2Handler()
		{
		}

		public override string GetEmailAddressFromUrl(HttpContextBase context)
		{
			return context.Request.Params["Email"];
		}
	}
}
