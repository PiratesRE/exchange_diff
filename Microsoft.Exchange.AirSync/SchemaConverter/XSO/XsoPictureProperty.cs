using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoPictureProperty : XsoProperty, IPictureProperty, IProperty
	{
		public XsoPictureProperty() : base(null)
		{
		}

		public virtual string PictureData
		{
			get
			{
				this.GetPicture();
				return this.cachedPicture;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			IPictureProperty pictureProperty = (IPictureProperty)srcProperty;
			if (string.IsNullOrEmpty(pictureProperty.PictureData))
			{
				this.InternalSetToDefault(srcProperty);
				return;
			}
			if (pictureProperty.PictureData.Length > 50000)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.ProtocolTracer, null, "The size of contact picture is bigger than 50000 base64 chars");
				throw new ConversionException("The size of contact picture is bigger than 50000 base64 chars");
			}
			AttachmentId attachmentId = null;
			Contact contact = base.XsoItem as Contact;
			contact.Load(new PropertyDefinition[]
			{
				XsoPictureProperty.hasPicture
			});
			object obj = contact.TryGetProperty(XsoPictureProperty.hasPicture);
			if (obj != null && obj is bool && (bool)obj)
			{
				foreach (AttachmentHandle handle in contact.AttachmentCollection)
				{
					using (Attachment attachment = contact.AttachmentCollection.Open(handle))
					{
						attachment.Load(new PropertyDefinition[]
						{
							XsoPictureProperty.ispictureAttach
						});
						obj = attachment.TryGetProperty(XsoPictureProperty.ispictureAttach);
						if (obj != null && obj is bool && (bool)obj)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.InternalCopyFromModified(), picture attachment exists.");
							attachmentId = attachment.Id;
							break;
						}
					}
				}
			}
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.InternalCopyFromModified(), creating a new picture attachment.");
			using (Attachment attachment2 = contact.AttachmentCollection.Create(AttachmentType.Stream))
			{
				attachment2[XsoPictureProperty.ispictureAttach] = true;
				attachment2[AttachmentSchema.DisplayName] = "ContactPicture.jpg";
				attachment2[XsoPictureProperty.attachFileName] = string.Empty;
				attachment2[XsoPictureProperty.attachLongFileName] = "ContactPicture.jpg";
				attachment2[XsoPictureProperty.urlCompName] = "ContactPicture.jpg";
				attachment2[XsoPictureProperty.attachExtension] = "jpg";
				attachment2[XsoPictureProperty.attachFlags] = 0;
				attachment2[XsoPictureProperty.attachmentFlags] = 0;
				attachment2[XsoPictureProperty.attachmentHidden] = false;
				attachment2[XsoPictureProperty.attachmentLinkId] = 0;
				attachment2[XsoPictureProperty.attachEncoding] = new byte[0];
				attachment2[XsoPictureProperty.exceptionStartTime] = new DateTime(4501, 1, 1);
				attachment2[XsoPictureProperty.exceptionEndTime] = new DateTime(4501, 1, 1);
				contact[XsoPictureProperty.hasPicture] = true;
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.InternalCopyFromModified(), writing the data to picture attachment.");
				using (Stream contentStream = ((StreamAttachment)attachment2).GetContentStream())
				{
					contentStream.Position = 0L;
					byte[] array = Convert.FromBase64String(pictureProperty.PictureData);
					contentStream.Write(array, 0, array.Length);
				}
				attachment2.Save();
			}
			if (attachmentId != null)
			{
				contact.AttachmentCollection.Remove(attachmentId);
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Contact contact = base.XsoItem as Contact;
			contact.Load(new PropertyDefinition[]
			{
				XsoPictureProperty.hasPicture
			});
			object obj = contact.TryGetProperty(XsoPictureProperty.hasPicture);
			if (obj != null && obj is bool && (bool)obj)
			{
				foreach (AttachmentHandle handle in contact.AttachmentCollection)
				{
					using (Attachment attachment = contact.AttachmentCollection.Open(handle))
					{
						attachment.Load(new PropertyDefinition[]
						{
							XsoPictureProperty.ispictureAttach
						});
						obj = attachment.TryGetProperty(XsoPictureProperty.ispictureAttach);
						if (obj != null && obj is bool && (bool)obj)
						{
							contact.AttachmentCollection.Remove(attachment.Id);
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.InternalSetToDefault(), removed picture attachment");
							break;
						}
					}
				}
			}
		}

		public override void Unbind()
		{
			this.cachedPicture = null;
			base.Unbind();
		}

		private void GetPicture()
		{
			this.cachedPicture = null;
			Contact contact = base.XsoItem as Contact;
			contact.Load(new PropertyDefinition[]
			{
				XsoPictureProperty.hasPicture
			});
			object obj = contact.TryGetProperty(XsoPictureProperty.hasPicture);
			if (obj != null && obj is bool && (bool)obj)
			{
				foreach (AttachmentHandle handle in contact.AttachmentCollection)
				{
					using (Attachment attachment = contact.AttachmentCollection.Open(handle))
					{
						StreamAttachment streamAttachment = attachment as StreamAttachment;
						if (streamAttachment != null)
						{
							streamAttachment.Load(new PropertyDefinition[]
							{
								XsoPictureProperty.ispictureAttach
							});
							obj = streamAttachment.TryGetProperty(XsoPictureProperty.ispictureAttach);
							if (obj != null && obj is bool && (bool)obj)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.GetPicture(), picture attachment exists.");
								this.LoadPictureData(streamAttachment);
								break;
							}
						}
					}
				}
			}
		}

		private void LoadPictureData(StreamAttachment picture)
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.CommonTracer, null, "XsoPictureProperty.GetPicture(), getting picture attachment data.");
			Stream stream = null;
			try
			{
				stream = picture.GetContentStream();
				if (stream.Length <= 37500L)
				{
					byte[] array = new byte[stream.Length];
					int num = stream.Read(array, 0, array.Length);
					if (num == array.Length)
					{
						this.cachedPicture = Convert.ToBase64String(array);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		private static PropertyDefinition attachEncoding = PropertyTagPropertyDefinition.CreateCustom("attachEncoding", 922878210U);

		private static PropertyDefinition attachExtension = PropertyTagPropertyDefinition.CreateCustom("attachExtension", 922943519U);

		private static PropertyDefinition attachFileName = PropertyTagPropertyDefinition.CreateCustom("attachFileName", 923009055U);

		private static PropertyDefinition attachFlags = PropertyTagPropertyDefinition.CreateCustom("attachLongFileName", 924057603U);

		private static PropertyDefinition attachLongFileName = PropertyTagPropertyDefinition.CreateCustom("attachLongFileName", 923205663U);

		private static PropertyDefinition attachmentFlags = PropertyTagPropertyDefinition.CreateCustom("attachmentFlags", 2147287043U);

		private static PropertyDefinition attachmentHidden = PropertyTagPropertyDefinition.CreateCustom("attachmentHidden", 2147352587U);

		private static PropertyDefinition attachmentLinkId = PropertyTagPropertyDefinition.CreateCustom("attachmentLinkId", 2147090435U);

		private static PropertyDefinition exceptionEndTime = PropertyTagPropertyDefinition.CreateCustom("exceptionEndTime", 2147221568U);

		private static PropertyDefinition exceptionStartTime = PropertyTagPropertyDefinition.CreateCustom("exceptionStartTime", 2147156032U);

		private static PropertyDefinition hasPicture = GuidIdPropertyDefinition.CreateCustom("hasPicture", typeof(bool), new Guid("{00062004-0000-0000-C000-000000000046}"), 32789, PropertyFlags.None);

		private static PropertyDefinition ispictureAttach = PropertyTagPropertyDefinition.CreateCustom("isPictureAttach", 2147418123U);

		private static PropertyDefinition urlCompName = PropertyTagPropertyDefinition.CreateCustom("urlCompName", 284360735U);

		private string cachedPicture;
	}
}
