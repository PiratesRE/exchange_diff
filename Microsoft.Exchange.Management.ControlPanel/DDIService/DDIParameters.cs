using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[DataContract]
	public class DDIParameters
	{
		[DataMember]
		public JsonDictionary<object> Parameters { get; set; }
	}
}
