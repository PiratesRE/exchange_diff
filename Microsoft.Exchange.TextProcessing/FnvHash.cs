using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	internal class FnvHash
	{
		public static ulong Fnv1A64(byte[] input)
		{
			ulong num = 14695981039346656037UL;
			if (input != null && input.Length != 0)
			{
				foreach (byte b in input)
				{
					num = (num ^ (ulong)b) * 1099511628211UL;
				}
			}
			return num;
		}

		public static ulong Fnv1A64(string input)
		{
			ulong num = 14695981039346656037UL;
			if (!string.IsNullOrEmpty(input))
			{
				foreach (char c in input)
				{
					num = (num ^ (ulong)c) * 1099511628211UL;
				}
			}
			return num;
		}

		public static uint Fnv1A32(string input)
		{
			uint num = 2166136261U;
			if (!string.IsNullOrEmpty(input))
			{
				foreach (char c in input)
				{
					num = (num ^ (uint)c) * 16777619U;
				}
			}
			return num;
		}

		public static uint Fnv1A32(List<char> input)
		{
			uint num = 2166136261U;
			if (input != null && input.Count != 0)
			{
				foreach (char c in input)
				{
					num = (num ^ (uint)c) * 16777619U;
				}
			}
			return num;
		}
	}
}
