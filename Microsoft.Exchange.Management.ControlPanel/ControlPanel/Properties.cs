using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[PersistChildren(true)]
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[RequiredScript(typeof(WebServiceMethod))]
	[ParseChildren(true)]
	[ToolboxData("<{0}:Properties runat=server></{0}:Properties>")]
	public class Properties : DataBoundControl, IBaseFormContentControl, IScriptControl
	{
		public Properties()
		{
			this.ID = "Properties";
			this.UseSetObject = true;
			this.HasSaveMethod = true;
			this.SkipReadOnlyCheck = false;
			this.Sections = new SectionCollection(this);
			this.ExceptionHandlers = new List<WebServiceExceptionHandler>();
			this.CaptionTextField = "Identity.DisplayName";
			this.UrlRequiresId = true;
			this.Bindings = new DataContractBinding();
		}

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

		public string IdQueryStringField { get; set; }

		public WebServiceReference ServiceUrl { get; set; }

		public WebServiceMethod RefreshWebServiceMethod { get; private set; }

		public WebServiceMethod SaveWebServiceMethod { get; private set; }

		public bool UseSetObject { get; set; }

		public bool? AlwaysInvokeSave { get; set; }

		public bool GetObjectForNew { get; set; }

		public bool HasSaveMethod { get; set; }

		public string SaveMethod { get; set; }

		[DefaultValue(false)]
		public bool SkipReadOnlyCheck { get; set; }

		public bool ReadOnly
		{
			get
			{
				return !this.SkipReadOnlyCheck && this.allControlsDisabled;
			}
		}

		public string SaveMethodExpression { get; set; }

		public WebServiceParameterNames ParameterSet { get; set; }

		public string OnRefreshSucceed { get; set; }

		public bool UrlRequiresId { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public SectionCollection Sections { get; private set; }

		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<WebServiceExceptionHandler> ExceptionHandlers { get; private set; }

		[TemplateContainer(typeof(PropertiesContentPanel))]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Browsable(false)]
		[DefaultValue(null)]
		[Description("Property Pane Content")]
		[TemplateInstance(TemplateInstance.Single)]
		public virtual ITemplate Content { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ControlCollection Controls
		{
			get
			{
				this.EnsureChildControls();
				return base.Controls;
			}
		}

		[DefaultValue("Identity.DisplayName")]
		public string CaptionTextField { get; set; }

		public string NameProperty { get; set; }

		public bool UseWarningPanel { get; set; }

		public bool SuppressWarning { get; set; }

		public bool HideClientValidationError { get; set; }

		public string SaveConfirmationText { get; set; }

		protected PowerShellResults Results { get; set; }

		protected DataContractBinding Bindings { get; set; }

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		private bool IsResultBindingAllowed
		{
			get
			{
				return this.RefreshWebServiceMethod != null || (this.Page is BaseForm && (this.Page as BaseForm).PassingDataOnClient);
			}
		}

		private bool RequireDataAtInitialize
		{
			get
			{
				return this.UseSetObject || this.GetObjectForNew;
			}
		}

		public PropertiesContentPanel ContentContainer { get; private set; }

		public void AddBinding(string contractPropertyName, Control targetControl, string clientPropertyName)
		{
			this.Bindings.Bindings.Add(contractPropertyName, new ClientControlBinding(targetControl, clientPropertyName));
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return new ScriptControlDescriptor[]
			{
				this.GetScriptDescriptor()
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(base.GetType());
		}

		internal static void ApplyRolesFilterRecursive(Control c, IPrincipal currentUser, IVersionable versionableObject)
		{
			IAttributeAccessor attributeAccessor = c as IAttributeAccessor;
			if (attributeAccessor != null)
			{
				string attribute = attributeAccessor.GetAttribute("SetRoles");
				string attribute2 = attributeAccessor.GetAttribute("DataBoundProperty");
				PropertyDefinition propertyDefinition = (versionableObject != null && !string.IsNullOrEmpty(attribute2)) ? versionableObject.ObjectSchema[attribute2] : null;
				if (propertyDefinition != null && !versionableObject.IsPropertyAccessible(propertyDefinition))
				{
					Properties.HideControl(c, Properties.FindAssociatedLabel(c));
				}
				else if ((!string.IsNullOrEmpty(attribute) && !LoginUtil.IsInRoles(currentUser, attribute.Split(new char[]
				{
					','
				}))) || (!string.IsNullOrEmpty(attribute2) && versionableObject != null && versionableObject.IsReadOnly))
				{
					string attribute3 = attributeAccessor.GetAttribute("NoRoleState");
					Label associatedLabel = Properties.FindAssociatedLabel(c);
					if (!string.IsNullOrEmpty(attribute3) && NoRoleState.Hide == (NoRoleState)Enum.Parse(typeof(NoRoleState), attribute3))
					{
						Properties.HideControl(c, associatedLabel);
					}
					else
					{
						Properties.MakeControlRbacDisabled(c, associatedLabel);
						if (!string.IsNullOrEmpty(attributeAccessor.GetAttribute("helpId")))
						{
							attributeAccessor.SetAttribute("helpId", string.Empty);
						}
						attributeAccessor.SetAttribute("MandatoryParam", null);
					}
				}
			}
			if (c.HasControls())
			{
				foreach (object obj in c.Controls)
				{
					Control c2 = (Control)obj;
					Properties.ApplyRolesFilterRecursive(c2, currentUser, versionableObject);
				}
			}
		}

		internal static Label FindAssociatedLabel(Control control)
		{
			return control.NamingContainer.FindControl(control.ID + "_label") as Label;
		}

		internal static void HideControl(Control c, Label associatedLabel)
		{
			c.Visible = false;
			if (associatedLabel != null)
			{
				associatedLabel.Visible = false;
			}
		}

		internal static void MakeControlRbacDisabled(Control c, Label associatedLabel)
		{
			Util.MakeControlRbacDisabled(c);
			if (associatedLabel != null)
			{
				associatedLabel.Enabled = false;
				Util.MarkRBACDisabled(associatedLabel);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.EnsureChildControls();
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

		protected override void Render(HtmlTextWriter writer)
		{
			this.RemoveMetaAttributes(this);
			base.Render(writer);
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.warningPanel = this.CreateWarningPanel("PropertyPaneWarningPanel");
			this.Controls.Add(this.warningPanel);
			if (this.Content != null && this.Sections.Count > 0)
			{
				throw new NotSupportedException("Properties control cannot have both sections and content");
			}
			if (this.Content != null)
			{
				this.ContentContainer = new PropertiesContentPanel();
				this.ContentContainer.ID = "contentContainer";
				this.Controls.Add(this.ContentContainer);
				this.Content.InstantiateIn(this.ContentContainer);
			}
			if (this.ServiceUrl != null)
			{
				if (this.HasSaveMethod)
				{
					if (!this.UseSetObject && (!string.IsNullOrEmpty(this.SaveMethod) || !string.IsNullOrEmpty(this.SaveMethodExpression)))
					{
						throw new NotSupportedException("Not supported to set \"UseSetObject\" to false and then use the properties \"SaveMethod\" or \"SaveMethodExpression\" on properties control");
					}
					this.SaveWebServiceMethod = new WebServiceMethod();
					this.SaveWebServiceMethod.ID = "Save";
					this.SaveWebServiceMethod.ServiceUrl = this.ServiceUrl;
					this.SaveWebServiceMethod.AlwaysInvokeSave = (this.AlwaysInvokeSave ?? (!this.UseSetObject));
					if (string.IsNullOrEmpty(this.SaveMethodExpression) && !string.IsNullOrEmpty(this.SaveMethod))
					{
						this.SaveWebServiceMethod.Method = this.SaveMethod;
					}
					else
					{
						this.SaveWebServiceMethod.Method = (this.UseSetObject ? "SetObject" : "NewObject");
					}
					if (this.ParameterSet != WebServiceParameterNames.NONE)
					{
						this.SaveWebServiceMethod.ParameterNames = this.ParameterSet;
					}
					else
					{
						this.SaveWebServiceMethod.ParameterNames = (this.UseSetObject ? WebServiceParameterNames.SetObject : WebServiceParameterNames.NewObject);
					}
					IPrincipal user = this.Context.User;
					foreach (WebServiceExceptionHandler webServiceExceptionHandler in this.ExceptionHandlers)
					{
						if (webServiceExceptionHandler.ApplyRbacRolesAndAddControls(this, user))
						{
							this.SaveWebServiceMethod.ExceptionHandlers.Add(webServiceExceptionHandler);
							this.RemoveMetaAttributes(webServiceExceptionHandler);
						}
					}
					this.Controls.Add(this.SaveWebServiceMethod);
				}
				if (this.UseSetObject)
				{
					this.RefreshWebServiceMethod = new WebServiceMethod();
					this.RefreshWebServiceMethod.ID = "Refresh";
					this.RefreshWebServiceMethod.ServiceUrl = this.ServiceUrl;
					this.RefreshWebServiceMethod.Method = "GetObject";
					this.RefreshWebServiceMethod.ParameterNames = WebServiceParameterNames.GetObject;
					this.Controls.Add(this.RefreshWebServiceMethod);
				}
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<Properties>(this);
			if (this.ServiceUrl != null && this.RequireDataAtInitialize)
			{
				if (this.UrlRequiresId && null == this.ObjectIdentity)
				{
					throw new BadQueryParameterException("id");
				}
				if (this.UseSetObject)
				{
					this.Results = this.ServiceUrl.GetObject(this.ObjectIdentity);
					this.Results.UseAsRbacScopeInCurrentHttpContext();
					this.configObject = (RbacQuery.LegacyTargetObject as IVersionable);
					if (this.configObject != null && this.configObject.IsReadOnly)
					{
						string[] array = new string[]
						{
							Strings.VersionMismatchWarning(this.configObject.ExchangeVersion.ExchangeBuild)
						};
						this.Results.Warnings = (this.Results.Warnings.IsNullOrEmpty() ? array : this.Results.Warnings.Concat(array).ToArray<string>());
					}
				}
				else if (this.GetObjectForNew)
				{
					this.Results = this.ServiceUrl.GetObjectForNew(this.ObjectIdentity);
				}
			}
			this.Page.PreRenderComplete += this.OnPreRenderComplete;
			base.OnPreRender(e);
		}

		protected void OnPreRenderComplete(object sender, EventArgs e)
		{
			Properties.ApplyRolesFilterRecursive(this, this.Context.User, this.configObject);
			this.CreateClientBindings();
			this.HideSectionWithoutAnyDataBinding();
		}

		protected WebControl GetCaptionLabel()
		{
			WebControl result = null;
			BaseForm baseForm = this.Page as BaseForm;
			if (baseForm != null)
			{
				result = baseForm.CaptionLabel;
			}
			else
			{
				for (Control parent = this.Parent; parent != null; parent = parent.Parent)
				{
					SlabFrame slabFrame = parent as SlabFrame;
					if (slabFrame != null)
					{
						result = slabFrame.CaptionLabel;
						break;
					}
				}
			}
			return result;
		}

		protected virtual ScriptControlDescriptor GetScriptDescriptor()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("PropertyPage", this.ClientID);
			scriptControlDescriptor.AddProperty("UseSetObject", this.UseSetObject, true);
			scriptControlDescriptor.AddProperty("RequireDataAtInitialize", this.RequireDataAtInitialize, true);
			BaseForm baseForm = this.Page as BaseForm;
			if (baseForm != null)
			{
				scriptControlDescriptor.AddComponentProperty("Form", "aspnetForm", true);
			}
			scriptControlDescriptor.AddClientIdProperty("ContentContainerID", this.ContentContainer);
			if (null != this.ObjectIdentity)
			{
				scriptControlDescriptor.AddScriptProperty("ObjectIdentity", this.ObjectIdentity.ToJsonString(null));
			}
			if (this.Results != null)
			{
				scriptControlDescriptor.AddProperty("JsonResults", this.Results.ToJsonString(DDIService.KnownTypes.Value));
				if (!string.IsNullOrEmpty(this.SaveMethodExpression))
				{
					scriptControlDescriptor.AddScriptProperty("SaveMethodExpression", "function($_){ return " + this.SaveMethodExpression + "}");
				}
			}
			if (this.Bindings != null)
			{
				scriptControlDescriptor.AddScriptProperty("Bindings", this.Bindings.ToJavaScript(null));
				WebControl captionLabel = this.GetCaptionLabel();
				if (captionLabel != null && !string.IsNullOrEmpty(this.CaptionTextField))
				{
					scriptControlDescriptor.AddScriptProperty("CaptionTextField", "function($_){ return $_." + this.CaptionTextField + "; }");
					scriptControlDescriptor.AddElementProperty("CaptionControl", captionLabel.ClientID);
				}
			}
			scriptControlDescriptor.AddComponentProperty("RefreshWebServiceMethod", this.RefreshWebServiceMethod);
			if (!this.ReadOnly)
			{
				scriptControlDescriptor.AddComponentProperty("SaveWebServiceMethod", this.SaveWebServiceMethod);
			}
			scriptControlDescriptor.AddProperty("UseWarningPanel", this.UseWarningPanel, true);
			scriptControlDescriptor.AddProperty("SuppressWarning", this.SuppressWarning, true);
			scriptControlDescriptor.AddClientIdProperty("WarningPanelID", this.warningPanel);
			scriptControlDescriptor.AddProperty("HideClientValidationError", this.HideClientValidationError, true);
			scriptControlDescriptor.AddProperty("SaveConfirmationText", this.SaveConfirmationText, true);
			if (!string.IsNullOrEmpty(this.OnRefreshSucceed))
			{
				scriptControlDescriptor.AddScriptProperty("OnRefreshSucceed", this.OnRefreshSucceed);
			}
			if (this.NameProperty != "Name")
			{
				scriptControlDescriptor.AddProperty("NameProperty", this.NameProperty);
			}
			return scriptControlDescriptor;
		}

		private static bool IsReadOnly(WebControl c)
		{
			if (c is TextBox)
			{
				return ((TextBox)c).ReadOnly;
			}
			if (c is EcpCollectionEditor)
			{
				return ((EcpCollectionEditor)c).ReadOnly;
			}
			return c is WebServiceListSource || c is EllipsisLabel || !c.Enabled;
		}

		private void RemoveMetaAttributes(Control c)
		{
			WebControl webControl = c as WebControl;
			if (webControl != null && webControl.HasAttributes)
			{
				webControl.Attributes.Remove("DataBoundProperty");
				webControl.Attributes.Remove("BoundControlProperty");
				webControl.Attributes.Remove("ClientPropertyName");
				webControl.Attributes.Remove("SetRoles");
				webControl.Attributes.Remove("NoRoleState");
				webControl.Attributes.Remove("EncodeHtml");
				webControl.Attributes.Remove("MandatoryParam");
				webControl.Attributes.Remove("SortedDirection");
			}
			if (c.HasControls())
			{
				foreach (object obj in c.Controls)
				{
					Control c2 = (Control)obj;
					this.RemoveMetaAttributes(c2);
				}
			}
		}

		private bool HasAssociatedChildControls(RadioButtonList rbl)
		{
			if (rbl.Items.Count == 2 && ((rbl.Items[0].Value == "true" && rbl.Items[1].Value == "false") || (rbl.Items[0].Value == "false" && rbl.Items[1].Value == "true")))
			{
				foreach (object obj in rbl.NamingContainer.Controls)
				{
					Control control = (Control)obj;
					if (control is ControlAssociationExtender && ((ControlAssociationExtender)control).TargetControlID == rbl.ID)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private IEnumerable<WebControl> GetVisibleClientBoundControls(Control parent)
		{
			if (parent.Visible)
			{
				WebControl wc = parent as WebControl;
				if (wc != null)
				{
					string dataProperty = wc.Attributes["DataBoundProperty"];
					if (!string.IsNullOrEmpty(dataProperty))
					{
						yield return wc;
					}
				}
				foreach (object obj in parent.Controls)
				{
					Control subControl = (Control)obj;
					foreach (WebControl c in this.GetVisibleClientBoundControls(subControl))
					{
						yield return c;
					}
				}
			}
			yield break;
		}

		private void HideSectionWithoutAnyDataBinding()
		{
			foreach (Section section in this.Sections)
			{
				IEnumerator<WebControl> enumerator2 = this.GetVisibleClientBoundControls(section).GetEnumerator();
				if (!enumerator2.MoveNext())
				{
					section.Visible = false;
				}
			}
		}

		private void CreateClientBindings()
		{
			foreach (WebControl webControl in this.GetVisibleClientBoundControls(this))
			{
				string key = webControl.Attributes["DataBoundProperty"];
				string text = webControl.Attributes["ClientPropertyName"];
				string text2 = webControl.Attributes["MandatoryParam"];
				if (text2 != null && text2 != "false" && text2 != "true")
				{
					throw new NotSupportedException("MandatoryParam attribute value can either be 'true' or 'false'.");
				}
				bool flag = text2 != null && Convert.ToBoolean(text2);
				Binding binding;
				if (webControl is Label && (text == null || text == "innerHTML"))
				{
					string text3 = webControl.Attributes["EncodeHtml"];
					if (text3 != null && text3 != "false" && text3 != "true")
					{
						throw new NotSupportedException("EncodeHtml attribute value can either be 'true' or 'false'.");
					}
					bool flag2 = text3 == null || Convert.ToBoolean(text3);
					if (flag2)
					{
						binding = new LabelBinding(webControl);
					}
					else
					{
						binding = new NonEncodedLabelBinding(webControl);
					}
				}
				else
				{
					if (text == null)
					{
						if (webControl is CheckBox && !(webControl is RadioButton))
						{
							text = "checked";
						}
						else
						{
							text = "value";
						}
					}
					if (webControl is IScriptControl)
					{
						if (webControl is AjaxUploader)
						{
							binding = new AjaxUploaderBinding(webControl, text);
						}
						else
						{
							binding = new ComponentBinding(webControl, text);
						}
					}
					else if (webControl is DownloadedImage)
					{
						DownloadedImage downloadedImage = (DownloadedImage)webControl;
						binding = new ImageUrlBinding(downloadedImage, downloadedImage.ReadOnly);
					}
					else if (webControl is DropDownList)
					{
						string text4 = webControl.Attributes["SortedDirection"];
						if (text4 != null && text4 != SortDirection.Ascending.ToString() && text4 != SortDirection.Descending.ToString())
						{
							throw new NotSupportedException("SortedDirection attribute value can either be 'Ascending' or 'Descending'.");
						}
						if (text4 == null)
						{
							binding = new ComboBoxBinding(webControl, text);
						}
						else
						{
							binding = new SortedComboBoxBinding(webControl, text, (SortDirection)Enum.Parse(typeof(SortDirection), text4));
						}
					}
					else if (webControl is RadioButton || webControl is RadioButtonList)
					{
						binding = new RadioButtonBinding(webControl, text);
					}
					else if (text == "value")
					{
						binding = new ValueBinding(webControl);
					}
					else
					{
						binding = new ClientControlBinding(webControl, text);
					}
					if (flag || webControl is Label)
					{
						binding = new MandatoryBinding(binding);
					}
					else if (webControl is RadioButtonList && this.HasAssociatedChildControls((RadioButtonList)webControl))
					{
						binding = new MandatoryBinding(binding);
					}
					else if (Properties.IsReadOnly(webControl))
					{
						binding = new NeverDirtyBinding(binding);
					}
				}
				if (this.UseSetObject || !(binding is NeverDirtyBinding))
				{
					this.Bindings.Bindings.Add(key, binding);
					if (this.allControlsDisabled && !(webControl is Label) && !(webControl is EllipsisLabel) && !(webControl is CollectionViewer) && !Properties.IsReadOnly(webControl))
					{
						this.allControlsDisabled = false;
					}
				}
			}
		}

		internal const string DataBoundProperty = "DataBoundProperty";

		internal const string BoundControlProperty = "BoundControlProperty";

		internal const string SetRoles = "SetRoles";

		internal const string NoRoleStateStr = "NoRoleState";

		internal const string EncodeHtml = "EncodeHtml";

		internal const string MandatoryParam = "MandatoryParam";

		internal const string SortedDirection = "SortedDirection";

		internal const string StrHelpId = "helpId";

		private const string ClientPropertyName = "ClientPropertyName";

		private Panel warningPanel;

		private bool allControlsDisabled = true;

		private IVersionable configObject;

		private Identity objectIdentity;
	}
}
