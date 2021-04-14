using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Assistants;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "ArchiveConnectivity", SupportsShouldProcess = true)]
	public sealed class TestArchiveConnectivityTask : Task
	{
		static TestArchiveConnectivityTask()
		{
			PropertyDefinition[] array = new PropertyDefinition[]
			{
				StoreObjectSchema.EntryId,
				StoreObjectSchema.ChangeKey,
				StoreObjectSchema.ParentEntryId,
				StoreObjectSchema.ParentItemId,
				StoreObjectSchema.SearchKey,
				StoreObjectSchema.RecordKey
			};
			TestArchiveConnectivityTask.mailboxExtendedProperties = new List<PropertyDefinition>(MailboxSchema.Instance.AllProperties);
			foreach (PropertyDefinition item in array)
			{
				TestArchiveConnectivityTask.mailboxExtendedProperties.Remove(item);
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
		public SmtpAddress UserSmtp
		{
			get
			{
				return (SmtpAddress)base.Fields[TestArchiveConnectivityTask.UserSmtpParam];
			}
			set
			{
				base.Fields[TestArchiveConnectivityTask.UserSmtpParam] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeArchiveMRMConfiguration
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeArchiveMRMConfiguration"] ?? false);
			}
			set
			{
				base.Fields["IncludeArchiveMRMConfiguration"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MessageId
		{
			get
			{
				return (string)base.Fields[TestArchiveConnectivityTask.MessageIdParam];
			}
			set
			{
				base.Fields[TestArchiveConnectivityTask.MessageIdParam] = value;
			}
		}

		private void PerformArchiveConnectivityTest(ref ArchiveConnectivityOutcome result)
		{
			bool flag = false;
			bool flag2 = false;
			try
			{
				SmtpAddress userSmtp = this.UserSmtp;
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromProxyAddress(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(userSmtp.Domain), userSmtp.ToString());
				if (exchangePrincipal != null)
				{
					if (this.IncludeArchiveMRMConfiguration || !string.IsNullOrEmpty(this.MessageId))
					{
						this.LogonPrimary(exchangePrincipal);
						result.PrimaryMRMConfiguration = this.primaryFAI;
						result.PrimaryLastProcessedTime = this.primaryLastProcessedTime;
					}
					flag = true;
					ADObjectId objectId = exchangePrincipal.ObjectId;
					IRecipientSession tenantOrRootOrgRecipientSession;
					if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
					{
						tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 276, "PerformArchiveConnectivityTest", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ArchiveConnectivity\\TestArchiveConnectivityTask.cs");
					}
					else
					{
						tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 284, "PerformArchiveConnectivityTest", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Monitoring\\ArchiveConnectivity\\TestArchiveConnectivityTask.cs");
					}
					ADUser aduser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(objectId);
					this.complianceConfiguration = aduser.ElcMailboxFlags.ToString();
					result.ComplianceConfiguration = this.complianceConfiguration;
					if (exchangePrincipal.GetArchiveMailbox() == null)
					{
						result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, Strings.ArchiveConnectivityResultArchiveNotProvisioned);
					}
					else
					{
						if (aduser.ArchiveDomain != null)
						{
							result.ArchiveDomain = aduser.ArchiveDomain.ToString();
							flag2 = true;
						}
						if (aduser.ArchiveDatabase != null)
						{
							result.ArchiveDatabase = aduser.ArchiveDatabase.ToString();
						}
						if (aduser.RecipientType == RecipientType.UserMailbox)
						{
							if (flag2)
							{
								if (ArchiveStatusFlags.Active != aduser.ArchiveStatus)
								{
									result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, Strings.ArchiveConnectivityResultArchiveNotActive);
								}
								else if (this.LogonArchive(this.GetArchivePrincipal(exchangePrincipal, aduser)))
								{
									result.Update(ArchiveConnectivityResultEnum.Success, "");
								}
							}
							else if (this.LogonArchive(this.GetArchivePrincipal(exchangePrincipal, aduser)))
							{
								result.Update(ArchiveConnectivityResultEnum.Success, "");
							}
						}
						if (this.IncludeArchiveMRMConfiguration)
						{
							result.ArchiveMRMConfiguration = this.archiveFAI;
							result.ArchiveLastProcessedTime = this.archiveLastProcessedTime;
						}
					}
				}
				this.mrmProperties = this.mrmPropReport.ToString();
				if (!string.IsNullOrEmpty(this.mrmProperties))
				{
					result.ItemMRMProperties = this.mrmProperties;
				}
				else if (!string.IsNullOrEmpty(this.MessageId))
				{
					result.ItemMRMProperties = "Item not found.";
				}
			}
			catch (ObjectNotFoundException ex)
			{
				if (!flag)
				{
					result.Update(ArchiveConnectivityResultEnum.PrimaryFailure, this.GetAllInnerExceptions(ex));
				}
				else
				{
					result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex));
				}
			}
			catch (ConnectionFailedTransientException ex2)
			{
				result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex2));
			}
			catch (AutoDAccessException ex3)
			{
				result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex3));
			}
			catch (StoragePermanentException ex4)
			{
				result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex4));
			}
			catch (StorageTransientException ex5)
			{
				result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex5));
			}
			catch (ArgumentException ex6)
			{
				result.Update(ArchiveConnectivityResultEnum.ArchiveFailure, this.GetAllInnerExceptions(ex6));
			}
		}

		private bool LogonArchive(ExchangePrincipal mailboxEP)
		{
			bool result;
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(mailboxEP, CultureInfo.InvariantCulture, "Client=Monitoring;Action=Test-ArchiveConnectivity"))
			{
				if (mailboxSession != null)
				{
					if (this.IncludeArchiveMRMConfiguration || !string.IsNullOrEmpty(this.MessageId))
					{
						UserConfiguration userConfiguration = ElcMailboxHelper.OpenFaiMessage(mailboxSession, "MRM", false);
						if (userConfiguration != null)
						{
							using (Stream xmlStream = userConfiguration.GetXmlStream())
							{
								using (StreamReader streamReader = new StreamReader(xmlStream))
								{
									this.archiveFAI = streamReader.ReadToEnd();
								}
								goto IL_80;
							}
						}
						this.archiveFAI = "No FAI found in Archive Mailbox.";
						IL_80:
						this.archiveLastProcessedTime = this.ReadMailboxTableProperties(mailboxSession);
						if (!string.IsNullOrEmpty(this.MessageId))
						{
							this.GetELCItemProperties(mailboxSession, this.MessageId);
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void LogonPrimary(ExchangePrincipal primaryEP)
		{
			using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(primaryEP, CultureInfo.InvariantCulture, "Client=Monitoring;Action=Test-ArchiveConnectivity"))
			{
				if (mailboxSession != null)
				{
					UserConfiguration userConfiguration = ElcMailboxHelper.OpenFaiMessage(mailboxSession, "MRM", false);
					if (userConfiguration != null)
					{
						using (Stream xmlStream = userConfiguration.GetXmlStream())
						{
							using (StreamReader streamReader = new StreamReader(xmlStream))
							{
								this.primaryFAI = streamReader.ReadToEnd();
							}
							goto IL_63;
						}
					}
					this.primaryFAI = "No FAI found in Primary Mailbox.";
					IL_63:
					this.primaryLastProcessedTime = this.ReadMailboxTableProperties(mailboxSession);
					if (!string.IsNullOrEmpty(this.MessageId))
					{
						this.GetELCItemProperties(mailboxSession, this.MessageId);
					}
				}
			}
		}

		private void GetELCItemProperties(MailboxSession mailboxSession, string messageId)
		{
			if (mailboxSession.GetDefaultFolderId(DefaultFolderType.AllItems) == null)
			{
				mailboxSession.CreateDefaultFolder(DefaultFolderType.AllItems);
			}
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.InternetMessageId, messageId);
			object[][] array = null;
			foreach (DefaultFolderType defaultFolderType in TestArchiveConnectivityTask.mailboxFolders)
			{
				if (mailboxSession.GetDefaultFolderId(defaultFolderType) != null)
				{
					using (Folder folder = Folder.Bind(mailboxSession, defaultFolderType))
					{
						using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, new PropertyDefinition[]
						{
							ItemSchema.Id,
							StoreObjectSchema.ParentItemId
						}))
						{
							array = queryResult.GetRows(queryResult.EstimatedRowCount);
							if (array.Length > 0)
							{
								for (int j = 0; j < array.Length; j++)
								{
									VersionedId storeId = (VersionedId)array[j][0];
									using (Item item = Item.Bind(mailboxSession, storeId, TestArchiveConnectivityTask.mrmStoreProps))
									{
										if (item != null)
										{
											this.mrmPropReport.AppendLine(this.GetMRMProps(item));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private string GetMRMProps(Item item)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			byte[] propertyBytes = null;
			string str = string.Empty;
			try
			{
				Guid guid = new Guid((byte[])item[StoreObjectSchema.PolicyTag]);
				stringBuilder.AppendLine("RetentionTagGuid: " + guid);
			}
			catch (PropertyErrorException)
			{
				stringBuilder.AppendLine("RetentionTagGuid: Not Found");
			}
			try
			{
				propertyBytes = (byte[])item[ItemSchema.StartDateEtc];
				flag = true;
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				if (flag)
				{
					DateTime value = CompositeProperty.Parse(propertyBytes, true).Date.Value;
					stringBuilder.AppendLine("StartDate: " + value);
				}
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				DateTime universalTime = ((ExDateTime)item[ItemSchema.RetentionDate]).UniversalTime;
				stringBuilder.AppendLine("RetentionDate: " + universalTime);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				int? num = new int?((int)item[StoreObjectSchema.RetentionPeriod]);
				stringBuilder.AppendLine("RetentionPeriod: " + num);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				Guid guid2 = new Guid((byte[])item[StoreObjectSchema.ArchiveTag]);
				stringBuilder.AppendLine("ArchiveGuid: " + guid2);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				DateTime universalTime2 = ((ExDateTime)item[ItemSchema.ArchiveDate]).UniversalTime;
				stringBuilder.AppendLine("ArchiveDate: " + universalTime2);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				int? num2 = new int?((int)item[StoreObjectSchema.ArchivePeriod]);
				stringBuilder.AppendLine("ArchivePeriod: " + num2);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				DateTime universalTime3 = ((ExDateTime)item[ItemSchema.ReceivedTime]).UniversalTime;
				stringBuilder.AppendLine("ReceivedDate: " + universalTime3);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				DateTime universalTime4 = ((ExDateTime)item[StoreObjectSchema.LastModifiedTime]).UniversalTime;
				stringBuilder.AppendLine("LastModifiedTime: " + universalTime4);
			}
			catch (PropertyErrorException)
			{
			}
			try
			{
				str = item[StoreObjectSchema.ParentItemId].ToString();
				stringBuilder.AppendLine("FolderId: " + str);
			}
			catch (PropertyErrorException)
			{
			}
			return stringBuilder.ToString();
		}

		private ExchangePrincipal GetArchivePrincipal(ExchangePrincipal primaryEP, ADUser adUser)
		{
			return primaryEP.GetArchiveExchangePrincipal(RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
		}

		private string GetAllInnerExceptions(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (ex != null)
			{
				stringBuilder.Append(ex.Message.ToString());
				stringBuilder.AppendLine();
				ex = ex.InnerException;
			}
			return stringBuilder.ToString();
		}

		private string ReadMailboxTableProperties(MailboxSession archiveSession)
		{
			string result = string.Empty;
			archiveSession.Mailbox.Load(TestArchiveConnectivityTask.mailboxExtendedProperties);
			object[] properties = archiveSession.Mailbox.GetProperties(TestArchiveConnectivityTask.mailboxExtendedProperties);
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in TestArchiveConnectivityTask.mailboxExtendedProperties)
			{
				if (!(properties[num] is PropertyError))
				{
					if (propertyDefinition.Name.Equals(MailboxSchema.ControlDataForElcAssistant.Name))
					{
						if (propertyDefinition.Type.Equals(typeof(byte[])))
						{
							byte[] serializedData = (byte[])properties[num];
							ControlData controlData = ControlData.CreateFromByteArray(serializedData);
							result = controlData.LastProcessedDate.ToString("MMMM dd yyyy hh:mm tt") + " - GMT";
							break;
						}
						break;
					}
					else
					{
						result = "No Last Processed time found for the mailbox.";
					}
				}
				num++;
			}
			return result;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalValidate();
				if (base.HasErrors)
				{
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ArchiveConnectivityOutcome sendToPipeline = new ArchiveConnectivityOutcome(this.UserSmtp.ToString(), this.primaryFAI, this.primaryLastProcessedTime, this.archiveDomain, this.archiveDatabase, this.archiveFAI, this.archiveLastProcessedTime, this.complianceConfiguration, this.mrmProperties);
				this.PerformArchiveConnectivityTest(ref sendToPipeline);
				base.WriteObject(sendToPipeline);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestArchiveConnectivityIdentity(this.UserSmtp.ToString());
			}
		}

		internal const string ArchiveConnectivity = "ArchiveConnectivity";

		private static SmtpAddress UserSmtpParam = default(SmtpAddress);

		private static string MessageIdParam = string.Empty;

		private StringBuilder mrmPropReport = new StringBuilder();

		private string primaryFAI = string.Empty;

		private string primaryLastProcessedTime = string.Empty;

		private string archiveFAI = string.Empty;

		private string archiveLastProcessedTime = string.Empty;

		private string complianceConfiguration = string.Empty;

		private readonly string archiveDomain = string.Empty;

		private readonly string archiveDatabase = string.Empty;

		private static List<PropertyDefinition> mailboxExtendedProperties = null;

		private string mrmProperties = string.Empty;

		private static readonly DefaultFolderType[] mailboxFolders = new DefaultFolderType[]
		{
			DefaultFolderType.AllItems,
			DefaultFolderType.RecoverableItemsVersions,
			DefaultFolderType.RecoverableItemsDeletions,
			DefaultFolderType.RecoverableItemsPurges,
			DefaultFolderType.RecoverableItemsDiscoveryHolds
		};

		private static readonly PropertyDefinition[] mrmStoreProps = new PropertyDefinition[]
		{
			StoreObjectSchema.PolicyTag,
			ItemSchema.StartDateEtc,
			ItemSchema.RetentionDate,
			StoreObjectSchema.RetentionPeriod,
			StoreObjectSchema.ArchiveTag,
			StoreObjectSchema.ArchivePeriod,
			ItemSchema.ArchiveDate,
			ItemSchema.ReceivedTime,
			StoreObjectSchema.LastModifiedTime,
			StoreObjectSchema.ParentItemId
		};
	}
}
