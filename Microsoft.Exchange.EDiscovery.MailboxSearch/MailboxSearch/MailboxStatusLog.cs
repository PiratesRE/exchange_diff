using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxStatusLog : IStatusLog, IDisposable
	{
		public MailboxStatusLog(OrganizationId orgId, string searchName)
		{
			MailboxStatusLog <>4__this = this;
			Util.ThrowIfNull(orgId, "orgId");
			Util.ThrowIfNullOrEmpty(searchName, "searchName");
			MailboxStatusLog.RetryOnTransientException("MailboxStatusLog::ctor", delegate
			{
				<>4__this.mailboxSession = MailboxStatusLog.CreateMailboxSession(orgId);
				using (Folder discoveryFolder = MailboxStatusLog.GetDiscoveryFolder(<>4__this.mailboxSession))
				{
					VersionedId statusLogItemId = MailboxStatusLog.GetStatusLogItemId(<>4__this.mailboxSession, discoveryFolder, searchName);
					if (statusLogItemId == null)
					{
						<>4__this.statusLogItem = Item.Create(<>4__this.mailboxSession, "IPM.Configuration.MailboxDiscoverySearch.StatusLog", discoveryFolder.StoreObjectId);
						<>4__this.statusLogItem[StatusLogStorageSchema.NameProperty] = searchName;
						<>4__this.sourceStatusAttachment = (StreamAttachment)<>4__this.statusLogItem.AttachmentCollection.Create(AttachmentType.Stream);
						<>4__this.sourceStatusAttachment.FileName = "SourceStatusProperty";
						<>4__this.sourceStatusAttachment.Save();
						<>4__this.sourceStatusAttachment.Load();
						<>4__this.sourceConfigurationAttachment = (StreamAttachment)<>4__this.statusLogItem.AttachmentCollection.Create(AttachmentType.Stream);
						<>4__this.sourceConfigurationAttachment.FileName = "SourceConfigurationProperty";
						<>4__this.sourceConfigurationAttachment.Save();
						<>4__this.sourceConfigurationAttachment.Load();
						<>4__this.statusLogItem.Save(SaveMode.NoConflictResolution);
						<>4__this.statusLogItem.Load(MailboxStatusLog.StatusProperties);
					}
					else
					{
						<>4__this.statusLogItem = Item.Bind(<>4__this.mailboxSession, statusLogItemId, MailboxStatusLog.StatusProperties);
						foreach (AttachmentHandle handle in <>4__this.statusLogItem.AttachmentCollection)
						{
							StreamAttachment streamAttachment = (StreamAttachment)<>4__this.statusLogItem.AttachmentCollection.Open(handle);
							if (streamAttachment.FileName == "SourceConfigurationProperty")
							{
								<>4__this.sourceConfigurationAttachment = streamAttachment;
							}
							else if (streamAttachment.FileName == "SourceStatusProperty")
							{
								<>4__this.sourceStatusAttachment = streamAttachment;
							}
						}
					}
				}
			});
		}

		public static void DeleteStatusLog(OrganizationId orgId, string searchName)
		{
			MailboxStatusLog.RetryOnTransientException("DeleteStatusLog", delegate
			{
				using (MailboxSession mailboxSession = MailboxStatusLog.CreateMailboxSession(orgId))
				{
					using (Folder discoveryFolder = MailboxStatusLog.GetDiscoveryFolder(mailboxSession))
					{
						VersionedId statusLogItemId = MailboxStatusLog.GetStatusLogItemId(mailboxSession, discoveryFolder, searchName);
						if (statusLogItemId != null)
						{
							mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
							{
								statusLogItemId.ObjectId
							});
						}
					}
				}
			});
		}

		public void Dispose()
		{
			if (this.sourceStatusStream != null)
			{
				this.sourceStatusStream.Dispose();
				this.sourceStatusStream = null;
				this.sourceStatusBuffer = null;
			}
			if (this.sourceConfigurationAttachment != null)
			{
				this.sourceConfigurationAttachment.Dispose();
				this.sourceConfigurationAttachment = null;
			}
			if (this.sourceStatusAttachment != null)
			{
				this.sourceStatusAttachment.Dispose();
				this.sourceStatusAttachment = null;
			}
			if (this.statusLogItem != null)
			{
				this.statusLogItem.Dispose();
				this.statusLogItem = null;
			}
			if (this.mailboxSession != null)
			{
				this.mailboxSession.Dispose();
				this.mailboxSession = null;
			}
		}

		public void ResetStatusLog(SourceInformationCollection allSourceInformation, OperationStatus status, ExportSettings exportSettings)
		{
			MailboxStatusLog.RetryOnTransientException("ResetStatusLog", delegate
			{
				this.InternalUpdateStatus(allSourceInformation, status, true, exportSettings);
			});
		}

		public void UpdateSourceStatus(SourceInformation source, int sourceIndex)
		{
			this.sourceStatusStream.Seek((long)(sourceIndex * 128), SeekOrigin.Begin);
			source.Status.SaveToStream(this.sourceStatusStream);
			MailboxStatusLog.SaveStreamProperty(this.sourceStatusAttachment, this.sourceStatusBuffer);
			MailboxStatusLog.RetryOnTransientException("UpdateSourceStatus", delegate
			{
				this.statusLogItem.Save(SaveMode.ResolveConflicts);
				this.statusLogItem.Load(MailboxStatusLog.StatusProperties);
			});
		}

		public void UpdateStatus(SourceInformationCollection allSourceInformation, OperationStatus status)
		{
			MailboxStatusLog.RetryOnTransientException("InternalUpdateStatus", delegate
			{
				this.InternalUpdateStatus(allSourceInformation, status, false, null);
			});
		}

		public ExportSettings LoadStatus(out SourceInformationCollection allSourceInformation, out OperationStatus status)
		{
			SourceInformationCollection sourceInformationCollection = null;
			OperationStatus operationStatus = OperationStatus.None;
			byte[] array = null;
			if (this.statusLogItem == null)
			{
				throw new ObjectDisposedException("MailboxStatusLog");
			}
			bool flag = false;
			Exception ex = null;
			try
			{
				int? valueAsNullable = this.statusLogItem.GetValueAsNullable<int>(StatusLogStorageSchema.OperationStatusProperty);
				if (valueAsNullable != null)
				{
					operationStatus = (OperationStatus)valueAsNullable.Value;
					array = this.statusLogItem.GetValueOrDefault<byte[]>(StatusLogStorageSchema.ExportSettingsProperty, null);
					byte[] array2 = MailboxStatusLog.LoadStreamProperty(this.sourceStatusAttachment);
					if (array2 != null && array2.Length % 128 == 0)
					{
						int num = array2.Length / 128;
						byte[] array3 = MailboxStatusLog.LoadStreamProperty(this.sourceConfigurationAttachment);
						if (array3 != null)
						{
							sourceInformationCollection = new SourceInformationCollection(num);
							BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
							using (MemoryStream memoryStream = new MemoryStream(array3))
							{
								for (int i = 0; i < num; i++)
								{
									SourceInformation.SourceConfiguration sourceConfiguration = binaryFormatter.Deserialize(memoryStream) as SourceInformation.SourceConfiguration;
									if (sourceConfiguration == null)
									{
										flag = true;
										break;
									}
									sourceInformationCollection[sourceConfiguration.Id] = new SourceInformation(sourceConfiguration.Name, sourceConfiguration.Id, sourceConfiguration.SourceFilter, sourceConfiguration.ServiceEndpoint, sourceConfiguration.LegacyExchangeDN);
									this.sourceStatusBuffer = array2;
									this.sourceStatusStream = new MemoryStream(this.sourceStatusBuffer);
									this.sourceStatusStream.Seek((long)(i * 128), SeekOrigin.Begin);
									sourceInformationCollection[i].Status.LoadFromStream(this.sourceStatusStream);
								}
							}
						}
					}
				}
			}
			catch (StoragePermanentException ex2)
			{
				ex = ex2;
				flag = true;
			}
			catch (StorageTransientException ex3)
			{
				ex = ex3;
				flag = true;
			}
			catch (SerializationException ex4)
			{
				ex = ex4;
				flag = true;
			}
			catch (ArgumentOutOfRangeException ex5)
			{
				ex = ex5;
				flag = true;
			}
			if (ex != null)
			{
				Util.Tracer.TraceError<Exception>(0L, "MailboxStatusLog.LoadStatus: Exception: {0}", ex);
			}
			if (flag)
			{
				Util.Tracer.TraceError(0L, "MailboxStatusLog.LoadStatus: status corrupted.");
				this.sourceStatusBuffer = null;
				this.sourceStatusStream = null;
				sourceInformationCollection = null;
				operationStatus = OperationStatus.None;
			}
			allSourceInformation = sourceInformationCollection;
			status = operationStatus;
			if (array == null)
			{
				return null;
			}
			return ExportSettings.FromBinary(array, 0);
		}

		public void Delete()
		{
			if (this.statusLogItem == null)
			{
				throw new ObjectDisposedException("MailboxStatusLog");
			}
			MailboxStatusLog.RetryOnTransientException("Delete", delegate
			{
				this.mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					this.statusLogItem.StoreObjectId
				});
			});
		}

		private static void RetryOnTransientException(string description, Action action)
		{
			int num = 3;
			try
			{
				IL_02:
				action();
			}
			catch (StorageTransientException arg)
			{
				Util.Tracer.TraceError<string, StorageTransientException>(0L, "MailboxStatusLog::RetryOnTransientException: failed {0}.  Exception: {1}", description, arg);
				if (num-- <= 0)
				{
					throw;
				}
				goto IL_02;
			}
		}

		private static MailboxSession CreateMailboxSession(OrganizationId orgId)
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, orgId, true);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, adsessionSettings, 433, "CreateMailboxSession", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\Mailbox\\MailboxStatusLog.cs");
			ADUser discoveryMailbox = MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession);
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(adsessionSettings, discoveryMailbox, RemotingOptions.LocalConnectionsOnly);
			return MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=EventBased MSExchangeMailboxAssistants;Action:DiscoverySearch");
		}

		private static VersionedId GetStatusLogItemId(MailboxSession session, Folder discoveryFolder, string searchName)
		{
			VersionedId result = null;
			if (discoveryFolder != null)
			{
				QueryFilter queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, StatusLogStorageSchema.NameProperty, searchName),
					new ComparisonFilter(ComparisonOperator.Equal, StatusLogStorageSchema.ItemClassProperty, "IPM.Configuration.MailboxDiscoverySearch.StatusLog")
				});
				using (QueryResult queryResult = discoveryFolder.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
				{
					StatusLogStorageSchema.ItemIdProperty
				}))
				{
					object[][] rows = queryResult.GetRows(1);
					if (rows != null && rows.Length > 0)
					{
						result = (rows[0][0] as VersionedId);
					}
				}
			}
			return result;
		}

		private static Folder GetDiscoveryFolder(MailboxSession session)
		{
			Folder folder = null;
			using (Folder folder2 = Folder.Bind(session, DefaultFolderType.Inbox))
			{
				StoreObjectId storeObjectId = StatusLogStorageSchema.FindChildFolderByName(folder2, "Discovery");
				if (storeObjectId == null)
				{
					folder = Folder.Create(session, folder2.StoreObjectId, StoreObjectType.Folder, "Discovery", CreateMode.OpenIfExists);
					folder.Save();
					folder.Load();
				}
				else
				{
					folder = Folder.Bind(session, storeObjectId);
				}
			}
			return folder;
		}

		private static void SaveStreamProperty(StreamAttachment attachment, byte[] value)
		{
			using (Stream contentStream = attachment.GetContentStream())
			{
				contentStream.Seek(0L, SeekOrigin.Begin);
				contentStream.Write(value, 0, value.Length);
			}
			attachment.Save();
			attachment.Load();
		}

		private static byte[] LoadStreamProperty(StreamAttachment attachment)
		{
			byte[] array = null;
			using (Stream contentStream = attachment.GetContentStream())
			{
				if (contentStream.Length > 0L)
				{
					array = new byte[contentStream.Length];
					contentStream.Read(array, 0, (int)contentStream.Length);
				}
			}
			return array;
		}

		private void InternalUpdateStatus(SourceInformationCollection allSourceInformation, OperationStatus status, bool updateConfiguration, ExportSettings exportSettings)
		{
			if (this.statusLogItem == null)
			{
				throw new ObjectDisposedException("MailboxStatusLog");
			}
			if (this.sourceStatusBuffer == null)
			{
				this.sourceStatusBuffer = new byte[allSourceInformation.Count * 128];
				this.sourceStatusStream = new MemoryStream(this.sourceStatusBuffer);
			}
			if (exportSettings != null)
			{
				this.statusLogItem[StatusLogStorageSchema.ExportSettingsProperty] = exportSettings.ToBinary();
			}
			this.statusLogItem[StatusLogStorageSchema.OperationStatusProperty] = (int)status;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
				int num = 0;
				foreach (SourceInformation sourceInformation in allSourceInformation.Values)
				{
					if (updateConfiguration)
					{
						binaryFormatter.Serialize(memoryStream, sourceInformation.Configuration);
					}
					this.sourceStatusStream.Seek((long)(num * 128), SeekOrigin.Begin);
					sourceInformation.Status.SaveToStream(this.sourceStatusStream);
					num++;
				}
				if (updateConfiguration)
				{
					MailboxStatusLog.SaveStreamProperty(this.sourceConfigurationAttachment, memoryStream.ToArray());
				}
			}
			MailboxStatusLog.SaveStreamProperty(this.sourceStatusAttachment, this.sourceStatusBuffer);
			this.statusLogItem.Save(SaveMode.ResolveConflicts);
			this.statusLogItem.Load(MailboxStatusLog.StatusProperties);
		}

		private const int SourceStatusMaxLength = 128;

		private const int TransientRetries = 3;

		private const string SourceStatusPropertyName = "SourceStatusProperty";

		private const string SourceConfigurationPropertyName = "SourceConfigurationProperty";

		private static readonly PropertyDefinition[] StatusProperties = new PropertyDefinition[]
		{
			StatusLogStorageSchema.OperationStatusProperty,
			StatusLogStorageSchema.ExportSettingsProperty
		};

		private MailboxSession mailboxSession;

		private Item statusLogItem;

		private byte[] sourceStatusBuffer;

		private MemoryStream sourceStatusStream;

		private StreamAttachment sourceConfigurationAttachment;

		private StreamAttachment sourceStatusAttachment;
	}
}
