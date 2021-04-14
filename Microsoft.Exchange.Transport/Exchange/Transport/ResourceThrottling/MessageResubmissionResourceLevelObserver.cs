using System;
using Microsoft.Exchange.Transport.MessageResubmission;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class MessageResubmissionResourceLevelObserver : ResourceLevelObserver
	{
		public MessageResubmissionResourceLevelObserver(MessageResubmissionComponent messageResubmissionComponent) : base("MessageResubmission", messageResubmissionComponent, null)
		{
		}

		internal const string ResourceObserverName = "MessageResubmission";
	}
}
