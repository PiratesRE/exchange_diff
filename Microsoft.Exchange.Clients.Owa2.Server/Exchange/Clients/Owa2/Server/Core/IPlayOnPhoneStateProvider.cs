using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IPlayOnPhoneStateProvider : IDisposable
	{
		UMCallState GetCallState(string callId);
	}
}
