using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "SecurityPrincipalPicker")]
	public interface ISecurityPrincipalPicker : IGetListService<SecurityPrincipalPickerFilter, SecurityPrincipalPickerObject>
	{
	}
}
