using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal static class AddressBookViewDescriptors
	{
		private static Dictionary<AddressBookViewDescriptors.Key, ViewDescriptor> CreateDescriptorTable()
		{
			return new Dictionary<AddressBookViewDescriptors.Key, ViewDescriptor>
			{
				{
					AddressBookViewDescriptors.Key.ContactBrowse,
					AddressBookViewDescriptors.ContactBrowseSingleLine
				},
				{
					AddressBookViewDescriptors.Key.ContactBrowseSingleLineJapan,
					AddressBookViewDescriptors.ContactBrowseSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactBrowseMultiLine,
					AddressBookViewDescriptors.ContactBrowseMultiLine
				},
				{
					AddressBookViewDescriptors.Key.ContactBrowseMultiLineJapan,
					AddressBookViewDescriptors.ContactBrowseMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactPicker,
					AddressBookViewDescriptors.ContactPickerSingleLine
				},
				{
					AddressBookViewDescriptors.Key.ContactPickerSingleLineJapan,
					AddressBookViewDescriptors.ContactPickerSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactPickerMultiLine,
					AddressBookViewDescriptors.ContactPickerMultiLine
				},
				{
					AddressBookViewDescriptors.Key.ContactPickerMultiLineJapan,
					AddressBookViewDescriptors.ContactPickerMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactMobileNumberPickerSingleLine,
					AddressBookViewDescriptors.ContactMobileNumberPickerSingleLine
				},
				{
					AddressBookViewDescriptors.Key.ContactMobileNumberPickerSingleLineJapan,
					AddressBookViewDescriptors.ContactMobileNumberPickerSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactMobileNumberPickerMultiLine,
					AddressBookViewDescriptors.ContactMobileNumberPickerMultiLine
				},
				{
					AddressBookViewDescriptors.Key.ContactMobileNumberPickerMultiLineJapan,
					AddressBookViewDescriptors.ContactMobileNumberPickerMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.ContactModule,
					AddressBookViewDescriptors.ContactModuleSingleLine
				},
				{
					AddressBookViewDescriptors.Key.Japan,
					AddressBookViewDescriptors.ContactModuleSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.MultiLine,
					AddressBookViewDescriptors.ContactModuleMultiLine
				},
				{
					AddressBookViewDescriptors.Key.ContactModuleMultiLineJapan,
					AddressBookViewDescriptors.ContactModuleMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryBrowse,
					AddressBookViewDescriptors.DirectoryBrowseSingleLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryBrowseSingleLineJapan,
					AddressBookViewDescriptors.DirectoryBrowseSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryBrowseMultiLine,
					AddressBookViewDescriptors.DirectoryBrowseMultiLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryBrowseMultiLineJapan,
					AddressBookViewDescriptors.DirectoryBrowseMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryPicker,
					AddressBookViewDescriptors.DirectoryPickerSingleLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryPickerSingleLineJapan,
					AddressBookViewDescriptors.DirectoryPickerSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryPickerMultiLine,
					AddressBookViewDescriptors.DirectoryPickerMultiLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryPickerMultiLineJapan,
					AddressBookViewDescriptors.DirectoryPickerMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryMobileNumberPickerSingleLine,
					AddressBookViewDescriptors.DirectoryMobileNumberPickerSingleLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryMobileNumberPickerSingleLineJapan,
					AddressBookViewDescriptors.DirectoryMobileNumberPickerSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.DirectoryMobileNumberPickerMultiLine,
					AddressBookViewDescriptors.DirectoryMobileNumberPickerMultiLine
				},
				{
					AddressBookViewDescriptors.Key.DirectoryMobileNumberPickerMultiLineJapan,
					AddressBookViewDescriptors.DirectoryMobileNumberPickerMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.RoomBrowse,
					AddressBookViewDescriptors.RoomBrowseSingleLine
				},
				{
					AddressBookViewDescriptors.Key.RoomBrowseSingleLineJapan,
					AddressBookViewDescriptors.RoomBrowseSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.RoomBrowseMultiLine,
					AddressBookViewDescriptors.RoomBrowseMultiLine
				},
				{
					AddressBookViewDescriptors.Key.RoomBrowseMultiLineJapan,
					AddressBookViewDescriptors.RoomBrowseMultiLineJapan
				},
				{
					AddressBookViewDescriptors.Key.RoomPicker,
					AddressBookViewDescriptors.RoomPickerSingleLine
				},
				{
					AddressBookViewDescriptors.Key.RoomPickerSingleLineJapan,
					AddressBookViewDescriptors.RoomPickerSingleLineJapan
				},
				{
					AddressBookViewDescriptors.Key.RoomPickerMultiLine,
					AddressBookViewDescriptors.RoomPickerMultiLine
				},
				{
					AddressBookViewDescriptors.Key.RoomPickerMultiLineJapan,
					AddressBookViewDescriptors.RoomPickerMultiLineJapan
				}
			};
		}

		internal static ViewDescriptor GetViewDescriptor(bool isMultiLine, bool isPhoneticNamesEnabled, bool isMobilePicker, ViewType viewType)
		{
			int num = 0;
			while (num < AddressBookViewDescriptors.viewTypes.Length && viewType != AddressBookViewDescriptors.viewTypes[num])
			{
				num++;
			}
			if (num == AddressBookViewDescriptors.viewTypes.Length)
			{
				throw new ArgumentException("Invalid ViewType: {0}");
			}
			AddressBookViewDescriptors.Key key = (AddressBookViewDescriptors.Key)num;
			if (isPhoneticNamesEnabled)
			{
				key |= AddressBookViewDescriptors.Key.Japan;
			}
			if (isMultiLine)
			{
				key |= AddressBookViewDescriptors.Key.MultiLine;
			}
			if (isMobilePicker)
			{
				key |= AddressBookViewDescriptors.Key.MobileNumber;
			}
			ViewDescriptor result;
			if (!AddressBookViewDescriptors.descriptorTable.TryGetValue(key, out result))
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"There is no ViewDescriptor that matches isMultiLine=",
					isMultiLine,
					", IsPhoneticNamesEnabled=",
					isPhoneticNamesEnabled,
					", viewType=",
					viewType
				}));
			}
			return result;
		}

		private static readonly ViewDescriptor ContactModuleSingleLine = new ViewDescriptor(ColumnId.FileAs, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.BusinessPhone,
			ColumnId.BusinessFax,
			ColumnId.MobilePhone,
			ColumnId.HomePhone
		});

		private static readonly ViewDescriptor ContactModuleMultiLine = new ViewDescriptor(ColumnId.FileAs, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.BusinessPhone,
			ColumnId.MobilePhone,
			ColumnId.HomePhone
		});

		private static readonly ViewDescriptor ContactBrowseSingleLine = new ViewDescriptor(ColumnId.FileAs, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.Title,
			ColumnId.BusinessPhone,
			ColumnId.BusinessFax,
			ColumnId.MobilePhone,
			ColumnId.HomePhone,
			ColumnId.CompanyName
		});

		private static readonly ViewDescriptor ContactBrowseMultiLine = AddressBookViewDescriptors.ContactModuleMultiLine;

		private static readonly ViewDescriptor ContactPickerSingleLine = new ViewDescriptor(ColumnId.FileAs, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.EmailAddresses,
			ColumnId.Title,
			ColumnId.CompanyName
		});

		private static readonly ViewDescriptor ContactPickerMultiLine = new ViewDescriptor(ColumnId.FileAs, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.EmailAddresses
		});

		private static readonly ViewDescriptor ContactMobileNumberPickerSingleLine = new ViewDescriptor(ColumnId.FileAs, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.MobilePhone,
			ColumnId.Title,
			ColumnId.CompanyName
		});

		private static readonly ViewDescriptor ContactMobileNumberPickerMultiLine = new ViewDescriptor(ColumnId.FileAs, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.MobilePhone
		});

		private static readonly ViewDescriptor ContactModuleSingleLineJapan = new ViewDescriptor(ColumnId.YomiLastName, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.YomiCompanyName,
			ColumnId.BusinessPhone,
			ColumnId.BusinessFax,
			ColumnId.MobilePhone,
			ColumnId.HomePhone
		});

		private static readonly ViewDescriptor ContactModuleMultiLineJapan = new ViewDescriptor(ColumnId.YomiLastName, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Surname,
			ColumnId.GivenName,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.YomiFullName,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.YomiCompanyName,
			ColumnId.BusinessPhone,
			ColumnId.MobilePhone,
			ColumnId.HomePhone
		});

		private static readonly ViewDescriptor ContactBrowseSingleLineJapan = new ViewDescriptor(ColumnId.YomiLastName, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.Title,
			ColumnId.BusinessPhone,
			ColumnId.BusinessFax,
			ColumnId.MobilePhone,
			ColumnId.HomePhone,
			ColumnId.CompanyName,
			ColumnId.YomiCompanyName
		});

		private static readonly ViewDescriptor ContactBrowseMultiLineJapan = AddressBookViewDescriptors.ContactModuleMultiLineJapan;

		private static readonly ViewDescriptor ContactPickerSingleLineJapan = new ViewDescriptor(ColumnId.YomiLastName, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.EmailAddresses,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.YomiCompanyName
		});

		private static readonly ViewDescriptor ContactPickerMultiLineJapan = new ViewDescriptor(ColumnId.YomiLastName, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Surname,
			ColumnId.GivenName,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.YomiFullName,
			ColumnId.YomiCompanyName,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.EmailAddresses
		});

		private static readonly ViewDescriptor ContactMobileNumberPickerSingleLineJapan = new ViewDescriptor(ColumnId.YomiLastName, true, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.ContactCategories,
			ColumnId.ContactFlagDueDate,
			ColumnId.FileAs,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.MobilePhone,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.YomiCompanyName
		});

		private static readonly ViewDescriptor ContactMobileNumberPickerMultiLineJapan = new ViewDescriptor(ColumnId.YomiLastName, false, new ColumnId[]
		{
			ColumnId.ContactIcon,
			ColumnId.FileAs,
			ColumnId.Surname,
			ColumnId.GivenName,
			ColumnId.YomiLastName,
			ColumnId.YomiFirstName,
			ColumnId.YomiFullName,
			ColumnId.YomiCompanyName,
			ColumnId.Title,
			ColumnId.CompanyName,
			ColumnId.MobilePhone
		});

		private static readonly ViewDescriptor DirectoryBrowseSingleLine = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.AliasAD,
			ColumnId.EmailAddressAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.BusinessFaxAD,
			ColumnId.MobilePhoneAD,
			ColumnId.HomePhoneAD,
			ColumnId.OfficeAD,
			ColumnId.CompanyAD
		});

		private static readonly ViewDescriptor DirectoryBrowseMultiLine = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.MobilePhoneAD,
			ColumnId.HomePhoneAD
		});

		private static readonly ViewDescriptor DirectoryPickerSingleLine = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.AliasAD,
			ColumnId.EmailAddressAD,
			ColumnId.CompanyAD
		});

		private static readonly ViewDescriptor DirectoryPickerMultiLine = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.EmailAddressAD
		});

		private static readonly ViewDescriptor DirectoryMobileNumberPickerSingleLine = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.AliasAD,
			ColumnId.MobilePhoneAD,
			ColumnId.CompanyAD
		});

		private static readonly ViewDescriptor DirectoryMobileNumberPickerMultiLine = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.MobilePhoneAD
		});

		private static readonly ViewDescriptor DirectoryBrowseSingleLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.YomiDepartmentAD,
			ColumnId.AliasAD,
			ColumnId.EmailAddressAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.BusinessFaxAD,
			ColumnId.MobilePhoneAD,
			ColumnId.HomePhoneAD,
			ColumnId.OfficeAD,
			ColumnId.CompanyAD,
			ColumnId.YomiCompanyAD
		});

		private static readonly ViewDescriptor DirectoryBrowseMultiLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.MobilePhoneAD,
			ColumnId.HomePhoneAD
		});

		private static readonly ViewDescriptor DirectoryPickerSingleLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.YomiDepartmentAD,
			ColumnId.AliasAD,
			ColumnId.EmailAddressAD,
			ColumnId.CompanyAD,
			ColumnId.YomiCompanyAD
		});

		private static readonly ViewDescriptor DirectoryPickerMultiLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.EmailAddressAD
		});

		private static readonly ViewDescriptor DirectoryMobileNumberPickerSingleLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.ADIcon,
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.YomiDepartmentAD,
			ColumnId.AliasAD,
			ColumnId.MobilePhoneAD,
			ColumnId.CompanyAD,
			ColumnId.YomiCompanyAD
		});

		private static readonly ViewDescriptor DirectoryMobileNumberPickerMultiLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.MobilePhoneAD
		});

		private static readonly ViewDescriptor RoomBrowseSingleLine = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.ResourceCapacityAD,
			ColumnId.OfficeAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.EmailAddressAD
		});

		private static readonly ViewDescriptor RoomBrowseMultiLine = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.OfficeAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.YomiDepartmentAD
		});

		private static readonly ViewDescriptor RoomPickerSingleLine = AddressBookViewDescriptors.RoomBrowseSingleLine;

		private static readonly ViewDescriptor RoomPickerMultiLine = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.OfficeAD,
			ColumnId.EmailAddressAD,
			ColumnId.TitleAD,
			ColumnId.DepartmentAD,
			ColumnId.YomiDepartmentAD
		});

		private static readonly ViewDescriptor RoomBrowseSingleLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, true, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.ResourceCapacityAD,
			ColumnId.OfficeAD,
			ColumnId.BusinessPhoneAD,
			ColumnId.EmailAddressAD
		});

		private static readonly ViewDescriptor RoomBrowseMultiLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.OfficeAD,
			ColumnId.BusinessPhoneAD
		});

		private static readonly ViewDescriptor RoomPickerSingleLineJapan = AddressBookViewDescriptors.RoomBrowseSingleLineJapan;

		private static readonly ViewDescriptor RoomPickerMultiLineJapan = new ViewDescriptor(ColumnId.DisplayNameAD, false, new ColumnId[]
		{
			ColumnId.DisplayNameAD,
			ColumnId.YomiDisplayNameAD,
			ColumnId.OfficeAD,
			ColumnId.EmailAddressAD
		});

		private static readonly Dictionary<AddressBookViewDescriptors.Key, ViewDescriptor> descriptorTable = AddressBookViewDescriptors.CreateDescriptorTable();

		private static readonly ViewType[] viewTypes = new ViewType[]
		{
			ViewType.ContactModule,
			ViewType.ContactBrowser,
			ViewType.ContactPicker,
			ViewType.DirectoryBrowser,
			ViewType.DirectoryPicker,
			ViewType.RoomBrowser,
			ViewType.RoomPicker
		};

		[Flags]
		private enum Key
		{
			ContactModule = 0,
			ContactBrowse = 1,
			ContactPicker = 2,
			DirectoryBrowse = 3,
			DirectoryPicker = 4,
			RoomBrowse = 5,
			RoomPicker = 6,
			Japan = 16,
			MultiLine = 32,
			MobileNumber = 64,
			ContactModuleSingleLine = 0,
			ContactModuleSingleLineJapan = 16,
			ContactModuleMultiLine = 32,
			ContactModuleMultiLineJapan = 48,
			ContactBrowseSingleLine = 1,
			ContactBrowseSingleLineJapan = 17,
			ContactBrowseMultiLine = 33,
			ContactBrowseMultiLineJapan = 49,
			ContactPickerSingleLine = 2,
			ContactPickerSingleLineJapan = 18,
			ContactPickerMultiLine = 34,
			ContactPickerMultiLineJapan = 50,
			ContactMobileNumberPickerSingleLine = 66,
			ContactMobileNumberPickerSingleLineJapan = 82,
			ContactMobileNumberPickerMultiLine = 98,
			ContactMobileNumberPickerMultiLineJapan = 114,
			DirectoryBrowseSingleLine = 3,
			DirectoryBrowseSingleLineJapan = 19,
			DirectoryBrowseMultiLine = 35,
			DirectoryBrowseMultiLineJapan = 51,
			DirectoryPickerSingleLine = 4,
			DirectoryPickerSingleLineJapan = 20,
			DirectoryPickerMultiLine = 36,
			DirectoryPickerMultiLineJapan = 52,
			DirectoryMobileNumberPickerSingleLine = 68,
			DirectoryMobileNumberPickerSingleLineJapan = 84,
			DirectoryMobileNumberPickerMultiLine = 100,
			DirectoryMobileNumberPickerMultiLineJapan = 116,
			RoomBrowseSingleLine = 5,
			RoomBrowseSingleLineJapan = 21,
			RoomBrowseMultiLine = 37,
			RoomBrowseMultiLineJapan = 53,
			RoomPickerSingleLine = 6,
			RoomPickerSingleLineJapan = 22,
			RoomPickerMultiLine = 38,
			RoomPickerMultiLineJapan = 54
		}
	}
}
