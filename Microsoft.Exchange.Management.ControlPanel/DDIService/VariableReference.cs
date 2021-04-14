using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class VariableReference
	{
		public VariableReference()
		{
			this.UseInput = true;
		}

		public string Variable { get; set; }

		public bool UseInput { get; set; }
	}
}
