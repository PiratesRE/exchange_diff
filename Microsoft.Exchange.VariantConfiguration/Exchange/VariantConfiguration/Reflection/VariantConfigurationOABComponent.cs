using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOABComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOABComponent() : base("OAB")
		{
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "LinkedOABGenMailboxes", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "SkipServiceTopologyDiscovery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "EnforceManifestVersionMatch", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "SharedTemplateFiles", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "GenerateRequestedOABsOnly", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OAB.settings.ini", "OabHttpClientAccessRulesEnabled", typeof(IFeature), false));
		}

		public VariantConfigurationSection LinkedOABGenMailboxes
		{
			get
			{
				return base["LinkedOABGenMailboxes"];
			}
		}

		public VariantConfigurationSection SkipServiceTopologyDiscovery
		{
			get
			{
				return base["SkipServiceTopologyDiscovery"];
			}
		}

		public VariantConfigurationSection EnforceManifestVersionMatch
		{
			get
			{
				return base["EnforceManifestVersionMatch"];
			}
		}

		public VariantConfigurationSection SharedTemplateFiles
		{
			get
			{
				return base["SharedTemplateFiles"];
			}
		}

		public VariantConfigurationSection GenerateRequestedOABsOnly
		{
			get
			{
				return base["GenerateRequestedOABsOnly"];
			}
		}

		public VariantConfigurationSection OabHttpClientAccessRulesEnabled
		{
			get
			{
				return base["OabHttpClientAccessRulesEnabled"];
			}
		}
	}
}
