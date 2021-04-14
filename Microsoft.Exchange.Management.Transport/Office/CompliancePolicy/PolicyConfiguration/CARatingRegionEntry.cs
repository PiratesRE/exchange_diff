using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum CARatingRegionEntry
	{
		[EnumMember]
		us = 1,
		[EnumMember]
		au,
		[EnumMember]
		ca,
		[EnumMember]
		de,
		[EnumMember]
		fr,
		[EnumMember]
		ie,
		[EnumMember]
		jp,
		[EnumMember]
		nz,
		[EnumMember]
		gb
	}
}
