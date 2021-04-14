using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data.DocumentLibrary.SharepointService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SharepointDocumentLibraryItem : SharepointObject, IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal SharepointDocumentLibraryItem(SharepointDocumentLibraryItemId id, SharepointSession session, XmlNode dataCache, Schema schema) : base(id, session, dataCache, schema)
		{
			this.CultureInfo = id.CultureInfo;
		}

		public new static SharepointDocumentLibraryItem Read(SharepointSession session, ObjectId objectId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			SharepointDocumentLibraryItemId itemId = objectId as SharepointDocumentLibraryItemId;
			if (itemId == null)
			{
				throw new ArgumentException("objectId");
			}
			if (itemId.UriFlags != UriFlags.SharepointDocument && itemId.UriFlags != UriFlags.SharepointFolder)
			{
				throw new ArgumentException("objectId");
			}
			if (session.Uri != itemId.SiteUri)
			{
				throw new ObjectNotFoundException(itemId, Strings.ExObjectMovedOrDeleted(itemId.ToString()));
			}
			if (itemId.Cache != null && itemId.Cache.Value.Key == session.Identity.Name)
			{
				if (itemId.UriFlags == UriFlags.SharepointFolder)
				{
					return new SharepointDocumentLibraryFolder(itemId, session, itemId.Cache.Value.Value);
				}
				if (itemId.UriFlags == UriFlags.SharepointDocument)
				{
					return new SharepointDocument(itemId, session, itemId.Cache.Value.Value);
				}
			}
			return Utils.DoSharepointTask<SharepointDocumentLibraryItem>(session.Identity, itemId, itemId, false, Utils.MethodType.Read, delegate
			{
				XmlNode nodeForItem = SharepointDocumentLibraryItem.GetNodeForItem(session, itemId);
				if (nodeForItem != null)
				{
					SharepointList sharepointList = SharepointList.Read(session, new SharepointListId(itemId.ListName, itemId.SiteUri, null, UriFlags.SharepointDocumentLibrary));
					itemId.Cache = new KeyValuePair<string, XmlNode>?(new KeyValuePair<string, XmlNode>(session.Identity.Name, nodeForItem));
					itemId.CultureInfo = sharepointList.GetRegionalSettings();
					if (itemId.UriFlags == UriFlags.SharepointFolder)
					{
						return new SharepointDocumentLibraryFolder(itemId, session, itemId.Cache.Value.Value);
					}
					if (itemId.UriFlags == UriFlags.SharepointDocument)
					{
						return new SharepointDocument(itemId, session, itemId.Cache.Value.Value);
					}
				}
				throw new ObjectNotFoundException(itemId, Strings.ExObjectNotFound(itemId.ToString()));
			});
		}

		public string DisplayName
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.Name);
			}
		}

		public override Uri Uri
		{
			get
			{
				return base.GetValueOrDefault<Uri>(SharepointDocumentLibraryItemSchema.EncodedAbsoluteUri);
			}
		}

		ObjectId IDocumentLibraryItem.Id
		{
			get
			{
				return base.Id;
			}
		}

		public bool IsFolder
		{
			get
			{
				return (bool)base[SharepointDocumentLibraryItemSchema.FileSystemObjectType];
			}
		}

		IDocumentLibraryFolder IDocumentLibraryItem.Parent
		{
			get
			{
				return this.ParentFolder;
			}
		}

		IDocumentLibrary IDocumentLibraryItem.Library
		{
			get
			{
				return this.Library;
			}
		}

		public override string Title
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.Name);
			}
		}

		public List<KeyValuePair<string, Uri>> GetHierarchy()
		{
			SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = base.Id as SharepointDocumentLibraryItemId;
			List<KeyValuePair<string, Uri>> list = new List<KeyValuePair<string, Uri>>(sharepointDocumentLibraryItemId.ItemHierarchy.Count);
			if (sharepointDocumentLibraryItemId.SiteUri.Segments.Length > 1)
			{
				list.Add(new KeyValuePair<string, Uri>(sharepointDocumentLibraryItemId.SiteUri.Segments[sharepointDocumentLibraryItemId.SiteUri.Segments.Length - 1], sharepointDocumentLibraryItemId.SiteUri));
			}
			else
			{
				list.Add(new KeyValuePair<string, Uri>(sharepointDocumentLibraryItemId.SiteUri.Host, sharepointDocumentLibraryItemId.SiteUri));
			}
			UriBuilder uriBuilder = new UriBuilder(sharepointDocumentLibraryItemId.SiteUri);
			for (int i = 0; i < sharepointDocumentLibraryItemId.ItemHierarchy.Count - 1; i++)
			{
				uriBuilder.Path = uriBuilder.Path + "/" + sharepointDocumentLibraryItemId.ItemHierarchy[i];
				list.Add(new KeyValuePair<string, Uri>(sharepointDocumentLibraryItemId.ItemHierarchy[i], uriBuilder.Uri));
			}
			return list;
		}

		public ExDateTime? LastModified
		{
			get
			{
				object valueOrDefault = base.GetValueOrDefault<object>(SharepointDocumentLibraryItemSchema.LastModifiedTime);
				if (valueOrDefault != null)
				{
					return new ExDateTime?((ExDateTime)valueOrDefault);
				}
				return null;
			}
		}

		public string Editor
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.Editor);
			}
		}

		public ExDateTime? CreatedDate
		{
			get
			{
				object valueOrDefault = base.GetValueOrDefault<object>(SharepointDocumentLibraryItemSchema.CreationTime);
				if (valueOrDefault != null)
				{
					return new ExDateTime?((ExDateTime)valueOrDefault);
				}
				return null;
			}
		}

		public string DocIcon
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.DocIcon);
			}
		}

		public string ModifiedBy
		{
			get
			{
				return this.Editor;
			}
		}

		public string CreatedBy
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.Author);
			}
		}

		public string ServerUri
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.ServerUri);
			}
		}

		public string BaseName
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.BaseName);
			}
		}

		public SharepointDocumentLibraryFolder ParentFolder
		{
			get
			{
				if (this.parent != null)
				{
					return this.parent;
				}
				SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = base.Id as SharepointDocumentLibraryItemId;
				if (sharepointDocumentLibraryItemId.ItemHierarchy.Count > 2)
				{
					List<string> list = new List<string>(sharepointDocumentLibraryItemId.ItemHierarchy);
					list.RemoveAt(list.Count - 1);
					string propertyValue = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					SharepointListId listId;
					if (list.Count == 1)
					{
						listId = new SharepointListId(sharepointDocumentLibraryItemId.ListName, sharepointDocumentLibraryItemId.SiteUri, sharepointDocumentLibraryItemId.CultureInfo, UriFlags.SharepointDocumentLibrary);
					}
					else
					{
						listId = new SharepointDocumentLibraryItemId("-1", sharepointDocumentLibraryItemId.ListName, sharepointDocumentLibraryItemId.SiteUri, sharepointDocumentLibraryItemId.CultureInfo, UriFlags.SharepointFolder, list);
					}
					PropertyDefinition[] propsToReturn = new PropertyDefinition[]
					{
						SharepointDocumentLibraryItemSchema.ID
					};
					ITableView tableView = SharepointDocumentLibraryFolder.InternalGetView(new ComparisonFilter(ComparisonOperator.Equal, SharepointDocumentLibraryItemSchema.Name, propertyValue), null, DocumentLibraryQueryOptions.FoldersAndFiles, propsToReturn, this.Session, listId);
					if (tableView.EstimatedRowCount == 1)
					{
						this.parent = SharepointDocumentLibraryFolder.Read(this.Session, (ObjectId)tableView.GetRows(1)[0][0]);
					}
				}
				return this.parent;
			}
		}

		public SharepointDocumentLibrary Library
		{
			get
			{
				if (this.documentLibrary == null)
				{
					SharepointItemId sharepointItemId = this.SharepointId as SharepointItemId;
					SharepointListId listId = new SharepointListId(sharepointItemId.ListName, sharepointItemId.SiteUri, sharepointItemId.CultureInfo, UriFlags.SharepointDocumentLibrary);
					this.documentLibrary = (SharepointDocumentLibrary)SharepointList.Read(this.Session, listId);
				}
				return this.documentLibrary;
			}
		}

		internal static XmlNode GetNodeForItem(SharepointSession session, SharepointDocumentLibraryItemId itemId)
		{
			XmlNode result;
			using (Lists lists = new Lists(session.Uri.ToString()))
			{
				lists.Credentials = CredentialCache.DefaultCredentials;
				XmlNode query = SharepointHelpers.GenerateQueryCAML(new ComparisonFilter(ComparisonOperator.Equal, SharepointDocumentLibraryItemSchema.ID, itemId.ItemId));
				XmlNode queryOptions = SharepointHelpers.GenerateQueryOptionsXml(itemId.ParentDirectoryStructure);
				XmlNode viewFields = SharepointHelpers.GenerateViewFieldCAML(SharepointDocumentSchema.Instance, SharepointDocumentSchema.Instance.AllProperties.Keys);
				XmlNode listItems = lists.GetListItems(itemId.ListName, null, query, viewFields, "2", queryOptions);
				XmlNodeList xmlNodeList = listItems.SelectNodes("/rs:data/z:row", SharepointHelpers.SharepointNamespaceManager);
				result = ((xmlNodeList.Count == 1) ? xmlNodeList[0] : null);
			}
			return result;
		}

		private SharepointDocumentLibrary documentLibrary;

		private SharepointDocumentLibraryFolder parent;
	}
}
