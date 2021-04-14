using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationADComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationADComponent() : base("AD")
		{
			base.Add(new VariantConfigurationSection("AD.settings.ini", "DelegatedSetupRoleGroupValue", typeof(IDelegatedSetupRoleGroupSettings), false));
			base.Add(new VariantConfigurationSection("AD.settings.ini", "DisplayNameMustContainReadableCharacter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("AD.settings.ini", "MailboxLocations", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("AD.settings.ini", "EnableUseIsDescendantOfForRecipientViewRoot", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("AD.settings.ini", "UseGlobalCatalogIsSetToFalse", typeof(IFeature), false));
		}

		public VariantConfigurationSection DelegatedSetupRoleGroupValue
		{
			get
			{
				return base["DelegatedSetupRoleGroupValue"];
			}
		}

		public VariantConfigurationSection DisplayNameMustContainReadableCharacter
		{
			get
			{
				return base["DisplayNameMustContainReadableCharacter"];
			}
		}

		public VariantConfigurationSection MailboxLocations
		{
			get
			{
				return base["MailboxLocations"];
			}
		}

		public VariantConfigurationSection EnableUseIsDescendantOfForRecipientViewRoot
		{
			get
			{
				return base["EnableUseIsDescendantOfForRecipientViewRoot"];
			}
		}

		public VariantConfigurationSection UseGlobalCatalogIsSetToFalse
		{
			get
			{
				return base["UseGlobalCatalogIsSetToFalse"];
			}
		}
	}
}
