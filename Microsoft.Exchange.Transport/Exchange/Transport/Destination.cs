using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Transport
{
	internal class Destination : IEquatable<Destination>
	{
		public Destination.DestinationType Type
		{
			get
			{
				return this.type;
			}
		}

		public byte[] Blob
		{
			get
			{
				return this.blob;
			}
		}

		public Destination(Destination.DestinationType type, byte[] data)
		{
			this.blob = data;
			this.type = type;
		}

		public Destination(Destination.DestinationType type, Guid data) : this(type, data.ToByteArray())
		{
		}

		public Destination(Destination.DestinationType type, string data) : this(type, Encoding.Unicode.GetBytes(data))
		{
		}

		public bool Equals(Destination other)
		{
			if (other == null || this.type != other.type)
			{
				return false;
			}
			if (this.blob.Length != other.blob.Length)
			{
				return false;
			}
			for (int i = 0; i < this.blob.Length; i++)
			{
				if (this.blob[i] != other.blob[i])
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Destination);
		}

		public override string ToString()
		{
			if (this.type == Destination.DestinationType.Mdb)
			{
				return this.ToGuid().ToString();
			}
			return Encoding.Unicode.GetString(this.blob);
		}

		public Guid ToGuid()
		{
			return new Guid(this.blob);
		}

		public override int GetHashCode()
		{
			uint num = 0U;
			for (int i = 0; i < this.blob.Length; i++)
			{
				num = (num << 1 | num >> 31);
				num += (uint)this.blob[i];
			}
			return (int)num;
		}

		internal static readonly Dictionary<int, Destination.DestinationType> DestinationTypeDictionary = new Dictionary<int, Destination.DestinationType>();

		private readonly byte[] blob;

		private readonly Destination.DestinationType type;

		internal enum DestinationType : byte
		{
			Mdb = 1,
			Shadow,
			ExternalFqdn,
			Conditional
		}
	}
}
