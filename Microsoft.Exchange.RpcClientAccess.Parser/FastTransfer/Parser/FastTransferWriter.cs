using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferWriter : BaseObject, IFastTransferDataInterface, IDisposable
	{
		internal FastTransferWriter(ArraySegment<byte> buffer)
		{
			this.buffer = buffer;
			this.writer = new BufferWriter(this.buffer);
			this.overflowWriter = new BufferWriter(this.overflowBuffer);
		}

		public int AvailableBufferSize
		{
			get
			{
				if (!this.OverflowOccurred && !this.forceBufferFull)
				{
					return (int)(this.writer.AvailableSpace + 1U);
				}
				return 0;
			}
		}

		public bool IsBufferFull
		{
			get
			{
				return this.AvailableBufferSize == 0 || this.forceBufferFull;
			}
		}

		public void ForceBufferFull()
		{
			this.forceBufferFull = true;
		}

		public int CopyFrom(Stream source, int count)
		{
			int num = Math.Min(this.AvailableBufferSize, count);
			int num2 = Math.Min(num, (int)this.writer.AvailableSpace);
			int num3 = this.writer.CopyFrom(source, num2);
			if (num3 == num2 && num > num2)
			{
				num3 += this.overflowWriter.CopyFrom(source, num - num2);
			}
			return num3;
		}

		public int Position
		{
			get
			{
				return (int)this.writer.Position;
			}
		}

		public ArraySegment<byte> GetOverflowBytes()
		{
			if (this.OverflowOccurred)
			{
				return this.overflowBuffer.SubSegment(0, (int)this.overflowWriter.Position);
			}
			return default(ArraySegment<byte>);
		}

		public void PutMarker(PropertyTag marker)
		{
			if (!marker.IsMarker)
			{
				throw new ArgumentException("Should be a marker", "marker");
			}
			uint atom = FastTransferWriter.DenormalizeTag(null, marker);
			this.WriteAtom<uint>(delegate(Writer writer, uint value)
			{
				writer.WriteUInt32(value);
			}, atom);
		}

		public void WritePropertyInfo(FastTransferDownloadContext context, PropertyTag propertyTag, NamedProperty namedProperty)
		{
			uint key = FastTransferWriter.DenormalizeTag(context, propertyTag);
			this.WriteAtom<KeyValuePair<uint, NamedProperty>>(delegate(Writer writer, KeyValuePair<uint, NamedProperty> value)
			{
				FastTransferWriter.PropertyInfoWriter(writer, value);
			}, new KeyValuePair<uint, NamedProperty>(key, namedProperty));
		}

		public void WriteLength(uint length)
		{
			this.WriteAtom<uint>(delegate(Writer writer, uint value)
			{
				writer.WriteUInt32(value);
			}, length);
		}

		public void SerializeFixedSizeValue(PropertyValue propertyValue)
		{
			PropertyType propertyType = propertyValue.PropertyTag.PropertyType;
			if (propertyType == PropertyType.Bool)
			{
				this.WriteAtom<short>(delegate(Writer writer, short value)
				{
					writer.WriteInt16(value);
				}, propertyValue.GetValueAssert<bool>() ? 1 : 0);
				return;
			}
			this.WriteAtom<PropertyValue>(delegate(Writer writer, PropertyValue pv)
			{
				writer.WritePropertyValueWithoutTag(pv, CTSGlobals.AsciiEncoding, WireFormatStyle.Rop);
			}, propertyValue);
		}

		public void SerializeVariableSizeValue(byte[] data, int startIndex, int sizeToWriteOut)
		{
			if (sizeToWriteOut > this.AvailableBufferSize)
			{
				throw new ArgumentOutOfRangeException("sizeToWriteOut");
			}
			int num = Math.Min(sizeToWriteOut, (int)this.writer.AvailableSpace);
			this.writer.WriteBytesSegment(new ArraySegment<byte>(data, startIndex, num));
			if (num < sizeToWriteOut)
			{
				this.overflowWriter.WriteBytesSegment(new ArraySegment<byte>(data, startIndex + num, sizeToWriteOut - num));
			}
			this.atomsWrittenSinceLastChangeOfState++;
		}

		public bool TryWriteOverflow(ref ArraySegment<byte> overflowBytes)
		{
			if (overflowBytes.Count == 0)
			{
				return true;
			}
			if ((long)overflowBytes.Count <= (long)((ulong)this.writer.AvailableSpace))
			{
				this.writer.WriteBytesSegment(overflowBytes);
				overflowBytes = default(ArraySegment<byte>);
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("FX Writer: {0}/{1}, overflow size = {2}", this.writer.Position, this.buffer.Count, this.overflowWriter.Position);
		}

		[DebuggerStepThrough]
		public void NotifyCanSplitBuffers()
		{
			this.atomsWrittenSinceLastChangeOfState = 0;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferWriter>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.writer);
			Util.DisposeIfPresent(this.overflowWriter);
			base.InternalDispose();
		}

		private static uint DenormalizeTag(FastTransferDownloadContext context, PropertyTag propertyTag)
		{
			if (context == null)
			{
				return FastTransferWriter.DenormalizeTag(true, false, propertyTag);
			}
			return FastTransferWriter.DenormalizeTag(context.UseCpidOrUnicode, context.UseCpid, propertyTag);
		}

		internal static uint DenormalizeTag(bool useCpidOrUnicode, bool useCpid, PropertyTag propertyTag)
		{
			uint num = propertyTag;
			if (PropertyTag.IsStringPropertyType(propertyTag.ElementPropertyType))
			{
				num &= 4294930432U;
				if (useCpidOrUnicode)
				{
					if (useCpid)
					{
						num |= 33968U;
					}
					else
					{
						num |= 31U;
					}
				}
				else
				{
					num |= 30U;
				}
			}
			return num;
		}

		private static void PropertyInfoWriter(Writer writer, KeyValuePair<uint, NamedProperty> propertyInfo)
		{
			uint key = propertyInfo.Key;
			NamedProperty value = propertyInfo.Value;
			writer.WriteUInt32(key);
			if (value != null)
			{
				writer.WriteGuid(value.Guid);
				writer.WriteByte((byte)value.Kind);
				if (value.Kind == NamedPropertyKind.String)
				{
					writer.WriteUnicodeString(value.Name, StringFlags.IncludeNull);
					return;
				}
				if (value.Kind == NamedPropertyKind.Id)
				{
					writer.WriteUInt32(value.Id);
				}
			}
		}

		private void WriteAtom<TAtom>(Action<Writer, TAtom> writeDelegate, TAtom atom)
		{
			if (writeDelegate.Target != null)
			{
				throw new ArgumentException("Writer should be a static delegate for perf reasons");
			}
			bool flag = true;
			if (this.writer.AvailableSpace < 536U)
			{
				using (CountWriter countWriter = new CountWriter())
				{
					writeDelegate(countWriter, atom);
					if ((ulong)this.writer.AvailableSpace < (ulong)countWriter.Position)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				writeDelegate(this.writer, atom);
			}
			else
			{
				writeDelegate(this.overflowWriter, atom);
			}
			this.atomsWrittenSinceLastChangeOfState++;
		}

		private bool OverflowOccurred
		{
			get
			{
				return this.overflowWriter.Position > 0L;
			}
		}

		private const int MaxOutputBufferBytes = 32767;

		private const int OverflowLimit = 536;

		private const int Teaser = 1;

		private const int PerformanceConstantMaxBuffersPerProcessor = 5;

		private readonly ArraySegment<byte> buffer;

		private readonly BufferWriter writer;

		private readonly ArraySegment<byte> overflowBuffer = new ArraySegment<byte>(new byte[536]);

		private readonly BufferWriter overflowWriter;

		private bool forceBufferFull;

		private int atomsWrittenSinceLastChangeOfState;
	}
}
