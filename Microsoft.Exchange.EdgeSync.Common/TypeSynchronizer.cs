using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.EdgeSync
{
	internal class TypeSynchronizer
	{
		public TypeSynchronizer(Filter filterDelegate, PreDecorate preDecorateDelegate, PostDecorate postDecorateDelegate, LoadTargetCache loadTargetCacheDelegate, TargetCacheLookup targetCacheLookupDelegate, TargetCacheRemoveTargetOnlyEntries targetCacheCleanupDelegate, string name, string sourceQueryFilter, string targetQueryFilter, SearchScope searchScope, string[] copyAttributes, string[] filterAttributes, bool skipDeletedEntriesInFullSyncMode)
		{
			int num = 0;
			if (loadTargetCacheDelegate != null)
			{
				num++;
			}
			if (targetCacheLookupDelegate != null)
			{
				num++;
			}
			if (targetCacheCleanupDelegate != null)
			{
				num++;
			}
			if (num != 0 && num != 3)
			{
				throw new ArgumentException("loadTargetCacheDelegate, targetCacheLookupDelegate, and targetCacheCleanupDelegate must be all be set");
			}
			this.filterDelegate = filterDelegate;
			this.preDecorateDelegate = preDecorateDelegate;
			this.postDecorateDelegate = postDecorateDelegate;
			this.loadTargetCacheDelegate = loadTargetCacheDelegate;
			this.targetCacheLookupDelegate = targetCacheLookupDelegate;
			this.targetCacheCleanupDelegate = targetCacheCleanupDelegate;
			this.name = name;
			this.sourceQueryFilter = sourceQueryFilter;
			this.targetQueryFilter = targetQueryFilter;
			this.searchScope = searchScope;
			this.copyAttributes = copyAttributes;
			this.filterAttributes = filterAttributes;
			this.skipDeletedEntriesInFullSyncMode = skipDeletedEntriesInFullSyncMode;
			this.targetCache = new Dictionary<byte[], byte[]>(ArrayComparer<byte>.Comparer);
			this.targetCacheEnabled = false;
			this.targetCacheFullyLoaded = false;
			this.hasTargetCacheFullSyncError = false;
			if (loadTargetCacheDelegate != null)
			{
				this.targetCacheEnabled = true;
			}
			if (!this.skipDeletedEntriesInFullSyncMode && this.targetCacheEnabled)
			{
				throw new InvalidOperationException("Can't set skipDeletedEntriesInFullSyncMode to false when target cache is enabled");
			}
			int num2 = (copyAttributes == null) ? 0 : copyAttributes.Length;
			int num3 = (filterAttributes == null) ? 0 : filterAttributes.Length;
			this.readSourceAttributes = new string[num2 + num3 + TypeSynchronizer.srcDeltaAttributes.Length];
			if (num2 > 0)
			{
				copyAttributes.CopyTo(this.readSourceAttributes, 0);
			}
			if (num3 > 0)
			{
				filterAttributes.CopyTo(this.readSourceAttributes, num2);
			}
			TypeSynchronizer.srcDeltaAttributes.CopyTo(this.readSourceAttributes, num2 + num3);
			this.readTargetAttributes = new string[num2 + TypeSynchronizer.targetDeltaAttributes.Length];
			if (num2 > 0)
			{
				copyAttributes.CopyTo(this.readTargetAttributes, 0);
			}
			TypeSynchronizer.targetDeltaAttributes.CopyTo(this.readTargetAttributes, num2);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string SourceQueryFilter
		{
			get
			{
				return this.sourceQueryFilter;
			}
		}

		public string TargetQueryFilter
		{
			get
			{
				return this.targetQueryFilter;
			}
		}

		public string[] CopyAttributes
		{
			get
			{
				return this.copyAttributes;
			}
		}

		public string[] FilterAttributes
		{
			get
			{
				return this.filterAttributes;
			}
		}

		public bool SkipDeletedEntriesInFullSyncMode
		{
			get
			{
				return this.skipDeletedEntriesInFullSyncMode;
			}
		}

		public string[] ReadSourceAttributes
		{
			get
			{
				return this.readSourceAttributes;
			}
		}

		public string[] ReadTargetAttributes
		{
			get
			{
				return this.readTargetAttributes;
			}
		}

		public SearchScope SearchScope
		{
			get
			{
				return this.searchScope;
			}
		}

		public Dictionary<byte[], byte[]> TargetCache
		{
			get
			{
				return this.targetCache;
			}
		}

		public bool TargetCacheEnabled
		{
			get
			{
				return this.targetCacheEnabled;
			}
		}

		public bool TargetCacheFullyLoaded
		{
			get
			{
				return this.targetCacheFullyLoaded;
			}
			set
			{
				this.targetCacheFullyLoaded = value;
			}
		}

		public bool HasTargetCacheFullSyncError
		{
			get
			{
				return this.hasTargetCacheFullSyncError;
			}
			set
			{
				this.hasTargetCacheFullSyncError = value;
			}
		}

		public Filter Filter
		{
			get
			{
				return this.filterDelegate;
			}
		}

		public PreDecorate PreDecorate
		{
			get
			{
				return this.preDecorateDelegate;
			}
		}

		public PostDecorate PostDecorate
		{
			get
			{
				return this.postDecorateDelegate;
			}
		}

		public LoadTargetCache LoadTargetCache
		{
			get
			{
				return this.loadTargetCacheDelegate;
			}
		}

		public TargetCacheLookup TargetCacheLookup
		{
			get
			{
				return this.targetCacheLookupDelegate;
			}
		}

		public TargetCacheRemoveTargetOnlyEntries TargetCacheRemoveTargetOnlyEntries
		{
			get
			{
				return this.targetCacheCleanupDelegate;
			}
		}

		public void ResetTargetCacheState(bool hasTargetCacheFullSyncError)
		{
			this.targetCache.Clear();
			this.targetCacheFullyLoaded = false;
			this.hasTargetCacheFullSyncError = hasTargetCacheFullSyncError;
		}

		private static readonly string[] srcDeltaAttributes = new string[]
		{
			"objectGUID",
			"objectClass",
			"whenCreated",
			"parentGUID",
			"name"
		};

		private static readonly string[] targetDeltaAttributes = new string[]
		{
			"msExchEdgeSyncSourceGuid",
			"name"
		};

		private readonly bool skipDeletedEntriesInFullSyncMode;

		private readonly string name;

		private string sourceQueryFilter;

		private string targetQueryFilter;

		private string[] copyAttributes;

		private string[] filterAttributes;

		private string[] readSourceAttributes;

		private string[] readTargetAttributes;

		private SearchScope searchScope;

		private Dictionary<byte[], byte[]> targetCache;

		private bool targetCacheEnabled;

		private bool targetCacheFullyLoaded;

		private bool hasTargetCacheFullSyncError;

		private Filter filterDelegate;

		private PreDecorate preDecorateDelegate;

		private PostDecorate postDecorateDelegate;

		private LoadTargetCache loadTargetCacheDelegate;

		private TargetCacheLookup targetCacheLookupDelegate;

		private TargetCacheRemoveTargetOnlyEntries targetCacheCleanupDelegate;
	}
}
