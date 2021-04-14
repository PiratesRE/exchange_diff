using System;
using Microsoft.Exchange.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationConfig : ConfigurableObject
	{
		public MigrationConfig() : base(new SimpleProviderPropertyBag())
		{
		}

		public new MigrationConfigId Identity
		{
			get
			{
				return (MigrationConfigId)base.Identity;
			}
			internal set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		public int MaxNumberOfBatches
		{
			get
			{
				return (int)this[MigrationConfig.MigrationConfigSchema.MaxNumberOfBatches];
			}
			internal set
			{
				this[MigrationConfig.MigrationConfigSchema.MaxNumberOfBatches] = value;
			}
		}

		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)this[MigrationConfig.MigrationConfigSchema.MaxConcurrentMigrations];
			}
			internal set
			{
				this[MigrationConfig.MigrationConfigSchema.MaxConcurrentMigrations] = value;
			}
		}

		public MigrationFeature Features
		{
			get
			{
				return (MigrationFeature)this[MigrationConfig.MigrationConfigSchema.MigrationFeatures];
			}
			internal set
			{
				this[MigrationConfig.MigrationConfigSchema.MigrationFeatures] = value;
			}
		}

		public bool CanSubmitNewBatch
		{
			get
			{
				return (bool)this[MigrationConfig.MigrationConfigSchema.CanSubmitNewBatch];
			}
			internal set
			{
				this[MigrationConfig.MigrationConfigSchema.CanSubmitNewBatch] = value;
			}
		}

		public bool SupportsCutover
		{
			get
			{
				return (bool)this[MigrationConfig.MigrationConfigSchema.SupportsCutover];
			}
			internal set
			{
				this[MigrationConfig.MigrationConfigSchema.SupportsCutover] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MigrationConfig.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public override string ToString()
		{
			return Strings.MigrationConfigString(this.MaxNumberOfBatches.ToString(), this.Features.ToString());
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<MigrationConfig.MigrationConfigSchema>();

		private class MigrationConfigSchema : SimpleProviderObjectSchema
		{
			public static readonly SimpleProviderPropertyDefinition MaxNumberOfBatches = new SimpleProviderPropertyDefinition("MaxNumberOfBatches", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 100, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly SimpleProviderPropertyDefinition MigrationFeatures = new SimpleProviderPropertyDefinition("MigrationFeatures", ExchangeObjectVersion.Exchange2010, typeof(MigrationFeature), PropertyDefinitionFlags.None, MigrationFeature.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition CanSubmitNewBatch = new SimpleProviderPropertyDefinition("CanSubmitNewBatch", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition SupportsCutover = new SimpleProviderPropertyDefinition("SupportsCutover", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MaxConcurrentMigrations = new SimpleProviderPropertyDefinition("MaxConcurrentMigrations", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.TaskPopulated, Unlimited<int>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
