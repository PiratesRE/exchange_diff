using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class SetPropertyUpdate : PropertyUpdate
	{
		[XmlIgnore]
		[IgnoreDataMember]
		internal abstract ServiceObject ServiceObject { get; }
	}
}
