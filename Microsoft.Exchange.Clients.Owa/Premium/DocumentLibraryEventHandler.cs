using System;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("DocumentLibrary")]
	internal sealed class DocumentLibraryEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(DocumentLibraryEventHandler));
		}

		[OwaEvent("ClassifyLink")]
		[OwaEventParameter("uri", typeof(string))]
		public void ClassifyLink()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.ClassifyLink");
			string text = (string)base.GetParameter("uri");
			Uri uri = Utilities.TryParseUri(text);
			if (uri == null && !text.StartsWith("\\\\", StringComparison.Ordinal))
			{
				text = "http://" + text;
				uri = Utilities.TryParseUri(text);
			}
			if (uri == null || string.IsNullOrEmpty(uri.Scheme) || string.IsNullOrEmpty(uri.Host))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Cannot parse a Uri from string.");
				base.RenderPartialFailure(-2054976140, new Strings.IDs?(-381883412), ButtonDialogIcon.Warning);
				return;
			}
			if (!DocumentLibraryUtilities.IsTrustedProtocol(uri.Scheme))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI protocol is not http, https or file.");
				base.RenderPartialFailure(1453018462, new Strings.IDs?(-743095750), ButtonDialogIcon.Information);
				return;
			}
			if (!DocumentLibraryUtilities.IsInternalUri(uri.Host, base.UserContext))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Not an internal URI");
				base.RenderPartialFailure(-1721073157, new Strings.IDs?(1325518514), ButtonDialogIcon.Warning);
				return;
			}
			if (DocumentLibraryUtilities.IsBlockedHostName(uri.Host, base.UserContext))
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Host name is in the blocked list.");
				base.RenderPartialFailure(343095135, new Strings.IDs?(-777407791), ButtonDialogIcon.Warning);
				return;
			}
			ClassifyResult documentLibraryObjectId = DocumentLibraryUtilities.GetDocumentLibraryObjectId(uri, base.UserContext);
			if (documentLibraryObjectId.Error != ClassificationError.None)
			{
				if (documentLibraryObjectId.Error == ClassificationError.ConnectionFailed)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Connection could not be made to server.");
					base.RenderPartialFailure(678272416, new Strings.IDs?(-820112926), ButtonDialogIcon.Warning);
					return;
				}
				if (documentLibraryObjectId.Error == ClassificationError.ObjectNotFound)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI object not found.");
					base.RenderPartialFailure(-54320700, new Strings.IDs?(1599334062), ButtonDialogIcon.Warning);
					return;
				}
				if (documentLibraryObjectId.Error == ClassificationError.AccessDenied)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI access is denied.");
					if (Utilities.IsBasicAuthentication(base.OwaContext.HttpContext.Request))
					{
						base.RenderPartialFailure(234621291, new Strings.IDs?(-3401788), ButtonDialogIcon.Warning);
						return;
					}
					base.RenderPartialFailure(1819837349, new Strings.IDs?(-3401788), ButtonDialogIcon.Warning);
					return;
				}
				else
				{
					if (documentLibraryObjectId.Error == ClassificationError.UriTypeNotSupported)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI is not supported.");
						base.RenderPartialFailure(1453018462, new Strings.IDs?(-743095750), ButtonDialogIcon.Information);
						return;
					}
					if (documentLibraryObjectId.Error == ClassificationError.InvalidUri)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was Invalid.");
						base.RenderPartialFailure(-2054976140, new Strings.IDs?(-381883412), ButtonDialogIcon.Warning);
						return;
					}
					if (documentLibraryObjectId.Error == ClassificationError.ProxyError)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI was Invalid.");
						base.RenderPartialFailure(1454208029, new Strings.IDs?(1335662059), ButtonDialogIcon.Warning);
						return;
					}
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Other classification error.  ClassifyResult.Error: " + documentLibraryObjectId.Error.ToString());
					base.RenderPartialFailure(-785304559, new Strings.IDs?(-86901060), ButtonDialogIcon.Warning);
					return;
				}
			}
			else
			{
				DocumentLibraryObjectId objectId = documentLibraryObjectId.ObjectId;
				if (objectId == null)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "ObjectId could not be created from Uri");
					base.RenderPartialFailure(-2054976140, new Strings.IDs?(-381883412), ButtonDialogIcon.Warning);
					return;
				}
				UriFlags uriFlags = objectId.UriFlags;
				bool flag = (uriFlags & UriFlags.Sharepoint) == UriFlags.Sharepoint;
				bool flag2 = (uriFlags & UriFlags.Unc) == UriFlags.Unc;
				bool flag3 = DocumentLibraryUtilities.IsNavigationToWSSAllowed(base.UserContext);
				bool flag4 = DocumentLibraryUtilities.IsNavigationToUNCAllowed(base.UserContext);
				if (objectId.UriFlags == UriFlags.Other)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Link could not be classified as Sharepoint or UNC");
					if (flag4 && flag3)
					{
						base.RenderPartialFailure(1528018289, new Strings.IDs?(-743095750), ButtonDialogIcon.Information);
						return;
					}
					if (flag4)
					{
						base.RenderPartialFailure(-1758685302, new Strings.IDs?(-743095750), ButtonDialogIcon.Information);
						return;
					}
					if (flag3)
					{
						base.RenderPartialFailure(762710799, new Strings.IDs?(-743095750), ButtonDialogIcon.Information);
					}
					return;
				}
				else
				{
					if ((flag2 && !flag4) || (flag && !flag3))
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Segmentation failure. Access Denied");
						base.RenderPartialFailure(flag2 ? 813043446 : -972777689, new Strings.IDs?(-3401788), ButtonDialogIcon.Warning);
						return;
					}
					this.Writer.Write("<div id=uri>");
					this.Writer.Write(text);
					this.Writer.Write("</div><div id=uf>");
					this.Writer.Write((int)uriFlags);
					this.Writer.Write("</div><div id=oid>");
					this.Writer.Write(objectId.ToBase64String());
					this.Writer.Write("</div>");
					AttachmentPolicy attachmentPolicy = base.UserContext.AttachmentPolicy;
					this.Writer.Write("<div id=divFwr>");
					this.Writer.Write((attachmentPolicy.ForceWebReadyDocumentViewingFirst && DocumentLibraryUtilities.IsWebReadyDocument(objectId, base.UserContext)) ? 1 : 0);
					this.Writer.Write("</div>");
					return;
				}
			}
		}

		[OwaEvent("GetFavFldrs")]
		public void GetFavoriteFolders()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.GetFavoriteFolders");
			DocumentLibraryUtilities.RenderFavorites(this.Writer, base.UserContext);
			base.UserContext.UserOptions.IsDocumentFavoritesVisible = true;
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEventParameter("uri", typeof(string), true, false)]
		[OwaEvent("AddFav")]
		public void AddFavoriteLibrary()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.AddFavoriteLibrary");
			string[] array = (string[])base.GetParameter("uri");
			bool flag = false;
			this.Writer.Write("<div id=divAFavLibs>");
			for (int i = 0; i < array.Length; i++)
			{
				ClassifyResult documentLibraryObjectId = DocumentLibraryUtilities.GetDocumentLibraryObjectId(array[i], base.UserContext);
				if (documentLibraryObjectId == null)
				{
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Could not classify link: " + array[i]);
				}
				else
				{
					DocumentLibraryObjectId objectId = documentLibraryObjectId.ObjectId;
					if (objectId != null)
					{
						UriFlags uriFlags = objectId.UriFlags;
						DocumentLibrary documentLibrary = new DocumentLibrary();
						bool flag2 = DocumentLibraryUtilities.IsNavigationToWSSAllowed(base.UserContext);
						bool flag3 = DocumentLibraryUtilities.IsNavigationToUNCAllowed(base.UserContext);
						UriFlags uriFlags2 = uriFlags;
						switch (uriFlags2)
						{
						case UriFlags.Sharepoint:
						{
							if (!flag2)
							{
								throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
							}
							SharepointSession sharepointSession = base.UserContext.LogonIdentity.CreateSharepointSession(objectId);
							documentLibrary.DisplayName = (string.IsNullOrEmpty(sharepointSession.DisplayName) ? LocalizedStrings.GetNonEncoded(-527057840) : sharepointSession.DisplayName);
							documentLibrary.SiteName = sharepointSession.DisplayName;
							break;
						}
						case UriFlags.Unc:
						{
							if (!flag3)
							{
								throw new OwaSegmentationException("Access to Unc documents is disabled");
							}
							UncSession uncSession = base.UserContext.LogonIdentity.CreateUncSession(objectId);
							documentLibrary.DisplayName = uncSession.Title;
							documentLibrary.SiteName = uncSession.Uri.Host;
							break;
						}
						case UriFlags.Sharepoint | UriFlags.Unc:
						case UriFlags.DocumentLibrary:
							goto IL_34F;
						case UriFlags.SharepointDocumentLibrary:
						{
							if (!flag2)
							{
								throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
							}
							SharepointSession sharepointSession2 = base.UserContext.LogonIdentity.CreateSharepointSession(objectId);
							SharepointDocumentLibrary sharepointDocumentLibrary = SharepointDocumentLibrary.Read(sharepointSession2, objectId);
							documentLibrary.DisplayName = (string.IsNullOrEmpty(sharepointDocumentLibrary.Title) ? LocalizedStrings.GetNonEncoded(477016274) : sharepointDocumentLibrary.Title);
							documentLibrary.SiteName = sharepointSession2.DisplayName;
							break;
						}
						case UriFlags.UncDocumentLibrary:
						{
							if (!flag3)
							{
								throw new OwaSegmentationException("Access to Unc documents is disabled");
							}
							UncSession session = base.UserContext.LogonIdentity.CreateUncSession(objectId);
							UncDocumentLibrary uncDocumentLibrary = UncDocumentLibrary.Read(session, objectId);
							documentLibrary.DisplayName = (string.IsNullOrEmpty(uncDocumentLibrary.Title) ? LocalizedStrings.GetNonEncoded(477016274) : uncDocumentLibrary.Title);
							documentLibrary.SiteName = uncDocumentLibrary.Uri.Host;
							break;
						}
						default:
							switch (uriFlags2)
							{
							case UriFlags.SharepointFolder:
							{
								if (!flag2)
								{
									throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
								}
								SharepointSession sharepointSession3 = base.UserContext.LogonIdentity.CreateSharepointSession(objectId);
								SharepointDocumentLibraryFolder sharepointDocumentLibraryFolder = SharepointDocumentLibraryFolder.Read(sharepointSession3, objectId);
								documentLibrary.DisplayName = (string.IsNullOrEmpty(sharepointDocumentLibraryFolder.DisplayName) ? LocalizedStrings.GetNonEncoded(-527057840) : sharepointDocumentLibraryFolder.DisplayName);
								documentLibrary.SiteName = sharepointSession3.DisplayName;
								break;
							}
							case UriFlags.UncFolder:
							{
								if (!flag3)
								{
									throw new OwaSegmentationException("Access to Unc documents is disabled");
								}
								UncSession session2 = base.UserContext.LogonIdentity.CreateUncSession(objectId);
								UncDocumentLibraryFolder uncDocumentLibraryFolder = UncDocumentLibraryFolder.Read(session2, objectId);
								documentLibrary.DisplayName = (string.IsNullOrEmpty(uncDocumentLibraryFolder.DisplayName) ? LocalizedStrings.GetNonEncoded(-527057840) : uncDocumentLibraryFolder.DisplayName);
								documentLibrary.SiteName = uncDocumentLibraryFolder.Uri.Host;
								break;
							}
							default:
								goto IL_34F;
							}
							break;
						}
						documentLibrary.Type = uriFlags;
						documentLibrary.Uri = array[i];
						int num = 0;
						if (DocumentLibraryUtilities.AddFavorite(documentLibrary, base.UserContext, out num) && num <= 1000)
						{
							DocumentLibraryUtilities.RenderLibraryItem(this.Writer, documentLibrary, base.UserContext, true);
							flag = true;
							goto IL_3A0;
						}
						goto IL_3A0;
						IL_34F:
						throw new OwaNotSupportedException("Unhandled document library type");
					}
					ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "ObjectId could not be created from URI parameter: " + array[i]);
				}
				IL_3A0:;
			}
			this.Writer.Write("</div>");
			if (flag)
			{
				this.Writer.Write("<div id=divMsgFav>");
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(344777715));
				this.Writer.Write("</div><div id=divMsgTtl>");
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(803639727));
			}
			else
			{
				this.Writer.Write("<div id=divNoLib>");
			}
			this.Writer.Write("</div>");
		}

		[OwaEvent("DelFav")]
		[OwaEventParameter("uri", typeof(string))]
		public void DeleteFavoriteLibrary()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.DeleteFavoriteLibrary");
			string uri = (string)base.GetParameter("uri");
			DocumentLibraryUtilities.DeleteFavorite(uri, base.UserContext);
		}

		[OwaEvent("RenFav")]
		[OwaEventParameter("uri", typeof(string))]
		[OwaEventParameter("dn", typeof(string))]
		public void RenameFavoriteLibrary()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.RenameFavoriteLibrary");
			string uri = (string)base.GetParameter("uri");
			string newDisplayName = (string)base.GetParameter("dn");
			DocumentLibraryUtilities.RenameFavorite(uri, newDisplayName, base.UserContext);
		}

		[OwaEvent("PersistFavFldrs")]
		[OwaEventParameter("fV", typeof(bool))]
		public void PersistFavoriteFoldersBlock()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.PersistFavoriteFoldersBlock");
			bool isDocumentFavoritesVisible = (bool)base.GetParameter("fV");
			base.UserContext.UserOptions.IsDocumentFavoritesVisible = isDocumentFavoritesVisible;
			base.UserContext.UserOptions.CommitChanges();
		}

		[OwaEvent("SendByEmail")]
		[OwaEventParameter("uri", typeof(string), true, false)]
		public void SendByEmail()
		{
			ExTraceGlobals.DocumentsCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentLibraryEventHandler.SendByEmail");
			MessageItem messageItem = null;
			string[] array = (string[])base.GetParameter("uri");
			Stream stream = null;
			bool flag = DocumentLibraryUtilities.IsNavigationToWSSAllowed(base.UserContext);
			bool flag2 = DocumentLibraryUtilities.IsNavigationToUNCAllowed(base.UserContext);
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					ClassifyResult documentLibraryObjectId = DocumentLibraryUtilities.GetDocumentLibraryObjectId(array[i], base.UserContext);
					if (documentLibraryObjectId == null)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "Could not classify link: " + array[i]);
						throw new OwaEventHandlerException("Could not add attachment to email", LocalizedStrings.GetNonEncoded(1948229493), OwaEventHandlerErrorCode.SendByEmailError);
					}
					DocumentLibraryObjectId objectId = documentLibraryObjectId.ObjectId;
					if (objectId == null)
					{
						ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "ObjectId could not be created from URI parameter: " + array[i]);
						throw new OwaEventHandlerException("Could not add attachment to email", LocalizedStrings.GetNonEncoded(1948229493), OwaEventHandlerErrorCode.SendByEmailError);
					}
					UriFlags uriFlags = objectId.UriFlags;
					try
					{
						string text;
						switch (uriFlags)
						{
						case UriFlags.SharepointDocument:
						{
							if (!flag)
							{
								throw new OwaSegmentationException("Access to Sharepoint documents is disabled");
							}
							SharepointSession session = base.UserContext.LogonIdentity.CreateSharepointSession(objectId);
							SharepointDocument sharepointDocument = SharepointDocument.Read(session, objectId);
							stream = sharepointDocument.GetDocument();
							text = (string.IsNullOrEmpty(sharepointDocument.DisplayName) ? LocalizedStrings.GetNonEncoded(1797976510) : sharepointDocument.DisplayName);
							break;
						}
						case UriFlags.UncDocument:
						{
							if (!flag2)
							{
								throw new OwaSegmentationException("Access to Unc documents is disabled");
							}
							UncSession session2 = base.UserContext.LogonIdentity.CreateUncSession(objectId);
							UncDocument uncDocument = UncDocument.Read(session2, objectId);
							stream = uncDocument.GetDocument();
							text = Path.GetFileName(uncDocument.Uri.ToString());
							if (!string.IsNullOrEmpty(text))
							{
								text = HttpUtility.UrlDecode(text);
							}
							break;
						}
						default:
							throw new OwaNotSupportedException("Unhandled document library type");
						}
						if (messageItem == null)
						{
							messageItem = MessageItem.Create(base.UserContext.MailboxSession, base.UserContext.DraftsFolderId);
							messageItem[ItemSchema.ConversationIndexTracking] = true;
						}
						int num;
						AttachmentAddResult attachmentAddResult = AttachmentUtility.AddAttachmentFromStream(messageItem, text, null, stream, base.UserContext, out num);
						if (Globals.ArePerfCountersEnabled)
						{
							if (uriFlags == UriFlags.UncDocument)
							{
								OwaSingleCounters.UncRequests.Increment();
								OwaSingleCounters.UncBytes.IncrementBy((long)num);
							}
							else
							{
								OwaSingleCounters.WssRequests.Increment();
								OwaSingleCounters.WssBytes.IncrementBy((long)num);
							}
						}
						if (attachmentAddResult.ResultCode != AttachmentAddResultCode.NoError)
						{
							throw new OwaEventHandlerException("Could not add attachment to email. " + attachmentAddResult.Message, LocalizedStrings.GetNonEncoded(1948229493), OwaEventHandlerErrorCode.SendByEmailError);
						}
					}
					finally
					{
						if (stream != null)
						{
							stream.Close();
							stream = null;
						}
					}
				}
				messageItem.Save(SaveMode.ResolveConflicts);
				messageItem.Load();
				this.Writer.Write(messageItem.Id.ObjectId.ToBase64String());
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
		}

		public const string MethodClassifyLink = "ClassifyLink";

		public const string MethodGetFavoriteFolders = "GetFavFldrs";

		public const string MethodAddFavoriteLibrary = "AddFav";

		public const string MethodDeleteFavoriteLibrary = "DelFav";

		public const string MethodRenameFavoriteLibrary = "RenFav";

		public const string MethodDeleteFavoritesList = "DelFavList";

		public const string MethodPersistFavoriteFoldersBlock = "PersistFavFldrs";

		public const string DisplayName = "dn";

		public const string UriQueryParameter = "uri";

		public const string IsVisible = "fV";
	}
}
