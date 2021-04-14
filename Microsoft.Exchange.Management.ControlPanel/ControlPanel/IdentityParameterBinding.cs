using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class IdentityParameterBinding : QueryStringBinding
	{
		protected override object GetInternalValue()
		{
			object result = null;
			if (!string.IsNullOrEmpty(base.QueryStringValue))
			{
				result = Identity.ParseIdentity(base.QueryStringValue);
			}
			return result;
		}
	}
}
