using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class StringValuePair : ValuePair
	{
		public override object Value
		{
			get
			{
				return this.StringValue;
			}
			set
			{
				throw new NotSupportedException("Please use StringValue property instead");
			}
		}

		[DefaultValue("")]
		public string StringValue { get; set; }
	}
}
