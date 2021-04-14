using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleIKnowEmailAddressCollection_v2 : BinarySearchableEmailAddressCollection
	{
		public PeopleIKnowEmailAddressCollection_v2(IDictionary<string, PeopleIKnowMetadata> peopleInfo, ITracer tracer, int traceId) : base(peopleInfo.Keys, tracer, traceId)
		{
			this.metadataSerializer = new PeopleIKnowEmailAddressCollection_v2.MetadataSerializerV2(peopleInfo);
		}

		public PeopleIKnowEmailAddressCollection_v2(byte[] data, ITracer tracer, int traceId) : base(data, tracer, traceId)
		{
			this.metadataSerializer = new PeopleIKnowEmailAddressCollection_v2.MetadataSerializerV2(null);
		}

		protected sealed override byte Version
		{
			get
			{
				return 2;
			}
		}

		protected sealed override BinarySearchableEmailAddressCollection.IMetadataSerializer MetadataSerializer
		{
			get
			{
				return this.metadataSerializer;
			}
		}

		private readonly BinarySearchableEmailAddressCollection.IMetadataSerializer metadataSerializer;

		private class MetadataSerializerV2 : BinarySearchableEmailAddressCollection.IMetadataSerializer
		{
			public MetadataSerializerV2(IDictionary<string, PeopleIKnowMetadata> metadata = null)
			{
				this.metadata = metadata;
			}

			public byte[] Serialize(string email)
			{
				return BitConverter.GetBytes(this.metadata[email].RelevanceScore);
			}

			public PeopleIKnowMetadata Deserialize(byte[] buffer)
			{
				if (buffer.Length != this.SizeOfMetadata)
				{
					throw new ArgumentException(string.Format("Size of the buffer unexpected: {0}", buffer.Length));
				}
				return new PeopleIKnowMetadata
				{
					RelevanceScore = BitConverter.ToInt32(buffer, 0)
				};
			}

			public int SizeOfMetadata
			{
				get
				{
					return 4;
				}
			}

			private readonly IDictionary<string, PeopleIKnowMetadata> metadata;
		}
	}
}
