using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal sealed class DocumentLibraryUtilities
	{
		private DocumentLibraryUtilities()
		{
		}

		public static bool IsTrustedProtocol(string protocol)
		{
			if (string.IsNullOrEmpty(protocol))
			{
				throw new ArgumentException("protocol must not be null or empty");
			}
			for (int i = 0; i < DocumentLibraryUtilities.trustedProtocols.Length; i++)
			{
				if (CultureInfo.InvariantCulture.CompareInfo.Compare(protocol, DocumentLibraryUtilities.trustedProtocols[i], CompareOptions.IgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsDocumentsAccessEnabled(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return (!userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.WssIntegrationFromPublicComputer) || userContext.IsFeatureEnabled(Feature.UncIntegrationFromPublicComputer)) && (userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.WssIntegrationFromPrivateComputer) || userContext.IsFeatureEnabled(Feature.UncIntegrationFromPrivateComputer));
		}

		public static bool IsNavigationToWSSAllowed(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return (!userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.WssIntegrationFromPublicComputer)) && (userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.WssIntegrationFromPrivateComputer));
		}

		public static bool IsNavigationToUNCAllowed(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return (!userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.UncIntegrationFromPublicComputer)) && (userContext.IsPublicLogon || userContext.IsFeatureEnabled(Feature.UncIntegrationFromPrivateComputer));
		}

		public static bool IsBlockedHostName(string hostName, UserContext userContext)
		{
			if (string.IsNullOrEmpty(hostName))
			{
				throw new ArgumentException("hostName cannot be null or empty");
			}
			RemoteDocumentsActions remoteDocumentsActionForUnknownServers = userContext.Configuration.RemoteDocumentsActionForUnknownServers;
			string[] blockedDocumentStoreList = userContext.Configuration.BlockedDocumentStoreList;
			for (int i = 0; i < blockedDocumentStoreList.Length; i++)
			{
				if (string.Equals(hostName, blockedDocumentStoreList[i], StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			string[] allowedDocumentStoreList = userContext.Configuration.AllowedDocumentStoreList;
			for (int j = 0; j < allowedDocumentStoreList.Length; j++)
			{
				if (string.Equals(hostName, allowedDocumentStoreList[j], StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return remoteDocumentsActionForUnknownServers == RemoteDocumentsActions.Block;
		}

		public static bool IsInternalUri(string hostName, UserContext userContext)
		{
			if (string.IsNullOrEmpty(hostName))
			{
				throw new ArgumentException("hostName cannot be null or empty");
			}
			string[] internalFQDNSuffixList = userContext.Configuration.InternalFQDNSuffixList;
			for (int i = 0; i < internalFQDNSuffixList.Length; i++)
			{
				if (hostName.EndsWith(internalFQDNSuffixList[i], StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			IPAddress ipaddress = new IPAddress(0L);
			bool result;
			try
			{
				result = (!IPAddress.TryParse(hostName, out ipaddress) && hostName.IndexOf('.') == -1);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Invalid Uri Format in internal URI determination");
				result = false;
			}
			return result;
		}

		public static void RenderFavorites(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			List<DocumentLibrary> favoritesList = DocumentLibraryUtilities.GetFavoritesList(userContext);
			IEnumerator<DocumentLibrary> enumerator = favoritesList.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
				if (num > 1000)
				{
					break;
				}
				DocumentLibraryUtilities.RenderLibraryItem(output, enumerator.Current, userContext, true);
			}
		}

		public static bool AddFavorite(DocumentLibrary library, UserContext userContext, out int countOfFavorites)
		{
			if (library == null)
			{
				throw new ArgumentNullException("library");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			List<DocumentLibrary> favoritesList = DocumentLibraryUtilities.GetFavoritesList(userContext);
			countOfFavorites = favoritesList.Count;
			if (DocumentLibraryUtilities.FavoriteExists(favoritesList, library.Uri))
			{
				return false;
			}
			favoritesList.Add(library);
			favoritesList.Sort();
			DocumentLibraryUtilities.SaveList(favoritesList, userContext);
			return true;
		}

		public static void DeleteFavorite(string uri, UserContext userContext)
		{
			if (string.IsNullOrEmpty(uri))
			{
				throw new ArgumentException("uri cannot be null or empty");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Uri uri2 = Utilities.TryParseUri(uri);
			if (uri2 == null)
			{
				return;
			}
			uri = uri2.ToString();
			int num = 0;
			bool flag = false;
			List<DocumentLibrary> favoritesList = DocumentLibraryUtilities.GetFavoritesList(userContext);
			foreach (DocumentLibrary documentLibrary in favoritesList)
			{
				string text = documentLibrary.Uri;
				if (string.IsNullOrEmpty(text))
				{
					num++;
				}
				else
				{
					Uri uri3 = Utilities.TryParseUri(text);
					if (uri3 == null)
					{
						num++;
					}
					else
					{
						text = uri3.ToString();
						if (text.Equals(uri, StringComparison.OrdinalIgnoreCase))
						{
							flag = true;
							break;
						}
						num++;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			favoritesList.RemoveAt(num);
			DocumentLibraryUtilities.SaveList(favoritesList, userContext);
		}

		public static void RenameFavorite(string uri, string newDisplayName, UserContext userContext)
		{
			if (string.IsNullOrEmpty(uri))
			{
				throw new ArgumentException("uri cannot be null or empty");
			}
			if (string.IsNullOrEmpty(newDisplayName))
			{
				throw new ArgumentException("newDisplayName cannot be null or empty");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Uri uri2 = Utilities.TryParseUri(uri);
			if (uri2 == null)
			{
				return;
			}
			uri = uri2.ToString();
			List<DocumentLibrary> favoritesList = DocumentLibraryUtilities.GetFavoritesList(userContext);
			foreach (DocumentLibrary documentLibrary in favoritesList)
			{
				string text = documentLibrary.Uri;
				if (!string.IsNullOrEmpty(text))
				{
					Uri uri3 = Utilities.TryParseUri(text);
					if (!(uri3 == null))
					{
						text = uri3.ToString();
						if (text.Equals(uri, StringComparison.OrdinalIgnoreCase))
						{
							IEnumerator<DocumentLibrary> enumerator;
							enumerator.Current.DisplayName = newDisplayName;
							break;
						}
					}
				}
			}
			favoritesList.Sort();
			DocumentLibraryUtilities.SaveList(favoritesList, userContext);
		}

		public static ClassifyResult GetDocumentLibraryObjectId(string uri, UserContext userContext)
		{
			if (string.IsNullOrEmpty(uri))
			{
				throw new ArgumentException("uri cannot be null or empty");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			Uri uri2 = Utilities.TryParseUri(uri);
			if (uri2 == null || string.IsNullOrEmpty(uri2.Host) || string.IsNullOrEmpty(uri2.Host))
			{
				return null;
			}
			return DocumentLibraryUtilities.GetDocumentLibraryObjectId(uri2, userContext);
		}

		public static ClassifyResult GetDocumentLibraryObjectId(Uri uri, UserContext userContext)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (!DocumentLibraryUtilities.IsInternalUri(uri.Host, userContext))
			{
				return null;
			}
			if (DocumentLibraryUtilities.IsBlockedHostName(uri.Host, userContext))
			{
				return null;
			}
			ClassifyResult[] array = null;
			OwaWindowsIdentity owaWindowsIdentity = userContext.LogonIdentity as OwaWindowsIdentity;
			if (owaWindowsIdentity != null && owaWindowsIdentity.WindowsPrincipal != null)
			{
				try
				{
					array = LinkClassifier.ClassifyLinks(owaWindowsIdentity.WindowsPrincipal, new Uri[]
					{
						uri
					});
				}
				catch (WebException)
				{
				}
			}
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public static DocumentLibraryObjectId CreateDocumentLibraryObjectId(OwaContext owaContext)
		{
			DocumentLibraryObjectId documentLibraryObjectId = null;
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			HttpContext httpContext = owaContext.HttpContext;
			HttpRequest request = httpContext.Request;
			string queryStringParameter = Utilities.GetQueryStringParameter(request, "TranslatedURL", false);
			bool flag = !string.IsNullOrEmpty(queryStringParameter);
			string queryStringParameter2 = Utilities.GetQueryStringParameter(request, "id", false);
			bool flag2 = !string.IsNullOrEmpty(queryStringParameter2);
			Uri uri;
			if (!flag2)
			{
				string queryStringParameter3 = Utilities.GetQueryStringParameter(request, "URL", false);
				uri = Utilities.TryParseUri(queryStringParameter3);
			}
			else
			{
				documentLibraryObjectId = DocumentLibraryObjectId.Deserialize(queryStringParameter2);
				if (documentLibraryObjectId == null)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Other classification error.  objectId " + queryStringParameter2);
					Utilities.TransferToErrorPage(owaContext, LocalizedStrings.GetNonEncoded(-785304559), null, ThemeFileId.Warning, true);
					return null;
				}
				if (documentLibraryObjectId is UncObjectId)
				{
					uri = (documentLibraryObjectId as UncObjectId).Path;
				}
				else
				{
					uri = (documentLibraryObjectId as SharepointSiteId).SiteUri;
				}
			}
			if (uri == null || string.IsNullOrEmpty(uri.Host) || string.IsNullOrEmpty(uri.Scheme))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was Invalid.");
				Utilities.TransferToErrorPage(owaContext, LocalizedStrings.GetNonEncoded(-2054976140), flag ? string.Format(LocalizedStrings.GetHtmlEncoded(-666455008), "<a href=\"" + Utilities.UrlEncode(queryStringParameter) + "\" target=\"_blank\" class=lnk>", "</a>") : null, ThemeFileId.Warning, true);
				return null;
			}
			if (!DocumentLibraryUtilities.IsTrustedProtocol(uri.Scheme))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI protocol is not http, https or file.");
				Utilities.TransferToErrorPage(owaContext, LocalizedStrings.GetNonEncoded(1453018462), null, ThemeFileId.ButtonDialogInfo, true);
				return null;
			}
			if (!DocumentLibraryUtilities.IsInternalUri(uri.Host, owaContext.UserContext))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was not internal.");
				Utilities.TransferToErrorPage(owaContext, LocalizedStrings.GetNonEncoded(-1721073157), null, ThemeFileId.Warning, true);
				return null;
			}
			if (DocumentLibraryUtilities.IsBlockedHostName(uri.Host, owaContext.UserContext))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was blocked.");
				Utilities.TransferToErrorPage(owaContext, LocalizedStrings.GetNonEncoded(343095135), null, ThemeFileId.Warning, true);
				return null;
			}
			ClassifyResult documentLibraryObjectId2 = DocumentLibraryUtilities.GetDocumentLibraryObjectId(uri, owaContext.UserContext);
			if (documentLibraryObjectId2.Error != ClassificationError.None)
			{
				string errorDetailedDescription = null;
				ThemeFileId icon = ThemeFileId.Warning;
				string nonEncoded;
				if (documentLibraryObjectId2.Error == ClassificationError.ConnectionFailed)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Connection could not be made to server.");
					nonEncoded = LocalizedStrings.GetNonEncoded(678272416);
				}
				else if (documentLibraryObjectId2.Error == ClassificationError.ObjectNotFound)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI object not found.");
					nonEncoded = LocalizedStrings.GetNonEncoded(-54320700);
				}
				else if (documentLibraryObjectId2.Error == ClassificationError.AccessDenied)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI access is denied.");
					nonEncoded = LocalizedStrings.GetNonEncoded(234621291);
				}
				else if (documentLibraryObjectId2.Error == ClassificationError.UriTypeNotSupported)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI is not supported.");
					nonEncoded = LocalizedStrings.GetNonEncoded(1453018462);
					icon = ThemeFileId.ButtonDialogInfo;
				}
				else if (documentLibraryObjectId2.Error == ClassificationError.InvalidUri)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was Invalid.");
					nonEncoded = LocalizedStrings.GetNonEncoded(-2054976140);
					if (flag)
					{
						errorDetailedDescription = string.Format(LocalizedStrings.GetNonEncoded(-666455008), "<a href=\"" + queryStringParameter + "\" target=\"_blank\" class=lnk>", "</a>");
					}
				}
				else
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Other classification error.  ClassifyResult.Error: " + documentLibraryObjectId2.Error.ToString());
					nonEncoded = LocalizedStrings.GetNonEncoded(-785304559);
				}
				Utilities.TransferToErrorPage(owaContext, nonEncoded, errorDetailedDescription, icon, true);
				return null;
			}
			if (!flag2)
			{
				documentLibraryObjectId = documentLibraryObjectId2.ObjectId;
			}
			return documentLibraryObjectId;
		}

		public static void RenderSecondaryNavigation(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			bool isDocumentFavoritesVisible = userContext.UserOptions.IsDocumentFavoritesVisible;
			output.Write("<div id=\"divOpnLoc\" ");
			Utilities.RenderScriptHandler(output, "onclick", "tbA(\"openlocdl\");");
			output.Write(" ");
			Utilities.RenderScriptHandler(output, "oncontextmenu", "tbA(\"openlocdl\");");
			output.Write(" ");
			RenderingUtilities.RenderAttribute(output, "L_OK", LocalizedStrings.GetNonEncoded(2041362128));
			RenderingUtilities.RenderAttribute(output, "L_Opn", LocalizedStrings.GetNonEncoded(197744374));
			RenderingUtilities.RenderAttribute(output, "L_Cncl", LocalizedStrings.GetNonEncoded(-1936577052));
			RenderingUtilities.RenderAttribute(output, "L_OpnLoc", LocalizedStrings.GetNonEncoded(1216727381));
			RenderingUtilities.RenderAttribute(output, "L_OpnLocDesc", LocalizedStrings.GetNonEncoded(1895882695));
			RenderingUtilities.RenderAttribute(output, "L_Opning", LocalizedStrings.GetNonEncoded(-2011554051));
			output.Write(">");
			userContext.RenderThemeImage(output, ThemeFileId.DownButton3, "opnLocDrpDnImg", new object[0]);
			output.Write(LocalizedStrings.GetHtmlEncoded(1216727381));
			output.Write("</div>");
			output.Write("<div id=\"divFavFldrsHdr\" ");
			Utilities.RenderScriptHandler(output, "onclick", "tglDocBlk();");
			output.Write(">");
			userContext.RenderThemeImage(output, isDocumentFavoritesVisible ? ThemeFileId.Collapse : ThemeFileId.Expand, "imgFavFldrs", new object[]
			{
				"id=\"imgFavFldrs\""
			});
			output.Write(LocalizedStrings.GetHtmlEncoded(364750115));
			output.Write("</div>");
			output.Write("<div id=\"divFavFldrs\" class=\"docLibBlk\"");
			if (isDocumentFavoritesVisible)
			{
				output.Write(">");
				DocumentLibraryUtilities.RenderFavorites(output, userContext);
			}
			else
			{
				output.Write(" style=\"display:none\">");
			}
			output.Write("</div>");
			DocumentFolderContextMenu documentFolderContextMenu = new DocumentFolderContextMenu(userContext);
			documentFolderContextMenu.Render(output);
		}

		public static void RenderLibraryItem(TextWriter output, DocumentLibrary library, UserContext userContext, bool isFavorite)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (library == null)
			{
				throw new ArgumentNullException("library");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<div class=\"docFavItm\" title=\"");
			Utilities.HtmlEncode(library.DisplayName, output);
			output.Write("\n");
			Utilities.HtmlEncode(library.SiteName, output);
			output.Write("\n");
			Utilities.HtmlEncode(library.Uri, output);
			output.Write("\" ");
			string arg = Utilities.JavascriptEncode(library.Uri);
			string handlerCode = string.Format("tbA(\"opendl\",new Array(\"{0}\",{1}));", arg, (int)library.Type);
			Utilities.RenderScriptHandler(output, "onclick", handlerCode);
			output.Write(" ");
			Utilities.RenderScriptHandler(output, "oncontextmenu", "shwDocMnu(_this," + (isFavorite ? "1" : "0") + ");");
			output.Write(" uri=\"{0}\" uf={1}><div class=\"docFavNmLine\">", Utilities.HtmlEncode(library.Uri), (int)library.Type);
			UriFlags type = library.Type;
			switch (type)
			{
			case UriFlags.Sharepoint:
			case UriFlags.Unc:
			case UriFlags.SharepointDocumentLibrary:
				goto IL_14E;
			case UriFlags.Sharepoint | UriFlags.Unc:
			case UriFlags.DocumentLibrary:
				goto IL_165;
			case UriFlags.UncDocumentLibrary:
				break;
			default:
				switch (type)
				{
				case UriFlags.SharepointFolder:
					goto IL_14E;
				case UriFlags.UncFolder:
					break;
				default:
					goto IL_165;
				}
				break;
			}
			userContext.RenderThemeImage(output, ThemeFileId.Folder, "docFavItmImg", new object[0]);
			goto IL_165;
			IL_14E:
			userContext.RenderThemeImage(output, ThemeFileId.WebFolder, "docFavItmImg", new object[0]);
			IL_165:
			output.Write("<span id=\"spnDocFavDN\">");
			Utilities.HtmlEncode(library.DisplayName, output);
			output.Write("</span></div><span class=\"docFavSN\">");
			if (library.Type == UriFlags.UncDocumentLibrary || library.Type == UriFlags.UncFolder || library.Type == UriFlags.Unc)
			{
				output.Write("\\\\");
			}
			Utilities.HtmlEncode(library.SiteName, output);
			output.Write("</span></div>");
		}

		public static bool IsFavoritesAvailable(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			return DocumentLibraryUtilities.GetFavoritesList(userContext).Count > 0;
		}

		private static bool FavoriteExists(List<DocumentLibrary> libraries, string uri)
		{
			IEnumerator<DocumentLibrary> enumerator = libraries.GetEnumerator();
			Uri uri2;
			if (null == (uri2 = Utilities.TryParseUri(uri)))
			{
				return false;
			}
			while (enumerator.MoveNext())
			{
				Uri obj;
				if (null == (obj = Utilities.TryParseUri(enumerator.Current.Uri)))
				{
					return false;
				}
				if (uri2.Equals(obj))
				{
					return true;
				}
			}
			return false;
		}

		private static List<DocumentLibrary> GetFavoritesList(UserContext userContext)
		{
			List<DocumentLibrary> list = new List<DocumentLibrary>();
			using (UserConfiguration userConfiguration = UserConfigurationUtilities.GetUserConfiguration("Owa.DocumentLibraryFavorites", userContext))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					if (xmlStream != null && xmlStream.Length > 0L)
					{
						using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(xmlStream))
						{
							DocumentLibraryUtilities.ParseFavoritesList(xmlTextReader, list, userContext);
						}
					}
				}
			}
			return list;
		}

		private static void ParseFavoritesList(XmlTextReader xmlReader, List<DocumentLibrary> libraries, UserContext userContext)
		{
			bool isSharepointEnabled = DocumentLibraryUtilities.IsNavigationToWSSAllowed(userContext);
			bool isUncEnabled = DocumentLibraryUtilities.IsNavigationToUNCAllowed(userContext);
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "docLib")
				{
					DocumentLibraryUtilities.ParseFavoritesListNode(xmlReader, libraries, isSharepointEnabled, isUncEnabled);
				}
			}
		}

		private static void ParseFavoritesListNode(XmlTextReader xmlReader, List<DocumentLibrary> libraries, bool isSharepointEnabled, bool isUncEnabled)
		{
			DocumentLibrary documentLibrary = new DocumentLibrary();
			while (xmlReader.MoveToNextAttribute())
			{
				string name;
				if ((name = xmlReader.Name) != null)
				{
					if (!(name == "uri"))
					{
						if (!(name == "dn"))
						{
							if (!(name == "hn"))
							{
								if (name == "uf")
								{
									documentLibrary.Type = (UriFlags)Convert.ToInt32(xmlReader.Value);
								}
							}
							else
							{
								documentLibrary.SiteName = xmlReader.Value;
							}
						}
						else
						{
							documentLibrary.DisplayName = xmlReader.Value;
						}
					}
					else
					{
						documentLibrary.Uri = xmlReader.Value;
					}
				}
			}
			bool flag = (documentLibrary.Type & UriFlags.Sharepoint) == UriFlags.Sharepoint;
			bool flag2 = (documentLibrary.Type & UriFlags.Unc) == UriFlags.Unc;
			if ((flag2 && isUncEnabled) || (flag && isSharepointEnabled))
			{
				libraries.Add(documentLibrary);
			}
		}

		private static void SaveList(List<DocumentLibrary> libraries, UserContext userContext)
		{
			using (UserConfiguration userConfiguration = UserConfigurationUtilities.GetUserConfiguration("Owa.DocumentLibraryFavorites", userContext))
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					using (StreamWriter streamWriter = new StreamWriter(xmlStream))
					{
						using (XmlTextWriter xmlTextWriter = new XmlTextWriter(streamWriter))
						{
							xmlTextWriter.WriteStartElement("docLibs");
							foreach (DocumentLibrary library in libraries)
							{
								DocumentLibraryUtilities.RenderLibraryItemAsXml(xmlTextWriter, library);
							}
							xmlTextWriter.WriteFullEndElement();
						}
					}
				}
				try
				{
					userConfiguration.Save();
				}
				catch (ObjectNotFoundException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "DocumentLibraryUtilities.SaveList: Failed. Exception: {0}", ex.Message);
				}
				catch (QuotaExceededException ex2)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "DocumentLibraryUtilities.SaveList: Failed. Exception: {0}", ex2.Message);
				}
				catch (SaveConflictException ex3)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "DocumentLibraryUtilities.SaveList: Failed. Exception: {0}", ex3.Message);
				}
			}
		}

		private static void RenderLibraryItemAsXml(XmlTextWriter xmlWriter, DocumentLibrary library)
		{
			xmlWriter.WriteStartElement("docLib");
			if (!string.IsNullOrEmpty(library.Uri))
			{
				xmlWriter.WriteAttributeString("uri", library.Uri);
			}
			if (!string.IsNullOrEmpty(library.DisplayName))
			{
				xmlWriter.WriteAttributeString("dn", library.DisplayName);
			}
			if (!string.IsNullOrEmpty(library.SiteName))
			{
				xmlWriter.WriteAttributeString("hn", library.SiteName);
			}
			xmlWriter.WriteAttributeString("uf", ((int)library.Type).ToString());
			xmlWriter.WriteEndElement();
		}

		public static ColumnId GetSortedColumn(ColumnId columnId, UriFlags libraryType)
		{
			DocumentSortType sortTypeofLibrary = DocumentLibraryUtilities.GetSortTypeofLibrary(columnId);
			ColumnId result = ColumnId.Count;
			switch (sortTypeofLibrary)
			{
			case DocumentSortType.Name:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryDisplayName;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_73;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_73;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentDisplayName;
				break;
				IL_73:
				result = ColumnId.SharepointDocumentDisplayName;
				break;
			case DocumentSortType.ModifiedByDate:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryLastModified;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_C6;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_C6;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentLastModified;
				break;
				IL_C6:
				result = ColumnId.SharepointDocumentLastModified;
				break;
			case DocumentSortType.FileSize:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryDisplayName;
				case UriFlags.Unc:
					return ColumnId.UncDocumentDisplayName;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_121;
				case UriFlags.UncDocumentLibrary:
					break;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_121;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentFileSize;
				break;
				IL_121:
				result = ColumnId.SharepointDocumentFileSize;
				break;
			case DocumentSortType.Description:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryDisplayName;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_1C7;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_1C7;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentDisplayName;
				break;
				IL_1C7:
				result = ColumnId.SharepointDocumentLibraryDescription;
				break;
			case DocumentSortType.ModifiedBy:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryDisplayName;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_25B;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_25B;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentDisplayName;
				break;
				IL_25B:
				result = ColumnId.SharepointDocumentModifiedBy;
				break;
			case DocumentSortType.CheckedOutTo:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryDisplayName;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_174;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_174;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentDisplayName;
				break;
				IL_174:
				result = ColumnId.SharepointDocumentCheckedOutTo;
				break;
			case DocumentSortType.DocumentCount:
				switch (libraryType)
				{
				case UriFlags.Sharepoint:
					return ColumnId.SharepointDocumentLibraryItemCount;
				case UriFlags.Unc:
				case UriFlags.UncDocumentLibrary:
					break;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					return result;
				case UriFlags.SharepointDocumentLibrary:
					goto IL_214;
				default:
					switch (libraryType)
					{
					case UriFlags.SharepointFolder:
						goto IL_214;
					case UriFlags.UncFolder:
						break;
					default:
						return result;
					}
					break;
				}
				result = ColumnId.UncDocumentDisplayName;
				break;
				IL_214:
				result = ColumnId.SharepointDocumentDisplayName;
				break;
			}
			return result;
		}

		private static DocumentSortType GetSortTypeofLibrary(ColumnId columnId)
		{
			switch (columnId)
			{
			case ColumnId.SharepointDocumentDisplayName:
			case ColumnId.UncDocumentDisplayName:
			case ColumnId.SharepointDocumentLibraryDisplayName:
				return DocumentSortType.Name;
			case ColumnId.SharepointDocumentLastModified:
			case ColumnId.UncDocumentLastModified:
			case ColumnId.SharepointDocumentLibraryLastModified:
				return DocumentSortType.ModifiedByDate;
			case ColumnId.SharepointDocumentModifiedBy:
				return DocumentSortType.ModifiedBy;
			case ColumnId.SharepointDocumentCheckedOutTo:
				return DocumentSortType.CheckedOutTo;
			case ColumnId.SharepointDocumentFileSize:
			case ColumnId.UncDocumentFileSize:
				return DocumentSortType.FileSize;
			case ColumnId.SharepointDocumentLibraryDescription:
				return DocumentSortType.Description;
			case ColumnId.SharepointDocumentLibraryItemCount:
				return DocumentSortType.DocumentCount;
			}
			return DocumentSortType.Name;
		}

		internal static bool IsWebReadyDocument(DocumentLibraryObjectId objectId, UserContext userContext)
		{
			IDocument document;
			try
			{
				document = DocumentLibraryUtilities.LoadDocumentLibraryItem(objectId, userContext);
			}
			catch (AccessDeniedException)
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI access is denied.");
				return false;
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI object not found.");
				return false;
			}
			if (document == null)
			{
				return false;
			}
			PropertyDefinition propertyDefinition = null;
			if ((objectId.UriFlags & UriFlags.SharepointDocument) == UriFlags.SharepointDocument)
			{
				propertyDefinition = SharepointDocumentSchema.FileType;
			}
			else if ((objectId.UriFlags & UriFlags.UncDocument) == UriFlags.UncDocument)
			{
				propertyDefinition = UncDocumentSchema.FileType;
			}
			object obj = document.TryGetProperty(propertyDefinition);
			string mimeType;
			if (!(obj is PropertyError))
			{
				mimeType = (obj as string);
			}
			else
			{
				mimeType = string.Empty;
			}
			string fileName = Path.GetFileName(document.Uri.ToString());
			string fileExtension;
			if (string.IsNullOrEmpty(fileName))
			{
				fileExtension = string.Empty;
			}
			else
			{
				fileExtension = Path.GetExtension(fileName);
			}
			return AttachmentUtility.IsWebReadyDocument(fileExtension, mimeType);
		}

		internal static IDocument LoadDocumentLibraryItem(DocumentLibraryObjectId objectId, UserContext userContext)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			IDocument result = null;
			if ((objectId.UriFlags & UriFlags.SharepointDocument) == UriFlags.SharepointDocument)
			{
				SharepointSession session = userContext.LogonIdentity.CreateSharepointSession(objectId);
				result = SharepointDocument.Read(session, objectId);
			}
			else if ((objectId.UriFlags & UriFlags.UncDocument) == UriFlags.UncDocument)
			{
				UncSession session2 = userContext.LogonIdentity.CreateUncSession(objectId);
				result = UncDocument.Read(session2, objectId);
			}
			return result;
		}

		public const string UrlQueryParameter = "URL";

		public const string TranslatedUrlQueryParameter = "TranslatedURL";

		public const string UniqueSnippetOfFBAUrlToLogonPage = "/auth/logon.aspx?replacecurrent=1&url=";

		public const int MaxFavoritesInDisplay = 1000;

		private const string FavoritesConfigurationName = "Owa.DocumentLibraryFavorites";

		private const string DocumentLibrariesRootNode = "docLibs";

		private const string DocumentLibraryNode = "docLib";

		private const string UriAttribute = "uri";

		private const string DisplayNameAttribute = "dn";

		private const string HostNameAttribute = "hn";

		private const string DocumentTypeAttribute = "uf";

		private static readonly string[] trustedProtocols = new string[]
		{
			"http",
			"https",
			"file"
		};
	}
}
