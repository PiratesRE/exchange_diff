using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PST;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class PSTPropertyBag : IPropertyBag
	{
		public PSTPropertyBag(PstMailbox pstMailbox, IPropertyBag iPstPropertyBag)
		{
			this.session = new PSTSession(pstMailbox);
			this.iPstPropertyBag = iPstPropertyBag;
			this.values = new Dictionary<PropertyTag, object>();
		}

		public int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		public Encoding CachedEncoding
		{
			get
			{
				if (this.cachedEncoding == null)
				{
					PstMailbox pstMailbox = ((PSTSession)this.Session).PstMailbox;
					Encoding encoding;
					if ((encoding = pstMailbox.ContentEncoding) == null && (encoding = this.TryGetEncoding(PstMailbox.InternetCPID)) == null && (encoding = pstMailbox.CachedEncoding) == null)
					{
						encoding = (this.TryGetEncoding(PstMailbox.MessageCodePage) ?? Encoding.Default);
					}
					this.cachedEncoding = encoding;
					if (this.cachedEncoding.CodePage != this.cachedEncoding.WindowsCodePage)
					{
						this.cachedEncoding = Encoding.GetEncoding(this.cachedEncoding.WindowsCodePage);
					}
				}
				return this.cachedEncoding;
			}
		}

		public AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			NamedProperty namedProperty = null;
			if (propertyTag.PropertyId == PropertyTag.Html.PropertyId)
			{
				propertyTag = PropertyTag.Html;
			}
			PropertyValue propertyValue = this.GetProperty(propertyTag);
			if (propertyTag.IsNamedProperty)
			{
				this.session.TryResolveToNamedProperty(propertyTag, out namedProperty);
				if (namedProperty == null)
				{
					propertyValue = PropertyValue.Error(propertyTag.PropertyId, (ErrorCode)2147746063U);
				}
			}
			return new AnnotatedPropertyValue(propertyTag, propertyValue, namedProperty);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			foreach (KeyValuePair<ushort, IProperty> kvp in this.iPstPropertyBag.Properties)
			{
				KeyValuePair<ushort, IProperty> keyValuePair = kvp;
				if (keyValuePair.Key == (ushort)PropertyTag.DisplayName.PropertyId)
				{
					yield return this.GetAnnotatedProperty(PropertyTag.DisplayName);
				}
				else
				{
					KeyValuePair<ushort, IProperty> keyValuePair2 = kvp;
					yield return this.GetAnnotatedProperty(new PropertyTag(keyValuePair2.Value.PropTag));
				}
			}
			yield break;
		}

		public void Delete(PropertyTag property)
		{
			this.values.Remove(property);
		}

		public Stream GetPropertyStream(PropertyTag propertyTag)
		{
			IProperty pstProperty = null;
			if (!this.iPstPropertyBag.Properties.TryGetValue((ushort)propertyTag.PropertyId, out pstProperty))
			{
				return null;
			}
			return new PSTPropertyStream(true, pstProperty);
		}

		public virtual Stream SetPropertyStream(PropertyTag propertyTag, long dataSizeEstimate)
		{
			IProperty property = null;
			if (!this.iPstPropertyBag.Properties.TryGetValue((ushort)propertyTag.PropertyId, out property))
			{
				property = this.iPstPropertyBag.AddProperty(propertyTag);
			}
			if (property == null)
			{
				throw new NullReferenceException("Cannot SetPropertyStream on a null pstProperty");
			}
			return new PSTPropertyStream(false, property);
		}

		public PropertyValue GetProperty(PropertyTag propertyTag)
		{
			PropertyValue result = PropertyValue.Error(propertyTag.PropertyId, (ErrorCode)2147746063U);
			object value;
			if (this.values.TryGetValue(propertyTag, out value))
			{
				return new PropertyValue(propertyTag, value);
			}
			IProperty property = null;
			if (!this.iPstPropertyBag.Properties.TryGetValue((ushort)propertyTag.PropertyId, out property))
			{
				return result;
			}
			try
			{
				if (propertyTag.IsMultiValuedProperty)
				{
					List<List<byte>> list = property.ReadMultiValueData();
					if (list == null || list.Count == 0)
					{
						return result;
					}
					PropertyType propertyType = propertyTag.PropertyType;
					if (propertyType <= PropertyType.MultiValueUnicode)
					{
						switch (propertyType)
						{
						case PropertyType.MultiValueInt16:
						{
							short[] array = new short[list.Count];
							for (int i = 0; i < list.Count; i++)
							{
								array[i] = BitConverter.ToInt16(list[i].ToArray(), 0);
							}
							result = new PropertyValue(propertyTag, array);
							goto IL_3CF;
						}
						case PropertyType.MultiValueInt32:
						{
							int[] array2 = new int[list.Count];
							for (int j = 0; j < list.Count; j++)
							{
								array2[j] = BitConverter.ToInt32(list[j].ToArray(), 0);
							}
							result = new PropertyValue(propertyTag, array2);
							goto IL_3CF;
						}
						case PropertyType.MultiValueFloat:
						{
							float[] array3 = new float[list.Count];
							for (int k = 0; k < list.Count; k++)
							{
								array3[k] = BitConverter.ToSingle(list[k].ToArray(), 0);
							}
							result = new PropertyValue(propertyTag, array3);
							goto IL_3CF;
						}
						case PropertyType.MultiValueDouble:
						case PropertyType.MultiValueAppTime:
						{
							double[] array4 = new double[list.Count];
							for (int l = 0; l < list.Count; l++)
							{
								array4[l] = BitConverter.ToDouble(list[l].ToArray(), 0);
							}
							result = new PropertyValue(propertyTag, array4);
							goto IL_3CF;
						}
						case PropertyType.MultiValueCurrency:
							break;
						default:
							if (propertyType != PropertyType.MultiValueInt64)
							{
								switch (propertyType)
								{
								case PropertyType.MultiValueString8:
								case PropertyType.MultiValueUnicode:
								{
									string[] array5 = new string[list.Count];
									for (int m = 0; m < list.Count; m++)
									{
										if (propertyTag.PropertyType == PropertyType.MultiValueUnicode)
										{
											array5[m] = Encoding.Unicode.GetString(list[m].ToArray());
										}
										else
										{
											array5[m] = Encoding.ASCII.GetString(list[m].ToArray());
										}
									}
									result = new PropertyValue(propertyTag, array5);
									goto IL_3CF;
								}
								default:
									goto IL_3CF;
								}
							}
							break;
						}
						long[] array6 = new long[list.Count];
						for (int n = 0; n < list.Count; n++)
						{
							array6[n] = BitConverter.ToInt64(list[n].ToArray(), 0);
						}
						result = new PropertyValue(propertyTag, array6);
					}
					else if (propertyType != PropertyType.MultiValueSysTime)
					{
						if (propertyType != PropertyType.MultiValueGuid)
						{
							if (propertyType == PropertyType.MultiValueBinary)
							{
								byte[][] array7 = new byte[list.Count][];
								for (int num = 0; num < list.Count; num++)
								{
									array7[num] = list[num].ToArray();
								}
								result = new PropertyValue(propertyTag, array7);
							}
						}
						else
						{
							Guid[] array8 = new Guid[list.Count];
							for (int num2 = 0; num2 < list.Count; num2++)
							{
								array8[num2] = new Guid(list[num2].ToArray());
							}
							result = new PropertyValue(propertyTag, array8);
						}
					}
					else
					{
						ExDateTime[] array9 = new ExDateTime[list.Count];
						for (int num3 = 0; num3 < list.Count; num3++)
						{
							DateTime dateTime;
							try
							{
								dateTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(list[num3].ToArray(), 0));
							}
							catch (ArgumentOutOfRangeException)
							{
								dateTime = DateTime.MaxValue;
							}
							array9[num3] = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime);
						}
						result = new PropertyValue(propertyTag, array9);
					}
					IL_3CF:
					return result;
				}
				else
				{
					IPropertyReader propertyReader = property.OpenStreamReader();
					if (propertyReader.Length > 81760 && (propertyTag.PropertyType == PropertyType.Binary || propertyTag.PropertyType == PropertyType.Unicode || propertyTag.PropertyType == PropertyType.String8 || propertyTag.PropertyType == PropertyType.Object))
					{
						propertyReader.Close();
						return PropertyValue.CreateNotEnoughMemory(propertyTag.PropertyId);
					}
					byte[] array10 = propertyReader.Read();
					if (propertyTag.EstimatedValueSize != 0 && array10.Length != propertyTag.EstimatedValueSize)
					{
						if (propertyTag.PropertyType == PropertyType.Bool && array10.Length == 1)
						{
							result = new PropertyValue(propertyTag, BitConverter.ToBoolean(array10, 0));
						}
						else if (propertyTag.PropertyType == PropertyType.Int64 && array10.Length == 4)
						{
							result = new PropertyValue(propertyTag, (long)BitConverter.ToInt32(array10, 0));
						}
						else if (propertyTag.PropertyType == PropertyType.Int32 && array10.Length == 8)
						{
							long num4 = BitConverter.ToInt64(array10, 0);
							if (num4 <= 2147483647L)
							{
								result = new PropertyValue(propertyTag, (int)num4);
							}
						}
						propertyReader.Close();
						return result;
					}
					PropertyType propertyType2 = propertyTag.PropertyType;
					if (propertyType2 <= PropertyType.Int64)
					{
						switch (propertyType2)
						{
						case PropertyType.Int16:
							result = new PropertyValue(propertyTag, BitConverter.ToInt16(array10, 0));
							goto IL_72F;
						case PropertyType.Int32:
							result = new PropertyValue(propertyTag, BitConverter.ToInt32(array10, 0));
							goto IL_72F;
						case PropertyType.Float:
							result = new PropertyValue(propertyTag, BitConverter.ToSingle(array10, 0));
							goto IL_72F;
						case PropertyType.Double:
						case PropertyType.AppTime:
							result = new PropertyValue(propertyTag, BitConverter.ToDouble(array10, 0));
							goto IL_72F;
						case PropertyType.Currency:
							break;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
							goto IL_66E;
						case PropertyType.Bool:
							result = new PropertyValue(propertyTag, BitConverter.ToBoolean(array10, 0));
							goto IL_72F;
						default:
							if (propertyType2 != PropertyType.Int64)
							{
								goto IL_66E;
							}
							break;
						}
						result = new PropertyValue(propertyTag, BitConverter.ToInt64(array10, 0));
						goto IL_72F;
					}
					if (propertyType2 == PropertyType.SysTime)
					{
						DateTime dateTime2;
						try
						{
							dateTime2 = DateTime.FromFileTimeUtc(BitConverter.ToInt64(array10, 0));
						}
						catch (ArgumentOutOfRangeException)
						{
							dateTime2 = DateTime.MaxValue;
						}
						result = new PropertyValue(propertyTag, new ExDateTime(ExTimeZone.UtcTimeZone, dateTime2));
						goto IL_72F;
					}
					if (propertyType2 == PropertyType.Guid)
					{
						result = new PropertyValue(propertyTag, new Guid(array10));
						goto IL_72F;
					}
					IL_66E:
					List<byte> list2 = new List<byte>(array10);
					while (!propertyReader.IsEnd)
					{
						list2.AddRange(propertyReader.Read());
					}
					PropertyType propertyType3 = propertyTag.PropertyType;
					if (propertyType3 <= PropertyType.Unicode)
					{
						if (propertyType3 != PropertyType.Object)
						{
							switch (propertyType3)
							{
							case PropertyType.String8:
							case PropertyType.Unicode:
							{
								bool flag = (property.PropTag & 65535U) == 31U;
								Encoding encoding;
								if (flag)
								{
									encoding = Encoding.Unicode;
								}
								else if (this.CachedEncoding == Encoding.Unicode)
								{
									encoding = Encoding.Default;
								}
								else
								{
									encoding = this.CachedEncoding;
								}
								result = new PropertyValue(propertyTag, encoding.GetString(list2.ToArray()));
								goto IL_72F;
							}
							default:
								goto IL_72F;
							}
						}
					}
					else if (propertyType3 != PropertyType.ServerId && propertyType3 != PropertyType.Binary)
					{
						goto IL_72F;
					}
					result = new PropertyValue(propertyTag, list2.ToArray());
					IL_72F:
					propertyReader.Close();
					if (propertyTag.PropertyId == PropertyTag.Subject.PropertyId && result.Value is string)
					{
						string text = (string)result.Value;
						if (text.Length >= 2 && text[0] == '\u0001')
						{
							result = new PropertyValue(result.PropertyTag, text.Substring(2, text.Length - 2));
						}
					}
				}
			}
			catch (PSTIOException innerException)
			{
				throw new UnableToGetPSTPropsTransientException(((PSTSession)this.session).PstMailbox.IPst.FileName, innerException);
			}
			catch (PSTExceptionBase innerException2)
			{
				throw new UnableToGetPSTPropsPermanentException(((PSTSession)this.session).PstMailbox.IPst.FileName, innerException2);
			}
			return result;
		}

		public virtual void SetProperty(PropertyValue propertyValue)
		{
			if (!this.iPstPropertyBag.IsWritable || PSTPropertyBag.propertiesToSkip.Contains((ushort)propertyValue.PropertyTag.PropertyId))
			{
				this.values[propertyValue.PropertyTag] = propertyValue.Value;
				return;
			}
			IProperty property;
			if (!this.iPstPropertyBag.Properties.TryGetValue((ushort)propertyValue.PropertyTag.PropertyId, out property))
			{
				property = this.iPstPropertyBag.AddProperty(propertyValue.PropertyTag);
			}
			if (property == null)
			{
				throw new NullReferenceException("Cannot SetProperty on a null pstProperty");
			}
			if (property.PropTag != propertyValue.PropertyTag)
			{
				throw new PropertyTagsDoNotMatchPermanentException(property.PropTag, propertyValue.PropertyTag);
			}
			try
			{
				if (propertyValue.PropertyTag.IsMultiValuedProperty)
				{
					List<List<byte>> list = new List<List<byte>>(((Array)propertyValue.Value).Length);
					int i = 0;
					while (i < ((Array)propertyValue.Value).Length)
					{
						PropertyType propertyType = propertyValue.PropertyTag.PropertyType;
						byte[] source;
						if (propertyType <= PropertyType.MultiValueUnicode)
						{
							switch (propertyType)
							{
							case PropertyType.MultiValueInt16:
								source = BitConverter.GetBytes(((short[])propertyValue.Value)[i]);
								goto IL_27D;
							case PropertyType.MultiValueInt32:
								source = BitConverter.GetBytes(((int[])propertyValue.Value)[i]);
								goto IL_27D;
							case PropertyType.MultiValueFloat:
								source = BitConverter.GetBytes(((float[])propertyValue.Value)[i]);
								goto IL_27D;
							case PropertyType.MultiValueDouble:
							case PropertyType.MultiValueAppTime:
								source = BitConverter.GetBytes(((double[])propertyValue.Value)[i]);
								goto IL_27D;
							case PropertyType.MultiValueCurrency:
								break;
							default:
								if (propertyType != PropertyType.MultiValueInt64)
								{
									switch (propertyType)
									{
									case PropertyType.MultiValueString8:
										source = Encoding.ASCII.GetBytes(((string[])propertyValue.Value)[i]);
										goto IL_27D;
									case PropertyType.MultiValueUnicode:
										source = Encoding.Unicode.GetBytes(((string[])propertyValue.Value)[i]);
										goto IL_27D;
									default:
										goto IL_289;
									}
								}
								break;
							}
							source = BitConverter.GetBytes(((long[])propertyValue.Value)[i]);
							goto IL_27D;
						}
						if (propertyType == PropertyType.MultiValueSysTime)
						{
							long value;
							try
							{
								value = ((ExDateTime[])propertyValue.Value)[i].ToFileTimeUtc();
							}
							catch (ArgumentOutOfRangeException)
							{
								value = 0L;
							}
							source = BitConverter.GetBytes(value);
							goto IL_27D;
						}
						if (propertyType == PropertyType.MultiValueGuid)
						{
							source = ((Guid[])propertyValue.Value)[i].ToByteArray();
							goto IL_27D;
						}
						if (propertyType == PropertyType.MultiValueBinary)
						{
							source = ((byte[][])propertyValue.Value)[i];
							goto IL_27D;
						}
						IL_289:
						i++;
						continue;
						IL_27D:
						list.Add(source.ToList<byte>());
						goto IL_289;
					}
					property.WriteMultiValueData(list);
				}
				else
				{
					IPropertyWriter propertyWriter = property.OpenStreamWriter();
					PropertyType propertyType2 = propertyValue.PropertyTag.PropertyType;
					if (propertyType2 <= PropertyType.Unicode)
					{
						switch (propertyType2)
						{
						case PropertyType.Int16:
							propertyWriter.Write(BitConverter.GetBytes((short)propertyValue.Value));
							goto IL_4B4;
						case PropertyType.Int32:
							propertyWriter.Write(BitConverter.GetBytes((int)propertyValue.Value));
							goto IL_4B4;
						case PropertyType.Float:
							propertyWriter.Write(BitConverter.GetBytes((float)propertyValue.Value));
							goto IL_4B4;
						case PropertyType.Double:
						case PropertyType.AppTime:
							propertyWriter.Write(BitConverter.GetBytes((double)propertyValue.Value));
							goto IL_4B4;
						case PropertyType.Currency:
							break;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							goto IL_4B2;
						case PropertyType.Bool:
							propertyWriter.Write(BitConverter.GetBytes((bool)propertyValue.Value));
							goto IL_4B4;
						case PropertyType.Object:
							goto IL_49D;
						default:
							if (propertyType2 != PropertyType.Int64)
							{
								switch (propertyType2)
								{
								case PropertyType.String8:
									propertyWriter.Write(Encoding.ASCII.GetBytes((string)propertyValue.Value));
									goto IL_4B4;
								case PropertyType.Unicode:
									propertyWriter.Write(Encoding.Unicode.GetBytes((string)propertyValue.Value));
									goto IL_4B4;
								default:
									goto IL_4B2;
								}
							}
							break;
						}
						propertyWriter.Write(BitConverter.GetBytes((long)propertyValue.Value));
						goto IL_4B4;
					}
					if (propertyType2 <= PropertyType.Guid)
					{
						if (propertyType2 == PropertyType.SysTime)
						{
							long value2;
							try
							{
								value2 = ((ExDateTime)propertyValue.Value).ToFileTimeUtc();
							}
							catch (ArgumentOutOfRangeException)
							{
								value2 = 0L;
							}
							propertyWriter.Write(BitConverter.GetBytes(value2));
							goto IL_4B4;
						}
						if (propertyType2 != PropertyType.Guid)
						{
							goto IL_4B2;
						}
						propertyWriter.Write(((Guid)propertyValue.Value).ToByteArray());
						goto IL_4B4;
					}
					else if (propertyType2 != PropertyType.ServerId && propertyType2 != PropertyType.Binary)
					{
						goto IL_4B2;
					}
					IL_49D:
					propertyWriter.Write((byte[])propertyValue.Value);
					goto IL_4B4;
					IL_4B2:
					return;
					IL_4B4:
					propertyWriter.Close();
				}
			}
			catch (PSTExceptionBase innerException)
			{
				throw new MailboxReplicationPermanentException(new LocalizedString(((PSTSession)this.session).PstMailbox.IPst.FileName), innerException);
			}
		}

		public IEnumerator<KeyValuePair<PropertyTag, object>> GetEnumerator()
		{
			return this.values.GetEnumerator();
		}

		private static SortedList<PropertyTag, object> FilteredValues(PSTPropertyBag propertyBag)
		{
			SortedList<PropertyTag, object> sortedList = new SortedList<PropertyTag, object>();
			foreach (PropertyTag key in propertyBag.values.Keys)
			{
				if (key.PropertyType == PropertyType.Error)
				{
					ErrorCode errorCode = (ErrorCode)propertyBag.values[key];
					if (errorCode == (ErrorCode)2147746063U || errorCode == (ErrorCode)2147942405U)
					{
						continue;
					}
				}
				sortedList.Add(key, propertyBag.values[key]);
			}
			return sortedList;
		}

		private Encoding TryGetEncoding(PropertyTag propertyTag)
		{
			Encoding result = null;
			PropertyValue property = this.GetProperty(propertyTag);
			if (!property.IsError && !property.IsNotFound)
			{
				try
				{
					result = Encoding.GetEncoding((int)property.Value);
				}
				catch (NotSupportedException)
				{
					MrsTracer.Provider.Warning("Ignoring unsupported code page '{0}'", new object[]
					{
						(int)property.Value
					});
				}
				catch (ArgumentException)
				{
					MrsTracer.Provider.Warning("Ignoring invalid code page '{0}'", new object[]
					{
						(int)property.Value
					});
				}
			}
			return result;
		}

		private const int StreamableLength = 81760;

		private const ushort LtpRowId = 26610;

		private const ushort LtpRowVer = 26611;

		private static HashSet<ushort> propertiesToSkip = new HashSet<ushort>(new ushort[]
		{
			(ushort)PropTag.AssocContentCount.Id(),
			(ushort)PropTag.ContentCount.Id(),
			(ushort)PropTag.ContentUnread.Id(),
			(ushort)PropTag.DeletedMsgCount.Id(),
			(ushort)PropTag.DeletedAssocMsgCount.Id(),
			(ushort)PropertyTag.DeletedFolderCt.PropertyId,
			(ushort)PropTag.DeletedCountTotal.Id(),
			(ushort)PropTag.SubFolders.Id(),
			(ushort)PropTag.MessageSize.Id(),
			26610,
			26611
		});

		private IDictionary<PropertyTag, object> values;

		private IPropertyBag iPstPropertyBag;

		private ISession session;

		private Encoding cachedEncoding;
	}
}
