using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "IImportObjectService")]
	public interface IImportObjectService<O, U> where O : BaseRow
	{
		[OperationContract]
		PowerShellResults<O> ImportObject(Identity identity, U properties);
	}
}
