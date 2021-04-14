using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PermissionSecurityPrincipal
	{
		public PermissionSecurityPrincipal(ADRecipient adRecipient)
		{
			this.type = PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal;
			this.adRecipient = adRecipient;
		}

		public PermissionSecurityPrincipal(ExternalUser externalUser)
		{
			this.type = PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal;
			this.externalUser = externalUser;
		}

		public PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType specialPrincipalType)
		{
			EnumValidator.ThrowIfInvalid<PermissionSecurityPrincipal.SpecialPrincipalType>(specialPrincipalType, "specialPrincipalType");
			this.type = PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal;
			this.specialPrincipalType = specialPrincipalType;
		}

		public PermissionSecurityPrincipal(string memberName, byte[] entryId, long memberId)
		{
			this.type = PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal;
			this.memberName = memberName;
			this.memberEntryId = entryId;
			this.memberId = memberId;
		}

		public PermissionSecurityPrincipal.SecurityPrincipalType Type
		{
			get
			{
				return this.type;
			}
		}

		public string UnknownPrincipalMemberName
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.memberName;
			}
		}

		public byte[] UnknownPrincipalEntryId
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.memberEntryId;
			}
		}

		public long UnknownPrincipalMemberId
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.memberId;
			}
		}

		public PermissionSecurityPrincipal.SpecialPrincipalType SpecialType
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.specialPrincipalType;
			}
		}

		public ADRecipient ADRecipient
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.adRecipient;
			}
		}

		public ExternalUser ExternalUser
		{
			get
			{
				if (this.type != PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
				{
					throw new InvalidOperationException("Incorrect principal type.");
				}
				return this.externalUser;
			}
		}

		public override string ToString()
		{
			return this.IndexString;
		}

		public string IndexString
		{
			get
			{
				switch (this.type)
				{
				case PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal:
					return this.adRecipient.LegacyExchangeDN;
				case PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal:
					return this.externalUser.SmtpAddress.ToString();
				case PermissionSecurityPrincipal.SecurityPrincipalType.UnknownPrincipal:
					return this.memberId.ToString();
				}
				return string.Empty;
			}
		}

		private readonly PermissionSecurityPrincipal.SecurityPrincipalType type;

		private readonly ADRecipient adRecipient;

		private readonly ExternalUser externalUser;

		private readonly PermissionSecurityPrincipal.SpecialPrincipalType specialPrincipalType;

		private readonly string memberName;

		private readonly byte[] memberEntryId;

		private readonly long memberId;

		public enum SecurityPrincipalType
		{
			ADRecipientPrincipal,
			ExternalUserPrincipal,
			UnknownPrincipal,
			SpecialPrincipal
		}

		public enum SpecialPrincipalType
		{
			Anonymous,
			Default
		}
	}
}
