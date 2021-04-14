using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class UMFeatureRestrictedCapabilityIdentifierEvaluator : CapabilityIdentifierEvaluator
	{
		public UMFeatureRestrictedCapabilityIdentifierEvaluator(Capability capability) : base(capability)
		{
		}

		public override CapabilityEvaluationResult Evaluate(ADRawEntry adObject)
		{
			if (adObject == null)
			{
				throw new ArgumentNullException("adObject");
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Entering UMFeatureRestrictedCapabilityIdentifierEvaluator.Evaluate('{0}') CapabilityToCheck '{1}'.", adObject.GetDistinguishedNameOrName(), base.Capability.ToString());
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "MasteredOnPremiseCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'. CapabilityToCheck '{2}' - not datacenter mode.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			CountryInfo countryInfo = (CountryInfo)adObject[ADRecipientSchema.UsageLocation];
			if (null == countryInfo)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "UMFeatureRestrictedCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'.  CapabilityToCheck '{2}' - UsageLocation is '<NULL>'.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.NotApplicable.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.NotApplicable;
			}
			CountryList countryList = CountryListIdCache.Singleton.Get(UMFeatureRestrictedCapabilityIdentifierEvaluator.UMCountryListKey);
			if (countryList == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceWarning<string, string, string>((long)this.GetHashCode(), "UMFeatureRestrictedCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'.  CapabilityToCheck '{2}' - Cache lookup returned '<NULL>'.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.Yes.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.Yes;
			}
			if (countryList.Countries.Contains(countryInfo))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "UMFeatureRestrictedCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'.  CapabilityToCheck '{2}'.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.No.ToString(), base.Capability.ToString());
				return CapabilityEvaluationResult.No;
			}
			ExTraceGlobals.AccessCheckTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "UMFeatureRestrictedCapabilityIdentifierEvaluator.Evaluate('{0}') return '{1}'.  CapabilityToCheck '{2}'.", adObject.GetDistinguishedNameOrName(), CapabilityEvaluationResult.Yes.ToString(), base.Capability.ToString());
			return CapabilityEvaluationResult.Yes;
		}

		public override bool TryGetFilter(OrganizationId organizationId, out QueryFilter queryFilter, out LocalizedString errorMessage)
		{
			errorMessage = LocalizedString.Empty;
			CountryList countryList = CountryListIdCache.Singleton.Get(UMFeatureRestrictedCapabilityIdentifierEvaluator.UMCountryListKey);
			if (countryList == null)
			{
				queryFilter = UMFeatureRestrictedCapabilityIdentifierEvaluator.existsFilter;
				return true;
			}
			QueryFilter[] array = new QueryFilter[countryList.Countries.Count + 1];
			for (int i = 0; i < countryList.Countries.Count; i++)
			{
				array[i] = new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.InternalUsageLocation, countryList.Countries[i].Name);
			}
			array[countryList.Countries.Count] = UMFeatureRestrictedCapabilityIdentifierEvaluator.existsFilter;
			queryFilter = QueryFilter.AndTogether(array);
			return true;
		}

		private static readonly CountryListKey UMCountryListKey = new CountryListKey(CountryList.UMAllowedCountryListName);

		private static readonly ExistsFilter existsFilter = new ExistsFilter(ADRecipientSchema.InternalUsageLocation);
	}
}
