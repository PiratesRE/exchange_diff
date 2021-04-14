using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.DocumentLibrary.SharepointService;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointList : SharepointObject
	{
		internal SharepointList(SharepointListId listId, SharepointSession session, XmlNode dataCache) : base(listId, session, dataCache, SharepointListSchema.Instance)
		{
			if (listId.CultureInfo != null)
			{
				this.CultureInfo = listId.CultureInfo;
				return;
			}
			if (dataCache.ChildNodes.Count > 0)
			{
				foreach (object obj in dataCache.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Name == "RegionalSettings")
					{
						using (IEnumerator enumerator2 = xmlNode.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (xmlNode2.Name == "Locale")
								{
									try
									{
										this.CultureInfo = CultureInfo.GetCultureInfo(int.Parse(xmlNode2.InnerText));
										listId.CultureInfo = this.CultureInfo;
									}
									catch (FormatException)
									{
										throw new CorruptDataException(this, Strings.ExCorruptRegionalSetting);
									}
									catch (ArgumentException)
									{
										throw new CorruptDataException(this, Strings.ExCorruptRegionalSetting);
									}
								}
							}
							break;
						}
					}
				}
				if (this.CultureInfo == null)
				{
					throw new CorruptDataException(this, Strings.ExCorruptRegionalSetting);
				}
			}
		}

		public new static SharepointList Read(SharepointSession session, ObjectId listId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (listId == null)
			{
				throw new ArgumentNullException("listId");
			}
			SharepointListId spListId = listId as SharepointListId;
			if (spListId == null)
			{
				throw new ArgumentException("listId");
			}
			if (spListId.SiteUri != session.Uri)
			{
				throw new ObjectNotFoundException(listId, Strings.ExObjectNotFound(listId.ToString()));
			}
			if ((spListId.UriFlags & UriFlags.SharepointList) != UriFlags.SharepointList && (spListId.UriFlags & UriFlags.SharepointDocumentLibrary) != UriFlags.SharepointDocumentLibrary)
			{
				throw new ArgumentException("listId");
			}
			if (spListId.Cache != null && spListId.Cache.Value.Key == session.Identity.Name)
			{
				return SharepointList.ReadHelper(spListId.Cache.Value.Value, session, spListId);
			}
			return Utils.DoSharepointTask<SharepointList>(session.Identity, spListId, spListId, false, Utils.MethodType.Read, delegate
			{
				using (Lists lists = new Lists(session.Uri.ToString()))
				{
					new List<Result<SharepointList>>();
					XmlNode xmlNode = lists.GetListAndView(spListId.ListName, null).SelectSingleNode("/sp:List", SharepointHelpers.SharepointNamespaceManager);
					if (xmlNode != null)
					{
						return SharepointList.ReadHelper(xmlNode, session, spListId);
					}
				}
				throw new ObjectNotFoundException(spListId, Strings.ExObjectNotFound(spListId.ToString()));
			});
		}

		public override string Title
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointListSchema.Title);
			}
		}

		public override SharepointItemType ItemType
		{
			get
			{
				return SharepointItemType.List;
			}
		}

		public Uri SiteUri
		{
			get
			{
				return this.SharepointId.SiteUri;
			}
		}

		public override Uri Uri
		{
			get
			{
				SharepointListId sharepointListId = base.Id as SharepointListId;
				return new Uri(new Uri(sharepointListId.SiteUri.GetLeftPart(UriPartial.Authority)), this.DefautViewUri);
			}
		}

		public Uri DocTemplateUri
		{
			get
			{
				return base.GetValueOrDefault<Uri>(SharepointListSchema.DocTemplateUri);
			}
		}

		public Uri DefautViewUri
		{
			get
			{
				return base.GetValueOrDefault<Uri>(SharepointListSchema.DefaultViewUri);
			}
		}

		public Uri ImageUri
		{
			get
			{
				return base.GetValueOrDefault<Uri>(SharepointListSchema.ImageUri);
			}
		}

		public override object TryGetProperty(PropertyDefinition propDef)
		{
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propDef as DocumentLibraryPropertyDefinition;
			if (documentLibraryPropertyDefinition != null && documentLibraryPropertyDefinition.PropertyId == DocumentLibraryPropertyId.Uri)
			{
				return this.Uri;
			}
			return base.TryGetProperty(propDef);
		}

		internal bool HasRegionalSettings
		{
			get
			{
				return this.CultureInfo != null;
			}
		}

		internal CultureInfo GetRegionalSettings()
		{
			if (this.HasRegionalSettings)
			{
				return this.CultureInfo;
			}
			SharepointListId sharepointListId = (SharepointListId)base.Id;
			SharepointListId listId = new SharepointListId(sharepointListId.ListName, sharepointListId.SiteUri, null, sharepointListId.UriFlags);
			SharepointList sharepointList = SharepointList.Read(this.Session, listId);
			if (!sharepointList.HasRegionalSettings)
			{
				throw new CorruptDataException(this, Strings.ExCorruptRegionalSetting);
			}
			this.CultureInfo = sharepointList.GetRegionalSettings();
			sharepointListId.CultureInfo = this.CultureInfo;
			return this.CultureInfo;
		}

		private static SharepointList ReadHelper(XmlNode node, SharepointSession session, SharepointListId spListId)
		{
			object[] valuesFromCAMLView = SharepointHelpers.GetValuesFromCAMLView(SharepointListSchema.Instance, node, null, new PropertyDefinition[]
			{
				SharepointListSchema.Name,
				SharepointListSchema.ListType,
				SharepointListSchema.PredefinedListType
			});
			ListBaseType listBaseType = (ListBaseType)valuesFromCAMLView[1];
			spListId.Cache = new KeyValuePair<string, XmlNode>?(new KeyValuePair<string, XmlNode>(session.Identity.Name, node));
			if (listBaseType == ListBaseType.DocumentLibrary)
			{
				return new SharepointDocumentLibrary(spListId, session, node);
			}
			return new SharepointList(spListId, session, node);
		}
	}
}
