using System;

namespace Microsoft.Exchange.Data
{
	internal class ThrottlingPolicyOwaSettings : ThrottlingPolicyBaseSettingsWithCommonAttributes
	{
		public ThrottlingPolicyOwaSettings()
		{
		}

		private ThrottlingPolicyOwaSettings(string value) : base(value)
		{
			Unlimited<uint>? voiceMaxConcurrency = this.VoiceMaxConcurrency;
			Unlimited<uint>? voiceMaxBurst = this.VoiceMaxBurst;
			Unlimited<uint>? voiceRechargeRate = this.VoiceRechargeRate;
			Unlimited<uint>? voiceCutoffBalance = this.VoiceCutoffBalance;
		}

		public static ThrottlingPolicyOwaSettings Parse(string stateToParse)
		{
			return new ThrottlingPolicyOwaSettings(stateToParse);
		}

		internal Unlimited<uint>? VoiceMaxConcurrency
		{
			get
			{
				return base.GetValueFromPropertyBag("VoiceMaxConcur");
			}
			set
			{
				base.SetValueInPropertyBag("VoiceMaxConcur", value);
			}
		}

		internal Unlimited<uint>? VoiceMaxBurst
		{
			get
			{
				return base.GetValueFromPropertyBag("VoiceMaxBurst");
			}
			set
			{
				base.SetValueInPropertyBag("VoiceMaxBurst", value);
			}
		}

		internal Unlimited<uint>? VoiceRechargeRate
		{
			get
			{
				return base.GetValueFromPropertyBag("VoiceRechargeRate");
			}
			set
			{
				base.SetValueInPropertyBag("VoiceRechargeRate", value);
			}
		}

		internal Unlimited<uint>? VoiceCutoffBalance
		{
			get
			{
				return base.GetValueFromPropertyBag("VoiceCutoff");
			}
			set
			{
				base.SetValueInPropertyBag("VoiceCutoff", value);
			}
		}

		private const string VoiceMaxConcurrencyPrefix = "VoiceMaxConcur";

		private const string VoiceMaxBurstPrefix = "VoiceMaxBurst";

		private const string VoiceRechargeRatePrefix = "VoiceRechargeRate";

		private const string VoiceCutoffBalancePrefix = "VoiceCutoff";
	}
}
