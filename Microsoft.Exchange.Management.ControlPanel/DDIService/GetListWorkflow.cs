using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetListWorkflow : Workflow
	{
		public GetListWorkflow()
		{
			base.Name = "GetList";
			this.ResultSize = (DDIHelper.IsGetListAsync ? DDIHelper.GetListAsyncModeResultSize.ToString() : DDIHelper.GetListDefaultResultSize.ToString());
		}

		protected GetListWorkflow(GetListWorkflow workflow) : base(workflow)
		{
			this.ResultSize = workflow.ResultSize;
		}

		public override Workflow Clone()
		{
			return new GetListWorkflow(this);
		}

		[DefaultValue("=>IIF(!DDIHelper.IsGetListAsync, DDIHelper.GetListDefaultResultSize, DDIHelper.GetListAsyncModeResultSize)")]
		[DDIValidLambdaExpression]
		public string ResultSize { get; set; }

		public int GetResultSizeInt32(DataRow input, DataTable dataTable)
		{
			if (this.resultSizeInt32 == -1 && !string.IsNullOrEmpty(this.ResultSize))
			{
				int num = -1;
				if (DDIHelper.IsLambdaExpression(this.ResultSize))
				{
					num = (int)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.ResultSize), typeof(int), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
				}
				else
				{
					int.TryParse(this.ResultSize, out num);
				}
				this.resultSizeInt32 = num;
			}
			return this.resultSizeInt32;
		}

		protected override void Initialize(DataRow input, DataTable dataTable)
		{
			int resultSize = this.GetResultSizeInt32(input, dataTable);
			foreach (Activity activity in base.Activities)
			{
				activity.SetResultSize(resultSize);
			}
		}

		private int resultSizeInt32 = -1;
	}
}
