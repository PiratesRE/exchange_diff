using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class BaseAttachmentFetchProvider : DisposeTrackableBase, IItemOperationsProvider, IReusable, IMultipartResponse
	{
		internal BaseAttachmentFetchProvider(MailboxSession session, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger, Unlimited<ByteQuantifiedSize> maxAttachmentSize, bool attachmentsEnabled)
		{
			this.Session = session;
			this.SyncStateStorage = syncStateStorage;
			this.ProtocolLogger = protocolLogger;
			this.MaxAttachmentSize = maxAttachmentSize;
			this.AttachmentsEnabled = attachmentsEnabled;
			AirSyncCounters.NumberOfMailboxAttachmentFetches.Increment();
		}

		public bool RightsManagementSupport { get; private set; }

		protected int MinRange { get; set; }

		protected int MaxRange { get; set; }

		protected bool MultiPartResponse { get; set; }

		protected int PartNumber { get; set; }

		protected int Total { get; set; }

		protected string FileReference { get; set; }

		protected string ContentType { get; set; }

		protected AirSyncStream OutStream { get; set; }

		protected bool RangeSpecified { get; set; }

		protected MailboxSession Session { get; set; }

		protected SyncStateStorage SyncStateStorage { get; set; }

		protected ProtocolLogger ProtocolLogger { get; set; }

		protected Unlimited<ByteQuantifiedSize> MaxAttachmentSize { get; set; }

		protected bool AttachmentsEnabled { get; set; }

		public void Reset()
		{
			this.MinRange = 0;
			this.MaxRange = 0;
			this.Total = 0;
			this.FileReference = null;
			this.ContentType = null;
			this.RangeSpecified = false;
			this.RightsManagementSupport = false;
		}

		public void BuildResponse(XmlNode responseNode, int partNumber)
		{
			this.PartNumber = partNumber;
			this.MultiPartResponse = true;
			this.BuildResponse(responseNode);
		}

		public Stream GetResponseStream()
		{
			Stream outStream = this.OutStream;
			this.OutStream = null;
			return outStream;
		}

		public void ParseRequest(XmlNode fetchNode)
		{
			foreach (object obj in fetchNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name;
				if ((name = xmlNode.Name) != null)
				{
					if (name == "FileReference")
					{
						this.FileReference = xmlNode.InnerText;
						continue;
					}
					if (name == "Options")
					{
						this.ParseOptions(xmlNode);
						continue;
					}
					if (name == "Store")
					{
						continue;
					}
				}
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidNodeInAttachmentFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
			}
		}

		public void Execute()
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Attachment Fetch command received. Processing request...");
			int count = -1;
			if (this.RangeSpecified)
			{
				count = this.MaxRange - this.MinRange + 1;
			}
			if (!this.AttachmentsEnabled)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentsNotEnabledMbxAttProvider");
				throw new AirSyncPermanentException(HttpStatusCode.Forbidden, StatusCode.AccessDenied, null, false);
			}
			this.OutStream = new AirSyncStream();
			try
			{
				this.Total = this.InternalExecute(count);
				if (this.Total == 0)
				{
					this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ZeroAttachmentSizeForFetch");
					throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_NotificationGUID, null, false);
				}
			}
			catch (FormatException innerException)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadAttachmentIdInFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_TooManyFolders, innerException, false);
			}
			catch (ObjectNotFoundException innerException2)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentNotFoundInFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException2, false);
			}
			catch (ArgumentOutOfRangeException innerException3)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentRangeErrorInFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ObjectNotFound, innerException3, false);
			}
			catch (IOException innerException4)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "IOErrorInAttachmentFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_FolderHierarchyRequired, innerException4, false);
			}
			catch (DataTooLargeException innerException5)
			{
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "AttachmentTooBigInFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_NotificationsNotProvisioned, innerException5, false);
			}
			this.ProtocolLogger.IncrementValue(ProtocolLoggerData.Attachments);
			this.ProtocolLogger.IncrementValueBy(ProtocolLoggerData.AttachmentBytes, this.Total);
		}

		public void BuildResponse(XmlNode responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("FileReference", "AirSyncBase:");
			XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("Properties", "ItemOperations:");
			XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("ContentType", "AirSyncBase:");
			if (this.RangeSpecified)
			{
				XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("Range", "ItemOperations:");
				xmlNode6.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[]
				{
					this.MinRange,
					(long)this.MinRange + this.OutStream.Length - 1L
				});
				xmlNode4.AppendChild(xmlNode6);
				XmlNode xmlNode7 = responseNode.OwnerDocument.CreateElement("Total", "ItemOperations:");
				xmlNode7.InnerText = this.Total.ToString(CultureInfo.InvariantCulture);
				xmlNode4.AppendChild(xmlNode7);
			}
			xmlNode5.InnerText = this.ContentType;
			xmlNode4.AppendChild(xmlNode5);
			if (this.MultiPartResponse)
			{
				XmlNode xmlNode8 = responseNode.OwnerDocument.CreateElement("Part", "ItemOperations:");
				xmlNode8.InnerText = this.PartNumber.ToString(CultureInfo.InvariantCulture);
				xmlNode4.AppendChild(xmlNode8);
			}
			else
			{
				this.OutStream.DoBase64Conversion = true;
				AirSyncBlobXmlNode airSyncBlobXmlNode = new AirSyncBlobXmlNode(null, "Data", "ItemOperations:", responseNode.OwnerDocument);
				airSyncBlobXmlNode.Stream = this.GetResponseStream();
				airSyncBlobXmlNode.StreamDataSize = airSyncBlobXmlNode.Stream.Length;
				airSyncBlobXmlNode.OriginalNodeType = XmlNodeType.Text;
				xmlNode4.AppendChild(airSyncBlobXmlNode);
			}
			xmlNode2.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode2);
			xmlNode3.InnerText = this.FileReference;
			xmlNode.AppendChild(xmlNode3);
			xmlNode.AppendChild(xmlNode4);
			responseNode.AppendChild(xmlNode);
		}

		public void BuildErrorResponse(string statusCode, XmlNode responseNode, ProtocolLogger protocolLogger)
		{
			if (protocolLogger != null)
			{
				protocolLogger.IncrementValue(ProtocolLoggerData.IOFetchAttErrors);
			}
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			xmlNode2.InnerText = statusCode;
			xmlNode.AppendChild(xmlNode2);
			if (!string.IsNullOrEmpty(this.FileReference))
			{
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("FileReference", "AirSyncBase:");
				xmlNode3.InnerText = this.FileReference;
				xmlNode.AppendChild(xmlNode3);
			}
			responseNode.AppendChild(xmlNode);
		}

		protected abstract int InternalExecute(int count);

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.OutStream != null)
			{
				this.OutStream.Dispose();
				this.OutStream = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BaseAttachmentFetchProvider>(this);
		}

		private void ParseOptions(XmlNode optionsNode)
		{
			foreach (object obj in optionsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string localName;
				if ((localName = xmlNode.LocalName) != null)
				{
					if (!(localName == "Range"))
					{
						if (localName == "RightsManagementSupport")
						{
							string innerText;
							if ((innerText = xmlNode.InnerText) != null)
							{
								if (innerText == "0")
								{
									this.RightsManagementSupport = false;
									continue;
								}
								if (innerText == "1")
								{
									this.RightsManagementSupport = true;
									continue;
								}
							}
							this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidRightsManagementSupportInAttachmentFetch");
							throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
						}
					}
					else
					{
						string[] array = xmlNode.InnerText.Split(new char[]
						{
							'-'
						});
						this.MinRange = int.Parse(array[0], CultureInfo.InvariantCulture);
						this.MaxRange = int.Parse(array[1], CultureInfo.InvariantCulture);
						if (this.MinRange > this.MaxRange)
						{
							this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MinMoreThanMaxInAttachmentFetch");
							throw new AirSyncPermanentException(StatusCode.Sync_ObjectNotFound, false);
						}
						this.RangeSpecified = true;
						continue;
					}
				}
				this.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UnknownOptionInAttachmentFetch");
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false);
			}
		}
	}
}
