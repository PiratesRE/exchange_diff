using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ExportMailboxChanges")]
	public interface IExportMailboxChanges : INewObjectService<ExportMailboxChangesRow, ExportMailboxChangesParameters>
	{
	}
}
