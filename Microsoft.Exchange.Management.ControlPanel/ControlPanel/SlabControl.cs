using System;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SlabControl : UserControl, IThemable
	{
		public string HelpId
		{
			get
			{
				return this.helpId;
			}
			set
			{
				this.helpId = value;
			}
		}

		public string Title { get; set; }

		public string Caption { get; set; }

		public string Roles { get; set; }

		public bool ShowCloseButton { get; set; }

		public string IncludeCssFiles { get; set; }

		public Unit Height { get; set; }

		public bool AlwaysDockSaveButton { get; set; }

		public bool HideSlabBorder { get; set; }

		public string FVAResource { get; set; }

		public bool IsPrimarySlab { get; set; }

		public bool UsePropertyPageStyle { get; set; }

		public FeatureSet FeatureSet { get; set; }

		public bool LayoutOnly { get; set; }

		internal FieldValidationAssistantExtender FieldValidationAssistantExtender
		{
			get
			{
				return this.fieldValidationAssistantExtender;
			}
		}

		internal IBaseFormContentControl PropertiesControl
		{
			get
			{
				if (this.propertyControl == null)
				{
					foreach (object obj in this.Controls)
					{
						Control control = (Control)obj;
						IBaseFormContentControl baseFormContentControl = control as IBaseFormContentControl;
						if (baseFormContentControl != null)
						{
							this.propertyControl = baseFormContentControl;
							break;
						}
					}
				}
				return this.propertyControl;
			}
		}

		internal bool AccessibleToUser(IPrincipal user)
		{
			return string.IsNullOrEmpty(this.Roles) || LoginUtil.IsInRoles(user, this.Roles.Split(new char[]
			{
				','
			}));
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Context.ThrowIfViewOptionsWithBEParam(this.FeatureSet);
			base.EnsureID();
			if (!string.IsNullOrEmpty(this.FVAResource))
			{
				this.fieldValidationAssistantExtender = new FieldValidationAssistantExtender();
				this.fieldValidationAssistantExtender.HelpId = this.HelpId;
				this.fieldValidationAssistantExtender.IndentCssClass = "baseFrmFvaIndent";
				this.fieldValidationAssistantExtender.LocStringsResource = this.FVAResource;
				if (this.PropertiesControl != null)
				{
					this.fieldValidationAssistantExtender.TargetControlID = this.Parent.UniqueID;
					this.fieldValidationAssistantExtender.Canvas = this.Parent.ClientID;
				}
				this.Controls.Add(this.fieldValidationAssistantExtender);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (!string.IsNullOrEmpty(this.FVAResource))
			{
				ScriptManager current = ScriptManager.GetCurrent(this.Page);
				current.EnableScriptLocalization = true;
				((ToolkitScriptManager)current).CombineScript(this.FVAResource);
				if (this.fieldValidationAssistantExtender.TargetControlID == null)
				{
					throw new InvalidOperationException("You enabled FVA in the slab but the TargetControlID is not set. See example in DeliverReport.ascx.cs. To turn FVA off on this slab, remove the FVAResource attribute.");
				}
			}
		}

		private FieldValidationAssistantExtender fieldValidationAssistantExtender;

		private IBaseFormContentControl propertyControl;

		private string helpId = string.Empty;
	}
}
