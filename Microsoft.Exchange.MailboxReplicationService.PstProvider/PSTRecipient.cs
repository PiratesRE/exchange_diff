using System;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTRecipient : IRecipient
	{
		public PSTRecipient(int recipientNumber, PSTPropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
			PropertyValue property = this.propertyBag.GetProperty(PropertyTag.RowId);
			if (property.IsError)
			{
				property = new PropertyValue(PropertyTag.RowId, recipientNumber);
			}
			this.propertyBag.SetProperty(property);
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public void Save()
		{
		}

		private readonly PSTPropertyBag propertyBag;
	}
}
