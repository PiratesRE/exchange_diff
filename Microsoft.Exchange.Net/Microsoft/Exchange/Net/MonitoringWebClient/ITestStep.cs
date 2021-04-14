using System;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface ITestStep
	{
		IAsyncResult BeginExecute(IHttpSession session, AsyncCallback callback, object state);

		void EndExecute(IAsyncResult result);

		Task CreateTask(IHttpSession session);

		object Result { get; }

		TimeSpan? MaxRunTime { get; set; }
	}
}
