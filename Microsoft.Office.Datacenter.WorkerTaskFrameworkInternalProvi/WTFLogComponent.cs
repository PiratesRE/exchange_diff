using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class WTFLogComponent
	{
		public WTFLogComponent(Guid category, int logTag, string name, bool traceLogEnabled)
		{
			this.category = category;
			this.logTag = logTag;
			this.name = name;
			this.traceLogEnabled = traceLogEnabled;
		}

		public Guid Category
		{
			get
			{
				return this.category;
			}
		}

		public int LogTag
		{
			get
			{
				return this.logTag;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsTraceLoggingEnabled
		{
			get
			{
				return this.traceLogEnabled;
			}
		}

		private readonly Guid category;

		private readonly int logTag;

		private readonly string name;

		private readonly bool traceLogEnabled;
	}
}
