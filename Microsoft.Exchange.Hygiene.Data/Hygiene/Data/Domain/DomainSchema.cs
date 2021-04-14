using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class DomainSchema : ADObjectSchema
	{
		public static ObjectId GetObjectId(Guid guid)
		{
			return new ConfigObjectId(guid.ToString());
		}

		public static object GetNullIfGuidEmpty(Guid guid)
		{
			if (guid != System.Guid.Empty)
			{
				return guid;
			}
			return null;
		}

		public static Guid GetGuidEmptyIfNull(object value)
		{
			if (value != null)
			{
				return (Guid)value;
			}
			return System.Guid.Empty;
		}

		public static object GetNullIfStringEmpty(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value;
			}
			return null;
		}

		public static readonly HygienePropertyDefinition Identifier = new HygienePropertyDefinition("Identifier", typeof(Guid));

		public static readonly HygienePropertyDefinition DomainTargetEnvironmentId = new HygienePropertyDefinition("DomainTargetEnvironmentId", typeof(Guid));

		public static readonly HygienePropertyDefinition TargetServiceId = new HygienePropertyDefinition("TargetServiceId", typeof(Guid));

		public static readonly HygienePropertyDefinition PropertiesAsId = new HygienePropertyDefinition("PropertiesAsIdTable", typeof(IDictionary<int, IDictionary<int, string>>));

		public static readonly HygienePropertyDefinition DomainKey = new HygienePropertyDefinition("DomainKey", typeof(string));

		public static readonly HygienePropertyDefinition UpdateDomainKey = new HygienePropertyDefinition("UpdateDomainKey", typeof(bool), false, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DomainKeys = new HygienePropertyDefinition("DomainKeys", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition TenantTargetEnvironmentId = new HygienePropertyDefinition("TenantTargetEnvironmentId", typeof(Guid));

		public static readonly HygienePropertyDefinition TenantId = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		public static readonly HygienePropertyDefinition TenantIds = new HygienePropertyDefinition("OrganizationalUnitRoots", typeof(Guid), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition ZoneId = new HygienePropertyDefinition("ZoneId", typeof(Guid));

		public static readonly HygienePropertyDefinition ResourceRecordId = new HygienePropertyDefinition("ResourceRecordId", typeof(Guid));

		public static readonly HygienePropertyDefinition ResourceRecordTypeId = new HygienePropertyDefinition("ResourceRecordTypeId", typeof(Guid));

		public static readonly HygienePropertyDefinition DomainName = new HygienePropertyDefinition("DomainName", typeof(string));

		public static readonly HygienePropertyDefinition DomainNames = new HygienePropertyDefinition("DomainNames", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition NameServer = new HygienePropertyDefinition("NameServer", typeof(string));

		public static readonly HygienePropertyDefinition IpAddress = new HygienePropertyDefinition("IpAddress", typeof(string));

		public static readonly HygienePropertyDefinition PrimaryNameServer = new HygienePropertyDefinition("PrimaryNameServer", typeof(string));

		public static readonly HygienePropertyDefinition ResponsibleMailServer = new HygienePropertyDefinition("ResponsibleMailServer", typeof(string));

		public static readonly HygienePropertyDefinition Refresh = new HygienePropertyDefinition("Refresh", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition Retry = new HygienePropertyDefinition("Retry", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition Expire = new HygienePropertyDefinition("Expire", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition Serial = new HygienePropertyDefinition("Serial", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DefaultTtl = new HygienePropertyDefinition("DefaultTtl", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition UpdatedDomains = new HygienePropertyDefinition("UpdatedDomains", typeof(IEnumerable<string>));

		public static readonly HygienePropertyDefinition DomainKeyFlags = new HygienePropertyDefinition("DomainKeyFlags", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition UserTargetEnvironmentId = new HygienePropertyDefinition("UserTargetEnvironmentId", typeof(Guid));

		public static readonly HygienePropertyDefinition UserKey = new HygienePropertyDefinition("UserKey", typeof(string));

		public static readonly HygienePropertyDefinition MSAUserName = new HygienePropertyDefinition("MSAUserName", typeof(string));

		public static readonly HygienePropertyDefinition ObjectStateProp = DalHelper.ObjectStateProp;

		public static readonly HygienePropertyDefinition IsTracerTokenProp = DalHelper.IsTracerTokenProp;

		public static readonly HygienePropertyDefinition WhenChangedProp = DalHelper.WhenChangedProp;

		public static readonly HygienePropertyDefinition CreatedDatetime = new HygienePropertyDefinition("CreatedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChangedDatetime = new HygienePropertyDefinition("ChangedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DeletedDatetime = new HygienePropertyDefinition("DeletedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyChangedDatetime = new HygienePropertyDefinition("PropertyChangedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyCreatedDatetime = new HygienePropertyDefinition("PropertyCreatedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyDeletedDatetime = new HygienePropertyDefinition("PropertyDeletedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyId = new HygienePropertyDefinition("PropertyId", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition EntityId = new HygienePropertyDefinition("EntityId", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyValue = new HygienePropertyDefinition("PropertyValue", typeof(string));
	}
}
