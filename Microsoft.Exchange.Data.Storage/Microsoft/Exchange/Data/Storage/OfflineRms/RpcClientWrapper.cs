using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.OfflineRms;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RpcClientWrapper
	{
		private RpcClientWrapper()
		{
		}

		private static void InitializeIfNeeded()
		{
			if (!RpcClientWrapper.instance.initialized)
			{
				lock (RpcClientWrapper.instance.initializeLockObject)
				{
					if (!RpcClientWrapper.instance.initialized)
					{
						RpcClientWrapper.instance.Initialize();
						RpcClientWrapper.instance.initialized = true;
					}
				}
			}
			bool flag2 = DateTime.UtcNow - RpcClientWrapper.instance.topologyLastUpdated > RpcClientWrapper.topologyExpiryTimeSpan;
			if (flag2)
			{
				Exception ex = null;
				RpcClientWrapper.instance.TryLoadTopologies(out ex);
			}
		}

		public static ActiveCryptoModeRpcResult[] GetTenantActiveCryptoMode(RmsClientManagerContext clientContext)
		{
			if (clientContext == null)
			{
				throw new ArgumentNullException("clientContext");
			}
			RpcClientWrapper.InitializeIfNeeded();
			string randomRpcTargetServerName = RpcClientWrapper.instance.GetRandomRpcTargetServerName();
			byte[] data = null;
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Entry, clientContext, string.Format("OfflineRmsRpcClient.GetTenantActiveCryptoMode against RPC server {0}", randomRpcTargetServerName));
			try
			{
				using (OfflineRmsRpcClient offlineRmsRpcClient = new OfflineRmsRpcClient(randomRpcTargetServerName))
				{
					data = offlineRmsRpcClient.GetTenantActiveCryptoMode(1, new GetTenantActiveCryptoModeRpcParameters(clientContext).Serialize());
				}
			}
			catch (RpcException ex)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.GetTenantActiveCryptoMode against RPC server {0} failed with RPC Exception {1}", randomRpcTargetServerName, ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.RpcClientException("GetTenantActiveCryptoMode", randomRpcTargetServerName), ex, false);
			}
			GetTenantActiveCryptoModeRpcResults getTenantActiveCryptoModeRpcResults = new GetTenantActiveCryptoModeRpcResults(data);
			if (getTenantActiveCryptoModeRpcResults.OverallRpcResult.Status == OverallRpcStatus.Success)
			{
				return getTenantActiveCryptoModeRpcResults.ActiveCryptoModeRpcResults;
			}
			string serializedString = ErrorResult.GetSerializedString(getTenantActiveCryptoModeRpcResults.OverallRpcResult.ErrorResults);
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.GetTenantActiveCryptoMode against RPC server {0} failed with WellKnownErrorCode {1}, and with Exception {2}", randomRpcTargetServerName, getTenantActiveCryptoModeRpcResults.OverallRpcResult.WellKnownErrorCode, serializedString));
			throw new RightsManagementServerException(ServerStrings.FailedToRpcAcquireUseLicenses(clientContext.OrgId.ToString(), serializedString, randomRpcTargetServerName), getTenantActiveCryptoModeRpcResults.OverallRpcResult.WellKnownErrorCode, getTenantActiveCryptoModeRpcResults.OverallRpcResult.Status == OverallRpcStatus.PermanentFailure);
		}

		public static UseLicenseRpcResult[] AcquireUseLicenses(RmsClientManagerContext clientContext, XmlNode[] rightsAccountCertificate, XmlNode[] issuanceLicense, LicenseeIdentity[] licenseeIdentities)
		{
			if (clientContext == null)
			{
				throw new ArgumentNullException("clientContext");
			}
			if (rightsAccountCertificate == null)
			{
				throw new ArgumentNullException("rightsAccountCertificate");
			}
			if (issuanceLicense == null || issuanceLicense.Length < 1)
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			if (licenseeIdentities == null || licenseeIdentities.Length < 1)
			{
				throw new ArgumentNullException("licenseeIdentities");
			}
			RpcClientWrapper.InitializeIfNeeded();
			string randomRpcTargetServerName = RpcClientWrapper.instance.GetRandomRpcTargetServerName();
			byte[] data = null;
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Entry, clientContext, string.Format("OfflineRmsRpcClient.AcquireUseLicenses against RPC server {0}", randomRpcTargetServerName));
			try
			{
				using (OfflineRmsRpcClient offlineRmsRpcClient = new OfflineRmsRpcClient(randomRpcTargetServerName))
				{
					data = offlineRmsRpcClient.AcquireUseLicenses(1, new AcquireUseLicensesRpcParameters(clientContext, rightsAccountCertificate, issuanceLicense, licenseeIdentities).Serialize());
				}
			}
			catch (RpcException ex)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.AcquireUseLicenses against RPC server {0} failed with RPC Exception {1}", randomRpcTargetServerName, ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.RpcClientException("AcquireUseLicenses", randomRpcTargetServerName), ex, false);
			}
			AcquireUseLicensesRpcResults acquireUseLicensesRpcResults = new AcquireUseLicensesRpcResults(data);
			if (acquireUseLicensesRpcResults.OverallRpcResult.Status == OverallRpcStatus.Success)
			{
				return acquireUseLicensesRpcResults.UseLicenseRpcResults;
			}
			string serializedString = ErrorResult.GetSerializedString(acquireUseLicensesRpcResults.OverallRpcResult.ErrorResults);
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.AcquireUseLicenses against RPC server {0} failed with WellKnownErrorCode {1}, and with Exception {2}", randomRpcTargetServerName, acquireUseLicensesRpcResults.OverallRpcResult.WellKnownErrorCode, serializedString));
			throw new RightsManagementServerException(ServerStrings.FailedToRpcAcquireUseLicenses(clientContext.OrgId.ToString(), serializedString, randomRpcTargetServerName), acquireUseLicensesRpcResults.OverallRpcResult.WellKnownErrorCode, acquireUseLicensesRpcResults.OverallRpcResult.Status == OverallRpcStatus.PermanentFailure);
		}

		public static void AcquireTenantLicenses(RmsClientManagerContext clientContext, XmlNode[] machineCertificateChain, string identity, out XmlNode[] racXml, out XmlNode[] clcXml)
		{
			if (clientContext == null)
			{
				throw new ArgumentNullException("clientContext");
			}
			if (machineCertificateChain == null || machineCertificateChain.Length < 1)
			{
				throw new ArgumentNullException("machineCertificateChain");
			}
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("identity");
			}
			racXml = null;
			clcXml = null;
			RpcClientWrapper.InitializeIfNeeded();
			string randomRpcTargetServerName = RpcClientWrapper.instance.GetRandomRpcTargetServerName();
			byte[] data = null;
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Entry, clientContext, string.Format("OfflineRmsRpcClient.AcquireTenantLicenses against RPC server {0}", randomRpcTargetServerName));
			try
			{
				using (OfflineRmsRpcClient offlineRmsRpcClient = new OfflineRmsRpcClient(randomRpcTargetServerName))
				{
					data = offlineRmsRpcClient.AcquireTenantLicenses(1, new AcquireTenantLicensesRpcParameters(clientContext, identity, machineCertificateChain).Serialize());
				}
			}
			catch (RpcException ex)
			{
				ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.AcquireTenantLicenses against RPC server {0} failed with RPC Exception {1}", randomRpcTargetServerName, ServerManagerLog.GetExceptionLogString(ex, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
				throw new RightsManagementServerException(ServerStrings.RpcClientException("AcquireTenantLicenses", randomRpcTargetServerName), ex, false);
			}
			AcquireTenantLicensesRpcResults acquireTenantLicensesRpcResults = new AcquireTenantLicensesRpcResults(data);
			if (acquireTenantLicensesRpcResults.OverallRpcResult.Status == OverallRpcStatus.Success)
			{
				racXml = acquireTenantLicensesRpcResults.RacXml;
				clcXml = acquireTenantLicensesRpcResults.ClcXml;
				return;
			}
			string serializedString = ErrorResult.GetSerializedString(acquireTenantLicensesRpcResults.OverallRpcResult.ErrorResults);
			ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, clientContext, string.Format("OfflineRmsRpcClient.AcquireTenantLicenses against RPC server {0} failed with WellKnownErrorCode {1} and with Exception {2}", randomRpcTargetServerName, acquireTenantLicensesRpcResults.OverallRpcResult.WellKnownErrorCode, serializedString));
			throw new RightsManagementServerException(ServerStrings.FailedToRpcAcquireRacAndClc(clientContext.OrgId.ToString(), serializedString, randomRpcTargetServerName), acquireTenantLicensesRpcResults.OverallRpcResult.WellKnownErrorCode, acquireTenantLicensesRpcResults.OverallRpcResult.Status == OverallRpcStatus.PermanentFailure);
		}

		private void Initialize()
		{
			Exception innerException = null;
			if (!this.TryLoadTopologies(out innerException))
			{
				throw new RightsManagementServerException(ServerStrings.RpcClientWrapperFailedToLoadTopology, innerException, false);
			}
		}

		private string GetRandomRpcTargetServerName()
		{
			if ((this.localServer.CurrentServerRole & ServerRole.HubTransport) == ServerRole.HubTransport)
			{
				return this.localServer.Name;
			}
			if (this.localSiteBridgeheadServers == null || this.localSiteBridgeheadServers.Count == 0)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToFindAvailableHubs, false);
			}
			int index = this.random.Next(0, this.localSiteBridgeheadServers.Count - 1);
			return this.localSiteBridgeheadServers[index].Name;
		}

		private bool TryLoadTopologies(out Exception e)
		{
			bool result;
			try
			{
				e = null;
				if (Interlocked.Increment(ref this.loadTopologyCount) == 1)
				{
					ADOperationResult adoperationResult;
					if (this.localServer == null)
					{
						adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							this.localServer = this.rootOrgConfigSession.FindLocalServer();
						}, 3);
						if (!adoperationResult.Succeeded)
						{
							e = adoperationResult.Exception;
							ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, null, string.Format("Failed find local server with Exception {0}", ServerManagerLog.GetExceptionLogString(e, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
							return false;
						}
						if (this.localServer.ServerSite == null)
						{
							ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, null, "Local server doesn't have AD site");
							return false;
						}
						this.localSiteHubsFilter = new AndFilter(new QueryFilter[]
						{
							new BitMaskAndFilter(ServerSchema.CurrentServerRole, 32UL),
							new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, this.localServer.ServerSite)
						});
					}
					List<Server> bridgeheadServers = new List<Server>();
					if (!ADNotificationAdapter.TryReadConfigurationPaged<Server>(() => this.rootOrgConfigSession.FindPaged<Server>(null, QueryScope.SubTree, this.localSiteHubsFilter, null, 0), delegate(Server server)
					{
						ServerVersion a = new ServerVersion(server.VersionNumber);
						if (ServerVersion.Compare(a, RpcClientWrapper.minRequiredRpcServerVersion) >= 0)
						{
							bridgeheadServers.Add(server);
						}
					}, out adoperationResult))
					{
						e = adoperationResult.Exception;
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Error, null, string.Format("Failed to load topology with exception {0}", ServerManagerLog.GetExceptionLogString(e, ServerManagerLog.ExceptionLogOption.IncludeStack | ServerManagerLog.ExceptionLogOption.IncludeInnerException)));
						result = false;
					}
					else
					{
						Interlocked.Exchange<List<Server>>(ref this.localSiteBridgeheadServers, bridgeheadServers);
						RpcClientWrapper.instance.topologyLastUpdated = DateTime.UtcNow;
						StringBuilder stringBuilder = new StringBuilder();
						foreach (Server server2 in bridgeheadServers)
						{
							stringBuilder.Append(server2.Name);
							stringBuilder.Append(",");
						}
						ServerManagerLog.LogEvent(ServerManagerLog.Subcomponent.RpcClientWrapper, ServerManagerLog.EventType.Success, null, string.Format("Sucessfully load topology with servers {0}", stringBuilder.ToString()));
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.loadTopologyCount);
			}
			return result;
		}

		private static readonly RpcClientWrapper instance = new RpcClientWrapper();

		private static readonly TimeSpan topologyExpiryTimeSpan = TimeSpan.FromHours(1.0);

		private static readonly ServerVersion minRequiredRpcServerVersion = new ServerVersion(14, 1, 114, 0);

		private readonly object initializeLockObject = new object();

		private readonly Random random = new Random();

		private readonly ITopologyConfigurationSession rootOrgConfigSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 65, "rootOrgConfigSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\OfflineRms\\RpcClientWrapper.cs");

		private bool initialized;

		private DateTime topologyLastUpdated = DateTime.MinValue;

		private List<Server> localSiteBridgeheadServers = new List<Server>();

		private volatile Server localServer;

		private int loadTopologyCount;

		private QueryFilter localSiteHubsFilter;
	}
}
