using System;
using System.Collections;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Globalization
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class CultureInfo : ICloneable, IFormatProvider
	{
		private static bool Init()
		{
			if (CultureInfo.s_InvariantCultureInfo == null)
			{
				CultureInfo.s_InvariantCultureInfo = new CultureInfo("", false)
				{
					m_isReadOnly = true
				};
			}
			CultureInfo.s_userDefaultCulture = (CultureInfo.s_userDefaultUICulture = CultureInfo.s_InvariantCultureInfo);
			CultureInfo.s_userDefaultCulture = CultureInfo.InitUserDefaultCulture();
			CultureInfo.s_userDefaultUICulture = CultureInfo.InitUserDefaultUICulture();
			return true;
		}

		[SecuritySafeCritical]
		private static CultureInfo InitUserDefaultCulture()
		{
			string defaultLocaleName = CultureInfo.GetDefaultLocaleName(1024);
			if (defaultLocaleName == null)
			{
				defaultLocaleName = CultureInfo.GetDefaultLocaleName(2048);
				if (defaultLocaleName == null)
				{
					return CultureInfo.InvariantCulture;
				}
			}
			CultureInfo cultureByName = CultureInfo.GetCultureByName(defaultLocaleName, true);
			cultureByName.m_isReadOnly = true;
			return cultureByName;
		}

		private static CultureInfo InitUserDefaultUICulture()
		{
			string userDefaultUILanguage = CultureInfo.GetUserDefaultUILanguage();
			if (userDefaultUILanguage == CultureInfo.UserDefaultCulture.Name)
			{
				return CultureInfo.UserDefaultCulture;
			}
			CultureInfo cultureByName = CultureInfo.GetCultureByName(userDefaultUILanguage, true);
			if (cultureByName == null)
			{
				return CultureInfo.InvariantCulture;
			}
			cultureByName.m_isReadOnly = true;
			return cultureByName;
		}

		[SecuritySafeCritical]
		internal static CultureInfo GetCultureInfoForUserPreferredLanguageInAppX()
		{
			if (CultureInfo.ts_IsDoingAppXCultureInfoLookup)
			{
				return null;
			}
			if (AppDomain.IsAppXNGen)
			{
				return null;
			}
			CultureInfo result = null;
			try
			{
				CultureInfo.ts_IsDoingAppXCultureInfoLookup = true;
				if (CultureInfo.s_WindowsRuntimeResourceManager == null)
				{
					CultureInfo.s_WindowsRuntimeResourceManager = ResourceManager.GetWinRTResourceManager();
				}
				result = CultureInfo.s_WindowsRuntimeResourceManager.GlobalResourceContextBestFitCultureInfo;
			}
			finally
			{
				CultureInfo.ts_IsDoingAppXCultureInfoLookup = false;
			}
			return result;
		}

		[SecuritySafeCritical]
		internal static bool SetCultureInfoForUserPreferredLanguageInAppX(CultureInfo ci)
		{
			if (AppDomain.IsAppXNGen)
			{
				return false;
			}
			if (CultureInfo.s_WindowsRuntimeResourceManager == null)
			{
				CultureInfo.s_WindowsRuntimeResourceManager = ResourceManager.GetWinRTResourceManager();
			}
			return CultureInfo.s_WindowsRuntimeResourceManager.SetGlobalResourceContextDefaultCulture(ci);
		}

		[__DynamicallyInvokable]
		public CultureInfo(string name) : this(name, true)
		{
		}

		public CultureInfo(string name, bool useUserOverride)
		{
			this.cultureID = 127;
			base..ctor();
			if (name == null)
			{
				throw new ArgumentNullException("name", Environment.GetResourceString("ArgumentNull_String"));
			}
			this.m_cultureData = CultureData.GetCultureData(name, useUserOverride);
			if (this.m_cultureData == null)
			{
				throw new CultureNotFoundException("name", name, Environment.GetResourceString("Argument_CultureNotSupported"));
			}
			this.m_name = this.m_cultureData.CultureName;
			this.m_isInherited = (base.GetType() != typeof(CultureInfo));
		}

		private CultureInfo(CultureData cultureData)
		{
			this.cultureID = 127;
			base..ctor();
			this.m_cultureData = cultureData;
			this.m_name = cultureData.CultureName;
			this.m_isInherited = false;
		}

		private static CultureInfo CreateCultureInfoNoThrow(string name, bool useUserOverride)
		{
			CultureData cultureData = CultureData.GetCultureData(name, useUserOverride);
			if (cultureData == null)
			{
				return null;
			}
			return new CultureInfo(cultureData);
		}

		public CultureInfo(int culture) : this(culture, true)
		{
		}

		public CultureInfo(int culture, bool useUserOverride)
		{
			this.cultureID = 127;
			base..ctor();
			if (culture < 0)
			{
				throw new ArgumentOutOfRangeException("culture", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
			}
			this.InitializeFromCultureId(culture, useUserOverride);
		}

		private void InitializeFromCultureId(int culture, bool useUserOverride)
		{
			if (culture <= 1024)
			{
				if (culture != 0 && culture != 1024)
				{
					goto IL_43;
				}
			}
			else if (culture != 2048 && culture != 3072 && culture != 4096)
			{
				goto IL_43;
			}
			throw new CultureNotFoundException("culture", culture, Environment.GetResourceString("Argument_CultureNotSupported"));
			IL_43:
			this.m_cultureData = CultureData.GetCultureData(culture, useUserOverride);
			this.m_isInherited = (base.GetType() != typeof(CultureInfo));
			this.m_name = this.m_cultureData.CultureName;
		}

		internal static void CheckDomainSafetyObject(object obj, object container)
		{
			if (obj.GetType().Assembly != typeof(CultureInfo).Assembly)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_SubclassedObject"), obj.GetType(), container.GetType()));
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			if (this.m_name == null || CultureInfo.IsAlternateSortLcid(this.cultureID))
			{
				this.InitializeFromCultureId(this.cultureID, this.m_useUserOverride);
			}
			else
			{
				this.m_cultureData = CultureData.GetCultureData(this.m_name, this.m_useUserOverride);
				if (this.m_cultureData == null)
				{
					throw new CultureNotFoundException("m_name", this.m_name, Environment.GetResourceString("Argument_CultureNotSupported"));
				}
			}
			this.m_isInherited = (base.GetType() != typeof(CultureInfo));
			if (base.GetType().Assembly == typeof(CultureInfo).Assembly)
			{
				if (this.textInfo != null)
				{
					CultureInfo.CheckDomainSafetyObject(this.textInfo, this);
				}
				if (this.compareInfo != null)
				{
					CultureInfo.CheckDomainSafetyObject(this.compareInfo, this);
				}
			}
		}

		private static bool IsAlternateSortLcid(int lcid)
		{
			return lcid == 1034 || (lcid & 983040) != 0;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext ctx)
		{
			this.m_name = this.m_cultureData.CultureName;
			this.m_useUserOverride = this.m_cultureData.UseUserOverride;
			this.cultureID = this.m_cultureData.ILANGUAGE;
		}

		internal bool IsSafeCrossDomain
		{
			get
			{
				return this.m_isSafeCrossDomain;
			}
		}

		internal int CreatedDomainID
		{
			get
			{
				return this.m_createdDomainID;
			}
		}

		internal void StartCrossDomainTracking()
		{
			if (this.m_createdDomainID != 0)
			{
				return;
			}
			if (this.CanSendCrossDomain())
			{
				this.m_isSafeCrossDomain = true;
			}
			Thread.MemoryBarrier();
			this.m_createdDomainID = Thread.GetDomainID();
		}

		internal bool CanSendCrossDomain()
		{
			bool result = false;
			if (base.GetType() == typeof(CultureInfo))
			{
				result = true;
			}
			return result;
		}

		internal CultureInfo(string cultureName, string textAndCompareCultureName)
		{
			this.cultureID = 127;
			base..ctor();
			if (cultureName == null)
			{
				throw new ArgumentNullException("cultureName", Environment.GetResourceString("ArgumentNull_String"));
			}
			this.m_cultureData = CultureData.GetCultureData(cultureName, false);
			if (this.m_cultureData == null)
			{
				throw new CultureNotFoundException("cultureName", cultureName, Environment.GetResourceString("Argument_CultureNotSupported"));
			}
			this.m_name = this.m_cultureData.CultureName;
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(textAndCompareCultureName);
			this.compareInfo = cultureInfo.CompareInfo;
			this.textInfo = cultureInfo.TextInfo;
		}

		private static CultureInfo GetCultureByName(string name, bool userOverride)
		{
			try
			{
				return userOverride ? new CultureInfo(name) : CultureInfo.GetCultureInfo(name);
			}
			catch (ArgumentException)
			{
			}
			return null;
		}

		public static CultureInfo CreateSpecificCulture(string name)
		{
			CultureInfo cultureInfo;
			try
			{
				cultureInfo = new CultureInfo(name);
			}
			catch (ArgumentException)
			{
				cultureInfo = null;
				for (int i = 0; i < name.Length; i++)
				{
					if ('-' == name[i])
					{
						try
						{
							cultureInfo = new CultureInfo(name.Substring(0, i));
							break;
						}
						catch (ArgumentException)
						{
							throw;
						}
					}
				}
				if (cultureInfo == null)
				{
					throw;
				}
			}
			if (!cultureInfo.IsNeutralCulture)
			{
				return cultureInfo;
			}
			return new CultureInfo(cultureInfo.m_cultureData.SSPECIFICCULTURE);
		}

		internal static bool VerifyCultureName(string cultureName, bool throwException)
		{
			int i = 0;
			while (i < cultureName.Length)
			{
				char c = cultureName[i];
				if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
				{
					if (throwException)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidResourceCultureName", new object[]
						{
							cultureName
						}));
					}
					return false;
				}
				else
				{
					i++;
				}
			}
			return true;
		}

		internal static bool VerifyCultureName(CultureInfo culture, bool throwException)
		{
			return !culture.m_isInherited || CultureInfo.VerifyCultureName(culture.Name, throwException);
		}

		[__DynamicallyInvokable]
		public static CultureInfo CurrentCulture
		{
			[__DynamicallyInvokable]
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (AppDomain.IsAppXModel() && CultureInfo.SetCultureInfoForUserPreferredLanguageInAppX(value))
				{
					return;
				}
				Thread.CurrentThread.CurrentCulture = value;
			}
		}

		internal static CultureInfo UserDefaultCulture
		{
			get
			{
				CultureInfo cultureInfo = CultureInfo.s_userDefaultCulture;
				if (cultureInfo == null)
				{
					CultureInfo.s_userDefaultCulture = CultureInfo.InvariantCulture;
					cultureInfo = CultureInfo.InitUserDefaultCulture();
					CultureInfo.s_userDefaultCulture = cultureInfo;
				}
				return cultureInfo;
			}
		}

		internal static CultureInfo UserDefaultUICulture
		{
			get
			{
				CultureInfo cultureInfo = CultureInfo.s_userDefaultUICulture;
				if (cultureInfo == null)
				{
					CultureInfo.s_userDefaultUICulture = CultureInfo.InvariantCulture;
					cultureInfo = CultureInfo.InitUserDefaultUICulture();
					CultureInfo.s_userDefaultUICulture = cultureInfo;
				}
				return cultureInfo;
			}
		}

		[__DynamicallyInvokable]
		public static CultureInfo CurrentUICulture
		{
			[__DynamicallyInvokable]
			get
			{
				return Thread.CurrentThread.CurrentUICulture;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (AppDomain.IsAppXModel() && CultureInfo.SetCultureInfoForUserPreferredLanguageInAppX(value))
				{
					return;
				}
				Thread.CurrentThread.CurrentUICulture = value;
			}
		}

		public static CultureInfo InstalledUICulture
		{
			get
			{
				CultureInfo cultureInfo = CultureInfo.s_InstalledUICultureInfo;
				if (cultureInfo == null)
				{
					string systemDefaultUILanguage = CultureInfo.GetSystemDefaultUILanguage();
					cultureInfo = CultureInfo.GetCultureByName(systemDefaultUILanguage, true);
					if (cultureInfo == null)
					{
						cultureInfo = CultureInfo.InvariantCulture;
					}
					cultureInfo.m_isReadOnly = true;
					CultureInfo.s_InstalledUICultureInfo = cultureInfo;
				}
				return cultureInfo;
			}
		}

		[__DynamicallyInvokable]
		public static CultureInfo DefaultThreadCurrentCulture
		{
			[__DynamicallyInvokable]
			get
			{
				return CultureInfo.s_DefaultThreadCurrentCulture;
			}
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
			set
			{
				CultureInfo.s_DefaultThreadCurrentCulture = value;
			}
		}

		[__DynamicallyInvokable]
		public static CultureInfo DefaultThreadCurrentUICulture
		{
			[__DynamicallyInvokable]
			get
			{
				return CultureInfo.s_DefaultThreadCurrentUICulture;
			}
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			[SecurityPermission(SecurityAction.Demand, ControlThread = true)]
			set
			{
				if (value != null)
				{
					CultureInfo.VerifyCultureName(value, true);
				}
				CultureInfo.s_DefaultThreadCurrentUICulture = value;
			}
		}

		[__DynamicallyInvokable]
		public static CultureInfo InvariantCulture
		{
			[__DynamicallyInvokable]
			get
			{
				return CultureInfo.s_InvariantCultureInfo;
			}
		}

		[__DynamicallyInvokable]
		public virtual CultureInfo Parent
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				if (this.m_parent == null)
				{
					string sparent = this.m_cultureData.SPARENT;
					CultureInfo cultureInfo;
					if (string.IsNullOrEmpty(sparent))
					{
						cultureInfo = CultureInfo.InvariantCulture;
					}
					else
					{
						cultureInfo = CultureInfo.CreateCultureInfoNoThrow(sparent, this.m_cultureData.UseUserOverride);
						if (cultureInfo == null)
						{
							cultureInfo = CultureInfo.InvariantCulture;
						}
					}
					Interlocked.CompareExchange<CultureInfo>(ref this.m_parent, cultureInfo, null);
				}
				return this.m_parent;
			}
		}

		public virtual int LCID
		{
			get
			{
				return this.m_cultureData.ILANGUAGE;
			}
		}

		[ComVisible(false)]
		public virtual int KeyboardLayoutId
		{
			get
			{
				return this.m_cultureData.IINPUTLANGUAGEHANDLE;
			}
		}

		public static CultureInfo[] GetCultures(CultureTypes types)
		{
			if ((types & CultureTypes.UserCustomCulture) == CultureTypes.UserCustomCulture)
			{
				types |= CultureTypes.ReplacementCultures;
			}
			return CultureData.GetCultures(types);
		}

		[__DynamicallyInvokable]
		public virtual string Name
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.m_nonSortName == null)
				{
					this.m_nonSortName = this.m_cultureData.SNAME;
					if (this.m_nonSortName == null)
					{
						this.m_nonSortName = string.Empty;
					}
				}
				return this.m_nonSortName;
			}
		}

		internal string SortName
		{
			get
			{
				if (this.m_sortName == null)
				{
					this.m_sortName = this.m_cultureData.SCOMPAREINFO;
				}
				return this.m_sortName;
			}
		}

		[ComVisible(false)]
		public string IetfLanguageTag
		{
			get
			{
				string name = this.Name;
				if (name == "zh-CHT")
				{
					return "zh-Hant";
				}
				if (!(name == "zh-CHS"))
				{
					return this.Name;
				}
				return "zh-Hans";
			}
		}

		[__DynamicallyInvokable]
		public virtual string DisplayName
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				return this.m_cultureData.SLOCALIZEDDISPLAYNAME;
			}
		}

		[__DynamicallyInvokable]
		public virtual string NativeName
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				return this.m_cultureData.SNATIVEDISPLAYNAME;
			}
		}

		[__DynamicallyInvokable]
		public virtual string EnglishName
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				return this.m_cultureData.SENGDISPLAYNAME;
			}
		}

		[__DynamicallyInvokable]
		public virtual string TwoLetterISOLanguageName
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				return this.m_cultureData.SISO639LANGNAME;
			}
		}

		public virtual string ThreeLetterISOLanguageName
		{
			[SecuritySafeCritical]
			get
			{
				return this.m_cultureData.SISO639LANGNAME2;
			}
		}

		public virtual string ThreeLetterWindowsLanguageName
		{
			[SecuritySafeCritical]
			get
			{
				return this.m_cultureData.SABBREVLANGNAME;
			}
		}

		[__DynamicallyInvokable]
		public virtual CompareInfo CompareInfo
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.compareInfo == null)
				{
					CompareInfo result = this.UseUserOverride ? CultureInfo.GetCultureInfo(this.m_name).CompareInfo : new CompareInfo(this);
					if (!CompatibilitySwitches.IsCompatibilityBehaviorDefined)
					{
						return result;
					}
					this.compareInfo = result;
				}
				return this.compareInfo;
			}
		}

		private RegionInfo Region
		{
			get
			{
				if (this.regionInfo == null)
				{
					RegionInfo regionInfo = new RegionInfo(this.m_cultureData);
					this.regionInfo = regionInfo;
				}
				return this.regionInfo;
			}
		}

		[__DynamicallyInvokable]
		public virtual TextInfo TextInfo
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.textInfo == null)
				{
					TextInfo textInfo = new TextInfo(this.m_cultureData);
					textInfo.SetReadOnlyState(this.m_isReadOnly);
					if (!CompatibilitySwitches.IsCompatibilityBehaviorDefined)
					{
						return textInfo;
					}
					this.textInfo = textInfo;
				}
				return this.textInfo;
			}
		}

		[__DynamicallyInvokable]
		public override bool Equals(object value)
		{
			if (this == value)
			{
				return true;
			}
			CultureInfo cultureInfo = value as CultureInfo;
			return cultureInfo != null && this.Name.Equals(cultureInfo.Name) && this.CompareInfo.Equals(cultureInfo.CompareInfo);
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this.Name.GetHashCode() + this.CompareInfo.GetHashCode();
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.m_name;
		}

		[__DynamicallyInvokable]
		public virtual object GetFormat(Type formatType)
		{
			if (formatType == typeof(NumberFormatInfo))
			{
				return this.NumberFormat;
			}
			if (formatType == typeof(DateTimeFormatInfo))
			{
				return this.DateTimeFormat;
			}
			return null;
		}

		[__DynamicallyInvokable]
		public virtual bool IsNeutralCulture
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_cultureData.IsNeutralCulture;
			}
		}

		[ComVisible(false)]
		public CultureTypes CultureTypes
		{
			get
			{
				CultureTypes cultureTypes = (CultureTypes)0;
				if (this.m_cultureData.IsNeutralCulture)
				{
					cultureTypes |= CultureTypes.NeutralCultures;
				}
				else
				{
					cultureTypes |= CultureTypes.SpecificCultures;
				}
				cultureTypes |= (this.m_cultureData.IsWin32Installed ? CultureTypes.InstalledWin32Cultures : ((CultureTypes)0));
				cultureTypes |= (this.m_cultureData.IsFramework ? CultureTypes.FrameworkCultures : ((CultureTypes)0));
				cultureTypes |= (this.m_cultureData.IsSupplementalCustomCulture ? CultureTypes.UserCustomCulture : ((CultureTypes)0));
				return cultureTypes | (this.m_cultureData.IsReplacementCulture ? (CultureTypes.UserCustomCulture | CultureTypes.ReplacementCultures) : ((CultureTypes)0));
			}
		}

		[__DynamicallyInvokable]
		public virtual NumberFormatInfo NumberFormat
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.numInfo == null)
				{
					this.numInfo = new NumberFormatInfo(this.m_cultureData)
					{
						isReadOnly = this.m_isReadOnly
					};
				}
				return this.numInfo;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
				}
				this.VerifyWritable();
				this.numInfo = value;
			}
		}

		[__DynamicallyInvokable]
		public virtual DateTimeFormatInfo DateTimeFormat
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.dateTimeInfo == null)
				{
					DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo(this.m_cultureData, this.Calendar);
					dateTimeFormatInfo.m_isReadOnly = this.m_isReadOnly;
					Thread.MemoryBarrier();
					this.dateTimeInfo = dateTimeFormatInfo;
				}
				return this.dateTimeInfo;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_Obj"));
				}
				this.VerifyWritable();
				this.dateTimeInfo = value;
			}
		}

		public void ClearCachedData()
		{
			CultureInfo.s_userDefaultUICulture = null;
			CultureInfo.s_userDefaultCulture = null;
			RegionInfo.s_currentRegionInfo = null;
			TimeZone.ResetTimeZone();
			TimeZoneInfo.ClearCachedData();
			CultureInfo.s_LcidCachedCultures = null;
			CultureInfo.s_NameCachedCultures = null;
			CultureData.ClearCachedData();
		}

		internal static Calendar GetCalendarInstance(int calType)
		{
			if (calType == 1)
			{
				return new GregorianCalendar();
			}
			return CultureInfo.GetCalendarInstanceRare(calType);
		}

		internal static Calendar GetCalendarInstanceRare(int calType)
		{
			switch (calType)
			{
			case 2:
			case 9:
			case 10:
			case 11:
			case 12:
				return new GregorianCalendar((GregorianCalendarTypes)calType);
			case 3:
				return new JapaneseCalendar();
			case 4:
				return new TaiwanCalendar();
			case 5:
				return new KoreanCalendar();
			case 6:
				return new HijriCalendar();
			case 7:
				return new ThaiBuddhistCalendar();
			case 8:
				return new HebrewCalendar();
			case 14:
				return new JapaneseLunisolarCalendar();
			case 15:
				return new ChineseLunisolarCalendar();
			case 20:
				return new KoreanLunisolarCalendar();
			case 21:
				return new TaiwanLunisolarCalendar();
			case 22:
				return new PersianCalendar();
			case 23:
				return new UmAlQuraCalendar();
			}
			return new GregorianCalendar();
		}

		[__DynamicallyInvokable]
		public virtual Calendar Calendar
		{
			[__DynamicallyInvokable]
			get
			{
				if (this.calendar == null)
				{
					Calendar defaultCalendar = this.m_cultureData.DefaultCalendar;
					Thread.MemoryBarrier();
					defaultCalendar.SetReadOnlyState(this.m_isReadOnly);
					this.calendar = defaultCalendar;
				}
				return this.calendar;
			}
		}

		[__DynamicallyInvokable]
		public virtual Calendar[] OptionalCalendars
		{
			[__DynamicallyInvokable]
			get
			{
				int[] calendarIds = this.m_cultureData.CalendarIds;
				Calendar[] array = new Calendar[calendarIds.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = CultureInfo.GetCalendarInstance(calendarIds[i]);
				}
				return array;
			}
		}

		public bool UseUserOverride
		{
			get
			{
				return this.m_cultureData.UseUserOverride;
			}
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public CultureInfo GetConsoleFallbackUICulture()
		{
			CultureInfo cultureInfo = this.m_consoleFallbackCulture;
			if (cultureInfo == null)
			{
				cultureInfo = CultureInfo.CreateSpecificCulture(this.m_cultureData.SCONSOLEFALLBACKNAME);
				cultureInfo.m_isReadOnly = true;
				this.m_consoleFallbackCulture = cultureInfo;
			}
			return cultureInfo;
		}

		[__DynamicallyInvokable]
		public virtual object Clone()
		{
			CultureInfo cultureInfo = (CultureInfo)base.MemberwiseClone();
			cultureInfo.m_isReadOnly = false;
			if (!this.m_isInherited)
			{
				if (this.dateTimeInfo != null)
				{
					cultureInfo.dateTimeInfo = (DateTimeFormatInfo)this.dateTimeInfo.Clone();
				}
				if (this.numInfo != null)
				{
					cultureInfo.numInfo = (NumberFormatInfo)this.numInfo.Clone();
				}
			}
			else
			{
				cultureInfo.DateTimeFormat = (DateTimeFormatInfo)this.DateTimeFormat.Clone();
				cultureInfo.NumberFormat = (NumberFormatInfo)this.NumberFormat.Clone();
			}
			if (this.textInfo != null)
			{
				cultureInfo.textInfo = (TextInfo)this.textInfo.Clone();
			}
			if (this.calendar != null)
			{
				cultureInfo.calendar = (Calendar)this.calendar.Clone();
			}
			return cultureInfo;
		}

		[__DynamicallyInvokable]
		public static CultureInfo ReadOnly(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new ArgumentNullException("ci");
			}
			if (ci.IsReadOnly)
			{
				return ci;
			}
			CultureInfo cultureInfo = (CultureInfo)ci.MemberwiseClone();
			if (!ci.IsNeutralCulture)
			{
				if (!ci.m_isInherited)
				{
					if (ci.dateTimeInfo != null)
					{
						cultureInfo.dateTimeInfo = DateTimeFormatInfo.ReadOnly(ci.dateTimeInfo);
					}
					if (ci.numInfo != null)
					{
						cultureInfo.numInfo = NumberFormatInfo.ReadOnly(ci.numInfo);
					}
				}
				else
				{
					cultureInfo.DateTimeFormat = DateTimeFormatInfo.ReadOnly(ci.DateTimeFormat);
					cultureInfo.NumberFormat = NumberFormatInfo.ReadOnly(ci.NumberFormat);
				}
			}
			if (ci.textInfo != null)
			{
				cultureInfo.textInfo = TextInfo.ReadOnly(ci.textInfo);
			}
			if (ci.calendar != null)
			{
				cultureInfo.calendar = Calendar.ReadOnly(ci.calendar);
			}
			cultureInfo.m_isReadOnly = true;
			return cultureInfo;
		}

		[__DynamicallyInvokable]
		public bool IsReadOnly
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_isReadOnly;
			}
		}

		private void VerifyWritable()
		{
			if (this.m_isReadOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
			}
		}

		internal bool HasInvariantCultureName
		{
			get
			{
				return this.Name == CultureInfo.InvariantCulture.Name;
			}
		}

		internal static CultureInfo GetCultureInfoHelper(int lcid, string name, string altName)
		{
			Hashtable hashtable = CultureInfo.s_NameCachedCultures;
			if (name != null)
			{
				name = CultureData.AnsiToLower(name);
			}
			if (altName != null)
			{
				altName = CultureData.AnsiToLower(altName);
			}
			CultureInfo cultureInfo;
			if (hashtable == null)
			{
				hashtable = Hashtable.Synchronized(new Hashtable());
			}
			else if (lcid == -1)
			{
				cultureInfo = (CultureInfo)hashtable[name + "�" + altName];
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
			}
			else if (lcid == 0)
			{
				cultureInfo = (CultureInfo)hashtable[name];
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
			}
			Hashtable hashtable2 = CultureInfo.s_LcidCachedCultures;
			if (hashtable2 == null)
			{
				hashtable2 = Hashtable.Synchronized(new Hashtable());
			}
			else if (lcid > 0)
			{
				cultureInfo = (CultureInfo)hashtable2[lcid];
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
			}
			try
			{
				if (lcid != -1)
				{
					if (lcid != 0)
					{
						cultureInfo = new CultureInfo(lcid, false);
					}
					else
					{
						cultureInfo = new CultureInfo(name, false);
					}
				}
				else
				{
					cultureInfo = new CultureInfo(name, altName);
				}
			}
			catch (ArgumentException)
			{
				return null;
			}
			cultureInfo.m_isReadOnly = true;
			if (lcid == -1)
			{
				hashtable[name + "�" + altName] = cultureInfo;
				cultureInfo.TextInfo.SetReadOnlyState(true);
			}
			else
			{
				string text = CultureData.AnsiToLower(cultureInfo.m_name);
				hashtable[text] = cultureInfo;
				if ((cultureInfo.LCID != 4 || !(text == "zh-hans")) && (cultureInfo.LCID != 31748 || !(text == "zh-hant")))
				{
					hashtable2[cultureInfo.LCID] = cultureInfo;
				}
			}
			if (-1 != lcid)
			{
				CultureInfo.s_LcidCachedCultures = hashtable2;
			}
			CultureInfo.s_NameCachedCultures = hashtable;
			return cultureInfo;
		}

		public static CultureInfo GetCultureInfo(int culture)
		{
			if (culture <= 0)
			{
				throw new ArgumentOutOfRangeException("culture", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
			}
			CultureInfo cultureInfoHelper = CultureInfo.GetCultureInfoHelper(culture, null, null);
			if (cultureInfoHelper == null)
			{
				throw new CultureNotFoundException("culture", culture, Environment.GetResourceString("Argument_CultureNotSupported"));
			}
			return cultureInfoHelper;
		}

		public static CultureInfo GetCultureInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			CultureInfo cultureInfoHelper = CultureInfo.GetCultureInfoHelper(0, name, null);
			if (cultureInfoHelper == null)
			{
				throw new CultureNotFoundException("name", name, Environment.GetResourceString("Argument_CultureNotSupported"));
			}
			return cultureInfoHelper;
		}

		public static CultureInfo GetCultureInfo(string name, string altName)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (altName == null)
			{
				throw new ArgumentNullException("altName");
			}
			CultureInfo cultureInfoHelper = CultureInfo.GetCultureInfoHelper(-1, name, altName);
			if (cultureInfoHelper == null)
			{
				throw new CultureNotFoundException("name or altName", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_OneOfCulturesNotSupported"), name, altName));
			}
			return cultureInfoHelper;
		}

		public static CultureInfo GetCultureInfoByIetfLanguageTag(string name)
		{
			if (name == "zh-CHT" || name == "zh-CHS")
			{
				throw new CultureNotFoundException("name", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureIetfNotSupported"), name));
			}
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo(name);
			if (cultureInfo.LCID > 65535 || cultureInfo.LCID == 1034)
			{
				throw new CultureNotFoundException("name", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_CultureIetfNotSupported"), name));
			}
			return cultureInfo;
		}

		internal static bool IsTaiwanSku
		{
			get
			{
				if (!CultureInfo.s_haveIsTaiwanSku)
				{
					CultureInfo.s_isTaiwanSku = (CultureInfo.GetSystemDefaultUILanguage() == "zh-TW");
					CultureInfo.s_haveIsTaiwanSku = true;
				}
				return CultureInfo.s_isTaiwanSku;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string nativeGetLocaleInfoEx(string localeName, uint field);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int nativeGetLocaleInfoExInt(string localeName, uint field);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool nativeSetThreadLocale(string localeName);

		[SecurityCritical]
		private static string GetDefaultLocaleName(int localeType)
		{
			string result = null;
			if (CultureInfo.InternalGetDefaultLocaleName(localeType, JitHelpers.GetStringHandleOnStack(ref result)))
			{
				return result;
			}
			return string.Empty;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool InternalGetDefaultLocaleName(int localetype, StringHandleOnStack localeString);

		[SecuritySafeCritical]
		private static string GetUserDefaultUILanguage()
		{
			string result = null;
			if (CultureInfo.InternalGetUserDefaultUILanguage(JitHelpers.GetStringHandleOnStack(ref result)))
			{
				return result;
			}
			return string.Empty;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool InternalGetUserDefaultUILanguage(StringHandleOnStack userDefaultUiLanguage);

		[SecuritySafeCritical]
		private static string GetSystemDefaultUILanguage()
		{
			string result = null;
			if (CultureInfo.InternalGetSystemDefaultUILanguage(JitHelpers.GetStringHandleOnStack(ref result)))
			{
				return result;
			}
			return string.Empty;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool InternalGetSystemDefaultUILanguage(StringHandleOnStack systemDefaultUiLanguage);

		internal bool m_isReadOnly;

		internal CompareInfo compareInfo;

		internal TextInfo textInfo;

		[NonSerialized]
		internal RegionInfo regionInfo;

		internal NumberFormatInfo numInfo;

		internal DateTimeFormatInfo dateTimeInfo;

		internal Calendar calendar;

		[OptionalField(VersionAdded = 1)]
		internal int m_dataItem;

		[OptionalField(VersionAdded = 1)]
		internal int cultureID;

		[NonSerialized]
		internal CultureData m_cultureData;

		[NonSerialized]
		internal bool m_isInherited;

		[NonSerialized]
		private bool m_isSafeCrossDomain;

		[NonSerialized]
		private int m_createdDomainID;

		[NonSerialized]
		private CultureInfo m_consoleFallbackCulture;

		internal string m_name;

		[NonSerialized]
		private string m_nonSortName;

		[NonSerialized]
		private string m_sortName;

		private static volatile CultureInfo s_userDefaultCulture;

		private static volatile CultureInfo s_InvariantCultureInfo;

		private static volatile CultureInfo s_userDefaultUICulture;

		private static volatile CultureInfo s_InstalledUICultureInfo;

		private static volatile CultureInfo s_DefaultThreadCurrentUICulture;

		private static volatile CultureInfo s_DefaultThreadCurrentCulture;

		private static volatile Hashtable s_LcidCachedCultures;

		private static volatile Hashtable s_NameCachedCultures;

		[SecurityCritical]
		private static volatile WindowsRuntimeResourceManagerBase s_WindowsRuntimeResourceManager;

		[ThreadStatic]
		private static bool ts_IsDoingAppXCultureInfoLookup;

		[NonSerialized]
		private CultureInfo m_parent;

		internal const int LOCALE_NEUTRAL = 0;

		private const int LOCALE_USER_DEFAULT = 1024;

		private const int LOCALE_SYSTEM_DEFAULT = 2048;

		internal const int LOCALE_CUSTOM_DEFAULT = 3072;

		internal const int LOCALE_CUSTOM_UNSPECIFIED = 4096;

		internal const int LOCALE_INVARIANT = 127;

		private const int LOCALE_TRADITIONAL_SPANISH = 1034;

		private static readonly bool init = CultureInfo.Init();

		private bool m_useUserOverride;

		private const int LOCALE_SORTID_MASK = 983040;

		private static volatile bool s_isTaiwanSku;

		private static volatile bool s_haveIsTaiwanSku;
	}
}
