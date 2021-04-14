using System;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class RawCmdlet : CmdletActivity
	{
		public RawCmdlet()
		{
		}

		protected RawCmdlet(RawCmdlet activity) : base(activity)
		{
			this.RawResultAction = activity.RawResultAction;
		}

		public override Activity Clone()
		{
			return new RawCmdlet(this);
		}

		[DDIValidCodeBehindMethod]
		public string RawResultAction { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			base.ExecuteCmdlet(null, runResult, out powerShellResults, false);
			if (!runResult.ErrorOccur && powerShellResults.Output != null && null != codeBehind && !string.IsNullOrEmpty(this.RawResultAction) && !string.IsNullOrEmpty(base.DataObjectName))
			{
				store.UpdateDataObject(base.DataObjectName, powerShellResults);
				DDIHelper.Trace("RawResultAction: " + this.RawResultAction);
				codeBehind.GetMethod(this.RawResultAction).Invoke(null, new object[]
				{
					input,
					dataTable,
					store
				});
			}
			return runResult;
		}
	}
}
