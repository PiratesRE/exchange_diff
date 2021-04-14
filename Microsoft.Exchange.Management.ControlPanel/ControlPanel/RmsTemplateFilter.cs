using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RmsTemplateFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-RMSTemplate";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}
	}
}
