using System;

namespace Microsoft.Exchange.Connections.Pop
{
	internal enum Pop3ClientState
	{
		ProcessConnection,
		ProcessCapaCommand,
		ProcessTopCommand,
		ProcessStlsCommand,
		ProcessAuthNtlmCommand,
		ProcessUserCommand,
		ProcessPassCommand,
		ProcessStatCommand,
		ProcessUidlCommand,
		ProcessListCommand,
		ProcessRetrCommand,
		ProcessDeleCommand,
		ProcessQuitCommand,
		ProcessEnd
	}
}
