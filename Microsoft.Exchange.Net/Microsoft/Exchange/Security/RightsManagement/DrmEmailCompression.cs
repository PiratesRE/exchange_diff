using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class DrmEmailCompression
	{
		public static void CompressStream(Stream stream, bool writeMagicHeader)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			ZLib.ZStream zstream = default(ZLib.ZStream);
			long length = stream.Length;
			long num = 0L;
			long num2 = 0L;
			List<GCHandle> list = new List<GCHandle>();
			List<GCHandle> list2 = new List<GCHandle>();
			List<uint> list3 = new List<uint>();
			byte[] array = new byte[8192];
			GCHandle gchandle = default(GCHandle);
			bool flag = false;
			try
			{
				ZLib.ErrorCode errorCode = SafeNativeMethods.rms_deflate_init(ref zstream, 9, "1.2.3", Marshal.SizeOf(zstream));
				if (errorCode != ZLib.ErrorCode.Success)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ZLib deflate init failed. {0}", new object[]
					{
						errorCode
					}));
				}
				flag = true;
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				ZLib.ErrorCode errorCode2;
				for (;;)
				{
					if (num != length && (num - num2 < 8192L || list.Count == 0))
					{
						stream.Seek(num, SeekOrigin.Begin);
						GCHandle item = default(GCHandle);
						try
						{
							if (list2.Count > 0)
							{
								item = list2[0];
								list2.RemoveAt(0);
							}
							else
							{
								item = GCHandle.Alloc(new byte[4096], GCHandleType.Pinned);
							}
							uint num3 = (uint)stream.Read((byte[])item.Target, 0, 4096);
							if (num3 == 0U)
							{
								item.Free();
								goto IL_15E;
							}
							list.Add(item);
							list3.Add(num3);
							num = stream.Position;
						}
						catch
						{
							if (item.IsAllocated)
							{
								item.Free();
							}
							throw;
						}
						continue;
					}
					IL_15E:
					if (list.Count == 0)
					{
						goto IL_282;
					}
					IntPtr pinBuf = list[0].AddrOfPinnedObject();
					uint num4 = list3[0];
					list2.Add(list[0]);
					list.RemoveAt(0);
					list3.RemoveAt(0);
					zstream.PInBuf = pinBuf;
					zstream.CbIn = num4;
					zstream.POutBuf = gchandle.AddrOfPinnedObject();
					zstream.CbOut = 8192U;
					errorCode2 = SafeNativeMethods.rms_deflate(ref zstream, 2);
					if (errorCode2 != ZLib.ErrorCode.Success)
					{
						break;
					}
					stream.Seek(num2, SeekOrigin.Begin);
					if (num2 == 0L && writeMagicHeader)
					{
						stream.Write(DrmEmailConstants.CompressedDRMHeader, 0, DrmEmailConstants.CompressedDRMHeader.Length);
					}
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					binaryWriter.Write(DrmEmailConstants.MagicDrmHeaderChecksum);
					binaryWriter.Write(num4);
					binaryWriter.Write(8192U - zstream.CbOut);
					binaryWriter.Write(array, 0, (int)(8192U - zstream.CbOut));
					num2 = stream.Position;
				}
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ZLib failed to compress the stream. {0}", new object[]
				{
					errorCode2
				}));
				IL_282:;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				foreach (GCHandle gchandle2 in list)
				{
					gchandle2.Free();
				}
				list.Clear();
				foreach (GCHandle gchandle3 in list2)
				{
					gchandle3.Free();
				}
				list2.Clear();
				if (flag)
				{
					SafeNativeMethods.rms_deflateEnd(ref zstream);
				}
			}
			stream.SetLength(num2);
		}

		public static void DecompressStream(Stream sourceStream, Stream destinationStream, bool readMagicHeader)
		{
			if (sourceStream == null)
			{
				throw new ArgumentNullException("sourceStream");
			}
			if (destinationStream == null)
			{
				throw new ArgumentNullException("destinationStream");
			}
			ZLib.ZStream zstream = default(ZLib.ZStream);
			bool flag = false;
			if (readMagicHeader)
			{
				byte[] array = new byte[DrmEmailConstants.CompressedDRMHeader.Length];
				int num = sourceStream.ReadExact(array, 0, array.Length);
				if (num != array.Length)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DRMMagicHeaderNotRead"));
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != DrmEmailConstants.CompressedDRMHeader[i])
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DRMMagicHeaderNotMatch"));
					}
				}
			}
			byte[] array2 = new byte[4096];
			GCHandle gchandle = default(GCHandle);
			byte[] array3 = new byte[8192];
			GCHandle gchandle2 = default(GCHandle);
			byte[] array4 = new byte[12];
			try
			{
				ZLib.ErrorCode errorCode = SafeNativeMethods.rms_inflate_init(ref zstream, "1.2.3", Marshal.SizeOf(zstream));
				if (errorCode != ZLib.ErrorCode.Success)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "ZLIB inflate init failed {0}", new object[]
					{
						errorCode
					}));
				}
				flag = true;
				gchandle2 = GCHandle.Alloc(array3, GCHandleType.Pinned);
				gchandle = GCHandle.Alloc(array2, GCHandleType.Pinned);
				ZLib.ErrorCode errorCode2;
				for (;;)
				{
					int num2 = sourceStream.ReadExact(array4, 0, 12);
					if (num2 == 0)
					{
						goto IL_297;
					}
					if (num2 != 12)
					{
						break;
					}
					int num3 = BitConverter.ToInt32(array4, 0);
					if ((long)num3 != (long)((ulong)DrmEmailConstants.MagicDrmHeaderChecksum))
					{
						goto Block_11;
					}
					int num4 = BitConverter.ToInt32(array4, 4);
					if (num4 <= 0 || num4 > 4096)
					{
						goto IL_191;
					}
					int num5 = BitConverter.ToInt32(array4, 8);
					if (num5 <= 0 || num5 > 8192)
					{
						goto IL_1BE;
					}
					int num6 = sourceStream.ReadExact(array3, 0, num5);
					if (num6 != num5)
					{
						goto Block_14;
					}
					zstream.PInBuf = gchandle2.AddrOfPinnedObject();
					zstream.CbIn = (uint)num5;
					zstream.POutBuf = gchandle.AddrOfPinnedObject();
					zstream.CbOut = (uint)num4;
					errorCode2 = SafeNativeMethods.rms_inflate(ref zstream, 2);
					if (errorCode2 != ZLib.ErrorCode.Success || zstream.CbOut != 0U)
					{
						goto IL_240;
					}
					destinationStream.Write(array2, 0, 4096);
				}
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("CompressHeaderNotRead"));
				Block_11:
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("WrongChecksum"));
				IL_191:
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("WrongUncompressedCount"));
				IL_1BE:
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("WrongCompressedCount"));
				Block_14:
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("CompressedBytesNotRead"));
				IL_240:
				throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat(string.Format(CultureInfo.InvariantCulture, "ZLibDecompressFailed:ZlibError:{0} cbOut:{1}", new object[]
				{
					errorCode2,
					zstream.CbOut
				})));
				IL_297:;
			}
			finally
			{
				if (gchandle2.IsAllocated)
				{
					gchandle2.Free();
				}
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
				if (flag)
				{
					SafeNativeMethods.rms_inflateEnd(ref zstream);
				}
			}
			destinationStream.Position = 0L;
		}

		public static byte[] CompressString(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			byte[] array = null;
			using (Stream stream = new MemoryStream())
			{
				DrmEmailUtils.WriteLicenseString(new BinaryWriter(stream), value);
				DrmEmailCompression.CompressStream(stream, false);
				stream.Seek(0L, SeekOrigin.Begin);
				array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
			}
			return array;
		}

		public static void CompressUseLicense(string useLicense, Stream targetStream)
		{
			if (string.IsNullOrEmpty(useLicense))
			{
				throw new ArgumentNullException("useLicense");
			}
			if (targetStream == null)
			{
				throw new ArgumentNullException("targetStream");
			}
			using (GZipStream gzipStream = new GZipStream(targetStream, CompressionMode.Compress, true))
			{
				using (StreamWriter streamWriter = new StreamWriter(gzipStream, Encoding.Unicode))
				{
					streamWriter.Write(useLicense);
				}
			}
		}

		public static string DecompressUseLicense(Stream compressedLicenseStream)
		{
			if (compressedLicenseStream == null)
			{
				throw new ArgumentNullException("compressedLicenseStream");
			}
			GZipStream gzipStream = null;
			string result;
			try
			{
				gzipStream = new GZipStream(compressedLicenseStream, CompressionMode.Decompress);
				using (StreamReader streamReader = new StreamReader(gzipStream, Encoding.Unicode))
				{
					result = streamReader.ReadToEnd();
				}
			}
			catch (InvalidDataException)
			{
				result = string.Empty;
			}
			finally
			{
				if (gzipStream != null)
				{
					gzipStream.Close();
					gzipStream = null;
				}
			}
			return result;
		}

		public static string DecompressString(byte[] compressedBytes)
		{
			if (compressedBytes == null)
			{
				throw new ArgumentNullException("compressedBytes");
			}
			string result = string.Empty;
			using (Stream stream = new MemoryStream(compressedBytes))
			{
				using (Stream stream2 = new MemoryStream())
				{
					DrmEmailCompression.DecompressStream(stream, stream2, false);
					stream2.Seek(0L, SeekOrigin.Begin);
					result = DrmEmailUtils.ReadLicenseString(new BinaryReader(stream2));
				}
			}
			return result;
		}

		private static int ReadExact(this Stream stream, byte[] buffer, int offset, int count)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			int num = 0;
			int num2;
			while (num < count && (num2 = stream.Read(buffer, offset + num, count - num)) > 0)
			{
				num += num2;
			}
			return num;
		}
	}
}
