using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class TargetMailbox : ITargetMailbox, ITarget, IDisposable
	{
		public TargetMailbox(OrganizationId orgId, string primarySmtpAddress, string legacyDN, Uri serviceEndpoint, IExportContext exportContext) : this(orgId, primarySmtpAddress, legacyDN, new EwsClient(serviceEndpoint, new ServerToServerEwsCallingContext(null)), exportContext)
		{
		}

		public TargetMailbox(OrganizationId orgId, string primarySmtpAddress, string legacyDN, IEwsClient ewsClient, IExportContext exportContext)
		{
			Util.ThrowIfNullOrEmpty(primarySmtpAddress, "primarySmtpAddress");
			Util.ThrowIfNullOrEmpty(legacyDN, "legacyDN");
			Util.ThrowIfNull(ewsClient, "ewsClient");
			Util.ThrowIfNull(exportContext, "exportContext");
			this.organizationId = orgId;
			this.ewsClient = ewsClient;
			this.targetLocation = exportContext.TargetLocation;
			this.Initialize(primarySmtpAddress, legacyDN, exportContext);
		}

		public TargetMailbox(OrganizationId orgId, string primarySmtpAddress, string legacyDN, Uri serviceEndpoint, ITargetLocation targetLocation) : this(orgId, primarySmtpAddress, legacyDN, new EwsClient(serviceEndpoint, new ServerToServerEwsCallingContext(null)), targetLocation)
		{
		}

		public TargetMailbox(OrganizationId orgId, string primarySmtpAddress, string legacyDN, IEwsClient ewsClient, ITargetLocation targetLocation)
		{
			Util.ThrowIfNullOrEmpty(primarySmtpAddress, "primarySmtpAddress");
			Util.ThrowIfNullOrEmpty(legacyDN, "legacyDN");
			Util.ThrowIfNull(ewsClient, "ewsClient");
			Util.ThrowIfNull(targetLocation, "targetLocation");
			this.organizationId = orgId;
			this.ewsClient = ewsClient;
			this.targetLocation = targetLocation;
			this.Initialize(primarySmtpAddress, legacyDN, null);
		}

		public static Regex InvalidFileCharExpression
		{
			get
			{
				if (TargetMailbox.invalidFileCharExpression == null)
				{
					string str = new string(Path.GetInvalidFileNameChars());
					TargetMailbox.invalidFileCharExpression = new Regex("[" + Regex.Escape(str) + "#]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				}
				return TargetMailbox.invalidFileCharExpression;
			}
		}

		public string PrimarySmtpAddress { get; private set; }

		public string LegacyDistinguishedName { get; private set; }

		public IExportContext ExportContext { get; private set; }

		public ExportSettings ExportSettings { get; set; }

		public IEwsClient EwsClientInstance
		{
			get
			{
				return this.ewsClient;
			}
		}

		public bool ExportLocationExist
		{
			get
			{
				return this.GetWorkingOrResultFolder(this.targetLocation.ExportLocation) != null;
			}
		}

		public bool WorkingLocationExist
		{
			get
			{
				return this.GetWorkingOrResultFolder(this.targetLocation.WorkingLocation) != null;
			}
		}

		public string[] StatusMailRecipients { get; set; }

		public void Dispose()
		{
			if (!this.disposed)
			{
				if (this.exportRecordAttachmentLog != null)
				{
					this.exportRecordAttachmentLog.Dispose();
					this.exportRecordAttachmentLog = null;
				}
				if (this.statusLog != null)
				{
					this.statusLog.Dispose();
					this.statusLog = null;
				}
				this.disposed = true;
			}
		}

		public void CheckInitialStatus(SourceInformationCollection allSourceInformation, OperationStatus status)
		{
		}

		public IItemIdList CreateItemIdList(string mailboxId, bool isUnsearchable)
		{
			return new MailboxItemIdList(this.PrimarySmtpAddress, mailboxId, this.targetLocation.WorkingLocation, this.ewsClient, isUnsearchable);
		}

		public void RemoveItemIdList(string mailboxId, bool isUnsearchable)
		{
			new MailboxItemIdList(this.PrimarySmtpAddress, mailboxId, this.targetLocation.WorkingLocation, this.ewsClient, isUnsearchable).Delete();
		}

		public IContextualBatchDataWriter<List<ItemInformation>> CreateDataWriter(IProgressController progressController)
		{
			return new MailboxWriter(this.ExportContext, this, progressController);
		}

		public void Rollback(SourceInformationCollection allSourceInformation)
		{
			this.PreRemoveSearchResults(false);
			this.RemoveSearchResults();
		}

		public IStatusLog GetStatusLog()
		{
			if (this.statusLog == null)
			{
				this.statusLog = new MailboxStatusLog(this.organizationId, this.ExportContext.ExportMetadata.ExportName);
			}
			return this.statusLog;
		}

		public string CreateResultFolder(string resultFolderName)
		{
			BaseFolderType baseFolderType = this.CreateRootResultFolderIfNotExist();
			BaseFolderType baseFolderType2 = this.ewsClient.GetFolderByName(this.PrimarySmtpAddress, baseFolderType.FolderId, resultFolderName);
			if (baseFolderType2 == null)
			{
				baseFolderType2 = this.CreateFolder(baseFolderType.FolderId, resultFolderName, false);
			}
			return baseFolderType2.FolderId.Id;
		}

		public void PreRemoveSearchResults(bool removeLogs)
		{
			BaseFolderType orCreateRecycleFolder = this.GetOrCreateRecycleFolder(true);
			BaseFolderType workingOrResultFolder = this.GetWorkingOrResultFolder(this.targetLocation.ExportLocation);
			if (workingOrResultFolder != null)
			{
				if (removeLogs)
				{
					this.ewsClient.MoveFolder(this.PrimarySmtpAddress, orCreateRecycleFolder.FolderId, new BaseFolderIdType[]
					{
						workingOrResultFolder.FolderId
					});
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				this.ewsClient.GetAllFolders(this.PrimarySmtpAddress, workingOrResultFolder.FolderId.Id, false, false, dictionary);
				List<BaseFolderIdType> list = new List<BaseFolderIdType>(dictionary.Count);
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					list.Add(new FolderIdType
					{
						Id = keyValuePair.Key
					});
				}
				if (list.Count > 0)
				{
					this.ewsClient.MoveFolder(this.PrimarySmtpAddress, orCreateRecycleFolder.FolderId, list.ToArray());
				}
			}
		}

		public void RemoveSearchResults()
		{
			this.DeleteWorkingFolders();
			BaseFolderType orCreateRecycleFolder = this.GetOrCreateRecycleFolder(false);
			if (orCreateRecycleFolder == null)
			{
				return;
			}
			this.ewsClient.DeleteFolder(this.PrimarySmtpAddress, new BaseFolderIdType[]
			{
				orCreateRecycleFolder.FolderId
			});
		}

		public BaseFolderType GetFolder(string folderId)
		{
			FolderIdType folderId2 = new FolderIdType
			{
				Id = folderId
			};
			return this.ewsClient.GetFolderById(this.PrimarySmtpAddress, folderId2);
		}

		public BaseFolderType GetFolderByName(BaseFolderIdType parentFolderId, string folderName)
		{
			return this.ewsClient.GetFolderByName(this.PrimarySmtpAddress, parentFolderId, folderName);
		}

		public BaseFolderType CreateFolder(BaseFolderIdType parentFolderId, string newFolderName, bool isHidden)
		{
			FolderType folderType = new FolderType
			{
				DisplayName = newFolderName
			};
			if (isHidden)
			{
				folderType.ExtendedProperty = new ExtendedPropertyType[]
				{
					MailboxItemIdList.IsHiddenExtendedProperty
				};
			}
			List<BaseFolderType> list = this.ewsClient.CreateFolder(this.PrimarySmtpAddress, parentFolderId, new BaseFolderType[]
			{
				folderType
			});
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			return list[0];
		}

		public List<ItemInformation> CopyItems(string parentFolderId, IList<ItemInformation> items)
		{
			FolderIdType parentFolderId2 = new FolderIdType
			{
				Id = parentFolderId
			};
			return this.ewsClient.UploadItems(this.PrimarySmtpAddress, parentFolderId2, items, true);
		}

		public void CreateOrUpdateSearchLogEmail(MailboxDiscoverySearch searchObject, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes)
		{
			Util.ThrowIfNull(searchObject, "searchObject");
			this.exportRecordLogFileName = Path.ChangeExtension(this.ToSafeFileNameString(searchObject.Name), ".csv");
			this.InternalCreateOrUpdateSearchLogEmail(searchObject, successfulMailboxes, unsuccessfulMailboxes, true);
		}

		public void WriteExportRecordLog(MailboxDiscoverySearch searchObject, IEnumerable<ExportRecord> records)
		{
			Util.ThrowIfNull(searchObject, "searchObject");
			this.InternalCreateOrUpdateSearchLogEmail(searchObject, null, null, false);
			this.InternalWriteExportRecordLog(records);
		}

		public void AttachDiscoveryLogFiles()
		{
			this.InternalAttachDiscoveryLogFiles();
		}

		private void InternalCreateOrUpdateSearchLogEmail(MailboxDiscoverySearch searchObject, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes, bool updateIfExists)
		{
			BaseFolderType baseFolderType = this.CreateRootResultFolderIfNotExist();
			if (string.IsNullOrEmpty(this.logItemId))
			{
				this.logItemId = this.CreateSearchLogEmail(baseFolderType.FolderId, searchObject, successfulMailboxes, unsuccessfulMailboxes);
				return;
			}
			ItemType item = this.ewsClient.GetItem(this.PrimarySmtpAddress, this.logItemId);
			if (item == null)
			{
				this.logItemId = this.CreateSearchLogEmail(baseFolderType.FolderId, searchObject, successfulMailboxes, unsuccessfulMailboxes);
				return;
			}
			this.logItemId = item.ItemId.Id;
			if (updateIfExists)
			{
				ItemChangeType itemChangeType = new ItemChangeType
				{
					Item = item.ItemId,
					Updates = new ItemChangeDescriptionType[]
					{
						new SetItemFieldType
						{
							Item = new PathToUnindexedFieldType
							{
								FieldURI = UnindexedFieldURIType.itemBody
							},
							Item1 = new MessageType
							{
								Body = new BodyType
								{
									BodyType1 = BodyTypeType.HTML,
									Value = Util.CreateLogMailBody(searchObject, this.StatusMailRecipients, successfulMailboxes, unsuccessfulMailboxes, this.ExportContext.Sources)
								}
							}
						}
					}
				};
				this.ewsClient.UpdateItems(this.PrimarySmtpAddress, baseFolderType.FolderId, new ItemChangeType[]
				{
					itemChangeType
				});
			}
		}

		private string CreateSearchLogEmail(FolderIdType folderId, MailboxDiscoverySearch searchObject, List<string> successfulMailboxes, List<string> unsuccessfulMailboxes)
		{
			ItemType itemType = new ItemType
			{
				Subject = string.Format("{0}-{1}", searchObject.Name, searchObject.LastStartTime.ToString()),
				Body = new BodyType
				{
					BodyType1 = BodyTypeType.HTML,
					Value = Util.CreateLogMailBody(searchObject, this.StatusMailRecipients, successfulMailboxes, unsuccessfulMailboxes, this.ExportContext.Sources)
				}
			};
			List<ItemType> list = this.ewsClient.CreateItems(this.PrimarySmtpAddress, folderId, new ItemType[]
			{
				itemType
			});
			return list[0].ItemId.Id;
		}

		private void InternalWriteExportRecordLog(IEnumerable<ExportRecord> records)
		{
			if (records == null)
			{
				return;
			}
			if (this.exportRecordAttachmentLog == null)
			{
				this.exportRecordAttachmentLog = new AttachmentLog(this.exportRecordLogFileName, Strings.SearchLogHeader);
			}
			this.exportRecordAttachmentLog.WriteLogs(this.CreateLogEntriesFromExportRecords(records));
		}

		private void InternalAttachDiscoveryLogFiles()
		{
			if (!string.IsNullOrEmpty(this.exportRecordAttachmentId))
			{
				this.ewsClient.DeleteAttachments(this.PrimarySmtpAddress, new string[]
				{
					this.exportRecordAttachmentId
				});
			}
			if (this.exportRecordAttachmentLog == null)
			{
				this.exportRecordAttachmentLog = new AttachmentLog(this.exportRecordLogFileName, Strings.SearchLogHeader);
			}
			FileAttachmentType fileAttachmentType = new FileAttachmentType
			{
				Name = Path.ChangeExtension(this.exportRecordLogFileName, ".zip"),
				Content = this.exportRecordAttachmentLog.GetCompressedLogData()
			};
			List<AttachmentType> list = this.ewsClient.CreateAttachments(this.PrimarySmtpAddress, this.logItemId, new AttachmentType[]
			{
				fileAttachmentType
			});
			if (list != null && list.Count > 0)
			{
				this.exportRecordAttachmentId = list[0].AttachmentId.Id;
			}
		}

		private IEnumerable<string> CreateLogEntriesFromExportRecords(IEnumerable<ExportRecord> records)
		{
			foreach (ExportRecord er in records)
			{
				yield return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24}", new object[]
				{
					er.SourceId,
					Util.QuoteValueIfRequired(er.OriginalPath.Replace(er.SourceId, string.Empty)),
					Util.QuoteValueIfRequired(er.Title),
					er.IsRead,
					er.SentTime,
					er.ReceivedTime,
					Util.QuoteValueIfRequired(er.Sender),
					Util.QuoteValueIfRequired(er.SenderSmtpAddress),
					er.Importance,
					string.Empty,
					er.Id,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				});
			}
			yield break;
		}

		private void Initialize(string primarySmtpAddress, string legacyDN, IExportContext exportContext)
		{
			this.PrimarySmtpAddress = primarySmtpAddress;
			this.LegacyDistinguishedName = legacyDN;
			this.ExportContext = exportContext;
			if (this.ExportContext != null)
			{
				if (!this.ExportContext.IsResume)
				{
					this.DeleteWorkingFolders();
				}
				this.CreateWorkingFolderIfNotExist();
			}
			this.StatusMailRecipients = new string[0];
		}

		private BaseFolderType GetWorkingOrResultFolder(string folderName)
		{
			DistinguishedFolderIdType parentFolderId = new DistinguishedFolderIdType
			{
				Id = DistinguishedFolderIdNameType.msgfolderroot,
				Mailbox = new EmailAddressType
				{
					EmailAddress = this.PrimarySmtpAddress
				}
			};
			return this.ewsClient.GetFolderByName(this.PrimarySmtpAddress, parentFolderId, folderName);
		}

		private BaseFolderType GetOrCreateRecycleFolder(bool createFolderIfNotExist)
		{
			DistinguishedFolderIdType folderId = new DistinguishedFolderIdType
			{
				Id = DistinguishedFolderIdNameType.root,
				Mailbox = new EmailAddressType
				{
					EmailAddress = this.PrimarySmtpAddress
				}
			};
			BaseFolderType folderById = this.ewsClient.GetFolderById(this.PrimarySmtpAddress, folderId);
			BaseFolderType baseFolderType = this.ewsClient.GetFolderByName(this.PrimarySmtpAddress, folderById.FolderId, Constants.MailboxSearchRecycleFolderName);
			if (baseFolderType == null && createFolderIfNotExist)
			{
				baseFolderType = this.CreateFolder(folderById.FolderId, Constants.MailboxSearchRecycleFolderName, true);
			}
			return baseFolderType;
		}

		private void CreateWorkingFolderIfNotExist()
		{
			BaseFolderIdType parentFolderId = new DistinguishedFolderIdType
			{
				Id = DistinguishedFolderIdNameType.msgfolderroot,
				Mailbox = new EmailAddressType
				{
					EmailAddress = this.PrimarySmtpAddress
				}
			};
			this.workingFolder = this.GetWorkingOrResultFolder(this.targetLocation.WorkingLocation);
			if (this.workingFolder == null)
			{
				FolderType folderType = new FolderType
				{
					DisplayName = this.targetLocation.WorkingLocation
				};
				folderType.ExtendedProperty = new ExtendedPropertyType[]
				{
					MailboxItemIdList.IsHiddenExtendedProperty
				};
				List<BaseFolderType> list = this.ewsClient.CreateFolder(this.PrimarySmtpAddress, parentFolderId, new BaseFolderType[]
				{
					folderType
				});
				this.workingFolder = list[0];
			}
		}

		private void DeleteWorkingFolders()
		{
			this.RemoveItemIdList(null, false);
			this.RemoveItemIdList(null, true);
		}

		private BaseFolderType CreateRootResultFolderIfNotExist()
		{
			BaseFolderType baseFolderType = this.GetWorkingOrResultFolder(this.targetLocation.ExportLocation);
			if (baseFolderType == null)
			{
				DistinguishedFolderIdType parentFolderId = new DistinguishedFolderIdType
				{
					Id = DistinguishedFolderIdNameType.msgfolderroot,
					Mailbox = new EmailAddressType
					{
						EmailAddress = this.PrimarySmtpAddress
					}
				};
				baseFolderType = this.CreateFolder(parentFolderId, this.targetLocation.ExportLocation, false);
			}
			return baseFolderType;
		}

		private string ToSafeFileNameString(string fileName)
		{
			return TargetMailbox.InvalidFileCharExpression.Replace(fileName, "_");
		}

		private static Regex invalidFileCharExpression;

		private readonly IEwsClient ewsClient;

		private readonly ITargetLocation targetLocation;

		private BaseFolderType workingFolder;

		private OrganizationId organizationId;

		private bool disposed;

		private string logItemId;

		private string exportRecordAttachmentId;

		private AttachmentLog exportRecordAttachmentLog;

		private string exportRecordLogFileName;

		private IStatusLog statusLog;
	}
}
