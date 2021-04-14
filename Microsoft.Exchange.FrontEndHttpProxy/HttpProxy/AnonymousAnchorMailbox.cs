using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AnonymousAnchorMailbox : AnchorMailbox
	{
		public AnonymousAnchorMailbox(IRequestContext requestContext) : base(AnchorSource.Anonymous, AnonymousAnchorMailbox.AnonymousIdentifier, requestContext)
		{
		}

		public override BackEndServer TryDirectBackEndCalculation()
		{
			return MailboxServerCache.Instance.GetRandomE15Server(base.RequestContext);
		}

		internal static readonly string AnonymousIdentifier = "Anonymous";
	}
}
