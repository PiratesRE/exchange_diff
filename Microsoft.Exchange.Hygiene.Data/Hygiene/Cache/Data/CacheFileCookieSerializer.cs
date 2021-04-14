using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal class CacheFileCookieSerializer
	{
		internal static LinkedList<CacheFileCookie> ReadCookieList(FileStream fs)
		{
			return CacheFileCookieSerializer.ReadCookieList(fs, -1L, -1);
		}

		internal static CacheFileCookie ReadCookie(FileStream fs, long offset)
		{
			LinkedList<CacheFileCookie> linkedList = CacheFileCookieSerializer.ReadCookieList(fs, offset, 1);
			if (linkedList.First == null)
			{
				return null;
			}
			return linkedList.First.Value;
		}

		internal static CacheFileCookie ReadLastCookie(FileStream fs)
		{
			LinkedList<CacheFileCookie> linkedList = CacheFileCookieSerializer.ReadCookieList(fs, -1L, -1);
			if (linkedList.Last == null)
			{
				return null;
			}
			return linkedList.Last.Value;
		}

		internal static LinkedList<CacheFileCookie> ReadCookieList(FileStream fs, long offset, int pageSize)
		{
			LinkedList<CacheFileCookie> linkedList = new LinkedList<CacheFileCookie>();
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			CacheFileCookieSerializer.SetOffsetAndPageSize(fs, binaryFormatter, offset, ref pageSize);
			for (int i = 0; i < pageSize; i++)
			{
				long position = fs.Position;
				if (position == fs.Length)
				{
					break;
				}
				CacheFileCookie cacheFileCookie = (CacheFileCookie)binaryFormatter.Deserialize(fs);
				cacheFileCookie.NextCookieOffset = fs.Position;
				linkedList.AddLast(cacheFileCookie);
			}
			return linkedList;
		}

		internal static void ReadNextCookies(FileStream fs, long offset, int pageSize, LinkedList<CacheFileCookie> cookieList, int capacity)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			CacheFileCookieSerializer.SetOffsetAndPageSize(fs, binaryFormatter, offset, ref pageSize);
			for (int i = 0; i < pageSize; i++)
			{
				long position = fs.Position;
				if (position == fs.Length)
				{
					return;
				}
				CacheFileCookie cacheFileCookie = (CacheFileCookie)binaryFormatter.Deserialize(fs);
				cacheFileCookie.NextCookieOffset = fs.Position;
				cookieList.AddLast(cacheFileCookie);
				if (cookieList.Count > capacity)
				{
					cookieList.RemoveFirst();
				}
			}
		}

		internal static void ReadPreCookies(FileStream fs, long offset, int pageSize, LinkedList<CacheFileCookie> cookieList, int capacity)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			CacheFileCookieSerializer.SetOffsetAndPageSize(fs, binaryFormatter, offset, ref pageSize);
			for (int i = 0; i < pageSize; i++)
			{
				long position = fs.Position;
				if (position == fs.Length)
				{
					return;
				}
				CacheFileCookie cacheFileCookie = (CacheFileCookie)binaryFormatter.Deserialize(fs);
				cacheFileCookie.NextCookieOffset = fs.Position;
				cookieList.AddFirst(cacheFileCookie);
				if (cookieList.Count > capacity)
				{
					cookieList.RemoveLast();
				}
				if (cacheFileCookie.PreCookieOffset < 0L)
				{
					return;
				}
				fs.Seek(cacheFileCookie.PreCookieOffset, SeekOrigin.Begin);
			}
		}

		internal static void WriteCookieToFile(FileStream cookieFileStream, CacheFileCookie cookie, CacheFileCookie preCookie)
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			if (cookieFileStream.Length == 0L)
			{
				binaryFormatter.Serialize(cookieFileStream, 0);
			}
			cookie.PreCookieOffset = preCookie.CookieOffset;
			cookie.CookieOffset = cookieFileStream.Position;
			binaryFormatter.Serialize(cookieFileStream, cookie);
			cookieFileStream.SetLength(cookieFileStream.Position);
		}

		internal static void StampCookieCount(FileStream cookieFileStream, int count)
		{
			cookieFileStream.Seek(0L, SeekOrigin.Begin);
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			binaryFormatter.Serialize(cookieFileStream, count);
		}

		internal static void SetOffsetAndPageSize(FileStream fs, BinaryFormatter formatter, long offset, ref int pageSize)
		{
			if (offset < 0L)
			{
				fs.Seek(0L, SeekOrigin.Begin);
				int num = (int)formatter.Deserialize(fs);
				if (pageSize < 0)
				{
					pageSize = num;
					return;
				}
			}
			else
			{
				fs.Seek(offset, SeekOrigin.Begin);
			}
		}
	}
}
