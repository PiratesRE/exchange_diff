using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct ServerObjectHandle : IFormattable, IEquatable<ServerObjectHandle>
	{
		public byte LogonIndex
		{
			get
			{
				return (byte)(this.handleValue >> 24);
			}
		}

		public uint HandleValue
		{
			get
			{
				return this.handleValue;
			}
		}

		public ServerObjectHandle(uint handleValue)
		{
			this.handleValue = handleValue;
		}

		public ServerObjectHandle(byte logonIndex, uint counter)
		{
			if (counter > 16777214U)
			{
				throw new ArgumentOutOfRangeException("counter", "Counter too large");
			}
			this.handleValue = ServerObjectHandle.LogonHandle(logonIndex) + counter;
		}

		public static ServerObjectHandle Parse(Reader reader)
		{
			uint num = reader.ReadUInt32();
			return new ServerObjectHandle(num);
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt32(this.handleValue);
		}

		public bool IsLogonHandle(byte logonIndex)
		{
			return this.handleValue == ServerObjectHandle.LogonHandle(logonIndex);
		}

		public static ServerObjectHandle CreateLogonHandle(byte logonIndex)
		{
			return new ServerObjectHandle(ServerObjectHandle.LogonHandle(logonIndex));
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		private static uint LogonHandle(byte logonIndex)
		{
			return (uint)((uint)logonIndex << 24);
		}

		public bool Equals(ServerObjectHandle other)
		{
			return this.handleValue == other.handleValue;
		}

		public string ToString(string format, IFormatProvider fp)
		{
			if (format != null)
			{
				if (!(format == "B"))
				{
					if (!(format == "G"))
					{
					}
				}
				else
				{
					if (!(this != ServerObjectHandle.None))
					{
						return "?";
					}
					return string.Format(fp, "0x{0:X}", new object[]
					{
						this.handleValue
					});
				}
			}
			return string.Format(fp, "ServerObjectHandle: 0x{0:X}", new object[]
			{
				this.handleValue
			});
		}

		public override bool Equals(object obj)
		{
			return obj is ServerObjectHandle && this.Equals((ServerObjectHandle)obj);
		}

		public override int GetHashCode()
		{
			return this.handleValue.GetHashCode();
		}

		public static bool operator ==(ServerObjectHandle left, ServerObjectHandle right)
		{
			return left.handleValue == right.handleValue;
		}

		public static bool operator !=(ServerObjectHandle left, ServerObjectHandle right)
		{
			return left.handleValue != right.handleValue;
		}

		private readonly uint handleValue;

		public static readonly ServerObjectHandle None = new ServerObjectHandle(uint.MaxValue);

		public static readonly ServerObjectHandle ReleasedObject = new ServerObjectHandle(1, 16777214U);
	}
}
