using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.Settings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Settings
{
	[XmlRoot(Namespace = "Settings", ElementName = "Settings")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Settings", TypeName = "SettingsResponse")]
	public class SettingsResponse : Settings, IEasServerResponse<SettingsStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<SettingsStatus>.IsSucceeded(SettingsStatus status)
		{
			return SettingsStatus.Success == status;
		}

		SettingsStatus IEasServerResponse<SettingsStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!SettingsResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (SettingsStatus)status;
			}
			return SettingsResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, SettingsStatus> StatusToEnumMap = new Dictionary<byte, SettingsStatus>
		{
			{
				1,
				SettingsStatus.Success
			},
			{
				2,
				SettingsStatus.ProtocolError
			},
			{
				3,
				SettingsStatus.AccessDenied
			},
			{
				4,
				SettingsStatus.ServerUnavailable
			},
			{
				5,
				SettingsStatus.InvalidArguments
			},
			{
				6,
				SettingsStatus.ConflictingArguments
			},
			{
				7,
				SettingsStatus.DeniedByPolicy
			}
		};
	}
}
