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
	internal class SharepointDocumentLibraryFolder : SharepointDocumentLibraryItem, IDocumentLibraryFolder, IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal SharepointDocumentLibraryFolder(SharepointDocumentLibraryItemId id, SharepointSession session, XmlNode dataCache) : base(id, session, dataCache, SharepointFolderSchema.Instance)
		{
		}

		public new static SharepointDocumentLibraryFolder Read(SharepointSession session, ObjectId objectId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = objectId as SharepointDocumentLibraryItemId;
			if (sharepointDocumentLibraryItemId == null)
			{
				throw new ArgumentException("objectId");
			}
			if (sharepointDocumentLibraryItemId.UriFlags != UriFlags.SharepointFolder)
			{
				throw new ArgumentException("objectId");
			}
			return (SharepointDocumentLibraryFolder)SharepointDocumentLibraryItem.Read(session, objectId);
		}

		public ITableView GetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, params PropertyDefinition[] propsToReturn)
		{
			return SharepointDocumentLibraryFolder.InternalGetView(query, sortBy, queryOptions, propsToReturn, this.Session, (SharepointListId)base.Id);
		}

		public override SharepointItemType ItemType
		{
			get
			{
				return SharepointItemType.Folder;
			}
		}

		public override string Title
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentLibraryItemSchema.Name);
			}
		}

		internal static ITableView InternalGetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, PropertyDefinition[] propsToReturn, SharepointSession session, SharepointListId listId)
		{
			EnumValidator.ThrowIfInvalid<DocumentLibraryQueryOptions>(queryOptions, "queryOptions");
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length == 0)
			{
				throw new ArgumentException("propsToReturn");
			}
			return Utils.DoSharepointTask<ArrayTableView>(session.Identity, new SharepointListId(listId.ListName, listId.SiteUri, listId.CultureInfo, UriFlags.SharepointList), listId, true, Utils.MethodType.GetView, delegate
			{
				XmlNode xmlNode = null;
				using (Lists lists = new Lists(listId.SiteUri.ToString()))
				{
					lists.Credentials = CredentialCache.DefaultCredentials;
					XmlNode query2 = null;
					QueryFilter queryFilter = null;
					if (queryOptions == DocumentLibraryQueryOptions.Files)
					{
						queryFilter = new ComparisonFilter(ComparisonOperator.Equal, SharepointDocumentLibraryItemSchema.FileSystemObjectType, 0);
					}
					else if (queryOptions == DocumentLibraryQueryOptions.Folders)
					{
						queryFilter = new ComparisonFilter(ComparisonOperator.Equal, SharepointDocumentLibraryItemSchema.FileSystemObjectType, 1);
					}
					if (query != null && queryFilter != null)
					{
						query = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							query
						});
					}
					else if (queryFilter != null)
					{
						query = queryFilter;
					}
					if (query != null)
					{
						query2 = SharepointHelpers.GenerateQueryCAML(query);
					}
					XmlNode viewFields = SharepointHelpers.GenerateViewFieldCAML(SharepointDocumentSchema.Instance, SharepointDocumentSchema.Instance.AllProperties.Keys);
					if (listId.CultureInfo == null)
					{
						SharepointList sharepointList = SharepointList.Read(session, listId);
						listId.CultureInfo = sharepointList.GetRegionalSettings();
					}
					SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = listId as SharepointDocumentLibraryItemId;
					XmlNode queryOptions2;
					if (sharepointDocumentLibraryItemId != null)
					{
						queryOptions2 = SharepointHelpers.GenerateQueryOptionsXml(sharepointDocumentLibraryItemId.ItemHierarchy);
					}
					else
					{
						queryOptions2 = SharepointHelpers.GenerateQueryOptionsXml(null);
					}
					xmlNode = lists.GetListItems(listId.ListName, null, query2, viewFields, Utils.GetViewMaxRows.ToString(), queryOptions2);
				}
				List<object[]> list = new List<object[]>();
				foreach (object obj in xmlNode.SelectNodes("/rs:data/z:row", SharepointHelpers.SharepointNamespaceManager))
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					object[] valuesFromCAMLView = SharepointHelpers.GetValuesFromCAMLView(SharepointDocumentSchema.Instance, xmlNode2, listId.CultureInfo, new PropertyDefinition[]
					{
						SharepointDocumentLibraryItemSchema.FileSystemObjectType,
						SharepointDocumentLibraryItemSchema.ID,
						SharepointDocumentLibraryItemSchema.EncodedAbsoluteUri
					});
					int num = 0;
					int num2 = num + 1;
					int num3 = num2 + 1;
					bool flag = valuesFromCAMLView[num] is bool && (bool)valuesFromCAMLView[num];
					UriFlags uriFlags = flag ? UriFlags.SharepointFolder : UriFlags.SharepointDocument;
					Uri uri = (Uri)valuesFromCAMLView[num3];
					List<string> list2 = new List<string>();
					for (int i = listId.SiteUri.Segments.Length; i < uri.Segments.Length; i++)
					{
						list2.Add(Uri.UnescapeDataString(uri.Segments[i]));
					}
					SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId2 = new SharepointDocumentLibraryItemId((string)valuesFromCAMLView[num2], listId.ListName, listId.SiteUri, listId.CultureInfo, uriFlags, list2);
					sharepointDocumentLibraryItemId2.Cache = new KeyValuePair<string, XmlNode>?(new KeyValuePair<string, XmlNode>(session.Identity.Name, xmlNode2));
					object[] valuesFromCAMLView2 = SharepointHelpers.GetValuesFromCAMLView(SharepointDocumentSchema.Instance, xmlNode2, listId.CultureInfo, propsToReturn);
					object obj2 = null;
					for (int j = 0; j < propsToReturn.Length; j++)
					{
						if (((DocumentLibraryPropertyDefinition)propsToReturn[j]).PropertyId == DocumentLibraryPropertyId.Id)
						{
							valuesFromCAMLView2[j] = sharepointDocumentLibraryItemId2;
						}
						else if (propsToReturn[j] == SharepointDocumentSchema.VersionControl)
						{
							if (obj2 == null)
							{
								string text = SharepointHelpers.GetValuesFromCAMLView(SharepointDocumentSchema.Instance, xmlNode2, listId.CultureInfo, new PropertyDefinition[]
								{
									SharepointDocumentSchema.CheckedOutUserId
								})[0] as string;
								object obj3 = SharepointHelpers.GetValuesFromCAMLView(SharepointDocumentSchema.Instance, xmlNode2, listId.CultureInfo, new PropertyDefinition[]
								{
									SharepointDocumentSchema.VersionId
								})[0];
								if (obj3 is int && !flag)
								{
									obj2 = new VersionControl(!string.IsNullOrEmpty(text), text, (int)obj3);
								}
								else
								{
									obj2 = new PropertyError(propsToReturn[j], PropertyErrorCode.NotFound);
								}
							}
							valuesFromCAMLView2[j] = obj2;
						}
					}
					list.Add(valuesFromCAMLView2);
				}
				return new ArrayTableView(null, sortBy, propsToReturn, list);
			});
		}
	}
}
