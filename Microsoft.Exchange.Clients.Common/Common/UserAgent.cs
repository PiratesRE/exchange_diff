using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Common
{
	public class UserAgent
	{
		public UserAgent(string userAgent, bool changeLayoutFeatureEnabled, HttpCookieCollection cookies)
		{
			this.userAgent = userAgent;
			this.layoutOverrideFeatureEnabled = changeLayoutFeatureEnabled;
			this.cookies = cookies;
		}

		public string RawString
		{
			get
			{
				return this.userAgent;
			}
		}

		public LayoutType Layout
		{
			get
			{
				if (this.layout == null)
				{
					if (this.HasString("iPad") || (this.HasString("Android") && !this.HasString("Mobile")))
					{
						this.layout = new LayoutType?(LayoutType.TouchWide);
					}
					else if (this.HasString("iPhone") || this.HasString("Android") || this.HasString("mobile") || this.HasString("phone"))
					{
						this.layout = new LayoutType?(LayoutType.TouchNarrow);
					}
					else if (this.layoutOverrideFeatureEnabled)
					{
						HttpCookie httpCookie = this.cookies.Get("Layout");
						LayoutType value;
						if (httpCookie != null && Enum.TryParse<LayoutType>(httpCookie.Value, out value))
						{
							this.layout = new LayoutType?(value);
						}
						else if (this.IsBrowserIE() && this.HasString("ARM"))
						{
							this.layout = new LayoutType?(LayoutType.TouchWide);
						}
						else
						{
							this.layout = new LayoutType?(LayoutType.Mouse);
						}
					}
					else
					{
						this.layout = new LayoutType?(LayoutType.Mouse);
					}
				}
				return this.layout.Value;
			}
			set
			{
				this.layout = new LayoutType?(value);
			}
		}

		public string LayoutString
		{
			get
			{
				switch (this.Layout)
				{
				case LayoutType.TouchNarrow:
					return "tnarrow";
				case LayoutType.TouchWide:
					return "twide";
				case LayoutType.Mouse:
					return "mouse";
				default:
					throw new InvalidProgramException();
				}
			}
		}

		public string Platform
		{
			get
			{
				if (string.IsNullOrEmpty(this.platform))
				{
					for (int i = 0; i < UserAgent.clientPlatform.Length; i++)
					{
						if (-1 != this.userAgent.IndexOf(UserAgent.clientPlatform[i], StringComparison.OrdinalIgnoreCase) && (!UserAgent.clientPlatform[i].Equals("Macintosh") || (this.userAgent.IndexOf("iPhone", StringComparison.OrdinalIgnoreCase) < 0 && this.userAgent.IndexOf("iPad", StringComparison.OrdinalIgnoreCase) < 0 && this.userAgent.IndexOf("iPod", StringComparison.OrdinalIgnoreCase) < 0)) && (!UserAgent.clientPlatform[i].Equals("Linux") || this.userAgent.IndexOf("Android", StringComparison.OrdinalIgnoreCase) < 0))
						{
							this.platform = UserAgent.clientPlatform[i];
							break;
						}
					}
				}
				return this.platform;
			}
		}

		public string Browser
		{
			get
			{
				if (string.IsNullOrEmpty(this.browser))
				{
					for (int i = 0; i < UserAgent.browserList.Length; i++)
					{
						if (-1 != this.userAgent.IndexOf(UserAgent.browserList[i], StringComparison.OrdinalIgnoreCase) && (!string.Equals(UserAgent.browserList[i], "Safari", StringComparison.OrdinalIgnoreCase) || -1 == this.userAgent.IndexOf("Chrome", StringComparison.OrdinalIgnoreCase)))
						{
							this.browser = UserAgent.browserList[i];
							break;
						}
					}
					if (string.IsNullOrEmpty(this.browser) && this.userAgent.IndexOf("AppleWebKit") >= 0)
					{
						this.browser = "Safari";
					}
				}
				return this.browser;
			}
		}

		public UserAgentVersion PlatformVersion
		{
			get
			{
				if (this.platformVersion == null)
				{
					this.ParsePlatformVersion();
				}
				return this.platformVersion;
			}
		}

		public UserAgentVersion BrowserVersion
		{
			get
			{
				if (this.browserVersion == null)
				{
					this.ParseBrowserVersion();
				}
				return this.browserVersion;
			}
		}

		public bool IsIos
		{
			get
			{
				return string.Equals("iPhone", this.Platform) || string.Equals("iPad", this.Platform);
			}
		}

		public bool IsAndroid
		{
			get
			{
				return string.Equals("Android", this.Platform);
			}
		}

		public bool IsOsWindows8OrLater()
		{
			return this.IsOsWindowsNtVersionOrLater(6, 2);
		}

		public bool IsOsWindowsNtVersionOrLater(int minMajorVersion, int minMinorVersion)
		{
			if (this.Platform != "Windows NT")
			{
				return false;
			}
			Match match = UserAgent.windowsNTVersionRegex.Match(this.RawString);
			int num;
			int num2;
			return match.Success && int.TryParse(match.Groups["majorVersion"].Value, out num) && int.TryParse(match.Groups["minorVersion"].Value, out num2) && (num > minMajorVersion || (num == minMajorVersion && num2 >= minMinorVersion));
		}

		public bool IsBrowserIE10Plus()
		{
			return this.HasString("MSIE 1") || this.IsBrowserIE11Plus();
		}

		public bool IsBrowserIE11Plus()
		{
			return this.HasString("rv:") && this.HasString("Trident");
		}

		public bool IsBrowserIE()
		{
			if (this.isIE == null)
			{
				this.isIE = new bool?(this.HasString("MSIE") || this.IsBrowserIE11Plus());
			}
			return this.isIE.Value;
		}

		public bool IsBrowserIE8()
		{
			return this.IsBrowserIE() && this.BrowserVersion.Build == 8;
		}

		public double GetTridentVersion()
		{
			double result = 0.0;
			if (this.IsBrowserIE())
			{
				Match match = UserAgent.tridentVersionRegex.Match(this.RawString);
				if (match.Groups.Count == 2)
				{
					double.TryParse(match.Groups[1].Value, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result);
				}
			}
			return result;
		}

		public bool IsBrowserMobileIE()
		{
			return this.IsBrowserIE() && (this.HasString("IEMobile") || this.HasString("ZuneWP7") || this.HasString("WPDesktop"));
		}

		public bool IsMobileIEDesktopMode()
		{
			return this.HasString("WPDesktop");
		}

		public bool IsBrowserFirefox()
		{
			if (this.isFirefox == null)
			{
				this.isFirefox = new bool?(this.HasString("Firefox") && !this.HasString("Trident"));
			}
			return this.isFirefox.Value;
		}

		public bool IsBrowserChrome()
		{
			if (this.isChrome == null)
			{
				this.isChrome = new bool?(this.HasString("Chrome") && !this.HasString("Trident"));
			}
			return this.isChrome.Value;
		}

		public bool IsBrowserSafari()
		{
			if (this.isSafari == null)
			{
				this.isSafari = new bool?(this.HasString("Safari") && !this.HasString("Chrome") && !this.HasString("silk-accelerated") && !this.HasString("Trident"));
			}
			return this.isSafari.Value;
		}

		public void SetLayoutFromString(string layout)
		{
			if (string.IsNullOrEmpty(layout))
			{
				return;
			}
			layout = layout.ToLowerInvariant();
			if (layout == "mouse")
			{
				this.Layout = LayoutType.Mouse;
				return;
			}
			if (layout == "twide" || layout == "touchwide")
			{
				this.Layout = LayoutType.TouchWide;
				return;
			}
			if (layout == "tnarrow" || layout == "touchnarrow")
			{
				this.Layout = LayoutType.TouchNarrow;
			}
		}

		private bool HasString(string str)
		{
			return this.userAgent.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1;
		}

		private void ParseBrowserVersion()
		{
			int num = -1;
			int num2;
			if (this.IsBrowserIE11Plus())
			{
				string text = "rv:";
				num2 = this.userAgent.IndexOf(text) + text.Length;
			}
			else if (this.IsBrowserIE() || this.IsBrowserFirefox() || this.IsBrowserChrome())
			{
				num2 = this.userAgent.IndexOf(this.Browser, StringComparison.OrdinalIgnoreCase) + this.Browser.Length + 1;
			}
			else
			{
				if (!this.IsBrowserSafari())
				{
					this.browserVersion = new UserAgentVersion(0, 0, 0);
					return;
				}
				string text2 = "Version/";
				num2 = this.userAgent.IndexOf(text2) + text2.Length;
			}
			int i;
			for (i = num2; i < this.userAgent.Length; i++)
			{
				if (!char.IsDigit(this.userAgent, i) && this.userAgent[i] != '.' && this.userAgent[i] != '_')
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = this.userAgent.Length;
			}
			if (i == num2)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "Unable to parse browser version.  Could not find semicolon in user agent string {0}", this.userAgent);
				this.browserVersion = new UserAgentVersion(0, 0, 0);
				return;
			}
			string text3 = this.userAgent.Substring(num2, num - num2);
			try
			{
				this.browserVersion = new UserAgentVersion(text3);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "TryParse failed, unable to parse browser version = {0}", text3);
				this.browserVersion = new UserAgentVersion(0, 0, 0);
			}
		}

		private void ParsePlatformVersion()
		{
			int num = -1;
			int num2;
			if (string.Equals("iPhone", this.Platform) || string.Equals("iPad", this.Platform))
			{
				string text = "OS ";
				num2 = this.userAgent.IndexOf(text) + text.Length;
			}
			else
			{
				if (!string.Equals("Android", this.Platform))
				{
					this.platformVersion = new UserAgentVersion(0, 0, 0);
					return;
				}
				string text2 = "Android ";
				num2 = this.userAgent.IndexOf(text2) + text2.Length;
			}
			int i;
			for (i = num2; i < this.userAgent.Length; i++)
			{
				if (!char.IsDigit(this.userAgent, i) && this.userAgent[i] != '.' && this.userAgent[i] != '_')
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = this.userAgent.Length;
			}
			if (i == num2)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "Unable to parse platform version.  Could not find semicolon in user agent string {0}", this.userAgent);
				this.platformVersion = new UserAgentVersion(0, 0, 0);
				return;
			}
			string text3 = this.userAgent.Substring(num2, num - num2);
			try
			{
				this.platformVersion = new UserAgentVersion(text3);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "TryParse failed, unable to parse platform version = {0}", text3);
				this.platformVersion = new UserAgentVersion(0, 0, 0);
			}
		}

		private const string WindowsNTString = "Windows NT";

		internal const string DesktopLayoutString = "mouse";

		private const string LayoutCookieName = "Layout";

		private const string TabletLayoutString = "twide";

		private const string PhoneLayoutString = "tnarrow";

		private static Regex tridentVersionRegex = new Regex("Trident\\/([\\d+.]*\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static Regex windowsNTVersionRegex = new Regex("Windows NT (?<majorVersion>\\d+)\\.(?<minorVersion>\\d+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static string[] clientPlatform = new string[]
		{
			"Windows NT",
			"Windows 98; Win 9x 4.90",
			"Windows 2000",
			"iPhone",
			"iPad",
			"Android",
			"Linux",
			"Macintosh",
			"CrOS"
		};

		private static string[] browserList = new string[]
		{
			"Opera",
			"Netscape",
			"MSIE",
			"Safari",
			"Firefox",
			"Chrome"
		};

		private readonly string userAgent;

		private bool? isIE;

		private bool? isFirefox;

		private bool? isChrome;

		private bool? isSafari;

		private string browser;

		private string platform;

		private UserAgentVersion browserVersion;

		private UserAgentVersion platformVersion;

		private LayoutType? layout;

		private readonly bool layoutOverrideFeatureEnabled;

		private HttpCookieCollection cookies;
	}
}
