using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CompleteFindInGALSpeechRecognitionRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CompleteFindInGALSpeechRecognitionRequest : BaseRequest
	{
		[XmlElement("RecognitionId")]
		[DataMember(Name = "RecognitionId", IsRequired = true)]
		public RecognitionId RecognitionId { get; set; }

		[XmlElement("AudioData")]
		[DataMember(Name = "AudioData", IsRequired = true)]
		public byte[] AudioData { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CompleteFindInGALSpeechRecognitionCommand(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}
	}
}
