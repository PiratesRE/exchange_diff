using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyPushNotificationSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyPushNotificationSettings()
		{
		}

		private ThrottlingPolicyPushNotificationSettings(string value) : base(value)
		{
			Unlimited<uint>? pushNotificationMaxBurstPerDevice = this.PushNotificationMaxBurstPerDevice;
			Unlimited<uint>? pushNotificationRechargeRatePerDevice = this.PushNotificationRechargeRatePerDevice;
			Unlimited<uint>? pushNotificationSamplingPeriodPerDevice = this.PushNotificationSamplingPeriodPerDevice;
		}

		public static ThrottlingPolicyPushNotificationSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyPushNotificationSettings(stateToParse);
		}

		internal Unlimited<uint>? PushNotificationMaxBurstPerDevice
		{
			get
			{
				return base.GetValueFromPropertyBag("PushNotificationMaximumLimitPerDevice");
			}
			set
			{
				base.SetValueInPropertyBag("PushNotificationMaximumLimitPerDevice", value);
			}
		}

		internal Unlimited<uint>? PushNotificationRechargeRatePerDevice
		{
			get
			{
				return base.GetValueFromPropertyBag("PushNotificationRechargeRatePerDevice");
			}
			set
			{
				base.SetValueInPropertyBag("PushNotificationRechargeRatePerDevice", value);
			}
		}

		internal Unlimited<uint>? PushNotificationSamplingPeriodPerDevice
		{
			get
			{
				return base.GetValueFromPropertyBag("PushNotificationSamplingPeriodPerDevice");
			}
			set
			{
				base.SetValueInPropertyBag("PushNotificationSamplingPeriodPerDevice", value);
			}
		}

		internal const string PushNotificationMaxBurstPerDevicePrefix = "PushNotificationMaximumLimitPerDevice";

		internal const string PushNotificationRechargeRatePerDevicePrefix = "PushNotificationRechargeRatePerDevice";

		internal const string PushNotificationSamplingPeriodPerDevicePrefix = "PushNotificationSamplingPeriodPerDevice";
	}
}
