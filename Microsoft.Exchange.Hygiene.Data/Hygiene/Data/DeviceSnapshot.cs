using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.MessageTrace;
using Microsoft.Exchange.Hygiene.Data.MessageTrace.Reports;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DeviceSnapshot : Schema
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[DeviceSnapshot.OrganizationalUnitRootProperty];
			}
			set
			{
				this[DeviceSnapshot.OrganizationalUnitRootProperty] = value;
			}
		}

		public string Platform
		{
			get
			{
				return (string)this[DeviceSnapshot.PlatformProperty];
			}
			set
			{
				this[DeviceSnapshot.PlatformProperty] = value;
			}
		}

		public int TotalDevicesCount
		{
			get
			{
				return (int)this[DeviceSnapshot.TotalDevicesCountProperty];
			}
			set
			{
				this[DeviceSnapshot.TotalDevicesCountProperty] = value;
			}
		}

		public int AllowedDevicesCount
		{
			get
			{
				return (int)this[DeviceSnapshot.AllowedDevicesCountProperty];
			}
			set
			{
				this[DeviceSnapshot.AllowedDevicesCountProperty] = value;
			}
		}

		public int BlockedDevicesCount
		{
			get
			{
				return (int)this[DeviceSnapshot.BlockedDevicesCountProperty];
			}
			set
			{
				this[DeviceSnapshot.BlockedDevicesCountProperty] = value;
			}
		}

		public int QuarantinedDevicesCount
		{
			get
			{
				return (int)this[DeviceSnapshot.QuarantinedDevicesCountProperty];
			}
			set
			{
				this[DeviceSnapshot.QuarantinedDevicesCountProperty] = value;
			}
		}

		public int UnknownDevicesCount
		{
			get
			{
				return (int)this[DeviceSnapshot.UnknownDevicesCountProperty];
			}
			set
			{
				this[DeviceSnapshot.UnknownDevicesCountProperty] = value;
			}
		}

		public DateTime LastUpdatedTime
		{
			get
			{
				return (DateTime)this[DeviceSnapshot.LastUpdatedTimeProperty];
			}
			set
			{
				this[DeviceSnapshot.LastUpdatedTimeProperty] = value;
			}
		}

		internal static readonly HygienePropertyDefinition OrganizationalUnitRootProperty = CommonReportingSchema.OrganizationalUnitRootProperty;

		internal static readonly HygienePropertyDefinition PlatformProperty = new HygienePropertyDefinition("Platform", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TotalDevicesCountProperty = new HygienePropertyDefinition("TotalDevicesCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition AllowedDevicesCountProperty = new HygienePropertyDefinition("AllowedDevicesCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition BlockedDevicesCountProperty = new HygienePropertyDefinition("BlockedDevicesCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition QuarantinedDevicesCountProperty = new HygienePropertyDefinition("QuarantinedDevicesCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition UnknownDevicesCountProperty = new HygienePropertyDefinition("UnknownDevicesCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LastUpdatedTimeProperty = new HygienePropertyDefinition("LastUpdatedTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
