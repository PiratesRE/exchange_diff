using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ImportContactList")]
	public interface IImportContactList : IImportObjectService<ImportContactsResult, ImportContactListParameters>
	{
	}
}
