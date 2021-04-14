using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class GrammarFileNames
	{
		internal static string GetFileNameForGALUser()
		{
			return "gal";
		}

		internal static string GetFileNameForDL()
		{
			return "distributionList";
		}

		internal static string GetFileNameForDialPlan(UMDialPlan dp)
		{
			ValidateArgument.NotNull(dp, "UMDialPlan");
			return dp.Id.ObjectGuid.ToString();
		}

		internal static string GetFileNameForQBDN(ADObjectId objectId)
		{
			ValidateArgument.NotNull(objectId, "ADObjectId");
			return objectId.ObjectGuid.ToString();
		}

		internal static string GetFileNameForCustomAddressList(ADObjectId objectId)
		{
			return GrammarFileNames.GetFileNameForQBDN(objectId);
		}

		internal const string DiskGrammarCacheDirectoryName = "Cache";
	}
}
