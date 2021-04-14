using System;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallerNameOrNumberPrompt : VariablePrompt<NameOrNumberOfCaller>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"callerNameOrNumber",
				base.Config.PromptName,
				string.Empty,
				base.SbLog.ToString()
			});
		}

		internal override string ToSsml()
		{
			return base.SbSsml.ToString();
		}

		protected override void InternalInitialize()
		{
			switch (base.InitVal.TypeOfCall)
			{
			case NameOrNumberOfCaller.TypeOfVoiceCall.MissedCall:
				this.AddMissedCallPrompts();
				return;
			case NameOrNumberOfCaller.TypeOfVoiceCall.VoicemailCall:
				this.AddVoicemailCallPrompts();
				return;
			default:
				throw new InvalidArgumentException("TypeOfCall");
			}
		}

		private void AddMissedCallPrompts()
		{
			if (!this.IsNullEmailSender() && base.InitVal.CallerId != null)
			{
				Prompt[] paramPrompt = new Prompt[]
				{
					new SpokenNamePrompt("emailSender", base.Culture, base.InitVal.EmailSender),
					new TelephoneNumberPrompt("senderCallerID", base.Culture, base.InitVal.CallerId)
				};
				base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiEmailStatusTypeMissedCallNameAndNumber", base.Culture, paramPrompt));
				return;
			}
			if (this.IsNullEmailSender() && base.InitVal.CallerId != null)
			{
				TelephoneNumberPrompt telephoneNumberPrompt = new TelephoneNumberPrompt("senderCallerID", base.Culture, base.InitVal.CallerId);
				base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiEmailStatusTypeMissedCallNumber", base.Culture, new Prompt[]
				{
					telephoneNumberPrompt
				}));
				return;
			}
			if (this.IsNullEmailSender() && base.InitVal.CallerId == null)
			{
				base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiEmailStatusTypeMissedCallUnknown", base.Culture, null));
			}
		}

		private void AddVoicemailCallPrompts()
		{
			if (base.InitVal.CallerId != null)
			{
				Prompt[] paramPrompt = new Prompt[]
				{
					new TimePrompt("messageReceivedTime", base.Culture, base.InitVal.MessageReceivedTime),
					new TelephoneNumberPrompt("senderCallerID", base.Culture, base.InitVal.CallerId)
				};
				base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiVoiceMessageEnvelope", base.Culture, paramPrompt));
				return;
			}
			if (!string.IsNullOrEmpty(base.InitVal.CallerName))
			{
				Prompt[] paramPrompt2 = new Prompt[]
				{
					new TimePrompt("messageReceivedTime", base.Culture, base.InitVal.MessageReceivedTime),
					new TextPrompt("senderInfo", base.Culture, base.InitVal.CallerName)
				};
				base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiVoiceMessageEnvelope", base.Culture, paramPrompt2));
				return;
			}
			TimePrompt timePrompt = new TimePrompt("messageReceivedTime", base.Culture, base.InitVal.MessageReceivedTime);
			base.AddPrompt(PromptUtils.CreateSingleStatementPrompt("tuiMessageReceived", base.Culture, new Prompt[]
			{
				timePrompt
			}));
		}

		private bool IsNullEmailSender()
		{
			if (base.InitVal.EmailSender is string)
			{
				return string.IsNullOrEmpty((string)base.InitVal.EmailSender);
			}
			return base.InitVal.EmailSender == null;
		}

		private const string EmailStatusMissedCallNameAndNumberPrompt = "tuiEmailStatusTypeMissedCallNameAndNumber";

		private const string EmailStatusTypeMissedCallNumberPrompt = "tuiEmailStatusTypeMissedCallNumber";

		private const string EmailStatusTypeMissedCallUnknownPrompt = "tuiEmailStatusTypeMissedCallUnknown";

		private const string VoiceMessageEnvelopePrompt = "tuiVoiceMessageEnvelope";

		private const string VoiceMessageReceivedPrompt = "tuiMessageReceived";
	}
}
