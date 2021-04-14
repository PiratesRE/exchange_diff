using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UserOptionsType : UserConfigurationBaseType
	{
		public UserOptionsType() : base("OWA.UserOptions")
		{
		}

		private static ExTimeZone LegacyDataCenterTimeZone
		{
			get
			{
				if (UserOptionsType.legacyDataCenterTimeZone == null)
				{
					ExTimeZone exTimeZone;
					ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName("Greenwich Standard Time", out exTimeZone);
					UserOptionsType.legacyDataCenterTimeZone = exTimeZone;
				}
				return UserOptionsType.legacyDataCenterTimeZone;
			}
		}

		[DataMember]
		public string TimeZone
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.TimeZone];
			}
			set
			{
				base[UserConfigurationPropertyId.TimeZone] = value;
			}
		}

		[DataMember]
		public string TimeFormat
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.TimeFormat];
			}
			set
			{
				base[UserConfigurationPropertyId.TimeFormat] = value;
			}
		}

		[DataMember]
		public string DateFormat
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.DateFormat];
			}
			set
			{
				base[UserConfigurationPropertyId.DateFormat] = value;
			}
		}

		[DataMember]
		public int WeekStartDay
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.WeekStartDay];
			}
			set
			{
				base[UserConfigurationPropertyId.WeekStartDay] = value;
			}
		}

		[DataMember]
		public int HourIncrement
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.HourIncrement];
			}
			set
			{
				base[UserConfigurationPropertyId.HourIncrement] = value;
			}
		}

		[DataMember]
		public bool ShowWeekNumbers
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowWeekNumbers];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowWeekNumbers] = value;
			}
		}

		[DataMember]
		public bool CheckNameInContactsFirst
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CheckNameInContactsFirst];
			}
			set
			{
				base[UserConfigurationPropertyId.CheckNameInContactsFirst] = value;
			}
		}

		[DataMember]
		public CalendarWeekRule FirstWeekOfYear
		{
			get
			{
				return ((FirstWeekRules)base[UserConfigurationPropertyId.FirstWeekOfYear]).ToCalendarWeekRule();
			}
			set
			{
				base[UserConfigurationPropertyId.FirstWeekOfYear] = value.ToFirstWeekRules();
			}
		}

		[DataMember]
		public bool EnableReminders
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.EnableReminders];
			}
			set
			{
				base[UserConfigurationPropertyId.EnableReminders] = value;
			}
		}

		[DataMember]
		public bool EnableReminderSound
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.EnableReminderSound];
			}
			set
			{
				base[UserConfigurationPropertyId.EnableReminderSound] = value;
			}
		}

		[DataMember]
		public NewNotification NewItemNotify
		{
			get
			{
				return (NewNotification)base[UserConfigurationPropertyId.NewItemNotify];
			}
			set
			{
				base[UserConfigurationPropertyId.NewItemNotify] = value;
			}
		}

		[DataMember]
		public int ViewRowCount
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.ViewRowCount];
			}
			set
			{
				base[UserConfigurationPropertyId.ViewRowCount] = value;
			}
		}

		[DataMember]
		public int SpellingDictionaryLanguage
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.SpellingDictionaryLanguage];
			}
			set
			{
				base[UserConfigurationPropertyId.SpellingDictionaryLanguage] = value;
			}
		}

		[DataMember]
		public bool SpellingIgnoreUppercase
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.SpellingIgnoreUppercase];
			}
			set
			{
				base[UserConfigurationPropertyId.SpellingIgnoreUppercase] = value;
			}
		}

		[DataMember]
		public bool SpellingIgnoreMixedDigits
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.SpellingIgnoreMixedDigits];
			}
			set
			{
				base[UserConfigurationPropertyId.SpellingIgnoreMixedDigits] = value;
			}
		}

		[DataMember]
		public bool SpellingCheckBeforeSend
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.SpellingCheckBeforeSend];
			}
			set
			{
				base[UserConfigurationPropertyId.SpellingCheckBeforeSend] = value;
			}
		}

		[DataMember]
		public bool SmimeEncrypt
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.SmimeEncrypt];
			}
			set
			{
				base[UserConfigurationPropertyId.SmimeEncrypt] = value;
			}
		}

		[DataMember]
		public bool SmimeSign
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.SmimeSign];
			}
			set
			{
				base[UserConfigurationPropertyId.SmimeSign] = value;
			}
		}

		[DataMember]
		public bool AlwaysShowBcc
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AlwaysShowBcc];
			}
			set
			{
				base[UserConfigurationPropertyId.AlwaysShowBcc] = value;
			}
		}

		[DataMember]
		public bool AlwaysShowFrom
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AlwaysShowFrom];
			}
			set
			{
				base[UserConfigurationPropertyId.AlwaysShowFrom] = value;
			}
		}

		public Markup ComposeMarkup
		{
			get
			{
				return (Markup)base[UserConfigurationPropertyId.ComposeMarkup];
			}
			set
			{
				base[UserConfigurationPropertyId.ComposeMarkup] = value;
			}
		}

		[DataMember(Name = "ComposeMarkup")]
		public string ComposeMarkupString
		{
			get
			{
				return this.ComposeMarkup.ToString();
			}
			set
			{
				this.ComposeMarkup = (Markup)Enum.Parse(typeof(Markup), value);
			}
		}

		[DataMember]
		public string ComposeFontName
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ComposeFontName];
			}
			set
			{
				base[UserConfigurationPropertyId.ComposeFontName] = value;
			}
		}

		[DataMember]
		public int ComposeFontSize
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.ComposeFontSize];
			}
			set
			{
				base[UserConfigurationPropertyId.ComposeFontSize] = value;
			}
		}

		[DataMember]
		public string ComposeFontColor
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ComposeFontColor];
			}
			set
			{
				base[UserConfigurationPropertyId.ComposeFontColor] = value;
			}
		}

		[DataMember(Name = "ComposeFontFlags")]
		public FontFlags ComposeFontFlags
		{
			get
			{
				return (FontFlags)base[UserConfigurationPropertyId.ComposeFontFlags];
			}
			set
			{
				base[UserConfigurationPropertyId.ComposeFontFlags] = value;
			}
		}

		[DataMember]
		public bool AutoAddSignature
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AutoAddSignature];
			}
			set
			{
				base[UserConfigurationPropertyId.AutoAddSignature] = value;
			}
		}

		[DataMember]
		public string SignatureText
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SignatureText];
			}
			set
			{
				base[UserConfigurationPropertyId.SignatureText] = value;
			}
		}

		[DataMember]
		public string SignatureHtml
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SignatureHtml];
			}
			set
			{
				base[UserConfigurationPropertyId.SignatureHtml] = value;
			}
		}

		[DataMember]
		public bool AutoAddSignatureOnMobile
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AutoAddSignatureOnMobile];
			}
			set
			{
				base[UserConfigurationPropertyId.AutoAddSignatureOnMobile] = value;
			}
		}

		[DataMember]
		public string SignatureTextOnMobile
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SignatureTextOnMobile];
			}
			set
			{
				base[UserConfigurationPropertyId.SignatureTextOnMobile] = value;
			}
		}

		[DataMember]
		public bool UseDesktopSignature
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.UseDesktopSignature];
			}
			set
			{
				base[UserConfigurationPropertyId.UseDesktopSignature] = value;
			}
		}

		[DataMember]
		public bool BlockExternalContent
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.BlockExternalContent];
			}
			set
			{
				base[UserConfigurationPropertyId.BlockExternalContent] = value;
			}
		}

		public MarkAsRead PreviewMarkAsRead
		{
			get
			{
				return (MarkAsRead)base[UserConfigurationPropertyId.PreviewMarkAsRead];
			}
			set
			{
				base[UserConfigurationPropertyId.PreviewMarkAsRead] = value;
			}
		}

		[DataMember(Name = "PreviewMarkAsRead")]
		public string PreviewMarkAsReadString
		{
			get
			{
				return this.PreviewMarkAsRead.ToString();
			}
			set
			{
				this.PreviewMarkAsRead = (MarkAsRead)Enum.Parse(typeof(MarkAsRead), value);
			}
		}

		public EmailComposeMode EmailComposeMode
		{
			get
			{
				return (EmailComposeMode)base[UserConfigurationPropertyId.EmailComposeMode];
			}
			set
			{
				base[UserConfigurationPropertyId.EmailComposeMode] = value;
			}
		}

		[DataMember(Name = "EmailComposeMode")]
		public string EmailComposeModeString
		{
			get
			{
				return this.EmailComposeMode.ToString();
			}
			set
			{
				this.EmailComposeMode = (EmailComposeMode)Enum.Parse(typeof(EmailComposeMode), value);
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] SendAsMruAddresses
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.SendAsMruAddresses];
			}
			set
			{
				base[UserConfigurationPropertyId.SendAsMruAddresses] = value;
			}
		}

		[DataMember]
		public bool CheckForForgottenAttachments
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CheckForForgottenAttachments];
			}
			set
			{
				base[UserConfigurationPropertyId.CheckForForgottenAttachments] = value;
			}
		}

		[DataMember]
		public int MarkAsReadDelaytime
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.MarkAsReadDelaytime];
			}
			set
			{
				base[UserConfigurationPropertyId.MarkAsReadDelaytime] = value;
			}
		}

		public NextSelectionDirection NextSelection
		{
			get
			{
				return (NextSelectionDirection)base[UserConfigurationPropertyId.NextSelection];
			}
			set
			{
				base[UserConfigurationPropertyId.NextSelection] = value;
			}
		}

		[DataMember(Name = "NextSelection")]
		public string NextSelectionString
		{
			get
			{
				return this.NextSelection.ToString();
			}
			set
			{
				this.NextSelection = (NextSelectionDirection)Enum.Parse(typeof(NextSelectionDirection), value);
			}
		}

		public ReadReceiptResponse ReadReceipt
		{
			get
			{
				return (ReadReceiptResponse)base[UserConfigurationPropertyId.ReadReceipt];
			}
			set
			{
				base[UserConfigurationPropertyId.ReadReceipt] = value;
			}
		}

		[DataMember(Name = "ReadReceipt")]
		public string ReadReceiptString
		{
			get
			{
				return this.ReadReceipt.ToString();
			}
			set
			{
				this.ReadReceipt = (ReadReceiptResponse)Enum.Parse(typeof(ReadReceiptResponse), value);
			}
		}

		[DataMember]
		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.EmptyDeletedItemsOnLogoff];
			}
			set
			{
				base[UserConfigurationPropertyId.EmptyDeletedItemsOnLogoff] = value;
			}
		}

		[DataMember]
		public int NavigationBarWidth
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.NavigationBarWidth];
			}
			set
			{
				base[UserConfigurationPropertyId.NavigationBarWidth] = value;
			}
		}

		[DataMember]
		public string NavigationBarWidthRatio
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.NavigationBarWidthRatio];
			}
			set
			{
				base[UserConfigurationPropertyId.NavigationBarWidthRatio] = value;
			}
		}

		[DataMember]
		public bool MailFolderPaneExpanded
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.MailFolderPaneExpanded];
			}
			set
			{
				base[UserConfigurationPropertyId.MailFolderPaneExpanded] = value;
			}
		}

		[DataMember]
		public bool IsFavoritesFolderTreeCollapsed
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsFavoritesFolderTreeCollapsed];
			}
			set
			{
				base[UserConfigurationPropertyId.IsFavoritesFolderTreeCollapsed] = value;
			}
		}

		[DataMember]
		public bool IsPeopleIKnowFolderTreeCollapsed
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsPeopleIKnowFolderTreeCollapsed];
			}
			set
			{
				base[UserConfigurationPropertyId.IsPeopleIKnowFolderTreeCollapsed] = value;
			}
		}

		[DataMember]
		public bool ShowReadingPaneOnFirstLoad
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowReadingPaneOnFirstLoad];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowReadingPaneOnFirstLoad] = value;
			}
		}

		[DataMember]
		public NavigationPaneView NavigationPaneViewOption
		{
			get
			{
				return (NavigationPaneView)base[UserConfigurationPropertyId.NavigationPaneViewOption];
			}
			set
			{
				base[UserConfigurationPropertyId.NavigationPaneViewOption] = value;
			}
		}

		[DataMember]
		public bool IsMailRootFolderTreeCollapsed
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsMailRootFolderTreeCollapsed];
			}
			set
			{
				base[UserConfigurationPropertyId.IsMailRootFolderTreeCollapsed] = value;
			}
		}

		[DataMember]
		public bool IsMiniBarVisible
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsMiniBarVisible];
			}
			set
			{
				base[UserConfigurationPropertyId.IsMiniBarVisible] = value;
			}
		}

		[DataMember]
		public bool IsQuickLinksBarVisible
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsQuickLinksBarVisible];
			}
			set
			{
				base[UserConfigurationPropertyId.IsQuickLinksBarVisible] = value;
			}
		}

		[DataMember]
		public bool IsTaskDetailsVisible
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsTaskDetailsVisible];
			}
			set
			{
				base[UserConfigurationPropertyId.IsTaskDetailsVisible] = value;
			}
		}

		[DataMember]
		public bool IsDocumentFavoritesVisible
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsDocumentFavoritesVisible];
			}
			set
			{
				base[UserConfigurationPropertyId.IsDocumentFavoritesVisible] = value;
			}
		}

		[DataMember]
		public bool IsOutlookSharedFoldersVisible { get; set; }

		[DataMember(Name = "FormatBarState")]
		public FormatBarButtonGroups FormatBarState
		{
			get
			{
				return (FormatBarButtonGroups)base[UserConfigurationPropertyId.FormatBarState];
			}
			set
			{
				base[UserConfigurationPropertyId.FormatBarState] = value;
			}
		}

		[DataMember]
		public string[] MruFonts
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.MruFonts];
			}
			set
			{
				base[UserConfigurationPropertyId.MruFonts] = value;
			}
		}

		[DataMember]
		public bool PrimaryNavigationCollapsed
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.PrimaryNavigationCollapsed];
			}
			set
			{
				base[UserConfigurationPropertyId.PrimaryNavigationCollapsed] = value;
			}
		}

		[DataMember]
		public string ThemeStorageId
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ThemeStorageId];
			}
			set
			{
				base[UserConfigurationPropertyId.ThemeStorageId] = value;
			}
		}

		[DataMember]
		public bool MailFindBarOn
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.MailFindBarOn];
			}
			set
			{
				base[UserConfigurationPropertyId.MailFindBarOn] = value;
			}
		}

		[DataMember]
		public bool CalendarFindBarOn
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CalendarFindBarOn];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarFindBarOn] = value;
			}
		}

		[DataMember]
		public bool ContactsFindBarOn
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ContactsFindBarOn];
			}
			set
			{
				base[UserConfigurationPropertyId.ContactsFindBarOn] = value;
			}
		}

		[DataMember]
		public int SearchScope
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.SearchScope];
			}
			set
			{
				base[UserConfigurationPropertyId.SearchScope] = value;
			}
		}

		[DataMember]
		public int ContactsSearchScope
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.ContactsSearchScope];
			}
			set
			{
				base[UserConfigurationPropertyId.ContactsSearchScope] = value;
			}
		}

		[DataMember]
		public int TasksSearchScope
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.TasksSearchScope];
			}
			set
			{
				base[UserConfigurationPropertyId.TasksSearchScope] = value;
			}
		}

		[DataMember]
		public bool IsOptimizedForAccessibility
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsOptimizedForAccessibility];
			}
			set
			{
				base[UserConfigurationPropertyId.IsOptimizedForAccessibility] = value;
			}
		}

		[DataMember]
		public PontType NewEnabledPonts
		{
			get
			{
				return (PontType)base[UserConfigurationPropertyId.NewEnabledPonts];
			}
			set
			{
				base[UserConfigurationPropertyId.NewEnabledPonts] = value;
			}
		}

		public FlagAction FlagAction
		{
			get
			{
				return (FlagAction)base[UserConfigurationPropertyId.FlagAction];
			}
			set
			{
				base[UserConfigurationPropertyId.FlagAction] = value;
			}
		}

		[DataMember(Name = "FlagAction")]
		public string FlagActionString
		{
			get
			{
				return this.FlagAction.ToString();
			}
			set
			{
				this.FlagAction = (FlagAction)Enum.Parse(typeof(FlagAction), value);
			}
		}

		[DataMember]
		public bool AddRecipientsToAutoCompleteCache
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.AddRecipientsToAutoCompleteCache];
			}
			set
			{
				base[UserConfigurationPropertyId.AddRecipientsToAutoCompleteCache] = value;
			}
		}

		[DataMember]
		public bool ManuallyPickCertificate
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ManuallyPickCertificate];
			}
			set
			{
				base[UserConfigurationPropertyId.ManuallyPickCertificate] = value;
			}
		}

		[DataMember]
		public string SigningCertificateSubject
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SigningCertificateSubject];
			}
			set
			{
				base[UserConfigurationPropertyId.SigningCertificateSubject] = value;
			}
		}

		[DataMember]
		public string SigningCertificateId
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SigningCertificateId];
			}
			set
			{
				base[UserConfigurationPropertyId.SigningCertificateId] = value;
			}
		}

		[DataMember]
		public int UseDataCenterCustomTheme
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.UseDataCenterCustomTheme];
			}
			set
			{
				base[UserConfigurationPropertyId.UseDataCenterCustomTheme] = value;
			}
		}

		[DataMember(Name = "ConversationSortOrder")]
		public string ConversationSortOrderString
		{
			get
			{
				return this.ConversationSortOrder.ToString();
			}
			set
			{
				this.ConversationSortOrder = (ConversationSortOrder)Enum.Parse(typeof(ConversationSortOrder), value);
			}
		}

		public ConversationSortOrder ConversationSortOrder
		{
			get
			{
				return (ConversationSortOrder)base[UserConfigurationPropertyId.ConversationSortOrder];
			}
			set
			{
				base[UserConfigurationPropertyId.ConversationSortOrder] = value;
			}
		}

		[DataMember]
		public bool ShowTreeInListView
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowTreeInListView];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowTreeInListView] = value;
			}
		}

		[DataMember]
		public bool HideDeletedItems
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HideDeletedItems];
			}
			set
			{
				base[UserConfigurationPropertyId.HideDeletedItems] = value;
			}
		}

		[DataMember]
		public bool HideMailTipsByDefault
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HideMailTipsByDefault];
			}
			set
			{
				base[UserConfigurationPropertyId.HideMailTipsByDefault] = value;
			}
		}

		[DataMember]
		public string SendAddressDefault
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.SendAddressDefault];
			}
			set
			{
				base[UserConfigurationPropertyId.SendAddressDefault] = value;
			}
		}

		[DataMember]
		public WorkingHoursType WorkingHours
		{
			get
			{
				return this.workingHours;
			}
			set
			{
				this.workingHours = value;
			}
		}

		[DataMember]
		public int DefaultReminderTimeInMinutes
		{
			get
			{
				return this.defaultReminderTimeInMinutes;
			}
			set
			{
				this.defaultReminderTimeInMinutes = value;
			}
		}

		[DataMember]
		public TimeZoneOffsetsType[] MailboxTimeZoneOffset
		{
			get
			{
				if (this.mailboxTimeZoneOffset == null)
				{
					ExDateTime startTime = ExDateTime.UtcNow.AddYears(-2);
					ExDateTime endTime = ExDateTime.UtcNow.AddYears(3);
					this.mailboxTimeZoneOffset = GetTimeZoneOffsetsCore.GetTheTimeZoneOffsets(startTime, endTime, this.TimeZone);
				}
				return this.mailboxTimeZoneOffset;
			}
			set
			{
				this.mailboxTimeZoneOffset = value;
			}
		}

		[DataMember]
		public bool ShowInferenceUiElements
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowInferenceUiElements];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowInferenceUiElements] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterBarIntroductionMouse
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterBarIntroductionMouse];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterBarIntroductionMouse] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterDeleteAllIntroductionMouse
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionMouse];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionMouse] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterBarIntroductionTNarrow
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterBarIntroductionTNarrow];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterBarIntroductionTNarrow] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterDeleteAllIntroductionTNarrow
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionTNarrow];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionTNarrow] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterBarIntroductionTWide
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterBarIntroductionTWide];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterBarIntroductionTWide] = value;
			}
		}

		[DataMember]
		public bool HasShownClutterDeleteAllIntroductionTWide
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionTWide];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownClutterDeleteAllIntroductionTWide] = value;
			}
		}

		[DataMember]
		public bool HasShownIntroductionForPeopleCentricTriage
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownIntroductionForPeopleCentricTriage];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownIntroductionForPeopleCentricTriage] = value;
			}
		}

		[DataMember]
		public bool HasShownIntroductionForModernGroups
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownIntroductionForModernGroups];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownIntroductionForModernGroups] = value;
			}
		}

		[DataMember]
		public UserOptionsLearnabilityTypes LearnabilityTypesShown
		{
			get
			{
				return (UserOptionsLearnabilityTypes)base[UserConfigurationPropertyId.LearnabilityTypesShown];
			}
			set
			{
				base[UserConfigurationPropertyId.LearnabilityTypesShown] = value;
			}
		}

		[DataMember]
		public bool HasShownPeopleIKnow
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.HasShownPeopleIKnow];
			}
			set
			{
				base[UserConfigurationPropertyId.HasShownPeopleIKnow] = value;
			}
		}

		[DataMember]
		public bool ShowSenderOnTopInListView
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowSenderOnTopInListView];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowSenderOnTopInListView] = value;
			}
		}

		[DataMember]
		public bool ShowPreviewTextInListView
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ShowPreviewTextInListView];
			}
			set
			{
				base[UserConfigurationPropertyId.ShowPreviewTextInListView] = value;
			}
		}

		[DataMember]
		public int GlobalReadingPanePosition
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.GlobalReadingPanePosition];
			}
			set
			{
				base[UserConfigurationPropertyId.GlobalReadingPanePosition] = value;
			}
		}

		[DataMember]
		public bool ReportJunkSelected
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.ReportJunkSelected];
			}
			set
			{
				base[UserConfigurationPropertyId.ReportJunkSelected] = value;
			}
		}

		[DataMember]
		public bool CheckForReportJunkDialog
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.CheckForReportJunkDialog];
			}
			set
			{
				base[UserConfigurationPropertyId.CheckForReportJunkDialog] = value;
			}
		}

		public UserOptionsMigrationState UserOptionsMigrationState
		{
			get
			{
				return (UserOptionsMigrationState)base[UserConfigurationPropertyId.UserOptionsMigrationState];
			}
			set
			{
				base[UserConfigurationPropertyId.UserOptionsMigrationState] = value;
			}
		}

		[DataMember]
		public bool IsInferenceSurveyComplete
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.IsInferenceSurveyComplete];
			}
			set
			{
				base[UserConfigurationPropertyId.IsInferenceSurveyComplete] = value;
			}
		}

		[DataMember]
		public int ActiveSurvey
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.ActiveSurvey];
			}
			set
			{
				base[UserConfigurationPropertyId.ActiveSurvey] = value;
			}
		}

		[DataMember]
		public int CompletedSurveys
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CompletedSurveys];
			}
			set
			{
				base[UserConfigurationPropertyId.CompletedSurveys] = value;
			}
		}

		[DataMember]
		public int DismissedSurveys
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.DismissedSurveys];
			}
			set
			{
				base[UserConfigurationPropertyId.DismissedSurveys] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string LastSurveyDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.LastSurveyDate];
			}
			set
			{
				base[UserConfigurationPropertyId.LastSurveyDate] = value;
			}
		}

		[DataMember]
		public bool DontShowSurveys
		{
			get
			{
				return (bool)base[UserConfigurationPropertyId.DontShowSurveys];
			}
			set
			{
				base[UserConfigurationPropertyId.DontShowSurveys] = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string PeopleIKnowFirstUseDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.PeopleIKnowFirstUseDate];
			}
			set
			{
				base[UserConfigurationPropertyId.PeopleIKnowFirstUseDate] = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string PeopleIKnowLastUseDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.PeopleIKnowLastUseDate];
			}
			set
			{
				base[UserConfigurationPropertyId.PeopleIKnowFirstUseDate] = value;
			}
		}

		[DataMember]
		public int PeopleIKnowUse
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.PeopleIKnowUse];
			}
			set
			{
				base[UserConfigurationPropertyId.PeopleIKnowUse] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string ModernGroupsFirstUseDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ModernGroupsFirstUseDate];
			}
			set
			{
				base[UserConfigurationPropertyId.ModernGroupsFirstUseDate] = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string ModernGroupsLastUseDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ModernGroupsLastUseDate];
			}
			set
			{
				base[UserConfigurationPropertyId.ModernGroupsLastUseDate] = value;
			}
		}

		[DataMember]
		public int ModernGroupsUseCount
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.ModernGroupsUseCount];
			}
			set
			{
				base[UserConfigurationPropertyId.ModernGroupsUseCount] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false)]
		public string BuildGreenLightSurveyLastShownDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.BuildGreenLightSurveyLastShownDate];
			}
			set
			{
				base[UserConfigurationPropertyId.BuildGreenLightSurveyLastShownDate] = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		[DateTimeString]
		public string InferenceSurveyDate
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.InferenceSurveyDate];
			}
			set
			{
				base[UserConfigurationPropertyId.InferenceSurveyDate] = value;
			}
		}

		[DataMember]
		public int CalendarSearchUseCount
		{
			get
			{
				return (int)base[UserConfigurationPropertyId.CalendarSearchUseCount];
			}
			set
			{
				base[UserConfigurationPropertyId.CalendarSearchUseCount] = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] FrequentlyUsedFolders
		{
			get
			{
				return (string[])base[UserConfigurationPropertyId.FrequentlyUsedFolders];
			}
			set
			{
				base[UserConfigurationPropertyId.FrequentlyUsedFolders] = value;
			}
		}

		[DataMember]
		public string ArchiveFolderId
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.ArchiveFolderId];
			}
			set
			{
				base[UserConfigurationPropertyId.ArchiveFolderId] = value;
			}
		}

		[DataMember]
		public string DefaultAttachmentsUploadFolderId
		{
			get
			{
				return (string)base[UserConfigurationPropertyId.DefaultAttachmentsUploadFolderId];
			}
			set
			{
				base[UserConfigurationPropertyId.DefaultAttachmentsUploadFolderId] = value;
			}
		}

		internal override UserConfigurationPropertySchemaBase Schema
		{
			get
			{
				return UserOptionPropertySchema.Instance;
			}
		}

		internal void Load(MailboxSession mailboxSession, bool loadCalendarOptions, bool performMigrationFixup)
		{
			StorePerformanceCountersCapture countersCapture = StorePerformanceCountersCapture.Start(mailboxSession);
			bool flag = false;
			IList<UserConfigurationPropertyDefinition> properties = new List<UserConfigurationPropertyDefinition>(base.OptionProperties.Keys);
			base.Load(mailboxSession, properties, true);
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.UserOptionsLoad, countersCapture, true);
			bool flag2 = string.IsNullOrEmpty(this.TimeZone);
			if (flag2)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UserOptions loaded without mailbox time zone set. User:{0}", mailboxSession.DisplayAddress);
				this.TimeZone = ExTimeZone.CurrentTimeZone.Id;
			}
			if (!loadCalendarOptions)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UserOptions loaded without calendar options. User:{0}", mailboxSession.DisplayAddress);
				return;
			}
			if (flag2)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UserOptions loaded with calendar options but without mailbox time zone set. User:{0}", mailboxSession.DisplayAddress);
				this.PopulateDefaultWorkingHours(ExTimeZone.CurrentTimeZone);
				this.PopulateDefaultReminderOptions();
				return;
			}
			this.LoadWorkingHours(mailboxSession);
			if (performMigrationFixup)
			{
				flag |= this.PerformWorkingHoursFixup(mailboxSession);
			}
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.WorkingHours, countersCapture, true);
			this.LoadReminderOptions(mailboxSession);
			OwaUserConfigurationLogUtilities.LogAndResetPerfCapture(OwaUserConfigurationLogType.LoadReminderOptions, countersCapture, false);
			if (flag)
			{
				this.CommitUserOptionMigrationState(mailboxSession);
			}
		}

		private void CommitUserOptionMigrationState(MailboxSession mailboxSession)
		{
			try
			{
				base.Commit(mailboxSession, new UserConfigurationPropertyDefinition[]
				{
					UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.UserOptionsMigrationState)
				});
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceError<StoragePermanentException, string>((long)this.GetHashCode(), "Permanent error while trying to update UserOptionsMigrationState. Error: {0}. User: {1}", arg, mailboxSession.DisplayAddress);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.UserOptionsDataTracer.TraceError<StorageTransientException, string>((long)this.GetHashCode(), "Transient error while trying to update UserOptionsMigrationState. Error: {0}. User: {1}.", arg2, mailboxSession.DisplayAddress);
			}
		}

		private void LoadWorkingHours(MailboxSession mailboxSession)
		{
			Exception ex = null;
			try
			{
				this.WorkingHours = WorkingHoursType.Load(mailboxSession, this.TimeZone);
			}
			catch (ObjectExistedException ex2)
			{
				ex = ex2;
			}
			catch (QuotaExceededException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceError<string, Exception>((long)this.GetHashCode(), "UserOptionsType: WorkingHoursType.Load failed. User:{0} Exception: {1}.", mailboxSession.DisplayAddress, ex);
				this.PopulateDefaultWorkingHours(this.TimeZone);
			}
		}

		private bool PerformWorkingHoursFixup(MailboxSession session)
		{
			if ((this.UserOptionsMigrationState & UserOptionsMigrationState.WorkingHoursTimeZoneFixUp) == UserOptionsMigrationState.None)
			{
				bool flag = true;
				if (this.WorkingHours != null && this.WorkingHours.IsTimeZoneDifferent && (this.WorkingHours.WorkingHoursTimeZone.Id == ExTimeZone.CurrentTimeZone.Id || (UserOptionsType.LegacyDataCenterTimeZone != null && this.WorkingHours.WorkingHoursTimeZone.Id == UserOptionsType.LegacyDataCenterTimeZone.Id)))
				{
					ExTimeZone newTimeZone = null;
					if (ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(this.TimeZone, out newTimeZone))
					{
						ExTraceGlobals.UserOptionsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UserOptionsType: Performing WorkingHours fixup - OM:227120 - User:{0}", session.DisplayAddress);
						try
						{
							WorkingHoursType.MoveWorkingHoursToTimeZone(session, newTimeZone);
							this.LoadWorkingHours(session);
						}
						catch (StorageTransientException arg)
						{
							ExTraceGlobals.UserOptionsCallTracer.TraceDebug<string, StorageTransientException>((long)this.GetHashCode(), "UserOptionsType: Caught transient error while performing WorkingHours fixup - OM:227120. User:{0} Exception: {1}", session.DisplayAddress, arg);
							flag = false;
						}
						catch (CorruptDataException arg2)
						{
							ExTraceGlobals.UserOptionsCallTracer.TraceError<string, CorruptDataException>((long)this.GetHashCode(), "UserOptionsType: Caught corrupt data error while performing WorkingHours fixup - OM:227120. User:{0} Exception: {1}", session.DisplayAddress, arg2);
						}
						catch (StoragePermanentException arg3)
						{
							ExTraceGlobals.UserOptionsCallTracer.TraceError<string, StoragePermanentException>((long)this.GetHashCode(), "UserOptionsType: Caught permanent error while performing WorkingHours fixup - OM:227120. User:{0} Exception: {1}", session.DisplayAddress, arg3);
						}
					}
				}
				if (flag)
				{
					this.UserOptionsMigrationState |= UserOptionsMigrationState.WorkingHoursTimeZoneFixUp;
				}
				return flag;
			}
			return false;
		}

		private void PopulateDefaultWorkingHours(ExTimeZone timeZone)
		{
			this.WorkingHours = WorkingHoursType.GetDefaultWorkingHoursInTimeZone(timeZone);
		}

		private void PopulateDefaultWorkingHours(string timeZoneId)
		{
			ExTimeZone timeZone;
			if (!string.IsNullOrEmpty(timeZoneId) && ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(timeZoneId, out timeZone))
			{
				this.PopulateDefaultWorkingHours(timeZone);
				return;
			}
			ExTraceGlobals.UserOptionsCallTracer.TraceError<string>((long)this.GetHashCode(), "Unable to map time zone id for Default working hours. Using UTC instead. TimeZoneId: {0}", string.IsNullOrEmpty(timeZoneId) ? "NullorEmpty" : timeZoneId);
			this.PopulateDefaultWorkingHours(ExTimeZone.UtcTimeZone);
		}

		private void PopulateDefaultReminderOptions()
		{
			this.DefaultReminderTimeInMinutes = 15;
		}

		private void LoadReminderOptions(MailboxSession mailboxSession)
		{
			try
			{
				CalendarConfiguration calendarConfiguration;
				using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(mailboxSession))
				{
					calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
				}
				if (calendarConfiguration == null)
				{
					ExTraceGlobals.UserOptionsCallTracer.TraceError<string>((long)this.GetHashCode(), "Unable to load Calendar configuration object for mailbox {0}", mailboxSession.DisplayAddress);
				}
				else
				{
					this.DefaultReminderTimeInMinutes = calendarConfiguration.DefaultReminderTime;
				}
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceError<string, StoragePermanentException>((long)this.GetHashCode(), "Unable to load Calendar configuration object for mailbox {0}. Exception: {1}", mailboxSession.DisplayAddress, arg);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.UserOptionsCallTracer.TraceError<string, StorageTransientException>((long)this.GetHashCode(), "Unable to load Calendar configuration object for mailbox {0}, Exception: {1}", mailboxSession.DisplayAddress, arg2);
			}
		}

		private const string LegacyDataCenterTimeZoneId = "Greenwich Standard Time";

		private const string ConfigurationName = "OWA.UserOptions";

		private static ExTimeZone legacyDataCenterTimeZone;

		private WorkingHoursType workingHours;

		private int defaultReminderTimeInMinutes = 15;

		private TimeZoneOffsetsType[] mailboxTimeZoneOffset;
	}
}
