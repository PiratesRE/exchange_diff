using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class NamedProperty : IEquatable<NamedProperty>
	{
		public NamedProperty()
		{
			this.kind = NamedPropertyKind.Null;
		}

		public NamedProperty(Guid guid, string name)
		{
			Util.ThrowOnNullArgument(name, "name");
			if (name.Length > 256)
			{
				throw new ArgumentException("name");
			}
			this.kind = NamedPropertyKind.String;
			this.guid = guid;
			this.name = name;
		}

		public NamedProperty(Guid guid, uint id)
		{
			this.kind = NamedPropertyKind.Id;
			this.guid = guid;
			this.id = id;
		}

		public bool IsMapiNamespace
		{
			get
			{
				return this.guid.Equals(NamedProperty.PS_MAPI);
			}
		}

		public NamedPropertyKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string Name
		{
			get
			{
				if (this.kind != NamedPropertyKind.String)
				{
					throw new InvalidOperationException(string.Format("Accessing NamedProperty.Name when it is not a NamedPropertyKind.String [NamedPropertyKind = {0}].", this.kind));
				}
				return this.name;
			}
		}

		public uint Id
		{
			get
			{
				if (this.kind != NamedPropertyKind.Id)
				{
					throw new InvalidOperationException(string.Format("Accessing NamedProperty.Id when it is not a NamedPropertyKind.Id [NamedPropertyKind = {0}].", this.kind));
				}
				return this.id;
			}
		}

		public override int GetHashCode()
		{
			return this.kind.GetHashCode() ^ this.guid.GetHashCode() ^ this.id.GetHashCode() ^ ((this.name != null) ? this.name.GetHashCode() : 0);
		}

		public override bool Equals(object obj)
		{
			NamedProperty namedProperty = obj as NamedProperty;
			return namedProperty != null && this.Equals(namedProperty);
		}

		public bool Equals(NamedProperty v)
		{
			return this.kind == v.kind && this.guid == v.guid && this.id == v.id && this.name == v.name;
		}

		public override string ToString()
		{
			NamedPropertyKind namedPropertyKind = this.kind;
			switch (namedPropertyKind)
			{
			case NamedPropertyKind.Id:
				return string.Format("NamedProperty: [Kind: {0}, Guid: {1}, Id: {2}]", this.kind, this.guid, this.id);
			case NamedPropertyKind.String:
				return string.Format("NamedProperty: [Kind: {0}, Guid: {1}, Name: {2}]", this.kind, this.guid, this.name);
			default:
				if (namedPropertyKind == NamedPropertyKind.Null)
				{
					return string.Format("NamedProperty: [Kind: Null]", new object[0]);
				}
				return "NamedProperty: [Kind: Invalid]";
			}
		}

		internal static NamedProperty CreateMAPINamedProperty(PropertyId propertyId)
		{
			return new NamedProperty(NamedProperty.PS_MAPI, (uint)propertyId);
		}

		internal static NamedProperty Parse(Reader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			NamedPropertyKind namedPropertyKind = (NamedPropertyKind)reader.ReadByte();
			if (namedPropertyKind == NamedPropertyKind.Null)
			{
				return new NamedProperty();
			}
			Guid guid = reader.ReadGuid();
			if (namedPropertyKind == NamedPropertyKind.String)
			{
				StringFlags flags = StringFlags.IncludeNull | StringFlags.Sized;
				return new NamedProperty(guid, reader.ReadUnicodeString(flags));
			}
			if (namedPropertyKind == NamedPropertyKind.Id)
			{
				return new NamedProperty(guid, reader.ReadUInt32());
			}
			throw new BufferParseException("Invalid named property kind");
		}

		internal void Serialize(Writer writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteByte((byte)this.kind);
			if (this.kind == NamedPropertyKind.Null)
			{
				return;
			}
			writer.WriteGuid(this.guid);
			if (this.kind == NamedPropertyKind.String)
			{
				StringFlags stringFlags = StringFlags.IncludeNull | StringFlags.Sized;
				if (this.guid.Equals(NamedProperty.PSETID_InternetHeaders))
				{
					stringFlags |= StringFlags.SevenBitAscii;
				}
				writer.WriteUnicodeString(this.name, stringFlags);
				return;
			}
			if (this.kind == NamedPropertyKind.Id)
			{
				writer.WriteUInt32(this.id);
			}
		}

		internal void SerializeForActions(Writer writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteGuid(this.guid);
			writer.WriteByte((byte)this.kind);
			if (this.kind == NamedPropertyKind.String)
			{
				writer.WriteUnicodeString(this.name, StringFlags.IncludeNull);
				return;
			}
			if (this.kind == NamedPropertyKind.Id)
			{
				writer.WriteUInt32(this.id);
			}
		}

		internal const int MaxNameLength = 256;

		internal static readonly uint MinimumSize = 1U;

		private static readonly Guid PSETID_InternetHeaders = new Guid("{00020386-0000-0000-C000-000000000046}");

		private static readonly Guid PS_MAPI = new Guid("{00020328-0000-0000-C000-000000000046}");

		private readonly NamedPropertyKind kind;

		private readonly Guid guid;

		private readonly string name;

		private readonly uint id;
	}
}
