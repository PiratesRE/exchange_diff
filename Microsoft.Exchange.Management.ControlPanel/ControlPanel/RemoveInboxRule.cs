using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RemoveInboxRule : ChangeInboxRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Remove-InboxRule";
			}
		}
	}
}
