using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SetObjectProperties : WebServiceParameters
	{
		public SetObjectProperties()
		{
		}

		[DataMember]
		public ReturnObjectTypes ReturnObjectType { get; set; }
	}
}
