using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.GetItemEstimate;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.GetItemEstimate
{
	[XmlRoot(Namespace = "GetItemEstimate", ElementName = "GetItemEstimate")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "GetItemEstimate", TypeName = "GetItemEstimateResponse")]
	public class GetItemEstimateResponse : GetItemEstimate, IEasServerResponse<GetItemEstimateStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		internal int? Estimate
		{
			get
			{
				if (base.Response == null || base.Response.Collection == null)
				{
					return null;
				}
				return new int?(base.Response.Collection.Estimate);
			}
		}

		bool IEasServerResponse<GetItemEstimateStatus>.IsSucceeded(GetItemEstimateStatus status)
		{
			return GetItemEstimateStatus.Success == status;
		}

		GetItemEstimateStatus IEasServerResponse<GetItemEstimateStatus>.ConvertStatusToEnum()
		{
			byte status = base.Response.Status;
			if (!GetItemEstimateResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (GetItemEstimateStatus)status;
			}
			return GetItemEstimateResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, GetItemEstimateStatus> StatusToEnumMap = new Dictionary<byte, GetItemEstimateStatus>
		{
			{
				1,
				GetItemEstimateStatus.Success
			},
			{
				2,
				GetItemEstimateStatus.InvalidCollection
			},
			{
				3,
				GetItemEstimateStatus.SyncNotPrimed
			},
			{
				4,
				GetItemEstimateStatus.InvalidSyncKey
			}
		};
	}
}
