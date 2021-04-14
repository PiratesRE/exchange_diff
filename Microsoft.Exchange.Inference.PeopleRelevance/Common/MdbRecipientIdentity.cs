using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	[Serializable]
	internal class MdbRecipientIdentity : IIdentity, IEquatable<IIdentity>
	{
		internal MdbRecipientIdentity(string smtpAddress)
		{
			if (string.IsNullOrEmpty(smtpAddress))
			{
				throw new ArgumentException("SMTP address is null or empty");
			}
			this.smtpAddress = smtpAddress.ToUpper();
		}

		internal string SmptAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as IIdentity);
		}

		public virtual bool Equals(IIdentity other)
		{
			return this.Equals(other as MdbRecipientIdentity);
		}

		public override int GetHashCode()
		{
			return this.SmptAddress.GetHashCode();
		}

		public override string ToString()
		{
			return this.SmptAddress;
		}

		private bool Equals(MdbRecipientIdentity other)
		{
			return other != null && (object.ReferenceEquals(other, this) || string.Equals(this.SmptAddress, other.SmptAddress, StringComparison.OrdinalIgnoreCase));
		}

		private readonly string smtpAddress;
	}
}
