using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("DLPPicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ToolboxData("<{0}:DLPPicker runat=server></{0}:DLPPicker>")]
	public class DLPPicker : ScriptControlBase
	{
		public DLPPicker() : base(HtmlTextWriterTag.Div)
		{
			this.ecpCollectionEditor = new DLPCollectionEditor();
		}

		public string EcpCollectionEditorID
		{
			get
			{
				this.EnsureChildControls();
				return this.ecpCollectionEditor.ClientID;
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ECPCollectionEditor", this.EcpCollectionEditorID, this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.ecpCollectionEditor.ID = "dlpCollectionEditor";
			this.ecpCollectionEditor.EmptyDataText = Strings.DLPPickerEmptyText;
			this.ecpCollectionEditor.ShowListviewHeader = true;
			this.ecpCollectionEditor.Height = Unit.Pixel(400);
			this.ecpCollectionEditor.Width = Unit.Pixel(650);
			this.ecpCollectionEditor.PickerFormUrl = "~/DLPPolicy/SensitiveInformationPicker.aspx";
			this.ecpCollectionEditor.EditCommandUrl = "~/RulesEditor/EditDLPAttributes.aspx";
			this.ecpCollectionEditor.ValueProperty = "name";
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = "displayName";
			columnHeader.Text = Strings.DLPPickerName;
			columnHeader.Width = Unit.Percentage(20.0);
			ColumnHeader columnHeader2 = new ColumnHeader();
			columnHeader2.Name = "minCount";
			columnHeader2.Text = Strings.DLPPickerMinCount;
			columnHeader2.Width = Unit.Percentage(20.0);
			columnHeader2.EmptyText = Strings.DLPAny;
			ColumnHeader columnHeader3 = new ColumnHeader();
			columnHeader3.Name = "maxCount";
			columnHeader3.Text = Strings.DLPPickerMaxCount;
			columnHeader3.Width = Unit.Percentage(20.0);
			columnHeader3.EmptyText = Strings.DLPAny;
			ColumnHeader columnHeader4 = new ColumnHeader();
			columnHeader4.Name = "minConfidence";
			columnHeader4.Text = Strings.DLPPickerMinConfidence;
			columnHeader4.Width = Unit.Percentage(20.0);
			columnHeader4.EmptyText = Strings.DLPDefault;
			ColumnHeader columnHeader5 = new ColumnHeader();
			columnHeader5.Name = "maxConfidence";
			columnHeader5.Text = Strings.DLPPickerMaxConfidence;
			columnHeader5.Width = Unit.Percentage(20.0);
			columnHeader5.EmptyText = Strings.DLPDefault;
			this.ecpCollectionEditor.Columns.Add(columnHeader);
			this.ecpCollectionEditor.Columns.Add(columnHeader2);
			this.ecpCollectionEditor.Columns.Add(columnHeader3);
			this.ecpCollectionEditor.Columns.Add(columnHeader4);
			this.ecpCollectionEditor.Columns.Add(columnHeader5);
			this.ecpCollectionEditor.Columns.ForEach(delegate(ColumnHeader f)
			{
				f.DefaultOff = false;
			});
			this.ecpCollectionEditor.AddCommandDialogHeight = 450;
			this.ecpCollectionEditor.AddCommandDialogWidth = 585;
			this.ecpCollectionEditor.EditCommandDialogHeight = 400;
			this.ecpCollectionEditor.EditCommandDialogWidth = 575;
			this.ecpCollectionEditor.UseModalessForAdd = true;
			this.Controls.Add(this.ecpCollectionEditor);
		}

		public const string ID_KEY = "name";

		public const string DISPLAYNAME_KEY = "displayName";

		public const string MINCOUNT_KEY = "minCount";

		public const string MAXCOUNT_KEY = "maxCount";

		public const string MINCONFIDENCE_KEY = "minConfidence";

		public const string MAXCONFIDENCE_KEY = "maxConfidence";

		private DLPCollectionEditor ecpCollectionEditor;
	}
}
