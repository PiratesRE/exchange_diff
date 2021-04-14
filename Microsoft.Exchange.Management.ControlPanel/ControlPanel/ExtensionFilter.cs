using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExtensionFilter : WebServiceParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-App";
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
