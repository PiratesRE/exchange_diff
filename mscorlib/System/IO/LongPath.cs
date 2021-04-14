using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.IO
{
	[ComVisible(false)]
	internal static class LongPath
	{
		[SecurityCritical]
		internal static string NormalizePath(string path)
		{
			return LongPath.NormalizePath(path, true);
		}

		[SecurityCritical]
		internal static string NormalizePath(string path, bool fullCheck)
		{
			return Path.NormalizePath(path, fullCheck, 32767);
		}

		internal static string InternalCombine(string path1, string path2)
		{
			bool flag;
			string path3 = LongPath.TryRemoveLongPathPrefix(path1, out flag);
			string text = Path.InternalCombine(path3, path2);
			if (flag)
			{
				text = Path.AddLongPathPrefix(text);
			}
			return text;
		}

		internal static int GetRootLength(string path)
		{
			bool flag;
			string path2 = LongPath.TryRemoveLongPathPrefix(path, out flag);
			int num = Path.GetRootLength(path2);
			if (flag)
			{
				num += 4;
			}
			return num;
		}

		internal static bool IsPathRooted(string path)
		{
			string path2 = Path.RemoveLongPathPrefix(path);
			return Path.IsPathRooted(path2);
		}

		[SecurityCritical]
		internal static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			bool flag;
			string path2 = LongPath.TryRemoveLongPathPrefix(path, out flag);
			path2 = LongPath.NormalizePath(path2, false);
			string text = path.Substring(0, LongPath.GetRootLength(path2));
			if (flag)
			{
				text = Path.AddLongPathPrefix(text);
			}
			return text;
		}

		[SecurityCritical]
		internal static string GetDirectoryName(string path)
		{
			if (path != null)
			{
				bool flag;
				string text = LongPath.TryRemoveLongPathPrefix(path, out flag);
				Path.CheckInvalidPathChars(text, false);
				path = LongPath.NormalizePath(text, false);
				int rootLength = LongPath.GetRootLength(text);
				int num = text.Length;
				if (num > rootLength)
				{
					num = text.Length;
					if (num == rootLength)
					{
						return null;
					}
					while (num > rootLength && text[--num] != Path.DirectorySeparatorChar && text[num] != Path.AltDirectorySeparatorChar)
					{
					}
					string text2 = text.Substring(0, num);
					if (flag)
					{
						text2 = Path.AddLongPathPrefix(text2);
					}
					return text2;
				}
			}
			return null;
		}

		internal static string TryRemoveLongPathPrefix(string path, out bool removed)
		{
			removed = Path.HasLongPathPrefix(path);
			if (!removed)
			{
				return path;
			}
			return Path.RemoveLongPathPrefix(path);
		}
	}
}
