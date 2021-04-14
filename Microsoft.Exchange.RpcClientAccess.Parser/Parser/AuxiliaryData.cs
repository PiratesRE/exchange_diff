using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class AuxiliaryData
	{
		private AuxiliaryData(ArraySegment<byte> auxiliaryInputBuffer)
		{
			Util.ThrowOnNullArgument(auxiliaryInputBuffer.Array, "auxiliaryInputBuffer");
			this.inputBlocks = AuxiliaryData.ParseAuxiliaryBuffer(auxiliaryInputBuffer);
		}

		public IReadOnlyList<AuxiliaryBlock> Input
		{
			get
			{
				return this.inputBlocks;
			}
		}

		public IReadOnlyList<AuxiliaryBlock> Output
		{
			get
			{
				return this.outputBlocks;
			}
		}

		public static AuxiliaryData GetEmptyAuxiliaryData()
		{
			return AuxiliaryData.Parse(null);
		}

		public static AuxiliaryData Parse(byte[] auxIn)
		{
			return AuxiliaryData.Parse(new ArraySegment<byte>(auxIn ?? Array<byte>.Empty));
		}

		public static AuxiliaryData Parse(ArraySegment<byte> auxIn)
		{
			return new AuxiliaryData(auxIn);
		}

		public void AppendOutput(IEnumerable<AuxiliaryBlock> outputBlocks)
		{
			Util.ThrowOnNullArgument(outputBlocks, "outputBlocks");
			foreach (AuxiliaryBlock outputBlock in outputBlocks)
			{
				this.AppendOutput(outputBlock);
			}
		}

		public void AppendOutput(AuxiliaryBlock outputBlock)
		{
			Util.ThrowOnNullArgument(outputBlock, "outputBlock");
			this.outputBlocks.Add(outputBlock);
		}

		public void ReportClientPerformance(IClientPerformanceDataSink sink, Predicate<AuxiliaryBlock> includeBlock)
		{
			foreach (AuxiliaryBlock auxiliaryBlock in this.inputBlocks)
			{
				if (includeBlock(auxiliaryBlock))
				{
					auxiliaryBlock.ReportClientPerformance(sink);
				}
			}
		}

		public ArraySegment<byte> Serialize(ArraySegment<byte> auxiliaryOutputBuffer)
		{
			int count;
			this.Serialize(auxiliaryOutputBuffer, out count);
			return auxiliaryOutputBuffer.SubSegment(0, count);
		}

		public void Serialize(ArraySegment<byte> auxiliaryOutputBuffer, out int auxiliaryOutputSize)
		{
			AuxiliaryData.SerializeAuxiliaryBlocks(this.outputBlocks, auxiliaryOutputBuffer, out auxiliaryOutputSize);
		}

		public int CalculateSerializedOutputSize()
		{
			int num = 0;
			foreach (AuxiliaryBlock auxiliaryBlock in this.outputBlocks)
			{
				num += (int)auxiliaryBlock.CalculateSerializedSize();
			}
			return num;
		}

		internal static AuxiliaryData CreateEmpty()
		{
			return AuxiliaryData.Parse(null);
		}

		internal static void SerializeAuxiliaryBlocks(IEnumerable<AuxiliaryBlock> auxiliaryBlocks, ArraySegment<byte> auxiliaryBuffer, out int size)
		{
			using (BufferWriter bufferWriter = new BufferWriter(auxiliaryBuffer))
			{
				IList<AuxiliaryBlock> list = auxiliaryBlocks as IList<AuxiliaryBlock>;
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!list[i].TrySerialize(bufferWriter))
						{
							break;
						}
					}
				}
				else
				{
					foreach (AuxiliaryBlock auxiliaryBlock in auxiliaryBlocks)
					{
						if (!auxiliaryBlock.TrySerialize(bufferWriter))
						{
							break;
						}
					}
				}
				size = (int)bufferWriter.Position;
			}
		}

		internal static List<AuxiliaryBlock> ParseAuxiliaryBuffer(ArraySegment<byte> buffer)
		{
			List<AuxiliaryBlock> list = new List<AuxiliaryBlock>(4);
			using (BufferReader bufferReader = Reader.CreateBufferReader(buffer))
			{
				while (bufferReader.Length - bufferReader.Position > 0L)
				{
					list.Add(AuxiliaryBlock.Parse(bufferReader));
				}
			}
			return list;
		}

		private readonly List<AuxiliaryBlock> inputBlocks;

		private readonly List<AuxiliaryBlock> outputBlocks = new List<AuxiliaryBlock>(4);
	}
}
