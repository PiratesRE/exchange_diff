using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class SupplementalData
	{
		public SupplementalData()
		{
			this.data = new Dictionary<string, Dictionary<string, string>>();
		}

		public void Add(string dataType, KeyValuePair<string, string> supplementalData)
		{
			Dictionary<string, string> dictionary;
			if (this.data.TryGetValue(dataType, out dictionary))
			{
				if (!dictionary.ContainsKey(supplementalData.Key))
				{
					dictionary.Add(supplementalData.Key, supplementalData.Value);
					return;
				}
			}
			else
			{
				dictionary = new Dictionary<string, string>(1);
				dictionary.Add(supplementalData.Key, supplementalData.Value);
				this.data[dataType] = dictionary;
			}
		}

		public Dictionary<string, string> Get(string dataType)
		{
			Dictionary<string, string> result;
			if (this.data.TryGetValue(dataType, out result))
			{
				return result;
			}
			return new Dictionary<string, string>();
		}

		private readonly Dictionary<string, Dictionary<string, string>> data;
	}
}
