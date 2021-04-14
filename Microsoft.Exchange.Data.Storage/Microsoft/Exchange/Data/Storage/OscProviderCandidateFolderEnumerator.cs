using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscProviderCandidateFolderEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		internal OscProviderCandidateFolderEnumerator(IMailboxSession session, Guid provider, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			this.session = session;
			this.provider = provider;
			this.xsoFactory = xsoFactory;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			foreach (IStorePropertyBag candidate in this.EnumerateCandidatesThatMatchDefaultNamingConvention())
			{
				yield return candidate;
			}
			foreach (IStorePropertyBag candidate2 in this.EnumerateAllContactFolders())
			{
				yield return candidate2;
			}
			yield break;
		}

		private IEnumerable<IStorePropertyBag> EnumerateCandidatesThatMatchDefaultNamingConvention()
		{
			string defaultFolderDisplayName;
			if (!OscProviderRegistry.TryGetDefaultFolderDisplayName(this.provider, out defaultFolderDisplayName))
			{
				OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "Candidate folder enumerator: provider {0} is unknown.  Cannot enumerate candidates that match naming convention", this.provider);
			}
			else
			{
				TextFilter displayNameStartsWithProviderName = new TextFilter(FolderSchema.DisplayName, defaultFolderDisplayName, MatchOptions.Prefix, MatchFlags.IgnoreCase);
				OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Candidate folder enumerator: the default folder display name for provider {0} is {1}", this.provider, defaultFolderDisplayName);
				using (IFolder rootFolder = this.xsoFactory.BindToFolder(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Root)))
				{
					using (IQueryResult subFoldersQuery = rootFolder.IFolderQuery(FolderQueryFlags.None, null, OscProviderCandidateFolderEnumerator.SortByDisplayNameAscending, OscProviderCandidateFolderEnumerator.FolderPropertiesToLoad))
					{
						if (!subFoldersQuery.SeekToCondition(SeekReference.OriginBeginning, displayNameStartsWithProviderName, SeekToConditionFlags.AllowExtendedFilters))
						{
							OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Candidate folder enumerator: SeekToCondition = false.  No folder has display name matching {0}", defaultFolderDisplayName);
							yield break;
						}
						IStorePropertyBag[] folders = subFoldersQuery.GetPropertyBags(10);
						while (folders.Length > 0)
						{
							foreach (IStorePropertyBag folder in folders)
							{
								string displayName = folder.GetValueOrDefault<string>(FolderSchema.DisplayName, string.Empty);
								VersionedId folderId = folder.GetValueOrDefault<VersionedId>(FolderSchema.Id, null);
								if (folderId == null)
								{
									OscProviderCandidateFolderEnumerator.Tracer.TraceError<string>((long)this.GetHashCode(), "Candidate folder enumerator: skipping bogus folder '{0}' because it has a blank id.", displayName);
								}
								else
								{
									string containerClass = folder.GetValueOrDefault<string>(StoreObjectSchema.ContainerClass, string.Empty);
									if (string.IsNullOrEmpty(containerClass) || !ObjectClass.IsContactsFolder(containerClass))
									{
										OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "Candidate folder enumerator: skipping folder '{0}' (ID={1}) because it's not a contacts folder.", displayName, folderId);
									}
									else
									{
										if (string.IsNullOrEmpty(displayName) || !displayName.StartsWith(defaultFolderDisplayName, StringComparison.OrdinalIgnoreCase))
										{
											OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Candidate folder enumerator: we've iterated past candidates that follow the naming convention.  Current folder is '{0}'", displayName);
											yield break;
										}
										OscProviderCandidateFolderEnumerator.Tracer.TraceDebug<string, VersionedId>((long)this.GetHashCode(), "Candidate folder enumerator: folder: {0}; id: {1}; is a good candidate.", displayName, folderId);
										yield return folder;
									}
								}
							}
							folders = subFoldersQuery.GetPropertyBags(10);
						}
					}
				}
			}
			yield break;
		}

		private IEnumerable<IStorePropertyBag> EnumerateAllContactFolders()
		{
			return new ContactFoldersEnumerator(this.session, this.xsoFactory, ContactFoldersEnumeratorOptions.SkipHiddenFolders, new PropertyDefinition[0]);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private static readonly SortBy[] SortByDisplayNameAscending = new SortBy[]
		{
			new SortBy(FolderSchema.DisplayName, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] FolderPropertiesToLoad = new PropertyDefinition[]
		{
			StoreObjectSchema.ContainerClass,
			FolderSchema.DisplayName,
			FolderSchema.Id
		};

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;

		private readonly Guid provider;
	}
}
