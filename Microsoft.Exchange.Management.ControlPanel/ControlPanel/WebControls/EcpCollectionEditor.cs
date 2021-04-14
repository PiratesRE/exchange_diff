using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:EcpCollectionEditor runat=server></{0}:EcpCollectionEditor>")]
	[ClientScriptResource("EcpCollectionEditor", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ControlValueProperty("Values")]
	[DefaultProperty("Text")]
	public class EcpCollectionEditor : ScriptControlBase, INamingContainer
	{
		public EcpCollectionEditor()
		{
			this.listview = new ListView();
			this.showListviewHeader = true;
			this.listSource = new ListSource();
			this.listSource.ID = "DataSource";
			this.ListViewCssClass = "collectionEditorListView";
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (this.ValueProperty != "Identity")
			{
				descriptor.AddProperty("ValueProperty", this.ValueProperty);
			}
			descriptor.AddUrlProperty("EditCommandUrl", this.EditCommandUrl, this);
			descriptor.AddUrlProperty("PickerFormUrl", this.PickerFormUrl, this);
			descriptor.AddProperty("SplitAddRemove", this.SplitAddRemove, true);
			descriptor.AddProperty("UseModalPopup", this.AlwaysOpenModalDialog || !string.IsNullOrEmpty(this.EditCommandUrl));
			descriptor.AddProperty("UseModalessForAdd", this.UseModalessForAdd);
			descriptor.AddUrlProperty("WholeListEditorUrl", this.WholeListEditorUrl, this);
			if (this.DialogHeight != 530)
			{
				descriptor.AddProperty("DialogHeight", this.DialogHeight);
			}
			if (this.DialogWidth != 450)
			{
				descriptor.AddProperty("DialogWidth", this.DialogWidth);
			}
			if (this.AddCommandDialogHeight != 0)
			{
				descriptor.AddProperty("AddCommandDialogHeight", this.AddCommandDialogHeight);
			}
			if (this.AddCommandDialogWidth != 0)
			{
				descriptor.AddProperty("AddCommandDialogWidth", this.AddCommandDialogWidth);
			}
			if (this.EditCommandDialogHeight != 0)
			{
				descriptor.AddProperty("EditCommandDialogHeight", this.EditCommandDialogHeight);
			}
			if (this.EditCommandDialogWidth != 0)
			{
				descriptor.AddProperty("EditCommandDialogWidth", this.EditCommandDialogWidth);
			}
			descriptor.AddComponentProperty("ListView", this.ListViewID, this);
			descriptor.AddComponentProperty("DataSource", this.ListSourceID, this);
			if (this.ReadOnly)
			{
				descriptor.AddProperty("ReadOnly", true);
			}
			descriptor.AddProperty("IsUsingOwaPeoplePicker", OwaPickerUtil.CanUseOwaPicker && this.IsUsingOwaPeoplePicker);
			descriptor.AddProperty("PickerCallerType", this.PickerCallerType);
			descriptor.AddProperty("IgnoreCaseWhenComparison", this.IgnoreCaseWhenComparison);
			descriptor.AddProperty("KeepOrderChange", this.KeepOrderChange);
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		protected ListView Listview
		{
			get
			{
				return this.listview;
			}
		}

		public bool ShowListviewHeader
		{
			get
			{
				return this.showListviewHeader;
			}
			set
			{
				this.showListviewHeader = value;
			}
		}

		protected Command EditCommand
		{
			get
			{
				return this.editCommand;
			}
			set
			{
				this.editCommand = value;
			}
		}

		[IDReferenceProperty(typeof(DataSourceControl))]
		public string DataSourceID
		{
			get
			{
				return this.listSource.DataSourceID;
			}
			set
			{
				this.listSource.DataSourceID = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue("")]
		[Themeable(false)]
		[Bindable(true)]
		public object DataSource
		{
			get
			{
				return this.listSource.DataSource;
			}
			set
			{
				if (value is MultiValuedProperty<ADObjectId>)
				{
					value = RecipientObjectResolver.Instance.ResolveObjects(((MultiValuedProperty<ADObjectId>)value).ToArray()).ToArray<RecipientObjectResolverRow>();
				}
				this.listSource.DataSource = value;
			}
		}

		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue("")]
		public List<ColumnHeader> Columns
		{
			get
			{
				return this.listview.Columns;
			}
		}

		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public CommandCollection Commands
		{
			get
			{
				return this.commands;
			}
		}

		[TemplateContainer(typeof(PropertiesContentPanel))]
		[Description("Content to build query parameters")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		[DefaultValue(null)]
		public virtual ITemplate Content { get; set; }

		protected virtual void InitListviewCommandCollection()
		{
			if (!string.IsNullOrEmpty(this.ViewDetailsCommandUrl))
			{
				this.viewDetailsCommand.Name = "ViewDetails";
				this.viewDetailsCommand.NavigateUrl = this.ViewDetailsCommandUrl;
				if (this.viewDetailsDialogSize != Size.Empty)
				{
					this.viewDetailsCommand.DialogSize = this.viewDetailsDialogSize;
				}
				this.listview.Commands.Add(this.viewDetailsCommand);
			}
			if (!string.IsNullOrEmpty(this.WholeListEditorUrl))
			{
				this.editWholeListCommand = new Command(string.Empty, CommandSprite.SpriteId.ToolBarProperties);
				this.editWholeListCommand.SelectionMode = SelectionMode.SelectionIndependent;
				this.editWholeListCommand.OnClientClick = "$find('" + this.ClientID + "').editWholeListCommand();";
				this.editWholeListCommand.ImageAltText = Strings.EditCommandText;
				this.listview.Commands.Add(this.editWholeListCommand);
				return;
			}
			if (!this.ReadOnly)
			{
				string[] roles = (!string.IsNullOrEmpty(base.Attributes["SetRoles"])) ? base.Attributes["SetRoles"].ToArrayOfStrings() : null;
				this.addCommand.Name = "Add";
				this.addCommand.ImageAltText = Strings.CollectionEditorAddText;
				this.addCommand.SelectionMode = SelectionMode.SelectionIndependent;
				this.addCommand.OnClientClick = "$find('" + this.ClientID + "').addCommand();";
				this.addCommand.Roles = roles;
				this.listview.Commands.Add(this.addCommand);
				if (!string.IsNullOrEmpty(this.EditCommandUrl))
				{
					this.editCommand = new Command(string.Empty, CommandSprite.SpriteId.ToolBarProperties);
					this.editCommand.ImageAltText = Strings.EditCommandText;
					this.editCommand.SelectionMode = SelectionMode.RequiresSingleSelection;
					this.editCommand.DefaultCommand = true;
					this.editCommand.OnClientClick = "$find('" + this.ClientID + "').editCommand();";
					this.editCommand.Roles = roles;
					this.listview.Commands.Add(this.editCommand);
				}
				if (!this.DisableRemove)
				{
					this.removeCommand.Roles = roles;
					this.listview.Commands.Add(this.removeCommand);
				}
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(this.listSource);
			this.listview.CssClass = this.ListViewCssClass;
			this.listview.ID = "listview";
			this.listview.Height = Unit.Percentage(100.0);
			this.listview.ShowSearchBar = false;
			this.listview.ShowHeader = false;
			if (this.showListviewHeader)
			{
				foreach (ColumnHeader columnHeader in this.listview.Columns)
				{
					if (!string.IsNullOrEmpty(columnHeader.Text))
					{
						this.listview.ShowHeader = true;
						break;
					}
				}
			}
			this.listview.ShowTitle = false;
			this.listview.ShowStatus = false;
			this.listview.EmptyDataText = this.EmptyDataText;
			this.listview.DataSourceID = this.listSource.ID;
			this.InitListviewCommandCollection();
			this.Controls.Add(this.listview);
			if (this.Content != null)
			{
				PropertiesContentPanel propertiesContentPanel = new PropertiesContentPanel();
				propertiesContentPanel.ID = "contentContainer";
				this.Controls.Add(propertiesContentPanel);
				this.Content.InstantiateIn(propertiesContentPanel);
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (this.ReadOnly)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
			}
			this.CssClass = "EcpCollectionEditor " + this.CssClass;
			if (this.ReadOnly && string.IsNullOrEmpty(this.ViewDetailsCommandUrl))
			{
				this.CssClass += " EcpCollectionEditorReadOnly";
			}
			base.AddAttributesToRender(writer);
		}

		public string ViewDetailsCommandUrl
		{
			get
			{
				return this.viewDetailsUrl;
			}
			set
			{
				this.viewDetailsUrl = value;
			}
		}

		public ViewDetailsCommand ViewDetailsCommand
		{
			get
			{
				return this.viewDetailsCommand;
			}
		}

		public string EditCommandUrl
		{
			get
			{
				return this.editUrl;
			}
			set
			{
				this.editUrl = value;
			}
		}

		[DefaultValue(false)]
		public bool SplitAddRemove { get; set; }

		public Size ViewDetailsDialogSize
		{
			get
			{
				return this.viewDetailsDialogSize;
			}
			set
			{
				this.viewDetailsDialogSize = value;
			}
		}

		[DefaultValue("Identity")]
		public string ValueProperty
		{
			get
			{
				return this.listview.IdentityProperty;
			}
			set
			{
				this.listview.IdentityProperty = value;
			}
		}

		[DefaultValue("")]
		[UrlProperty("*.aspx")]
		public string PickerFormUrl
		{
			get
			{
				return this.pickerFormUrl;
			}
			set
			{
				this.pickerFormUrl = value;
			}
		}

		[UrlProperty("*.aspx")]
		[DefaultValue("")]
		public string WholeListEditorUrl { get; set; }

		[DefaultValue(530)]
		public int DialogHeight
		{
			get
			{
				return this.dialogHeight;
			}
			set
			{
				this.dialogHeight = value;
			}
		}

		[DefaultValue(450)]
		public int DialogWidth
		{
			get
			{
				return this.dialogWidth;
			}
			set
			{
				this.dialogWidth = value;
			}
		}

		[DefaultValue(0)]
		public int AddCommandDialogHeight
		{
			get
			{
				return this.addCommandDialogHeight;
			}
			set
			{
				this.addCommandDialogHeight = value;
			}
		}

		[DefaultValue(0)]
		public int AddCommandDialogWidth
		{
			get
			{
				return this.addCommandDialogWidth;
			}
			set
			{
				this.addCommandDialogWidth = value;
			}
		}

		[DefaultValue(0)]
		public int EditCommandDialogHeight
		{
			get
			{
				return this.editCommandDialogHeight;
			}
			set
			{
				this.editCommandDialogHeight = value;
			}
		}

		[DefaultValue(0)]
		public int EditCommandDialogWidth
		{
			get
			{
				return this.editCommandDialogWidth;
			}
			set
			{
				this.editCommandDialogWidth = value;
			}
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

		public Command AddCommand
		{
			get
			{
				return this.addCommand;
			}
		}

		public Command RemoveCommand
		{
			get
			{
				return this.removeCommand;
			}
		}

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
				if (this.ReadOnly != value)
				{
					this.readOnly = value;
					if (this.addCommand != null)
					{
						this.addCommand.Visible = !value;
					}
					if (this.editCommand != null)
					{
						this.editCommand.Visible = !value;
					}
					if (this.removeCommand != null)
					{
						this.removeCommand.Visible = !value;
					}
				}
			}
		}

		public bool DisableRemove { get; set; }

		[Localizable(true)]
		[DefaultValue("")]
		public string EmptyDataText
		{
			get
			{
				return this.emptyDataText;
			}
			set
			{
				this.emptyDataText = (string.IsNullOrEmpty(value) ? " " : value);
			}
		}

		[Themeable(true)]
		[DefaultValue(null)]
		public string SpriteProperty
		{
			get
			{
				return this.listview.SpriteProperty;
			}
			set
			{
				this.listview.SpriteProperty = value;
			}
		}

		[Themeable(true)]
		[DefaultValue(null)]
		public string SpriteAltTextProperty
		{
			get
			{
				return this.listview.SpriteAltTextProperty;
			}
			set
			{
				this.listview.SpriteAltTextProperty = value;
			}
		}

		[DefaultValue(false)]
		public bool AlwaysOpenModalDialog { get; set; }

		[DefaultValue(true)]
		public bool AllowSorting
		{
			get
			{
				return this.listview != null && this.listview.AllowSorting;
			}
			set
			{
				if (this.listview != null)
				{
					this.listview.AllowSorting = value;
				}
			}
		}

		[DefaultValue(false)]
		public bool UseModalessForAdd { get; set; }

		[DefaultValue(false)]
		public bool IsUsingOwaPeoplePicker { get; set; }

		[DefaultValue(false)]
		public bool IgnoreCaseWhenComparison { get; set; }

		[DefaultValue(false)]
		public bool KeepOrderChange { get; set; }

		public PickerCallerType PickerCallerType { get; set; }

		[DefaultValue("collectionEditorListView")]
		[Description("The CSS class of ListView in the control.")]
		[DisplayName("ListView Css Class")]
		public string ListViewCssClass
		{
			get
			{
				return this.listViewCssClass;
			}
			set
			{
				this.listViewCssClass = value;
			}
		}

		[Bindable(true)]
		[DefaultValue(false)]
		public bool BindOnDataSourceItem
		{
			get
			{
				return this.bindOnDataSourceItem;
			}
			set
			{
				this.bindOnDataSourceItem = value;
			}
		}

		public string OnClientItemUpdated
		{
			set
			{
				this.Listview.OnClientItemUpdated = value;
			}
		}

		private string emptyDataText;

		private ListSource listSource;

		private ListView listview;

		private Command addCommand = new Command(string.Empty, CommandSprite.SpriteId.MetroAdd);

		private Command removeCommand = new RemoveCommand(false);

		private Command editWholeListCommand;

		private string pickerFormUrl;

		private ViewDetailsCommand viewDetailsCommand = new ViewDetailsCommand();

		private string viewDetailsUrl;

		private Command editCommand;

		private string editUrl;

		private bool showListviewHeader;

		private Size viewDetailsDialogSize = Size.Empty;

		private bool bindOnDataSourceItem;

		private bool readOnly;

		private string listViewCssClass;

		private CommandCollection commands = new CommandCollection();

		private int dialogHeight = 530;

		private int dialogWidth = 450;

		private int addCommandDialogHeight;

		private int addCommandDialogWidth;

		private int editCommandDialogHeight;

		private int editCommandDialogWidth;
	}
}
