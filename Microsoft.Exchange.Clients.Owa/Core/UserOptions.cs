using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class UserOptions
	{
		private UserOptions(UserContext userContext, MailboxSession mailboxSession)
		{
			this.userContext = userContext;
			this.mailboxSession = mailboxSession;
			this.CreateOptionsProperties();
		}

		private void CreateOptionsProperties()
		{
			this.optionProperties = new Dictionary<UserOptionPropertyDefinition, UserOptions.UserOptionPropertyValue>();
			for (int i = 0; i < UserOptionPropertySchema.Count; i++)
			{
				this.optionProperties.Add(UserOptionPropertySchema.GetPropertyDefinition(i), null);
			}
		}

		public UserOptions(UserContext userContext) : this(userContext, null)
		{
		}

		internal static UserOptions CreateTemporaryInstance(MailboxSession mailboxSession)
		{
			return new UserOptions(null, mailboxSession);
		}

		private MailboxSession MailboxSession
		{
			get
			{
				if (this.userContext != null)
				{
					return this.userContext.MailboxSession;
				}
				return this.mailboxSession;
			}
		}

		public void ReloadAll()
		{
			this.CreateOptionsProperties();
			this.LoadAll();
		}

		public void LoadAll()
		{
			IList<UserOptionPropertyDefinition> properties = new List<UserOptionPropertyDefinition>(this.optionProperties.Keys);
			this.Load(properties);
			this.isSynced = true;
		}

		public void CommitChanges()
		{
			if (this.isSynced)
			{
				return;
			}
			IList<UserOptionPropertyDefinition> list = new List<UserOptionPropertyDefinition>();
			foreach (UserOptionPropertyDefinition userOptionPropertyDefinition in this.optionProperties.Keys)
			{
				if (this.optionProperties[userOptionPropertyDefinition] != null && this.optionProperties[userOptionPropertyDefinition].IsModified)
				{
					list.Add(userOptionPropertyDefinition);
				}
			}
			this.Commit(list);
			this.isSynced = true;
		}

		public bool IsSynced
		{
			get
			{
				return this.isSynced;
			}
		}

		private UserConfiguration GetUserConfiguration()
		{
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = this.MailboxSession.UserConfigurationManager.GetMailboxConfiguration("OWA.UserOptions", UserConfigurationTypes.Dictionary);
			}
			catch (AccessDeniedException ex)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Logon user does not have access permissions to user configuration object. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
				if (this.userContext.IsWebPartRequest)
				{
					return null;
				}
				throw;
			}
			catch (InvalidDataException ex2)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Recreating User Options due to invalid data. Error: {0}. Stack: {1}.", ex2.Message, ex2.StackTrace);
				return this.RecreateUserConfiguration(true);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug((long)this.GetHashCode(), "Creating User Options because it does not exist.");
				return this.RecreateUserConfiguration(false);
			}
			bool flag = false;
			UserConfiguration result;
			try
			{
				userConfiguration.GetDictionary();
				flag = true;
				result = userConfiguration;
			}
			catch (InvalidOperationException ex3)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Recreating User Options due to no dictionary in UserConfiguration. Error: {0}. Stack: {1}.", ex3.Message, ex3.StackTrace);
				result = this.RecreateUserConfiguration(true);
			}
			catch (InvalidDataException ex4)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Recreating User Options due corrupt dictionary. Error: {0}. Stack: {1}.", ex4.Message, ex4.StackTrace);
				result = this.RecreateUserConfiguration(true);
			}
			finally
			{
				if (!flag && userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
			}
			return result;
		}

		private UserConfiguration RecreateUserConfiguration(bool deleteFirst)
		{
			UserConfiguration userConfiguration = null;
			bool flag = false;
			UserConfiguration result;
			try
			{
				if (deleteFirst)
				{
					this.MailboxSession.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
					{
						"OWA.UserOptions"
					});
				}
				userConfiguration = this.MailboxSession.UserConfigurationManager.CreateMailboxConfiguration("OWA.UserOptions", UserConfigurationTypes.Dictionary);
				userConfiguration.Save();
				flag = true;
				result = userConfiguration;
			}
			catch (StoragePermanentException ex)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to recreate configuration data. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
				throw;
			}
			catch (StorageTransientException ex2)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to recreate configuration data. Error: {0}. Stack: {1}.", ex2.Message, ex2.StackTrace);
				throw;
			}
			finally
			{
				if (!flag && userConfiguration != null)
				{
					userConfiguration.Dispose();
				}
			}
			return result;
		}

		private void Load(IList<UserOptionPropertyDefinition> properties)
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration())
			{
				if (userConfiguration != null)
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					for (int i = 0; i < properties.Count; i++)
					{
						UserOptionPropertyDefinition userOptionPropertyDefinition = properties[i];
						string propertyName = userOptionPropertyDefinition.PropertyName;
						object originalValue = dictionary[userOptionPropertyDefinition.PropertyName];
						this.optionProperties[userOptionPropertyDefinition] = new UserOptions.UserOptionPropertyValue(userOptionPropertyDefinition.GetValidatedProperty(originalValue), false);
						ExTraceGlobals.UserOptionsDataTracer.TraceDebug((long)this.GetHashCode(), "Loaded property: {0}", new object[]
						{
							this.optionProperties[userOptionPropertyDefinition].Value
						});
					}
				}
				else
				{
					string value = WebPartUtilities.TryGetLocalMachineTimeZone();
					UserOptionPropertyDefinition propertyDefinition = UserOptionPropertySchema.GetPropertyDefinition(UserOptionPropertySchema.UserOptionPropertyID.TimeZone);
					this.optionProperties[propertyDefinition] = new UserOptions.UserOptionPropertyValue(value, false);
				}
			}
		}

		private void Commit(IList<UserOptionPropertyDefinition> properties)
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration())
			{
				IDictionary dictionary = userConfiguration.GetDictionary();
				Type typeFromHandle = typeof(int);
				for (int i = 0; i < properties.Count; i++)
				{
					UserOptionPropertyDefinition userOptionPropertyDefinition = properties[i];
					string propertyName = userOptionPropertyDefinition.PropertyName;
					if (userOptionPropertyDefinition.PropertyType == typeFromHandle)
					{
						dictionary[userOptionPropertyDefinition.PropertyName] = (int)this.optionProperties[userOptionPropertyDefinition].Value;
					}
					else
					{
						dictionary[userOptionPropertyDefinition.PropertyName] = this.optionProperties[userOptionPropertyDefinition].Value;
					}
					this.optionProperties[userOptionPropertyDefinition].IsModified = false;
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug((long)this.GetHashCode(), "Committed property: {0}", new object[]
					{
						this.optionProperties[userOptionPropertyDefinition].Value
					});
				}
				try
				{
					userConfiguration.Save();
				}
				catch (StoragePermanentException ex)
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to save configuration data. Error: {0}. Stack: {1}.", ex.Message, ex.StackTrace);
					throw;
				}
				catch (StorageTransientException ex2)
				{
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Failed to save configuration data. Error: {0}. Stack: {1}.", ex2.Message, ex2.StackTrace);
					throw;
				}
			}
		}

		private object this[UserOptionPropertySchema.UserOptionPropertyID propertyID]
		{
			get
			{
				UserOptionPropertyDefinition propertyDefinition = UserOptionPropertySchema.GetPropertyDefinition(propertyID);
				object obj;
				if (this.optionProperties.ContainsKey(propertyDefinition) && this.optionProperties[propertyDefinition] != null)
				{
					obj = this.optionProperties[propertyDefinition].Value;
				}
				else
				{
					obj = propertyDefinition.GetValidatedProperty(null);
				}
				ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, object>((long)this.GetHashCode(), "Get property: '{0}'; value: '{1}'", propertyDefinition.PropertyName, obj);
				return obj;
			}
			set
			{
				UserOptionPropertyDefinition propertyDefinition = UserOptionPropertySchema.GetPropertyDefinition(propertyID);
				UserOptions.UserOptionPropertyValue userOptionPropertyValue = new UserOptions.UserOptionPropertyValue(propertyDefinition.GetValidatedProperty(value), true);
				if (!this.optionProperties.ContainsKey(propertyDefinition))
				{
					this.optionProperties.Add(propertyDefinition, userOptionPropertyValue);
					this.isSynced = false;
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, object>((long)this.GetHashCode(), "Set property: '{0}'; value: '{1}'", propertyDefinition.PropertyName, this.optionProperties[propertyDefinition].Value);
					return;
				}
				if (this.optionProperties[propertyDefinition] == null || !userOptionPropertyValue.Value.Equals(this.optionProperties[propertyDefinition].Value))
				{
					this.optionProperties[propertyDefinition] = userOptionPropertyValue;
					this.isSynced = false;
					ExTraceGlobals.UserOptionsDataTracer.TraceDebug<string, object>((long)this.GetHashCode(), "Set property: '{0}'; value: '{1}'", propertyDefinition.PropertyName, this.optionProperties[propertyDefinition].Value);
				}
			}
		}

		public string TimeZone
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.TimeZone];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.TimeZone] = value;
			}
		}

		public string TimeFormat
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.TimeFormat];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.TimeFormat] = value;
			}
		}

		public string DateFormat
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.DateFormat];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.DateFormat] = value;
			}
		}

		public string GetDateFormatNoYear()
		{
			Match match = UserOptions.dateFormatNoYearRegEx.Match(this.DateFormat);
			if (match.Success)
			{
				return match.Groups[0].Value;
			}
			return this.DateFormat;
		}

		public DayOfWeek WeekStartDay
		{
			get
			{
				return (DayOfWeek)this[UserOptionPropertySchema.UserOptionPropertyID.WeekStartDay];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.WeekStartDay] = value;
			}
		}

		public int HourIncrement
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.HourIncrement];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.HourIncrement] = value;
			}
		}

		public bool ShowWeekNumbers
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.ShowWeekNumbers];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ShowWeekNumbers] = value;
			}
		}

		public bool CheckNameInContactsFirst
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.CheckNameInContactsFirst] || !this.userContext.IsFeatureEnabled(Feature.GlobalAddressList);
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.CheckNameInContactsFirst] = value;
			}
		}

		public string ThemeStorageId
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.ThemeStorageId];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ThemeStorageId] = value;
			}
		}

		public int FirstWeekOfYear
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.FirstWeekOfYear];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.FirstWeekOfYear] = value;
			}
		}

		public bool EnableReminders
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.EnableReminders];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.EnableReminders] = value;
			}
		}

		public bool EnableReminderSound
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.EnableReminderSound];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.EnableReminderSound] = value;
			}
		}

		public NewNotification NewItemNotify
		{
			get
			{
				return (NewNotification)this[UserOptionPropertySchema.UserOptionPropertyID.NewItemNotify];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.NewItemNotify] = value;
			}
		}

		public int ViewRowCount
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.ViewRowCount];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ViewRowCount] = value;
			}
		}

		public int BasicViewRowCount
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.BasicViewRowCount];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.BasicViewRowCount] = value;
			}
		}

		public int SpellingDictionaryLanguage
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.SpellingDictionaryLanguage];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SpellingDictionaryLanguage] = value;
			}
		}

		public bool SpellingIgnoreUppercase
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.SpellingIgnoreUppercase];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SpellingIgnoreUppercase] = value;
			}
		}

		public bool SpellingIgnoreMixedDigits
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.SpellingIgnoreMixedDigits];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SpellingIgnoreMixedDigits] = value;
			}
		}

		public bool SpellingCheckBeforeSend
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.SpellingCheckBeforeSend];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SpellingCheckBeforeSend] = value;
			}
		}

		public bool SmimeEncrypt
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.SmimeEncrypt];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SmimeEncrypt] = value;
			}
		}

		public bool SmimeSign
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.SmimeSign];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SmimeSign] = value;
			}
		}

		public bool AlwaysShowBcc
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.AlwaysShowBcc];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.AlwaysShowBcc] = value;
			}
		}

		public bool AlwaysShowFrom
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.AlwaysShowFrom];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.AlwaysShowFrom] = value;
			}
		}

		public Markup ComposeMarkup
		{
			get
			{
				return (Markup)this[UserOptionPropertySchema.UserOptionPropertyID.ComposeMarkup];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ComposeMarkup] = value;
			}
		}

		public string ComposeFontName
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontName];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontName] = value;
			}
		}

		public int ComposeFontSize
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontSize];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontSize] = value;
			}
		}

		public string ComposeFontColor
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontColor];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontColor] = value;
			}
		}

		public FontFlags ComposeFontFlags
		{
			get
			{
				return (FontFlags)this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontFlags];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ComposeFontFlags] = value;
			}
		}

		public bool AutoAddSignature
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.AutoAddSignature];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.AutoAddSignature] = value;
			}
		}

		public string SignatureText
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.SignatureText];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SignatureText] = value;
			}
		}

		public string SignatureHtml
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.SignatureHtml];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SignatureHtml] = value;
			}
		}

		public bool BlockExternalContent
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.BlockExternalContent];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.BlockExternalContent] = value;
			}
		}

		public MarkAsRead PreviewMarkAsRead
		{
			get
			{
				return (MarkAsRead)this[UserOptionPropertySchema.UserOptionPropertyID.PreviewMarkAsRead];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.PreviewMarkAsRead] = value;
			}
		}

		public int MarkAsReadDelaytime
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.MarkAsReadDelaytime];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.MarkAsReadDelaytime] = value;
			}
		}

		public NextSelectionDirection NextSelection
		{
			get
			{
				return (NextSelectionDirection)this[UserOptionPropertySchema.UserOptionPropertyID.NextSelection];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.NextSelection] = value;
			}
		}

		public ReadReceiptResponse ReadReceipt
		{
			get
			{
				return (ReadReceiptResponse)this[UserOptionPropertySchema.UserOptionPropertyID.ReadReceipt];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ReadReceipt] = value;
			}
		}

		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.EmptyDeletedItemsOnLogoff];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.EmptyDeletedItemsOnLogoff] = value;
			}
		}

		public int NavigationBarWidth
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.NavigationBarWidth];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.NavigationBarWidth] = value;
			}
		}

		public bool IsMiniBarVisible
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.IsMiniBarVisible];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.IsMiniBarVisible] = value;
			}
		}

		public bool IsQuickLinksBarVisible
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.IsQuickLinksBarVisible];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.IsQuickLinksBarVisible] = value;
			}
		}

		public bool IsTaskDetailsVisible
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.IsTaskDetailsVisible];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.IsTaskDetailsVisible] = value;
			}
		}

		public bool IsDocumentFavoritesVisible
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.IsDocumentFavoritesVisible];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.IsDocumentFavoritesVisible] = value;
			}
		}

		public FormatBarButtonGroups FormatBarState
		{
			get
			{
				return (FormatBarButtonGroups)this[UserOptionPropertySchema.UserOptionPropertyID.FormatBarState];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.FormatBarState] = value;
			}
		}

		public string MruFonts
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.MruFonts];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.MruFonts] = value;
			}
		}

		public bool PrimaryNavigationCollapsed
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.PrimaryNavigationCollapsed];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.PrimaryNavigationCollapsed] = value;
			}
		}

		public bool MailFindBarOn
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.MailFindBarOn];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.MailFindBarOn] = value;
			}
		}

		public bool CalendarFindBarOn
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.CalendarFindBarOn];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.CalendarFindBarOn] = value;
			}
		}

		public bool ContactsFindBarOn
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.ContactsFindBarOn];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ContactsFindBarOn] = value;
			}
		}

		public string SendAddressDefault
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.SendAddressDefault];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SendAddressDefault] = value;
			}
		}

		internal SearchScope GetSearchScope(OutlookModule outlookModule)
		{
			if (!this.MailboxSession.Mailbox.IsContentIndexingEnabled)
			{
				return SearchScope.SelectedFolder;
			}
			switch (outlookModule)
			{
			case OutlookModule.Tasks:
				return (SearchScope)this[UserOptionPropertySchema.UserOptionPropertyID.TasksSearchScope];
			case OutlookModule.Contacts:
				return (SearchScope)this[UserOptionPropertySchema.UserOptionPropertyID.ContactsSearchScope];
			default:
				return (SearchScope)this[UserOptionPropertySchema.UserOptionPropertyID.SearchScope];
			}
		}

		internal void SetSearchScope(OutlookModule outlookModule, SearchScope searchScope)
		{
			switch (outlookModule)
			{
			case OutlookModule.Tasks:
				this[UserOptionPropertySchema.UserOptionPropertyID.TasksSearchScope] = searchScope;
				return;
			case OutlookModule.Contacts:
				this[UserOptionPropertySchema.UserOptionPropertyID.ContactsSearchScope] = searchScope;
				return;
			default:
				this[UserOptionPropertySchema.UserOptionPropertyID.SearchScope] = searchScope;
				return;
			}
		}

		public bool IsOptimizedForAccessibility
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.IsOptimizedForAccessibility];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.IsOptimizedForAccessibility] = value;
			}
		}

		public PontType EnabledPonts
		{
			get
			{
				return (PontType)this[UserOptionPropertySchema.UserOptionPropertyID.NewEnabledPonts];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.NewEnabledPonts] = value;
			}
		}

		public bool IsPontEnabled(PontType pontType)
		{
			return (this.EnabledPonts & pontType) == pontType;
		}

		public FlagAction FlagAction
		{
			get
			{
				return (FlagAction)this[UserOptionPropertySchema.UserOptionPropertyID.FlagAction];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.FlagAction] = value;
			}
		}

		public bool AddRecipientsToAutoCompleteCache
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.AddRecipientsToAutoCompleteCache];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.AddRecipientsToAutoCompleteCache] = value;
			}
		}

		private string FormatDateTime(string dateTimeFormat, bool useFullWeekdayFormat)
		{
			string text = useFullWeekdayFormat ? "dddd" : "ddd";
			CultureInfo userCulture = this.userContext.UserCulture;
			int lcid = userCulture.LCID;
			switch (lcid)
			{
			case 1041:
				if (dateTimeFormat.Contains("ddd"))
				{
					return dateTimeFormat;
				}
				return dateTimeFormat + " (" + text + ")";
			case 1042:
				break;
			default:
				if (lcid != 1055 && lcid != 1063)
				{
					return text + " " + dateTimeFormat;
				}
				break;
			}
			return dateTimeFormat + " (" + text + ")";
		}

		public string GetWeekdayDateTimeFormat(bool useFullWeekdayFormat)
		{
			return this.GetWeekdayDateFormat(useFullWeekdayFormat) + " " + this.TimeFormat;
		}

		public string GetWeekdayDateFormat(bool useFullWeekdayFormat)
		{
			return this.FormatDateTime(this.DateFormat, useFullWeekdayFormat);
		}

		public string GetWeekdayTimeFormat(bool useFullWeekdayFormat)
		{
			return this.FormatDateTime(this.TimeFormat, useFullWeekdayFormat);
		}

		public string GetWeekdayDateNoYearFormat(bool useFullWeekdayFormat)
		{
			return this.FormatDateTime(this.GetDateFormatNoYear(), useFullWeekdayFormat);
		}

		public bool ManuallyPickCertificate
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.ManuallyPickCertificate];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ManuallyPickCertificate] = value;
			}
		}

		public string SigningCertificateSubject
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.SigningCertificateSubject];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SigningCertificateSubject] = value;
			}
		}

		public string SigningCertificateId
		{
			get
			{
				return (string)this[UserOptionPropertySchema.UserOptionPropertyID.SigningCertificateId];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.SigningCertificateId] = value;
			}
		}

		public bool UseManuallyPickedSigningCertificate
		{
			get
			{
				return OwaRegistryKeys.AllowUserChoiceOfSigningCertificate && this.ManuallyPickCertificate && !string.IsNullOrEmpty(this.SigningCertificateId);
			}
		}

		public int UseDataCenterCustomTheme
		{
			get
			{
				return (int)this[UserOptionPropertySchema.UserOptionPropertyID.UseDataCenterCustomTheme];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.UseDataCenterCustomTheme] = value;
			}
		}

		public ConversationSortOrder ConversationSortOrder
		{
			get
			{
				return (ConversationSortOrder)this[UserOptionPropertySchema.UserOptionPropertyID.ConversationSortOrder];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ConversationSortOrder] = (int)value;
			}
		}

		public bool ShowTreeInListView
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.ShowTreeInListView];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.ShowTreeInListView] = value;
			}
		}

		public bool HideDeletedItems
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.HideDeletedItems];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.HideDeletedItems] = value;
			}
		}

		public bool HideMailTipsByDefault
		{
			get
			{
				return (bool)this[UserOptionPropertySchema.UserOptionPropertyID.HideMailTipsByDefault];
			}
			set
			{
				this[UserOptionPropertySchema.UserOptionPropertyID.HideMailTipsByDefault] = value;
			}
		}

		private const string ConfigurationName = "OWA.UserOptions";

		private const string DateFormatNoYearExpression = "(M{1,2}[\\/\\.\\- ]d{1,2})|(d{1,2}[\\/\\.\\- ]M{1,2})";

		private static readonly Regex dateFormatNoYearRegEx = new Regex("(M{1,2}[\\/\\.\\- ]d{1,2})|(d{1,2}[\\/\\.\\- ]M{1,2})", RegexOptions.Compiled);

		private MailboxSession mailboxSession;

		private UserContext userContext;

		private bool isSynced;

		private Dictionary<UserOptionPropertyDefinition, UserOptions.UserOptionPropertyValue> optionProperties;

		private class UserOptionPropertyValue
		{
			internal UserOptionPropertyValue(object value, bool isModified)
			{
				this.propertyValue = value;
				this.isModified = isModified;
			}

			internal object Value
			{
				get
				{
					return this.propertyValue;
				}
			}

			internal bool IsModified
			{
				get
				{
					return this.isModified;
				}
				set
				{
					this.isModified = value;
				}
			}

			private object propertyValue;

			private bool isModified;
		}
	}
}
