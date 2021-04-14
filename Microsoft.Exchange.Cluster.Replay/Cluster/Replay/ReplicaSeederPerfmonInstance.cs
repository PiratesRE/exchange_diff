using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class ReplicaSeederPerfmonInstance : PerformanceCounterInstance
	{
		internal ReplicaSeederPerfmonInstance(string instanceName, ReplicaSeederPerfmonInstance autoUpdateTotalInstance) : base(instanceName, "MSExchange Replica Seeder")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DbSeedingProgress = new ExPerformanceCounter(base.CategoryName, "Database Seeding Progress %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingProgress);
				this.DbSeedingBytesRead = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Read (KB)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesRead);
				this.DbSeedingBytesReadPerSecond = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Read (KB/sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesReadPerSecond);
				this.DbSeedingBytesWritten = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Written (KB)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesWritten);
				this.DbSeedingBytesWrittenPerSecond = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Written (KB/sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesWrittenPerSecond);
				this.CiSeedingInProgress = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding In Progress", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingInProgress);
				this.CiSeedingPercent = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Progress %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingPercent);
				this.CiSeedingSuccesses = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Successes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingSuccesses);
				this.CiSeedingFailures = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingFailures);
				long num = this.DbSeedingProgress.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal ReplicaSeederPerfmonInstance(string instanceName) : base(instanceName, "MSExchange Replica Seeder")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.DbSeedingProgress = new ExPerformanceCounter(base.CategoryName, "Database Seeding Progress %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingProgress);
				this.DbSeedingBytesRead = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Read (KB)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesRead);
				this.DbSeedingBytesReadPerSecond = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Read (KB/sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesReadPerSecond);
				this.DbSeedingBytesWritten = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Written (KB)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesWritten);
				this.DbSeedingBytesWrittenPerSecond = new ExPerformanceCounter(base.CategoryName, "Database Seeding Bytes Written (KB/sec)", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DbSeedingBytesWrittenPerSecond);
				this.CiSeedingInProgress = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding In Progress", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingInProgress);
				this.CiSeedingPercent = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Progress %", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingPercent);
				this.CiSeedingSuccesses = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Successes", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingSuccesses);
				this.CiSeedingFailures = new ExPerformanceCounter(base.CategoryName, "Catalog Seeding Failures", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CiSeedingFailures);
				long num = this.DbSeedingProgress.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter DbSeedingProgress;

		public readonly ExPerformanceCounter DbSeedingBytesRead;

		public readonly ExPerformanceCounter DbSeedingBytesReadPerSecond;

		public readonly ExPerformanceCounter DbSeedingBytesWritten;

		public readonly ExPerformanceCounter DbSeedingBytesWrittenPerSecond;

		public readonly ExPerformanceCounter CiSeedingInProgress;

		public readonly ExPerformanceCounter CiSeedingPercent;

		public readonly ExPerformanceCounter CiSeedingSuccesses;

		public readonly ExPerformanceCounter CiSeedingFailures;
	}
}
