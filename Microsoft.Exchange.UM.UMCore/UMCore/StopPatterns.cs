using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StopPatterns : IEnumerable<string>, IEnumerable
	{
		static StopPatterns()
		{
			StopPatterns.anyKeyOnly.Add("anyKey", true);
			StopPatterns.empty = new StopPatterns();
		}

		public StopPatterns()
		{
			this.patternBargeinMap = new Dictionary<string, bool>();
		}

		public StopPatterns(int initialCapacity)
		{
			this.patternBargeinMap = new Dictionary<string, bool>(initialCapacity);
		}

		public static StopPatterns AnyKeyOnly
		{
			get
			{
				return StopPatterns.anyKeyOnly;
			}
		}

		public static StopPatterns Empty
		{
			get
			{
				return StopPatterns.empty;
			}
		}

		public int Count
		{
			get
			{
				return this.patternBargeinMap.Count;
			}
		}

		public bool ContainsAnyKey
		{
			get
			{
				return this.containsAnyKey;
			}
		}

		public static bool IsAnyKeyInput(ReadOnlyCollection<byte> digits)
		{
			bool result = false;
			if (digits.Count > 0)
			{
				result = true;
				foreach (byte b in digits)
				{
					if (b == 35 || b == 42)
					{
						result = false;
					}
				}
			}
			return result;
		}

		public void Add(string pattern, bool bargeIn)
		{
			this.patternBargeinMap[pattern] = bargeIn;
			this.containsAnyKey |= pattern.Equals("anyKey", StringComparison.OrdinalIgnoreCase);
		}

		public void Add(StopPatterns patterns)
		{
			foreach (KeyValuePair<string, bool> keyValuePair in patterns.patternBargeinMap)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this.patternBargeinMap.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.patternBargeinMap.Keys.GetEnumerator();
		}

		public void CountMatches(ReadOnlyCollection<byte> input, out int partialMatches, out int completeMatches)
		{
			partialMatches = (completeMatches = 0);
			foreach (string stopPattern in this)
			{
				bool flag;
				bool flag2;
				StopPatterns.ComputeMatch(input, stopPattern, out flag, out flag2);
				if (flag)
				{
					partialMatches++;
				}
				if (flag2)
				{
					completeMatches++;
				}
			}
		}

		public bool IsBargeInPattern(ReadOnlyCollection<byte> input)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			foreach (KeyValuePair<string, bool> keyValuePair in this.patternBargeinMap)
			{
				bool flag4;
				bool flag5;
				StopPatterns.ComputeMatch(input, keyValuePair.Key, out flag4, out flag5);
				if (flag4)
				{
					flag2 = true;
					flag3 &= keyValuePair.Value;
					if (flag5)
					{
						flag = keyValuePair.Value;
						break;
					}
				}
			}
			return flag || (flag2 && flag3);
		}

		private static void ComputeMatch(ReadOnlyCollection<byte> input, string stopPattern, out bool partial, out bool complete)
		{
			partial = (complete = false);
			if (StopPatterns.IsAnyKeyPattern(stopPattern) && StopPatterns.IsAnyKeyInput(input))
			{
				partial = true;
				complete = false;
			}
			else if (StopPatterns.IsDiagnosticSequence(input))
			{
				partial = true;
				complete = true;
			}
			else if (input.Count <= stopPattern.Length)
			{
				partial = true;
				complete = (input.Count == stopPattern.Length);
				for (int i = 0; i < input.Count; i++)
				{
					if ((char)input[i] != stopPattern[i])
					{
						partial = (complete = false);
						break;
					}
				}
			}
			ExAssert.RetailAssert(!complete || partial, "A sequence cannot be a complete match without also being a partial match!");
		}

		private static bool IsAnyKeyPattern(string stopPattern)
		{
			return stopPattern != null && stopPattern.Equals("anyKey", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsDiagnosticSequence(ReadOnlyCollection<byte> digits)
		{
			bool result = false;
			if (digits.Count > 0)
			{
				switch (digits[0])
				{
				case 65:
					result = true;
					break;
				case 66:
					result = true;
					break;
				case 67:
					result = true;
					break;
				case 68:
					result = true;
					break;
				}
			}
			return result;
		}

		private static StopPatterns anyKeyOnly = new StopPatterns();

		private static StopPatterns empty;

		private Dictionary<string, bool> patternBargeinMap;

		private bool containsAnyKey;
	}
}
