using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.Move;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.MoveItems
{
	[XmlType(Namespace = "Move", TypeName = "MoveItemsResponse")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(Namespace = "Move", ElementName = "MoveItems")]
	public class MoveItemsResponse : MoveItems, IEasServerResponse<MoveItemsStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<MoveItemsStatus>.IsSucceeded(MoveItemsStatus status)
		{
			return MoveItemsStatus.Success == status;
		}

		MoveItemsStatus IEasServerResponse<MoveItemsStatus>.ConvertStatusToEnum()
		{
			if (base.Responses.Length > 1)
			{
				IEnumerable<byte> source = from c in base.Responses
				where c.Status != 3
				select c.Status;
				if (source.Count<byte>() == 0)
				{
					return MoveItemsStatus.Success;
				}
				return MoveItemsStatus.CompositeStatusError;
			}
			else
			{
				byte status = base.Responses[0].Status;
				if (!MoveItemsResponse.StatusToEnumMap.ContainsKey(status))
				{
					return (MoveItemsStatus)status;
				}
				return MoveItemsResponse.StatusToEnumMap[status];
			}
		}

		private static readonly IReadOnlyDictionary<byte, MoveItemsStatus> StatusToEnumMap = new Dictionary<byte, MoveItemsStatus>
		{
			{
				1,
				MoveItemsStatus.InvalidSourceId
			},
			{
				2,
				MoveItemsStatus.InvalidDestinationId
			},
			{
				3,
				MoveItemsStatus.Success
			},
			{
				4,
				MoveItemsStatus.SourceDestinationIdentical
			},
			{
				5,
				MoveItemsStatus.CannotMove
			},
			{
				7,
				MoveItemsStatus.Retry
			}
		};
	}
}
