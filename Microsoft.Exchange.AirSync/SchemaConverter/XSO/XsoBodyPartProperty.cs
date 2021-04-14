using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoBodyPartProperty : XsoProperty, IBodyPartProperty, IProperty
	{
		public XsoBodyPartProperty(PropertyType type) : base(null, type)
		{
		}

		public XsoBodyPartProperty() : this(PropertyType.ReadOnly)
		{
		}

		public string Preview
		{
			get
			{
				Item item = base.XsoItem as Item;
				if (item == null)
				{
					return null;
				}
				ConversationId valueOrDefault = item.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId, null);
				if (valueOrDefault != null)
				{
					Conversation conversation;
					Command.CurrentCommand.GetOrCreateConversation(valueOrDefault, true, out conversation);
					IConversationTreeNode conversationTreeNode;
					if (conversation != null && conversation.ConversationTree.TryGetConversationTreeNode(item.StoreObjectId, out conversationTreeNode))
					{
						object obj = conversationTreeNode.StorePropertyBags[0].TryGetProperty(ItemSchema.TextBody);
						string text = obj as string;
						if (string.IsNullOrEmpty(text))
						{
							PropertyError arg = obj as PropertyError;
							AirSyncDiagnostics.TraceError<PropertyError>(ExTraceGlobals.AirSyncTracer, this, "Empty preview or Property error when getting conversation diff preview. Error: {0}", arg);
						}
						return text;
					}
				}
				return null;
			}
		}

		public Stream GetData(BodyType type, long truncationSize, out long totalDataSize, out IEnumerable<AirSyncAttachmentInfo> attachments)
		{
			Item item = base.XsoItem as Item;
			attachments = null;
			if (item == null)
			{
				totalDataSize = 0L;
				return null;
			}
			Stream stream;
			if (string.Equals(item.ClassName, "IPM.Note.SMIME", StringComparison.OrdinalIgnoreCase) && truncationSize != 0L)
			{
				string smimenotSupportedBodyHtml = XsoBodyPartProperty.GetSMIMENotSupportedBodyHtml(item.Session);
				stream = new MemoryStream(Encoding.UTF8.GetBytes(smimenotSupportedBodyHtml));
				totalDataSize = stream.Length;
				return stream;
			}
			switch (type)
			{
			case BodyType.None:
			case BodyType.PlainText:
			case BodyType.Rtf:
			case BodyType.Mime:
				throw new ConversionException(string.Format("Invalid body type requested: {0}", type));
			case BodyType.Html:
			{
				ConversationId valueOrDefault = item.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId, null);
				if (valueOrDefault == null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, null, false)
					{
						ErrorStringForProtocolLogger = "NoConversationIdForItem"
					};
				}
				Conversation conversation;
				bool orCreateConversation = Command.CurrentCommand.GetOrCreateConversation(valueOrDefault, true, out conversation);
				if (conversation == null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, null, false)
					{
						ErrorStringForProtocolLogger = "ConversationObjectLoadFailedForItem"
					};
				}
				if (orCreateConversation)
				{
					conversation.LoadItemParts(new List<StoreObjectId>
					{
						item.StoreObjectId
					});
				}
				IConversationTreeNode conversationTreeNode = null;
				if (!conversation.ConversationTree.TryGetConversationTreeNode(item.StoreObjectId, out conversationTreeNode))
				{
					AirSyncDiagnostics.TraceError<StoreObjectId>(ExTraceGlobals.AirSyncTracer, this, "Cannot find itemId {0} in conversation tree!", item.StoreObjectId);
					totalDataSize = 0L;
					return null;
				}
				bool flag = false;
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest<bool>(3970313533U, ref flag);
				if (flag)
				{
					totalDataSize = 0L;
					return null;
				}
				ItemPart itemPart = conversation.GetItemPart(item.StoreObjectId);
				if (!itemPart.DidLoadSucceed)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.AirSyncTracer, this, "ItemPart.DidLoadSucceed is false!");
					stream = null;
					totalDataSize = 0L;
					return stream;
				}
				using (AirSyncStream airSyncStream = new AirSyncStream())
				{
					try
					{
						using (HtmlWriter htmlWriter = new HtmlWriter(airSyncStream, Encoding.UTF8))
						{
							itemPart.WriteUniquePart(htmlWriter);
							htmlWriter.Flush();
						}
					}
					catch (TextConvertersException innerException)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, null, innerException, false);
					}
					airSyncStream.Seek(0L, SeekOrigin.Begin);
					stream = new AirSyncStream();
					uint streamHash;
					StreamHelper.CopyStream(airSyncStream, stream, Encoding.UTF8, (int)truncationSize, true, out streamHash);
					((AirSyncStream)stream).StreamHash = (int)streamHash;
					totalDataSize = ((truncationSize < 0L || airSyncStream.Length < truncationSize) ? stream.Length : airSyncStream.Length);
					Dictionary<AttachmentId, AirSyncAttachmentInfo> dictionary = null;
					if (itemPart.Attachments != null && itemPart.Attachments.Count > 0)
					{
						dictionary = itemPart.Attachments.ToDictionary((AttachmentInfo x) => x.AttachmentId, (AttachmentInfo x) => new AirSyncAttachmentInfo
						{
							AttachmentId = x.AttachmentId,
							IsInline = x.IsInline,
							ContentId = x.ContentId
						});
					}
					Dictionary<AttachmentId, string> dictionary2;
					Command.CurrentCommand.InlineAttachmentContentIdLookUp.TryGetValue(item.Id.ObjectId, out dictionary2);
					if (dictionary2 != null)
					{
						if (dictionary != null)
						{
							foreach (KeyValuePair<AttachmentId, string> keyValuePair in dictionary2)
							{
								AirSyncAttachmentInfo airSyncAttachmentInfo;
								if (dictionary.TryGetValue(keyValuePair.Key, out airSyncAttachmentInfo) && airSyncAttachmentInfo != null)
								{
									airSyncAttachmentInfo.IsInline = true;
									airSyncAttachmentInfo.ContentId = keyValuePair.Value;
								}
								else
								{
									dictionary[keyValuePair.Key] = new AirSyncAttachmentInfo
									{
										AttachmentId = keyValuePair.Key,
										IsInline = true,
										ContentId = keyValuePair.Value
									};
								}
							}
							attachments = dictionary.Values;
						}
						else
						{
							attachments = from inlineAttachment in dictionary2
							select new AirSyncAttachmentInfo
							{
								AttachmentId = inlineAttachment.Key,
								IsInline = true,
								ContentId = inlineAttachment.Value
							};
						}
					}
					else
					{
						attachments = ((dictionary != null) ? dictionary.Values : null);
					}
					return stream;
				}
				break;
			}
			}
			stream = null;
			totalDataSize = 0L;
			return stream;
		}

		private static string GetSMIMENotSupportedBodyHtml(StoreSession session)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<div>");
			stringBuilder.Append("<font color=\"red\">");
			stringBuilder.Append(Strings.SMIMENotSupportedBodyText.ToString(session.PreferedCulture));
			stringBuilder.Append("</font>");
			stringBuilder.Append("</div>");
			return stringBuilder.ToString();
		}
	}
}
