using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon.Exceptions;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal static class UMReportUtil
	{
		public static void DoAggregation(MailboxSession session)
		{
			ValidateArgument.NotNull(session, "session");
			CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "Starting UMReportUtil.DoAggregation", new object[0]);
			Folder folder = null;
			UserConfiguration userConfiguration = null;
			try
			{
				if (!UMStagingFolder.TryOpenUMReportingFolder(session, out folder))
				{
					throw new UnableToFindUMReportDataException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				UMReportAggregatedData aggregatedData = null;
				if (!UMReportUtil.TryGetUMReportConfig(session, out userConfiguration))
				{
					aggregatedData = new UMReportAggregatedData();
					userConfiguration = session.UserConfigurationManager.CreateMailboxConfiguration("UM.UMReportAggregatedData", UserConfigurationTypes.XML);
				}
				else
				{
					using (Stream xmlStream = userConfiguration.GetXmlStream())
					{
						aggregatedData = UMTypeSerializer.DeserializeFromStream<UMReportAggregatedData>(xmlStream);
					}
				}
				ExDateTime startDate = new ExDateTime(ExTimeZone.UtcTimeZone, aggregatedData.WaterMark);
				ExDateTime endDate = Utils.RunningInTestMode ? ExDateTime.UtcNow : ExDateTime.UtcNow.Subtract(UMReportUtil.TimeDelta);
				UMReportUtil.ReadAndProcessCDRsHelper(folder, startDate, endDate, 0, 500000, delegate(CDRData cdrData)
				{
					aggregatedData.AddCDR(cdrData);
				});
				aggregatedData.Cleanup(session.MailboxOwner.MailboxInfo.OrganizationId);
				using (Stream xmlStream2 = userConfiguration.GetXmlStream())
				{
					xmlStream2.SetLength(0L);
					UMTypeSerializer.SerializeToStream<UMReportAggregatedData>(xmlStream2, aggregatedData);
				}
				userConfiguration.Save();
				CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "Returning UMReportUtil.DoAggregation", new object[0]);
			}
			catch (SerializationException ex)
			{
				CallIdTracer.TraceError(UMReportUtil.Tracer, 0, "DoAggregation: Exception {0}", new object[]
				{
					ex
				});
				throw new CorruptDataException(Strings.SerializationError(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()), ex);
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
				if (userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
			}
		}

		public static bool TryGetUMReportConfig(MailboxSession session, out UserConfiguration config)
		{
			ValidateArgument.NotNull(session, "session");
			config = null;
			try
			{
				config = session.UserConfigurationManager.GetMailboxConfiguration("UM.UMReportAggregatedData", UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException ex)
			{
				CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "TryGetUMReportConfig: Exception {0}", new object[]
				{
					ex
				});
			}
			return config != null;
		}

		public static bool TryDeleteUMReportConfig(MailboxSession session)
		{
			ValidateArgument.NotNull(session, "session");
			return session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
			{
				"UM.UMReportAggregatedData"
			}) == OperationResult.Succeeded;
		}

		public static CDRData[] ReadCDRs(MailboxSession session, ExDateTime startDate, ExDateTime endDate, int offset, int numberOfRecords)
		{
			ValidateArgument.NotNull(session, "session");
			if (numberOfRecords < 0)
			{
				throw new ArgumentException("Number of records to be fetched should be greater than or equal to 0");
			}
			if (offset < 0)
			{
				throw new ArgumentException("Offset should be greater than or equal to 0");
			}
			if (numberOfRecords > 5000)
			{
				numberOfRecords = 5000;
			}
			Folder folder = null;
			CDRData[] result;
			try
			{
				if (!UMStagingFolder.TryOpenUMReportingFolder(session, out folder))
				{
					throw new UnableToFindUMReportDataException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				List<CDRData> cdrDataCollection = new List<CDRData>(numberOfRecords);
				UMReportUtil.ReadAndProcessCDRsHelper(folder, startDate, endDate, offset, numberOfRecords, delegate(CDRData cdrData)
				{
					cdrDataCollection.Add(cdrData);
				});
				result = cdrDataCollection.ToArray();
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
			}
			return result;
		}

		public static UMReportRawCounters[] QueryUMReport(MailboxSession session, Guid dialPlanGuid, Guid gatewayGuid, GroupBy groupBy)
		{
			ValidateArgument.NotNull(session, "session");
			UMReportRawCounters[] result;
			try
			{
				IUMAggregatedData iumaggregatedData = UMReportUtil.LoadUMReport(session);
				result = iumaggregatedData.QueryAggregatedData(dialPlanGuid, gatewayGuid, groupBy);
			}
			catch (UnableToFindUMReportDataException ex)
			{
				CallIdTracer.TraceError(UMReportUtil.Tracer, 0, "QueryUMReport: Exception {0}", new object[]
				{
					ex
				});
				result = null;
			}
			return result;
		}

		public static CDRData[] PerformCDRSearchUsingCI(MailboxSession session, string userLegacyExchangeDN, out SearchState searchState)
		{
			searchState = SearchState.None;
			List<CDRData> list = new List<CDRData>();
			StoreObjectId folderId = UMReportUtil.ExecuteCISearch(session, userLegacyExchangeDN, out searchState);
			using (Folder folder = Folder.Bind(session, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, UMReportUtil.CallStartTimeSortByDescending, UMReportUtil.ColumnsToPreLoad))
				{
					bool flag = true;
					while (flag)
					{
						object[][] rows = queryResult.GetRows(10000);
						flag = (rows.Length > 0);
						foreach (object[] row in rows)
						{
							CDRData item = null;
							if (UMReportUtil.TryCreateCDRDataFromQueryResult(row, out item))
							{
								list.Add(item);
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		private static StoreObjectId ExecuteCISearch(MailboxSession session, string userLegacyExchangeDN, out SearchState searchState)
		{
			if (!session.Mailbox.IsContentIndexingEnabled)
			{
				throw new ContentIndexingNotEnabledException();
			}
			StoreObjectId result = null;
			TextFilter[] array = new TextFilter[UMReportUtil.UserPropertiesToSearch.Length];
			for (int i = 0; i < UMReportUtil.UserPropertiesToSearch.Length; i++)
			{
				array[i] = new TextFilter(UMReportUtil.UserPropertiesToSearch[i], userLegacyExchangeDN, MatchOptions.FullString, MatchFlags.IgnoreCase);
			}
			QueryFilter searchQuery = new OrFilter(array);
			Folder folder = null;
			try
			{
				if (!UMStagingFolder.TryOpenUMReportingFolder(session, out folder))
				{
					throw new UnableToFindUMReportDataException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				string displayName = "EXUM-Reporting" + Guid.NewGuid().ToString();
				using (SearchFolder searchFolder = SearchFolder.Create(session, session.GetDefaultFolderId(DefaultFolderType.SearchFolders), displayName, CreateMode.OpenIfExists))
				{
					searchFolder.Save();
					searchFolder.Load();
					IAsyncResult asyncResult = searchFolder.BeginApplyOneTimeSearch(new SearchFolderCriteria(searchQuery, new StoreId[]
					{
						folder.StoreObjectId
					})
					{
						DeepTraversal = false
					}, null, null);
					searchFolder.EndApplyOneTimeSearch(asyncResult);
					searchFolder.Save();
					searchFolder.Load(new PropertyDefinition[]
					{
						FolderSchema.ItemCount
					});
					searchState = searchFolder.GetSearchCriteria().SearchState;
					result = searchFolder.Id.ObjectId;
				}
			}
			finally
			{
				if (folder != null)
				{
					folder.Dispose();
				}
			}
			return result;
		}

		public static IUMAggregatedData LoadUMReport(MailboxSession session)
		{
			ValidateArgument.NotNull(session, "session");
			UserConfiguration userConfiguration = null;
			IUMAggregatedData result;
			try
			{
				if (!UMReportUtil.TryGetUMReportConfig(session, out userConfiguration))
				{
					throw new UnableToFindUMReportDataException(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					result = UMTypeSerializer.DeserializeFromStream<UMReportAggregatedData>(xmlStream);
				}
			}
			catch (SerializationException ex)
			{
				CallIdTracer.TraceError(UMReportUtil.Tracer, 0, "LoadUMReport: Exception {0}", new object[]
				{
					ex
				});
				throw new CorruptDataException(Strings.SerializationError(session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()), ex);
			}
			finally
			{
				if (userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
			}
			return result;
		}

		public static bool TryCreateCDRDataFromQueryResult(object[] row, out CDRData cdrData)
		{
			ValidateArgument.NotNull(row, "row");
			DateTime universalTime = XsoUtil.GetProperty<ExDateTime>(row, MessageItemSchema.XCDRDataCallStartTime, UMReportUtil.ColumnsToPreLoad).UniversalTime;
			AudioQuality audioQuality = AudioQuality.CreateDefaultAudioQuality();
			audioQuality.NMOS = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataNMOS, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.NMOSDegradation = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataNMOSDegradation, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.NMOSDegradationPacketLoss = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataNMOSDegradationPacketLoss, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.NMOSDegradationJitter = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataNMOSDegradationJitter, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.Jitter = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataJitter, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.PacketLoss = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataPacketLoss, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.RoundTrip = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataRoundTrip, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.BurstDensity = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataBurstDensity, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.BurstDuration = XsoUtil.GetProperty<float>(row, MessageItemSchema.XCDRDataBurstDuration, UMReportUtil.ColumnsToPreLoad, AudioQuality.UnknownValue);
			audioQuality.AudioCodec = XsoUtil.GetProperty<string>(row, MessageItemSchema.XCDRDataAudioCodec, UMReportUtil.ColumnsToPreLoad, string.Empty);
			cdrData = new CDRData(universalTime, XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCallType, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCallIdentity, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataParentCallIdentity, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataUMServerName, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty<Guid>(row, MessageItemSchema.XCDRDataDialPlanGuid, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataDialPlanName, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty<int>(row, MessageItemSchema.XCDRDataCallDuration, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataIPGatewayAddress, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataIPGatewayName, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty<Guid>(row, MessageItemSchema.XCDRDataGatewayGuid, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCalledPhoneNumber, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCallerPhoneNumber, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataOfferResult, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataDropCallReason, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataReasonForCall, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataTransferredNumber, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataDialedString, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCallerMailboxAlias, UMReportUtil.ColumnsToPreLoad), XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataCalleeMailboxAlias, UMReportUtil.ColumnsToPreLoad), string.Empty, string.Empty, XsoUtil.GetProperty(row, MessageItemSchema.XCDRDataAutoAttendantName, UMReportUtil.ColumnsToPreLoad), Guid.Empty, audioQuality, Guid.Empty);
			cdrData.CreationTime = XsoUtil.GetProperty<ExDateTime>(row, StoreObjectSchema.CreationTime, UMReportUtil.ColumnsToPreLoad).UniversalTime;
			string property = XsoUtil.GetProperty(row, StoreObjectSchema.ItemClass, UMReportUtil.ColumnsToPreLoad);
			if (string.IsNullOrEmpty(property) || !ObjectClass.IsUMCDRMessage(property) || string.IsNullOrEmpty(cdrData.CallType) || !(cdrData.CallStartTime > DateTime.MinValue) || !(cdrData.CallStartTime < DateTime.MaxValue))
			{
				bool flag = true;
				FaultInjectionUtils.FaultInjectChangeValue<bool>(3945147709U, ref flag);
				cdrData = null;
				return false;
			}
			return true;
		}

		public static void SetMailboxExtendedProperty(MailboxSession session, bool hasUMReportData)
		{
			ValidateArgument.NotNull(session, "session");
			session.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.HasUMReportData
			});
			object obj = session.Mailbox.TryGetProperty(MailboxSchema.HasUMReportData);
			if (obj is PropertyError || (bool)obj != hasUMReportData)
			{
				session.Mailbox[MailboxSchema.HasUMReportData] = hasUMReportData;
				session.Mailbox.Save();
				session.Mailbox.Load();
			}
		}

		private static void ReadAndProcessCDRsHelper(Folder umReportingFolder, ExDateTime startDate, ExDateTime endDate, int offset, int numberOfRecords, UMReportUtil.ProcessCDR userFunction)
		{
			CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "UMReportUtil.ReadAndProcessCDRsHelper. Starting to read CDRs with startDate {0} and endDate {1}.", new object[]
			{
				startDate,
				endDate
			});
			if (numberOfRecords < 0)
			{
				throw new ArgumentException("Number of records to be fetched should be greater than or equal to 0");
			}
			if (offset < 0)
			{
				throw new ArgumentException("Offset should be greater than or equal to 0");
			}
			numberOfRecords = Math.Min(numberOfRecords, 500000);
			using (QueryResult queryResult = umReportingFolder.ItemQuery(ItemQueryType.None, null, UMReportUtil.CreationTimeSortBy, UMReportUtil.ColumnsToPreLoad))
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, StoreObjectSchema.CreationTime, startDate);
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, queryFilter, SeekToConditionFlags.None))
				{
					queryResult.SeekToOffset(SeekReference.OriginCurrent, offset);
					CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "UMReportUtil.ReadAndProcessCDRsHelper. Starting to read CDRs with Filter {0} and queryResult {1} and number of records queried {2}.", new object[]
					{
						queryFilter,
						queryResult.EstimatedRowCount,
						numberOfRecords
					});
					int num = 0;
					bool flag = true;
					while (flag)
					{
						int rowCount = Math.Min(numberOfRecords - num, 10000);
						object[][] rows = queryResult.GetRows(rowCount);
						flag = (rows.Length > 0);
						foreach (object[] row in rows)
						{
							CDRData cdrdata = null;
							num++;
							if (UMReportUtil.TryCreateCDRDataFromQueryResult(row, out cdrdata))
							{
								if (cdrdata.CreationTime > endDate.UniversalTime)
								{
									flag = false;
									break;
								}
								userFunction(cdrdata);
							}
						}
					}
					CallIdTracer.TraceDebug(UMReportUtil.Tracer, 0, "UMReportUtil.ReadAndProcessCDRsHelper. Returning after reading {0} items.", new object[]
					{
						num
					});
				}
			}
		}

		public const string UMReportConfigName = "UM.UMReportAggregatedData";

		public const int MaxItemsToAggregate = 500000;

		public const int MaxBatchSizeToReadCDR = 5000;

		private static readonly TimeSpan TimeDelta = TimeSpan.FromMinutes(15.0);

		private static readonly SortBy[] CreationTimeSortBy = new SortBy[]
		{
			new SortBy(StoreObjectSchema.CreationTime, SortOrder.Ascending)
		};

		private static readonly SortBy[] CallStartTimeSortByDescending = new SortBy[]
		{
			new SortBy(MessageItemSchema.XCDRDataCallStartTime, SortOrder.Descending)
		};

		private static readonly Trace Tracer = ExTraceGlobals.UMReportsTracer;

		public static StorePropertyDefinition[] ColumnsToPreLoad = new StorePropertyDefinition[]
		{
			MessageItemSchema.XCDRDataCallStartTime,
			MessageItemSchema.XCDRDataCallType,
			MessageItemSchema.XCDRDataCallIdentity,
			MessageItemSchema.XCDRDataParentCallIdentity,
			MessageItemSchema.XCDRDataUMServerName,
			MessageItemSchema.XCDRDataDialPlanGuid,
			MessageItemSchema.XCDRDataDialPlanName,
			MessageItemSchema.XCDRDataCallDuration,
			MessageItemSchema.XCDRDataIPGatewayAddress,
			MessageItemSchema.XCDRDataIPGatewayName,
			MessageItemSchema.XCDRDataGatewayGuid,
			MessageItemSchema.XCDRDataCalledPhoneNumber,
			MessageItemSchema.XCDRDataCallerPhoneNumber,
			MessageItemSchema.XCDRDataOfferResult,
			MessageItemSchema.XCDRDataDropCallReason,
			MessageItemSchema.XCDRDataReasonForCall,
			MessageItemSchema.XCDRDataTransferredNumber,
			MessageItemSchema.XCDRDataDialedString,
			MessageItemSchema.XCDRDataCallerMailboxAlias,
			MessageItemSchema.XCDRDataCalleeMailboxAlias,
			MessageItemSchema.XCDRDataAutoAttendantName,
			MessageItemSchema.XCDRDataAudioCodec,
			MessageItemSchema.XCDRDataBurstDensity,
			MessageItemSchema.XCDRDataBurstDuration,
			MessageItemSchema.XCDRDataJitter,
			MessageItemSchema.XCDRDataNMOS,
			MessageItemSchema.XCDRDataNMOSDegradation,
			MessageItemSchema.XCDRDataNMOSDegradationJitter,
			MessageItemSchema.XCDRDataNMOSDegradationPacketLoss,
			MessageItemSchema.XCDRDataPacketLoss,
			MessageItemSchema.XCDRDataRoundTrip,
			StoreObjectSchema.CreationTime,
			StoreObjectSchema.ItemClass
		};

		private static readonly PropertyDefinition[] UserPropertiesToSearch = new PropertyDefinition[]
		{
			MessageItemSchema.SenderDisplayName,
			ItemSchema.DisplayTo
		};

		public delegate void ProcessCDR(CDRData cdr);
	}
}
