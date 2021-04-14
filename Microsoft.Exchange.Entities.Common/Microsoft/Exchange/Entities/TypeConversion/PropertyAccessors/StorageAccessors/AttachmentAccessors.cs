using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors.StorageAccessors
{
	internal static class AttachmentAccessors
	{
		public static readonly IStoragePropertyAccessor<IStreamAttachment, byte[]> Content = new DelegatedStoragePropertyAccessor<IStreamAttachment, byte[]>(delegate(IStreamAttachment container, out byte[] value)
		{
			bool result;
			using (Stream contentStream = container.GetContentStream())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					if (contentStream != null)
					{
						Util.StreamHandler.CopyStreamData(contentStream, memoryStream);
						value = memoryStream.ToArray();
					}
					else
					{
						value = null;
					}
					result = true;
				}
			}
			return result;
		}, delegate(IStreamAttachment container, byte[] value)
		{
			using (MemoryStream memoryStream = new MemoryStream(value))
			{
				using (Stream contentStream = container.GetContentStream())
				{
					Util.StreamHandler.CopyStreamData(memoryStream, contentStream);
				}
			}
		}, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<IStreamAttachment, string> ContentId = new DefaultStoragePropertyAccessor<IStreamAttachment, string>(AttachmentSchema.AttachContentId, false);

		public static readonly IStoragePropertyAccessor<IStreamAttachment, string> ContentLocation = new DefaultStoragePropertyAccessor<IStreamAttachment, string>(AttachmentSchema.AttachContentLocation, false);

		public static readonly IStoragePropertyAccessor<IAttachment, string> ContentType = new DelegatedStoragePropertyAccessor<IAttachment, string>(delegate(IAttachment container, out string value)
		{
			value = container.ContentType;
			return true;
		}, delegate(IAttachment container, string value)
		{
			container.ContentType = value;
		}, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<IAttachment, string> Id = new DelegatedStoragePropertyAccessor<IAttachment, string>(delegate(IAttachment container, out string value)
		{
			value = IdConverter.Instance.ToStringId(container.Id);
			return true;
		}, null, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<IAttachment, bool> IsInline = new DefaultStoragePropertyAccessor<IAttachment, bool>(AttachmentSchema.IsInline, false);

		public static readonly IStoragePropertyAccessor<IAttachment, ExDateTime> LastModifiedTime = new DelegatedStoragePropertyAccessor<IAttachment, ExDateTime>(delegate(IAttachment container, out ExDateTime value)
		{
			value = container.LastModifiedTime;
			return true;
		}, null, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<IAttachment, string> Name = new DelegatedStoragePropertyAccessor<IAttachment, string>(delegate(IAttachment container, out string value)
		{
			value = container.DisplayName;
			return true;
		}, delegate(IAttachment container, string value)
		{
			container[AttachmentSchema.DisplayName] = value;
			IStreamAttachment streamAttachment = container as IStreamAttachment;
			if (streamAttachment != null)
			{
				streamAttachment.FileName = value;
			}
		}, null, null, new PropertyDefinition[0]);

		public static readonly IStoragePropertyAccessor<IReferenceAttachment, string> PathName = new DefaultStoragePropertyAccessor<IReferenceAttachment, string>(AttachmentSchema.AttachLongPathName, false);

		public static readonly IStoragePropertyAccessor<IReferenceAttachment, string> ProviderEndpointUrl = new DefaultStoragePropertyAccessor<IReferenceAttachment, string>(AttachmentSchema.AttachmentProviderEndpointUrl, false);

		public static readonly IStoragePropertyAccessor<IReferenceAttachment, string> ProviderType = new DefaultStoragePropertyAccessor<IReferenceAttachment, string>(AttachmentSchema.AttachmentProviderType, false);

		public static readonly IStoragePropertyAccessor<IAttachment, long> Size = new DelegatedStoragePropertyAccessor<IAttachment, long>(delegate(IAttachment container, out long value)
		{
			value = container.Size;
			return true;
		}, null, null, null, new PropertyDefinition[0]);
	}
}
