using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("WebServiceListSource", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	public class WebServiceListSource : ListSource, INamingContainer
	{
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public BindingCollection FilterParameters
		{
			get
			{
				return this.filterParameters;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public BindingCollection SortParameters
		{
			get
			{
				return this.sortParameters;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		public BindingCollection DDIParameters
		{
			get
			{
				return this.ddiParameters;
			}
		}

		public WebServiceMethod RefreshWebServiceMethod { get; private set; }

		[UrlProperty("*.svc")]
		[DefaultValue(null)]
		public WebServiceReference ServiceUrl { get; set; }

		[DefaultValue(false)]
		public bool SupportAsyncGetList { get; set; }

		[DefaultValue(false)]
		public bool ClientSort { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] RefreshAfter { get; set; }

		[DefaultValue(null)]
		public string RefreshCookieName { get; set; }

		public WebServiceListSource()
		{
			this.RefreshWebServiceMethod = new WebServiceMethod();
			this.RefreshWebServiceMethod.ID = "Refresh";
			this.Controls.Add(this.RefreshWebServiceMethod);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (this.ServiceUrl != null)
			{
				this.RefreshWebServiceMethod.Method = "GetList";
				this.RefreshWebServiceMethod.ServiceUrl = this.ServiceUrl;
				this.RefreshWebServiceMethod.ParameterNames = WebServiceParameterNames.GetList;
				this.UpdateParameters();
			}
			base.OnPreRender(e);
		}

		public void UpdateParameters()
		{
			if (this.RefreshWebServiceMethod != null)
			{
				this.RefreshWebServiceMethod.Parameters.Clear();
				DataContractBinding dataContractBinding = new DataContractBinding();
				foreach (Binding binding in this.FilterParameters)
				{
					dataContractBinding.Bindings.Add(binding.Name, binding);
				}
				if (DDIService.UseDDIService(this.ServiceUrl))
				{
					foreach (Binding binding2 in this.DDIParameters)
					{
						dataContractBinding.Bindings.Add(binding2.Name, binding2);
					}
				}
				this.RefreshWebServiceMethod.Parameters.Add(dataContractBinding);
				DataContractBinding dataContractBinding2 = new DataContractBinding();
				foreach (Binding binding3 in this.SortParameters)
				{
					dataContractBinding2.Bindings.Add(binding3.Name, binding3);
				}
				this.RefreshWebServiceMethod.Parameters.Add(dataContractBinding2);
			}
		}

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			IEnumerable<ScriptDescriptor> scriptDescriptors = base.GetScriptDescriptors();
			ScriptControlDescriptor scriptControlDescriptor = (ScriptControlDescriptor)scriptDescriptors.First<ScriptDescriptor>();
			scriptControlDescriptor.AddProperty("ServiceUrl", EcpUrl.ProcessUrl(this.ServiceUrl.ServiceUrl));
			scriptControlDescriptor.AddProperty("SupportAsyncGetList", this.SupportAsyncGetList, true);
			scriptControlDescriptor.AddProperty("ClientSort", this.ClientSort, true);
			if (this.RefreshCookieName != null)
			{
				scriptControlDescriptor.AddProperty("RefreshCookieName", this.RefreshCookieName);
			}
			if (!this.RefreshAfter.IsNullOrEmpty())
			{
				scriptControlDescriptor.AddProperty("RefreshAfter", this.RefreshAfter);
			}
			scriptControlDescriptor.AddComponentProperty("RefreshWebServiceMethod", this.RefreshWebServiceMethod.ClientID);
			return scriptDescriptors;
		}

		private BindingCollection filterParameters = new BindingCollection();

		private BindingCollection sortParameters = new BindingCollection();

		private BindingCollection ddiParameters = new BindingCollection();
	}
}
