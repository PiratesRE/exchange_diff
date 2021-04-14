using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PeopleIKnowEmailAddressCollection
	{
		public static PeopleIKnowEmailAddressCollection CreateFromStringCollection(IDictionary<string, PeopleIKnowMetadata> peopleInfo, ITracer tracer, int traceId)
		{
			return PeopleIKnowEmailAddressCollection.CreateFromStringCollection(peopleInfo, tracer, traceId, 1);
		}

		public static PeopleIKnowEmailAddressCollection CreateFromStringCollection(IDictionary<string, PeopleIKnowMetadata> peopleInfo, ITracer tracer, int traceId, int version)
		{
			ArgumentValidator.ThrowIfNull("peopleInfo", peopleInfo);
			switch (version)
			{
			case 1:
				return new PeopleIKnowEmailAddressCollection_v1(peopleInfo.Keys, tracer, traceId);
			case 2:
				return new PeopleIKnowEmailAddressCollection_v2(peopleInfo, tracer, traceId);
			default:
				throw new ArgumentOutOfRangeException("Version number unexpected: {0}".FormatWith(new object[]
				{
					version
				}));
			}
		}

		public static PeopleIKnowEmailAddressCollection CreateFromByteArray(byte[] bytes, ITracer tracer, int traceId)
		{
			ArgumentValidator.ThrowIfNull("bytes", bytes);
			if (bytes.Length == 0)
			{
				throw new ArgumentOutOfRangeException("bytes");
			}
			byte b = bytes[0];
			if (b == 0)
			{
				throw new InvalidDataException("0 is not a valid version for this data format");
			}
			if (b == 1)
			{
				return new PeopleIKnowEmailAddressCollection_v1(bytes, tracer, traceId);
			}
			if (b == 2)
			{
				return new PeopleIKnowEmailAddressCollection_v2(bytes, tracer, traceId);
			}
			return null;
		}

		public abstract byte[] Data { get; }

		public bool Contains(string s)
		{
			PeopleIKnowMetadata peopleIKnowMetadata;
			return this.Contains(s, out peopleIKnowMetadata);
		}

		public abstract bool Contains(string s, out PeopleIKnowMetadata metadata);
	}
}
