using System;
using System.ComponentModel;
using System.Configuration;
using AjaxControlToolkit;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class StaticBinding : Binding, ISupportServerSideEvaluate
	{
		public virtual object Value { get; set; }

		public virtual bool HasValue
		{
			get
			{
				return this.Value != null;
			}
		}

		public bool Optional { get; set; }

		[TypeConverter(typeof(TypeNameConverter))]
		public Type TargetType { get; set; }

		public string DefaultValue { get; set; }

		public override string ToJavaScript(IControlResolver resolver)
		{
			object obj = this.Value ?? this.DefaultValue;
			if (this.TargetType != null)
			{
				obj = ValueConvertor.ConvertValue(obj, this.TargetType, null);
			}
			return string.Format("new StaticBinding({0})", obj.ToJsonString(null));
		}
	}
}
