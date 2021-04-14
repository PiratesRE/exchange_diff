using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetGroup : SetGroupBase
	{
		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}
	}
}
