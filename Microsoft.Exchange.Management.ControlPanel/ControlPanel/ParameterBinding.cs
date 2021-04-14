using System;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ParameterBinding : Binding, ISupportServerSideEvaluate
	{
		public virtual object Value { get; set; }

		public override string ToJavaScript(IControlResolver resolver)
		{
			return string.Format("new ParameterBinding({0})", this.Value.ToJsonString(null));
		}
	}
}
