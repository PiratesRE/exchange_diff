using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Principal;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Data.DocumentLibrary.SharepointService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointSession
	{
		internal SharepointSession(SharepointSiteId sharepointId, WindowsPrincipal windowsPrincipal)
		{
			if (sharepointId == null)
			{
				throw new ArgumentNullException("sharepointId");
			}
			if (windowsPrincipal == null)
			{
				throw new ArgumentNullException("windowsPrincipal");
			}
			this.windowsIdentity = (WindowsIdentity)windowsPrincipal.Identity;
			this.sharepointId = sharepointId;
		}

		public static SharepointSession Open(ObjectId objectId, IPrincipal authenticatedUser)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			SharepointSiteId sharepointSiteId = objectId as SharepointSiteId;
			WindowsPrincipal windowsPrincipal = authenticatedUser as WindowsPrincipal;
			if (sharepointSiteId == null)
			{
				throw new ArgumentException("objectId");
			}
			if (windowsPrincipal == null)
			{
				throw new ArgumentException("authenticatedUser");
			}
			return new SharepointSession(sharepointSiteId, windowsPrincipal);
		}

		public static SharepointSession Open(SharepointWeb web, IPrincipal authenticatedUser)
		{
			if (web == null)
			{
				throw new ArgumentNullException("web");
			}
			return SharepointSession.Open(web.Id, authenticatedUser);
		}

		public ObjectId Id
		{
			get
			{
				return this.sharepointId;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.sharepointId.SiteUri;
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.Uri.Segments.Length > 1)
				{
					return this.Uri.Segments[this.Uri.Segments.Length - 1];
				}
				return this.Uri.Host;
			}
		}

		public Uri BaseSiteUri
		{
			get
			{
				if (!this.baseSiteUriInitialized)
				{
					if (this.Uri.Segments.Length > 1)
					{
						WindowsImpersonationContext windowsImpersonationContext = Utils.ImpersonateUser(this.Identity);
						try
						{
							try
							{
								using (Webs webs = new Webs(this.Uri.GetLeftPart(UriPartial.Authority)))
								{
									webs.Credentials = CredentialCache.DefaultCredentials;
									UriBuilder uriBuilder = new UriBuilder(this.Uri.GetLeftPart(UriPartial.Authority));
									for (int i = 1; i < this.Uri.Segments.Length - 1; i++)
									{
										UriBuilder uriBuilder2 = uriBuilder;
										uriBuilder2.Path += this.Uri.Segments[i];
									}
									string uriString = webs.WebUrlFromPageUrl(uriBuilder.Uri.ToString());
									this.baseSiteUri = new Uri(uriString);
									this.baseSiteUriInitialized = true;
								}
							}
							catch (SoapException)
							{
								Utils.UndoContext(ref windowsImpersonationContext);
								this.baseSiteUri = this.Uri;
								this.baseSiteUriInitialized = true;
							}
							catch
							{
								Utils.UndoContext(ref windowsImpersonationContext);
								throw;
							}
							goto IL_10A;
						}
						finally
						{
							Utils.UndoContext(ref windowsImpersonationContext);
						}
					}
					this.baseSiteUri = this.Uri;
					this.baseSiteUriInitialized = true;
				}
				IL_10A:
				return this.baseSiteUri;
			}
		}

		public ITableView GetView(PredefinedListType predefinedListType, params PropertyDefinition[] propsToReturn)
		{
			return this.InternalGetView(ListBaseType.Any, predefinedListType, null, propsToReturn);
		}

		public ITableView GetView(PredefinedListType predefinedListType, SortBy[] sortBy, PropertyDefinition[] propsToReturn)
		{
			return this.InternalGetView(ListBaseType.Any, predefinedListType, sortBy, propsToReturn);
		}

		public ITableView GetView(ListBaseType listBaseType, params PropertyDefinition[] propsToReturn)
		{
			return this.InternalGetView(listBaseType, PredefinedListType.Any, null, propsToReturn);
		}

		public ITableView GetView(ListBaseType listBaseType, SortBy[] sortBy, PropertyDefinition[] propsToReturn)
		{
			return this.InternalGetView(listBaseType, PredefinedListType.Any, sortBy, propsToReturn);
		}

		private ITableView InternalGetView(ListBaseType listBaseType, PredefinedListType predefinedListType, SortBy[] sortBy, params PropertyDefinition[] propsToReturn)
		{
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length == 0)
			{
				throw new ArgumentException("propsToReturn");
			}
			EnumValidator.ThrowIfInvalid<ListBaseType>(listBaseType, "listBaseType");
			EnumValidator.ThrowIfInvalid<PredefinedListType>(predefinedListType, "listBaseType");
			if (listBaseType != ListBaseType.Any && predefinedListType != PredefinedListType.Any)
			{
				throw new ArgumentException("listBaseType && predefinedListType");
			}
			return Utils.DoSharepointTask<ArrayTableView>(this.Identity, this.Id, (SharepointSiteId)this.Id, true, Utils.MethodType.GetView, delegate
			{
				List<object[]> list = new List<object[]>();
				ArrayTableView result;
				using (Lists lists = new Lists(this.Uri.ToString()))
				{
					lists.Credentials = CredentialCache.DefaultCredentials;
					foreach (object obj in lists.GetListCollection().SelectNodes("/sp:List", SharepointHelpers.SharepointNamespaceManager))
					{
						XmlNode xmlNode = (XmlNode)obj;
						object[] valuesFromCAMLView = SharepointHelpers.GetValuesFromCAMLView(SharepointListSchema.Instance, xmlNode, null, new PropertyDefinition[]
						{
							SharepointListSchema.ID,
							SharepointListSchema.ListType,
							SharepointListSchema.PredefinedListType,
							SharepointListSchema.IsHidden
						});
						int num = 0;
						int num2 = num + 1;
						int num3 = num2 + 1;
						int num4 = num3 + 1;
						string text = valuesFromCAMLView[num] as string;
						if (text != null && valuesFromCAMLView[num2] is int && valuesFromCAMLView[num3] is int && valuesFromCAMLView[num4] is bool && !(bool)valuesFromCAMLView[num4])
						{
							ListBaseType listBaseType2 = (ListBaseType)valuesFromCAMLView[num2];
							PredefinedListType predefinedListType2 = (PredefinedListType)valuesFromCAMLView[num3];
							if ((listBaseType == ListBaseType.Any || listBaseType == listBaseType2) && (predefinedListType == PredefinedListType.Any || predefinedListType == predefinedListType2))
							{
								SharepointListId sharepointListId;
								if (listBaseType2 == ListBaseType.DocumentLibrary)
								{
									sharepointListId = new SharepointListId(text, this.Uri, null, UriFlags.SharepointDocumentLibrary);
								}
								else
								{
									sharepointListId = new SharepointListId(text, this.Uri, null, UriFlags.SharepointList);
								}
								sharepointListId.Cache = new KeyValuePair<string, XmlNode>?(new KeyValuePair<string, XmlNode>(this.Identity.Name, xmlNode));
								object[] valuesFromCAMLView2 = SharepointHelpers.GetValuesFromCAMLView(SharepointListSchema.Instance, xmlNode, null, propsToReturn);
								for (int i = 0; i < propsToReturn.Length; i++)
								{
									DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propsToReturn[i] as DocumentLibraryPropertyDefinition;
									if (documentLibraryPropertyDefinition != null)
									{
										if (documentLibraryPropertyDefinition.PropertyId == DocumentLibraryPropertyId.Id)
										{
											valuesFromCAMLView2[i] = sharepointListId;
										}
										else if (documentLibraryPropertyDefinition.PropertyId == DocumentLibraryPropertyId.Uri)
										{
											Uri uri = SharepointHelpers.GetValuesFromCAMLView(SharepointListSchema.Instance, xmlNode, null, new PropertyDefinition[]
											{
												SharepointListSchema.DefaultViewUri
											})[0] as Uri;
											if (uri != null)
											{
												valuesFromCAMLView2[i] = new UriBuilder(sharepointListId.SiteUri.Scheme, sharepointListId.SiteUri.Host, sharepointListId.SiteUri.Port, uri.ToString()).Uri;
											}
										}
									}
								}
								list.Add(valuesFromCAMLView2);
							}
						}
					}
					result = new ArrayTableView(null, sortBy, propsToReturn, list);
				}
				return result;
			});
		}

		public ReadOnlyCollection<SharepointWeb> GetSubWebs()
		{
			return Utils.DoSharepointTask<ReadOnlyCollection<SharepointWeb>>(this.Identity, this.Id, null, true, Utils.MethodType.GetView, delegate
			{
				ReadOnlyCollection<SharepointWeb> result;
				using (Webs webs = new Webs(this.Uri.ToString()))
				{
					webs.Credentials = CredentialCache.DefaultCredentials;
					XmlNode webCollection = webs.GetWebCollection();
					List<SharepointWeb> list = new List<SharepointWeb>();
					foreach (object obj in webCollection.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						list.Add(new SharepointWeb(xmlNode.Attributes["Title"].Value, new SharepointSiteId(xmlNode.Attributes["Url"].Value, UriFlags.Sharepoint)));
					}
					result = new ReadOnlyCollection<SharepointWeb>(list);
				}
				return result;
			});
		}

		internal WindowsIdentity Identity
		{
			get
			{
				return this.windowsIdentity;
			}
		}

		private readonly SharepointSiteId sharepointId;

		private readonly WindowsIdentity windowsIdentity;

		private Uri baseSiteUri;

		private bool baseSiteUriInitialized;
	}
}
