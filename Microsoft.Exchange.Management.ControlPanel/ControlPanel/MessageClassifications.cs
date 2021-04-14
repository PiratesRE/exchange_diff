using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class MessageClassifications : DataSourceService, IMessageClassifications, IGetListService<MessageClassificationFilter, MessageClassification>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-MessageClassification@R:Self")]
		public PowerShellResults<MessageClassification> GetList(MessageClassificationFilter filter, SortOptions sort)
		{
			PowerShellResults<MessageClassification> list = base.GetList<MessageClassification, MessageClassificationFilter>("Get-MessageClassification", filter, sort);
			if (list.Output != null && list.Output.Length > 0)
			{
				list.Output = Array.FindAll<MessageClassification>(list.Output, (MessageClassification x) => x.PermissionMenuVisible);
			}
			return list;
		}

		private const string Noun = "MessageClassification";

		internal const string GetCmdlet = "Get-MessageClassification";

		internal const string ReadScope = "@R:Self";

		internal const string GetListRole = "Get-MessageClassification@R:Self";
	}
}
