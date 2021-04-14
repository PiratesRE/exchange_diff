using System;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	internal enum OnDemandQueryLogEvent
	{
		New = 1,
		Submit,
		Step,
		Complete,
		View,
		Fail,
		FailToSendEmail
	}
}
