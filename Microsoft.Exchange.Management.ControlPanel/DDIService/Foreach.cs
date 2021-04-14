using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Activity")]
	public class Foreach : Activity
	{
		public Foreach()
		{
			base.ErrorBehavior = ErrorBehavior.SilentlyContinue;
		}

		protected Foreach(Foreach activity) : base(activity)
		{
			this.Collection = activity.Collection;
			this.FailedCollection = activity.FailedCollection;
			this.Item = activity.Item;
			this.Activity = activity.Activity.Clone();
		}

		public override Activity Clone()
		{
			return new Foreach(this);
		}

		[DDIMandatoryValue]
		[DDIVariableNameExist]
		public string Collection { get; set; }

		[DDIVariableNameExist]
		public string FailedCollection { get; set; }

		[DDIExtendedVariableName]
		[DDIMandatoryValue]
		public string Item { get; set; }

		[DDIMandatoryValue]
		public Activity Activity { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DDIHelper.CheckDataTableForSingleObject(dataTable);
			DataRow dataRow = dataTable.Rows[0];
			RunResult runResult = new RunResult();
			IList<object> list = new List<object>();
			IEnumerable<object> enumerable = DDIHelper.GetVariableValue(store.ModifiedColumns, this.Collection, input, dataTable, store.IsGetListWorkflow) as IEnumerable<object>;
			this.totalItems = enumerable.Count<object>();
			this.executedItemCount = 0;
			foreach (object obj in enumerable)
			{
				dataRow[this.Item] = obj;
				if (this.Activity.IsRunnable(input, dataTable, store))
				{
					RunResult runResult2 = this.Activity.RunCore(input, dataTable, store, codeBehind, updateTableDelegate);
					runResult.DataObjectes.AddRange(runResult2.DataObjectes);
					this.statusReport = this.statusReport.Concat(this.Activity.GetStatusReport(input, dataTable, store)).ToArray<PowerShellResults>();
					if (runResult2.ErrorOccur)
					{
						list.Add(obj);
						if (base.ErrorBehavior == ErrorBehavior.Stop && this.Activity.ErrorBehavior == ErrorBehavior.Stop)
						{
							runResult.ErrorOccur = true;
							break;
						}
					}
				}
				this.executedItemCount++;
			}
			if (!string.IsNullOrEmpty(this.FailedCollection))
			{
				dataRow[this.FailedCollection] = list;
			}
			return runResult;
		}

		public override List<DataColumn> GetExtendedColumns()
		{
			List<DataColumn> list = new List<DataColumn>();
			list.Add(new DataColumn(this.Item, typeof(object)));
			list.AddRange(this.Activity.GetExtendedColumns());
			return list;
		}

		public override bool IsRunnable(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (!base.IsRunnable(input, dataTable, store))
			{
				return false;
			}
			DDIHelper.CheckDataTableForSingleObject(dataTable);
			object variableValue = DDIHelper.GetVariableValue(store.ModifiedColumns, this.Collection, input, dataTable, store.IsGetListWorkflow);
			return variableValue is IEnumerable<object> && (variableValue as IEnumerable<object>).Count<object>() > 0;
		}

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return this.statusReport;
		}

		internal override IEnumerable<Activity> Find(Func<Activity, bool> predicate)
		{
			return this.Activity.Find(predicate);
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			Variable variable = updatingVariable;
			if (this.Collection.Equals(updatingVariable.Name, StringComparison.OrdinalIgnoreCase))
			{
				variable = updatingVariable.ShallowClone();
				variable.Name = this.Item;
				variable.MappingProperty = this.Item;
			}
			return this.Activity.FindAndCheckPermission(predicate, input, dataTable, store, variable);
		}

		internal override int ProgressPercent
		{
			get
			{
				return ProgressCalculatorBase.CalculatePercentageHelper(base.ProgressPercent, this.executedItemCount, this.totalItems, this.Activity);
			}
		}

		private PowerShellResults[] statusReport = new PowerShellResults[0];

		private int executedItemCount;

		private int totalItems;
	}
}
