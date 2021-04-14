using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesResultPane : CaptionedResultPane
	{
		public override string SelectionHelpTopic
		{
			get
			{
				return SelectionHelpTopics.DetailsTemplate;
			}
		}

		static DetailsTemplatesResultPane()
		{
			DetailsTemplatesResultPane.iconLibrary.Icons.Add("DetailsTemplate", Icons.DetailsTemplate);
			DetailsTemplatesResultPane.iconLibrary.Icons.Add("RestoreDefaultTemplate", Icons.RestoreDefaultTemplate);
		}

		public DetailsTemplatesResultPane() : base(ResultPaneProfileLoader.Loader, "DetailsTemplate")
		{
			base.Name = "DetailsTemplatesResultPane";
			base.Icon = Icons.DetailsTemplate;
			ObjectList objectList = new ObjectList();
			objectList.Name = "DetailsTemplatesResultPaneDataListView";
			objectList.Dock = DockStyle.Fill;
			objectList.ListView.IconLibrary = DetailsTemplatesResultPane.iconLibrary;
			objectList.ListView.ImageIndex = 0;
			objectList.FilterControl.ObjectSchema = ObjectSchema.GetInstance<DetailsTemplatesResultPane.DetailsTemplatesFilterSchema>();
			FilterablePropertyDescription filterablePropertyDescription = new FilterablePropertyDescription(DetailsTemplatesResultPane.DetailsTemplatesFilterSchema.TemplateType, Strings.TypeColumnName, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual
			});
			filterablePropertyDescription.FilterableListSource = new ObjectListSource(new string[]
			{
				"Contact",
				"Group",
				"Mailbox Agent",
				"Public Folder",
				"Search Dialog",
				"User"
			});
			objectList.FilterControl.PropertiesToFilter.Add(filterablePropertyDescription);
			objectList.FilterControl.PropertiesToFilter.Add(new FilterablePropertyDescription(DetailsTemplatesResultPane.DetailsTemplatesFilterSchema.Language, Strings.LanguageColumnName, new PropertyFilterOperator[]
			{
				PropertyFilterOperator.Equal,
				PropertyFilterOperator.NotEqual,
				PropertyFilterOperator.Contains,
				PropertyFilterOperator.NotContains,
				PropertyFilterOperator.StartsWith,
				PropertyFilterOperator.EndsWith
			}));
			base.ListControl = objectList.ListView;
			base.FilterControl = objectList.FilterControl;
			base.Controls.Add(objectList);
		}

		protected override void SetupCommandsProfile()
		{
			base.SetupCommandsProfile();
			ResultsCommandProfile item = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "restoreDetailsTemplate",
					Icon = Icons.RestoreDefaultTemplate,
					Text = Strings.RestoreDetailsTemplate
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Update,
					IsSelectionCommand = true
				},
				Action = new ResultsTaskCommandAction
				{
					CommandText = "Restore-DetailsTemplate",
					SingleSelectionConfirmation = new SingleSelectionMessageDelegate(Strings.RestoreDetailsTemplateConfirmMessage),
					MultipleSelectionConfirmation = new MultipleSelectionMessageDelegate(Strings.RestoreDetailsTemplatesConfirmMessage)
				}
			};
			base.CommandsProfile.CustomSelectionCommands.Add(item);
			ResultsCommandProfile item2 = new ResultsCommandProfile
			{
				Command = new Command
				{
					Name = "Properties",
					Icon = Icons.DetailsTemplate,
					Text = Strings.EditDetailsTemplate
				},
				Setting = new ResultsCommandSetting
				{
					Operation = CommandOperation.Update,
					IsSelectionCommand = true,
					RequiresSingleSelection = true,
					IsPropertiesCommand = true,
					UseSingleRowRefresh = true
				},
				Action = new ShowDetailsTemplatesProperitiesAction()
			};
			base.CommandsProfile.ShowSelectionPropertiesCommands.Add(item2);
		}

		protected override ExchangeSettings CreatePrivateSettings(IComponent owner)
		{
			return new DetailsTemplatesEditorSettings(this);
		}

		protected override int MultiRowRefreshThreshold
		{
			get
			{
				return 1;
			}
		}

		internal const string TypeColumnName = "TemplateType";

		internal const string LanguageColumnName = "Language";

		private static IconLibrary iconLibrary = new IconLibrary();

		public class DetailsTemplatesFilterSchema : ObjectSchema
		{
			public static readonly ADPropertyDefinition Language = new ADPropertyDefinition("Language", ExchangeObjectVersion.Exchange2003, typeof(string), "Language", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

			public static readonly ADPropertyDefinition TemplateType = new ADPropertyDefinition("TemplateType", ExchangeObjectVersion.Exchange2003, typeof(string), "TemplateType", ADPropertyDefinitionFlags.ReadOnly, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
		}
	}
}
