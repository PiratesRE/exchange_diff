using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscSyncLockEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		internal OscSyncLockEnumerator(IMailboxSession session, StoreObjectId folder, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(folder, "folder");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			this.session = session;
			this.folder = folder;
			this.xsoFactory = xsoFactory;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, this.folder))
			{
				using (IQueryResult query = folder.IItemQuery(ItemQueryType.Associated, null, OscSyncLockEnumerator.SortByItemClassAscending, OscSyncLockEnumerator.PropertiesToLoadFromFAI))
				{
					if (!query.SeekToCondition(SeekReference.OriginBeginning, OscSyncLockEnumerator.ItemClassStartsWithOscSyncLockPrefix, SeekToConditionFlags.AllowExtendedFilters))
					{
						OscSyncLockEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock enumerator: no SyncLock in this folder.");
						yield break;
					}
					IStorePropertyBag[] messages = query.GetPropertyBags(50);
					while (messages.Length > 0)
					{
						foreach (IStorePropertyBag message in messages)
						{
							string itemClass = message.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
							if (string.IsNullOrEmpty(itemClass))
							{
								OscSyncLockEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock enumerator: skipping message with blank item class.");
							}
							else
							{
								if (!itemClass.StartsWith("IPM.Microsoft.OSC.SyncLock.", StringComparison.OrdinalIgnoreCase))
								{
									OscSyncLockEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock enumerator: no further SyncLocks in this folder.");
									yield break;
								}
								yield return message;
							}
						}
						messages = query.GetPropertyBags(50);
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private static readonly SortBy[] SortByItemClassAscending = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] PropertiesToLoadFromFAI = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			MessageItemSchema.OscSyncEnabledOnServer
		};

		private static readonly TextFilter ItemClassStartsWithOscSyncLockPrefix = new TextFilter(StoreObjectSchema.ItemClass, "IPM.Microsoft.OSC.SyncLock.", MatchOptions.Prefix, MatchFlags.IgnoreCase);

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;

		private readonly StoreObjectId folder;
	}
}
