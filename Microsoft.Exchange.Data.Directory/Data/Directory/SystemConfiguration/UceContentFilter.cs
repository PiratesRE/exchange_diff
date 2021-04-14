using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class UceContentFilter : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return UceContentFilter.schema;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return UceContentFilter.parentPath;
			}
		}

		public int SCLJunkThreshold
		{
			get
			{
				return (int)this.propertyBag[UceContentFilterSchema.SCLJunkThreshold];
			}
			internal set
			{
				this.propertyBag[UceContentFilterSchema.SCLJunkThreshold] = value;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchUce";
			}
		}

		private const string MostDerivedClass = "msExchUce";

		private static readonly ADObjectId parentPath = new ADObjectId("CN=Message Delivery,CN=Global Settings");

		private static readonly UceContentFilterSchema schema = ObjectSchema.GetInstance<UceContentFilterSchema>();
	}
}
