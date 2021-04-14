using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public abstract class RecipientPickerBase<F, L> : DataSourceService where F : RecipientPickerFilterBase, new() where L : RecipientPickerObject
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties")]
		public PowerShellResults<L> GetList(F filter, SortOptions sort)
		{
			return base.GetList<L, F>("Get-Recipient", filter, sort, "DisplayName");
		}

		private const string GetListRole = "Get-Recipient?ResultSize&Filter&RecipientTypeDetails&Properties";
	}
}
