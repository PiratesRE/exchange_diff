using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.MessageSecurity.MessageClassifications
{
	internal static class ClassificationUtils
	{
		public static List<string> ExtractClassifications(HeaderList headers)
		{
			Header[] array = headers.FindAll("X-MS-Exchange-Organization-Classification");
			List<string> list = new List<string>(array.Length);
			foreach (Header header in array)
			{
				string text = ClassificationUtils.ExtractId(header.Value);
				if (text.Length > 0)
				{
					list.Add(text);
				}
			}
			return list;
		}

		public static void PromoteStoreClassifications(HeaderList headers)
		{
			foreach (Header header in headers.FindAll("x-microsoft-classID"))
			{
				string value = ClassificationUtils.ExtractId(header.Value);
				if (!string.IsNullOrEmpty(value))
				{
					headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Classification", value));
				}
			}
			ClassificationUtils.DropStoreLabels(headers);
		}

		public static void DropStoreLabels(HeaderList headers)
		{
			headers.RemoveAll("x-microsoft-classified");
			headers.RemoveAll("x-microsoft-classification");
			headers.RemoveAll("x-microsoft-classDesc");
			headers.RemoveAll("x-microsoft-classID");
			headers.RemoveAll("X-microsoft-classKeep");
		}

		public static void PromoteIfUnclassified(HeaderList headers, string classificationGuidText)
		{
			string text = ClassificationUtils.ExtractId(classificationGuidText);
			if (text.Length == 0)
			{
				return;
			}
			if (headers.FindFirst("X-MS-Exchange-Organization-Classification") != null)
			{
				return;
			}
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Classification", text));
		}

		private static string ExtractId(string classificationGuidText)
		{
			int num = classificationGuidText.IndexOf(';');
			string text = (num < 0) ? classificationGuidText : classificationGuidText.Substring(0, num);
			return text.Trim(ClassificationUtils.LabelTrimCharacters);
		}

		private static readonly char[] LabelTrimCharacters = new char[]
		{
			'{',
			'}',
			' '
		};
	}
}
