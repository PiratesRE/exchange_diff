using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SupervisionTag : EnumValue
	{
		public SupervisionTag(string name) : base(name, name)
		{
		}
	}
}
