using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetResourceConfigurationBase : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-CalendarProcessing";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}
	}
}
