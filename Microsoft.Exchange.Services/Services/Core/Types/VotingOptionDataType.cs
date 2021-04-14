using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", TypeName = "VotingOptionDataType")]
	[Serializable]
	public class VotingOptionDataType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string DisplayName { get; set; }

		[IgnoreDataMember]
		public SendPromptType SendPrompt { get; set; }

		[DataMember(Name = "SendPrompt", Order = 2)]
		[XmlIgnore]
		public string SendPromptString
		{
			get
			{
				return EnumUtilities.ToString<SendPromptType>(this.SendPrompt);
			}
			set
			{
				this.SendPrompt = EnumUtilities.Parse<SendPromptType>(value);
			}
		}
	}
}
