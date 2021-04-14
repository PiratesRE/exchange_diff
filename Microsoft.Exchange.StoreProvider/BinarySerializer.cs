using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BinarySerializer : DisposeTrackableBase
	{
		public BinaryWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		public BinarySerializer()
		{
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
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
				if (this.writer != null)
				{
					this.writer.Dispose();
					this.writer = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BinarySerializer>(this);
		}

		public byte[] ToArray()
		{
			return this.stream.ToArray();
		}

		public void Write(int value)
		{
			this.writer.Write(value);
		}

		public void Write(Guid value)
		{
			this.writer.Write(value.ToByteArray());
		}

		public void Write(string value)
		{
			this.writer.Write(value);
		}

		public void Write(ulong value)
		{
			this.writer.Write(value);
		}

		public void Write(byte[] buffer)
		{
			this.Write(buffer.Length);
			this.writer.Write(buffer);
		}

		public void Write(PropValue pv)
		{
			this.Write((int)pv.PropTag);
			PropType propType = pv.PropTag.ValueType();
			if (propType <= PropType.String)
			{
				if (propType <= PropType.Boolean)
				{
					if (propType == PropType.Int)
					{
						this.Write(pv.GetInt());
						return;
					}
					switch (propType)
					{
					case PropType.Error:
						this.Write((int)pv.RawValue);
						return;
					case PropType.Boolean:
						this.Write(pv.GetBoolean() ? 1 : 0);
						return;
					}
				}
				else
				{
					if (propType == PropType.Long)
					{
						this.Write((ulong)pv.GetLong());
						return;
					}
					switch (propType)
					{
					case PropType.AnsiString:
					case PropType.String:
						this.Write(pv.GetString());
						return;
					}
				}
			}
			else if (propType <= PropType.Guid)
			{
				if (propType == PropType.SysTime)
				{
					this.Write((ulong)pv.GetLong());
					return;
				}
				if (propType == PropType.Guid)
				{
					this.Write(pv.GetGuid());
					return;
				}
			}
			else
			{
				if (propType == PropType.Binary)
				{
					this.Write(pv.GetBytes());
					return;
				}
				switch (propType)
				{
				case PropType.AnsiStringArray:
				case PropType.StringArray:
				{
					string[] stringArray = pv.GetStringArray();
					this.Write(stringArray.Length);
					foreach (string value in stringArray)
					{
						this.Write(value);
					}
					return;
				}
				}
			}
			MapiExceptionHelper.ThrowIfError(string.Format("Unable to serialize PropValue type {0}", pv.PropTag.ValueType()), -2147221246);
		}

		public void Write(PropValue[] pva)
		{
			this.Write(pva.Length);
			foreach (PropValue pv in pva)
			{
				this.Write(pv);
			}
		}

		public static byte[] Serialize(BinarySerializer.SerializeDelegate del)
		{
			byte[] result;
			using (BinarySerializer binarySerializer = new BinarySerializer())
			{
				del(binarySerializer);
				result = binarySerializer.ToArray();
			}
			return result;
		}

		private MemoryStream stream;

		private BinaryWriter writer;

		public delegate void SerializeDelegate(BinarySerializer serializer);
	}
}
