using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	internal class PropertyGroupMapping
	{
		public PropertyGroupMapping(int mappingId, IList<IList<AnnotatedPropertyTag>> propGroups)
		{
			Util.ThrowOnNullArgument(propGroups, "propGroups");
			this.mappingId = mappingId;
			this.propGroups = propGroups;
			for (int i = 0; i < propGroups.Count; i++)
			{
				IList<AnnotatedPropertyTag> list = propGroups[i];
				foreach (AnnotatedPropertyTag annotatedPropertyTag in list)
				{
					if (annotatedPropertyTag.PropertyTag == PropertyTag.MessageRecipients)
					{
						this.recipientsGroupIndex = i;
					}
					else if (annotatedPropertyTag.PropertyTag == PropertyTag.MessageAttachments)
					{
						this.attachmentsGroupIndex = i;
					}
				}
				if (this.recipientsGroupIndex != -1 && this.attachmentsGroupIndex != -1)
				{
					return;
				}
			}
		}

		public int MappingId
		{
			get
			{
				return this.mappingId;
			}
		}

		public int GroupCount
		{
			get
			{
				return this.propGroups.Count;
			}
		}

		public IList<AnnotatedPropertyTag> this[int groupIndex]
		{
			get
			{
				return this.propGroups[groupIndex];
			}
		}

		public int RecipientsGroupIndex
		{
			get
			{
				return this.recipientsGroupIndex;
			}
		}

		public int AttachmentsGroupIndex
		{
			get
			{
				return this.attachmentsGroupIndex;
			}
		}

		public static PropertyGroupMapping Deserialize(byte[] buffer)
		{
			PropertyGroupMapping result;
			using (BufferReader bufferReader = Reader.CreateBufferReader(buffer))
			{
				ulong num = bufferReader.ReadUInt64();
				if (0L != 0L)
				{
					throw new BufferParseException("Error parsing property group mapping - invalid mapping ID");
				}
				int num2 = bufferReader.ReadInt32();
				if (num2 <= 0 || num2 > 64)
				{
					throw new BufferParseException("Error parsing property group mapping - invalid group count");
				}
				List<IList<AnnotatedPropertyTag>> list = new List<IList<AnnotatedPropertyTag>>(num2);
				for (int i = 0; i < num2; i++)
				{
					int num3 = bufferReader.ReadInt32();
					if (num3 < 0 || num3 > 1000)
					{
						throw new BufferParseException("Error parsing property group mapping - invalid property count");
					}
					List<AnnotatedPropertyTag> list2 = new List<AnnotatedPropertyTag>(num3);
					for (int j = 0; j < num3; j++)
					{
						PropertyTag propertyTag = new PropertyTag(bufferReader.ReadUInt32());
						NamedProperty namedProperty = null;
						if (propertyTag.IsNamedProperty)
						{
							Guid guid = bufferReader.ReadGuid();
							uint num4 = bufferReader.ReadUInt32();
							if (num4 == 0U)
							{
								namedProperty = new NamedProperty(guid, bufferReader.ReadUInt32());
							}
							else
							{
								if (num4 != 1U)
								{
									throw new BufferParseException("Error parsing property group mapping - invalid named property kind");
								}
								namedProperty = new NamedProperty(guid, bufferReader.ReadUnicodeString(StringFlags.Sized32));
							}
						}
						if (propertyTag.PropertyId != PropertyId.Null)
						{
							list2.Add(new AnnotatedPropertyTag(propertyTag, namedProperty));
						}
					}
					list.Add(new ReadOnlyCollection<AnnotatedPropertyTag>(list2));
				}
				result = new PropertyGroupMapping((int)num, new ReadOnlyCollection<IList<AnnotatedPropertyTag>>(list));
			}
			return result;
		}

		public bool IsValidPropGroupIndex(int propGroupIndex)
		{
			return propGroupIndex >= -1 && propGroupIndex < this.GroupCount;
		}

		public byte[] Serialize()
		{
			uint num = 0U;
			using (CountWriter countWriter = new CountWriter())
			{
				this.Serialize(countWriter);
				num = (uint)countWriter.Position;
			}
			byte[] array = new byte[num];
			using (Writer writer = new BufferWriter(array))
			{
				this.Serialize(writer);
			}
			return array;
		}

		public bool IsPropertyInAnyGroup(PropertyTag propertyTag)
		{
			if (this.propertiesInAnyNumberedGroup == null)
			{
				this.propertiesInAnyNumberedGroup = new HashSet<PropertyTag>();
				for (int i = 0; i < this.propGroups.Count; i++)
				{
					IList<AnnotatedPropertyTag> list = this.propGroups[i];
					for (int j = 0; j < list.Count; j++)
					{
						this.propertiesInAnyNumberedGroup.Add(list[j].PropertyTag);
					}
				}
			}
			return this.propertiesInAnyNumberedGroup.Contains(propertyTag);
		}

		private void Serialize(Writer writer)
		{
			writer.WriteUInt64((ulong)this.mappingId);
			writer.WriteInt32(this.propGroups.Count);
			for (int i = 0; i < this.propGroups.Count; i++)
			{
				IList<AnnotatedPropertyTag> list = this.propGroups[i];
				writer.WriteInt32(list.Count);
				for (int j = 0; j < list.Count; j++)
				{
					AnnotatedPropertyTag annotatedPropertyTag = list[j];
					writer.WritePropertyTag(annotatedPropertyTag.PropertyTag);
					if (annotatedPropertyTag.PropertyTag.IsNamedProperty)
					{
						writer.WriteGuid(annotatedPropertyTag.NamedProperty.Guid);
						writer.WriteUInt32((uint)annotatedPropertyTag.NamedProperty.Kind);
						if (annotatedPropertyTag.NamedProperty.Kind == NamedPropertyKind.String)
						{
							writer.WriteUnicodeString(annotatedPropertyTag.NamedProperty.Name, StringFlags.Sized32);
						}
						else if (annotatedPropertyTag.NamedProperty.Kind == NamedPropertyKind.Id)
						{
							writer.WriteUInt32(annotatedPropertyTag.NamedProperty.Id);
						}
					}
				}
			}
		}

		public const int OtherGroupIndex = -1;

		public static byte[] SerializedFakeMapping = new PropertyGroupMapping(-1, new IList<AnnotatedPropertyTag>[0]).Serialize();

		private int mappingId;

		private IList<IList<AnnotatedPropertyTag>> propGroups;

		private int recipientsGroupIndex = -1;

		private int attachmentsGroupIndex = -1;

		private HashSet<PropertyTag> propertiesInAnyNumberedGroup;
	}
}
