using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal static class TextConvertersDefaults
	{
		public static int MinDecodeBytes(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 64;
			}
			return 1;
		}

		public static int InitialTokenRuns(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 64;
			}
			return 7;
		}

		public static int MaxTokenRuns(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 512;
			}
			return 16;
		}

		public static int InitialTokenBufferSize(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 1024;
			}
			return 32;
		}

		public static int MaxTokenSize(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 4096;
			}
			return 128;
		}

		public static int InitialHtmlAttributes(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 8;
			}
			return 1;
		}

		public static int MaxHtmlAttributes(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 128;
			}
			return 5;
		}

		public static int MaxHtmlNormalizerNesting(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 4096;
			}
			return 10;
		}

		public static int MaxHtmlMetaRestartOffset(bool boundaryTest)
		{
			if (!boundaryTest)
			{
				return 4096;
			}
			return 4096;
		}

		private const int NormalMinDecodeBytes = 64;

		private const int NormalInitialTokenRuns = 64;

		private const int NormalMaxTokenRuns = 512;

		private const int NormalInitialTokenBufferSize = 1024;

		private const int NormalMaxTokenSize = 4096;

		private const int NormalInitialHtmlAttributes = 8;

		private const int NormalMaxHtmlAttributes = 128;

		private const int NormalMaxHtmlNormalizerNesting = 4096;

		private const int NormalMaxHtmlMetaRestartOffset = 4096;

		private const int BoundaryMinDecodeBytes = 1;

		private const int BoundaryInitialTokenRuns = 7;

		private const int BoundaryMaxTokenRuns = 16;

		private const int BoundaryInitialTokenBufferSize = 32;

		private const int BoundaryMaxTokenSize = 128;

		private const int BoundaryInitialHtmlAttributes = 1;

		private const int BoundaryMaxHtmlAttributes = 5;

		private const int BoundaryMaxHtmlNormalizerNesting = 10;

		private const int BoundaryMaxHtmlMetaRestartOffset = 4096;
	}
}
