using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	internal class DomainMatchMap<T> where T : class, DomainMatchMap<T>.IDomainEntry
	{
		public DomainMatchMap(IList<T> entries)
		{
			this.exact = new Dictionary<DomainMatchMap<T>.SubString, T>(entries.Count);
			this.wildcard = new Dictionary<DomainMatchMap<T>.SubString, T>(entries.Count);
			foreach (T value in entries)
			{
				if (value.DomainName.SmtpDomain == null)
				{
					this.star = value;
				}
				else
				{
					int num = DomainMatchMap<T>.CountDots(value.DomainName.Domain);
					if (num > this.maxDots)
					{
						this.maxDots = num;
					}
					DomainMatchMap<T>.SubString key = new DomainMatchMap<T>.SubString(value.DomainName.Domain, 0);
					if (value.DomainName.IncludeSubDomains)
					{
						this.wildcard[key] = value;
						if (!this.exact.ContainsKey(key))
						{
							this.exact.Add(key, value);
						}
					}
					else
					{
						this.exact[key] = value;
					}
				}
			}
			this.maxDots++;
		}

		public T Star
		{
			get
			{
				return this.star;
			}
		}

		public static int CountDots(string s)
		{
			int num = 0;
			for (int num2 = s.IndexOf('.'); num2 != -1; num2 = s.IndexOf('.', num2 + 1))
			{
				num++;
			}
			return num;
		}

		public static int[] FindAllDots(string s)
		{
			int num = DomainMatchMap<T>.CountDots(s);
			int[] array = new int[num];
			int num2 = s.IndexOf('.');
			int num3 = 0;
			while (num2 != -1)
			{
				array[num3] = num2;
				num2 = s.IndexOf('.', num2 + 1);
				num3++;
			}
			return array;
		}

		public T GetBestMatch(SmtpDomain domain)
		{
			if (domain == null)
			{
				return default(T);
			}
			DomainMatchMap<T>.SubString subString = new DomainMatchMap<T>.SubString(domain.Domain, 0);
			T result;
			if (this.exact.TryGetValue(subString, out result))
			{
				return result;
			}
			int[] array = DomainMatchMap<T>.FindAllDots(domain.Domain);
			for (int i = (array.Length > this.maxDots) ? (array.Length - this.maxDots) : 0; i < array.Length; i++)
			{
				subString.SetIndex(array[i] + 1);
				if (this.wildcard.TryGetValue(subString, out result))
				{
					return result;
				}
			}
			return this.star;
		}

		public T GetBestMatch(string domainName)
		{
			SmtpDomain domain;
			SmtpDomain.TryParse(domainName, out domain);
			return this.GetBestMatch(domain);
		}

		public IEnumerator<PublicT> GetAllDomains<PublicT>() where PublicT : class
		{
			foreach (T entry in this.exact.Values)
			{
				PublicT publicEntry = entry as PublicT;
				if (publicEntry != null)
				{
					yield return publicEntry;
				}
			}
			foreach (T entry2 in this.wildcard.Values)
			{
				PublicT publicEntry = entry2 as PublicT;
				if (publicEntry != null)
				{
					yield return publicEntry;
				}
			}
			yield break;
		}

		private const char Dot = '.';

		private readonly Dictionary<DomainMatchMap<T>.SubString, T> exact;

		private readonly Dictionary<DomainMatchMap<T>.SubString, T> wildcard;

		private readonly T star = default(T);

		private readonly int maxDots;

		public interface IDomainEntry
		{
			SmtpDomainWithSubdomains DomainName { get; }
		}

		public class SubString
		{
			public SubString(string s, int start)
			{
				this.s = s;
				this.start = start;
			}

			public void SetIndex(int i)
			{
				this.start = i;
			}

			public override int GetHashCode()
			{
				int num = 0;
				for (int i = this.start; i < this.s.Length; i++)
				{
					num = num * 65599 + (int)char.ToLowerInvariant(this.s[i]);
				}
				return num;
			}

			public override bool Equals(object obj)
			{
				DomainMatchMap<T>.SubString subString = obj as DomainMatchMap<T>.SubString;
				return subString != null && 0 == string.Compare(this.s, this.start, subString.s, subString.start, int.MaxValue, StringComparison.OrdinalIgnoreCase);
			}

			private int start;

			private string s;
		}
	}
}
