using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("EcpAutoCompleteBehavior", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	[RequiredScript(typeof(ExtenderControlBase))]
	[TargetControlType(typeof(TextBox))]
	public class EcpAutoCompleteExtender : ExtenderControlBase
	{
		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("MinimumPrefixLength", this.MinimumPrefixLength);
			descriptor.AddProperty("FixedMinimumPrefixLength", this.FixedMinimumPrefixLength);
			descriptor.AddProperty("CompletionInterval", this.CompletionInterval);
			descriptor.AddProperty("AutoSuggestionPropertyNames", this.AutoSuggestionPropertyNames);
			descriptor.AddProperty("AutoSuggestionPropertyValues", this.AutoSuggestionPropertyValues);
			descriptor.AddComponentProperty("WebServiceMethod", this.WebServiceMethod.ClientID);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(this.WebServiceMethod);
		}

		[ClientPropertyName("MinimumPrefixLength")]
		[ExtenderControlProperty]
		public virtual int MinimumPrefixLength
		{
			get
			{
				return base.GetPropertyValue<int>("MinimumPrefixLength", 2);
			}
			set
			{
				base.SetPropertyValue<int>("MinimumPrefixLength", value);
			}
		}

		[ExtenderControlProperty]
		[ClientPropertyName("FixedMinimumPrefixLength")]
		public virtual bool FixedMinimumPrefixLength
		{
			get
			{
				return base.GetPropertyValue<bool>("FixedMinimumPrefixLength", false);
			}
			set
			{
				base.SetPropertyValue<bool>("FixedMinimumPrefixLength", value);
			}
		}

		[ClientPropertyName("CompletionInterval")]
		[ExtenderControlProperty]
		public virtual int CompletionInterval
		{
			get
			{
				return base.GetPropertyValue<int>("CompletionInterval", 1000);
			}
			set
			{
				base.SetPropertyValue<int>("CompletionInterval", value);
			}
		}

		[ExtenderControlProperty]
		[ClientPropertyName("CompletionSetCount")]
		public virtual int CompletionSetCount
		{
			get
			{
				return base.GetPropertyValue<int>("CompletionSetCount", 10);
			}
			set
			{
				base.SetPropertyValue<int>("CompletionSetCount", value);
			}
		}

		[ClientPropertyName("AutoSuggestionPropertyNames")]
		[ExtenderControlProperty]
		public virtual string AutoSuggestionPropertyNames
		{
			get
			{
				return base.GetPropertyValue<string>("AutoSuggestionPropertyNames", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("AutoSuggestionPropertyNames", value);
			}
		}

		[ClientPropertyName("AutoSuggestionPropertyValues")]
		[ExtenderControlProperty]
		public virtual string AutoSuggestionPropertyValues
		{
			get
			{
				return base.GetPropertyValue<string>("AutoSuggestionPropertyValues", string.Empty);
			}
			set
			{
				base.SetPropertyValue<string>("AutoSuggestionPropertyValues", value);
			}
		}

		[ClientPropertyName("WebServiceMethod")]
		public WebServiceMethod WebServiceMethod { get; set; }
	}
}
