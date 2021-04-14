using System;
using System.ServiceModel;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "NavBar")]
	public interface INavBarService
	{
		[OperationContract]
		PowerShellResults<NavBarPack> GetObject(Identity identity);
	}
}
