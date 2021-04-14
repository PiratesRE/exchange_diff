using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SharepointDocument : SharepointDocumentLibraryItem, IDocument, IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal SharepointDocument(SharepointDocumentLibraryItemId id, SharepointSession session, XmlNode dataCache) : base(id, session, dataCache, SharepointDocumentSchema.Instance)
		{
		}

		public new static SharepointDocument Read(SharepointSession session, ObjectId id)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = id as SharepointDocumentLibraryItemId;
			if (sharepointDocumentLibraryItemId == null)
			{
				throw new ArgumentException("id");
			}
			if (sharepointDocumentLibraryItemId.UriFlags != UriFlags.SharepointDocument)
			{
				throw new ArgumentException("id");
			}
			return (SharepointDocument)SharepointDocumentLibraryItem.Read(session, id);
		}

		public VersionControl VersionControl
		{
			get
			{
				string text = this.TryGetProperty(SharepointDocumentSchema.CheckedOutUserId) as string;
				int versionId = (int)base[SharepointDocumentSchema.VersionId];
				return new VersionControl(!string.IsNullOrEmpty(text), text, versionId);
			}
		}

		public string FileType
		{
			get
			{
				return base.GetValueOrDefault<string>(SharepointDocumentSchema.FileType);
			}
		}

		public long Size
		{
			get
			{
				return (long)base[SharepointDocumentSchema.FileSize];
			}
		}

		public Stream GetDocument()
		{
			return Utils.DoSharepointTask<Stream>(this.Session.Identity, base.Id, (SharepointSiteId)base.Id, true, Utils.MethodType.GetStream, delegate
			{
				Stream stream = null;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.Uri);
				httpWebRequest.KeepAlive = false;
				httpWebRequest.Headers.Set("Pragma", "no-cache");
				httpWebRequest.Headers.Set("Depth", "0");
				httpWebRequest.ContentType = "text/xml";
				httpWebRequest.ContentLength = 0L;
				httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				stream = httpWebResponse.GetResponseStream();
				bool flag = false;
				try
				{
					SharepointDocumentLibraryItemId sharepointDocumentLibraryItemId = (SharepointDocumentLibraryItemId)base.Id;
					XmlNode nodeForItem = SharepointDocumentLibraryItem.GetNodeForItem(this.Session, sharepointDocumentLibraryItemId);
					SharepointDocument sharepointDocument = new SharepointDocument(sharepointDocumentLibraryItemId, this.Session, nodeForItem);
					if (sharepointDocument.VersionControl.TipVersion != this.VersionControl.TipVersion)
					{
						throw new DocumentModifiedException(base.Id, this.Uri.ToString());
					}
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						stream.Dispose();
						stream = null;
					}
				}
				return stream;
			});
		}

		public override SharepointItemType ItemType
		{
			get
			{
				return SharepointItemType.Document;
			}
		}

		public override object TryGetProperty(PropertyDefinition propDef)
		{
			if (propDef == SharepointDocumentSchema.VersionControl)
			{
				return this.VersionControl;
			}
			return base.TryGetProperty(propDef);
		}
	}
}
