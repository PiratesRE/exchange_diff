using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationReportItem : IMigrationSerializable
	{
		private MigrationReportItem()
		{
		}

		protected MigrationReportItem(string reportName, Guid? jobId, MigrationType migrationType, MigrationReportType reportType, bool isStaged)
		{
			this.ReportName = reportName;
			this.JobId = jobId;
			this.MigrationType = migrationType;
			this.ReportType = reportType;
			this.IsStagedMigration = isStaged;
		}

		public long Version { get; protected set; }

		public StoreObjectId MessageId { get; protected set; }

		public ExDateTime CreationTime { get; protected set; }

		public ExDateTime? ReportedTime { get; protected set; }

		public MigrationReportId Identifier
		{
			get
			{
				return new MigrationReportId(this.MessageId);
			}
		}

		public string ReportName { get; private set; }

		public Guid? JobId { get; private set; }

		public MigrationType MigrationType { get; private set; }

		public MigrationReportType ReportType { get; private set; }

		public bool IsStagedMigration { get; private set; }

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MigrationReportItem.MigrationReportItemColumnsIndex;
			}
		}

		public static IEnumerable<MigrationReportItem> GetAll(IMigrationDataProvider dataProvider)
		{
			IEnumerable<StoreObjectId> messageIds = MigrationHelper.FindMessageIds(dataProvider, MigrationReportItem.MessageClassEqualityFilter, null, null, null);
			foreach (StoreObjectId messageId in messageIds)
			{
				yield return MigrationReportItem.Get(dataProvider, new MigrationReportId(messageId));
			}
			yield break;
		}

		public static int GetCount(IMigrationDataProvider dataProvider, Guid jobId)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnGuidEmptyArgument(jobId, "jobId");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, MigrationBatchMessageSchema.MigrationJobId, jobId);
			return dataProvider.CountMessages(filter, null);
		}

		public static IEnumerable<MigrationReportItem> GetByJobId(IMigrationDataProvider dataProvider, Guid? jobId, int maxCount)
		{
			MigrationEqualityFilter[] secondaryFilters = null;
			if (jobId != null)
			{
				secondaryFilters = new MigrationEqualityFilter[]
				{
					new MigrationEqualityFilter(MigrationBatchMessageSchema.MigrationJobId, jobId.Value)
				};
			}
			SortBy[] sortBy = new SortBy[]
			{
				new SortBy(InternalSchema.MigrationJobItemStateLastUpdated, SortOrder.Ascending),
				new SortBy(InternalSchema.CreationTime, SortOrder.Ascending)
			};
			IEnumerable<StoreObjectId> messageIds = MigrationHelper.FindMessageIds(dataProvider, MigrationReportItem.MessageClassEqualityFilter, secondaryFilters, sortBy, new int?(maxCount));
			foreach (StoreObjectId messageId in messageIds)
			{
				yield return MigrationReportItem.Get(dataProvider, new MigrationReportId(messageId));
			}
			yield break;
		}

		public static MigrationReportItem Get(IMigrationDataProvider dataProvider, MigrationReportId reportId)
		{
			MigrationReportItem result;
			using (IMigrationMessageItem migrationMessageItem = dataProvider.FindMessage(reportId.Id, MigrationReportItem.MigrationReportItemColumnsIndex))
			{
				MigrationReportItem migrationReportItem = new MigrationReportItem();
				migrationReportItem.ReadFromMessageItem(migrationMessageItem);
				result = migrationReportItem;
			}
			return result;
		}

		public static MigrationReportItem Create(IMigrationDataProvider dataProvider, Guid? jobId, MigrationType migrationType, bool isStaged, MigrationReportType reportType, string reportName)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullOrEmptyArgument(reportName, "reportName");
			MigrationReportItem migrationReportItem = new MigrationReportItem(reportName, jobId, migrationType, reportType, isStaged);
			migrationReportItem.Version = 2L;
			using (IMigrationMessageItem migrationMessageItem = dataProvider.CreateMessage())
			{
				using (IMigrationAttachment migrationAttachment = migrationMessageItem.CreateAttachment(reportName))
				{
					migrationAttachment.Save(null);
				}
				migrationReportItem.WriteToMessageItem(migrationMessageItem, true);
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
				migrationMessageItem.Load(MigrationHelper.ItemIdProperties);
				migrationReportItem.MessageId = migrationMessageItem.Id;
			}
			return migrationReportItem;
		}

		public MigrationReport GetMigrationReport(IMigrationDataProvider dataProvider, Stream csvStream, int startingRowIndex, int rowCount, bool revealPassword)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "provider");
			MigrationReport migrationReport = new MigrationReport();
			migrationReport.Identity = this.Identifier;
			migrationReport.ReportName = this.ReportName;
			migrationReport.JobId = (this.JobId ?? Guid.Empty);
			migrationReport.MigrationType = this.MigrationType;
			migrationReport.ReportType = this.ReportType;
			using (IMigrationMessageItem migrationMessageItem = dataProvider.FindMessage(this.MessageId, MigrationReportItem.MigrationReportItemColumnsIndex))
			{
				using (IMigrationAttachment attachment = migrationMessageItem.GetAttachment(this.ReportName, PropertyOpenMode.ReadOnly))
				{
					CsvSchema csvSchema = null;
					switch (this.ReportType)
					{
					case MigrationReportType.BatchSuccessReport:
						csvSchema = MigrationSuccessReportCsvSchema.GetSchema(this.MigrationType, true, this.IsStagedMigration);
						break;
					case MigrationReportType.BatchFailureReport:
						csvSchema = MigrationFailureReportCsvSchema.GetSchema(this.MigrationType, true);
						break;
					case MigrationReportType.FinalizationSuccessReport:
						csvSchema = MigrationSuccessReportCsvSchema.GetSchema(this.MigrationType, false, this.IsStagedMigration);
						break;
					case MigrationReportType.FinalizationFailureReport:
						csvSchema = MigrationFailureReportCsvSchema.GetSchema(this.MigrationType, false);
						break;
					case MigrationReportType.BatchReport:
						csvSchema = new MigrationReportCsvSchema(MigrationJob.MigrationTypeSupportsProvisioning(this.MigrationType));
						break;
					}
					bool containsPasswordColumn = (this.MigrationType == MigrationType.ExchangeOutlookAnywhere && !this.IsStagedMigration && this.ReportType == MigrationReportType.BatchSuccessReport) || (this.ReportType == MigrationReportType.BatchReport && MigrationJob.MigrationTypeSupportsProvisioning(this.MigrationType));
					bool flag = false;
					Stream stream = null;
					Stream stream2 = csvStream;
					if (stream2 == null)
					{
						flag = true;
						stream = new MemoryStream();
						stream2 = stream;
					}
					using (stream)
					{
						if (csvSchema != null)
						{
							try
							{
								csvSchema.Copy(attachment.Stream, new StreamWriter(stream2), delegate(CsvRow source)
								{
									if (containsPasswordColumn && source.Index != 0)
									{
										if (revealPassword)
										{
											string encryptedString = source["Password"];
											string value = MigrationUtil.EncryptedStringToClearText(encryptedString);
											source["Password"] = value;
										}
										else
										{
											source["Password"] = "password hidden from DC admins";
										}
									}
									return source;
								});
								goto IL_1C4;
							}
							catch (CsvFileIsEmptyException)
							{
								goto IL_1C4;
							}
						}
						Util.StreamHandler.CopyStreamData(attachment.Stream, stream2);
						IL_1C4:
						if (flag)
						{
							stream.Seek(0L, SeekOrigin.Begin);
							migrationReport.Rows = MigrationReportItem.GetRows(stream2, startingRowIndex, rowCount);
						}
						else
						{
							migrationReport.Rows = new MultiValuedProperty<string>();
						}
					}
				}
			}
			return migrationReport;
		}

		public string GetUrl(IMigrationDataProvider dataProvider)
		{
			Uri ecpUrl = dataProvider.GetEcpUrl();
			if (ecpUrl == null)
			{
				return null;
			}
			UriBuilder uriBuilder = new UriBuilder(ecpUrl);
			if (!uriBuilder.Path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				UriBuilder uriBuilder2 = uriBuilder;
				uriBuilder2.Path += "/";
			}
			UriBuilder uriBuilder3 = uriBuilder;
			uriBuilder3.Path += "Migration/DownloadReport.aspx";
			string text = "OrganizationContext";
			if (dataProvider.ADProvider.IsMSOSyncEnabled)
			{
				text = "DelegatedOrg";
			}
			uriBuilder.Query = string.Format(CultureInfo.InvariantCulture, "HandlerClass=MigrationReportHandler&Name={0}&{1}={2}&realm={2}&exsvurl=1&Identity={3}", new object[]
			{
				HttpUtility.UrlEncode(this.ReportName),
				text,
				HttpUtility.UrlEncode(dataProvider.ADProvider.TenantOrganizationName),
				HttpUtility.UrlEncode(this.Identifier.ToString())
			});
			return uriBuilder.Uri.AbsoluteUri;
		}

		public void WriteStream(IMigrationDataProvider dataProvider, Action<StreamWriter> writer)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(writer, "writer");
			using (IMigrationMessageItem migrationMessageItem = dataProvider.FindMessage(this.MessageId, MigrationReportItem.MigrationReportItemColumnsIndex))
			{
				migrationMessageItem.OpenAsReadWrite();
				using (IMigrationAttachment attachment = migrationMessageItem.GetAttachment(this.ReportName, PropertyOpenMode.Modify))
				{
					attachment.Stream.Seek(0L, SeekOrigin.End);
					using (StreamWriter streamWriter = new StreamWriter(attachment.Stream, Encoding.UTF8))
					{
						writer(streamWriter);
					}
					attachment.Save(null);
					this.ReportedTime = new ExDateTime?(ExDateTime.UtcNow);
					migrationMessageItem[MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated] = this.ReportedTime;
				}
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
		}

		public void UpdateReportItem(IMigrationDataProvider dataProvider, Guid jobId)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			using (IMigrationMessageItem migrationMessageItem = dataProvider.FindMessage(this.MessageId, MigrationReportItem.MigrationReportItemUpdate))
			{
				migrationMessageItem.OpenAsReadWrite();
				migrationMessageItem[MigrationBatchMessageSchema.MigrationJobId] = jobId;
				migrationMessageItem.Save(SaveMode.NoConflictResolution);
			}
			this.JobId = new Guid?(jobId);
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			message[MigrationBatchMessageSchema.MigrationVersion] = this.Version;
			message[StoreObjectSchema.ItemClass] = MigrationBatchMessageSchema.MigrationReportItemClass;
			message[MigrationBatchMessageSchema.MigrationReportName] = this.ReportName;
			if (this.Version == 1L)
			{
				return;
			}
			if (this.JobId != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobId] = this.JobId;
			}
			message[MigrationBatchMessageSchema.MigrationType] = (int)this.MigrationType;
			message[MigrationBatchMessageSchema.MigrationReportType] = (int)this.ReportType;
			message[MigrationBatchMessageSchema.MigrationJobIsStaged] = this.IsStagedMigration;
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			this.MessageId = message.Id;
			this.CreationTime = message.CreationTime;
			this.Version = (long)message[MigrationBatchMessageSchema.MigrationVersion];
			if (this.Version > 3L)
			{
				throw new MigrationVersionMismatchException(this.Version, 3L);
			}
			this.ReportName = (string)message[MigrationBatchMessageSchema.MigrationReportName];
			if (this.Version > 1L)
			{
				Guid guidProperty = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobId, false);
				if (guidProperty != Guid.Empty)
				{
					this.JobId = new Guid?(guidProperty);
				}
				this.MigrationType = (MigrationType)message[MigrationBatchMessageSchema.MigrationType];
				this.ReportType = (MigrationReportType)message[MigrationBatchMessageSchema.MigrationReportType];
				this.IsStagedMigration = message.GetValueOrDefault<bool>(MigrationBatchMessageSchema.MigrationJobIsStaged, false);
			}
			this.ReportedTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated);
			return true;
		}

		public void Delete(IMigrationDataProvider provider)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			provider.RemoveMessage(this.MessageId);
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationReportItem", new object[]
			{
				new XElement("ReportName", this.ReportName),
				new XElement("CreationTime", this.CreationTime),
				new XElement("ReportedTime", this.ReportedTime),
				new XElement("MessageId", this.MessageId),
				new XElement("Version", this.Version),
				new XElement("JobID", this.JobId),
				new XElement("MigrationType", this.MigrationType),
				new XElement("MigrationReportType", this.ReportType)
			});
			if (dataProvider != null && argument.HasArgument("storage"))
			{
				xelement.Add(this.GetStorageDiagnosticInfo(dataProvider, argument));
			}
			return xelement;
		}

		private static MultiValuedProperty<string> GetRows(Stream sourceStream, int startingRowIndex, int rowCount)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", rowCount, "rowCount must be greater than 0.");
			}
			if (startingRowIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startingRowIndex", startingRowIndex, "startingRowIndex must be greater than 0.");
			}
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			using (StreamReader streamReader = new StreamReader(sourceStream, true))
			{
				int num = 0;
				string text = streamReader.ReadLine();
				if (text == null)
				{
					return multiValuedProperty;
				}
				multiValuedProperty.Add(text);
				while (multiValuedProperty.Count < rowCount + 1)
				{
					num++;
					text = streamReader.ReadLine();
					if (text == null)
					{
						break;
					}
					if (num > startingRowIndex)
					{
						multiValuedProperty.Add(text);
					}
				}
			}
			return multiValuedProperty;
		}

		private XElement GetStorageDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			ExAssert.RetailAssert(this.MessageId != null, "Need to persist the objects before trying to retrieve their diagnostics");
			XElement diagnosticInfo;
			using (IMigrationMessageItem migrationMessageItem = dataProvider.FindMessage(this.MessageId, MigrationReportItem.MigrationReportItemColumnsIndex))
			{
				diagnosticInfo = migrationMessageItem.GetDiagnosticInfo(this.PropertyDefinitions, argument);
			}
			return diagnosticInfo;
		}

		private const string DownloadReportPagePath = "Migration/DownloadReport.aspx";

		private const string MigrationReportHandlerQueryFormat = "HandlerClass=MigrationReportHandler&Name={0}&{1}={2}&realm={2}&exsvurl=1&Identity={3}";

		private const string BposOrgContextParameter = "DelegatedOrg";

		private const string EduOrgContextParameter = "OrganizationContext";

		protected const long MigrationReportItemLegacySupportedVersion = 2L;

		protected const long MigrationReportItemCurrentSupportedVersion = 3L;

		private static readonly MigrationEqualityFilter MessageClassEqualityFilter = new MigrationEqualityFilter(StoreObjectSchema.ItemClass, MigrationBatchMessageSchema.MigrationReportItemClass);

		internal static readonly PropertyDefinition[] MigrationReportItemColumnsIndex = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass,
				MigrationBatchMessageSchema.MigrationVersion,
				MigrationBatchMessageSchema.MigrationReportName,
				MigrationBatchMessageSchema.MigrationJobId,
				MigrationBatchMessageSchema.MigrationType,
				MigrationBatchMessageSchema.MigrationReportType,
				MigrationBatchMessageSchema.MigrationJobIsStaged,
				MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated
			},
			MigrationStoreObject.IdPropertyDefinition
		});

		private static readonly PropertyDefinition[] MigrationReportItemUpdate = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobId
		};
	}
}
