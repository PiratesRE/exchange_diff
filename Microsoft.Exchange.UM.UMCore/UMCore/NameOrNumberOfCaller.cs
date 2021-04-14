using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class NameOrNumberOfCaller
	{
		internal PhoneNumber CallerId { get; set; }

		internal string CallerName { get; set; }

		internal object EmailSender { get; set; }

		internal NameOrNumberOfCaller.TypeOfVoiceCall TypeOfCall { get; private set; }

		internal ExDateTime MessageReceivedTime { get; set; }

		internal NameOrNumberOfCaller(NameOrNumberOfCaller.TypeOfVoiceCall typeOfCall)
		{
			this.TypeOfCall = typeOfCall;
		}

		internal void ClearProperties()
		{
			this.CallerId = null;
			this.CallerName = null;
			this.MessageReceivedTime = ExDateTime.MinValue;
			this.EmailSender = null;
		}

		internal enum TypeOfVoiceCall
		{
			MissedCall,
			VoicemailCall
		}
	}
}
