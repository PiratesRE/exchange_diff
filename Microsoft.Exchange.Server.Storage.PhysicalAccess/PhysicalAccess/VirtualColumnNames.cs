using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public static class VirtualColumnNames
	{
		public const string PageNumber = "DatabasePageNumber";

		public const string DataSize = "RecordDataSize";

		public const string LongValueDataSize = "RecordLongValueDataSize";

		public const string OverheadSize = "RecordOverheadSize";

		public const string LongValueOverheadSize = "RecordLongValueOverheadSize";

		public const string NonTaggedColumnCount = "RecordNonTaggedColumnCount";

		public const string TaggedColumnCount = "RecordTaggedColumnCount";

		public const string LongValueCount = "RecordLongValueCount";

		public const string MultiValueCount = "RecordMultiValueCount";

		public const string CompressedColumnCount = "RecordCompressedColumnCount";

		public const string CompressedDataSize = "RecordCompressedDataSize";

		public const string CompressedLongValueDataSize = "RecordCompressedLongValueDataSize";
	}
}
