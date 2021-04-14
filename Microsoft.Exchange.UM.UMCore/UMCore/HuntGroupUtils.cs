using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class HuntGroupUtils
	{
		public static bool TryGetDefaultHuntGroup(UMIPGateway gatewayConfig, string pilotNumber, out UMHuntGroup huntGroup)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "HuntgroupUtils: TryGetDefaultHuntGroup : pilotNumber :{0}, gateway: {1}", new object[]
			{
				pilotNumber,
				gatewayConfig.Name
			});
			huntGroup = null;
			MultiValuedProperty<UMHuntGroup> huntGroups = gatewayConfig.HuntGroups;
			for (int i = 0; i < huntGroups.Count; i++)
			{
				UMHuntGroup umhuntGroup = huntGroups[i];
				if (string.IsNullOrEmpty(umhuntGroup.PilotIdentifier))
				{
					huntGroup = umhuntGroup;
					break;
				}
			}
			return huntGroup != null;
		}

		public static UMHuntGroup GetHuntGroup(string pilotNumber, UMIPGateway gatewayConfig, PlatformSipUri requestUriOfCall, UMSmartHost remotePeer, IADSystemConfigurationLookup adSession, bool securedCall, out ADObjectId gatewayDialPlanId, out bool gatewayInOnlyOneDialplan)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "HuntgroupUtils: GetHuntGroup : pilotNumber :{0}, gateway: {1}", new object[]
			{
				pilotNumber,
				gatewayConfig.Name
			});
			MultiValuedProperty<UMHuntGroup> huntGroups = gatewayConfig.HuntGroups;
			UMHuntGroup result = null;
			gatewayDialPlanId = null;
			gatewayInOnlyOneDialplan = true;
			if (pilotNumber == null)
			{
				if (huntGroups != null && huntGroups.Count > 0 && !HuntGroupUtils.TryFindHuntgroupWhenNoPilotIdentifier(huntGroups, adSession, securedCall, out result))
				{
					throw CallRejectedException.Create(Strings.CallFromInvalidHuntGroup(requestUriOfCall.ToString(), remotePeer.ToString()), CallEndingReason.IncorrectHuntGroup, "Pilot number: {0}. UMIPGateway: {1}.", new object[]
					{
						requestUriOfCall.ToString(),
						remotePeer.ToString()
					});
				}
				using (MultiValuedProperty<UMHuntGroup>.Enumerator enumerator = huntGroups.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						UMHuntGroup umhuntGroup = enumerator.Current;
						if (gatewayDialPlanId == null)
						{
							gatewayDialPlanId = umhuntGroup.UMDialPlan;
						}
						else if (gatewayDialPlanId.ObjectGuid != umhuntGroup.UMDialPlan.ObjectGuid)
						{
							gatewayInOnlyOneDialplan = false;
						}
					}
					return result;
				}
			}
			foreach (UMHuntGroup umhuntGroup2 in huntGroups)
			{
				if (gatewayDialPlanId == null)
				{
					gatewayDialPlanId = umhuntGroup2.UMDialPlan;
				}
				else if (gatewayDialPlanId.ObjectGuid != umhuntGroup2.UMDialPlan.ObjectGuid)
				{
					gatewayInOnlyOneDialplan = false;
				}
				if (string.Equals(umhuntGroup2.PilotIdentifier, pilotNumber, StringComparison.InvariantCultureIgnoreCase))
				{
					UMDialPlan dialPlanFromId = adSession.GetDialPlanFromId(umhuntGroup2.UMDialPlan);
					if (dialPlanFromId != null)
					{
						bool flag = dialPlanFromId.VoIPSecurity != UMVoIPSecurityType.Unsecured;
						if (flag != securedCall)
						{
							throw CallRejectedException.Create(Strings.CallFromInvalidHuntGroup(pilotNumber, remotePeer.ToString()), CallEndingReason.IncorrectHuntGroup, "Pilot number: {0}. UMIPGateway: {1}.", new object[]
							{
								pilotNumber,
								remotePeer.ToString()
							});
						}
						result = umhuntGroup2;
						break;
					}
				}
			}
			return result;
		}

		private static bool TryFindHuntgroupWhenNoPilotIdentifier(MultiValuedProperty<UMHuntGroup> huntGroups, IADSystemConfigurationLookup adSession, bool securedCall, out UMHuntGroup hg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "HuntgroupUtils: TryFindHuntgroupWhenNoPilotIdentifier ", new object[0]);
			hg = null;
			for (int i = 0; i < huntGroups.Count; i++)
			{
				UMHuntGroup umhuntGroup = huntGroups[i];
				if (hg == null || string.IsNullOrEmpty(umhuntGroup.PilotIdentifier))
				{
					UMDialPlan dialPlanFromId = adSession.GetDialPlanFromId(umhuntGroup.UMDialPlan);
					bool flag = dialPlanFromId == null;
					FaultInjectionUtils.FaultInjectChangeValue<bool>(3945147709U, ref flag);
					if (flag)
					{
						CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "TryFindHuntgroupWhenNoPilotIdentifier - Dial plan object '{0}' not found.", new object[]
						{
							umhuntGroup.UMDialPlan
						});
						throw CallRejectedException.Create(Strings.DialPlanNotFound(umhuntGroup.UMDialPlan.Name), CallEndingReason.DialPlanNotFound, "UM dial plan: {0}.", new object[]
						{
							umhuntGroup.UMDialPlan.Name
						});
					}
					bool flag2 = dialPlanFromId.VoIPSecurity != UMVoIPSecurityType.Unsecured;
					if (flag2 == securedCall)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "TryFindHuntgroupWhenNoPilotIdentifier - Found hunt group '{0}'.", new object[]
						{
							umhuntGroup.Name
						});
						hg = umhuntGroup;
						if (string.IsNullOrEmpty(umhuntGroup.PilotIdentifier))
						{
							break;
						}
					}
				}
			}
			if (hg == null)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, null, "TryFindHuntgroupWhenNoPilotIdentifier - Hunt group not found.", new object[0]);
			}
			return hg != null;
		}
	}
}
