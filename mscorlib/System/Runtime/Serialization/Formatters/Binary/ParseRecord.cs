using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class ParseRecord
	{
		internal ParseRecord()
		{
		}

		internal void Init()
		{
			this.PRparseTypeEnum = InternalParseTypeE.Empty;
			this.PRobjectTypeEnum = InternalObjectTypeE.Empty;
			this.PRarrayTypeEnum = InternalArrayTypeE.Empty;
			this.PRmemberTypeEnum = InternalMemberTypeE.Empty;
			this.PRmemberValueEnum = InternalMemberValueE.Empty;
			this.PRobjectPositionEnum = InternalObjectPositionE.Empty;
			this.PRname = null;
			this.PRvalue = null;
			this.PRkeyDt = null;
			this.PRdtType = null;
			this.PRdtTypeCode = InternalPrimitiveTypeE.Invalid;
			this.PRisEnum = false;
			this.PRobjectId = 0L;
			this.PRidRef = 0L;
			this.PRarrayElementTypeString = null;
			this.PRarrayElementType = null;
			this.PRisArrayVariant = false;
			this.PRarrayElementTypeCode = InternalPrimitiveTypeE.Invalid;
			this.PRrank = 0;
			this.PRlengthA = null;
			this.PRpositionA = null;
			this.PRlowerBoundA = null;
			this.PRupperBoundA = null;
			this.PRindexMap = null;
			this.PRmemberIndex = 0;
			this.PRlinearlength = 0;
			this.PRrectangularMap = null;
			this.PRisLowerBound = false;
			this.PRtopId = 0L;
			this.PRheaderId = 0L;
			this.PRisValueTypeFixup = false;
			this.PRnewObj = null;
			this.PRobjectA = null;
			this.PRprimitiveArray = null;
			this.PRobjectInfo = null;
			this.PRisRegistered = false;
			this.PRmemberData = null;
			this.PRsi = null;
			this.PRnullCount = 0;
		}

		internal static int parseRecordIdCount = 1;

		internal int PRparseRecordId;

		internal InternalParseTypeE PRparseTypeEnum;

		internal InternalObjectTypeE PRobjectTypeEnum;

		internal InternalArrayTypeE PRarrayTypeEnum;

		internal InternalMemberTypeE PRmemberTypeEnum;

		internal InternalMemberValueE PRmemberValueEnum;

		internal InternalObjectPositionE PRobjectPositionEnum;

		internal string PRname;

		internal string PRvalue;

		internal object PRvarValue;

		internal string PRkeyDt;

		internal Type PRdtType;

		internal InternalPrimitiveTypeE PRdtTypeCode;

		internal bool PRisVariant;

		internal bool PRisEnum;

		internal long PRobjectId;

		internal long PRidRef;

		internal string PRarrayElementTypeString;

		internal Type PRarrayElementType;

		internal bool PRisArrayVariant;

		internal InternalPrimitiveTypeE PRarrayElementTypeCode;

		internal int PRrank;

		internal int[] PRlengthA;

		internal int[] PRpositionA;

		internal int[] PRlowerBoundA;

		internal int[] PRupperBoundA;

		internal int[] PRindexMap;

		internal int PRmemberIndex;

		internal int PRlinearlength;

		internal int[] PRrectangularMap;

		internal bool PRisLowerBound;

		internal long PRtopId;

		internal long PRheaderId;

		internal ReadObjectInfo PRobjectInfo;

		internal bool PRisValueTypeFixup;

		internal object PRnewObj;

		internal object[] PRobjectA;

		internal PrimitiveArray PRprimitiveArray;

		internal bool PRisRegistered;

		internal object[] PRmemberData;

		internal SerializationInfo PRsi;

		internal int PRnullCount;
	}
}
