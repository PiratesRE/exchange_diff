using System;

namespace Microsoft.Forefront.Reporting.Common
{
	public enum QueryProperty
	{
		FromDate = 1,
		ToDate,
		Direction,
		Sender,
		Recipient,
		MsgID,
		MsgStatus,
		SenderIP,
		RuleID,
		RuleAction,
		DLPID,
		DLPAction,
		DLPAuditSeverity,
		DLPSenderOverride,
		Subject
	}
}
