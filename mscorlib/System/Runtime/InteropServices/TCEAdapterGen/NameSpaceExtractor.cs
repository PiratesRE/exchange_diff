using System;

namespace System.Runtime.InteropServices.TCEAdapterGen
{
	internal static class NameSpaceExtractor
	{
		public static string ExtractNameSpace(string FullyQualifiedTypeName)
		{
			int num = FullyQualifiedTypeName.LastIndexOf(NameSpaceExtractor.NameSpaceSeperator);
			if (num == -1)
			{
				return "";
			}
			return FullyQualifiedTypeName.Substring(0, num);
		}

		private static char NameSpaceSeperator = '.';
	}
}
