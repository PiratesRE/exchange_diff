using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class OUPickerParameter : FormletParameter
	{
		public OUPickerParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, string serviceUrl) : base(name, dialogTitle, dialogLabel)
		{
			this.ServiceUrl = serviceUrl;
		}

		[DataMember]
		public string ServiceUrl { get; private set; }
	}
}
