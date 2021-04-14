using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class EasFxCalendarRecipient : IRecipient
	{
		public EasFxCalendarRecipient(IPropertyBag propertyBag)
		{
			this.PropertyBag = propertyBag;
		}

		public IPropertyBag PropertyBag { get; private set; }

		public void Save()
		{
		}
	}
}
