using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NotificationPhoneNumberParameter : FormletParameter
	{
		public NotificationPhoneNumberParameter(string name) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
		}
	}
}
