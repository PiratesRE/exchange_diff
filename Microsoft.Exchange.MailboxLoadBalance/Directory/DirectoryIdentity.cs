using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory.ExchangeDirectory;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DirectoryIdentity : IEquatable<DirectoryIdentity>
	{
		public DirectoryIdentity(DirectoryObjectType objectType, Guid objectGuid, string objectName, Guid organizationId)
		{
			this.ObjectType = objectType;
			this.Guid = objectGuid;
			this.Name = objectName;
			this.OrganizationId = organizationId;
		}

		[DataMember]
		public string Name { get; private set; }

		[DataMember]
		public Guid Guid { get; private set; }

		[DataMember]
		public Guid OrganizationId { get; private set; }

		public DirectoryObjectType ObjectType { get; private set; }

		public ADObjectId ADObjectId
		{
			get
			{
				if (this.adObjectId == null)
				{
					this.adObjectId = new ADObjectId(this.Guid);
				}
				return this.adObjectId;
			}
		}

		[DataMember]
		protected int ObjectTypeInt
		{
			get
			{
				return (int)this.ObjectType;
			}
			set
			{
				this.ObjectType = (DirectoryObjectType)value;
			}
		}

		public static DirectoryIdentity CreateForestIdentity(string forestName)
		{
			return new DirectoryIdentity(DirectoryObjectType.Forest, Guid.Empty, forestName, Guid.Empty);
		}

		public static DirectoryIdentity CreateFromADObjectId(ADObjectId adObjectId, DirectoryObjectType objectType = DirectoryObjectType.Unknown)
		{
			return new DirectoryIdentity(objectType, adObjectId.ObjectGuid, adObjectId.Name, Guid.Empty);
		}

		public static DirectoryIdentity CreateMailboxIdentity(Guid mailboxGuid, Guid organizationId, DirectoryObjectType objectType = DirectoryObjectType.Mailbox)
		{
			return new DirectoryIdentity(objectType, mailboxGuid, string.Empty, organizationId);
		}

		public static DirectoryIdentity CreateMailboxIdentity(Guid guid, TenantPartitionHint tph, DirectoryObjectType objectType = DirectoryObjectType.Mailbox)
		{
			return DirectoryIdentity.CreateMailboxIdentity(guid, TenantPartitionHintAdapter.FromPartitionHint(tph), objectType);
		}

		public static DirectoryIdentity CreateMailboxIdentity(Guid guid, TenantPartitionHintAdapter tph, DirectoryObjectType objectType = DirectoryObjectType.Mailbox)
		{
			return DirectoryIdentity.CreateMailboxIdentity(guid, tph.ExternalDirectoryOrganizationId, objectType);
		}

		public static DirectoryIdentity CreateNonConnectedMailboxIdentity(Guid mailboxGuid, Guid organizationId)
		{
			return new DirectoryIdentity(DirectoryObjectType.NonConnectedMailbox, mailboxGuid, string.Empty, organizationId);
		}

		public static DirectoryIdentity CreateConsumerMailboxIdentity(Guid mailboxGuid, Guid databaseGuid, Guid organizationId)
		{
			return new DirectoryIdentity(DirectoryObjectType.ConsumerMailbox, mailboxGuid, string.Empty, organizationId);
		}

		public bool Equals(DirectoryIdentity other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (string.Equals(this.Name, other.Name) && this.Guid.Equals(other.Guid) && this.OrganizationId.Equals(other.OrganizationId) && this.ObjectType == other.ObjectType));
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((DirectoryIdentity)obj)));
		}

		public override int GetHashCode()
		{
			int num = (this.Name != null) ? this.Name.GetHashCode() : 0;
			num = (num * 397 ^ this.Guid.GetHashCode());
			num = (num * 397 ^ this.OrganizationId.GetHashCode());
			return num * 397 ^ (int)this.ObjectType;
		}

		public override string ToString()
		{
			if (object.ReferenceEquals(this, DirectoryIdentity.NullIdentity))
			{
				return "{Null Identity}";
			}
			if (this.ObjectType == DirectoryObjectType.ConstraintSet)
			{
				return string.Format("[CSET: {{{0}}}]", this.Name);
			}
			return string.Format("[{0} {1} ID: {2} OID: {3}]", new object[]
			{
				this.ObjectType,
				this.Name,
				this.Guid,
				this.OrganizationId
			});
		}

		public static readonly DirectoryIdentity NullIdentity = new DirectoryIdentity(DirectoryObjectType.Unknown, Guid.Empty, string.Empty, Guid.Empty);

		[IgnoreDataMember]
		private ADObjectId adObjectId;
	}
}
