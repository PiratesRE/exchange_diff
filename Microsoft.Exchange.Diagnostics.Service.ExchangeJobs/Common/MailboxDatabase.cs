using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using Microsoft.ExLogAnalyzer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Common
{
	internal sealed class MailboxDatabase : IEquatable<MailboxDatabase>
	{
		public MailboxDatabase(string directoryPath)
		{
			if (string.IsNullOrEmpty(directoryPath))
			{
				throw new ArgumentNullException("directoryPath");
			}
			using (DirectoryEntry directoryEntry = new DirectoryEntry(directoryPath))
			{
				byte[] b = MailboxDatabase.ValidateDirectoryProperty<byte[]>(directoryEntry, "ObjectGuid");
				string text = MailboxDatabase.ValidateDirectoryProperty<string>(directoryEntry, "Name");
				string path = MailboxDatabase.ValidateDirectoryProperty<string>(directoryEntry, "msExchEDBFile");
				string text2 = MailboxDatabase.ValidateDirectoryProperty<string>(directoryEntry, "msExchESEParamLogFilePath");
				string text3 = MailboxDatabase.ValidateDirectoryProperty<string>(directoryEntry, "msExchESEParamBaseName");
				string text4 = MailboxDatabase.ValidateDirectoryProperty<string>(directoryEntry, "msExchDatabaseGroup");
				this.guid = new Guid(b);
				this.name = text;
				this.edbFolderPath = Path.GetDirectoryName(path);
				this.logFolderPath = text2;
				this.logFilePrefix = text3;
				this.databaseGroup = text4;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		internal string DatabaseGroup
		{
			get
			{
				return this.databaseGroup;
			}
		}

		internal string EdbFolderPath
		{
			get
			{
				return this.edbFolderPath;
			}
		}

		internal string LogFolderPath
		{
			get
			{
				return this.logFolderPath;
			}
		}

		internal string LogFilePrefix
		{
			get
			{
				return this.logFilePrefix;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailboxDatabase);
		}

		public bool Equals(MailboxDatabase other)
		{
			return other != null && this.name.Equals(other.name, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.name);
		}

		public bool TryLoadStatistics(out Dictionary<string, float> statistics)
		{
			bool result = false;
			statistics = null;
			try
			{
				ulong num = 0UL;
				ulong num2 = 0UL;
				int[] array = new int[2];
				int[] array2 = array;
				long[] array3 = new long[2];
				long[] array4 = array3;
				long[] array5 = new long[2];
				long[] array6 = array5;
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=EDS", Environment.MachineName, null, null, null))
				{
					exRpcAdmin.GetDatabaseSize(this.guid, out num, out num2);
					PropValue[][] mailboxTable = exRpcAdmin.GetMailboxTable(this.guid, MailboxDatabase.MailboxTableProperties);
					foreach (PropValue[] source in mailboxTable)
					{
						Dictionary<PropTag, PropValue> dictionary = (from value in source
						where value.Value != null
						select value).ToDictionary((PropValue value) => value.PropTag);
						PropValue propValue;
						int num3 = dictionary.TryGetValue(PropTag.DateDiscoveredAbsentInDS, out propValue) ? 1 : 0;
						if (dictionary.TryGetValue(PropTag.MessageSizeExtended, out propValue))
						{
							array4[num3] += (long)propValue.Value;
						}
						if (dictionary.TryGetValue(PropTag.DeletedMessageSizeExtended, out propValue))
						{
							array6[num3] += (long)propValue.Value;
						}
						array2[num3]++;
					}
				}
				statistics = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
				statistics.Add("AvailableNewMailboxSpace", num2);
				statistics.Add("DisconnectedTotalItemSize", (float)array4[1]);
				statistics.Add("DisconnectedTotalDeletedItemSize", (float)array6[1]);
				statistics.Add("MailboxCount", (float)array2[0]);
				statistics.Add("DisconnectedMailboxCount", (float)array2[1]);
				statistics.Add("CatalogSize", this.GetCatalogSize());
				statistics.Add("LogSize", this.GetLogFileSize());
				long num4 = (long)(num - num2);
				long num5 = array4[0] + array6[0] + array4[1] + array6[1];
				statistics.Add("DatabasePhysicalUsedSize", (float)num4);
				statistics.Add("DatabaseLogicalSize", (float)num5);
				if (num4 > 0L)
				{
					statistics.Add("LogicalPhysicalSizeRatio", (float)((double)num5 / (double)num4));
				}
				else
				{
					statistics.Add("LogicalPhysicalSizeRatio", 0f);
				}
				result = true;
			}
			catch (MapiExceptionMdbOffline)
			{
			}
			catch (Exception ex)
			{
				Log.LogErrorMessage("An exception occurred while trying to gather database '{0}' statistics: '{1}'", new object[]
				{
					this.name,
					ex
				});
			}
			return result;
		}

		internal static bool TryDiscoverLocalMailboxDatabases(out IEnumerable<MailboxDatabase> databases)
		{
			bool result = false;
			databases = null;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://RootDSE"))
				{
					using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://CN=Microsoft Exchange,CN=Services," + directoryEntry.Properties["configurationNamingContext"].Value.ToString()))
					{
						using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry2))
						{
							directorySearcher.CacheResults = false;
							directorySearcher.Filter = string.Format("(&(ObjectClass=msExchExchangeServer)(cn={0}))", Environment.MachineName);
							directorySearcher.PropertiesToLoad.Add("msExchHostServerBL");
							using (SearchResultCollection searchResultCollection = directorySearcher.FindAll())
							{
								List<MailboxDatabase> list = new List<MailboxDatabase>();
								foreach (object obj in searchResultCollection)
								{
									SearchResult searchResult = (SearchResult)obj;
									ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["msExchHostServerBL"];
									using (IEnumerator enumerator2 = resultPropertyValueCollection.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											object obj2 = enumerator2.Current;
											string str = MailboxDatabase.NavigateUpDistinguishedName(obj2.ToString());
											MailboxDatabase item = new MailboxDatabase("LDAP://" + str);
											list.Add(item);
										}
										break;
									}
								}
								if (list.Any<MailboxDatabase>())
								{
									databases = list;
									result = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.LogErrorMessage("An exception occurred while trying to query AD: '{0}'", new object[]
				{
					ex
				});
			}
			return result;
		}

		internal static string NavigateUpDistinguishedName(string distinguishedName)
		{
			if (string.IsNullOrEmpty(distinguishedName))
			{
				throw new ArgumentNullException("distinguishedName");
			}
			string result = string.Empty;
			for (int i = 0; i < distinguishedName.Length; i++)
			{
				if (distinguishedName[i] == ',' && distinguishedName[(i - 1 < 0) ? 0 : (i - 1)] != '\\')
				{
					result = distinguishedName.Substring(i + 1);
					break;
				}
			}
			return result;
		}

		internal float GetLogFileSize()
		{
			float num = 0f;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(this.logFolderPath);
				FileInfo[] files;
				if (!string.IsNullOrEmpty(this.logFilePrefix))
				{
					files = directoryInfo.GetFiles(this.logFilePrefix + "*");
				}
				else
				{
					files = directoryInfo.GetFiles();
				}
				foreach (FileInfo fileInfo in files)
				{
					num += (float)fileInfo.Length;
				}
			}
			catch (Exception ex)
			{
				Log.LogErrorMessage("Unable to calculate the log size. LogFolderPath: '{0}', LogFilePrefix: '{1}', Exception: '{2}'", new object[]
				{
					this.logFolderPath,
					this.logFilePrefix,
					ex
				});
			}
			return num;
		}

		internal float GetCatalogSize()
		{
			float num = 0f;
			try
			{
				string[] directories = Directory.GetDirectories(this.edbFolderPath, string.Format("{0}*.Single", this.guid));
				foreach (string path in directories)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(path);
					if (directoryInfo.Exists)
					{
						FileInfo[] files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
						foreach (FileInfo fileInfo in files)
						{
							num += (float)fileInfo.Length;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.LogErrorMessage("Unable to calculate the catalog size. EdbFolderPath: '{0}', Exception: '{1}'", new object[]
				{
					this.edbFolderPath,
					ex
				});
			}
			return num;
		}

		private static TValue ValidateDirectoryProperty<TValue>(DirectoryEntry entry, string propertyName) where TValue : class
		{
			if (!entry.Properties.Contains(propertyName) && !propertyName.Equals("msExchDatabaseGroup"))
			{
				throw new InvalidOperationException(string.Format("The directory object with the path '{0}', does not contain the required '{1}' property.", entry.Path, propertyName));
			}
			if (!entry.Properties.Contains(propertyName))
			{
				return default(TValue);
			}
			TValue tvalue = entry.Properties[propertyName].Value as TValue;
			if (tvalue == null)
			{
				throw new InvalidOperationException(string.Format("The value of the '{0}' property cannot be null.", propertyName));
			}
			return tvalue;
		}

		public const string DatabasePhysicalUsedSizeProperty = "DatabasePhysicalUsedSize";

		public const string AvailableNewMailboxSpaceProperty = "AvailableNewMailboxSpace";

		public const string DatabaseLogicalSizeProperty = "DatabaseLogicalSize";

		public const string LogicalPhysicalSizeRatioProperty = "LogicalPhysicalSizeRatio";

		public const string DisconnectedTotalItemSizeProperty = "DisconnectedTotalItemSize";

		public const string DisconnectedTotalDeletedItemSizeProperty = "DisconnectedTotalDeletedItemSize";

		public const string MailboxCountProperty = "MailboxCount";

		public const string DisconnectedMailboxCountProperty = "DisconnectedMailboxCount";

		public const string CatalogSizeProperty = "CatalogSize";

		public const string LogSizeProperty = "LogSize";

		private const string DirectoryMailboxDatabaseGroupProperty = "msExchDatabaseGroup";

		private static readonly PropTag[] MailboxTableProperties = new PropTag[]
		{
			PropTag.MessageSizeExtended,
			PropTag.DeletedMessageSizeExtended,
			PropTag.DateDiscoveredAbsentInDS
		};

		private readonly string name;

		private readonly Guid guid;

		private readonly string databaseGroup;

		private readonly string edbFolderPath;

		private readonly string logFolderPath;

		private readonly string logFilePrefix;
	}
}
