using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class WSListDataHandler : WSDataHandler
	{
		public WSListDataHandler(Service service, string workflowName) : base(service, workflowName)
		{
		}

		public WSListDataHandler(string schemaFilesInstallPath, string resourceName, string workflowName, DDIParameters parameters, SortOptions sortOptions) : base(schemaFilesInstallPath, resourceName, workflowName, parameters)
		{
			this.sortOptions = sortOptions;
		}

		protected override PowerShellResults<JsonDictionary<object>> ExecuteCore(Workflow workflow)
		{
			PowerShellResults<JsonDictionary<object>> powerShellResults = new PowerShellResults<JsonDictionary<object>>();
			PowerShellResults powerShellResults2 = workflow.Run(base.Input, base.Table, base.DataObjectStore, base.ProfileBuilder.Class, delegate(string result, bool fillAllColumns)
			{
			});
			powerShellResults.MergeErrors(powerShellResults2);
			powerShellResults.MergeProgressData<JsonDictionary<object>>(powerShellResults2 as PowerShellResults<JsonDictionary<object>>);
			if (powerShellResults.ErrorRecords.Length == 0)
			{
				using (EcpPerformanceData.DDITypeConversion.StartRequestTimer())
				{
					Dictionary<string, object> dictionary = null;
					List<JsonDictionary<object>> list = new List<JsonDictionary<object>>();
					IList<string> outputList = base.GetOutputVariables(base.OutputVariableWorkflow);
					DataColumn[] columns = (from DataColumn c in base.Table.Columns
					where outputList == null || outputList.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase)
					select c).ToArray<DataColumn>();
					foreach (object obj in base.Table.DefaultView)
					{
						DataRowView dataRowView = (DataRowView)obj;
						base.ExtractDataRow(dataRowView.Row, columns, out dictionary);
						list.Add(dictionary);
					}
					this.AddSortDataToResult(powerShellResults, list, outputList);
					if (this.sortOptions != null && outputList.Contains(this.sortOptions.PropertyName))
					{
						Func<JsonDictionary<object>[], JsonDictionary<object>[]> ddisortFunction = this.sortOptions.GetDDISortFunction();
						powerShellResults.Output = ddisortFunction(list.ToArray());
					}
					else
					{
						powerShellResults.Output = list.ToArray();
					}
					GetListWorkflow getListWorkflow = workflow as GetListWorkflow;
					if (getListWorkflow != null)
					{
						int resultSizeInt = getListWorkflow.GetResultSizeInt32(base.Input, base.Table);
						if (resultSizeInt > 0 && resultSizeInt < list.Count)
						{
							powerShellResults.Output = powerShellResults.Output.Take(resultSizeInt).ToArray<JsonDictionary<object>>();
							if (!powerShellResults.Warnings.Contains(Strings.WarningMoreResultsAvailable))
							{
								powerShellResults.Warnings = powerShellResults.Warnings.Concat(new string[]
								{
									Strings.WarningMoreResultsAvailable
								}).ToArray<string>();
							}
						}
					}
				}
			}
			DDIHelper.Trace(TraceType.InfoTrace, "Result: ");
			DDIHelper.Trace<PowerShellResults<JsonDictionary<object>>>(TraceType.InfoTrace, powerShellResults);
			return powerShellResults;
		}

		private void AddSortDataToResult(PowerShellResults<JsonDictionary<object>> results, List<JsonDictionary<object>> outputs, IList<string> outputList)
		{
			List<string> unicodeVariablesFrom = base.GetUnicodeVariablesFrom(outputList);
			if (unicodeVariablesFrom.Count > 0)
			{
				if (!DDIHelper.ForGetListProgress || base.DataObjectStore.AsyncType == ListAsyncType.GetListEndLoad)
				{
					this.AddSortDataForSynchronousGetList(results, outputs, outputList, unicodeVariablesFrom);
					return;
				}
				this.AddSortDataForAsynchronousGetList(results, outputs, outputList, unicodeVariablesFrom);
			}
		}

		private void AddSortDataForSynchronousGetList(PowerShellResults<JsonDictionary<object>> results, List<JsonDictionary<object>> outputs, IList<string> outputList, List<string> unicodeColumnNames)
		{
			if (outputs != null && outputs.Count > 0)
			{
				List<Tuple<int, JsonDictionary<object>>> list = new List<Tuple<int, JsonDictionary<object>>>(outputs.Count);
				for (int i = 0; i < outputs.Count; i++)
				{
					list.Add(new Tuple<int, JsonDictionary<object>>(i, outputs[i]));
				}
				CultureInfo culture = CultureInfo.CurrentCulture;
				using (List<string>.Enumerator enumerator = unicodeColumnNames.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string column = enumerator.Current;
						list.Sort((Tuple<int, JsonDictionary<object>> tuple1, Tuple<int, JsonDictionary<object>> tuple2) => string.Compare((string)tuple1.Item2.RawDictionary[column], (string)tuple2.Item2.RawDictionary[column], true, culture));
						int num = -1;
						string key = column + "_s";
						Tuple<int, JsonDictionary<object>> tuple = null;
						foreach (Tuple<int, JsonDictionary<object>> tuple3 in list)
						{
							if (tuple == null || !string.Equals((string)tuple.Item2.RawDictionary[column], (string)tuple3.Item2.RawDictionary[column], StringComparison.CurrentCultureIgnoreCase))
							{
								num++;
							}
							tuple3.Item2.RawDictionary[key] = num;
							tuple = tuple3;
						}
					}
				}
			}
		}

		private void AddSortDataForAsynchronousGetList(PowerShellResults<JsonDictionary<object>> results, List<JsonDictionary<object>> outputs, IList<string> outputList, List<string> unicodeColumnNames)
		{
			AsyncGetListContext asyncGetListContext = results.AsyncGetListContext;
			if (asyncGetListContext != null)
			{
				int count = unicodeColumnNames.Count;
				if (outputs != null && outputs.Count > 0)
				{
					if (asyncGetListContext.UnicodeColumns == null)
					{
						asyncGetListContext.UnicodeColumns = new List<Tuple<int, string[], string>>(Math.Max(outputs.Count, DDIHelper.GetListDefaultResultSize * 4));
					}
					int m = 0;
					int num = asyncGetListContext.UnicodeColumns.Count;
					while (m < outputs.Count)
					{
						Dictionary<string, object> rawDictionary = outputs[m].RawDictionary;
						string[] array = new string[count];
						for (int j = 0; j < count; j++)
						{
							array[j] = (string)rawDictionary[unicodeColumnNames[j]];
						}
						string rawIdentity = ((Identity)rawDictionary["Identity"]).RawIdentity;
						asyncGetListContext.UnicodeColumns.Add(new Tuple<int, string[], string>(num, array, rawIdentity));
						m++;
						num++;
					}
					if (asyncGetListContext.Completed)
					{
						WSListDataHandler.<>c__DisplayClassc CS$<>8__locals1 = new WSListDataHandler.<>c__DisplayClassc();
						List<Tuple<int, string[], string>> unicodeColumns = asyncGetListContext.UnicodeColumns;
						results.SortColumnNames = unicodeColumnNames.ToArray();
						results.SortDataRawId = new string[unicodeColumns.Count];
						for (int k = 0; k < unicodeColumns.Count; k++)
						{
							results.SortDataRawId[k] = unicodeColumns[k].Item3;
						}
						results.SortData = new int[unicodeColumnNames.Count][];
						CS$<>8__locals1.culture = CultureInfo.CurrentCulture;
						int i;
						for (i = 0; i < unicodeColumnNames.Count; i++)
						{
							string text = unicodeColumnNames[i];
							results.SortData[i] = new int[unicodeColumns.Count];
							unicodeColumns.Sort((Tuple<int, string[], string> tuple1, Tuple<int, string[], string> tuple2) => string.Compare(tuple1.Item2[i], tuple2.Item2[i], true, CS$<>8__locals1.culture));
							int num2 = -1;
							Tuple<int, string[], string> tuple = null;
							for (int l = 0; l < unicodeColumns.Count; l++)
							{
								Tuple<int, string[], string> tuple3 = unicodeColumns[l];
								if (tuple == null || !string.Equals(tuple.Item2[i], tuple3.Item2[i], StringComparison.CurrentCultureIgnoreCase))
								{
									num2++;
								}
								results.SortData[i][tuple3.Item1] = num2;
								tuple = tuple3;
							}
						}
					}
				}
			}
		}

		private SortOptions sortOptions;
	}
}
