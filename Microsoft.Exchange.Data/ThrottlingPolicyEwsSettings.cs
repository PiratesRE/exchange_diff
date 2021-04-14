using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyEwsSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyEwsSettings()
		{
		}

		private ThrottlingPolicyEwsSettings(string value) : base(value)
		{
			Unlimited<uint>? maxSubscriptions = this.MaxSubscriptions;
			Unlimited<uint>? outlookServiceMaxConcurrency = this.OutlookServiceMaxConcurrency;
			Unlimited<uint>? outlookServiceMaxBurst = this.OutlookServiceMaxBurst;
			Unlimited<uint>? outlookServiceRechargeRate = this.OutlookServiceRechargeRate;
			Unlimited<uint>? outlookServiceCutoffBalance = this.OutlookServiceCutoffBalance;
			Unlimited<uint>? outlookServiceMaxSubscriptions = this.OutlookServiceMaxSubscriptions;
			Unlimited<uint>? outlookServiceMaxSocketConnectionsPerDevice = this.OutlookServiceMaxSocketConnectionsPerDevice;
			Unlimited<uint>? outlookServiceMaxSocketConnectionsPerUser = this.OutlookServiceMaxSocketConnectionsPerUser;
		}

		public static ThrottlingPolicyEwsSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyEwsSettings(stateToParse);
		}

		internal Unlimited<uint>? MaxSubscriptions
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxSub");
			}
			set
			{
				base.SetValueInPropertyBag("MaxSub", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceMaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceMaxConcur", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceMaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceMaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceMaxBurst", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceRechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceRechargeRate");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceRechargeRate", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceCutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceCutoff");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceCutoff", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceMaxSubscriptions
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceMaxSub");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceMaxSub", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerDevice
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceMaxSocketConDevice");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceMaxSocketConDevice", value);
			}
		}

		internal Unlimited<uint>? OutlookServiceMaxSocketConnectionsPerUser
		{
			get
			{
				return base.GetValueFromPropertyBag("OutlookServiceMaxSocketConUser");
			}
			set
			{
				base.SetValueInPropertyBag("OutlookServiceMaxSocketConUser", value);
			}
		}

		private const string MaxSubscriptionsPrefix = "MaxSub";

		private const string OutlookServiceMaxConcurrencyPrefix = "OutlookServiceMaxConcur";

		private const string OutlookServiceMaxBurstPrefix = "OutlookServiceMaxBurst";

		private const string OutlookServiceRechargeRatePrefix = "OutlookServiceRechargeRate";

		private const string OutlookServiceCutoffBalancePrefix = "OutlookServiceCutoff";

		private const string OutlookServiceMaxSubscriptionsPrefix = "OutlookServiceMaxSub";

		private const string OutlookServiceMaxSocketConnectionsPerDevicePrefix = "OutlookServiceMaxSocketConDevice";

		private const string OutlookServiceMaxSocketConnectionsPerUserPrefix = "OutlookServiceMaxSocketConUser";
	}
}
