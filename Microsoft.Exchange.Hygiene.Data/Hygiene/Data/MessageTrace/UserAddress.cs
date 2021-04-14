using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class UserAddress : ConfigurablePropertyBag
	{
		public UserAddress()
		{
			this.UserAddressId = CombGuidGenerator.NewGuid();
		}

		public UserAddress(string emailPrefix, string emailDomain) : this()
		{
			this.EmailPrefix = emailPrefix;
			this.EmailDomain = emailDomain;
		}

		public Guid UserAddressId
		{
			get
			{
				return (Guid)this[UserAddressSchema.UserAddressIdProperty];
			}
			set
			{
				this[UserAddressSchema.UserAddressIdProperty] = value;
			}
		}

		public string EmailDomain
		{
			get
			{
				return (string)this[UserAddressSchema.EmailDomainProperty];
			}
			set
			{
				this[UserAddressSchema.EmailDomainProperty] = value;
			}
		}

		public string EmailPrefix
		{
			get
			{
				return (string)this[UserAddressSchema.EmailPrefixProperty];
			}
			set
			{
				this[UserAddressSchema.EmailPrefixProperty] = value;
			}
		}

		public int? DigestFrequency
		{
			get
			{
				return (int?)this[UserAddressSchema.DigestFrequencyProperty];
			}
			set
			{
				this[UserAddressSchema.DigestFrequencyProperty] = value;
			}
		}

		public DateTime? LastNotified
		{
			get
			{
				return (DateTime?)this[UserAddressSchema.LastNotifiedProperty];
			}
			set
			{
				this[UserAddressSchema.LastNotifiedProperty] = value;
			}
		}

		public bool? BlockEsn
		{
			get
			{
				return (bool?)this[UserAddressSchema.BlockEsnProperty];
			}
			set
			{
				this[UserAddressSchema.BlockEsnProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.UserAddressId.ToString());
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(UserAddressSchema);
		}
	}
}
