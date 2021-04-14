using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MonitoringOverride : ADConfigurationObject
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static object ApplyVersionGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[MonitoringOverrideSchema.ApplyVersionRaw];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			object result;
			try
			{
				result = ServerVersion.ParseFromSerialNumber(text);
			}
			catch (FormatException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("ApplyVersion", ex.Message), MonitoringOverrideSchema.ApplyVersionRaw, propertyBag[MonitoringOverrideSchema.ApplyVersionRaw]), ex);
			}
			return result;
		}

		internal static void ApplyVersionSetter(object value, IPropertyBag propertyBag)
		{
			ServerVersion serverVersion = (ServerVersion)value;
			propertyBag[MonitoringOverrideSchema.ApplyVersionRaw] = serverVersion.ToString(true);
		}

		internal static object ExpirationTimeGetter(IPropertyBag propertyBag)
		{
			string s = (string)propertyBag[MonitoringOverrideSchema.ExpirationTimeRaw];
			DateTime dateTime;
			if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
			{
				return dateTime.ToUniversalTime();
			}
			return null;
		}

		internal static void ExpirationTimeSetter(object value, IPropertyBag propertyBag)
		{
			DateTime? dateTime = value as DateTime?;
			if (dateTime == null)
			{
				propertyBag[MonitoringOverrideSchema.ExpirationTimeRaw] = string.Empty;
			}
			propertyBag[MonitoringOverrideSchema.ExpirationTimeRaw] = dateTime.Value.ToString("u");
		}

		public string HealthSet
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.HealthSet];
			}
			set
			{
				this[MonitoringOverrideSchema.HealthSet] = value;
			}
		}

		public string MonitoringItemName
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.MonitoringItemName];
			}
			set
			{
				this[MonitoringOverrideSchema.MonitoringItemName] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[ADObjectSchema.RawName];
			}
		}

		public string PropertyValue
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.PropertyValue];
			}
			set
			{
				this[MonitoringOverrideSchema.PropertyValue] = value;
			}
		}

		public ServerVersion ApplyVersion
		{
			get
			{
				return (ServerVersion)this[MonitoringOverrideSchema.ApplyVersion];
			}
			set
			{
				this[MonitoringOverrideSchema.ApplyVersion] = value;
			}
		}

		public DateTime? ExpirationTime
		{
			get
			{
				return (DateTime?)this[MonitoringOverrideSchema.ExpirationTime];
			}
			set
			{
				this[MonitoringOverrideSchema.ExpirationTime] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[MonitoringOverrideSchema.CreatedBy];
			}
			set
			{
				this[MonitoringOverrideSchema.CreatedBy] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MonitoringOverride.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MonitoringOverride.mostDerivedClass;
			}
		}

		internal static readonly ADObjectId RdnContainer = new ADObjectId("CN=Monitoring Settings");

		internal static readonly string ContainerName = "Overrides";

		private static MonitoringOverrideSchema schema = ObjectSchema.GetInstance<MonitoringOverrideSchema>();

		private static string mostDerivedClass = "msExchMonitoringOverride";
	}
}
