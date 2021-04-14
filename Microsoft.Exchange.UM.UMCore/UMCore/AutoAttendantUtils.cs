using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AutoAttendantUtils
	{
		public static UMAutoAttendant LookupAutoAttendantInDialPlan(string pilotNumberOrName, bool numberIsPilot, ADObjectId dialPlanId, IADSystemConfigurationLookup adSession)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils :LookupAutoAttendantInDialPlan", new object[0]);
			UMAutoAttendant result;
			if (numberIsPilot)
			{
				result = adSession.GetAutoAttendantFromPilotIdentifierAndDialPlan(pilotNumberOrName, dialPlanId);
			}
			else
			{
				result = AutoAttendantUtils.GetFirstOrgAutoAttendantFromNameInDialPlan(pilotNumberOrName, dialPlanId);
			}
			return result;
		}

		public static UMAutoAttendant GetAutoAttendant(string pilotNumber, UMHuntGroup hg, PlatformSipUri requestUri, bool securedCall, IADSystemConfigurationLookup adSession)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils :GetAutoAttendant using huntgroup", new object[0]);
			UMDialPlan umdialPlan = null;
			string simplifiedUri = requestUri.SimplifiedUri;
			string pilotNumberOrName;
			UMAutoAttendant result;
			if (AutoAttendantUtils.IsValidAASipUri(simplifiedUri, securedCall, out pilotNumberOrName, out umdialPlan) && string.Compare(umdialPlan.Id.DistinguishedName, hg.UMDialPlan.DistinguishedName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = AutoAttendantUtils.LookupAutoAttendantInDialPlan(pilotNumberOrName, false, umdialPlan.Id, adSession);
			}
			else
			{
				result = AutoAttendantUtils.LookupAutoAttendantInDialPlan(pilotNumber, true, hg.UMDialPlan, adSession);
			}
			return result;
		}

		public static UMAutoAttendant GetAutoAttendant(string pilotNumber, PlatformSipUri requestUri, UMIPGateway gateway, bool gatewayInOnlyOneDialplan, bool securedCall, ADObjectId gatewayDialPlanId, IADSystemConfigurationLookup adSession)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils :GetAutoAttendant", new object[0]);
			UMDialPlan umdialPlan = null;
			string pilotNumberOrName = null;
			string simplifiedUri = requestUri.SimplifiedUri;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "TryGetAutoAttendant() Pilot#: [{0}] ExtendedCalleeId: [{1}] GWinOnlyOneDIalPlan: [{2}].", new object[]
			{
				pilotNumber,
				simplifiedUri,
				gatewayInOnlyOneDialplan
			});
			UMAutoAttendant result;
			if (AutoAttendantUtils.IsValidAASipUri(simplifiedUri, securedCall, out pilotNumberOrName, out umdialPlan))
			{
				result = AutoAttendantUtils.LookupAutoAttendantInDialPlan(pilotNumberOrName, false, umdialPlan.Id, adSession);
			}
			else
			{
				if (gatewayDialPlanId == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "HandleDirectAutoAttendantCallNoHuntGroup: gatewayDialPlanId=null, returning.", new object[0]);
					return null;
				}
				UMDialPlan dialPlanFromId = adSession.GetDialPlanFromId(gatewayDialPlanId);
				if (dialPlanFromId == null)
				{
					throw CallRejectedException.Create(Strings.DialPlanNotFound(gatewayDialPlanId.DistinguishedName), CallEndingReason.DialPlanNotFound, "UM dial plan: {0}.", new object[]
					{
						gatewayDialPlanId.Name
					});
				}
				if (!gatewayInOnlyOneDialplan)
				{
					return null;
				}
				UMUriType uritype = dialPlanFromId.URIType;
				bool flag;
				if (uritype == UMUriType.SipName)
				{
					flag = Utils.IsUriValid(simplifiedUri, dialPlanFromId.URIType);
				}
				else
				{
					flag = Utils.IsUriValid(pilotNumber, dialPlanFromId.URIType, dialPlanFromId.NumberOfDigitsInExtension);
				}
				if (!flag)
				{
					CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "The Uri:PilotNumber {0}:{1} is not valid according to dialplan uritype: {2}.", new object[]
					{
						simplifiedUri,
						pilotNumber,
						dialPlanFromId.URIType
					});
					return null;
				}
				result = AutoAttendantUtils.LookupAutoAttendantInDialPlan(pilotNumber, true, gatewayDialPlanId, adSession);
			}
			return result;
		}

		public static bool IsAutoAttendantUsable(UMAutoAttendant aa, string pilotNumberOrName)
		{
			bool result = true;
			if (aa != null)
			{
				LocalizedString localizedString;
				if (aa.Status != StatusEnum.Enabled)
				{
					CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils : VerifyAutoAttendantIsUsable :AutoAttendant with PilotNumber {0} in dialplan {1} is not Enabled.", new object[]
					{
						pilotNumberOrName,
						aa.UMDialPlan.Name
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallToUnusableAA, null, new object[]
					{
						aa.Name,
						Strings.DisabledAA
					});
					result = false;
				}
				else if (!AutoAttendantCore.IsRunnableAutoAttendant(aa, out localizedString))
				{
					CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils : VerifyAutoAttendantIsUsable :AutoAttendant with PilotNumber {0} in dialplan {1} is not Enabled, or does not have any features enabled.", new object[]
					{
						pilotNumberOrName,
						aa.UMDialPlan.Name
					});
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallToUnusableAA, null, new object[]
					{
						aa.Name,
						localizedString
					});
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool IsValidAASipUri(string extendedCalleeId, bool securedCall, out string autoAttendantName, out UMDialPlan dialPlan)
		{
			autoAttendantName = null;
			string text = null;
			dialPlan = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "IsValidAASipUri({0}).", new object[]
			{
				extendedCalleeId
			});
			if (!Utils.IsUriValid(extendedCalleeId, UMUriType.SipName))
			{
				return false;
			}
			int num = extendedCalleeId.IndexOf("@", StringComparison.InvariantCulture);
			if (num == -1)
			{
				return false;
			}
			string text2 = extendedCalleeId.Substring(0, num);
			int num2 = text2.IndexOf(".", StringComparison.InvariantCulture);
			string text3;
			if (num2 != -1)
			{
				text3 = text2.Substring(0, num2);
				text = text2.Substring(num2 + 1);
			}
			else
			{
				text3 = text2;
			}
			if (text == null)
			{
				return false;
			}
			UMDialPlan firstOrgDialPlanFromName = AutoAttendantUtils.GetFirstOrgDialPlanFromName(text);
			if (firstOrgDialPlanFromName == null)
			{
				throw CallRejectedException.Create(Strings.DialPlanNotFound(text), CallEndingReason.DialPlanNotFound, "UM dial plan: {0}.", new object[]
				{
					text
				});
			}
			if (firstOrgDialPlanFromName.URIType == UMUriType.SipName)
			{
				bool flag = firstOrgDialPlanFromName.VoIPSecurity != UMVoIPSecurityType.Unsecured;
				if (flag == securedCall)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "IsValidAASipUri(): URI {0} is a valid SIP application name, where AppId= {1}, in SIP DialPlan: {2}.", new object[]
					{
						extendedCalleeId,
						text3,
						text
					});
					dialPlan = firstOrgDialPlanFromName;
					autoAttendantName = text3;
					return true;
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "IsValidAASipUri(): URI {0} is not a valid SIP URI of an application, where application name= {1}, in SIP DialPlan: {2}.", new object[]
			{
				extendedCalleeId,
				text3,
				text ?? "<null>"
			});
			return false;
		}

		private static UMDialPlan GetFirstOrgDialPlanFromName(string dialPlanName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils :GetFirstOrgDialPlanFromName dpName:{0}", new object[]
			{
				dialPlanName
			});
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 483, "GetFirstOrgDialPlanFromName", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\callrouter\\AutoAttendantUtils.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, dialPlanName);
			ADObjectId descendantId = tenantOrTopologyConfigurationSession.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=UM DialPlan Container", Guid.Empty));
			UMDialPlan[] array = tenantOrTopologyConfigurationSession.Find<UMDialPlan>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		private static UMAutoAttendant GetFirstOrgAutoAttendantFromNameInDialPlan(string autoAttendantName, ADObjectId dialPlanId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "AutoAttendantUtils :GetFirstOrgAutoAttendantFromNameInDialPlan autoAttendantName:{0}", new object[]
			{
				autoAttendantName
			});
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 522, "GetFirstOrgAutoAttendantFromNameInDialPlan", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\callrouter\\AutoAttendantUtils.cs");
			QueryFilter filter = new AndFilter(new List<QueryFilter>
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, autoAttendantName),
				new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, dialPlanId)
			}.ToArray());
			ADObjectId descendantId = tenantOrTopologyConfigurationSession.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=UM AutoAttendant Container", Guid.Empty));
			UMAutoAttendant[] array = tenantOrTopologyConfigurationSession.Find<UMAutoAttendant>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			return null;
		}
	}
}
