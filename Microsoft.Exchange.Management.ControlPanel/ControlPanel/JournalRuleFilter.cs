using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JournalRuleFilter : WebServiceParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-JournalRule";
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
