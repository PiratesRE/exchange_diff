using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DeviceCommonSchema
	{
		public static HygienePropertyDefinition OrganizationalUnitRootProperty = new HygienePropertyDefinition("id_OrganizationalUnitRoot", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		public static HygienePropertyDefinition DeviceIdProperty = new HygienePropertyDefinition("id_DeviceId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		public static HygienePropertyDefinition EASIdProperty = new HygienePropertyDefinition("nvc_EASId", typeof(string), null, ADPropertyDefinitionFlags.Mandatory);

		public static HygienePropertyDefinition IntuneIdProperty = new HygienePropertyDefinition("id_IntuneId", typeof(Guid?), null, ADPropertyDefinitionFlags.None);

		public static HygienePropertyDefinition UserProperty = new HygienePropertyDefinition("nvc_User", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DeviceNameProperty = new HygienePropertyDefinition("nvc_DeviceName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DeviceModelProperty = new HygienePropertyDefinition("nvc_DeviceModel", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DeviceTypeProperty = new HygienePropertyDefinition("nvc_DeviceType", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition FirstSyncTimeProperty = new HygienePropertyDefinition("dt_FirstSyncTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition LastSyncTimeProperty = new HygienePropertyDefinition("dt_LastSyncTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition IMEIProperty = new HygienePropertyDefinition("nvc_IMEI", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition PhoneNumberProperty = new HygienePropertyDefinition("nvc_PhoneNumber", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition MobileNetworkProperty = new HygienePropertyDefinition("nvc_MobileNetwork", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition EASVersionProperty = new HygienePropertyDefinition("nvc_EASVersion", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition UserAgentProperty = new HygienePropertyDefinition("nvc_UserAgent", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DeviceLanguageProperty = new HygienePropertyDefinition("nvc_DeviceLanguage", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DeletedTimeProperty = new HygienePropertyDefinition("dt_DeletedTime", typeof(DateTime?), null, ADPropertyDefinitionFlags.None);

		public static HygienePropertyDefinition ActivityIdProperty = new HygienePropertyDefinition("id_ActivityId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.Mandatory);

		public static HygienePropertyDefinition TimeStampProperty = new HygienePropertyDefinition("dt_TimeStamp", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition DateKeyProperty = new HygienePropertyDefinition("i_DateKey", typeof(int), 19700101, ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition PlatformProperty = new HygienePropertyDefinition("nvc_Platform", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition AccessStateProperty = new HygienePropertyDefinition("i_AccessState", typeof(int?), null, ADPropertyDefinitionFlags.None);

		public static HygienePropertyDefinition AccessStateReasonProperty = new HygienePropertyDefinition("i_AccessStateReason", typeof(int?), null, ADPropertyDefinitionFlags.None);

		public static HygienePropertyDefinition AccessSetByProperty = new HygienePropertyDefinition("nvc_AccessSetBy", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition PolicyAppliedProperty = new HygienePropertyDefinition("nvc_PolicyApplied", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition HashBucketProperty = new HygienePropertyDefinition("si_HashBucket", typeof(short), 0, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
