using System;

namespace Microsoft.Exchange.TextProcessing
{
	internal class LshFingerprintConstants
	{
		public const int MaximumContentLength = 50000;

		public const int MaxShingleLength = 8000;

		public const short AlgorithmVersion = 2;

		public const int HashPermutation = 64;

		public const int HashBinNumbers = 4;

		public const int HashBits = 2;

		public const int CharBits = 16;

		public const int FingerprintDenorminator = 48;

		public const int FingerprintDenorminatorOne = 32;

		public const int FingerprintNumeratorOffset = 16;

		public const int ShortBits = 16;

		public const uint BBits = 3U;

		public const uint BBitsOne = 1U;

		public const int MinimumShingleCountExclude = 10;

		public const int OneBitMask = 1431655765;

		public const ulong Fnv64OffsetBasis = 14695981039346656037UL;

		public const ulong Fnv64Prime = 1099511628211UL;

		public const uint Fnv32OffsetBasis = 2166136261U;

		public const uint Fnv32Prime = 16777619U;

		public const int ReasonableTriesForFingreprintConflicts = 4;

		public static readonly char[] DotDelimit = new char[]
		{
			'.'
		};

		public static readonly char[] BreakerRange = new char[]
		{
			'0',
			'9',
			'A',
			'Z',
			'a',
			'﻿',
			'Ａ',
			'⿿',
			'぀',
			'ｚ',
			'ｦ',
			'‐',
			'⷟'
		};

		public static readonly char[] UrlStart = new char[]
		{
			'h',
			't',
			't',
			'p',
			's',
			':',
			'/',
			'/'
		};

		public static readonly char[] UrlEnd = new char[]
		{
			' ',
			'>'
		};

		public static readonly char[] DomanEnd = new char[]
		{
			'/',
			':',
			' ',
			'>'
		};
	}
}
