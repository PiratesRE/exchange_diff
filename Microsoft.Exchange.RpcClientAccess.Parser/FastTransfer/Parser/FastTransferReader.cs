using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FastTransferReader : BaseObject, IFastTransferReader, IFastTransferDataInterface, IDisposable
	{
		internal FastTransferReader(ArraySegment<byte> buffer)
		{
			this.reader = Reader.CreateBufferReader(buffer);
		}

		internal FastTransferReader(Reader reader)
		{
			this.reader = reader;
		}

		public bool IsDataAvailable
		{
			get
			{
				return this.AvailableBufferSize > 0;
			}
		}

		public bool TryPeekMarker(out PropertyTag propertyTag)
		{
			if (this.AvailableBufferSize >= 4)
			{
				int num;
				propertyTag = new PropertyTag(FastTransferReader.NormalizeTag(this.reader.PeekUInt32(0L), out num));
				return propertyTag.IsMarker;
			}
			throw new BufferParseException(string.Format("The buffer has residual data, which cannot be parsed. Residual data size = {0}.", this.AvailableBufferSize));
		}

		public void ReadMarker(PropertyTag expectedMarker)
		{
			if (!expectedMarker.IsMarker)
			{
				throw new ArgumentException("Should be a marker", "expectedMarker");
			}
			int num;
			PropertyTag propertyTag = new PropertyTag(FastTransferReader.NormalizeTag(this.reader.ReadUInt32(), out num));
			this.OnAtomRead();
			if (propertyTag != expectedMarker)
			{
				throw new RopExecutionException(string.Format("Unexpected marker found: {0}. Expected: {1}", propertyTag, expectedMarker), ErrorCode.FxUnexpectedMarker);
			}
		}

		public PropertyTag ReadPropertyInfo(out NamedProperty namedProperty, out int codePage)
		{
			PropertyTag propertyTag = new PropertyTag(FastTransferReader.NormalizeTag(this.reader.ReadUInt32(), out codePage));
			this.OnAtomRead();
			if (propertyTag.IsMarker && !propertyTag.IsMetaProperty)
			{
				throw new RopExecutionException(string.Format("Marker was not expected: {0}", propertyTag), ErrorCode.FxUnexpectedMarker);
			}
			if (propertyTag.IsNamedProperty)
			{
				Guid guid = this.reader.ReadGuid();
				NamedPropertyKind namedPropertyKind = (NamedPropertyKind)this.reader.ReadByte();
				switch (namedPropertyKind)
				{
				case NamedPropertyKind.Id:
				{
					uint id = this.reader.ReadUInt32();
					namedProperty = new NamedProperty(guid, id);
					break;
				}
				case NamedPropertyKind.String:
				{
					string name = this.reader.ReadUnicodeString(StringFlags.IncludeNull);
					namedProperty = new NamedProperty(guid, name);
					break;
				}
				default:
					throw new BufferParseException(string.Format("Unrecognized Kind \"{0}\" for the named property {1}.", namedPropertyKind, propertyTag));
				}
			}
			else
			{
				namedProperty = null;
			}
			return propertyTag;
		}

		public PropertyValue ReadAndParseFixedSizeValue(PropertyTag propertyTag)
		{
			PropertyType propertyType = propertyTag.PropertyType;
			PropertyValue result;
			if (propertyType == PropertyType.Bool)
			{
				result = new PropertyValue(propertyTag, 0 != this.reader.ReadInt16());
			}
			else
			{
				result = this.reader.ReadPropertyValueForTag(propertyTag, WireFormatStyle.Rop);
			}
			this.OnAtomRead();
			return result;
		}

		public int ReadLength(int maxValue)
		{
			int num = this.reader.ReadInt32();
			this.OnAtomRead();
			if (num < 0 || num > maxValue)
			{
				throw new RopExecutionException(string.Format("The length is not in the range. Length = {0}, maxValue = {1}.", num, maxValue), (ErrorCode)2147746565U);
			}
			return num;
		}

		public ArraySegment<byte> ReadVariableSizeValue(int maxToRead)
		{
			if (maxToRead < 0)
			{
				throw new ArgumentOutOfRangeException("sizeToRead");
			}
			int count = Math.Min(this.AvailableBufferSize, maxToRead);
			this.atomsReadSinceLastChangeOfState++;
			return this.reader.ReadArraySegment((uint)count);
		}

		public void NotifyCanSplitBuffers()
		{
			this.atomsReadSinceLastChangeOfState = 0;
		}

		public override string ToString()
		{
			return string.Format("FX Reader: {0}/{1}", this.reader.Position, this.reader.Length);
		}

		protected override void InternalDispose()
		{
			this.reader.Dispose();
			base.InternalDispose();
		}

		internal static uint NormalizeTag(uint rawPropertyTag, out int codePage)
		{
			uint num = rawPropertyTag;
			codePage = 4095;
			if ((rawPropertyTag & 32768U) != 0U)
			{
				codePage = CodePagePropertyTypeTranslator.PropertyTagEncodedCodePageToCodePage((int)(rawPropertyTag & (rawPropertyTag & 4095U)));
				num = (rawPropertyTag & 4294930432U);
				if ((long)codePage == 1200L)
				{
					num |= 31U;
				}
				else
				{
					num |= 30U;
				}
			}
			return num;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferReader>(this);
		}

		private int AvailableBufferSize
		{
			get
			{
				return (int)(this.reader.Length - this.reader.Position);
			}
		}

		private void OnAtomRead()
		{
			this.atomsReadSinceLastChangeOfState++;
			if (this.atomsReadSinceLastChangeOfState > 1)
			{
				throw new InvalidOperationException("At most one atom can be read between changes of states");
			}
		}

		private readonly Reader reader;

		private int atomsReadSinceLastChangeOfState;
	}
}
