using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderDelete
{
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderDelete")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderDeleteResponse")]
	public class FolderDeleteResponse : FolderDelete, IEasServerResponse<FolderDeleteStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<FolderDeleteStatus>.IsSucceeded(FolderDeleteStatus status)
		{
			return FolderDeleteStatus.Success == status;
		}

		FolderDeleteStatus IEasServerResponse<FolderDeleteStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!FolderDeleteResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (FolderDeleteStatus)status;
			}
			return FolderDeleteResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, FolderDeleteStatus> StatusToEnumMap = new Dictionary<byte, FolderDeleteStatus>
		{
			{
				1,
				FolderDeleteStatus.Success
			},
			{
				3,
				FolderDeleteStatus.SystemFolder
			},
			{
				4,
				FolderDeleteStatus.FolderNotFound
			},
			{
				6,
				FolderDeleteStatus.ServerError
			},
			{
				9,
				FolderDeleteStatus.SyncKeyMismatchOrInvalid
			},
			{
				10,
				FolderDeleteStatus.IncorrectRequestFormat
			},
			{
				11,
				FolderDeleteStatus.UnknownError
			},
			{
				12,
				FolderDeleteStatus.CodeUnknown
			}
		};
	}
}
