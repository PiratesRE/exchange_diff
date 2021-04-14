using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public abstract class CmdletActivity : ParameterActivity
	{
		public CmdletActivity()
		{
			this.StatusReport = new PowerShellResults();
			this.IdentityName = "Identity";
			this.IdentityVariable = "Identity";
			this.ShouldContinueParam = "Force";
			this.ShouldContinueOperation = ShouldContinueOperation.AddParam;
			this.DisableLogging = false;
		}

		protected CmdletActivity(CmdletActivity activity) : base(activity)
		{
			this.StatusReport = activity.StatusReport;
			this.DataObjectName = activity.DataObjectName;
			this.CommandText = activity.CommandText;
			this.SnapInAlias = activity.SnapInAlias;
			this.IdentityVariable = activity.IdentityVariable;
			this.IdentityName = activity.IdentityName;
			this.ShouldContinueParam = activity.ShouldContinueParam;
			this.ShouldContinueOperation = activity.ShouldContinueOperation;
			this.AllowExceuteThruHttpGetRequest = activity.AllowExceuteThruHttpGetRequest;
			this.DisableLogging = activity.DisableLogging;
		}

		public sealed override bool IsRunnable(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (AsyncServiceManager.IsCurrentWorkCancelled())
			{
				return false;
			}
			List<Parameter> parametersToInvoke = this.GetParametersToInvoke(input, dataTable, store);
			bool flag = this.IsToExecuteCmdlet(input, dataTable, store, parametersToInvoke);
			if (!flag)
			{
				return false;
			}
			List<string> parameters = (from x in parametersToInvoke
			select x.Name).ToList<string>();
			string cmdletRbacString = this.GetCmdletRbacString(store, parameters);
			bool flag2 = base.RbacChecker.IsInRole(cmdletRbacString);
			DDIHelper.Trace("Checking RBAC: {0} : {1}", new object[]
			{
				cmdletRbacString,
				flag2
			});
			if (!flag2)
			{
				throw new CmdletAccessDeniedException(Strings.AccessDeniedMessage);
			}
			return true;
		}

		public override IList<Parameter> GetEffectiveParameters(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			IList<Parameter> list = base.GetEffectiveParameters(input, dataTable, store);
			list = (list ?? new List<Parameter>());
			ShouldContinueContext shouldContinueContext = base.EacHttpContext.ShouldContinueContext;
			if (shouldContinueContext == null && dataTable.Columns.Contains("ShouldContinue") && dataTable.Rows.Count > 0)
			{
				bool? flag = dataTable.Rows[0]["ShouldContinue"] as bool?;
				if (flag.IsTrue())
				{
					shouldContinueContext = (dataTable.Rows[0]["LastErrorContext"] as ShouldContinueContext);
					base.EacHttpContext.ShouldContinueContext = shouldContinueContext;
				}
			}
			if (shouldContinueContext != null && shouldContinueContext.CmdletsPrompted.Contains(this.GetCommandText(store)))
			{
				if (this.ShouldContinueOperation == ShouldContinueOperation.AddParam)
				{
					list.Add(new Parameter
					{
						Name = this.ShouldContinueParam,
						Type = ParameterType.Switch
					});
				}
				else if (this.ShouldContinueOperation == ShouldContinueOperation.RemoveParam)
				{
					list.Remove(new Parameter
					{
						Name = this.ShouldContinueParam,
						Type = ParameterType.Switch
					});
				}
			}
			return list;
		}

		protected virtual bool IsToExecuteCmdlet(DataRow input, DataTable dataTable, DataObjectStore store, List<Parameter> parameters)
		{
			return true;
		}

		protected virtual List<Parameter> GetParametersToInvoke(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return (from c in this.GetEffectiveParameters(input, dataTable, store)
			where c.IsRunnable(input, dataTable)
			select c).ToList<Parameter>();
		}

		public PowerShellResults StatusReport { get; set; }

		[DDIDataObjectNameExist]
		public string DataObjectName { get; set; }

		[DDIValidCommandText]
		public string CommandText { get; set; }

		[DefaultValue("Force")]
		public string ShouldContinueParam { get; set; }

		[DefaultValue(ShouldContinueOperation.AddParam)]
		public ShouldContinueOperation ShouldContinueOperation { get; set; }

		[DefaultValue(false)]
		public bool DisableLogging { get; set; }

		public string SnapInAlias { get; set; }

		public bool AllowExceuteThruHttpGetRequest { get; set; }

		[DDIVariableNameExist]
		public string IdentityVariable { get; set; }

		public string IdentityName { get; set; }

		internal object DataObject { get; set; }

		internal IPSCommandWrapper Command { get; set; }

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (this.StatusReport != null)
			{
				if (base.ErrorBehavior == ErrorBehavior.SilentlyContinue)
				{
					this.StatusReport.ErrorRecords = Array<Microsoft.Exchange.Management.ControlPanel.ErrorRecord>.Empty;
				}
				else if (base.ErrorBehavior == ErrorBehavior.ErrorAsWarning && !this.StatusReport.ErrorRecords.IsNullOrEmpty())
				{
					string[] array = Array.ConvertAll<Microsoft.Exchange.Management.ControlPanel.ErrorRecord, string>(this.StatusReport.ErrorRecords, (Microsoft.Exchange.Management.ControlPanel.ErrorRecord x) => x.Exception.Message);
					this.StatusReport.Warnings = (this.StatusReport.Warnings.IsNullOrEmpty() ? array : this.StatusReport.Warnings.Concat(array).ToArray<string>());
					this.StatusReport.ErrorRecords = Array<Microsoft.Exchange.Management.ControlPanel.ErrorRecord>.Empty;
				}
			}
			return new PowerShellResults[]
			{
				this.StatusReport
			};
		}

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			RunResult runResult = new RunResult();
			PowerShellResults<PSObject> powerShellResults;
			this.ExecuteCmdlet(null, runResult, out powerShellResults, false);
			return runResult;
		}

		internal void ExecuteCmdlet(IEnumerable pipelineInput, RunResult runResult, out PowerShellResults<PSObject> result, bool isGetListAsync = false)
		{
			DDIHelper.Trace(TraceType.InfoTrace, "Executing :" + base.GetType().Name);
			DDIHelper.Trace(TraceType.InfoTrace, "Task: {0}", new object[]
			{
				this.Command
			});
			DDIHelper.Trace(TraceType.InfoTrace, "Pipeline: {0}", new object[]
			{
				pipelineInput
			});
			WebServiceParameters parameters = null;
			if (this.AllowExceuteThruHttpGetRequest)
			{
				parameters = CmdletActivity.allowExceuteThruHttpGetRequestParameters;
			}
			result = this.Command.Invoke<PSObject>(DataSourceService.UserRunspaces, pipelineInput, parameters, this, isGetListAsync);
			if (this.DisableLogging)
			{
				result.CmdletLogInfo = null;
			}
			this.StatusReport = result;
			runResult.ErrorOccur = !this.StatusReport.Succeeded;
		}

		protected virtual string GetVerb()
		{
			return null;
		}

		protected override void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			this.BuildCommand(input, dataTable, store, codeBehind);
		}

		internal void BuildCommand(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			this.Command = base.PowershellFactory.CreatePSCommand().AddCommand(this.GetCommandText(store));
			foreach (Parameter parameter in this.GetEffectiveParameters(input, dataTable, store))
			{
				if (parameter.IsRunnable(input, dataTable))
				{
					switch (parameter.Type)
					{
					case ParameterType.Switch:
						this.Command.AddParameter(parameter.Name);
						break;
					case ParameterType.Mandatory:
					case ParameterType.RunOnModified:
					{
						object value = Parameter.ConvertToParameterValue(input, dataTable, parameter, store);
						this.Command.AddParameter(parameter.Name, value);
						break;
					}
					default:
						throw new NotSupportedException();
					}
				}
			}
		}

		protected string GetCmdletRbacString(DataObjectStore store, IEnumerable<string> parameters)
		{
			string[] exculdedParameters = new string[]
			{
				this.ShouldContinueParam
			};
			parameters = from x in parameters
			where !exculdedParameters.Contains(x, StringComparer.OrdinalIgnoreCase)
			select x;
			if (parameters.Any<string>())
			{
				return this.GetRbacCommandText(store) + "?" + string.Join("&", parameters);
			}
			return this.GetRbacCommandText(store);
		}

		protected string GetRbacCommandText(DataObjectStore store)
		{
			if (!string.IsNullOrEmpty(this.SnapInAlias))
			{
				return this.SnapInAlias + "\\" + this.GetCommandText(store);
			}
			return this.GetCommandText(store);
		}

		protected string GetCommandText(DataObjectStore store)
		{
			if (string.IsNullOrEmpty(this.CommandText))
			{
				return this.GetVerb() + store.GetDataObjectType(this.DataObjectName).Name;
			}
			return this.CommandText;
		}

		internal override bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			List<string> parameters;
			if (updatingVariable != null && !string.IsNullOrWhiteSpace(updatingVariable.MappingProperty))
			{
				parameters = (from c in this.GetEffectiveParameters(input, dataTable, store)
				where c.Type != ParameterType.RunOnModified || c.IsAccessingVariable(updatingVariable.MappingProperty)
				select c.Name).ToList<string>();
			}
			else
			{
				parameters = (from c in this.GetEffectiveParameters(input, dataTable, store)
				where c.IsRunnable(input, dataTable)
				select c.Name).ToList<string>();
			}
			return this.CheckPermission(store, parameters, updatingVariable);
		}

		protected virtual bool CheckPermission(DataObjectStore store, List<string> parameters, Variable variable)
		{
			string name = this.DataObjectName;
			if (variable != null)
			{
				name = (variable.RbacDataObjectName ?? this.DataObjectName);
			}
			string cmdletRbacString = this.GetCmdletRbacString(store, parameters);
			ADRawEntry adrawEntry = store.IsDataObjectDummy(name) ? null : (store.GetDataObject(name) as ADRawEntry);
			bool flag = base.RbacChecker.IsInRole(cmdletRbacString, adrawEntry);
			DDIHelper.Trace("Checking {0} on object {1}: {2}", new object[]
			{
				cmdletRbacString,
				(adrawEntry != null) ? adrawEntry.ToString() : string.Empty,
				flag
			});
			return flag;
		}

		internal override bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			IEnumerable<Activity> enumerable = new List<Activity>
			{
				this
			}.Where(predicate);
			bool? result = null;
			foreach (Activity activity in enumerable)
			{
				if (this is IReadOnlyChecker)
				{
					if (!base.Parameters.Any((Parameter p) => p.IsAccessingVariable(updatingVariable.Name)))
					{
						continue;
					}
				}
				result = new bool?(activity.HasPermission(input, dataTable, store, updatingVariable));
			}
			return result;
		}

		internal void OnPSProgressReport(ProgressReportEventArgs e)
		{
			this.ProgressPercent = e.Percent;
			this.OnPSProgressChanged(e);
		}

		internal virtual void OnPSProgressChanged(ProgressReportEventArgs e)
		{
			if (this.PSProgressChanged != null)
			{
				this.PSProgressChanged(this, e);
			}
		}

		public event EventHandler<ProgressReportEventArgs> PSProgressChanged;

		private static BaseWebServiceParameters allowExceuteThruHttpGetRequestParameters = new BaseWebServiceParameters
		{
			AllowExceuteThruHttpGetRequest = true
		};
	}
}
