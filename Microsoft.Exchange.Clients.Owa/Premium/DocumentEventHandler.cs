using System;
using System.Globalization;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class DocumentEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(UncDocumentEventHandler));
			OwaEventRegistry.RegisterHandler(typeof(SharepointDocumentEventHandler));
		}

		[OwaEvent("GetDoc")]
		[OwaEventParameter("TranslatedURL", typeof(string), false, true)]
		[OwaEventVerb(OwaEventVerb.Get)]
		[OwaEventParameter("URL", typeof(string))]
		[OwaEventParameter("id", typeof(string), false, true)]
		[OwaEventParameter("allowLevel2", typeof(int), false, true)]
		public void GetDocument()
		{
			bool flag = false;
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "DocumentEventHandler.GetDocument");
			base.ShowErrorInPage = true;
			base.DontWriteHeaders = true;
			HttpContext httpContext = base.OwaContext.HttpContext;
			if (!DocumentLibraryUtilities.IsDocumentsAccessEnabled(base.UserContext))
			{
				throw new OwaSegmentationException("Access to this document library is disabled");
			}
			string text = (string)base.GetParameter("id");
			string s = (string)base.GetParameter("URL");
			DocumentLibraryObjectId documentLibraryObjectId = DocumentLibraryUtilities.CreateDocumentLibraryObjectId(base.OwaContext);
			if (documentLibraryObjectId == null)
			{
				return;
			}
			try
			{
				this.DataBind(documentLibraryObjectId);
			}
			finally
			{
				if (this.stream == null)
				{
					this.Dispose();
				}
			}
			if (this.stream == null)
			{
				return;
			}
			UserContext userContext = base.OwaContext.UserContext;
			AttachmentPolicy.Level levelForAttachment = AttachmentLevelLookup.GetLevelForAttachment(Path.GetExtension(this.fileName), this.contentType, userContext);
			if (base.IsParameterSet("allowLevel2"))
			{
				flag = true;
			}
			if (levelForAttachment == AttachmentPolicy.Level.Block)
			{
				string errorDescription = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetNonEncoded(1280363351), new object[]
				{
					this.fileName
				});
				Utilities.TransferToErrorPage(base.OwaContext, errorDescription, null, ThemeFileId.ButtonDialogInfo, true);
				return;
			}
			if (levelForAttachment == AttachmentPolicy.Level.ForceSave && !flag)
			{
				string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "ns");
				string text2 = string.Concat(new string[]
				{
					"<br> <a onclick=\"return false;\" href=\"ev.owa?ns=",
					queryStringParameter,
					"&ev=GetDoc&allowLevel2=1&URL=",
					Utilities.UrlEncode(s),
					"&id=",
					Utilities.UrlEncode(documentLibraryObjectId.ToBase64String()),
					Utilities.GetCanaryRequestParameter(),
					"\">",
					Utilities.HtmlEncode(this.fileName),
					"</a>"
				});
				string errorDetailedDescription = string.Format(CultureInfo.InvariantCulture, LocalizedStrings.GetHtmlEncoded(-625229753), new object[]
				{
					text2
				});
				Utilities.TransferToErrorPage(base.OwaContext, LocalizedStrings.GetHtmlEncoded(-226672911), errorDetailedDescription, ThemeFileId.ButtonDialogInfo, true, true);
				return;
			}
			int num = AttachmentHandler.SendDocumentContentToHttpStream(httpContext, this.stream, this.fileName, DocumentEventHandler.CalculateFileExtension(this.fileName), this.contentType);
			if (this.contentType != null && this.contentType.Equals("application/x-zip-compressed", StringComparison.OrdinalIgnoreCase))
			{
				Utilities.DisableContentEncodingForThisResponse(base.OwaContext.HttpContext.Response);
			}
			if (Globals.ArePerfCountersEnabled)
			{
				if ((documentLibraryObjectId.UriFlags & UriFlags.Sharepoint) != (UriFlags)0)
				{
					OwaSingleCounters.WssBytes.IncrementBy((long)num);
					OwaSingleCounters.WssRequests.Increment();
					return;
				}
				if ((documentLibraryObjectId.UriFlags & UriFlags.Unc) != (UriFlags)0)
				{
					OwaSingleCounters.UncBytes.IncrementBy((long)num);
					OwaSingleCounters.UncRequests.Increment();
				}
			}
		}

		protected abstract void PreDataBind();

		protected void DataBind(DocumentLibraryObjectId objectId)
		{
			this.PreDataBind();
			IDocument document;
			try
			{
				document = DocumentLibraryUtilities.LoadDocumentLibraryItem(objectId, base.UserContext);
			}
			catch (AccessDeniedException)
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI access is denied.");
				Utilities.TransferToErrorPage(base.OwaContext, LocalizedStrings.GetNonEncoded(234621291));
				return;
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.DocumentsTracer.TraceDebug(0L, "URI object not found.");
				Utilities.TransferToErrorPage(base.OwaContext, LocalizedStrings.GetNonEncoded(1599334062));
				return;
			}
			if (document == null)
			{
				throw new OwaInvalidRequestException("objectId is invalid for unc/wss document");
			}
			object obj = document.TryGetProperty(this.contentTypePropertyDefinition);
			if (!(obj is PropertyError))
			{
				this.contentType = (obj as string);
			}
			else
			{
				this.contentType = string.Empty;
			}
			this.stream = document.GetDocument();
			this.fileName = Path.GetFileName(document.Uri.ToString());
			if (!string.IsNullOrEmpty(this.fileName))
			{
				this.fileName = HttpUtility.UrlDecode(this.fileName);
			}
		}

		private static string CalculateFileExtension(string fileName)
		{
			if (fileName == null)
			{
				return null;
			}
			int num = fileName.LastIndexOf('.');
			if (num >= 0 && num < fileName.Length - 1)
			{
				return fileName.Substring(num);
			}
			return string.Empty;
		}

		public const string MethodGetDocument = "GetDoc";

		public const string DocumentIdQueryParameter = "id";

		public const string DocumentLevel2AllowParameter = "allowLevel2";

		protected string contentType;

		protected string fileName;

		protected Stream stream;

		protected PropertyDefinition contentTypePropertyDefinition;
	}
}
