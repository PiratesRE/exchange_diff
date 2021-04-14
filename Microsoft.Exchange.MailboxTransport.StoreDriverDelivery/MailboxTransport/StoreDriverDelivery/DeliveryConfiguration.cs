using System;
using Microsoft.Exchange.Data.Directory.IsMemberOfProvider;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Extensibility;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.MessageThrottling;
using Microsoft.Exchange.Transport.RecipientAPI;
using Microsoft.Exchange.Transport.ResourceMonitoring;
using Microsoft.Exchange.Transport.Storage.Messaging;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryConfiguration : IDeliveryConfiguration
	{
		private DeliveryConfiguration()
		{
			DeliveryConfiguration.components = new Components(string.Empty, false);
			DeliveryConfiguration.app = new AppConfig();
			this.isInitialized = false;
		}

		public static IDeliveryConfiguration Instance
		{
			get
			{
				if (DeliveryConfiguration.configuration == null)
				{
					DeliveryConfiguration.configuration = new DeliveryConfiguration();
				}
				return DeliveryConfiguration.configuration;
			}
			set
			{
				DeliveryConfiguration.configuration = value;
			}
		}

		public IAppConfiguration App
		{
			get
			{
				return DeliveryConfiguration.app;
			}
		}

		public DeliveryPoisonHandler PoisonHandler
		{
			get
			{
				return DeliveryConfiguration.poisonHandler;
			}
		}

		public IThrottlingConfig Throttling
		{
			get
			{
				return DeliveryConfiguration.throttlingConfig;
			}
		}

		public void Load(IMbxDeliveryListener submitHandler)
		{
			if (!this.isInitialized)
			{
				DeliveryConfiguration.app.Load();
				this.submitHandler = submitHandler;
				this.ConstructComponentLoadTree();
				DeliveryConfiguration.components.Start(new Components.StopServiceHandler(DeliveryConfiguration.OnStopServiceBecauseOfFailure), false, false, true, true);
				DeliveryConfiguration.components.Continue();
				MessageTrackingLog.Configure(Components.Configuration.LocalServer.TransportServer);
				LatencyTracker.Start(Components.TransportAppConfig.LatencyTracker, ProcessTransportRole.MailboxDelivery);
				this.isInitialized = true;
			}
		}

		public void Unload()
		{
			if (this.isInitialized)
			{
				DeliveryConfiguration.components.Stop();
				this.isInitialized = false;
			}
		}

		public void ConfigUpdate()
		{
			DeliveryConfiguration.components.ConfigUpdate();
		}

		private static void OnStopServiceBecauseOfFailure(string reason, bool canRetry, bool retryAlways, bool failServiceWithException)
		{
			Environment.Exit(1);
		}

		private void ConstructComponentLoadTree()
		{
			TransportAppConfig.IsMemberOfResolverConfiguration transportIsMemberOfResolverConfig = Components.TransportAppConfig.TransportIsMemberOfResolverConfig;
			IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver adAdapter = new IsMemberOfResolverADAdapter<RoutingAddress>.RoutingAddressResolver(transportIsMemberOfResolverConfig.DisableDynamicGroups);
			TransportAppConfig.IsMemberOfResolverConfiguration mailboxRulesIsMemberOfResolverConfig = Components.TransportAppConfig.MailboxRulesIsMemberOfResolverConfig;
			IsMemberOfResolverADAdapter<string>.LegacyDNResolver adAdapter2 = new IsMemberOfResolverADAdapter<string>.LegacyDNResolver(mailboxRulesIsMemberOfResolverConfig.DisableDynamicGroups);
			Components.AgentComponent = new AgentComponent();
			Components.RoutingComponent = new RoutingComponent();
			Components.EnhancedDns = new EnhancedDns();
			Components.DsnGenerator = new DsnGenerator();
			Components.UnhealthyTargetFilterComponent = new UnhealthyTargetFilterComponent();
			Components.TransportIsMemberOfResolverComponent = new IsMemberOfResolverComponent<RoutingAddress>("Transport", transportIsMemberOfResolverConfig, adAdapter);
			Components.MailboxRulesIsMemberOfResolverComponent = new IsMemberOfResolverComponent<string>("MailboxRules", mailboxRulesIsMemberOfResolverConfig, adAdapter2);
			Components.StoreDriverDelivery = StoreDriverDelivery.CreateStoreDriver();
			Components.Categorizer = this.submitHandler;
			Components.CertificateComponent = new CertificateComponent();
			Components.Configuration = new ConfigurationComponent(ProcessTransportRole.MailboxDelivery);
			Components.MessageThrottlingComponent = new MessageThrottlingComponent();
			Components.ResourceManagerComponent = new ResourceManagerComponent(ResourceManagerResources.PrivateBytes | ResourceManagerResources.TotalBytes);
			Components.SmtpInComponent = new SmtpInComponent(this.IsModernSmtpStackEnabled());
			Components.SmtpOutConnectionHandler = new SmtpOutConnectionHandler();
			Components.SystemCheckComponent = new SystemCheckComponent();
			Components.TransportMailItemLoader item = new Components.TransportMailItemLoader();
			Components.ProxyHubSelectorComponent = new ProxyHubSelectorComponent();
			Components.PoisonMessageComponent = new PoisonMessage();
			Components.Logging = new Components.LoggingComponent(false, false, false, false, false);
			StorageFactory.SchemaToUse = StorageFactory.Schema.NullSchema;
			Components.MessagingDatabase = new MessagingDatabaseComponent();
			Components.PerfCountersLoader perfCountersLoader = new Components.PerfCountersLoader(false);
			Components.PerfCountersLoaderComponent = perfCountersLoader;
			Components.ResourceThrottlingComponent = new ResourceThrottlingComponent(new ResourceMeteringConfig(8000, null), new ResourceThrottlingConfig(null), new ComponentsWrapper(), Components.MessagingDatabase, null, Components.Configuration, ResourceManagerResources.PrivateBytes | ResourceManagerResources.TotalBytes, ResourceObservingComponents.EnhancedDns | ResourceObservingComponents.IsMemberOfResolver | ResourceObservingComponents.SmtpIn);
			ParallelTransportComponent parallelTransportComponent = new ParallelTransportComponent("Parallel Group 1");
			parallelTransportComponent.TransportComponents.Add(Components.ResourceManagerComponent);
			parallelTransportComponent.TransportComponents.Add(Components.CertificateComponent);
			parallelTransportComponent.TransportComponents.Add(Components.TransportIsMemberOfResolverComponent);
			parallelTransportComponent.TransportComponents.Add(Components.MailboxRulesIsMemberOfResolverComponent);
			ParallelTransportComponent parallelTransportComponent2 = new ParallelTransportComponent("Parallel Group 2");
			parallelTransportComponent2.TransportComponents.Add(item);
			parallelTransportComponent2.TransportComponents.Add(new Components.MicrosoftExchangeRecipientLoader());
			parallelTransportComponent2.TransportComponents.Add(perfCountersLoader);
			parallelTransportComponent2.TransportComponents.Add(Components.Logging);
			parallelTransportComponent2.TransportComponents.Add(Components.PoisonMessageComponent);
			parallelTransportComponent2.TransportComponents.Add(Components.MessageThrottlingComponent);
			parallelTransportComponent2.TransportComponents.Add(Components.StoreDriverDelivery);
			parallelTransportComponent2.TransportComponents.Add((ITransportComponent)Components.AgentComponent);
			ParallelTransportComponent parallelTransportComponent3 = new ParallelTransportComponent("Parallel Group 3");
			parallelTransportComponent3.TransportComponents.Add(Components.SmtpInComponent);
			parallelTransportComponent3.TransportComponents.Add(Components.SmtpOutConnectionHandler);
			parallelTransportComponent3.TransportComponents.Add(Components.RoutingComponent);
			parallelTransportComponent3.TransportComponents.Add(Components.UnhealthyTargetFilterComponent);
			parallelTransportComponent3.TransportComponents.Add(Components.DsnGenerator);
			parallelTransportComponent3.TransportComponents.Add(DeliveryConfiguration.poisonHandler);
			Components.SetRootComponent(new SequentialTransportComponent("Root Component")
			{
				TransportComponents = 
				{
					(ITransportComponent)Components.Configuration,
					Components.SystemCheckComponent,
					parallelTransportComponent,
					parallelTransportComponent2,
					parallelTransportComponent3,
					Components.ResourceThrottlingComponent,
					Components.ProxyHubSelectorComponent
				}
			});
		}

		private bool IsModernSmtpStackEnabled()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			return snapshot != null && snapshot.MailboxTransport.MailboxTransportSmtpIn.Enabled;
		}

		private static Components components;

		private static IAppConfiguration app;

		private static IDeliveryConfiguration configuration;

		private static DeliveryPoisonHandler poisonHandler = new DeliveryPoisonHandler(Components.TransportAppConfig.PoisonMessage.CrashDetectionWindow, DeliveryConfiguration.Instance.App.PoisonRegistryEntryMaxCount);

		private static IThrottlingConfig throttlingConfig = ThrottlingConfigFactory.Create();

		private bool isInitialized;

		private IMbxDeliveryListener submitHandler;
	}
}
