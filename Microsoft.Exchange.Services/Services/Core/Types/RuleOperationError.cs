using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleOperationErrorType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RuleOperationError
	{
		[XmlElement(Order = 0)]
		public int OperationIndex { get; set; }

		[XmlArrayItem("Error", Type = typeof(RuleValidationError))]
		[XmlArray(Order = 1)]
		public RuleValidationError[] ValidationErrors { get; set; }
	}
}
