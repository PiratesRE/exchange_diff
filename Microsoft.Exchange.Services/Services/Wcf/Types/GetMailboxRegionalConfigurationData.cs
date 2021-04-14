using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMailboxRegionalConfigurationData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public string DateFormat { get; set; }

		[DataMember]
		public bool DefaultFolderNameMatchingUserLanguage { get; set; }

		[DataMember]
		public string Language { get; set; }

		[DataMember]
		public string TimeFormat { get; set; }

		[DataMember]
		public string TimeZone { get; set; }
	}
}
