using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class AuxiliaryBlock
	{
		internal AuxiliaryBlock(byte blockVersion, AuxiliaryBlockTypes blockType)
		{
			this.version = blockVersion;
			this.type = blockType;
		}

		protected AuxiliaryBlock(Reader reader)
		{
			this.version = reader.ReadByte();
			this.type = (AuxiliaryBlockTypes)reader.ReadByte();
		}

		internal byte Version
		{
			get
			{
				return this.version;
			}
		}

		internal AuxiliaryBlockTypes Type
		{
			get
			{
				return this.type;
			}
		}

		public static AuxiliaryBlock Parse(BufferReader reader)
		{
			long num = reader.Length - reader.Position;
			if (num < 2L)
			{
				return new CorruptAuxiliaryBlock(reader);
			}
			int num2 = (int)reader.PeekUInt16(0L);
			if ((long)num2 > num || num2 <= 2)
			{
				return new CorruptAuxiliaryBlock(reader);
			}
			AuxiliaryBlock result;
			using (Reader reader2 = reader.SubReader(num2))
			{
				try
				{
					reader2.ReadUInt16();
					result = AuxiliaryBlock.InternalParse(reader2);
				}
				catch (BufferParseException)
				{
					reader2.Position = 0L;
					result = new CorruptAuxiliaryBlock(reader2);
				}
				finally
				{
					reader.Position += (long)num2;
				}
			}
			return result;
		}

		protected internal virtual void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
		}

		internal bool TrySerialize(BufferWriter writer)
		{
			ushort num = this.CalculateSerializedSize();
			if (writer.AvailableSpace < (uint)num)
			{
				num = (ushort)this.Truncate((int)writer.AvailableSpace, (int)num);
				if (writer.AvailableSpace < (uint)num)
				{
					return false;
				}
			}
			using (Writer writer2 = writer.SubWriter())
			{
				writer2.WriteUInt16(num);
				this.Serialize(writer2);
			}
			writer.Position += (long)((ulong)num);
			return true;
		}

		internal ushort CalculateSerializedSize()
		{
			ushort result;
			using (CountWriter countWriter = new CountWriter())
			{
				countWriter.WriteUInt16(0);
				this.Serialize(countWriter);
				result = (ushort)countWriter.Position;
			}
			return result;
		}

		protected static string ReadUnicodeStringAtPosition(Reader reader, ushort offset)
		{
			if (offset == 0)
			{
				return string.Empty;
			}
			long position = reader.Position;
			reader.Position = (long)((ulong)offset);
			string result = reader.ReadUnicodeString(StringFlags.IncludeNull);
			reader.Position = position;
			return result;
		}

		protected static string ReadAsciiStringAtPosition(Reader reader, ushort offset)
		{
			if (offset == 0)
			{
				return string.Empty;
			}
			long position = reader.Position;
			reader.Position = (long)((ulong)offset);
			string result = reader.ReadAsciiString(StringFlags.IncludeNull);
			reader.Position = position;
			return result;
		}

		protected static ArraySegment<byte> ReadBytesAtPosition(Reader reader, ushort offset, ushort count)
		{
			if (count == 0)
			{
				return Array<byte>.EmptySegment;
			}
			long position = reader.Position;
			reader.Position = (long)((ulong)offset);
			ArraySegment<byte> result = reader.ReadArraySegment((uint)count);
			reader.Position = position;
			return result;
		}

		protected static void WriteUnicodeStringAndUpdateOffset(Writer writer, string stringToWrite, long offsetPosition)
		{
			if (string.IsNullOrEmpty(stringToWrite))
			{
				AuxiliaryBlock.WriteOffset(writer, offsetPosition, 0L);
				return;
			}
			AuxiliaryBlock.WriteOffset(writer, offsetPosition);
			writer.WriteUnicodeString(stringToWrite, StringFlags.IncludeNull);
		}

		protected static void WriteAsciiStringAndUpdateOffset(Writer writer, string stringToWrite, long offsetPosition)
		{
			if (string.IsNullOrEmpty(stringToWrite))
			{
				AuxiliaryBlock.WriteOffset(writer, offsetPosition, 0L);
				return;
			}
			AuxiliaryBlock.WriteOffset(writer, offsetPosition);
			writer.WriteAsciiString(stringToWrite, StringFlags.IncludeNull);
		}

		protected static void WriteBytesAndUpdateOffset(Writer writer, ArraySegment<byte> bytes, long offsetPosition)
		{
			if (bytes.Count == 0)
			{
				AuxiliaryBlock.WriteOffset(writer, offsetPosition, 0L);
				return;
			}
			AuxiliaryBlock.WriteOffset(writer, offsetPosition);
			writer.WriteBytesSegment(bytes);
		}

		protected virtual void Serialize(Writer writer)
		{
			writer.WriteByte(this.version);
			writer.WriteByte((byte)this.type);
		}

		protected virtual int Truncate(int maxSerializedSize, int currentSize)
		{
			return currentSize;
		}

		private static AuxiliaryBlock InternalParse(Reader reader)
		{
			byte b = reader.PeekByte(0L);
			byte b2 = reader.PeekByte(1L);
			if (b == 1)
			{
				AuxiliaryBlockTypes auxiliaryBlockTypes = (AuxiliaryBlockTypes)b2;
				switch (auxiliaryBlockTypes)
				{
				case AuxiliaryBlockTypes.PerfRequestId:
					return new PerfRequestIdAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfClientInfo:
					return new PerfClientInfoAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfServerInfo:
				case AuxiliaryBlockTypes.PerfSessionInfo:
				case AuxiliaryBlockTypes.OsInfo:
					break;
				case AuxiliaryBlockTypes.PerfDefMdbSuccess:
					return new PerfDefMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfDefGcSuccess:
					return new PerfDefGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfMdbSuccess:
					return new PerfMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfGcSuccess:
					return new PerfGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFailure:
					return new PerfFailureAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.ClientControl:
					return new ClientControlAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfProcessInfo:
					return new PerfProcessInfoAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgDefMdbSuccess:
					return new PerfBgDefMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgDefGcSuccess:
					return new PerfBgDefGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgMdbSuccess:
					return new PerfBgMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgGcSuccess:
					return new PerfBgGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgFailure:
					return new PerfBgFailureAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgDefMdbSuccess:
					return new PerfFgDefMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgDefGcSuccess:
					return new PerfFgDefGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgMdbSuccess:
					return new PerfFgMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgGcSuccess:
					return new PerfFgGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgFailure:
					return new PerfFgFailureAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.ExOrgInfo:
					return new ExOrgInfoAuxiliaryBlock(reader);
				default:
					switch (auxiliaryBlockTypes)
					{
					case AuxiliaryBlockTypes.DiagCtxReqId:
						return new DiagCtxReqIdAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.DiagCtxCtxData:
						return new DiagCtxCtxDataAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.PerRpcStatistics:
						return new PerRpcStatisticsAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ClientSessionInfo:
						return new ClientSessionInfoAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ServerCapabilities:
						return new ServerCapabilitiesAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.DiagCtxClientId:
						return new DiagCtxClientIdAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.EndpointCapabilities:
						return new EndpointCapabilitiesAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ExceptionTrace:
						return new ExceptionTraceAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ClientConnectionInfo:
						return new ClientConnectionInfoAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ServerSessionInfo:
						return new ServerSessionInfoAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.SetMonitoringContext:
						return new SetMonitoringContextAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ClientActivity:
						return new ClientActivityAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ProtocolDeviceIdentification:
						return new ProtocolDeviceIdentificationAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.MonitoringActivity:
						return new MonitoringActivityAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.ServerInformation:
						return new ServerInformationAuxiliaryBlock(reader);
					case AuxiliaryBlockTypes.IdentityCorrelationInfo:
						return new IdentityCorrelationAuxiliaryBlock(reader);
					}
					break;
				}
			}
			else if (b == 2)
			{
				AuxiliaryBlockTypes auxiliaryBlockTypes2 = (AuxiliaryBlockTypes)b2;
				switch (auxiliaryBlockTypes2)
				{
				case AuxiliaryBlockTypes.PerfMdbSuccess:
					return new PerfMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfGcSuccess:
					return new PerfGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFailure:
					return new PerfFailureAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.ClientControl:
				case AuxiliaryBlockTypes.PerfBgDefMdbSuccess:
				case AuxiliaryBlockTypes.PerfBgDefGcSuccess:
				case AuxiliaryBlockTypes.PerfFgDefMdbSuccess:
				case AuxiliaryBlockTypes.PerfFgDefGcSuccess:
					break;
				case AuxiliaryBlockTypes.PerfProcessInfo:
					return new PerfProcessInfoAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgMdbSuccess:
					return new PerfBgMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgGcSuccess:
					return new PerfBgGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfBgFailure:
					return new PerfBgFailureAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgMdbSuccess:
					return new PerfFgMdbSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgGcSuccess:
					return new PerfFgGcSuccessAuxiliaryBlock(reader);
				case AuxiliaryBlockTypes.PerfFgFailure:
					return new PerfFgFailureAuxiliaryBlock(reader);
				default:
					if (auxiliaryBlockTypes2 == AuxiliaryBlockTypes.PerRpcStatistics)
					{
						return new PerRpcStatisticsAuxiliaryBlock(reader);
					}
					break;
				}
			}
			else if (b == 3)
			{
				AuxiliaryBlockTypes auxiliaryBlockTypes3 = (AuxiliaryBlockTypes)b2;
				if (auxiliaryBlockTypes3 == AuxiliaryBlockTypes.PerRpcStatistics)
				{
					return new PerRpcStatisticsAuxiliaryBlock(reader);
				}
			}
			else if (b == 4)
			{
				AuxiliaryBlockTypes auxiliaryBlockTypes4 = (AuxiliaryBlockTypes)b2;
				if (auxiliaryBlockTypes4 == AuxiliaryBlockTypes.PerRpcStatistics)
				{
					return new PerRpcStatisticsAuxiliaryBlock(reader);
				}
			}
			else if (b == 5)
			{
				AuxiliaryBlockTypes auxiliaryBlockTypes5 = (AuxiliaryBlockTypes)b2;
				if (auxiliaryBlockTypes5 == AuxiliaryBlockTypes.PerRpcStatistics)
				{
					return new PerRpcStatisticsAuxiliaryBlock(reader);
				}
			}
			AuxiliaryBlockTypes auxiliaryBlockTypes6 = (AuxiliaryBlockTypes)b2;
			if (auxiliaryBlockTypes6 == AuxiliaryBlockTypes.MapiEndpoint)
			{
				return new MapiEndpointAuxiliaryBlock(reader);
			}
			return new UnknownAuxiliaryBlock(reader);
		}

		private static void WriteOffset(Writer writer, long offsetPosition)
		{
			AuxiliaryBlock.WriteOffset(writer, offsetPosition, writer.Position);
		}

		private static void WriteOffset(Writer writer, long offsetPosition, long offsetValue)
		{
			long position = writer.Position;
			writer.Position = offsetPosition;
			writer.WriteUInt16((ushort)offsetValue);
			writer.Position = position;
		}

		private readonly byte version;

		private readonly AuxiliaryBlockTypes type;
	}
}
