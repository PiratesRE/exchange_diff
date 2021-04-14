using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class TaskIOPipeline : ITaskIOPipeline
	{
		internal TaskIOPipeline(TaskContext context)
		{
			this.context = context;
		}

		internal void PrependTaskIOPipelineHandler(ITaskIOPipeline pipeline)
		{
			this.pipelines.Insert(0, pipeline);
		}

		public bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			return this.ExecutePipeline<LocalizedString>(delegate(ITaskIOPipeline p, LocalizedString i, out LocalizedString o)
			{
				return p.WriteVerbose(i, out o);
			}, "WriteVerbose", input, out output);
		}

		public bool WriteDebug(LocalizedString input, out LocalizedString output)
		{
			return this.ExecutePipeline<LocalizedString>(delegate(ITaskIOPipeline p, LocalizedString i, out LocalizedString o)
			{
				return p.WriteDebug(i, out o);
			}, "WriteDebug", input, out output);
		}

		public bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			return this.ExecutePipeline<LocalizedString>(delegate(ITaskIOPipeline p, LocalizedString i, out LocalizedString o)
			{
				return p.WriteWarning(i, helperUrl, out o);
			}, "WriteWarning", input, out output);
		}

		public bool WriteError(TaskErrorInfo input, out TaskErrorInfo output)
		{
			return this.ExecutePipeline<TaskErrorInfo>(delegate(ITaskIOPipeline p, TaskErrorInfo i, out TaskErrorInfo o)
			{
				return p.WriteError(i, out o);
			}, "WriteError", input, out output);
		}

		public bool WriteObject(object input, out object output)
		{
			return this.ExecutePipeline<object>(delegate(ITaskIOPipeline p, object i, out object o)
			{
				return p.WriteObject(i, out o);
			}, "WriteObject", input, out output);
		}

		public bool WriteProgress(ExProgressRecord input, out ExProgressRecord output)
		{
			return this.ExecutePipeline<ExProgressRecord>(delegate(ITaskIOPipeline p, ExProgressRecord i, out ExProgressRecord o)
			{
				return p.WriteProgress(i, out o);
			}, "WriteProgress", input, out output);
		}

		public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll, out bool? output)
		{
			bool result = true;
			output = null;
			using (List<ITaskIOPipeline>.Enumerator enumerator = this.pipelines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ITaskIOPipeline pipeline = enumerator.Current;
					string text = base.GetType().Name + ".ShouldContinue";
					using (new CmdletMonitoredScope(this.context.UniqueId, "TaskModuleLatency", text, LoggerHelper.CmdletPerfMonitors))
					{
						ICriticalFeature feature = pipeline as ICriticalFeature;
						bool yesToAllFromDelegate = yesToAll;
						bool noToAllFromDelegate = noToAll;
						bool? outputFromDelegate = output;
						feature.Execute(delegate
						{
							result = pipeline.ShouldContinue(query, caption, ref yesToAllFromDelegate, ref noToAllFromDelegate, out outputFromDelegate);
						}, this.context, text);
						yesToAll = yesToAllFromDelegate;
						noToAll = noToAllFromDelegate;
						output = outputFromDelegate;
					}
					if (!result)
					{
						break;
					}
				}
			}
			return result;
		}

		public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out bool? output)
		{
			return this.ExecutePipeline<bool?>(delegate(ITaskIOPipeline p, bool? i, out bool? o)
			{
				return p.ShouldProcess(verboseDescription, verboseWarning, caption, out o);
			}, "ShouldProcess", null, out output);
		}

		private bool ExecutePipeline<T>(TaskIOPipeline.PipelineExecuter<T> pipelineExecuter, string methodName, T input, out T output)
		{
			bool result = true;
			output = input;
			using (List<ITaskIOPipeline>.Enumerator enumerator = this.pipelines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TaskIOPipeline.<>c__DisplayClass1a<T> CS$<>8__locals2 = new TaskIOPipeline.<>c__DisplayClass1a<T>();
					CS$<>8__locals2.pipeline = enumerator.Current;
					ICriticalFeature feature = CS$<>8__locals2.pipeline as ICriticalFeature;
					T outputFromExecuter = input;
					string text = base.GetType().Name + "." + methodName;
					using (new CmdletMonitoredScope(this.context.UniqueId, "TaskModuleLatency", text, LoggerHelper.CmdletPerfMonitors))
					{
						feature.Execute(delegate
						{
							result = pipelineExecuter(CS$<>8__locals2.pipeline, input, out outputFromExecuter);
						}, this.context, text);
					}
					output = outputFromExecuter;
					if (!result)
					{
						break;
					}
					input = output;
				}
			}
			return result;
		}

		private readonly List<ITaskIOPipeline> pipelines = new List<ITaskIOPipeline>();

		private readonly TaskContext context;

		private delegate bool PipelineExecuter<T>(ITaskIOPipeline pipeline, T input, out T output);
	}
}
