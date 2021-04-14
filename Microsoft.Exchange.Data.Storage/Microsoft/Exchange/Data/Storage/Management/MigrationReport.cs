using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationReport : ConfigurableObject
	{
		public MigrationReport() : base(new SimplePropertyBag(MigrationReport.MigrationReportSchema.Identity, MigrationReport.MigrationReportSchema.ObjectState, MigrationReport.MigrationReportSchema.ExchangeVersion))
		{
			base.ResetChangeTracking();
		}

		public new MigrationReportId Identity
		{
			get
			{
				return (MigrationReportId)base.Identity;
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.Identity] = value;
			}
		}

		public string ReportName
		{
			get
			{
				return (string)this[MigrationReport.MigrationReportSchema.MigrationReportName];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.MigrationReportName] = value;
			}
		}

		public Guid JobId
		{
			get
			{
				return (Guid)this[MigrationReport.MigrationReportSchema.MigrationReportId];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.MigrationReportId] = value;
			}
		}

		public MigrationType MigrationType
		{
			get
			{
				return (MigrationType)this[MigrationReport.MigrationReportSchema.MigrationType];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.MigrationType] = (int)value;
			}
		}

		public MigrationReportType ReportType
		{
			get
			{
				return (MigrationReportType)this[MigrationReport.MigrationReportSchema.MigrationReportType];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.MigrationReportType] = (int)value;
			}
		}

		public bool IsStagedMigration
		{
			get
			{
				return (bool)this[MigrationReport.MigrationReportSchema.IsStagedMigration];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.IsStagedMigration] = value;
			}
		}

		public MultiValuedProperty<string> Rows
		{
			get
			{
				return (MultiValuedProperty<string>)this[MigrationReport.MigrationReportSchema.MigrationReportRows];
			}
			internal set
			{
				this[MigrationReport.MigrationReportSchema.MigrationReportRows] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MigrationReport.MigrationReportSchema>();
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private class MigrationReportSchema : ObjectSchema
		{
			public static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(MigrationReportId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ObjectState = UserConfigurationObjectSchema.ObjectState;

			public static readonly ProviderPropertyDefinition ExchangeVersion = UserConfigurationObjectSchema.ExchangeVersion;

			public static readonly ProviderPropertyDefinition MigrationReportName = new SimpleProviderPropertyDefinition("MigrationReportName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MigrationReportId = new SimpleProviderPropertyDefinition("MigrationReportId", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.TaskPopulated, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MigrationType = new SimpleProviderPropertyDefinition("MigrationType", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition IsStagedMigration = new SimpleProviderPropertyDefinition("IsStagedMigration", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MigrationReportType = new SimpleProviderPropertyDefinition("MigrationReportType", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MigrationReportRows = new SimpleProviderPropertyDefinition("MigrationReportRows", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
