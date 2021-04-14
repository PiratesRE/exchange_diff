using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleIKnowEmailAddressCollection_v1 : BinarySearchableEmailAddressCollection
	{
		public PeopleIKnowEmailAddressCollection_v1(ICollection<string> strings, ITracer tracer, int traceId) : base(strings, tracer, traceId)
		{
		}

		public PeopleIKnowEmailAddressCollection_v1(byte[] data, ITracer tracer, int traceId) : base(data, tracer, traceId)
		{
		}

		protected sealed override byte Version
		{
			get
			{
				return 1;
			}
		}

		protected sealed override BinarySearchableEmailAddressCollection.IMetadataSerializer MetadataSerializer
		{
			get
			{
				return new PeopleIKnowEmailAddressCollection_v1.MetadataSerializerV1();
			}
		}

		private class MetadataSerializerV1 : BinarySearchableEmailAddressCollection.IMetadataSerializer
		{
			public int SizeOfMetadata
			{
				get
				{
					return 0;
				}
			}

			public byte[] Serialize(string email)
			{
				return Array<byte>.Empty;
			}

			public PeopleIKnowMetadata Deserialize(byte[] buffer)
			{
				return null;
			}
		}
	}
}
