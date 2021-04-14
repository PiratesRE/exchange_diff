using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public static class StoreUsageStatisticsResult
	{
		internal static List<StoreUsageStatisticsData> GetStoreUsageStatisticsData(Guid databaseGuid, string digestCategory)
		{
			if (databaseGuid == Guid.Empty)
			{
				throw new ArgumentException("Guid of the database to be validated cannot be empty", "DatabaseGuid");
			}
			List<StoreUsageStatisticsData> result;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=StoreActiveMonitoring", Environment.MachineName, null, null, null))
			{
				PropValue[][] resourceMonitorDigest = exRpcAdmin.GetResourceMonitorDigest(databaseGuid, StoreUsageStatisticsResult.propsToGet);
				List<StoreUsageStatisticsData> list = StoreUsageStatisticsResult.PopulateSUSDataList(resourceMonitorDigest, digestCategory);
				result = list;
			}
			return result;
		}

		internal static string SaveStoreUsageStatisticsData(List<StoreUsageStatisticsData> storeUsageStatisticsData, string serverName, string databaseName, string isActiveString)
		{
			if (storeUsageStatisticsData.Count == 0)
			{
				return null;
			}
			string storeUsageStatisticsFilePath = StoreMonitoringHelpers.GetStoreUsageStatisticsFilePath();
			if (!string.IsNullOrEmpty(storeUsageStatisticsFilePath))
			{
				using (FileStream fileStream = File.Create(storeUsageStatisticsFilePath))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, new ASCIIEncoding()))
					{
						streamWriter.Write("ServerName,DatabaseName,IsDatabaseCopyActive,DigestCategory,SampleId,SampleTime,MailboxGuid,TimeInServer,RopCount,LogRecordBytes,LogRecordCount,TimeInCPU,PageRead,PagePreread,LdapReads,LdapSearches,IsMailboxQuarantined");
						foreach (StoreUsageStatisticsData storeUsageStatisticsData2 in storeUsageStatisticsData)
						{
							string value = string.Format("\r\n{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", new object[]
							{
								serverName,
								databaseName,
								isActiveString,
								storeUsageStatisticsData2.DigestCategory,
								storeUsageStatisticsData2.SampleId,
								storeUsageStatisticsData2.SampleTime,
								storeUsageStatisticsData2.MailboxGuid,
								storeUsageStatisticsData2.TimeInServer,
								storeUsageStatisticsData2.RopCount,
								storeUsageStatisticsData2.LogRecordBytes,
								storeUsageStatisticsData2.LogRecordCount,
								storeUsageStatisticsData2.TimeInCPU,
								storeUsageStatisticsData2.PageRead,
								storeUsageStatisticsData2.PagePreread,
								storeUsageStatisticsData2.LdapReads,
								storeUsageStatisticsData2.LdapSearches,
								storeUsageStatisticsData2.IsMailboxQuarantined
							});
							streamWriter.Write(value);
						}
						return storeUsageStatisticsFilePath;
					}
				}
			}
			return null;
		}

		internal static List<StoreUsageStatisticsData> PopulateSUSDataList(PropValue[][] entries, string digestCategory)
		{
			List<StoreUsageStatisticsData> list = new List<StoreUsageStatisticsData>(0);
			if (entries != null && entries.Length > 0)
			{
				list = new List<StoreUsageStatisticsData>(entries.Length);
				for (int i = 0; i < entries.Length; i++)
				{
					if (entries[i].Length == StoreUsageStatisticsResult.propsToGet.Length && StoreUsageStatisticsResult.ValidateValue(entries[i][0].Value, typeof(string)))
					{
						string text = entries[i][0].Value.ToString();
						if (!string.IsNullOrWhiteSpace(text) && string.Equals(text, digestCategory, StringComparison.InvariantCultureIgnoreCase) && StoreUsageStatisticsResult.ValidateValue(entries[i][3].Value, typeof(byte[])))
						{
							string mailboxGuid = new Guid((byte[])entries[i][3].Value).ToString();
							if (StoreUsageStatisticsResult.ValidateValue(entries[i][1].Value, typeof(int)))
							{
								int sampleId = (int)entries[i][1].Value;
								if (StoreUsageStatisticsResult.ValidateValue(entries[i][2].Value, typeof(DateTime)))
								{
									DateTime sampleTime = (DateTime)entries[i][2].Value;
									if (StoreUsageStatisticsResult.ValidateValue(entries[i][4].Value, typeof(string)))
									{
										string displayName = entries[i][4].Value.ToString();
										if (StoreUsageStatisticsResult.ValidateValue(entries[i][5].Value, typeof(int)))
										{
											int timeInServer = (int)entries[i][5].Value;
											if (StoreUsageStatisticsResult.ValidateValue(entries[i][6].Value, typeof(int)))
											{
												int ropCount = (int)entries[i][6].Value;
												if (StoreUsageStatisticsResult.ValidateValue(entries[i][7].Value, typeof(int)))
												{
													int logRecordBytes = (int)entries[i][7].Value;
													if (StoreUsageStatisticsResult.ValidateValue(entries[i][8].Value, typeof(int)))
													{
														int logRecordCount = (int)entries[i][8].Value;
														if (StoreUsageStatisticsResult.ValidateValue(entries[i][9].Value, typeof(int)))
														{
															int timeInCpu = (int)entries[i][9].Value;
															if (StoreUsageStatisticsResult.ValidateValue(entries[i][10].Value, typeof(int)))
															{
																int pageRead = (int)entries[i][10].Value;
																if (StoreUsageStatisticsResult.ValidateValue(entries[i][11].Value, typeof(int)))
																{
																	int pagePreRead = (int)entries[i][11].Value;
																	if (StoreUsageStatisticsResult.ValidateValue(entries[i][12].Value, typeof(int)))
																	{
																		int ldapReads = (int)entries[i][12].Value;
																		if (StoreUsageStatisticsResult.ValidateValue(entries[i][13].Value, typeof(int)))
																		{
																			int ldapSearches = (int)entries[i][13].Value;
																			if (StoreUsageStatisticsResult.ValidateValue(entries[i][14].Value, typeof(bool)))
																			{
																				bool isMailboxQuarantined = (bool)entries[i][14].Value;
																				StoreUsageStatisticsData item = new StoreUsageStatisticsData(text, sampleId, sampleTime, mailboxGuid, displayName, timeInServer, ropCount, logRecordBytes, logRecordCount, timeInCpu, pageRead, pagePreRead, ldapReads, ldapSearches, isMailboxQuarantined);
																				list.Add(item);
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		private static bool ValidateValue(object value, Type expectedType)
		{
			if (value != null)
			{
				Type type = value.GetType();
				if (type == expectedType)
				{
					return true;
				}
			}
			return false;
		}

		internal const string TimeInServer = "TimeInServer";

		internal const string LogBytes = "LogBytes";

		private static PropTag[] propsToGet = new PropTag[]
		{
			PropTag.DigestCategory,
			PropTag.SampleId,
			PropTag.SampleTime,
			PropTag.UserGuid,
			PropTag.DisplayName,
			PropTag.TimeInServer,
			PropTag.ROPCount,
			PropTag.LogRecordBytes,
			PropTag.LogRecordCount,
			PropTag.TimeInCPU,
			PropTag.PageRead,
			PropTag.PagePreread,
			PropTag.LdapReads,
			PropTag.LdapSearches,
			PropTag.MailboxQuarantined
		};
	}
}
