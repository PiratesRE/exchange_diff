using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class ResponseMessage : ResourceBase, IResponseMessage, IResourceBase<ResponseMessageConfiguration, SetResponseMessageConfiguration>, IEditObjectService<ResponseMessageConfiguration, SetResponseMessageConfiguration>, IGetObjectService<ResponseMessageConfiguration>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self")]
		public PowerShellResults<ResponseMessageConfiguration> GetObject(Identity identity)
		{
			return base.GetObject<ResponseMessageConfiguration>(identity);
		}

		[PrincipalPermission(SecurityAction.Demand, Role = "Resource+Get-CalendarProcessing?Identity@R:Self+Set-CalendarProcessing?Identity@W:Self")]
		public PowerShellResults<ResponseMessageConfiguration> SetObject(Identity identity, SetResponseMessageConfiguration properties)
		{
			return base.SetObject<ResponseMessageConfiguration, SetResponseMessageConfiguration>(identity, properties);
		}
	}
}
