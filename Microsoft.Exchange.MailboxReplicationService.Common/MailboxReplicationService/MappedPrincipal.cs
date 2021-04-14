using System;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MappedPrincipal : IEquatable<MappedPrincipal>
	{
		public MappedPrincipal()
		{
		}

		public SecurityIdentifier ObjectSid
		{
			get
			{
				return this.objectSid;
			}
			set
			{
				this.objectSid = value;
				this.UpdatePresentFields(MappedPrincipalFields.ObjectSid, value != null);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
			set
			{
				this.mailboxGuid = value;
				this.UpdatePresentFields(MappedPrincipalFields.MailboxGuid, value != Guid.Empty);
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "ObjectSid")]
		public string ObjectSidString
		{
			get
			{
				if (!(this.ObjectSid != null))
				{
					return null;
				}
				return this.ObjectSid.ToString();
			}
			set
			{
				this.ObjectSid = (string.IsNullOrEmpty(value) ? null : new SecurityIdentifier(value));
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public Guid ObjectGuid
		{
			get
			{
				return this.objectGuid;
			}
			set
			{
				this.objectGuid = value;
				this.UpdatePresentFields(MappedPrincipalFields.ObjectGuid, value != Guid.Empty);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string ObjectDN
		{
			get
			{
				return this.objectDN;
			}
			set
			{
				this.objectDN = value;
				this.UpdatePresentFields(MappedPrincipalFields.ObjectDN, !string.IsNullOrEmpty(value));
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string LegacyDN
		{
			get
			{
				return this.legacyDN;
			}
			set
			{
				this.legacyDN = value;
				this.UpdatePresentFields(MappedPrincipalFields.LegacyDN, !string.IsNullOrEmpty(value));
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ProxyAddresses
		{
			get
			{
				return this.proxyAddresses;
			}
			set
			{
				if (value != null && value.Length == 0)
				{
					value = null;
				}
				this.proxyAddresses = value;
				this.UpdatePresentFields(MappedPrincipalFields.ProxyAddresses, value != null);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
				this.UpdatePresentFields(MappedPrincipalFields.Alias, !string.IsNullOrEmpty(value));
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
				this.UpdatePresentFields(MappedPrincipalFields.DisplayName, !string.IsNullOrEmpty(value));
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public MappedPrincipal NextEntry
		{
			get
			{
				return this.nextEntry;
			}
			set
			{
				this.nextEntry = value;
			}
		}

		private void UpdatePresentFields(MappedPrincipalFields field, bool isPresent)
		{
			if (isPresent)
			{
				this.presentFields |= field;
				return;
			}
			this.presentFields &= ~field;
		}

		public MappedPrincipal(Guid mailboxGuid)
		{
			this.MailboxGuid = mailboxGuid;
		}

		public MappedPrincipal(SecurityIdentifier objectSid)
		{
			this.ObjectSid = objectSid;
		}

		public MappedPrincipal(ADRawEntry entry)
		{
			this.ObjectGuid = entry.Id.ObjectGuid;
			this.ObjectDN = entry.Id.DistinguishedName;
			object[] properties = entry.GetProperties(MappedPrincipal.PrincipalProperties);
			this.MailboxGuid = ((properties[0] is Guid) ? ((Guid)properties[0]) : Guid.Empty);
			this.LegacyDN = (properties[1] as string);
			this.ObjectSid = (properties[2] as SecurityIdentifier);
			ProxyAddressCollection proxyAddressCollection = properties[3] as ProxyAddressCollection;
			this.ProxyAddresses = ((proxyAddressCollection != null) ? proxyAddressCollection.ToStringArray() : null);
			SecurityIdentifier securityIdentifier = properties[4] as SecurityIdentifier;
			if (securityIdentifier != null && !securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
			{
				this.ObjectSid = securityIdentifier;
			}
			this.Alias = (properties[5] as string);
			this.DisplayName = (properties[6] as string);
		}

		public MappedPrincipalFields PresentFields
		{
			get
			{
				return this.presentFields;
			}
		}

		public bool HasField(MappedPrincipalFields field)
		{
			return (this.presentFields & field) != MappedPrincipalFields.None;
		}

		public override int GetHashCode()
		{
			if (this.HasField(MappedPrincipalFields.MailboxGuid))
			{
				return this.MailboxGuid.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.ObjectSid))
			{
				return this.ObjectSid.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.ObjectGuid))
			{
				return this.ObjectGuid.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.LegacyDN))
			{
				return this.LegacyDN.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.ProxyAddresses))
			{
				return this.ProxyAddresses.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.Alias))
			{
				return this.Alias.GetHashCode();
			}
			if (this.HasField(MappedPrincipalFields.DisplayName))
			{
				return this.DisplayName.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.HasField(MappedPrincipalFields.Alias))
			{
				stringBuilder.AppendFormat("Alias: {0}; ", this.Alias);
			}
			if (this.HasField(MappedPrincipalFields.DisplayName))
			{
				stringBuilder.AppendFormat("DisplayName: {0}; ", this.DisplayName);
			}
			if (this.HasField(MappedPrincipalFields.MailboxGuid))
			{
				stringBuilder.AppendFormat("MailboxGuid: {0}; ", this.MailboxGuid);
			}
			if (this.HasField(MappedPrincipalFields.ObjectSid))
			{
				stringBuilder.AppendFormat("SID: {0}; ", this.ObjectSid);
			}
			if (this.HasField(MappedPrincipalFields.ObjectGuid))
			{
				stringBuilder.AppendFormat("ObjectGuid: {0}; ", this.ObjectGuid);
			}
			if (this.HasField(MappedPrincipalFields.LegacyDN))
			{
				stringBuilder.AppendFormat("LegDN: {0}; ", this.LegacyDN);
			}
			if (this.HasField(MappedPrincipalFields.ProxyAddresses))
			{
				stringBuilder.AppendFormat("Proxies: [{0}]; ", string.Join("; ", this.ProxyAddresses));
			}
			if (this.NextEntry != null)
			{
				stringBuilder.AppendFormat("Next: {0}; ", this.NextEntry.ToString());
			}
			return stringBuilder.ToString().Trim();
		}

		public bool Equals(MappedPrincipal other)
		{
			if (this.HasField(MappedPrincipalFields.MailboxGuid))
			{
				return other.HasField(MappedPrincipalFields.MailboxGuid) && this.MailboxGuid.Equals(other.MailboxGuid);
			}
			if (this.HasField(MappedPrincipalFields.ObjectSid))
			{
				return other.HasField(MappedPrincipalFields.ObjectSid) && this.ObjectSid.Equals(other.ObjectSid);
			}
			return this.HasField(MappedPrincipalFields.ObjectGuid) && other.HasField(MappedPrincipalFields.ObjectGuid) && this.ObjectGuid.Equals(other.ObjectGuid);
		}

		private MappedPrincipalFields presentFields;

		private Guid mailboxGuid = Guid.Empty;

		private SecurityIdentifier objectSid;

		private Guid objectGuid = Guid.Empty;

		private string objectDN;

		private string legacyDN;

		private string[] proxyAddresses;

		private string alias;

		private string displayName;

		private MappedPrincipal nextEntry;

		public static readonly PropertyDefinition[] PrincipalProperties = new PropertyDefinition[]
		{
			ADMailboxRecipientSchema.ExchangeGuid,
			ADRecipientSchema.LegacyExchangeDN,
			ADMailboxRecipientSchema.Sid,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.MasterAccountSid,
			ADRecipientSchema.Alias,
			ADRecipientSchema.DisplayName
		};
	}
}
