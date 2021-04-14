using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("WebServiceDropDown", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	[TargetControlType(typeof(DropDownList))]
	public class WebServiceDropDown : DropDownList, IScriptControl, INamingContainer
	{
		public WebServiceReference ServiceUrl { get; set; }

		[DefaultValue("GetList")]
		public string WebServiceMethodName { get; set; }

		[DefaultValue(null)]
		public string SortProperty { get; set; }

		[DefaultValue(SortDirection.Ascending)]
		public SortDirection SortDirection { get; set; }

		public string InitialValue { get; set; }

		public WebServiceMethod RefreshWebServiceMethod { get; private set; }

		public string RefreshProperties { get; set; }

		protected override ControlCollection CreateControlCollection()
		{
			return new ControlCollection(this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			string text = base.Attributes["SetRoles"];
			this.hasWebServicePermission = ((string.IsNullOrEmpty(text) || LoginUtil.IsInRoles(this.Context.User, text.Split(new char[]
			{
				','
			}))) && null != this.ServiceUrl);
			if (this.hasWebServicePermission && !string.IsNullOrEmpty(this.RefreshProperties))
			{
				this.RefreshWebServiceMethod = new WebServiceMethod();
				this.RefreshWebServiceMethod.ID = "WebServiceDropDownRefresh";
				this.Controls.Add(this.RefreshWebServiceMethod);
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			writer.AddAttribute("role", "combobox");
			base.AddAttributesToRender(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (this.hasWebServicePermission)
			{
				if (this.RefreshWebServiceMethod == null)
				{
					SortOptions sortOptions = null;
					MethodInfo method = this.ServiceUrl.ServiceType.GetMethod(this.WebServiceMethodName ?? "GetList");
					if (!string.IsNullOrEmpty(this.SortProperty))
					{
						sortOptions = new SortOptions();
						sortOptions.PropertyName = this.SortProperty;
						sortOptions.Direction = this.SortDirection;
					}
					try
					{
						this.resutls = (PowerShellResults)method.Invoke(this.ServiceUrl.ServiceInstance, new object[]
						{
							null,
							sortOptions
						});
						goto IL_D6;
					}
					catch (TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				}
				this.RefreshWebServiceMethod.Method = (this.WebServiceMethodName ?? "GetList");
				this.RefreshWebServiceMethod.ServiceUrl = this.ServiceUrl;
				this.RefreshWebServiceMethod.ParameterNames = WebServiceParameterNames.GetList;
				this.UpdateParameters();
			}
			IL_D6:
			ScriptManager.GetCurrent(this.Page).RegisterScriptControl<WebServiceDropDown>(this);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (!base.DesignMode)
			{
				ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
			}
			base.Render(writer);
			if (this.RefreshWebServiceMethod != null)
			{
				this.RefreshWebServiceMethod.RenderControl(writer);
			}
		}

		public void UpdateParameters()
		{
			if (this.RefreshWebServiceMethod != null)
			{
				this.RefreshWebServiceMethod.Parameters.Clear();
				DataContractBinding item = new DataContractBinding();
				this.RefreshWebServiceMethod.Parameters.Add(item);
				DataContractBinding dataContractBinding = new DataContractBinding();
				if (!string.IsNullOrEmpty(this.SortProperty))
				{
					Binding binding = new StaticBinding
					{
						Name = "PropertyName",
						Value = this.SortProperty
					};
					dataContractBinding.Bindings.Add(binding.Name, binding);
					Binding binding2 = new StaticBinding
					{
						Name = "Direction",
						Value = this.SortDirection
					};
					dataContractBinding.Bindings.Add(binding2.Name, binding2);
				}
				this.RefreshWebServiceMethod.Parameters.Add(dataContractBinding);
			}
		}

		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("WebServiceDropDown", this.ClientID);
			if (this.resutls != null)
			{
				scriptControlDescriptor.AddScriptProperty("Results", this.resutls.ToJsonString(null));
			}
			if (!string.IsNullOrEmpty(this.InitialValue))
			{
				scriptControlDescriptor.AddProperty("InitialValue", this.InitialValue);
			}
			if (this.RefreshWebServiceMethod != null)
			{
				scriptControlDescriptor.AddProperty("RefreshProperties", this.RefreshProperties);
				scriptControlDescriptor.AddComponentProperty("RefreshWebServiceMethod", this.RefreshWebServiceMethod);
			}
			return new ScriptDescriptor[]
			{
				scriptControlDescriptor
			};
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return ScriptObjectBuilder.GetScriptReferences(typeof(EllipsisLabel));
		}

		private PowerShellResults resutls;

		private bool hasWebServicePermission;
	}
}
