using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EmailAddress : IEquatable<EmailAddress>
	{
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string OriginalDisplayName
		{
			get
			{
				return this.originalDisplayName;
			}
			set
			{
				this.originalDisplayName = value;
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
			set
			{
				this.routingType = value;
			}
		}

		public bool Equals(EmailAddress other)
		{
			return other != null && string.Equals(this.address, other.address, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as EmailAddress);
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(this.address))
			{
				return 0;
			}
			return this.address.GetHashCode();
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.routingType,
				":",
				this.address,
				":",
				this.name
			});
		}

		private string name;

		private string originalDisplayName;

		private string address;

		private string routingType;
	}
}
