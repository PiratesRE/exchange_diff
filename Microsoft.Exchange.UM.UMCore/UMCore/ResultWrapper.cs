using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ResultWrapper : IComparable
	{
		internal ResultWrapper(List<IUMRecognitionPhrase> results)
		{
			this.count = results.Count;
			this.alternateHash = ResultWrapper.GetHash(results);
		}

		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			ResultWrapper resultWrapper = obj as ResultWrapper;
			if (resultWrapper == null)
			{
				return -1;
			}
			int num = this.count - resultWrapper.Count;
			if (num != 0)
			{
				return num;
			}
			num = 0;
			foreach (string key in this.alternateHash.Keys)
			{
				if (!resultWrapper.Contains(key))
				{
					num = -1;
					break;
				}
			}
			return num;
		}

		internal static string GetIdFromPhrase(IUMRecognitionPhrase phrase)
		{
			string result = null;
			ResultWrapper.GetResultTypeAndIdFromPhrase(phrase, out result);
			return result;
		}

		internal static ResultType GetResultTypeAndIdFromPhrase(IUMRecognitionPhrase phrase, out string id)
		{
			id = null;
			string text = (string)phrase["ResultType"];
			ResultType result = ResultType.None;
			string a;
			if ((a = text) != null)
			{
				if (!(a == "DirectoryContact"))
				{
					if (!(a == "PersonalContact"))
					{
						if (a == "Department")
						{
							id = (string)phrase["DepartmentName"];
							result = ResultType.Department;
						}
					}
					else
					{
						id = (string)phrase["ContactId"];
						result = ResultType.PersonalContact;
					}
				}
				else
				{
					id = (string)phrase["ObjectGuid"];
					result = ResultType.DirectoryContact;
				}
			}
			return result;
		}

		internal static Dictionary<string, IUMRecognitionPhrase> GetHash(List<IUMRecognitionPhrase> results)
		{
			Dictionary<string, IUMRecognitionPhrase> dictionary = new Dictionary<string, IUMRecognitionPhrase>();
			foreach (IUMRecognitionPhrase iumrecognitionPhrase in results)
			{
				string idFromPhrase = ResultWrapper.GetIdFromPhrase(iumrecognitionPhrase);
				if (!dictionary.ContainsKey(idFromPhrase))
				{
					dictionary.Add(idFromPhrase, iumrecognitionPhrase);
				}
			}
			return dictionary;
		}

		internal bool Contains(string key)
		{
			return this.alternateHash.ContainsKey(key);
		}

		private int count;

		private Dictionary<string, IUMRecognitionPhrase> alternateHash;
	}
}
