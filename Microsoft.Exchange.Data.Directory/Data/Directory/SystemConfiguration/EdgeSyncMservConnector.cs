using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Mserve;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class EdgeSyncMservConnector : EdgeSyncConnector
	{
		[Parameter(Mandatory = false)]
		public Uri ProvisionUrl
		{
			get
			{
				return (Uri)this[EdgeSyncMservConnectorSchema.ProvisionUrl];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.ProvisionUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri SettingUrl
		{
			get
			{
				return (Uri)this[EdgeSyncMservConnectorSchema.SettingUrl];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.SettingUrl] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LocalCertificate
		{
			get
			{
				return (string)this[EdgeSyncMservConnectorSchema.LocalCertificate];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.LocalCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string RemoteCertificate
		{
			get
			{
				return (string)this[EdgeSyncMservConnectorSchema.RemoteCertificate];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.RemoteCertificate] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PrimaryLeaseLocation
		{
			get
			{
				return (string)this[EdgeSyncMservConnectorSchema.PrimaryLeaseLocation];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.PrimaryLeaseLocation] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string BackupLeaseLocation
		{
			get
			{
				return (string)this[EdgeSyncMservConnectorSchema.BackupLeaseLocation];
			}
			set
			{
				this[EdgeSyncMservConnectorSchema.BackupLeaseLocation] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return EdgeSyncMservConnector.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchEdgeSyncMservConnector";
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		internal static string GetMserveWebServiceClientTokenFromEndpointConfig(ITopologyConfigurationSession configSession)
		{
			if (configSession == null)
			{
				configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 153, "GetMserveWebServiceClientTokenFromEndpointConfig", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\EdgeSyncMservConnector.cs");
			}
			ServiceEndpointContainer endpointContainer = configSession.GetEndpointContainer();
			ServiceEndpoint endpoint = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerProvision);
			return endpoint.Token;
		}

		internal static MserveWebService CreateDefaultMserveWebService(string domainController)
		{
			return EdgeSyncMservConnector.CreateDefaultMserveWebService(null, false);
		}

		internal static MserveWebService CreateDefaultMserveWebService(string domainController, bool batchMode)
		{
			return EdgeSyncMservConnector.CreateDefaultMserveWebService(domainController, batchMode, 0);
		}

		internal static MserveWebService CreateDefaultMserveWebService(string domainController, bool batchMode, int initialChunkSize)
		{
			ITopologyConfigurationSession rootOrgSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 200, "CreateDefaultMserveWebService", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\EdgeSyncMservConnector.cs");
			EdgeSyncServiceConfig config = null;
			string clientToken = null;
			ADSite localSite = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localSite = rootOrgSession.GetLocalSite();
				if (localSite == null)
				{
					throw new TransientException(DirectoryStrings.CannotGetLocalSite);
				}
				config = rootOrgSession.Read<EdgeSyncServiceConfig>(localSite.Id.GetChildId("EdgeSyncService"));
				clientToken = EdgeSyncMservConnector.GetMserveWebServiceClientTokenFromEndpointConfig(rootOrgSession);
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				throw adoperationResult.Exception;
			}
			if (config == null)
			{
				throw new MserveException(string.Format("No EdgeSync configuration found. Site {0}", localSite.DistinguishedName));
			}
			if (string.IsNullOrEmpty(clientToken))
			{
				throw new InvalidOperationException(string.Format("clientToken from Endpoint configuration is null or empty . Site {0}", localSite.DistinguishedName));
			}
			List<EdgeSyncMservConnector> connectors = new List<EdgeSyncMservConnector>();
			if (!ADNotificationAdapter.TryReadConfigurationPaged<EdgeSyncMservConnector>(() => rootOrgSession.FindPaged<EdgeSyncMservConnector>(config.Id, QueryScope.SubTree, null, null, 0), delegate(EdgeSyncMservConnector connector)
			{
				connectors.Add(connector);
			}, 3, out adoperationResult))
			{
				throw adoperationResult.Exception;
			}
			if (connectors.Count == 0)
			{
				throw new InvalidOperationException(string.Format("No MServ configuration found. Site {0}", localSite.DistinguishedName));
			}
			MserveWebService mserveWebService = new MserveWebService(connectors[0].ProvisionUrl.AbsoluteUri, connectors[0].SettingUrl.AbsoluteUri, connectors[0].RemoteCertificate, clientToken, batchMode);
			mserveWebService.Initialize(initialChunkSize);
			return mserveWebService;
		}

		internal static int GetMserveEntryTenantNegoConfig(string domainName)
		{
			IGlobalDirectorySession globalSession = DirectorySessionFactory.GetGlobalSession(null);
			bool flag;
			if (!globalSession.TryGetDomainFlag(domainName, GlsDomainFlags.Nego2Enabled, out flag))
			{
				return -1;
			}
			if (!flag)
			{
				return 0;
			}
			return 1;
		}

		internal static string GetRedirectServer(string redirectFormat, Guid orgId, int currentSiteId, int startRange, int endRange)
		{
			return EdgeSyncMservConnector.GetRedirectServer(redirectFormat, orgId, currentSiteId, startRange, endRange, false, false);
		}

		internal static string GetRedirectServer(string redirectFormat, Guid orgId, int currentSiteId, int startRange, int endRange, bool overrideCurrentSiteCheck, bool throwExceptions)
		{
			string result;
			try
			{
				IGlobalDirectorySession globalSession = DirectorySessionFactory.GetGlobalSession(redirectFormat);
				result = globalSession.GetRedirectServer(orgId);
			}
			catch (MServTransientException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (MServPermanentException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (InvalidOperationException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (TransientException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			return result;
		}

		internal static string GetRedirectServer(string redirectFormat, string address, int currentSiteId, int startRange, int endRange)
		{
			return EdgeSyncMservConnector.GetRedirectServer(redirectFormat, address, currentSiteId, startRange, endRange, false, false);
		}

		internal static string GetRedirectServer(string redirectFormat, string address, int currentSiteId, int startRange, int endRange, bool overrideCurrentSiteCheck, bool throwExceptions)
		{
			string result;
			try
			{
				IGlobalDirectorySession globalSession = DirectorySessionFactory.GetGlobalSession(redirectFormat);
				result = globalSession.GetRedirectServer(address);
			}
			catch (MServTransientException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (MServPermanentException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (InvalidOperationException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			catch (TransientException)
			{
				if (throwExceptions)
				{
					throw;
				}
				result = string.Empty;
			}
			return result;
		}

		internal static string GetRedirectServerFromPartnerId(string redirectFormat, int partnerId, int currentSiteId, int startRange, int endRange, Trace tracer)
		{
			return EdgeSyncMservConnector.GetRedirectServerFromPartnerId(redirectFormat, partnerId, currentSiteId, startRange, endRange, tracer, false, false);
		}

		internal static string GetRedirectServerFromPartnerId(string redirectFormat, int partnerId, int currentSiteId, int startRange, int endRange, Trace tracer, bool overrideCurrentSiteCheck, bool throwExceptions)
		{
			if (startRange > endRange)
			{
				throw new InvalidOperationException(string.Format("startRange: {0} greater than endRange: {1}", startRange, endRange));
			}
			if (string.IsNullOrEmpty(redirectFormat))
			{
				return string.Empty;
			}
			if (partnerId == -1 || partnerId < startRange || partnerId > endRange)
			{
				if (partnerId == -1)
				{
					string message = string.Format("The partner id {0} is invalid", partnerId);
					EdgeSyncMservConnector.TraceError(tracer, message);
					if (throwExceptions)
					{
						throw new InvalidPartnerIdException(message);
					}
				}
				else
				{
					string message2 = string.Format("The partner id {0} is out of range", partnerId);
					EdgeSyncMservConnector.TraceError(tracer, message2);
					if (throwExceptions)
					{
						throw new InvalidOperationException(message2);
					}
				}
				return string.Empty;
			}
			if (partnerId == currentSiteId && !overrideCurrentSiteCheck)
			{
				EdgeSyncMservConnector.TraceDebug(tracer, string.Format("The partner id {0} is the same as the current site id", partnerId));
				return string.Empty;
			}
			return string.Format(CultureInfo.InvariantCulture, redirectFormat, new object[]
			{
				partnerId
			});
		}

		private static void TraceDebug(Trace tracer, string message)
		{
			if (tracer != null)
			{
				tracer.TraceDebug(0L, message);
			}
		}

		private static void TraceError(Trace tracer, string message)
		{
			if (tracer != null)
			{
				tracer.TraceError(0L, message);
			}
		}

		internal const string MostDerivedClass = "msExchEdgeSyncMservConnector";

		private const char CommonNameSeperatorChar = '/';

		public const string ArchiveAddressDomainSuffix = "@archive.exchangelabs.com";

		private static readonly EdgeSyncMservConnectorSchema schema = ObjectSchema.GetInstance<EdgeSyncMservConnectorSchema>();
	}
}
