using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DlpPolicyMatchDetail
	{
		[DataMember]
		public DlpPolicyTipAction Action { get; set; }

		[DataMember]
		public AttachmentIdType[] AttachmentIds { get; set; }

		[DataMember]
		public EmailAddressWrapper[] Recipients { get; set; }

		[DataMember]
		public string[] Classifications { get; set; }

		public override string ToString()
		{
			string format = "Action:{0}/AttachmentIds:{1}/Recipients:{2}/Classifications:{3}.";
			object[] array = new object[4];
			array[0] = this.Action.ToString();
			object[] array2 = array;
			int num = 1;
			string text;
			if (this.AttachmentIds != null)
			{
				text = string.Join(";", from attachId in this.AttachmentIds
				select attachId.Id);
			}
			else
			{
				text = string.Empty;
			}
			array2[num] = text;
			object[] array3 = array;
			int num2 = 2;
			string text2;
			if (this.Recipients != null)
			{
				text2 = string.Join(";", from emailAddressWrapper in this.Recipients
				select PolicyTipRequestLogger.MarkAsPII(emailAddressWrapper.EmailAddress));
			}
			else
			{
				text2 = string.Empty;
			}
			array3[num2] = text2;
			array[3] = ((this.Classifications == null) ? string.Empty : string.Join(";", this.Classifications));
			return string.Format(format, array);
		}

		public static string ToString(List<DlpPolicyMatchDetail> dlpPolicyMatchDetails)
		{
			if (dlpPolicyMatchDetails == null || !dlpPolicyMatchDetails.Any<DlpPolicyMatchDetail>())
			{
				return string.Empty;
			}
			return string.Join(";", from dlpPolicyMatchDetail in dlpPolicyMatchDetails
			select dlpPolicyMatchDetail.ToString());
		}

		private const string ToStringFormatString = "Action:{0}/AttachmentIds:{1}/Recipients:{2}/Classifications:{3}.";
	}
}
