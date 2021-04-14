using System;
using System.Globalization;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class GetItemEstimateCommand : SyncBase
	{
		internal GetItemEstimateCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfItemEstimateRequests;
		}

		protected override string RootNodeName
		{
			get
			{
				return "GetItemEstimate";
			}
		}

		public void GetChanges(SyncCollection collection, bool autoLoadFilterAndSyncKey, bool tryNullSync)
		{
			this.GetChanges(collection, autoLoadFilterAndSyncKey, tryNullSync, true, true);
		}

		public void GetChanges(SyncCollection collection, bool autoLoadFilterAndSyncKey, bool tryNullSync, bool commitSyncState)
		{
			this.GetChanges(collection, autoLoadFilterAndSyncKey, tryNullSync, commitSyncState, true);
		}

		public void GetChanges(SyncCollection collection, bool autoLoadFilterAndSyncKey, bool tryNullSync, bool commitSyncState, bool enumerateAllchanges)
		{
			AirSyncDiagnostics.TraceInfo<bool, bool, bool>(ExTraceGlobals.RequestsTracer, this, "GIE:GetChanges autoLoadFilterAndSyncKey:{0} tryNullSync:{1} commitSyncState:{2}", autoLoadFilterAndSyncKey, tryNullSync, commitSyncState);
			base.InitializeVersionFactory(base.Version);
			collection.SetDeviceSettings(this);
			long utcTicks = ExDateTime.Now.Date.UtcTicks;
			bool nullSyncAllowed = !base.IsInQuarantinedState || base.IsQuarantineMailAvailable;
			if (tryNullSync && !collection.CollectionRequiresSync(autoLoadFilterAndSyncKey, nullSyncAllowed))
			{
				base.ProtocolLogger.SetProviderSyncType(collection.CollectionId, ProviderSyncType.N);
				collection.SetEmptyServerChanges();
				return;
			}
			collection.OpenSyncState(autoLoadFilterAndSyncKey, base.SyncStateStorage);
			if (base.Context.Request.Version < 160 && collection.FolderType == DefaultFolderType.Drafts)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "[GIE.GetChanges] Ignoring request to sync drafts.");
				collection.SetEmptyServerChanges();
				return;
			}
			try
			{
				if (collection.HasOptionsNodes)
				{
					collection.ParseSyncOptions();
				}
				else if (collection.HasFilterNode)
				{
					collection.ParseFilterType(null);
				}
				else
				{
					collection.ParseStickyOptions();
				}
			}
			catch (AirSyncPermanentException ex)
			{
				base.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("ParsingError:{0}", ex.ErrorStringForProtocolLogger));
				base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
				throw;
			}
			collection.AddDefaultOptions();
			collection.InitializeSchemaConverter(base.VersionFactory, base.GlobalInfo);
			bool syncProviderOptions = base.IsQuarantineMailAvailable && (base.GlobalInfo.DeviceAccessStateReason == DeviceAccessStateReason.ExternalCompliance || base.GlobalInfo.DeviceAccessStateReason == DeviceAccessStateReason.ExternalEnrollment);
			collection.SetSyncProviderOptions(syncProviderOptions);
			collection.OpenFolderSync();
			collection.VerifySyncKey(false, base.GlobalInfo);
			if (collection.CollectionId != null)
			{
				base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.FolderId, collection.CollectionId);
			}
			base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.ClientSyncKey, collection.SyncKey.ToString(CultureInfo.InvariantCulture));
			if (!base.IsInQuarantinedState || base.IsQuarantineMailAvailable)
			{
				collection.SetFolderSyncOptions(base.VersionFactory, base.IsQuarantineMailAvailable, base.GlobalInfo);
				base.GlobalWindowSize -= collection.GetServerChanges(base.GlobalWindowSize, enumerateAllchanges);
			}
			else
			{
				collection.SetEmptyServerChanges();
				AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "Setting empty server changes for quarantined state. Current AccessState = {0}", base.CurrentAccessState);
			}
			if (!base.IsInQuarantinedState && tryNullSync && collection.ServerChanges.Count == 0 && commitSyncState)
			{
				collection.SyncState.CustomVersion = new int?(9);
				object[] nullSyncPropertiesToSave = collection.GetNullSyncPropertiesToSave();
				try
				{
					if (nullSyncPropertiesToSave != null)
					{
						collection.SyncState.CommitState(collection.PropertiesToSaveForNullSync, nullSyncPropertiesToSave);
					}
					else
					{
						collection.SyncState.CommitState(null, null);
					}
					collection.UpdateSavedNullSyncPropertiesInCache(nullSyncPropertiesToSave);
					base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKbCommitted, (int)collection.SyncState.GetLastCommittedSize() >> 10);
				}
				catch (StorageTransientException)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Transient exception thrown while saving syncstate in GIE\r\nSkipping exception due to only being for the null sync optimization.");
				}
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			this.OnExecute();
			base.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, (int)base.GlobalStatus);
			return Command.ExecutionState.Complete;
		}

		protected override string GetStatusString(SyncBase.ErrorCodeStatus error)
		{
			switch (error)
			{
			case SyncBase.ErrorCodeStatus.Success:
				return "1";
			case SyncBase.ErrorCodeStatus.ProtocolVersionMismatch:
				break;
			case SyncBase.ErrorCodeStatus.InvalidSyncKey:
				return "4";
			case SyncBase.ErrorCodeStatus.ProtocolError:
				return "2";
			case SyncBase.ErrorCodeStatus.ServerError:
				return "2";
			default:
				switch (error)
				{
				case SyncBase.ErrorCodeStatus.InvalidCollection:
					return "2";
				case SyncBase.ErrorCodeStatus.UnprimedSyncState:
					return "3";
				}
				break;
			}
			return "2";
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		internal override bool ValidateXml()
		{
			bool flag = base.ValidateXml();
			if (!flag)
			{
				base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
			}
			return flag;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (base.Version < 140)
			{
				if (GetItemEstimateCommand.validationErrorXml == null)
				{
					XmlDocument commandXmlStub = base.GetCommandXmlStub();
					XmlElement newChild = commandXmlStub.CreateElement("Response", this.RootNodeNamespace);
					XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
					xmlElement.InnerText = XmlConvert.ToString(2);
					commandXmlStub[this.RootNodeName].AppendChild(newChild).AppendChild(xmlElement);
					GetItemEstimateCommand.validationErrorXml = commandXmlStub;
				}
				return GetItemEstimateCommand.validationErrorXml;
			}
			if (GetItemEstimateCommand.validationErrorXmlV14AndLater == null)
			{
				XmlDocument commandXmlStub2 = base.GetCommandXmlStub();
				XmlElement newChild2 = commandXmlStub2.CreateElement("Response", this.RootNodeNamespace);
				XmlElement xmlElement2 = commandXmlStub2.CreateElement("Status", this.RootNodeNamespace);
				xmlElement2.InnerText = XmlConvert.ToString(103);
				commandXmlStub2[this.RootNodeName].AppendChild(newChild2).AppendChild(xmlElement2);
				GetItemEstimateCommand.validationErrorXmlV14AndLater = commandXmlStub2;
			}
			return GetItemEstimateCommand.validationErrorXmlV14AndLater;
		}

		private static void ParseSyncKey(SyncCollection collection)
		{
			collection.AllowRecovery = collection.ParseSynckeyAndDetermineRecovery();
			if (collection.SyncKey == 0U)
			{
				collection.Status = SyncBase.ErrorCodeStatus.UnprimedSyncState;
				throw new AirSyncPermanentException(false)
				{
					ErrorStringForProtocolLogger = "SyncNotPrimed"
				};
			}
		}

		private void FinalizeResponseNode(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "GIE.FinalizeResponseNode");
			XmlElement xmlElement = base.XmlResponse.CreateElement("Response", "GetItemEstimate:");
			this.getItemEstimateXmlNode.AppendChild(xmlElement);
			XmlElement xmlElement2 = base.XmlResponse.CreateElement("Status", "GetItemEstimate:");
			xmlElement2.InnerText = this.GetStatusString(collection.Status);
			base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.PerFolderStatus, (int)collection.Status);
			XmlElement xmlElement3 = base.XmlResponse.CreateElement("Collection", "GetItemEstimate:");
			XmlElement xmlElement4 = base.XmlResponse.CreateElement("Class", "GetItemEstimate:");
			xmlElement4.InnerText = collection.ClassType;
			XmlElement xmlElement5 = base.XmlResponse.CreateElement("CollectionId", "GetItemEstimate:");
			xmlElement5.InnerText = collection.CollectionId;
			if (base.Version < 121)
			{
				xmlElement3.AppendChild(xmlElement4);
			}
			if (collection.ReturnCollectionId)
			{
				xmlElement3.AppendChild(xmlElement5);
			}
			if (collection.Status == SyncBase.ErrorCodeStatus.Success)
			{
				XmlElement xmlElement6 = base.XmlResponse.CreateElement("Estimate", "GetItemEstimate:");
				xmlElement6.InnerText = collection.ServerChanges.Count.ToString(CultureInfo.InvariantCulture);
				xmlElement3.AppendChild(xmlElement6);
			}
			base.ProtocolLogger.IncrementValue(ProtocolLoggerData.TotalFolders);
			xmlElement.AppendChild(xmlElement2);
			xmlElement.AppendChild(xmlElement3);
			this.getItemEstimateXmlNode.AppendChild(xmlElement);
		}

		private void InitializeResponseXmlDocument()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "GIE.InitializeResponseXmlDocument");
			base.XmlResponse = new SafeXmlDocument();
			XmlElement newChild = base.XmlResponse.CreateElement("GetItemEstimate", "GetItemEstimate:");
			base.XmlResponse.AppendChild(newChild);
			this.getItemEstimateXmlNode = newChild;
		}

		private void OnExecute()
		{
			try
			{
				this.ReadXmlRequest();
				this.InitializeResponseXmlDocument();
				foreach (SyncCollection syncCollection in base.Collections.Values)
				{
					AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "GIE.OnExecute CollectionId:{0}", syncCollection.CollectionId);
					syncCollection.WindowSize = 10000;
					try
					{
						GetItemEstimateCommand.ParseSyncKey(syncCollection);
						syncCollection.CreateSyncProvider();
						this.GetChanges(syncCollection, false, true, false, false);
						this.FinalizeResponseNode(syncCollection);
					}
					catch (AirSyncPermanentException ex)
					{
						if (syncCollection.Status == SyncBase.ErrorCodeStatus.Success)
						{
							throw;
						}
						AirSyncDiagnostics.TraceInfo<AirSyncPermanentException>(ExTraceGlobals.RequestsTracer, this, "GIE.OnExecute exception caught, skipping collection\n\r{0}", ex);
						if (base.MailboxLogger != null)
						{
							base.MailboxLogger.SetData(MailboxLogDataName.GetItemEstimateCommand_OnExecute_Exception, ex);
						}
						if (syncCollection.Status == SyncBase.ErrorCodeStatus.ObjectNotFound && syncCollection is RecipientInfoCacheSyncCollection)
						{
							syncCollection.Status = SyncBase.ErrorCodeStatus.Success;
							syncCollection.HasBeenSaved = true;
							syncCollection.SetEmptyServerChanges();
						}
						this.FinalizeResponseNode(syncCollection);
						base.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("FId:{0}:{1}", syncCollection.CollectionId, ex.ErrorStringForProtocolLogger));
						base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
						base.PartialFailure = true;
					}
					finally
					{
						if (syncCollection.SyncState != null)
						{
							if (!syncCollection.SyncState.IsColdStateDeserialized())
							{
								base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.SyncStateKbLeftCompressed, (int)syncCollection.SyncState.GetColdStateCompressedSize() >> 10);
								AirSyncCounters.SyncStateKbLeftCompressed.IncrementBy(syncCollection.SyncState.GetColdStateCompressedSize() >> 10);
							}
							base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.SyncStateKb, (int)syncCollection.SyncState.GetTotalCompressedSize() >> 10);
							AirSyncCounters.SyncStateKbTotal.IncrementBy(syncCollection.SyncState.GetTotalCompressedSize() >> 10);
							base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.TotalSaveCount, syncCollection.SyncState.TotalSaveCount);
							base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.ColdSaveCount, syncCollection.SyncState.ColdSaveCount);
							base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.ColdCopyCount, syncCollection.SyncState.ColdCopyCount);
							base.ProtocolLogger.IncrementValueBy(syncCollection.InternalName, PerFolderProtocolLoggerData.TotalLoadCount, syncCollection.SyncState.TotalLoadCount);
						}
						syncCollection.Dispose();
					}
				}
			}
			catch (Exception lastException)
			{
				base.LastException = lastException;
				if (base.Collections != null)
				{
					foreach (SyncCollection syncCollection2 in base.Collections.Values)
					{
						syncCollection2.Dispose();
					}
				}
				throw;
			}
		}

		private void ReadXmlRequest()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "GIE.ReadXmlRequest");
			XmlNode xmlRequest = base.XmlRequest;
			XmlNode xmlNode = xmlRequest["Collections", "GetItemEstimate:"];
			if (xmlNode.ChildNodes.Count >= GlobalSettings.MaxNumOfFolders)
			{
				AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "GIE: client specified too many folders: {0}", GlobalSettings.MaxNumOfFolders);
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false)
				{
					ErrorStringForProtocolLogger = "GIE.TooManyFolders"
				};
			}
			foreach (object obj in xmlNode.ChildNodes)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				SyncCollection syncCollection = null;
				bool flag = false;
				try
				{
					if (base.Version >= 140)
					{
						syncCollection = SyncCollection.ParseCollection(null, xmlNode2, base.Version, base.MailboxSession);
					}
					else
					{
						XmlNode xmlNode3 = xmlNode2["CollectionId"];
						string collectionId = (xmlNode3 == null) ? null : xmlNode3.InnerText;
						syncCollection = SyncCollection.CreateSyncCollection(base.MailboxSession, base.Version, collectionId);
						foreach (object obj2 in xmlNode2.ChildNodes)
						{
							XmlNode xmlNode4 = (XmlNode)obj2;
							string localName;
							if ((localName = xmlNode4.LocalName) != null)
							{
								if (!(localName == "Class"))
								{
									if (localName == "SyncKey")
									{
										syncCollection.SyncKeyString = xmlNode4.InnerText;
										continue;
									}
									if (localName == "CollectionId")
									{
										syncCollection.CollectionId = xmlNode4.InnerText;
										continue;
									}
									if (localName == "FilterType")
									{
										int num;
										if (!int.TryParse(xmlNode4.InnerText, out num) || num > 8 || num < 0)
										{
											base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
											base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidFilterValue");
											throw new AirSyncPermanentException(false);
										}
										syncCollection.FilterType = (AirSyncV25FilterTypes)num;
										syncCollection.HasFilterNode = true;
										continue;
									}
								}
								else
								{
									string innerText = xmlNode4.InnerText;
									string a;
									if ((a = innerText) != null && (a == "Email" || a == "Calendar" || a == "Contacts" || a == "Tasks" || a == "Notes"))
									{
										syncCollection.ClassType = innerText;
										continue;
									}
									base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidClassType");
									throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.UnexpectedItemClass, null, false);
								}
							}
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadNodeInRequest");
							throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false);
						}
					}
					base.Collections[syncCollection.InternalName] = syncCollection;
					flag = true;
				}
				finally
				{
					if (syncCollection != null && !flag)
					{
						syncCollection.Dispose();
						syncCollection = null;
					}
				}
			}
		}

		private const int MaxCollectionWindowSize = 10000;

		private static XmlDocument validationErrorXml;

		private static XmlDocument validationErrorXmlV14AndLater;

		private XmlNode getItemEstimateXmlNode;

		private static class ErrorStatus
		{
			public const string Success = "1";

			public const string InvalidCollection = "2";

			public const string UnprimedSyncState = "3";

			public const string InvalidSyncKey = "4";
		}
	}
}
