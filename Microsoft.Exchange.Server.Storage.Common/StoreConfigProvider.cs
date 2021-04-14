using System;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal class StoreConfigProvider : ConfigProviderBase
	{
		internal ConfigSchemaBase Schema { get; private set; }

		public static StoreConfigProvider Default
		{
			get
			{
				return StoreConfigProvider.instance;
			}
		}

		public StoreConfigProvider(ConfigSchemaBase schema) : base(schema)
		{
			this.Schema = schema;
			StoreConfigProvider.instance = this;
			this.AddConfigDriver(new AppConfigDriver(schema));
		}

		public new void AddConfigDriver(IConfigDriver configDriver)
		{
			configDriver.Initialize();
			base.AddConfigDriver(configDriver);
		}

		internal new void RemoveConfigDriver(IConfigDriver configDriver)
		{
			base.RemoveConfigDriver(configDriver);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<StoreConfigProvider>(this);
		}

		private static StoreConfigProvider instance;
	}
}
