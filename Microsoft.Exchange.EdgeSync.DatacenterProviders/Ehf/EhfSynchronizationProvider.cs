using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Ehf;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfSynchronizationProvider : SynchronizationProvider
	{
		public override string Identity
		{
			get
			{
				if (this.identity == null)
				{
					throw new InvalidOperationException("EhfSynchronizationProvider.Identity property is accessed before EhfSynchronizationProvider.Initialize() method is invoked.");
				}
				return this.identity;
			}
		}

		public override int LeaseLockTryCount
		{
			get
			{
				return 1;
			}
		}

		public override List<TargetServerConfig> TargetServerConfigs
		{
			get
			{
				if (this.targetServerConfigs == null)
				{
					throw new InvalidOperationException("EhfSynchronizationProvider.TargetServerConfigs property is accessed before EhfSynchronizationProvider.Initialize() method is invoked.");
				}
				return this.targetServerConfigs;
			}
		}

		public override EnhancedTimeSpan RecipientSyncInterval
		{
			get
			{
				return this.adminSyncInterval;
			}
		}

		public override EnhancedTimeSpan ConfigurationSyncInterval
		{
			get
			{
				return EdgeSyncSvc.EdgeSync.Config.ServiceConfig.ConfigurationSyncInterval;
			}
		}

		public EhfSyncErrorTracker AdminSyncErrorTracker
		{
			get
			{
				return this.adminSyncErrorTracker;
			}
		}

		public static void ValidateProvisioningUrl(Uri url, PSCredential authCredential, string connectorId)
		{
			string text = null;
			if (url == null)
			{
				text = "is not specified; an absolute URI must be specified";
			}
			else if (!url.IsAbsoluteUri)
			{
				text = "is not absolute; an absolute URI must be specified";
			}
			else
			{
				bool flag = authCredential != null && !string.IsNullOrEmpty(authCredential.UserName);
				string scheme = url.Scheme;
				if (scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
				{
					if (flag)
					{
						text = "has the 'http' scheme that is not allowed when authentication credentials are provided; the 'https' scheme is expected";
					}
				}
				else if (scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
				{
					if (!flag)
					{
						text = "has the 'https' scheme that is not allowed when authentication credentials are not provided; the 'http' scheme is expected";
					}
				}
				else
				{
					text = "has an unsupported scheme; only 'http' and 'https' schemes are allowed";
				}
			}
			if (text != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "EHF connector <{0}>: Provisioning URL <{1}> {2}", new object[]
				{
					connectorId,
					url ?? string.Empty,
					text
				});
				EhfSynchronizationProvider.tracer.TraceError<string>(0L, "{0}", text);
				throw new ExDirectoryException(new ArgumentException(text));
			}
		}

		public static int GetResellerId(EdgeSyncEhfConnector connector)
		{
			int result;
			if (!int.TryParse(connector.ResellerId, out result))
			{
				string text = string.Format(CultureInfo.InvariantCulture, "EHF connector <{0}> conatins invalid Reseller ID <{1}>; reseller ID must be an integer value", new object[]
				{
					connector.DistinguishedName,
					connector.ResellerId ?? "null"
				});
				EhfSynchronizationProvider.tracer.TraceError<string>((long)connector.GetHashCode(), "{0}", text);
				throw new ExDirectoryException(new FormatException(text));
			}
			return result;
		}

		public override void Initialize(EdgeSyncConnector connector)
		{
			EdgeSyncEhfConnector connector2 = (EdgeSyncEhfConnector)connector;
			this.InitializeIdentity(connector2);
			this.InitializeTargetServerConfig(connector2);
		}

		public override List<TypeSynchronizer> CreateTypeSynchronizer(SyncTreeType type)
		{
			List<TypeSynchronizer> list = new List<TypeSynchronizer>();
			if (this.ehfWebServiceVersion != EhfWebServiceVersion.Version1 && this.ehfWebServiceVersion != EhfWebServiceVersion.Version2)
			{
				EdgeSyncEvents.Log.LogEvent(EdgeSyncEventLogConstants.Tuple_EhfWebServiceVersionIsNotSupported, null, new object[]
				{
					this.ehfWebServiceVersion,
					this.identity
				});
				return list;
			}
			if (type == SyncTreeType.Configuration)
			{
				list.Add(EhfCompanySynchronizer.CreateTypeSynchronizer());
				list.Add(EhfDomainSynchronizer.CreateTypeSynchronizer());
			}
			else if (type == SyncTreeType.Recipients)
			{
				list.Add(EhfAdminAccountSynchronizer.CreateTypeSynchronizer());
			}
			return list;
		}

		public override TargetConnection CreateTargetConnection(TargetServerConfig targetServerConfig, SyncTreeType type, TestShutdownAndLeaseDelegate testShutdownAndLease, EdgeSyncLogSession logSession)
		{
			if (type == SyncTreeType.Configuration)
			{
				return new EhfConfigTargetConnection(EdgeSyncSvc.EdgeSync.Topology.LocalServer.VersionNumber, (EhfTargetServerConfig)targetServerConfig, this.ConfigurationSyncInterval, logSession);
			}
			if (type == SyncTreeType.Recipients)
			{
				return new EhfRecipientTargetConnection(EdgeSyncSvc.EdgeSync.Topology.LocalServer.VersionNumber, (EhfTargetServerConfig)targetServerConfig, this, logSession);
			}
			throw new NotSupportedException("Only config and recipient synchronization is supported by EHF sync provider");
		}

		private void InitializeIdentity(EdgeSyncEhfConnector connector)
		{
			ADObjectId id = connector.Id;
			if (id.Depth > 2)
			{
				string name = id.AncestorDN(2).Name;
				this.identity = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
				{
					name,
					connector.Name
				});
			}
			else
			{
				this.identity = id.DistinguishedName;
			}
			EhfSynchronizationProvider.tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Initialized provider identity to <{0}> based on the connector DN <{1}>", this.identity, id.DistinguishedName);
		}

		private void InitializeTargetServerConfig(EdgeSyncEhfConnector connector)
		{
			EhfTargetServerConfig ehfTargetServerConfig = new EhfTargetServerConfig(connector, EdgeSyncSvc.EdgeSync.Topology.LocalServer.InternetWebProxy);
			this.targetServerConfigs = new List<TargetServerConfig>(1);
			this.targetServerConfigs.Add(ehfTargetServerConfig);
			this.ehfWebServiceVersion = ehfTargetServerConfig.EhfWebServiceVersion;
			this.adminSyncInterval = ehfTargetServerConfig.EhfSyncAppConfig.EhfAdminSyncInterval;
			this.adminSyncErrorTracker.Initialize(ehfTargetServerConfig.EhfSyncAppConfig);
			EhfSynchronizationProvider.tracer.TraceDebug((long)this.GetHashCode(), "Initialized target server configuration for connector: <{0}> ProvisioningUrl: <{1}> PrimaryLeaseLocation: <{2}> BackupLeaseLocation: <{3}> Version: <{4}>", new object[]
			{
				ehfTargetServerConfig.Name,
				ehfTargetServerConfig.ProvisioningUrl,
				ehfTargetServerConfig.PrimaryLeaseLocation,
				ehfTargetServerConfig.BackupLeaseLocation,
				connector.Version
			});
		}

		private const int LeaseLockTryCountValue = 1;

		private static Trace tracer = ExTraceGlobals.ProviderTracer;

		private string identity;

		private TimeSpan adminSyncInterval;

		private List<TargetServerConfig> targetServerConfigs;

		private EhfWebServiceVersion ehfWebServiceVersion;

		private EhfSyncErrorTracker adminSyncErrorTracker = new EhfSyncErrorTracker();
	}
}
