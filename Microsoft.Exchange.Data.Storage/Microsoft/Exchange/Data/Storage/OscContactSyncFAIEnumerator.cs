using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscContactSyncFAIEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		internal OscContactSyncFAIEnumerator(IMailboxSession session, StoreObjectId folder, IXSOFactory xsoFactory)
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
				using (IQueryResult query = folder.IItemQuery(ItemQueryType.Associated, null, OscContactSyncFAIEnumerator.SortByItemClassAscending, OscContactSyncFAIEnumerator.PropertiesToLoadFromFAI))
				{
					if (!query.SeekToCondition(SeekReference.OriginBeginning, OscContactSyncFAIEnumerator.ItemClassEqualsContactSync))
					{
						OscContactSyncFAIEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "ContactSync FAI enumerator: SeekToCondition = false.  No ContactSync FAI exists in this folder.");
						yield break;
					}
					IStorePropertyBag[] messages = query.GetPropertyBags(10);
					while (messages.Length > 0)
					{
						foreach (IStorePropertyBag message in messages)
						{
							string itemClass = message.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
							if (string.IsNullOrEmpty(itemClass))
							{
								OscContactSyncFAIEnumerator.Tracer.TraceError((long)this.GetHashCode(), "ContactSync FAI enumerator: skipping message with blank item class.");
							}
							else
							{
								if (!ObjectClass.IsOfClass(itemClass, "IPM.Microsoft.OSC.ContactSync"))
								{
									OscContactSyncFAIEnumerator.Tracer.TraceDebug((long)this.GetHashCode(), "ContactSync FAI enumerator: no further ContactSync FAIs in this folder.");
									yield break;
								}
								yield return message;
							}
						}
						messages = query.GetPropertyBags(10);
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
			MessageItemSchema.OscContactSources,
			StoreObjectSchema.ItemClass
		};

		private static readonly ComparisonFilter ItemClassEqualsContactSync = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.Microsoft.OSC.ContactSync");

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;

		private readonly StoreObjectId folder;
	}
}
