using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal interface IConfigAdapter
	{
		string GetSetting(string key);
	}
}
