using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ServerHistoryEntry : ADConfigurationObject
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ServerHistoryEntry.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ServerHistoryEntry.SchemaInstance;
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
				return (DateTime)this[ServerHistoryEntrySchema.Timestamp];
			}
			set
			{
				this[ServerHistoryEntrySchema.Timestamp] = value;
			}
		}

		internal static object TimestampUtcGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ServerHistoryEntrySchema.Timestamp];
			if (!string.IsNullOrEmpty(text))
			{
				return new DateTime?(DateTime.ParseExact(text, "yyyyMMddHHmmss'.0Z'", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal));
			}
			return null;
		}

		public int Version
		{
			get
			{
				return (int)this[ServerHistoryEntrySchema.Version];
			}
			set
			{
				this[ServerHistoryEntrySchema.Version] = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return (byte[])this[ServerHistoryEntrySchema.Data];
			}
			set
			{
				this[ServerHistoryEntrySchema.Data] = value;
			}
		}

		internal static readonly string MostDerivedClass = "msExchMSOForwardSyncCookie";

		private static readonly ServerHistoryEntrySchema SchemaInstance = ObjectSchema.GetInstance<ServerHistoryEntrySchema>();
	}
}
