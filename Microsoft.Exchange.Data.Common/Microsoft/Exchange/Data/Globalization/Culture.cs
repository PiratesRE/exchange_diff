using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class Culture
	{
		internal Culture(int lcid, string name)
		{
			this.lcid = lcid;
			this.name = name;
		}

		public static Culture Default
		{
			get
			{
				return CultureCharsetDatabase.Data.DefaultCulture;
			}
		}

		public static bool FallbackToDefaultCharset
		{
			get
			{
				return CultureCharsetDatabase.Data.FallbackToDefaultCharset;
			}
		}

		public static Culture Invariant
		{
			get
			{
				return CultureCharsetDatabase.Data.InvariantCulture;
			}
		}

		public int LCID
		{
			get
			{
				return this.lcid;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public Charset WindowsCharset
		{
			get
			{
				return this.windowsCharset;
			}
		}

		public Charset MimeCharset
		{
			get
			{
				return this.mimeCharset;
			}
		}

		public Charset WebCharset
		{
			get
			{
				return this.webCharset;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public string NativeDescription
		{
			get
			{
				return this.nativeDescription;
			}
		}

		public Culture ParentCulture
		{
			get
			{
				return this.parentCulture;
			}
		}

		internal int[] CodepageDetectionPriorityOrder
		{
			get
			{
				return this.GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.Data);
			}
		}

		public static Culture GetCulture(string name)
		{
			Culture result;
			if (!Culture.TryGetCulture(name, out result))
			{
				throw new UnknownCultureException(name);
			}
			return result;
		}

		public static bool TryGetCulture(string name, out Culture culture)
		{
			if (name == null)
			{
				culture = null;
				return false;
			}
			return CultureCharsetDatabase.Data.NameToCulture.TryGetValue(name, out culture);
		}

		public static Culture GetCulture(int lcid)
		{
			Culture result;
			if (!Culture.TryGetCulture(lcid, out result))
			{
				throw new UnknownCultureException(lcid);
			}
			return result;
		}

		public static bool TryGetCulture(int lcid, out Culture culture)
		{
			return CultureCharsetDatabase.Data.LcidToCulture.TryGetValue(lcid, out culture);
		}

		public CultureInfo GetCultureInfo()
		{
			if (this.cultureInfo == null)
			{
				return CultureInfo.InvariantCulture;
			}
			return this.cultureInfo;
		}

		internal void SetWindowsCharset(Charset windowsCharset)
		{
			this.windowsCharset = windowsCharset;
		}

		internal void SetMimeCharset(Charset mimeCharset)
		{
			this.mimeCharset = mimeCharset;
		}

		internal void SetWebCharset(Charset webCharset)
		{
			this.webCharset = webCharset;
		}

		internal void SetDescription(string description)
		{
			this.description = description;
		}

		internal void SetNativeDescription(string description)
		{
			this.nativeDescription = description;
		}

		internal void SetParentCulture(Culture parentCulture)
		{
			this.parentCulture = parentCulture;
		}

		internal void SetCultureInfo(CultureInfo cultureInfo)
		{
			this.cultureInfo = cultureInfo;
		}

		internal int[] GetCodepageDetectionPriorityOrder(CultureCharsetDatabase.GlobalizationData data)
		{
			if (this.codepageDetectionPriorityOrder == null)
			{
				this.codepageDetectionPriorityOrder = CultureCharsetDatabase.GetCultureSpecificCodepageDetectionPriorityOrder(this, (this.parentCulture == null || this.parentCulture == this) ? data.DefaultDetectionPriorityOrder : this.parentCulture.GetCodepageDetectionPriorityOrder(data));
			}
			return this.codepageDetectionPriorityOrder;
		}

		internal void SetCodepageDetectionPriorityOrder(int[] codepageDetectionPriorityOrder)
		{
			this.codepageDetectionPriorityOrder = codepageDetectionPriorityOrder;
		}

		internal CultureInfo GetSpecificCultureInfo()
		{
			if (this.cultureInfo == null)
			{
				return CultureInfo.InvariantCulture;
			}
			CultureInfo result;
			try
			{
				result = CultureInfo.CreateSpecificCulture(this.cultureInfo.Name);
			}
			catch (ArgumentException)
			{
				result = CultureInfo.InvariantCulture;
			}
			return result;
		}

		private int lcid;

		private string name;

		private Charset windowsCharset;

		private Charset mimeCharset;

		private Charset webCharset;

		private string description;

		private string nativeDescription;

		private Culture parentCulture;

		private int[] codepageDetectionPriorityOrder;

		private CultureInfo cultureInfo;
	}
}
