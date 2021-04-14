using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractCustomPropertySerializer
	{
		protected AbstractCustomPropertySerializer(int version, int minSupportedDeserializerVersion)
		{
			this.Version = version;
			this.MinSupportedDeserializerVersion = minSupportedDeserializerVersion;
		}

		public abstract byte[] Serialize(IDictionary<string, string> data, out bool isTruncated);

		public abstract Dictionary<string, string> Deserialize(byte[] array, out bool isTruncated);

		public const int ByteLimit = 1024;

		public const int HeaderLength = 3;

		public const int HeaderVersionIndex = 0;

		public const int HeaderMinSupportedVersionIndex = 1;

		public const int HeaderFlagsIndex = 2;

		public readonly int Version;

		public readonly int MinSupportedDeserializerVersion;
	}
}
