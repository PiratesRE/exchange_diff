using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal static class KnownStringsHelper
	{
		public static Dictionary<KnownStringType, string> KnownStringToSqlLookupParam
		{
			get
			{
				return new Dictionary<KnownStringType, string>
				{
					{
						KnownStringType.ServerName,
						"Server"
					},
					{
						KnownStringType.DatabaseName,
						"Database"
					},
					{
						KnownStringType.MailboxType,
						"MailboxType"
					},
					{
						KnownStringType.TenantName,
						"TenantName"
					},
					{
						KnownStringType.MigrationType,
						"MigrationType"
					},
					{
						KnownStringType.MigrationStatus,
						"MigrationStatus"
					},
					{
						KnownStringType.BadItemKind,
						"BadItemKind"
					},
					{
						KnownStringType.BadItemWkfTypeId,
						"WKFType"
					},
					{
						KnownStringType.BadItemMessageClass,
						"MessageClass"
					},
					{
						KnownStringType.BadItemCategory,
						"Category"
					},
					{
						KnownStringType.FailureType,
						"FailureType"
					},
					{
						KnownStringType.FailureSide,
						"FailureSide"
					},
					{
						KnownStringType.RequestType,
						"RequestType"
					},
					{
						KnownStringType.RequestStatus,
						"Status"
					},
					{
						KnownStringType.RequestStatusDetail,
						"StatusDetail"
					},
					{
						KnownStringType.RequestPriority,
						"Priority"
					},
					{
						KnownStringType.RequestBatchName,
						"Batch"
					},
					{
						KnownStringType.Version,
						"Version"
					},
					{
						KnownStringType.RemoteHostName,
						"RemoteHostName"
					},
					{
						KnownStringType.TargetDeliveryDomain,
						"TargetDeliveryDomain"
					},
					{
						KnownStringType.RequestSyncStage,
						"SyncStage"
					},
					{
						KnownStringType.RequestJobType,
						"JobType"
					},
					{
						KnownStringType.RequestWorkloadType,
						"WorkloadType"
					},
					{
						KnownStringType.MaxProviderDurationMethodName,
						"MaxProviderDurationMethodName"
					},
					{
						KnownStringType.OwnerResourceNameType,
						"OwnerResourceName"
					},
					{
						KnownStringType.OwnerResourceTypeType,
						"OwnerResourceType"
					},
					{
						KnownStringType.ResourceKeyType,
						"ResourceKey"
					},
					{
						KnownStringType.LoadStateType,
						"LoadState"
					},
					{
						KnownStringType.ReservationFailureReasonType,
						"ReservationFailureReason"
					},
					{
						KnownStringType.ReservationFailureResourceTypeType,
						"ReservationFailureResourceType"
					},
					{
						KnownStringType.ReservationFailureWLMResourceTypeType,
						"ReservationFailureWLMResourceType"
					},
					{
						KnownStringType.PickupResultsType,
						"PickupResult"
					},
					{
						KnownStringType.LastScanFailureFailureType,
						"LastScanFailureFailure"
					},
					{
						KnownStringType.DrumTestingTestType,
						"DrumTestingTest"
					},
					{
						KnownStringType.DrumTestingObjectType,
						"DrumTestingObject"
					},
					{
						KnownStringType.DrumTestingResultType,
						"DrumTestingResult"
					},
					{
						KnownStringType.DrumTestingResultCategoryType,
						"DrumTestingResultCategory"
					},
					{
						KnownStringType.SyncProtocol,
						"SyncProtocol"
					},
					{
						KnownStringType.MigrationDirection,
						"MigrationDirection"
					},
					{
						KnownStringType.Locale,
						"Locale"
					},
					{
						KnownStringType.MigrationSkipSteps,
						"MigrationSkipSteps"
					},
					{
						KnownStringType.MigrationBatchFlags,
						"MigrationBatchFlags"
					},
					{
						KnownStringType.EndpointGuid,
						"MigrationEndpointGuid"
					},
					{
						KnownStringType.EndpointState,
						"MigrationEndpointState"
					},
					{
						KnownStringType.EndpointPermission,
						"MigrationEndpointPermission"
					},
					{
						KnownStringType.WatsonHash,
						"WatsonHash"
					},
					{
						KnownStringType.DisconnectReason,
						"DisconnectReason"
					},
					{
						KnownStringType.AppVersion,
						"AppVersion"
					}
				};
			}
		}

		public static string ConvertStringValueByType(KnownStringType type, string value)
		{
			string result;
			if (type == KnownStringType.Version)
			{
				result = KnownStringsHelper.VersionIntToString(value);
			}
			else
			{
				result = value;
			}
			return result;
		}

		private static string VersionIntToString(string versionIntString)
		{
			int versionNumber;
			if (int.TryParse(versionIntString, out versionNumber))
			{
				ServerVersion serverVersion = new ServerVersion(versionNumber);
				return string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					serverVersion.Major,
					serverVersion.Minor,
					serverVersion.Build,
					serverVersion.Revision
				});
			}
			return string.Empty;
		}

		public static readonly List<KnownStringType> SpecialKnownStrings = new List<KnownStringType>
		{
			KnownStringType.WatsonHash,
			KnownStringType.TenantName,
			KnownStringType.EndpointGuid
		};
	}
}
