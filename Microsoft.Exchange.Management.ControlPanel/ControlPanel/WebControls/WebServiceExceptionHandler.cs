using System;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("WebServiceExceptionHandler", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public abstract class WebServiceExceptionHandler : ScriptComponent
	{
		public WebServiceExceptionHandler()
		{
		}

		public string ExceptionName { get; set; }

		public virtual bool ApplyRbacRolesAndAddControls(WebControl parentControl, IPrincipal currentUser)
		{
			return true;
		}

		public ExceptionHandlerType ExceptionHandlerType { get; set; }

		public ErrorCoExistingActionType ErrorCoExistingAction { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("ExceptionName", this.ExceptionName);
			descriptor.AddProperty("ExceptionHandlerType", this.ExceptionHandlerType);
			descriptor.AddProperty("ErrorCoExistingAction", this.ErrorCoExistingAction);
		}
	}
}
