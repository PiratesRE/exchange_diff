using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class SipPeerManager
	{
		protected static SipPeerManager ManagerInstance { get; set; }

		protected SipPeerManager(UMADSettings adSettings)
		{
			ExAssert.RetailAssert(adSettings != null, "UMADSettings is null");
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.ProcessName = currentProcess.ProcessName.ToLower();
			}
			this.ServiceADSettings = adSettings;
			this.ServiceADSettings.SubscribeToADNotifications(new ADNotificationEvent(this.ADNotification_ServerConfigChanged));
		}

		internal event EventHandler SipPeerListChanged;

		internal static SipPeerManager Instance
		{
			get
			{
				ExAssert.RetailAssert(SipPeerManager.ManagerInstance != null, "SipPeerManager was not initialized");
				return SipPeerManager.ManagerInstance;
			}
		}

		protected virtual bool SkipCertPHeaderCheckforMonitoring
		{
			get
			{
				return AppConfig.Instance.Service.SkipCertPHeaderCheckforActiveMonitoring;
			}
		}

		internal abstract UMSipPeer AVAuthenticationService { get; }

		internal abstract UMSipPeer SIPAccessService { get; }

		internal abstract bool IsHeuristicBasedSIPAccessService { get; }

		internal abstract UMSipPeer SBCService { get; }

		private protected UMADSettings ServiceADSettings { protected get; private set; }

		private protected string ProcessName { protected get; private set; }

		internal static void Initialize(bool generateNoSipPeerWarning, UMADSettings adSettings)
		{
			if (SipPeerManager.ManagerInstance == null)
			{
				lock (SipPeerManager.syncLock)
				{
					if (SipPeerManager.ManagerInstance == null)
					{
						if (CommonConstants.UseDataCenterCallRouting)
						{
							SipPeerManager.ManagerInstance = new DatacenterSipPeerManager(adSettings);
						}
						else
						{
							SipPeerManager.ManagerInstance = new EnterpriseSipPeerManager(generateNoSipPeerWarning, adSettings);
						}
					}
				}
			}
		}

		internal abstract List<UMSipPeer> GetSecuredSipPeers();

		internal UMSipPeer GetSipPeerFromUMCertificateSubjectName()
		{
			UMSipPeer result = null;
			if (!string.IsNullOrEmpty(this.ServiceADSettings.UMCertificateThumbprint))
			{
				X509Certificate2 x509Certificate = CertificateUtils.FindCertByThumbprint(this.ServiceADSettings.UMCertificateThumbprint);
				if (x509Certificate != null)
				{
					string subjectFqdn = CertificateUtils.GetSubjectFqdn(x509Certificate);
					try
					{
						result = new UMSipPeer(new UMSmartHost(subjectFqdn), 10000, false, true, IPAddressFamily.Any);
					}
					catch (ArgumentException ex)
					{
						string text = CommonUtil.ToEventLogString(ex);
						UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SipPeerCertificateSubjectName, null, new object[]
						{
							subjectFqdn,
							text
						});
						this.DebugTrace("SipPeerManager::GetSipPeerFromUMCertificateSubjectName UM Local Sip Option Probe will fail {0}", new object[]
						{
							ex
						});
					}
				}
			}
			return result;
		}

		internal virtual UMIPGateway GetUMIPGateway(string fqdn, Guid tenantGuid)
		{
			ValidateArgument.NotNullOrEmpty(fqdn, "fqdn");
			return this.GetUMIPGateway(new string[]
			{
				fqdn
			}, tenantGuid, false);
		}

		internal abstract bool IsAccessProxy(string matchedFqdn);

		internal abstract bool IsAccessProxyWithOrgTestHook(string matchedFqdn, string orgParameter);

		internal abstract bool IsSIPSBC(string matchedFqdn);

		internal bool IsAuthorized(X509Certificate certificate, IPAddress connectingGW, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, PlatformSipUri requestUri, Guid tenantGuid, ref string matchedFqdn, out UMIPGateway gateway)
		{
			ValidateArgument.NotNull(connectingGW, "connectingGW");
			bool flag = false;
			gateway = null;
			if (certificate == null)
			{
				flag = this.DoAuthorizationForTcpEndPoint(connectingGW, sipHeaders, tenantGuid, out gateway);
			}
			else
			{
				try
				{
					flag = this.DoAuthorizationForTlsEndPoint(new X509Certificate2(certificate), connectingGW, sipHeaders, requestUri, tenantGuid, ref matchedFqdn, out gateway);
				}
				catch (CryptographicException ex)
				{
					this.DebugTrace("SipPeerManager::IsAuthorized Got CryptographicException exception {0}", new object[]
					{
						ex.Message
					});
					string text = string.Empty;
					try
					{
						text = certificate.GetCertHashString();
					}
					catch
					{
					}
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidClientCertificate, null, new object[]
					{
						text,
						CommonUtil.ToEventLogString(ex.ToString())
					});
				}
			}
			if (gateway != null && gateway.Status != GatewayStatus.Enabled)
			{
				UmGlobals.ExEvent.LogEvent(gateway.OrganizationId, UMEventLogConstants.Tuple_CallRejectedSinceGatewayDisabled, gateway.Name, (CallId.Id == null) ? string.Empty : CallId.Id, gateway.Address);
				flag = false;
			}
			this.DebugTrace("SipPeerManager::IsAuthorized Call authorized = {0}", new object[]
			{
				flag
			});
			return flag;
		}

		internal bool IsLocalDiagnosticCall(IPAddress connectingGW, IEnumerable<PlatformSignalingHeader> headers)
		{
			ValidateArgument.NotNull(connectingGW, "connectingGW");
			bool flag = false;
			bool flag2 = false;
			bool result = false;
			if (headers != null)
			{
				this.DebugTrace("SipPeerManager::IsLocalDiagnosticCall: Looking for the diagnostic and user agent header", new object[0]);
				foreach (PlatformSignalingHeader platformSignalingHeader in headers)
				{
					if (string.Equals(platformSignalingHeader.Name, "msexum-connectivitytest", StringComparison.OrdinalIgnoreCase))
					{
						if (!string.Equals(platformSignalingHeader.Value, "local", StringComparison.OrdinalIgnoreCase))
						{
							this.DebugTrace("SipPeerManager:IsLocalDiagnosticCall:: Diagnostic header value '{0}' is not valid", new object[]
							{
								(platformSignalingHeader.Value == null) ? "<null>" : platformSignalingHeader.Value
							});
							break;
						}
						flag = true;
					}
					else if (string.Equals(platformSignalingHeader.Name, "user-agent", StringComparison.OrdinalIgnoreCase))
					{
						if (platformSignalingHeader.Value == null || platformSignalingHeader.Value.IndexOf("MSExchangeUM-Diagnostics") <= 0)
						{
							this.DebugTrace("SipPeerManager:IsLocalDiagnosticCall:: user header value '{0}' is not valid", new object[]
							{
								(platformSignalingHeader.Value == null) ? "<null>" : platformSignalingHeader.Value
							});
							break;
						}
						flag2 = true;
					}
				}
			}
			if (flag && flag2)
			{
				if (!this.IsLocalCall(connectingGW))
				{
					this.DebugTrace("SipPeerManager:IsLocalDiagnosticCall:: Local diagnostic call attempted from a remote machine {0}", new object[]
					{
						connectingGW.ToString()
					});
					throw CallRejectedException.Create(Strings.DiagnosticCallFromRemoteHost(connectingGW.ToString()), CallEndingReason.TransientError, null, new object[0]);
				}
				result = true;
				this.DebugTrace("SipPeerManager::IsLocalDiagnosticCall:: Local diagnostic call detected", new object[0]);
			}
			else
			{
				this.DebugTrace("SipPeerManager::IsLocalDiagnosticCall: Not a diagnositc call", new object[0]);
			}
			return result;
		}

		public bool IsActiveMonitoringCall(IEnumerable<PlatformSignalingHeader> diagnosticsHeaders)
		{
			bool flag = false;
			foreach (PlatformSignalingHeader platformSignalingHeader in diagnosticsHeaders)
			{
				if (string.Equals(platformSignalingHeader.Name, "user-agent", StringComparison.OrdinalIgnoreCase))
				{
					string value = platformSignalingHeader.Value;
					if (value != null && value.IndexOf("ActiveMonitoringClient", StringComparison.OrdinalIgnoreCase) > 0)
					{
						flag = true;
						break;
					}
					break;
				}
			}
			this.DebugTrace("SipPeerManager::IsActiveMonitoringCall returning = {0}", new object[]
			{
				flag
			});
			return flag;
		}

		protected void RaisePeerListChangeEvent()
		{
			if (this.SipPeerListChanged != null)
			{
				this.DebugTrace("SipPeerManager::RaisePeerListChangeEvent", new object[0]);
				this.SipPeerListChanged(this, EventArgs.Empty);
			}
		}

		protected void DebugTrace(string message, params object[] args)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.SipPeerManagerTracer, this.GetHashCode(), message, args);
		}

		protected bool HandleADException(Exception ex)
		{
			bool result = true;
			if (ex is ADTransientException)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADTransientError, null, new object[]
				{
					(CallId.Id == null) ? "<null>" : CallId.Id,
					CommonUtil.ToEventLogString(ex.ToString())
				});
			}
			else if (ex is DataSourceOperationException)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADPermanentError, null, new object[]
				{
					(CallId.Id == null) ? "<null>" : CallId.Id,
					CommonUtil.ToEventLogString(ex.ToString())
				});
			}
			else if (ex is DataValidationException)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_ADDataError, null, new object[]
				{
					(CallId.Id == null) ? "<null>" : CallId.Id,
					CommonUtil.ToEventLogString(ex.ToString())
				});
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected UMIPGateway GetUMIPGateway(IList<string> fqdns, Guid tenantGuid, bool isMonitoring)
		{
			UMIPGateway result = null;
			try
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FindUMIPGatewayInAD, null, new object[]
				{
					fqdns.ToString(Environment.NewLine)
				});
				IADSystemConfigurationLookup adlookup = this.GetADLookup(tenantGuid);
				IEnumerable<UMIPGateway> allIPGateways = adlookup.GetAllIPGateways();
				if (isMonitoring && this.SkipCertPHeaderCheckforMonitoring)
				{
					result = this.GetMonitoringGateway(allIPGateways);
				}
				else
				{
					result = this.GetIPGatewayFromAddress(allIPGateways, fqdns);
				}
			}
			catch (Exception ex)
			{
				this.DebugTrace("SipPeerManager::GetUMIPGateway An exception occured {0}", new object[]
				{
					ex.Message
				});
				if (!this.HandleADException(ex))
				{
					throw;
				}
			}
			return result;
		}

		protected virtual IADSystemConfigurationLookup GetADLookup(Guid tenantGuid)
		{
			return ADSystemConfigurationLookupFactory.CreateFromTenantGuid(tenantGuid);
		}

		protected UMIPGateway GetMonitoringGateway(IEnumerable<UMIPGateway> gateways)
		{
			IEnumerable<UMIPGateway> source = from gw in gateways
			where gw.Address.ToString().EndsWith("o365.exchangemon.net", StringComparison.InvariantCultureIgnoreCase)
			select gw;
			return source.FirstOrDefault<UMIPGateway>();
		}

		protected UMIPGateway GetIPGatewayFromAddress(IEnumerable<UMIPGateway> gateways, IList<string> fqdns)
		{
			if (fqdns == null || fqdns.Count == 0)
			{
				throw new ArgumentException("Null or empty", "fqdns");
			}
			IEnumerable<UMIPGateway> source = from gw in gateways
			where fqdns.Contains(gw.Address.ToString(), StringComparer.InvariantCultureIgnoreCase)
			select gw;
			return source.FirstOrDefault<UMIPGateway>();
		}

		protected abstract UMIPGateway FindGatewayForTlsEndPoint(X509Certificate2 certificate, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, PlatformSipUri requestUri, Guid tenantGuid, ref string matchedFqdn);

		protected abstract void LogUnknownTcpGateway(string remoteEndPoint);

		protected abstract void LogUnknownTlsGateway(X509Certificate2 certificate, string matchedFqdn, string remoteEndPoint);

		protected abstract void OnLocalServerUpdated();

		private bool DoAuthorizationForTlsEndPoint(X509Certificate2 certificate, IPAddress connectingGW, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, PlatformSipUri requestUri, Guid tenantGuid, ref string matchedFqdn, out UMIPGateway gateway)
		{
			ValidateArgument.NotNull(certificate, "certificate");
			this.DebugTrace("SipPeerManager::DoAuthorizationForTlsEndPoint.", new object[0]);
			bool result = false;
			gateway = null;
			if (string.IsNullOrEmpty(matchedFqdn))
			{
				this.DebugTrace("MatchedFqdn is null or emtpy for a Tls connection. The inbound call is being rejected", new object[0]);
			}
			else
			{
				gateway = this.FindGatewayForTlsEndPoint(certificate, sipHeaders, requestUri, tenantGuid, ref matchedFqdn);
				if (gateway != null)
				{
					this.DebugTrace("SipPeerManager::DoAuthorizationForTlsEndPoint. Found the gateway {0}", new object[]
					{
						gateway.Id
					});
					result = true;
				}
				else
				{
					this.LogUnknownTlsGateway(certificate, matchedFqdn, connectingGW.ToString());
				}
			}
			return result;
		}

		private bool DoAuthorizationForTcpEndPoint(IPAddress connectingGW, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, Guid tenantGuid, out UMIPGateway gateway)
		{
			this.DebugTrace("SipPeerManager::DoAuthorizationForTcpEndPoint", new object[0]);
			bool result = false;
			gateway = this.GetUMIPGateway(connectingGW.ToString(), tenantGuid);
			if (gateway != null)
			{
				this.DebugTrace("SipPeerManager::DoAuthorizationForTcpEndPoint. Found the gateway {0}", new object[]
				{
					gateway.Id
				});
				result = true;
			}
			else
			{
				this.LogUnknownTcpGateway(connectingGW.ToString());
			}
			return result;
		}

		private bool IsLocalCall(IPAddress connectingGateway)
		{
			return ComputerInformation.GetLocalIPAddresses().Exists((IPAddress ip) => ip.Equals(connectingGateway));
		}

		private void ADNotification_ServerConfigChanged(ADNotificationEventArgs args)
		{
			if (args != null && args.Id != null && args.ChangeType == ADNotificationChangeType.ModifyOrAdd && this.ServiceADSettings.Id.Equals(args.Id))
			{
				this.ServiceADSettings = this.ServiceADSettings.RefreshFromAD();
				this.OnLocalServerUpdated();
			}
		}

		private static object syncLock = new object();
	}
}
