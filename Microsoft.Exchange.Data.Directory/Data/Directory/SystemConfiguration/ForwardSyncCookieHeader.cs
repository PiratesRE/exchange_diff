using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ForwardSyncCookieHeader : ADConfigurationObject
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ForwardSyncCookieHeader.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ForwardSyncCookieHeader.SchemaInstance;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return (DateTime)this[ForwardSyncCookieHeaderSchema.Timestamp];
			}
			set
			{
				this[ForwardSyncCookieHeaderSchema.Timestamp] = value;
			}
		}

		public ForwardSyncCookieType Type
		{
			get
			{
				return (ForwardSyncCookieType)this[ForwardSyncCookieHeaderSchema.Type];
			}
			set
			{
				this[ForwardSyncCookieHeaderSchema.Type] = (int)value;
			}
		}

		internal static object TimestampUtcGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ForwardSyncCookieHeaderSchema.Timestamp];
			if (!string.IsNullOrEmpty(text))
			{
				return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
			}
			return null;
		}

		internal static readonly string MostDerivedClass = "msExchMSOForwardSyncCookie";

		private static readonly ForwardSyncCookieHeaderSchema SchemaInstance = ObjectSchema.GetInstance<ForwardSyncCookieHeaderSchema>();
	}
}
