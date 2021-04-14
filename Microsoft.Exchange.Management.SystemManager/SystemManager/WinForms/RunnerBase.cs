using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class RunnerBase
	{
		public abstract void Run(object interactionHandler, DataRow row, DataObjectStore store);

		public abstract void Cancel();

		[DefaultValue(null)]
		public IRunnable RunnableTester { get; set; }

		[DDIValidLambdaExpression]
		[DefaultValue(null)]
		public string RunnableLambdaExpression { get; set; }

		public virtual bool IsRunnable(DataRow row, DataObjectStore store)
		{
			if (!string.IsNullOrEmpty(this.RunnableLambdaExpression))
			{
				return (bool)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.RunnableLambdaExpression), typeof(bool), row, null);
			}
			return this.RunnableTester == null || this.RunnableTester.IsRunnable(row);
		}

		public virtual void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
		}

		internal event EventHandler<ProgressReportEventArgs> ProgressReport;

		internal void OnProgressReport(object sender, ProgressReportEventArgs e)
		{
			if (this.ProgressReport != null)
			{
				this.ProgressReport(sender, e);
			}
		}
	}
}
