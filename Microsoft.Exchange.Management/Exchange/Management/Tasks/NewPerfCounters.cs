using System;
using System.Diagnostics;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "PerfCounters")]
	[LocDescription(Strings.IDs.NewPerfCounters)]
	public class NewPerfCounters : ManagePerfCounters
	{
		[Parameter(Mandatory = false)]
		[ValidateRange(0, 2147483647)]
		public int FileMappingSize
		{
			get
			{
				return this.fileMappingSize;
			}
			set
			{
				this.fileMappingSize = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (new PerfCounterCategoryExistsCondition(this.CategoryName).Verify())
				{
					base.DeletePerfCounterCategory();
					PerformanceCounter.CloseSharedResources();
				}
				base.CreatePerfCounterCategory();
			}
			catch (CorruptedPerformanceCountersException)
			{
			}
			PerformanceCounter.CloseSharedResources();
			TaskLogger.LogExit();
		}

		private enum Checkpoints
		{
			Started,
			RegistryKeysCreated,
			Finished
		}
	}
}
