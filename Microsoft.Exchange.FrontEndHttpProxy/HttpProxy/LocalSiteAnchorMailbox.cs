using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class LocalSiteAnchorMailbox : AnchorMailbox
	{
		public LocalSiteAnchorMailbox(IRequestContext requestContext) : base(AnchorSource.Anonymous, LocalSiteAnchorMailbox.LocalSiteIdentifier, requestContext)
		{
		}

		public override BackEndServer TryDirectBackEndCalculation()
		{
			BackEndServer backEndServer = LocalSiteMailboxServerCache.Instance.TryGetRandomE15Server(base.RequestContext);
			if (backEndServer != null)
			{
				return backEndServer;
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ServersCache.Enabled)
			{
				try
				{
					MiniServer anyBackEndServerFromLocalSite = ServersCache.GetAnyBackEndServerFromLocalSite(Server.E15MinVersion, false);
					return new BackEndServer(anyBackEndServerFromLocalSite.Fqdn, anyBackEndServerFromLocalSite.VersionNumber);
				}
				catch (ServerHasNotBeenFoundException)
				{
					return base.CheckForNullAndThrowIfApplicable<BackEndServer>(null);
				}
			}
			return HttpProxyBackEndHelper.GetAnyBackEndServerForVersion<WebServicesService>(new ServerVersion(Server.E15MinVersion), false, ClientAccessType.InternalNLBBypass, true);
		}

		internal static readonly string LocalSiteIdentifier = "LocalSite";
	}
}
