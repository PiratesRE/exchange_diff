using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class IServerModelConfigurationExtensions
	{
		public static IEnumerable<int> GetAllModelVersions(this IServerModelConfiguration serverModelConfig)
		{
			return new List<int>(Enumerable.Range(serverModelConfig.MinModelVersion, serverModelConfig.MaxModelVersion - serverModelConfig.MinModelVersion + 1));
		}

		public static IEnumerable<int> GetSupportedModelVersions(this IServerModelConfiguration serverModelConfig)
		{
			if (serverModelConfig.BlockedModelVersions == null)
			{
				return serverModelConfig.GetAllModelVersions();
			}
			return serverModelConfig.GetAllModelVersions().Except(serverModelConfig.BlockedModelVersions);
		}

		public static int GetLatestSupportedModelVersion(this IServerModelConfiguration serverModelConfig)
		{
			return serverModelConfig.GetSupportedModelVersions().Max();
		}

		public static IEnumerable<int> GetSupportedClassificationModelVersions(this IServerModelConfiguration serverModelConfig)
		{
			IEnumerable<int> second = serverModelConfig.ClassificationModelVersions ?? Enumerable.Empty<int>();
			return serverModelConfig.GetSupportedModelVersions().Intersect(second);
		}

		public static int GetLatestSupportedClassificationModelVersion(this IServerModelConfiguration serverModelConfig)
		{
			return serverModelConfig.GetSupportedClassificationModelVersions().Max();
		}
	}
}
