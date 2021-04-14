using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class CollectionCommand : Command
	{
		internal CollectionCommand()
		{
		}

		internal override int MaxVersion
		{
			get
			{
				return 121;
			}
		}

		internal string CollectionId
		{
			get
			{
				string legacyUrlParameter = base.Request.GetLegacyUrlParameter("CollectionId");
				if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CollectionIdInvalid");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidIDs, null, false);
				}
				return legacyUrlParameter;
			}
		}

		internal string ParentId
		{
			get
			{
				string legacyUrlParameter = base.Request.GetLegacyUrlParameter("ParentId");
				if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ParentIdInvalid");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidIDs, null, false);
				}
				return legacyUrlParameter;
			}
		}

		internal string CollectionName
		{
			get
			{
				string legacyUrlParameter = base.Request.GetLegacyUrlParameter("CollectionName");
				if (legacyUrlParameter != null && (legacyUrlParameter.Length < 1 || legacyUrlParameter.Length > 256))
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CollectionNameInvalid");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidIDs, null, false);
				}
				return legacyUrlParameter;
			}
		}

		protected sealed override string RootNodeName
		{
			get
			{
				return "Invalid";
			}
		}

		protected CollectionCommand.CollectionRequestStruct CollectionRequest
		{
			get
			{
				return this.collectionRequest;
			}
		}

		protected CustomSyncState SyncState
		{
			get
			{
				return this.syncState;
			}
		}

		internal static XmlDocument ConstructErrorXml(StatusCode statusCode)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = xmlDocument.CreateElement("Response", "FolderHierarchy:");
			XmlNode xmlNode2 = xmlDocument.CreateElement("Status", "FolderHierarchy:");
			XmlNode xmlNode3 = xmlNode2;
			int num = (int)statusCode;
			xmlNode3.InnerText = num.ToString(CultureInfo.InvariantCulture);
			xmlDocument.AppendChild(xmlNode);
			xmlNode.AppendChild(xmlNode2);
			return xmlDocument;
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			try
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				try
				{
					this.LoadSyncState();
					this.ParseRequest(base.MailboxSession);
					this.ProcessCommand(base.MailboxSession, xmlDocument);
					if (((FolderIdMapping)this.syncState[CustomStateDatumType.IdMapping]).IsDirty)
					{
						this.syncState.Commit();
					}
				}
				finally
				{
					if (this.syncState != null)
					{
						this.syncState.Dispose();
					}
					this.syncState = null;
				}
				base.XmlResponse = xmlDocument;
			}
			catch (QuotaExceededException)
			{
				throw;
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ProtocolError), innerException, false)
				{
					ErrorStringForProtocolLogger = "FolderNotFound"
				};
			}
			catch (StoragePermanentException ex)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ClientServerConversion), ex, false)
				{
					ErrorStringForProtocolLogger = ex.GetType().FullName
				};
			}
			return Command.ExecutionState.Complete;
		}

		protected abstract void ProcessCommand(MailboxSession mailboxSession, XmlDocument doc);

		protected override bool HandleQuarantinedState()
		{
			base.XmlResponse = CollectionCommand.ConstructErrorXml(StatusCode.Sync_ClientServerConversion);
			return false;
		}

		private void LoadSyncState()
		{
			this.syncState = base.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
			if (this.syncState == null)
			{
				throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, null, false)
				{
					ErrorStringForProtocolLogger = "CorruptSyncState"
				};
			}
		}

		private void ParseRequest(MailboxSession mailboxSession)
		{
			this.collectionRequest = default(CollectionCommand.CollectionRequestStruct);
			string collectionId = this.CollectionId;
			if (collectionId != null)
			{
				MailboxSyncItemId mailboxSyncItemId = ((FolderIdMapping)this.syncState[CustomStateDatumType.IdMapping])[collectionId] as MailboxSyncItemId;
				if (mailboxSyncItemId == null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ProtocolError), null, false)
					{
						ErrorStringForProtocolLogger = "FolderNotFound2"
					};
				}
				this.collectionRequest.CollectionId = (StoreObjectId)mailboxSyncItemId.NativeId;
				AirSyncDiagnostics.TraceDebug<string, StoreObjectId>(ExTraceGlobals.RequestsTracer, this, "Received request with syncCollectionId {0}, which maps to collection Id {1}.", collectionId, this.collectionRequest.CollectionId);
			}
			string parentId = this.ParentId;
			if (parentId == "0")
			{
				this.collectionRequest.ParentId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Root);
			}
			else if (parentId != null)
			{
				MailboxSyncItemId mailboxSyncItemId2 = ((FolderIdMapping)this.syncState[CustomStateDatumType.IdMapping])[parentId] as MailboxSyncItemId;
				if (mailboxSyncItemId2 == null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ServerError, CollectionCommand.ConstructErrorXml(StatusCode.Sync_ServerError), null, false)
					{
						ErrorStringForProtocolLogger = "NoIdMappingForParentId"
					};
				}
				this.collectionRequest.ParentId = (StoreObjectId)mailboxSyncItemId2.NativeId;
				AirSyncDiagnostics.TraceDebug<string, StoreObjectId>(ExTraceGlobals.RequestsTracer, this, "Received request with syncParentId {0}, which maps to parent Id {1}.", parentId, this.collectionRequest.ParentId);
			}
			this.collectionRequest.CollectionName = this.CollectionName;
		}

		internal const string RootFolderId = "0";

		private CollectionCommand.CollectionRequestStruct collectionRequest;

		private CustomSyncState syncState;

		protected struct CollectionRequestStruct
		{
			internal StoreObjectId CollectionId;

			internal string CollectionName;

			internal StoreObjectId ParentId;
		}
	}
}
