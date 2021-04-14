using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class DeviceConditionalAccessPolicy : PsCompliancePolicyBase
	{
		private new MultiValuedProperty<BindingMetadata> ExchangeBinding { get; set; }

		private new MultiValuedProperty<BindingMetadata> SharePointBinding { get; set; }

		public DeviceConditionalAccessPolicy()
		{
		}

		public DeviceConditionalAccessPolicy(PolicyStorage policyStorage) : base(policyStorage)
		{
		}

		public PolicyScenario Type
		{
			get
			{
				return PolicyScenario.DeviceConditionalAccess;
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return DeviceConditionalAccessPolicy.policySchema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly PsCompliancePolicyBaseSchema policySchema = ObjectSchema.GetInstance<PsCompliancePolicyBaseSchema>();
	}
}
