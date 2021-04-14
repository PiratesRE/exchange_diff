using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EnterpriseSipPeerManager : SipPeerManager
	{
		internal override UMSipPeer AVAuthenticationService
		{
			get
			{
				return null;
			}
		}

		internal override UMSipPeer SIPAccessService
		{
			get
			{
				return this.sipAccessService;
			}
		}

		internal override bool IsHeuristicBasedSIPAccessService
		{
			get
			{
				return this.heuristicBasedSIPAccessService;
			}
		}

		internal override UMSipPeer SBCService
		{
			get
			{
				return null;
			}
		}

		internal EnterpriseSipPeerManager(bool generateNoSipPeerWarning, UMADSettings adSettings) : base(adSettings)
		{
			this.authorizationCache = new Dictionary<string, List<UMIPGateway>>(StringComparer.OrdinalIgnoreCase);
			this.peerListCache = new List<UMSipPeer>();
			this.generateNoSipPeerWarning = generateNoSipPeerWarning;
			this.PopulateSipPeers(Strings.CacheRefreshInitialization);
			ADNotificationsManager.Instance.UMIPGateway.ConfigChanged += this.ADUpdateCallback;
			ADNotificationsManager.Instance.UMDialPlan.ConfigChanged += this.ADUpdateCallback;
			ADNotificationsManager.Instance.UMHuntGroup.ConfigChanged += this.ADUpdateCallback;
			ADNotificationsManager.Instance.CallRouterSettings.ConfigChanged += this.ADUpdateCallback;
		}

		internal override List<UMSipPeer> GetSecuredSipPeers()
		{
			if (UmServiceGlobals.StartupMode != UMStartupMode.TCP)
			{
				List<UMSipPeer> list = this.peerListCache.FindAll((UMSipPeer p) => p.UseMutualTLS);
				ExAssert.RetailAssert(list.Count > 0, "UM Service Mode is {0} but there is no secured peer", new object[]
				{
					UmServiceGlobals.StartupMode
				});
				return list;
			}
			return new List<UMSipPeer>(0);
		}

		internal override bool IsAccessProxy(string matchedFqdn)
		{
			return false;
		}

		internal override bool IsAccessProxyWithOrgTestHook(string matchedFqdn, string orgParameter)
		{
			return false;
		}

		internal override bool IsSIPSBC(string matchedFqdn)
		{
			return false;
		}

		internal override UMIPGateway GetUMIPGateway(string address, Guid tenantGuid)
		{
			ValidateArgument.NotNullOrEmpty(address, "fqdn");
			UMIPGateway result = null;
			List<UMIPGateway> list;
			if (this.authorizationCache.TryGetValue(address, out list))
			{
				result = list[0];
				base.DebugTrace("GetUMIPGateway::Found the Gateway with address {0}", new object[]
				{
					address
				});
				if (list.Count > 1)
				{
					this.LogDuplicatePeersEvent(list);
				}
			}
			else
			{
				base.DebugTrace("GetUMIPGateway::Could not find the Gateway with address {0}", new object[]
				{
					address
				});
			}
			return result;
		}

		protected override UMIPGateway FindGatewayForTlsEndPoint(X509Certificate2 certificate, ReadOnlyCollection<PlatformSignalingHeader> sipHeaders, PlatformSipUri requestUri, Guid tenantGuid, ref string matchedFqdn)
		{
			base.DebugTrace("EnterpriseSipPeerManager::FindGatewayForTlsEndPoint.", new object[0]);
			return this.GetUMIPGateway(matchedFqdn, tenantGuid);
		}

		protected override void LogUnknownTcpGateway(string remoteEndPoint)
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRejected, null, new object[]
			{
				(CallId.Id == null) ? "<null>" : CallId.Id,
				Strings.CallFromInvalidGateway(remoteEndPoint).ToString()
			});
		}

		protected override void LogUnknownTlsGateway(X509Certificate2 certificate, string matchedFqdn, string remoteEndPoint)
		{
			this.LogUnknownTcpGateway(matchedFqdn);
		}

		protected override void OnLocalServerUpdated()
		{
			base.DebugTrace("SipPeerManager::OnLocalServerUpdated", new object[0]);
			lock (this.syncLock)
			{
				this.PopulateSipPeers(Strings.CacheRefreshInitialization);
			}
		}

		protected virtual void ReadUMIPGateways(out List<UMIPGateway> secureGateways, out List<UMIPGateway> unsecureGateways)
		{
			Utils.GetIPGatewayList(base.ServiceADSettings.Fqdn, false, false, out secureGateways, out unsecureGateways);
		}

		protected virtual UMSipPeer ReadUMPoolSipPeer()
		{
			return base.GetSipPeerFromUMCertificateSubjectName();
		}

		private static void AddGatewayToAuthCache(Dictionary<string, List<UMIPGateway>> authCache, string key, UMIPGatewaySipPeer gateway)
		{
			List<UMIPGateway> list = null;
			if (!authCache.TryGetValue(key, out list))
			{
				list = new List<UMIPGateway>(1);
				authCache[key] = list;
			}
			list.Add(gateway.ToUMIPGateway(OrganizationId.ForestWideOrgId));
		}

		private void PopulateSipPeers(LocalizedString reason)
		{
			List<UMSipPeer> securePeers;
			List<UMSipPeer> list;
			this.GetAllowedPeers(out securePeers, out list);
			list = this.ValidateTcpSipPeerList(list);
			this.GenerateAndUpdateCache(securePeers, list);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SipPeerCacheRefreshed, null, new object[]
			{
				base.ProcessName,
				this.peerListCache.ToString(Environment.NewLine),
				reason
			});
			this.DetermineSIPAccessService(securePeers);
			base.RaisePeerListChangeEvent();
		}

		private void GetAllowedPeers(out List<UMSipPeer> secureUMPeers, out List<UMSipPeer> unsecureUMPeers)
		{
			secureUMPeers = new List<UMSipPeer>();
			unsecureUMPeers = new List<UMSipPeer>();
			List<UMIPGateway> list;
			List<UMIPGateway> list2;
			this.ReadUMIPGateways(out list, out list2);
			Hashtable hashtable = new Hashtable();
			foreach (UMIPGateway gateway in list2)
			{
				this.AddSipPeerIfNotPresentAlready(hashtable, unsecureUMPeers, new UMIPGatewaySipPeer(gateway, false));
			}
			hashtable.Clear();
			foreach (UMIPGateway gateway2 in list)
			{
				this.AddSipPeerIfNotPresentAlready(hashtable, secureUMPeers, new UMIPGatewaySipPeer(gateway2, true));
			}
			UMSipPeer umsipPeer = this.ReadUMPoolSipPeer();
			if (umsipPeer != null)
			{
				this.AddSipPeerIfNotPresentAlready(hashtable, secureUMPeers, umsipPeer);
			}
			if (this.generateNoSipPeerWarning)
			{
				this.CheckIfNoPeers(secureUMPeers, unsecureUMPeers);
			}
			if (UmServiceGlobals.StartupMode != UMStartupMode.TCP)
			{
				this.TrustCafeServersIfNecessary(hashtable, secureUMPeers);
				string fqdn = base.ServiceADSettings.Fqdn.ToString().ToLowerInvariant();
				this.AddSipPeerIfNotPresentAlready(hashtable, secureUMPeers, UMSipPeer.CreateForTlsAuth(fqdn));
				if (UmServiceGlobals.StartupMode == UMStartupMode.TLS)
				{
					unsecureUMPeers.Clear();
					return;
				}
			}
			else
			{
				secureUMPeers.Clear();
			}
		}

		private void AddSipPeerIfNotPresentAlready(Hashtable duplicateDetector, List<UMSipPeer> list, UMSipPeer peer)
		{
			string key = peer.Address.ToString().ToLowerInvariant();
			if (!duplicateDetector.Contains(key))
			{
				list.Add(peer);
				duplicateDetector.Add(key, peer);
			}
		}

		private void TrustCafeServersIfNecessary(Hashtable duplicateDetector, List<UMSipPeer> secureUMPeers)
		{
			if (base.ServiceADSettings is UMServiceADSettings)
			{
				ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
				IEnumerable<Server> allCafeServers = adtopologyLookup.GetAllCafeServers();
				if (allCafeServers != null)
				{
					foreach (Server server in allCafeServers)
					{
						this.AddSipPeerIfNotPresentAlready(duplicateDetector, secureUMPeers, UMSipPeer.CreateForTlsAuth(server.Fqdn));
					}
				}
			}
		}

		private void GenerateAndUpdateCache(List<UMSipPeer> securePeers, List<UMSipPeer> unsecurePeers)
		{
			Dictionary<string, List<UMIPGateway>> authCache = new Dictionary<string, List<UMIPGateway>>(StringComparer.OrdinalIgnoreCase);
			List<UMSipPeer> list = new List<UMSipPeer>(securePeers.Count + unsecurePeers.Count);
			foreach (UMSipPeer umsipPeer in securePeers)
			{
				UMIPGatewaySipPeer umipgatewaySipPeer = umsipPeer as UMIPGatewaySipPeer;
				if (umipgatewaySipPeer != null)
				{
					string text = umsipPeer.Address.ToString();
					EnterpriseSipPeerManager.AddGatewayToAuthCache(authCache, text, umipgatewaySipPeer);
					base.DebugTrace("EnterpriseSipPeerManager::GenerateAndUpdateCache: add {0} (TLS)", new object[]
					{
						text
					});
				}
			}
			foreach (UMSipPeer umsipPeer2 in unsecurePeers)
			{
				UMIPGatewaySipPeer umipgatewaySipPeer2 = umsipPeer2 as UMIPGatewaySipPeer;
				if (umipgatewaySipPeer2 != null)
				{
					foreach (IPAddress ipaddress in umsipPeer2.ResolvedIPAddress)
					{
						string text2 = ipaddress.ToString();
						EnterpriseSipPeerManager.AddGatewayToAuthCache(authCache, text2, umipgatewaySipPeer2);
						base.DebugTrace("EnterpriseSipPeerManager::GenerateAndUpdateCache: add {0} (TCP)", new object[]
						{
							text2
						});
					}
				}
			}
			this.LogAllDuplicatePeers(authCache);
			list.AddRange(securePeers);
			list.AddRange(unsecurePeers);
			this.peerListCache = list;
			this.authorizationCache = authCache;
		}

		private void LogAllDuplicatePeers(Dictionary<string, List<UMIPGateway>> authCache)
		{
			foreach (List<UMIPGateway> list in authCache.Values)
			{
				if (list.Count > 1)
				{
					this.LogDuplicatePeersEvent(list);
				}
			}
		}

		private void LogDuplicatePeersEvent(List<UMIPGateway> gatewayList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine();
			foreach (UMIPGateway umipgateway in gatewayList)
			{
				stringBuilder.AppendLine(umipgateway.Name);
			}
			string text = CommonUtil.ToEventLogString(stringBuilder);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DuplicatePeersFound, text, new object[]
			{
				text
			});
		}

		private void DetermineSIPAccessService(List<UMSipPeer> securePeers)
		{
			if (!base.ServiceADSettings.IsSIPAccessServiceNeeded)
			{
				return;
			}
			try
			{
				if (base.ServiceADSettings.SIPAccessService != null)
				{
					this.sipAccessService = new UMSipPeer(new UMSmartHost(base.ServiceADSettings.SIPAccessService.Hostname.HostnameString), base.ServiceADSettings.SIPAccessService.Port, true, true, IPAddressFamily.Any);
					this.heuristicBasedSIPAccessService = false;
				}
				else
				{
					this.HeuristicallyDetermineSIPAccessService(securePeers);
				}
			}
			catch (Exception ex)
			{
				base.DebugTrace("EnterpriseSipPeerManager::DetermineSIPAccessService: Exception: {0}", new object[]
				{
					ex
				});
				if (!base.HandleADException(ex))
				{
					throw;
				}
			}
		}

		private void HeuristicallyDetermineSIPAccessService(List<UMSipPeer> securePeers)
		{
			List<UMSipPeer> list = new List<UMSipPeer>();
			List<UMSipPeer> list2 = new List<UMSipPeer>();
			foreach (UMSipPeer umsipPeer in securePeers)
			{
				if (umsipPeer.IsOcs && umsipPeer.ToUMIPGateway(OrganizationId.ForestWideOrgId).Status == GatewayStatus.Enabled)
				{
					list2.Add(umsipPeer);
					if (umsipPeer.AllowOutboundCalls)
					{
						list.Add(umsipPeer);
					}
				}
			}
			if (list.Count > 0)
			{
				this.sipAccessService = this.FindLongestSuffixMatchingPeer(list);
				return;
			}
			if (list2.Count > 0)
			{
				this.sipAccessService = this.FindLongestSuffixMatchingPeer(list2);
			}
		}

		private UMSipPeer FindLongestSuffixMatchingPeer(List<UMSipPeer> listOfpeers)
		{
			int index = -1;
			int num = -1;
			for (int i = 0; i < listOfpeers.Count; i++)
			{
				int num2 = Utils.DetermineLongestSuffixMatch(listOfpeers[i].Address.ToString(), base.ServiceADSettings.Fqdn);
				if (num2 > num)
				{
					num = num2;
					index = i;
				}
			}
			this.heuristicBasedSIPAccessService = true;
			return listOfpeers[index];
		}

		private List<UMSipPeer> ValidateTcpSipPeerList(List<UMSipPeer> peerList)
		{
			List<UMSipPeer> list = new List<UMSipPeer>(peerList.Count);
			foreach (UMSipPeer umsipPeer in peerList)
			{
				if (this.ValidateSIPPeer(umsipPeer))
				{
					list.Add(umsipPeer);
				}
			}
			return list;
		}

		private bool ValidateSIPPeer(UMSipPeer peer)
		{
			bool flag = false;
			ExAssert.RetailAssert(!peer.UseMutualTLS, "ValidateSIPPeer can only be called for TCP peers");
			if (!peer.Address.IsIPAddress)
			{
				IPHostEntry iphostEntry = null;
				string text = peer.Address.ToString();
				if (Utils.HasValidDNSRecord(text, out iphostEntry))
				{
					flag = true;
					if (iphostEntry.AddressList != null && iphostEntry.AddressList.Length > 0)
					{
						List<IPAddress> list = new List<IPAddress>();
						foreach (IPAddress item in iphostEntry.AddressList)
						{
							list.Add(item);
						}
						peer.ResolvedIPAddress = list;
					}
				}
				else
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidPeerDNSHostName, text, new object[]
					{
						text,
						string.Empty
					});
				}
			}
			else
			{
				flag = true;
			}
			base.DebugTrace("SipPeerManager::ValidateSIPPeers: {0} Is valid:{1}", new object[]
			{
				peer,
				flag
			});
			return flag;
		}

		private bool HasDuplicateAddresses(Dictionary<IPAddress, UMSipPeer> addressTable, UMSipPeer peer, out UMSipPeer duplicate)
		{
			duplicate = null;
			if (peer.ResolvedIPAddress != null)
			{
				foreach (IPAddress key in peer.ResolvedIPAddress)
				{
					if (addressTable.ContainsKey(key))
					{
						duplicate = addressTable[key];
						return true;
					}
				}
				foreach (IPAddress key2 in peer.ResolvedIPAddress)
				{
					addressTable[key2] = peer;
				}
				return false;
			}
			return false;
		}

		private void CheckIfNoPeers(List<UMSipPeer> securePeerList, List<UMSipPeer> unsecurePeerList)
		{
			switch (UMRecyclerConfig.UMStartupType)
			{
			case UMStartupMode.TCP:
				if (unsecurePeerList.Count == 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_WNoPeersFound, null, new object[]
					{
						Strings.UnSecured
					});
				}
				if (securePeerList.Count != 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_IncorrectPeers, null, new object[]
					{
						Strings.UnSecured,
						Strings.Secured,
						securePeerList.ToString(Environment.NewLine)
					});
					return;
				}
				break;
			case UMStartupMode.TLS:
				if (securePeerList.Count == 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_WNoPeersFound, null, new object[]
					{
						Strings.Secured
					});
				}
				if (unsecurePeerList.Count != 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_IncorrectPeers, null, new object[]
					{
						Strings.Secured,
						Strings.UnSecured,
						unsecurePeerList.ToString(Environment.NewLine)
					});
					return;
				}
				break;
			default:
				if (unsecurePeerList.Count == 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_WNoPeersFound, null, new object[]
					{
						Strings.UnSecured
					});
				}
				if (securePeerList.Count == 0)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_WNoPeersFound, null, new object[]
					{
						Strings.Secured
					});
				}
				break;
			}
		}

		private void ADUpdateCallback(ADNotificationEventArgs args)
		{
			if (args != null && args.Id != null)
			{
				this.RepopulateSipPeerList(args.Id, args.ChangeType);
				return;
			}
			base.DebugTrace("ADUpdateCallback : Ignore the AD callback because args or args.id is null.", new object[0]);
		}

		private void RepopulateSipPeerList(ADObjectId updatedObjectId, ADNotificationChangeType changeType)
		{
			base.DebugTrace("SipPeerManager::RepopulateSipPeerList", new object[0]);
			lock (this.syncLock)
			{
				if (changeType == ADNotificationChangeType.Delete)
				{
					this.PopulateSipPeers(Strings.CacheRefreshADDeleteNotification(updatedObjectId.Name.Replace("\n", "\\n")));
				}
				else
				{
					this.PopulateSipPeers(Strings.CacheRefreshADUpdateNotification(updatedObjectId.Name));
				}
			}
		}

		private volatile Dictionary<string, List<UMIPGateway>> authorizationCache;

		private bool generateNoSipPeerWarning;

		private volatile UMSipPeer sipAccessService;

		private volatile bool heuristicBasedSIPAccessService = true;

		private volatile List<UMSipPeer> peerListCache;

		private object syncLock = new object();
	}
}
