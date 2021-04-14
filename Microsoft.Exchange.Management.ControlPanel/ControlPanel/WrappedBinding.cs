using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class WrappedBinding : Binding
	{
		protected WrappedBinding(Binding binding)
		{
			this.binding = binding;
		}

		public override string ToJavaScript(IControlResolver resolver)
		{
			return string.Format("new {0}({1})", base.GetType().Name, this.binding.ToJavaScript(resolver));
		}

		private Binding binding;
	}
}
