using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsLicenseStoreInfoMap
	{
		public RmsLicenseStoreInfoMap(string path, int maxCount, IMruDictionaryPerfCounters perfCounters)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			this.path = path;
			string uniqueFileNameForProcess = RmsClientManagerUtils.GetUniqueFileNameForProcess("RmsLicenseStoreInfoMap_2.dat");
			this.dictionary = new MruDictionary<MultiValueKey, RmsLicenseStoreInfo>(maxCount, new RmsLicenseStoreInfoMap.MultiValueKeyComparer(), perfCounters);
			this.dictionary.OnRemoved += this.MruDictionaryOnRemoved;
			this.dictionary.OnReplaced += this.MruDictionaryOnReplaced;
			this.serializer = new MruDictionarySerializer<MultiValueKey, RmsLicenseStoreInfo>(path, uniqueFileNameForProcess, RmsLicenseStoreInfo.ColumnNames, new MruDictionarySerializerRead<MultiValueKey, RmsLicenseStoreInfo>(RmsLicenseStoreInfoMap.TryReadValues), new MruDictionarySerializerWrite<MultiValueKey, RmsLicenseStoreInfo>(RmsLicenseStoreInfoMap.TryWriteValues), perfCounters);
			if (!this.serializer.TryReadFromDisk(this.dictionary))
			{
				RmsLicenseStoreInfoMap.Tracer.TraceError<string>(0L, "Rms License Store Info Map failed to read map-file ({0}).", uniqueFileNameForProcess);
			}
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool TryGet(MultiValueKey key, out RmsLicenseStoreInfo entry)
		{
			if (key == null)
			{
				entry = null;
				return false;
			}
			if (!this.dictionary.TryGetValue(key, out entry))
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (entry.RacExpire < utcNow || entry.ClcExpire < utcNow)
			{
				RmsLicenseStoreInfoMap.Tracer.TraceDebug<MultiValueKey>(0L, "Rms License Store Info Map removed expired entry for key ({0}).", key);
				this.Remove(key);
				entry = null;
				return false;
			}
			return true;
		}

		public void Add(RmsLicenseStoreInfo entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			MultiValueKey key = new MultiValueKey(new object[]
			{
				entry.TenantId,
				entry.Url
			});
			this.dictionary.Add(key, entry);
			this.serializer.TryWriteToDisk(this.dictionary);
		}

		public void Remove(MultiValueKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.dictionary.Remove(key))
			{
				this.serializer.TryWriteToDisk(this.dictionary);
			}
		}

		private static bool TryReadValues(string[] values, out MultiValueKey key, out RmsLicenseStoreInfo value)
		{
			if (RmsLicenseStoreInfo.TryParse(values, out value))
			{
				key = new MultiValueKey(new object[]
				{
					value.TenantId,
					value.Url
				});
				return true;
			}
			key = null;
			return false;
		}

		private static bool TryWriteValues(MultiValueKey key, RmsLicenseStoreInfo value, out string[] values)
		{
			if (key == null || value == null)
			{
				RmsLicenseStoreInfoMap.Tracer.TraceDebug(0L, "Rms License Store Info Map failed to write values.");
				values = null;
				return false;
			}
			values = value.ToStringArray();
			return true;
		}

		private void MruDictionaryOnRemoved(object sender, MruDictionaryElementRemovedEventArgs<MultiValueKey, RmsLicenseStoreInfo> e)
		{
			if (e.KeyValuePair.Value != null)
			{
				this.DeleteFile(e.KeyValuePair.Value.RacFileName);
				this.DeleteFile(e.KeyValuePair.Value.ClcFileName);
			}
		}

		private void MruDictionaryOnReplaced(object sender, MruDictionaryElementReplacedEventArgs<MultiValueKey, RmsLicenseStoreInfo> e)
		{
			if (e.OldKeyValuePair.Value != null && e.NewKeyValuePair.Value != null)
			{
				if (!string.Equals(e.OldKeyValuePair.Value.RacFileName, e.NewKeyValuePair.Value.RacFileName, StringComparison.OrdinalIgnoreCase))
				{
					this.DeleteFile(e.OldKeyValuePair.Value.RacFileName);
				}
				if (!string.Equals(e.OldKeyValuePair.Value.ClcFileName, e.NewKeyValuePair.Value.ClcFileName, StringComparison.OrdinalIgnoreCase))
				{
					this.DeleteFile(e.OldKeyValuePair.Value.ClcFileName);
				}
			}
		}

		private bool DeleteFile(string file)
		{
			try
			{
				if (!string.IsNullOrEmpty(file))
				{
					File.Delete(Path.Combine(this.path, file));
					RmsLicenseStoreInfoMap.Tracer.TraceDebug<string>(0L, "Rms License Store Info Map deleted RAC-CLC file ({0}).", file);
					return true;
				}
			}
			catch (IOException arg)
			{
				RmsLicenseStoreInfoMap.Tracer.TraceError<string, IOException>(0L, "Rms License Store Info Map failed to delete RAC-CLC file ({0}). IOException - {1}", file, arg);
			}
			catch (UnauthorizedAccessException arg2)
			{
				RmsLicenseStoreInfoMap.Tracer.TraceError<string, UnauthorizedAccessException>(0L, "Rms License Store Info Map failed to delete RAC-CLC file ({0}). UnauthorizedAccessException - {1}", file, arg2);
			}
			catch (SecurityException arg3)
			{
				RmsLicenseStoreInfoMap.Tracer.TraceError<string, SecurityException>(0L, "Rms License Store Info Map failed to delete RAC-CLC file ({0}). SecurityException - {1}", file, arg3);
			}
			return false;
		}

		private const string MapFileSuffix = "RmsLicenseStoreInfoMap_2.dat";

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private MruDictionary<MultiValueKey, RmsLicenseStoreInfo> dictionary;

		private MruDictionarySerializer<MultiValueKey, RmsLicenseStoreInfo> serializer;

		private string path;

		private sealed class MultiValueKeyComparer : IComparer<MultiValueKey>
		{
			public int Compare(MultiValueKey x, MultiValueKey y)
			{
				if (x == null && y == null)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				Guid guid = (Guid)x.GetKey(0);
				Guid value = (Guid)y.GetKey(0);
				int num = guid.CompareTo(value);
				if (num == 0)
				{
					Uri uri = (Uri)x.GetKey(1);
					Uri uri2 = (Uri)y.GetKey(1);
					return Uri.Compare(uri, uri2, UriComponents.HostAndPort, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase);
				}
				return num;
			}
		}
	}
}
