using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public sealed class MailboxFolderUserId : ObjectId
	{
		private MailboxFolderUserId(PermissionSecurityPrincipal.SpecialPrincipalType specialPrincipalType)
		{
			switch (specialPrincipalType)
			{
			case PermissionSecurityPrincipal.SpecialPrincipalType.Default:
				this.userType = MailboxFolderUserId.MailboxFolderUserType.Default;
				return;
			}
			this.userType = MailboxFolderUserId.MailboxFolderUserType.Anonymous;
		}

		private MailboxFolderUserId(ADRecipient adRecipient)
		{
			if (adRecipient == null)
			{
				throw new ArgumentNullException("adRecipient");
			}
			this.userType = MailboxFolderUserId.MailboxFolderUserType.Internal;
			this.adRecipient = adRecipient;
		}

		private MailboxFolderUserId(SmtpAddress smtpAddress)
		{
			this.userType = MailboxFolderUserId.MailboxFolderUserType.External;
			this.smtpAddress = smtpAddress;
		}

		private MailboxFolderUserId(ExternalUser externalUser)
		{
			if (externalUser == null)
			{
				throw new ArgumentNullException("externalUser");
			}
			this.userType = MailboxFolderUserId.MailboxFolderUserType.External;
			this.smtpAddress = externalUser.SmtpAddress;
			this.externalUser = externalUser;
		}

		private MailboxFolderUserId(string unknownMemberName)
		{
			if (string.IsNullOrEmpty(unknownMemberName))
			{
				throw new ArgumentNullException("unknownMemberName");
			}
			this.userType = MailboxFolderUserId.MailboxFolderUserType.Unknown;
			this.unknownMemberName = unknownMemberName;
		}

		internal void EnsureExternalUser(MailboxSession mailboxSession)
		{
			if (this.UserType != MailboxFolderUserId.MailboxFolderUserType.External)
			{
				throw new InvalidOperationException("Only support External user type.");
			}
			if (this.externalUser != null)
			{
				return;
			}
			using (ExternalUserCollection externalUsers = mailboxSession.GetExternalUsers())
			{
				ExternalUser externalUser = externalUsers.FindExternalUser(this.smtpAddress);
				if (externalUser == null)
				{
					throw new InvalidExternalUserIdException(this.smtpAddress.ToString());
				}
				this.externalUser = externalUser;
			}
		}

		internal static MailboxFolderUserId CreateFromSecurityPrincipal(PermissionSecurityPrincipal securityPrincipal)
		{
			if (securityPrincipal == null)
			{
				throw new ArgumentNullException("securityPrincipal");
			}
			switch (securityPrincipal.Type)
			{
			case PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal:
				return new MailboxFolderUserId(securityPrincipal.ADRecipient);
			case PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal:
				return new MailboxFolderUserId(securityPrincipal.ExternalUser);
			case PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal:
				return new MailboxFolderUserId(securityPrincipal.SpecialType);
			}
			return new MailboxFolderUserId(securityPrincipal.UnknownPrincipalMemberName);
		}

		internal static MailboxFolderUserId TryCreateFromSmtpAddress(string smtpAddress, MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (string.IsNullOrEmpty(smtpAddress))
			{
				throw new ArgumentNullException("smtpAddress");
			}
			if (!SmtpAddress.IsValidSmtpAddress(smtpAddress))
			{
				throw new ArgumentException("smtpAddress");
			}
			MailboxFolderUserId mailboxFolderUserId = new MailboxFolderUserId(new SmtpAddress(smtpAddress));
			try
			{
				mailboxFolderUserId.EnsureExternalUser(mailboxSession);
			}
			catch (InvalidExternalUserIdException)
			{
				return null;
			}
			return mailboxFolderUserId;
		}

		internal static MailboxFolderUserId TryCreateFromADRecipient(ADRecipient recipient, bool allowInvalidSecurityPrincipal)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (allowInvalidSecurityPrincipal || recipient.IsValidSecurityPrincipal)
			{
				return new MailboxFolderUserId(recipient);
			}
			return null;
		}

		internal static MailboxFolderUserId CreateFromUnknownUser(string unknownUser)
		{
			if (string.IsNullOrEmpty(unknownUser))
			{
				throw new ArgumentNullException("unknownUser");
			}
			return new MailboxFolderUserId(unknownUser);
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.DisplayName;
		}

		internal PermissionSecurityPrincipal ToSecurityPrincipal()
		{
			switch (this.UserType)
			{
			case MailboxFolderUserId.MailboxFolderUserType.Internal:
				return new PermissionSecurityPrincipal(this.adRecipient);
			case MailboxFolderUserId.MailboxFolderUserType.External:
				if (this.externalUser == null)
				{
					throw new InvalidOperationException("external user is null");
				}
				return new PermissionSecurityPrincipal(this.externalUser);
			default:
				throw new NotSupportedException("Only support internal or external user");
			}
		}

		internal bool Equals(PermissionSecurityPrincipal securityPrincipal)
		{
			if (securityPrincipal == null)
			{
				return false;
			}
			switch (securityPrincipal.Type)
			{
			case PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal:
				return this.UserType == MailboxFolderUserId.MailboxFolderUserType.Internal && securityPrincipal.ADRecipient.Id.Equals(this.adRecipient.Id);
			case PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal:
				return this.UserType == MailboxFolderUserId.MailboxFolderUserType.External && securityPrincipal.ExternalUser.SmtpAddress.Equals(this.smtpAddress);
			case PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal:
				return (securityPrincipal.SpecialType == PermissionSecurityPrincipal.SpecialPrincipalType.Default && this.UserType == MailboxFolderUserId.MailboxFolderUserType.Default) || (securityPrincipal.SpecialType == PermissionSecurityPrincipal.SpecialPrincipalType.Anonymous && this.UserType == MailboxFolderUserId.MailboxFolderUserType.Anonymous);
			}
			return this.UserType == MailboxFolderUserId.MailboxFolderUserType.Unknown && StringComparer.InvariantCultureIgnoreCase.Equals(this.unknownMemberName, securityPrincipal.UnknownPrincipalMemberName);
		}

		public MailboxFolderUserId.MailboxFolderUserType UserType
		{
			get
			{
				return this.userType;
			}
		}

		public ADRecipient ADRecipient
		{
			get
			{
				return this.adRecipient;
			}
		}

		public string DisplayName
		{
			get
			{
				switch (this.UserType)
				{
				case MailboxFolderUserId.MailboxFolderUserType.Default:
					return Strings.DefaultUser;
				case MailboxFolderUserId.MailboxFolderUserType.Anonymous:
					return Strings.AnonymousUser;
				case MailboxFolderUserId.MailboxFolderUserType.Internal:
					return this.adRecipient.DisplayName;
				case MailboxFolderUserId.MailboxFolderUserType.External:
					return this.smtpAddress.ToString();
				}
				return this.unknownMemberName;
			}
		}

		internal static readonly MailboxFolderUserId DefaultUserId = new MailboxFolderUserId(PermissionSecurityPrincipal.SpecialPrincipalType.Default);

		internal static readonly MailboxFolderUserId AnonymousUserId = new MailboxFolderUserId(PermissionSecurityPrincipal.SpecialPrincipalType.Anonymous);

		private readonly MailboxFolderUserId.MailboxFolderUserType userType;

		private readonly ADRecipient adRecipient;

		private readonly SmtpAddress smtpAddress;

		private readonly string unknownMemberName;

		[NonSerialized]
		private ExternalUser externalUser;

		public enum MailboxFolderUserType
		{
			Default,
			Anonymous,
			Internal,
			External,
			Unknown
		}
	}
}
