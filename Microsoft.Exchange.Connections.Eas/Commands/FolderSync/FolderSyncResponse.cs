using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderSync
{
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderSync")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderSyncResponse")]
	public class FolderSyncResponse : FolderSync, IEasServerResponse<FolderSyncStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<FolderSyncStatus>.IsSucceeded(FolderSyncStatus status)
		{
			return FolderSyncStatus.Success == status;
		}

		FolderSyncStatus IEasServerResponse<FolderSyncStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!FolderSyncResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (FolderSyncStatus)status;
			}
			return FolderSyncResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, FolderSyncStatus> StatusToEnumMap = new Dictionary<byte, FolderSyncStatus>
		{
			{
				1,
				FolderSyncStatus.Success
			},
			{
				6,
				FolderSyncStatus.ServerError
			},
			{
				9,
				FolderSyncStatus.SyncKeyMismatchOrInvalid
			},
			{
				10,
				FolderSyncStatus.IncorrectRequestFormat
			},
			{
				11,
				FolderSyncStatus.UnknownError
			},
			{
				12,
				FolderSyncStatus.CodeUnknown
			},
			{
				110,
				FolderSyncStatus.ServerBusy
			}
		};
	}
}
