using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class SyncedPerimeterConfig : PerimeterConfig
	{
		public MultiValuedProperty<string> SyncErrors
		{
			get
			{
				return (MultiValuedProperty<string>)this[SyncedPerimeterConfigSchema.SyncErrors];
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncedPerimeterConfig.SchemaObject;
			}
		}

		private static readonly SyncedPerimeterConfigSchema SchemaObject = ObjectSchema.GetInstance<SyncedPerimeterConfigSchema>();
	}
}
