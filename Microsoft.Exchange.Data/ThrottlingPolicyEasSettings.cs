using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyEasSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyEasSettings()
		{
		}

		private ThrottlingPolicyEasSettings(string value) : base(value)
		{
			Unlimited<uint>? maxDevices = this.MaxDevices;
			Unlimited<uint>? maxDeviceDeletesPerMonth = this.MaxDeviceDeletesPerMonth;
			Unlimited<uint>? maxInactivityForDeviceCleanup = this.MaxInactivityForDeviceCleanup;
		}

		public static ThrottlingPolicyEasSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyEasSettings(stateToParse);
		}

		internal Unlimited<uint>? MaxDevices
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxDevices");
			}
			set
			{
				base.SetValueInPropertyBag("MaxDevices", value);
			}
		}

		internal Unlimited<uint>? MaxDeviceDeletesPerMonth
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxDeviceDeletesPerMonth");
			}
			set
			{
				base.SetValueInPropertyBag("MaxDeviceDeletesPerMonth", value);
			}
		}

		internal Unlimited<uint>? MaxInactivityForDeviceCleanup
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxInactivityForDeviceCleanup");
			}
			set
			{
				base.SetValueInPropertyBag("MaxInactivityForDeviceCleanup", value);
			}
		}

		private const string MaxDevicesPrefix = "MaxDevices";

		private const string MaxDeviceDeletesPerMonthPrefix = "MaxDeviceDeletesPerMonth";

		private const string MaxInactivityForDeviceCleanupPrefix = "MaxInactivityForDeviceCleanup";
	}
}
