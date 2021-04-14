using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("InlineEditor", "Microsoft.Exchange.Management.ControlPanel.Client.Rules.js")]
	[ControlValueProperty("Value")]
	[ToolboxData("<{0}:InlineEditor runat=server></{0}:InlineEditor>")]
	public class InlineEditor : ScriptControlBase, INamingContainer
	{
		public InlineEditor()
		{
			this.listview = new ListView();
			this.listSource = new ListSource();
			this.MaxLength = 128;
			this.listSource.ID = "DataSource";
			this.listview.IdentityProperty = null;
			this.listview.NameProperty = null;
			this.listview.WarningAsError = true;
			this.ValidationExpression = string.Empty;
			this.CssClass = "InlineEditor";
			this.ListViewCssClass = "InlineEditorListView";
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ListView", this.ListViewID, this);
			descriptor.AddComponentProperty("DataSource", this.ListSourceID, this);
			if (!string.IsNullOrEmpty(this.ValidationExpression))
			{
				descriptor.AddScriptProperty("ValidationExpression", "function($_) { return /" + this.ValidationExpression + "/.test($_) }");
				descriptor.AddProperty("ValidationErrorMessage", this.ValidationErrorMessage);
			}
			if (this.DuplicateHandlingType != DuplicateHandlingType.AllowDuplicate)
			{
				descriptor.AddProperty("DuplicateHandlingType", this.DuplicateHandlingType);
			}
			if (this.ReadOnly)
			{
				descriptor.AddProperty("ReadOnly", true);
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(this.listSource);
			this.listview.ID = "listview";
			this.listview.Height = Unit.Percentage(100.0);
			this.listview.AllowSorting = false;
			this.listview.SortProperty = null;
			this.listview.ShowSearchBar = false;
			this.listview.ShowHeader = false;
			this.listview.ShowTitle = false;
			this.listview.ShowStatus = false;
			this.listview.DataSourceID = this.listSource.ID;
			this.listview.IsEditable = true;
			this.listview.EmptyDataText = null;
			this.listview.Columns.Add(new ColumnHeader
			{
				Width = Unit.Percentage(100.0)
			});
			this.editCommand = new Command(string.Empty, CommandSprite.SpriteId.ToolBarProperties);
			this.editCommand.ImageAltText = Strings.EditCommandText;
			this.editCommand.SelectionMode = SelectionMode.RequiresSingleSelectionDisabledOnInlineEdit;
			this.editCommand.OnClientClick = "$find('" + this.ClientID + "').editCommand();";
			this.editCommand.DefaultCommand = true;
			this.listview.Commands.Add(this.editCommand);
			this.removeCommand = new RemoveCommand(false);
			this.listview.Commands.Add(this.removeCommand);
			this.Controls.Add(this.listview);
		}

		public string ListViewID
		{
			get
			{
				return this.listview.ClientID;
			}
		}

		public string ListSourceID
		{
			get
			{
				return this.listSource.ClientID;
			}
		}

		[DefaultValue(128)]
		public int MaxLength
		{
			get
			{
				return this.listview.InlineEditMaxLength;
			}
			set
			{
				this.listview.InlineEditMaxLength = value;
			}
		}

		public string InputWaterMarkText
		{
			get
			{
				return this.listview.InputWaterMarkText;
			}
			set
			{
				this.listview.InputWaterMarkText = value;
			}
		}

		[DefaultValue("InlineEditorListView")]
		public string ListViewCssClass
		{
			get
			{
				return this.listview.CssClass;
			}
			set
			{
				this.listview.CssClass = value + " " + this.ListViewCssClass;
			}
		}

		public DuplicateHandlingType DuplicateHandlingType { get; set; }

		public string ValidationExpression { get; set; }

		public string ValidationErrorMessage { get; set; }

		[Bindable(true)]
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (this.ReadOnly)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
			}
			base.AddAttributesToRender(writer);
		}

		private ListSource listSource;

		private ListView listview;

		private Command removeCommand;

		private Command editCommand;

		private bool readOnly;
	}
}
