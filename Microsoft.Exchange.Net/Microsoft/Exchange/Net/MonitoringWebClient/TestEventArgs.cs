using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class TestEventArgs : EventArgs
	{
		public TestEventArgs(TestId testId, object eventState)
		{
			this.TestId = testId;
			this.EventState = eventState;
		}

		public TestId TestId { get; set; }

		public object EventState { get; set; }
	}
}
