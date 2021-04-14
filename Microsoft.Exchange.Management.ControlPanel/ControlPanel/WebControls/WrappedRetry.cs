using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ParseChildren(true)]
	[ToolboxData("<{0}:WrappedRetry  runat=server></{0}:WrappedRetry >")]
	[ClientScriptResource("WrappedRetry", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class WrappedRetry : SimpleRetry
	{
		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateInstance(TemplateInstance.Single)]
		[Browsable(false)]
		[Description("WrappedRetry Content")]
		[TemplateContainer(typeof(PropertiesContentPanel))]
		public virtual ITemplate Content { get; set; }

		public override string Description
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override bool ApplyRbacRolesAndAddControls(WebControl parentControl, IPrincipal currentUser)
		{
			if (base.ApplyRbacRolesAndAddControls(parentControl, currentUser))
			{
				this.EnsureChildControls();
				parentControl.Controls.Add(this.contentContainer);
				Properties.ApplyRolesFilterRecursive(this.contentContainer, currentUser, null);
				return true;
			}
			return false;
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.Content != null)
			{
				this.contentContainer = new Panel();
				this.contentContainer.ID = "wrappedRetryContentContainer";
				this.contentContainer.Height = Unit.Percentage(100.0);
				this.contentContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
				this.Controls.Add(this.contentContainer);
				this.Content.InstantiateIn(this.contentContainer);
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddElementProperty("ContentElement", this.contentContainer.ClientID);
		}

		private Panel contentContainer;
	}
}
