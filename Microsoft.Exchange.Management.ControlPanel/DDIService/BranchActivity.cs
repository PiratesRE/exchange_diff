using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Then")]
	public abstract class BranchActivity : Activity
	{
		public BranchActivity()
		{
		}

		protected BranchActivity(BranchActivity activity) : base(activity)
		{
			if (activity.Then != null)
			{
				this.Then = activity.Then.Clone();
			}
			if (activity.Else != null)
			{
				this.Else = activity.Else.Clone();
			}
		}

		public Activity Then { get; set; }

		public Activity Else { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult result = new RunResult();
			if (this.CheckCondition(input, dataTable) && this.Then.IsRunnable(input, dataTable, store))
			{
				this.currentExecutingActivity = this.Then;
				result = this.Then.RunCore(input, dataTable, store, codeBehind, updateTableDelegate);
			}
			else if (this.Else != null && this.Else.IsRunnable(input, dataTable, store))
			{
				this.currentExecutingActivity = this.Else;
				result = this.Else.RunCore(input, dataTable, store, codeBehind, updateTableDelegate);
			}
			this.currentExecutingActivity = null;
			return result;
		}

		public override bool IsRunnable(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			this.cachedCondtion = null;
			return base.IsRunnable(input, dataTable, store);
		}

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (this.CheckCondition(input, dataTable))
			{
				return this.Then.GetStatusReport(input, dataTable, store);
			}
			if (this.Else != null)
			{
				return this.Else.GetStatusReport(input, dataTable, store);
			}
			return new PowerShellResults[0];
		}

		public override List<DataColumn> GetExtendedColumns()
		{
			List<DataColumn> list = new List<DataColumn>();
			if (this.Then != null)
			{
				list.AddRange(this.Then.GetExtendedColumns());
			}
			if (this.Else != null)
			{
				list.AddRange(this.Else.GetExtendedColumns());
			}
			return list;
		}

		internal override IEnumerable<Activity> Find(Func<Activity, bool> predicate)
		{
			List<Activity> list = new List<Activity>();
			if (this.cachedCondtion == null)
			{
				if (this.Then != null)
				{
					list.AddRange(this.Then.Find(predicate));
				}
				if (this.Else != null)
				{
					list.AddRange(this.Else.Find(predicate));
				}
			}
			else if (this.cachedCondtion.IsTrue())
			{
				list.AddRange(this.Then.Find(predicate));
			}
			else if (this.Else != null)
			{
				list.AddRange(this.Else.Find(predicate));
			}
			return list;
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			bool? newVal = null;
			bool? oldVal = null;
			bool? flag = null;
			if (this.Then != null)
			{
				newVal = this.Then.FindAndCheckPermission(predicate, input, dataTable, store, updatingVariable);
			}
			if (this.Else != null)
			{
				oldVal = this.Else.FindAndCheckPermission(predicate, input, dataTable, store, updatingVariable);
			}
			return oldVal.Or(newVal);
		}

		protected bool CheckCondition(DataRow input, DataTable dataTable)
		{
			if (this.cachedCondtion == null)
			{
				this.cachedCondtion = new bool?(this.CalculateCondition(input, dataTable));
			}
			return this.cachedCondtion.Value;
		}

		protected abstract bool CalculateCondition(DataRow input, DataTable dataTable);

		internal override int ProgressPercent
		{
			get
			{
				if (this.currentExecutingActivity == null)
				{
					return base.ProgressPercent;
				}
				return this.currentExecutingActivity.ProgressPercent;
			}
		}

		internal override void SetResultSize(int resultSize)
		{
			this.Then.SetResultSize(resultSize);
			if (this.Else != null)
			{
				this.Else.SetResultSize(resultSize);
			}
		}

		private bool? cachedCondtion;

		private Activity currentExecutingActivity;
	}
}
