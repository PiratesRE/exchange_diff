using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UriKindValidatorInfo : ValidatorInfo
	{
		internal UriKindValidatorInfo(UriKindConstraint constraint) : base("UriKindValidator")
		{
			this.ExpectedUriKind = Enum.GetName(typeof(UriKind), constraint.ExpectedUriKind);
		}

		[DataMember]
		public string ExpectedUriKind { get; set; }
	}
}
