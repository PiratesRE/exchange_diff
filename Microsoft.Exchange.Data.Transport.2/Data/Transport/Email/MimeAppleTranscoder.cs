using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal static class MimeAppleTranscoder
	{
		public static void WriteWholeApplefile(Stream dataForkStream, Stream outStream)
		{
			MimeAppleTranscoder.WriteApplefileHeader(new List<MimeAppleTranscoder.EntryDescriptor>(1)
			{
				new MimeAppleTranscoder.EntryDescriptor(1, 0, (int)dataForkStream.Length)
			}, false, outStream);
			MimeAppleTranscoder.CopyStreamData(dataForkStream, outStream);
		}

		public static void WriteWholeApplefile(Stream applefileStream, Stream dataForkStream, Stream outStream)
		{
			int num = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
			if (num != 333319)
			{
				throw new MimeException(EmailMessageStrings.WrongAppleMagicNumber);
			}
			List<MimeAppleTranscoder.EntryDescriptor> list = MimeAppleTranscoder.ReadAppleFileHeaderEntries(applefileStream);
			int entryOffset = 0;
			foreach (MimeAppleTranscoder.EntryDescriptor entryDescriptor in list)
			{
				if (entryDescriptor.EntryId == 1)
				{
					throw new MimeException(EmailMessageStrings.TooManyEntriesInApplefile);
				}
				entryOffset = entryDescriptor.EntryOffset + entryDescriptor.EntryLength;
			}
			list.Add(new MimeAppleTranscoder.EntryDescriptor(1, entryOffset, (int)dataForkStream.Length));
			MimeAppleTranscoder.WriteApplefileHeader(list, false, outStream);
			MimeAppleTranscoder.CopyStreamData(applefileStream, outStream);
			MimeAppleTranscoder.CopyStreamData(dataForkStream, outStream);
		}

		public static Stream ExtractDataFork(Stream macBinStream)
		{
			MacBinaryHeader macBinaryHeader = MimeAppleTranscoder.ExtractMacBinHeader(macBinStream);
			int num = 128;
			if (macBinaryHeader.SecondaryHeaderLength > 0)
			{
				num += macBinaryHeader.SecondaryHeaderLength + MimeAppleTranscoder.GetMacBinPaddingSize(macBinaryHeader.SecondaryHeaderLength);
			}
			return new BoundedStream(macBinStream, (long)num, macBinaryHeader.DataForkLength);
		}

		public static bool IsMacBinStream(Stream stream)
		{
			MacBinaryHeader macBinaryHeader = null;
			try
			{
				macBinaryHeader = MimeAppleTranscoder.ExtractMacBinHeader(stream);
			}
			catch (MimeException)
			{
				stream.Position = 0L;
				return false;
			}
			long num = 128L;
			if (macBinaryHeader.SecondaryHeaderLength > 0)
			{
				num += (long)(macBinaryHeader.SecondaryHeaderLength + MimeAppleTranscoder.GetMacBinPaddingSize(macBinaryHeader.SecondaryHeaderLength));
			}
			if (macBinaryHeader.DataForkLength > 0L)
			{
				num += macBinaryHeader.DataForkLength + (long)MimeAppleTranscoder.GetMacBinPaddingSize((int)macBinaryHeader.DataForkLength);
			}
			if (macBinaryHeader.ResourceForkLength > 0L)
			{
				num += macBinaryHeader.ResourceForkLength + (long)MimeAppleTranscoder.GetMacBinPaddingSize((int)macBinaryHeader.ResourceForkLength);
			}
			if (macBinaryHeader.GetInfoLength > 0)
			{
				num += (long)macBinaryHeader.GetInfoLength;
			}
			stream.Position = 0L;
			return stream.Length == num;
		}

		private static void AddEntry(List<MimeAppleTranscoder.EntryDescriptor> descriptors, MimeAppleTranscoder.EntryDescriptor descr, ref int virtualOffset)
		{
			descriptors.Add(descr);
			virtualOffset += descr.EntryLength;
		}

		public static void MacBinToApplefile(Stream macBinStream, Stream outStream, out string fileName, out byte[] additionalInfo)
		{
			MacBinaryHeader macBinaryHeader = MimeAppleTranscoder.ExtractMacBinHeader(macBinStream);
			fileName = macBinaryHeader.FileName;
			if (outStream == null)
			{
				additionalInfo = null;
				return;
			}
			additionalInfo = MimeAppleTranscoder.EncodeAdditionalInfo(macBinaryHeader.FileCreator, macBinaryHeader.FileType);
			List<MimeAppleTranscoder.EntryDescriptor> descriptors = new List<MimeAppleTranscoder.EntryDescriptor>(6);
			int entryOffset = 0;
			byte[] array = null;
			if (macBinaryHeader.FileName != null)
			{
				MimeAppleTranscoder.EntryDescriptor descr = new MimeAppleTranscoder.EntryDescriptor(3, entryOffset, macBinaryHeader.FileNameLength);
				try
				{
					array = MimeAppleTranscoder.macEncoding.GetBytes(fileName);
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new MimeException(EmailMessageStrings.MacBinWrongFilename, innerException);
				}
				MimeAppleTranscoder.AddEntry(descriptors, descr, ref entryOffset);
			}
			MimeAppleTranscoder.EntryDescriptor entryDescriptor = new MimeAppleTranscoder.EntryDescriptor(8, entryOffset, 16);
			MimeAppleTranscoder.AddEntry(descriptors, entryDescriptor, ref entryOffset);
			int appleDate = MimeAppleTranscoder.GetAppleDate(macBinaryHeader.CreationDate);
			int appleDate2 = MimeAppleTranscoder.GetAppleDate(macBinaryHeader.ModificationDate);
			byte[] array2 = new byte[entryDescriptor.EntryLength];
			int num = 0;
			MimeAppleTranscoder.WriteIntData(appleDate, array2, ref num);
			MimeAppleTranscoder.WriteIntData(appleDate2, array2, ref num);
			MimeAppleTranscoder.WriteIntData(0, array2, ref num);
			MimeAppleTranscoder.WriteIntData(0, array2, ref num);
			MimeAppleTranscoder.EntryDescriptor entryDescriptor2 = new MimeAppleTranscoder.EntryDescriptor(9, entryOffset, 28);
			MimeAppleTranscoder.AddEntry(descriptors, entryDescriptor2, ref entryOffset);
			byte[] array3 = new byte[entryDescriptor2.EntryLength];
			num = 0;
			MimeAppleTranscoder.WriteIntData(macBinaryHeader.FileType, array3, ref num);
			MimeAppleTranscoder.WriteIntData(macBinaryHeader.FileCreator, array3, ref num);
			MimeAppleTranscoder.WriteShortData((int)((short)macBinaryHeader.FinderFlags), array3, ref num);
			MimeAppleTranscoder.WriteShortData((int)((short)macBinaryHeader.YIcon), array3, ref num);
			MimeAppleTranscoder.WriteShortData((int)((short)macBinaryHeader.XIcon), array3, ref num);
			MimeAppleTranscoder.WriteShortData((int)((short)macBinaryHeader.FileId), array3, ref num);
			MimeAppleTranscoder.WriteIntData(0, array3, ref num);
			MimeAppleTranscoder.WriteIntData(0, array3, ref num);
			MimeAppleTranscoder.WriteIntData(0, array3, ref num);
			MimeAppleTranscoder.EntryDescriptor entryDescriptor3 = new MimeAppleTranscoder.EntryDescriptor(10, entryOffset, 4);
			MimeAppleTranscoder.AddEntry(descriptors, entryDescriptor3, ref entryOffset);
			byte[] array4 = new byte[entryDescriptor3.EntryLength];
			array4[0] = 0;
			array4[1] = 0;
			array4[2] = 0;
			array4[3] = (macBinaryHeader.Protected ? 1 : 0);
			if (macBinaryHeader.ResourceForkLength > 0L)
			{
				MimeAppleTranscoder.EntryDescriptor descr2 = new MimeAppleTranscoder.EntryDescriptor(2, entryOffset, (int)macBinaryHeader.ResourceForkLength);
				MimeAppleTranscoder.AddEntry(descriptors, descr2, ref entryOffset);
			}
			if (macBinaryHeader.GetInfoLength > 0)
			{
				MimeAppleTranscoder.EntryDescriptor descr3 = new MimeAppleTranscoder.EntryDescriptor(4, entryOffset, macBinaryHeader.GetInfoLength);
				MimeAppleTranscoder.AddEntry(descriptors, descr3, ref entryOffset);
			}
			MimeAppleTranscoder.WriteApplefileHeader(descriptors, true, outStream);
			if (array != null)
			{
				outStream.Write(array, 0, array.Length);
			}
			outStream.Write(array2, 0, array2.Length);
			outStream.Write(array3, 0, array3.Length);
			outStream.Write(array4, 0, array4.Length);
			if (macBinaryHeader.SecondaryHeaderLength > 0)
			{
				macBinStream.Position += (long)(macBinaryHeader.SecondaryHeaderLength + MimeAppleTranscoder.GetMacBinPaddingSize(macBinaryHeader.SecondaryHeaderLength));
			}
			if (macBinaryHeader.DataForkLength > 0L)
			{
				macBinStream.Position += macBinaryHeader.DataForkLength + (long)MimeAppleTranscoder.GetMacBinPaddingSize((int)macBinaryHeader.DataForkLength);
			}
			if (macBinaryHeader.ResourceForkLength > 0L)
			{
				MimeAppleTranscoder.CopyStreamData(macBinStream, outStream, new int?((int)macBinaryHeader.ResourceForkLength));
				macBinStream.Position += (long)MimeAppleTranscoder.GetMacBinPaddingSize((int)macBinaryHeader.ResourceForkLength);
			}
			if (macBinaryHeader.GetInfoLength > 0)
			{
				MimeAppleTranscoder.CopyStreamData(macBinStream, outStream, new int?(macBinaryHeader.GetInfoLength));
				macBinStream.Position += (long)MimeAppleTranscoder.GetMacBinPaddingSize(macBinaryHeader.GetInfoLength);
			}
		}

		public static void ApplesingleToMacBin(Stream applesingleStream, Stream outAttachMacInfo, Stream outMacBinStream, out string fileName, out byte[] additionalInfo)
		{
			Stream stream = null;
			MimeAppleTranscoder.Transcode(applesingleStream, null, outAttachMacInfo, outMacBinStream, ref stream, out fileName, out additionalInfo);
		}

		public static void AppledoubleToMacBin(Stream resourceForkStream, Stream dataForkStream, Stream outMacBinStream, out string fileName, out byte[] additionalInfo)
		{
			Stream stream = null;
			MimeAppleTranscoder.Transcode(resourceForkStream, dataForkStream, null, outMacBinStream, ref stream, out fileName, out additionalInfo);
		}

		public static string GetFileNameFromResourceFork(Stream resourceForkStream)
		{
			Stream stream = null;
			string result;
			byte[] array;
			MimeAppleTranscoder.Transcode(resourceForkStream, null, null, null, ref stream, out result, out array);
			return result;
		}

		public static void GetDataForkFromAppleSingle(Stream appleSingle, out Stream dataFork)
		{
			Stream @null = Stream.Null;
			string text;
			byte[] array;
			MimeAppleTranscoder.Transcode(appleSingle, null, null, null, ref @null, out text, out array);
			dataFork = @null;
		}

		private static int ReadUIntFromStream(Stream dataStream)
		{
			byte[] array = new byte[4];
			MimeAppleTranscoder.ReadFixed(dataStream, array, 0, 4);
			return (int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3];
		}

		private static int ReadUShortFromStream(Stream dataStream)
		{
			byte[] array = new byte[2];
			MimeAppleTranscoder.ReadFixed(dataStream, array, 0, 2);
			return (int)array[0] << 8 | (int)array[1];
		}

		private static void WriteUIntData(int data, byte[] dest, ref int offset)
		{
			dest[offset] = (byte)((uint)(data & -16777216) >> 24);
			dest[offset + 1] = (byte)((uint)(data & 16711680) >> 16);
			dest[offset + 2] = (byte)((uint)(data & 65280) >> 8);
			dest[offset + 3] = (byte)(data & 255);
			offset += 4;
		}

		private static void WriteIntData(int data, byte[] dest, ref int offset)
		{
			dest[offset] = (byte)(((long)data & (long)((ulong)-16777216)) >> 24);
			dest[offset + 1] = (byte)((data & 16711680) >> 16);
			dest[offset + 2] = (byte)((data & 65280) >> 8);
			dest[offset + 3] = (byte)(data & 255);
			offset += 4;
		}

		private static void WriteUShortData(int data, byte[] dest, ref int offset)
		{
			dest[offset] = (byte)((data & 65280) >> 8);
			dest[offset + 1] = (byte)(data & 255);
			offset += 2;
		}

		private static void WriteShortData(int data, byte[] dest, ref int offset)
		{
			short num = (short)data;
			dest[offset] = (byte)(((int)num & 65280) >> 8);
			dest[offset + 1] = (byte)(num & 255);
			offset += 2;
		}

		private static int GetAppleDate(DateTime dt)
		{
			return dt.ToUniversalTime().Subtract(MimeAppleTranscoder.zeroDate).Seconds;
		}

		private static MacBinaryHeader ExtractMacBinHeader(Stream macBinStream)
		{
			byte[] array = new byte[128];
			MimeAppleTranscoder.ReadFixed(macBinStream, array, 0, 128);
			MacBinaryHeader result;
			try
			{
				result = new MacBinaryHeader(array);
			}
			catch (ArgumentException innerException)
			{
				throw new MimeException(EmailMessageStrings.WrongMacBinHeader, innerException);
			}
			catch (ByteEncoderException innerException2)
			{
				throw new MimeException(EmailMessageStrings.WrongMacBinHeader, innerException2);
			}
			return result;
		}

		private static void EncodeAdditionalInfoByte(byte data, byte[] result, ref int offset)
		{
			result[offset] = MimeAppleTranscoder.slashBytes[0];
			offset++;
			if (data == MimeAppleTranscoder.semiColonBytes[0] || data == MimeAppleTranscoder.slashBytes[0] || data == MimeAppleTranscoder.colonBytes[0])
			{
				result[offset] = data;
				offset++;
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				byte b = (byte)(data >> 3 * (2 - i) & 7);
				result[offset] = b + 48;
				offset++;
			}
		}

		private static void EncodeAdditionalInfoInt(int data, byte[] result, ref int offset)
		{
			result[offset] = MimeAppleTranscoder.colonBytes[0];
			offset++;
			for (int i = 0; i < 4; i++)
			{
				byte b = (byte)(data >> 8 * (3 - i) & 255);
				if (b == MimeAppleTranscoder.semiColonBytes[0] || b == MimeAppleTranscoder.slashBytes[0] || b == MimeAppleTranscoder.colonBytes[0] || b == 127 || b < 32 || b >= 252)
				{
					MimeAppleTranscoder.EncodeAdditionalInfoByte(b, result, ref offset);
				}
				else
				{
					result[offset] = b;
					offset++;
				}
			}
		}

		private static byte[] EncodeAdditionalInfo(int creator, int type)
		{
			byte[] array = new byte[49];
			int num = 0;
			MimeAppleTranscoder.EncodeAdditionalInfoInt(creator, array, ref num);
			MimeAppleTranscoder.EncodeAdditionalInfoInt(type, array, ref num);
			array[num] = 0;
			num++;
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}

		private static ushort CalcCRC16(byte[] buff, int size)
		{
			ushort num = 0;
			for (int i = 0; i < size; i++)
			{
				byte b = buff[i];
				for (int j = 0; j < 8; j++)
				{
					ushort num2 = num & 32768;
					num = (ushort)((int)num << 1 | b >> 7);
					if (num2 != 0)
					{
						num ^= 4129;
					}
					b = (byte)(b << 1);
				}
			}
			return num;
		}

		private static int GetMacBinPaddingSize(int lastBlobSize)
		{
			int num = 128 - lastBlobSize % 128;
			if (num == 128)
			{
				return 0;
			}
			return num;
		}

		private static void WriteMacBinPadding(Stream outStream, int lastBlobSize)
		{
			int macBinPaddingSize = MimeAppleTranscoder.GetMacBinPaddingSize(lastBlobSize);
			if (macBinPaddingSize != 0)
			{
				outStream.Write(MimeAppleTranscoder.padBytes, 0, macBinPaddingSize);
			}
		}

		private static void CopyDataWithPadding(Stream inputStream, Stream outputStream, int inputOffset, int inputLength)
		{
			if (inputStream.Length <= (long)inputOffset)
			{
				throw new MimeException(EmailMessageStrings.ArgumentInvalidOffLen);
			}
			inputStream.Position = (long)inputOffset;
			MimeAppleTranscoder.CopyStreamData(inputStream, outputStream, new int?(inputLength));
			MimeAppleTranscoder.WriteMacBinPadding(outputStream, inputLength);
		}

		private static void ReadFixed(Stream stream, byte[] buffer, int offset, int length)
		{
			while (length > 0)
			{
				int num = stream.Read(buffer, offset, length);
				if (num == 0)
				{
					throw new MimeException(EmailMessageStrings.UnexpectedEndOfStream);
				}
				length -= num;
				offset += num;
			}
		}

		private static void CopyStreamData(Stream readStream, Stream writeStream)
		{
			MimeAppleTranscoder.CopyStreamData(readStream, writeStream, null);
		}

		private static void CopyStreamData(Stream readStream, Stream writeStream, int? numBytes)
		{
			byte[] array = new byte[71680];
			while (numBytes == null || numBytes > 0)
			{
				int num = array.Length;
				if (numBytes != null && numBytes < num)
				{
					num = numBytes.Value;
				}
				int num2 = readStream.Read(array, 0, num);
				if (num2 == 0)
				{
					return;
				}
				writeStream.Write(array, 0, num2);
				if (numBytes != null)
				{
					numBytes -= num2;
				}
			}
		}

		private static void WriteApplefileHeader(List<MimeAppleTranscoder.EntryDescriptor> descriptors, bool isAppleDouble, Stream outStream)
		{
			int num = 26 + 12 * descriptors.Count;
			byte[] array = new byte[num];
			int num2 = 0;
			MimeAppleTranscoder.WriteUIntData(isAppleDouble ? 333319 : 333312, array, ref num2);
			MimeAppleTranscoder.WriteUIntData(131072, array, ref num2);
			num2 += 16;
			MimeAppleTranscoder.WriteUShortData(descriptors.Count, array, ref num2);
			int num3 = num;
			foreach (MimeAppleTranscoder.EntryDescriptor entryDescriptor in descriptors)
			{
				MimeAppleTranscoder.WriteUIntData(entryDescriptor.EntryId, array, ref num2);
				MimeAppleTranscoder.WriteUIntData(num3, array, ref num2);
				MimeAppleTranscoder.WriteUIntData(entryDescriptor.EntryLength, array, ref num2);
				num3 += entryDescriptor.EntryLength;
			}
			outStream.Write(array, 0, num);
		}

		private static List<MimeAppleTranscoder.EntryDescriptor> ReadAppleFileHeaderEntries(Stream applefileStream)
		{
			int num = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
			if (num != 131072)
			{
				throw new MimeException(EmailMessageStrings.WrongAppleVersionNumber);
			}
			applefileStream.Position += 16L;
			int num2 = MimeAppleTranscoder.ReadUShortFromStream(applefileStream);
			if (num2 > 100)
			{
				throw new MimeException(EmailMessageStrings.TooManyEntriesInApplefile);
			}
			List<MimeAppleTranscoder.EntryDescriptor> list = new List<MimeAppleTranscoder.EntryDescriptor>(num2);
			for (int i = 0; i < num2; i++)
			{
				int entryId = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
				int num3 = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
				int num4 = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
				if ((long)num3 < applefileStream.Position || (long)num3 > applefileStream.Length || num4 < 0 || (long)(num4 + num3) > applefileStream.Length)
				{
					throw new MimeException(EmailMessageStrings.WrongOffsetsInApplefile);
				}
				MimeAppleTranscoder.EntryDescriptor item = new MimeAppleTranscoder.EntryDescriptor(entryId, num3, num4);
				list.Add(item);
			}
			list.Sort(new Comparison<MimeAppleTranscoder.EntryDescriptor>(MimeAppleTranscoder.EntryDescriptor.CompareByOffset));
			return list;
		}

		private static void Transcode(Stream applefileStream, Stream dataForkStream, Stream outAttachMacInfo, Stream outMacBinStream, ref Stream outDataForkStream, out string fileName, out byte[] additionalInfo)
		{
			if (applefileStream.Length < 26L)
			{
				throw new MimeException(EmailMessageStrings.UnexpectedEndOfStream);
			}
			int num = MimeAppleTranscoder.ReadUIntFromStream(applefileStream);
			if (num != 333319 && num != 333312)
			{
				if (dataForkStream != null)
				{
					throw new MimeException(EmailMessageStrings.WrongAppleMagicNumber);
				}
				applefileStream.Position = 0L;
				MimeAppleTranscoder.MacBinToApplefile(applefileStream, outAttachMacInfo, out fileName, out additionalInfo);
				applefileStream.Position = 0L;
				if (outMacBinStream == null)
				{
					return;
				}
				MimeAppleTranscoder.CopyStreamData(applefileStream, outMacBinStream);
				return;
			}
			else
			{
				fileName = null;
				byte[] array = new byte[128];
				array[0] = 0;
				int num2 = 0;
				if (dataForkStream != null)
				{
					num2 = 83;
					MimeAppleTranscoder.WriteIntData((int)dataForkStream.Length, array, ref num2);
				}
				byte[] bytes = MimeAppleTranscoder.macEncoding.GetBytes("mBIN");
				for (int i = 0; i < 4; i++)
				{
					array[102 + i] = bytes[i];
				}
				array[122] = 130;
				array[123] = 129;
				int inputOffset = 0;
				MimeAppleTranscoder.EntryDescriptor entryDescriptor = null;
				int inputOffset2 = 0;
				List<MimeAppleTranscoder.EntryDescriptor> list = MimeAppleTranscoder.ReadAppleFileHeaderEntries(applefileStream);
				foreach (MimeAppleTranscoder.EntryDescriptor entryDescriptor2 in list)
				{
					switch (entryDescriptor2.EntryId)
					{
					case 1:
						entryDescriptor = entryDescriptor2;
						num2 = 83;
						MimeAppleTranscoder.WriteIntData(entryDescriptor2.EntryLength, array, ref num2);
						break;
					case 2:
						inputOffset = entryDescriptor2.EntryOffset;
						num2 = 87;
						MimeAppleTranscoder.WriteIntData(entryDescriptor2.EntryLength, array, ref num2);
						break;
					case 4:
						inputOffset2 = entryDescriptor2.EntryOffset;
						num2 = 99;
						MimeAppleTranscoder.WriteShortData(entryDescriptor2.EntryLength, array, ref num2);
						break;
					}
				}
				if (entryDescriptor != null)
				{
					list.Remove(entryDescriptor);
				}
				if (outAttachMacInfo != null)
				{
					MimeAppleTranscoder.WriteApplefileHeader(list, true, outAttachMacInfo);
				}
				int num3 = (int)applefileStream.Position;
				foreach (MimeAppleTranscoder.EntryDescriptor entryDescriptor3 in list)
				{
					if (entryDescriptor3.EntryOffset < num3)
					{
						throw new MimeException(EmailMessageStrings.WrongOffsetsInApplefile);
					}
					if (entryDescriptor3.EntryOffset > num3)
					{
						if (applefileStream.Length < (long)entryDescriptor3.EntryOffset)
						{
							throw new MimeException(EmailMessageStrings.WrongOffsetsInApplefile);
						}
						applefileStream.Position = (long)entryDescriptor3.EntryOffset;
					}
					if (entryDescriptor3.EntryLength < 0)
					{
						throw new MimeException(EmailMessageStrings.EntryLengthTooBigInApplefile((long)((ulong)entryDescriptor3.EntryLength)));
					}
					int num4 = 0;
					byte[] array2 = null;
					int entryId = entryDescriptor3.EntryId;
					if (entryId != 3)
					{
						switch (entryId)
						{
						case 8:
							num4 = Math.Min(8, entryDescriptor3.EntryLength);
							array2 = new byte[num4];
							MimeAppleTranscoder.ReadFixed(applefileStream, array2, 0, num4);
							for (int j = 0; j < num4; j++)
							{
								array[91 + j] = array2[j];
							}
							break;
						case 9:
							num4 = Math.Min(28, entryDescriptor3.EntryLength);
							array2 = new byte[num4];
							MimeAppleTranscoder.ReadFixed(applefileStream, array2, 0, num4);
							if (num4 >= 16)
							{
								for (int k = 0; k < 16; k++)
								{
									array[65 + k] = array2[k];
								}
								array[101] = array[73];
								array[73] = array[74];
								array[74] = 0;
								if (num4 >= 22)
								{
									array[106] = array2[20];
									array[107] = array2[21];
								}
							}
							break;
						case 10:
							num4 = Math.Min(4, entryDescriptor3.EntryLength);
							array2 = new byte[num4];
							MimeAppleTranscoder.ReadFixed(applefileStream, array2, 0, num4);
							if (num4 == 4)
							{
								array[81] = (byte)((array2[3] & 2) >> 1);
							}
							break;
						}
					}
					else
					{
						num4 = Math.Min(63, entryDescriptor3.EntryLength);
						array2 = new byte[num4];
						MimeAppleTranscoder.ReadFixed(applefileStream, array2, 0, num4);
						fileName = MimeAppleTranscoder.macEncoding.GetString(array2, 0, num4);
						array[1] = (byte)num4;
						for (int l = 0; l < num4; l++)
						{
							array[2 + l] = array2[l];
						}
					}
					if (outAttachMacInfo != null)
					{
						if (array2 != null)
						{
							outAttachMacInfo.Write(array2, 0, num4);
						}
						if (num4 != entryDescriptor3.EntryLength)
						{
							MimeAppleTranscoder.CopyStreamData(applefileStream, outAttachMacInfo, new int?(entryDescriptor3.EntryLength - num4));
						}
					}
					num3 = (int)applefileStream.Position;
				}
				num2 = 124;
				MimeAppleTranscoder.WriteUShortData((int)MimeAppleTranscoder.CalcCRC16(array, 126), array, ref num2);
				MacBinaryHeader macBinaryHeader = new MacBinaryHeader(array);
				if (outMacBinStream == null)
				{
					additionalInfo = null;
					if (outDataForkStream != null)
					{
						ReadableDataStorageOnStream readableDataStorageOnStream = new ReadableDataStorageOnStream(applefileStream, false);
						if (entryDescriptor != null)
						{
							outDataForkStream = readableDataStorageOnStream.OpenReadStream((long)entryDescriptor.EntryOffset, (long)(entryDescriptor.EntryOffset + (int)macBinaryHeader.DataForkLength));
							return;
						}
						outDataForkStream = readableDataStorageOnStream.OpenReadStream(0L, 0L);
					}
					return;
				}
				additionalInfo = MimeAppleTranscoder.EncodeAdditionalInfo(macBinaryHeader.FileCreator, macBinaryHeader.FileType);
				outMacBinStream.Write(array, 0, array.Length);
				if (macBinaryHeader.DataForkLength > 0L)
				{
					if (dataForkStream != null)
					{
						MimeAppleTranscoder.CopyStreamData(dataForkStream, outMacBinStream);
						MimeAppleTranscoder.WriteMacBinPadding(outMacBinStream, (int)macBinaryHeader.DataForkLength);
					}
					else if (entryDescriptor != null)
					{
						MimeAppleTranscoder.CopyDataWithPadding(applefileStream, outMacBinStream, entryDescriptor.EntryOffset, (int)macBinaryHeader.DataForkLength);
					}
				}
				if (macBinaryHeader.ResourceForkLength > 0L)
				{
					MimeAppleTranscoder.CopyDataWithPadding(applefileStream, outMacBinStream, inputOffset, (int)macBinaryHeader.ResourceForkLength);
				}
				if (macBinaryHeader.GetInfoLength > 0)
				{
					MimeAppleTranscoder.CopyDataWithPadding(applefileStream, outMacBinStream, inputOffset2, macBinaryHeader.GetInfoLength);
				}
				return;
			}
		}

		private const int MagicNumberAppleSingle = 333312;

		private const int MagicNumberAppleDouble = 333319;

		private const int AppleHeaderVersion = 131072;

		private const int MacBinHeaderLength = 128;

		private const int MaxApplefileEntries = 100;

		private const int MaxMacFileName = 63;

		private static byte[] colonBytes = ByteString.StringToBytes(":", true);

		private static byte[] semiColonBytes = ByteString.StringToBytes(";", true);

		private static byte[] slashBytes = ByteString.StringToBytes("\\", true);

		private static Encoding macEncoding = CTSGlobals.AsciiEncoding;

		private static byte[] padBytes = new byte[128];

		private static readonly DateTime zeroDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private enum AppleFileEntryId
		{
			DataFork = 1,
			ResourceFork,
			RealFileName,
			MacComment,
			FileDatesInfo = 8,
			FinderInfo,
			MacFileInfo
		}

		private class EntryDescriptor
		{
			public EntryDescriptor(int entryId, int entryOffset, int entryLength)
			{
				this.EntryId = entryId;
				this.EntryOffset = entryOffset;
				this.EntryLength = entryLength;
			}

			public static int CompareByOffset(MimeAppleTranscoder.EntryDescriptor x, MimeAppleTranscoder.EntryDescriptor y)
			{
				int num = x.EntryOffset - y.EntryOffset;
				if (num == 0)
				{
					num = x.EntryLength - y.EntryLength;
				}
				return num;
			}

			public readonly int EntryId;

			public readonly int EntryOffset;

			public readonly int EntryLength;
		}
	}
}
