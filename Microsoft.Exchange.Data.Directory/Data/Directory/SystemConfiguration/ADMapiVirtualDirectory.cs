using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADMapiVirtualDirectory : ExchangeVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADMapiVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADMapiVirtualDirectory.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADMapiVirtualDirectorySchema.IISAuthenticationMethods];
			}
			set
			{
				this[ADMapiVirtualDirectorySchema.IISAuthenticationMethods] = value;
			}
		}

		internal static object GetIISAuthenticationMethods(IPropertyBag propertyBag)
		{
			return ADVirtualDirectory.InternalAuthenticationMethodsGetter(propertyBag);
		}

		internal static void SetIISAuthenticationMethods(object value, IPropertyBag propertyBag)
		{
			ADVirtualDirectory.InternalAuthenticationMethodsSetter(value, propertyBag);
			ADVirtualDirectory.ExternalAuthenticationMethodsSetter(value, propertyBag);
		}

		private static readonly ADMapiVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADMapiVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchMapiVirtualDirectory";
	}
}
