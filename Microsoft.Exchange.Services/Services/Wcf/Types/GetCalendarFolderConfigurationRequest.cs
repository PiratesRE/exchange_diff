using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCalendarFolderConfigurationRequest
	{
		[DataMember]
		public BaseFolderId FolderId { get; set; }

		internal void ValidateRequest()
		{
			if (this.FolderId == null)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
		}
	}
}
