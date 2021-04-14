using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal class StoreConfigContext : SettingsContextBase
	{
		public static void Initialize()
		{
			StoreConfigContext.Default = new StoreConfigContext();
		}

		private StoreConfigContext() : base(null)
		{
			StoreConfigContext.ConfigProvider = new StoreConfigProvider(new StoreConfigSchema());
			StoreConfigContext.ConfigProvider.Initialize();
		}

		public static IConfigProvider ConfigProvider { get; private set; }

		public override Guid? DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public override Guid? DagOrServerGuid
		{
			get
			{
				return this.dagOrServerGuid;
			}
		}

		public void SetDatabaseContext(Guid? databaseGuid, Guid? dagOrServerGuid)
		{
			this.databaseGuid = databaseGuid;
			this.dagOrServerGuid = dagOrServerGuid;
		}

		public T GetConfig<T>(string settingName, T defaultValue)
		{
			return StoreConfigContext.ConfigProvider.GetConfig<T>(this, settingName, defaultValue);
		}

		public static StoreConfigContext Default;

		private Guid? databaseGuid;

		private Guid? dagOrServerGuid;
	}
}
