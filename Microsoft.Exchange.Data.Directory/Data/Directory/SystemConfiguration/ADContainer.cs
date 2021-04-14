using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ADContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADContainer.MostDerivedClass;
			}
		}

		public MultiValuedProperty<DNWithBinary> OtherWellKnownObjects
		{
			get
			{
				return (MultiValuedProperty<DNWithBinary>)this.propertyBag[ADContainerSchema.OtherWellKnownObjects];
			}
			internal set
			{
				this.propertyBag[ADContainerSchema.OtherWellKnownObjects] = value;
			}
		}

		private static ADContainerSchema schema = ObjectSchema.GetInstance<ADContainerSchema>();

		internal static string MostDerivedClass = "container";
	}
}
