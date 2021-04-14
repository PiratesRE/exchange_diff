using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Configuration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class OwaHelpUrlData : SerializableDataBase, IEquatable<OwaHelpUrlData>
	{
		[DataMember]
		public string HelpUrl { get; set; }

		public bool Equals(OwaHelpUrlData other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(other, this) || string.Equals(this.HelpUrl, other.HelpUrl, StringComparison.OrdinalIgnoreCase));
		}

		protected override bool InternalEquals(object other)
		{
			return this.Equals(other as OwaHelpUrlData);
		}

		protected override int InternalGetHashCode()
		{
			int num = 17;
			return num * 397 ^ (this.HelpUrl ?? string.Empty).ToLowerInvariant().GetHashCode();
		}
	}
}
