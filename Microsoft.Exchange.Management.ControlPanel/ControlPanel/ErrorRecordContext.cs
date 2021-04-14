using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ErrorRecordContext
	{
		[DataMember(EmitDefaultValue = false)]
		public JsonDictionary<object> LastOuput { get; set; }
	}
}
