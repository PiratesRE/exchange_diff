using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyRcaSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyRcaSettings()
		{
		}

		private ThrottlingPolicyRcaSettings(string value) : base(value)
		{
			Unlimited<uint>? cpaMaxConcurrency = this.CpaMaxConcurrency;
			Unlimited<uint>? cpaMaxBurst = this.CpaMaxBurst;
			Unlimited<uint>? cpaRechargeRate = this.CpaRechargeRate;
			Unlimited<uint>? cpaCutoffBalance = this.CpaCutoffBalance;
		}

		public static ThrottlingPolicyRcaSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyRcaSettings(stateToParse);
		}

		internal Unlimited<uint>? CpaMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("CpaMaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("CpaMaxConcur", value);
			}
		}

		internal Unlimited<uint>? CpaMaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("CpaMaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("CpaMaxBurst", value);
			}
		}

		internal Unlimited<uint>? CpaRechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("CpaRecharge");
			}
			set
			{
				base.SetValueInPropertyBag("CpaRecharge", value);
			}
		}

		internal Unlimited<uint>? CpaCutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("CpaCutoff");
			}
			set
			{
				base.SetValueInPropertyBag("CpaCutoff", value);
			}
		}

		internal const string CpaMaxConcurrencyPrefix = "CpaMaxConcur";

		internal const string CpaMaxBurstPrefix = "CpaMaxBurst";

		internal const string CpaRechargeRatePrefix = "CpaRecharge";

		internal const string CpaCutoffBalancePrefix = "CpaCutoff";
	}
}
