using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class Breadcrumbs
	{
		public Breadcrumbs(int size, TracingContext tracingContext) : this(size)
		{
			this.trace = tracingContext;
			this.isTraceInitialized = true;
		}

		public Breadcrumbs(int size)
		{
			size = Math.Max(size, 512);
			this.size = size;
			this.Delimiter = "\r\n";
			this.InsertTimestamp = true;
			this.InsertThreadId = false;
			this.UseSmartTruncation = true;
			this.sb = new StringBuilder(size / 2);
			this.timeStarted = DateTime.UtcNow;
			this.isTraceInitialized = false;
		}

		public Breadcrumbs(TracingContext tracingContext) : this(int.MaxValue)
		{
			this.trace = tracingContext;
			this.isTraceInitialized = true;
		}

		public string Delimiter { get; set; }

		public bool InsertThreadId { get; set; }

		public bool InsertTimestamp { get; set; }

		public bool UseSmartTruncation { get; set; }

		public TimeSpan ElapsedTime
		{
			get
			{
				return DateTime.UtcNow - this.timeStarted;
			}
		}

		public void Drop(string s)
		{
			bool flag = this.sb.Length > this.size;
			if (this.InsertTimestamp)
			{
				double totalSeconds = this.ElapsedTime.TotalSeconds;
				this.sb.AppendFormat("[{0:000.000}] ", totalSeconds);
			}
			if (this.InsertThreadId)
			{
				int managedThreadId = Thread.CurrentThread.ManagedThreadId;
				this.sb.AppendFormat("[{0:X8}] ", managedThreadId);
			}
			this.sb.Append(s + this.Delimiter);
			if (!flag && this.sb.Length > this.size && this.isTraceInitialized)
			{
				WTFDiagnostics.TraceWarning<int>(ExTraceGlobals.CommonComponentsTracer, this.trace, "Breadcrumbs.Drop: exceeded defined limit of {0} characters.", this.size, null, "Drop", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\Breadcrumbs.cs", 175);
			}
		}

		public void Drop(string formatString, params object[] args)
		{
			string s = string.Format(formatString, args);
			this.Drop(s);
		}

		public void Clear()
		{
			this.sb.Clear();
		}

		public override string ToString()
		{
			if (this.sb.Length <= this.size)
			{
				return this.sb.ToString();
			}
			if (!this.UseSmartTruncation)
			{
				return this.sb.ToString(0, this.size);
			}
			string text = this.sb.ToString();
			int num = text.IndexOf(this.Delimiter);
			if (num == -1)
			{
				return text.Substring(0, this.size);
			}
			num += this.Delimiter.Length;
			int num2 = text.Length - this.size + "...TRUNCATED...".Length;
			if (num2 < 0)
			{
				return text.Substring(text.Length - this.size, this.size);
			}
			int startIndex = num + num2;
			return text.Substring(0, num) + "...TRUNCATED..." + text.Substring(startIndex);
		}

		public const string DefaultDelimiter = "\r\n";

		public const string DelimiterForActiveMonitoring = " ++ ";

		private const string Truncated = "...TRUNCATED...";

		private readonly int size;

		private readonly DateTime timeStarted;

		private readonly bool isTraceInitialized;

		private StringBuilder sb;

		private TracingContext trace;
	}
}
