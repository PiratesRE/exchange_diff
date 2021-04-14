using System;
using System.Diagnostics;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.RemovePerfCounters)]
	[Cmdlet("Remove", "PerfCounters")]
	public class RemovePerfCounters : ManagePerfCounters
	{
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
			}
			catch (CorruptedPerformanceCountersException)
			{
			}
			TaskLogger.LogExit();
		}

		private enum Checkpoints
		{
			Started,
			UnloadedLocalizedText,
			Finished
		}
	}
}
