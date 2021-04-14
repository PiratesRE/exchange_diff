using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADEmailTransport : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADEmailTransport.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADEmailTransport.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, Pop3AdConfiguration.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, Imap4AdConfiguration.MostDerivedClass)
				});
			}
		}

		internal static object ServerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
				if (adobjectId == null && (ObjectState)propertyBag[ADObjectSchema.ObjectState] != ObjectState.New)
				{
					throw new InvalidOperationException(DirectoryStrings.IdIsNotSet);
				}
				result = ((adobjectId == null) ? null : adobjectId.AncestorDN(3));
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), ADEmailTransportSchema.Server, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[ADEmailTransportSchema.Server];
			}
		}

		private static readonly string mostDerivedClass = "protocolCfg";

		private static readonly ADObjectSchema schema = ObjectSchema.GetInstance<ADEmailTransportProperties>();
	}
}
