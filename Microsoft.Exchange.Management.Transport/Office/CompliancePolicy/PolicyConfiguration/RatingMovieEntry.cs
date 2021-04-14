using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	[DataContract]
	public enum RatingMovieEntry
	{
		[EnumMember]
		DontAllow,
		[EnumMember]
		AllowAll = 1000,
		[EnumMember]
		USRatingG = 100,
		[EnumMember]
		USRatingPG = 200,
		[EnumMember]
		USRatingPG13 = 300,
		[EnumMember]
		USRatingR = 400,
		[EnumMember]
		USRatingNC17 = 500,
		[EnumMember]
		AURatingG = 100,
		[EnumMember]
		AURatingPG = 200,
		[EnumMember]
		AURatingM = 350,
		[EnumMember]
		AURatingMA15plus = 375,
		[EnumMember]
		AURatingR18plus = 400,
		[EnumMember]
		CARatingG = 100,
		[EnumMember]
		CARatingPG = 200,
		[EnumMember]
		CARating14A = 325,
		[EnumMember]
		CARating18A = 400,
		[EnumMember]
		CARatingR = 500,
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
		IERatingG = 100,
		[EnumMember]
		IERatingPG = 200,
		[EnumMember]
		IERating12 = 300,
		[EnumMember]
		IERating15 = 350,
		[EnumMember]
		IERating16 = 375,
		[EnumMember]
		IERating18 = 400,
		[EnumMember]
		JPRatingG = 100,
		[EnumMember]
		JPRatingPG12 = 200,
		[EnumMember]
		JPRatingRdash15 = 300,
		[EnumMember]
		JPRatingRdash18 = 400,
		[EnumMember]
		NZRatingG = 100,
		[EnumMember]
		NZRatingPG = 200,
		[EnumMember]
		NZRatingM = 300,
		[EnumMember]
		NZRatingR13 = 325,
		[EnumMember]
		NZRatingR15 = 350,
		[EnumMember]
		NZRatingR16 = 375,
		[EnumMember]
		NZRatingR18 = 400,
		[EnumMember]
		NZRatingR = 500,
		[EnumMember]
		NZRatingRP16 = 600,
		[EnumMember]
		GBRatingU = 100,
		[EnumMember]
		GBRatingUc = 150,
		[EnumMember]
		GBRatingPG = 200,
		[EnumMember]
		GBRating12 = 300,
		[EnumMember]
		GBRating12A = 325,
		[EnumMember]
		GBRating15 = 350,
		[EnumMember]
		GBRating18 = 400
	}
}
