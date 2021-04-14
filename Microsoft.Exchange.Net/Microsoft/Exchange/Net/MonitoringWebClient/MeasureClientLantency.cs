using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class MeasureClientLantency : BaseTestStep
	{
		protected override TestId Id
		{
			get
			{
				return TestId.MeasureClientLatency;
			}
		}

		protected override void StartTest()
		{
			HttpPingServer instance = HttpPingServer.Instance;
			instance.Initialize();
			string uri = string.Format("http://localhost:{0}", instance.Port);
			this.session.BeginGet(this.Id, uri, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.ClientPingResponseReceived), tempResult);
			}, null);
		}

		private void ClientPingResponseReceived(IAsyncResult result)
		{
			try
			{
				this.session.EndGet<object>(result, null);
			}
			catch (Exception)
			{
			}
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.MeasureClientLatency;
	}
}
