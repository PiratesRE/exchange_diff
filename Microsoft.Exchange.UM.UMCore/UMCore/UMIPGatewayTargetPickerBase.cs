using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMIPGatewayTargetPickerBase : IServerPicker<IMwiTarget, Guid>
	{
		public IMwiTarget PickNextServer(Guid dialPlanGuid)
		{
			throw new NotSupportedException();
		}

		public IMwiTarget PickNextServer(Guid dialPlanGuid, out int totalServers)
		{
			throw new NotSupportedException();
		}

		public IMwiTarget PickNextServer(Guid dialPlanGuid, Guid tenantGuid, out int totalServers)
		{
			totalServers = 0;
			IMwiTarget result = null;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromTenantGuid(tenantGuid);
			UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(new ADObjectId(dialPlanGuid));
			if (dialPlanFromId != null)
			{
				result = this.PickNextServer(dialPlanFromId, out totalServers);
			}
			else
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer() Could not find dialplan with guid {0}", new object[]
				{
					dialPlanGuid
				});
			}
			return result;
		}

		public void ServerUnavailable(IMwiTarget server)
		{
			CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::ServerUnavailable()", new object[0]);
		}

		public IMwiTarget PickNextServer(UMDialPlan dialPlan, out int totalServers)
		{
			totalServers = 0;
			SipNotifyMwiTarget result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "{0}::PickNextServer() DialPlan Id = {1}", new object[]
			{
				base.GetType().Name,
				dialPlan.Id.ObjectGuid
			});
			if (SipRoutingHelper.UseGlobalAccessProxyForOutbound(dialPlan))
			{
				UMSipPeer sipaccessService = SipPeerManager.Instance.SIPAccessService;
				if (sipaccessService != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "SIP proxy required. Using {0}.", new object[]
					{
						sipaccessService
					});
					result = new SipNotifyMwiTarget(sipaccessService, dialPlan.OrganizationId);
					totalServers = 1;
				}
				else
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "SIP proxy required but not configured!", new object[0]);
				}
			}
			else if (this.IsDialPlanCompatible(dialPlan))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer(): DialPlan {0} has VoIPSecurity equal to {1}", new object[]
				{
					dialPlan.Id,
					dialPlan.VoIPSecurity
				});
				List<UMIPGateway> gwlistForDialPlan = this.GetGWListForDialPlan(dialPlan);
				if (gwlistForDialPlan.Count > 0)
				{
					totalServers = gwlistForDialPlan.Count;
					int index = this.randomNumberGenerator.Next(0, gwlistForDialPlan.Count);
					UMIPGateway umipgateway = gwlistForDialPlan[index];
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer() Returning Gateway = {0}:{1} for DialPlan {2}", new object[]
					{
						umipgateway.Address,
						umipgateway.Port,
						dialPlan.Id
					});
					UMSipPeer umsipPeer = new UMIPGatewaySipPeer(umipgateway, dialPlan);
					if (SipRoutingHelper.UseGlobalSBCSettingsForOutbound(umipgateway))
					{
						umsipPeer.NextHopForOutboundRouting = SipPeerManager.Instance.SBCService;
					}
					result = new SipNotifyMwiTarget(umsipPeer, dialPlan.OrganizationId);
				}
				else
				{
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer() No valid gateways found for the DialPlan with Id = {0} and GlobalCallRountingScheme {1}", new object[]
					{
						dialPlan.Id,
						dialPlan.GlobalCallRoutingScheme
					});
				}
			}
			else
			{
				CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer() The security mode {0} of DialPlan {1} is not compatible with UMService Mode {2}.", new object[]
				{
					dialPlan.VoIPSecurity,
					dialPlan.Id,
					UmServiceGlobals.StartupMode
				});
			}
			return result;
		}

		protected abstract bool InternalIsValid(UMIPGateway gw);

		private List<UMIPGateway> GetGWListForDialPlan(UMDialPlan dp)
		{
			List<UMIPGateway> list;
			if (dp.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.E164)
			{
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromRootOrg(true);
				list = new List<UMIPGateway>(iadsystemConfigurationLookup.GetAllGlobalGateways());
			}
			else
			{
				ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
				Server localServer = adtopologyLookup.GetLocalServer();
				list = Utils.GetIPGatewayList(localServer.Fqdn, dp, false, false);
				List<UMIPGateway> list2 = new List<UMIPGateway>();
				foreach (UMIPGateway umipgateway in list)
				{
					if (umipgateway.Name.StartsWith(dp.Name, StringComparison.InvariantCultureIgnoreCase))
					{
						list2.Add(umipgateway);
					}
				}
				if (list2.Count > 0)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UMIPGatewayTargetPickerBase::PickNextServer(): Selecting the gateways whose name starts with DialPlan name {0}. Id ={1}", new object[]
					{
						dp.Name,
						dp.Id
					});
					list = list2;
				}
			}
			list = this.GetValidGateways(list, dp);
			return list;
		}

		private List<UMIPGateway> GetValidGateways(List<UMIPGateway> gateways, UMDialPlan dp)
		{
			return gateways.FindAll((UMIPGateway gw) => !gw.Simulator && gw.Status == GatewayStatus.Enabled && (dp.VoIPSecurity == UMVoIPSecurityType.Unsecured || !gw.Address.IsIPAddress) && this.InternalIsValid(gw));
		}

		private bool IsDialPlanCompatible(UMDialPlan diaplan)
		{
			switch (UmServiceGlobals.StartupMode)
			{
			case UMStartupMode.TCP:
				return diaplan.VoIPSecurity == UMVoIPSecurityType.Unsecured;
			case UMStartupMode.TLS:
				return diaplan.VoIPSecurity != UMVoIPSecurityType.Unsecured;
			case UMStartupMode.Dual:
				return true;
			default:
				throw new NotImplementedException(UmServiceGlobals.StartupMode.ToString() + " type is not supported");
			}
		}

		private Random randomNumberGenerator = new Random();
	}
}
