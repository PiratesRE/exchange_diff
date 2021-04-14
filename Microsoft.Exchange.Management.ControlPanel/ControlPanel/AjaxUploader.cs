using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:AjaxUploader runat=server></{0}:AjaxUploader>")]
	[ClientScriptResource("AjaxUploader", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class AjaxUploader : ScriptControlBase
	{
		public AjaxUploader() : base(HtmlTextWriterTag.Div)
		{
			this.HasDefaultValue = true;
			HtmlGenericControl htmlGenericControl = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			htmlGenericControl.Attributes["class"] = "AjaxUploaderNameDiv";
			this.fileNameLabel = new EncodingLabel();
			this.fileNameLabel.ID = "fileNameLbl";
			htmlGenericControl.Controls.Add(this.fileNameLabel);
			this.progressLabel = new EncodingLabel();
			this.progressLabel.ID = "progressLbl";
			this.progressLabel.Text = Strings.Uploading;
			htmlGenericControl.Controls.Add(this.progressLabel);
			this.Controls.Add(htmlGenericControl);
			this.separator = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			this.separator.ID = "separator";
			this.separator.Attributes["class"] = "AjaxUploaderSeparator";
			this.Controls.Add(this.separator);
			htmlGenericControl = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			htmlGenericControl.Attributes["class"] = "AjaxUploaderNameDiv";
			this.cancelButton = new HyperLink();
			this.cancelButton.ID = "cancelBtn";
			this.cancelButton.NavigateUrl = "#";
			this.cancelButton.Text = Strings.CancelUpload;
			htmlGenericControl.Controls.Add(this.cancelButton);
			this.deleteButton = new HyperLink();
			this.deleteButton.ID = "deleteBtn";
			this.deleteButton.NavigateUrl = "#";
			CommandSprite commandSprite = new CommandSprite();
			commandSprite.ImageId = CommandSprite.SpriteId.ToolBarDeleteSmall;
			commandSprite.AlternateText = Strings.DeleteCommandText;
			this.deleteButton.Controls.Add(commandSprite);
			htmlGenericControl.Controls.Add(this.deleteButton);
			EncodingLabel child = Util.CreateHiddenForSRLabel(string.Empty, this.cancelButton.ID);
			EncodingLabel child2 = Util.CreateHiddenForSRLabel(string.Empty, this.deleteButton.ID);
			htmlGenericControl.Controls.Add(child);
			htmlGenericControl.Controls.Add(child2);
			this.Controls.Add(htmlGenericControl);
			this.Controls.Add(new LiteralControl("<br />"));
			htmlGenericControl = new HtmlGenericControl(HtmlTextWriterTag.Div.ToString());
			htmlGenericControl.Attributes["class"] = "AjaxUploaderButtonDiv";
			this.editFileButton = new IconButton();
			this.editFileButton.CssClass = "ajaxUploaderEditButton";
			this.editFileButton.ID = "editFileBtn";
			if (string.IsNullOrEmpty(this.editFileButton.Text))
			{
				this.editFileButton.Text = Strings.DefaultEditButtonText;
			}
			htmlGenericControl.Controls.Add(this.editFileButton);
			this.uploaderBase = new UploaderBase();
			this.uploaderBase.ID = "uploader";
			htmlGenericControl.Controls.Add(this.uploaderBase);
			this.Controls.Add(htmlGenericControl);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.deleteButton.Attributes["SetRoles"] = this.ChangeFileNameRoles;
			base.Attributes["SetRoles"] = this.ChangeFileNameRoles;
			string text = string.Empty;
			if (this.ChangeFileNameRoles != null)
			{
				text += this.ChangeFileNameRoles;
			}
			if (this.UploadRoles != null)
			{
				if (text != string.Empty)
				{
					text += "+";
				}
				text += this.UploadRoles;
			}
			if (text != string.Empty)
			{
				this.editFileButton.Attributes["SetRoles"] = text;
			}
			foreach (Binding item in this.Parameters)
			{
				this.uploaderBase.Parameters.Add(item);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public string EditButtonText
		{
			get
			{
				return this.editFileButton.Text;
			}
			set
			{
				this.editFileButton.Text = value;
			}
		}

		public string DefaultText { get; set; }

		public string UploadRoles { get; set; }

		public string ChangeFileNameRoles { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public BindingCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] ParameterNames
		{
			get
			{
				return this.uploaderBase.ParameterNames;
			}
			set
			{
				this.uploaderBase.ParameterNames = value;
			}
		}

		public UploadHandlers UploadHandlerClass
		{
			get
			{
				return this.uploaderBase.UploadHandlerClass;
			}
			set
			{
				this.uploaderBase.UploadHandlerClass = value;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Extensions
		{
			get
			{
				return this.uploaderBase.Extensions;
			}
			set
			{
				this.uploaderBase.Extensions = value;
			}
		}

		[DefaultValue(true)]
		public bool HasDefaultValue { get; set; }

		public bool InitStateAsEditClicked { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("DefaultText", this.DefaultText);
			descriptor.AddProperty("HasDefaultValue", this.HasDefaultValue);
			descriptor.AddProperty("InitStateAsEditClicked", this.InitStateAsEditClicked);
			descriptor.AddElementProperty("FileNameLbl", this.fileNameLabel.ClientID);
			descriptor.AddElementProperty("ProgressLbl", this.progressLabel.ClientID);
			descriptor.AddElementProperty("SeparatorDiv", this.separator.ClientID);
			descriptor.AddElementProperty("CancelBtn", this.cancelButton.ClientID);
			descriptor.AddElementProperty("DeleteBtn", this.deleteButton.ClientID);
			descriptor.AddElementProperty("EditFileBtn", this.editFileButton.ClientID);
			descriptor.AddComponentProperty("UploaderImplementation", this.uploaderBase.ClientID);
		}

		private EncodingLabel fileNameLabel;

		private EncodingLabel progressLabel;

		private HtmlGenericControl separator;

		private HyperLink cancelButton;

		protected HyperLink deleteButton;

		protected IconButton editFileButton;

		private UploaderBase uploaderBase;

		private BindingCollection parameters = new BindingCollection();
	}
}
