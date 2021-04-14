using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class WSSingleObjectDataHandler : WSDataHandler
	{
		public WSSingleObjectDataHandler(Service service, string workflowName) : base(service, workflowName)
		{
			this.Initialize();
		}

		public WSSingleObjectDataHandler(string schemaFilesInstallPath, string resourceName, string workflowName) : base(schemaFilesInstallPath, resourceName, workflowName, null)
		{
			this.Initialize();
		}

		protected override PowerShellResults<JsonDictionary<object>> ExecuteCore(Workflow workflow)
		{
			DataRow row = base.Table.NewRow();
			base.Table.Rows.Add(row);
			base.Row.ItemArray = base.Input.ItemArray;
			PowerShellResults<JsonDictionary<object>> powerShellResults = new PowerShellResults<JsonDictionary<object>>();
			powerShellResults.MergeErrors(workflow.Run(base.Input, base.Table, base.DataObjectStore, base.ProfileBuilder.Class, new Workflow.UpdateTableDelegate(this.UpdateTable)));
			if (powerShellResults.ErrorRecords.Length == 0)
			{
				if (workflow.AsyncRunning && AsyncServiceManager.IsCurrentWorkCancelled())
				{
					goto IL_451;
				}
				using (EcpPerformanceData.DDITypeConversion.StartRequestTimer())
				{
					IList<string> outputVariables = base.GetOutputVariables(base.OutputVariableWorkflow);
					Func<DataColumn, bool> predicate;
					if (this.IsGetObjectForNew)
					{
						predicate = ((DataColumn c) => outputVariables.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase) && this.Row[c.ColumnName] != DBNull.Value);
					}
					else
					{
						predicate = ((DataColumn c) => outputVariables.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase) || (Variable.MandatoryVariablesForGetObject.Contains(c.ColumnName) && this.ExecutingWorkflow is GetObjectWorkflow));
					}
					IEnumerable<DataColumn> columns = (outputVariables == null) ? base.Table.Columns.Cast<DataColumn>() : base.Table.Columns.Cast<DataColumn>().Where(predicate);
					Dictionary<string, object> dictionary;
					base.ExtractDataRow(base.Table.Rows[0], columns, out dictionary);
					if (base.ExecutingWorkflow is GetObjectWorkflow)
					{
						foreach (string name in this.GetOutputRelatedDataObjects(columns))
						{
							IVersionable versionable = base.DataObjectStore.GetDataObject(name) as IVersionable;
							if (versionable != null && versionable.ExchangeVersion != null)
							{
								bool flag = versionable.ExchangeVersion.IsOlderThan(base.DataObjectStore.GetDataObjectDeclaration(name).MinSupportedVersion);
								if (versionable.IsReadOnly || flag)
								{
									base.Table.Rows[0]["IsReadOnly"] = true;
									if (dictionary.ContainsKey("IsReadOnly"))
									{
										dictionary["IsReadOnly"] = true;
									}
									string text = flag ? Strings.ObjectTooOld : Strings.VersionMismatchWarning(versionable.ExchangeVersion.ExchangeBuild);
									powerShellResults.Warnings = ((powerShellResults.Warnings == null) ? new string[]
									{
										text
									} : powerShellResults.Warnings.Concat(new string[]
									{
										text
									}).ToArray<string>());
									break;
								}
							}
						}
					}
					Dictionary<string, ValidatorInfo[]> dictionary2;
					IList<string> source;
					IList<string> source2;
					workflow.LoadMetaData(base.Input, base.Table, base.DataObjectStore, outputVariables, out dictionary2, out source, out source2, base.ProfileBuilder);
					powerShellResults.Validators = dictionary2;
					powerShellResults.ReadOnlyProperties = source.ToArray<string>();
					powerShellResults.NoAccessProperties = source2.ToArray<string>();
					powerShellResults.Output = new JsonDictionary<object>[]
					{
						dictionary
					};
					goto IL_451;
				}
			}
			ShouldContinueException ex = powerShellResults.ErrorRecords[0].Exception as ShouldContinueException;
			if (ex != null)
			{
				ShouldContinueContext shouldContinueContext = (HttpContext.Current.Items["ShouldContinueContext"] as ShouldContinueContext) ?? new ShouldContinueContext();
				if (!shouldContinueContext.CmdletsPrompted.Contains(ex.Details.CurrentCmdlet))
				{
					shouldContinueContext.CmdletsPrompted.Add(ex.Details.CurrentCmdlet);
				}
				powerShellResults.ErrorRecords[0].Context = shouldContinueContext;
			}
			if (!workflow.OutputOnError.IsNullOrEmpty())
			{
				IEnumerable<DataColumn> columns2 = from DataColumn c in base.Table.Columns
				where workflow.OutputOnError.Contains(c.ColumnName)
				select c;
				Dictionary<string, object> dictionary3;
				base.ExtractDataRow(base.Table.Rows[0], columns2, out dictionary3);
				if (powerShellResults.ErrorRecords[0].Context == null)
				{
					powerShellResults.ErrorRecords[0].Context = new ErrorRecordContext();
				}
				powerShellResults.ErrorRecords[0].Context.LastOuput = dictionary3;
			}
			IL_451:
			DDIHelper.Trace(TraceType.InfoTrace, "Result: ");
			DDIHelper.Trace<PowerShellResults<JsonDictionary<object>>>(TraceType.InfoTrace, powerShellResults);
			return powerShellResults;
		}

		private IEnumerable<string> GetOutputRelatedDataObjects(IEnumerable<DataColumn> columns)
		{
			IEnumerable<string> variables = DDIHelper.GetOutputRawVariables(columns);
			return (from c in base.ProfileBuilder.Variables
			where variables.Contains(c.Name) && !string.IsNullOrWhiteSpace(c.DataObjectName)
			select c.DataObjectName).Distinct<string>();
		}

		private void Initialize()
		{
			base.Table.ColumnChanged += this.Table_ColumnChanged;
		}

		private void Table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			DDIHelper.Trace("      New value: " + EcpTraceExtensions.FormatParameterValue(base.Row[e.Column]) + ", Variable: " + e.Column.ColumnName);
		}

		private void UpdateTable(string targetConfigObject, bool fillAllColumns)
		{
			object dataObject = base.DataObjectStore.GetDataObject(targetConfigObject);
			if (dataObject == null)
			{
				return;
			}
			foreach (object obj in base.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				Variable variable = dataColumn.ExtendedProperties["Variable"] as Variable;
				if (variable != null && variable.DataObjectName != null && (fillAllColumns || targetConfigObject.Equals(variable.DataObjectName, StringComparison.OrdinalIgnoreCase)))
				{
					object obj2 = variable.PersistWholeObject ? dataObject : base.DataObjectStore.GetValue(targetConfigObject, variable.MappingProperty);
					obj2 = (obj2 ?? DBNull.Value);
					if (!this.IsGetObjectForNew || !DDIHelper.IsEmptyValue(obj2))
					{
						base.Row[dataColumn.ColumnName] = obj2;
					}
				}
			}
		}

		private bool IsGetObjectForNew
		{
			get
			{
				if (this.isGetObjectForNew == null)
				{
					this.isGetObjectForNew = new bool?(base.ExecutingWorkflow is GetObjectForNewWorkflow);
				}
				return this.isGetObjectForNew.Value;
			}
		}

		private bool? isGetObjectForNew = null;
	}
}
