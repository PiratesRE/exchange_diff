using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal struct MetadataGetAllRecord
	{
		public MBIdentifier Identifier;

		public MBAttributes Attributes;

		public MBUserType UserType;

		public MBDataType DataType;

		public int DataLen;

		public int DataOffset;

		public int DataTag;
	}
}
