using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class SendAddressIdentity : ObjectId, IEquatable<SendAddressIdentity>
	{
		public SendAddressIdentity()
		{
			this.isUniqueIdentity = false;
		}

		public SendAddressIdentity(string mailboxIdParameterString, string addressId)
		{
			if (mailboxIdParameterString == null)
			{
				throw new ArgumentNullException("userId");
			}
			if (mailboxIdParameterString.Length == 0)
			{
				throw new ArgumentException("mailboxIdParameterString cannot be empty.", "mailboxIdParameterString");
			}
			if (addressId == null)
			{
				throw new ArgumentNullException("addressId");
			}
			this.isUniqueIdentity = true;
			this.mailboxIdParameterString = mailboxIdParameterString;
			this.addressId = addressId;
		}

		public SendAddressIdentity(string stringIdentity)
		{
			if (stringIdentity == null)
			{
				throw new ArgumentNullException("stringIdentity");
			}
			if (stringIdentity.Length == 0)
			{
				throw new ArgumentException("stringIdentity was set to empty.", "stringIdentity");
			}
			int num = stringIdentity.LastIndexOf('\\');
			if (num <= 0)
			{
				throw new ArgumentException(ServerStrings.InvalidSendAddressIdentity, "id");
			}
			this.isUniqueIdentity = true;
			this.mailboxIdParameterString = stringIdentity.Substring(0, num);
			this.addressId = ((num == stringIdentity.Length - 1) ? string.Empty : stringIdentity.Substring(num + 1));
		}

		public bool IsUniqueIdentity
		{
			get
			{
				return this.isUniqueIdentity;
			}
		}

		public string MailboxIdParameterString
		{
			get
			{
				return this.mailboxIdParameterString;
			}
		}

		public string AddressId
		{
			get
			{
				return this.addressId;
			}
		}

		public bool Equals(SendAddressIdentity other)
		{
			return other != null && this.mailboxIdParameterString.Equals(other.mailboxIdParameterString) && this.addressId.Equals(other.addressId);
		}

		public override byte[] GetBytes()
		{
			return null;
		}

		public override string ToString()
		{
			if (this.mailboxIdParameterString == null || this.addressId == null)
			{
				return string.Empty;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				this.mailboxIdParameterString,
				'\\',
				this.addressId
			});
		}

		private const char Separator = '\\';

		private string mailboxIdParameterString;

		private string addressId;

		private bool isUniqueIdentity;
	}
}
