using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class BooleanParameter : HiddenParameter
	{
		public BooleanParameter(string name) : base(name, true)
		{
			base.EditorType = "HiddenParameterEditor";
		}
	}
}
