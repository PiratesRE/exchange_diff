using System;
using System.Data;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class OutputObjectCmdlet : CmdletActivity
	{
		public OutputObjectCmdlet()
		{
		}

		protected OutputObjectCmdlet(OutputObjectCmdlet activity) : base(activity)
		{
			this.FillAllColumns = activity.FillAllColumns;
		}

		public bool FillAllColumns { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			base.ExecuteCmdlet(null, runResult, out powerShellResults, false);
			if (!runResult.ErrorOccur && powerShellResults.Succeeded && !string.IsNullOrEmpty(base.DataObjectName))
			{
				runResult.DataObjectes.Add(base.DataObjectName);
				store.UpdateDataObject(base.DataObjectName, null);
				if (store.GetDataObjectType(base.DataObjectName) == typeof(object))
				{
					if (powerShellResults.Output != null)
					{
						store.UpdateDataObject(base.DataObjectName, (from c in powerShellResults.Output
						select c.BaseObject).ToList<object>());
					}
				}
				else if (powerShellResults.HasValue && powerShellResults.Value != null)
				{
					store.UpdateDataObject(base.DataObjectName, powerShellResults.Value.BaseObject);
				}
				updateTableDelegate(base.DataObjectName, this.FillAllColumns);
			}
			return runResult;
		}
	}
}
