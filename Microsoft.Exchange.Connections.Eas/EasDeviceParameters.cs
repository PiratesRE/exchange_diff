using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class EasDeviceParameters
	{
		internal EasDeviceParameters(string deviceId, string deviceType, string userAgent = "MRS-EASConnection-UserAgent", string deviceIdPrefix = "")
		{
			this.deviceIdPrefix = deviceIdPrefix;
			this.deviceType = deviceType;
			this.userAgent = userAgent;
			this.DeviceId = this.deviceIdPrefix + deviceId;
			if (this.DeviceId.Length > 32)
			{
				throw new ArgumentOutOfRangeException("DeviceId");
			}
		}

		internal EasDeviceParameters(string deviceId, EasDeviceParameters other) : this(deviceId, other.deviceType, other.userAgent, other.deviceIdPrefix)
		{
		}

		internal string DeviceId { get; private set; }

		internal string DeviceType
		{
			get
			{
				return this.deviceType;
			}
		}

		internal string UserAgent
		{
			get
			{
				return this.userAgent;
			}
		}

		internal const int MaxDeviceIdLength = 32;

		private readonly string deviceIdPrefix;

		private readonly string deviceType;

		private readonly string userAgent;
	}
}
