using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationUMComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationUMComponent() : base("UM")
		{
			base.Add(new VariantConfigurationSection("UM.settings.ini", "UMDataCenterLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "VoicemailDiskSpaceDatacenterLimit", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "DatacenterUMGrammarTenantCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "DirectoryGrammarCountLimit", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "UMDataCenterAD", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "AddressListGrammars", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "GetServerDialPlans", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "UMDataCenterLanguages", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "UMDataCenterCallRouting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "HuntGroupCreationForSipDialplans", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "DTMFMapGenerator", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "AlwaysLogTraces", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "AnonymizeLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "ServerDialPlanLink", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("UM.settings.ini", "SipInfoNotifications", typeof(IFeature), false));
		}

		public VariantConfigurationSection UMDataCenterLogging
		{
			get
			{
				return base["UMDataCenterLogging"];
			}
		}

		public VariantConfigurationSection VoicemailDiskSpaceDatacenterLimit
		{
			get
			{
				return base["VoicemailDiskSpaceDatacenterLimit"];
			}
		}

		public VariantConfigurationSection DatacenterUMGrammarTenantCache
		{
			get
			{
				return base["DatacenterUMGrammarTenantCache"];
			}
		}

		public VariantConfigurationSection DirectoryGrammarCountLimit
		{
			get
			{
				return base["DirectoryGrammarCountLimit"];
			}
		}

		public VariantConfigurationSection UMDataCenterAD
		{
			get
			{
				return base["UMDataCenterAD"];
			}
		}

		public VariantConfigurationSection AddressListGrammars
		{
			get
			{
				return base["AddressListGrammars"];
			}
		}

		public VariantConfigurationSection GetServerDialPlans
		{
			get
			{
				return base["GetServerDialPlans"];
			}
		}

		public VariantConfigurationSection UMDataCenterLanguages
		{
			get
			{
				return base["UMDataCenterLanguages"];
			}
		}

		public VariantConfigurationSection UMDataCenterCallRouting
		{
			get
			{
				return base["UMDataCenterCallRouting"];
			}
		}

		public VariantConfigurationSection HuntGroupCreationForSipDialplans
		{
			get
			{
				return base["HuntGroupCreationForSipDialplans"];
			}
		}

		public VariantConfigurationSection DTMFMapGenerator
		{
			get
			{
				return base["DTMFMapGenerator"];
			}
		}

		public VariantConfigurationSection AlwaysLogTraces
		{
			get
			{
				return base["AlwaysLogTraces"];
			}
		}

		public VariantConfigurationSection AnonymizeLogging
		{
			get
			{
				return base["AnonymizeLogging"];
			}
		}

		public VariantConfigurationSection ServerDialPlanLink
		{
			get
			{
				return base["ServerDialPlanLink"];
			}
		}

		public VariantConfigurationSection SipInfoNotifications
		{
			get
			{
				return base["SipInfoNotifications"];
			}
		}
	}
}
