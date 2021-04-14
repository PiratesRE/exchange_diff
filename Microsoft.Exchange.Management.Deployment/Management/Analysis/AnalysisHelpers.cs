using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Management.Analysis.Features;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Management.Analysis
{
	internal static class AnalysisHelpers
	{
		[DllImport("Advapi32.dll", SetLastError = true)]
		public static extern bool GetSecurityDescriptorControl(IntPtr pSecurityDescriptor, out ValidationConstant.SecurityDescriptorControl sdcontrol, out int dwRevision);

		public static int VersionCompare(string first, string second)
		{
			Version version = new Version(first);
			Version value = new Version(second);
			return version.CompareTo(value);
		}

		public static string[] Replace(string[] arrayToSearch, string pattern, string replacement)
		{
			if (arrayToSearch == null || arrayToSearch.Length == 0)
			{
				return arrayToSearch;
			}
			List<string> list = new List<string>();
			foreach (string val in arrayToSearch)
			{
				string text = AnalysisHelpers.Replace(val, pattern, replacement);
				if (text != null)
				{
					list.Add(text);
				}
			}
			return list.ToArray();
		}

		public static string Replace(string val, string pattern, string replacement)
		{
			if (string.IsNullOrEmpty(val))
			{
				return val;
			}
			if (string.IsNullOrEmpty(pattern))
			{
				return val;
			}
			if (string.IsNullOrEmpty(replacement))
			{
				return val;
			}
			return Regex.Replace(val, pattern, replacement, AnalysisHelpers.matchOptions);
		}

		public static string ConvertBinaryToString(object val)
		{
			string text = string.Empty;
			if (AnalysisHelpers.IsNullOrEmpty(val))
			{
				return text;
			}
			Type type = val.GetType();
			StringBuilder stringBuilder = new StringBuilder();
			if (type == typeof(string))
			{
				val = ((string)val).ToCharArray();
				text = string.Empty;
				foreach (char c in (char[])val)
				{
					string str = text;
					int num = (int)c;
					text = str + num.ToString("X4");
				}
			}
			else
			{
				if (!(type == typeof(byte[])))
				{
					throw new ArgumentException("val must be of type string or byte[].");
				}
				stringBuilder.Capacity = 2 * ((byte[])val).Length + 1;
				foreach (byte b in (byte[])val)
				{
					stringBuilder.Append(b.ToString("X2"));
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static string ConvertToStringSid(object val)
		{
			if (AnalysisHelpers.IsNullOrEmpty(val))
			{
				return string.Empty;
			}
			if (val.GetType() == typeof(byte[]))
			{
				return AnalysisHelpers.ConvertToStringSid((byte[])val);
			}
			return val.ToString();
		}

		public static string ConvertToStringSid(byte[] sid)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			GCHandle gchandle = GCHandle.Alloc(sid, GCHandleType.Pinned);
			IntPtr sid2 = gchandle.AddrOfPinnedObject();
			string empty = string.Empty;
			try
			{
				NativeMethodProvider.ConvertSidToStringSid(sid2, ref empty);
			}
			finally
			{
				gchandle.Free();
			}
			return empty;
		}

		public static bool IsNullOrEmpty(object obj)
		{
			bool result = false;
			if (obj == null)
			{
				result = true;
			}
			else if (obj.GetType().IsArray)
			{
				if (((Array)obj).Length == 0)
				{
					result = true;
				}
			}
			else if (obj is string && string.IsNullOrEmpty((string)obj))
			{
				result = true;
			}
			return result;
		}

		public static object GetObjectPropertyByName(object val, string propName)
		{
			PropertyInfo property = val.GetType().GetProperty(propName);
			if (property == null)
			{
				throw new ArgumentException(propName);
			}
			return property.GetValue(val, null);
		}

		public static bool Exist(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentException("filePath should not be null or empty");
			}
			return File.Exists(filePath) || Directory.Exists(filePath);
		}

		public static bool Match(string pattern, params string[] strings)
		{
			if (string.IsNullOrEmpty(pattern))
			{
				throw new ArgumentException("Argument 'pattern' should not be null or empty.");
			}
			if (!AnalysisHelpers.IsNullOrEmpty(strings))
			{
				foreach (string text in strings)
				{
					if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, pattern, AnalysisHelpers.matchOptions))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static int SdGet(byte[] sd)
		{
			int result = 0;
			GCHandle gchandle = GCHandle.Alloc(sd, GCHandleType.Pinned);
			IntPtr pSecurityDescriptor = gchandle.AddrOfPinnedObject();
			try
			{
				ValidationConstant.SecurityDescriptorControl securityDescriptorControl;
				int num;
				AnalysisHelpers.GetSecurityDescriptorControl(pSecurityDescriptor, out securityDescriptorControl, out num);
				result = (int)securityDescriptorControl;
			}
			finally
			{
				gchandle.Free();
			}
			return result;
		}

		private static RegexOptions matchOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
	}
}
