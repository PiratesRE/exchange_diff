using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.AirSync
{
	internal class StreamHelper
	{
		public static uint UpdateCrc32(uint crc32, byte[] buffer, int offset, int length)
		{
			crc32 ^= uint.MaxValue;
			while (--length >= 0)
			{
				crc32 = (StreamHelper.crcTable[(int)((UIntPtr)((crc32 ^ (uint)buffer[offset++]) & 255U))] ^ crc32 >> 8);
			}
			crc32 ^= uint.MaxValue;
			return crc32;
		}

		public static int CopyStreamWithBase64Conversion(Stream source, Stream target, int byteCount, bool encode)
		{
			if (byteCount == 0)
			{
				return 0;
			}
			if (byteCount < 0)
			{
				byteCount = int.MaxValue;
			}
			byte[] array = null;
			StreamHelper.CharArrayBuffer charArrayBuffer = null;
			int num = 0;
			int num2 = 0;
			int result;
			try
			{
				array = StreamHelper.bytesBufferPool.Acquire();
				charArrayBuffer = StreamHelper.charsBufferPool.Acquire();
				if (array == null)
				{
					throw new InvalidOperationException("bytesBuffer should never be null.");
				}
				if (charArrayBuffer == null)
				{
					throw new InvalidOperationException("base64CharsBuffer should never be null.");
				}
				do
				{
					int count = (18432 > byteCount - num) ? (byteCount - num) : 18432;
					int num3 = source.Read(array, 0, count);
					if (num3 == 0)
					{
						break;
					}
					if (encode)
					{
						int charCount = Convert.ToBase64CharArray(array, 0, num3, charArrayBuffer.Buffer, 0);
						int bytes = Encoding.UTF8.GetBytes(charArrayBuffer.Buffer, 0, charCount, array, 0);
						target.Write(array, 0, bytes);
						num2 += bytes;
					}
					else
					{
						int chars = Encoding.UTF8.GetChars(array, 0, num3, charArrayBuffer.Buffer, 0);
						byte[] array2 = Convert.FromBase64CharArray(charArrayBuffer.Buffer, 0, chars);
						target.Write(array2, 0, array2.Length);
						num2 += array2.Length;
					}
					num += num3;
				}
				while (num < byteCount);
				result = num2;
			}
			finally
			{
				if (array != null)
				{
					StreamHelper.bytesBufferPool.Release(array);
				}
				if (charArrayBuffer != null)
				{
					charArrayBuffer.Clear();
					StreamHelper.charsBufferPool.Release(charArrayBuffer);
				}
			}
			return result;
		}

		internal static int CopyStream(Stream source, Stream target, Encoding encoding, int charCount, bool needCrc, out uint crc)
		{
			crc = 0U;
			byte[] array = null;
			StreamHelper.CharArrayBuffer charArrayBuffer = null;
			int num = 0;
			if (charCount == 0)
			{
				return 0;
			}
			if (charCount < 0)
			{
				charCount = int.MaxValue;
			}
			int result;
			try
			{
				array = StreamHelper.bytesBufferPool.Acquire();
				charArrayBuffer = StreamHelper.charsBufferPool.Acquire();
				if (array == null)
				{
					throw new InvalidOperationException("bytesBuffer should never be null.");
				}
				if (charArrayBuffer == null)
				{
					throw new InvalidOperationException("charsBuffer should never be null.");
				}
				StreamReader streamReader = new StreamReader(source, encoding);
				do
				{
					int num2 = (6144 > charCount - num - 1) ? (charCount - num - 1) : 6144;
					if (num2 == 0)
					{
						num2 = 1;
					}
					int num3 = streamReader.ReadBlock(charArrayBuffer.Buffer, 0, num2);
					if (num3 == 0)
					{
						break;
					}
					if (StreamHelper.NeedToHandleSurrogate(charArrayBuffer.Buffer, num2, num3))
					{
						int num4 = streamReader.Read();
						if (num4 != -1)
						{
							charArrayBuffer.Buffer[num3] = (char)num4;
							char c = charArrayBuffer.Buffer[num3];
							char c2 = charArrayBuffer.Buffer[num3 - 1];
							if (!char.IsSurrogatePair(c2, c))
							{
								string message = string.Format(CultureInfo.InvariantCulture, "Invalid surrogate chars: first={0}, second={1}", new object[]
								{
									c2,
									c
								});
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, message);
							}
							num3++;
						}
					}
					num += num3;
					int bytes = encoding.GetBytes(charArrayBuffer.Buffer, 0, num3, array, 0);
					target.Write(array, 0, bytes);
					if (needCrc)
					{
						crc = StreamHelper.UpdateCrc32(crc, array, 0, bytes);
					}
				}
				while (num < charCount);
				result = num;
			}
			finally
			{
				if (array != null)
				{
					StreamHelper.bytesBufferPool.Release(array);
				}
				if (charArrayBuffer != null)
				{
					charArrayBuffer.Clear();
					StreamHelper.charsBufferPool.Release(charArrayBuffer);
				}
			}
			return result;
		}

		internal static int CopyStream(Stream source, Stream target)
		{
			return StreamHelper.CopyStream(source, target, int.MaxValue);
		}

		internal static int CopyStream(Stream source, Stream target, int byteCount)
		{
			uint num;
			return StreamHelper.CopyStream(source, target, 0, byteCount, false, out num);
		}

		internal static int CopyStream(Stream source, Stream target, int byteCount, out uint crc)
		{
			return StreamHelper.CopyStream(source, target, 0, byteCount, true, out crc);
		}

		internal static int CopyStream(Stream source, Stream target, int offset, int count)
		{
			uint num;
			return StreamHelper.CopyStream(source, target, offset, count, false, out num);
		}

		internal static int CopyStream(Stream source, Stream target, int offset, int count, bool needCrc, out uint crc)
		{
			byte[] array = null;
			int num = 0;
			crc = 0U;
			if (count == 0)
			{
				return 0;
			}
			if (count < 0)
			{
				count = int.MaxValue;
			}
			try
			{
				array = StreamHelper.bytesBufferPool.Acquire();
				if (array == null)
				{
					throw new InvalidOperationException("buffer should never be null.");
				}
				int num2;
				if (source.CanSeek)
				{
					source.Seek((long)offset, SeekOrigin.Begin);
				}
				else if (offset > 0)
				{
					do
					{
						num2 = source.Read(array, 0, (offset < array.Length) ? offset : array.Length);
						offset -= num2;
					}
					while (num2 > 0 && offset > 0);
				}
				do
				{
					num2 = source.Read(array, 0, (count < array.Length) ? count : array.Length);
					if (num2 != 0)
					{
						target.Write(array, 0, num2);
						if (needCrc)
						{
							crc = StreamHelper.UpdateCrc32(crc, array, 0, num2);
						}
						AirSyncDiagnostics.TraceBinaryData(ExTraceGlobals.RawBodyBytesTracer, null, array, num2);
						count -= num2;
						num += num2;
					}
				}
				while (num2 > 0 && count > 0);
			}
			finally
			{
				if (array != null)
				{
					StreamHelper.bytesBufferPool.Release(array);
				}
			}
			return num;
		}

		private static bool NeedToHandleSurrogate(char[] data, int neededLength, int actualLength)
		{
			if (neededLength > 1 && actualLength == 1 && char.IsSurrogate(data[0]))
			{
				return true;
			}
			if (actualLength > 1)
			{
				char c = data[actualLength - 1];
				char c2 = data[actualLength - 2];
				if (char.IsSurrogate(c) && !char.IsSurrogate(c2))
				{
					return true;
				}
				if (char.IsSurrogate(c) && char.IsSurrogate(c2) && !char.IsSurrogatePair(c2, c))
				{
					return true;
				}
			}
			return false;
		}

		private const int BufferBaseUnit = 3072;

		private const int MaxBufferBaseUnitCount = 10;

		private static readonly uint[] crcTable = new uint[]
		{
			0U,
			1996959894U,
			3993919788U,
			2567524794U,
			124634137U,
			1886057615U,
			3915621685U,
			2657392035U,
			249268274U,
			2044508324U,
			3772115230U,
			2547177864U,
			162941995U,
			2125561021U,
			3887607047U,
			2428444049U,
			498536548U,
			1789927666U,
			4089016648U,
			2227061214U,
			450548861U,
			1843258603U,
			4107580753U,
			2211677639U,
			325883990U,
			1684777152U,
			4251122042U,
			2321926636U,
			335633487U,
			1661365465U,
			4195302755U,
			2366115317U,
			997073096U,
			1281953886U,
			3579855332U,
			2724688242U,
			1006888145U,
			1258607687U,
			3524101629U,
			2768942443U,
			901097722U,
			1119000684U,
			3686517206U,
			2898065728U,
			853044451U,
			1172266101U,
			3705015759U,
			2882616665U,
			651767980U,
			1373503546U,
			3369554304U,
			3218104598U,
			565507253U,
			1454621731U,
			3485111705U,
			3099436303U,
			671266974U,
			1594198024U,
			3322730930U,
			2970347812U,
			795835527U,
			1483230225U,
			3244367275U,
			3060149565U,
			1994146192U,
			31158534U,
			2563907772U,
			4023717930U,
			1907459465U,
			112637215U,
			2680153253U,
			3904427059U,
			2013776290U,
			251722036U,
			2517215374U,
			3775830040U,
			2137656763U,
			141376813U,
			2439277719U,
			3865271297U,
			1802195444U,
			476864866U,
			2238001368U,
			4066508878U,
			1812370925U,
			453092731U,
			2181625025U,
			4111451223U,
			1706088902U,
			314042704U,
			2344532202U,
			4240017532U,
			1658658271U,
			366619977U,
			2362670323U,
			4224994405U,
			1303535960U,
			984961486U,
			2747007092U,
			3569037538U,
			1256170817U,
			1037604311U,
			2765210733U,
			3554079995U,
			1131014506U,
			879679996U,
			2909243462U,
			3663771856U,
			1141124467U,
			855842277U,
			2852801631U,
			3708648649U,
			1342533948U,
			654459306U,
			3188396048U,
			3373015174U,
			1466479909U,
			544179635U,
			3110523913U,
			3462522015U,
			1591671054U,
			702138776U,
			2966460450U,
			3352799412U,
			1504918807U,
			783551873U,
			3082640443U,
			3233442989U,
			3988292384U,
			2596254646U,
			62317068U,
			1957810842U,
			3939845945U,
			2647816111U,
			81470997U,
			1943803523U,
			3814918930U,
			2489596804U,
			225274430U,
			2053790376U,
			3826175755U,
			2466906013U,
			167816743U,
			2097651377U,
			4027552580U,
			2265490386U,
			503444072U,
			1762050814U,
			4150417245U,
			2154129355U,
			426522225U,
			1852507879U,
			4275313526U,
			2312317920U,
			282753626U,
			1742555852U,
			4189708143U,
			2394877945U,
			397917763U,
			1622183637U,
			3604390888U,
			2714866558U,
			953729732U,
			1340076626U,
			3518719985U,
			2797360999U,
			1068828381U,
			1219638859U,
			3624741850U,
			2936675148U,
			906185462U,
			1090812512U,
			3747672003U,
			2825379669U,
			829329135U,
			1181335161U,
			3412177804U,
			3160834842U,
			628085408U,
			1382605366U,
			3423369109U,
			3138078467U,
			570562233U,
			1426400815U,
			3317316542U,
			2998733608U,
			733239954U,
			1555261956U,
			3268935591U,
			3050360625U,
			752459403U,
			1541320221U,
			2607071920U,
			3965973030U,
			1969922972U,
			40735498U,
			2617837225U,
			3943577151U,
			1913087877U,
			83908371U,
			2512341634U,
			3803740692U,
			2075208622U,
			213261112U,
			2463272603U,
			3855990285U,
			2094854071U,
			198958881U,
			2262029012U,
			4057260610U,
			1759359992U,
			534414190U,
			2176718541U,
			4139329115U,
			1873836001U,
			414664567U,
			2282248934U,
			4279200368U,
			1711684554U,
			285281116U,
			2405801727U,
			4167216745U,
			1634467795U,
			376229701U,
			2685067896U,
			3608007406U,
			1308918612U,
			956543938U,
			2808555105U,
			3495958263U,
			1231636301U,
			1047427035U,
			2932959818U,
			3654703836U,
			1088359270U,
			936918000U,
			2847714899U,
			3736837829U,
			1202900863U,
			817233897U,
			3183342108U,
			3401237130U,
			1404277552U,
			615818150U,
			3134207493U,
			3453421203U,
			1423857449U,
			601450431U,
			3009837614U,
			3294710456U,
			1567103746U,
			711928724U,
			3020668471U,
			3272380065U,
			1510334235U,
			755167117U
		};

		private static BufferPool bytesBufferPool = new BufferPool(30720, GlobalSettings.MaxWorkerThreadsPerProc);

		private static ThrottlingObjectPool<StreamHelper.CharArrayBuffer> charsBufferPool = new ThrottlingObjectPool<StreamHelper.CharArrayBuffer>(Environment.ProcessorCount, GlobalSettings.MaxWorkerThreadsPerProc * Environment.ProcessorCount);

		private class CharArrayBuffer
		{
			public char[] Buffer
			{
				get
				{
					return this.buffer;
				}
			}

			public void Clear()
			{
				Array.Clear(this.buffer, 0, this.buffer.Length);
			}

			private char[] buffer = new char[30720];
		}
	}
}
