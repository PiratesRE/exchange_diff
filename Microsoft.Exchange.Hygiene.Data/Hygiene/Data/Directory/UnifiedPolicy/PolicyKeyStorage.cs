using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Hygiene.Data.Directory.UnifiedPolicy
{
	[Serializable]
	internal sealed class PolicyKeyStorage : Dictionary<string, string>
	{
		public PolicyKeyStorage() : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public PolicyKeyStorage(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
