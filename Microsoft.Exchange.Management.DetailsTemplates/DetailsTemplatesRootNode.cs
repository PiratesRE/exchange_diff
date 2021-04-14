using System;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[NodeType("3B7B0725-B474-48cd-B59F-B5C2CA2EA8C2", Description = "Details Templates root node")]
	public sealed class DetailsTemplatesRootNode : ExchangeScopeNode
	{
		public DetailsTemplatesRootNode()
		{
			base.DisplayName = Strings.RootNodeDisplayName;
			base.Icon = Icons.DetailsTemplate;
			base.ViewDescriptions.Add(ExchangeFormView.CreateViewDescription(typeof(DetailsTemplatesResultPane)));
			base.HelpTopic = HelpId.DetailsTemplateRootNode.ToString();
			if (WinformsHelper.IsRemoteEnabled())
			{
				base.RegisterConnectionToPSServerAction();
			}
		}

		public const string NodeGuid = "3B7B0725-B474-48cd-B59F-B5C2CA2EA8C2";

		public const string NodeDescription = "Details Templates root node";
	}
}
