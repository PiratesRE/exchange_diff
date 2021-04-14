using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BinaryDeserializer : DisposeTrackableBase
	{
		public BinaryDeserializer(byte[] buffer)
		{
			if (buffer != null)
			{
				this.stream = new MemoryStream(buffer);
			}
			else
			{
				this.stream = new MemoryStream();
			}
			this.stream.Position = 0L;
			this.reader = new BinaryReader(this.stream);
		}

		public BinaryDeserializer(ArraySegment<byte> segment)
		{
			this.stream = new MemoryStream(segment.Array, segment.Offset, segment.Count);
			this.stream.Position = 0L;
			this.reader = new BinaryReader(this.stream);
		}

		public BinaryReader Reader
		{
			get
			{
				return this.reader;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.stream != null)
				{
					this.stream.Dispose();
					this.stream = null;
				}
				if (this.reader != null)
				{
					this.reader.Dispose();
					this.reader = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BinaryDeserializer>(this);
		}

		public T[] ReadArray<T>(Func<BinaryDeserializer, T> deserializeOneElement)
		{
			int num = this.ReadInt();
			if (num < 0 || (long)num > this.stream.Length - this.stream.Position || num > 20000)
			{
				MapiExceptionHelper.ThrowIfError("Invalid serialized array size", -2147024809);
			}
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = deserializeOneElement(this);
			}
			return array;
		}

		public int ReadInt()
		{
			return this.reader.ReadInt32();
		}

		public Guid ReadGuid()
		{
			return new Guid(this.reader.ReadBytes(16));
		}

		public string ReadString()
		{
			return this.reader.ReadString();
		}

		public ulong ReadUInt64()
		{
			return this.reader.ReadUInt64();
		}

		public byte[] ReadBytes()
		{
			int num = this.ReadInt();
			if (num < 0 || (long)num > this.stream.Length - this.stream.Position)
			{
				MapiExceptionHelper.ThrowIfError("Invalid serialized byte array size", -2147024809);
			}
			return this.reader.ReadBytes(num);
		}

		public PropValue ReadPropValue()
		{
			PropTag propTag = (PropTag)this.ReadInt();
			object value = null;
			PropType propType = propTag.ValueType();
			if (propType <= PropType.String)
			{
				if (propType <= PropType.Boolean)
				{
					if (propType == PropType.Int)
					{
						value = this.ReadInt();
						goto IL_151;
					}
					switch (propType)
					{
					case PropType.Error:
						value = this.ReadInt();
						goto IL_151;
					case PropType.Boolean:
						value = (this.ReadInt() != 0);
						goto IL_151;
					}
				}
				else
				{
					if (propType == PropType.Long)
					{
						value = (long)this.ReadUInt64();
						goto IL_151;
					}
					switch (propType)
					{
					case PropType.AnsiString:
					case PropType.String:
						value = this.ReadString();
						goto IL_151;
					}
				}
			}
			else if (propType <= PropType.Guid)
			{
				if (propType == PropType.SysTime)
				{
					long longValue = (long)this.ReadUInt64();
					value = PropValue.LongAsDateTime(longValue);
					goto IL_151;
				}
				if (propType == PropType.Guid)
				{
					value = this.ReadGuid();
					goto IL_151;
				}
			}
			else
			{
				if (propType == PropType.Binary)
				{
					value = this.ReadBytes();
					goto IL_151;
				}
				switch (propType)
				{
				case PropType.AnsiStringArray:
				case PropType.StringArray:
					value = this.ReadArray<string>((BinaryDeserializer binaryDeserializer) => binaryDeserializer.ReadString());
					goto IL_151;
				}
			}
			MapiExceptionHelper.ThrowIfError(string.Format("Unable to deserialize PropValue type {0}", propTag.ValueType()), -2147221246);
			IL_151:
			return new PropValue(propTag, value);
		}

		public PropValue[] ReadPropValues()
		{
			return this.ReadArray<PropValue>((BinaryDeserializer deserializer) => deserializer.ReadPropValue());
		}

		public static void Deserialize(byte[] request, BinaryDeserializer.DeserializeDelegate del)
		{
			if (request == null)
			{
				MapiExceptionHelper.ThrowIfError("Invalid serialized input data", -2147024809);
			}
			try
			{
				using (BinaryDeserializer binaryDeserializer = new BinaryDeserializer(request))
				{
					del(binaryDeserializer);
				}
			}
			catch (EndOfStreamException)
			{
				MapiExceptionHelper.ThrowIfError("Invalid serialized input data", -2147024809);
			}
		}

		public void Deserialize(BinaryDeserializer.DeserializeDelegate del)
		{
			try
			{
				del(this);
			}
			catch (EndOfStreamException)
			{
				MapiExceptionHelper.ThrowIfError("Invalid serialized input data", -2147024809);
			}
		}

		private MemoryStream stream;

		private BinaryReader reader;

		public delegate void DeserializeDelegate(BinaryDeserializer deserializer);
	}
}
