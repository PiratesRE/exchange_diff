using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIServiceHelper
	{
		public DDIServiceHelper(string schema, string workflow) : this(DDIService.DDIPath, schema, workflow)
		{
		}

		internal event EventHandler<ExecutingWorkflowEventArgs> WSDataHandlerCreated;

		internal DDIServiceHelper(string schemaFilesInstallPath, string schema, string workflow)
		{
			this.schemaFilesInstallPath = schemaFilesInstallPath;
			this.schema = schema;
			this.workflow = workflow;
		}

		internal PowerShellResults<JsonDictionary<object>> GetList(DDIParameters filter, SortOptions sort)
		{
			return this.GetListCommon(filter, sort, false);
		}

		internal PowerShellResults<JsonDictionary<object>> GetObject(Identity identity)
		{
			string text = this.workflow ?? "GetObject";
			return this.Get(text, identity);
		}

		internal PowerShellResults<JsonDictionary<object>> GetObjectOnDemand(Identity identity)
		{
			return this.Get(this.workflow, identity);
		}

		internal PowerShellResults<JsonDictionary<object>> GetObjectForNew(Identity identity)
		{
			string text = this.workflow ?? "GetObjectForNew";
			return this.Get(text, identity);
		}

		internal PowerShellResults<JsonDictionary<object>> SetObject(Identity identity, DDIParameters properties)
		{
			properties.FaultIfNull("properties");
			string text = this.workflow ?? "SetObject";
			return this.Execute(text, (identity == null) ? null : new Identity[]
			{
				identity
			}, properties);
		}

		internal PowerShellResults<JsonDictionary<object>> GetProgress(string progressId, bool async)
		{
			string text = this.workflow ?? "GetProgress";
			if (!string.IsNullOrEmpty(this.schema) && this.IsWorkflowDefined(text))
			{
				return this.ExecuteProgressWorkflow(text, progressId);
			}
			if (async)
			{
				DDIHelper.ProgressIdForGetListAsync = progressId;
				DDIParameters ddiparameters = new DDIParameters();
				AsyncGetListContext registeredContext = AsyncServiceManager.GetRegisteredContext(progressId);
				Dictionary<string, object> dictionary;
				if (registeredContext != null && registeredContext.Parameters != null && registeredContext.Parameters.Parameters != null)
				{
					dictionary = registeredContext.Parameters.Parameters.RawDictionary;
				}
				else
				{
					dictionary = new Dictionary<string, object>();
				}
				if (registeredContext != null && !string.IsNullOrEmpty(registeredContext.WorkflowOutput))
				{
					dictionary["workflowOutput"] = registeredContext.WorkflowOutput;
				}
				dictionary["ProgressId"] = progressId;
				ddiparameters.Parameters = new JsonDictionary<object>(dictionary);
				return this.GetListCommon(ddiparameters, null, true);
			}
			return AsyncServiceManager.GetProgress(progressId);
		}

		internal PowerShellResults Cancel(string progressId)
		{
			string text = this.workflow ?? "Cancel";
			if (!string.IsNullOrEmpty(this.schema) && this.IsWorkflowDefined(text))
			{
				return this.ExecuteProgressWorkflow(text, progressId);
			}
			return AsyncServiceManager.Cancel(progressId);
		}

		internal PowerShellResults<JsonDictionary<object>> NewObject(DDIParameters properties)
		{
			string text = this.workflow ?? "NewObject";
			return this.Execute(text, null, properties);
		}

		internal PowerShellResults RemoveObjects(Identity[] identities, DDIParameters parameters)
		{
			if (identities == null)
			{
				throw new FaultException(new ArgumentNullException("identities").Message);
			}
			this.ValidateIdentities(identities);
			string text = this.workflow ?? "RemoveObjects";
			return new PowerShellResults(this.Execute(text, identities, parameters));
		}

		internal PowerShellResults<JsonDictionary<object>> MultiObjectExecute(Identity[] identities, DDIParameters parameters)
		{
			this.ValidateIdentities(identities);
			return this.Execute(this.workflow, identities, parameters);
		}

		internal PowerShellResults<JsonDictionary<object>> SingleObjectExecute(Identity identity, DDIParameters properties)
		{
			return this.Execute(this.workflow, (identity == null) ? null : new Identity[]
			{
				identity
			}, properties);
		}

		private PowerShellResults<JsonDictionary<object>> GetListCommon(DDIParameters filter, SortOptions sort, bool forGetProgress)
		{
			string workflowName = this.workflow ?? "GetList";
			WSDataHandler wsdataHandler = new WSListDataHandler(this.schemaFilesInstallPath, this.schema, workflowName, filter, sort);
			this.RaiseWSDataHandlerCreatedEvent(wsdataHandler);
			this.SetWorkflowOutput(workflowName, wsdataHandler, filter);
			wsdataHandler.DataObjectStore.SetModifiedColumns(this.InitializeDDIParameters(wsdataHandler, filter));
			wsdataHandler.DataObjectStore.IsGetListWorkflow = true;
			if (filter != null && filter.Parameters != null && filter.Parameters.ContainsKey("ProgressId") && !forGetProgress)
			{
				wsdataHandler.DataObjectStore.AsyncType = ListAsyncType.GetListEndLoad;
				DDIHelper.ProgressIdForGetListAsync = (string)filter.Parameters["ProgressId"];
			}
			else if (DDIHelper.IsGetListAsync)
			{
				wsdataHandler.DataObjectStore.AsyncType = ListAsyncType.GetListAsync;
			}
			else if (DDIHelper.IsGetListPreLoad)
			{
				wsdataHandler.DataObjectStore.AsyncType = ListAsyncType.GetListPreLoad;
			}
			PowerShellResults<JsonDictionary<object>> powerShellResults = wsdataHandler.Execute();
			if (powerShellResults.HasWarnings)
			{
				for (int i = 0; i < powerShellResults.Warnings.Length; i++)
				{
					if (powerShellResults.Warnings[i] == Strings.WarningMoreResultsAvailable)
					{
						powerShellResults.Warnings[i] = ClientStrings.ListViewMoreResultsWarning;
					}
				}
			}
			return powerShellResults;
		}

		private PowerShellResults<JsonDictionary<object>> Execute(string workflow, Identity[] identities, DDIParameters properties)
		{
			WSDataHandler wsdataHandler = new WSSingleObjectDataHandler(this.schemaFilesInstallPath, this.schema, workflow);
			this.RaiseWSDataHandlerCreatedEvent(wsdataHandler);
			if (identities != null)
			{
				wsdataHandler.InputIdentity(identities);
			}
			wsdataHandler.OutputVariableWorkflow = "GetList";
			wsdataHandler.DataObjectStore.SetModifiedColumns(this.InitializeDDIParameters(wsdataHandler, properties));
			return wsdataHandler.Execute();
		}

		private PowerShellResults<JsonDictionary<object>> Get(string workflow, Identity identity)
		{
			string workflowName = string.IsNullOrEmpty(workflow) ? "GetObject" : workflow;
			WSDataHandler wsdataHandler = new WSSingleObjectDataHandler(this.schemaFilesInstallPath, this.schema, workflowName);
			this.RaiseWSDataHandlerCreatedEvent(wsdataHandler);
			if (identity != null)
			{
				wsdataHandler.InputIdentity(new Identity[]
				{
					identity
				});
			}
			return wsdataHandler.Execute();
		}

		private void RaiseWSDataHandlerCreatedEvent(WSDataHandler dataHandler)
		{
			if (this.WSDataHandlerCreated != null)
			{
				this.WSDataHandlerCreated(this, new ExecutingWorkflowEventArgs(dataHandler.ExecutingWorkflow));
			}
		}

		private List<string> InitializeDDIParameters(WSDataHandler dataHandler, DDIParameters parameters)
		{
			List<string> list = new List<string>();
			if (parameters != null && parameters.Parameters != null)
			{
				Dictionary<string, object> dictionary = parameters.Parameters;
				foreach (string text in dictionary.Keys)
				{
					if (dataHandler.Table.Columns.Contains(text))
					{
						dataHandler.InputValue(text, dictionary[text]);
						list.Add(text);
					}
				}
			}
			return list;
		}

		private void ValidateIdentities(Identity[] identities)
		{
			int num = 0;
			if (int.TryParse(ConfigurationManager.AppSettings["MaximumIdentities"], out num) && identities != null && identities.Length > num)
			{
				throw new FaultException(new ArgumentOutOfRangeException("identities").Message);
			}
		}

		private bool IsWorkflowDefined(string workflowName)
		{
			DDIHelper.Trace("IsWorkflowDefined: Schema:{0} WorkflowName:{1}", new object[]
			{
				this.schema,
				workflowName
			});
			bool flag = (from c in ServiceManager.GetInstance().GetService(this.schemaFilesInstallPath, this.schema).Workflows
			where c.Name.Equals(workflowName, StringComparison.InvariantCultureIgnoreCase)
			select c).Count<Workflow>() > 0;
			DDIHelper.Trace("IsWorkflowDefined({0}) returning {1}", new object[]
			{
				workflowName,
				flag
			});
			return flag;
		}

		private PowerShellResults<JsonDictionary<object>> ExecuteProgressWorkflow(string executingWorkflowName, string progressId)
		{
			DDIParameters ddiparameters = new DDIParameters();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ProgressId"] = progressId;
			ddiparameters.Parameters = new JsonDictionary<object>(dictionary);
			return this.Execute(executingWorkflowName, null, ddiparameters);
		}

		private void SetWorkflowOutput(string workflowName, WSDataHandler dataHandler, DDIParameters filter)
		{
			Workflow workFlow = dataHandler.GetWorkFlow(workflowName);
			string text = null;
			if (filter != null && filter.Parameters != null)
			{
				Dictionary<string, object> dictionary = filter.Parameters;
				if (dictionary.ContainsKey("workflowOutput"))
				{
					text = (string)dictionary["workflowOutput"];
				}
				else if (dictionary.ContainsKey("AdditionalCustomOutputs"))
				{
					text = workFlow.Output + ',' + dictionary["AdditionalCustomOutputs"];
				}
			}
			if (!string.IsNullOrWhiteSpace(text) && workFlow != null)
			{
				workFlow.Output = text;
			}
		}

		private const string GetObjectString = "GetObject";

		private const string ProgressIdString = "ProgressId";

		private const string FilterString = "filter";

		private const string SortString = "sort";

		private const string ASC = "ASC";

		private const string DESC = "DESC";

		private const string GetObjectForNewString = "GetObjectForNew";

		private const string SetObjectString = "SetObject";

		private const string NewObjectString = "NewObject";

		private const string GetProgressString = "GetProgress";

		private const string CancelString = "Cancel";

		private const string RemoveObjectsString = "RemoveObjects";

		public const string WorkflowOutput = "workflowOutput";

		public const string AdditionalCustomOutputs = "AdditionalCustomOutputs";

		private readonly string schema;

		private readonly string workflow;

		private readonly string schemaFilesInstallPath;
	}
}
