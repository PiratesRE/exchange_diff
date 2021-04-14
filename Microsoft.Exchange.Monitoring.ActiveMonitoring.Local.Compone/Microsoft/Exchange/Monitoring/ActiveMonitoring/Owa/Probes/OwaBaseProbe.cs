using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Probes
{
	public abstract class OwaBaseProbe : ProbeWorkItem
	{
		protected abstract string UserAgent { get; }

		internal virtual SslValidationOptions DefaultSslValidationOptions
		{
			get
			{
				return SslValidationOptions.NoSslValidation;
			}
		}

		internal abstract ITestStep CreateScenario(Uri targetUri);

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (base.Definition.Attributes.ContainsKey("LogFileInstanceName"))
			{
				this.traceCollector = new TraceCollector();
			}
			Uri uri = new Uri(base.Definition.Endpoint);
			this.TraceInformation("Starting Owa probe with Target: {0}, Username: {1} ", new object[]
			{
				uri,
				base.Definition.Account
			});
			IRequestAdapter requestAdapter = new RequestAdapter(cancellationToken, this.GetStaticNameMapping());
			this.responseTracker = new ResponseTracker();
			requestAdapter.RequestTimeout = TimeSpan.FromSeconds((double)int.Parse(base.Definition.Attributes.GetValueOrDefault("RequestTimeout", OwaBaseProbe.DefaultRequestTimeout.TotalSeconds.ToString())));
			Dictionary<string, RequestTarget> dictionary = new Dictionary<string, RequestTarget>();
			dictionary.Add("login.live.com", RequestTarget.LiveIdConsumer);
			dictionary.Add("login.microsoftonline.com", RequestTarget.LiveIdBusiness);
			dictionary.Add("login.partner.microsoftonline.cn", RequestTarget.LiveIdBusiness);
			dictionary.Add("res.outlook.com", RequestTarget.Akamai);
			dictionary.Add("res.partner.outlook.cn", RequestTarget.Akamai);
			dictionary.Add("outlook.com", RequestTarget.Owa);
			dictionary.Add("outlook.office365.com", RequestTarget.Owa);
			dictionary.Add("outlook.cn", RequestTarget.Owa);
			dictionary.Add("mail.live.com", RequestTarget.Hotmail);
			dictionary.Add("hotmail.com", RequestTarget.Hotmail);
			if (!dictionary.ContainsKey(uri.Host))
			{
				dictionary.Add(uri.Host, RequestTarget.Owa);
			}
			if (this.ReadAttribute("IsDedicated", false))
			{
				dictionary.Add(string.Empty, RequestTarget.Owa);
			}
			IExceptionAnalyzer exceptionAnalyzer = new OwaExceptionAnalyzer(dictionary);
			IHttpSession httpSession = new HttpSession(requestAdapter, exceptionAnalyzer, this.responseTracker);
			httpSession.TestStarted += this.TestStepStarted;
			httpSession.TestFinished += this.TestStepFinished;
			httpSession.ResponseReceived += this.ResponseReceived;
			httpSession.SendingRequest += this.RequestSent;
			SslValidationOptions sslValidationOptions;
			if (base.Definition.Attributes.ContainsKey("SslValidationOptions") && Enum.TryParse<SslValidationOptions>(base.Definition.Attributes["SslValidationOptions"], true, out sslValidationOptions))
			{
				httpSession.SslValidationOptions = sslValidationOptions;
			}
			else
			{
				httpSession.SslValidationOptions = this.DefaultSslValidationOptions;
			}
			httpSession.UserAgent = this.UserAgent;
			ITestStep testStep = this.CreateScenario(uri);
			testStep.MaxRunTime = new TimeSpan?(TimeSpan.FromSeconds((double)int.Parse(base.Definition.Attributes.GetValueOrDefault("ScenarioTimeout", OwaBaseProbe.DefaultScenarioTimeout.TotalSeconds.ToString()))));
			this.acceptPassiveCopyErrorAsSuccess = bool.Parse(base.Definition.Attributes.GetValueOrDefault("AcceptPassiveCopyErrorAsSuccess", false.ToString()));
			Task task = testStep.CreateTask(httpSession);
			task.ContinueWith(new Action<Task>(this.TestScenarioSucceeded), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
			task.ContinueWith(new Action<Task>(this.TestScenarioFailed), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled);
		}

		internal virtual void PopulateResult(ScenarioException exception, IEnumerable<ResponseTrackerItem> responseItems)
		{
			IEnumerable<string> list = from item in responseItems
			where item.TargetType == RequestTarget.Owa.ToString() && item.RespondingServer != null
			select item.RespondingServer;
			string stateAttribute = null;
			string stateAttribute2 = null;
			string text = this.AggregateIntoCommaSeparatedList(list, out stateAttribute, out stateAttribute2);
			string value = this.AggregateIntoCommaSeparatedList(from item in responseItems
			where item.TargetType == RequestTarget.LiveIdConsumer.ToString() && item.RespondingServer != null
			select item.RespondingServer);
			string text2 = this.AggregateIntoCommaSeparatedList(from item in responseItems
			where item.TargetType == RequestTarget.LiveIdBusiness.ToString() && item.RespondingServer != null
			select item.RespondingServer);
			string value2 = this.AggregateIntoCommaSeparatedList(from item in responseItems
			where item.TargetType == RequestTarget.Hotmail.ToString() && item.RespondingServer != null
			select item.RespondingServer);
			IEnumerable<string> enumerable = from item in responseItems
			where item.MailboxServer != null
			select item.MailboxServer;
			string stateAttribute3 = enumerable.LastOrDefault<string>();
			string text3 = this.AggregateIntoCommaSeparatedList(enumerable);
			var source = from item in responseItems
			where item.TargetType == RequestTarget.Owa.ToString() && item.StepId != TestId.MeasureClientLatency.ToString()
			group item by 1 into g
			select new
			{
				TotalLatency = g.Sum((ResponseTrackerItem item) => item.TotalLatency.TotalMilliseconds),
				LdapLatency = g.Sum(delegate(ResponseTrackerItem item)
				{
					if (item.LdapLatency == null)
					{
						return 0.0;
					}
					return item.LdapLatency.Value.TotalMilliseconds;
				}),
				MbxLatency = g.Sum(delegate(ResponseTrackerItem item)
				{
					if (item.RpcLatency == null)
					{
						return 0.0;
					}
					return item.RpcLatency.Value.TotalMilliseconds;
				}),
				MservLatency = g.Sum(delegate(ResponseTrackerItem item)
				{
					if (item.MservLatency == null)
					{
						return 0.0;
					}
					return item.MservLatency.Value.TotalMilliseconds;
				}),
				CasLatency = g.Sum(delegate(ResponseTrackerItem item)
				{
					if (item.CasLatency == null)
					{
						return 0.0;
					}
					return item.CasLatency.Value.TotalMilliseconds;
				})
			};
			var enumerable2 = from item in responseItems
			group item by item.TargetType into g
			select new
			{
				TargetType = g.Key,
				TotalLatency = g.Sum((ResponseTrackerItem item) => item.TotalLatency.TotalMilliseconds)
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("User", base.Definition.Account);
			dictionary.Add("Pwd", this.ReadAttribute("HidePasswordInLog", true) ? "******" : base.Definition.AccountPassword);
			if (exception != null)
			{
				dictionary.Add("Component", exception.FailingComponent.ToString());
				dictionary.Add("Reason", exception.FailureReason.ToString());
				dictionary.Add("Source", exception.FailureSource.ToString());
				base.Result.FailureCategory = (int)exception.FailingComponent;
				base.Result.StateAttribute1 = exception.FailingComponent.ToString();
				base.Result.StateAttribute2 = exception.FailureReason.ToString();
				base.Result.StateAttribute22 = exception.ExceptionHint;
				IEnumerable<ResponseTrackerItem> enumerable3 = from item in responseItems
				where item.FailingTargetHostname != null
				select item;
				if (enumerable3 != null && enumerable3.Count<ResponseTrackerItem>() > 0)
				{
					ResponseTrackerItem responseTrackerItem = enumerable3.First<ResponseTrackerItem>();
					dictionary.Add("FailingServer", responseTrackerItem.FailingServer);
					dictionary.Add("FailingCode", (responseTrackerItem.FailureHttpResponseCode != null) ? responseTrackerItem.FailureHttpResponseCode.Value.ToString() : "null");
					base.Result.StateAttribute4 = responseTrackerItem.FailingServer;
					base.Result.StateAttribute15 = responseTrackerItem.FailingTargetHostname;
					base.Result.StateAttribute21 = responseTrackerItem.FailingTargetIPAddress;
				}
			}
			dictionary.Add("CAS", text);
			dictionary.Add("LID_BUS", text2);
			dictionary.Add("LID_CON", value);
			dictionary.Add("HOTML", value2);
			dictionary.Add("MBX", text3);
			double stateAttribute4 = 0.0;
			double stateAttribute5 = 0.0;
			double stateAttribute6 = 0.0;
			double stateAttribute7 = 0.0;
			double stateAttribute8 = 0.0;
			var <>f__AnonymousType = source.FirstOrDefault(item => true);
			if (<>f__AnonymousType != null)
			{
				stateAttribute4 = <>f__AnonymousType.CasLatency - <>f__AnonymousType.MbxLatency - <>f__AnonymousType.LdapLatency - <>f__AnonymousType.MservLatency;
				stateAttribute5 = <>f__AnonymousType.TotalLatency - <>f__AnonymousType.CasLatency;
				stateAttribute6 = <>f__AnonymousType.MbxLatency;
				stateAttribute7 = <>f__AnonymousType.LdapLatency;
				stateAttribute8 = <>f__AnonymousType.MservLatency;
				dictionary.Add("MBX_LTCY", stateAttribute6.ToString());
				dictionary.Add("DC_LTCY", stateAttribute7.ToString());
				dictionary.Add("MSERV_LTCY", stateAttribute8.ToString());
				dictionary.Add("OWA_LTCY", stateAttribute4.ToString());
				dictionary.Add("NTW_LTCY", stateAttribute5.ToString());
			}
			foreach (var <>f__AnonymousType2 in enumerable2)
			{
				RequestTarget target = (RequestTarget)Enum.Parse(typeof(RequestTarget), <>f__AnonymousType2.TargetType);
				string key = this.GetTargetAbbreviation(target) + "_LTCY";
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, <>f__AnonymousType2.TotalLatency.ToString());
				}
				else
				{
					dictionary[key] = (double.Parse(dictionary[key]) + <>f__AnonymousType2.TotalLatency).ToString();
				}
				switch (target)
				{
				case RequestTarget.Owa:
					base.Result.StateAttribute18 = <>f__AnonymousType2.TotalLatency;
					break;
				case RequestTarget.LocalClient:
					base.Result.StateAttribute20 = <>f__AnonymousType2.TotalLatency;
					break;
				case RequestTarget.Akamai:
					base.Result.StateAttribute18 = <>f__AnonymousType2.TotalLatency;
					break;
				case RequestTarget.LiveIdBusiness:
					base.Result.StateAttribute17 = <>f__AnonymousType2.TotalLatency;
					break;
				case RequestTarget.Hotmail:
					base.Result.StateAttribute19 = <>f__AnonymousType2.TotalLatency;
					break;
				}
			}
			base.Result.ExecutionContext = this.TransformToString(dictionary);
			base.Result.StateAttribute3 = text;
			base.Result.StateAttribute5 = text2;
			base.Result.StateAttribute6 = stateAttribute4;
			base.Result.StateAttribute7 = stateAttribute5;
			base.Result.StateAttribute8 = stateAttribute6;
			base.Result.StateAttribute9 = stateAttribute7;
			base.Result.StateAttribute10 = stateAttribute8;
			base.Result.StateAttribute11 = stateAttribute;
			base.Result.StateAttribute12 = stateAttribute2;
			base.Result.StateAttribute13 = stateAttribute3;
			base.Result.StateAttribute14 = text3;
			base.Result.StateAttribute16 = responseItems.Sum((ResponseTrackerItem i) => i.TotalLatency.TotalMilliseconds);
			base.Result.StateAttribute23 = Utils.GetAccountForest(base.Definition.Account);
			if (this.responseTracker != null)
			{
				base.Result.FailureContext = (this.responseTracker as ResponseTracker).GetActivitySummary(true);
			}
		}

		internal virtual Dictionary<string, List<NamedVip>> GetStaticNameMapping()
		{
			return null;
		}

		private void TestScenarioSucceeded(Task scenarioTask)
		{
			this.TraceInformation("OWA logon scenario succeeded", new object[0]);
			double sampleValue = this.responseTracker.Items.Sum(delegate(ResponseTrackerItem item)
			{
				if (string.IsNullOrEmpty(item.StepId) || !item.StepId.Equals("OwaStartPage", StringComparison.OrdinalIgnoreCase))
				{
					return 0.0;
				}
				return item.TotalLatency.TotalMilliseconds;
			});
			base.Result.SampleValue = sampleValue;
			this.PopulateResult(null, this.responseTracker.Items);
		}

		private void TestScenarioFailed(Task scenarioTask)
		{
			try
			{
				Exception innerException = scenarioTask.Exception.InnerException;
				ScenarioException scenarioException = innerException.GetScenarioException();
				if (this.acceptPassiveCopyErrorAsSuccess && scenarioException != null && scenarioException.FailureReason == FailureReason.PassiveDatabase)
				{
					this.TraceInformation("OWA logon scenario failed for a passive copy error and the probe was configured to accept it as success: {0}{1}{2}", new object[]
					{
						innerException,
						Environment.NewLine,
						scenarioException
					});
					this.TestScenarioSucceeded(scenarioTask);
					base.Result.StateAttribute25 = "PassiveDatabase";
					scenarioTask.Exception.Flatten().Handle((Exception e) => true);
				}
				else
				{
					this.TraceInformation("OWA logon scenario failed: {0}{1}{2}", new object[]
					{
						innerException,
						Environment.NewLine,
						scenarioException
					});
					this.PopulateResult(scenarioException, this.responseTracker.Items);
					base.Result.SampleValue = base.Result.StateAttribute16;
					this.FlushTraces();
					scenarioTask.Exception.Flatten().Handle((Exception e) => false);
				}
			}
			catch (Exception ex)
			{
				this.TraceInformation("Exception thrown on TestScenarioFailed function: {0}", new object[]
				{
					ex
				});
				throw;
			}
		}

		private void FlushTraces()
		{
			if (this.traceCollector != null)
			{
				string text = "OWA\\" + base.Definition.Attributes["LogFileInstanceName"];
				MonitoringLogConfiguration logConfiguration = new MonitoringLogConfiguration(text);
				this.traceCollector.FlushToFile(text, logConfiguration);
			}
		}

		private void TestStepStarted(object sender, TestEventArgs eventArgs)
		{
			this.TraceInformation("Test step started: {0}", new object[]
			{
				eventArgs.TestId
			});
		}

		private void TestStepFinished(object sender, TestEventArgs eventArgs)
		{
			this.TraceInformation("Test step finished: {0}", new object[]
			{
				eventArgs.TestId
			});
		}

		private void RequestSent(object sender, HttpWebEventArgs eventArgs)
		{
			this.TraceInformation("Request being sent:{1}{0}", new object[]
			{
				eventArgs.Request.ToStringNoBody(),
				Environment.NewLine
			});
		}

		private void ResponseReceived(object sender, HttpWebEventArgs eventArgs)
		{
			this.TraceInformation("Response received:{1}{0}", new object[]
			{
				eventArgs.Response.ToStringNoBody(),
				Environment.NewLine
			});
		}

		private void TraceInformation(string message, params object[] parameters)
		{
			if (this.traceCollector == null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format(message, parameters), null, "TraceInformation", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\OwaBaseProbe.cs", 535);
				return;
			}
			this.traceCollector.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, string.Format(message, parameters), new object[0]);
		}

		private string GetTargetAbbreviation(RequestTarget target)
		{
			switch (target)
			{
			case RequestTarget.Owa:
				return "CAS";
			case RequestTarget.Akamai:
				return "AKMAI";
			case RequestTarget.LiveIdConsumer:
				return "LID_CON";
			case RequestTarget.LiveIdBusiness:
				return "LID_BUS";
			case RequestTarget.Hotmail:
				return "HOTML";
			}
			return "UNK";
		}

		private string AggregateIntoCommaSeparatedList(IEnumerable<string> list, out string first, out string last)
		{
			string text;
			last = (text = null);
			first = text;
			if (list == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string text2 in list)
			{
				if (!hashSet.Contains(text2))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(text2);
					last = text2;
					if (first == null)
					{
						first = text2;
					}
					hashSet.Add(text2);
				}
			}
			return stringBuilder.ToString();
		}

		private string AggregateIntoCommaSeparatedList(IEnumerable<string> list)
		{
			string text;
			string text2;
			return this.AggregateIntoCommaSeparatedList(list, out text, out text2);
		}

		private string TransformToString(Dictionary<string, string> keyPairs)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string text in keyPairs.Keys)
			{
				if (!string.IsNullOrEmpty(keyPairs[text]))
				{
					if (num > 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.AppendFormat("{0}={1}", text, keyPairs[text]);
					num++;
				}
			}
			return stringBuilder.ToString();
		}

		public const string SslValidationOptionsParameterName = "SslValidationOptions";

		public const string LogFileInstanceParameterName = "LogFileInstanceName";

		public const string ScenarioTimeoutParameterName = "ScenarioTimeout";

		public const string RequestTimeoutParameterName = "RequestTimeout";

		public const string AcceptPassiveCopyErrorAsSuccessParameterName = "AcceptPassiveCopyErrorAsSuccess";

		public const string IsDedicatedParameterName = "IsDedicated";

		private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(120.0);

		private static readonly TimeSpan DefaultScenarioTimeout = TimeSpan.FromSeconds(180.0);

		private IResponseTracker responseTracker;

		private TraceCollector traceCollector;

		private bool acceptPassiveCopyErrorAsSuccess;
	}
}
