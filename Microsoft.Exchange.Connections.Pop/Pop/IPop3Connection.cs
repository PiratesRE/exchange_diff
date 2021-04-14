using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public interface IPop3Connection : IConnection<IPop3Connection>, IDisposable
	{
		Pop3ConnectionContext ConnectionContext { get; }

		void ConnectAndAuthenticate(Pop3ServerParameters serverParameters, Pop3AuthenticationParameters authenticationParameters);

		Pop3ResultData DeleteEmails(List<int> messageIds);

		Pop3ResultData GetEmail(int messageId);

		Pop3ResultData GetUniqueIds();

		Pop3ResultData Quit();

		Pop3ResultData VerifyAccount();
	}
}
