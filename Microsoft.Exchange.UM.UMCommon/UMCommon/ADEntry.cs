using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ADEntry
	{
		public ADEntry(List<string> names, string smtpAddress, Guid objectGuid, RecipientType recipientType, Guid dialPlanGuid, List<Guid> addressListGuids)
		{
			this.Names = names;
			this.SmtpAddress = smtpAddress;
			this.ObjectGuid = objectGuid;
			this.RecipientType = recipientType;
			this.DialPlanGuid = dialPlanGuid;
			this.AddressListGuids = addressListGuids;
		}

		public List<string> Names { get; private set; }

		public string SmtpAddress { get; private set; }

		public Guid ObjectGuid { get; private set; }

		public RecipientType RecipientType { get; private set; }

		public Guid DialPlanGuid { get; private set; }

		public List<Guid> AddressListGuids { get; private set; }
	}
}
