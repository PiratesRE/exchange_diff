using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.TextProcessing
{
	public class TextScanContext
	{
		public TextScanContext(string data)
		{
			this.data = data;
			this.fingerprint = null;
		}

		public TextScanContext(Stream stream) : this(TextScanContext.ReadWholeStream(stream))
		{
		}

		public bool IsNull
		{
			get
			{
				return this.data == null;
			}
		}

		public string Data
		{
			get
			{
				return this.data ?? string.Empty;
			}
		}

		public string NormalizedData
		{
			get
			{
				string result;
				if ((result = this.normalizedData) == null)
				{
					result = (this.normalizedData = this.Data.ToUpperInvariant());
				}
				return result;
			}
		}

		internal LshFingerprint Fingerprint
		{
			get
			{
				if (this.fingerprint == null)
				{
					LshFingerprint lshFingerprint;
					if (LshFingerprintGenerator.TryGetFingerprint(this.Data, out lshFingerprint, ""))
					{
						this.fingerprint = lshFingerprint;
					}
					else
					{
						LshFingerprint.TryCreateFingerprint(null, out lshFingerprint, "");
						this.fingerprint = lshFingerprint;
					}
				}
				return this.fingerprint;
			}
			set
			{
				this.fingerprint = value;
			}
		}

		internal void SetTrieScanComplete(long trieID)
		{
			if (this.scannedTrieIDs == null)
			{
				this.scannedTrieIDs = new Dictionary<long, bool>(8);
			}
			this.scannedTrieIDs[trieID] = true;
		}

		internal bool IsTrieScanComplete(long trieID)
		{
			return this.scannedTrieIDs != null && this.scannedTrieIDs.ContainsKey(trieID);
		}

		internal void AddMatchedTermSetID(long id)
		{
			if (this.detectedTermIds == null)
			{
				this.detectedTermIds = new Dictionary<long, bool>(256);
			}
			this.detectedTermIds[id] = true;
		}

		internal bool IsMatchedTermSet(long id)
		{
			return this.detectedTermIds != null && this.detectedTermIds.ContainsKey(id);
		}

		internal bool TryGetCachedResult(long id, out bool result)
		{
			result = false;
			return this.cachedResult != null && this.cachedResult.TryGetValue(id, out result);
		}

		internal void SetCachedResult(long id, bool result)
		{
			if (this.cachedResult == null)
			{
				this.cachedResult = new Dictionary<long, bool>();
			}
			this.cachedResult[id] = result;
		}

		private static string ReadWholeStream(Stream stream)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(stream))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		private readonly string data;

		private string normalizedData;

		private LshFingerprint fingerprint;

		private Dictionary<long, bool> scannedTrieIDs;

		private Dictionary<long, bool> detectedTermIds;

		private Dictionary<long, bool> cachedResult;
	}
}
