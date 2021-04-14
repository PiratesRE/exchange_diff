using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NotNullOrEmptyValidatorInfo : ValidatorInfo
	{
		public NotNullOrEmptyValidatorInfo() : base("NotNullOrEmptyValidator")
		{
		}
	}
}
