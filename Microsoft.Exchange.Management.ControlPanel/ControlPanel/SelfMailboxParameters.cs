using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SelfMailboxParameters : SetObjectProperties
	{
		public SelfMailboxParameters()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Mailbox"] = Identity.FromExecutingUserId();
		}

		public const string RbacParameters = "?Mailbox";

		protected const string IdentitySuffix = "&Identity";

		public const string RbacParametersWithIdentity = "?Mailbox&Identity";
	}
}
