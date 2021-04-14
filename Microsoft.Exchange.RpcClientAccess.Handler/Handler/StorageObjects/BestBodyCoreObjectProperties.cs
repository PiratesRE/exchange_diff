using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects
{
	internal class BestBodyCoreObjectProperties : CoreObjectProperties
	{
		public BestBodyCoreObjectProperties(ICoreItem coreItem, ICorePropertyBag corePropertyBag, Encoding string8Encoding, Func<BodyReadConfiguration, Stream> getBodyConversionStreamCallback) : base(corePropertyBag)
		{
			Util.ThrowOnNullArgument(coreItem, "coreItem");
			Util.ThrowOnNullArgument(string8Encoding, "string8Encoding");
			Util.ThrowOnNullArgument(getBodyConversionStreamCallback, "getBodyConversionStreamCallback");
			this.coreItem = coreItem;
			this.bodyHelper = new BodyHelper(coreItem, corePropertyBag, string8Encoding, getBodyConversionStreamCallback);
		}

		public BestBodyCoreObjectProperties(ICoreItem coreItem, ICorePropertyBag corePropertyBag, BodyHelper bodyHelper) : base(corePropertyBag)
		{
			Util.ThrowOnNullArgument(coreItem, "coreItem");
			Util.ThrowOnNullArgument(bodyHelper, "bodyHelper");
			this.coreItem = coreItem;
			this.bodyHelper = bodyHelper;
		}

		public BodyHelper BodyHelper
		{
			get
			{
				return this.bodyHelper;
			}
		}

		public override void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			this.bodyHelper.SetProperty(propertyDefinition, value);
		}

		public override void DeleteProperty(PropertyDefinition propertyDefinition)
		{
			this.bodyHelper.DeleteProperty(propertyDefinition);
		}

		public override Stream OpenStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			return this.bodyHelper.OpenStream(propertyDefinition, openMode);
		}

		public override object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			return this.bodyHelper.TryGetProperty(propertyDefinition);
		}

		public void ResetBody()
		{
			this.bodyHelper.PrepareForSave();
			this.bodyHelper.UpdateBodyPreviewIfNeeded(this.coreItem.Body);
			this.bodyHelper.Reset();
		}

		private readonly ICoreItem coreItem;

		private readonly BodyHelper bodyHelper;
	}
}
