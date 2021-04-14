using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class EcpWebServiceWizardStep : EcpWizardStep
	{
		public EcpWebServiceWizardStep()
		{
			base.ViewModel = "WebServiceWizardStepViewModel";
			this.ShowErrors = true;
			this.NextOnError = false;
		}

		public string WebServiceMethodName { get; set; }

		public WebServiceReference ServiceUrl { get; set; }

		public WebServiceParameterNames WebServiceParameterName { get; set; }

		[DefaultValue(true)]
		public bool ShowErrors { get; set; }

		[DefaultValue(false)]
		public bool NextOnError { get; set; }

		[DefaultValue(false)]
		public bool NextOnCancel { get; set; }

		public string ParameterNamesList { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			base.Attributes.Add("vm-WebServiceMethodName", this.WebServiceMethodName);
			base.Attributes.Add("vm-WebServiceParameterName", this.WebServiceParameterName.ToJsonString(null));
			base.Attributes.Add("vm-ShowErrors", this.ShowErrors.ToJsonString(null));
			base.Attributes.Add("vm-NextOnError", this.NextOnError.ToJsonString(null));
			base.Attributes.Add("vm-NextOnCancel", this.NextOnCancel.ToJsonString(null));
			base.Attributes.Add("vm-ParameterNamesList", this.ParameterNamesList);
			if (this.ServiceUrl != null)
			{
				base.Attributes.Add("vm-ServiceUrl", EcpUrl.ProcessUrl(this.ServiceUrl.ServiceUrl));
			}
		}
	}
}
