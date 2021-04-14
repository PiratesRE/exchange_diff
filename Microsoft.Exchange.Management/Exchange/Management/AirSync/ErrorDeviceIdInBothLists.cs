using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorDeviceIdInBothLists : LocalizedException
	{
		public ErrorDeviceIdInBothLists(string deviceId) : base(Strings.ErrorDeviceIdInBothLists(deviceId))
		{
			this.deviceId = deviceId;
		}

		public ErrorDeviceIdInBothLists(string deviceId, Exception innerException) : base(Strings.ErrorDeviceIdInBothLists(deviceId), innerException)
		{
			this.deviceId = deviceId;
		}

		protected ErrorDeviceIdInBothLists(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.deviceId = (string)info.GetValue("deviceId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("deviceId", this.deviceId);
		}

		public string DeviceId
		{
			get
			{
				return this.deviceId;
			}
		}

		private readonly string deviceId;
	}
}
