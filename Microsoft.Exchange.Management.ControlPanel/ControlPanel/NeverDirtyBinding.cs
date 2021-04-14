using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NeverDirtyBinding : WrappedBinding
	{
		public NeverDirtyBinding(Binding binding) : base(binding)
		{
		}
	}
}
