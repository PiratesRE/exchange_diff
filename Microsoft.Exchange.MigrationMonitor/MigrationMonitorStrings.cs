using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MigrationMonitorStrings
	{
		static MigrationMonitorStrings()
		{
			MigrationMonitorStrings.stringIDs.Add(1815070528U, "ErrorLookUpServerId");
			MigrationMonitorStrings.stringIDs.Add(2311404879U, "ErrorUploadDatabaseInfoInBatch");
			MigrationMonitorStrings.stringIDs.Add(2787859741U, "ErrorUploadMrsAvailabilityLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(2842050225U, "ErrorUploadJobPickupResultsLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(779663980U, "ErrorUploadMrsLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(3175692275U, "ErrorUploadMigrationJobDataInBatch");
			MigrationMonitorStrings.stringIDs.Add(1616066425U, "ErrorUploadWLMResourceStatsLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(1288218702U, "ErrorUploadMigrationJobItemDataInBatch");
			MigrationMonitorStrings.stringIDs.Add(3126953968U, "ErrorUploadQueueStatsLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(2399146322U, "ErrorLookUpStringId");
			MigrationMonitorStrings.stringIDs.Add(2743222918U, "ErrorUploadMrsDrumTestingLogInBatch");
			MigrationMonitorStrings.stringIDs.Add(648032374U, "ErrorLookUpEndpointId");
			MigrationMonitorStrings.stringIDs.Add(774821795U, "ErrorLookUpWatsonId");
			MigrationMonitorStrings.stringIDs.Add(2500238689U, "ErrorUploadMailboxStatsInBatch");
			MigrationMonitorStrings.stringIDs.Add(2952266827U, "ErrorUploadMigrationEndpointDataInBatch");
			MigrationMonitorStrings.stringIDs.Add(4180397884U, "ErrorHealthStatusPublishFailureException");
			MigrationMonitorStrings.stringIDs.Add(2876186303U, "ErrorLookUpTenantId");
		}

		public static LocalizedString ErrorLookUpServerId
		{
			get
			{
				return new LocalizedString("ErrorLookUpServerId", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadDatabaseInfoInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadDatabaseInfoInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLogFileRead(string fileName)
		{
			return new LocalizedString("ErrorLogFileRead", MigrationMonitorStrings.ResourceManager, new object[]
			{
				fileName
			});
		}

		public static LocalizedString ErrorUploadMrsAvailabilityLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMrsAvailabilityLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadJobPickupResultsLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadJobPickupResultsLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSqlServerUnreachableException(string connectionString)
		{
			return new LocalizedString("ErrorSqlServerUnreachableException", MigrationMonitorStrings.ResourceManager, new object[]
			{
				connectionString
			});
		}

		public static LocalizedString ErrorDirectoryNotExist(string dirName)
		{
			return new LocalizedString("ErrorDirectoryNotExist", MigrationMonitorStrings.ResourceManager, new object[]
			{
				dirName
			});
		}

		public static LocalizedString ErrorUploadMrsLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMrsLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnexpectedNullFromSproc(string msg)
		{
			return new LocalizedString("ErrorUnexpectedNullFromSproc", MigrationMonitorStrings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString ErrorUploadMigrationJobDataInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMigrationJobDataInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadWLMResourceStatsLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadWLMResourceStatsLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadMigrationJobItemDataInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMigrationJobItemDataInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadQueueStatsLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadQueueStatsLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLogFileLoad(string dirName)
		{
			return new LocalizedString("ErrorLogFileLoad", MigrationMonitorStrings.ResourceManager, new object[]
			{
				dirName
			});
		}

		public static LocalizedString ErrorSqlConnectionString(string connection)
		{
			return new LocalizedString("ErrorSqlConnectionString", MigrationMonitorStrings.ResourceManager, new object[]
			{
				connection
			});
		}

		public static LocalizedString ErrorLookUpStringId
		{
			get
			{
				return new LocalizedString("ErrorLookUpStringId", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadMrsDrumTestingLogInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMrsDrumTestingLogInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLookUpEndpointId
		{
			get
			{
				return new LocalizedString("ErrorLookUpEndpointId", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSqlServerTimeout(string sprocName)
		{
			return new LocalizedString("ErrorSqlServerTimeout", MigrationMonitorStrings.ResourceManager, new object[]
			{
				sprocName
			});
		}

		public static LocalizedString ErrorSqlQueryFailed(string sprocName)
		{
			return new LocalizedString("ErrorSqlQueryFailed", MigrationMonitorStrings.ResourceManager, new object[]
			{
				sprocName
			});
		}

		public static LocalizedString ErrorLookUpWatsonId
		{
			get
			{
				return new LocalizedString("ErrorLookUpWatsonId", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadMailboxStatsInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMailboxStatsInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUploadMigrationEndpointDataInBatch
		{
			get
			{
				return new LocalizedString("ErrorUploadMigrationEndpointDataInBatch", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHealthStatusPublishFailureException
		{
			get
			{
				return new LocalizedString("ErrorHealthStatusPublishFailureException", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLookUpTenantId
		{
			get
			{
				return new LocalizedString("ErrorLookUpTenantId", MigrationMonitorStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(MigrationMonitorStrings.IDs key)
		{
			return new LocalizedString(MigrationMonitorStrings.stringIDs[(uint)key], MigrationMonitorStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(17);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Servicelets.MigrationMonitor.Strings", typeof(MigrationMonitorStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorLookUpServerId = 1815070528U,
			ErrorUploadDatabaseInfoInBatch = 2311404879U,
			ErrorUploadMrsAvailabilityLogInBatch = 2787859741U,
			ErrorUploadJobPickupResultsLogInBatch = 2842050225U,
			ErrorUploadMrsLogInBatch = 779663980U,
			ErrorUploadMigrationJobDataInBatch = 3175692275U,
			ErrorUploadWLMResourceStatsLogInBatch = 1616066425U,
			ErrorUploadMigrationJobItemDataInBatch = 1288218702U,
			ErrorUploadQueueStatsLogInBatch = 3126953968U,
			ErrorLookUpStringId = 2399146322U,
			ErrorUploadMrsDrumTestingLogInBatch = 2743222918U,
			ErrorLookUpEndpointId = 648032374U,
			ErrorLookUpWatsonId = 774821795U,
			ErrorUploadMailboxStatsInBatch = 2500238689U,
			ErrorUploadMigrationEndpointDataInBatch = 2952266827U,
			ErrorHealthStatusPublishFailureException = 4180397884U,
			ErrorLookUpTenantId = 2876186303U
		}

		private enum ParamIDs
		{
			ErrorLogFileRead,
			ErrorSqlServerUnreachableException,
			ErrorDirectoryNotExist,
			ErrorUnexpectedNullFromSproc,
			ErrorLogFileLoad,
			ErrorSqlConnectionString,
			ErrorSqlServerTimeout,
			ErrorSqlQueryFailed
		}
	}
}
