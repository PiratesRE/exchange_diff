using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.PropTags;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferRecipient : FastTransferPropertyBag, IRecipient
	{
		public FastTransferRecipient(FastTransferDownloadContext downloadContext, MapiPerson mapiRecipient) : base(downloadContext, mapiRecipient, true, null)
		{
		}

		public FastTransferRecipient(FastTransferUploadContext uploadContext, MapiPerson mapiRecipient) : base(uploadContext, mapiRecipient)
		{
		}

		public void Reinitialize(MapiPerson mapiRecipient)
		{
			base.Reinitialize(mapiRecipient);
		}

		protected override bool IncludeTag(StorePropTag propTag)
		{
			if (base.ForMoveUser && propTag.IsCategory(4))
			{
				return true;
			}
			ushort propId = propTag.PropId;
			return propId != 12288 && base.IncludeTag(propTag);
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				return this;
			}
		}

		public void Save()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferRecipient>(this);
		}
	}
}
