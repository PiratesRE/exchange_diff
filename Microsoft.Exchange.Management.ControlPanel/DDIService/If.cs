using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class If : BranchActivity
	{
		public If()
		{
		}

		protected If(If activity) : base(activity)
		{
			this.Condition = activity.Condition;
		}

		public override Activity Clone()
		{
			return new If(this);
		}

		[DefaultValue(null)]
		[DDIValidLambdaExpression]
		public virtual string Condition { get; set; }

		protected override bool CalculateCondition(DataRow input, DataTable dataTable)
		{
			return (bool)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.Condition), typeof(bool), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
		}
	}
}
