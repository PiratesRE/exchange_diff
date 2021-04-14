using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class DiagnosticContext
	{
		public static DiagnosticContext.IDiagnosticContextTest DiagnosticContextTest
		{
			get
			{
				if (DiagnosticContext.diagnosticContextTestFactory != null)
				{
					if (DiagnosticContext.diagnosticContextTest == null)
					{
						DiagnosticContext.diagnosticContextTest = DiagnosticContext.diagnosticContextTestFactory();
					}
					return DiagnosticContext.diagnosticContextTest;
				}
				return null;
			}
		}

		public static bool HasData
		{
			get
			{
				BipBuffer buffer = DiagnosticContext.GetBuffer();
				return buffer.AllocatedSize > 0;
			}
		}

		public static int Size
		{
			get
			{
				BipBuffer buffer = DiagnosticContext.GetBuffer();
				return buffer.AllocatedSize + DiagnosticContext.ContextBuffer.StructSize;
			}
		}

		public static uint ContextSignatureMask
		{
			get
			{
				return 4293918720U;
			}
		}

		public static uint ContextLidMask
		{
			get
			{
				return 1048575U;
			}
		}

		public static void SetTestHook(DiagnosticContext.DiagnosticContextTestFactory diagnosticContextFactory)
		{
			DiagnosticContext.diagnosticContextTestFactory = diagnosticContextFactory;
		}

		public static void SetOnLIDCallback(Action<LID> callback)
		{
			DiagnosticContext.onLIDCallback = callback;
		}

		public unsafe static void TraceLocation(LID lid)
		{
			DiagnosticContext.TraceTestLocation(lid.Value);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(268435456U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationRecord* ptr2 = (DiagnosticContext.LocationRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 268435456U);
			}
		}

		public unsafe static void TraceDword(LID lid, uint info)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, 0U, info);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(269484032U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndDwordRecord* ptr2 = (DiagnosticContext.LocationAndDwordRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 269484032U);
				ptr2->Info = info;
			}
		}

		public unsafe static void TraceLong(LID lid, ulong info)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, (uint)(info >> 32), (uint)(info & (ulong)-1));
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(544210944U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndLongRecord* ptr2 = (DiagnosticContext.LocationAndLongRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 544210944U);
				ptr2->Info = info;
			}
		}

		public unsafe static void TraceGenericError(LID lid, uint error)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, error);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(270532608U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndGenericErrorRecord* ptr2 = (DiagnosticContext.LocationAndGenericErrorRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 270532608U);
				ptr2->Error = error;
			}
		}

		public unsafe static void TraceWin32Error(LID lid, uint error)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, error);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(271581184U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndWin32ErrorRecord* ptr2 = (DiagnosticContext.LocationAndWin32ErrorRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 271581184U);
				ptr2->WinError = error;
			}
		}

		public unsafe static void TraceStoreError(LID lid, uint storeError)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, storeError);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(272629760U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndStoreErrorRecord* ptr2 = (DiagnosticContext.LocationAndStoreErrorRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 272629760U);
				ptr2->StoreError = storeError;
			}
		}

		public unsafe static void TracePropTagError(LID lid, uint storeError, uint propTag)
		{
			DiagnosticContext.TraceTestLocation(lid.Value, storeError, propTag);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(543162368U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndStoreErrorAndProptagRecord* ptr2 = (DiagnosticContext.LocationAndStoreErrorAndProptagRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 543162368U);
				ptr2->StoreError = storeError;
				ptr2->PropTag = propTag;
			}
		}

		public unsafe static void TraceDwordAndString(LID lid, uint info, string str)
		{
			DiagnosticContext.TraceTestLocation(lid.Value);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			DiagnosticContext.GetBuffer();
			int bytes = DiagnosticContext.asciiEncoding.GetBytes(str, 0, Math.Min(103, str.Length), DiagnosticContext.stringBuffer, 0);
			int num = Math.Min(bytes, 103) + 1;
			int num2 = 1 + (num + 7) / 8;
			uint num3 = (uint)((uint)num2 << 28);
			num3 |= 5242880U;
			byte[] array;
			int num4;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(num3), out array, out num4);
			fixed (byte* ptr = &array[num4])
			{
				DiagnosticContext.LocationAndDwordAndStringRecord* ptr2 = (DiagnosticContext.LocationAndDwordAndStringRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, num3);
				ptr2->Info = info;
			}
			Array.Copy(DiagnosticContext.stringBuffer, 0, array, num4 + DiagnosticContext.LocationAndDwordAndStringRecord.StructSize, num - 1);
			array[num4 + DiagnosticContext.LocationAndDwordAndStringRecord.StructSize + num - 1] = 0;
		}

		public unsafe static void TraceGuid(LID lid, Guid info)
		{
			DiagnosticContext.TraceTestLocation(lid.Value);
			if (DiagnosticContext.onLIDCallback != null)
			{
				DiagnosticContext.onLIDCallback(lid);
			}
			byte[] array;
			int num;
			DiagnosticContext.GetBufferPointer(DiagnosticContext.SizeOfRecordFromSignature(813694976U), out array, out num);
			fixed (byte* ptr = &array[num])
			{
				DiagnosticContext.LocationAndGuidRecord* ptr2 = (DiagnosticContext.LocationAndGuidRecord*)ptr;
				ptr2->Lid = DiagnosticContext.AdjustLID(lid.Value, 813694976U);
				ptr2->Info = info;
			}
		}

		public static DiagnosticContext.MeasuredScope TraceLatency(LID lid)
		{
			return new DiagnosticContext.MeasuredScope(lid);
		}

		public static void ExtractInfo(int maxSize, out byte flags, out byte[] info)
		{
			BipBuffer buffer = DiagnosticContext.GetBuffer();
			DiagnosticContext.Shared.ExtractInfo(buffer, maxSize, out info);
			flags = (byte)(2 | (DiagnosticContext.overFlow ? 1 : 0));
		}

		public unsafe static int PackInfo(byte[] destinationBuffer, ref int pos, int maxSize)
		{
			BipBuffer buffer = DiagnosticContext.GetBuffer();
			if (maxSize < DiagnosticContext.ContextBuffer.StructSize)
			{
				return 0;
			}
			while (maxSize < DiagnosticContext.Size)
			{
				DiagnosticContext.FlushHeadRecord(buffer);
			}
			fixed (byte* ptr = &destinationBuffer[pos])
			{
				DiagnosticContext.ContextBuffer* ptr2 = (DiagnosticContext.ContextBuffer*)ptr;
				ptr2->Format = byte.MaxValue;
				ptr2->ThreadID = (uint)Environment.CurrentManagedThreadId;
				ptr2->RequestID = 0U;
				ptr2->Flags = (byte)(2 | (DiagnosticContext.overFlow ? 1 : 0));
				ptr2->Length = (uint)buffer.AllocatedSize;
			}
			pos += DiagnosticContext.ContextBuffer.StructSize;
			maxSize -= DiagnosticContext.ContextBuffer.StructSize;
			int allocatedSize = buffer.AllocatedSize;
			buffer.Extract(destinationBuffer, pos, allocatedSize);
			pos += allocatedSize;
			return allocatedSize + DiagnosticContext.ContextBuffer.StructSize;
		}

		public static byte[] PackInfo()
		{
			BipBuffer buffer = DiagnosticContext.GetBuffer();
			byte[] array = new byte[DiagnosticContext.ContextBuffer.StructSize + buffer.AllocatedSize];
			int num = 0;
			DiagnosticContext.PackInfo(array, ref num, array.Length);
			return array;
		}

		public static void Reset()
		{
			if (DiagnosticContext.DiagnosticContextTest != null)
			{
				DiagnosticContext.DiagnosticContextTest.Reset();
			}
			BipBuffer buffer = DiagnosticContext.GetBuffer();
			DiagnosticContext.Shared.Reset(buffer);
			DiagnosticContext.overFlow = false;
		}

		public static void AppendToBuffer(byte[] sourceBuffer)
		{
			int size = sourceBuffer.Length - DiagnosticContext.ContextBuffer.StructSize;
			byte[] destinationArray;
			int destinationIndex;
			DiagnosticContext.GetBufferPointer(size, out destinationArray, out destinationIndex);
			Array.Copy(sourceBuffer, DiagnosticContext.ContextBuffer.StructSize, destinationArray, destinationIndex, sourceBuffer.Length - DiagnosticContext.ContextBuffer.StructSize);
		}

		private static void TraceTestLocation(uint testLid)
		{
			if (DiagnosticContext.DiagnosticContextTest != null)
			{
				DiagnosticContext.DiagnosticContextTest.TraceTestLocation(testLid, 0U, 0U);
			}
		}

		private static void TraceTestLocation(uint testLid, uint error)
		{
			if (DiagnosticContext.DiagnosticContextTest != null)
			{
				DiagnosticContext.DiagnosticContextTest.TraceTestLocation(testLid, error, 0U);
			}
		}

		private static void TraceTestLocation(uint testLid, uint error, uint info)
		{
			if (DiagnosticContext.DiagnosticContextTest != null)
			{
				DiagnosticContext.DiagnosticContextTest.TraceTestLocation(testLid, error, info);
			}
		}

		private static BipBuffer GetBuffer()
		{
			BipBuffer bipBuffer = DiagnosticContext.bipBuffer;
			if (bipBuffer == null)
			{
				bipBuffer = (DiagnosticContext.bipBuffer = new BipBuffer(2048));
				DiagnosticContext.overFlow = false;
				DiagnosticContext.stringBuffer = new byte[104];
			}
			return bipBuffer;
		}

		private static uint AdjustLID(uint lid, uint signature)
		{
			return DiagnosticContext.Shared.AdjustLID(lid, signature);
		}

		private static int SizeOfRecordFromSignature(uint signature)
		{
			return DiagnosticContext.Shared.SizeOfRecordFromSignature(signature);
		}

		private static void FlushHeadRecord(BipBuffer buf)
		{
			byte b;
			buf.Extract(3, out b);
			int num = (b >> 4) * 8;
			buf.Release(num - 4);
			DiagnosticContext.overFlow = true;
		}

		private static void GetBufferPointer(int size, out byte[] buffer, out int index)
		{
			BipBuffer buffer2 = DiagnosticContext.GetBuffer();
			DiagnosticContext.Shared.GetBufferPointer(buffer2, size, out buffer, out index);
		}

		private const uint ContextRecordLengthMask = 4026531840U;

		private const uint ContextRecordTypeMask = 267386880U;

		private const uint ContextRecordLidMask = 1048575U;

		private const int RecordLengthBitShift = 28;

		private static readonly Encoding asciiEncoding = Encoding.GetEncoding("us-ascii");

		[ThreadStatic]
		private static DiagnosticContext.IDiagnosticContextTest diagnosticContextTest;

		private static DiagnosticContext.DiagnosticContextTestFactory diagnosticContextTestFactory;

		[ThreadStatic]
		private static BipBuffer bipBuffer;

		[ThreadStatic]
		private static bool overFlow;

		[ThreadStatic]
		private static byte[] stringBuffer;

		[ThreadStatic]
		private static Stopwatch cachedStopwatch;

		private static Action<LID> onLIDCallback;

		public delegate DiagnosticContext.IDiagnosticContextTest DiagnosticContextTestFactory();

		public interface IDiagnosticContextTest
		{
			void TraceTestLocation(uint testLid, uint error = 0U, uint info = 0U);

			void Reset();
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationRecord
		{
			public const uint Signature = 268435456U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationRecord));

			public uint Lid;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndDwordRecord
		{
			public const uint Signature = 269484032U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndDwordRecord));

			public uint Lid;

			public uint Info;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndGenericErrorRecord
		{
			public const uint Signature = 270532608U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndGenericErrorRecord));

			public uint Lid;

			public uint Error;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndWin32ErrorRecord
		{
			public const uint Signature = 271581184U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndWin32ErrorRecord));

			public uint Lid;

			public uint WinError;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndStoreErrorRecord
		{
			public const uint Signature = 272629760U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndStoreErrorRecord));

			public uint Lid;

			public uint StoreError;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndDwordAndStringRecord
		{
			public const uint Signature = 5242880U;

			public const int MaxStringLength = 103;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndDwordAndStringRecord));

			public uint Lid;

			public uint Info;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndGuidRecord
		{
			public const uint Signature = 813694976U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndGuidRecord));

			public uint Lid;

			public Guid Info;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndStoreErrorAndProptagRecord
		{
			public const uint Signature = 543162368U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndStoreErrorAndProptagRecord));

			public uint Lid;

			public uint StoreError;

			public uint PropTag;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct LocationAndLongRecord
		{
			public const uint Signature = 544210944U;

			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.LocationAndLongRecord));

			public uint Lid;

			public ulong Info;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ContextBuffer
		{
			public static readonly int StructSize = Marshal.SizeOf(typeof(DiagnosticContext.ContextBuffer));

			public byte Format;

			public uint ThreadID;

			public uint RequestID;

			public byte Flags;

			public uint Length;
		}

		public static class Shared
		{
			public static void ExtractInfo(BipBuffer buf, int maxSize, out byte[] info)
			{
				while (maxSize < buf.AllocatedSize)
				{
					DiagnosticContext.FlushHeadRecord(buf);
				}
				int allocatedSize = buf.AllocatedSize;
				info = new byte[allocatedSize];
				buf.Extract(info, 0, allocatedSize);
			}

			public static void GetBufferPointer(BipBuffer buf, int size, out byte[] buffer, out int index)
			{
				do
				{
					index = buf.Allocate(size);
					if (-1 == index)
					{
						DiagnosticContext.FlushHeadRecord(buf);
					}
				}
				while (-1 == index);
				buffer = buf.Buffer;
			}

			public static void Reset(BipBuffer buf)
			{
				buf.Release(buf.AllocatedSize);
			}

			public static uint AdjustLID(uint lid, uint signature)
			{
				return lid | signature;
			}

			public static int SizeOfRecordFromSignature(uint signature)
			{
				return (int)(8U * (signature >> 28));
			}
		}

		public struct MeasuredScope : IDisposable
		{
			public MeasuredScope(LID lid)
			{
				if (DiagnosticContext.cachedStopwatch != null)
				{
					this.stopwatch = DiagnosticContext.cachedStopwatch;
					DiagnosticContext.cachedStopwatch = null;
				}
				else
				{
					this.stopwatch = new Stopwatch();
				}
				this.lid = lid;
				this.stopwatch.Restart();
			}

			public void Dispose()
			{
				this.stopwatch.Stop();
				DiagnosticContext.TraceDword(this.lid, (uint)this.stopwatch.ElapsedMilliseconds);
				if (DiagnosticContext.cachedStopwatch == null)
				{
					DiagnosticContext.cachedStopwatch = this.stopwatch;
				}
			}

			private Stopwatch stopwatch;

			private LID lid;
		}
	}
}
