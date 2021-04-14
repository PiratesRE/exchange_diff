using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EnableInboxRule : ChangeInboxRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Enable-InboxRule";
			}
		}
	}
}
