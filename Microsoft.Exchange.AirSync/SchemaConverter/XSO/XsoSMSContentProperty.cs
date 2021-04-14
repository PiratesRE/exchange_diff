using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoSMSContentProperty : XsoContent14Property
	{
		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			base.InternalCopyFromModified(srcProperty);
			MessageItem messageItem = (MessageItem)base.XsoItem;
			StreamReader streamReader = new StreamReader(((IContentProperty)srcProperty).Body);
			char[] array = new char[160];
			int length = streamReader.ReadBlock(array, 0, array.Length);
			messageItem.Subject = new string(array, 0, length);
		}
	}
}
