using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	public enum Stage
	{
		None,
		LoadItem,
		SendAsCheck,
		CreateMailItem,
		OnDemotedEvent,
		SubmitNdrForInvalidRecipients,
		CommitMailItem,
		StartedSMTPOutOperation,
		FinishedSMTPOutOperation,
		SMTPOutThrewException,
		SubmitMailItem,
		DoneWithMessage,
		EventHandled
	}
}
