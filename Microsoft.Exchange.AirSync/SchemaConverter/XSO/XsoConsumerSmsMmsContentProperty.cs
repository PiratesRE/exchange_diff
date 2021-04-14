using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoConsumerSmsMmsContentProperty : XsoContent14Property
	{
		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			IContentProperty contentProperty = (IContentProperty)srcProperty;
			BodyType nativeType = contentProperty.GetNativeType();
			if (nativeType == BodyType.Mime && string.Equals(base.XsoItem.ClassName, "IPM.NOTE.MOBILE.MMS", StringComparison.OrdinalIgnoreCase))
			{
				Item itemOut = (Item)base.XsoItem;
				InboundConversionOptions inboundConversionOptions = AirSyncUtility.GetInboundConversionOptions();
				inboundConversionOptions.ClearCategories = false;
				try
				{
					Stream body = ((IContentProperty)srcProperty).Body;
					ItemConversion.ConvertAnyMimeToItem(itemOut, body, inboundConversionOptions);
					goto IL_8A;
				}
				catch (ExchangeDataException ex)
				{
					throw new ConversionException("Mime conversion for MMS item failed due to InvalidCharSetError", ex.InnerException);
				}
				catch (ConversionFailedException ex2)
				{
					throw new ConversionException("Mime conversion for MMS item failed due to InvalidMime", ex2.InnerException);
				}
			}
			base.InternalCopyFromModified(srcProperty);
			IL_8A:
			if (string.Equals(base.XsoItem.ClassName, "IPM.NOTE.MOBILE.SMS", StringComparison.OrdinalIgnoreCase))
			{
				MessageItem messageItem = (MessageItem)base.XsoItem;
				StreamReader streamReader = new StreamReader(((IContentProperty)srcProperty).Body);
				char[] array = new char[78];
				int length = streamReader.ReadBlock(array, 0, array.Length);
				messageItem.Subject = new string(array, 0, length);
			}
		}
	}
}
