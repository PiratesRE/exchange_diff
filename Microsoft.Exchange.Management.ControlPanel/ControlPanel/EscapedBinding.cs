using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public sealed class EscapedBinding : WrappedBinding
	{
		public EscapedBinding(Binding binding) : base(binding)
		{
		}
	}
}
