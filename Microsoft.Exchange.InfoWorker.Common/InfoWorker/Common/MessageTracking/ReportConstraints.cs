using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ReportConstraints
	{
		internal DeliveryStatus? Status;

		internal SmtpAddress[] RecipientPathFilter;

		internal string[] Recipients;

		internal ReportTemplate ReportTemplate;

		internal bool BypassDelegateChecking;

		internal MessageTrackingDetailLevel DetailLevel;

		internal bool DoNotResolve;

		internal Unlimited<uint> ResultSize;

		internal bool TrackingAsSender;

		internal SmtpAddress Sender;

		internal bool ReturnQueueEvents;
	}
}
