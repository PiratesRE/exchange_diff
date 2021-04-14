using System;

namespace Microsoft.Exchange.Diagnostics
{
	public sealed class PerformanceCounterMultipleInstanceWithAutoUpdateTotal : PerformanceCounterMultipleInstance
	{
		public PerformanceCounterMultipleInstanceWithAutoUpdateTotal(string categoryName, CreateInstanceDelegate instanceCreator) : base(categoryName, instanceCreator)
		{
			this.totalInstance = instanceCreator("_Total", null);
		}

		public PerformanceCounterMultipleInstanceWithAutoUpdateTotal(string categoryName, CreateInstanceDelegate instanceCreator, CreateTotalInstanceDelegate totalInstanceCreator) : base(categoryName, instanceCreator)
		{
			this.totalInstance = totalInstanceCreator("_Total");
		}

		public PerformanceCounterInstance TotalInstance
		{
			get
			{
				return this.totalInstance;
			}
		}

		public override PerformanceCounterInstance GetInstance(string instanceName)
		{
			if (PerformanceCounterMultipleInstanceWithAutoUpdateTotal.IsTotalInstanceName(instanceName))
			{
				return this.totalInstance;
			}
			return base.GetInstance(instanceName, this.totalInstance);
		}

		public override void CloseInstance(string instanceName)
		{
			if (PerformanceCounterMultipleInstanceWithAutoUpdateTotal.IsTotalInstanceName(instanceName))
			{
				throw new ArgumentException("You cannot close the TotalInstance", "instanceName");
			}
			base.CloseInstance(instanceName);
		}

		public override void RemoveInstance(string instanceName)
		{
			if (PerformanceCounterMultipleInstanceWithAutoUpdateTotal.IsTotalInstanceName(instanceName))
			{
				throw new ArgumentException("You cannot remove the TotalInstance", "instanceName");
			}
			base.RemoveInstance(instanceName);
		}

		public override void ResetInstance(string instanceName)
		{
			if (PerformanceCounterMultipleInstanceWithAutoUpdateTotal.IsTotalInstanceName(instanceName))
			{
				throw new ArgumentException("You cannot reset the TotalInstance", "instanceName");
			}
			base.ResetInstance(instanceName);
		}

		public override void RemoveAllInstances()
		{
			lock (this)
			{
				base.RemoveAllInstancesInternal();
				this.totalInstance.Close();
				this.totalInstance = base.InstanceCreator("_Total", null);
			}
		}

		private static bool IsTotalInstanceName(string instanceName)
		{
			return StringComparer.OrdinalIgnoreCase.Equals(instanceName, "_Total");
		}

		private const string TotalInstanceName = "_Total";

		private PerformanceCounterInstance totalInstance;
	}
}
