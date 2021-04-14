using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class WSDataHandler : AutomatedDataHandlerBase
	{
		public Workflow ExecutingWorkflow { get; private set; }

		public WSDataHandler(Service service, string workflowName) : base(service)
		{
			this.Construct(workflowName);
		}

		public WSDataHandler(string schemaFilesInstallPath, string resourceName, string workflowName, DDIParameters parameters = null) : base(schemaFilesInstallPath, resourceName)
		{
			this.Construct(workflowName);
			this.parameters = parameters;
		}

		public string OutputVariableWorkflow { get; set; }

		internal string UniqueLogonUserIdentity
		{
			get
			{
				if (string.IsNullOrEmpty(this.uniqueLogonUserIdentity))
				{
					this.uniqueLogonUserIdentity = HttpContext.Current.GetCachedUserUniqueKey();
				}
				return this.uniqueLogonUserIdentity;
			}
			set
			{
				this.uniqueLogonUserIdentity = value;
			}
		}

		public void InputIdentity(params Identity[] identities)
		{
			if (identities != null && identities.Length > 0)
			{
				this.InputValue("Identity", (identities.Length == 1) ? identities[0] : identities);
			}
		}

		public PowerShellResults<JsonDictionary<object>> Execute()
		{
			PowerShellResults<JsonDictionary<object>> result;
			using (EcpPerformanceData.DDIServiceExecution.StartRequestTimer())
			{
				switch (this.ExecutingWorkflow.AsyncMode)
				{
				case AsyncMode.AsynchronousOnly:
					this.ExecutingWorkflow.AsyncRunning = true;
					break;
				case AsyncMode.SynchronousAndAsynchronous:
					this.ExecutingWorkflow.AsyncRunning = (base.DataObjectStore.AsyncType == ListAsyncType.GetListAsync || base.DataObjectStore.AsyncType == ListAsyncType.GetListPreLoad);
					break;
				}
				if (!this.ExecutingWorkflow.AsyncRunning || DDIHelper.ForGetListProgress)
				{
					result = this.ExecuteCore(this.ExecutingWorkflow);
				}
				else
				{
					if (!typeof(ProgressCalculatorBase).IsAssignableFrom(this.ExecutingWorkflow.ProgressCalculator))
					{
						throw new ArgumentException("A valid ProgressCalculator type must be specified for async running workflow.");
					}
					Func<PowerShellResults> callback = delegate()
					{
						IList<string> outputVariables = this.GetOutputVariables(this.OutputVariableWorkflow);
						AsyncGetListContext asyncGetListContext = new AsyncGetListContext
						{
							WorkflowOutput = string.Join(",", outputVariables ?? ((IList<string>)Array<string>.Empty)),
							UnicodeOutputColumnNames = this.GetUnicodeVariablesFrom(outputVariables),
							Parameters = this.parameters
						};
						AsyncServiceManager.RegisterWorkflow(this.ExecutingWorkflow, asyncGetListContext);
						return this.ExecuteCore(this.ExecutingWorkflow);
					};
					AsyncTaskType taskType = AsyncTaskType.Default;
					string text = this.ExecutingWorkflow.Name;
					if (base.DataObjectStore.AsyncType == ListAsyncType.GetListAsync)
					{
						taskType = AsyncTaskType.AsyncGetList;
					}
					else if (base.DataObjectStore.AsyncType == ListAsyncType.GetListPreLoad)
					{
						taskType = AsyncTaskType.AsyncGetListPreLoad;
						text += "_PreLoad";
					}
					PowerShellResults powerShellResults = AsyncServiceManager.InvokeAsync(callback, null, this.UniqueLogonUserIdentity, taskType, text);
					result = new PowerShellResults<JsonDictionary<object>>
					{
						ProgressId = powerShellResults.ProgressId,
						ErrorRecords = powerShellResults.ErrorRecords
					};
				}
			}
			return result;
		}

		public virtual void InputValue(string columnName, object value)
		{
			Variable variable = base.Table.Columns[columnName].ExtendedProperties["Variable"] as Variable;
			if (variable != null && variable.InputConverter != null && variable.InputConverter.CanConvert(value))
			{
				value = variable.InputConverter.Convert(value);
			}
			DDIHelper.Trace("      Initial value: " + EcpTraceExtensions.FormatParameterValue(value) + ", Column: " + columnName);
			base.Input[columnName] = value;
		}

		public IList<string> GetOutputVariables(string referWorkflowName)
		{
			IEnumerable<Workflow> source = from c in this.workflows
			where c.Name == referWorkflowName
			select c;
			IList<string> outputVariables = this.GetWorkFlow(this.executingWorkflowName).GetOutputVariables();
			if (outputVariables == null && 0 < source.Count<Workflow>())
			{
				return source.First<Workflow>().GetOutputVariables();
			}
			return outputVariables;
		}

		public List<string> GetUnicodeVariablesFrom(IList<string> variables)
		{
			List<string> list = new List<string>((variables != null) ? variables.Count : 0);
			if (variables != null)
			{
				foreach (Variable variable in base.ProfileBuilder.Variables)
				{
					if (variable.UnicodeString && variables.Contains(variable.Name))
					{
						list.Add(variable.Name);
					}
				}
			}
			return list;
		}

		public Workflow GetWorkFlow(string workflowName)
		{
			IEnumerable<Workflow> source = from c in this.workflows
			where c.Name == workflowName
			select c;
			if (source.Count<Workflow>() != 1)
			{
				throw new WorkflowNotExistException(workflowName);
			}
			return source.First<Workflow>();
		}

		protected void ExtractDataRow(DataRow row, IEnumerable<DataColumn> columns, out Dictionary<string, object> output)
		{
			output = new Dictionary<string, object>();
			foreach (DataColumn dataColumn in columns)
			{
				Variable variable = dataColumn.ExtendedProperties["Variable"] as Variable;
				if (variable != null)
				{
					base.FillColumnsBasedOnLambdaExpression(row, variable);
					output[dataColumn.ColumnName] = DDIHelper.PrepareVariableForSerialization(row[dataColumn.ColumnName], variable);
				}
			}
		}

		protected abstract PowerShellResults<JsonDictionary<object>> ExecuteCore(Workflow workflow);

		private void Construct(string workflowName)
		{
			this.executingWorkflowName = workflowName;
			this.workflows = base.ProfileBuilder.Workflows;
			this.ExtendTableSchema();
			this.ExecutingWorkflow = this.GetWorkFlow(this.executingWorkflowName);
		}

		private void ExtendTableSchema()
		{
			base.Table.Columns.AddRange(base.ProfileBuilder.ExtendedColumns.Distinct(new WSDataHandler.DataColumnComparer()).ToArray<DataColumn>());
		}

		private IList<Workflow> workflows;

		private string executingWorkflowName;

		private string uniqueLogonUserIdentity;

		private DDIParameters parameters;

		private class DataColumnComparer : IEqualityComparer<DataColumn>
		{
			public bool Equals(DataColumn x, DataColumn y)
			{
				return object.ReferenceEquals(x, y) || (!object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null) && x.ColumnName == y.ColumnName);
			}

			public int GetHashCode(DataColumn column)
			{
				if (object.ReferenceEquals(column, null))
				{
					return 0;
				}
				return (column.ColumnName == null) ? 0 : column.ColumnName.GetHashCode();
			}
		}
	}
}
