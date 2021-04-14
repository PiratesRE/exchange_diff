using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UserExtensionsFilter : WebServiceParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-UMMailbox";
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
