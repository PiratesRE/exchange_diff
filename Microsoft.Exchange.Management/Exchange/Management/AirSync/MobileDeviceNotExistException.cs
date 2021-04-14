using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MobileDeviceNotExistException : LocalizedException
	{
		public MobileDeviceNotExistException(string deviceId) : base(Strings.MobileDeviceNotExistException(deviceId))
		{
			this.deviceId = deviceId;
		}

		public MobileDeviceNotExistException(string deviceId, Exception innerException) : base(Strings.MobileDeviceNotExistException(deviceId), innerException)
		{
			this.deviceId = deviceId;
		}

		protected MobileDeviceNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
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
