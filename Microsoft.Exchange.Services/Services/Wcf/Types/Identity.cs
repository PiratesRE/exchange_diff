using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Services.Core;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class Identity : INamedIdentity
	{
		public Identity(string identity) : this(identity, identity)
		{
		}

		public Identity(ADObjectId identity) : this(identity, identity.Name)
		{
		}

		internal Identity(string rawIdentity, string displayName)
		{
			this.RawIdentity = rawIdentity;
			this.DisplayName = displayName;
			this.OnDeserialized(default(StreamingContext));
		}

		internal Identity(ADObjectId identity, string displayName) : this((identity.ObjectGuid == Guid.Empty) ? identity.DistinguishedName : identity.ObjectGuid.ToString(), displayName)
		{
		}

		[DataMember]
		public string DisplayName { get; private set; }

		[DataMember(IsRequired = true)]
		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			private set
			{
				ServiceCommandBase.ThrowIfNull(value, "value", "Identity:RawIdentity:Set");
				this.rawIdentity = value;
			}
		}

		string INamedIdentity.Identity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		public static bool operator ==(Identity identity1, Identity identity2)
		{
			return (identity1 == null && identity2 == null) || (identity1 != null && identity2 != null && string.Compare(identity1.RawIdentity, identity2.RawIdentity, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public static bool operator !=(Identity identity1, Identity identity2)
		{
			return !(identity1 == identity2);
		}

		public bool Equals(Identity identity)
		{
			return this == identity;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((Identity)obj);
		}

		public override int GetHashCode()
		{
			return this.RawIdentity.GetHashCode();
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (string.IsNullOrEmpty(this.DisplayName))
			{
				this.DisplayName = this.RawIdentity;
			}
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{",
				this.RawIdentity,
				",",
				this.DisplayName,
				"}"
			});
		}

		private string rawIdentity;
	}
}
