using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class ShowDetailsTemplatesProperitiesAction : ResultsCommandAction
	{
		protected override void OnExecute()
		{
			base.OnExecute();
			DetailsTemplatesResultPane detailsTemplatesResultPane = base.ResultPane as DetailsTemplatesResultPane;
			string templateIdentity = detailsTemplatesResultPane.SelectedIdentity.ToString();
			string text = WinformsHelper.GenerateFormName<DetailsTemplatesEditor>((ADObjectId)detailsTemplatesResultPane.SelectedIdentity);
			if (!ExchangeForm.ActivateSingleInstanceForm(text))
			{
				DetailsTemplatesEditor detailsTemplatesEditor = new DetailsTemplatesEditor(templateIdentity);
				detailsTemplatesEditor.Icon = Icons.DetailsTemplate;
				detailsTemplatesEditor.Name = text;
				detailsTemplatesEditor.RefreshOnFinish = detailsTemplatesResultPane.GetSelectionRefreshObjects();
				detailsTemplatesEditor.PrivateSettings = (detailsTemplatesResultPane.PrivateSettings as DetailsTemplatesEditorSettings);
				detailsTemplatesEditor.ShowModeless(detailsTemplatesResultPane);
				detailsTemplatesEditor.HelpTopic = detailsTemplatesResultPane.SelectionHelpTopic;
			}
		}
	}
}
