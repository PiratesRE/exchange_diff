using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Injector;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceServiceBootstrapper : DisposeTrackableBase, IAnchorService, IDisposable
	{
		public LoadBalanceServiceBootstrapper()
		{
		}

		public LoadBalanceServiceBootstrapper(LoadBalanceAnchorContext anchorContext)
		{
			AnchorUtil.ThrowOnNullArgument(anchorContext, "anchorContext");
			((IAnchorService)this).Initialize(anchorContext);
		}

		public MailboxLoadBalanceService Service { get; private set; }

		public IEnumerable<IDiagnosable> GetDiagnosableComponents()
		{
			yield return new LoadBalanceServiceDiagnosable(this.anchorContext);
			yield return new LoadBalanceBandSettingsStorageDiagnosable(this.anchorContext);
			yield return new LoadBalanceTopologyDiagnosable(this.anchorContext);
			yield return this.anchorContext.Config;
			yield break;
		}

		public void Start()
		{
			MoveInjector moveInjector = this.anchorContext.MoveInjector;
			this.Service = new MailboxLoadBalanceService(this.anchorContext);
			this.logger.Log(MigrationEventType.Verbose, "Starting load balancer service.", new object[0]);
			LoadBalanceService serviceInstance = new LoadBalanceService(this.Service, this.anchorContext);
			this.EnableProvisioningCache();
			this.balancerHost = this.StartServiceEndpoint(serviceInstance, LoadBalanceService.EndpointAddress);
			this.logger.Log(MigrationEventType.Verbose, "Starting injector service.", new object[0]);
			this.injectorHost = this.StartServiceEndpoint(new InjectorService(this.directoryProvider, this.logger, moveInjector), InjectorService.EndpointAddress);
			this.logger.Log(MigrationEventType.Verbose, "Service started.", new object[0]);
		}

		bool IAnchorService.Initialize(AnchorContext anchorContext)
		{
			LoadBalanceAnchorContext loadBalanceAnchorContext = (LoadBalanceAnchorContext)anchorContext;
			this.anchorContext = loadBalanceAnchorContext;
			this.directoryProvider = loadBalanceAnchorContext.Directory;
			this.logger = loadBalanceAnchorContext.Logger;
			return loadBalanceAnchorContext.Settings.IsEnabled;
		}

		protected virtual void EnableProvisioningCache()
		{
			if (this.anchorContext.Settings.UseHeatMapProvisioning)
			{
				this.logger.Log(MigrationEventType.Verbose, "Initializing local forest heat map cache.", new object[0]);
				this.anchorContext.InitializeForestHeatMap();
			}
			else
			{
				this.logger.Log(MigrationEventType.Verbose, "Enabling provisioning cache.", new object[0]);
				ProvisioningCache.InitializeAppRegistrySettings("Powershell");
			}
			if (this.anchorContext.Settings.BuildLocalCacheOnStartup)
			{
				this.anchorContext.InitializeLocalServerHeatMap();
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			this.DisposeServiceHost(ref this.injectorHost);
			this.DisposeServiceHost(ref this.balancerHost);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<LoadBalanceServiceBootstrapper>(this);
		}

		private void DisposeServiceHost(ref ServiceHost serviceHost)
		{
			if (serviceHost != null)
			{
				serviceHost.Abort();
				serviceHost.Close();
				serviceHost = null;
			}
		}

		private ServiceHost StartServiceEndpoint(object serviceInstance, ServiceEndpointAddress serviceAddress)
		{
			AnchorUtil.ThrowOnNullArgument(serviceInstance, "serviceInstance");
			ServiceHost result;
			try
			{
				ServiceHost serviceHost = new ServiceHost(serviceInstance, serviceAddress.GetBaseUris());
				serviceHost.AddDefaultEndpoints();
				this.logger.Log(MigrationEventType.Verbose, "Opening service host for {0}, with service type {1} and namespace {2}.", new object[]
				{
					serviceHost.Description.Name,
					serviceHost.Description.ServiceType.FullName,
					serviceHost.Description.Namespace
				});
				ServiceDebugBehavior serviceDebugBehavior = serviceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
				if (serviceDebugBehavior == null)
				{
					serviceDebugBehavior = new ServiceDebugBehavior();
					serviceHost.Description.Behaviors.Add(serviceDebugBehavior);
				}
				serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
				foreach (System.ServiceModel.Description.ServiceEndpoint serviceEndpoint in serviceHost.Description.Endpoints)
				{
					NetTcpBinding netTcpBinding = serviceEndpoint.Binding as NetTcpBinding;
					if (netTcpBinding != null)
					{
						netTcpBinding.MaxReceivedMessageSize = 10485760L;
						netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10.0);
						netTcpBinding.SendTimeout = TimeSpan.FromMinutes(10.0);
					}
					this.logger.LogVerbose("Using binging: {0} ({1})", new object[]
					{
						serviceEndpoint.Binding.Name,
						serviceEndpoint.Binding.MessageVersion
					});
					LoadBalanceUtils.UpdateAndLogServiceEndpoint(this.logger, serviceEndpoint);
				}
				serviceHost.Open();
				result = serviceHost;
			}
			catch (Exception exception)
			{
				this.logger.LogError(exception, "Failed to register endpoint for service {0}", new object[]
				{
					serviceInstance.GetType().Name
				});
				throw;
			}
			return result;
		}

		private LoadBalanceAnchorContext anchorContext;

		private IDirectoryProvider directoryProvider;

		private ILogger logger;

		private ServiceHost balancerHost;

		private ServiceHost injectorHost;
	}
}
