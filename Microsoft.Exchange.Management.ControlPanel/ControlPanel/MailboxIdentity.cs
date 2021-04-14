using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxIdentity : Identity
	{
		public MailboxIdentity(MailboxId rawIdentity) : base(rawIdentity.ToString(), rawIdentity.ToString())
		{
		}
	}
}
