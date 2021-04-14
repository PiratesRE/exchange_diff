using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ReroutableItem : RestrictedItem
	{
		public ReroutableItem(MailRecipient recipient) : base(recipient)
		{
		}

		public ADObjectId HomeMtaServerId
		{
			get
			{
				return base.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.HomeMtaServerId");
			}
		}

		public string HomeMTA
		{
			get
			{
				ADObjectId homeMtaServerId = this.HomeMtaServerId;
				if (homeMtaServerId != null)
				{
					return homeMtaServerId.DistinguishedName;
				}
				return null;
			}
		}

		public override void Allow(Expansion expansion)
		{
			string homeMTA = this.HomeMTA;
			if (string.IsNullOrEmpty(homeMTA) || homeMTA.Equals(ResolverConfiguration.ServerDN, StringComparison.OrdinalIgnoreCase))
			{
				this.ProcessLocally(expansion);
				return;
			}
			this.ProcessRemotely();
		}

		public override void AddItemVisited(Expansion expansion)
		{
			ExTraceGlobals.ResolverTracer.TraceDebug<RoutingAddress>(0L, "Adding group '{0}' to the visited list", base.Email);
			expansion.Resolver.ResolverCache.AddToResolvedRecipientCache(base.ObjectGuid);
		}

		protected virtual void ProcessLocally(Expansion expansion)
		{
		}

		protected virtual void ProcessRemotely()
		{
		}
	}
}
