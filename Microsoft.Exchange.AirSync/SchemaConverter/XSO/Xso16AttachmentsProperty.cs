using System;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class Xso16AttachmentsProperty : Xso14AttachmentsProperty
	{
		public Xso16AttachmentsProperty(IdMapping idmapping, PropertyType propertyType) : base(idmapping, propertyType)
		{
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			Item item = base.XsoItem as Item;
			if (item == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "XSO16AttachmentProperty:InternalCopyFromModified. msgItem can not be null.");
				throw new UnexpectedTypeException("Item", base.XsoItem);
			}
			IAttachments12Property attachments12Property = srcProperty as IAttachments12Property;
			if (attachments12Property == null)
			{
				throw new UnexpectedTypeException("IAttachments12Property", srcProperty);
			}
			new List<IAttachment>(attachments12Property.Count);
			List<string> list = new List<string>();
			foreach (Attachment12Data attachment12Data in attachments12Property)
			{
				Attachment16Data attachment16Data = attachment12Data as Attachment16Data;
				if (attachment16Data == null)
				{
					throw new UnexpectedTypeException("Attachment16Data", attachment12Data);
				}
				if (attachment16Data.ChangeType == AttachmentAction.Delete)
				{
					list.Add(attachment16Data.FileReference);
				}
			}
			if (list.Count > 0)
			{
				AttachmentHelper.DeleteAttachments(item, list);
			}
			foreach (Attachment12Data attachment12Data2 in attachments12Property)
			{
				Attachment16Data attachment16Data2 = attachment12Data2 as Attachment16Data;
				if (attachment16Data2 == null)
				{
					throw new UnexpectedTypeException("Attachment16Data", attachment12Data2);
				}
				if (attachment16Data2.ChangeType == AttachmentAction.Add)
				{
					AttachmentHelper.CreateAttachment(item, attachment16Data2);
				}
			}
		}
	}
}
