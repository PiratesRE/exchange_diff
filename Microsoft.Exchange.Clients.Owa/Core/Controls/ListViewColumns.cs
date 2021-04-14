using System;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public static class ListViewColumns
	{
		public static Column GetColumn(ColumnId columnId)
		{
			return ListViewColumns.columns[(int)columnId];
		}

		public static bool IsSupportedColumnId(ColumnId columnId)
		{
			return columnId >= ColumnId.MailIcon && columnId < (ColumnId)ListViewColumns.columns.Length;
		}

		private static readonly ColumnBehavior AliasBehavior = new ColumnBehavior(15, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior CapacityBehavior = new ColumnBehavior(HorizontalAlign.Right, 9, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior CheckBoxBehavior = new ColumnBehavior(HorizontalAlign.Center, 20, true, SortOrder.Descending, GroupType.None);

		private static readonly ColumnBehavior CheckBoxContactBehavior = new ColumnBehavior(HorizontalAlign.Center, 2, false, SortOrder.Descending, GroupType.None);

		private static readonly ColumnBehavior CheckBoxADBehavior = new ColumnBehavior(HorizontalAlign.Center, 2, false, SortOrder.Descending, GroupType.None);

		private static readonly ColumnBehavior CompanyBehavior = new ColumnBehavior(16, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior ContactIconBehavior = new ColumnBehavior(HorizontalAlign.Center, 2, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior ConversationUnreadCountBehavior = new ColumnBehavior(16, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior DepartmentBehavior = new ColumnBehavior(16, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior EmailAddressBehavior = new ColumnBehavior(20, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior DistributionListMemberEmailAddressBehavior = new ColumnBehavior(20, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior FileAsBehavior = new ColumnBehavior(15, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior FlagBehavior = new ColumnBehavior(20, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior ContactFlagBehavior = new ColumnBehavior(2, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior TaskFlagBehavior = new ColumnBehavior(20, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior CategoryBehavior = new ColumnBehavior(HorizontalAlign.Right, 24, true);

		private static readonly ColumnBehavior ContactCategoryBehavior = new ColumnBehavior(HorizontalAlign.Right, 2, true);

		private static readonly ColumnBehavior FullNameBehavior = new ColumnBehavior(18, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior HalfName = new ColumnBehavior(20, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior HasAttachmentsBehavior = new ColumnBehavior(HorizontalAlign.Center, 16, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior ImportanceBehavior = new ColumnBehavior(HorizontalAlign.Center, 15, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior MailIconBehavior = new ColumnBehavior(HorizontalAlign.Center, 20, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior MarkCompleteCheckboxBehavior = new ColumnBehavior(HorizontalAlign.Center, 20, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior OfficeBehavior = new ColumnBehavior(12, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior PhoneNumberBehavior = new ColumnBehavior(11, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SizeBehavior = new ColumnBehavior(HorizontalAlign.Right, 6, false, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior SubjectBehavior = new ColumnBehavior(44, false, SortOrder.Ascending, GroupType.Collapsed);

		private static readonly ColumnBehavior TaskIconBehavior = new ColumnBehavior(HorizontalAlign.Center, 20, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior TimeBehavior = new ColumnBehavior(90, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior DumpsterTimeBehavior = new ColumnBehavior(160, true, SortOrder.Descending, GroupType.Expanded);

		private static readonly ColumnBehavior TitleBehavior = new ColumnBehavior(15, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior SharepointDocumentIconBehavior = new ColumnBehavior(24, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentDisplayNameBehavior = new ColumnBehavior(100, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentLastModifiedBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior SharepointDocumentModifiedByBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentCheckedOutToBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentFileSizeBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior UncDocumentIconBehavior = new ColumnBehavior(24, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior UncDocumentDisplayNameBehavior = new ColumnBehavior(250, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior UncDocumentLastModifiedBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior UncDocumentFileSizeBehavior = new ColumnBehavior(39, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior SharepointDocumentLibraryIconBehavior = new ColumnBehavior(24, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentLibraryDisplayNameBehavior = new ColumnBehavior(200, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentLibraryDescriptionBehavior = new ColumnBehavior(250, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentLibraryItemCountBehavior = new ColumnBehavior(40, false, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior SharepointDocumentLibraryLastModifiedBehavior = new ColumnBehavior(150, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior YomiCompanyBehavior = new ColumnBehavior(16, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior YomiDepartmentBehavior = new ColumnBehavior(16, true, SortOrder.Ascending, GroupType.None);

		private static readonly ColumnBehavior YomiFullNameBehavior = new ColumnBehavior(23, false, SortOrder.Ascending, GroupType.Expanded);

		private static readonly ColumnBehavior YomiHalfName = new ColumnBehavior(20, true, SortOrder.Ascending, GroupType.Expanded);

		private static readonly Column ADIcon = new Column(ColumnId.ADIcon, ListViewColumns.ContactIconBehavior, false, new ColumnHeader(ThemeFileId.AddressBookIcon), new PropertyDefinition[]
		{
			ADObjectSchema.ObjectCategory
		});

		private static readonly Column AliasAD = new Column(ColumnId.AliasAD, ListViewColumns.AliasBehavior, false, new ColumnHeader(-638921847), new PropertyDefinition[]
		{
			ADRecipientSchema.Alias
		});

		private static readonly Column BusinessFaxAD = new Column(ColumnId.BusinessFaxAD, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-714530665), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Fax
		});

		private static readonly Column BusinessPhoneAD = new Column(ColumnId.BusinessPhoneAD, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-296653598), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Phone
		});

		private static readonly Column CheckBoxAD = new Column(ColumnId.CheckBoxAD, ListViewColumns.CheckBoxADBehavior, false, new ColumnHeader(true), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ADObjectSchema.Guid
		});

		private static readonly Column CompanyAD = new Column(ColumnId.CompanyAD, ListViewColumns.CompanyBehavior, false, new ColumnHeader(1750481828), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Company
		});

		private static readonly Column DepartmentAD = new Column(ColumnId.DepartmentAD, ListViewColumns.DepartmentBehavior, false, new ColumnHeader(-977196825), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Department
		});

		private static readonly Column DisplayNameAD = new Column(ColumnId.DisplayNameAD, ListViewColumns.FullNameBehavior, false, new ColumnHeader(-228177434), new SortBoundaries(1445002207, -155175775, 878694989), new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName
		});

		private static readonly Column EmailAddressAD = new Column(ColumnId.EmailAddressAD, ListViewColumns.EmailAddressBehavior, false, new ColumnHeader(1162538767), new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress,
			ADRecipientSchema.RecipientDisplayType
		});

		private static readonly Column HomePhoneAD = new Column(ColumnId.HomePhoneAD, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(1664796201), new PropertyDefinition[]
		{
			ADOrgPersonSchema.HomePhone
		});

		private static readonly Column MobilePhoneAD = new Column(ColumnId.MobilePhoneAD, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-1298303582), new PropertyDefinition[]
		{
			ADOrgPersonSchema.MobilePhone
		});

		private static readonly Column OfficeAD = new Column(ColumnId.OfficeAD, ListViewColumns.OfficeBehavior, false, new ColumnHeader(1053060679), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Office
		});

		private static readonly Column PhoneAD = new Column(ColumnId.PhoneAD, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-2111177296), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Phone
		});

		private static readonly Column ResourceCapacityAD = new Column(ColumnId.ResourceCapacityAD, ListViewColumns.CapacityBehavior, false, new ColumnHeader(1799661901), new PropertyDefinition[]
		{
			ADRecipientSchema.ResourceCapacity
		});

		private static readonly Column TitleAD = new Column(ColumnId.TitleAD, ListViewColumns.TitleBehavior, false, new ColumnHeader(-1029833905), new PropertyDefinition[]
		{
			ADOrgPersonSchema.Title
		});

		private static readonly Column YomiDisplayNameAD = new Column(ColumnId.YomiDisplayNameAD, ListViewColumns.YomiFullNameBehavior, false, new ColumnHeader(-1991902276), new PropertyDefinition[]
		{
			ADRecipientSchema.PhoneticDisplayName
		});

		private static readonly Column YomiDepartmentAD = new Column(ColumnId.YomiDepartmentAD, ListViewColumns.YomiDepartmentBehavior, false, new ColumnHeader(1590675473), new PropertyDefinition[]
		{
			ADRecipientSchema.PhoneticDepartment
		});

		private static readonly Column YomiCompanyAD = new Column(ColumnId.YomiCompanyAD, ListViewColumns.YomiCompanyBehavior, false, new ColumnHeader(1292568250), new PropertyDefinition[]
		{
			ADRecipientSchema.PhoneticCompany
		});

		private static readonly Column BusinessFax = new Column(ColumnId.BusinessFax, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-714530665), new SortBoundaries(1192139348, 1839024124, 696226150), new PropertyDefinition[]
		{
			ContactSchema.WorkFax
		});

		private static readonly Column BusinessPhone = new Column(ColumnId.BusinessPhone, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-296653598), new SortBoundaries(-123259807, 1839024124, 696226150), new PropertyDefinition[]
		{
			ContactSchema.BusinessPhoneNumber
		});

		private static readonly Column Categories = new Column(ColumnId.Categories, ListViewColumns.CategoryBehavior, false, new ColumnHeader(ThemeFileId.CategoriesHeader), new PropertyDefinition[]
		{
			ItemSchema.Categories,
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.IsToDoItem
		});

		private static readonly Column ContactCategories = new Column(ColumnId.ContactCategories, ListViewColumns.ContactCategoryBehavior, false, new ColumnHeader(ThemeFileId.CategoriesHeader), new PropertyDefinition[]
		{
			ItemSchema.Categories,
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.IsToDoItem
		});

		private static readonly Column CheckBox = new Column(ColumnId.CheckBox, ListViewColumns.CheckBoxBehavior, false, new ColumnHeader(true), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ItemSchema.Id
		});

		private static readonly Column CheckBoxContact = new Column(ColumnId.CheckBoxContact, ListViewColumns.CheckBoxContactBehavior, false, new ColumnHeader(true), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ItemSchema.Id
		});

		private static readonly Column CompanyName = new Column(ColumnId.CompanyName, ListViewColumns.CompanyBehavior, true, new ColumnHeader(1750481828), new SortBoundaries(-826838917, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.CompanyName
		});

		private static readonly Column DistributionListMemberDisplayName = new Column(ColumnId.MemberDisplayName, ListViewColumns.FullNameBehavior, false, new ColumnHeader(-228177434), new SortBoundaries(1445002207, -155175775, 878694989), new PropertyDefinition[]
		{
			StoreObjectSchema.DisplayName
		});

		private static readonly Column DistributionListMemberIcon = new Column(ColumnId.MemberIcon, ListViewColumns.MailIconBehavior, true, new ColumnHeader(ThemeFileId.Contact), new SortBoundaries(785343019, -927268579, -1832517975), new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		});

		private static readonly Column ContactIcon = new Column(ColumnId.ContactIcon, ListViewColumns.ContactIconBehavior, true, new ColumnHeader(ThemeFileId.Contact), new SortBoundaries(785343019, -927268579, -1832517975), new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		});

		private static readonly Column Department = new Column(ColumnId.Department, ListViewColumns.DepartmentBehavior, true, new ColumnHeader(-611050349), new SortBoundaries(-611050349, -155175775, 878694989), false, new PropertyDefinition[]
		{
			ContactSchema.Department
		});

		private static readonly Column DueDate = new Column(ColumnId.DueDate, ListViewColumns.TimeBehavior, true, new ColumnHeader(1629617734), new SortBoundaries(-66960209, -629880559, 464390861), new PropertyDefinition[]
		{
			TaskSchema.DueDate
		});

		private static readonly Column EmailAddresses = new Column(ColumnId.EmailAddresses, ListViewColumns.EmailAddressBehavior, false, new ColumnHeader(961315020), new SortBoundaries(173930168, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactSchema.ContactBusinessFax,
			ContactSchema.ContactHomeFax,
			ContactSchema.ContactOtherFax,
			ContactSchema.Email1,
			ContactSchema.Email2,
			ContactSchema.Email3
		});

		private static readonly Column DistributionListMemberEmail = new Column(ColumnId.MemberEmail, ListViewColumns.DistributionListMemberEmailAddressBehavior, false, new ColumnHeader(-748689469), new SortBoundaries(173930168, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Email1,
			ParticipantSchema.EmailAddress,
			ParticipantSchema.RoutingType,
			ItemSchema.RecipientType
		});

		private static readonly Column Email1 = new Column(ColumnId.Email1, ListViewColumns.EmailAddressBehavior, false, new ColumnHeader(-748689469), new SortBoundaries(173930168, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Email1
		});

		private static readonly Column Email2 = new Column(ColumnId.Email2, ListViewColumns.EmailAddressBehavior, false, new ColumnHeader(-345404942), new SortBoundaries(173930169, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Email2
		});

		private static readonly Column Email3 = new Column(ColumnId.Email3, ListViewColumns.EmailAddressBehavior, false, new ColumnHeader(-1911488883), new SortBoundaries(173930170, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Email3
		});

		private static readonly Column FileAs = new Column(ColumnId.FileAs, ListViewColumns.FileAsBehavior, false, new ColumnHeader(1178724274), new SortBoundaries(13085779, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactBaseSchema.FileAs
		});

		private static readonly Column GivenName = new Column(ColumnId.GivenName, ListViewColumns.HalfName, true, new ColumnHeader(2145983474), new SortBoundaries(-1876431821, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.GivenName
		});

		private static readonly Column From = new Column(ColumnId.From, ListViewColumns.FullNameBehavior, true, new ColumnHeader(-1656488396), new SortBoundaries(1309845117, -155175775, 878694989), new PropertyDefinition[]
		{
			ItemSchema.SentRepresentingDisplayName
		});

		private static readonly Column HasAttachments = new Column(ColumnId.HasAttachment, ListViewColumns.HasAttachmentsBehavior, true, new ColumnHeader(ThemeFileId.Attachment2), new SortBoundaries(1072079569, 1348123951, 1845030095), new PropertyDefinition[]
		{
			ItemSchema.HasAttachment
		});

		private static readonly Column HomePhone = new Column(ColumnId.HomePhone, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(1664796201), new SortBoundaries(326004004, 1839024124, 696226150), new PropertyDefinition[]
		{
			ContactSchema.HomePhone
		});

		private static readonly Column Importance = new Column(ColumnId.Importance, ListViewColumns.ImportanceBehavior, true, new ColumnHeader(ThemeFileId.ImportanceHigh), new SortBoundaries(1569168155, 544952141, 975394505), new PropertyDefinition[]
		{
			ItemSchema.Importance
		});

		private static readonly Column Surname = new Column(ColumnId.Surname, ListViewColumns.HalfName, true, new ColumnHeader(1200027237), new SortBoundaries(1759499200, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Surname
		});

		private static readonly Column MailIcon = new Column(ColumnId.MailIcon, ListViewColumns.MailIconBehavior, true, new ColumnHeader(ThemeFileId.EMail), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		});

		private static readonly Column MarkCompleteCheckbox = new Column(ColumnId.MarkCompleteCheckbox, ListViewColumns.MarkCompleteCheckboxBehavior, false, new ColumnHeader(ThemeFileId.CheckboxHeader), new SortBoundaries(-153493007, 499380967, -1587174585), new PropertyDefinition[]
		{
			ItemSchema.IsComplete
		});

		private static readonly Column MobilePhone = new Column(ColumnId.MobilePhone, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-1298303582), new SortBoundaries(-2076111245, 1839024124, 696226150), new PropertyDefinition[]
		{
			ContactSchema.MobilePhone
		});

		private static readonly Column PhoneNumbers = new Column(ColumnId.PhoneNumbers, ListViewColumns.PhoneNumberBehavior, false, new ColumnHeader(-2111177296), new SortBoundaries(-123259807, 1839024124, 696226150), new PropertyDefinition[]
		{
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.PrimaryTelephoneNumber,
			ContactSchema.BusinessPhoneNumber2,
			ContactSchema.MobilePhone,
			ContactSchema.HomePhone,
			ContactSchema.HomePhone2,
			ContactSchema.HomeFax,
			ContactSchema.WorkFax,
			ContactSchema.OtherFax,
			ContactSchema.AssistantPhoneNumber,
			ContactSchema.CallbackPhone,
			ContactSchema.CarPhone,
			ContactSchema.Pager,
			ContactSchema.OtherTelephone,
			ContactSchema.RadioPhone,
			ContactSchema.TtyTddPhoneNumber
		});

		private static readonly Column ReceivedTime = new Column(ColumnId.DeliveryTime, ListViewColumns.TimeBehavior, true, new ColumnHeader(-375576855), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ItemSchema.ReceivedTime
		});

		private static readonly Column SharepointDocumentIcon = new Column(ColumnId.SharepointDocumentIcon, ListViewColumns.SharepointDocumentIconBehavior, false, new ColumnHeader(ThemeFileId.FlatDocument), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			DocumentSchema.FileType
		});

		private static readonly Column SharepointDocumentDisplayName = new Column(ColumnId.SharepointDocumentDisplayName, ListViewColumns.SharepointDocumentDisplayNameBehavior, false, new ColumnHeader(-1966747349), new SortBoundaries(-839171991, -155175775, 878694989), new PropertyDefinition[]
		{
			SharepointDocumentLibraryItemSchema.Name
		});

		private static readonly Column SharepointDocumentLastModified = new Column(ColumnId.SharepointDocumentLastModified, ListViewColumns.SharepointDocumentLastModifiedBehavior, true, new ColumnHeader(869905365), new SortBoundaries(869905365, -629880559, 464390861), new PropertyDefinition[]
		{
			SharepointDocumentLibraryItemSchema.LastModifiedTime
		});

		private static readonly Column SharepointDocumentModifiedBy = new Column(ColumnId.SharepointDocumentModifiedBy, ListViewColumns.SharepointDocumentModifiedByBehavior, false, new ColumnHeader(1276881056), new SortBoundaries(1276881056, -155175775, 878694989), new PropertyDefinition[]
		{
			SharepointDocumentLibraryItemSchema.Editor
		});

		private static readonly Column SharepointDocumentCheckedOutTo = new Column(ColumnId.SharepointDocumentCheckedOutTo, ListViewColumns.SharepointDocumentCheckedOutToBehavior, false, new ColumnHeader(-580782680), new SortBoundaries(-580782680, -155175775, 878694989), new PropertyDefinition[]
		{
			SharepointDocumentSchema.CheckedOutUserId
		});

		private static readonly Column SharepointDocumentFileSize = new Column(ColumnId.SharepointDocumentFileSize, ListViewColumns.SharepointDocumentFileSizeBehavior, true, new ColumnHeader(-837446919), new SortBoundaries(1128018090, 499418978, -1417517224), new PropertyDefinition[]
		{
			SharepointDocumentSchema.FileSize
		});

		private static readonly Column UncDocumentIcon = new Column(ColumnId.UncDocumentIcon, ListViewColumns.UncDocumentIconBehavior, false, new ColumnHeader(ThemeFileId.FlatDocument), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			DocumentSchema.FileType
		});

		private static readonly Column UncDocumentLibraryIcon = new Column(ColumnId.UncDocumentLibraryIcon, ListViewColumns.UncDocumentIconBehavior, false, new ColumnHeader(ThemeFileId.FlatDocument), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			DocumentSchema.FileType
		});

		private static readonly Column UncDocumentDisplayName = new Column(ColumnId.UncDocumentDisplayName, ListViewColumns.UncDocumentDisplayNameBehavior, false, new ColumnHeader(-1966747349), new SortBoundaries(-839171991, -155175775, 878694989), new PropertyDefinition[]
		{
			UncItemSchema.DisplayName
		});

		private static readonly Column UncDocumentLastModified = new Column(ColumnId.UncDocumentLastModified, ListViewColumns.UncDocumentLastModifiedBehavior, true, new ColumnHeader(869905365), new SortBoundaries(869905365, -629880559, 464390861), new PropertyDefinition[]
		{
			UncItemSchema.LastModifiedDate
		});

		private static readonly Column UncDocumentFileSize = new Column(ColumnId.UncDocumentFileSize, ListViewColumns.UncDocumentFileSizeBehavior, true, new ColumnHeader(-837446919), new SortBoundaries(1128018090, 499418978, -1417517224), new PropertyDefinition[]
		{
			UncDocumentSchema.FileSize
		});

		private static readonly Column SharepointDocumentLibraryIcon = new Column(ColumnId.SharepointDocumentLibraryIcon, ListViewColumns.SharepointDocumentLibraryIconBehavior, false, new ColumnHeader(ThemeFileId.FlatDocument), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			DocumentSchema.FileType
		});

		private static readonly Column SharepointDocumentLibraryDisplayName = new Column(ColumnId.SharepointDocumentLibraryDisplayName, ListViewColumns.SharepointDocumentLibraryDisplayNameBehavior, false, new ColumnHeader(-1966747349), new SortBoundaries(-839171991, -155175775, 878694989), new PropertyDefinition[]
		{
			SharepointListSchema.Title
		});

		private static readonly Column SharepointDocumentLibraryDescription = new Column(ColumnId.SharepointDocumentLibraryDescription, ListViewColumns.SharepointDocumentLibraryDescriptionBehavior, false, new ColumnHeader(873740972), new SortBoundaries(873740972, -155175775, 878694989), new PropertyDefinition[]
		{
			SharepointListSchema.Description
		});

		private static readonly Column SharepointDocumentLibraryItemCount = new Column(ColumnId.SharepointDocumentLibraryItemCount, ListViewColumns.SharepointDocumentLibraryItemCountBehavior, false, new ColumnHeader(780414746), new SortBoundaries(780414746, -1178532886, -1942189936), new PropertyDefinition[]
		{
			SharepointListSchema.ItemCount
		});

		private static readonly Column SharepointDocumentLibraryLastModified = new Column(ColumnId.SharepointDocumentLibraryLastModified, ListViewColumns.SharepointDocumentLibraryLastModifiedBehavior, true, new ColumnHeader(869905365), new SortBoundaries(869905365, -629880559, 464390861), new PropertyDefinition[]
		{
			SharepointListSchema.LastModifiedTime
		});

		private static readonly Column Size = new Column(ColumnId.Size, ListViewColumns.SizeBehavior, true, new ColumnHeader(-943768673), new SortBoundaries(1128018090, 499418978, -1417517224), new PropertyDefinition[]
		{
			ItemSchema.Size
		});

		private static readonly Column SentTime = new Column(ColumnId.SentTime, ListViewColumns.TimeBehavior, true, new ColumnHeader(-2005811526), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ItemSchema.SentTime
		});

		private static readonly Column Subject = new Column(ColumnId.Subject, ListViewColumns.SubjectBehavior, true, new ColumnHeader(601895112), new SortBoundaries(2014646167, -155175775, 878694989), new PropertyDefinition[]
		{
			ItemSchema.Subject
		});

		private static readonly Column TaskIcon = new Column(ColumnId.TaskIcon, ListViewColumns.TaskIconBehavior, true, new ColumnHeader(ThemeFileId.Task), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass
		});

		private static readonly Column Title = new Column(ColumnId.Title, ListViewColumns.TitleBehavior, true, new ColumnHeader(355579046), new SortBoundaries(1436331631, -155175775, 878694989), new PropertyDefinition[]
		{
			ContactSchema.Title
		});

		private static readonly Column To = new Column(ColumnId.To, ListViewColumns.FullNameBehavior, true, new ColumnHeader(2145670281), new SortBoundaries(262509582, -155175775, 878694989), new PropertyDefinition[]
		{
			ItemSchema.DisplayTo
		});

		private static readonly Column YomiCompanyName = new Column(ColumnId.YomiCompanyName, ListViewColumns.YomiCompanyBehavior, true, new ColumnHeader(1292568250), new SortBoundaries(-1574585415, -1732685949, -577489661), new PropertyDefinition[]
		{
			ContactSchema.YomiCompany
		});

		private static readonly Column YomiFirstName = new Column(ColumnId.YomiFirstName, ListViewColumns.YomiHalfName, true, new ColumnHeader(1215703485), new SortBoundaries(1506085866, -1732685949, -577489661), new PropertyDefinition[]
		{
			ContactSchema.YomiFirstName
		});

		private static readonly Column YomiFullName = new Column(ColumnId.YomiFullName, ListViewColumns.YomiFullNameBehavior, true, new ColumnHeader(-1644089428), new SortBoundaries(1703559301, -1732685949, -577489661), new PropertyDefinition[]
		{
			ContactSchema.YomiLastName,
			ContactSchema.YomiFirstName
		});

		private static readonly Column YomiLastName = new Column(ColumnId.YomiLastName, ListViewColumns.YomiHalfName, true, new ColumnHeader(-1420908403), new SortBoundaries(-1514589286, -1732685949, -577489661), new PropertyDefinition[]
		{
			ContactSchema.YomiLastName
		});

		private static readonly Column FlagDueDate = new Column(ColumnId.FlagDueDate, ListViewColumns.FlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1587370059, 571886510, -568934371), new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.IsComplete
		});

		private static readonly Column FlagStartDate = new Column(ColumnId.FlagStartDate, ListViewColumns.FlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1580556595, 571886510, -568934371), new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.IsComplete
		});

		private static readonly Column FlagDueDateContact = new Column(ColumnId.ContactFlagDueDate, ListViewColumns.ContactFlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1587370059, 571886510, -568934371), new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.IsComplete
		});

		private static readonly Column FlagStartDateContact = new Column(ColumnId.ContactFlagStartDate, ListViewColumns.ContactFlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1580556595, 571886510, -568934371), new PropertyDefinition[]
		{
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate,
			ItemSchema.IsComplete
		});

		private static readonly Column TaskFlag = new Column(ColumnId.TaskFlag, ListViewColumns.TaskFlagBehavior, false, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1587370059, 571886510, -568934371), new PropertyDefinition[]
		{
			ItemSchema.IsComplete,
			ItemSchema.FlagStatus,
			ItemSchema.ItemColor,
			ItemSchema.UtcDueDate,
			ItemSchema.UtcStartDate
		});

		private static readonly Column DeletedOnTime = new Column(ColumnId.DeletedOnTime, ListViewColumns.DumpsterTimeBehavior, false, new ColumnHeader(-56656194), new SortBoundaries(1932142663, -629880559, 464390861), new PropertyDefinition[]
		{
			StoreObjectSchema.LastModifiedTime
		});

		private static readonly Column DumpsterReceivedTime = new Column(ColumnId.DumpsterReceivedTime, ListViewColumns.DumpsterTimeBehavior, false, new ColumnHeader(-375576855), new PropertyDefinition[]
		{
			ItemSchema.ReceivedTime
		});

		private static readonly Column ObjectDisplayName = new Column(ColumnId.ObjectDisplayName, ListViewColumns.SubjectBehavior, false, new ColumnHeader(601895112), new PropertyDefinition[]
		{
			ItemSchema.Subject,
			FolderSchema.DisplayName
		});

		private static readonly Column ObjectIcon = new Column(ColumnId.ObjectIcon, ListViewColumns.MailIconBehavior, true, new ColumnHeader(ThemeFileId.EMail), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			FolderSchema.Id
		});

		private static readonly Column ConversationLastDeliveryTime = new Column(ColumnId.ConversationLastDeliveryTime, ListViewColumns.TimeBehavior, true, new ColumnHeader(-1795472081), new SortBoundaries(-1795472081, -629880559, 464390861), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationLastDeliveryTime
		});

		private static readonly Column ConversationIcon = new Column(ColumnId.ConversationIcon, ListViewColumns.MailIconBehavior, true, new ColumnHeader(ThemeFileId.EMail), new SortBoundaries(785343019, -1759181059, 1822314281), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationMessageClasses
		});

		private static readonly Column ConversationSubject = new Column(ColumnId.ConversationSubject, ListViewColumns.SubjectBehavior, true, new ColumnHeader(601895112), new SortBoundaries(2014646167, -155175775, 878694989), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationTopic
		});

		private static readonly Column ConversationUnreadCount = new Column(ColumnId.ConversationUnreadCount, new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationUnreadMessageCount,
			ConversationItemSchema.ConversationMessageCount
		});

		private static readonly Column ConversationHasAttachment = new Column(ColumnId.ConversationHasAttachment, ListViewColumns.HasAttachmentsBehavior, true, new ColumnHeader(ThemeFileId.Attachment2), new SortBoundaries(1072079569, 1348123951, 1845030095), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationHasAttach
		});

		private static readonly Column ConversationSenderList = new Column(ColumnId.ConversationSenderList, ListViewColumns.FullNameBehavior, true, new ColumnHeader(-1656488396), new SortBoundaries(1309845117, -155175775, 878694989), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationMVFrom
		});

		private static readonly Column ConversationImportance = new Column(ColumnId.ConversationImportance, ListViewColumns.ImportanceBehavior, true, new ColumnHeader(ThemeFileId.ImportanceHigh), new SortBoundaries(1569168155, 544952141, 975394505), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationImportance
		});

		private static readonly Column ConversationCategories = new Column(ColumnId.ConversationCategories, new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationCategories
		});

		private static readonly Column ConversationSize = new Column(ColumnId.ConversationSize, ListViewColumns.SizeBehavior, true, new ColumnHeader(-943768673), new SortBoundaries(1128018090, 499418978, -1417517224), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationMessageSize
		});

		private static readonly Column ConversationFlagStatus = new Column(ColumnId.ConversationFlagStatus, ListViewColumns.FlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(-568934371, 571886510, -568934371), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationFlagStatus,
			ConversationItemSchema.ConversationFlagCompleteTime
		});

		private static readonly Column ConversationFlagDueDate = new Column(ColumnId.ConversationFlagStatus, ListViewColumns.FlagBehavior, true, new ColumnHeader(ThemeFileId.FlagEmpty), new SortBoundaries(1587370059, 571886510, -568934371), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationFlagStatus,
			ConversationItemSchema.ConversationFlagCompleteTime
		});

		private static readonly Column ConversationToList = new Column(ColumnId.ConversationToList, ListViewColumns.FullNameBehavior, true, new ColumnHeader(2145670281), new SortBoundaries(262509582, -155175775, 878694989), new PropertyDefinition[]
		{
			ConversationItemSchema.ConversationMVTo
		});

		private static readonly Column IMAddress = new Column(ColumnId.IMAddress, new PropertyDefinition[]
		{
			ContactSchema.IMAddress
		});

		private static readonly Column[] columns = new Column[]
		{
			ListViewColumns.MailIcon,
			ListViewColumns.From,
			ListViewColumns.To,
			ListViewColumns.Subject,
			ListViewColumns.Department,
			ListViewColumns.HasAttachments,
			ListViewColumns.Importance,
			ListViewColumns.ReceivedTime,
			ListViewColumns.SentTime,
			ListViewColumns.Size,
			ListViewColumns.ContactIcon,
			ListViewColumns.FileAs,
			ListViewColumns.Title,
			ListViewColumns.CompanyName,
			ListViewColumns.PhoneNumbers,
			ListViewColumns.BusinessPhone,
			ListViewColumns.BusinessFax,
			ListViewColumns.MobilePhone,
			ListViewColumns.HomePhone,
			ListViewColumns.EmailAddresses,
			ListViewColumns.Email1,
			ListViewColumns.Email2,
			ListViewColumns.Email3,
			ListViewColumns.GivenName,
			ListViewColumns.Surname,
			ListViewColumns.Categories,
			ListViewColumns.ContactCategories,
			ListViewColumns.SharepointDocumentIcon,
			ListViewColumns.SharepointDocumentDisplayName,
			ListViewColumns.SharepointDocumentLastModified,
			ListViewColumns.SharepointDocumentModifiedBy,
			ListViewColumns.SharepointDocumentCheckedOutTo,
			ListViewColumns.SharepointDocumentFileSize,
			ListViewColumns.UncDocumentIcon,
			ListViewColumns.UncDocumentLibraryIcon,
			ListViewColumns.UncDocumentDisplayName,
			ListViewColumns.UncDocumentLastModified,
			ListViewColumns.UncDocumentFileSize,
			ListViewColumns.SharepointDocumentLibraryIcon,
			ListViewColumns.SharepointDocumentLibraryDisplayName,
			ListViewColumns.SharepointDocumentLibraryDescription,
			ListViewColumns.SharepointDocumentLibraryItemCount,
			ListViewColumns.SharepointDocumentLibraryLastModified,
			ListViewColumns.CheckBox,
			ListViewColumns.CheckBoxContact,
			ListViewColumns.ADIcon,
			ListViewColumns.AliasAD,
			ListViewColumns.BusinessFaxAD,
			ListViewColumns.BusinessPhoneAD,
			ListViewColumns.CheckBoxAD,
			ListViewColumns.CompanyAD,
			ListViewColumns.DepartmentAD,
			ListViewColumns.DisplayNameAD,
			ListViewColumns.EmailAddressAD,
			ListViewColumns.HomePhoneAD,
			ListViewColumns.MobilePhoneAD,
			ListViewColumns.OfficeAD,
			ListViewColumns.PhoneAD,
			ListViewColumns.TitleAD,
			ListViewColumns.YomiCompanyName,
			ListViewColumns.YomiCompanyAD,
			ListViewColumns.YomiFirstName,
			ListViewColumns.YomiFullName,
			ListViewColumns.YomiLastName,
			ListViewColumns.YomiDisplayNameAD,
			ListViewColumns.YomiDepartmentAD,
			ListViewColumns.ResourceCapacityAD,
			ListViewColumns.FlagDueDate,
			ListViewColumns.FlagStartDate,
			ListViewColumns.FlagDueDateContact,
			ListViewColumns.FlagStartDateContact,
			ListViewColumns.TaskFlag,
			ListViewColumns.TaskIcon,
			ListViewColumns.MarkCompleteCheckbox,
			ListViewColumns.DueDate,
			ListViewColumns.DistributionListMemberDisplayName,
			ListViewColumns.DistributionListMemberEmail,
			ListViewColumns.DistributionListMemberIcon,
			ListViewColumns.DeletedOnTime,
			ListViewColumns.DumpsterReceivedTime,
			ListViewColumns.ObjectDisplayName,
			ListViewColumns.ObjectIcon,
			ListViewColumns.ConversationLastDeliveryTime,
			ListViewColumns.ConversationIcon,
			ListViewColumns.ConversationSubject,
			ListViewColumns.ConversationUnreadCount,
			ListViewColumns.ConversationHasAttachment,
			ListViewColumns.ConversationSenderList,
			ListViewColumns.ConversationImportance,
			ListViewColumns.ConversationCategories,
			ListViewColumns.ConversationSize,
			ListViewColumns.ConversationFlagStatus,
			ListViewColumns.ConversationFlagDueDate,
			ListViewColumns.IMAddress,
			ListViewColumns.ConversationToList
		};
	}
}
