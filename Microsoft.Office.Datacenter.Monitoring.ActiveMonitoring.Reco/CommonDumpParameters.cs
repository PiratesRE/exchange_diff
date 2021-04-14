using System;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public class CommonDumpParameters : ICloneable
	{
		public CommonDumpParameters()
		{
			this.IgnoreRegistryOverride = false;
		}

		public DumpMode Mode { get; set; }

		public string Path { get; set; }

		public double MinimumFreeSpace { get; set; }

		public int MaximumDurationInSeconds { get; set; }

		public bool IgnoreRegistryOverride { get; set; }

		public bool IsDumpRequested
		{
			get
			{
				return this.Mode != DumpMode.None;
			}
		}

		public CommonDumpParameters Clone()
		{
			return (CommonDumpParameters)base.MemberwiseClone();
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
