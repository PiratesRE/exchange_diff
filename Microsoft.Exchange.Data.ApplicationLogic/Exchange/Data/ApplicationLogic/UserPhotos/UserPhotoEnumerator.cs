using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class UserPhotoEnumerator : IEnumerable<IStorePropertyBag>, IEnumerable
	{
		internal UserPhotoEnumerator(IMailboxSession session, StoreObjectId folder, string photoItemClass, IXSOFactory xsoFactory, ITracer upstreamTracer)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (folder == null)
			{
				throw new ArgumentNullException("folder");
			}
			if (string.IsNullOrEmpty(photoItemClass))
			{
				throw new ArgumentNullException("photoItemClass");
			}
			if (xsoFactory == null)
			{
				throw new ArgumentNullException("xsoFactory");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.session = session;
			this.folder = folder;
			this.photoItemClass = photoItemClass;
			this.filter = this.GetFilterForItemClass(photoItemClass);
			this.xsoFactory = xsoFactory;
		}

		public IEnumerator<IStorePropertyBag> GetEnumerator()
		{
			using (IFolder folder = this.xsoFactory.BindToFolder(this.session, this.folder))
			{
				using (IQueryResult query = folder.IItemQuery(ItemQueryType.None, null, UserPhotoEnumerator.SortByItemClassAscending, UserPhotoEnumerator.PropertiesToLoad))
				{
					if (!query.SeekToCondition(SeekReference.OriginBeginning, this.filter))
					{
						this.tracer.TraceDebug((long)this.GetHashCode(), "UserPhoto enumerator: no photos in this folder.");
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
								this.tracer.TraceDebug((long)this.GetHashCode(), "UserPhoto enumerator: skipping message with blank item class.");
							}
							else
							{
								if (!itemClass.Equals(this.photoItemClass, StringComparison.OrdinalIgnoreCase))
								{
									this.tracer.TraceDebug((long)this.GetHashCode(), "UserPhoto enumerator: no further photos in this folder.");
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

		private QueryFilter GetFilterForItemClass(string itemClass)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, itemClass);
		}

		private static readonly SortBy[] SortByItemClassAscending = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] PropertiesToLoad = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly StoreObjectId folder;

		private readonly string photoItemClass;

		private readonly QueryFilter filter;
	}
}
