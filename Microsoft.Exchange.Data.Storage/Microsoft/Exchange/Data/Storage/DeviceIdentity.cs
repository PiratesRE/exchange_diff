using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeviceIdentity : IComparable<DeviceIdentity>, IEquatable<DeviceIdentity>, IEquatable<string>
	{
		public DeviceIdentity(string deviceId, string deviceType, string protocol)
		{
			this.Initialize(deviceId, deviceType, protocol);
		}

		public DeviceIdentity(string deviceId, string deviceType, MobileClientType mobileClientType)
		{
			string protocol;
			if (!DeviceIdentity.TryGetProtocol(mobileClientType, out protocol))
			{
				throw new ArgumentException("Unsupported MobileClientType value: " + mobileClientType);
			}
			this.Initialize(deviceId, deviceType, protocol);
		}

		public DeviceIdentity(string compositeIdentity)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("CompositeIdentity", compositeIdentity);
			this.CompositeKey = compositeIdentity;
			string protocol;
			string deviceType;
			string deviceId;
			DeviceIdentity.ParseSyncFolderName(compositeIdentity, out protocol, out deviceType, out deviceId);
			this.Protocol = protocol;
			this.DeviceType = deviceType;
			this.DeviceId = deviceId;
			DeviceIdentity.VerifyPart(this.DeviceId, "DeviceId");
			DeviceIdentity.VerifyPart(this.DeviceType, "DeviceType");
			ArgumentValidator.ThrowIfNullOrEmpty("Protocol", this.Protocol);
			this.IsDnMangled = this.DeviceId.Contains("\n");
		}

		public static DeviceIdentity FromMobileDevice(MobileDevice mobileDevice)
		{
			return new DeviceIdentity(mobileDevice.DeviceId, mobileDevice.DeviceType, (mobileDevice.ClientType == MobileClientType.EAS) ? "AirSync" : "MOWA");
		}

		public bool IsDnMangled { get; private set; }

		public string DeviceId { get; private set; }

		public string DeviceType { get; private set; }

		public string Protocol { get; private set; }

		public string CompositeKey { get; private set; }

		public bool TryGetMobileClientType(out MobileClientType mobileClientType)
		{
			return DeviceIdentity.TryGetMobileClientType(this.Protocol, out mobileClientType);
		}

		public static bool TryGetMobileClientType(string protocol, out MobileClientType mobileClientType)
		{
			mobileClientType = MobileClientType.EAS;
			if (string.Equals(protocol, "AirSync", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.Equals(protocol, "MOWA", StringComparison.OrdinalIgnoreCase))
			{
				mobileClientType = MobileClientType.MOWA;
				return true;
			}
			return false;
		}

		public static bool TryGetProtocol(MobileClientType mobileClientType, out string protocol)
		{
			switch (mobileClientType)
			{
			case MobileClientType.EAS:
				protocol = "AirSync";
				return true;
			case MobileClientType.MOWA:
				protocol = "MOWA";
				return true;
			default:
				protocol = null;
				return false;
			}
		}

		public bool IsDeviceId(string deviceId)
		{
			return this.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsDeviceType(string deviceType)
		{
			return this.DeviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsProtocol(string protocol)
		{
			return string.Equals(this.Protocol, protocol, StringComparison.OrdinalIgnoreCase);
		}

		public static string BuildCompositeKey(string protocol, string deviceType, string deviceId)
		{
			return string.Format("{0}-{1}-{2}", protocol, deviceType, deviceId);
		}

		public static void ParseSyncFolderName(string combinedName, out string protocol, out string deviceType, out string deviceId)
		{
			protocol = null;
			deviceType = null;
			deviceId = null;
			int num = combinedName.LastIndexOf('-');
			if (num < 0 || num >= combinedName.Length - 1)
			{
				throw new InvalidOperationException(string.Format("[DeviceId] SyncStateStorage has an invalid name: '{0}'", combinedName));
			}
			deviceId = combinedName.Substring(num + 1);
			combinedName = combinedName.Substring(0, num);
			num = combinedName.LastIndexOf('-');
			if (num < 0 || num >= combinedName.Length - 1)
			{
				throw new InvalidOperationException(string.Format("[DeviceType] SyncStateStorage has an invalid name: '{0}'", combinedName));
			}
			deviceType = combinedName.Substring(num + 1);
			protocol = combinedName.Substring(0, num);
		}

		public override string ToString()
		{
			return this.CompositeKey;
		}

		public override int GetHashCode()
		{
			return this.CompositeKey.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DeviceIdentity))
			{
				return false;
			}
			DeviceIdentity deviceIdentity = (DeviceIdentity)obj;
			return deviceIdentity.CompositeKey.Equals(this.CompositeKey, StringComparison.OrdinalIgnoreCase);
		}

		public bool Equals(string deviceId, string deviceType)
		{
			return this.IsDeviceId(deviceId) && this.IsDeviceType(deviceType);
		}

		public bool Equals(string deviceId, string deviceType, string protocol)
		{
			return this.IsDeviceId(deviceId) && this.IsDeviceType(deviceType) && this.IsProtocol(protocol);
		}

		private static void VerifyPart(string toCheck, string part)
		{
			if (string.IsNullOrEmpty(toCheck))
			{
				throw new ArgumentException(string.Format("{0} cannot be null or empty.", part));
			}
			if (toCheck.Contains("-"))
			{
				throw new InvalidOperationException(string.Format("{0} cannot contain hyphens.  Supplied value - '{1}'", part, toCheck));
			}
		}

		private void Initialize(string deviceId, string deviceType, string protocol)
		{
			DeviceIdentity.VerifyPart(deviceId, "DeviceId");
			DeviceIdentity.VerifyPart(deviceType, "DeviceType");
			ArgumentValidator.ThrowIfNullOrEmpty("Protocol", protocol);
			this.DeviceId = deviceId;
			this.DeviceType = deviceType;
			this.Protocol = protocol;
			this.IsDnMangled = (deviceId != null && deviceId.Contains("\n"));
			this.CompositeKey = DeviceIdentity.BuildCompositeKey(protocol, deviceType, deviceId);
		}

		int IComparable<DeviceIdentity>.CompareTo(DeviceIdentity other)
		{
			if (other == null)
			{
				return -1;
			}
			return this.CompositeKey.CompareTo(other.CompositeKey);
		}

		bool IEquatable<DeviceIdentity>.Equals(DeviceIdentity other)
		{
			return other != null && other.CompositeKey.Equals(this.CompositeKey);
		}

		bool IEquatable<string>.Equals(string other)
		{
			return this.CompositeKey.Equals(other);
		}

		private const string deviceIdPart = "DeviceId";

		private const string deviceTypePart = "DeviceType";

		private const string protocolPart = "Protocol";

		private const string compositeIdentityPart = "CompositeIdentity";

		public const string AirSyncProtocol = "AirSync";

		public const string MOWAProtocol = "MOWA";
	}
}
