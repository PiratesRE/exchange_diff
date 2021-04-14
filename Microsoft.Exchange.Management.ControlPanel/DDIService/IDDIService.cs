using System;
using System.ServiceModel;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[ServiceContract(Namespace = "ECP", Name = "DDIService")]
	[ServiceKnownType("GetKnownTypes", typeof(DDIService))]
	public interface IDDIService
	{
		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetList(DDIParameters filter, SortOptions sort);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetObject(Identity identity);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetObjectOnDemand(Identity identity, string workflowName);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetObjectForNew(Identity identity);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> SetObject(Identity identity, DDIParameters properties);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> NewObject(DDIParameters properties);

		[OperationContract]
		PowerShellResults RemoveObjects(Identity[] identities, DDIParameters parameters);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> MultiObjectExecute(Identity[] identities, DDIParameters parameters);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> SingleObjectExecute(Identity identity, DDIParameters properties);

		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetProgress(string progressId);

		[OperationContract]
		PowerShellResults Cancel(string progressId);
	}
}
