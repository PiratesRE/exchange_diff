using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UserExtensions")]
	public interface IUserExtensions : IGetListService<UserExtensionsFilter, UMMailboxExtension>
	{
	}
}
