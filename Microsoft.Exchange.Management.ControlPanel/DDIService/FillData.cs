using System;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class FillData : Activity
	{
		public FillData()
		{
		}

		protected FillData(FillData activity) : base(activity)
		{
			this.LoadDataAction = activity.LoadDataAction;
		}

		public override Activity Clone()
		{
			return new FillData(this);
		}

		[DDIValidCodeBehindMethod]
		public string LoadDataAction { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult result = new RunResult();
			if (null != codeBehind && !string.IsNullOrEmpty(this.LoadDataAction))
			{
				DDIHelper.Trace("LoadDataAction: " + this.LoadDataAction);
				codeBehind.GetMethod(this.LoadDataAction).Invoke(null, new object[]
				{
					input,
					dataTable,
					store
				});
			}
			return result;
		}
	}
}
