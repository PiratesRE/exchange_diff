using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointDocumentLibrary : SharepointList, IDocumentLibrary, IReadOnlyPropertyBag
	{
		internal SharepointDocumentLibrary(SharepointListId siteId, SharepointSession session, XmlNode dataCache) : base(siteId, session, dataCache)
		{
		}

		public new static SharepointDocumentLibrary Read(SharepointSession session, ObjectId listId)
		{
			if (listId == null)
			{
				throw new ArgumentNullException("listId");
			}
			SharepointListId sharepointListId = listId as SharepointListId;
			if (sharepointListId == null)
			{
				throw new ArgumentException("listId");
			}
			if (sharepointListId.UriFlags != UriFlags.SharepointDocumentLibrary)
			{
				throw new ArgumentNullException("listId");
			}
			return (SharepointDocumentLibrary)SharepointList.Read(session, sharepointListId);
		}

		ObjectId IDocumentLibrary.Id
		{
			get
			{
				return base.Id;
			}
		}

		string IDocumentLibrary.Title
		{
			get
			{
				return this.Title;
			}
		}

		public string Description
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointListSchema.Description);
			}
		}

		public List<KeyValuePair<string, Uri>> GetHierarchy()
		{
			SharepointListId sharepointListId = base.Id as SharepointListId;
			List<KeyValuePair<string, Uri>> list = new List<KeyValuePair<string, Uri>>(1);
			if (sharepointListId.SiteUri.Segments.Length > 1)
			{
				list.Add(new KeyValuePair<string, Uri>(sharepointListId.SiteUri.Segments[sharepointListId.SiteUri.Segments.Length - 1].TrimEnd(new char[]
				{
					'/',
					'\\'
				}), sharepointListId.SiteUri));
			}
			else
			{
				list.Add(new KeyValuePair<string, Uri>(sharepointListId.SiteUri.Host, sharepointListId.SiteUri));
			}
			return list;
		}

		IDocumentLibraryItem IDocumentLibrary.Read(ObjectId objectId, params PropertyDefinition[] propsToReturn)
		{
			return this.Read(objectId, propsToReturn);
		}

		public SharepointDocumentLibraryItem Read(ObjectId objectId, params PropertyDefinition[] propsToReturn)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			SharepointItemId sharepointItemId = objectId as SharepointItemId;
			SharepointListId sharepointListId = base.Id as SharepointListId;
			if (sharepointItemId == null)
			{
				throw new ArgumentException("objectId as SharepointItemId");
			}
			if (sharepointItemId.ListName != sharepointListId.ListName || sharepointItemId.SiteUri != sharepointListId.SiteUri)
			{
				throw new ObjectNotFoundException(objectId, Strings.ExObjectNotFound(objectId.ToString()));
			}
			return SharepointDocumentLibraryItem.Read(this.Session, objectId);
		}

		public ITableView GetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, params PropertyDefinition[] propsToReturn)
		{
			return SharepointDocumentLibraryFolder.InternalGetView(query, sortBy, queryOptions, propsToReturn, this.Session, (SharepointListId)base.Id);
		}

		public override SharepointItemType ItemType
		{
			get
			{
				return SharepointItemType.DocumentLibrary;
			}
		}
	}
}
