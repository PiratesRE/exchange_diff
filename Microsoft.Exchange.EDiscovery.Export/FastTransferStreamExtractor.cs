using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.PST;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class FastTransferStreamExtractor
	{
		public FastTransferStreamExtractor(ExtractContext context, bool removeMetadata)
		{
			this.extractContext = context;
			this.data = context.Item.Data;
			this.currentStreamBufferEnd = 0;
			this.removeMetadata = removeMetadata;
			this.fixedSizePropertyExtractor = new FastTransferStreamExtractor.FixedSizePropertyExtractor();
			this.variableSizePropertyExtractor = new FastTransferStreamExtractor.VariableSizePropertyExtractor();
			this.multivaluedFixedSizePropertyExtractor = new FastTransferStreamExtractor.MultivaluedFixedSizePropertyExtractor();
			this.multivaluedVariableSizePropertyExtractor = new FastTransferStreamExtractor.MultivaluedVariableSizePropertyExtractor();
		}

		public void Extract()
		{
			using (this.dataStream = new MemoryStream(this.data, false))
			{
				this.ReadStreamHeader();
				while (this.dataStream.Position < this.dataStream.Length)
				{
					Guid a;
					uint? num;
					string a2;
					PropertyTag propTag = this.ReadPropertyTag(out a, out num, out a2);
					ushort id = propTag.Id;
					switch (id)
					{
					case 16384:
						this.extractContext.EnterAttachmentContext();
						break;
					case 16385:
						this.extractContext.EnterMessageContext(null);
						break;
					case 16386:
						this.extractContext.ExitMessageContext();
						break;
					case 16387:
						this.extractContext.EnterRecipientContext();
						break;
					case 16388:
						this.extractContext.ExitRecipientContext();
						break;
					default:
						if (id != 16398)
						{
							if (id != 16406)
							{
								FastTransferStreamExtractor.PropertyExtractor propertyExtractor = this.GetPropertyExtractor(propTag);
								if (this.removeMetadata)
								{
									Func<object, object> propertyValueProcessor = null;
									if (propTag.Value == PropertyTag.Importance.Value)
									{
										propertyValueProcessor = delegate(object x)
										{
											((byte[])x)[0] = 1;
											return x;
										};
									}
									else if (propTag.Value == PropertyTag.MessageFlags.Value)
									{
										propertyValueProcessor = delegate(object x)
										{
											byte[] array = (byte[])x;
											byte[] array2 = array;
											int num2 = 0;
											array2[num2] |= 1;
											byte[] array3 = array;
											int num3 = 1;
											array3[num3] |= 4;
											return array;
										};
									}
									else if (propTag.IsNamedProperty && a == FastTransferStreamExtractor.PublicStringsPropertySet && a2 == "Keywords")
									{
										propertyValueProcessor = ((object x) => null);
									}
									propertyExtractor.PropertyValueProcessor = propertyValueProcessor;
								}
								propertyExtractor.Extract();
							}
							else
							{
								this.ReadBytes(4);
							}
						}
						else
						{
							this.extractContext.ExitAttachmentContext();
						}
						break;
					}
				}
			}
		}

		private byte[] ReadBytes(int size)
		{
			if ((long)size + this.dataStream.Position > this.dataStream.Length)
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted, "Request reading more data than the message stream.");
			}
			byte[] array = new byte[size];
			int i = size;
			while (i > 0)
			{
				if (this.dataStream.Position + (long)i <= (long)this.currentStreamBufferEnd)
				{
					this.SafeReadBytes(array, size - i, i);
					i = 0;
				}
				else
				{
					int num = this.currentStreamBufferEnd - (int)this.dataStream.Position;
					this.SafeReadBytes(array, size - i, num);
					i -= num;
					byte[] array2 = new byte[4];
					this.SafeReadBytes(array2, 0, 4);
					if (BitConverter.ToUInt32(array2, 0) != 2U)
					{
						throw new ExportException(ExportErrorType.MessageDataCorrupted, string.Format(CultureInfo.CurrentCulture, "Unexpected data 0x{0}, expecting 0x00000002", new object[]
						{
							BitConverter.ToUInt32(array2, 0).ToString("X", CultureInfo.InvariantCulture)
						}));
					}
					this.SafeReadBytes(array2, 0, 4);
					int num2 = BitConverter.ToInt32(array2, 0);
					this.currentStreamBufferEnd += 8 + num2;
					this.CheckStreamBufferEnd();
				}
			}
			return array;
		}

		private PropertyTag ReadPropertyTag(out Guid propertySet, out uint? numericId, out string propertyName)
		{
			byte[] value = this.ReadBytes(4);
			PropertyTag result = default(PropertyTag);
			result.Value = BitConverter.ToUInt32(value, 0);
			if (result.IsNamedProperty)
			{
				byte[] b = this.ReadBytes(16);
				byte[] array = this.ReadBytes(1);
				propertySet = new Guid(b);
				uint num;
				if (array[0] == 0)
				{
					byte[] value2 = this.ReadBytes(4);
					numericId = new uint?(BitConverter.ToUInt32(value2, 0));
					num = (uint)this.extractContext.PstSession.ReadIdFromNamedProp(null, numericId.Value, propertySet, true);
					propertyName = null;
				}
				else
				{
					string @string;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						byte[] array2;
						do
						{
							array2 = this.ReadBytes(2);
							memoryStream.Write(array2, 0, 2);
						}
						while (array2[0] != 0 || array2[1] != 0);
						memoryStream.Seek(0L, SeekOrigin.Begin);
						byte[] array3 = new byte[memoryStream.Length - 2L];
						memoryStream.Read(array3, 0, array3.Length);
						@string = Encoding.Unicode.GetString(array3);
					}
					num = (uint)this.extractContext.PstSession.ReadIdFromNamedProp(@string, 0U, propertySet, true);
					propertyName = @string;
					numericId = null;
				}
				result.Value = ((uint)result.Type | num << 16);
			}
			else
			{
				propertySet = Guid.Empty;
				numericId = null;
				propertyName = null;
			}
			return result;
		}

		private FastTransferStreamExtractor.PropertyExtractor GetPropertyExtractor(PropertyTag propTag)
		{
			FastTransferStreamExtractor.PropertyExtractor propertyExtractor;
			if (propTag.IsMultivalued)
			{
				if (propTag.IsFixedSize)
				{
					propertyExtractor = this.multivaluedFixedSizePropertyExtractor;
				}
				else
				{
					propertyExtractor = this.multivaluedVariableSizePropertyExtractor;
				}
			}
			else if (propTag.IsFixedSize)
			{
				propertyExtractor = this.fixedSizePropertyExtractor;
			}
			else
			{
				propertyExtractor = this.variableSizePropertyExtractor;
			}
			propertyExtractor.PropertyTag = propTag;
			propertyExtractor.PropertyBag = this.extractContext.CurrentPropertyBag;
			propertyExtractor.StreamExtractor = this;
			propertyExtractor.PropertyValueProcessor = null;
			return propertyExtractor;
		}

		private void SafeReadBytes(byte[] buffer, int offset, int size)
		{
			int num = this.dataStream.Read(buffer, offset, size);
			if (num != size)
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted, string.Format(CultureInfo.CurrentCulture, "SafeReadBytes: requesting {0} bytes but only {1} bytes left", new object[]
				{
					size,
					num
				}));
			}
		}

		private void ReadStreamHeader()
		{
			byte[] array = new byte[4];
			byte[] array2 = new byte[4];
			int num2;
			for (;;)
			{
				this.SafeReadBytes(array, 0, 4);
				this.SafeReadBytes(array2, 0, 4);
				uint num = BitConverter.ToUInt32(array, 0);
				num2 = BitConverter.ToInt32(array2, 0);
				if (num == 2U)
				{
					break;
				}
				this.dataStream.Seek((long)num2, SeekOrigin.Current);
				if (this.dataStream.Position > this.dataStream.Length)
				{
					goto Block_2;
				}
			}
			this.currentStreamBufferEnd = (int)this.dataStream.Position + num2;
			this.CheckStreamBufferEnd();
			return;
			Block_2:
			throw new ExportException(ExportErrorType.MessageDataCorrupted, "Reading message stream header. The message stream end reached unexpectedly.");
		}

		private void CheckStreamBufferEnd()
		{
			if (this.currentStreamBufferEnd < 0 || this.currentStreamBufferEnd > this.data.Length)
			{
				throw new ExportException(ExportErrorType.MessageDataCorrupted, string.Format(CultureInfo.CurrentCulture, "The FX stream ops code 0x00000002 indicated wrong end position of the buffer. It is {0}. It should be less than the stream length {1}", new object[]
				{
					this.currentStreamBufferEnd,
					this.data.Length
				}));
			}
		}

		private const uint FastTransferBufferOpCode = 2U;

		private const string CategoriesPropertyName = "Keywords";

		private static readonly Guid PublicStringsPropertySet = new Guid("00020329-0000-0000-C000-000000000046");

		private readonly byte[] data;

		private readonly ExtractContext extractContext;

		private readonly bool removeMetadata;

		private readonly FastTransferStreamExtractor.FixedSizePropertyExtractor fixedSizePropertyExtractor;

		private readonly FastTransferStreamExtractor.VariableSizePropertyExtractor variableSizePropertyExtractor;

		private readonly FastTransferStreamExtractor.MultivaluedFixedSizePropertyExtractor multivaluedFixedSizePropertyExtractor;

		private readonly FastTransferStreamExtractor.MultivaluedVariableSizePropertyExtractor multivaluedVariableSizePropertyExtractor;

		private MemoryStream dataStream;

		private int currentStreamBufferEnd;

		private abstract class PropertyExtractor
		{
			public FastTransferStreamExtractor StreamExtractor { get; set; }

			public IPropertyBag PropertyBag { get; set; }

			public PropertyTag PropertyTag { get; set; }

			public Func<object, object> PropertyValueProcessor { get; set; }

			public abstract void Extract();

			protected static byte[] TrimStringEnd(PropertyTag.PropertyType propType, byte[] data)
			{
				int num = 0;
				switch (propType)
				{
				case PropertyTag.PropertyType.AnsiString:
					goto IL_43;
				case PropertyTag.PropertyType.String:
					break;
				default:
					if (propType != PropertyTag.PropertyType.Unicode)
					{
						if (propType != PropertyTag.PropertyType.Ascii)
						{
							goto IL_54;
						}
						goto IL_43;
					}
					break;
				}
				if (data.Length >= 2 && data[data.Length - 1] == 0 && data[data.Length - 2] == 0)
				{
					num = 2;
					goto IL_54;
				}
				goto IL_54;
				IL_43:
				if (data.Length >= 1 && data[data.Length - 1] == 0)
				{
					num = 1;
				}
				IL_54:
				if (num > 0)
				{
					byte[] array = new byte[data.Length - num];
					Array.Copy(data, array, array.Length);
					data = array;
				}
				return data;
			}
		}

		private class FixedSizePropertyExtractor : FastTransferStreamExtractor.PropertyExtractor
		{
			public override void Extract()
			{
				int sizeOfFixedSizeProperty = PropertyTag.GetSizeOfFixedSizeProperty(base.PropertyTag.Type);
				if (sizeOfFixedSizeProperty > 0)
				{
					byte[] array = base.StreamExtractor.ReadBytes(sizeOfFixedSizeProperty);
					if (base.PropertyTag.Type == PropertyTag.PropertyType.Boolean)
					{
						array = new byte[]
						{
							array[0]
						};
					}
					if (base.PropertyValueProcessor != null)
					{
						array = (byte[])base.PropertyValueProcessor(array);
					}
					if (array != null)
					{
						IProperty property = base.PropertyBag.AddProperty(base.PropertyTag.Value);
						IPropertyWriter propertyWriter = property.OpenStreamWriter();
						propertyWriter.Write(array);
						propertyWriter.Close();
					}
				}
			}
		}

		private class VariableSizePropertyExtractor : FastTransferStreamExtractor.PropertyExtractor
		{
			public override void Extract()
			{
				byte[] value = base.StreamExtractor.ReadBytes(4);
				int size = BitConverter.ToInt32(value, 0);
				byte[] array = base.StreamExtractor.ReadBytes(size);
				if (base.PropertyValueProcessor != null)
				{
					array = (byte[])base.PropertyValueProcessor(array);
				}
				if (array != null)
				{
					IProperty property = base.PropertyBag.AddProperty(base.PropertyTag.NormalizedValueForPst);
					IPropertyWriter propertyWriter = property.OpenStreamWriter();
					propertyWriter.Write(FastTransferStreamExtractor.PropertyExtractor.TrimStringEnd(base.PropertyTag.Type, array));
					propertyWriter.Close();
				}
			}
		}

		private class MultivaluedFixedSizePropertyExtractor : FastTransferStreamExtractor.PropertyExtractor
		{
			public override void Extract()
			{
				byte[] value = base.StreamExtractor.ReadBytes(4);
				int num = BitConverter.ToInt32(value, 0);
				List<List<byte>> list = new List<List<byte>>(num);
				int sizeOfFixedSizeProperty = PropertyTag.GetSizeOfFixedSizeProperty(base.PropertyTag.Type);
				for (int i = 0; i < num; i++)
				{
					byte[] collection = base.StreamExtractor.ReadBytes(sizeOfFixedSizeProperty);
					List<byte> item = new List<byte>(collection);
					list.Add(item);
				}
				if (base.PropertyValueProcessor != null)
				{
					list = (List<List<byte>>)base.PropertyValueProcessor(list);
				}
				if (list != null)
				{
					IProperty property = base.PropertyBag.AddProperty(base.PropertyTag.NormalizedValueForPst);
					property.WriteMultiValueData(list);
				}
			}
		}

		private class MultivaluedVariableSizePropertyExtractor : FastTransferStreamExtractor.PropertyExtractor
		{
			public override void Extract()
			{
				byte[] value = base.StreamExtractor.ReadBytes(4);
				int num = BitConverter.ToInt32(value, 0);
				List<List<byte>> list = new List<List<byte>>(num);
				for (int i = 0; i < num; i++)
				{
					byte[] value2 = base.StreamExtractor.ReadBytes(4);
					int size = BitConverter.ToInt32(value2, 0);
					byte[] data = base.StreamExtractor.ReadBytes(size);
					List<byte> item = new List<byte>(FastTransferStreamExtractor.PropertyExtractor.TrimStringEnd(base.PropertyTag.Type & ~PropertyTag.PropertyType.Multivalued, data));
					list.Add(item);
				}
				if (base.PropertyValueProcessor != null)
				{
					list = (List<List<byte>>)base.PropertyValueProcessor(list);
				}
				if (list != null)
				{
					IProperty property = base.PropertyBag.AddProperty(base.PropertyTag.NormalizedValueForPst);
					property.WriteMultiValueData(list);
				}
			}
		}
	}
}
