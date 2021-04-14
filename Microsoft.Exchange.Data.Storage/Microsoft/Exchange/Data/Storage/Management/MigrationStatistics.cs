using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationStatistics : ConfigurableObject
	{
		public MigrationStatistics() : base(new SimpleProviderPropertyBag())
		{
		}

		public new MigrationStatisticsId Identity
		{
			get
			{
				return (MigrationStatisticsId)base.Identity;
			}
			internal set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		public int TotalCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.TotalCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.TotalCount] = value;
			}
		}

		public int ActiveCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.ActiveCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.ActiveCount] = value;
			}
		}

		public int StoppedCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.StoppedCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.StoppedCount] = value;
			}
		}

		public int SyncedCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.SyncedCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.SyncedCount] = value;
			}
		}

		public int FinalizedCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.FinalizedCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.FinalizedCount] = value;
			}
		}

		public int FailedCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.FailedCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.FailedCount] = value;
			}
		}

		public int PendingCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.PendingCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.PendingCount] = value;
			}
		}

		public int ProvisionedCount
		{
			get
			{
				return (int)this[MigrationStatistics.MigrationStatisticsSchema.ProvisionedCount];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.ProvisionedCount] = value;
			}
		}

		public MigrationType MigrationType
		{
			get
			{
				return (MigrationType)this[MigrationStatistics.MigrationStatisticsSchema.MigrationType];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.MigrationType] = (int)value;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return (string)this[MigrationStatistics.MigrationStatisticsSchema.DiagnosticInfo];
			}
			internal set
			{
				this[MigrationStatistics.MigrationStatisticsSchema.DiagnosticInfo] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MigrationStatistics.MigrationStatisticsSchema>();
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private class MigrationStatisticsSchema : SimpleProviderObjectSchema
		{
			public static readonly ProviderPropertyDefinition TotalCount = new SimpleProviderPropertyDefinition("TotalCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ActiveCount = new SimpleProviderPropertyDefinition("ActiveCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition StoppedCount = new SimpleProviderPropertyDefinition("StoppedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition SyncedCount = new SimpleProviderPropertyDefinition("SyncedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition FinalizedCount = new SimpleProviderPropertyDefinition("FinalizedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition FailedCount = new SimpleProviderPropertyDefinition("FailedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition PendingCount = new SimpleProviderPropertyDefinition("PendingCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition ProvisionedCount = new SimpleProviderPropertyDefinition("ProvisionedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition MigrationType = new SimpleProviderPropertyDefinition("MigrationType", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

			public static readonly ProviderPropertyDefinition DiagnosticInfo = new SimpleProviderPropertyDefinition("DiagnosticInfo", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}
	}
}
