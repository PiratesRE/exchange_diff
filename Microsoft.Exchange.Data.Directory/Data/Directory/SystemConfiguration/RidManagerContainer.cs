using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class RidManagerContainer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RidManagerContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RidManagerContainer.mostDerivedClass;
			}
		}

		public ADObjectId FsmoRoleOwner
		{
			get
			{
				return (ADObjectId)this[RidManagerContainerSchema.FsmoRoleOwner];
			}
		}

		public MultiValuedProperty<string> ReplicationAttributeMetadata
		{
			get
			{
				return (MultiValuedProperty<string>)this[RidManagerContainerSchema.ReplicationAttributeMetadata];
			}
		}

		private static RidManagerContainerSchema schema = ObjectSchema.GetInstance<RidManagerContainerSchema>();

		private static string mostDerivedClass = "rIDManager";
	}
}
