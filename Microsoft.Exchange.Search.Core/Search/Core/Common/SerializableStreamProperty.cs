using System;
using System.IO;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal sealed class SerializableStreamProperty : SerializableProperty<Stream>
	{
		internal SerializableStreamProperty(SerializablePropertyId id, byte[] bytes) : base(id, new MemoryStream(bytes, false))
		{
		}

		internal SerializableStreamProperty(BinaryReader reader)
		{
			this.reader = reader;
		}

		public override SerializablePropertyType Type
		{
			get
			{
				return SerializablePropertyType.Stream;
			}
		}

		public void CopyTo(Stream outputStream)
		{
			Util.ThrowOnNullArgument(outputStream, "outputStream");
			if (this.reader == null)
			{
				throw new InvalidOperationException("No reader is initialized");
			}
			int num = this.reader.ReadInt32();
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size4K);
			byte[] array = bufferPool.Acquire();
			try
			{
				int num2;
				for (int i = num; i > 0; i -= num2)
				{
					num2 = this.reader.Read(array, 0, Math.Min(i, array.Length));
					if (num2 == 0)
					{
						throw new EndOfStreamException("unexpected end of stream");
					}
					outputStream.Write(array, 0, num2);
				}
			}
			finally
			{
				bufferPool.Release(array);
			}
		}

		protected override void SerializeValue(BinaryWriter writer)
		{
			writer.Write((int)base.PropertyValue.Length);
			BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size4K);
			byte[] array = bufferPool.Acquire();
			try
			{
				int count;
				while ((count = base.PropertyValue.Read(array, 0, array.Length)) != 0)
				{
					writer.Write(array, 0, count);
				}
			}
			finally
			{
				bufferPool.Release(array);
			}
		}

		protected override void DeserializeValue(BinaryReader reader)
		{
		}

		private readonly BinaryReader reader;
	}
}
