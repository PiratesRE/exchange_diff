using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaLogin : BaseTestStep
	{
		public Uri Uri { get; private set; }

		public string UserName { get; private set; }

		public string UserDomain { get; private set; }

		public SecureString Password { get; private set; }

		public OwaLoginParameters OwaLoginParameters { get; private set; }

		public ITestFactory TestFactory { get; private set; }

		public AuthenticationParameters AuthenticationParameters { get; set; }

		protected override TestId Id
		{
			get
			{
				return TestId.OwaLoginScenario;
			}
		}

		public OwaLogin(Uri uri, string userName, string userDomain, SecureString password, OwaLoginParameters owaLoginParameters, ITestFactory factory)
		{
			this.Uri = uri;
			this.UserName = userName;
			this.UserDomain = userDomain;
			this.Password = password;
			this.OwaLoginParameters = owaLoginParameters;
			this.TestFactory = factory;
		}

		protected override void Finally()
		{
			this.session.CloseConnections();
		}

		protected override void ExceptionThrown(ScenarioException e)
		{
			try
			{
				HttpWebResponseWrapperException ex = e.InnerException as HttpWebResponseWrapperException;
				if (e.FailureReason == FailureReason.CafeTimeoutContactingBackend && ex != null)
				{
					Uri uri = new Uri(ex.Request.RequestUri, "/owa/exhealth.reporttimeout");
					IAsyncResult asyncResult = this.session.BeginGet(this.Id, uri.ToString(), null, new Dictionary<string, object>
					{
						{
							"RequestTimeout",
							TimeSpan.FromSeconds(15.0)
						}
					});
					asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(20.0));
					this.session.EndGet<object>(asyncResult, null);
				}
			}
			catch (Exception)
			{
			}
		}

		protected override void StartTest()
		{
			this.session.PersistentHeaders.Add("X-OWA-ActionName", "Monitoring");
			this.session.PersistentHeaders.Add("X-MonitoringInstance", ExMonitoringRequestTracker.Instance.MonitoringInstanceId);
			if (this.OwaLoginParameters.CafeOutboundRequestTimeout > TimeSpan.Zero)
			{
				this.session.PersistentHeaders.Add(WellKnownHeader.FrontEndToBackEndTimeout, this.OwaLoginParameters.CafeOutboundRequestTimeout.TotalSeconds.ToString());
			}
			if (this.AuthenticationParameters == null)
			{
				this.AuthenticationParameters = new AuthenticationParameters();
			}
			this.AuthenticationParameters.ShouldDownloadStaticFileOnLogonPage = this.OwaLoginParameters.ShouldDownloadStaticFileOnLogonPage;
			ITestStep testStep = this.TestFactory.CreateAuthenticateStep(this.Uri, this.UserName, this.UserDomain, this.Password, this.AuthenticationParameters, this.TestFactory);
			testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.AuthenticationCompleted), tempResult);
			}, testStep);
			if (this.OwaLoginParameters.ShouldMeasureClientLatency)
			{
				ITestStep testStep2 = this.TestFactory.CreateMeasureClientLatencyStep();
				testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.ClientLatencyMeasured), tempResult);
				}, testStep2);
			}
		}

		private void AuthenticationCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			Uri uri = testStep.Result as Uri;
			if (uri != null)
			{
				this.Uri = uri;
			}
			ITestStep testStep2 = this.TestFactory.CreateOwaStartPageStep(this.Uri);
			testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
			{
				base.AsyncCallbackWrapper(new AsyncCallback(this.StartPageTestCompleted), tempResult);
			}, testStep2);
		}

		private void ClientLatencyMeasured(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
		}

		private void StartPageTestCompleted(IAsyncResult result)
		{
			OwaStartPage owaStartPage = result.AsyncState as OwaStartPage;
			owaStartPage.EndExecute(result);
			this.Uri = owaStartPage.Uri;
			this.startPage = owaStartPage.StartPage;
			if (this.startPage is Owa14StartPage)
			{
				ITestStep testStep = this.TestFactory.CreateOwaPingStep(this.Uri);
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.VersionSpecificTestCompleted), tempResult);
				}, testStep);
			}
			else
			{
				ITestStep testStep2 = this.TestFactory.CreateOwaWebServiceStep(this.Uri, "GetFolderMruConfiguration");
				testStep2.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.VersionSpecificTestCompleted), tempResult);
				}, testStep2);
			}
			if (this.OwaLoginParameters.ShouldDownloadStaticFile)
			{
				this.simultaneousStepCount = 2;
				ITestStep testStep3 = this.TestFactory.CreateOwaDownloadStaticFileStep(this.startPage.StaticFileUri);
				testStep3.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.DownloadStaticFileStepCompleted), tempResult);
				}, testStep3);
				return;
			}
			this.simultaneousStepCount = 1;
		}

		private void VersionSpecificTestCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			this.KickoffLogoffStep();
		}

		private void DownloadStaticFileStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			this.KickoffLogoffStep();
		}

		private void KickoffLogoffStep()
		{
			int num = Interlocked.Increment(ref this.currentFinishedStepCount);
			if (num >= this.simultaneousStepCount)
			{
				ITestStep testStep = this.TestFactory.CreateLogoffStep(this.Uri, "logoff.owa");
				testStep.BeginExecute(this.session, delegate(IAsyncResult tempResult)
				{
					base.AsyncCallbackWrapper(new AsyncCallback(this.LogOffStepCompleted), tempResult);
				}, testStep);
			}
		}

		private void LogOffStepCompleted(IAsyncResult result)
		{
			ITestStep testStep = result.AsyncState as ITestStep;
			testStep.EndExecute(result);
			base.ExecutionCompletedSuccessfully();
		}

		private const TestId ID = TestId.OwaLoginScenario;

		private int simultaneousStepCount;

		private int currentFinishedStepCount;

		private OwaStartPage startPage;
	}
}
