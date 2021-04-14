using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EmbeddedMessage : Message
	{
		internal EmbeddedMessage(CoreItem parentCoreItem, Logon logon, Encoding string8Encoding) : base(parentCoreItem, logon, string8Encoding)
		{
		}

		protected override StoreId GetMessageIdAfterSave()
		{
			return default(StoreId);
		}

		protected override MessageAdaptor CreateDownloadMessageAdaptor(DownloadBodyOption downloadBodyOption, FastTransferSendOption sendOptions, bool isFastTransferCopyProperties)
		{
			base.PropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
			return new MessageAdaptor(base.ReferenceCoreItem, new MessageAdaptor.Options
			{
				IsReadOnly = true,
				IsEmbedded = true,
				DownloadBodyOption = downloadBodyOption,
				IsUpload = sendOptions.IsUpload(),
				IsFastTransferCopyProperties = isFastTransferCopyProperties
			}, base.LogonObject.LogonString8Encoding, sendOptions.WantUnicode(), null);
		}

		protected override MessageAdaptor CreateUploadMessageAdaptor()
		{
			return new MessageAdaptor(base.ReferenceCoreItem, new MessageAdaptor.Options
			{
				IsReadOnly = false,
				IsEmbedded = true,
				DownloadBodyOption = DownloadBodyOption.AllBodyProperties
			}, base.LogonObject.LogonString8Encoding, true, base.LogonObject);
		}

		protected override bool SaveChangesToLinkedDocumentLibraryIfNecessary()
		{
			return false;
		}

		public override StoreId SaveChanges(SaveChangesMode saveChangesMode)
		{
			return base.SaveChanges(saveChangesMode);
		}
	}
}
