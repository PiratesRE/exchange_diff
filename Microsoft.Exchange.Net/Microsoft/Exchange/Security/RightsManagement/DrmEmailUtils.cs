using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class DrmEmailUtils
	{
		public static void DwordAlign(BinaryReader reader)
		{
			long num = (reader.BaseStream.Position + 3L & -4L) - reader.BaseStream.Position;
			if (num > 0L)
			{
				reader.BaseStream.Seek(num, SeekOrigin.Current);
			}
		}

		public static int DwordAlign(int position)
		{
			return position + 3 & -4;
		}

		public static void WriteByteLengthUnicodeString(BinaryWriter writer, string value)
		{
			int length = value.Length;
			if (length > 65535)
			{
				throw new InvalidOperationException("Length more than maximum value of unsigned short is not supported");
			}
			if (length < 255)
			{
				writer.Write((byte)length);
			}
			else
			{
				writer.Write(byte.MaxValue);
				writer.Write((ushort)length);
			}
			if (value.Length > 0)
			{
				writer.Write(Encoding.Unicode.GetBytes(value));
			}
		}

		public static string ReadByteLengthUnicodeString(BinaryReader reader)
		{
			int num = 0;
			byte b = reader.ReadByte();
			if (b == 255)
			{
				num = (int)reader.ReadUInt16();
			}
			else
			{
				num = (int)b;
			}
			num *= 2;
			string result = string.Empty;
			if (num > 0)
			{
				try
				{
					if (checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
					{
						throw new EndOfStreamException(DrmStrings.ReadOutlookUnicodeStringErrorBefore);
					}
				}
				catch (OverflowException innerException)
				{
					throw new EndOfStreamException(DrmStrings.ReadOutlookUnicodeStringErrorOverflow, innerException);
				}
				result = Encoding.Unicode.GetString(reader.ReadBytes(num));
			}
			return result;
		}

		public static string ReadByteLengthAnsiString(BinaryReader reader)
		{
			int num = 0;
			byte b = reader.ReadByte();
			if (b == 255)
			{
				num = (int)reader.ReadUInt16();
			}
			else
			{
				num = (int)b;
			}
			string result = string.Empty;
			if (num > 0)
			{
				try
				{
					if (checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
					{
						throw new EndOfStreamException(DrmStrings.ReadOutlookAnsiStringErrorBefore);
					}
				}
				catch (OverflowException innerException)
				{
					throw new EndOfStreamException(DrmStrings.ReadOutlookAnsiStringErrorOverflow, innerException);
				}
				result = Encoding.ASCII.GetString(reader.ReadBytes(num));
			}
			return result;
		}

		public static void WriteUnicodeString(BinaryWriter writer, string value)
		{
			int value2 = value.Length * 2;
			writer.Write(value2);
			writer.Write(Encoding.Unicode.GetBytes(value));
			DrmEmailUtils.WriteZeroBytesToDwordAlign(writer);
		}

		public static string ReadUnicodeString(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			try
			{
				if (num <= 0 || checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
				{
					throw new EndOfStreamException(DrmStrings.ReadUnicodeStringErrorBefore);
				}
			}
			catch (OverflowException innerException)
			{
				throw new EndOfStreamException(DrmStrings.ReadUnicodeStringErrorOverflow, innerException);
			}
			byte[] array = new byte[num];
			int num2 = reader.Read(array, 0, num);
			if (num2 != num)
			{
				throw new EndOfStreamException(DrmStrings.ReadUnicodeStringError);
			}
			string @string = Encoding.Unicode.GetString(array);
			DrmEmailUtils.DwordAlign(reader);
			return @string;
		}

		public static void WriteLicenseString(BinaryWriter writer, string value)
		{
			writer.Write(value.Length);
			writer.Write(Encoding.Unicode.GetBytes(value));
			DrmEmailUtils.WriteZeroBytesToDwordAlign(writer);
		}

		public static string ReadLicenseString(BinaryReader reader)
		{
			int num = reader.ReadInt32() * 2;
			try
			{
				if (num <= 0 || checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
				{
					throw new EndOfStreamException(DrmStrings.ReadLicenseStringErrorBefore);
				}
			}
			catch (OverflowException innerException)
			{
				throw new EndOfStreamException(DrmStrings.ReadLicenseStringErrorOverflow, innerException);
			}
			byte[] array = new byte[num];
			int num2 = reader.Read(array, 0, num);
			if (num2 != num)
			{
				throw new EndOfStreamException(DrmStrings.ReadLicenseStringError);
			}
			string @string = Encoding.Unicode.GetString(array);
			DrmEmailUtils.DwordAlign(reader);
			return @string;
		}

		public static int GetUnicodeStringLength(string value)
		{
			return DrmEmailUtils.DwordAlign(4 + value.Length * 2);
		}

		public static void WriteUTF8String(BinaryWriter writer, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			writer.Write(bytes.Length);
			if (bytes.Length > 0)
			{
				writer.Write(bytes);
				DrmEmailUtils.WriteZeroBytesToDwordAlign(writer);
			}
		}

		public static string ReadUTF8String(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			try
			{
				if (num <= 0 || checked(reader.BaseStream.Position + unchecked((long)num)) > reader.BaseStream.Length)
				{
					throw new EndOfStreamException(DrmStrings.ReadUTF8StringErrorBefore);
				}
			}
			catch (OverflowException innerException)
			{
				throw new EndOfStreamException(DrmStrings.ReadUTF8StringErrorOverflow, innerException);
			}
			byte[] array = new byte[num];
			int num2 = reader.Read(array, 0, num);
			if (num2 != num)
			{
				throw new EndOfStreamException(DrmStrings.ReadUTF8StringError);
			}
			string @string = Encoding.UTF8.GetString(array);
			DrmEmailUtils.DwordAlign(reader);
			return @string;
		}

		private static void WriteZeroBytesToDwordAlign(BinaryWriter writer)
		{
			long position = writer.BaseStream.Position;
			long num = position + 3L & -4L;
			if (num != position)
			{
				int count = (int)(num - position);
				writer.Write(DrmEmailUtils.ZeroBytes, 0, count);
			}
		}

		public static void CopyStream(Stream sourceStream, IStream destinationStream)
		{
			DrmEmailUtils.CopyStream(sourceStream, destinationStream, null);
		}

		public unsafe static void CopyStream(Stream sourceStream, IStream destinationStream, SHA256Cng hashAlgo)
		{
			byte[] array = new byte[16384];
			fixed (IntPtr* ptr = array)
			{
				IntPtr buf = new IntPtr((void*)ptr);
				for (;;)
				{
					int num = sourceStream.Read(array, 0, 16384);
					if (num == 0)
					{
						goto IL_66;
					}
					if (destinationStream.Write(buf, num) != num)
					{
						break;
					}
					if (hashAlgo != null)
					{
						hashAlgo.TransformBlock(array, 0, num, array, 0);
					}
				}
				throw new InvalidOperationException("Wrote an incorrect number of bytes to IStream.");
				IL_66:;
			}
		}

		public static void CopyStream(IStream sourceStream, Stream destinationStream)
		{
			DrmEmailUtils.CopyStream(sourceStream, destinationStream, null);
		}

		public unsafe static void CopyStream(IStream sourceStream, Stream destinationStream, SHA256Cng hashAlgo)
		{
			byte[] array = new byte[16384];
			fixed (IntPtr* ptr = array)
			{
				IntPtr buf = new IntPtr((void*)ptr);
				for (;;)
				{
					int num = sourceStream.Read(buf, 16384);
					if (num == 0)
					{
						break;
					}
					destinationStream.Write(array, 0, num);
					if (hashAlgo != null)
					{
						hashAlgo.TransformBlock(array, 0, num, array, 0);
					}
				}
			}
		}

		public static IStorage OpenStorageOverStream(Stream stream)
		{
			ILockBytes plkbyt = new ILockBytesOverStream(stream);
			IStorage result;
			int errorCode = SafeNativeMethods.StgOpenStorageOnILockBytes(plkbyt, null, 16U, IntPtr.Zero, 0U, out result);
			Marshal.ThrowExceptionForHR(errorCode);
			return result;
		}

		public static IStorage CreateStorageOverStream(Stream stream)
		{
			ILockBytes plkbyt = new ILockBytesOverStream(stream);
			IStorage result;
			int errorCode = SafeNativeMethods.StgCreateDocfileOnILockBytes(plkbyt, 4114U, 0U, out result);
			Marshal.ThrowExceptionForHR(errorCode);
			return result;
		}

		public static IStorage EnsureStorage(IStorage parentStorage, string storageName, bool throwOnError)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>(0L, "Opening storage. Name: {0}", storageName);
			IStorage result;
			int errorCode = parentStorage.OpenStorage(storageName, IntPtr.Zero, 16, IntPtr.Zero, 0, out result);
			if (throwOnError)
			{
				Exception exceptionForHR = Marshal.GetExceptionForHR(errorCode);
				if (exceptionForHR != null)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.OpenStorageError(storageName), exceptionForHR);
				}
			}
			return result;
		}

		public static IStorage EnsureStorage(IStorage parentStorage, string storageName)
		{
			return DrmEmailUtils.EnsureStorage(parentStorage, storageName, true);
		}

		public static IStream EnsureStream(IStorage parentStorage, string streamName)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>(0L, "Opening stream. Name: {0}", streamName);
			IStream result;
			int errorCode = parentStorage.OpenStream(streamName, IntPtr.Zero, 16, 0, out result);
			Exception exceptionForHR = Marshal.GetExceptionForHR(errorCode);
			if (exceptionForHR != null)
			{
				throw new InvalidRpmsgFormatException(DrmStrings.OpenStreamError(streamName), exceptionForHR);
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static DrmEmailUtils()
		{
			byte[] zeroBytes = new byte[4];
			DrmEmailUtils.ZeroBytes = zeroBytes;
		}

		private static readonly byte[] ZeroBytes;
	}
}
