using System;
using System.ComponentModel;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ValuePair : Control
	{
		[DefaultValue("")]
		public string Name { get; set; }

		[DefaultValue("")]
		public virtual object Value { get; set; }
	}
}
