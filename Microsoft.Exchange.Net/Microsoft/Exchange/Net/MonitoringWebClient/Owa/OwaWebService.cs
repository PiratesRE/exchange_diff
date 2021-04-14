using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaWebService : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public RequestBody RequestBody { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaWebService;
			}
		}

		public OwaWebService(Uri uri, string actionName)
		{
			this.Uri = uri;
			this.action = actionName;
			this.RequestBody = RequestBody.Format(string.Empty, new object[0]);
		}

		public OwaWebService(Uri uri, string actionName, RequestBody requestBody) : this(uri, actionName)
		{
			this.RequestBody = requestBody;
		}

		protected override void StartTest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("action", this.action);
			this.session.BeginPost(this.Id, new Uri(this.Uri, "service.svc?action=" + this.action).ToString(), this.RequestBody, "application/json; charset=utf-8", dictionary, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.WebServiceResponseReceived), tempResult);
			}, null);
		}

		private void WebServiceResponseReceived(IAsyncResult result)
		{
			this.session.EndGet<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaWebService;

		private readonly string action;
	}
}
