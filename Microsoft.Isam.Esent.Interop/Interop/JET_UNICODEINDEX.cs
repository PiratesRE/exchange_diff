using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_UNICODEINDEX : IContentEquatable<JET_UNICODEINDEX>, IDeepCloneable<JET_UNICODEINDEX>
	{
		public int lcid
		{
			[DebuggerStepThrough]
			get
			{
				return this.localeId;
			}
			set
			{
				this.localeId = value;
			}
		}

		public string szLocaleName
		{
			[DebuggerStepThrough]
			get
			{
				return this.localeName;
			}
			set
			{
				this.localeName = value;
			}
		}

		[CLSCompliant(false)]
		public uint dwMapFlags
		{
			[DebuggerStepThrough]
			get
			{
				return this.mapStringFlags;
			}
			set
			{
				this.mapStringFlags = value;
			}
		}

		public JET_UNICODEINDEX DeepClone()
		{
			return (JET_UNICODEINDEX)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_UNICODEINDEX({0}:{1}:0x{2:X})", new object[]
			{
				this.localeId,
				this.localeName,
				this.mapStringFlags
			});
		}

		public bool ContentEquals(JET_UNICODEINDEX other)
		{
			return other != null && (this.localeId == other.localeId && this.mapStringFlags == other.mapStringFlags) && string.Compare(this.localeName, other.localeName, StringComparison.Ordinal) == 0;
		}

		internal NATIVE_UNICODEINDEX GetNativeUnicodeIndex()
		{
			if (!string.IsNullOrEmpty(this.localeName))
			{
				throw new ArgumentException("localeName was specified, but this version of the API does not accept locale names. Use LCIDs or a different API.");
			}
			return new NATIVE_UNICODEINDEX
			{
				lcid = (uint)this.lcid,
				dwMapFlags = this.dwMapFlags
			};
		}

		static JET_UNICODEINDEX()
		{
			JET_UNICODEINDEX.LcidToLocales.Add(1033, "en-us");
			JET_UNICODEINDEX.LcidToLocales.Add(1046, "pt-br");
			JET_UNICODEINDEX.LcidToLocales.Add(3084, "fr-ca");
		}

		public string GetEffectiveLocaleName()
		{
			if (!string.IsNullOrEmpty(this.szLocaleName))
			{
				return this.szLocaleName;
			}
			return JET_UNICODEINDEX.LimitedLcidToLocaleNameMapping(this.lcid);
		}

		internal static string LimitedLcidToLocaleNameMapping(int lcid)
		{
			string result;
			JET_UNICODEINDEX.LcidToLocales.TryGetValue(lcid, out result);
			return result;
		}

		internal NATIVE_UNICODEINDEX2 GetNativeUnicodeIndex2()
		{
			if (this.lcid != 0 && !JET_UNICODEINDEX.LcidToLocales.ContainsKey(this.lcid))
			{
				throw new ArgumentException("lcid was specified, but this version of the API does not accept LCIDs. Use a locale name or a different API.");
			}
			return new NATIVE_UNICODEINDEX2
			{
				dwMapFlags = this.dwMapFlags
			};
		}

		private int localeId;

		private string localeName;

		private uint mapStringFlags;

		private static readonly Dictionary<int, string> LcidToLocales = new Dictionary<int, string>(10);
	}
}
