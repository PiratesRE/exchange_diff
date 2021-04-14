using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoAttachments12Property : XsoProperty, IAttachments12Property, IMultivaluedProperty<Attachment12Data>, IProperty, IEnumerable<Attachment12Data>, IEnumerable
	{
		public XsoAttachments12Property(IdMapping idmapping, PropertyType propertyType = PropertyType.ReadOnly) : base(null, propertyType)
		{
			this.idmapping = idmapping;
		}

		public XsoAttachments12Property(IdMapping idmapping, PropertyType propertyType = PropertyType.ReadOnly, params PropertyDefinition[] prefetchPropDef) : base(null, propertyType, prefetchPropDef)
		{
			this.idmapping = idmapping;
		}

		public int Count
		{
			get
			{
				if (this.IsItemDelegated())
				{
					return 0;
				}
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded((Item)base.XsoItem))
				{
					return ((RightsManagedMessageItem)base.XsoItem).ProtectedAttachmentCollection.Count;
				}
				if (BodyConversionUtilities.IsIRMFailedToDecode((Item)base.XsoItem))
				{
					return 0;
				}
				return ((Item)base.XsoItem).AttachmentCollection.Count;
			}
		}

		public IEnumerator<Attachment12Data> GetEnumerator()
		{
			Item message = base.XsoItem as Item;
			Attachment12Data attachmentData = null;
			if (message == null)
			{
				throw new UnexpectedTypeException("Item", base.XsoItem);
			}
			MeetingRequest meetingRequest = base.XsoItem as MeetingRequest;
			if (meetingRequest == null || !meetingRequest.IsDelegated())
			{
				string idbase = null;
				if (this.idmapping != null)
				{
					idbase = this.idmapping[MailboxSyncItemId.CreateForNewItem(message.Id.ObjectId)];
				}
				if (idbase == null)
				{
					idbase = message.Id.ObjectId.ToBase64String();
				}
				if (message is MessageItem && ((MessageItem)message).IsRestricted && !BodyConversionUtilities.IsMessageRestrictedAndDecoded(message) && !BodyConversionUtilities.IsIRMFailedToDecode(message))
				{
					object prop = message.TryGetProperty(MessageItemSchema.DRMLicense);
					if (prop is byte[][])
					{
						byte[][] license = (byte[][])prop;
						if (license.Length > 0)
						{
							attachmentData = new Attachment14Data();
							attachmentData.DisplayName = "message.rpmsg.license";
							attachmentData.Method = 1;
							attachmentData.EstimatedDataSize = (long)license[0].Length;
							attachmentData.IsInline = false;
							attachmentData.FileReference = HttpUtility.UrlEncode(idbase + ":DRMLicense");
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Add DRM license as attachment, message is MessageItem {0}, message.IsRestricted {1}, IsDecoded {2}, FailedToDecode {3}", new object[]
							{
								message is MessageItem,
								((MessageItem)message).IsRestricted,
								BodyConversionUtilities.IsMessageRestrictedAndDecoded(message),
								BodyConversionUtilities.IsIRMFailedToDecode(message)
							});
							yield return attachmentData;
						}
						else
						{
							AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.XsoTracer, this, "The license property on the DRM message is incorrect. Length = {0}", license.Length);
						}
					}
					else
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "The license property on the DRM message is incorrect. prop = {0}", new object[]
						{
							prop
						});
					}
				}
				AttachmentCollection attachmentCollection = null;
				if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(message))
				{
					attachmentCollection = ((RightsManagedMessageItem)message).ProtectedAttachmentCollection;
				}
				else
				{
					if (BodyConversionUtilities.IsIRMFailedToDecode(message))
					{
						goto IL_3FC;
					}
					attachmentCollection = message.AttachmentCollection;
				}
				int index = -1;
				foreach (AttachmentHandle handle in attachmentCollection)
				{
					using (Attachment attachment = attachmentCollection.Open(handle))
					{
						if (BodyUtility.IsClearSigned(message) && (string.Equals(attachment.FileName, "smime.p7m", StringComparison.OrdinalIgnoreCase) || string.Equals(attachment.ContentType, "multipart/signed", StringComparison.OrdinalIgnoreCase)))
						{
							continue;
						}
						attachmentData = this.GetAttachmentData(message, attachment, idbase, ref index);
					}
					if (attachmentData != null)
					{
						yield return attachmentData;
					}
				}
			}
			IL_3FC:
			yield break;
		}

		protected virtual Attachment12Data GetAttachmentData(Item message, Attachment attachment, string idbase, ref int index)
		{
			index++;
			Attachment14Data attachment14Data = new Attachment14Data();
			attachment14Data.Id = attachment.Id;
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(message) && AirSyncUtility.IsProtectedVoicemailItem(message))
			{
				if (AttachmentHelper.IsProtectedVoiceAttachment(attachment.DisplayName))
				{
					string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.RequireProtectedPlayOnPhone);
					if (valueOrDefault != null && valueOrDefault.Equals("true", StringComparison.OrdinalIgnoreCase))
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Skip attachment requires protected play on phone");
						return null;
					}
					attachment14Data.DisplayName = AttachmentHelper.GetUnprotectedVoiceAttachmentName(attachment.DisplayName);
				}
				else
				{
					if (AttachmentHelper.IsProtectedTranscriptAttachment(attachment.DisplayName))
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, this, "Skip protected transcript attachment");
						return null;
					}
					attachment14Data.DisplayName = attachment.DisplayName;
				}
			}
			else
			{
				attachment14Data.DisplayName = attachment.DisplayName;
			}
			if (string.IsNullOrEmpty(attachment14Data.DisplayName))
			{
				if (!string.IsNullOrEmpty(attachment.FileName))
				{
					attachment14Data.DisplayName = attachment.FileName;
					if (attachment.AttachmentType == AttachmentType.Ole && attachment.DisplayName.LastIndexOf('.') < 0)
					{
						Attachment14Data attachment14Data2 = attachment14Data;
						attachment14Data2.DisplayName += ".jpg";
					}
				}
				else if (attachment.AttachmentType == AttachmentType.EmbeddedMessage)
				{
					using (Item itemAsReadOnly = ((ItemAttachment)attachment).GetItemAsReadOnly(null))
					{
						attachment14Data.DisplayName = (itemAsReadOnly.TryGetProperty(ItemSchema.Subject) as string);
					}
				}
			}
			if (string.IsNullOrEmpty(attachment14Data.DisplayName))
			{
				attachment14Data.DisplayName = "????";
			}
			AirSyncDiagnostics.TraceDebug<int, string>(ExTraceGlobals.XsoTracer, this, "Attachment {0}, display name {1}", index, attachment14Data.DisplayName);
			attachment14Data.FileReference = HttpUtility.UrlEncode(idbase + ":" + index);
			attachment14Data.Method = Convert.ToByte((int)attachment.TryGetProperty(AttachmentSchema.AttachMethod));
			attachment14Data.EstimatedDataSize = attachment.Size;
			if (attachment.IsInline)
			{
				if (string.IsNullOrEmpty(attachment.ContentId))
				{
					attachment14Data.ContentId = Guid.NewGuid().ToString();
				}
				else
				{
					attachment14Data.ContentId = attachment.ContentId;
				}
			}
			if (attachment.ContentLocation != null)
			{
				attachment14Data.ContentLocation = attachment.ContentLocation.ToString();
			}
			if (!BodyConversionUtilities.IsMessageRestrictedAndDecoded(message))
			{
				attachment14Data.IsInline = attachment.IsInline;
			}
			return attachment14Data;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		[NonSerialized]
		private IdMapping idmapping;
	}
}
