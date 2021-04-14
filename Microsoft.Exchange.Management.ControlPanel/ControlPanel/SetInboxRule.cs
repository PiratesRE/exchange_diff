using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetInboxRule : InboxRuleParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-InboxRule";
			}
		}
	}
}
