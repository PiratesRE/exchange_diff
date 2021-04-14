using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class CalendarColorManager
	{
		private CalendarColorManager(UserContext userContext)
		{
			this.Load(userContext);
		}

		private UserConfiguration Configuration
		{
			get
			{
				return this.config;
			}
		}

		public static bool IsColorIndexValid(int colorIndex)
		{
			return colorIndex >= -2 && colorIndex < 15;
		}

		public static int GetCalendarFolderColor(UserContext userContext, StoreObjectId calendarFolderId)
		{
			CalendarColorManager.ThrowIfCannotActAsOwner(userContext);
			NavigationNodeCollection navigationNodeCollection = NavigationNodeCollection.TryCreateNavigationNodeCollection(userContext, userContext.MailboxSession, NavigationNodeGroupSection.Calendar);
			return CalendarColorManager.GetCalendarFolderColor(userContext, calendarFolderId, navigationNodeCollection);
		}

		public static int GetCalendarFolderColor(UserContext userContext, StoreObjectId calendarFolderId, NavigationNodeCollection navigationNodeCollection)
		{
			CalendarColorManager.ThrowIfCannotActAsOwner(userContext);
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (calendarFolderId == null)
			{
				throw new ArgumentNullException("calendarFolderId");
			}
			int result = -2;
			if (navigationNodeCollection != null)
			{
				try
				{
					NavigationNodeFolder[] navigationNodeFolders = navigationNodeCollection.FindFoldersById(calendarFolderId);
					result = CalendarColorManager.GetCalendarFolderColor(userContext, navigationNodeCollection, navigationNodeFolders);
				}
				catch (StoragePermanentException ex)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "CalendarColorManager.GetCalendarFolderColor. Unable to get calendar folder color. Exception: {0}.", new object[]
					{
						ex.Message
					});
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, message);
				}
				catch (StorageTransientException ex2)
				{
					string message2 = string.Format(CultureInfo.InvariantCulture, "CalendarColorManager.GetCalendarFolderColor. Unable to get calendar folder color. Exception: {0}.", new object[]
					{
						ex2.Message
					});
					ExTraceGlobals.CoreCallTracer.TraceDebug(0L, message2);
				}
			}
			return result;
		}

		public static int GetCalendarFolderColor(UserContext userContext, NavigationNodeCollection navigationNodeCollection, NavigationNodeFolder[] navigationNodeFolders)
		{
			CalendarColorManager.ThrowIfCannotActAsOwner(userContext);
			int num = -2;
			if (navigationNodeFolders != null && navigationNodeFolders.Length > 0)
			{
				num = navigationNodeFolders[0].NavigationNodeCalendarColor;
			}
			if (!CalendarColorManager.IsColorIndexValid(num))
			{
				num = -2;
				foreach (NavigationNodeFolder navigationNodeFolder in navigationNodeFolders)
				{
					navigationNodeFolder.NavigationNodeCalendarColor = num;
				}
				navigationNodeCollection.Save(userContext.MailboxSession);
			}
			return num;
		}

		public static int ParseColorIndexString(string sColor, bool isClientColorIndex)
		{
			if (sColor == null)
			{
				throw new ArgumentNullException("sColor");
			}
			int num;
			if (!int.TryParse(sColor, out num))
			{
				num = -2;
			}
			else if (isClientColorIndex)
			{
				num = CalendarColorManager.GetServerColorIndex(num);
			}
			if (!CalendarColorManager.IsColorIndexValid(num))
			{
				num = -2;
			}
			return num;
		}

		public static int GetClientColorIndex(int serverColorIndex)
		{
			if (serverColorIndex < 0)
			{
				return serverColorIndex;
			}
			return serverColorIndex + 1;
		}

		public static int GetServerColorIndex(int clientColorIndex)
		{
			if (clientColorIndex <= 0)
			{
				return clientColorIndex;
			}
			return clientColorIndex - 1;
		}

		public static void ChangeColorName(UserContext userContext, int calendarColorIndex, string calendarColorName)
		{
			CalendarColorManager.ThrowIfCannotActAsOwner(userContext);
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (-2 > calendarColorIndex || calendarColorIndex >= 15)
			{
				throw new ArgumentOutOfRangeException("calendarColorIndex");
			}
			if (string.IsNullOrEmpty(calendarColorName))
			{
				throw new ArgumentNullException("calendarColorName");
			}
			if (calendarColorName.Length > 255)
			{
				throw new ArgumentException("calendarColorName may not exceed 255 characters in length");
			}
			CalendarColorManager calendarColorManager = new CalendarColorManager(userContext);
			IDictionary dictionary = calendarColorManager.Configuration.GetDictionary();
			if (!dictionary.Contains(calendarColorIndex))
			{
				throw new OwaInvalidInputException(string.Format(CultureInfo.InvariantCulture, "Value {0} is not a valid calendar color index", new object[]
				{
					calendarColorIndex
				}));
			}
			dictionary[calendarColorIndex] = calendarColorName;
			calendarColorManager.Configuration.Save();
		}

		private static Dictionary<DefaultCalendarColor, string> MapColorNames()
		{
			Dictionary<DefaultCalendarColor, string> dictionary = new Dictionary<DefaultCalendarColor, string>();
			dictionary[DefaultCalendarColor.NoneSet] = LocalizedStrings.GetNonEncoded(-789554764);
			dictionary[DefaultCalendarColor.Auto] = LocalizedStrings.GetNonEncoded(185042824);
			dictionary[DefaultCalendarColor.Brown] = LocalizedStrings.GetNonEncoded(1091142534);
			dictionary[DefaultCalendarColor.BrightGreen] = LocalizedStrings.GetNonEncoded(1509769609);
			dictionary[DefaultCalendarColor.Purple] = LocalizedStrings.GetNonEncoded(-415545442);
			dictionary[DefaultCalendarColor.TaupeDarkGrey] = LocalizedStrings.GetNonEncoded(-2088323323);
			dictionary[DefaultCalendarColor.KhakiGreen] = LocalizedStrings.GetNonEncoded(-40076381);
			dictionary[DefaultCalendarColor.CoralPink] = LocalizedStrings.GetNonEncoded(173435619);
			dictionary[DefaultCalendarColor.GrassGreen] = LocalizedStrings.GetNonEncoded(-242192433);
			dictionary[DefaultCalendarColor.PeriwinkleBlue] = LocalizedStrings.GetNonEncoded(16181640);
			dictionary[DefaultCalendarColor.TealGreen] = LocalizedStrings.GetNonEncoded(-1942954909);
			dictionary[DefaultCalendarColor.Magenta] = LocalizedStrings.GetNonEncoded(-195962327);
			dictionary[DefaultCalendarColor.DarkBlue] = LocalizedStrings.GetNonEncoded(-1300826558);
			dictionary[DefaultCalendarColor.SageGreen] = LocalizedStrings.GetNonEncoded(-2009804245);
			dictionary[DefaultCalendarColor.CamelBrown] = LocalizedStrings.GetNonEncoded(66432428);
			dictionary[DefaultCalendarColor.ElectricBlue] = LocalizedStrings.GetNonEncoded(916435529);
			dictionary[DefaultCalendarColor.Cinnamon] = LocalizedStrings.GetNonEncoded(1001882251);
			return dictionary;
		}

		private static bool InitializeConfiguration(IDictionary source, IDictionary target)
		{
			bool result = false;
			foreach (object obj in source)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (!target.Contains((int)dictionaryEntry.Key))
				{
					target.Add((int)dictionaryEntry.Key, (string)dictionaryEntry.Value);
					result = true;
				}
				else if (string.IsNullOrEmpty(target[(int)dictionaryEntry.Key] as string))
				{
					target[(int)dictionaryEntry.Key] = (string)source[(int)dictionaryEntry.Key];
					result = true;
				}
			}
			return result;
		}

		private static void ThrowIfCannotActAsOwner(UserContext userContext)
		{
			if (!userContext.CanActAsOwner)
			{
				throw new OwaAccessDeniedException(LocalizedStrings.GetNonEncoded(1622692336), true);
			}
		}

		private void Load(UserContext userContext)
		{
			try
			{
				this.config = userContext.MailboxSession.UserConfigurationManager.GetFolderConfiguration("IPM.Configuration.CalendarColorList", UserConfigurationTypes.Dictionary, userContext.CalendarFolderId);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.CalendarCallTracer.TraceDebug((long)this.GetHashCode(), "CalendarColorManager::Load. No existing Calendar colors list configuration was found. We are creating new one.");
				if (userContext.CanActAsOwner)
				{
					this.config = userContext.MailboxSession.UserConfigurationManager.CreateFolderConfiguration("IPM.Configuration.CalendarColorList", UserConfigurationTypes.Dictionary, userContext.CalendarFolderId);
				}
			}
			if (userContext.CanActAsOwner && CalendarColorManager.InitializeConfiguration(CalendarColorManager.colorNamesMap, this.config.GetDictionary()))
			{
				this.config.Save();
			}
		}

		private const string CalendarColorConfigurationName = "IPM.Configuration.CalendarColorList";

		private static readonly Dictionary<DefaultCalendarColor, string> colorNamesMap = CalendarColorManager.MapColorNames();

		private UserConfiguration config;
	}
}
