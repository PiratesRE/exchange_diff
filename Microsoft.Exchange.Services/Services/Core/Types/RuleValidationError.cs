using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleValidationErrorType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RuleValidationError
	{
		[XmlElement(Order = 0)]
		public RuleFieldURI FieldURI { get; set; }

		[XmlElement(Order = 1)]
		public RuleValidationErrorCode ErrorCode { get; set; }

		[XmlElement(Order = 2)]
		public string ErrorMessage { get; set; }

		[XmlElement(Order = 3)]
		public string FieldValue { get; set; }
	}
}
