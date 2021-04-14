using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RecognitionResultType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class RecognitionResult
	{
		public RecognitionResult()
		{
		}

		internal RecognitionResult(string result)
		{
			this.Result = result;
		}

		[XmlAttribute("Result", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(IsRequired = true)]
		public string Result { get; set; }
	}
}
