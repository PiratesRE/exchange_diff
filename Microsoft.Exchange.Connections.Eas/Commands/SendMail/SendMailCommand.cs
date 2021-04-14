using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.SendMail
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SendMailCommand : EasServerCommand<SendMailRequest, SendMailResponse, SendMailStatus>
	{
		internal SendMailCommand(EasConnectionSettings easConnectionSettings) : base(Command.SendMail, easConnectionSettings)
		{
		}
	}
}
