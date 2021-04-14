using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMyGroup : SetGroupBase
	{
		public override string RbacScope
		{
			get
			{
				return "@W:MyDistributionGroups";
			}
		}
	}
}
