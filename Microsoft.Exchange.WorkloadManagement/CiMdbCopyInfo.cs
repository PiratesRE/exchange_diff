using System;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class CiMdbCopyInfo
	{
		public CiMdbCopyInfo(string server, bool mounted, int metric)
		{
			this.Server = server;
			this.IsAvailable = true;
			this.Mounted = mounted;
			this.Metric = metric;
			this.Used = false;
		}

		public CiMdbCopyInfo(string server)
		{
			this.Server = server;
			this.IsAvailable = false;
		}

		public string Server { get; private set; }

		public bool IsAvailable { get; private set; }

		public bool Mounted { get; private set; }

		public int Metric { get; private set; }

		public bool Used { get; internal set; }

		public override string ToString()
		{
			if (this.IsAvailable)
			{
				return string.Concat(new object[]
				{
					this.Server,
					",",
					this.Mounted ? "Mounted" : string.Empty,
					",",
					this.Metric,
					",",
					this.Used ? "Used" : string.Empty
				});
			}
			return this.Server + ",NoStatus";
		}
	}
}
