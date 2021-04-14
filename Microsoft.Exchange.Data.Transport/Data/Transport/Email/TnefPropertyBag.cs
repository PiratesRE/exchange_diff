using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class TnefPropertyBag
	{
		static TnefPropertyBag()
		{
			int num = 0;
			TnefPropertyBag.MessageProperties = TnefPropertyBag.GetMessageProperties(ref num);
			TnefPropertyBag.NamedMessageProperties = TnefPropertyBag.GetNamedMessageProperties(ref num);
			TnefPropertyBag.MessagePropertyCount = num;
			num = 0;
			TnefPropertyBag.AttachmentProperties = TnefPropertyBag.GetAttachmentProperties(ref num);
			TnefPropertyBag.NamedAttachmentProperties = new Dictionary<TnefNameTag, int>(0, TnefPropertyBag.NameTagComparer);
			TnefPropertyBag.AttachmentPropertyCount = num;
		}

		internal TnefPropertyBag(PureTnefMessage parentMessage)
		{
			this.parentMessage = parentMessage;
			this.attachmentData = null;
			this.supportedProperties = TnefPropertyBag.MessageProperties;
			this.supportedNamedProperties = TnefPropertyBag.NamedMessageProperties;
			this.properties = new TnefPropertyBag.PropertyData[TnefPropertyBag.MessagePropertyCount];
		}

		internal TnefPropertyBag(TnefAttachmentData attachmentData)
		{
			this.attachmentData = attachmentData;
			this.parentMessage = null;
			this.supportedProperties = TnefPropertyBag.AttachmentProperties;
			this.supportedNamedProperties = TnefPropertyBag.NamedAttachmentProperties;
			this.properties = new TnefPropertyBag.PropertyData[TnefPropertyBag.AttachmentPropertyCount];
		}

		private static Dictionary<TnefPropertyTag, int> GetMessageProperties(ref int index)
		{
			Dictionary<TnefPropertyTag, int> dictionary = new Dictionary<TnefPropertyTag, int>(61, TnefPropertyBag.PropertyTagComparer);
			dictionary.Add(TnefPropertyTag.TnefCorrelationKey, index++);
			dictionary.Add(TnefPropertyTag.MessageCodepage, index++);
			dictionary.Add(TnefPropertyTag.InternetCPID, index++);
			dictionary.Add(TnefPropertyTag.MessageLocaleID, index++);
			dictionary.Add(TnefPropertyTag.ContentIdentifierA, index);
			dictionary.Add(TnefPropertyTag.ContentIdentifierW, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptRequested, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptDisplayNameA, index);
			dictionary.Add(TnefPropertyTag.ReadReceiptDisplayNameW, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptEmailAddressA, index);
			dictionary.Add(TnefPropertyTag.ReadReceiptEmailAddressW, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptAddrtypeA, index);
			dictionary.Add(TnefPropertyTag.ReadReceiptAddrtypeW, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptSmtpAddressA, index);
			dictionary.Add(TnefPropertyTag.ReadReceiptSmtpAddressW, index++);
			dictionary.Add(TnefPropertyTag.ReadReceiptEntryId, index++);
			dictionary.Add(TnefPropertyTag.SenderNameA, index);
			dictionary.Add(TnefPropertyTag.SenderNameW, index++);
			dictionary.Add(TnefPropertyTag.SenderEmailAddressA, index);
			dictionary.Add(TnefPropertyTag.SenderEmailAddressW, index++);
			dictionary.Add(TnefPropertyTag.SenderAddrtypeA, index);
			dictionary.Add(TnefPropertyTag.SenderAddrtypeW, index++);
			dictionary.Add(TnefPropertyTag.SenderEntryId, index++);
			dictionary.Add(TnefPropertyTag.SentRepresentingNameA, index);
			dictionary.Add(TnefPropertyTag.SentRepresentingNameW, index++);
			dictionary.Add(TnefPropertyTag.SentRepresentingEmailAddressA, index);
			dictionary.Add(TnefPropertyTag.SentRepresentingEmailAddressW, index++);
			dictionary.Add(TnefPropertyTag.SentRepresentingAddrtypeA, index);
			dictionary.Add(TnefPropertyTag.SentRepresentingAddrtypeW, index++);
			dictionary.Add(TnefPropertyTag.SentRepresentingEntryId, index++);
			dictionary.Add(TnefPropertyTag.ClientSubmitTime, index++);
			dictionary.Add(TnefPropertyTag.LastModificationTime, index++);
			dictionary.Add(TnefPropertyTag.ExpiryTime, index++);
			dictionary.Add(TnefPropertyTag.ReplyTime, index++);
			dictionary.Add(TnefPropertyTag.SubjectA, index);
			dictionary.Add(TnefPropertyTag.SubjectW, index++);
			dictionary.Add(TnefPropertyTag.NormalizedSubjectA, index);
			dictionary.Add(TnefPropertyTag.NormalizedSubjectW, index++);
			dictionary.Add(TnefPropertyTag.SubjectPrefixA, index);
			dictionary.Add(TnefPropertyTag.SubjectPrefixW, index++);
			dictionary.Add(TnefPropertyTag.ConversationTopicA, index);
			dictionary.Add(TnefPropertyTag.ConversationTopicW, index++);
			dictionary.Add(TnefPropertyTag.InternetMessageIdA, index);
			dictionary.Add(TnefPropertyTag.InternetMessageIdW, index++);
			dictionary.Add(TnefPropertyTag.Importance, index++);
			dictionary.Add(TnefPropertyTag.Priority, index++);
			dictionary.Add(TnefPropertyTag.Sensitivity, index++);
			dictionary.Add(TnefPropertyTag.MessageClassA, index);
			dictionary.Add(TnefPropertyTag.MessageClassW, index++);
			dictionary.Add(new TnefPropertyTag(TnefPropertyId.Body, TnefPropertyType.Binary), index);
			dictionary.Add(TnefPropertyTag.BodyA, index);
			dictionary.Add(TnefPropertyTag.BodyW, index++);
			dictionary.Add(TnefPropertyTag.RtfCompressed, index++);
			dictionary.Add(TnefPropertyTag.BodyHtmlB, index);
			dictionary.Add(TnefPropertyTag.BodyHtmlA, index);
			dictionary.Add(TnefPropertyTag.BodyHtmlW, index++);
			dictionary.Add(TnefPropertyTag.SendRecallReport, index++);
			dictionary.Add(TnefPropertyTag.OofReplyType, index++);
			dictionary.Add(TnefPropertyTag.AutoForwarded, index++);
			dictionary.Add(TnefPropertyTag.INetMailOverrideFormat, index++);
			dictionary.Add(TnefPropertyTag.INetMailOverrideCharset, index++);
			TnefPropertyBag.AddLookupEntries(dictionary);
			return dictionary;
		}

		private static Dictionary<TnefNameTag, int> GetNamedMessageProperties(ref int index)
		{
			Dictionary<TnefNameTag, int> dictionary = new Dictionary<TnefNameTag, int>(9, TnefPropertyBag.NameTagComparer);
			dictionary.Add(TnefPropertyBag.TnefNameTagIsClassified, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagClassification, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagClassificationDescription, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagClassificationGuid, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagClassificationKeep, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagContentTypeA, index);
			dictionary.Add(TnefPropertyBag.TnefNameTagContentTypeW, index++);
			dictionary.Add(TnefPropertyBag.TnefNameTagContentClassA, index);
			dictionary.Add(TnefPropertyBag.TnefNameTagContentClassW, index++);
			TnefPropertyBag.AddNamedLookupEntries(dictionary);
			return dictionary;
		}

		private static Dictionary<TnefPropertyTag, int> GetAttachmentProperties(ref int index)
		{
			Dictionary<TnefPropertyTag, int> dictionary = new Dictionary<TnefPropertyTag, int>(25, TnefPropertyBag.PropertyTagComparer);
			dictionary.Add(TnefPropertyTag.AttachDataBin, index);
			dictionary.Add(TnefPropertyTag.AttachDataObj, index++);
			dictionary.Add(TnefPropertyTag.AttachMethod, index++);
			dictionary.Add(TnefPropertyTag.AttachLongFilenameA, index);
			dictionary.Add(TnefPropertyTag.AttachLongFilenameW, index++);
			dictionary.Add(TnefPropertyTag.AttachFilenameA, index);
			dictionary.Add(TnefPropertyTag.AttachFilenameW, index++);
			dictionary.Add(TnefPropertyTag.AttachExtensionA, index);
			dictionary.Add(TnefPropertyTag.AttachExtensionW, index++);
			dictionary.Add(TnefPropertyTag.DisplayNameA, index);
			dictionary.Add(TnefPropertyTag.DisplayNameW, index++);
			dictionary.Add(TnefPropertyTag.AttachTransportNameA, index);
			dictionary.Add(TnefPropertyTag.AttachTransportNameW, index++);
			dictionary.Add(TnefPropertyTag.AttachPathnameA, index);
			dictionary.Add(TnefPropertyTag.AttachPathnameW, index++);
			dictionary.Add(TnefPropertyTag.AttachMimeTagA, index);
			dictionary.Add(TnefPropertyTag.AttachMimeTagW, index++);
			dictionary.Add(TnefPropertyTag.RenderingPosition, index++);
			dictionary.Add(TnefPropertyTag.AttachRendering, index++);
			dictionary.Add(TnefPropertyTag.AttachContentIdA, index++);
			dictionary.Add(TnefPropertyTag.AttachContentIdW, index++);
			dictionary.Add(TnefPropertyTag.AttachContentLocationA, index++);
			dictionary.Add(TnefPropertyTag.AttachContentLocationW, index++);
			dictionary.Add(TnefPropertyTag.AttachmentFlags, index++);
			dictionary.Add(TnefPropertyTag.AttachHidden, index++);
			TnefPropertyBag.AddLookupEntries(dictionary);
			return dictionary;
		}

		internal void Dispose()
		{
			foreach (TnefPropertyBag.PropertyData propertyData in this.properties)
			{
				StoragePropertyValue storagePropertyValue = propertyData.Value as StoragePropertyValue;
				if (storagePropertyValue != null)
				{
					storagePropertyValue.DisposeStorage();
				}
			}
			if (this.newProperties != null)
			{
				foreach (KeyValuePair<TnefPropertyTag, object> keyValuePair in this.newProperties)
				{
					StoragePropertyValue storagePropertyValue2 = keyValuePair.Value as StoragePropertyValue;
					if (storagePropertyValue2 != null)
					{
						storagePropertyValue2.DisposeStorage();
					}
				}
				this.newProperties = null;
			}
			this.newNamedProperties = null;
		}

		public object GetProperty(TnefPropertyTag tag, bool toUnicode)
		{
			if (toUnicode && TnefPropertyType.String8 == tag.TnefType)
			{
				tag = tag.ToUnicode();
			}
			return this[tag];
		}

		public void SetProperty(TnefPropertyTag tag, bool toUnicode, object value)
		{
			if (toUnicode && TnefPropertyType.String8 == tag.TnefType)
			{
				tag = tag.ToUnicode();
			}
			this[tag] = value;
		}

		internal object this[TnefPropertyTag tag]
		{
			get
			{
				object obj = this[tag.Id];
				if (obj != null)
				{
					return obj;
				}
				if (this.newProperties == null)
				{
					return null;
				}
				if (!this.newProperties.TryGetValue(tag, out obj))
				{
					return null;
				}
				return obj;
			}
			set
			{
				if (this[tag.Id] != null)
				{
					this[tag.Id] = value;
					return;
				}
				if (value != null)
				{
					if (this.newProperties == null)
					{
						this.newProperties = new Dictionary<TnefPropertyTag, object>(TnefPropertyBag.PropertyTagComparer);
					}
					this.newProperties[tag] = value;
					return;
				}
				if (this.newProperties != null)
				{
					this.newProperties.Remove(tag);
				}
			}
		}

		internal object this[TnefNameTag nameTag]
		{
			get
			{
				object obj = this[nameTag.Id];
				if (obj != null)
				{
					return obj;
				}
				if (this.newNamedProperties == null)
				{
					return null;
				}
				if (!this.newNamedProperties.TryGetValue(nameTag, out obj))
				{
					return null;
				}
				return obj;
			}
			set
			{
				if (this[nameTag.Id] != null)
				{
					this[nameTag.Id] = value;
					return;
				}
				if (value != null)
				{
					if (this.newNamedProperties == null)
					{
						this.newNamedProperties = new Dictionary<TnefNameTag, object>(TnefPropertyBag.NameTagComparer);
					}
					this.newNamedProperties[nameTag] = value;
					return;
				}
				if (this.newNamedProperties != null)
				{
					this.newNamedProperties.Remove(nameTag);
				}
			}
		}

		internal object this[TnefPropertyId id]
		{
			get
			{
				return this[this.GetIndex(id)];
			}
			set
			{
				if (this[id] != value)
				{
					this.SetDirty();
					this.Touch(id);
				}
				this[this.GetIndex(id)] = value;
			}
		}

		internal object this[TnefNameId nameId]
		{
			get
			{
				return this[this.GetIndex(nameId)];
			}
			set
			{
				if (this[nameId] != value)
				{
					this.SetDirty();
					this.Touch(nameId);
				}
				this[this.GetIndex(nameId)] = value;
			}
		}

		private static void StartAttributeIfNecessary(TnefWriter writer, TnefAttributeTag attributeTag, TnefAttributeLevel attributeLevel, ref bool startAttribute)
		{
			if (startAttribute)
			{
				writer.StartAttribute(attributeTag, attributeLevel);
				startAttribute = false;
			}
		}

		private static bool IsLegacyAttribute(TnefAttributeTag tag)
		{
			return tag != TnefAttributeTag.MapiProperties && tag != TnefAttributeTag.Attachment && tag != TnefAttributeTag.RecipientTable;
		}

		private static void AddLookupEntries(Dictionary<TnefPropertyTag, int> dictionary)
		{
			Dictionary<TnefPropertyTag, int> dictionary2 = new Dictionary<TnefPropertyTag, int>(dictionary.Count);
			foreach (KeyValuePair<TnefPropertyTag, int> keyValuePair in dictionary)
			{
				TnefPropertyTag key = new TnefPropertyTag(keyValuePair.Key.Id, TnefPropertyType.Null);
				if (!dictionary2.ContainsKey(key))
				{
					dictionary2.Add(key, keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<TnefPropertyTag, int> keyValuePair2 in dictionary2)
			{
				dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
		}

		private static void AddNamedLookupEntries(Dictionary<TnefNameTag, int> dictionary)
		{
			Dictionary<TnefNameTag, int> dictionary2 = new Dictionary<TnefNameTag, int>(dictionary.Count);
			foreach (KeyValuePair<TnefNameTag, int> keyValuePair in dictionary)
			{
				TnefNameTag key = new TnefNameTag(keyValuePair.Key.Id, TnefPropertyType.Null);
				if (!dictionary2.ContainsKey(key))
				{
					dictionary2.Add(key, keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<TnefNameTag, int> keyValuePair2 in dictionary2)
			{
				dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
		}

		private object this[int index]
		{
			get
			{
				return this.properties[index].Value;
			}
			set
			{
				StoragePropertyValue storagePropertyValue = this.properties[index].Value as StoragePropertyValue;
				if (storagePropertyValue != null)
				{
					storagePropertyValue.DisposeStorage();
				}
				this.properties[index].Value = value;
			}
		}

		private int GetIndex(TnefPropertyId id)
		{
			TnefPropertyTag key = new TnefPropertyTag(id, TnefPropertyType.Null);
			int result;
			this.supportedProperties.TryGetValue(key, out result);
			return result;
		}

		private int GetIndex(TnefNameId nameId)
		{
			TnefNameTag key = new TnefNameTag(nameId, TnefPropertyType.Null);
			int result;
			this.supportedNamedProperties.TryGetValue(key, out result);
			return result;
		}

		internal void Touch(TnefPropertyId id)
		{
			this.Touch(this.GetIndex(id));
		}

		private void Touch(TnefNameId nameId)
		{
			this.Touch(this.GetIndex(nameId));
		}

		private void Touch(int index)
		{
			this.properties[index].IsDirty = true;
		}

		internal bool Load(TnefReader reader, DataStorage tnefStorage, long tnefStart, long tnefEnd, TnefAttributeLevel level, int embeddingDepth, Charset binaryCharset)
		{
			bool result;
			while ((result = reader.ReadNextAttribute()) && TnefAttributeTag.AttachRenderData != reader.AttributeTag && level == reader.AttributeLevel)
			{
				if (TnefAttributeTag.RecipientTable == reader.AttributeTag)
				{
					if (level == TnefAttributeLevel.Message && this.parentMessage != null && 0 < embeddingDepth)
					{
						this.parentMessage.LoadRecipients(reader.PropertyReader);
					}
				}
				else
				{
					TnefPropertyReader propertyReader = reader.PropertyReader;
					while (propertyReader.ReadNextProperty())
					{
						this.LoadProperty(propertyReader, tnefStorage, tnefStart, tnefEnd, level, embeddingDepth, binaryCharset);
					}
				}
			}
			return result;
		}

		internal bool Write(TnefReader reader, TnefWriter writer, TnefAttributeLevel level, bool dropRecipientTable, bool forceUnicode, byte[] scratchBuffer)
		{
			IDictionary<TnefPropertyTag, object> dictionary = null;
			char[] array = null;
			bool result;
			for (;;)
			{
				TnefPropertyReader propertyReader = reader.PropertyReader;
				if (0 >= propertyReader.PropertyCount)
				{
					goto IL_37A;
				}
				TnefAttributeTag attributeTag = reader.AttributeTag;
				TnefAttributeLevel attributeLevel = reader.AttributeLevel;
				bool flag = true;
				while (propertyReader.ReadNextProperty())
				{
					TnefPropertyTag propertyTag = propertyReader.PropertyTag;
					if (TnefPropertyType.Null != propertyTag.ValueTnefType)
					{
						if (propertyReader.IsNamedProperty)
						{
							TnefNameId propertyNameId = propertyReader.PropertyNameId;
							TnefNameTag key = new TnefNameTag(propertyNameId, propertyTag.ValueTnefType);
							int num;
							if (this.supportedNamedProperties.TryGetValue(key, out num) && this.properties[num].IsDirty)
							{
								object obj = this[propertyNameId];
								if (obj != null)
								{
									TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref flag);
									writer.StartProperty(propertyTag, propertyNameId.PropertySetGuid, propertyNameId.Id);
									writer.WritePropertyValue(obj);
									continue;
								}
								continue;
							}
						}
						else
						{
							TnefPropertyId id = propertyTag.Id;
							int num2;
							if (this.supportedProperties.TryGetValue(propertyTag, out num2) && this.properties[num2].IsDirty && (this.attachmentData == null || this.attachmentData.EmbeddedMessage == null || TnefAttributeLevel.Attachment != level || TnefAttributeTag.AttachData != attributeTag || TnefPropertyId.AttachData != id))
							{
								object obj = this[id];
								if (obj == null)
								{
									continue;
								}
								if (!this.WriteModifiedProperty(writer, reader, propertyTag, obj, forceUnicode, ref flag, scratchBuffer))
								{
									if (dictionary == null)
									{
										dictionary = new Dictionary<TnefPropertyTag, object>(TnefPropertyBag.PropertyTagComparer);
									}
									if (!dictionary.ContainsKey(propertyTag))
									{
										dictionary.Add(propertyTag, obj);
										continue;
									}
									continue;
								}
								else
								{
									if (dictionary != null && dictionary.ContainsKey(propertyTag))
									{
										dictionary.Remove(propertyTag);
										continue;
									}
									continue;
								}
							}
						}
						if (propertyTag.ValueTnefType == TnefPropertyType.String8 && forceUnicode)
						{
							if (!TnefPropertyBag.IsLegacyAttribute(attributeTag))
							{
								TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref flag);
								TnefPropertyBag.WriteUnicodeProperty(writer, propertyReader, propertyTag.ToUnicode(), ref array);
							}
						}
						else if (propertyTag.IsTnefTypeValid)
						{
							TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref flag);
							writer.WriteProperty(propertyReader);
						}
					}
				}
				if ((TnefAttributeTag.MapiProperties == attributeTag && level == TnefAttributeLevel.Message) || (TnefAttributeTag.Attachment == attributeTag && level == TnefAttributeLevel.Attachment))
				{
					if (this.newProperties != null)
					{
						foreach (KeyValuePair<TnefPropertyTag, object> keyValuePair in this.newProperties)
						{
							object obj = keyValuePair.Value;
							if (obj != null)
							{
								this.WriteModifiedProperty(writer, reader, keyValuePair.Key, obj, forceUnicode, ref flag, scratchBuffer);
							}
						}
					}
					if (dictionary != null)
					{
						foreach (KeyValuePair<TnefPropertyTag, object> keyValuePair2 in dictionary)
						{
							this.WriteModifiedProperty(writer, reader, keyValuePair2.Key, keyValuePair2.Value, forceUnicode, ref flag, scratchBuffer);
						}
					}
					if (this.newNamedProperties != null)
					{
						using (IEnumerator<KeyValuePair<TnefNameTag, object>> enumerator3 = this.newNamedProperties.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<TnefNameTag, object> keyValuePair3 = enumerator3.Current;
								object obj = keyValuePair3.Value;
								if (obj != null)
								{
									TnefPropertyTag tag = new TnefPropertyTag((TnefPropertyId)(-32768), keyValuePair3.Key.Type);
									if (forceUnicode)
									{
										tag = tag.ToUnicode();
									}
									TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref flag);
									writer.StartProperty(tag, keyValuePair3.Key.Id.PropertySetGuid, keyValuePair3.Key.Id.Id);
									writer.WritePropertyValue(obj);
								}
							}
							goto IL_3AC;
						}
						goto IL_37A;
					}
				}
				IL_3AC:
				if (!(result = reader.ReadNextAttribute()) || level != reader.AttributeLevel || TnefAttributeTag.AttachRenderData == reader.AttributeTag)
				{
					break;
				}
				continue;
				IL_37A:
				if (level != TnefAttributeLevel.Message || TnefAttributeTag.RecipientTable != reader.AttributeTag)
				{
					writer.WriteAttribute(reader);
					goto IL_3AC;
				}
				if (!dropRecipientTable)
				{
					this.parentMessage.WriteRecipients(reader.PropertyReader, writer, ref array);
					goto IL_3AC;
				}
				goto IL_3AC;
			}
			return result;
		}

		private void LoadProperty(TnefPropertyReader propertyReader, DataStorage tnefStorage, long tnefStart, long tnefEnd, TnefAttributeLevel level, int embeddingDepth, Charset binaryCharset)
		{
			TnefPropertyTag propertyTag = propertyReader.PropertyTag;
			if (propertyTag.IsMultiValued)
			{
				return;
			}
			if (TnefPropertyType.Null == propertyTag.ValueTnefType)
			{
				return;
			}
			if (propertyReader.IsNamedProperty)
			{
				TnefNameId propertyNameId = propertyReader.PropertyNameId;
				TnefNameTag key = new TnefNameTag(propertyNameId, propertyTag.ValueTnefType);
				if (this.supportedNamedProperties.ContainsKey(key))
				{
					if (propertyReader.IsLargeValue)
					{
						return;
					}
					this[this.GetIndex(propertyNameId)] = propertyReader.ReadValue();
				}
				return;
			}
			if (!this.supportedProperties.ContainsKey(propertyTag))
			{
				return;
			}
			TnefPropertyId id = propertyTag.Id;
			int index = this.GetIndex(id);
			if (TnefPropertyId.Body == id || TnefPropertyId.RtfCompressed == id || TnefPropertyId.BodyHtml == id)
			{
				tnefStart += (long)propertyReader.RawValueStreamOffset;
				tnefEnd = tnefStart + (long)propertyReader.RawValueLength;
				this[index] = new StoragePropertyValue(propertyTag, tnefStorage, tnefStart, tnefEnd);
				return;
			}
			if (TnefPropertyId.AttachData == id)
			{
				tnefStart += (long)propertyReader.RawValueStreamOffset;
				tnefEnd = tnefStart + (long)propertyReader.RawValueLength;
				this[index] = new StoragePropertyValue(propertyTag, tnefStorage, tnefStart, tnefEnd);
				if (!propertyReader.IsEmbeddedMessage)
				{
					return;
				}
				if (++embeddingDepth > 100)
				{
					throw new MimeException(EmailMessageStrings.NestingTooDeep(embeddingDepth, 100));
				}
				using (TnefReader embeddedMessageReader = propertyReader.GetEmbeddedMessageReader())
				{
					PureTnefMessage pureTnefMessage = new PureTnefMessage(this.attachmentData, tnefStorage, tnefStart, tnefEnd);
					pureTnefMessage.Load(embeddedMessageReader, embeddingDepth, binaryCharset);
					EmailMessage embeddedMessage = new EmailMessage(pureTnefMessage);
					this.attachmentData.EmbeddedMessage = embeddedMessage;
					return;
				}
			}
			if (propertyReader.IsLargeValue)
			{
				return;
			}
			if (TnefPropertyId.InternetCPID == id)
			{
				if (TnefPropertyType.Long == propertyTag.TnefType)
				{
					int num = propertyReader.ReadValueAsInt32();
					this[index] = num;
					return;
				}
			}
			else
			{
				this[index] = propertyReader.ReadValue();
			}
		}

		internal static void WriteUnicodeProperty(TnefWriter writer, TnefPropertyReader propertyReader, TnefPropertyTag tag, ref char[] buffer)
		{
			if (tag.IsNamed)
			{
				TnefNameId propertyNameId = propertyReader.PropertyNameId;
				writer.StartProperty(tag, propertyNameId.PropertySetGuid, propertyNameId.Id);
			}
			else
			{
				writer.StartProperty(tag);
			}
			if (tag.IsMultiValued)
			{
				while (propertyReader.ReadNextValue())
				{
					writer.StartPropertyValue();
					TnefPropertyBag.WriteUnicodePropertyValue(writer, propertyReader, ref buffer);
				}
				return;
			}
			TnefPropertyBag.WriteUnicodePropertyValue(writer, propertyReader, ref buffer);
		}

		internal static void WriteUnicodePropertyValue(TnefWriter writer, TnefPropertyReader propertyReader, ref char[] buffer)
		{
			if (buffer == null)
			{
				buffer = new char[1024];
			}
			int count;
			while ((count = propertyReader.ReadTextValue(buffer, 0, buffer.Length)) != 0)
			{
				writer.WritePropertyTextValue(buffer, 0, count);
			}
		}

		private bool WriteModifiedProperty(TnefWriter writer, TnefReader reader, TnefPropertyTag propertyTag, object value, bool forceUnicode, ref bool startAttribute, byte[] scratchBuffer)
		{
			TnefPropertyReader propertyReader = reader.PropertyReader;
			TnefAttributeTag attributeTag = reader.AttributeTag;
			TnefAttributeLevel attributeLevel = reader.AttributeLevel;
			StoragePropertyValue storagePropertyValue = value as StoragePropertyValue;
			if (storagePropertyValue != null)
			{
				if (this.attachmentData != null && this.attachmentData.EmbeddedMessage != null && propertyReader.IsEmbeddedMessage)
				{
					using (TnefReader embeddedMessageReader = propertyReader.GetEmbeddedMessageReader())
					{
						TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref startAttribute);
						writer.StartProperty(propertyTag);
						EmailMessage embeddedMessage = this.attachmentData.EmbeddedMessage;
						PureTnefMessage pureTnefMessage = embeddedMessage.PureTnefMessage;
						Charset textCharset = pureTnefMessage.TextCharset;
						using (TnefWriter embeddedMessageWriter = writer.GetEmbeddedMessageWriter(textCharset.CodePage))
						{
							pureTnefMessage.WriteMessage(embeddedMessageReader, embeddedMessageWriter, scratchBuffer);
						}
						return true;
					}
				}
				using (Stream readStream = storagePropertyValue.GetReadStream())
				{
					int num = readStream.Read(scratchBuffer, 0, scratchBuffer.Length);
					if (num > 0)
					{
						propertyTag = storagePropertyValue.PropertyTag;
						if (propertyTag.ValueTnefType == TnefPropertyType.Unicode && TnefPropertyBag.IsLegacyAttribute(attributeTag))
						{
							return false;
						}
						TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref startAttribute);
						writer.StartProperty(propertyTag);
						do
						{
							writer.WritePropertyRawValue(scratchBuffer, 0, num);
							num = readStream.Read(scratchBuffer, 0, scratchBuffer.Length);
						}
						while (num > 0);
					}
					return true;
				}
			}
			if (propertyTag.ValueTnefType == TnefPropertyType.String8 && forceUnicode)
			{
				if (TnefPropertyBag.IsLegacyAttribute(attributeTag))
				{
					return false;
				}
				TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref startAttribute);
				writer.WriteProperty(propertyTag.ToUnicode(), value);
			}
			else
			{
				TnefPropertyBag.StartAttributeIfNecessary(writer, attributeTag, attributeLevel, ref startAttribute);
				writer.WriteProperty(propertyTag, value);
			}
			return true;
		}

		private void SetDirty()
		{
			if (this.attachmentData != null)
			{
				((PureTnefMessage)this.attachmentData.MessageImplementation).SetDirty();
				return;
			}
			if (this.parentMessage != null)
			{
				this.parentMessage.SetDirty();
			}
		}

		internal const int MaxTnefDepth = 100;

		internal static readonly TnefPropertyBag.TnefPropertyTagComparer PropertyTagComparer = new TnefPropertyBag.TnefPropertyTagComparer();

		internal static readonly TnefPropertyBag.TnefNameTagComparer NameTagComparer = new TnefPropertyBag.TnefNameTagComparer();

		internal static readonly TnefNameTag TnefNameTagIsClassified = new TnefNameTag(new TnefNameId(TnefPropertyBag.PropertySetIDCommon, 34229), TnefPropertyType.Boolean);

		internal static readonly TnefNameTag TnefNameTagClassification = new TnefNameTag(new TnefNameId(TnefPropertyBag.PropertySetIDCommon, 34230), TnefPropertyType.Unicode);

		internal static readonly TnefNameTag TnefNameTagClassificationDescription = new TnefNameTag(new TnefNameId(TnefPropertyBag.PropertySetIDCommon, 34231), TnefPropertyType.Unicode);

		internal static readonly TnefNameTag TnefNameTagClassificationGuid = new TnefNameTag(new TnefNameId(TnefPropertyBag.PropertySetIDCommon, 34232), TnefPropertyType.Unicode);

		internal static readonly TnefNameTag TnefNameTagClassificationKeep = new TnefNameTag(new TnefNameId(TnefPropertyBag.PropertySetIDCommon, 34234), TnefPropertyType.Boolean);

		internal static readonly TnefNameId TnefNameIdContentType = new TnefNameId(new Guid("00020386-0000-0000-c000-000000000046"), "content-type");

		internal static readonly TnefNameId TnefNameIdContentClass = new TnefNameId(new Guid("00020386-0000-0000-c000-000000000046"), "content-class");

		private static readonly Guid PropertySetIDCommon = new Guid("{00062008-0000-0000-C000-000000000046}");

		private static readonly TnefNameTag TnefNameTagContentTypeA = new TnefNameTag(TnefPropertyBag.TnefNameIdContentType, TnefPropertyType.String8);

		private static readonly TnefNameTag TnefNameTagContentTypeW = new TnefNameTag(TnefPropertyBag.TnefNameIdContentType, TnefPropertyType.Unicode);

		private static readonly TnefNameTag TnefNameTagContentClassA = new TnefNameTag(TnefPropertyBag.TnefNameIdContentClass, TnefPropertyType.String8);

		private static readonly TnefNameTag TnefNameTagContentClassW = new TnefNameTag(TnefPropertyBag.TnefNameIdContentClass, TnefPropertyType.Unicode);

		private static readonly Dictionary<TnefPropertyTag, int> MessageProperties;

		private static readonly Dictionary<TnefNameTag, int> NamedMessageProperties;

		private static readonly Dictionary<TnefPropertyTag, int> AttachmentProperties;

		private static readonly Dictionary<TnefNameTag, int> NamedAttachmentProperties;

		private static readonly int MessagePropertyCount;

		private static readonly int AttachmentPropertyCount;

		private Dictionary<TnefPropertyTag, int> supportedProperties;

		private Dictionary<TnefNameTag, int> supportedNamedProperties;

		private TnefPropertyBag.PropertyData[] properties;

		private PureTnefMessage parentMessage;

		private TnefAttachmentData attachmentData;

		private IDictionary<TnefPropertyTag, object> newProperties;

		private IDictionary<TnefNameTag, object> newNamedProperties;

		internal class TnefPropertyTagComparer : IEqualityComparer<TnefPropertyTag>
		{
			public bool Equals(TnefPropertyTag x, TnefPropertyTag y)
			{
				return x == y;
			}

			public int GetHashCode(TnefPropertyTag obj)
			{
				return obj;
			}
		}

		internal class TnefNameTagComparer : IEqualityComparer<TnefNameTag>
		{
			public bool Equals(TnefNameTag x, TnefNameTag y)
			{
				return x.Type == y.Type && x.Id.Kind == y.Id.Kind && x.Id.Id == y.Id.Id && x.Id.PropertySetGuid == y.Id.PropertySetGuid && string.Equals(x.Id.Name, y.Id.Name);
			}

			public int GetHashCode(TnefNameTag obj)
			{
				return (int)obj.Type ^ obj.Id.PropertySetGuid.GetHashCode() ^ ((obj.Id.Kind == TnefNameIdKind.Id) ? obj.Id.Id : obj.Id.Name.GetHashCode());
			}
		}

		internal struct PropertyData
		{
			public PropertyData(object value, bool isDirty)
			{
				this.value = value;
				this.isDirty = isDirty;
			}

			public object Value
			{
				get
				{
					return this.value;
				}
				set
				{
					this.value = value;
				}
			}

			public bool IsDirty
			{
				get
				{
					return this.isDirty;
				}
				set
				{
					this.isDirty = value;
				}
			}

			private object value;

			private bool isDirty;
		}
	}
}
