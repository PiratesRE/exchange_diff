using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum CARatingTvShowEntry
	{
		[EnumMember]
		DontAllow,
		[EnumMember]
		AllowAll = 1000,
		[EnumMember]
		USRatingTVY = 100,
		[EnumMember]
		USRatingTVY7 = 200,
		[EnumMember]
		USRatingTVG = 300,
		[EnumMember]
		USRatingTVPG = 400,
		[EnumMember]
		USRatingTV14 = 500,
		[EnumMember]
		USRatingTVMA = 600,
		[EnumMember]
		AURatingP = 100,
		[EnumMember]
		AURatingC = 200,
		[EnumMember]
		AURatingG = 300,
		[EnumMember]
		AURatingPG = 400,
		[EnumMember]
		AURatingM = 500,
		[EnumMember]
		AURatingMA15plus = 550,
		[EnumMember]
		AURatingAv15plus = 575,
		[EnumMember]
		CARatingC = 100,
		[EnumMember]
		CARatingC8 = 200,
		[EnumMember]
		CARatingG = 300,
		[EnumMember]
		CARatingPG = 400,
		[EnumMember]
		CARating14plus = 500,
		[EnumMember]
		CARating18plus = 600,
		[EnumMember]
		DERatingab0Jahren = 75,
		[EnumMember]
		DERatingab6Jahren = 100,
		[EnumMember]
		DERatingab12Jahren = 200,
		[EnumMember]
		DERatingab16Jahren = 500,
		[EnumMember]
		DERatingab18Jahren = 600,
		[EnumMember]
		FRRating10minus = 100,
		[EnumMember]
		FRRating12minus = 200,
		[EnumMember]
		FRRating16minus = 500,
		[EnumMember]
		FRRating18minus = 600,
		[EnumMember]
		IERatingGA = 100,
		[EnumMember]
		IERatingCh = 200,
		[EnumMember]
		IERatingYA = 400,
		[EnumMember]
		IERatingPS = 500,
		[EnumMember]
		IERatingMA = 600,
		[EnumMember]
		JPRatingExplicitAllowed = 500,
		[EnumMember]
		NZRatingG = 200,
		[EnumMember]
		NZRatingPGR = 400,
		[EnumMember]
		NZRatingAO = 600,
		[EnumMember]
		GBRatingCaution = 500
	}
}
