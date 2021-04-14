using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.AirSync
{
	internal class DocumentLibraryFetchProvider : DisposeTrackableBase, IItemOperationsProvider, IReusable, IMultipartResponse
	{
		internal DocumentLibraryFetchProvider(IAirSyncUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			this.user = user;
			AirSyncCounters.NumberOfDocumentLibraryFetches.Increment();
		}

		public void Reset()
		{
			this.multiPartResponse = false;
			this.partNumber = 0;
			this.minRange = 0;
			this.maxRange = 0;
			this.documentSize = 0;
			this.version = null;
			this.linkId = null;
			this.rangeSpecified = false;
			if (this.outStream != null)
			{
				this.outStream.Dispose();
				this.outStream = null;
			}
		}

		public void BuildResponse(XmlNode responseNode, int partNumber)
		{
			this.partNumber = partNumber;
			this.multiPartResponse = true;
			this.BuildResponse(responseNode);
		}

		public Stream GetResponseStream()
		{
			MemoryStream result = this.outStream;
			this.outStream = null;
			return result;
		}

		public bool RightsManagementSupport
		{
			get
			{
				return false;
			}
		}

		public void ParseRequest(XmlNode fetchNode)
		{
			XmlNode xmlNode = fetchNode["LinkId", "DocumentLibrary:"];
			if (xmlNode == null)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoLinkIdInDocFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
			}
			this.linkId = xmlNode.InnerText;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Fetch request linkId is {0}.", this.linkId);
			XmlNode xmlNode2 = fetchNode["Options", "ItemOperations:"];
			if (xmlNode2 != null)
			{
				int num = 0;
				XmlNode xmlNode3 = xmlNode2["Range", "ItemOperations:"];
				if (xmlNode3 != null)
				{
					string[] array = xmlNode3.InnerText.Split(new char[]
					{
						'-'
					});
					AirSyncDiagnostics.Assert(array.Length == 2);
					this.minRange = int.Parse(array[0], CultureInfo.InvariantCulture);
					this.maxRange = int.Parse(array[1], CultureInfo.InvariantCulture);
					AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, this, "Fetch command minRange is {0} and maxRange is {1}.", this.minRange, this.maxRange);
					if (this.minRange > this.maxRange)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MinMoreThanMaxOnDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
					}
					this.rangeSpecified = true;
					num++;
				}
				XmlNode xmlNode4 = xmlNode2["UserName", "ItemOperations:"];
				if (xmlNode4 != null)
				{
					AirSyncSecureStringXmlNode airSyncSecureStringXmlNode = xmlNode2["Password", "ItemOperations:"] as AirSyncSecureStringXmlNode;
					if (airSyncSecureStringXmlNode == null)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadCredsOnDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
					}
					this.userName = xmlNode4.InnerText;
					this.userPassword = airSyncSecureStringXmlNode.DetachSecureData();
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Fetch command userName is {0}.", this.userName);
					num += 2;
				}
				if (num != xmlNode2.ChildNodes.Count)
				{
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadOptionsInDocFetch");
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
				}
				if (!string.Equals(this.user.Identity.AuthenticationType, "Basic", StringComparison.OrdinalIgnoreCase) && this.userPassword == null)
				{
					if (GlobalSettings.EnableCredentialRequest && this.user.Context.Request.Version >= 121)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedPromptForCredsToProxy");
						throw new AirSyncPermanentException(StatusCode.ItemOperations_CredentialsRequired, false);
					}
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedCredsToProxy");
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
				}
			}
		}

		public void Execute()
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "DocumentLibrary Fetch command received. Processing request...");
			Uri uri = null;
			try
			{
				uri = new Uri(this.linkId);
			}
			catch (UriFormatException innerException)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: Bad document Uri {0} was specified!", new object[]
				{
					this.linkId
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, innerException, false);
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Processing Fetch command with Uri {0}.", uri.AbsoluteUri);
			if (!DocumentLibraryUtility.IsTrustedProtocol(uri.Scheme))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: untrusted protocol: {0}!", new object[]
				{
					uri.Scheme
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadProtocolInDocFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
			}
			if (!DocumentLibraryUtility.IsInternalUri(uri))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: Uri must be internal: {0}!", new object[]
				{
					uri.Host
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ExternalProtocolInDocFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
			}
			AuthenticationContext authenticationContext = new AuthenticationContext();
			try
			{
				IPrincipal principal;
				if (this.userPassword == null)
				{
					principal = this.user.WindowsPrincipal;
					if (principal == null)
					{
						if (GlobalSettings.EnableCredentialRequest && this.user.Context.Request.Version >= 121)
						{
							this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedPromptForCredsToProxy2");
							throw new AirSyncPermanentException(StatusCode.ItemOperations_CredentialsRequired, false);
						}
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedCredsToProxy2");
						throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
					}
				}
				else
				{
					SecurityStatus securityStatus = authenticationContext.LogonUser(this.userName, this.userPassword);
					this.userPassword.Dispose();
					this.userPassword = null;
					if (securityStatus != SecurityStatus.OK)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: Authentication failed with status {0}.", new object[]
						{
							securityStatus
						}));
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, string.Format(CultureInfo.InvariantCulture, "AuthenticationErrorStatus{0}", new object[]
						{
							securityStatus
						}));
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ServerError, null, false);
					}
					principal = new WindowsPrincipal(authenticationContext.Identity);
				}
				ClassifyResult classifyResult = LinkClassifier.ClassifyLinks(principal, new Uri[]
				{
					uri
				})[0];
				if (classifyResult.Error != ClassificationError.None)
				{
					AirSyncDiagnostics.TraceDebug<string, ClassificationError>(ExTraceGlobals.RequestsTracer, this, "The LinkClassifier failed to classify the link {0}, returned {1}", this.linkId, classifyResult.Error);
					switch (classifyResult.Error)
					{
					case ClassificationError.ConnectionFailed:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassConnFailedInDocFetch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_Conflict, null, false);
					case ClassificationError.AccessDenied:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassDeniedInDocFetch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ServerError, null, false);
					case ClassificationError.ObjectNotFound:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassNotFoundInDocFetch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ClientServerConversion, null, false);
					case ClassificationError.UriTypeNotSupported:
					case ClassificationError.InvalidUri:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassBadUriInDocFetch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ProtocolError, null, false);
					}
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassFailureInDocFetch");
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false);
				}
				IDocument document = null;
				UncSession uncSession = null;
				SharepointSession sharepointSession = null;
				if ((classifyResult.UriFlags & UriFlags.UncDocument) == UriFlags.UncDocument)
				{
					if (!DocumentLibraryUtility.IsUncAccessEnabled(this.user))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AccessDeniedInDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
					}
					if (DocumentLibraryUtility.IsBlockedHostName(uri.Host))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "HostBlockedInDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
					}
					uncSession = UncSession.Open(classifyResult.ObjectId, principal);
				}
				else
				{
					if ((classifyResult.UriFlags & UriFlags.SharepointDocument) != UriFlags.SharepointDocument)
					{
						AirSyncDiagnostics.TraceDebug<string, UriFlags>(ExTraceGlobals.RequestsTracer, this, "The Uri {0} of type {1} is not supported for Fetch", this.linkId, classifyResult.UriFlags);
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocFetch2");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false);
					}
					if (!DocumentLibraryUtility.IsWssAccessEnabled(this.user))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "WssDeniedInDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
					}
					if (DocumentLibraryUtility.IsBlockedHostName(uri.Host))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "HostBlockedInDocFetch2");
						throw new AirSyncPermanentException(StatusCode.Sync_Retry, false);
					}
					this.user.Context.ProtocolLogger.IncrementValue(ProtocolLoggerData.SharePointDocs);
					sharepointSession = SharepointSession.Open(classifyResult.ObjectId, principal);
				}
				try
				{
					if (uncSession != null)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Reading UNC document...");
						document = UncDocument.Read(uncSession, classifyResult.ObjectId);
					}
					else
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Reading Sharepoint document...");
						AirSyncDiagnostics.Assert(sharepointSession != null);
						document = SharepointDocument.Read(sharepointSession, classifyResult.ObjectId);
					}
				}
				catch (ObjectNotFoundException innerException2)
				{
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NotFoundInDocFetch");
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException2, false);
				}
				catch (AccessDeniedException innerException3)
				{
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AccessDeniedInDocFetch2");
					throw new AirSyncPermanentException(StatusCode.Sync_ServerError, innerException3, false);
				}
				this.documentSize = (int)document.Size;
				if (this.documentSize == 0)
				{
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "The file {0} was found to be empty!", this.linkId);
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "EmptyDocInDocFetch");
					throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_NotificationGUID, null, false);
				}
				object obj = document.TryGetProperty(DocumentLibraryItemSchema.LastModifiedDate);
				if (obj is PropertyError)
				{
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "GeneralErrorInDocFetch");
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false);
				}
				ExDateTime exDateTime;
				if (obj is DateTime)
				{
					exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)obj);
				}
				else
				{
					exDateTime = (ExDateTime)obj;
				}
				this.version = exDateTime.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
				if (this.rangeSpecified && this.minRange >= this.documentSize)
				{
					AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, this, "The minimum range specified {0} is greater than the document size {1}", this.minRange, this.documentSize);
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadMinSizeInDocFetch");
					throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ObjectNotFound, null, false);
				}
				using (Stream document2 = document.GetDocument())
				{
					int num;
					if (this.rangeSpecified)
					{
						num = this.maxRange - this.minRange + 1;
					}
					else
					{
						num = this.documentSize;
					}
					if (uncSession != null)
					{
						this.user.Context.ProtocolLogger.IncrementValue(ProtocolLoggerData.UNCFiles);
						this.user.Context.ProtocolLogger.IncrementValueBy(ProtocolLoggerData.UNCBytes, num);
					}
					if (sharepointSession != null)
					{
						this.user.Context.ProtocolLogger.IncrementValue(ProtocolLoggerData.SharePointDocs);
						this.user.Context.ProtocolLogger.IncrementValueBy(ProtocolLoggerData.SharePointBytes, num);
					}
					this.outStream = new MemoryStream(num);
					if (num > GlobalSettings.MaxDocumentDataSize)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "DocTooBigInDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_NotificationsNotProvisioned, false);
					}
					try
					{
						StreamHelper.CopyStream(document2, this.outStream, this.minRange, num);
					}
					catch (IOException innerException4)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "IOErrorInDocFetch");
						throw new AirSyncPermanentException(StatusCode.Sync_FolderHierarchyRequired, innerException4, false);
					}
				}
			}
			catch (UnknownErrorException innerException5)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "IOErrorInDocFetch2");
				throw new AirSyncPermanentException(StatusCode.Sync_FolderHierarchyRequired, innerException5, false);
			}
			catch (DocumentModifiedException innerException6)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "IOErrorInDocFetch3");
				throw new AirSyncPermanentException(StatusCode.Sync_FolderHierarchyRequired, innerException6, false);
			}
			catch (DocumentStreamAccessDeniedException innerException7)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AccessDeniedInDocFetch3");
				throw new AirSyncPermanentException(StatusCode.Sync_ServerError, innerException7, false);
			}
			catch (ObjectMovedOrDeletedException innerException8)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NotFoundInDocFetch2");
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException8, false);
			}
			catch (DocumentLibraryException innerException9)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "GeneralErrorInDocFetch2");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, innerException9, false);
			}
			finally
			{
				if (authenticationContext != null)
				{
					authenticationContext.Dispose();
					authenticationContext = null;
				}
			}
		}

		public void BuildResponse(XmlNode responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("LinkId", "DocumentLibrary:");
			XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("Properties", "ItemOperations:");
			XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("Version", "ItemOperations:");
			if (this.rangeSpecified)
			{
				XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("Range", "ItemOperations:");
				xmlNode6.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
				{
					this.minRange,
					(long)this.minRange + this.outStream.Length - 1L
				});
				xmlNode4.AppendChild(xmlNode6);
				XmlNode xmlNode7 = responseNode.OwnerDocument.CreateElement("Total", "ItemOperations:");
				xmlNode7.InnerText = this.documentSize.ToString(CultureInfo.InvariantCulture);
				xmlNode4.AppendChild(xmlNode7);
			}
			if (this.multiPartResponse)
			{
				XmlNode xmlNode8 = responseNode.OwnerDocument.CreateElement("Part", "ItemOperations:");
				xmlNode8.InnerText = this.partNumber.ToString(CultureInfo.InvariantCulture);
				xmlNode4.AppendChild(xmlNode8);
			}
			else
			{
				XmlNode xmlNode9 = responseNode.OwnerDocument.CreateElement("Data", "ItemOperations:");
				xmlNode9.InnerText = Convert.ToBase64String(this.outStream.GetBuffer(), 0, (int)this.outStream.Length);
				xmlNode4.AppendChild(xmlNode9);
			}
			xmlNode5.InnerText = this.version;
			xmlNode4.AppendChild(xmlNode5);
			xmlNode2.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode2);
			xmlNode3.InnerText = this.linkId;
			xmlNode.AppendChild(xmlNode3);
			xmlNode.AppendChild(xmlNode4);
			responseNode.AppendChild(xmlNode);
		}

		public void BuildErrorResponse(string statusCode, XmlNode responseNode, ProtocolLogger protocolLogger)
		{
			if (protocolLogger != null)
			{
				protocolLogger.IncrementValue(ProtocolLoggerData.IOFetchDocErrors);
			}
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			xmlNode2.InnerText = statusCode;
			xmlNode.AppendChild(xmlNode2);
			if (!string.IsNullOrEmpty(this.linkId))
			{
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("LinkId", "DocumentLibrary:");
				xmlNode3.InnerText = this.linkId;
				xmlNode.AppendChild(xmlNode3);
			}
			responseNode.AppendChild(xmlNode);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.outStream != null)
				{
					this.outStream.Dispose();
				}
				this.outStream = null;
				if (this.userPassword != null)
				{
					this.userPassword.Dispose();
					this.userPassword = null;
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DocumentLibraryFetchProvider>(this);
		}

		private int minRange;

		private int maxRange;

		private bool multiPartResponse;

		private int partNumber;

		private int documentSize;

		private string version;

		private string linkId;

		private MemoryStream outStream;

		private bool rangeSpecified;

		private IAirSyncUser user;

		private string userName;

		private SecureString userPassword;
	}
}
