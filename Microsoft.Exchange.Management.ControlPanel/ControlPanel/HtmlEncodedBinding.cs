using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class HtmlEncodedBinding : WrappedBinding
	{
		public HtmlEncodedBinding(Binding binding) : base(binding)
		{
		}
	}
}
