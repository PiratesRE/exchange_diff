using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal static class Utils
	{
		internal static byte[] DuplicateByteArray(byte[] src)
		{
			if (src == null || src.Length <= 0)
			{
				return null;
			}
			byte[] array = new byte[src.Length];
			Buffer.BlockCopy(src, 0, array, 0, src.Length);
			return array;
		}

		internal static string[] JsonStringToStringArray(string jsonString)
		{
			if (jsonString == null)
			{
				return null;
			}
			return new JavaScriptSerializer
			{
				MaxJsonLength = 1073741824
			}.Deserialize<string[]>(jsonString);
		}

		internal static string StringArrayToJsonString(IEnumerable<string> strings)
		{
			return new JavaScriptSerializer
			{
				MaxJsonLength = 1073741824
			}.Serialize(strings);
		}
	}
}
