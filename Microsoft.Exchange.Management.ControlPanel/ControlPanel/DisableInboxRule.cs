using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DisableInboxRule : ChangeInboxRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Disable-InboxRule";
			}
		}
	}
}
