using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Body")]
	public class Sequence : Activity
	{
		public Sequence()
		{
			this.Body = new Collection<Activity>();
		}

		protected Sequence(Sequence activity) : base(activity)
		{
			this.Body = new Collection<Activity>((from c in activity.Body
			select c.Clone()).ToList<Activity>());
		}

		public override Activity Clone()
		{
			return new Sequence(this);
		}

		[DDIMandatoryValue]
		public Collection<Activity> Body { get; internal set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			this.activitiesExecutedCount = 0;
			foreach (Activity activity in this.Body)
			{
				if (activity.IsRunnable(input, dataTable, store))
				{
					this.CurrentExecutingActivity = activity;
					RunResult runResult2 = activity.RunCore(input, dataTable, store, codeBehind, updateTableDelegate);
					this.statusReport = this.statusReport.Concat(activity.GetStatusReport(input, dataTable, store)).ToArray<PowerShellResults>();
					runResult.DataObjectes.AddRange(runResult2.DataObjectes);
					if (runResult2.ErrorOccur && base.ErrorBehavior == ErrorBehavior.Stop && activity.ErrorBehavior == ErrorBehavior.Stop)
					{
						runResult.ErrorOccur = true;
						break;
					}
				}
				this.activitiesExecutedCount++;
			}
			this.CurrentExecutingActivity = null;
			return runResult;
		}

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return this.statusReport;
		}

		public override List<DataColumn> GetExtendedColumns()
		{
			List<DataColumn> list = new List<DataColumn>();
			foreach (Activity activity in this.Body)
			{
				list.AddRange(activity.GetExtendedColumns());
			}
			return list;
		}

		internal override IEnumerable<Activity> Find(Func<Activity, bool> predicate)
		{
			return this.Body.SelectMany((Activity c) => c.Find(predicate));
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			bool? newVal = null;
			bool? flag = null;
			foreach (Activity activity in this.Body)
			{
				newVal = activity.FindAndCheckPermission(predicate, input, dataTable, store, updatingVariable);
				flag = flag.Or(newVal);
				if (flag.IsTrue())
				{
					break;
				}
			}
			return flag;
		}

		internal override int ProgressPercent
		{
			get
			{
				return ProgressCalculatorBase.CalculatePercentageHelper(base.ProgressPercent, this.activitiesExecutedCount, this.Body.Count, this.CurrentExecutingActivity);
			}
		}

		protected Activity CurrentExecutingActivity { get; set; }

		private PowerShellResults[] statusReport = new PowerShellResults[0];

		private int activitiesExecutedCount;
	}
}
