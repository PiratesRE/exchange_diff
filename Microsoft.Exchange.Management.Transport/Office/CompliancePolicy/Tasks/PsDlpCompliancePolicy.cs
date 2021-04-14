using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class PsDlpCompliancePolicy : PsCompliancePolicyBase
	{
		public PsDlpCompliancePolicy()
		{
		}

		public PsDlpCompliancePolicy(PolicyStorage policyStorage) : base(policyStorage)
		{
		}

		public PolicyScenario Type
		{
			get
			{
				return PolicyScenario.Dlp;
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PsDlpCompliancePolicy.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly PsCompliancePolicyBaseSchema schema = ObjectSchema.GetInstance<PsCompliancePolicyBaseSchema>();
	}
}
