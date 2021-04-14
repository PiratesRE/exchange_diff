using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ServerVersionAnchorMailbox<ServiceType> : AnchorMailbox where ServiceType : HttpService
	{
		public ServerVersionAnchorMailbox(ServerVersion serverVersion, ClientAccessType clientAccessType, IRequestContext requestContext) : base(AnchorSource.ServerVersion, serverVersion, requestContext)
		{
			this.ClientAccessType = clientAccessType;
			base.NotFoundExceptionCreator = delegate()
			{
				string message = string.Format("Cannot find Mailbox server with {0}.", this.ServerVersion);
				return new ServerNotFoundException(message, this.ServerVersion.ToString());
			};
		}

		public ServerVersionAnchorMailbox(ServerVersion serverVersion, ClientAccessType clientAccessType, bool exactVersionMatch, IRequestContext requestContext) : this(serverVersion, clientAccessType, requestContext)
		{
			this.ExactVersionMatch = exactVersionMatch;
		}

		public ServerVersion ServerVersion
		{
			get
			{
				return (ServerVersion)base.SourceObject;
			}
		}

		public ClientAccessType ClientAccessType { get; private set; }

		public bool ExactVersionMatch { get; private set; }

		public override BackEndServer TryDirectBackEndCalculation()
		{
			if (this.ServerVersion.Major == 15 && !this.ExactVersionMatch)
			{
				BackEndServer backEndServer = LocalSiteMailboxServerCache.Instance.TryGetRandomE15Server(base.RequestContext);
				if (backEndServer != null)
				{
					ServerVersion serverVersion = new ServerVersion(backEndServer.Version);
					if (serverVersion.Minor >= this.ServerVersion.Minor)
					{
						return backEndServer;
					}
				}
			}
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ServersCache.Enabled)
			{
				try
				{
					MiniServer miniServer;
					if (this.ExactVersionMatch)
					{
						miniServer = ServersCache.GetAnyBackEndServerWithExactVersion(this.ServerVersion.ToInt());
					}
					else
					{
						miniServer = ServersCache.GetAnyBackEndServerWithMinVersion(this.ServerVersion.ToInt());
					}
					return new BackEndServer(miniServer.Fqdn, miniServer.VersionNumber);
				}
				catch (ServerHasNotBeenFoundException)
				{
					return base.CheckForNullAndThrowIfApplicable<BackEndServer>(null);
				}
			}
			BackEndServer result;
			try
			{
				BackEndServer anyBackEndServerForVersion = HttpProxyBackEndHelper.GetAnyBackEndServerForVersion<ServiceType>(this.ServerVersion, this.ExactVersionMatch, this.ClientAccessType, false);
				result = anyBackEndServerForVersion;
			}
			catch (ServerNotFoundException)
			{
				result = base.CheckForNullAndThrowIfApplicable<BackEndServer>(null);
			}
			return result;
		}
	}
}
