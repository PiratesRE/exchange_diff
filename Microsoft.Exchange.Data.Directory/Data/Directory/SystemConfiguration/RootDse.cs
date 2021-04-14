using System;
using System.Globalization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class RootDse : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RootDse.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RootDse.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, "*");
			}
		}

		public ADObjectId ConfigurationNamingContext
		{
			get
			{
				return (ADObjectId)this[RootDseSchema.ConfigurationNamingContext];
			}
			set
			{
				this[RootDseSchema.ConfigurationNamingContext] = value;
			}
		}

		public ADObjectId DefaultNamingContext
		{
			get
			{
				return (ADObjectId)this[RootDseSchema.DefaultNamingContext];
			}
			set
			{
				this[RootDseSchema.DefaultNamingContext] = value;
			}
		}

		public string Fqdn
		{
			get
			{
				return (string)this[RootDseSchema.Fqdn];
			}
			set
			{
				this[RootDseSchema.Fqdn] = value;
			}
		}

		public long HighestCommittedUSN
		{
			get
			{
				return (long)this[RootDseSchema.HighestCommittedUSN];
			}
			set
			{
				this[RootDseSchema.HighestCommittedUSN] = value;
			}
		}

		public ExDateTime CurrentTime
		{
			get
			{
				return (ExDateTime)this[RootDseSchema.CurrentTime];
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return (bool)this[RootDseSchema.IsSynchronized];
			}
			set
			{
				this[RootDseSchema.IsSynchronized] = value;
			}
		}

		public ADObjectId NtDsDsa
		{
			get
			{
				return (ADObjectId)this[RootDseSchema.NtDsDsa];
			}
		}

		internal static object SiteGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[RootDseSchema.NtDsDsa];
			return adobjectId.AncestorDN(3);
		}

		internal static object CurrentTimeGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[RootDseSchema.CurrentTimeRaw];
			ExDateTime exDateTime;
			if (!string.IsNullOrEmpty(text) && ExDateTime.TryParseExact(text, "yyyyMMddHHmmss.fZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out exDateTime))
			{
				return exDateTime;
			}
			return ExDateTime.MinValue;
		}

		public ADObjectId Site
		{
			get
			{
				return (ADObjectId)this[RootDseSchema.Site];
			}
		}

		private static RootDseSchema schema = ObjectSchema.GetInstance<RootDseSchema>();

		private static string mostDerivedClass = "*";
	}
}
