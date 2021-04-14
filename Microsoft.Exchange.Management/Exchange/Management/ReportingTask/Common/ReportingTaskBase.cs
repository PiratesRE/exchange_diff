using System;
using System.Linq.Expressions;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	public abstract class ReportingTaskBase<TReportObject> : Task where TReportObject : ReportObject
	{
		protected ReportingTaskBase()
		{
			this.TaskContext = new TaskContext(this);
			this.piiProcessor = new PiiProcessor();
			this.reportContextFactory = new ReportContextFactory();
			if (typeof(TReportObject).IsSubclassOf(typeof(ScaledReportObject)))
			{
				this.reportProvider = new ScaledReportProvider<TReportObject>(this.TaskContext, this.reportContextFactory);
			}
			else
			{
				this.reportProvider = new ReportProvider<TReportObject>(this.TaskContext, this.reportContextFactory);
			}
			this.reportProvider.ReportReceived += this.Report;
			this.reportProvider.StatementLogged += this.LogStatement;
			this.resultSizeDecorator = new ResultSizeDecorator<TReportObject>(this.TaskContext);
			this.AddQueryDecorator(this.resultSizeDecorator);
		}

		[Parameter(Mandatory = false)]
		public Unlimited<int> ResultSize
		{
			get
			{
				return (Unlimited<int>)base.Fields["ResultSize"];
			}
			set
			{
				base.Fields["ResultSize"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Expression Expression
		{
			get
			{
				return this.reportProvider.Expression;
			}
			set
			{
				this.reportProvider.Expression = value;
			}
		}

		protected abstract DataMartType DataMartType { get; }

		protected bool IsExpressionEnforced
		{
			get
			{
				return this.reportProvider.IsExpressionEnforced;
			}
		}

		protected virtual string ViewName
		{
			get
			{
				return null;
			}
		}

		private protected ITaskContext TaskContext { protected get; private set; }

		protected void AddQueryDecorator(QueryDecorator<TReportObject> queryDecorator)
		{
			this.reportProvider.AddQueryDecorator(queryDecorator);
		}

		protected virtual void ProcessNonPipelineParameter()
		{
			if (base.Fields.IsModified("ResultSize"))
			{
				this.resultSizeDecorator.ResultSize = new Unlimited<int>?(this.ResultSize);
			}
		}

		protected virtual void ProcessPipelineParameter()
		{
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			this.ProcessNonPipelineParameter();
			this.reportContextFactory.DataMartType = this.DataMartType;
			this.reportContextFactory.ReportType = typeof(TReportObject);
			this.reportContextFactory.ViewName = this.ViewName;
			this.reportProvider.Validate(false);
			this.piiProcessor.SuppressPiiEnabled = base.NeedSuppressingPiiData;
			base.AdditionalLogData = string.Empty;
		}

		protected override void InternalProcessRecord()
		{
			this.ProcessPipelineParameter();
			this.reportProvider.Validate(true);
			this.totalCount = 0L;
			this.reportProvider.Execute();
		}

		protected override bool IsKnownException(Exception e)
		{
			return base.IsKnownException(e) || e is ManagementObjectAmbiguousException || e is ManagementObjectNotFoundException || e is ReportingException;
		}

		private void Report(TReportObject reportObject)
		{
			this.totalCount += 1L;
			if (!this.IsExpressionEnforced && this.resultSizeDecorator.IsTargetResultSizeReached(this.totalCount))
			{
				this.WriteWarningForTruncateRecords();
				return;
			}
			this.piiProcessor.Supress(reportObject);
			base.WriteObject(reportObject);
		}

		private void LogStatement(string key, string statement)
		{
			base.AdditionalLogData += string.Format("[{0}]", statement);
			CmdletLogger.SafeAppendGenericInfo(base.CurrentTaskContext.UniqueId, key, statement);
		}

		private void WriteWarningForTruncateRecords()
		{
			if (this.resultSizeDecorator.IsResultSizeReached(this.totalCount))
			{
				this.WriteWarning(Strings.WarningMoreResultsAvailable);
				return;
			}
			if (this.resultSizeDecorator.IsDefaultResultSizeReached(this.totalCount))
			{
				this.WriteWarning(Strings.WarningDefaultResultSizeReached(this.resultSizeDecorator.DefaultResultSize.Value.ToString()));
			}
		}

		private const string ResultSizeKey = "ResultSize";

		private readonly ResultSizeDecorator<TReportObject> resultSizeDecorator;

		private readonly ReportProvider<TReportObject> reportProvider;

		private readonly ReportContextFactory reportContextFactory;

		private readonly PiiProcessor piiProcessor;

		private long totalCount;
	}
}
