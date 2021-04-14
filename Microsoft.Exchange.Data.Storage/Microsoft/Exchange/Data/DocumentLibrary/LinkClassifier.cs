using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Principal;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data.DocumentLibrary.SharepointService;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LinkClassifier
	{
		private static ClassifyResult ClassifyUncLink(Uri uri, WindowsIdentity authenticatedUser)
		{
			WindowsImpersonationContext windowsImpersonationContext = null;
			ClassifyResult classifyResult = null;
			ClassifyResult result;
			try
			{
				windowsImpersonationContext = Utils.ImpersonateUser(authenticatedUser);
				UriFlags uriFlags = UriFlags.Other;
				if (!Utils.IsValidUncUri(uri))
				{
					result = new ClassifyResult(uri, ClassificationError.InvalidUri);
				}
				else
				{
					if (uri.Segments.Length == 1)
					{
						try
						{
							UncObjectId objectId = new UncObjectId(uri, UriFlags.Unc);
							UncSession uncSession = UncSession.Open(objectId, new WindowsPrincipal(WindowsIdentity.GetCurrent()));
							uncSession.GetView(null, null, new PropertyDefinition[0]);
						}
						catch (AccessDeniedException)
						{
							return new ClassifyResult(uri, ClassificationError.AccessDenied);
						}
						catch (ObjectMovedOrDeletedException)
						{
							return new ClassifyResult(uri, ClassificationError.ConnectionFailed);
						}
						uriFlags = UriFlags.Unc;
					}
					else if (uri.Segments.Length >= 2)
					{
						if (uri.Segments.Length >= 3)
						{
							try
							{
								FileSystemInfo fileSystemInfo = new FileInfo(uri.LocalPath);
								if (fileSystemInfo.Attributes == (FileAttributes)(-1))
								{
									return new ClassifyResult(uri, ClassificationError.ObjectNotFound);
								}
								if ((fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
								{
									uriFlags = UriFlags.UncFolder;
								}
								else
								{
									uriFlags = UriFlags.UncDocument;
								}
							}
							catch (UnauthorizedAccessException)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.AccessDenied);
							}
							catch (FileNotFoundException)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
							}
							catch (IOException)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.AccessDenied);
							}
						}
						if (classifyResult != null || uri.Segments.Length == 2)
						{
							IntPtr intPtr;
							int num = UncSession.NetShareGetInfo(uri.Host, Uri.UnescapeDataString(uri.Segments[1].TrimEnd(new char[]
							{
								'\\',
								'/'
							})), 1, out intPtr);
							if (intPtr != IntPtr.Zero)
							{
								UncSession.NetApiBufferFree(intPtr);
							}
							else if (num == 5 || num == 2311)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.AccessDenied);
							}
							else if (num == 53 || num == 2250)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.ConnectionFailed);
							}
							else if (num == 2310)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
							}
							else if (num != 0)
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.UnknownError);
							}
							uriFlags = UriFlags.UncDocumentLibrary;
						}
						if (classifyResult != null)
						{
							return classifyResult;
						}
					}
					if ((uriFlags & UriFlags.Unc) != (UriFlags)0)
					{
						result = new ClassifyResult(new UncObjectId(uri, uriFlags), uri, uriFlags);
					}
					else
					{
						result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
					}
				}
			}
			catch
			{
				Utils.UndoContext(ref windowsImpersonationContext);
				throw;
			}
			finally
			{
				Utils.UndoContext(ref windowsImpersonationContext);
			}
			return result;
		}

		private static ClassifyResult GetClassificationResultFromWebException(Uri uri, WebException ex)
		{
			if (ex.Response != null)
			{
				HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
				if (statusCode == HttpStatusCode.Unauthorized)
				{
					return new ClassifyResult(uri, ClassificationError.AccessDenied);
				}
				if (statusCode == HttpStatusCode.NotFound)
				{
					return new ClassifyResult(uri, ClassificationError.ObjectNotFound);
				}
				if (statusCode == HttpStatusCode.BadGateway || statusCode == HttpStatusCode.ProxyAuthenticationRequired)
				{
					return new ClassifyResult(uri, ClassificationError.ProxyError);
				}
				return new ClassifyResult(uri, ClassificationError.UnknownError);
			}
			else
			{
				switch (ex.Status)
				{
				case WebExceptionStatus.NameResolutionFailure:
					return new ClassifyResult(uri, ClassificationError.ObjectNotFound);
				case WebExceptionStatus.ConnectFailure:
				case WebExceptionStatus.ReceiveFailure:
				case WebExceptionStatus.SendFailure:
				case WebExceptionStatus.PipelineFailure:
				case WebExceptionStatus.RequestCanceled:
				case WebExceptionStatus.ProtocolError:
				case WebExceptionStatus.ConnectionClosed:
				case WebExceptionStatus.TrustFailure:
				case WebExceptionStatus.SecureChannelFailure:
				case WebExceptionStatus.ServerProtocolViolation:
				case WebExceptionStatus.KeepAliveFailure:
				case WebExceptionStatus.Pending:
				case WebExceptionStatus.Timeout:
				case WebExceptionStatus.MessageLengthLimitExceeded:
				case WebExceptionStatus.CacheEntryNotFound:
				case WebExceptionStatus.RequestProhibitedByCachePolicy:
					return new ClassifyResult(uri, ClassificationError.ConnectionFailed);
				case WebExceptionStatus.ProxyNameResolutionFailure:
				case WebExceptionStatus.RequestProhibitedByProxy:
					return new ClassifyResult(uri, ClassificationError.ProxyError);
				case WebExceptionStatus.UnknownError:
					return new ClassifyResult(uri, ClassificationError.UnknownError);
				default:
					return new ClassifyResult(uri, ClassificationError.UnknownError);
				}
			}
		}

		private static Uri GetSiteAddress(Uri uri, out ClassifyResult result)
		{
			result = null;
			Uri result2;
			using (Webs webs = new Webs(uri.GetLeftPart(UriPartial.Authority)))
			{
				webs.Credentials = CredentialCache.DefaultCredentials;
				try
				{
					Uri uri2 = new Uri(webs.WebUrlFromPageUrl(uri.ToString()));
					if (string.Compare(uri2.ToString().TrimEnd(new char[]
					{
						'/',
						'\\'
					}), uri.ToString().TrimEnd(new char[]
					{
						'/',
						'\\'
					}), StringComparison.OrdinalIgnoreCase) == 0)
					{
						result = new ClassifyResult(new SharepointSiteId(uri2.OriginalString, UriFlags.Sharepoint), uri, UriFlags.Sharepoint);
					}
					else if (uri.Segments.Length == uri2.Segments.Length + 1 && string.Compare(uri.Segments[uri.Segments.Length - 1], "default.aspx", StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (LinkClassifier.CheckIfLinkExists(uri, null))
						{
							result = new ClassifyResult(new SharepointSiteId(uri2.OriginalString, UriFlags.Sharepoint), uri, UriFlags.Sharepoint);
						}
						else
						{
							result = new ClassifyResult(null, uri, UriFlags.Other);
						}
					}
					return uri2;
				}
				catch (SoapException)
				{
					result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
				}
				catch (WebException)
				{
					throw;
				}
				catch (InvalidOperationException)
				{
					result = new ClassifyResult(null, uri, UriFlags.Other);
				}
				catch (NotSupportedException)
				{
					result = new ClassifyResult(null, ClassificationError.ConnectionFailed);
				}
				result2 = null;
			}
			return result2;
		}

		public static bool CheckIfLinkExists(Uri uri, WindowsIdentity authenticatedUser)
		{
			WindowsImpersonationContext windowsImpersonationContext = null;
			bool result;
			try
			{
				windowsImpersonationContext = ((authenticatedUser != null) ? Utils.ImpersonateUser(authenticatedUser) : null);
				WebRequest webRequest = WebRequest.Create(uri);
				webRequest.Credentials = CredentialCache.DefaultCredentials;
				using (webRequest.GetResponse())
				{
					result = true;
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					HttpStatusCode statusCode = ((HttpWebResponse)ex.Response).StatusCode;
					if (statusCode == HttpStatusCode.NotFound)
					{
						return false;
					}
				}
				Utils.UndoContext(ref windowsImpersonationContext);
				throw;
			}
			catch
			{
				Utils.UndoContext(ref windowsImpersonationContext);
				throw;
			}
			finally
			{
				Utils.UndoContext(ref windowsImpersonationContext);
			}
			return result;
		}

		private static SharepointList GetListSpecifics(Uri uri, SharepointSession session, out ClassifyResult result)
		{
			SharepointList sharepointList = null;
			int num = 0;
			result = null;
			foreach (object[] array in session.GetView(ListBaseType.Any, new PropertyDefinition[]
			{
				DocumentLibrarySchema.Id
			}).GetRows(int.MaxValue))
			{
				SharepointList sharepointList2 = SharepointList.Read(session, (ObjectId)array[0]);
				Uri uri2 = sharepointList2.Uri;
				int num2 = 0;
				int num3 = session.Uri.Segments.Length;
				while (num3 < uri2.Segments.Length && num3 < uri.Segments.Length && string.Compare(uri.Segments[num3].TrimEnd(new char[]
				{
					'/',
					'\\'
				}), uri2.Segments[num3].TrimEnd(new char[]
				{
					'/',
					'\\'
				}), StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					num3++;
					num2++;
				}
				if (num2 > num)
				{
					sharepointList = sharepointList2;
					num = num2;
				}
				else if (num2 == num)
				{
					sharepointList = null;
				}
			}
			if (sharepointList == null)
			{
				result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
			}
			else if (sharepointList.ItemType == SharepointItemType.List)
			{
				result = new ClassifyResult(sharepointList.Id, uri, UriFlags.SharepointList);
			}
			else if (!uri.Query.Contains("&RootFolder=") && !uri.Query.Contains("?RootFolder="))
			{
				Uri uri3 = sharepointList.Uri;
				if (string.Compare(uri.AbsolutePath, uri3.AbsolutePath, StringComparison.OrdinalIgnoreCase) == 0)
				{
					result = new ClassifyResult(sharepointList.Id, uri, UriFlags.SharepointDocumentLibrary);
				}
				else if (uri.Segments.Length == uri3.Segments.Length)
				{
					for (int j = 0; j < uri.Segments.Length - 1; j++)
					{
						if (string.Compare(uri3.Segments[j], uri.Segments[j], StringComparison.OrdinalIgnoreCase) != 0)
						{
							return sharepointList;
						}
					}
					if (LinkClassifier.CheckIfLinkExists(uri, null))
					{
						result = new ClassifyResult(sharepointList.Id, uri, UriFlags.SharepointDocumentLibrary);
					}
					else
					{
						result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
					}
				}
			}
			return sharepointList;
		}

		private static IList<string> GetItemHierarchy(Uri uri, SharepointList sharepointList)
		{
			List<string> list = null;
			if (uri.Query.Contains("?RootFolder=") || uri.Query.Contains("&RootFolder="))
			{
				foreach (string text in uri.Query.Split(new char[]
				{
					'?',
					'&'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					if (text.StartsWith("RootFolder="))
					{
						string text2 = Uri.UnescapeDataString(text.Substring("RootFolder=".Length));
						string text3 = text2.ToLower();
						Uri uri2;
						if (text3.StartsWith("http:") || text3.StartsWith("https:"))
						{
							uri2 = new Uri(text2);
						}
						else
						{
							uri2 = new Uri(new Uri(sharepointList.Session.Uri.GetLeftPart(UriPartial.Authority)), text2);
						}
						list = new List<string>();
						for (int j = sharepointList.Session.Uri.Segments.Length; j < uri2.Segments.Length; j++)
						{
							list.Add(Uri.UnescapeDataString(uri2.Segments[j]));
						}
						break;
					}
				}
			}
			else
			{
				list = new List<string>();
				for (int k = sharepointList.Session.Uri.Segments.Length; k < uri.Segments.Length; k++)
				{
					list.Add(Uri.UnescapeDataString(uri.Segments[k]));
				}
			}
			return list;
		}

		private static ClassifyResult ClassifySharepointLink(Uri uri, WindowsIdentity authenticatedUser)
		{
			WindowsImpersonationContext windowsImpersonationContext = null;
			ClassifyResult classifyResult = null;
			ClassifyResult result;
			try
			{
				windowsImpersonationContext = Utils.ImpersonateUser(authenticatedUser);
				Uri siteAddress = LinkClassifier.GetSiteAddress(uri, out classifyResult);
				if (classifyResult != null)
				{
					result = classifyResult;
				}
				else
				{
					SharepointSession session = SharepointSession.Open(new SharepointSiteId(siteAddress, UriFlags.Sharepoint), new WindowsPrincipal(authenticatedUser));
					SharepointList listSpecifics = LinkClassifier.GetListSpecifics(uri, session, out classifyResult);
					if (classifyResult != null)
					{
						result = classifyResult;
					}
					else
					{
						IList<string> itemHierarchy = LinkClassifier.GetItemHierarchy(uri, listSpecifics);
						if (itemHierarchy.Count < 2)
						{
							Uri uri2 = listSpecifics.Uri;
							bool flag = false;
							if (itemHierarchy.Count == 0 || string.Compare(itemHierarchy[0].TrimEnd(new char[]
							{
								'/',
								'\\'
							}), Uri.UnescapeDataString(uri2.Segments[siteAddress.Segments.Length].TrimEnd(new char[]
							{
								'/',
								'\\'
							})), StringComparison.OrdinalIgnoreCase) != 0)
							{
								flag = true;
							}
							if (!flag)
							{
								result = new ClassifyResult(listSpecifics.Id, uri, UriFlags.SharepointDocumentLibrary);
							}
							else
							{
								result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
							}
						}
						else
						{
							string propertyValue = itemHierarchy[itemHierarchy.Count - 1];
							itemHierarchy.RemoveAt(itemHierarchy.Count - 1);
							SharepointListId sharepointListId = (SharepointListId)listSpecifics.Id;
							SharepointDocumentLibraryItemId listId = new SharepointDocumentLibraryItemId("-1", sharepointListId.ListName, sharepointListId.SiteUri, listSpecifics.GetRegionalSettings(), UriFlags.SharepointFolder, itemHierarchy);
							ComparisonFilter query = new ComparisonFilter(ComparisonOperator.Equal, SharepointDocumentLibraryItemSchema.Name, propertyValue);
							PropertyDefinition[] propsToReturn = new PropertyDefinition[]
							{
								SharepointDocumentLibraryItemSchema.ID,
								SharepointDocumentLibraryItemSchema.FileSystemObjectType
							};
							object[][] rows = SharepointDocumentLibraryFolder.InternalGetView(query, null, DocumentLibraryQueryOptions.FoldersAndFiles, propsToReturn, session, listId).GetRows(2);
							if (rows.Length == 1)
							{
								result = new ClassifyResult((DocumentLibraryObjectId)rows[0][0], uri, ((DocumentLibraryObjectId)rows[0][0]).UriFlags);
							}
							else
							{
								result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
							}
						}
					}
				}
			}
			catch (SoapException)
			{
				result = new ClassifyResult(uri, ClassificationError.ConnectionFailed);
			}
			catch (ObjectNotFoundException)
			{
				result = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
			}
			catch (AccessDeniedException)
			{
				result = new ClassifyResult(uri, ClassificationError.AccessDenied);
			}
			catch (WebException ex)
			{
				result = LinkClassifier.GetClassificationResultFromWebException(uri, ex);
			}
			catch
			{
				Utils.UndoContext(ref windowsImpersonationContext);
				throw;
			}
			finally
			{
				Utils.UndoContext(ref windowsImpersonationContext);
			}
			return result;
		}

		private static ClassifyResult ClassifyLink(Uri uri, WindowsIdentity authenticatedUser)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException("uri");
			}
			ClassifyResult classifyResult;
			try
			{
				if (uri.IsUnc)
				{
					classifyResult = LinkClassifier.ClassifyUncLink(uri, authenticatedUser);
				}
				else
				{
					if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
					{
						classifyResult = LinkClassifier.ClassifySharepointLink(uri, authenticatedUser);
						if (classifyResult.Error == ClassificationError.None)
						{
							goto IL_9A;
						}
						try
						{
							if (LinkClassifier.CheckIfLinkExists(uri, authenticatedUser))
							{
								classifyResult = new ClassifyResult(null, uri, UriFlags.Other);
							}
							else
							{
								classifyResult = new ClassifyResult(uri, ClassificationError.ObjectNotFound);
							}
							goto IL_9A;
						}
						catch (WebException)
						{
							goto IL_9A;
						}
						catch (NotSupportedException)
						{
							goto IL_9A;
						}
					}
					classifyResult = new ClassifyResult(uri, ClassificationError.UriTypeNotSupported);
				}
				IL_9A:;
			}
			catch (AuthenticationException)
			{
				classifyResult = new ClassifyResult(uri, ClassificationError.AccessDenied);
			}
			return classifyResult;
		}

		public static ClassifyResult[] ClassifyLinks(IPrincipal authenticatedUser, params Uri[] uris)
		{
			if (authenticatedUser == null)
			{
				throw new ArgumentNullException("authenticatedUser");
			}
			if (uris == null)
			{
				throw new ArgumentNullException("uris");
			}
			if (uris.Length == 0)
			{
				throw new ArgumentException("uris");
			}
			if (!(authenticatedUser is WindowsPrincipal))
			{
				throw new ArgumentException("authenticatedUser");
			}
			ClassifyResult[] array = new ClassifyResult[uris.Length];
			WindowsIdentity authenticatedUser2 = (WindowsIdentity)authenticatedUser.Identity;
			for (int i = 0; i < uris.Length; i++)
			{
				if (uris[i] == null)
				{
					throw new ArgumentException("uris");
				}
				array[i] = LinkClassifier.ClassifyLink(uris[i], authenticatedUser2);
			}
			return array;
		}

		private const string DefaultPage = "default.aspx";
	}
}
