using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContactFoldersEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		public ContactFoldersEnumerator(IMailboxSession session) : this(session, new XSOFactory())
		{
		}

		public ContactFoldersEnumerator(IMailboxSession session, ContactFoldersEnumeratorOptions enumerateOptions) : this(session, new XSOFactory(), enumerateOptions, new PropertyDefinition[0])
		{
		}

		public ContactFoldersEnumerator(IMailboxSession session, IXSOFactory xsoFactory) : this(session, xsoFactory, ContactFoldersEnumeratorOptions.None, new PropertyDefinition[0])
		{
		}

		public ContactFoldersEnumerator(IMailboxSession session, IXSOFactory xsoFactory, ContactFoldersEnumeratorOptions enumerateOptions, params PropertyDefinition[] additionalProperties) : this(session, xsoFactory, DefaultFolderType.Root, enumerateOptions, additionalProperties)
		{
		}

		public ContactFoldersEnumerator(IMailboxSession session, IXSOFactory xsoFactory, DefaultFolderType parentFolderScope, ContactFoldersEnumeratorOptions enumerateOptions, params PropertyDefinition[] additionalProperties)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			EnumValidator.ThrowIfInvalid<ContactFoldersEnumeratorOptions>(enumerateOptions, "enumerateOptions");
			this.session = session;
			this.xsoFactory = xsoFactory;
			this.enumerateOptions = enumerateOptions;
			this.additionalProperties = additionalProperties;
			this.parentFolderScope = ((parentFolderScope != DefaultFolderType.None) ? parentFolderScope : DefaultFolderType.Root);
		}

		private bool ShouldIncludeParentFolder
		{
			get
			{
				return (this.enumerateOptions & ContactFoldersEnumeratorOptions.IncludeParentFolder) != ContactFoldersEnumeratorOptions.None;
			}
		}

		private bool ShouldSkipHiddenFolders
		{
			get
			{
				return (this.enumerateOptions & ContactFoldersEnumeratorOptions.SkipHiddenFolders) != ContactFoldersEnumeratorOptions.None;
			}
		}

		private bool ShouldSkipDeletedFolders
		{
			get
			{
				return (this.enumerateOptions & ContactFoldersEnumeratorOptions.SkipDeletedFolders) != ContactFoldersEnumeratorOptions.None;
			}
		}

		private StoreObjectId DeletedItemsFolderId
		{
			get
			{
				if (this.deletedItemsFolderId == null)
				{
					this.deletedItemsFolderId = this.session.GetDefaultFolderId(DefaultFolderType.DeletedItems);
				}
				return this.deletedItemsFolderId;
			}
		}

		private PropertyDefinition[] FolderPropertiesToBeLoaded
		{
			get
			{
				PropertyDefinition[] result;
				if (this.additionalProperties != null && this.additionalProperties.Length > 0)
				{
					result = PropertyDefinitionCollection.Merge<PropertyDefinition>(ContactFoldersEnumerator.DefaultFolderProperties, this.additionalProperties);
				}
				else
				{
					result = ContactFoldersEnumerator.DefaultFolderProperties;
				}
				return result;
			}
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			ContactFoldersEnumerator.DeletedItemsFolderEnumerationState deletedItemsFolderEnumerationState = new ContactFoldersEnumerator.DeletedItemsFolderEnumerationState();
			using (IFolder rootFolder = this.xsoFactory.BindToFolder(this.session, this.session.GetDefaultFolderId(this.parentFolderScope)))
			{
				if (this.ShouldIncludeParentFolder)
				{
					rootFolder.Load(this.FolderPropertiesToBeLoaded);
					if (this.ShouldEnumerateFolder(rootFolder, deletedItemsFolderEnumerationState))
					{
						yield return rootFolder;
					}
				}
				using (IQueryResult subFoldersQuery = rootFolder.IFolderQuery(FolderQueryFlags.DeepTraversal, null, null, this.FolderPropertiesToBeLoaded))
				{
					IStorePropertyBag[] folders = subFoldersQuery.GetPropertyBags(100);
					while (folders.Length > 0)
					{
						foreach (IStorePropertyBag folder in folders)
						{
							if (this.ShouldEnumerateFolder(folder, deletedItemsFolderEnumerationState))
							{
								yield return folder;
							}
						}
						folders = subFoldersQuery.GetPropertyBags(100);
					}
				}
			}
			yield break;
		}

		private bool ShouldEnumerateFolder(IStorePropertyBag folder, ContactFoldersEnumerator.DeletedItemsFolderEnumerationState deletedItemsFolderEnumerationState)
		{
			object obj = folder.TryGetProperty(FolderSchema.Id);
			object obj2 = folder.TryGetProperty(StoreObjectSchema.ContainerClass);
			string valueOrDefault = folder.GetValueOrDefault<string>(FolderSchema.DisplayName, string.Empty);
			if (obj is PropertyError || obj2 is PropertyError)
			{
				ContactFoldersEnumerator.Tracer.TraceDebug<string, object, object>((long)this.GetHashCode(), "Skiping bogus folder (DisplayName:{0}) without ID ({1}) or container class ({2})", valueOrDefault, obj, obj2);
				return false;
			}
			if (this.ShouldSkipDeletedFolders && this.IsDeletedFolder(folder, deletedItemsFolderEnumerationState))
			{
				ContactFoldersEnumerator.Tracer.TraceDebug<object, string>((long)this.GetHashCode(), "Skiping deleted folder - ID:{0}, DisplayName:{1}.", obj, valueOrDefault);
				return false;
			}
			if (!ObjectClass.IsContactsFolder((string)obj2))
			{
				ContactFoldersEnumerator.Tracer.TraceDebug<object, object, string>((long)this.GetHashCode(), "Skiping non-contact folder - ID:{0}, ContainerClass:{1}, DisplayName:{2}.", obj, obj2, valueOrDefault);
				return false;
			}
			if (this.ShouldSkipHiddenFolders && folder.TryGetProperty(FolderSchema.IsHidden) is bool && (bool)folder.TryGetProperty(FolderSchema.IsHidden))
			{
				ContactFoldersEnumerator.Tracer.TraceDebug<object, string>((long)this.GetHashCode(), "Skiping hidden folder - ID:{0}, DisplayName:{1}.", obj, valueOrDefault);
				return false;
			}
			ContactFoldersEnumerator.Tracer.TraceDebug<object, string>((long)this.GetHashCode(), "Enumerating folder - ID:{0}, DisplayName:{1}.", obj, valueOrDefault);
			return true;
		}

		private bool IsDeletedFolder(IStorePropertyBag folder, ContactFoldersEnumerator.DeletedItemsFolderEnumerationState deletedItemsFolderEnumerationState)
		{
			if (deletedItemsFolderEnumerationState.IsAlreadyEnumerated)
			{
				return false;
			}
			object obj = folder.TryGetProperty(FolderSchema.FolderHierarchyDepth);
			if (!(obj is int) || (int)obj < 0)
			{
				return false;
			}
			int num = (int)obj;
			StoreObjectId objectId = ((VersionedId)folder.TryGetProperty(FolderSchema.Id)).ObjectId;
			if (deletedItemsFolderEnumerationState.NotEnumeratedYet)
			{
				if (objectId.Equals(this.DeletedItemsFolderId))
				{
					deletedItemsFolderEnumerationState.MarkDeletedItemsFolderEncountered(num);
				}
				return false;
			}
			if (num == deletedItemsFolderEnumerationState.DeletedItemsFolderDepth)
			{
				deletedItemsFolderEnumerationState.MarkDeletedItemsFolderEnumerationDone();
				return false;
			}
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private static readonly Trace Tracer = ExTraceGlobals.ContactFoldersEnumeratorTracer;

		private static readonly PropertyDefinition[] DefaultFolderProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.ContainerClass,
			FolderSchema.DisplayName,
			FolderSchema.Id,
			FolderSchema.IsHidden,
			FolderSchema.FolderHierarchyDepth
		};

		private readonly IMailboxSession session;

		private readonly IXSOFactory xsoFactory;

		private readonly ContactFoldersEnumeratorOptions enumerateOptions;

		private readonly DefaultFolderType parentFolderScope;

		private readonly PropertyDefinition[] additionalProperties;

		private StoreObjectId deletedItemsFolderId;

		private sealed class DeletedItemsFolderEnumerationState
		{
			public DeletedItemsFolderEnumerationState()
			{
				this.currentPhase = ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.NotEnumeratedYet;
			}

			public bool IsAlreadyEnumerated
			{
				get
				{
					return this.currentPhase == ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.EnumerationDone;
				}
			}

			public bool NotEnumeratedYet
			{
				get
				{
					return this.currentPhase == ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.NotEnumeratedYet;
				}
			}

			public bool IsUnderEnumeration
			{
				get
				{
					return this.currentPhase == ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.UnderEnumeration;
				}
			}

			public int DeletedItemsFolderDepth
			{
				get
				{
					this.AssertCurrentPhase(ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.UnderEnumeration);
					return this.deletedItemsFolderDepth.Value;
				}
			}

			public void MarkDeletedItemsFolderEncountered(int folderHierarchyDepth)
			{
				Util.ThrowOnArgumentOutOfRangeOnLessThan(folderHierarchyDepth, 0, "folderHierarchyDepth");
				this.AssertCurrentPhase(ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.NotEnumeratedYet);
				this.deletedItemsFolderDepth = new int?(folderHierarchyDepth);
				this.currentPhase = ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.UnderEnumeration;
			}

			public void MarkDeletedItemsFolderEnumerationDone()
			{
				this.AssertCurrentPhase(ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.UnderEnumeration);
				this.currentPhase = ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase.EnumerationDone;
			}

			private void AssertCurrentPhase(ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase phaseTobeAsserted)
			{
			}

			private ContactFoldersEnumerator.DeletedItemsFolderEnumerationState.EnumerationPhase currentPhase;

			private int? deletedItemsFolderDepth;

			private enum EnumerationPhase
			{
				NotEnumeratedYet,
				UnderEnumeration,
				EnumerationDone
			}
		}
	}
}
