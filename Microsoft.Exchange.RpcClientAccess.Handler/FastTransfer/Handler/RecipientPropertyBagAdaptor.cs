using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal class RecipientPropertyBagAdaptor : CorePropertyBagAdaptor
	{
		public RecipientPropertyBagAdaptor(ICorePropertyBag corePropertyBag, ICoreObject propertyMappingReference, Encoding string8Encoding, bool wantUnicode) : base(new CoreObjectProperties(corePropertyBag), corePropertyBag, propertyMappingReference, ClientSideProperties.RecipientInstance, PropertyConverter.Recipient, DownloadBodyOption.AllBodyProperties, string8Encoding, wantUnicode, false, false)
		{
		}

		public override System.IO.Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			return MemoryPropertyBag.WrapPropertyWriteStream(this, property, dataSizeEstimate);
		}

		protected override bool IgnoreDownloadProperty(PropertyTag property)
		{
			return false;
		}

		protected override bool IgnoreUploadProperty(PropertyTag property)
		{
			return false;
		}
	}
}
