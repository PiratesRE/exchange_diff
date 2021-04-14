using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum SmtpInStateMachineEvents
	{
		CommandFailed,
		DataFailed,
		BdatFailed,
		NetworkError,
		SendResponseAndDisconnectClient,
		AuthProcessed,
		BdatProcessed,
		BdatLastProcessed,
		DataProcessed,
		EhloProcessed,
		ExpnProcessed,
		HeloProcessed,
		HelpProcessed,
		MailProcessed,
		NoopProcessed,
		QuitProcessed,
		RcptProcessed,
		RsetProcessed,
		StartTlsProcessed,
		VrfyProcessed,
		XExpsProcessed,
		XAnonymousTlsProcessed,
		XSessionParamsProcessed
	}
}
