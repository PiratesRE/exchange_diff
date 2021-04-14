using System;

namespace Microsoft.Exchange.Data.Transport.StoreDriver
{
	internal abstract class StoreDriverAgent : Agent
	{
		internal override object HostState
		{
			get
			{
				return base.HostStateInternal;
			}
			set
			{
				base.HostStateInternal = value;
				((SmtpServer)base.HostStateInternal).AssociatedAgent = this;
			}
		}

		internal override void AsyncComplete()
		{
			((SmtpServer)this.HostState).AddressBook.RecipientCache = null;
		}
	}
}
