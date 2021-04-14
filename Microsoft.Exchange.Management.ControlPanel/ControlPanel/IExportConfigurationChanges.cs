using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ExportConfigurationChanges")]
	public interface IExportConfigurationChanges : INewObjectService<ExportConfigurationChangesRow, ExportConfigurationChangesParameters>
	{
	}
}
