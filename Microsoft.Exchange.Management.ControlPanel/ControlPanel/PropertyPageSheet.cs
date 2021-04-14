using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("PropertyPageSheet", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class PropertyPageSheet : DataContextProvider, IBaseFormContentControl
	{
		public PropertyPageSheet()
		{
			this.Sections = new SectionCollection(this);
			base.ViewModel = "PropertyPageViewModel";
			this.UseSetObject = true;
			this.PreLoadData = true;
			this.HasSaveMethod = true;
			this.ReadOnDemand = false;
		}

		protected void ResolveServiceUrl()
		{
			string name = "sn";
			string text = this.Context.Request.QueryString[name];
			if (!string.IsNullOrWhiteSpace(text))
			{
				DDIHelper.CheckSchemaName(text);
				base.ServiceUrl = new WebServiceReference(EcpUrl.EcpVDirForStaticResource + "DDI/DDIService.svc?schema=" + text);
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (string.IsNullOrEmpty(this.CssClass))
			{
				this.CssClass = "propPane";
			}
			else
			{
				this.CssClass += " propPane";
			}
			base.AddAttributesToRender(writer);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.UseWarningPanel)
			{
				this.warningPanel = this.CreateWarningPanel("PropertyPaneWarningPanel");
				this.Controls.Add(this.warningPanel);
			}
			if (this.Content != null && this.Sections.Count > 0)
			{
				throw new NotSupportedException("PropertyPage control cannot have both sections and content");
			}
			if (this.Content != null)
			{
				PropertiesContentPanel propertiesContentPanel = new PropertiesContentPanel();
				propertiesContentPanel.ID = "contentContainer";
				this.Controls.Add(propertiesContentPanel);
				this.Content.InstantiateIn(propertiesContentPanel);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.ResolveServiceUrl();
			base["DataTransferMode"] = this.DataTransferMode;
			if (this.PreLoadData && base.ServiceUrl != null && this.DataTransferMode != DataTransferMode.Collaboration)
			{
				PowerShellResults powerShellResults;
				if (this.ReadOnDemand)
				{
					string workflowName = this.Sections[0].WorkflowName;
					powerShellResults = base.ServiceUrl.GetObjectOnDemand(this.ObjectIdentity, workflowName);
					powerShellResults.UseAsRbacScopeInCurrentHttpContext();
					this.InitialLoadedWorkflow = workflowName;
				}
				else if (this.UseSetObject)
				{
					powerShellResults = base.ServiceUrl.GetObject(this.ObjectIdentity);
					powerShellResults.UseAsRbacScopeInCurrentHttpContext();
				}
				else
				{
					powerShellResults = base.ServiceUrl.GetObjectForNew(this.ObjectIdentity);
				}
				base["PreLoadResults"] = powerShellResults;
			}
			foreach (Section section in this.Sections)
			{
				if (!string.IsNullOrEmpty(section.SetRoles) && !LoginUtil.IsInRoles(this.Context.User, section.SetRoles.Split(new char[]
				{
					','
				})))
				{
					section.Visible = false;
				}
			}
			base.OnPreRender(e);
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("UseWarningPanel", this.UseWarningPanel, true);
			descriptor.AddProperty("SuppressWarning", this.SuppressWarning, true);
			descriptor.AddProperty("HideErrors", this.HideErrors, true);
			descriptor.AddProperty("ReadOnDemand", this.ReadOnDemand, true);
			descriptor.AddProperty("InitialLoadedWorkflow", this.InitialLoadedWorkflow, true);
			if (this.warningPanel != null)
			{
				descriptor.AddElementProperty("WarningPanel", this.warningPanel.ClientID);
			}
		}

		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		[Description("Property Pane Content")]
		[TemplateContainer(typeof(PropertiesContentPanel))]
		public virtual ITemplate Content { get; set; }

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SectionCollection Sections { get; private set; }

		public string IdQueryStringField { get; set; }

		public Identity ObjectIdentity
		{
			get
			{
				if (this.objectIdentity == null)
				{
					string name = string.IsNullOrEmpty(this.IdQueryStringField) ? "id" : this.IdQueryStringField;
					string text = this.Context.Request.QueryString[name];
					if (!string.IsNullOrEmpty(text))
					{
						this.objectIdentity = Identity.ParseIdentity(text);
					}
				}
				return this.objectIdentity;
			}
			protected set
			{
				this.objectIdentity = value;
			}
		}

		public bool PreLoadData { get; set; }

		public bool ReadOnDemand { get; set; }

		private string InitialLoadedWorkflow { get; set; }

		private DataTransferMode DataTransferMode
		{
			get
			{
				DataTransferMode result = DataTransferMode.Default;
				string value = this.Context.Request["dtm"];
				if (!string.IsNullOrEmpty(value) && !Enum.TryParse<DataTransferMode>(value, out result))
				{
					throw new BadQueryParameterException("dtm");
				}
				return result;
			}
		}

		public bool UseSetObject
		{
			get
			{
				return ((bool?)base["UseSetObject"]) ?? true;
			}
			set
			{
				base["UseSetObject"] = value;
			}
		}

		public string SaveConfirmationText
		{
			get
			{
				return (string)base["SaveConfirmationText"];
			}
			set
			{
				base["SaveConfirmationText"] = value;
			}
		}

		public bool SuppressWarning { get; set; }

		public bool UseWarningPanel { get; set; }

		public bool HideErrors { get; set; }

		public bool HasSaveMethod { get; set; }

		WebServiceMethod IBaseFormContentControl.RefreshWebServiceMethod
		{
			get
			{
				return null;
			}
		}

		WebServiceMethod IBaseFormContentControl.SaveWebServiceMethod
		{
			get
			{
				return null;
			}
		}

		bool IBaseFormContentControl.ReadOnly
		{
			get
			{
				return false;
			}
		}

		private Panel warningPanel;

		private Identity objectIdentity;
	}
}
