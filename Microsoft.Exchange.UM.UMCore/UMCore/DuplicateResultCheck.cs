using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DuplicateResultCheck
	{
		internal DuplicateResultCheck()
		{
			this.resultsHash = new List<ResultWrapper>();
		}

		internal bool Contains(List<IUMRecognitionPhrase> results)
		{
			ResultWrapper resultWrapper = new ResultWrapper(results);
			if (this.resultsHash.Count == 0)
			{
				this.resultsHash.Add(resultWrapper);
				return false;
			}
			bool flag = false;
			foreach (ResultWrapper resultWrapper2 in this.resultsHash)
			{
				if (resultWrapper2.CompareTo(resultWrapper) == 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.resultsHash.Add(resultWrapper);
			}
			return flag;
		}

		private List<ResultWrapper> resultsHash;
	}
}
