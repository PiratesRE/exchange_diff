using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net.MonitoringWebClient;
using Microsoft.Exchange.Net.MonitoringWebClient.Rws;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Rws.Probes
{
	public class RwsProbe : WebClientProbeBase
	{
		internal override IExceptionAnalyzer CreateExceptionAnalyzer()
		{
			Dictionary<string, RequestTarget> dictionary = new Dictionary<string, RequestTarget>();
			Uri uri = new Uri(base.Definition.Endpoint);
			dictionary.Add(uri.Host, RequestTarget.Rws);
			return new RwsExceptionAnalyzer(dictionary);
		}

		internal override IResponseTracker CreateResponseTracker()
		{
			this.responseTracker = base.CreateResponseTracker();
			return this.responseTracker;
		}

		internal override void TraceInformation(string message, params object[] parameters)
		{
			this.traceCollector.TraceInformation(ExTraceGlobals.RWSTracer, base.TraceContext, message, parameters);
		}

		internal override void ScenarioSucceeded(Task scenarioTask)
		{
			this.TraceInformation("RWS endpoint call scenario succeeded", new object[0]);
			base.Result.ResultType = ResultType.Succeeded;
		}

		internal override void ScenarioFailed(Task scenarioTask)
		{
			try
			{
				Exception innerException = scenarioTask.Exception.InnerException;
				ScenarioException scenarioException = innerException.GetScenarioException();
				this.TraceInformation("RWS endpoint call scenario failed: {0}{1}{2}", new object[]
				{
					innerException,
					Environment.NewLine,
					scenarioException
				});
				base.Result.ResultType = ResultType.Failed;
				this.PopulateResult(scenarioException, this.responseTracker.Items);
				scenarioTask.Exception.Flatten().Handle((Exception e) => false);
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

		internal override void ScenarioCancelled(Task scenarioTask)
		{
			throw new NotSupportedException("RWSProbe doesn't support cancelling a scenario");
		}

		internal override Task ExecuteScenario(IHttpSession session)
		{
			Uri uri = new Uri(base.Definition.Endpoint);
			RwsAuthenticationInfo authenticationInfo;
			if (uri.Port == 444)
			{
				this.TraceInformation("Creating common access token for brick authentication.", new object[0]);
				CommonAccessToken commonAccessToken = CommonAccessTokenHelper.CreateLiveIdBasic(base.Definition.Account);
				this.TraceInformation("Token: type - LiveIdBasic MemberName - {0}", new object[]
				{
					base.Definition.Account
				});
				authenticationInfo = new RwsAuthenticationInfo(commonAccessToken);
				this.TraceInformation("Created common access token \"{0}\" for brick authentication.", new object[]
				{
					commonAccessToken.ToString()
				});
			}
			else
			{
				authenticationInfo = new RwsAuthenticationInfo(base.Definition.Account, base.Definition.Account.Split(new char[]
				{
					'@'
				})[1], base.Definition.AccountPassword.ConvertToSecureString());
			}
			ITestFactory testFactory = new TestFactory();
			ITestStep testStep = testFactory.CreateRwsCallScenario(uri, authenticationInfo, testFactory);
			testStep.MaxRunTime = new TimeSpan?(RwsProbe.ScenarioTimeout);
			return testStep.CreateTask(session);
		}

		private void PopulateFailingInformation(ScenarioException exception, IEnumerable<ResponseTrackerItem> responseItems, Dictionary<string, string> resultPairs)
		{
			if (exception != null)
			{
				resultPairs.Add("Component", exception.FailingComponent.ToString());
				resultPairs.Add("Reason", exception.FailureReason.ToString());
				resultPairs.Add("Source", exception.FailureSource.ToString());
				base.Result.FailureCategory = (int)exception.FailingComponent;
				base.Result.StateAttribute1 = exception.FailingComponent.ToString();
				base.Result.StateAttribute2 = exception.FailureReason.ToString();
				IEnumerable<ResponseTrackerItem> enumerable = from item in responseItems
				where item.FailingServer != null && item.FailureHttpResponseCode != null
				select item;
				if (enumerable != null && enumerable.Count<ResponseTrackerItem>() > 0)
				{
					ResponseTrackerItem responseTrackerItem = enumerable.First<ResponseTrackerItem>();
					resultPairs.Add("Server", responseTrackerItem.FailingServer);
					resultPairs.Add("HttpStatus", responseTrackerItem.FailureHttpResponseCode.Value.ToString());
					base.Result.FailureContext = responseTrackerItem.FailingServer;
				}
			}
		}

		private void PopulateRequestInformation(IEnumerable<ResponseTrackerItem> responseItems, Dictionary<string, string> resultPairs)
		{
			int num = 0;
			foreach (ResponseTrackerItem responseTrackerItem in responseItems)
			{
				if (responseTrackerItem.TargetType == RequestTarget.Rws.ToString())
				{
					resultPairs.Add(string.Format("{0}", num), string.Format("{0}{1}", responseTrackerItem.TargetHost, responseTrackerItem.PathAndQuery));
				}
				else
				{
					resultPairs.Add(string.Format("{0}", num), string.Format("{0}{1}", responseTrackerItem.TargetHost, responseTrackerItem.PathAndQuery.Split(new char[]
					{
						'?'
					})[0]));
				}
				num++;
			}
		}

		private void PopulateResult(ScenarioException exception, IEnumerable<ResponseTrackerItem> responseItems)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			this.PopulateFailingInformation(exception, responseItems, dictionary);
			this.PopulateRequestInformation(responseItems, dictionary);
			this.PopulateLatencyInformation(exception, responseItems, dictionary);
			this.PopulateServerInformation(responseItems, dictionary);
			base.Result.ExecutionContext = this.TransformToString(dictionary);
		}

		private string GetTargetAbbreviation(RequestTarget target)
		{
			switch (target)
			{
			case RequestTarget.Rws:
				return "RWS";
			case RequestTarget.LiveIdConsumer:
				return "LID_CON";
			case RequestTarget.LiveIdBusiness:
				return "LID_BUS";
			default:
				return "UNK";
			}
		}

		private void PopulateLatencyInformation(ScenarioException exception, IEnumerable<ResponseTrackerItem> responseItems, Dictionary<string, string> resultPairs)
		{
			var enumerable = from item in responseItems
			group item by item.TargetType into g
			select new
			{
				TargetType = g.Key,
				TotalLatency = g.Sum((ResponseTrackerItem item) => item.TotalLatency.TotalMilliseconds)
			};
			base.Result.SampleValue = this.responseTracker.Items.Sum((ResponseTrackerItem i) => i.TotalLatency.TotalMilliseconds);
			if (exception != null && string.Compare(exception.FailureReason.ToString().Trim(), "RequestTimeout", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				base.Result.SampleValue = WebClientProbeBase.RequestTimeout.TotalMilliseconds;
			}
			foreach (var <>f__AnonymousType in enumerable)
			{
				RequestTarget target = (RequestTarget)Enum.Parse(typeof(RequestTarget), <>f__AnonymousType.TargetType);
				resultPairs.Add(this.GetTargetAbbreviation(target) + "_LTCY", <>f__AnonymousType.TotalLatency.ToString());
				switch (target)
				{
				case RequestTarget.Rws:
					base.Result.StateAttribute18 = <>f__AnonymousType.TotalLatency;
					break;
				case RequestTarget.LiveIdConsumer:
					base.Result.StateAttribute16 = <>f__AnonymousType.TotalLatency;
					break;
				case RequestTarget.LiveIdBusiness:
					base.Result.StateAttribute17 = <>f__AnonymousType.TotalLatency;
					break;
				}
			}
		}

		private void PopulateServerInformation(IEnumerable<ResponseTrackerItem> responseItems, Dictionary<string, string> resultPairs)
		{
			IEnumerable<string> list = from item in responseItems
			where item.TargetType == RequestTarget.Rws.ToString() && item.RespondingServer != null
			select item.RespondingServer;
			string stateAttribute = null;
			string stateAttribute2 = null;
			string text = this.AggregateIntoCommaSeparatedUniqueList(list, out stateAttribute, out stateAttribute2);
			string text2 = this.AggregateIntoCommaSeparatedUniqueList(from item in responseItems
			where item.TargetType == RequestTarget.LiveIdConsumer.ToString() && item.RespondingServer != null
			select item.RespondingServer);
			string text3 = this.AggregateIntoCommaSeparatedUniqueList(from item in responseItems
			where item.TargetType == RequestTarget.LiveIdBusiness.ToString() && item.RespondingServer != null
			select item.RespondingServer);
			resultPairs.Add("RWS", text);
			resultPairs.Add("LID_BUS", text3);
			resultPairs.Add("LID_CON", text2);
			base.Result.StateAttribute3 = text;
			base.Result.StateAttribute4 = text2;
			base.Result.StateAttribute5 = text3;
			base.Result.StateAttribute11 = stateAttribute;
			base.Result.StateAttribute12 = stateAttribute2;
		}

		private string AggregateIntoCommaSeparatedUniqueList(IEnumerable<string> list, out string first, out string last)
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

		private string AggregateIntoCommaSeparatedUniqueList(IEnumerable<string> list)
		{
			string text;
			string text2;
			return this.AggregateIntoCommaSeparatedUniqueList(list, out text, out text2);
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

		public const string UserPuidParameterName = "UserPuid";

		public const string UserSidParameterName = "UserSid";

		public const string PartitionIdParameterName = "PartitionId";

		private const int BrickPort = 444;

		private static readonly TimeSpan ScenarioTimeout = TimeSpan.FromSeconds(100.0);

		private TraceCollector traceCollector = new TraceCollector();

		private IResponseTracker responseTracker;
	}
}
