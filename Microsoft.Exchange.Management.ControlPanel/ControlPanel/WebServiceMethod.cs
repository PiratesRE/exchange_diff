using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ParseChildren(true, "Parameters")]
	[ClientScriptResource("WebServiceMethod", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[DefaultProperty("Parameters")]
	public class WebServiceMethod : ScriptComponent
	{
		public WebServiceMethod()
		{
			this.ExceptionHandlers = new List<WebServiceExceptionHandler>();
		}

		public string Method { get; set; }

		public WebServiceReference ServiceUrl { get; set; }

		public bool AlwaysInvokeSave { get; set; }

		public string OnClientInvoking { get; set; }

		public string OnClientCompleted { get; set; }

		public string OnClientSucceeded { get; set; }

		public string OnClientFailed { get; set; }

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BindingCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public WebServiceParameterNames ParameterNames { get; set; }

		public List<WebServiceExceptionHandler> ExceptionHandlers { get; private set; }

		public string ExceptionHandlersIDs
		{
			get
			{
				return string.Join(",", (from handler in this.ExceptionHandlers
				select handler.ClientID).ToArray<string>());
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddEvent("Invoking", this.OnClientInvoking, true);
			descriptor.AddEvent("Completed", this.OnClientCompleted, true);
			descriptor.AddEvent("Succeeded", this.OnClientSucceeded, true);
			descriptor.AddEvent("Failed", this.OnClientFailed, true);
			descriptor.AddProperty("ParameterNames", this.ParameterNames);
			descriptor.AddProperty("ServiceUrl", EcpUrl.ProcessUrl(this.ServiceUrl.ServiceUrl));
			descriptor.AddProperty("MethodName", this.Method);
			descriptor.AddProperty("ExceptionHandlerIDs", this.ExceptionHandlersIDs);
			descriptor.AddScriptProperty("Parameters", this.Parameters.ToJavaScript(this));
			descriptor.AddProperty("AlwaysInvokeSave", this.AlwaysInvokeSave);
		}

		private BindingCollection parameters = new BindingCollection();
	}
}
