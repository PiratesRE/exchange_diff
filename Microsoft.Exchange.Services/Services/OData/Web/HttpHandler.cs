using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class HttpHandler : IHttpAsyncHandler, IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new InvalidOperationException("Should never hit here!");
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			this.asyncCallback = cb;
			this.requestBroker = new RequestBroker(context);
			Task task = this.requestBroker.Process();
			return task.ContinueWith(new Action<Task>(this.BrokerProcessingComplete));
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			this.requestBroker.Dispose();
			if (this.asyncException != null)
			{
				throw new HttpException(500, this.asyncException.Message, this.asyncException);
			}
		}

		private void BrokerProcessingComplete(Task brokerTask)
		{
			if (brokerTask.Exception != null)
			{
				this.asyncException = brokerTask.Exception.InnerException;
				ServiceDiagnostics.ReportException(this.asyncException, ServicesEventLogConstants.Tuple_InternalServerError, null, "Unhandled OData service exception: {0}");
			}
			this.asyncCallback(brokerTask);
		}

		private RequestBroker requestBroker;

		private Exception asyncException;

		private AsyncCallback asyncCallback;
	}
}
