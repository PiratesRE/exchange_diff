using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageCategoryFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MessageCategory";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}
	}
}
