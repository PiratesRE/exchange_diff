using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.ExchangeTopology
{
	internal sealed class TopologySiteLink : ADSiteLink, ITopologySiteLink
	{
		internal TopologySiteLink(ADSiteLink siteLink)
		{
			this.propertyBag = siteLink.propertyBag;
			this.SetIsReadOnly(siteLink.IsReadOnly);
			this.m_Session = siteLink.Session;
		}

		public ReadOnlyCollection<ITopologySite> TopologySites
		{
			get
			{
				return this.topologySites;
			}
			internal set
			{
				this.topologySites = value;
			}
		}

		ulong ITopologySiteLink.AbsoluteMaxMessageSize
		{
			get
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)this[ADSiteLinkSchema.MaxMessageSize];
				if (!unlimited.IsUnlimited)
				{
					return unlimited.Value.ToBytes();
				}
				return ADSiteLink.UnlimitedMaxMessageSize;
			}
		}

		private ReadOnlyCollection<ITopologySite> topologySites;
	}
}
