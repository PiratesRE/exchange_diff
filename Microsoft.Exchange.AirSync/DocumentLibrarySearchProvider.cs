using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.AirSync
{
	internal class DocumentLibrarySearchProvider : DisposeTrackableBase, ISearchProvider
	{
		internal DocumentLibrarySearchProvider(IAirSyncUser user)
		{
			this.user = user;
			AirSyncCounters.NumberOfDocumentLibrarySearches.Increment();
		}

		public int NumberResponses
		{
			get
			{
				return this.results;
			}
		}

		public bool RightsManagementSupport
		{
			get
			{
				return false;
			}
		}

		public void ParseOptions(XmlElement optionsNode)
		{
			if (optionsNode != null)
			{
				int num = 0;
				XmlNode xmlNode = optionsNode["Range", "Search:"];
				if (xmlNode != null)
				{
					string[] array = xmlNode.InnerText.Split(new char[]
					{
						'-'
					});
					AirSyncDiagnostics.Assert(array.Length == 2);
					this.minRange = int.Parse(array[0], CultureInfo.InvariantCulture);
					this.maxRange = int.Parse(array[1], CultureInfo.InvariantCulture);
					AirSyncDiagnostics.TraceDebug<int, int>(ExTraceGlobals.RequestsTracer, this, "Search command minRange is {0} and maxRange is {1}.", this.minRange, this.maxRange);
					if (this.minRange > this.maxRange)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MinMoreThanMaxOnDocSearch");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
					}
					this.rangeSpecified = true;
					num++;
				}
				XmlNode xmlNode2 = optionsNode["UserName", "Search:"];
				if (xmlNode2 != null)
				{
					AirSyncSecureStringXmlNode airSyncSecureStringXmlNode = optionsNode["Password", "Search:"] as AirSyncSecureStringXmlNode;
					if (airSyncSecureStringXmlNode == null)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadCredsOnDocSearch");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
					}
					this.userName = xmlNode2.InnerText;
					this.userPassword = airSyncSecureStringXmlNode.DetachSecureData();
					AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Search command userName is {0}.", this.userName);
					num += 2;
				}
				if (num != optionsNode.ChildNodes.Count)
				{
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadOptionsInDocSearch");
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
				}
				if (!string.Equals(this.user.Identity.AuthenticationType, "Basic", StringComparison.OrdinalIgnoreCase) && this.userPassword == null)
				{
					if (GlobalSettings.EnableCredentialRequest && this.user.Context.Request.Version >= 121)
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedPromptForCredsToProxy");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, false);
					}
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedCredsToProxy");
					throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
				}
			}
		}

		public void ParseQueryNode(XmlElement queryNode)
		{
			if (queryNode == null)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoOptionsInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
			}
			XmlNode xmlNode = queryNode["EqualTo", "Search:"];
			if (xmlNode == null)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoEqualToNodeInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
			}
			XmlNode xmlNode2 = xmlNode["Value", "Search:"];
			this.linkId = xmlNode2.InnerText;
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Search command linkId is {0}.", this.linkId);
		}

		public void Execute()
		{
			Uri uri = null;
			try
			{
				uri = new Uri(this.linkId);
			}
			catch (UriFormatException innerException)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "DocumentLibrarySearchProvider: Bad Uri {0} was specified!", new object[]
				{
					this.linkId
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, innerException, false);
			}
			AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Processing Search command with Uri {0}.", uri.AbsoluteUri);
			if (!DocumentLibraryUtility.IsTrustedProtocol(uri.Scheme))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: untrusted protocol: {0}!", new object[]
				{
					uri.Scheme
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadProtocolInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
			}
			if (!DocumentLibraryUtility.IsInternalUri(uri))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "ItemOperationsFetchProvider: Uri must be internal: {0}!", new object[]
				{
					uri.Host
				}));
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ExternalProtocolInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
			}
			AuthenticationContext authenticationContext = new AuthenticationContext();
			try
			{
				IPrincipal principal;
				if (this.userPassword == null)
				{
					principal = this.user.WindowsPrincipal;
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "DocumentLibrarySearchProvider: Created IPrincipal object from IAirSyncUser .");
					if (principal == null)
					{
						if (GlobalSettings.EnableCredentialRequest && this.user.Context.Request.Version >= 121)
						{
							this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NeedPromptForCredsToProxy2");
							throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, false);
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
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, string.Format(CultureInfo.InvariantCulture, "DocumentLibrarySearchProvider: Authentication failed with status {0}.", new object[]
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
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "DocumentLibrarySearchProvider: Created IPrincipal using AuthenticationContext LogonUser method.");
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
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassConnFailedInDocSearch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_Conflict, null, false);
					case ClassificationError.AccessDenied:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassDeniedInDocSearch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ServerError, null, false);
					case ClassificationError.ObjectNotFound:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassNotFoundInDocSearch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ClientServerConversion, null, false);
					case ClassificationError.UriTypeNotSupported:
					case ClassificationError.InvalidUri:
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassBadUriInDocSearch");
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ProtocolError, null, false);
					}
					this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "LinkClassFailureInDocSearch");
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false);
				}
				AirSyncDiagnostics.TraceDebug<UriFlags>(ExTraceGlobals.RequestsTracer, this, "Search command document type is {0}.", classifyResult.UriFlags);
				SharepointSession sharepointSession = null;
				UncSession uncSession = null;
				IDocumentLibraryItem documentLibraryItem = null;
				IDocumentLibrary documentLibrary = null;
				IDocumentLibraryFolder documentLibraryFolder = null;
				if ((classifyResult.UriFlags & UriFlags.Unc) == UriFlags.Unc)
				{
					if (!DocumentLibraryUtility.IsUncAccessEnabled(this.user))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AccessDeniedInDocSearch");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
					}
					if (DocumentLibraryUtility.IsBlockedHostName(uri.Host))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "HostBlockedInDocSearch");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
					}
					uncSession = UncSession.Open(classifyResult.ObjectId, principal);
				}
				else
				{
					if ((classifyResult.UriFlags & UriFlags.Sharepoint) != UriFlags.Sharepoint)
					{
						AirSyncDiagnostics.TraceDebug<string, UriFlags>(ExTraceGlobals.RequestsTracer, this, "The Uri {0} of type {1} is not supported for Search", this.linkId, classifyResult.UriFlags);
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocSearch2");
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false);
					}
					if (!DocumentLibraryUtility.IsWssAccessEnabled(this.user))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "WssDeniedInDocSearch");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
					}
					if (DocumentLibraryUtility.IsBlockedHostName(uri.Host))
					{
						this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "HostBlockedInDocSearch2");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, false);
					}
					sharepointSession = SharepointSession.Open(classifyResult.ObjectId, principal);
				}
				UriFlags uriFlags = classifyResult.UriFlags;
				switch (uriFlags)
				{
				case UriFlags.Sharepoint:
				case UriFlags.Unc:
					goto IL_645;
				case UriFlags.Sharepoint | UriFlags.Unc:
				case UriFlags.DocumentLibrary:
					break;
				case UriFlags.SharepointDocumentLibrary:
					documentLibrary = SharepointDocumentLibrary.Read(sharepointSession, classifyResult.ObjectId);
					goto IL_645;
				case UriFlags.UncDocumentLibrary:
					documentLibrary = UncDocumentLibrary.Read(uncSession, classifyResult.ObjectId);
					goto IL_645;
				default:
					switch (uriFlags)
					{
					case UriFlags.SharepointDocument:
						documentLibraryItem = SharepointDocument.Read(sharepointSession, classifyResult.ObjectId);
						goto IL_645;
					case UriFlags.UncDocument:
						documentLibraryItem = UncDocument.Read(uncSession, classifyResult.ObjectId);
						goto IL_645;
					default:
						switch (uriFlags)
						{
						case UriFlags.SharepointFolder:
							documentLibraryFolder = SharepointDocumentLibraryFolder.Read(sharepointSession, classifyResult.ObjectId);
							goto IL_645;
						case UriFlags.UncFolder:
							documentLibraryFolder = UncDocumentLibraryFolder.Read(uncSession, classifyResult.ObjectId);
							goto IL_645;
						}
						break;
					}
					break;
				}
				AirSyncDiagnostics.TraceDebug<string, UriFlags>(ExTraceGlobals.RequestsTracer, this, "The Uri {0} of type {1} is not supported for Search", this.linkId, classifyResult.UriFlags);
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocSearch3");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false);
				IL_645:
				if (documentLibrary != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Calling GetView() for document library...");
					this.tableView = documentLibrary.GetView(null, DocumentLibrarySearchProvider.sortBy, DocumentLibraryQueryOptions.FoldersAndFiles, DocumentLibrarySearchProvider.documentLibraryProps);
					this.rootProps = documentLibrary.GetProperties(DocumentLibrarySearchProvider.documentLibraryProps);
				}
				else if (documentLibraryFolder != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Calling GetView() for document folder...");
					this.tableView = documentLibraryFolder.GetView(null, DocumentLibrarySearchProvider.sortBy, DocumentLibraryQueryOptions.FoldersAndFiles, DocumentLibrarySearchProvider.documentLibraryProps);
					this.rootProps = documentLibraryFolder.GetProperties(DocumentLibrarySearchProvider.documentLibraryProps);
				}
				else if (documentLibraryItem != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Getting properties for the document item...");
					this.rootProps = documentLibraryItem.GetProperties(DocumentLibrarySearchProvider.documentLibraryProps);
				}
				else if (sharepointSession != null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Calling GetView() on Sharepoint session...");
					this.tableView = sharepointSession.GetView(ListBaseType.DocumentLibrary, DocumentLibrarySearchProvider.documentLibraryProps);
					this.rootProps = new object[DocumentLibrarySearchProvider.airSyncProps.GetLength(0)];
					this.rootProps[0] = sharepointSession.Uri;
					this.rootProps[1] = sharepointSession.DisplayName;
					this.rootProps[2] = true;
				}
				else
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Calling GetView() on UNC session...");
					this.tableView = uncSession.GetView(null, null, DocumentLibrarySearchProvider.documentLibraryProps);
					this.rootProps = new object[DocumentLibrarySearchProvider.airSyncProps.GetLength(0)];
					this.rootProps[0] = uncSession.Uri;
					this.rootProps[1] = uncSession.Title;
					this.rootProps[2] = true;
				}
			}
			catch (UnknownErrorException innerException2)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadLinkInDocSearch4");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, innerException2, false);
			}
			catch (AccessDeniedException innerException3)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AccessDeniedInDocSearch2");
				throw new AirSyncPermanentException(StatusCode.Sync_ServerError, innerException3, false);
			}
			catch (ConnectionException innerException4)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ConnFailedInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_Conflict, innerException4, false);
			}
			catch (ObjectNotFoundException innerException5)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NotFoundInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException5, false);
			}
			catch (DocumentLibraryException innerException6)
			{
				this.user.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "FailureInDocSearch");
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, innerException6, false);
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

		public void BuildResponse(XmlElement responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Status", "Search:");
			responseNode.AppendChild(xmlNode);
			xmlNode.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			if (this.minRange == 0)
			{
				this.BuildResultNode(responseNode, this.rootProps);
			}
			if (this.tableView != null)
			{
				int num = this.maxRange - this.minRange + 1;
				if (num > this.results)
				{
					this.tableView.SeekToOffset(SeekReference.OriginBeginning, (this.minRange > 0) ? (this.minRange - 1) : this.minRange);
					object[][] rows = this.tableView.GetRows(num - this.results);
					for (int i = 0; i < rows.GetLength(0); i++)
					{
						this.BuildResultNode(responseNode, rows[i]);
					}
				}
				this.total += this.tableView.EstimatedRowCount;
			}
			if (this.results == 0)
			{
				XmlNode newChild = responseNode.OwnerDocument.CreateElement("Result", "Search:");
				responseNode.AppendChild(newChild);
				return;
			}
			if (this.rangeSpecified)
			{
				XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Range", "Search:");
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("Total", "Search:");
				xmlNode2.InnerText = this.minRange.ToString(CultureInfo.InvariantCulture) + "-" + (this.minRange + this.results - 1).ToString(CultureInfo.InvariantCulture);
				responseNode.AppendChild(xmlNode2);
				xmlNode3.InnerText = this.total.ToString(CultureInfo.InvariantCulture);
				responseNode.AppendChild(xmlNode3);
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.userPassword != null)
			{
				this.userPassword.Dispose();
				this.userPassword = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DocumentLibrarySearchProvider>(this);
		}

		private void BuildResultNode(XmlNode responseNode, object[] row)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Result", "Search:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Properties", "Search:");
			xmlNode.AppendChild(xmlNode2);
			for (int i = 0; i < DocumentLibrarySearchProvider.documentLibraryProps.GetLength(0); i++)
			{
				object obj = row[i];
				string innerText = null;
				if (obj != null && !(obj is PropertyError))
				{
					string key;
					if ((key = DocumentLibrarySearchProvider.airSyncProps[i]) == null)
					{
						goto IL_1A7;
					}
					if (<PrivateImplementationDetails>{FFCBDD4C-C702-4D33-87A9-71BBCA1064B7}.$$method0x6000711-1 == null)
					{
						<PrivateImplementationDetails>{FFCBDD4C-C702-4D33-87A9-71BBCA1064B7}.$$method0x6000711-1 = new Dictionary<string, int>(8)
						{
							{
								"LinkId",
								0
							},
							{
								"DisplayName",
								1
							},
							{
								"ContentType",
								2
							},
							{
								"ContentLength",
								3
							},
							{
								"IsFolder",
								4
							},
							{
								"IsHidden",
								5
							},
							{
								"CreationDate",
								6
							},
							{
								"LastModifiedDate",
								7
							}
						};
					}
					int num;
					if (!<PrivateImplementationDetails>{FFCBDD4C-C702-4D33-87A9-71BBCA1064B7}.$$method0x6000711-1.TryGetValue(key, out num))
					{
						goto IL_1A7;
					}
					switch (num)
					{
					case 0:
						innerText = ((Uri)obj).OriginalString;
						break;
					case 1:
					case 2:
					case 3:
						innerText = obj.ToString();
						break;
					case 4:
					case 5:
						innerText = (((bool)obj) ? "1" : "0");
						break;
					case 6:
					case 7:
					{
						ExDateTime exDateTime;
						if (obj is DateTime)
						{
							exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, (DateTime)obj);
						}
						else
						{
							exDateTime = (ExDateTime)obj;
							if (exDateTime.TimeZone != ExTimeZone.UtcTimeZone)
							{
								exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, exDateTime.UniversalTime);
							}
						}
						innerText = exDateTime.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
						break;
					}
					default:
						goto IL_1A7;
					}
					IL_1AD:
					XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement(DocumentLibrarySearchProvider.airSyncProps[i], "DocumentLibrary:");
					xmlNode3.InnerText = innerText;
					xmlNode2.AppendChild(xmlNode3);
					goto IL_1D8;
					IL_1A7:
					AirSyncDiagnostics.Assert(false);
					goto IL_1AD;
				}
				IL_1D8:;
			}
			responseNode.AppendChild(xmlNode);
			this.results++;
		}

		private static readonly DocumentLibraryPropertyDefinition[] documentLibraryProps = new DocumentLibraryPropertyDefinition[]
		{
			DocumentLibraryItemSchema.Uri,
			DocumentLibraryItemSchema.DisplayName,
			DocumentLibraryItemSchema.IsFolder,
			DocumentLibraryItemSchema.CreationTime,
			DocumentLibraryItemSchema.LastModifiedDate,
			DocumentLibraryItemSchema.IsHidden,
			DocumentSchema.FileSize,
			DocumentSchema.FileType
		};

		private static readonly string[] airSyncProps = new string[]
		{
			"LinkId",
			"DisplayName",
			"IsFolder",
			"CreationDate",
			"LastModifiedDate",
			"IsHidden",
			"ContentLength",
			"ContentType"
		};

		private static readonly SortBy[] sortBy = new SortBy[]
		{
			new SortBy(DocumentLibraryItemSchema.IsFolder, SortOrder.Descending),
			new SortBy(DocumentLibraryItemSchema.DisplayName, SortOrder.Ascending)
		};

		private int minRange;

		private int maxRange = GlobalSettings.MaxDocumentLibrarySearchResults;

		private string linkId;

		private int results;

		private int total = 1;

		private object[] rootProps;

		private IAirSyncUser user;

		private string userName;

		private SecureString userPassword;

		private bool rangeSpecified;

		private ITableView tableView;
	}
}
