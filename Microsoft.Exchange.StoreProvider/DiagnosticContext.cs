using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class DiagnosticContext : ISerializable
	{
		internal DiagnosticContext()
		{
			this.fOverflow = false;
			this.fCircular = true;
			this.records = null;
		}

		internal unsafe DiagnosticContext(THREAD_DIAG_CONTEXT* pErrorInfo)
		{
			if (null != pErrorInfo)
			{
				this.fOverflow = (pErrorInfo->fDataOverflow != 0);
				this.fCircular = (pErrorInfo->fCircularBuffering != 0);
				this.records = null;
				if (pErrorInfo->dwDataSize > 0U && (pErrorInfo->dwDataSize < 8U || pErrorInfo->dwDataSize > 512U))
				{
					return;
				}
				if (pErrorInfo->dwSegm2Beg > 0U && (pErrorInfo->dwSegm2Len < 8U || pErrorInfo->dwSegm2Beg + pErrorInfo->dwSegm2Len > 512U))
				{
					return;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				for (;;)
				{
					DiagnosticContext.GetNextRecordInfo(pErrorInfo, ref num2, out num3, out num4);
					if (num2 == -1)
					{
						break;
					}
					num++;
				}
				if (num > 0)
				{
					this.records = new DiagRecord[num];
					num2 = 0;
					for (int i = 0; i < num; i++)
					{
						uint nextRecordInfo = DiagnosticContext.GetNextRecordInfo(pErrorInfo, ref num2, out num3, out num4);
						byte[] array = new byte[num4];
						for (int j = 0; j < num4; j++)
						{
							array[j] = (&pErrorInfo->byteData.FixedElementField)[num3 + j];
						}
						this.records[i] = new DiagRecord(nextRecordInfo, array);
					}
					return;
				}
			}
			else
			{
				this.fOverflow = false;
				this.records = null;
			}
		}

		internal DiagnosticContext(SerializationInfo info, StreamingContext context)
		{
			this.fOverflow = info.GetBoolean("fOvfl");
			this.fCircular = info.GetBoolean("fCirc");
			int @int = info.GetInt32("nRecs");
			if (@int > 0)
			{
				this.records = new DiagRecord[@int];
				for (int i = 0; i < @int; i++)
				{
					this.records[i] = new DiagRecord("diagRecord" + i, info, context);
				}
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("fOvfl", this.fOverflow);
			info.AddValue("fCirc", this.fCircular);
			info.AddValue("nRecs", (this.records != null) ? this.records.Length : 0);
			if (this.records != null)
			{
				for (int i = 0; i < this.records.Length; i++)
				{
					this.records[i].GetObjectData("diagRecord" + i, info, context);
				}
			}
		}

		public override string ToString()
		{
			int length = this.Length;
			StringBuilder stringBuilder = new StringBuilder(80 * (length + 2));
			if (length > 0 || this.fOverflow)
			{
				stringBuilder.Append(DiagnosticContext.MessageHeader);
			}
			if (this.fOverflow && this.fCircular)
			{
				stringBuilder.Append("\n    ......");
			}
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
				{
					stringBuilder.Append("\n    " + this.records[i].ToString());
				}
			}
			if (this.fOverflow && !this.fCircular)
			{
				stringBuilder.Append("\n    ......");
			}
			return stringBuilder.ToString();
		}

		public string ToCompactString()
		{
			int length = this.Length;
			StringBuilder stringBuilder = new StringBuilder(80 * (length + 2));
			if (this.fOverflow && this.fCircular)
			{
				stringBuilder.Append("...");
				if (length > 0)
				{
					stringBuilder.Append(", ");
				}
			}
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
				{
					stringBuilder.AppendFormat("{0}.{1}", (int)this.records[i].Layout, this.records[i].Lid);
					if (this.records[i].Data != null && this.records[i].Data.Length > 0)
					{
						stringBuilder.Append(":");
						for (int j = 0; j < this.records[i].Data.Length; j++)
						{
							stringBuilder.AppendFormat("{0:X2}", this.records[i].Data[j]);
						}
					}
					if (i != length - 1)
					{
						stringBuilder.Append(", ");
					}
				}
			}
			if (this.fOverflow && !this.fCircular)
			{
				if (length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("...");
			}
			return stringBuilder.ToString();
		}

		private static uint ToQuad(uint val)
		{
			return val + 7U & 4294967288U;
		}

		public byte[] ToByteArray()
		{
			int length = this.Length;
			byte b = 0;
			if (this.fOverflow)
			{
				b |= 1;
			}
			if (this.fCircular)
			{
				b |= 2;
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(0);
					long position = memoryStream.Position;
					binaryWriter.Write(0);
					long position2 = memoryStream.Position;
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Write(0);
					binaryWriter.Write(0);
					binaryWriter.Write(b);
					long position3 = memoryStream.Position;
					binaryWriter.Write(0);
					long position4 = memoryStream.Position;
					for (int i = 0; i < length; i++)
					{
						uint lid = (uint)this.records[i].Lid;
						uint layout = (uint)this.records[i].Layout;
						bool flag = false;
						uint num = DiagnosticContext.ToQuad((uint)(((this.records[i].Data != null) ? this.records[i].Data.Length : 0) + 4));
						uint num2;
						if (num <= 112U)
						{
							num2 = num / 8U;
							flag = true;
						}
						else
						{
							num = DiagnosticContext.ToQuad((uint)(((this.records[i].Data != null) ? this.records[i].Data.Length : 0) + 8));
							num2 = 15U;
						}
						uint value = (num2 << 28 & 4026531840U) | (layout << 20 & 267386880U) | (lid & 1048575U);
						uint num3 = 0U;
						binaryWriter.Write(value);
						num3 += 4U;
						if (!flag)
						{
							binaryWriter.Write(num);
							num3 += 4U;
						}
						if (num > 0U)
						{
							binaryWriter.Write(this.records[i].Data);
							num3 += (uint)this.records[i].Data.Length;
						}
						if (num3 < num)
						{
							uint num4;
							for (num4 = num - num3; num4 > 4U; num4 -= 4U)
							{
								binaryWriter.Write(0U);
							}
							while (num4 > 0U)
							{
								binaryWriter.Write(0);
								num4 -= 1U;
							}
						}
					}
					long position5 = memoryStream.Position;
					memoryStream.Position = position;
					binaryWriter.Write((int)(position5 - position2));
					memoryStream.Position = position3;
					binaryWriter.Write((int)(position5 - position4));
					memoryStream.Position = position5;
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public int Length
		{
			get
			{
				if (this.records != null)
				{
					return this.records.Length;
				}
				return 0;
			}
		}

		public DiagRecord this[int index]
		{
			get
			{
				return this.records[index];
			}
		}

		public DiagRecord FindRecordByLid(int lid, ref int inxStart)
		{
			if (this.records == null || inxStart >= this.records.Length)
			{
				return null;
			}
			for (int i = inxStart; i < this.records.Length; i++)
			{
				if (this.records[i].Lid == lid)
				{
					inxStart = i + 1;
					return this.records[i];
				}
			}
			inxStart = this.records.Length;
			return null;
		}

		public DiagRecord FindRecordByLayout(DiagRecordLayout layout, ref int inxStart)
		{
			if (this.records == null || inxStart >= this.records.Length)
			{
				return null;
			}
			for (int i = inxStart; i < this.records.Length; i++)
			{
				if (this.records[i].Layout == layout)
				{
					inxStart = i + 1;
					return this.records[i];
				}
			}
			inxStart = this.records.Length;
			return null;
		}

		public unsafe static DiagnosticContext GetCurrentContext()
		{
			THREAD_DIAG_CONTEXT thread_DIAG_CONTEXT;
			NativeMethods.DiagnosticCtxGetContext(out thread_DIAG_CONTEXT);
			return new DiagnosticContext(&thread_DIAG_CONTEXT);
		}

		public static void AddToCurrentContext(int lid)
		{
			NativeMethods.DiagnosticCtxLogLocation(lid);
		}

		public static void AddToCurrentContext(int lid, uint value, string message)
		{
			NativeMethods.DiagnosticCtxLogInfo(lid, value, message);
		}

		public static void ReleaseCurrentContext()
		{
			NativeMethods.DiagnosticCtxReleaseContext();
		}

		private unsafe static uint GetNextRecordInfo(THREAD_DIAG_CONTEXT* pErrorInfo, ref int recordOffs, out int dataOffs, out int dataLen)
		{
			dataOffs = (dataLen = 0);
			uint num = pErrorInfo->dwDataSize;
			if (recordOffs == -1 || recordOffs == -2)
			{
				recordOffs = -1;
				return 0U;
			}
			if (pErrorInfo->dwSegm2Beg == 0U)
			{
				if (recordOffs < 0 || (long)recordOffs > (long)((ulong)(pErrorInfo->dwDataSize - 8U)))
				{
					recordOffs = -1;
					return 0U;
				}
			}
			else
			{
				uint num2 = pErrorInfo->dwSegm2Beg + pErrorInfo->dwSegm2Len;
				if (recordOffs == 0)
				{
					recordOffs = (int)pErrorInfo->dwSegm2Beg;
					num = num2;
				}
				else if ((long)recordOffs >= (long)((ulong)pErrorInfo->dwSegm2Beg))
				{
					if ((long)recordOffs > (long)((ulong)(num2 - 8U)))
					{
						if (pErrorInfo->dwDataSize <= 0U)
						{
							recordOffs = -1;
							return 0U;
						}
						recordOffs = 0;
					}
					else
					{
						num = num2;
					}
				}
				else if ((long)recordOffs > (long)((ulong)(pErrorInfo->dwDataSize - 8U)))
				{
					recordOffs = -1;
					return 0U;
				}
			}
			uint ctxDataDword = DiagnosticContext.GetCtxDataDword(pErrorInfo, recordOffs);
			if (ComponentTrace<MapiNetTags>.CheckEnabled(400))
			{
				ComponentTrace<MapiNetTags>.Trace<int, uint>(10034, 400, 0L, "GetNextRecordInfo: offs={0:d2}, lid={1:x}", recordOffs, ctxDataDword);
			}
			int num3;
			int num4;
			if ((ctxDataDword & 4026531840U) != 4026531840U)
			{
				num3 = (int)(ctxDataDword >> 25);
				num3 &= 120;
				num4 = 4;
			}
			else
			{
				num3 = (int)DiagnosticContext.GetCtxDataDword(pErrorInfo, recordOffs + 4);
				num4 = 8;
				if (num3 < 0 || (long)num3 > 512L)
				{
					recordOffs = -1;
					return 0U;
				}
			}
			if ((num3 & 7) != 0 || num3 < 8 || (long)(recordOffs + num3) > (long)((ulong)num))
			{
				recordOffs = -1;
				return 0U;
			}
			dataOffs = recordOffs + num4;
			dataLen = num3 - num4;
			int num5 = recordOffs;
			recordOffs += num3;
			if (pErrorInfo->dwSegm2Beg > 0U && (long)num5 < (long)((ulong)pErrorInfo->dwSegm2Beg) && (long)recordOffs >= (long)((ulong)pErrorInfo->dwSegm2Beg))
			{
				recordOffs = -2;
			}
			return ctxDataDword;
		}

		private unsafe static uint GetCtxDataDword(THREAD_DIAG_CONTEXT* pI, int offs)
		{
			return (uint)((int)(&pI->byteData.FixedElementField)[offs] + ((int)(&pI->byteData.FixedElementField)[offs + 1] << 8) + ((int)(&pI->byteData.FixedElementField)[offs + 2] << 16) + ((int)(&pI->byteData.FixedElementField)[offs + 3] << 24));
		}

		private const int NoMoreRecords = -1;

		private const int LastRecord = -2;

		private bool fOverflow;

		private bool fCircular;

		private DiagRecord[] records;

		public static readonly string MessageHeader = "Diagnostic context:";
	}
}
