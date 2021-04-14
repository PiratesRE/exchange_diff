using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class AsrSearchResult
	{
		internal static AsrSearchResult Create(IUMRecognitionPhrase result, UMSubscriber subscriber, Guid tenantGuid)
		{
			string text = (string)result["ResultType"];
			string a;
			if ((a = text) != null)
			{
				if (a == "DirectoryContact")
				{
					return new AsrDirectorySearchResult(result, tenantGuid);
				}
				if (a == "Department")
				{
					return new AsrDepartmentSearchResult(result);
				}
				if (a == "PersonalContact")
				{
					return new AsrPersonalContactsSearchResult(result, subscriber);
				}
			}
			throw new InvalidResultTypeException(text);
		}

		internal static AsrSearchResult Create(string result)
		{
			return new AsrExtensionSearchResult(result);
		}

		internal static AsrSearchResult Create(CustomMenuKeyMapping result)
		{
			return new AsrDepartmentSearchResult(result);
		}

		internal abstract void SetManagerVariables(ActivityManager manager, BaseUMCallSession vo);
	}
}
