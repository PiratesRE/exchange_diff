using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Handler;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class CoreAttachmentPropertyBagAdaptor : CorePropertyBagAdaptor
	{
		public CoreAttachmentPropertyBagAdaptor(ICorePropertyBag corePropertyBag, ICoreObject propertyMappingReference, Encoding string8Encoding, bool wantUnicode, bool isUpload) : base(new CoreObjectProperties(corePropertyBag), corePropertyBag, propertyMappingReference, ClientSideProperties.AttachmentInstance, PropertyConverter.Attachment, DownloadBodyOption.AllBodyProperties, string8Encoding, wantUnicode, isUpload, false)
		{
		}

		public override void SetProperty(PropertyValue propertyValue)
		{
			if (!propertyValue.IsError && this.IgnoreUploadProperty(propertyValue.PropertyTag))
			{
				return;
			}
			base.SetProperty(propertyValue);
		}

		public override System.IO.Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			if (this.IgnoreUploadProperty(property))
			{
				return null;
			}
			return base.SetPropertyStream(property, dataSizeEstimate);
		}

		protected override bool IgnoreUploadProperty(PropertyTag property)
		{
			return CoreAttachmentPropertyBagAdaptor.StoreComputedAttachmentProperties.Contains(property, PropertyTag.PropertyIdComparer) || base.IgnoreUploadProperty(property);
		}

		private static readonly PropertyTag[] StoreComputedAttachmentProperties = new PropertyTag[]
		{
			PropertyTag.AttachmentNumber,
			PropertyTag.AccessLevel,
			PropertyTag.MappingSignature,
			PropertyTag.AttachmentSize
		};
	}
}
