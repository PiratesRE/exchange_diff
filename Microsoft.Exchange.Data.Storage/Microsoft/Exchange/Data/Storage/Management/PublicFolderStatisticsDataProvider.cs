using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderStatisticsDataProvider : DisposeTrackableBase, IConfigDataProvider
	{
		public PublicFolderDataProvider PublicFolderDataProvider
		{
			get
			{
				return this.publicFolderDataProvider;
			}
		}

		public OrganizationId CurrentOrganizationId
		{
			get
			{
				if (this.PublicFolderDataProvider == null)
				{
					return null;
				}
				return this.PublicFolderDataProvider.CurrentOrganizationId;
			}
		}

		public PublicFolderStatisticsDataProvider(IConfigurationSession configurationSession, string action, Guid mailboxGuid)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.publicFolderDataProvider = new PublicFolderDataProvider(configurationSession, action, mailboxGuid);
				this.mailboxGuid = mailboxGuid;
				disposeGuard.Success();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderStatisticsDataProvider>(this);
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			IConfigurable[] array = this.Find<T>(new FalseFilter(), identity, true, null);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			return (IConfigurable[])this.FindPaged<T>(filter, rootId, deepSearch, sortBy, 0).ToArray<T>();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			base.CheckDisposed();
			if (!typeof(PublicFolderStatistics).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
			{
				throw new NotSupportedException("FindPaged: " + typeof(T).FullName);
			}
			IEnumerable<PublicFolder> publicFolders = this.publicFolderDataProvider.FindPaged<PublicFolder>(filter, rootId, deepSearch, sortBy, pageSize);
			foreach (PublicFolder publicFolder in publicFolders)
			{
				PublicFolderSession contentSession = null;
				if (this.mailboxGuid == Guid.Empty)
				{
					contentSession = this.PublicFolderDataProvider.PublicFolderSessionCache.GetPublicFolderSession(publicFolder.InternalFolderIdentity.ObjectId);
				}
				else
				{
					contentSession = this.PublicFolderDataProvider.PublicFolderSessionCache.GetPublicFolderSession(this.mailboxGuid);
				}
				using (Folder contentFolder = Folder.Bind(contentSession, publicFolder.InternalFolderIdentity, PublicFolderStatisticsDataProvider.contentFolderProperties))
				{
					PublicFolderStatistics publicFolderStatistics = new PublicFolderStatistics();
					publicFolderStatistics.LoadDataFromXso(contentSession.MailboxPrincipal.ObjectId, contentFolder);
					uint ownerCount = 0U;
					uint contactCount = 0U;
					PermissionSet folderPermissionSet = contentFolder.GetPermissionSet();
					foreach (Permission permission in folderPermissionSet)
					{
						if (permission.IsFolderContact)
						{
							contactCount += 1U;
						}
						if (permission.IsFolderOwner)
						{
							ownerCount += 1U;
						}
					}
					publicFolderStatistics.OwnerCount = ownerCount;
					publicFolderStatistics.ContactCount = contactCount;
					StoreObjectId dumpsterId = PublicFolderCOWSession.GetRecoverableItemsDeletionsFolderId((CoreFolder)contentFolder.CoreObject);
					checked
					{
						if (dumpsterId != null)
						{
							try
							{
								using (CoreFolder coreFolder = CoreFolder.Bind(contentSession, dumpsterId, PublicFolderStatisticsDataProvider.dumpsterProperties))
								{
									publicFolderStatistics.DeletedItemCount = (uint)((int)coreFolder.PropertyBag[FolderSchema.ItemCount]);
									publicFolderStatistics.TotalDeletedItemSize = ByteQuantifiedSize.FromBytes((ulong)((long)coreFolder.PropertyBag[FolderSchema.ExtendedSize]));
								}
							}
							catch (ObjectNotFoundException)
							{
							}
						}
						yield return (T)((object)publicFolderStatistics);
					}
				}
			}
			yield break;
		}

		public void Save(IConfigurable instance)
		{
			throw new NotImplementedException();
		}

		public void Delete(IConfigurable instance)
		{
			throw new NotImplementedException();
		}

		public string Source
		{
			get
			{
				return ((IConfigDataProvider)this.publicFolderDataProvider).Source;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.publicFolderDataProvider != null)
			{
				this.publicFolderDataProvider.Dispose();
				this.publicFolderDataProvider = null;
			}
		}

		private readonly Guid mailboxGuid = Guid.Empty;

		private static ICollection<PropertyDefinition> contentFolderProperties = InternalSchema.Combine<PropertyDefinition>(FolderSchema.Instance.AutoloadProperties, PublicFolderStatistics.InternalSchema.AllDependentXsoProperties);

		private static StorePropertyDefinition[] dumpsterProperties = new StorePropertyDefinition[]
		{
			FolderSchema.ItemCount,
			FolderSchema.ExtendedSize
		};

		private PublicFolderDataProvider publicFolderDataProvider;
	}
}
