using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.MonitoringWebClient;

namespace Microsoft.Exchange.Monitoring
{
	public abstract class TestWebApplicationConnectivity2 : TestVirtualDirectoryConnectivity
	{
		internal TestWebApplicationConnectivity2(LocalizedString applicationName, LocalizedString applicationShortName, string monitoringEventSourceInternal, string monitoringEventSourceExternal) : base(applicationName, applicationShortName, null, monitoringEventSourceInternal, monitoringEventSourceExternal)
		{
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string URL
		{
			get
			{
				if (!(this.explicitlySetUrl == null))
				{
					return string.Empty;
				}
				return this.explicitlySetUrl.ToString();
			}
			set
			{
				this.explicitlySetUrl = new Uri(value);
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = false)]
		public PSCredential MailboxCredential
		{
			get
			{
				return this.explicitlySetMailboxCredential;
			}
			set
			{
				this.explicitlySetMailboxCredential = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter AllowUnsecureAccess
		{
			get
			{
				return this.allowUnsecureAccess;
			}
			set
			{
				this.allowUnsecureAccess = value;
			}
		}

		[ValidateRange(0, 360)]
		[Parameter(Mandatory = false)]
		public int RequestTimeout
		{
			get
			{
				if (this.requestTimeout != null)
				{
					return (int)this.requestTimeout.Value.TotalSeconds;
				}
				if (base.TestType == OwaConnectivityTestType.Internal)
				{
					return (int)TestWebApplicationConnectivity2.DefaultInternalRequestTimeOut.TotalSeconds;
				}
				return (int)TestWebApplicationConnectivity2.DefaultExternalRequestTimeOut.TotalSeconds;
			}
			set
			{
				this.requestTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)value));
			}
		}

		protected override DatacenterUserType DefaultUserType
		{
			get
			{
				return DatacenterUserType.EDU;
			}
		}

		protected override uint GetDefaultTimeOut()
		{
			if (base.TestType == OwaConnectivityTestType.Internal)
			{
				return (uint)TestWebApplicationConnectivity2.DefaultInternalTimeOut.TotalSeconds;
			}
			return (uint)TestWebApplicationConnectivity2.DefaultExternalTimeOut.TotalSeconds;
		}

		protected sealed override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			TaskLogger.LogEnter();
			try
			{
				VirtualDirectoryUriScope virtualDirectoryUriScope;
				Uri testUri = this.GetTestUri(instance, out virtualDirectoryUriScope);
				base.WriteVerbose(Strings.CasHealthWebAppStartTest(testUri));
				IRequestAdapter requestAdapter = this.CreateRequestAdapter(virtualDirectoryUriScope);
				IExceptionAnalyzer exceptionAnalyzer = this.CreateExceptionAnalyzer(testUri);
				IResponseTracker responseTracker = this.CreateResponseTracker();
				IHttpSession session = this.CreateHttpSession(requestAdapter, exceptionAnalyzer, responseTracker, instance);
				this.HookupEventHandlers(session);
				CasTransactionOutcome casTransactionOutcome = this.CreateOutcome(instance, testUri, responseTracker);
				string userName;
				string domain;
				SecureString secureString;
				this.GetUserParameters(instance, out userName, out domain, out secureString);
				ITestStep testStep = this.CreateScenario(instance, testUri, userName, domain, secureString, virtualDirectoryUriScope, instance.CasFqdn);
				testStep.BeginExecute(session, new AsyncCallback(this.ScenarioExecutionFinished), new object[]
				{
					testStep,
					instance,
					responseTracker,
					casTransactionOutcome,
					secureString
				});
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return null;
		}

		private void ScenarioExecutionFinished(IAsyncResult result)
		{
			TaskLogger.LogEnter();
			object[] array = result.AsyncState as object[];
			ITestStep testStep = array[0] as ITestStep;
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = array[1] as TestCasConnectivity.TestCasConnectivityRunInstance;
			IResponseTracker responseTracker = array[2] as IResponseTracker;
			CasTransactionOutcome casTransactionOutcome = array[3] as CasTransactionOutcome;
			SecureString secureString = array[4] as SecureString;
			try
			{
				testStep.EndExecute(result);
				this.CompleteSuccessfulOutcome(casTransactionOutcome, testCasConnectivityRunInstance, responseTracker);
			}
			catch (Exception ex)
			{
				testCasConnectivityRunInstance.Outcomes.Enqueue(TestWebApplicationConnectivity2.GenerateVerboseMessage(ex.ToString()));
				this.CompleteFailedOutcome(casTransactionOutcome, testCasConnectivityRunInstance, responseTracker, ex);
			}
			finally
			{
				TaskLogger.LogExit();
				if (secureString != null)
				{
					secureString.Dispose();
				}
				if (testCasConnectivityRunInstance != null)
				{
					testCasConnectivityRunInstance.Outcomes.Enqueue(casTransactionOutcome);
					testCasConnectivityRunInstance.Result.Complete();
				}
			}
		}

		internal abstract IExceptionAnalyzer CreateExceptionAnalyzer(Uri testUri);

		internal abstract ITestStep CreateScenario(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, string userName, string domain, SecureString password, VirtualDirectoryUriScope testType, string serverFqdn);

		internal abstract CasTransactionOutcome CreateOutcome(TestCasConnectivity.TestCasConnectivityRunInstance instance, Uri testUri, IResponseTracker responseTracker);

		internal abstract void CompleteSuccessfulOutcome(CasTransactionOutcome outcome, TestCasConnectivity.TestCasConnectivityRunInstance instance, IResponseTracker responseTracker);

		internal abstract void CompleteFailedOutcome(CasTransactionOutcome outcome, TestCasConnectivity.TestCasConnectivityRunInstance instance, IResponseTracker responseTracker, Exception e);

		internal virtual IRequestAdapter CreateRequestAdapter(VirtualDirectoryUriScope urlType)
		{
			return new RequestAdapter
			{
				RequestTimeout = TimeSpan.FromSeconds((double)this.RequestTimeout)
			};
		}

		internal virtual IResponseTracker CreateResponseTracker()
		{
			return new ResponseTracker();
		}

		internal virtual IHttpSession CreateHttpSession(IRequestAdapter requestAdapter, IExceptionAnalyzer exceptionAnalyzer, IResponseTracker responseTracker, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			return new HttpSession(requestAdapter, exceptionAnalyzer, responseTracker)
			{
				SslValidationOptions = (this.trustAllCertificates ? SslValidationOptions.NoSslValidation : SslValidationOptions.BasicCertificateValidation),
				UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; TESTCONN2)",
				EventState = instance
			};
		}

		internal virtual void HookupEventHandlers(IHttpSession session)
		{
			session.SendingRequest += this.SendingRequest;
			session.ResponseReceived += this.ResponseReceived;
			session.TestStarted += this.TestStarted;
			session.TestFinished += this.TestFinished;
		}

		protected Uri GetTestUri(TestCasConnectivity.TestCasConnectivityRunInstance instance, out VirtualDirectoryUriScope uriType)
		{
			if (this.explicitlySetUrl != null)
			{
				uriType = VirtualDirectoryUriScope.Unknown;
				return this.explicitlySetUrl;
			}
			uriType = instance.UrlType;
			return instance.baseUri;
		}

		protected static LocalizedString GenerateVerboseMessage(string verboseMessage)
		{
			string value = string.Format("[{1}] : {0}", verboseMessage, ExDateTime.Now.ToString("HH:mm:ss.fff"));
			return new LocalizedString(value);
		}

		private void SendingRequest(object sender, HttpWebEventArgs e)
		{
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Strings.CasHealthWebAppSendingRequest(e.Request.RequestUri)));
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Environment.NewLine + Environment.NewLine + e.Request.ToStringNoBody() + Environment.NewLine));
		}

		private void ResponseReceived(object sender, HttpWebEventArgs e)
		{
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Strings.CasHealthWebAppLiveIdResponseReceived(e.Request.RequestUri, e.Response.StatusCode, e.Response.RespondingFrontEndServer)));
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Environment.NewLine + Environment.NewLine + e.Response.ToStringNoBody() + Environment.NewLine));
		}

		private void TestStarted(object sender, TestEventArgs e)
		{
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Strings.CasHealthWebAppTestStepStarted(e.TestId.ToString())));
		}

		private void TestFinished(object sender, TestEventArgs e)
		{
			this.LogVerbose(e.EventState, TestWebApplicationConnectivity2.GenerateVerboseMessage(Strings.CasHealthWebAppTestStepFinished(e.TestId.ToString())));
		}

		private void LogVerbose(object eventState, LocalizedString stringToTrace)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance testCasConnectivityRunInstance = eventState as TestCasConnectivity.TestCasConnectivityRunInstance;
			testCasConnectivityRunInstance.Outcomes.Enqueue(stringToTrace);
			this.verboseOutput.Append(stringToTrace.ToString() + Environment.NewLine);
		}

		private void GetUserParameters(TestCasConnectivity.TestCasConnectivityRunInstance instance, out string userName, out string userDomain, out SecureString password)
		{
			if (this.explicitlySetMailboxCredential != null)
			{
				userName = this.explicitlySetMailboxCredential.UserName;
				userDomain = null;
				password = this.explicitlySetMailboxCredential.Password.Copy();
				return;
			}
			userName = instance.credentials.UserName;
			userDomain = instance.credentials.Domain;
			password = instance.credentials.Password.ConvertToSecureString();
		}

		protected const string UserAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; TESTCONN2)";

		protected const int EventIdSuccess = 1000;

		protected const int EventIdFailure = 1001;

		private static readonly TimeSpan DefaultExternalTimeOut = TimeSpan.FromMinutes(3.0);

		private static readonly TimeSpan DefaultInternalTimeOut = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan DefaultExternalRequestTimeOut = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan DefaultInternalRequestTimeOut = TimeSpan.FromSeconds(30.0);

		private TimeSpan? requestTimeout;

		protected StringBuilder verboseOutput = new StringBuilder();

		private Uri explicitlySetUrl;

		private PSCredential explicitlySetMailboxCredential;
	}
}
