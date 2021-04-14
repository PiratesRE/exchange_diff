using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.ShadowRedundancy;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class RoutingDependencies
	{
		public RoutingDependencies(TransportAppConfig appConfig, ITransportConfiguration transportConfig)
		{
			this.appConfig = appConfig;
			this.transportConfig = transportConfig;
		}

		public ShadowRedundancyComponent ShadowRedundancy
		{
			set
			{
				this.shadowRedundancy = value;
			}
		}

		public UnhealthyTargetFilterComponent UnhealthyTargetFilter
		{
			set
			{
				this.unhealthyTargetFilter = value;
			}
		}

		public CategorizerComponent Categorizer
		{
			set
			{
				this.categorizer = value;
			}
		}

		public virtual bool IsProcessShuttingDown
		{
			get
			{
				return Components.ShuttingDown;
			}
		}

		public virtual string LocalComputerFqdn
		{
			get
			{
				return RoutingDependencies.localComputerFqdn;
			}
		}

		public virtual LocalLongFullPath LogPath
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
				return this.transportConfig.LocalServer.TransportServer.RoutingTableLogPath;
			}
		}

		public virtual Unlimited<ByteQuantifiedSize> MaxLogDirectorySize
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
				return this.transportConfig.LocalServer.TransportServer.RoutingTableLogMaxDirectorySize;
			}
		}

		public virtual EnhancedTimeSpan MaxLogFileAge
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
				return this.transportConfig.LocalServer.TransportServer.RoutingTableLogMaxAge;
			}
		}

		public virtual ProcessTransportRole ProcessTransportRole
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
				return this.transportConfig.ProcessTransportRole;
			}
		}

		public virtual bool ShadowRedundancyPromotionEnabled
		{
			get
			{
				RoutingUtils.ThrowIfMissingDependency(this.appConfig, "AppConfig");
				RoutingUtils.ThrowIfMissingDependency(this.shadowRedundancy, "Shadow Redundancy");
				return this.shadowRedundancy.ShadowRedundancyManager.Configuration.Enabled && this.appConfig.ShadowRedundancy.ShadowRedundancyPromotionEnabled;
			}
		}

		public virtual bool IsUnhealthyFqdn(string fqdn, ushort port)
		{
			RoutingUtils.ThrowIfMissingDependency(this.unhealthyTargetFilter, "Unhealthy Target Filter");
			return this.unhealthyTargetFilter.UnhealthyTargetFqdnFilter.IsUnhealthy(new FqdnPortPair(fqdn, port));
		}

		public virtual void RegisterForLocalServerChanges(ConfigurationUpdateHandler<TransportServerConfiguration> handler)
		{
			RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
			this.transportConfig.LocalServerChanged += handler;
		}

		public virtual void RegisterForServiceControlConfigUpdates(EventHandler handler)
		{
			Components.ConfigChanged += handler;
		}

		public virtual bool ShouldShadowMailItem(IReadOnlyMailItem mailItem)
		{
			RoutingUtils.ThrowIfMissingDependency(this.shadowRedundancy, "Shadow Redundancy");
			return this.shadowRedundancy.ShadowRedundancyManager.ShouldShadowMailItem(mailItem);
		}

		public virtual bool TryDeencapsulate(RoutingAddress address, out ProxyAddress innerAddress)
		{
			return Resolver.TryDeencapsulate(address, out innerAddress);
		}

		public virtual bool TryGetServerForDatabase(Guid databaseGuid, out string fqdn)
		{
			fqdn = null;
			ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
			GetServerForDatabaseFlags getServerForDatabaseFlags = GetServerForDatabaseFlags.BasicQuery;
			if (this.appConfig.Routing.DagRoutingEnabled)
			{
				getServerForDatabaseFlags |= GetServerForDatabaseFlags.IgnoreAdSiteBoundary;
			}
			DatabaseLocationInfo databaseLocationInfo = null;
			Exception ex = null;
			try
			{
				databaseLocationInfo = cachingActiveManagerInstance.GetServerForDatabase(databaseGuid, getServerForDatabaseFlags);
			}
			catch (DatabaseNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			catch (StorageTransientException ex4)
			{
				ex = ex4;
			}
			if (databaseLocationInfo == null || string.IsNullOrEmpty(databaseLocationInfo.ServerFqdn))
			{
				RoutingDiag.Tracer.TraceError<Guid, object>((long)this.GetHashCode(), "Failed to obtain active server information for database <{0}>; exception details: {1}", databaseGuid, ex ?? "<null>");
				return false;
			}
			fqdn = databaseLocationInfo.ServerFqdn;
			return true;
		}

		public virtual void UnregisterFromLocalServerChanges(ConfigurationUpdateHandler<TransportServerConfiguration> handler)
		{
			RoutingUtils.ThrowIfMissingDependency(this.transportConfig, "Configuration");
			this.transportConfig.LocalServerChanged -= handler;
		}

		public virtual void UnregisterFromServiceControlConfigUpdates(EventHandler handler)
		{
			Components.ConfigChanged -= handler;
		}

		public virtual void BifurcateRecipientsAndDefer(TransportMailItem mailItem, ICollection<MailRecipient> recipientsToBeForked, TaskContext taskContext, SmtpResponse deferResponse, TimeSpan deferTime, DeferReason deferReason)
		{
			RoutingUtils.ThrowIfMissingDependency(this.categorizer, "Categorizer");
			this.categorizer.BifurcateRecipientsAndDefer(mailItem, recipientsToBeForked, taskContext, deferResponse, deferTime, deferReason);
		}

		private static readonly string localComputerFqdn = ComputerInformation.DnsFullyQualifiedDomainName;

		private TransportAppConfig appConfig;

		private ITransportConfiguration transportConfig;

		private ShadowRedundancyComponent shadowRedundancy;

		private UnhealthyTargetFilterComponent unhealthyTargetFilter;

		private CategorizerComponent categorizer;
	}
}
