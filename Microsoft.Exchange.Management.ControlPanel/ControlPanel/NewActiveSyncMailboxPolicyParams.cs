using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewActiveSyncMailboxPolicyParams : BaseActiveSyncMailboxPolicyParams
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-MobileMailboxPolicy";
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this.ProcessPolicyParams(null);
		}
	}
}
