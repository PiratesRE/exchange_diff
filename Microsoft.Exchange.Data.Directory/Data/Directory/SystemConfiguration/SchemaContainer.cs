using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class SchemaContainer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SchemaContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SchemaContainer.mostDerivedClass;
			}
		}

		public ADObjectId FsmoRoleOwner
		{
			get
			{
				return (ADObjectId)this[SchemaContainerSchema.FsmoRoleOwner];
			}
		}

		private static SchemaContainerSchema schema = ObjectSchema.GetInstance<SchemaContainerSchema>();

		private static string mostDerivedClass = "dMD";
	}
}
