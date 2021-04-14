using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class BulkCustomMaximumEventExpectedWorkflow : BulkEditWorkflow
	{
		public BulkCustomMaximumEventExpectedWorkflow()
		{
			base.Name = "BulkCustomMaximumEventExpected";
			base.ProgressCalculator = typeof(MaximumCountProgressCalculator);
		}

		[DDIValidLambdaExpression]
		public string MaxProgressBarEventsExpected { get; set; }

		protected override void Initialize(DataRow input, DataTable dataTable)
		{
			base.Initialize(input, dataTable);
			if (string.IsNullOrEmpty(this.MaxProgressBarEventsExpected) || !DDIHelper.IsLambdaExpression(this.MaxProgressBarEventsExpected))
			{
				return;
			}
			MaximumCountProgressCalculator maximumCountProgressCalculator = base.ProgressCalculatorInstance as MaximumCountProgressCalculator;
			if (maximumCountProgressCalculator != null)
			{
				maximumCountProgressCalculator.ProgressRecord.MaxCount = (int)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.MaxProgressBarEventsExpected), typeof(int), DDIHelper.GetLambdaExpressionDataRow(dataTable), input);
				return;
			}
			string message = string.Format("BulkCustomMaximumEventExpectedWorkflow should be used only with MaximumCountProgressCalculator. Current: {0}", (base.ProgressCalculatorInstance == null) ? "<null>" : base.ProgressCalculatorInstance.GetType().Name);
			throw new InvalidOperationException(message);
		}

		public override Workflow Clone()
		{
			BulkCustomMaximumEventExpectedWorkflow bulkCustomMaximumEventExpectedWorkflow = new BulkCustomMaximumEventExpectedWorkflow();
			bulkCustomMaximumEventExpectedWorkflow.Name = base.Name;
			bulkCustomMaximumEventExpectedWorkflow.Activities = new Collection<Activity>((from c in base.Activities
			select c.Clone()).ToList<Activity>());
			bulkCustomMaximumEventExpectedWorkflow.MaxProgressBarEventsExpected = this.MaxProgressBarEventsExpected;
			return bulkCustomMaximumEventExpectedWorkflow;
		}
	}
}
