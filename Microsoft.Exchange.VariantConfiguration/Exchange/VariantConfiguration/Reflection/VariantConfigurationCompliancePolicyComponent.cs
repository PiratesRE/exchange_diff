using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationCompliancePolicyComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationCompliancePolicyComponent() : base("CompliancePolicy")
		{
			base.Add(new VariantConfigurationSection("CompliancePolicy.settings.ini", "ProcessForestWideOrgEtrs", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CompliancePolicy.settings.ini", "ShowSupervisionPredicate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CompliancePolicy.settings.ini", "ValidateTenantOutboundConnector", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CompliancePolicy.settings.ini", "RuleConfigurationAdChangeNotifications", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("CompliancePolicy.settings.ini", "QuarantineAction", typeof(IFeature), false));
		}

		public VariantConfigurationSection ProcessForestWideOrgEtrs
		{
			get
			{
				return base["ProcessForestWideOrgEtrs"];
			}
		}

		public VariantConfigurationSection ShowSupervisionPredicate
		{
			get
			{
				return base["ShowSupervisionPredicate"];
			}
		}

		public VariantConfigurationSection ValidateTenantOutboundConnector
		{
			get
			{
				return base["ValidateTenantOutboundConnector"];
			}
		}

		public VariantConfigurationSection RuleConfigurationAdChangeNotifications
		{
			get
			{
				return base["RuleConfigurationAdChangeNotifications"];
			}
		}

		public VariantConfigurationSection QuarantineAction
		{
			get
			{
				return base["QuarantineAction"];
			}
		}
	}
}
