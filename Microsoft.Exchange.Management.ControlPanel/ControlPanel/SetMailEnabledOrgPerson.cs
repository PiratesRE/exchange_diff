using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SetMailEnabledOrgPerson : SetObjectProperties
	{
		public IEnumerable<string> EmailAddresses { get; set; }

		public string MailTip
		{
			get
			{
				return (string)base["MailTip"];
			}
			set
			{
				base["MailTip"] = value;
			}
		}
	}
}
