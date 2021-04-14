using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceBudgetKey : SidBudgetKey
	{
		public EasDeviceBudgetKey(SecurityIdentifier sid, string deviceId, string deviceType, ADSessionSettings settings) : base(sid, BudgetType.Eas, false, settings)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("deviceId", deviceId);
			ArgumentValidator.ThrowIfNullOrEmpty("deviceType", deviceType);
			this.DeviceId = deviceId;
			this.DeviceType = deviceType;
			this.cachedHashCode = (sid.GetHashCode() ^ deviceId.GetHashCode() ^ deviceType.GetHashCode());
			this.cachedToString = string.Format("{0}_{1}_{2}", base.NtAccount, this.DeviceId, this.DeviceType);
		}

		public string DeviceId { get; private set; }

		public string DeviceType { get; private set; }

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		public override bool Equals(object obj)
		{
			EasDeviceBudgetKey easDeviceBudgetKey = obj as EasDeviceBudgetKey;
			return !(easDeviceBudgetKey == null) && (object.ReferenceEquals(obj, this) || (easDeviceBudgetKey.Sid.Equals(base.Sid) && easDeviceBudgetKey.DeviceId == this.DeviceId && easDeviceBudgetKey.DeviceType == this.DeviceType));
		}

		public override string ToString()
		{
			return this.cachedToString;
		}

		private readonly int cachedHashCode;

		private readonly string cachedToString;
	}
}
