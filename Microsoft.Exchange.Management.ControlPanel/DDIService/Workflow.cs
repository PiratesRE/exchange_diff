using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Web.UI.WebControls;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Activities")]
	[DDIWorkflow]
	public class Workflow
	{
		public Workflow()
		{
			this.ProgressCalculator = typeof(SingleOperationProgressCalculator);
		}

		protected Workflow(Workflow workflow)
		{
			this.Name = workflow.Name;
			this.AsyncMode = workflow.AsyncMode;
			this.AsyncRunning = workflow.AsyncRunning;
			this.ProgressCalculator = workflow.ProgressCalculator;
			this.Output = workflow.Output;
			this.Activities = new Collection<Activity>((from c in workflow.Activities
			select c.Clone()).ToList<Activity>());
			this.OutputOnError = workflow.OutputOnError;
		}

		public virtual Workflow Clone()
		{
			return new Workflow(this);
		}

		[DDIMandatoryValue]
		public string Name { get; set; }

		[DefaultValue(false)]
		public bool AsyncRunning { get; set; }

		[DefaultValue(AsyncMode.SynchronousOnly)]
		public AsyncMode AsyncMode { get; set; }

		[DefaultValue(typeof(SingleOperationProgressCalculator))]
		public Type ProgressCalculator { get; set; }

		[DDICharSeparatorItems(AttributeType = typeof(DDIVariableNameExistAttribute), Separators = " ,")]
		[DefaultValue(null)]
		public virtual string Output { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		[DefaultValue(null)]
		[DDICollectionDecorator(AttributeType = typeof(DDIVariableNameExistAttribute))]
		public string[] OutputOnError
		{
			get
			{
				return this.outputOnError;
			}
			set
			{
				this.outputOnError = value;
			}
		}

		public Collection<Activity> Activities
		{
			get
			{
				return this.activities;
			}
			internal set
			{
				this.activities = value;
			}
		}

		public event EventHandler<ProgressReportEventArgs> ProgressChanged;

		public IList<string> GetOutputVariables()
		{
			if (this.Output == null)
			{
				return null;
			}
			IList<string> list = new List<string>();
			if (!string.IsNullOrEmpty(this.Output))
			{
				string[] array = this.Output.Split(new char[]
				{
					','
				});
				foreach (string text in array)
				{
					string text2 = text.Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
					}
				}
			}
			return list;
		}

		protected virtual void Initialize(DataRow input, DataTable dataTable)
		{
		}

		public PowerShellResults Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DDIHelper.Trace("Executing workflow: {0} {1}", new object[]
			{
				base.GetType().Name,
				this.Name
			});
			this.Initialize(input, dataTable);
			List<PowerShellResults> list = new List<PowerShellResults>();
			this.activitiesExecutedCount = 0;
			bool forGetListProgress = DDIHelper.ForGetListProgress;
			foreach (Activity activity in this.Activities)
			{
				if (activity.IsRunnable(input, dataTable, store))
				{
					this.currentExecutingActivity = activity;
					if (!forGetListProgress && this.AsyncRunning)
					{
						foreach (Activity activity2 in activity.Find((Activity x) => x is CmdletActivity))
						{
							CmdletActivity cmdletActivity = (CmdletActivity)activity2;
							cmdletActivity.PSProgressChanged += this.Activity_ProgressChanged;
						}
					}
					RunResult runResult = activity.RunCore(input, dataTable, store, codeBehind, updateTableDelegate);
					list.AddRange(activity.GetStatusReport(input, dataTable, store));
					if (runResult.ErrorOccur && activity.ErrorBehavior == ErrorBehavior.Stop)
					{
						break;
					}
				}
				this.activitiesExecutedCount++;
			}
			PowerShellResults powerShellResults = forGetListProgress ? new PowerShellResults<JsonDictionary<object>>() : new PowerShellResults();
			foreach (PowerShellResults powerShellResults2 in list)
			{
				powerShellResults.MergeErrors(powerShellResults2);
				if (forGetListProgress)
				{
					((PowerShellResults<JsonDictionary<object>>)powerShellResults).MergeProgressData<PSObject>(powerShellResults2 as PowerShellResults<PSObject>);
				}
			}
			return powerShellResults;
		}

		public int ProgressPercent
		{
			get
			{
				return ProgressCalculatorBase.CalculatePercentageHelper(100, this.activitiesExecutedCount, this.Activities.Count, this.currentExecutingActivity);
			}
		}

		private void Activity_ProgressChanged(object sender, ProgressReportEventArgs e)
		{
			if (this.ProgressChanged != null)
			{
				e.Percent = this.ProgressPercent;
				this.ProgressChanged(this, e);
			}
		}

		internal virtual void LoadMetaData(DataRow input, DataTable dataTable, DataObjectStore store, IList<string> outputVariables, out Dictionary<string, ValidatorInfo[]> validators, out IList<string> readOnlyProperties, out IList<string> notAccessProperties, Service service)
		{
			validators = new Dictionary<string, ValidatorInfo[]>();
			readOnlyProperties = new List<string>();
			notAccessProperties = new List<string>();
		}

		public ProgressCalculatorBase ProgressCalculatorInstance { get; set; }

		private Collection<Activity> activities = new Collection<Activity>();

		private Activity currentExecutingActivity;

		private int activitiesExecutedCount;

		private string[] outputOnError;

		public delegate void UpdateTableDelegate(string dataObject, bool fillAllColumns = false);
	}
}
