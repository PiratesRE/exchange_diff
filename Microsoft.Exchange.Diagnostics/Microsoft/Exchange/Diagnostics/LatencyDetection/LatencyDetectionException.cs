using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	public class LatencyDetectionException : Exception
	{
		internal LatencyDetectionException(LatencyDetectionContext trigger) : this(trigger.Location.Identity, trigger.StackTraceContext.ToString(), trigger.Elapsed, trigger.Latencies, string.Empty)
		{
		}

		internal LatencyDetectionException(LatencyDetectionContext trigger, IPerformanceDataProvider provider) : this(trigger.Location.Identity, trigger.StackTraceContext.ToString(), trigger.Elapsed, trigger.Latencies, provider.Name)
		{
		}

		private LatencyDetectionException(string context, string stackTraceContext, TimeSpan total, ICollection<LabeledTimeSpan> times, string nameOfDataProvider)
		{
			this.context = context;
			this.total = total;
			this.times = times;
			Type type = base.GetType();
			this.watsonExceptionName = ((!string.IsNullOrEmpty(nameOfDataProvider)) ? (type.Namespace + "." + nameOfDataProvider + type.Name) : type.FullName);
			this.stackTrace = new StackTrace(3, true);
			if (Uri.IsWellFormedUriString(stackTraceContext, UriKind.RelativeOrAbsolute))
			{
				this.watsonMethodName = stackTraceContext;
				return;
			}
			this.SetWatsonMethodNameFromStackTrace();
		}

		public override string StackTrace
		{
			get
			{
				return this.stackTrace.ToString();
			}
		}

		public override string Message
		{
			get
			{
				if (string.IsNullOrEmpty(this.message))
				{
					int capacity = "High latency seen in \"".Length + this.context.Length + 16 + this.times.Count * 26;
					StringBuilder stringBuilder = new StringBuilder(capacity);
					stringBuilder.Append("High latency seen in \"").Append(this.context).Append("\" context. Total: ").Append(this.total);
					foreach (LabeledTimeSpan labeledTimeSpan in this.times)
					{
						stringBuilder.Append("; ").Append(labeledTimeSpan.Label).Append(": ").Append(labeledTimeSpan.TimeSpan);
					}
					this.message = stringBuilder.ToString();
				}
				return this.message;
			}
		}

		internal string WatsonExceptionName
		{
			get
			{
				return this.watsonExceptionName;
			}
		}

		internal string WatsonMethodName
		{
			get
			{
				return this.watsonMethodName;
			}
		}

		private void SetWatsonMethodNameFromStackTrace()
		{
			int num = 0;
			string arg = string.Empty;
			string arg2 = string.Empty;
			StackFrame frame;
			MethodBase method;
			for (;;)
			{
				frame = this.stackTrace.GetFrame(num);
				method = frame.GetMethod();
				string name = method.Name;
				if (name.IndexOf("log", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					break;
				}
				num++;
				if (num >= this.stackTrace.FrameCount)
				{
					return;
				}
			}
			num += 2;
			frame = this.stackTrace.GetFrame(num);
			method = frame.GetMethod();
			arg = method.Name;
			arg2 = WatsonReport.GetShortParameter(method.DeclaringType.FullName);
			this.watsonMethodName = arg2 + '.' + arg;
		}

		private const string MessagePart1 = "High latency seen in \"";

		private const string MessagePart2 = "\" context. Total: ";

		private readonly string watsonExceptionName;

		private readonly StackTrace stackTrace;

		private readonly ICollection<LabeledTimeSpan> times;

		private readonly string context;

		private readonly TimeSpan total;

		private string watsonMethodName;

		private string message = string.Empty;
	}
}
