using System;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Injector;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ClientFactory : IClientFactory
	{
		internal ClientFactory(ILogger logger, LoadBalanceAnchorContext serviceContext)
		{
			AnchorUtil.ThrowOnNullArgument(serviceContext, "serviceContext");
			AnchorUtil.ThrowOnNullArgument(logger, "logger");
			this.logger = logger;
			this.serviceContext = serviceContext;
		}

		public IInjectorService GetInjectorClientForDatabase(DirectoryDatabase database)
		{
			return this.GetClient<IInjectorService>(database, true, new Func<DirectoryServer, IDirectoryProvider, IInjectorService>(this.CreateInjectorClient), new Func<DirectoryServer, IInjectorService>(this.CreateCompatibilityInjectorClient));
		}

		public ILoadBalanceService GetLoadBalanceClientForCentralServer()
		{
			DirectoryServer centralServer = this.serviceContext.CentralServer;
			return this.GetClient<ILoadBalanceService>(centralServer, false, new Func<DirectoryServer, IDirectoryProvider, ILoadBalanceService>(this.CreateLoadBalancerClient), new Func<DirectoryServer, ILoadBalanceService>(this.CreateCompatibilityLoadBalanceClient));
		}

		public ILoadBalanceService GetLoadBalanceClientForDatabase(DirectoryDatabase database)
		{
			return this.GetClient<ILoadBalanceService>(database, true, new Func<DirectoryServer, IDirectoryProvider, ILoadBalanceService>(this.CreateLoadBalancerClient), new Func<DirectoryServer, ILoadBalanceService>(this.CreateCompatibilityLoadBalanceClient));
		}

		public ILoadBalanceService GetLoadBalanceClientForServer(DirectoryServer server, bool allowFallbackToLocal)
		{
			return this.GetClient<ILoadBalanceService>(server, allowFallbackToLocal, new Func<DirectoryServer, IDirectoryProvider, ILoadBalanceService>(this.CreateLoadBalancerClient), new Func<DirectoryServer, ILoadBalanceService>(this.CreateCompatibilityLoadBalanceClient));
		}

		public IPhysicalDatabase GetPhysicalDatabaseConnection(DirectoryDatabase database)
		{
			return new PhysicalDatabase(database, this.serviceContext.StorePort, this.logger);
		}

		protected ILoadBalanceService CreateCompatibilityLoadBalanceClient(DirectoryServer server)
		{
			this.logger.LogVerbose("Creating full compatibility LB client for server {0}", new object[]
			{
				server.Name
			});
			BackCompatibleLoadBalanceClient service = new BackCompatibleLoadBalanceClient(null, server);
			BandAsMetricCapabilityDecorator service2 = new BandAsMetricCapabilityDecorator(service, this.serviceContext, server);
			SoftDeletedRemovalCapabilityDecorator service3 = new SoftDeletedRemovalCapabilityDecorator(service2, server);
			ConsumerMetricsLoadBalanceCapabilityDecorator service4 = new ConsumerMetricsLoadBalanceCapabilityDecorator(service3, server);
			return new CapacitySummaryCapabilityDecorator(service4, server, this.serviceContext);
		}

		protected virtual IInjectorService CreateInjectorClient(DirectoryServer server, IDirectoryProvider directory)
		{
			IInjectorService result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				InjectorClient injectorClient = InjectorClient.Create(server.Fqdn, directory, this.logger);
				disposeGuard.Add<InjectorClient>(injectorClient);
				bool flag = true;
				IInjectorService injectorService = injectorClient;
				if (!injectorClient.ServerVersion[1])
				{
					flag = false;
					injectorService = this.CreateCompatibilityInjectorClient(server);
				}
				if (!injectorClient.ServerVersion[2])
				{
					injectorService = new ConsumerMetricsInjectorCapabilityDecorator(injectorService);
				}
				if (flag)
				{
					disposeGuard.Success();
				}
				result = injectorService;
			}
			return result;
		}

		protected virtual ILoadBalanceService CreateLoadBalancerClient(DirectoryServer server, IDirectoryProvider directory)
		{
			ILoadBalanceService result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				LoadBalancerClient loadBalancerClient = LoadBalancerClient.Create(server.Fqdn, directory, this.logger);
				disposeGuard.Add<LoadBalancerClient>(loadBalancerClient);
				bool flag = true;
				ILoadBalanceService loadBalanceService = loadBalancerClient;
				if (!loadBalancerClient.ServerVersion[1])
				{
					flag = false;
					loadBalanceService = this.CreateCompatibilityLoadBalanceClient(server);
				}
				else if (!loadBalancerClient.ServerVersion[2])
				{
					loadBalanceService = new SoftDeletedRemovalCapabilityDecorator(loadBalanceService, server);
				}
				if (!loadBalancerClient.ServerVersion[3])
				{
					loadBalanceService = new ConsumerMetricsLoadBalanceCapabilityDecorator(loadBalanceService, server);
				}
				if (!loadBalancerClient.ServerVersion[5])
				{
					loadBalanceService = new CapacitySummaryCapabilityDecorator(loadBalanceService, server, this.serviceContext);
				}
				if (flag)
				{
					disposeGuard.Success();
				}
				result = loadBalanceService;
			}
			return result;
		}

		private IInjectorService CreateCompatibilityInjectorClient(DirectoryServer server)
		{
			this.logger.LogVerbose("Creating full compatibility injector client for server {0}", new object[]
			{
				server.Name
			});
			return new BackCompatibleInjectorClient(this.serviceContext.Service, this.serviceContext.MoveInjector);
		}

		private TClient GetClient<TClient>(DirectoryServer server, bool allowFallback, Func<DirectoryServer, IDirectoryProvider, TClient> clientFactory, Func<DirectoryServer, TClient> fallbackClientFactory) where TClient : class
		{
			try
			{
				return clientFactory(server, server.Directory);
			}
			catch (EndpointNotFoundTransientException ex)
			{
				this.logger.Log(MigrationEventType.Warning, "Could not open connection to server {0}: {1}.", new object[]
				{
					server.Fqdn,
					ex
				});
				if (!allowFallback)
				{
					throw;
				}
			}
			return fallbackClientFactory(server);
		}

		private TClient GetClient<TClient>(DirectoryDatabase database, bool allowFallback, Func<DirectoryServer, IDirectoryProvider, TClient> factory, Func<DirectoryServer, TClient> fallbackClientFactory) where TClient : class
		{
			EndpointNotFoundTransientException ex = null;
			foreach (DirectoryServer directoryServer in database.ActivationOrder)
			{
				try
				{
					return this.GetClient<TClient>(directoryServer, false, factory, null);
				}
				catch (EndpointNotFoundTransientException)
				{
					this.logger.Log(MigrationEventType.Verbose, "Failed to establish a {0} client to '{1}'..", new object[]
					{
						typeof(TClient).Name,
						directoryServer.Name
					});
				}
			}
			if (!allowFallback && ex != null)
			{
				throw new EndpointNotFoundTransientException(ex.ServiceURI, ex.LocalizedString, ex);
			}
			this.logger.Log(MigrationEventType.Warning, "Failed to establish a {0} client to the preferred servers for database '{1}'. Falling back to local.", new object[]
			{
				typeof(TClient).Name,
				database.Name
			});
			return fallbackClientFactory(database.ActivationOrder.First<DirectoryServer>());
		}

		private readonly ILogger logger;

		private readonly LoadBalanceAnchorContext serviceContext;
	}
}
