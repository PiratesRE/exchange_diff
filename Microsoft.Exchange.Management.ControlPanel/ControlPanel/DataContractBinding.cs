using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DataContractBinding : Binding
	{
		public string ContractType { get; set; }

		public BindingDictionary Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		public override string ToJavaScript(IControlResolver resolver)
		{
			string arg = string.IsNullOrEmpty(this.ContractType) ? "Object" : this.ContractType;
			return string.Format("new {0}({1},{2})", base.GetType().Name, arg, this.Bindings.ToJavaScript(resolver));
		}

		private BindingDictionary bindings = new BindingDictionary();
	}
}
