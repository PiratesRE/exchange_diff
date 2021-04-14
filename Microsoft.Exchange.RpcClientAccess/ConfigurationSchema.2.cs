using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ConfigurationSchema<TDerivedSchema> : ConfigurationSchema where TDerivedSchema : new()
	{
		public static TDerivedSchema Instance
		{
			get
			{
				if (ConfigurationSchema<TDerivedSchema>.instance == null)
				{
					ConfigurationSchema<TDerivedSchema>.instance = ((default(TDerivedSchema) == null) ? Activator.CreateInstance<TDerivedSchema>() : default(TDerivedSchema));
				}
				return ConfigurationSchema<TDerivedSchema>.instance;
			}
		}

		public override IEnumerable<ConfigurationSchema.DataSource> DataSources
		{
			get
			{
				return ConfigurationSchema<TDerivedSchema>.AllDataSources;
			}
		}

		public override void LoadAll(ConfigurationSchema.ConfigurationUpdater configurationUpdater, ConfigurationSchema.EventLogger eventLogger)
		{
			foreach (ConfigurationSchema.DataSource dataSource in ConfigurationSchema<TDerivedSchema>.AllDataSources)
			{
				dataSource.Load(configurationUpdater, eventLogger);
			}
		}

		protected static readonly List<ConfigurationSchema.DataSource> AllDataSources = new List<ConfigurationSchema.DataSource>();

		protected new static readonly ConfigurationSchema.ConstantDataSource ConstantDataSource = new ConfigurationSchema.ConstantDataSource(ConfigurationSchema<TDerivedSchema>.AllDataSources);

		private static TDerivedSchema instance;
	}
}
