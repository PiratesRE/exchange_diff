using System;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class QueryStringBinding : StaticBinding
	{
		public string QueryStringField { get; set; }

		public override bool HasValue
		{
			get
			{
				return this.GetInternalValue() != null;
			}
		}

		public override object Value
		{
			get
			{
				object internalValue = this.GetInternalValue();
				if (internalValue != null)
				{
					return internalValue;
				}
				if (base.Optional && !string.IsNullOrEmpty(base.DefaultValue))
				{
					return base.DefaultValue;
				}
				throw new BadQueryParameterException(this.QueryStringField ?? string.Empty);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected string QueryStringValue
		{
			get
			{
				return HttpContext.Current.Request[this.QueryStringField];
			}
		}

		protected virtual object GetInternalValue()
		{
			return this.QueryStringValue;
		}
	}
}
