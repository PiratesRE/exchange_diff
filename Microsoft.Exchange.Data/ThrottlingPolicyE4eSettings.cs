using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyE4eSettings : ThrottlingPolicyBaseSettings
	{
		public ThrottlingPolicyE4eSettings()
		{
		}

		private ThrottlingPolicyE4eSettings(string value) : base(value)
		{
			Unlimited<uint>? encryptionSenderMaxConcurrency = this.EncryptionSenderMaxConcurrency;
			Unlimited<uint>? encryptionSenderMaxBurst = this.EncryptionSenderMaxBurst;
			Unlimited<uint>? encryptionSenderRechargeRate = this.EncryptionSenderRechargeRate;
			Unlimited<uint>? encryptionSenderCutoffBalance = this.EncryptionSenderCutoffBalance;
			Unlimited<uint>? encryptionRecipientMaxConcurrency = this.EncryptionRecipientMaxConcurrency;
			Unlimited<uint>? encryptionRecipientMaxBurst = this.EncryptionRecipientMaxBurst;
			Unlimited<uint>? encryptionRecipientRechargeRate = this.EncryptionRecipientRechargeRate;
			Unlimited<uint>? encryptionRecipientCutoffBalance = this.EncryptionRecipientCutoffBalance;
		}

		public static ThrottlingPolicyE4eSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyE4eSettings(stateToParse);
		}

		internal Unlimited<uint>? EncryptionSenderMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("SenderMaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("SenderMaxConcur", value);
			}
		}

		internal Unlimited<uint>? EncryptionSenderMaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("SenderMaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("SenderMaxBurst", value);
			}
		}

		internal Unlimited<uint>? EncryptionSenderRechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("SenderRechargeRate");
			}
			set
			{
				base.SetValueInPropertyBag("SenderRechargeRate", value);
			}
		}

		internal Unlimited<uint>? EncryptionSenderCutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("SenderCutoff");
			}
			set
			{
				base.SetValueInPropertyBag("SenderCutoff", value);
			}
		}

		internal Unlimited<uint>? EncryptionRecipientMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("RecipientMaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("RecipientMaxConcur", value);
			}
		}

		internal Unlimited<uint>? EncryptionRecipientMaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("RecipientMaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("RecipientMaxBurst", value);
			}
		}

		internal Unlimited<uint>? EncryptionRecipientRechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("RecipientRechargeRate");
			}
			set
			{
				base.SetValueInPropertyBag("RecipientRechargeRate", value);
			}
		}

		internal Unlimited<uint>? EncryptionRecipientCutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("RecipientCutoff");
			}
			set
			{
				base.SetValueInPropertyBag("RecipientCutoff", value);
			}
		}

		internal const string SenderConcurrencyPrefix = "SenderMaxConcur";

		internal const string SenderMaxBurstPrefix = "SenderMaxBurst";

		internal const string SenderRechargeRatePrefix = "SenderRechargeRate";

		internal const string SenderCutoffBalancePrefix = "SenderCutoff";

		internal const string RecipientConcurrencyPrefix = "RecipientMaxConcur";

		internal const string RecipientMaxBurstPrefix = "RecipientMaxBurst";

		internal const string RecipientRechargeRatePrefix = "RecipientRechargeRate";

		internal const string RecipientCutoffBalancePrefix = "RecipientCutoff";
	}
}
