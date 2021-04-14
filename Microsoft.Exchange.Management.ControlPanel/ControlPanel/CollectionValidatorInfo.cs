using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(CollectionItemValidatorInfo))]
	public class CollectionValidatorInfo : ValidatorInfo
	{
		public CollectionValidatorInfo(string typeName) : base(typeName)
		{
		}
	}
}
