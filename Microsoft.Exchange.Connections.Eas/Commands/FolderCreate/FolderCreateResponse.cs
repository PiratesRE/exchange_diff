using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.FolderHierarchy;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.FolderCreate
{
	[XmlRoot(Namespace = "FolderHierarchy", ElementName = "FolderCreate")]
	[XmlType(Namespace = "FolderHierarchy", TypeName = "FolderCreateResponse")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class FolderCreateResponse : FolderCreate, IEasServerResponse<FolderCreateStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<FolderCreateStatus>.IsSucceeded(FolderCreateStatus status)
		{
			return FolderCreateStatus.Success == status;
		}

		FolderCreateStatus IEasServerResponse<FolderCreateStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!FolderCreateResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (FolderCreateStatus)status;
			}
			return FolderCreateResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, FolderCreateStatus> StatusToEnumMap = new Dictionary<byte, FolderCreateStatus>
		{
			{
				1,
				FolderCreateStatus.Success
			},
			{
				2,
				FolderCreateStatus.FolderExists
			},
			{
				5,
				FolderCreateStatus.ParentFolderNotFound
			},
			{
				6,
				FolderCreateStatus.ServerError
			},
			{
				9,
				FolderCreateStatus.SyncKeyMismatchOrInvalid
			},
			{
				10,
				FolderCreateStatus.IncorrectRequestFormat
			},
			{
				11,
				FolderCreateStatus.UnknownError
			},
			{
				12,
				FolderCreateStatus.CodeUnknown
			}
		};
	}
}
