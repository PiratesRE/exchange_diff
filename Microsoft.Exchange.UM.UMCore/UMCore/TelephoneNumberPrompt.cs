using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TelephoneNumberPrompt : VariablePrompt<PhoneNumber>
	{
		public TelephoneNumberPrompt()
		{
		}

		internal TelephoneNumberPrompt(string promptName, CultureInfo culture, PhoneNumber value) : base(promptName, culture, value)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"telephone",
				base.Config.PromptName,
				string.Empty,
				this.phoneNumber
			});
		}

		internal override string ToSsml()
		{
			return this.innerPrompt.ToSsml();
		}

		protected override void InternalInitialize()
		{
			this.phoneNumber = base.InitVal.ToDial;
			if (-1 != this.phoneNumber.IndexOf("@", StringComparison.InvariantCulture))
			{
				this.innerPrompt = new EmailAddressPrompt();
				this.innerPrompt.Initialize(base.Config, base.Culture, this.phoneNumber);
			}
			else if (TelephoneNumberPrompt.AtleastOneValidDigitRegex.IsMatch(this.phoneNumber))
			{
				this.innerPrompt = new DigitPrompt();
				this.innerPrompt.Initialize(base.Config, base.Culture, this.phoneNumber);
				PhoneNumber phoneNumber = null;
				if (PhoneNumber.TryParse(this.phoneNumber, out phoneNumber))
				{
					this.phoneNumber = phoneNumber.Number;
				}
			}
			else
			{
				this.innerPrompt = new EmailAddressPrompt();
				this.innerPrompt.Initialize(base.Config, base.Culture, this.phoneNumber);
			}
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, this.phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "TelephoneNumberPromp successfully intialized with text _PhoneNumber.", new object[0]);
		}

		private static readonly Regex AtleastOneValidDigitRegex = new Regex("\\d", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private string phoneNumber;

		private VariablePrompt<string> innerPrompt;
	}
}
