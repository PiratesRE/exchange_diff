using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CompareStringValidatorInfo : CompareValidatorInfo
	{
		public CompareStringValidatorInfo(string controlToCompare) : base("CompareStringValidator", controlToCompare)
		{
		}
	}
}
