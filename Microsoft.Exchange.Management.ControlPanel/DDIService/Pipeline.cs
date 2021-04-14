using System;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Body")]
	[DDIIsValidPipelineInnerActivity]
	public class Pipeline : Sequence
	{
		public Pipeline()
		{
		}

		protected Pipeline(Pipeline activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new Pipeline(this);
		}

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			IPSCommandWrapper ipscommandWrapper = base.PowershellFactory.CreatePSCommand();
			Activity activity = null;
			foreach (Activity activity2 in base.Body)
			{
				((CmdletActivity)activity2).BuildCommand(input, dataTable, store, codeBehind);
				ipscommandWrapper.AddCommand(((CmdletActivity)activity2).Command.Commands[0]);
				activity = activity2;
			}
			if (activity != null)
			{
				base.CurrentExecutingActivity = activity;
				((CmdletActivity)activity).Command = ipscommandWrapper;
				RunResult runResult2 = activity.Run(input, dataTable, store, codeBehind, updateTableDelegate);
				runResult.DataObjectes.AddRange(runResult2.DataObjectes);
				this.statusReport = this.statusReport.Concat(activity.GetStatusReport(input, dataTable, store)).ToArray<PowerShellResults>();
				base.CurrentExecutingActivity = null;
				if (runResult2.ErrorOccur && base.ErrorBehavior == ErrorBehavior.Stop && activity.ErrorBehavior == ErrorBehavior.Stop)
				{
					runResult.ErrorOccur = true;
					return runResult;
				}
			}
			return runResult;
		}

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return this.statusReport;
		}

		public override bool IsRunnable(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (!base.IsRunnable(input, dataTable, store))
			{
				return false;
			}
			for (int i = 0; i < base.Body.Count; i++)
			{
				Activity activity = base.Body[i];
				if (i < base.Body.Count - 1 && !(activity is OutputObjectCmdlet) && !(activity is GetListCmdlet))
				{
					return false;
				}
				if (!string.IsNullOrEmpty(((CmdletActivity)activity).PreAction) || !string.IsNullOrEmpty(((CmdletActivity)activity).PostAction))
				{
					return false;
				}
				if (!activity.IsRunnable(input, dataTable, store))
				{
					return false;
				}
			}
			return true;
		}

		internal override int ProgressPercent
		{
			get
			{
				if (base.CurrentExecutingActivity != null)
				{
					return base.CurrentExecutingActivity.ProgressPercent;
				}
				return base.ProgressPercent;
			}
		}

		private PowerShellResults[] statusReport = new PowerShellResults[0];
	}
}
