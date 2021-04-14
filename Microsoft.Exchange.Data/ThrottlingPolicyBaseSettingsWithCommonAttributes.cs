using System;

namespace Microsoft.Exchange.Data
{
	internal abstract class ThrottlingPolicyBaseSettingsWithCommonAttributes : ThrottlingPolicyBaseSettings
	{
		protected ThrottlingPolicyBaseSettingsWithCommonAttributes()
		{
		}

		protected ThrottlingPolicyBaseSettingsWithCommonAttributes(string value) : base(value)
		{
			Unlimited<uint>? maxConcurrency = this.MaxConcurrency;
			Unlimited<uint>? maxBurst = this.MaxBurst;
			Unlimited<uint>? rechargeRate = this.RechargeRate;
			Unlimited<uint>? cutoffBalance = this.CutoffBalance;
		}

		internal Unlimited<uint>? MaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("MaxConcur", value);
			}
		}

		internal Unlimited<uint>? MaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("MaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("MaxBurst", value);
			}
		}

		internal Unlimited<uint>? RechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("RechargeRate");
			}
			set
			{
				base.SetValueInPropertyBag("RechargeRate", value);
			}
		}

		internal Unlimited<uint>? CutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("Cutoff");
			}
			set
			{
				base.SetValueInPropertyBag("Cutoff", value);
			}
		}

		internal const string ConcurrencyPrefix = "MaxConcur";

		internal const string MaxBurstPrefix = "MaxBurst";

		internal const string RechargeRatePrefix = "RechargeRate";

		internal const string CutoffBalancePrefix = "Cutoff";
	}
}
