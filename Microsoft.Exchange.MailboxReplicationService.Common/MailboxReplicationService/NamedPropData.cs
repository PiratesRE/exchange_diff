using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class NamedPropData : IEquatable<NamedPropData>
	{
		public NamedPropData()
		{
		}

		[DataMember(IsRequired = true)]
		public int Kind { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid Guid { get; set; }

		public NamedPropData(Guid npGuid, string npName)
		{
			this.Guid = npGuid;
			this.Name = npName;
			this.Kind = 1;
		}

		public NamedPropData(Guid npGuid, int npId)
		{
			this.Guid = npGuid;
			this.Id = npId;
			this.Kind = 0;
		}

		public override int GetHashCode()
		{
			if (this.Kind == 0)
			{
				return this.Id.GetHashCode() ^ this.Guid.GetHashCode();
			}
			return this.Name.GetHashCode() ^ this.Guid.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[{0}:{1}]", (this.Kind == 0) ? ("0x" + this.Id.ToString("X")) : this.Name, this.Guid);
		}

		public bool Equals(NamedPropData other)
		{
			if (this.Kind != other.Kind)
			{
				return false;
			}
			if (this.Kind == 0)
			{
				return this.Id == other.Id && this.Guid == other.Guid;
			}
			return this.Guid == other.Guid && string.Compare(this.Name, other.Name) == 0;
		}
	}
}
