using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderUpdate
{
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderUpdateResponse")]
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderUpdate")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderUpdateResponse : FolderUpdate, IEasServerResponse<FolderUpdateStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<FolderUpdateStatus>.IsSucceeded(FolderUpdateStatus status)
		{
			return FolderUpdateStatus.Success == status;
		}

		FolderUpdateStatus IEasServerResponse<FolderUpdateStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!FolderUpdateResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (FolderUpdateStatus)status;
			}
			return FolderUpdateResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, FolderUpdateStatus> StatusToEnumMap = new Dictionary<byte, FolderUpdateStatus>
		{
			{
				1,
				FolderUpdateStatus.Success
			},
			{
				2,
				FolderUpdateStatus.FolderExists
			},
			{
				4,
				FolderUpdateStatus.FolderNotFound
			},
			{
				5,
				FolderUpdateStatus.ParentFolderNotFound
			},
			{
				6,
				FolderUpdateStatus.ServerError
			},
			{
				9,
				FolderUpdateStatus.SyncKeyMismatchOrInvalid
			},
			{
				10,
				FolderUpdateStatus.IncorrectRequestFormat
			},
			{
				11,
				FolderUpdateStatus.UnknownError
			},
			{
				12,
				FolderUpdateStatus.CodeUnknown
			}
		};
	}
}
