using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SupervisionListEntry")]
	public interface ISupervisionListEntryService : IGetListService<SupervisionListEntryFilter, SupervisionListEntryRow>
	{
	}
}
