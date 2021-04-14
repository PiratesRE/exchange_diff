using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;

namespace Microsoft.Exchange.HttpProxy
{
	internal class AnchoredRoutingTarget
	{
		public AnchoredRoutingTarget(AnchorMailbox anchorMailbox, BackEndServer backendServer)
		{
			if (anchorMailbox == null)
			{
				throw new ArgumentNullException("anchorMailbox");
			}
			if (backendServer == null)
			{
				throw new ArgumentNullException("backendServer");
			}
			this.AnchorMailbox = anchorMailbox;
			this.BackEndServer = backendServer;
		}

		public AnchoredRoutingTarget(ServerInfoAnchorMailbox serverInfoAnchorMailbox)
		{
			if (serverInfoAnchorMailbox == null)
			{
				throw new ArgumentNullException("serverAnchorMailbox");
			}
			this.AnchorMailbox = serverInfoAnchorMailbox;
			this.BackEndServer = serverInfoAnchorMailbox.BackEndServer;
		}

		public AnchorMailbox AnchorMailbox { get; private set; }

		public BackEndServer BackEndServer { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}~{1}", this.AnchorMailbox, this.BackEndServer.Fqdn);
		}

		public string GetSiteName()
		{
			string empty = string.Empty;
			if (this.BackEndServer != null && !string.IsNullOrEmpty(this.BackEndServer.Fqdn))
			{
				Utilities.TryGetSiteNameFromServerFqdn(this.BackEndServer.Fqdn, out empty);
			}
			return empty;
		}
	}
}
