using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class DeprecatedLoadBalanceSettings : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DeprecatedLoadBalanceSettings.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DeprecatedLoadBalanceSettings.mostDerivedClass;
			}
		}

		public MultiValuedProperty<ADObjectId> IncludedMailboxDatabases
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[DeprecatedLoadBalanceSettingsSchema.IncludedMailboxDatabases];
			}
			internal set
			{
				this[DeprecatedLoadBalanceSettingsSchema.IncludedMailboxDatabases] = value;
			}
		}

		[Parameter]
		public bool UseIncludedMailboxDatabases
		{
			get
			{
				return (bool)this[DeprecatedLoadBalanceSettingsSchema.UseIncludedMailboxDatabases];
			}
			set
			{
				this[DeprecatedLoadBalanceSettingsSchema.UseIncludedMailboxDatabases] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ExcludedMailboxDatabases
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[DeprecatedLoadBalanceSettingsSchema.ExcludedMailboxDatabases];
			}
			internal set
			{
				this[DeprecatedLoadBalanceSettingsSchema.ExcludedMailboxDatabases] = value;
			}
		}

		[Parameter]
		public bool UseExcludedMailboxDatabases
		{
			get
			{
				return (bool)this[DeprecatedLoadBalanceSettingsSchema.UseExcludedMailboxDatabases];
			}
			set
			{
				this[DeprecatedLoadBalanceSettingsSchema.UseExcludedMailboxDatabases] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.UseIncludedMailboxDatabases && this.UseExcludedMailboxDatabases)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.LoadBalanceCannotUseBothInclusionLists, DeprecatedLoadBalanceSettingsSchema.UseExcludedMailboxDatabases, this.UseIncludedMailboxDatabases));
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static DeprecatedLoadBalanceSettingsSchema schema = ObjectSchema.GetInstance<DeprecatedLoadBalanceSettingsSchema>();

		private static string mostDerivedClass = "msExchLoadBalancingSettings";
	}
}
