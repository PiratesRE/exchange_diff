using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess
{
	internal interface IUMUserMailboxStorage : IDisposeTrackable, IDisposable
	{
		void InitUMMailbox();

		void ResetUMMailbox(bool keepProperties);

		PINInfo ValidateUMPin(string pin, Guid userUMMailboxPolicyGuid);

		void SaveUMPin(PINInfo pin, Guid userUMMailboxPolicyGuid);

		PINInfo GetUMPin();

		void SendEmail(string recipientMailAddress, string messageSubject, string messageBody);

		PersonaType GetPersonaFromEmail(string emailAddress);

		UMSubscriberCallAnsweringData GetUMSubscriberCallAnsweringData(UMSubscriber subscriber, TimeSpan timeout);
	}
}
