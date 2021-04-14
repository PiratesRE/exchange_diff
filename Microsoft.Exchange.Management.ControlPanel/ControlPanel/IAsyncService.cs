using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IAsyncService")]
	public interface IAsyncService
	{
		[OperationContract]
		PowerShellResults<JsonDictionary<object>> GetProgress(string progressId);
	}
}
