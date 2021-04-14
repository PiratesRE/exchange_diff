using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ParseChildren(true)]
	[ClientScriptResource("SimpleRetry", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[ToolboxData("<{0}:SimpleRetry runat=server></{0}:SimpleRetry>")]
	public class SimpleRetry : WebServiceExceptionHandler
	{
		public SimpleRetry()
		{
			base.ExceptionHandlerType = ExceptionHandlerType.SimpleRetry;
		}

		public virtual string Description { get; set; }

		[DefaultValue("ClientStrings.IUnderstandAction")]
		public string AcceptanceText { get; set; }

		public string PropertiesToAdd { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (!this.Description.IsNullOrBlank())
			{
				descriptor.AddProperty("Description", this.Description);
			}
			if (!this.PropertiesToAdd.IsNullOrBlank())
			{
				descriptor.AddProperty("PropertiesToAdd", this.PropertiesToAdd);
			}
			if (!this.AcceptanceText.IsNullOrBlank())
			{
				descriptor.AddProperty("AcceptanceText", this.AcceptanceText);
			}
		}

		public override bool ApplyRbacRolesAndAddControls(WebControl parentControl, IPrincipal currentUser)
		{
			bool flag = true;
			if (base.HasAttributes)
			{
				string text = base.Attributes["SetRoles"];
				if (!string.IsNullOrEmpty(text) && !LoginUtil.IsInRoles(currentUser, text.Split(new char[]
				{
					','
				})))
				{
					flag = false;
				}
			}
			if (flag)
			{
				parentControl.Controls.Add(this);
			}
			return flag;
		}
	}
}
