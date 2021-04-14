using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class Set : ICloneable
	{
		public string Variable { get; set; }

		public object Value { get; set; }

		public object Clone()
		{
			return new Set
			{
				Variable = this.Variable,
				Value = this.Value
			};
		}
	}
}
