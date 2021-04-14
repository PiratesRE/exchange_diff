using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Util
	{
		internal static T[] CollectionToArray<T>(ICollection<T> collection)
		{
			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		internal static R[] CollectionToArray<T, R>(ICollection<T> collection) where T : R
		{
			R[] array = new R[collection.Count];
			int num = 0;
			foreach (T t in collection)
			{
				array[num++] = (R)((object)t);
			}
			return array;
		}

		internal static IEnumerable<T> CompositeEnumerator<T>(params IEnumerable<T>[] nestedEnumerators)
		{
			foreach (IEnumerable<T> nestedEnumerator in nestedEnumerators)
			{
				if (nestedEnumerator != null)
				{
					foreach (T value in nestedEnumerator)
					{
						yield return value;
					}
				}
			}
			yield break;
		}

		internal static K[] GetPairListKeys<K, V>(IList<KeyValuePair<K, V>> pairs)
		{
			K[] array = new K[pairs.Count];
			for (int i = 0; i < pairs.Count; i++)
			{
				array[i] = pairs[i].Key;
			}
			return array;
		}

		internal static V[] GetPairListValues<K, V>(IList<KeyValuePair<K, V>> pairs)
		{
			V[] array = new V[pairs.Count];
			for (int i = 0; i < pairs.Count; i++)
			{
				array[i] = pairs[i].Value;
			}
			return array;
		}

		internal static L AddElements<L, E>(L collection, params E[] values) where L : ICollection<E>
		{
			foreach (E item in values)
			{
				collection.Add(item);
			}
			return collection;
		}

		internal static void AddRange<C, E>(ICollection<C> collection, IEnumerable<E> values) where E : C
		{
			foreach (E e in values)
			{
				collection.Add((C)((object)e));
			}
		}

		internal static T[] MergeArrays<T>(params ICollection<T>[] collections)
		{
			int num = 0;
			foreach (ICollection<T> collection in collections)
			{
				if (collection != null)
				{
					num += collection.Count;
				}
			}
			T[] array = new T[num];
			int num2 = 0;
			foreach (ICollection<T> collection2 in collections)
			{
				if (collection2 != null)
				{
					collection2.CopyTo(array, num2);
					num2 += collection2.Count;
				}
			}
			return array;
		}

		internal static T[] RemoveArrayElements<T>(T[] array, params int[] indicies)
		{
			int num = 0;
			int num2 = -1;
			T[] array2 = new T[array.Length - indicies.Length];
			for (int i = 0; i < indicies.Length; i++)
			{
				int num3 = indicies[i];
				if (num3 <= num2)
				{
					throw new ArgumentException("Should be sorted", "indicies");
				}
				if (num < num3)
				{
					Array.Copy(array, num, array2, num - i, num3 - num);
				}
				num = num3 + 1;
			}
			if (num < array.Length)
			{
				Array.Copy(array, num, array2, num - indicies.Length, array.Length - num);
			}
			return array2;
		}

		internal static KeyValuePair<K, V> Pair<K, V>(K key, V value)
		{
			return new KeyValuePair<K, V>(key, value);
		}

		internal static bool Contains(IList<DefaultFolderType> list, DefaultFolderType item)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == item)
				{
					return true;
				}
			}
			return false;
		}

		internal static byte[] ReadByteArray(Stream stm)
		{
			int i = checked((int)(stm.Length - stm.Position));
			byte[] array = new byte[i];
			int num = 0;
			while (i > 0)
			{
				int num2 = stm.Read(array, num, i);
				if (num2 == 0)
				{
					throw new EndOfStreamException();
				}
				num += num2;
				i -= num2;
			}
			return array;
		}

		internal static string SubstringAfterPrefix(string source, string prefix, StringComparison comparison)
		{
			if (!source.StartsWith(prefix, comparison))
			{
				return null;
			}
			int length = prefix.Length;
			if (length < source.Length)
			{
				return source.Substring(length);
			}
			return string.Empty;
		}

		internal static string SubstringBetween(string source, string leftDelimiter, string rightDelimiter, SubstringOptions options)
		{
			return Util.SubstringBetween(source, leftDelimiter, rightDelimiter, ((options & SubstringOptions.Backward) == SubstringOptions.Backward) ? Math.Max(0, source.Length - 1) : 0, options);
		}

		internal static string SubstringBetween(string source, string leftDelimiter, string rightDelimiter, int startingIndex, SubstringOptions options)
		{
			bool flag = (options & SubstringOptions.Backward) == SubstringOptions.Backward;
			int num = (leftDelimiter != null) ? (flag ? source.LastIndexOf(leftDelimiter, startingIndex) : source.IndexOf(leftDelimiter, startingIndex)) : 0;
			if (num == -1)
			{
				if ((options & SubstringOptions.IgnoreMissingLeftDelimiter) == SubstringOptions.None)
				{
					return null;
				}
				num = 0;
			}
			else if (leftDelimiter != null)
			{
				num += leftDelimiter.Length;
			}
			if (num < source.Length)
			{
				int num2 = (rightDelimiter != null) ? (flag ? source.LastIndexOf(rightDelimiter, num) : source.IndexOf(rightDelimiter, num)) : source.Length;
				if (num2 == -1)
				{
					if ((options & SubstringOptions.IgnoreMissingRightDelimiter) == SubstringOptions.None)
					{
						return null;
					}
					num2 = source.Length;
				}
				return source.Substring(num, num2 - num);
			}
			if (rightDelimiter != null && (options & SubstringOptions.IgnoreMissingRightDelimiter) == SubstringOptions.None)
			{
				return null;
			}
			return string.Empty;
		}

		internal static int GetRegistryValueOrDefault(string registryKeyName, string registryValueName, int defaultValue, Trace tracer)
		{
			Util.ThrowOnNullOrEmptyArgument(registryKeyName, "registryKeyName");
			Util.ThrowOnNullOrEmptyArgument(registryValueName, "registryValueName");
			Exception ex = null;
			int result = defaultValue;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryKeyName))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(registryValueName, defaultValue);
						if (value != null && value is int)
						{
							result = (int)value;
						}
						else if (tracer != null)
						{
							tracer.TraceDebug<string, string, int>(0L, "Util::GetRegistryValueOrDefault. Failed to read integer value - {0}\\{1}. Using default value - {2}", registryKeyName, registryValueName, defaultValue);
						}
					}
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null && tracer != null)
			{
				tracer.TraceError<string, string, Exception>(0L, "Util::GetRegistryValueOrDefault. Failed to read registry entry - {0}\\{1}. Exception - {2}", registryKeyName, registryValueName, ex);
			}
			return result;
		}

		internal static T NullIf<T>(T value, T returnNullIfEquals) where T : class
		{
			if (!object.Equals(value, returnNullIfEquals))
			{
				return value;
			}
			return default(T);
		}

		internal static bool TryConvertTo<T>(object value, out T result)
		{
			bool flag = value is T;
			result = (flag ? ((T)((object)value)) : default(T));
			return flag;
		}

		internal static bool ValueEquals(object v1, object v2)
		{
			if (v1 is Enum || v2 is Enum)
			{
				return Util.EnumEquals(v1, v2);
			}
			if (object.Equals(v1, v2))
			{
				return true;
			}
			byte[] array = v1 as byte[];
			byte[] array2 = v2 as byte[];
			if (array != null && array2 != null)
			{
				return Util.CompareByteArray(array, array2);
			}
			IList list = v1 as IList;
			IList list2 = v2 as IList;
			if (list != null && list2 != null)
			{
				bool flag = list2.Count == list.Count;
				int num = 0;
				while (flag && num < list.Count)
				{
					flag = Util.ValueEquals(list[num], list2[num]);
					num++;
				}
				return flag;
			}
			return false;
		}

		internal static int CompareValues(object left, object right)
		{
			Type left2 = (left != null) ? left.GetType() : null;
			Type type = (right != null) ? right.GetType() : null;
			if (left2 == typeof(PropertyError))
			{
				if (type == typeof(PropertyError))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (type == typeof(PropertyError))
				{
					return 1;
				}
				if (left == null)
				{
					if (right != null)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (right == null)
					{
						return 1;
					}
					if (!(left2 == type))
					{
						throw new ArgumentException("left & right not of same type");
					}
					IComparable comparable = left as IComparable;
					if (comparable != null)
					{
						return comparable.CompareTo(right);
					}
					if (left is IList)
					{
						IList list = left as IList;
						IList list2 = right as IList;
						int num = 0;
						while (num < list.Count && num < list2.Count)
						{
							int num2 = Util.CompareValues(list[num], list2[num]);
							if (num2 != 0)
							{
								return num2;
							}
							num++;
						}
						return list.Count - list2.Count;
					}
					throw new ArgumentException("arguments not IComparable");
				}
			}
		}

		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static void ThrowOnNullOrEmptyArgument(string argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (argument.Length == 0)
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static void ThrowOnNullOrEmptyArgument(IEnumerable argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			if (!argument.GetEnumerator().MoveNext())
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static void ThrowOnMismatchType<T>(PropertyDefinition propertyDefinition, string argName)
		{
			if (!propertyDefinition.Type.Equals(typeof(T)))
			{
				throw new ArgumentException("Type mismatch for propertyDefinition", argName);
			}
		}

		internal static void ThrowOnArgumentOutOfRangeOnLessThan(int argument, int min, string argumentName)
		{
			if (argument < min)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		internal static void ThrowOnArgumentOutOfRangeOnLessThan(long argument, long min, string argumentName)
		{
			if (argument < min)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		internal static void ThrowOnArgumentInvalidOnGreaterThan(int argument, int max, string argumentName)
		{
			if (argument > max)
			{
				throw new ArgumentException(argumentName);
			}
		}

		internal static void DisposeIfPresent(IDisposable disposable)
		{
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		internal static bool IsSpecialLcid(int lcid)
		{
			return lcid == 0 || lcid == 1024 || lcid == 2048;
		}

		internal static int ConvertDateTimeToRTime(ExDateTime dateTime)
		{
			return Convert.ToInt32(((DateTime)dateTime - (DateTime)Util.Date1601).TotalMinutes);
		}

		internal static bool TryConvertRTimeToDateTime(int rTime, out ExDateTime retVal)
		{
			if (rTime >= 0 && rTime <= 1525252320)
			{
				retVal = (ExDateTime)((DateTime)Util.Date1601).Add(TimeSpan.FromMinutes((double)rTime));
				return true;
			}
			retVal = ExDateTime.MinValue;
			return false;
		}

		public static int ConvertTimeSpanToSCDTime(TimeSpan timeSpan)
		{
			return (timeSpan.Hours << 12) + (timeSpan.Minutes << 6) + timeSpan.Seconds;
		}

		public static TimeSpan ConvertSCDTimeToTimeSpan(int scdTime)
		{
			return new TimeSpan(scdTime >> 12 & 31, scdTime >> 6 & 63, scdTime & 63);
		}

		public static int ConvertDateTimeToSCDDate(ExDateTime date)
		{
			return (date.Year << 9) + (date.Month << 5) + date.Day;
		}

		public static ExDateTime ConvertSCDDateToDateTime(int scdDate)
		{
			return new ExDateTime(ExTimeZone.UnspecifiedTimeZone, scdDate >> 9, scdDate >> 5 & 15, scdDate & 31, 0, 0, 0);
		}

		public static ExDateTime ChopSecondsPart(ExDateTime dateTime)
		{
			return dateTime.Date + TimeSpan.FromMinutes((double)Convert.ToInt32(dateTime.TimeOfDay.TotalMinutes));
		}

		private static bool EnumEquals(object a, object b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (a is Enum)
			{
				a = (int)a;
			}
			if (b is Enum)
			{
				b = (int)b;
			}
			return a.Equals(b);
		}

		public static bool IsAttachmentSeparator(char ch)
		{
			return ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar || ch == Path.VolumeSeparatorChar;
		}

		public static void GetRandomBytes(byte[] bytes)
		{
			Util.randomNumberGenerator.GetBytes(bytes);
		}

		public static string NormalizePhoneNumber(string phoneNumber)
		{
			return DtmfString.DtmfEncode(phoneNumber);
		}

		public static byte[] GetBytesFromBuffer(MemoryStream stream)
		{
			return stream.GetBuffer();
		}

		public static string GetMachineName()
		{
			return Environment.MachineName;
		}

		public static IComparable GetClassComparable(object v)
		{
			return v.GetType().TypeHandle.Value.ToInt64();
		}

		public static XmlWriter GetXmlTextWriter(Stream destination, XmlWriterSettings writerSettings)
		{
			return XmlWriter.Create(destination, writerSettings);
		}

		public static string GetAssemblyLocation()
		{
			return Assembly.GetExecutingAssembly().Location;
		}

		public static string InternString(string stringToIntern)
		{
			return string.Intern(stringToIntern);
		}

		[DllImport("ntdll.dll", ExactSpelling = true)]
		internal static extern int RtlCompareMemory(IntPtr source1, IntPtr source2, int size);

		internal static bool CompareByteArray(byte[] ba1, byte[] ba2)
		{
			if (ba1 == ba2)
			{
				return true;
			}
			if (ba1 == null || ba2 == null)
			{
				return false;
			}
			if (ba1.Length != ba2.Length)
			{
				return false;
			}
			GCHandle gchandle = default(GCHandle);
			GCHandle gchandle2 = default(GCHandle);
			bool result;
			try
			{
				gchandle = GCHandle.Alloc(ba1, GCHandleType.Pinned);
				gchandle2 = GCHandle.Alloc(ba2, GCHandleType.Pinned);
				result = (ba1.Length == Util.RtlCompareMemory(gchandle.AddrOfPinnedObject(), gchandle2.AddrOfPinnedObject(), ba1.Length));
			}
			finally
			{
				if (gchandle2.IsAllocated)
				{
					gchandle2.Free();
				}
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		internal static bool CompareByteArraySegments(byte[] byteArray1, uint byteArray1Offset, byte[] byteArray2, uint byteArray2Offset, uint length)
		{
			ArgumentValidator.ThrowIfNull("byteArray1", byteArray1);
			ArgumentValidator.ThrowIfNull("byteArray2", byteArray2);
			if (length == 0U)
			{
				return false;
			}
			if ((ulong)byteArray1Offset >= (ulong)((long)byteArray1.Length) || (ulong)byteArray2Offset >= (ulong)((long)byteArray2.Length) || (ulong)(byteArray1Offset + length) > (ulong)((long)byteArray1.Length) || (ulong)(byteArray2Offset + length) > (ulong)((long)byteArray2.Length))
			{
				throw new ArgumentOutOfRangeException("byteOffsetOrLength", "Specified length and/or offsets are outside of the acceptable range.");
			}
			int num = 0;
			while ((long)num < (long)((ulong)length))
			{
				if (checked(byteArray1[(int)((IntPtr)(unchecked((ulong)byteArray1Offset + (ulong)((long)num))))] != byteArray2[(int)((IntPtr)(unchecked((ulong)byteArray2Offset + (ulong)((long)num))))]))
				{
					return false;
				}
				num++;
			}
			return true;
		}

		internal const int RtmNone = 1525252320;

		internal const int RtmMax = 1525252319;

		internal const int RtmMin = 0;

		internal const int DefaultYearWhenYearMissing = 1604;

		internal static readonly TimeSpan DateTimeComparisonRange = TimeSpan.FromSeconds(1.0);

		internal static readonly ExDateTime Date1601 = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 1, 1, 0, 0, 0);

		internal static readonly ExDateTime Date1601Utc = new ExDateTime(ExTimeZone.UtcTimeZone, 1601, 1, 1, 0, 0, 0);

		internal static readonly Util.PreferedCultureSelector CultureSelector = Util.PreferedCultureSelector.GetPreferedCultureSelector(ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Storage.ClientStrings", typeof(ClientStrings).GetTypeInfo().Assembly));

		private static RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();

		internal static class StreamHandler
		{
			internal static long CopyStreamData(Stream readStream, Stream writeStream)
			{
				return Util.StreamHandler.CopyStreamData(readStream, writeStream, null);
			}

			internal static long CopyStreamData(Stream readStream, Stream writeStream, int? numBytes)
			{
				return Util.StreamHandler.CopyStreamData(readStream, writeStream, numBytes, 0);
			}

			internal static long CopyStreamData(Stream readStream, Stream writeStream, int? numBytes, int trailingNulls)
			{
				return Util.StreamHandler.CopyStreamData(readStream, writeStream, numBytes, trailingNulls, 4096);
			}

			internal static long CopyStreamData(Stream readStream, Stream writeStream, int? numBytes, int trailingNulls, int bufferSize)
			{
				int size = Math.Min(numBytes ?? bufferSize, bufferSize);
				BufferPoolCollection.BufferSize bufferSize2;
				BufferPoolCollection.AutoCleanupCollection.TryMatchBufferSize(size, out bufferSize2);
				BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(bufferSize2);
				byte[] array = null;
				long result = 0L;
				try
				{
					array = bufferPool.Acquire();
					result = Util.StreamHandler.CopyStreamData(readStream, writeStream, numBytes, trailingNulls, array);
				}
				finally
				{
					if (array != null)
					{
						bufferPool.Release(array);
					}
				}
				return result;
			}

			internal static long CopyStreamData(Stream readStream, Stream writeStream, int? numBytes, int trailingNulls, byte[] data)
			{
				if (trailingNulls > data.Length)
				{
					throw new ArgumentOutOfRangeException("trailingNulls");
				}
				if (writeStream == null)
				{
					throw new ArgumentNullException("writeStream");
				}
				if (readStream == null)
				{
					return 0L;
				}
				int num = 0;
				long num2 = 0L;
				while (numBytes == null || numBytes > 0)
				{
					int num3 = data.Length - num;
					if (numBytes != null && numBytes < num3)
					{
						num3 = numBytes.Value;
					}
					int num4 = readStream.Read(data, num, num3);
					if (num4 == 0)
					{
						if (num < trailingNulls)
						{
							ExTraceGlobals.StorageTracer.Information<int, int>(0L, "Util.StreamHandler.CopyStreamData: expected {0} trailing nulls, found only {1}", trailingNulls, num);
							if (num != 0)
							{
								writeStream.Write(data, 0, num);
							}
						}
						return num2;
					}
					num2 += (long)num4;
					if (numBytes != null)
					{
						numBytes -= num4;
					}
					int num5 = num + num4;
					num = 0;
					while (num < trailingNulls && num5 != 0 && data[num5 - 1] == 0)
					{
						num++;
						num5--;
					}
					if (num5 != 0)
					{
						writeStream.Write(data, 0, num5);
					}
					if (num != 0)
					{
						for (int num6 = 0; num6 != num; num6++)
						{
							data[num6] = 0;
						}
					}
				}
				return num2;
			}

			public static int ReadLastBytesOfStream(Stream readStream, int lastNBytesToRead, out byte[] bytes)
			{
				if (!readStream.CanSeek)
				{
					bytes = null;
					return 0;
				}
				long num = Math.Max(0L, readStream.Length - (long)lastNBytesToRead);
				readStream.Position = num;
				long num2 = (num == 0L) ? readStream.Length : ((long)lastNBytesToRead);
				bytes = new byte[num2];
				return readStream.Read(bytes, 0, bytes.Length);
			}

			public static char[] ReadCharBuffer(TextReader reader, int size)
			{
				char[] array = new char[size];
				int num;
				int num2;
				for (num = 0; num != size; num += num2)
				{
					num2 = reader.Read(array, num, size - num);
					if (num2 == 0)
					{
						break;
					}
				}
				if (num == size)
				{
					return array;
				}
				char[] array2 = new char[num];
				Array.Copy(array, array2, num);
				return array2;
			}

			public static void CopyText(TextReader reader, TextWriter writer)
			{
				char[] array = new char[8192];
				for (;;)
				{
					int num = reader.Read(array, 0, array.Length);
					if (num == 0)
					{
						break;
					}
					writer.Write(array, 0, num);
				}
			}

			public static byte[] ReadBytesFromStream(Stream stream)
			{
				byte[] buffer;
				using (MemoryStream memoryStream = new MemoryStream(4096))
				{
					Util.StreamHandler.CopyStreamData(stream, memoryStream);
					buffer = memoryStream.GetBuffer();
				}
				return buffer;
			}

			// Note: this type is marked as 'beforefieldinit'.
			static StreamHandler()
			{
				byte[] array = new byte[2];
				Util.StreamHandler.zeroByteBuffer = array;
			}

			private static readonly byte[] zeroByteBuffer;
		}

		internal class PreferedCultureSelector
		{
			public static Util.PreferedCultureSelector GetPreferedCultureSelector(ExchangeResourceManager resourceManager)
			{
				Util.PreferedCultureSelector result;
				lock (Util.PreferedCultureSelector.preferedCultureSelectorCache)
				{
					Util.PreferedCultureSelector preferedCultureSelector;
					if (!Util.PreferedCultureSelector.preferedCultureSelectorCache.TryGetValue(resourceManager, out preferedCultureSelector))
					{
						preferedCultureSelector = new Util.PreferedCultureSelector(resourceManager);
						Util.PreferedCultureSelector.preferedCultureSelectorCache[resourceManager] = preferedCultureSelector;
					}
					result = preferedCultureSelector;
				}
				return result;
			}

			private PreferedCultureSelector(ExchangeResourceManager resourceManager)
			{
				this.resourceManager = resourceManager;
			}

			private void AddCultureMapping(CultureInfo culture, bool isResourcePresent)
			{
				Dictionary<int, bool> dictionary = new Dictionary<int, bool>(this.resourceAvailabilityMap);
				dictionary[LocaleMap.GetLcidFromCulture(culture)] = isResourcePresent;
				this.resourceAvailabilityMap = dictionary;
			}

			public static bool TestHookAllResourcesExist
			{
				get
				{
					return Util.PreferedCultureSelector.testHookAllResourcesExist;
				}
				set
				{
					Util.PreferedCultureSelector.testHookAllResourcesExist = value;
				}
			}

			public CultureInfo GetPreferedCulture(IList<CultureInfo> cultures)
			{
				if (cultures == null)
				{
					throw new ArgumentNullException("cultures");
				}
				for (int i = 0; i < cultures.Count; i++)
				{
					bool flag;
					if (!this.resourceAvailabilityMap.TryGetValue(cultures[i].LCID, out flag))
					{
						CultureInfo cultureInfo = cultures[i];
						if (ClientLanguageConstraint.IsSupportedCulture(cultureInfo))
						{
							ResourceSet resourceSet = this.resourceManager.GetResourceSet(cultureInfo, true, false);
							while (resourceSet == null && cultureInfo.Parent != CultureInfo.InvariantCulture)
							{
								cultureInfo = cultureInfo.Parent;
								resourceSet = this.resourceManager.GetResourceSet(cultureInfo, true, false);
							}
							if (resourceSet != null)
							{
								flag = true;
							}
						}
						this.AddCultureMapping(cultures[i], flag);
					}
					if (flag || Util.PreferedCultureSelector.TestHookAllResourcesExist)
					{
						CultureInfo cultureInfo2 = cultures[i];
						if (cultureInfo2.IsNeutralCulture)
						{
							try
							{
								cultureInfo2 = CultureInfo.CreateSpecificCulture(cultureInfo2.Name);
							}
							catch (ArgumentException)
							{
								goto IL_BC;
							}
						}
						return cultureInfo2;
					}
					IL_BC:;
				}
				return CultureInfo.CurrentCulture;
			}

			private static readonly Dictionary<ExchangeResourceManager, Util.PreferedCultureSelector> preferedCultureSelectorCache = new Dictionary<ExchangeResourceManager, Util.PreferedCultureSelector>();

			private readonly ExchangeResourceManager resourceManager;

			private static bool testHookAllResourcesExist = false;

			private Dictionary<int, bool> resourceAvailabilityMap = new Dictionary<int, bool>();
		}

		internal static class DebugUtil
		{
			[Obsolete("For use from debugger only", false)]
			internal static void DumpToFile(Stream data, string fileName)
			{
				long? num = data.CanSeek ? new long?(data.Position) : null;
				using (FileStream fileStream = File.Create(fileName))
				{
					Util.StreamHandler.CopyStreamData(data, fileStream);
				}
				if (data.CanSeek && num != null)
				{
					data.Position = num.Value;
				}
			}
		}
	}
}
