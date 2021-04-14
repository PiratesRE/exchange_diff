using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	internal sealed class PerfCounterCategoryExistsCondition : Condition
	{
		public PerfCounterCategoryExistsCondition(string categoryName)
		{
			this.categoryName = categoryName;
		}

		public override bool Verify()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			int num = 0;
			try
			{
				IL_09:
				Exception ex = null;
				try
				{
					flag = PerformanceCounterCategory.Exists(this.CategoryName);
				}
				catch (OverflowException ex2)
				{
					ex = ex2;
				}
				catch (FormatException ex3)
				{
					ex = ex3;
				}
				catch (InvalidOperationException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					TaskLogger.Trace(new LocalizedString(string.Format("PerformanceCounterCategory.Exists(\"{0}\") thrown exception {1}", this.CategoryName, ex)));
					throw new CorruptedPerformanceCountersException(ex);
				}
				TaskLogger.Trace(new LocalizedString(string.Format("PerformanceCounterCategory.Exists(\"{0}\") returned {1}", this.CategoryName, flag)));
			}
			catch (Win32Exception ex5)
			{
				if (ex5.NativeErrorCode == 21 && num < 10)
				{
					TaskLogger.Trace(new LocalizedString("Got \"Device is not ready exception\"; will sleep and retry"));
					PerformanceCounter.CloseSharedResources();
					Thread.Sleep(1000);
					num++;
					goto IL_09;
				}
				throw;
			}
			TaskLogger.LogExit();
			return flag;
		}

		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
		}

		private readonly string categoryName;
	}
}
