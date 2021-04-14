using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TenantPublicFolderConfiguration : TenantConfigurationCacheableItem<Organization>
	{
		public HeuristicsFlags HeuristicsFlags { get; private set; }

		public override long ItemSize
		{
			get
			{
				return this.estimatedSize;
			}
		}

		public PublicFoldersDeployment PublicFoldersDeploymentType
		{
			get
			{
				return this.publicFoldersDeploymentType;
			}
		}

		public override void ReadData(IConfigurationSession configurationSession)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, configurationSession.SessionSettings, 104, "ReadData", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\TenantPublicFolderConfiguration.cs");
			this.localPublicFolderRecipients = new Hashtable();
			this.remotePublicFolderRecipients = new Hashtable();
			this.estimatedSize = 0L;
			Organization orgContainer = configurationSession.GetOrgContainer();
			this.hierarchyMailboxInformation = orgContainer.DefaultPublicFolderMailbox;
			this.publicFoldersDeploymentType = orgContainer.PublicFoldersEnabled;
			this.estimatedSize += (long)this.hierarchyMailboxInformation.ItemSize;
			this.HeuristicsFlags = orgContainer.Heuristics;
			this.estimatedSize += 4L;
			this.estimatedSize += 4L;
			ADRawEntry[] array = Array<ADRawEntry>.Empty;
			if (this.hierarchyMailboxInformation.HierarchyMailboxGuid != Guid.Empty)
			{
				array = tenantOrRootOrgRecipientSession.FindPaged<ADRawEntry>(null, QueryScope.SubTree, Filters.GetRecipientTypeDetailsFilterOptimization(RecipientTypeDetails.PublicFolderMailbox), new SortBy(ADObjectSchema.WhenCreatedUTC, SortOrder.Ascending), 0, TenantPublicFolderConfiguration.PublicFolderRecipientProperties).ReadAllPages();
			}
			List<PublicFolderRecipient> list = new List<PublicFolderRecipient>();
			if (this.PublicFoldersDeploymentType == PublicFoldersDeployment.Remote && orgContainer.RemotePublicFolderMailboxes.Count > 0)
			{
				ADObjectId[] array2 = orgContainer.RemotePublicFolderMailboxes.ToArray();
				foreach (ADObjectId adobjectId in array2)
				{
					if (!adobjectId.IsDeleted)
					{
						MiniRecipient miniRecipient = tenantOrRootOrgRecipientSession.ReadMiniRecipient(adobjectId, null);
						if (miniRecipient != null)
						{
							PublicFolderRecipient publicFolderRecipient = new PublicFolderRecipient(miniRecipient.Name, Guid.Empty, null, miniRecipient.PrimarySmtpAddress, miniRecipient.Id, false);
							this.estimatedSize += publicFolderRecipient.ItemSize;
							this.remotePublicFolderRecipients.Add(publicFolderRecipient.ObjectId, publicFolderRecipient);
							list.Add(publicFolderRecipient);
						}
					}
				}
				if (list.Count > 0)
				{
					this.consistentHashSetForRemoteMailboxes = new ConsistentHashSet<PublicFolderRecipient, Guid>(list.ToArray(), 1, 64);
					this.estimatedSize += this.consistentHashSetForRemoteMailboxes.ItemSize;
				}
			}
			list.Clear();
			if (array.Length > 0)
			{
				for (int j = 0; j < array.Length; j++)
				{
					PublicFolderRecipient publicFolderRecipient = new PublicFolderRecipient((string)array[j][ADRecipientSchema.DisplayName], (Guid)array[j][ADMailboxRecipientSchema.ExchangeGuid], (ADObjectId)array[j][ADMailboxRecipientSchema.Database], (SmtpAddress)array[j][ADRecipientSchema.PrimarySmtpAddress], (ADObjectId)array[j][ADObjectSchema.Id], true);
					this.estimatedSize += publicFolderRecipient.ItemSize;
					this.localPublicFolderRecipients.Add(publicFolderRecipient.ObjectId, publicFolderRecipient);
					if (!(bool)array[j][ADRecipientSchema.IsExcludedFromServingHierarchy] && (bool)array[j][ADRecipientSchema.IsHierarchyReady])
					{
						list.Add(publicFolderRecipient);
					}
				}
				if (list.Count > 0)
				{
					this.consistentHashSetForLocalMailboxes = new ConsistentHashSet<PublicFolderRecipient, Guid>(list.ToArray(), 1, 64);
					this.estimatedSize += this.consistentHashSetForLocalMailboxes.ItemSize;
				}
			}
		}

		public PublicFolderRecipient GetLocalMailboxRecipient(Guid mailboxGuid)
		{
			if (mailboxGuid != Guid.Empty)
			{
				foreach (object obj in this.localPublicFolderRecipients)
				{
					PublicFolderRecipient publicFolderRecipient = (PublicFolderRecipient)((DictionaryEntry)obj).Value;
					if (publicFolderRecipient.MailboxGuid == mailboxGuid)
					{
						return publicFolderRecipient;
					}
				}
			}
			return null;
		}

		public PublicFolderRecipient GetPublicFolderRecipient(Guid actAsUserMailboxGuid, ADObjectId publicFolderMailboxId)
		{
			if (this.localPublicFolderRecipients.Count == 0 && this.remotePublicFolderRecipients.Count == 0)
			{
				return null;
			}
			if (publicFolderMailboxId != null)
			{
				PublicFolderRecipient publicFolderRecipient = this.GetPublicFolderRecipient(publicFolderMailboxId);
				if (publicFolderRecipient != null)
				{
					return publicFolderRecipient;
				}
			}
			if (this.PublicFoldersDeploymentType == PublicFoldersDeployment.Local)
			{
				if (this.consistentHashSetForLocalMailboxes != null)
				{
					return this.GetPublicFolderRecipient(this.consistentHashSetForLocalMailboxes.GetNearestNeighborSlot(actAsUserMailboxGuid).ObjectId);
				}
			}
			else if (this.PublicFoldersDeploymentType == PublicFoldersDeployment.Remote && this.consistentHashSetForRemoteMailboxes != null)
			{
				return this.GetPublicFolderRecipient(this.consistentHashSetForRemoteMailboxes.GetNearestNeighborSlot(actAsUserMailboxGuid).ObjectId);
			}
			return null;
		}

		public PublicFolderRecipient[] GetAllMailboxRecipients()
		{
			PublicFolderRecipient[] array = new PublicFolderRecipient[this.localPublicFolderRecipients.Count];
			this.localPublicFolderRecipients.Values.CopyTo(array, 0);
			return array;
		}

		public PublicFolderInformation GetHierarchyMailboxInformation()
		{
			return this.hierarchyMailboxInformation;
		}

		public Guid GetHierarchyMailboxGuidForUser(Guid actAsUserMailboxGuid, ADObjectId publicFolderMailboxId)
		{
			if (this.hierarchyMailboxInformation.Type != PublicFolderInformation.HierarchyType.MailboxGuid || this.hierarchyMailboxInformation.HierarchyMailboxGuid == Guid.Empty)
			{
				return Guid.Empty;
			}
			PublicFolderRecipient publicFolderRecipient = this.GetPublicFolderRecipient(actAsUserMailboxGuid, publicFolderMailboxId);
			if (publicFolderRecipient != null && publicFolderRecipient.IsLocal)
			{
				return publicFolderRecipient.MailboxGuid;
			}
			return Guid.Empty;
		}

		public Guid[] GetContentMailboxGuids()
		{
			List<Guid> list = new List<Guid>();
			if (this.localPublicFolderRecipients.Count > 1)
			{
				foreach (object obj in this.localPublicFolderRecipients)
				{
					PublicFolderRecipient publicFolderRecipient = (PublicFolderRecipient)((DictionaryEntry)obj).Value;
					if (publicFolderRecipient.MailboxGuid != this.hierarchyMailboxInformation.HierarchyMailboxGuid)
					{
						list.Add(publicFolderRecipient.MailboxGuid);
					}
				}
			}
			return list.ToArray();
		}

		private PublicFolderRecipient GetPublicFolderRecipient(ADObjectId objectId)
		{
			return (PublicFolderRecipient)(this.localPublicFolderRecipients[objectId] ?? this.remotePublicFolderRecipients[objectId]);
		}

		private const int consistentHashingReplicaCount = 1;

		private const int numberOfNeighborVisit = 64;

		private static readonly PropertyDefinition[] PublicFolderRecipientProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.IsExcludedFromServingHierarchy,
			ADRecipientSchema.IsHierarchyReady,
			ADRecipientSchema.PrimarySmtpAddress,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADMailboxRecipientSchema.Database
		};

		private PublicFolderInformation hierarchyMailboxInformation;

		private Hashtable localPublicFolderRecipients;

		private Hashtable remotePublicFolderRecipients;

		private ConsistentHashSet<PublicFolderRecipient, Guid> consistentHashSetForLocalMailboxes;

		private ConsistentHashSet<PublicFolderRecipient, Guid> consistentHashSetForRemoteMailboxes;

		private long estimatedSize;

		private PublicFoldersDeployment publicFoldersDeploymentType;
	}
}
