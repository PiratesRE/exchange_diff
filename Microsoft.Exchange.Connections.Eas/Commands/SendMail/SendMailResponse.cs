using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.ComposeMail;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.SendMail
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "ComposeMail", TypeName = "SendMailResponse")]
	[XmlRoot(Namespace = "ComposeMail", ElementName = "SendMail")]
	public class SendMailResponse : SendMail, IEasServerResponse<SendMailStatus>, IHaveAnHttpStatus
	{
		HttpStatus IHaveAnHttpStatus.HttpStatus { get; set; }

		bool IEasServerResponse<SendMailStatus>.IsSucceeded(SendMailStatus status)
		{
			return SendMailStatus.Success == status;
		}

		SendMailStatus IEasServerResponse<SendMailStatus>.ConvertStatusToEnum()
		{
			byte status = base.Status;
			if (!SendMailResponse.StatusToEnumMap.ContainsKey(status))
			{
				return (SendMailStatus)status;
			}
			return SendMailResponse.StatusToEnumMap[status];
		}

		private static readonly IReadOnlyDictionary<byte, SendMailStatus> StatusToEnumMap = new Dictionary<byte, SendMailStatus>
		{
			{
				0,
				SendMailStatus.Success
			},
			{
				107,
				SendMailStatus.InvalidMIME
			},
			{
				115,
				SendMailStatus.SendQuotaExceeded
			},
			{
				118,
				SendMailStatus.MessagePreviouslySent
			},
			{
				119,
				SendMailStatus.MessageHasNoRecipient
			},
			{
				120,
				SendMailStatus.MailSubmissionFailed
			}
		};
	}
}
