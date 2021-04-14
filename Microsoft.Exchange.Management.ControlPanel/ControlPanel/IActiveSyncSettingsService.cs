using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "ActiveSyncSettings")]
	public interface IActiveSyncSettingsService : IEditObjectService<ActiveSyncSettings, SetActiveSyncSettings>, IGetObjectService<ActiveSyncSettings>
	{
	}
}
