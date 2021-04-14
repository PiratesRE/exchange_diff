using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DispatchEntry : EventArgs
	{
		public DispatchEntry(MiniSubscriptionInformation miniSubscriptionInformation, WorkType workType, ExDateTime dispatchAttemptTime, ExDateTime enqueueTime)
		{
			this.miniSubscriptionInformation = miniSubscriptionInformation;
			this.workType = workType;
			this.dispatchAttemptTime = dispatchAttemptTime;
			this.dispatchEnqueueTime = enqueueTime;
		}

		internal MiniSubscriptionInformation MiniSubscriptionInformation
		{
			get
			{
				return this.miniSubscriptionInformation;
			}
		}

		internal WorkType WorkType
		{
			get
			{
				return this.workType;
			}
		}

		internal ExDateTime DispatchAttemptTime
		{
			get
			{
				return this.dispatchAttemptTime;
			}
		}

		internal ExDateTime DispatchEnqueuedTime
		{
			get
			{
				return this.dispatchEnqueueTime;
			}
		}

		public void SetDispatchAttemptTime(ExDateTime dispatchAttemptTime)
		{
			this.dispatchAttemptTime = dispatchAttemptTime;
		}

		public XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("DispatchEntry");
			xelement.Add(new XElement("subscriptionGuid", this.MiniSubscriptionInformation.SubscriptionGuid));
			xelement.Add(new XElement("workType", this.workType.ToString()));
			xelement.Add(new XElement("dispatchAttemptTime", this.dispatchAttemptTime.ToString("o")));
			return xelement;
		}

		private MiniSubscriptionInformation miniSubscriptionInformation;

		private WorkType workType;

		private ExDateTime dispatchAttemptTime;

		private ExDateTime dispatchEnqueueTime;
	}
}
