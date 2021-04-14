using System;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal static class CapabilityIdentifierEvaluatorFactory
	{
		internal static CapabilityIdentifierEvaluator Create(Capability capability)
		{
			ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "Entering ValidationRuleFactory.GetEvaluator({0})", capability.ToString());
			switch (capability)
			{
			case Capability.BPOS_S_Deskless:
			case Capability.BPOS_S_Standard:
			case Capability.BPOS_S_Enterprise:
			case Capability.BPOS_S_Archive:
			case Capability.BPOS_L_Standard:
			case Capability.BPOS_B_Standard:
			case Capability.BPOS_B_CustomDomain:
			case Capability.BPOS_S_MidSize:
			case Capability.BPOS_S_ArchiveAddOn:
			case Capability.BPOS_S_EopStandardAddOn:
			case Capability.BPOS_S_EopPremiumAddOn:
			case Capability.BPOS_Unmanaged:
			case Capability.TOU_Signed:
			case Capability.Partner_Managed:
			case Capability.ExcludedFromBackSync:
			case Capability.OrganizationCapabilityUMGrammar:
			case Capability.OrganizationCapabilityUMDataStorage:
			case Capability.OrganizationCapabilityOABGen:
			case Capability.OrganizationCapabilityGMGen:
			case Capability.OrganizationCapabilityClientExtensions:
			case Capability.OrganizationCapabilityUMGrammarReady:
			case Capability.OrganizationCapabilityMailRouting:
			case Capability.OrganizationCapabilityManagement:
			case Capability.OrganizationCapabilityTenantUpgrade:
			case Capability.OrganizationCapabilityScaleOut:
			case Capability.OrganizationCapabilityMessageTracking:
			case Capability.OrganizationCapabilityPstProvider:
			case Capability.OrganizationCapabilitySuiteServiceStorage:
			case Capability.OrganizationCapabilityOfficeMessageEncryption:
			case Capability.OrganizationCapabilityMigration:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns SimpleCapabilityIdentifierEvaluator", capability.ToString());
				return new SimpleCapabilityIdentifierEvaluator(capability);
			case Capability.FederatedUser:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns FederatedUserCapabilityIdentifierEvaluator", capability.ToString());
				return new FederatedUserCapabilityIdentifierEvaluator(capability);
			case Capability.MasteredOnPremise:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns MasteredOnPremiseCapabilityIdentifierEvaluator", capability.ToString());
				return new MasteredOnPremiseCapabilityIdentifierEvaluator(capability);
			case Capability.ResourceMailbox:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns ResourceMailboxCapabilityIdentifierEvaluator", capability.ToString());
				return new ResourceMailboxCapabilityIdentifierEvaluator(capability);
			case Capability.UMFeatureRestricted:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns UMFeatureRestrictedCapabilityIdentifierEvaluator", capability.ToString());
				return new UMFeatureRestrictedCapabilityIdentifierEvaluator(capability);
			case Capability.RichCoexistence:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns RichCoexistenceCapabilityIdentifierEvaluator", capability.ToString());
				return new RichCoexistenceCapabilityIdentifierEvaluator(capability);
			case Capability.BEVDirLockdown:
				ExTraceGlobals.PublicCreationAPITracer.TraceDebug<string>((long)capability.GetHashCode(), "ValidationRuleFactory.GetEvaluator({0}) returns BEVDirLockdownCapabilityIdentifierEvaluator", capability.ToString());
				return new BEVDirLockdownCapabilityIdentifierEvaluator(capability);
			}
			throw new NotSupportedException("Unsupported capability." + capability.ToString());
		}

		internal static MultiValuedProperty<Capability> GetCapabilities(ADRawEntry adObject)
		{
			MultiValuedProperty<Capability> multiValuedProperty = new MultiValuedProperty<Capability>();
			foreach (object obj in Enum.GetValues(typeof(Capability)))
			{
				Capability capability = (Capability)obj;
				if (capability != Capability.None)
				{
					CapabilityIdentifierEvaluator capabilityIdentifierEvaluator = CapabilityIdentifierEvaluatorFactory.Create(capability);
					CapabilityEvaluationResult capabilityEvaluationResult = capabilityIdentifierEvaluator.Evaluate(adObject);
					if (capabilityEvaluationResult == CapabilityEvaluationResult.Yes)
					{
						multiValuedProperty.Add(capability);
					}
				}
			}
			return multiValuedProperty;
		}
	}
}
