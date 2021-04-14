using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Monitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("test", "ActiveSyncConnectivity", SupportsShouldProcess = true)]
	public sealed class TestMobileSyncConnectivity : TestCasConnectivity
	{
		[Parameter(Mandatory = false)]
		public string MonitoringInstance
		{
			get
			{
				return this.monitoringInstance;
			}
			set
			{
				this.monitoringInstance = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseAutodiscoverForClientAccessServer
		{
			get
			{
				return this.autodiscoverCas;
			}
			set
			{
				this.autodiscoverCas = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Alias(new string[]
		{
			"Identity"
		})]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter ClientAccessServer
		{
			get
			{
				return this.clientAccessServer;
			}
			set
			{
				this.clientAccessServer = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0)]
		public string URL
		{
			get
			{
				if (!(this.testUri != null))
				{
					return string.Empty;
				}
				return this.testUri.AbsoluteUri;
			}
			set
			{
				this.testUri = new Uri(value);
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

		[Parameter(Mandatory = false)]
		public SwitchParameter TrustAnySSLCertificate
		{
			get
			{
				return this.trustAllCertificates;
			}
			set
			{
				this.trustAllCertificates = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential MailboxCredential
		{
			get
			{
				return this.mailboxCredential;
			}
			set
			{
				this.mailboxCredential = value;
			}
		}

		internal override TransientErrorCache GetTransientErrorCache()
		{
			if (!base.MonitoringContext)
			{
				return null;
			}
			return TransientErrorCache.EASTransientCache;
		}

		protected override uint GetDefaultTimeOut()
		{
			return 120U;
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string text = "";
				if (this.AllowUnsecureAccess)
				{
					text = Strings.CasHealthWarnCredenditialsPassedInTheClear.ToString() + "\r\n";
				}
				if (this.TrustAnySSLCertificate)
				{
					text = text + Strings.CasHealthWarnTrustAllCertificates.ToString() + "\r\n";
				}
				if (this.MailboxCredential != null)
				{
					try
					{
						NetworkCredential networkCredential = this.MailboxCredential.GetNetworkCredential();
						text = text + Strings.CasHealthWarnUserCredentials(networkCredential.Domain + "\\" + networkCredential.UserName).ToString() + "\r\n";
					}
					catch (PSArgumentException)
					{
						text = text + Strings.CasHealthWarnUserCredentials(this.MailboxCredential.UserName).ToString() + "\r\n";
					}
				}
				if (this.casToTest != null)
				{
					text = text + Strings.CasHealthConfirmTestActiveSyncWithServer(this.casToTest.Fqdn).ToString() + "\r\n";
				}
				else if (this.UseAutodiscoverForClientAccessServer)
				{
					text = text + Strings.CasHealthConfirmTestActiveSyncUsingAutodiscovery.ToString() + "\r\n";
				}
				else
				{
					text = text + Strings.CasHealthConfirmTestActiveSyncLocalMachine.ToString() + "\r\n";
				}
				return new LocalizedString(text);
			}
		}

		protected override string PerformanceObject
		{
			get
			{
				return this.MonitoringEventSource;
			}
		}

		protected override string MonitoringEventSource
		{
			get
			{
				return "MSExchange Monitoring ActiveSyncConnectivity " + ((this.testUri != null || this.autodiscoverCas) ? "External" : "Internal");
			}
		}

		protected override string TransactionSuccessEventMessage
		{
			get
			{
				return Strings.AllActiveSyncTransactionsSucceeded;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (this.testUri != null && this.clientAccessServer != null)
				{
					base.CasConnectivityWriteError(new ApplicationException(Strings.CasHealthOwaUrlAndCasArgumentError), ErrorCategory.InvalidArgument, null);
				}
				if (this.testUri != null && this.UseAutodiscoverForClientAccessServer)
				{
					base.CasConnectivityWriteError(new ApplicationException(Strings.CasHealthEasTestTypeAndAutodiscoverArgumentError), ErrorCategory.InvalidArgument, null);
				}
				Server server = null;
				try
				{
					server = base.CasConfigurationSession.FindLocalServer();
				}
				catch (LocalServerNotFoundException)
				{
				}
				catch (DataValidationException ex)
				{
					base.WriteWarning(ex.ToString());
				}
				if (base.ResetTestAccountCredentials)
				{
					base.DetermineCasServer();
					if (this.mailboxServer == null && !this.localServer.IsMailboxServer && this.casToTest == null)
					{
						CasHealthSpecifyMailboxForResetCredentialsException exception = new CasHealthSpecifyMailboxForResetCredentialsException();
						base.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
					}
				}
				else if (this.clientAccessServer == null && !this.autodiscoverCas && (server == null || !server.IsClientAccessServer))
				{
					CasHealthMustSpecifyCasException exception2 = new CasHealthMustSpecifyCasException();
					base.CasConnectivityWriteError(exception2, ErrorCategory.ObjectNotFound, null);
				}
			}
			catch (ADTransientException exception3)
			{
				base.CasConnectivityWriteError(exception3, ErrorCategory.ObjectNotFound, null);
			}
			finally
			{
				if (base.HasErrors && base.MonitoringContext)
				{
					base.WriteObject(this.monitoringData);
				}
				TaskLogger.LogExit();
			}
		}

		protected override List<TestCasConnectivity.TestCasConnectivityRunInstance> PopulateInfoPerCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<CasTransactionOutcome> outcomeList)
		{
			List<TestCasConnectivity.TestCasConnectivityRunInstance> list = new List<TestCasConnectivity.TestCasConnectivityRunInstance>();
			if (this.testUri != null)
			{
				base.WriteVerbose(Strings.CasHealthOwaTestUrlSpecified(this.testUri.AbsoluteUri));
				instance.baseUri = this.testUri;
				instance.CasFqdn = null;
				list.Add(instance);
			}
			else if (string.IsNullOrEmpty(instance.CasFqdn))
			{
				CasHealthMustSpecifyCasException exception = new CasHealthMustSpecifyCasException();
				base.CasConnectivityWriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			else
			{
				instance.baseUri = new Uri("https://" + instance.CasFqdn + "/Microsoft-Server-ActiveSync");
				list.Add(instance);
			}
			return list;
		}

		protected override List<CasTransactionOutcome> DoAutodiscoverCas(TestCasConnectivity.TestCasConnectivityRunInstance instance, List<TestCasConnectivity.TestCasConnectivityRunInstance> newInstances)
		{
			string additionalInformation = null;
			List<CasTransactionOutcome> list = new List<CasTransactionOutcome>();
			ExDateTime now = ExDateTime.Now;
			string text = null;
			if (this.casToTest != null)
			{
				text = this.casToTest.Fqdn;
			}
			bool flag = false;
			Uri uri;
			CasTransactionResultEnum result;
			CasTransactionOutcome casTransactionOutcome;
			if (text != null)
			{
				base.TraceInfo("Exchange Autodiscovery trying auto discver server name: " + text);
				now = ExDateTime.Now;
				uri = this.DoAutodiscoverCas(text, instance, ref additionalInformation, ref flag);
				result = ((null != uri) ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure);
				casTransactionOutcome = new CasTransactionOutcome(text, Strings.CasHealthScenarioAutodiscoverCas, Strings.CasHealthAutodiscoverServer(text, text), null, null, true, instance.credentials.UserName);
				casTransactionOutcome.Update(result, base.ComputeLatency(now), additionalInformation);
				base.WriteObject(casTransactionOutcome);
				list.Add(casTransactionOutcome);
				if (flag)
				{
					return list;
				}
				if (null != uri)
				{
					instance.baseUri = uri;
					instance.autodiscoveredCas = true;
					newInstances.Add(instance);
					return list;
				}
			}
			string text2 = instance.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			string[] array = text2.Split(new char[]
			{
				'@',
				'.'
			});
			if (array.Length == 0)
			{
				CasHealthAutodiscoveryServerNotFoundException exception = new CasHealthAutodiscoveryServerNotFoundException(text2, "");
				casTransactionOutcome = new CasTransactionOutcome(text, Strings.CasHealthScenarioLogon, Strings.CasHealthLogon(instance.credentials.Domain, instance.credentials.UserName), null, null, true, instance.credentials.UserName);
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(now), base.ShortErrorMsgFromException(exception));
				base.WriteObject(casTransactionOutcome);
				list.Add(casTransactionOutcome);
				return list;
			}
			for (int i = 1; i < array.Length - 1; i++)
			{
				text = "Autodiscover." + string.Join(".", array, i, array.Length - i);
				base.TraceInfo("Exchange Autodiscovery trying server: " + text);
				now = ExDateTime.Now;
				uri = this.DoAutodiscoverCas(text, instance, ref additionalInformation, ref flag);
				result = ((null != uri) ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure);
				casTransactionOutcome = new CasTransactionOutcome(text, Strings.CasHealthScenarioAutodiscoverCas, Strings.CasHealthAutodiscoverServer(text, text2), null, null, true, instance.credentials.UserName);
				casTransactionOutcome.Update(result, base.ComputeLatency(now), additionalInformation);
				base.WriteObject(casTransactionOutcome);
				list.Add(casTransactionOutcome);
				if (flag)
				{
					return list;
				}
				if (null != uri)
				{
					instance.baseUri = uri;
					instance.autodiscoveredCas = true;
					newInstances.Add(instance);
					return list;
				}
			}
			text = string.Join(".", array, 1, array.Length - 1);
			base.TraceInfo("Exchange Autodiscovery trying auto discver server name: " + text);
			now = ExDateTime.Now;
			uri = this.DoAutodiscoverCas(text, instance, ref additionalInformation, ref flag);
			result = ((null != uri) ? CasTransactionResultEnum.Success : CasTransactionResultEnum.Failure);
			casTransactionOutcome = new CasTransactionOutcome(text, Strings.CasHealthScenarioAutodiscoverCas, Strings.CasHealthAutodiscoverServer(text, text2), null, null, true, instance.credentials.UserName);
			casTransactionOutcome.Update(result, base.ComputeLatency(now), additionalInformation);
			base.WriteObject(casTransactionOutcome);
			if (flag)
			{
				return list;
			}
			if (null != uri)
			{
				instance.baseUri = uri;
				instance.autodiscoveredCas = true;
				newInstances.Add(instance);
			}
			return list;
		}

		protected override List<CasTransactionOutcome> ExecuteTests(TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			base.TraceInfo(instance.baseUri + " commencing probing task...");
			if (instance.trustAllCertificates)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TestMobileSyncConnectivity.TrustAllCertificatesCallback);
			}
			else
			{
				ServicePointManager.ServerCertificateValidationCallback = null;
			}
			string mailboxFqdn = (instance.exchangePrincipal != null) ? instance.exchangePrincipal.MailboxInfo.Location.ServerFqdn : null;
			ActiveSyncExecutionContext context = new ActiveSyncExecutionContext(instance, mailboxFqdn);
			this.DoOptionsCommandAsync(context);
			return null;
		}

		protected override List<CasTransactionOutcome> BuildPerformanceOutcomes(TestCasConnectivity.TestCasConnectivityRunInstance instance, string mailboxFqdn)
		{
			return new List<CasTransactionOutcome>
			{
				new CasTransactionOutcome((this.testUri == null) ? instance.CasFqdn : this.testUri.Host, Strings.CasHealthEasScenarioDirectPushAndSync, Strings.CasHealthDirectPushAndSyncAggregateScenarioDescription, "DirectPush Latency", base.LocalSiteName, true, instance.credentials.UserName)
			};
		}

		protected override string TransactionFailuresEventMessage(string detailedInformation)
		{
			return Strings.SomeActiveSyncTransactionsFailed(detailedInformation);
		}

		private static bool TrustAllCertificatesCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
		{
			return true;
		}

		private static CasTransactionResultEnum CheckWebResponse(HttpWebRequest webRequest, HttpWebResponse response, NetworkCredential credentials, ref string additionalInfo)
		{
			additionalInfo = null;
			if (!webRequest.HaveResponse)
			{
				additionalInfo = Strings.CasHealthNoHttpResponseRecieved(credentials.Domain, credentials.UserName);
				return CasTransactionResultEnum.Failure;
			}
			if (HttpStatusCode.OK != response.StatusCode)
			{
				StringBuilder stringBuilder = new StringBuilder(Strings.CasHealthIncorrectHttpResponseCode(credentials.Domain, credentials.UserName, response.StatusCode.ToString()));
				if (!string.IsNullOrEmpty(response.Headers["X-MS-Diagnostics"]))
				{
					stringBuilder.Append("\r\n");
					stringBuilder.Append(response.Headers["X-MS-Diagnostics"]);
				}
				additionalInfo = stringBuilder.ToString();
				return CasTransactionResultEnum.Failure;
			}
			additionalInfo = response.Headers["X-MS-Diagnostics"];
			return CasTransactionResultEnum.Success;
		}

		private Uri DoAutodiscoverCas(string autodiscoveryServer, TestCasConnectivity.TestCasConnectivityRunInstance instance, ref string additionalInfo, ref bool invalidUriFound)
		{
			HttpWebResponse httpWebResponse = null;
			invalidUriFound = false;
			additionalInfo = null;
			string str = "https";
			if (instance.allowUnsecureAccess)
			{
				str = "http";
			}
			if (instance.trustAllCertificates)
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TestMobileSyncConnectivity.TrustAllCertificatesCallback);
			}
			else
			{
				ServicePointManager.ServerCertificateValidationCallback = null;
			}
			string text = str + "://" + autodiscoveryServer + "/Autodiscover/Autodiscover.xml/";
			try
			{
				Uri uri = new Uri(text);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
				httpWebRequest.Credentials = instance.credentials.GetCredential(uri, "NTLM");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "text/xml";
				string s = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><Autodiscover xmlns=\"http://schemas.microsoft.com/exchange/autodiscover/mobilesync/requestschema/2006\"><Request><EMailAddress>" + instance.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString() + "</EMailAddress><AcceptableResponseSchema>http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006</AcceptableResponseSchema></Request></Autodiscover>";
				Stream requestStream = httpWebRequest.GetRequestStream();
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			}
			catch (WebException exception)
			{
				additionalInfo = base.ShortErrorMsgFromException(exception);
				return null;
			}
			catch (UriFormatException)
			{
				additionalInfo = Strings.CasHealthAutoDiscoveryBadUri(text);
				return null;
			}
			StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
			string text2 = streamReader.ReadToEnd();
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(text2);
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("ad", "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006");
			xmlNamespaceManager.AddNamespace("ms", "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006");
			string text3 = "/ad:Autodiscover/ms:Response/ms:Action/ms:Settings/ms:Server[child::ms:Type=\"MobileSync\"]/ms:Url";
			XmlNode xmlNode = safeXmlDocument.SelectSingleNode(text3, xmlNamespaceManager);
			if (xmlNode == null)
			{
				additionalInfo = Strings.CasHealthAutodiscoverIncorrectResult(text3, text2);
				return null;
			}
			Uri result;
			try
			{
				result = new Uri(xmlNode.InnerText);
			}
			catch (UriFormatException)
			{
				invalidUriFound = true;
				additionalInfo = Strings.CasHealthAutodiscoverIncorrectResult(text3, text2);
				return null;
			}
			return result;
		}

		private static void TimeoutCallback(object state, bool timeout)
		{
			if (timeout)
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)state;
				httpWebRequest.Abort();
			}
		}

		private CasTransactionOutcome DoOptionsCommandAsync(ActiveSyncExecutionContext context)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			string mailboxFqdn = context.MailboxFqdn;
			base.TraceInfo("Issuing OPTIONS command...");
			CasTransactionOutcome outcome = new CasTransactionOutcome(instance.baseUri.Host, Strings.CasHealthEasScenarioOptions, Strings.CasHealthEasOptionsScenarioDescription, null, base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			ExDateTime now = ExDateTime.Now;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(instance.baseUri);
			this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
			httpWebRequest.Method = "OPTIONS";
			context.CurrentState = new ActiveSyncState(httpWebRequest, outcome)
			{
				StartTime = now
			};
			IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(this.DoOptionsCommandCallback), context);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TestMobileSyncConnectivity.TimeoutCallback), httpWebRequest, this.GetDefaultTimeOut() * 1000U, true);
			return null;
		}

		internal void DoOptionsCommandCallback(IAsyncResult result)
		{
			ActiveSyncExecutionContext activeSyncExecutionContext = result.AsyncState as ActiveSyncExecutionContext;
			ActiveSyncState currentState = activeSyncExecutionContext.CurrentState;
			ExDateTime startTime = currentState.StartTime;
			TestCasConnectivity.TestCasConnectivityRunInstance instance = activeSyncExecutionContext.Instance;
			HttpWebRequest request = currentState.Request;
			CasTransactionOutcome outcome = currentState.Outcome;
			if (this.timeoutWaitHandle != null)
			{
				this.timeoutWaitHandle.Unregister(null);
				this.timeoutWaitHandle = null;
			}
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(result);
				string text = null;
				CasTransactionResultEnum casTransactionResultEnum = TestMobileSyncConnectivity.CheckWebResponse(request, httpWebResponse, instance.credentials, ref text);
				if (CasTransactionResultEnum.Success != casTransactionResultEnum)
				{
					outcome.Update(casTransactionResultEnum, base.ComputeLatency(startTime), text);
				}
				else
				{
					string text2 = "MS-Server-ActiveSync";
					string responseHeader = httpWebResponse.GetResponseHeader(text2);
					if (string.IsNullOrEmpty(responseHeader))
					{
						string text3 = instance.baseUri.ToString();
						if (instance.autodiscoveredCas)
						{
							text3 = text3 + "  " + Strings.CasHealthUriFoundViaAutodiscovery;
						}
						text = Strings.CasHealthAirSyncHeaderNotFound(text2, text3);
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(text);
						stringBuilder.Append("\r\n\r\n");
						stringBuilder.Append(Strings.CasHealthHttpResponseHeaders);
						stringBuilder.Append("\r\n\r\n");
						stringBuilder.Append(httpWebResponse.Headers.ToString());
						outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), stringBuilder.ToString());
					}
					else
					{
						outcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), text);
						base.TraceInfo("OPTIONS command succeeded!");
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is WebException)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					WebException ex2 = ex as WebException;
					if (ex2.Response != null && ex2.Response.Headers != null)
					{
						stringBuilder2.Append("\r\n\r\n");
						stringBuilder2.Append(Strings.CasHealthHttpResponseHeaders);
						stringBuilder2.Append("\r\n\r\n");
						stringBuilder2.Append(ex2.Response.Headers.ToString());
					}
					string text4 = stringBuilder2.ToString();
					if (instance.allowUnsecureAccess && instance.baseUri.Scheme == "https")
					{
						outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthNoteSecureAccess + "\r\n\r\n\r\n" + base.ShortErrorMsgFromException(ex) + text4);
						activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
						instance.baseUri = new Uri("http://" + instance.baseUri.Host + instance.baseUri.PathAndQuery);
						this.DoOptionsCommandAsync(activeSyncExecutionContext);
						return;
					}
					outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(ex) + text4);
				}
				else
				{
					try
					{
						this.HandleKnownExceptions(ex, outcome, startTime);
					}
					catch (Exception)
					{
						activeSyncExecutionContext.Instance.Result.Complete();
						throw;
					}
				}
			}
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(currentState.Outcome);
			}
			if (activeSyncExecutionContext.Instance.Result.DidTimeout() || (outcome != null && CasTransactionResultEnum.Success != outcome.Result.Value))
			{
				outcome.PerformanceCounterName = "DirectPush Latency";
				outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), outcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				return;
			}
			if (activeSyncExecutionContext.Instance.LightMode)
			{
				outcome.PerformanceCounterName = "DirectPush Latency";
				outcome.Update(CasTransactionResultEnum.Success, outcome.Latency, outcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				return;
			}
			this.DoFolderSyncCommandAsync(activeSyncExecutionContext);
		}

		private CasTransactionOutcome HandleKnownExceptions(Exception ex, CasTransactionOutcome outcome, ExDateTime startTime)
		{
			WebException ex2 = ex as WebException;
			if (ex is WbxmlException || ex2 != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (ex2 != null && ex2.Response != null && ex2.Response.Headers != null)
				{
					stringBuilder.Append("\r\n\r\n");
					stringBuilder.Append(Strings.CasHealthHttpResponseHeaders);
					stringBuilder.Append("\r\n\r\n");
					stringBuilder.Append(ex2.Response.Headers.ToString());
				}
				outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(ex) + stringBuilder.ToString());
				return outcome;
			}
			if (ex is SocketException)
			{
				outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(ex));
				return outcome;
			}
			throw ex;
		}

		private void DoFolderSyncCommandAsync(ActiveSyncExecutionContext context)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			string deviceId = context.Instance.deviceId;
			base.TraceInfo("Issuing FolderSync command...");
			CasTransactionOutcome outcome = new CasTransactionOutcome(instance.baseUri.Host, Strings.CasHealthEasScenarioFolderSync, Strings.CasHealthEasFolderSyncScenarioDescription, null, base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			ExDateTime now = ExDateTime.Now;
			string str = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "FolderSync", instance.credentials.UserName, deviceId);
			Uri requestUri = new Uri(instance.baseUri.ToString() + str);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FolderSync xmlns=\"FolderHierarchy:\"><SyncKey>0</SyncKey></FolderSync>";
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(xml);
			Stream requestStream = httpWebRequest.GetRequestStream();
			WbxmlWriter wbxmlWriter = new WbxmlWriter(requestStream);
			wbxmlWriter.WriteXmlDocument(safeXmlDocument);
			requestStream.Close();
			context.CurrentState = new ActiveSyncState(httpWebRequest, outcome)
			{
				StartTime = now
			};
			IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(this.DoFolderSyncCallback), context);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TestMobileSyncConnectivity.TimeoutCallback), httpWebRequest, this.GetDefaultTimeOut() * 1000U, true);
		}

		internal void DoFolderSyncCallback(IAsyncResult result)
		{
			ActiveSyncExecutionContext activeSyncExecutionContext = result.AsyncState as ActiveSyncExecutionContext;
			ActiveSyncState currentState = activeSyncExecutionContext.CurrentState;
			ExDateTime startTime = currentState.StartTime;
			TestCasConnectivity.TestCasConnectivityRunInstance instance = activeSyncExecutionContext.Instance;
			HttpWebRequest request = currentState.Request;
			CasTransactionOutcome outcome = currentState.Outcome;
			if (this.timeoutWaitHandle != null)
			{
				this.timeoutWaitHandle.Unregister(null);
				this.timeoutWaitHandle = null;
			}
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(result);
				string text = null;
				CasTransactionResultEnum casTransactionResultEnum = TestMobileSyncConnectivity.CheckWebResponse(request, httpWebResponse, instance.credentials, ref text);
				if (CasTransactionResultEnum.Success != casTransactionResultEnum)
				{
					outcome.Update(casTransactionResultEnum, base.ComputeLatency(startTime), text);
				}
				else
				{
					SafeXmlDocument safeXmlDocument = this.XmlDocFromWbxmlResponseStream(httpWebResponse.GetResponseStream());
					XmlNode xmlNode = safeXmlDocument.DocumentElement;
					string text2 = "FolderSync";
					if (text2 != xmlNode.Name)
					{
						outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
					}
					else
					{
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("FolderHierarchy:".TrimEnd(new char[]
						{
							':'
						}), "FolderHierarchy:");
						string text3 = "FolderHierarchy:";
						string xpath = string.Concat(new string[]
						{
							"/",
							text3,
							"FolderSync/",
							text3,
							"Status"
						});
						xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
						if (xmlNode == null)
						{
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
						}
						else if ("1" != xmlNode.InnerText)
						{
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthEasCommandFailedWithError("FolderSync", xmlNode.InnerText, text));
						}
						else
						{
							xpath = string.Concat(new string[]
							{
								"/",
								text3,
								"FolderSync/",
								text3,
								"Changes/",
								text3,
								"Add[child::",
								text3,
								"Type=2]/",
								text3,
								"ServerId"
							});
							xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
							if (xmlNode == null)
							{
								outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
							}
							else
							{
								activeSyncExecutionContext.CollectionId = xmlNode.InnerText;
								base.TraceInfo("FolderSync command succeeded!");
								outcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					this.HandleKnownExceptions(ex, outcome, startTime);
				}
				catch (Exception)
				{
					activeSyncExecutionContext.Instance.Result.Complete();
					throw;
				}
			}
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(currentState.Outcome);
			}
			if (activeSyncExecutionContext.Instance.Result.DidTimeout() || (outcome != null && CasTransactionResultEnum.Success != outcome.Result.Value))
			{
				outcome.PerformanceCounterName = "DirectPush Latency";
				outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), outcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				return;
			}
			this.DoSyncCommandAsync(activeSyncExecutionContext, Strings.CasHealthEasScenarioSyncZero, Strings.CasHealthEasSyncZeroScenarioDescription, null, false);
		}

		private void DoSyncCommandAsync(ActiveSyncExecutionContext context, string scenarioName, string scenarioDescription, string monitoringPerfCounter, bool verifyItemCameDown)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			context.VerifyItemCameDown = verifyItemCameDown;
			string syncState = context.SyncState;
			string collectionId = context.CollectionId;
			base.TraceInfo("Issuing Sync command with sync key: " + syncState);
			CasTransactionOutcome outcome = new CasTransactionOutcome(instance.baseUri.Host, scenarioName, scenarioDescription, monitoringPerfCounter, base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			ExDateTime now = ExDateTime.Now;
			string str = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "Sync", instance.credentials.UserName, instance.deviceId);
			Uri requestUri = new Uri(instance.baseUri.ToString() + str);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
			string xml = string.Concat(new string[]
			{
				"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Collections><Collection><Class>Email</Class><SyncKey>",
				syncState,
				"</SyncKey><CollectionId>",
				collectionId,
				"</CollectionId><DeletesAsMoves/>",
				("0" == syncState) ? "" : "<GetChanges/>",
				"</Collection></Collections></Sync>"
			});
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(xml);
			Stream requestStream = httpWebRequest.GetRequestStream();
			WbxmlWriter wbxmlWriter = new WbxmlWriter(requestStream);
			wbxmlWriter.WriteXmlDocument(safeXmlDocument);
			requestStream.Close();
			context.CurrentState = new ActiveSyncState(httpWebRequest, outcome)
			{
				StartTime = now
			};
			IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(this.DoSyncCallback), context);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TestMobileSyncConnectivity.TimeoutCallback), httpWebRequest, this.GetDefaultTimeOut() * 1000U, true);
		}

		internal void DoSyncCallback(IAsyncResult result)
		{
			ActiveSyncExecutionContext activeSyncExecutionContext = result.AsyncState as ActiveSyncExecutionContext;
			ActiveSyncState currentState = activeSyncExecutionContext.CurrentState;
			ExDateTime startTime = currentState.StartTime;
			TestCasConnectivity.TestCasConnectivityRunInstance instance = activeSyncExecutionContext.Instance;
			HttpWebRequest request = currentState.Request;
			CasTransactionOutcome casTransactionOutcome = currentState.Outcome;
			if (this.timeoutWaitHandle != null)
			{
				this.timeoutWaitHandle.Unregister(null);
				this.timeoutWaitHandle = null;
			}
			string text = null;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(result);
				CasTransactionResultEnum casTransactionResultEnum = TestMobileSyncConnectivity.CheckWebResponse(request, httpWebResponse, instance.credentials, ref text);
				if (CasTransactionResultEnum.Success != casTransactionResultEnum)
				{
					casTransactionOutcome.Update(casTransactionResultEnum, base.ComputeLatency(startTime), text);
				}
				else
				{
					SafeXmlDocument safeXmlDocument = this.XmlDocFromWbxmlResponseStream(httpWebResponse.GetResponseStream());
					XmlNode xmlNode = safeXmlDocument.DocumentElement;
					string text2 = "Sync";
					if (text2 != xmlNode.Name)
					{
						casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
					}
					else
					{
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("AirSync:".TrimEnd(new char[]
						{
							':'
						}), "AirSync:");
						string xpath = "/AirSync:Sync/AirSync:Collections/AirSync:Collection/AirSync:Status";
						xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
						if (xmlNode == null)
						{
							casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
						}
						else if ("1" != xmlNode.InnerText)
						{
							casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthEasCommandFailedWithError("Sync", xmlNode.InnerText, text));
						}
						else
						{
							xpath = "/AirSync:Sync/AirSync:Collections/AirSync:Collection/AirSync:SyncKey";
							xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
							if (xmlNode == null)
							{
								casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
							}
							else
							{
								activeSyncExecutionContext.SyncState = xmlNode.InnerText;
								if (activeSyncExecutionContext.VerifyItemCameDown)
								{
									xpath = "/AirSync:Sync/AirSync:Collections/AirSync:Collection/AirSync:Commands/AirSync:Add/AirSync:ServerId";
									if (safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager) == null)
									{
										casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
									}
									else
									{
										base.TraceInfo("Sync command succeeded!");
										casTransactionOutcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), text);
									}
								}
								else
								{
									base.TraceInfo("Sync command succeeded!");
									casTransactionOutcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), (text == null) ? "" : text);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					this.HandleKnownExceptions(ex, casTransactionOutcome, startTime);
				}
				catch (Exception)
				{
					activeSyncExecutionContext.Instance.Result.Complete();
					throw;
				}
			}
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(currentState.Outcome);
			}
			if (activeSyncExecutionContext.Instance.Result.DidTimeout() || (casTransactionOutcome != null && CasTransactionResultEnum.Success != casTransactionOutcome.Result.Value))
			{
				casTransactionOutcome.PerformanceCounterName = "DirectPush Latency";
				casTransactionOutcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), casTransactionOutcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				return;
			}
			if (activeSyncExecutionContext.NextStateToExecute == ActiveSyncExecutionState.GetItemEstimate)
			{
				this.DoGetItemEstimateAsync(activeSyncExecutionContext);
				return;
			}
			if (activeSyncExecutionContext.NextStateToExecute == ActiveSyncExecutionState.Ping)
			{
				this.DoPingAsync(activeSyncExecutionContext);
				return;
			}
			activeSyncExecutionContext.SyncAfterPingLatency = casTransactionOutcome.Latency;
			this.Cleanup(activeSyncExecutionContext);
			casTransactionOutcome = new CasTransactionOutcome(instance.baseUri.Host, Strings.CasHealthEasScenarioDirectPushAndSync, Strings.CasHealthDirectPushAndSyncAggregateScenarioDescription, "DirectPush Latency", base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			casTransactionOutcome.Update(CasTransactionResultEnum.Success, activeSyncExecutionContext.PingLatency + activeSyncExecutionContext.SyncAfterPingLatency, text);
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(casTransactionOutcome);
			}
			activeSyncExecutionContext.Instance.Result.Complete();
		}

		internal void Cleanup(ActiveSyncExecutionContext context)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			CasTransactionOutcome outcome = context.CurrentState.Outcome;
			string collectionId = context.CollectionId;
			bool flag = Datacenter.IsMultiTenancyEnabled();
			try
			{
				if (flag)
				{
					string str = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "ItemOperations", instance.credentials.UserName, instance.deviceId);
					Uri requestUri = new Uri(instance.baseUri.ToString() + str);
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
					this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
					httpWebRequest.Method = "POST";
					httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
					string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><ItemOperations xmlns:A=\"AirSync:\"  xmlns=\"ItemOperations:\"><EmptyFolderContents> <A:CollectionId>" + context.CollectionId + "</A:CollectionId></EmptyFolderContents></ItemOperations>";
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					safeXmlDocument.LoadXml(xml);
					using (Stream requestStream = httpWebRequest.GetRequestStream())
					{
						WbxmlWriter wbxmlWriter = new WbxmlWriter(requestStream);
						wbxmlWriter.WriteXmlDocument(safeXmlDocument);
					}
					using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
					{
						if (httpWebResponse.StatusCode != HttpStatusCode.OK)
						{
							CasHealthErrorPopulatingMailboxException exception = new CasHealthErrorPopulatingMailboxException("Failed to cleanup the populating message: " + httpWebResponse.StatusCode);
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(ExDateTime.Now), base.ShortErrorMsgFromException(exception));
							return;
						}
						goto IL_2BA;
					}
				}
				if (instance.createTestItemContext != null && instance.createTestItemContext.TestItemId != null && this.mailboxCredential == null)
				{
					using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(instance.createTestItemContext.ExchangePrincipal, Thread.CurrentThread.CurrentCulture, "Client=Monitoring;Action=Test-MobileSyncConnectivity"))
					{
						using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Inbox))
						{
							folder.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
							{
								instance.createTestItemContext.TestItemId.ObjectId
							});
							int num = 0;
							int num2 = 1;
							PropertyDefinition[] array = new PropertyDefinition[2];
							array[num] = ItemSchema.Id;
							array[num2] = StoreObjectSchema.CreationTime;
							using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, array))
							{
								bool flag2 = false;
								if (queryResult != null)
								{
									while (!flag2)
									{
										object[][] rows = queryResult.GetRows(10000);
										flag2 = (rows.Length == 0);
										for (int i = 0; i < rows.Length; i++)
										{
											ExDateTime t = (ExDateTime)rows[i][num2];
											if (t <= ExDateTime.Now.AddDays(-2.0))
											{
												folder.DeleteObjects(DeleteItemFlags.SoftDelete, new StoreId[]
												{
													(StoreId)rows[i][num]
												});
											}
										}
									}
								}
							}
						}
					}
				}
				IL_2BA:;
			}
			catch (LocalizedException)
			{
			}
			instance.createTestItemContext = null;
		}

		private void DoGetItemEstimateAsync(ActiveSyncExecutionContext context)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			CasTransactionOutcome outcome = new CasTransactionOutcome(instance.baseUri.Host, Strings.CasHealthEasScenarioGetItemEstimate, Strings.CasHealthEasGetItemEstimate, null, base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			base.TraceInfo("Issuing GetItemEstimate command with syncKey: " + context.SyncState);
			ExDateTime now = ExDateTime.Now;
			string str = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "GetItemEstimate", instance.credentials.UserName, instance.deviceId);
			Uri requestUri = new Uri(instance.baseUri.ToString() + str);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><GetItemEstimate xmlns=\"GetItemEstimate:\" xmlns:A=\"AirSync:\"><Collections><Collection><Class>Email</Class><CollectionId>" + context.CollectionId + "</CollectionId><A:FilterType>0</A:FilterType><A:SyncKey>1</A:SyncKey></Collection></Collections></GetItemEstimate>";
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(xml);
			Stream requestStream = httpWebRequest.GetRequestStream();
			WbxmlWriter wbxmlWriter = new WbxmlWriter(requestStream);
			wbxmlWriter.WriteXmlDocument(safeXmlDocument);
			requestStream.Close();
			context.CurrentState = new ActiveSyncState(httpWebRequest, outcome)
			{
				StartTime = now
			};
			IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(this.DoGetItemEstimateCallback), context);
			this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TestMobileSyncConnectivity.TimeoutCallback), httpWebRequest, this.GetDefaultTimeOut() * 1000U, true);
		}

		internal void DoGetItemEstimateCallback(IAsyncResult result)
		{
			ActiveSyncExecutionContext activeSyncExecutionContext = result.AsyncState as ActiveSyncExecutionContext;
			ActiveSyncState currentState = activeSyncExecutionContext.CurrentState;
			ExDateTime startTime = currentState.StartTime;
			TestCasConnectivity.TestCasConnectivityRunInstance instance = activeSyncExecutionContext.Instance;
			HttpWebRequest request = currentState.Request;
			CasTransactionOutcome outcome = currentState.Outcome;
			if (this.timeoutWaitHandle != null)
			{
				this.timeoutWaitHandle.Unregister(null);
				this.timeoutWaitHandle = null;
			}
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(result);
				string text = null;
				CasTransactionResultEnum casTransactionResultEnum = TestMobileSyncConnectivity.CheckWebResponse(request, httpWebResponse, instance.credentials, ref text);
				if (CasTransactionResultEnum.Success != casTransactionResultEnum)
				{
					outcome.Update(casTransactionResultEnum, base.ComputeLatency(startTime), text);
				}
				else
				{
					SafeXmlDocument safeXmlDocument = this.XmlDocFromWbxmlResponseStream(httpWebResponse.GetResponseStream());
					XmlNode xmlNode = safeXmlDocument.DocumentElement;
					string text2 = "GetItemEstimate";
					if (text2 != xmlNode.Name)
					{
						outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
					}
					else
					{
						XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
						xmlNamespaceManager.AddNamespace("AirSync:".TrimEnd(new char[]
						{
							':'
						}), "AirSync:");
						xmlNamespaceManager.AddNamespace("GetItemEstimate:".TrimEnd(new char[]
						{
							':'
						}), "GetItemEstimate:");
						string text3 = "GetItemEstimate:";
						string xpath = string.Concat(new string[]
						{
							"/",
							text3,
							"GetItemEstimate/",
							text3,
							"Response/",
							text3,
							"Status"
						});
						xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
						if (xmlNode == null)
						{
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
						}
						else if ("1" != xmlNode.InnerText)
						{
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthEasCommandFailedWithError("Sync", xmlNode.InnerText, text));
						}
						else
						{
							xpath = string.Concat(new string[]
							{
								"/",
								text3,
								"GetItemEstimate/",
								text3,
								"Response/",
								text3,
								"Collection/",
								text3,
								"Estimate"
							});
							xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
							if (xmlNode == null)
							{
								outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
							}
							else
							{
								activeSyncExecutionContext.Estimate = int.Parse(xmlNode.InnerText);
								base.TraceInfo("GetItemEstimate succeeded with estimate " + activeSyncExecutionContext.Estimate + "!");
								outcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					this.HandleKnownExceptions(ex, outcome, startTime);
				}
				catch (Exception)
				{
					activeSyncExecutionContext.Instance.Result.Complete();
					throw;
				}
			}
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(currentState.Outcome);
			}
			if (activeSyncExecutionContext.Instance.Result.DidTimeout() || (outcome != null && CasTransactionResultEnum.Success != outcome.Result.Value))
			{
				outcome.PerformanceCounterName = "DirectPush Latency";
				outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), outcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				return;
			}
			activeSyncExecutionContext.NextStateToExecute = ActiveSyncExecutionState.Ping;
			this.DoSyncCommandAsync(activeSyncExecutionContext, Strings.CasHealthEasScenarioSyncOne, Strings.CasHealthEasSyncOne, null, false);
		}

		private void SetStandardWebRequestParams(HttpWebRequest webRequest, NetworkCredential creds, bool monitoringContext, bool safeToBuildUpnString)
		{
			webRequest.Accept = "*/*";
			webRequest.AllowAutoRedirect = false;
			webRequest.UserAgent = "TestActiveSyncConnectivity";
			if (safeToBuildUpnString && creds.UserName.IndexOf('@') == -1)
			{
				creds = new NetworkCredential(string.Format("{0}@{1}", creds.UserName, creds.Domain), creds.Password);
			}
			webRequest.Headers["MS-ASProtocolVersion"] = "12.0";
			webRequest.Credentials = new CredentialCache
			{
				{
					webRequest.RequestUri,
					"Basic",
					creds
				}
			};
			webRequest.PreAuthenticate = true;
		}

		private void DoPingAsync(ActiveSyncExecutionContext context)
		{
			TestCasConnectivity.TestCasConnectivityRunInstance instance = context.Instance;
			string mailboxFqdn = context.MailboxFqdn;
			bool flag = Datacenter.IsMultiTenancyEnabled();
			CasTransactionOutcome casTransactionOutcome = new CasTransactionOutcome(instance.baseUri.Host, Strings.CasHealthEasScenarioPing, Strings.CasHealthEasPing, null, base.LocalSiteName, instance.baseUri.Scheme == "https", instance.credentials.UserName);
			bool flag2 = false;
			int num = 3000;
			ExDateTime startTime;
			if (!flag)
			{
				startTime = ExDateTime.Now.AddMilliseconds((double)num);
				LocalizedException ex = base.SetExchangePrincipal(instance);
				if (ex != null)
				{
					if (ex != null)
					{
						casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(ex));
					}
					else
					{
						casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), "");
					}
					this.UpdateOutcomeAndExit(casTransactionOutcome, instance);
					return;
				}
			}
			else
			{
				startTime = ExDateTime.Now;
				flag2 = true;
			}
			string text;
			string text2;
			if (instance.safeToBuildUpnString)
			{
				if (instance.credentials.UserName.IndexOf('@') == -1)
				{
					text = instance.credentials.UserName + "@" + instance.credentials.Domain;
				}
				else
				{
					text = instance.credentials.UserName;
				}
				text2 = null;
				base.TraceInfo("Calling LogonUser for user " + text);
			}
			else
			{
				text = instance.credentials.UserName;
				text2 = instance.credentials.Domain;
				base.TraceInfo("Calling LogonUser for user " + instance.credentials.Domain + "\\" + instance.credentials.UserName);
			}
			ExTraceGlobals.MonitoringTasksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "DoPingAsync: calling LogonUser for User: {0} Domain: {1}", text, text2);
			try
			{
				if (!flag2)
				{
					base.TraceInfo("Calling LogonUser for user " + instance.credentials.Domain + "\\" + instance.credentials.UserName);
					SafeUserTokenHandle safeUserTokenHandle;
					flag2 = NativeMethods.LogonUser(text, text2, instance.credentials.Password, 8, 0, out safeUserTokenHandle);
					safeUserTokenHandle.Dispose();
					safeUserTokenHandle = null;
				}
				if (!flag2)
				{
					CasHealthCouldNotLogUserException exception = new CasHealthCouldNotLogUserException(TestConnectivityCredentialsManager.GetDomainUserNameFromCredentials(instance.credentials), (mailboxFqdn == null) ? "" : mailboxFqdn, TestConnectivityCredentialsManager.NewTestUserScriptName, Marshal.GetLastWin32Error().ToString());
					casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(exception));
					this.UpdateOutcomeAndExit(casTransactionOutcome, instance);
				}
				else
				{
					base.TraceInfo(string.Concat(new string[]
					{
						"User logon successful for user: ",
						instance.credentials.Domain,
						"\\",
						instance.credentials.UserName,
						"!"
					}));
					base.TraceInfo("Issuing Ping command...");
					string str = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "Ping", instance.credentials.UserName, instance.deviceId);
					Uri requestUri = new Uri(instance.baseUri.ToString() + str);
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
					this.SetStandardWebRequestParams(httpWebRequest, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
					httpWebRequest.Method = "POST";
					httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
					string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><HeartbeatInterval>60</HeartbeatInterval><Folders><Folder><Id>" + context.CollectionId + "</Id><Class>Email</Class></Folder></Folders></Ping>";
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					safeXmlDocument.LoadXml(xml);
					Stream requestStream = httpWebRequest.GetRequestStream();
					WbxmlWriter wbxmlWriter = new WbxmlWriter(requestStream);
					wbxmlWriter.WriteXmlDocument(safeXmlDocument);
					requestStream.Close();
					if (flag)
					{
						instance.createTestItemContext = null;
						string str2 = string.Format("?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity", "SendMail", instance.credentials.UserName, instance.deviceId);
						Uri requestUri2 = new Uri(instance.baseUri.ToString() + str2);
						HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(requestUri2);
						this.SetStandardWebRequestParams(httpWebRequest2, instance.credentials, instance.MonitoringContext, instance.safeToBuildUpnString);
						httpWebRequest2.Method = "POST";
						httpWebRequest2.ContentType = "message/rfc822";
						string s = string.Format("From: {0}\r\nTo: {1}\r\nSubject: {2}\r\nMIME-Version: 1.0\r\nContent-Type: text/plain; Charset= {3}\r\nContent-Transfer-Encoding: {4}\r\nX-MimeOLE: Produced By Microsoft MimeOLE V6.00.2800.1441 \r\n{5}", new object[]
						{
							"",
							instance.credentials.UserName,
							"Exchange ActiveSync TestActiveSyncConnectivity",
							Encoding.UTF8.WebName,
							"7bit",
							""
						});
						using (Stream requestStream2 = httpWebRequest2.GetRequestStream())
						{
							byte[] bytes = Encoding.UTF8.GetBytes(s);
							requestStream2.Write(bytes, 0, bytes.Length);
						}
						using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest2.GetResponse())
						{
							if (httpWebResponse.StatusCode != HttpStatusCode.OK)
							{
								CasHealthErrorPopulatingMailboxException exception2 = new CasHealthErrorPopulatingMailboxException("");
								casTransactionOutcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(exception2));
								this.UpdateOutcomeAndExit(casTransactionOutcome, instance);
								return;
							}
						}
						startTime = ExDateTime.Now;
					}
					else
					{
						instance.createTestItemContext = new CreateTestItemContext(instance.exchangePrincipal, num);
						instance.createTestItemContext.CreateTestItemEvent.Reset();
						this.CreateTestItemInInbox(instance.createTestItemContext);
					}
					context.CurrentState = new ActiveSyncState(httpWebRequest, casTransactionOutcome)
					{
						StartTime = startTime
					};
					IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(this.DoPingCallback), context);
					this.timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(TestMobileSyncConnectivity.TimeoutCallback), httpWebRequest, this.GetDefaultTimeOut() * 1000U, true);
				}
			}
			catch (Exception ex2)
			{
				try
				{
					this.HandleKnownExceptions(ex2, casTransactionOutcome, startTime);
				}
				catch (Exception)
				{
					context.Instance.Result.Complete();
					throw;
				}
				this.UpdateOutcomeAndExit(casTransactionOutcome, instance);
			}
		}

		private void UpdateOutcomeAndExit(CasTransactionOutcome outcome, TestCasConnectivity.TestCasConnectivityRunInstance instance)
		{
			outcome.PerformanceCounterName = "DirectPush Latency";
			outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), outcome.Error);
			if (!instance.Result.DidTimeout())
			{
				instance.Outcomes.Enqueue(outcome);
				instance.Result.Outcomes.Add(outcome);
			}
			instance.Result.Complete();
		}

		internal void DoPingCallback(IAsyncResult result)
		{
			ActiveSyncExecutionContext activeSyncExecutionContext = result.AsyncState as ActiveSyncExecutionContext;
			ActiveSyncState currentState = activeSyncExecutionContext.CurrentState;
			ExDateTime startTime = currentState.StartTime;
			TestCasConnectivity.TestCasConnectivityRunInstance instance = activeSyncExecutionContext.Instance;
			HttpWebRequest request = currentState.Request;
			CasTransactionOutcome outcome = currentState.Outcome;
			if (this.timeoutWaitHandle != null)
			{
				this.timeoutWaitHandle.Unregister(null);
				this.timeoutWaitHandle = null;
			}
			try
			{
				HttpWebResponse httpWebResponse = null;
				try
				{
					httpWebResponse = (HttpWebResponse)request.EndGetResponse(result);
				}
				finally
				{
					if (instance.createTestItemContext != null)
					{
						instance.createTestItemContext.CreateTestItemEvent.WaitOne();
					}
				}
				if (instance.createTestItemContext != null && instance.createTestItemContext.LocalizedException != null)
				{
					outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), base.ShortErrorMsgFromException(instance.createTestItemContext.LocalizedException));
				}
				else
				{
					string text = null;
					CasTransactionResultEnum casTransactionResultEnum = TestMobileSyncConnectivity.CheckWebResponse(request, httpWebResponse, instance.credentials, ref text);
					if (CasTransactionResultEnum.Success != casTransactionResultEnum)
					{
						outcome.Update(casTransactionResultEnum, base.ComputeLatency(startTime), text);
					}
					else
					{
						SafeXmlDocument safeXmlDocument = this.XmlDocFromWbxmlResponseStream(httpWebResponse.GetResponseStream());
						XmlNode xmlNode = safeXmlDocument.DocumentElement;
						string text2 = "Ping";
						if (text2 != xmlNode.Name)
						{
							outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
						}
						else
						{
							XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(safeXmlDocument.NameTable);
							xmlNamespaceManager.AddNamespace("Ping:".TrimEnd(new char[]
							{
								':'
							}), "Ping:");
							string xpath = "/Ping:Ping/Ping:Status";
							xmlNode = safeXmlDocument.SelectSingleNode(xpath, xmlNamespaceManager);
							if (xmlNode == null)
							{
								outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthSyntaxErrorInResponse(text2, safeXmlDocument.InnerXml));
							}
							else if ("2" != xmlNode.InnerText && "7" != xmlNode.InnerText)
							{
								outcome.Update(CasTransactionResultEnum.Failure, base.ComputeLatency(startTime), Strings.CasHealthEasCommandFailedWithError("Ping", xmlNode.InnerText, text));
							}
							else
							{
								base.TraceInfo("Ping succeeded, found changes!");
								outcome.Update(CasTransactionResultEnum.Success, base.ComputeLatency(startTime), text);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					this.HandleKnownExceptions(ex, outcome, startTime);
				}
				catch (Exception)
				{
					activeSyncExecutionContext.Instance.Result.Complete();
					throw;
				}
			}
			finally
			{
				if (instance.createTestItemContext != null && instance.createTestItemContext.CreateTestItemEvent != null)
				{
					instance.createTestItemContext.CreateTestItemEvent.Close();
				}
			}
			if (!activeSyncExecutionContext.Instance.Result.DidTimeout())
			{
				activeSyncExecutionContext.Instance.Outcomes.Enqueue(currentState.Outcome);
				((List<CasTransactionOutcome>)activeSyncExecutionContext.Instance.Result.AsyncState).Add(currentState.Outcome);
			}
			activeSyncExecutionContext.PingLatency = outcome.Latency;
			if (activeSyncExecutionContext.Instance.Result.DidTimeout() || (outcome != null && CasTransactionResultEnum.Success != outcome.Result.Value))
			{
				outcome.PerformanceCounterName = "DirectPush Latency";
				outcome.Update(CasTransactionResultEnum.Failure, TimeSpan.FromMilliseconds(-1000.0), outcome.Error);
				activeSyncExecutionContext.Instance.Result.Complete();
				this.Cleanup(activeSyncExecutionContext);
				return;
			}
			activeSyncExecutionContext.NextStateToExecute = ActiveSyncExecutionState.Last;
			this.DoSyncCommandAsync(activeSyncExecutionContext, Strings.CasHealthEasScenarioSyncTestItem, Strings.CasHealthEasSyncDownTestItem, null, true);
		}

		private void CreateTestItemInInbox(CreateTestItemContext createTestItemContext)
		{
			Thread thread = new Thread(new ParameterizedThreadStart(this.CreateTestItemCallback));
			thread.Start(createTestItemContext);
		}

		private void CreateTestItemCallback(object data)
		{
			CreateTestItemContext createTestItemContext = data as CreateTestItemContext;
			try
			{
				Thread.Sleep(createTestItemContext.SleepTime);
				base.TraceInfo("Creating test item in mailbox...");
				try
				{
					using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(createTestItemContext.ExchangePrincipal, Thread.CurrentThread.CurrentCulture, "Client=Monitoring;Action=Test-ActiveSyncConnectivity"))
					{
						using (Folder folder = Folder.Bind(mailboxSession, DefaultFolderType.Inbox))
						{
							using (MessageItem messageItem = MessageItem.Create(mailboxSession, folder.Id))
							{
								messageItem.Subject = "Exchange ActiveSync TestActiveSyncConnectivity";
								messageItem.Save(SaveMode.NoConflictResolution);
								messageItem.Load();
								createTestItemContext.TestItemId = messageItem.Id;
							}
						}
					}
				}
				catch (LocalizedException exception)
				{
					CasHealthErrorPopulatingMailboxException localizedException = new CasHealthErrorPopulatingMailboxException(base.ShortErrorMsgFromException(exception));
					createTestItemContext.LocalizedException = localizedException;
				}
			}
			finally
			{
				createTestItemContext.CreateTestItemEvent.Set();
			}
		}

		private SafeXmlDocument XmlDocFromWbxmlResponseStream(Stream responseStream)
		{
			int num = 1024;
			byte[] buffer = new byte[num];
			SafeXmlDocument result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				for (;;)
				{
					int num2 = responseStream.Read(buffer, 0, num);
					if (num2 == 0)
					{
						break;
					}
					memoryStream.Write(buffer, 0, num2);
				}
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (WbxmlReader wbxmlReader = new WbxmlReader(memoryStream))
				{
					result = (SafeXmlDocument)wbxmlReader.ReadXmlDocument();
				}
			}
			return result;
		}

		private const string MonitoringDirectPushAndSyncLatencyPerfCounter = "DirectPush Latency";

		private const string MonitoringAutodiscoveryId = "External";

		private const string MonitoringNoAutodiscoveryId = "Internal";

		private const string ActiveSyncUri = "/Microsoft-Server-ActiveSync";

		private const string QueryStringFormat = "?Cmd={0}&User={1}&DeviceId={2}&DeviceType=TestActiveSyncConnectivity";

		private const string WbxmlContentType = "application/vnd.ms-sync.wbxml";

		private const string FolderSyncStatusSuccess = "1";

		private const string PingStatusChangesPending = "2";

		private const string PingStatusFolderSyncRequired = "7";

		private const string SyncStatusSuccess = "1";

		private const string FolderSyncInboxFolderType = "2";

		private const uint EASTimeOut = 120U;

		private const string MimeMessageFormat = "From: {0}\r\nTo: {1}\r\nSubject: {2}\r\nMIME-Version: 1.0\r\nContent-Type: text/plain; Charset= {3}\r\nContent-Transfer-Encoding: {4}\r\nX-MimeOLE: Produced By Microsoft MimeOLE V6.00.2800.1441 \r\n{5}";

		private Uri testUri;

		private RegisteredWaitHandle timeoutWaitHandle;
	}
}
