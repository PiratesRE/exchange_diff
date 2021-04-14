using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class Logoff : BaseTestStep
	{
		public Uri BaseUri { get; private set; }

		public string LogoffPath { get; private set; }

		protected override TestId Id
		{
			get
			{
				return TestId.Logoff;
			}
		}

		public Logoff(Uri uri, string logoffPath)
		{
			this.BaseUri = uri;
			this.LogoffPath = logoffPath;
		}

		protected override void StartTest()
		{
			this.session.BeginGetFollowingRedirections(this.Id, new Uri(this.BaseUri, this.LogoffPath).ToString(), RedirectionOptions.FollowUntilNo302OrSpecificRedirection, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.LogoffResponseReceived), tempResult);
			}, new Dictionary<string, object>
			{
				{
					"LastExpectedRedirection",
					new string[]
					{
						"logoff.aspx",
						"logon.aspx",
						"signout.aspx"
					}
				}
			});
		}

		private void LogoffResponseReceived(IAsyncResult result)
		{
			this.session.EndGetFollowingRedirections<object>(result, null);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.Logoff;
	}
}
