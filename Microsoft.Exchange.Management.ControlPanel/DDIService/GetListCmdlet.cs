using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetListCmdlet : CmdletActivity
	{
		[DDICollectionDecorator(AttributeType = typeof(DDIVariableNameExistAttribute), ObjectConverter = typeof(DDISetToVariableConverter))]
		public Collection<Set> DefaultValues
		{
			get
			{
				return this.sets;
			}
			set
			{
				this.sets = value;
			}
		}

		public GetListCmdlet()
		{
			base.AllowExceuteThruHttpGetRequest = true;
			base.IdentityVariable = string.Empty;
		}

		protected GetListCmdlet(GetListCmdlet activity) : base(activity)
		{
			this.DefaultValues = new Collection<Set>((from c in activity.DefaultValues
			select c.Clone() as Set).ToList<Set>());
		}

		public override Activity Clone()
		{
			return new GetListCmdlet(this);
		}

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DDIHelper.Trace(TraceType.InfoTrace, "Executing :" + base.GetType().Name);
			DDIHelper.Trace<IPSCommandWrapper>(TraceType.InfoTrace, base.Command);
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			if (!DDIHelper.ForGetListProgress)
			{
				base.ExecuteCmdlet(null, runResult, out powerShellResults, store.AsyncType == ListAsyncType.GetListAsync || store.AsyncType == ListAsyncType.GetListPreLoad);
			}
			else if (store.AsyncType == ListAsyncType.GetListEndLoad)
			{
				powerShellResults = AsyncServiceManager.GetPreLoadResult(DDIHelper.ProgressIdForGetListAsync);
			}
			else
			{
				string progressIdForGetListAsync = DDIHelper.ProgressIdForGetListAsync;
				powerShellResults = AsyncServiceManager.GetCurrentResult(progressIdForGetListAsync);
			}
			base.StatusReport = powerShellResults;
			runResult.ErrorOccur = !base.StatusReport.Succeeded;
			if (!runResult.ErrorOccur)
			{
				if (powerShellResults.Output != null && powerShellResults.Output.Length > 0)
				{
					Type type = null;
					Hashtable propertyHash = new Hashtable();
					Hashtable hashtable = new Hashtable();
					dataTable.BeginLoadData();
					foreach (PSObject psobject in powerShellResults.Output)
					{
						if (type == null)
						{
							type = psobject.BaseObject.GetType();
							type.GetProperties().Perform(delegate(PropertyInfo c)
							{
								propertyHash[c.Name] = c;
							});
						}
						DataRow dataRow = dataTable.NewRow();
						dataTable.Rows.Add(dataRow);
						dataRow.BeginEdit();
						foreach (object obj in dataTable.Columns)
						{
							DataColumn dataColumn = (DataColumn)obj;
							Variable variable = dataColumn.ExtendedProperties["Variable"] as Variable;
							if (variable != null)
							{
								if (variable.PersistWholeObject)
								{
									dataRow[dataColumn.ColumnName] = psobject.BaseObject;
								}
								else if (!hashtable.ContainsKey(dataColumn.ColumnName))
								{
									PropertyInfo propertyInfo = propertyHash[variable.MappingProperty] as PropertyInfo;
									if (propertyInfo != null)
									{
										dataRow[dataColumn.ColumnName] = propertyInfo.GetValue(psobject.BaseObject, null);
									}
									else
									{
										hashtable.Add(dataColumn.ColumnName, null);
									}
								}
							}
						}
						Collection<Set> defaultValues = this.DefaultValues;
						NewDefaultObject.SetDefaultValues(input, dataRow, dataTable, defaultValues);
						dataRow.EndEdit();
					}
					dataTable.EndLoadData();
				}
			}
			else if (this.isResultSizeAutoApplied)
			{
				Microsoft.Exchange.Management.ControlPanel.ErrorRecord errorRecord = base.StatusReport.ErrorRecords.FirstOrDefault((Microsoft.Exchange.Management.ControlPanel.ErrorRecord x) => x.Exception is ParameterBindingException && (x.Exception as ParameterBindingException).ParameterName == "ResultSize");
				if (errorRecord != null)
				{
					if (base.StatusReport.ErrorRecords.Length > 1)
					{
						base.StatusReport.ErrorRecords = base.StatusReport.ErrorRecords.Except(new Microsoft.Exchange.Management.ControlPanel.ErrorRecord[]
						{
							errorRecord
						}).ToArray<Microsoft.Exchange.Management.ControlPanel.ErrorRecord>();
					}
					else
					{
						base.StatusReport.ErrorRecords = new Microsoft.Exchange.Management.ControlPanel.ErrorRecord[]
						{
							new Microsoft.Exchange.Management.ControlPanel.ErrorRecord(new Exception(Strings.UnsupportedAutoApplyParamMsgText))
						};
					}
					lock (GetListCmdlet.unsupportedCmdletsLock)
					{
						string commandText = base.GetCommandText(store);
						if (!GetListCmdlet.unsupportedCmdlets.Contains(commandText))
						{
							GetListCmdlet.unsupportedCmdlets.Add(commandText);
						}
					}
				}
			}
			return runResult;
		}

		public override IList<Parameter> GetEffectiveParameters(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			IList<Parameter> effectiveParameters = base.GetEffectiveParameters(input, dataTable, store);
			this.isResultSizeAutoApplied = false;
			if (this.resultSize > 0)
			{
				Parameter parameter = new Parameter
				{
					Name = "ResultSize",
					Type = ParameterType.Mandatory,
					Value = this.resultSize
				};
				if (!effectiveParameters.Contains(parameter))
				{
					bool flag = false;
					lock (GetListCmdlet.unsupportedCmdletsLock)
					{
						flag = GetListCmdlet.unsupportedCmdlets.Contains(base.GetCommandText(store));
					}
					if (!flag && this.CheckPermission(store, new List<string>
					{
						parameter.Name
					}, null))
					{
						effectiveParameters.Add(parameter);
						this.isResultSizeAutoApplied = true;
					}
				}
			}
			return effectiveParameters;
		}

		internal override void SetResultSize(int resultSize)
		{
			this.resultSize = resultSize;
		}

		protected override string GetVerb()
		{
			return "Get-";
		}

		private Collection<Set> sets = new Collection<Set>();

		private int resultSize = -1;

		private bool isResultSizeAutoApplied;

		private static List<string> unsupportedCmdlets = new List<string>();

		private static object unsupportedCmdletsLock = new object();
	}
}
