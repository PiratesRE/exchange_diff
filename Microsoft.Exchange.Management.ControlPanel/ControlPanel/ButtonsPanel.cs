using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ToolboxData("<{0}:ButtonsPanel runat=\"server\" />")]
	public class ButtonsPanel : Panel, INamingContainer
	{
		public ButtonsPanel()
		{
			this.btnCommit = new HtmlButton();
			this.btnCancel = new HtmlButton();
			this.btnBack = new HtmlButton();
			this.btnNext = new HtmlButton();
			this.btnCommit.ID = "btnCommit";
			this.btnCommit.InnerText = Strings.CommitButtonText;
			this.btnCommit.Attributes.Add("data-control", "Button");
			this.btnCommit.Attributes.Add("data-Command", "{SaveCommand, Mode=OneWay}");
			this.btnCancel.ID = "btnCancel";
			this.btnCancel.InnerText = Strings.CancelButtonText;
			this.btnCancel.Attributes.Add("data-control", "Button");
			this.btnCancel.Attributes.Add("data-Command", "{CancelCommand, Mode=OneWay}");
			this.btnBack.ID = "btnBack";
			this.btnBack.InnerText = ClientStrings.Back;
			this.btnBack.Attributes.Add("data-control", "Button");
			this.btnBack.Attributes.Add("data-Command", "{BackCommand, Mode=OneWay}");
			this.btnBack.Visible = false;
			this.btnNext.ID = "btnNext";
			this.btnNext.InnerText = ClientStrings.Next;
			this.btnNext.Attributes.Add("data-control", "Button");
			this.btnNext.Attributes.Add("data-Command", "{NextCommand, Mode=OneWay}");
			this.btnNext.Visible = false;
			this.CssClass = "btnPane";
			this.ID = "ButtonsPanel";
			Util.RequireUpdateProgressPopUp(this);
			this.SaveWebServiceMethods = new List<WebServiceMethod>();
		}

		public HtmlButton CommitButton
		{
			get
			{
				return this.btnCommit;
			}
		}

		public ButtonsPanelState State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state != value)
				{
					this.state = value;
					switch (value)
					{
					case ButtonsPanelState.SaveCancel:
						this.btnCommit.Visible = true;
						this.btnCancel.Visible = true;
						this.CancelButtonText = Strings.CancelButtonText;
						return;
					case ButtonsPanelState.ReadOnly:
						this.btnCommit.Visible = false;
						this.btnCancel.Visible = true;
						this.CancelButtonText = ClientStrings.Close;
						return;
					case ButtonsPanelState.Save:
						this.btnCommit.Visible = true;
						this.btnCancel.Visible = false;
						return;
					case ButtonsPanelState.Wizard:
						this.btnBack.Visible = true;
						this.btnNext.Visible = true;
						this.btnCancel.Visible = true;
						this.btnCommit.Visible = true;
						this.CancelButtonText = Strings.CancelButtonText;
						this.CommitButtonText = Strings.FinishButtonText;
						break;
					default:
						return;
					}
				}
			}
		}

		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		[Bindable(true)]
		public string CommitButtonText
		{
			get
			{
				return this.btnCommit.InnerText;
			}
			set
			{
				this.btnCommit.InnerText = value;
			}
		}

		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		[Bindable(true)]
		public string CancelButtonText
		{
			get
			{
				return this.btnCancel.InnerText;
			}
			set
			{
				this.btnCancel.InnerText = value;
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[Category("Appearance")]
		[Localizable(true)]
		public string CancelButtonTextTagName
		{
			get
			{
				return this.btnCancel.Attributes["data-texttagname"];
			}
			set
			{
				this.btnCancel.Attributes.Add("data-texttagname", value);
			}
		}

		[Category("Behavior")]
		[Bindable(true)]
		[DefaultValue(false)]
		public bool CloseWindowOnCancel
		{
			get
			{
				return this.closeWindowOnCancel;
			}
			set
			{
				if (this.closeWindowOnCancel != value)
				{
					this.closeWindowOnCancel = value;
					if (this.closeWindowOnCancel)
					{
						this.btnCancel.Attributes["onclick"] = "window.close();";
					}
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue("")]
		public string CommitButtonUniqueID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnCommit.UniqueID;
			}
		}

		[Category("Behavior")]
		[DefaultValue("")]
		public string CommitButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnCommit.ClientID;
			}
		}

		public HtmlButton CancelButton
		{
			get
			{
				return this.btnCancel;
			}
		}

		[DefaultValue("")]
		[Category("Behavior")]
		public string CancelButtonUniqueID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnCancel.UniqueID;
			}
		}

		[Category("Behavior")]
		[DefaultValue("")]
		public string CancelButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnCancel.ClientID;
			}
		}

		[DefaultValue("")]
		[Category("Behavior")]
		public string BackButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnBack.ClientID;
			}
		}

		public HtmlButton BackButton
		{
			get
			{
				return this.btnBack;
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[Localizable(true)]
		[Category("Appearance")]
		public string BackButtonText
		{
			get
			{
				return this.btnBack.InnerText;
			}
			set
			{
				this.btnBack.InnerText = value;
			}
		}

		public HtmlButton NextButton
		{
			get
			{
				return this.btnNext;
			}
		}

		[Category("Appearance")]
		[Localizable(true)]
		[DefaultValue("")]
		[Bindable(true)]
		public string NextButtonText
		{
			get
			{
				return this.btnNext.InnerText;
			}
			set
			{
				this.btnNext.InnerText = value;
			}
		}

		[DefaultValue("")]
		[Category("Behavior")]
		public string NextButtonUniqueID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnNext.UniqueID;
			}
		}

		[DefaultValue("")]
		[Category("Behavior")]
		public string NextButtonClientID
		{
			get
			{
				this.EnsureChildControls();
				return this.btnNext.ClientID;
			}
		}

		public WebServiceMethod LoadWebServiceMethod { get; set; }

		public List<WebServiceMethod> SaveWebServiceMethods { get; private set; }

		protected override void OnPreRender(EventArgs e)
		{
			int count = this.SaveWebServiceMethods.Count;
			if (this.State != ButtonsPanelState.ReadOnly && count > 0)
			{
				this.invokeSaveWebService = new InvokeWebService();
				this.invokeSaveWebService.ID = "webServiceBehaviorForCommit";
				this.invokeSaveWebService.TargetControlID = this.btnCommit.ID;
				this.invokeSaveWebService.EnableConfirmation = true;
				this.invokeSaveWebService.EnableProgressPopup = true;
				this.invokeSaveWebService.IsSaveMethod = true;
				if (this.State == ButtonsPanelState.SaveCancel || this.State == ButtonsPanelState.Wizard)
				{
					this.invokeSaveWebService.CloseAfterSuccess = true;
				}
				if (this.State == ButtonsPanelState.SaveCancel)
				{
					if (count > 1)
					{
						throw new InvalidOperationException("Have more than one SaveWebServiceMethod while ButtonPanel state is SaveCancel.");
					}
					this.invokeSaveWebService.AssociateElementID = this.btnCancel.ClientID;
				}
				else if (this.State == ButtonsPanelState.Wizard)
				{
					this.invokeSaveWebService.AssociateElementID = this.btnBack.ClientID + "," + this.btnCancel.ClientID;
				}
				this.Controls.Add(this.invokeSaveWebService);
			}
			if (this.State == ButtonsPanelState.SaveCancel && this.LoadWebServiceMethod != null)
			{
				InvokeWebService invokeWebService = new InvokeWebService();
				invokeWebService.ID = "webServiceBehaviorForReload";
				invokeWebService.TargetControlID = this.btnCancel.ID;
				invokeWebService.WebServiceMethods.Add(this.LoadWebServiceMethod);
				this.Controls.Add(invokeWebService);
				InvokeWebService invokeWebService2 = new InvokeWebService();
				invokeWebService2.ID = "webServiceBehaviorForDisableSave";
				invokeWebService2.TargetControlID = this.btnCommit.ID;
				invokeWebService2.Trigger = string.Empty;
				invokeWebService2.WebServiceMethods.Add(this.LoadWebServiceMethod);
				this.Controls.Add(invokeWebService2);
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.State == ButtonsPanelState.Save || this.State == ButtonsPanelState.SaveCancel)
			{
				this.btnCommit.Attributes.Add("data-visible", "{IsReadOnly, Mode=OneWay, ConvertTo=ValueConverter.Not}");
				this.btnCancel.Attributes.Add("data-text", "{IsReadOnly, Mode=OneWay, ConvertTo=ValueConverter.IIF, ConverterParameter=json:[ImportedStrings.Close,'" + HttpUtility.JavaScriptStringEncode(this.CancelButtonText) + "']}");
			}
			if (this.invokeSaveWebService != null)
			{
				this.invokeSaveWebService.WebServiceMethods.Clear();
				this.invokeSaveWebService.WebServiceMethods.AddRange(this.SaveWebServiceMethods);
			}
			base.Render(writer);
		}

		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			switch (this.State)
			{
			case ButtonsPanelState.SaveCancel:
				this.Controls.Add(this.btnCommit);
				this.Controls.Add(this.btnCancel);
				return;
			case ButtonsPanelState.ReadOnly:
				this.Controls.Add(this.btnCancel);
				return;
			case ButtonsPanelState.Save:
				this.Controls.Add(this.btnCommit);
				return;
			case ButtonsPanelState.Wizard:
				this.Controls.Add(this.btnBack);
				this.Controls.Add(this.btnNext);
				this.Controls.Add(this.btnCommit);
				this.Controls.Add(this.btnCancel);
				return;
			default:
				return;
			}
		}

		private const string CloseWindowScript = "window.close();";

		private HtmlButton btnCommit;

		private HtmlButton btnCancel;

		private HtmlButton btnBack;

		private HtmlButton btnNext;

		private bool closeWindowOnCancel;

		private InvokeWebService invokeSaveWebService;

		private ButtonsPanelState state;
	}
}
