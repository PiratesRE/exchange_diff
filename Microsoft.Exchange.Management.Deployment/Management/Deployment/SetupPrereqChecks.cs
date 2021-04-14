using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Analysis;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Setup.Common;

namespace Microsoft.Exchange.Management.Deployment
{
	public class SetupPrereqChecks
	{
		public SetupPrereqChecks() : this(SetupMode.All, SetupRole.All, null)
		{
		}

		public SetupPrereqChecks(SetupMode mode, SetupRole role, GlobalParameters globalParameters)
		{
			this.mode = mode;
			this.role = role;
			this.globalParameters = globalParameters;
			this.DataProviderFactory = new DataProviderFactory();
		}

		internal IDataProviderFactory DataProviderFactory { get; set; }

		public void DoCheckPrereqs(Action<int> writeProgress, Microsoft.Exchange.Configuration.Tasks.Task task)
		{
			PrereqAnalysis prereqAnalysis = new PrereqAnalysis(this.DataProviderFactory, this.mode, this.role, this.globalParameters);
			this.DoAnalysis(writeProgress, prereqAnalysis);
			try
			{
				SetupLogger.IsPrereqLogging = true;
				foreach (Result result in prereqAnalysis.Conclusions)
				{
					RuleResult ruleResult = (RuleResult)result;
					if (!ruleResult.HasException && ruleResult.Value)
					{
						Rule rule = (Rule)ruleResult.Source;
						string text = string.Empty;
						MessageFeature messageFeature = (MessageFeature)rule.Features.First((Feature x) => x.GetType() == typeof(MessageFeature));
						if (messageFeature != null)
						{
							text = messageFeature.Text(ruleResult);
						}
						RuleTypeFeature ruleTypeFeature = (RuleTypeFeature)rule.Features.FirstOrDefault((Feature x) => x.GetType() == typeof(RuleTypeFeature));
						if (ruleTypeFeature != null)
						{
							RuleType ruleType = ruleTypeFeature.RuleType;
							LocalizedString text2 = new LocalizedString(text);
							string helpUrl = ruleResult.GetHelpUrl();
							if (ruleType == RuleType.Error)
							{
								task.WriteError(new Exception(text), ErrorCategory.NotSpecified, rule.Name, false, helpUrl);
							}
							else if (ruleType == RuleType.Warning)
							{
								task.WriteWarning(text2, helpUrl);
							}
							else
							{
								task.WriteVerbose(text2);
							}
						}
					}
				}
			}
			finally
			{
				SetupLogger.IsPrereqLogging = false;
			}
		}

		internal void DoAnalysis(Action<int> writeProgress, Analysis analysis)
		{
			BlockingCollection<int> progressCollection = new BlockingCollection<int>();
			writeProgress(1);
			analysis.ProgressUpdated += delegate(object sender, ProgressUpdateEventArgs arg)
			{
				try
				{
					int num = Math.Max(1, arg.CompletedPercentage);
					progressCollection.Add(num);
					if (num == 100)
					{
						progressCollection.CompleteAdding();
					}
				}
				catch (InvalidOperationException)
				{
				}
			};
			System.Threading.Tasks.Task.Factory.StartNew(delegate()
			{
				analysis.StartAnalysis();
			});
			while (!progressCollection.IsCompleted)
			{
				try
				{
					writeProgress(progressCollection.Take());
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		internal static Process GetParentProcess()
		{
			int num = 0;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				num = currentProcess.Id;
			}
			Process result = null;
			string queryString = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", num);
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", queryString))
			{
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher.Get().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ManagementBaseObject managementBaseObject = enumerator.Current;
						uint processId = (uint)managementBaseObject["ParentProcessId"];
						result = Process.GetProcessById((int)processId);
					}
				}
			}
			return result;
		}

		private const int ProgressStarted = 1;

		private const int ProgressCompleted = 100;

		private readonly SetupMode mode;

		private readonly SetupRole role;

		private GlobalParameters globalParameters;
	}
}
