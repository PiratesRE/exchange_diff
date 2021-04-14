using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:ADAttributePicker runat=server></{0}:ADAttributePicker>")]
	[ClientScriptResource("ADAttributePicker", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	public class ADAttributePicker : ScriptControlBase
	{
		public ADAttributePicker() : base(HtmlTextWriterTag.Div)
		{
			this.ecpCollectionEditor = new NameValueCollectionEditor();
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
			this.ecpCollectionEditor.ID = "ecpCollectionEditor";
			this.ecpCollectionEditor.Height = Unit.Pixel(200);
			this.ecpCollectionEditor.Width = Unit.Pixel(300);
			this.ecpCollectionEditor.PickerFormUrl = "~/RulesEditor/ADAttribute.aspx";
			this.ecpCollectionEditor.EditCommandUrl = "~/RulesEditor/ADAttribute.aspx";
			this.ecpCollectionEditor.ValueProperty = "Identity";
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = "Name";
			columnHeader.Text = Strings.ADAttributeNameColumnName;
			columnHeader.Width = Unit.Percentage(50.0);
			ColumnHeader columnHeader2 = new ColumnHeader();
			columnHeader2.Name = "Value";
			columnHeader2.Text = Strings.ADAttributeValueColumnName;
			columnHeader2.Width = Unit.Percentage(50.0);
			this.ecpCollectionEditor.Columns.Add(columnHeader);
			this.ecpCollectionEditor.Columns.Add(columnHeader2);
			this.ecpCollectionEditor.DialogHeight = 210;
			this.ecpCollectionEditor.DialogWidth = 585;
			this.Controls.Add(this.ecpCollectionEditor);
		}

		private NameValueCollectionEditor ecpCollectionEditor;
	}
}
