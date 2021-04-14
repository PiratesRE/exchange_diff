using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.ImportContacts
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookCsvLanguageSelect
	{
		private OutlookCsvLanguage CreateLanguageObject_ar_sa()
		{
			return new OutlookCsvLanguage(1256, new Dictionary<string, ImportContactProperties>
			{
				{
					"عنوان البريد الإلكتروني 2",
					ImportContactProperties.Email2Address
				},
				{
					"اللقب المهني",
					ImportContactProperties.JobTitle
				},
				{
					"بلد آخر/المنطقة",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"التلكس",
					ImportContactProperties.Telex
				},
				{
					"إنترنت متوفر/مشغول",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"شارع 2 آخر",
					ImportContactProperties.OtherStreet2
				},
				{
					"المهنة",
					ImportContactProperties.Profession
				},
				{
					"نوع البريد الإلكتروني",
					ImportContactProperties.EmailType
				},
				{
					"الحساب",
					ImportContactProperties.Account
				},
				{
					"الفئات",
					ImportContactProperties.Categories
				},
				{
					"تاريخ الميلاد",
					ImportContactProperties.Birthday
				},
				{
					"صندوق بريد عنوان العمل",
					ImportContactProperties.BusinessPOBox
				},
				{
					"شارع 3 آخر",
					ImportContactProperties.OtherStreet3
				},
				{
					"رقم معرّف الحكومة",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"اسم العائلة",
					ImportContactProperties.LastName
				},
				{
					"الهاتف الراديوي",
					ImportContactProperties.RadioPhone
				},
				{
					"الذكرى السنوية",
					ImportContactProperties.Anniversary
				},
				{
					"هاتف TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"نوع البريد الإلكتروني 2",
					ImportContactProperties.Email2Type
				},
				{
					"اسم العرض للبريد الإلكتروني 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"الرمز البريدي للعمل",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"هاتف آخر",
					ImportContactProperties.OtherPhone
				},
				{
					"الأولوية",
					ImportContactProperties.Priority
				},
				{
					"الأحرف الأولى",
					ImportContactProperties.Initials
				},
				{
					"الزوجة",
					ImportContactProperties.Spouse
				},
				{
					"القسم",
					ImportContactProperties.Department
				},
				{
					"هاتف المنزل",
					ImportContactProperties.HomePhone
				},
				{
					"هاتف العمل",
					ImportContactProperties.BusinessPhone
				},
				{
					"الشركة \"يومي\"",
					ImportContactProperties.CompanyYomi
				},
				{
					"النداء",
					ImportContactProperties.Pager
				},
				{
					"رمز بريدي آخر",
					ImportContactProperties.OtherPostalCode
				},
				{
					"ولاية العمل",
					ImportContactProperties.BusinessState
				},
				{
					"المستخدم 2",
					ImportContactProperties.User2
				},
				{
					"هاتف المنزل 2",
					ImportContactProperties.HomePhone2
				},
				{
					"الهاتف الجوال",
					ImportContactProperties.MobilePhone
				},
				{
					"لاحقة",
					ImportContactProperties.Suffix
				},
				{
					"الهواية",
					ImportContactProperties.Hobby
				},
				{
					"المستخدم 3",
					ImportContactProperties.User3
				},
				{
					"الهاتف الرئيسي للشركة",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"الأولاد",
					ImportContactProperties.Children
				},
				{
					"فاكس العمل",
					ImportContactProperties.BusinessFax
				},
				{
					"الرمز البريدي للمنزل",
					ImportContactProperties.HomePostalCode
				},
				{
					"الموقع",
					ImportContactProperties.Location
				},
				{
					"معلومات الفوترة",
					ImportContactProperties.BillingInformation
				},
				{
					"فاكس المنزل",
					ImportContactProperties.HomeFax
				},
				{
					"صفحة ويب",
					ImportContactProperties.WebPage
				},
				{
					"شارع المنزل 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"مدينة العمل",
					ImportContactProperties.BusinessCity
				},
				{
					"مدينة أخرى",
					ImportContactProperties.OtherCity
				},
				{
					"شارع العمل",
					ImportContactProperties.BusinessStreet
				},
				{
					"نوع البريد الإلكتروني 3",
					ImportContactProperties.Email3Type
				},
				{
					"اسم المساعد",
					ImportContactProperties.AssistantName
				},
				{
					"الجنس",
					ImportContactProperties.Gender
				},
				{
					"هاتف المساعد",
					ImportContactProperties.AssistantPhone
				},
				{
					"اسم العرض للبريد الإلكتروني",
					ImportContactProperties.EmailDisplayName
				},
				{
					"عنوان البريد الإلكتروني 3",
					ImportContactProperties.Email3Address
				},
				{
					"اللقب \"يومي\"",
					ImportContactProperties.SurnameYomi
				},
				{
					"المستخدم 4",
					ImportContactProperties.User4
				},
				{
					"اسم المدير",
					ImportContactProperties.ManagerName
				},
				{
					"بلد العمل/المنطقة",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"رد الاتصال",
					ImportContactProperties.Callback
				},
				{
					"الهاتف الأساسي",
					ImportContactProperties.PrimaryPhone
				},
				{
					"عنوان البريد الإلكتروني",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"هاتف العمل 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"ولاية المنزل",
					ImportContactProperties.HomeState
				},
				{
					"فاكس آخر",
					ImportContactProperties.OtherFax
				},
				{
					"شارع 3 للعمل",
					ImportContactProperties.BusinessStreet3
				},
				{
					"مدينة المنزل",
					ImportContactProperties.HomeCity
				},
				{
					"ولاية أخرى",
					ImportContactProperties.OtherState
				},
				{
					"شارع 2 للعمل",
					ImportContactProperties.BusinessStreet2
				},
				{
					"اللقب",
					ImportContactProperties.Title
				},
				{
					"شارع المنزل 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"موقع المكتب",
					ImportContactProperties.OfficeLocation
				},
				{
					"الاسم المعطى \"يومي\"",
					ImportContactProperties.GivenYomi
				},
				{
					"البلد/المنطقة",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"هاتف السيارة",
					ImportContactProperties.CarPhone
				},
				{
					"الشركة",
					ImportContactProperties.Company
				},
				{
					"صندوق بريد عنوان المنزل",
					ImportContactProperties.HomePOBox
				},
				{
					"صندوق بريد عنوان آخر",
					ImportContactProperties.OtherPOBox
				},
				{
					"شارع المنزل",
					ImportContactProperties.HomeStreet
				},
				{
					"اسم العرض للبريد الإلكتروني 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"الحساسية",
					ImportContactProperties.Sensitivity
				},
				{
					"ملاحظات",
					ImportContactProperties.Notes
				},
				{
					"الاسم الأول",
					ImportContactProperties.FirstName
				},
				{
					"شارع آخر",
					ImportContactProperties.OtherStreet
				},
				{
					"الاسم الأوسط",
					ImportContactProperties.MiddleName
				},
				{
					"عدد الأميال",
					ImportContactProperties.Mileage
				},
				{
					"المستخدم 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1025));
		}

		private OutlookCsvLanguage CreateLanguageObject_bg_bg()
		{
			return new OutlookCsvLanguage(1251, new Dictionary<string, ImportContactProperties>
			{
				{
					"Имейл адрес 2",
					ImportContactProperties.Email2Address
				},
				{
					"Длъжност",
					ImportContactProperties.JobTitle
				},
				{
					"Страна/регион (друг адрес)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Телекс",
					ImportContactProperties.Telex
				},
				{
					"Сведения за достъп в Интернет",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Улица 2 \u00a0(друг адрес)",
					ImportContactProperties.OtherStreet2
				},
				{
					"Професия",
					ImportContactProperties.Profession
				},
				{
					"Тип имейл",
					ImportContactProperties.EmailType
				},
				{
					"Сметка",
					ImportContactProperties.Account
				},
				{
					"Категории",
					ImportContactProperties.Categories
				},
				{
					"Рожден ден",
					ImportContactProperties.Birthday
				},
				{
					"Пощенска кутия (сл. адрес)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Улица 3 \u00a0(друг адрес)",
					ImportContactProperties.OtherStreet3
				},
				{
					"Правителствен код",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Фамилно име",
					ImportContactProperties.LastName
				},
				{
					"Радиотелефон",
					ImportContactProperties.RadioPhone
				},
				{
					"Годишнина",
					ImportContactProperties.Anniversary
				},
				{
					"Тип имейл 2",
					ImportContactProperties.Email2Type
				},
				{
					"Показвано име в имейл 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Пощенски код (сл. адрес)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Друг телефон",
					ImportContactProperties.OtherPhone
				},
				{
					"Приоритет",
					ImportContactProperties.Priority
				},
				{
					"Инициали",
					ImportContactProperties.Initials
				},
				{
					"Съпруг(а)",
					ImportContactProperties.Spouse
				},
				{
					"Отдел",
					ImportContactProperties.Department
				},
				{
					"Домашен телефон",
					ImportContactProperties.HomePhone
				},
				{
					"Служебен телефон",
					ImportContactProperties.BusinessPhone
				},
				{
					"Фирма Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Пейджър",
					ImportContactProperties.Pager
				},
				{
					"Пощенски код \u00a0(друг адрес)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Област (сл. адрес)",
					ImportContactProperties.BusinessState
				},
				{
					"Потребител 2",
					ImportContactProperties.User2
				},
				{
					"Домашен телефон 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Мобилен телефон",
					ImportContactProperties.MobilePhone
				},
				{
					"Суфикс",
					ImportContactProperties.Suffix
				},
				{
					"Хоби",
					ImportContactProperties.Hobby
				},
				{
					"Потребител 3",
					ImportContactProperties.User3
				},
				{
					"Основен телефон на фирмата",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Деца",
					ImportContactProperties.Children
				},
				{
					"Служебен факс",
					ImportContactProperties.BusinessFax
				},
				{
					"Пощенски код",
					ImportContactProperties.HomePostalCode
				},
				{
					"Местоположение",
					ImportContactProperties.Location
				},
				{
					"Платежна информация",
					ImportContactProperties.BillingInformation
				},
				{
					"Домашен факс",
					ImportContactProperties.HomeFax
				},
				{
					"Уеб страница",
					ImportContactProperties.WebPage
				},
				{
					"Улица 2 \u00a0(дом. адрес)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Град (сл. адрес)",
					ImportContactProperties.BusinessCity
				},
				{
					"Град \u00a0(друг адрес)",
					ImportContactProperties.OtherCity
				},
				{
					"Улица (сл. адрес)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Тип имейл 3",
					ImportContactProperties.Email3Type
				},
				{
					"Име на помощника",
					ImportContactProperties.AssistantName
				},
				{
					"Пол",
					ImportContactProperties.Gender
				},
				{
					"Телефон на помощника",
					ImportContactProperties.AssistantPhone
				},
				{
					"Показвано име в имейл",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Имейл адрес 3",
					ImportContactProperties.Email3Address
				},
				{
					"Фамилно Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Потребител 4",
					ImportContactProperties.User4
				},
				{
					"Име на ръководителя",
					ImportContactProperties.ManagerName
				},
				{
					"Страна/регион (служ. адрес)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Обратно повикване",
					ImportContactProperties.Callback
				},
				{
					"Основен телефон",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Имейл адрес",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Служебен телефон 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Област",
					ImportContactProperties.HomeState
				},
				{
					"Друг факс",
					ImportContactProperties.OtherFax
				},
				{
					"Улица 3 (сл. адрес)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Град",
					ImportContactProperties.HomeCity
				},
				{
					"Област (друг адрес)",
					ImportContactProperties.OtherState
				},
				{
					"Улица 2 (сл. адрес)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Обръщение",
					ImportContactProperties.Title
				},
				{
					"Улица 3 \u00a0(дом. адрес)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Местоположение на офиса",
					ImportContactProperties.OfficeLocation
				},
				{
					"Собствено Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Страна/регион (дом. адрес)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Автомобилен телефон",
					ImportContactProperties.CarPhone
				},
				{
					"Фирма",
					ImportContactProperties.Company
				},
				{
					"Пощенска кутия (дом. адрес)",
					ImportContactProperties.HomePOBox
				},
				{
					"Пощенска кутия (друг адрес)",
					ImportContactProperties.OtherPOBox
				},
				{
					"Улица",
					ImportContactProperties.HomeStreet
				},
				{
					"Показвано име в имейл 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Конфиденциалност",
					ImportContactProperties.Sensitivity
				},
				{
					"Бележки",
					ImportContactProperties.Notes
				},
				{
					"Собствено име",
					ImportContactProperties.FirstName
				},
				{
					"Улица (друг адрес)",
					ImportContactProperties.OtherStreet
				},
				{
					"Бащино име",
					ImportContactProperties.MiddleName
				},
				{
					"Разстояние",
					ImportContactProperties.Mileage
				},
				{
					"Потребител 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1026));
		}

		private OutlookCsvLanguage CreateLanguageObject_cs_cz()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-mailová adresa 2",
					ImportContactProperties.Email2Address
				},
				{
					"Funkce",
					ImportContactProperties.JobTitle
				},
				{
					"Jiná země",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Dálnopis",
					ImportContactProperties.Telex
				},
				{
					"Informace o volném čase v síti Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Jiná ulice 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profese",
					ImportContactProperties.Profession
				},
				{
					"Typ e-mailu",
					ImportContactProperties.EmailType
				},
				{
					"Účet",
					ImportContactProperties.Account
				},
				{
					"Kategorie",
					ImportContactProperties.Categories
				},
				{
					"Narozeniny",
					ImportContactProperties.Birthday
				},
				{
					"Poštovní přihrádka zaměstnání",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Jiná ulice 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Rodné číslo",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Příjmení",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Výročí",
					ImportContactProperties.Anniversary
				},
				{
					"Pro neslyšící",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Typ e-mailu 2",
					ImportContactProperties.Email2Type
				},
				{
					"Zobrazované jméno e-mailu 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"PSČ (zam.)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Jiný telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorita",
					ImportContactProperties.Priority
				},
				{
					"Iniciály",
					ImportContactProperties.Initials
				},
				{
					"Manžel/manželka",
					ImportContactProperties.Spouse
				},
				{
					"Oddělení",
					ImportContactProperties.Department
				},
				{
					"Telefon domů",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon (zam.)",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Operátor",
					ImportContactProperties.Pager
				},
				{
					"Jiné PSČ",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Země (zam.)",
					ImportContactProperties.BusinessState
				},
				{
					"Uživatel 2",
					ImportContactProperties.User2
				},
				{
					"Telefon 2 domů",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobilní telefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Za příjmením",
					ImportContactProperties.Suffix
				},
				{
					"Záliby",
					ImportContactProperties.Hobby
				},
				{
					"Uživatel 3",
					ImportContactProperties.User3
				},
				{
					"Telefon společnosti",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Děti",
					ImportContactProperties.Children
				},
				{
					"Fax (zam.)",
					ImportContactProperties.BusinessFax
				},
				{
					"PSČ (dom.)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Umístění",
					ImportContactProperties.Location
				},
				{
					"Fakturační údaje",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax domů",
					ImportContactProperties.HomeFax
				},
				{
					"Webová stránka",
					ImportContactProperties.WebPage
				},
				{
					"Ulice 2 (dom.)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Město (zam.)",
					ImportContactProperties.BusinessCity
				},
				{
					"Jiné město",
					ImportContactProperties.OtherCity
				},
				{
					"Ulice (zam.)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Typ e-mailu 3",
					ImportContactProperties.Email3Type
				},
				{
					"Jméno asistenta",
					ImportContactProperties.AssistantName
				},
				{
					"Pohlaví",
					ImportContactProperties.Gender
				},
				{
					"Telefon asistenta",
					ImportContactProperties.AssistantPhone
				},
				{
					"Zobrazované jméno e-mailu",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mailová adresa 3",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Uživatel 4",
					ImportContactProperties.User4
				},
				{
					"Jméno správce",
					ImportContactProperties.ManagerName
				},
				{
					"Země (zaměstnání)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Zpětné volání",
					ImportContactProperties.Callback
				},
				{
					"Primární telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mailová adresa",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon 2 (zam.)",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Země (dom.)",
					ImportContactProperties.HomeState
				},
				{
					"Jiný fax",
					ImportContactProperties.OtherFax
				},
				{
					"Ulice 3 (zam.)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Město (dom.)",
					ImportContactProperties.HomeCity
				},
				{
					"Jiný okres",
					ImportContactProperties.OtherState
				},
				{
					"Ulice 2 (zam.)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titul",
					ImportContactProperties.Title
				},
				{
					"Ulice 3 (dom.)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Umístění pracoviště",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Země (domů)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefon do auta",
					ImportContactProperties.CarPhone
				},
				{
					"Společnost",
					ImportContactProperties.Company
				},
				{
					"Poštovní přihrádka domů",
					ImportContactProperties.HomePOBox
				},
				{
					"Jiná poštovní přihrádka",
					ImportContactProperties.OtherPOBox
				},
				{
					"Ulice (dom.)",
					ImportContactProperties.HomeStreet
				},
				{
					"Zobrazované jméno e-mailu 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Utajení",
					ImportContactProperties.Sensitivity
				},
				{
					"Poznámky",
					ImportContactProperties.Notes
				},
				{
					"Jméno",
					ImportContactProperties.FirstName
				},
				{
					"Jiná ulice",
					ImportContactProperties.OtherStreet
				},
				{
					"2. křestní jméno",
					ImportContactProperties.MiddleName
				},
				{
					"Vzdálenost",
					ImportContactProperties.Mileage
				},
				{
					"Uživatel 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1029));
		}

		private OutlookCsvLanguage CreateLanguageObject_da_dk()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-mail 2-adresse",
					ImportContactProperties.Email2Address
				},
				{
					"Stilling",
					ImportContactProperties.JobTitle
				},
				{
					"Andet land/område",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Ledig/optaget via internettet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Anden gade 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profession",
					ImportContactProperties.Profession
				},
				{
					"E-mail-type",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategorier",
					ImportContactProperties.Categories
				},
				{
					"Fødselsdag",
					ImportContactProperties.Birthday
				},
				{
					"Adresse, postboks (arbejde)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Anden gade 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Se-nr.",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Efternavn",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Mærkedag",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD-telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-mail 2-type",
					ImportContactProperties.Email2Type
				},
				{
					"E-mail 2-vist navn",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Postnummer (arbejde)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Anden telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritet",
					ImportContactProperties.Priority
				},
				{
					"Initialer",
					ImportContactProperties.Initials
				},
				{
					"Ægtefælle",
					ImportContactProperties.Spouse
				},
				{
					"Afdeling",
					ImportContactProperties.Department
				},
				{
					"Telefon (privat)",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon (arbejde)",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Personsøger",
					ImportContactProperties.Pager
				},
				{
					"Andet postnummer",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Område (arbejde)",
					ImportContactProperties.BusinessState
				},
				{
					"Bruger 2",
					ImportContactProperties.User2
				},
				{
					"Telefon 2 (privat)",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffiks",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Bruger 3",
					ImportContactProperties.User3
				},
				{
					"Primær telefon (firma)",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Børn",
					ImportContactProperties.Children
				},
				{
					"Fax (arbejde)",
					ImportContactProperties.BusinessFax
				},
				{
					"Postnummer (privat)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Sted",
					ImportContactProperties.Location
				},
				{
					"Takstoplysninger",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax (privat)",
					ImportContactProperties.HomeFax
				},
				{
					"webside",
					ImportContactProperties.WebPage
				},
				{
					"Gade 2 (privat)",
					ImportContactProperties.HomeStreet2
				},
				{
					"By (arbejde)",
					ImportContactProperties.BusinessCity
				},
				{
					"Anden by",
					ImportContactProperties.OtherCity
				},
				{
					"Gade (arbejde)",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-mail 3-type",
					ImportContactProperties.Email3Type
				},
				{
					"Assistentens navn",
					ImportContactProperties.AssistantName
				},
				{
					"Køn",
					ImportContactProperties.Gender
				},
				{
					"Assistentens telefon",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-mail-vist navn",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mail 3-adresse",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Bruger 4",
					ImportContactProperties.User4
				},
				{
					"Navn på overordnet",
					ImportContactProperties.ManagerName
				},
				{
					"Land/område (arbejde)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Tilbagekald",
					ImportContactProperties.Callback
				},
				{
					"Primær telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mail-adresse",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon 2 (arbejde)",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Område (privat)",
					ImportContactProperties.HomeState
				},
				{
					"Anden fax",
					ImportContactProperties.OtherFax
				},
				{
					"Gade 3 (firma)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"By (privat)",
					ImportContactProperties.HomeCity
				},
				{
					"Andet område",
					ImportContactProperties.OtherState
				},
				{
					"Gade 2 (firma)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titel",
					ImportContactProperties.Title
				},
				{
					"Gade 3 (privat)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Kontoradresse",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Land/område (privat)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Biltelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Firma",
					ImportContactProperties.Company
				},
				{
					"Adresse, postboks (privat)",
					ImportContactProperties.HomePOBox
				},
				{
					"Anden adresse, postboks",
					ImportContactProperties.OtherPOBox
				},
				{
					"Gade (privat)",
					ImportContactProperties.HomeStreet
				},
				{
					"E-mail 3-vist navn",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Følsomhed",
					ImportContactProperties.Sensitivity
				},
				{
					"Notater",
					ImportContactProperties.Notes
				},
				{
					"Fornavn",
					ImportContactProperties.FirstName
				},
				{
					"Anden gade",
					ImportContactProperties.OtherStreet
				},
				{
					"Mellemnavn",
					ImportContactProperties.MiddleName
				},
				{
					"Kørte kilometer",
					ImportContactProperties.Mileage
				},
				{
					"Bruger 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1030));
		}

		private OutlookCsvLanguage CreateLanguageObject_de_de()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-Mail 2: Adresse",
					ImportContactProperties.Email2Address
				},
				{
					"Position",
					ImportContactProperties.JobTitle
				},
				{
					"Weiteres/e Land/Region",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Internet-Frei/Gebucht",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Weitere Straße 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Beruf",
					ImportContactProperties.Profession
				},
				{
					"E-Mail-Typ",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategorien",
					ImportContactProperties.Categories
				},
				{
					"Geburtstag",
					ImportContactProperties.Birthday
				},
				{
					"Postfach geschäftlich",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Weitere Straße 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Regierungsnr.",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Nachname",
					ImportContactProperties.LastName
				},
				{
					"Mobiltelefon 2",
					ImportContactProperties.RadioPhone
				},
				{
					"Jahrestag",
					ImportContactProperties.Anniversary
				},
				{
					"Telefon für Hörbehinderte",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-Mail 2: Typ",
					ImportContactProperties.Email2Type
				},
				{
					"E-Mail 2: Angezeigter Name",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Postleitzahl geschäftlich",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Weiteres Telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorität",
					ImportContactProperties.Priority
				},
				{
					"Initialen",
					ImportContactProperties.Initials
				},
				{
					"Partner",
					ImportContactProperties.Spouse
				},
				{
					"Abteilung",
					ImportContactProperties.Department
				},
				{
					"Telefon privat",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon geschäftlich",
					ImportContactProperties.BusinessPhone
				},
				{
					"Firma Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Weitere Postleitzahl",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Region geschäftlich",
					ImportContactProperties.BusinessState
				},
				{
					"Benutzer 2",
					ImportContactProperties.User2
				},
				{
					"Telefon privat 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffix",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Benutzer 3",
					ImportContactProperties.User3
				},
				{
					"Telefon Firma",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Kinder",
					ImportContactProperties.Children
				},
				{
					"Fax geschäftlich",
					ImportContactProperties.BusinessFax
				},
				{
					"Postleitzahl privat",
					ImportContactProperties.HomePostalCode
				},
				{
					"Ort",
					ImportContactProperties.Location
				},
				{
					"Abrechnungsinformation",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax privat",
					ImportContactProperties.HomeFax
				},
				{
					"Webseite",
					ImportContactProperties.WebPage
				},
				{
					"Straße privat 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Ort geschäftlich",
					ImportContactProperties.BusinessCity
				},
				{
					"Weiterer Ort",
					ImportContactProperties.OtherCity
				},
				{
					"Straße geschäftlich",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-Mail 3: Typ",
					ImportContactProperties.Email3Type
				},
				{
					"Name Assistent",
					ImportContactProperties.AssistantName
				},
				{
					"Geschlecht",
					ImportContactProperties.Gender
				},
				{
					"Telefon Assistent",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-Mail: Angezeigter Name",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-Mail 3: Adresse",
					ImportContactProperties.Email3Address
				},
				{
					"Nachname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Benutzer 4",
					ImportContactProperties.User4
				},
				{
					"Name des/der Vorgesetzten",
					ImportContactProperties.ManagerName
				},
				{
					"Land/Region geschäftlich",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Rückmeldung",
					ImportContactProperties.Callback
				},
				{
					"Haupttelefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-Mail-Adresse",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon geschäftlich 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Bundesland/Kanton privat",
					ImportContactProperties.HomeState
				},
				{
					"Weiteres Fax",
					ImportContactProperties.OtherFax
				},
				{
					"Straße geschäftlich 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Ort privat",
					ImportContactProperties.HomeCity
				},
				{
					"Weiteres/r Bundesland/Kanton",
					ImportContactProperties.OtherState
				},
				{
					"Straße geschäftlich 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Anrede",
					ImportContactProperties.Title
				},
				{
					"Straße privat 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Büro",
					ImportContactProperties.OfficeLocation
				},
				{
					"Vorname Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Land/Region privat",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autotelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Firma",
					ImportContactProperties.Company
				},
				{
					"Postfach privat",
					ImportContactProperties.HomePOBox
				},
				{
					"Weiteres Postfach",
					ImportContactProperties.OtherPOBox
				},
				{
					"Straße privat",
					ImportContactProperties.HomeStreet
				},
				{
					"E-Mail 3: Angezeigter Name",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Vertraulichkeit",
					ImportContactProperties.Sensitivity
				},
				{
					"Notizen",
					ImportContactProperties.Notes
				},
				{
					"Vorname",
					ImportContactProperties.FirstName
				},
				{
					"Weitere Straße",
					ImportContactProperties.OtherStreet
				},
				{
					"Weitere Vornamen",
					ImportContactProperties.MiddleName
				},
				{
					"Reisekilometer",
					ImportContactProperties.Mileage
				},
				{
					"Benutzer 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1031));
		}

		private OutlookCsvLanguage CreateLanguageObject_el_gr()
		{
			return new OutlookCsvLanguage(1253, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-mail 2 Address",
					ImportContactProperties.Email2Address
				},
				{
					"Job Title",
					ImportContactProperties.JobTitle
				},
				{
					"Άλλη χώρα/περιοχή",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Internet Free Busy",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Other Street 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profession",
					ImportContactProperties.Profession
				},
				{
					"E-mail Type",
					ImportContactProperties.EmailType
				},
				{
					"Account",
					ImportContactProperties.Account
				},
				{
					"Categories",
					ImportContactProperties.Categories
				},
				{
					"Birthday",
					ImportContactProperties.Birthday
				},
				{
					"Τ.Θ. διεύθυνσης εργασίας",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Other Street 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Government ID Number",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Last Name",
					ImportContactProperties.LastName
				},
				{
					"Radio Phone",
					ImportContactProperties.RadioPhone
				},
				{
					"Anniversary",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD Phone",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-mail 2 Type",
					ImportContactProperties.Email2Type
				},
				{
					"E-mail 2 Display Name",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Business Postal Code",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Other Phone",
					ImportContactProperties.OtherPhone
				},
				{
					"Priority",
					ImportContactProperties.Priority
				},
				{
					"Initials",
					ImportContactProperties.Initials
				},
				{
					"Spouse",
					ImportContactProperties.Spouse
				},
				{
					"Department",
					ImportContactProperties.Department
				},
				{
					"Home Phone",
					ImportContactProperties.HomePhone
				},
				{
					"Business Phone",
					ImportContactProperties.BusinessPhone
				},
				{
					"Εταιρικό Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Other Postal Code",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Business State",
					ImportContactProperties.BusinessState
				},
				{
					"User 2",
					ImportContactProperties.User2
				},
				{
					"Home Phone 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobile Phone",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffix",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"User 3",
					ImportContactProperties.User3
				},
				{
					"Company Main Phone",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Children",
					ImportContactProperties.Children
				},
				{
					"Business Fax",
					ImportContactProperties.BusinessFax
				},
				{
					"Home Postal Code",
					ImportContactProperties.HomePostalCode
				},
				{
					"Location",
					ImportContactProperties.Location
				},
				{
					"Billing Information",
					ImportContactProperties.BillingInformation
				},
				{
					"Home Fax",
					ImportContactProperties.HomeFax
				},
				{
					"Web Page",
					ImportContactProperties.WebPage
				},
				{
					"Home Street 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Business City",
					ImportContactProperties.BusinessCity
				},
				{
					"Other City",
					ImportContactProperties.OtherCity
				},
				{
					"Business Street",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-mail 3 Type",
					ImportContactProperties.Email3Type
				},
				{
					"Assistant's Name",
					ImportContactProperties.AssistantName
				},
				{
					"Gender",
					ImportContactProperties.Gender
				},
				{
					"Assistant's Phone",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-mail Display Name",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mail 3 Address",
					ImportContactProperties.Email3Address
				},
				{
					"Επώνυμο Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"User 4",
					ImportContactProperties.User4
				},
				{
					"Manager's Name",
					ImportContactProperties.ManagerName
				},
				{
					"Χώρα/Περιοχή εργασίας",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Callback",
					ImportContactProperties.Callback
				},
				{
					"Primary Phone",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mail Address",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Business Phone 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Home State",
					ImportContactProperties.HomeState
				},
				{
					"Other Fax",
					ImportContactProperties.OtherFax
				},
				{
					"Business Street 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Home City",
					ImportContactProperties.HomeCity
				},
				{
					"Other State",
					ImportContactProperties.OtherState
				},
				{
					"Business Street 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Title",
					ImportContactProperties.Title
				},
				{
					"Home Street 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Office Location",
					ImportContactProperties.OfficeLocation
				},
				{
					"Όνομα Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Χώρα/Περιοχή οικίας",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Car Phone",
					ImportContactProperties.CarPhone
				},
				{
					"Company",
					ImportContactProperties.Company
				},
				{
					"Τ.Θ. διεύθυνσης οικίας",
					ImportContactProperties.HomePOBox
				},
				{
					"Τ.Θ. άλλης διεύθυνσης",
					ImportContactProperties.OtherPOBox
				},
				{
					"Home Street",
					ImportContactProperties.HomeStreet
				},
				{
					"E-mail 3 Display Name",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Sensitivity",
					ImportContactProperties.Sensitivity
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"First Name",
					ImportContactProperties.FirstName
				},
				{
					"Other Street",
					ImportContactProperties.OtherStreet
				},
				{
					"Middle Name",
					ImportContactProperties.MiddleName
				},
				{
					"Mileage",
					ImportContactProperties.Mileage
				},
				{
					"User 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1032));
		}

		private OutlookCsvLanguage CreateLanguageObject_en_us()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"Title",
					ImportContactProperties.Title
				},
				{
					"First Name",
					ImportContactProperties.FirstName
				},
				{
					"Middle Name",
					ImportContactProperties.MiddleName
				},
				{
					"Last Name",
					ImportContactProperties.LastName
				},
				{
					"Suffix",
					ImportContactProperties.Suffix
				},
				{
					"Job Title",
					ImportContactProperties.JobTitle
				},
				{
					"Nickname",
					ImportContactProperties.Nickname
				},
				{
					"Company",
					ImportContactProperties.Company
				},
				{
					"Department",
					ImportContactProperties.Department
				},
				{
					"Business Street",
					ImportContactProperties.BusinessStreet
				},
				{
					"Business Street 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Business Street 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Business City",
					ImportContactProperties.BusinessCity
				},
				{
					"Business State",
					ImportContactProperties.BusinessState
				},
				{
					"Business Postal Code",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Business Country/Region",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Home Street",
					ImportContactProperties.HomeStreet
				},
				{
					"Home Street 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Home Street 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Home City",
					ImportContactProperties.HomeCity
				},
				{
					"Home State",
					ImportContactProperties.HomeState
				},
				{
					"Home Postal Code",
					ImportContactProperties.HomePostalCode
				},
				{
					"Home Country/Region",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Other Street",
					ImportContactProperties.OtherStreet
				},
				{
					"Other Street 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Other Street 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Other City",
					ImportContactProperties.OtherCity
				},
				{
					"Other State",
					ImportContactProperties.OtherState
				},
				{
					"Other Postal Code",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Other Country/Region",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Assistant's Phone",
					ImportContactProperties.AssistantPhone
				},
				{
					"Business Fax",
					ImportContactProperties.BusinessFax
				},
				{
					"Business Phone",
					ImportContactProperties.BusinessPhone
				},
				{
					"Business Phone 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Callback",
					ImportContactProperties.Callback
				},
				{
					"Car Phone",
					ImportContactProperties.CarPhone
				},
				{
					"Company Main Phone",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Home Fax",
					ImportContactProperties.HomeFax
				},
				{
					"Home Phone",
					ImportContactProperties.HomePhone
				},
				{
					"Home Phone 2",
					ImportContactProperties.HomePhone2
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Mobile Phone",
					ImportContactProperties.MobilePhone
				},
				{
					"Other Fax",
					ImportContactProperties.OtherFax
				},
				{
					"Other Phone",
					ImportContactProperties.OtherPhone
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Primary Phone",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Radio Phone",
					ImportContactProperties.RadioPhone
				},
				{
					"TTY/TDD Phone",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Anniversary",
					ImportContactProperties.Anniversary
				},
				{
					"Assistant's Name",
					ImportContactProperties.AssistantName
				},
				{
					"Birthday",
					ImportContactProperties.Birthday
				},
				{
					"Business Address PO Box",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Categories",
					ImportContactProperties.Categories
				},
				{
					"Children",
					ImportContactProperties.Children
				},
				{
					"E-mail Address",
					ImportContactProperties.EmailAddress
				},
				{
					"E-mail Type",
					ImportContactProperties.EmailType
				},
				{
					"E-mail Display Name",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mail 2 Address",
					ImportContactProperties.Email2Address
				},
				{
					"E-mail 2 Type",
					ImportContactProperties.Email2Type
				},
				{
					"E-mail 2 Display Name",
					ImportContactProperties.Email2DisplayName
				},
				{
					"E-mail 3 Address",
					ImportContactProperties.Email3Address
				},
				{
					"E-mail 3 Type",
					ImportContactProperties.Email3Type
				},
				{
					"E-mail 3 Display Name",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Government ID Number",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Home Address PO Box",
					ImportContactProperties.HomePOBox
				},
				{
					"Initials",
					ImportContactProperties.Initials
				},
				{
					"Internet Free Busy",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Manager's Name",
					ImportContactProperties.ManagerName
				},
				{
					"Mileage",
					ImportContactProperties.Mileage
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"Office Location",
					ImportContactProperties.OfficeLocation
				},
				{
					"Other Address PO Box",
					ImportContactProperties.OtherPOBox
				},
				{
					"Profession",
					ImportContactProperties.Profession
				},
				{
					"Spouse",
					ImportContactProperties.Spouse
				},
				{
					"User 1",
					ImportContactProperties.User1
				},
				{
					"User 2",
					ImportContactProperties.User2
				},
				{
					"User 3",
					ImportContactProperties.User3
				},
				{
					"User 4",
					ImportContactProperties.User4
				},
				{
					"Web Page",
					ImportContactProperties.WebPage
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Account",
					ImportContactProperties.Account
				},
				{
					"Billing Information",
					ImportContactProperties.BillingInformation
				},
				{
					"Location",
					ImportContactProperties.Location
				},
				{
					"Gender",
					ImportContactProperties.Gender
				},
				{
					"Priority",
					ImportContactProperties.Priority
				},
				{
					"Sensitivity",
					ImportContactProperties.Sensitivity
				},
				{
					"Given Name",
					ImportContactProperties.FirstName
				},
				{
					"Additional Name",
					ImportContactProperties.MiddleName
				},
				{
					"Family Name",
					ImportContactProperties.LastName
				},
				{
					"Given Name Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Family Name Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Name Prefix",
					ImportContactProperties.Title
				},
				{
					"Name Suffix",
					ImportContactProperties.Suffix
				},
				{
					"Occupation",
					ImportContactProperties.Profession
				},
				{
					"E-mail 1 - Value",
					ImportContactProperties.EmailAddress
				},
				{
					"E-mail 1 - Type",
					ImportContactProperties.IgnoredProperty
				},
				{
					"E-mail 2 - Value",
					ImportContactProperties.Email2Address
				},
				{
					"E-mail 2 - Type",
					ImportContactProperties.IgnoredProperty
				},
				{
					"E-mail 3 - Value",
					ImportContactProperties.Email3Address
				},
				{
					"E-mail 3 - Type",
					ImportContactProperties.IgnoredProperty
				},
				{
					"Business Address",
					ImportContactProperties.BusinessStreet
				},
				{
					"Business Country",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Home Address",
					ImportContactProperties.HomeStreet
				},
				{
					"Home Country",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Other Address",
					ImportContactProperties.OtherStreet
				},
				{
					"Other Country",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"FirstName",
					ImportContactProperties.FirstName
				},
				{
					"MiddleName",
					ImportContactProperties.MiddleName
				},
				{
					"LastName",
					ImportContactProperties.LastName
				},
				{
					"JobTitle",
					ImportContactProperties.JobTitle
				},
				{
					"BusinessStreet",
					ImportContactProperties.BusinessStreet
				},
				{
					"BusinessStreet2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"BusinessStreet3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"BusinessCity",
					ImportContactProperties.BusinessCity
				},
				{
					"BusinessState",
					ImportContactProperties.BusinessState
				},
				{
					"BusinessPostalCode",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"BusinessCountry",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"HomeStreet",
					ImportContactProperties.HomeStreet
				},
				{
					"HomeStreet2",
					ImportContactProperties.HomeStreet2
				},
				{
					"HomeStreet3",
					ImportContactProperties.HomeStreet3
				},
				{
					"HomeCity",
					ImportContactProperties.HomeCity
				},
				{
					"HomeState",
					ImportContactProperties.HomeState
				},
				{
					"HomePostalCode",
					ImportContactProperties.HomePostalCode
				},
				{
					"HomeCountry",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"E-mail",
					ImportContactProperties.EmailAddress
				},
				{
					"E-mail2",
					ImportContactProperties.Email2Address
				},
				{
					"WebPage",
					ImportContactProperties.WebPage
				},
				{
					"First",
					ImportContactProperties.FirstName
				},
				{
					"Middle",
					ImportContactProperties.MiddleName
				},
				{
					"Last",
					ImportContactProperties.LastName
				},
				{
					"Home ZIP",
					ImportContactProperties.HomePostalCode
				},
				{
					"Work Address",
					ImportContactProperties.BusinessStreet
				},
				{
					"Work City",
					ImportContactProperties.BusinessCity
				},
				{
					"Work State",
					ImportContactProperties.BusinessState
				},
				{
					"Work ZIP",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Work Country",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Business Website",
					ImportContactProperties.WebPage
				},
				{
					"Fax",
					ImportContactProperties.BusinessFax
				},
				{
					"Work",
					ImportContactProperties.BusinessPhone
				},
				{
					"Home",
					ImportContactProperties.HomePhone
				},
				{
					"Mobile",
					ImportContactProperties.MobilePhone
				},
				{
					"Other",
					ImportContactProperties.OtherPhone
				},
				{
					"Category",
					ImportContactProperties.Categories
				},
				{
					"Email",
					ImportContactProperties.EmailAddress
				},
				{
					"Alternate Email 1",
					ImportContactProperties.Email2Address
				},
				{
					"Alternate Email 2",
					ImportContactProperties.Email3Address
				},
				{
					"Mobile Phone 2",
					ImportContactProperties.MobilePhone2
				},
				{
					"Schools",
					ImportContactProperties.Schools
				},
				{
					"IMAddress",
					ImportContactProperties.IMAddress
				},
				{
					"Personal Web Page",
					ImportContactProperties.PersonalWebPage
				}
			}, new CultureInfo(1033));
		}

		private OutlookCsvLanguage CreateLanguageObject_es_es()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Dirección de correo electrónico 2",
					ImportContactProperties.Email2Address
				},
				{
					"Puesto",
					ImportContactProperties.JobTitle
				},
				{
					"Otro país o región",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Télex",
					ImportContactProperties.Telex
				},
				{
					"Internet Free Busy",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Otra calle 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesión",
					ImportContactProperties.Profession
				},
				{
					"E-mail Type",
					ImportContactProperties.EmailType
				},
				{
					"Account",
					ImportContactProperties.Account
				},
				{
					"Categorías",
					ImportContactProperties.Categories
				},
				{
					"Birthday",
					ImportContactProperties.Birthday
				},
				{
					"Apartado postal de la dirección del trabajo",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Otra calle 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Government ID Number",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Apellidos",
					ImportContactProperties.LastName
				},
				{
					"Radioteléfono",
					ImportContactProperties.RadioPhone
				},
				{
					"Anniversary",
					ImportContactProperties.Anniversary
				},
				{
					"Número de teletipo",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tipo de correo electrónico 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nombre de pantalla de correo electrónico 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Código postal del trabajo",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Otro teléfono",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioridad",
					ImportContactProperties.Priority
				},
				{
					"Initials",
					ImportContactProperties.Initials
				},
				{
					"Spouse",
					ImportContactProperties.Spouse
				},
				{
					"Department",
					ImportContactProperties.Department
				},
				{
					"Particular",
					ImportContactProperties.HomePhone
				},
				{
					"Teléfono del trabajo",
					ImportContactProperties.BusinessPhone
				},
				{
					"Compañía Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Otro código postal",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Provincia o estado de trabajo",
					ImportContactProperties.BusinessState
				},
				{
					"Usuario 2",
					ImportContactProperties.User2
				},
				{
					"Home Phone 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobile Phone",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffix",
					ImportContactProperties.Suffix
				},
				{
					"Aficiones",
					ImportContactProperties.Hobby
				},
				{
					"Usuario 3",
					ImportContactProperties.User3
				},
				{
					"Número de centralita de la organización",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Hijos",
					ImportContactProperties.Children
				},
				{
					"Business Fax",
					ImportContactProperties.BusinessFax
				},
				{
					"Home Postal Code",
					ImportContactProperties.HomePostalCode
				},
				{
					"Location",
					ImportContactProperties.Location
				},
				{
					"Facturación",
					ImportContactProperties.BillingInformation
				},
				{
					"Home Fax",
					ImportContactProperties.HomeFax
				},
				{
					"Página web",
					ImportContactProperties.WebPage
				},
				{
					"Home Street 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Ciudad de trabajo",
					ImportContactProperties.BusinessCity
				},
				{
					"Otra ciudad",
					ImportContactProperties.OtherCity
				},
				{
					"Calle del trabajo",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tipo de correo electrónico 3",
					ImportContactProperties.Email3Type
				},
				{
					"Assistant's Name",
					ImportContactProperties.AssistantName
				},
				{
					"Género",
					ImportContactProperties.Gender
				},
				{
					"Assistant's Phone",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nombre de pantalla de correo electrónico",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Dirección del correo electrónico 3",
					ImportContactProperties.Email3Address
				},
				{
					"Apellido Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Usuario 4",
					ImportContactProperties.User4
				},
				{
					"Nombre del director",
					ImportContactProperties.ManagerName
				},
				{
					"País o región del trabajo",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Devolución de llamada",
					ImportContactProperties.Callback
				},
				{
					"Teléfono principal",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mail Address",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Business Phone 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Home State",
					ImportContactProperties.HomeState
				},
				{
					"Otro fax",
					ImportContactProperties.OtherFax
				},
				{
					"Calle del trabajo 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Home City",
					ImportContactProperties.HomeCity
				},
				{
					"Otra provincia o estado",
					ImportContactProperties.OtherState
				},
				{
					"Calle del trabajo 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Title",
					ImportContactProperties.Title
				},
				{
					"Home Street 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Office Location",
					ImportContactProperties.OfficeLocation
				},
				{
					"Yomi especificada",
					ImportContactProperties.GivenYomi
				},
				{
					"País o región del domicilio",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Car Phone",
					ImportContactProperties.CarPhone
				},
				{
					"Organización",
					ImportContactProperties.Company
				},
				{
					"Apartado postal de la dirección personal",
					ImportContactProperties.HomePOBox
				},
				{
					"Otro apartado postal",
					ImportContactProperties.OtherPOBox
				},
				{
					"Home Street",
					ImportContactProperties.HomeStreet
				},
				{
					"Nombre de pantalla de correo electrónico 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Confidencialidad",
					ImportContactProperties.Sensitivity
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"Nombre",
					ImportContactProperties.FirstName
				},
				{
					"Otra calle",
					ImportContactProperties.OtherStreet
				},
				{
					"Segundo nombre",
					ImportContactProperties.MiddleName
				},
				{
					"Kilometraje",
					ImportContactProperties.Mileage
				},
				{
					"Usuario 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1034));
		}

		private OutlookCsvLanguage CreateLanguageObject_et_ee()
		{
			return new OutlookCsvLanguage(1257, new Dictionary<string, ImportContactProperties>
			{
				{
					"Meiliaadress 2",
					ImportContactProperties.Email2Address
				},
				{
					"Ametinimetus",
					ImportContactProperties.JobTitle
				},
				{
					"Muu riik/regioon",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Interneti vaba/hõivatud aja teave",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Muu tänav 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Elukutse",
					ImportContactProperties.Profession
				},
				{
					"Meilitüüp",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategooriad",
					ImportContactProperties.Categories
				},
				{
					"Sünnipäev",
					ImportContactProperties.Birthday
				},
				{
					"Tööaadressi postkast",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Muu tänav 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Valitsus-ID",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Perekonnanimi",
					ImportContactProperties.LastName
				},
				{
					"Raadiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Aastapäev",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Meilitüüp 2",
					ImportContactProperties.Email2Type
				},
				{
					"E-posti 2 kuvatav nimi",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Töökoha sihtnumber",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Muu telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioriteet",
					ImportContactProperties.Priority
				},
				{
					"Initsiaalid",
					ImportContactProperties.Initials
				},
				{
					"Abikaasa",
					ImportContactProperties.Spouse
				},
				{
					"Osakond",
					ImportContactProperties.Department
				},
				{
					"Kodutelefon",
					ImportContactProperties.HomePhone
				},
				{
					"Töötelefon",
					ImportContactProperties.BusinessPhone
				},
				{
					"Ettevõte yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Peiler",
					ImportContactProperties.Pager
				},
				{
					"Muu sihtnumber",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Töökoha osariik",
					ImportContactProperties.BusinessState
				},
				{
					"Kasutaja 2",
					ImportContactProperties.User2
				},
				{
					"Kodutelefon 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Nime järelliide",
					ImportContactProperties.Suffix
				},
				{
					"Huviala",
					ImportContactProperties.Hobby
				},
				{
					"Kasutaja 3",
					ImportContactProperties.User3
				},
				{
					"Ettevõtte põhitelefon",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Lapsed",
					ImportContactProperties.Children
				},
				{
					"Tööfaks",
					ImportContactProperties.BusinessFax
				},
				{
					"Kodu sihtnumber",
					ImportContactProperties.HomePostalCode
				},
				{
					"Asukoht",
					ImportContactProperties.Location
				},
				{
					"Arveldusteave",
					ImportContactProperties.BillingInformation
				},
				{
					"Kodufaks",
					ImportContactProperties.HomeFax
				},
				{
					"Veebileht",
					ImportContactProperties.WebPage
				},
				{
					"Kodutänav 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Töökoha linn",
					ImportContactProperties.BusinessCity
				},
				{
					"Muu linn",
					ImportContactProperties.OtherCity
				},
				{
					"Töökoha tänav",
					ImportContactProperties.BusinessStreet
				},
				{
					"Meilitüüp 3",
					ImportContactProperties.Email3Type
				},
				{
					"Abi nimi",
					ImportContactProperties.AssistantName
				},
				{
					"Sugu",
					ImportContactProperties.Gender
				},
				{
					"Abi telefon",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-posti kuvatav nimi",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Meiliaadress 3",
					ImportContactProperties.Email3Address
				},
				{
					"Liignimi yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Kasutaja 4",
					ImportContactProperties.User4
				},
				{
					"Juhi nimi",
					ImportContactProperties.ManagerName
				},
				{
					"Töökoha riik/regioon",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Tagasihelistamine",
					ImportContactProperties.Callback
				},
				{
					"Esmane telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Meiliaadress",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Töötelefon 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Koduosariik",
					ImportContactProperties.HomeState
				},
				{
					"Muu faks",
					ImportContactProperties.OtherFax
				},
				{
					"Töökoha tänav 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Kodulinn",
					ImportContactProperties.HomeCity
				},
				{
					"Muu osariik",
					ImportContactProperties.OtherState
				},
				{
					"Töökoha tänav 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Tiitel",
					ImportContactProperties.Title
				},
				{
					"Kodutänav 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Kontori asukoht",
					ImportContactProperties.OfficeLocation
				},
				{
					"Eesnimi yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Koduriik/-regioon",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autotelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Ettevõte",
					ImportContactProperties.Company
				},
				{
					"Koduse aadressi postkast",
					ImportContactProperties.HomePOBox
				},
				{
					"Muu aadressi postkast",
					ImportContactProperties.OtherPOBox
				},
				{
					"Kodutänav",
					ImportContactProperties.HomeStreet
				},
				{
					"E-posti 3 kuvatav nimi",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Tundlikkus",
					ImportContactProperties.Sensitivity
				},
				{
					"Märkmed",
					ImportContactProperties.Notes
				},
				{
					"Eesnimi",
					ImportContactProperties.FirstName
				},
				{
					"Muu tänav",
					ImportContactProperties.OtherStreet
				},
				{
					"Teine nimi",
					ImportContactProperties.MiddleName
				},
				{
					"Läbitud vahemaa",
					ImportContactProperties.Mileage
				},
				{
					"Kasutaja 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1061));
		}

		private OutlookCsvLanguage CreateLanguageObject_fi_fi()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Sähköpostiosoite 2",
					ImportContactProperties.Email2Address
				},
				{
					"Tehtävänimike",
					ImportContactProperties.JobTitle
				},
				{
					"Maa/alue (muu)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleksi",
					ImportContactProperties.Telex
				},
				{
					"Vapaat ja varatut ajat",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Lähiosoite (muu) 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Ammatti",
					ImportContactProperties.Profession
				},
				{
					"Sähköpostin laji",
					ImportContactProperties.EmailType
				},
				{
					"Tili",
					ImportContactProperties.Account
				},
				{
					"Luokat",
					ImportContactProperties.Categories
				},
				{
					"Syntymäpäivä",
					ImportContactProperties.Birthday
				},
				{
					"Yrityksen osoite (postilokero)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Lähiosoite (muu) 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Henkilötunnus",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Sukunimi",
					ImportContactProperties.LastName
				},
				{
					"Radiopuhelin",
					ImportContactProperties.RadioPhone
				},
				{
					"Vuosipäivä",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD-puhelin",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Sähköposti 2:n laji",
					ImportContactProperties.Email2Type
				},
				{
					"Näytettävä sähköpostinimi 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Postinumero (työ)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Muu puhelin",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioriteetti",
					ImportContactProperties.Priority
				},
				{
					"Nimikirjaimet",
					ImportContactProperties.Initials
				},
				{
					"Puoliso",
					ImportContactProperties.Spouse
				},
				{
					"Osasto",
					ImportContactProperties.Department
				},
				{
					"Kotipuhelin",
					ImportContactProperties.HomePhone
				},
				{
					"Työpuhelin",
					ImportContactProperties.BusinessPhone
				},
				{
					"Yritys Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Hakulaite",
					ImportContactProperties.Pager
				},
				{
					"Postinumero (muu)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Sijaintitiedot (työ)",
					ImportContactProperties.BusinessState
				},
				{
					"Käyttäjä 2",
					ImportContactProperties.User2
				},
				{
					"Kotipuhelin 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Matkapuhelin",
					ImportContactProperties.MobilePhone
				},
				{
					"Jälkiliite",
					ImportContactProperties.Suffix
				},
				{
					"Harrastus",
					ImportContactProperties.Hobby
				},
				{
					"Käyttäjä 3",
					ImportContactProperties.User3
				},
				{
					"Yrityksen puhelinvaihde",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Lapset",
					ImportContactProperties.Children
				},
				{
					"Työfaksi",
					ImportContactProperties.BusinessFax
				},
				{
					"Postinumero (koti)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Sijainti",
					ImportContactProperties.Location
				},
				{
					"Laskutustiedot",
					ImportContactProperties.BillingInformation
				},
				{
					"Kotifaksi",
					ImportContactProperties.HomeFax
				},
				{
					"Web-sivu",
					ImportContactProperties.WebPage
				},
				{
					"Lähiosoite 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Postitoimipaikka (työ)",
					ImportContactProperties.BusinessCity
				},
				{
					"Postitoimipaikka (muu)",
					ImportContactProperties.OtherCity
				},
				{
					"Lähiosoite (työ)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Sähköposti 3:n laji",
					ImportContactProperties.Email3Type
				},
				{
					"Avustajan nimi",
					ImportContactProperties.AssistantName
				},
				{
					"Sukupuoli",
					ImportContactProperties.Gender
				},
				{
					"Avustajan puhelinnumero",
					ImportContactProperties.AssistantPhone
				},
				{
					"Näytettävä sähköpostinimi",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Sähköpostiosoite 3",
					ImportContactProperties.Email3Address
				},
				{
					"Sukunimi Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Käyttäjä 4",
					ImportContactProperties.User4
				},
				{
					"Vastuuhenkilön nimi",
					ImportContactProperties.ManagerName
				},
				{
					"Maa/alue (työ)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Takaisinsoitto",
					ImportContactProperties.Callback
				},
				{
					"Ensisijainen puhelin",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Sähköpostiosoite",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Työpuhelin 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Sijaintitiedot (koti)",
					ImportContactProperties.HomeState
				},
				{
					"Muu faksi",
					ImportContactProperties.OtherFax
				},
				{
					"Lähiosoite (työ) 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Postitoimipaikka (koti)",
					ImportContactProperties.HomeCity
				},
				{
					"Sijaintitiedot (muu)",
					ImportContactProperties.OtherState
				},
				{
					"Lähiosoite (työ) 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Lähiosoite 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Toimisto",
					ImportContactProperties.OfficeLocation
				},
				{
					"Etunimi Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Maa/alue (koti)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autopuhelin",
					ImportContactProperties.CarPhone
				},
				{
					"Yritys",
					ImportContactProperties.Company
				},
				{
					"Kotiosoite (postilokero)",
					ImportContactProperties.HomePOBox
				},
				{
					"Muu osoite (postilokero)",
					ImportContactProperties.OtherPOBox
				},
				{
					"Lähiosoite (koti)",
					ImportContactProperties.HomeStreet
				},
				{
					"Näytettävä sähköpostinimi 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Luottamuksellisuus",
					ImportContactProperties.Sensitivity
				},
				{
					"Muistilaput",
					ImportContactProperties.Notes
				},
				{
					"Etunimi",
					ImportContactProperties.FirstName
				},
				{
					"Lähiosoite (muu)",
					ImportContactProperties.OtherStreet
				},
				{
					"Toinen nimi",
					ImportContactProperties.MiddleName
				},
				{
					"Matka",
					ImportContactProperties.Mileage
				},
				{
					"Käyttäjä 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1035));
		}

		private OutlookCsvLanguage CreateLanguageObject_fr_fr()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Adresse de messagerie 2",
					ImportContactProperties.Email2Address
				},
				{
					"Titre",
					ImportContactProperties.JobTitle
				},
				{
					"Pays/Région (autre)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Télex",
					ImportContactProperties.Telex
				},
				{
					"Disponibilité Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Rue (autre) 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profession",
					ImportContactProperties.Profession
				},
				{
					"Type de messagerie",
					ImportContactProperties.EmailType
				},
				{
					"Compte",
					ImportContactProperties.Account
				},
				{
					"Catégories",
					ImportContactProperties.Categories
				},
				{
					"Anniversaire",
					ImportContactProperties.Birthday
				},
				{
					"B.P. professionnelle",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Rue (autre) 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Code gouvernement",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Nom",
					ImportContactProperties.LastName
				},
				{
					"Radio téléphone",
					ImportContactProperties.RadioPhone
				},
				{
					"Anniversaire de mariage ou fête",
					ImportContactProperties.Anniversary
				},
				{
					"Téléphone TDD/TTY",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Type de messagerie 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nom complet de l'adresse de messagerie 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Code postal (bureau)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Téléphone (autre)",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorité",
					ImportContactProperties.Priority
				},
				{
					"Initiales",
					ImportContactProperties.Initials
				},
				{
					"Conjoint(e)",
					ImportContactProperties.Spouse
				},
				{
					"Service",
					ImportContactProperties.Department
				},
				{
					"Téléphone (domicile)",
					ImportContactProperties.HomePhone
				},
				{
					"Téléphone (bureau)",
					ImportContactProperties.BusinessPhone
				},
				{
					"Société Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Récepteur de radiomessagerie",
					ImportContactProperties.Pager
				},
				{
					"Code postal (autre)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Dép/Région (bureau)",
					ImportContactProperties.BusinessState
				},
				{
					"Utilisateur 2",
					ImportContactProperties.User2
				},
				{
					"Téléphone 2 (domicile)",
					ImportContactProperties.HomePhone2
				},
				{
					"Tél. mobile",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffixe",
					ImportContactProperties.Suffix
				},
				{
					"Passe-temps",
					ImportContactProperties.Hobby
				},
				{
					"Utilisateur 3",
					ImportContactProperties.User3
				},
				{
					"Téléphone société",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Enfants",
					ImportContactProperties.Children
				},
				{
					"Télécopie (bureau)",
					ImportContactProperties.BusinessFax
				},
				{
					"Code postal (domicile)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Emplacement",
					ImportContactProperties.Location
				},
				{
					"Informations facturation",
					ImportContactProperties.BillingInformation
				},
				{
					"Télécopie (domicile)",
					ImportContactProperties.HomeFax
				},
				{
					"Page Web",
					ImportContactProperties.WebPage
				},
				{
					"Rue (domicile) 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Ville (bureau)",
					ImportContactProperties.BusinessCity
				},
				{
					"Ville (autre)",
					ImportContactProperties.OtherCity
				},
				{
					"Rue (bureau)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Type de messagerie 3",
					ImportContactProperties.Email3Type
				},
				{
					"Nom de l'assistant(e)",
					ImportContactProperties.AssistantName
				},
				{
					"Sexe",
					ImportContactProperties.Gender
				},
				{
					"Téléphone de l'assistant(e)",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nom complet de l'adresse de messagerie",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Adresse de messagerie 3",
					ImportContactProperties.Email3Address
				},
				{
					"Nom Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Utilisateur 4",
					ImportContactProperties.User4
				},
				{
					"Responsable",
					ImportContactProperties.ManagerName
				},
				{
					"Pays/Région (bureau)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Rappel",
					ImportContactProperties.Callback
				},
				{
					"Téléphone principal",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Adresse de messagerie",
					ImportContactProperties.EmailAddress
				},
				{
					"RNIS",
					ImportContactProperties.ISDN
				},
				{
					"Téléphone 2 (bureau)",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Dép/Région (domicile)",
					ImportContactProperties.HomeState
				},
				{
					"Télécopie (autre)",
					ImportContactProperties.OtherFax
				},
				{
					"Rue (bureau) 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Ville (domicile)",
					ImportContactProperties.HomeCity
				},
				{
					"Dép/Région (autre)",
					ImportContactProperties.OtherState
				},
				{
					"Rue (bureau) 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Rue (domicile) 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Bureau",
					ImportContactProperties.OfficeLocation
				},
				{
					"Prénom Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Pays/Région (domicile)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Téléphone (voiture)",
					ImportContactProperties.CarPhone
				},
				{
					"Société",
					ImportContactProperties.Company
				},
				{
					"Boîte postale du domicile",
					ImportContactProperties.HomePOBox
				},
				{
					"Autre boîte postale",
					ImportContactProperties.OtherPOBox
				},
				{
					"Rue (domicile)",
					ImportContactProperties.HomeStreet
				},
				{
					"Nom complet de l'adresse de messagerie 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Critère de diffusion",
					ImportContactProperties.Sensitivity
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"Prénom",
					ImportContactProperties.FirstName
				},
				{
					"Rue (autre)",
					ImportContactProperties.OtherStreet
				},
				{
					"Deuxième prénom",
					ImportContactProperties.MiddleName
				},
				{
					"Kilométrage",
					ImportContactProperties.Mileage
				},
				{
					"Utilisateur 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1036));
		}

		private OutlookCsvLanguage CreateLanguageObject_he_il()
		{
			return new OutlookCsvLanguage(1255, new Dictionary<string, ImportContactProperties>
			{
				{
					"כתובת דואר אלקטרוני 2",
					ImportContactProperties.Email2Address
				},
				{
					"תפקיד",
					ImportContactProperties.JobTitle
				},
				{
					"מדינה/אזור אחר/ת",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"טלקס",
					ImportContactProperties.Telex
				},
				{
					"מועדים פנויים/לא פנויים באינטרנט",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"רחוב 2 אחר",
					ImportContactProperties.OtherStreet2
				},
				{
					"מקצוע",
					ImportContactProperties.Profession
				},
				{
					"סוג דואר אלקטרוני",
					ImportContactProperties.EmailType
				},
				{
					"חשבון",
					ImportContactProperties.Account
				},
				{
					"קטגוריות",
					ImportContactProperties.Categories
				},
				{
					"יום הולדת",
					ImportContactProperties.Birthday
				},
				{
					"תא דואר של כתובת העבודה",
					ImportContactProperties.BusinessPOBox
				},
				{
					"רחוב 3 אחר",
					ImportContactProperties.OtherStreet3
				},
				{
					"מספר זיהוי ממשלתי",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"שם משפחה",
					ImportContactProperties.LastName
				},
				{
					"טלפון רדיו",
					ImportContactProperties.RadioPhone
				},
				{
					"יום נישואין",
					ImportContactProperties.Anniversary
				},
				{
					"טלפון טקסט (TTY/TDD)",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"סוג דואר אלקטרוני 2",
					ImportContactProperties.Email2Type
				},
				{
					"שם תצוגה של דואר אלקטרוני 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"מיקוד כתובת העבודה",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"טלפון אחר",
					ImportContactProperties.OtherPhone
				},
				{
					"עדיפות",
					ImportContactProperties.Priority
				},
				{
					"ראשי תיבות",
					ImportContactProperties.Initials
				},
				{
					"בן/בת זוג",
					ImportContactProperties.Spouse
				},
				{
					"מחלקה",
					ImportContactProperties.Department
				},
				{
					"טלפון בבית",
					ImportContactProperties.HomePhone
				},
				{
					"טלפון בעבודה",
					ImportContactProperties.BusinessPhone
				},
				{
					"יומי חברה",
					ImportContactProperties.CompanyYomi
				},
				{
					"איתורית",
					ImportContactProperties.Pager
				},
				{
					"מיקוד אחר",
					ImportContactProperties.OtherPostalCode
				},
				{
					"אזור כתובת העבודה",
					ImportContactProperties.BusinessState
				},
				{
					"משתמש 2",
					ImportContactProperties.User2
				},
				{
					"טלפון בבית 2",
					ImportContactProperties.HomePhone2
				},
				{
					"טלפון נייד",
					ImportContactProperties.MobilePhone
				},
				{
					"סיומת",
					ImportContactProperties.Suffix
				},
				{
					"תחביב",
					ImportContactProperties.Hobby
				},
				{
					"משתמש 3",
					ImportContactProperties.User3
				},
				{
					"טלפון ראשי של החברה",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"ילדים",
					ImportContactProperties.Children
				},
				{
					"פקס בעבודה",
					ImportContactProperties.BusinessFax
				},
				{
					"מיקוד כתובת הבית",
					ImportContactProperties.HomePostalCode
				},
				{
					"מיקום",
					ImportContactProperties.Location
				},
				{
					"מידע חיוב",
					ImportContactProperties.BillingInformation
				},
				{
					"פקס בבית",
					ImportContactProperties.HomeFax
				},
				{
					"דף אינטרנט",
					ImportContactProperties.WebPage
				},
				{
					"רחוב 2 של כתובת הבית",
					ImportContactProperties.HomeStreet2
				},
				{
					"עיר כתובת העבודה",
					ImportContactProperties.BusinessCity
				},
				{
					"עיר אחרת",
					ImportContactProperties.OtherCity
				},
				{
					"רחוב כתובת העבודה",
					ImportContactProperties.BusinessStreet
				},
				{
					"סוג דואר אלקטרוני 3",
					ImportContactProperties.Email3Type
				},
				{
					"שם העוזר",
					ImportContactProperties.AssistantName
				},
				{
					"מין",
					ImportContactProperties.Gender
				},
				{
					"טלפון של העוזר",
					ImportContactProperties.AssistantPhone
				},
				{
					"שם התצוגה של הדואר האלקטרוני",
					ImportContactProperties.EmailDisplayName
				},
				{
					"כתובת דואר אלקטרוני 3",
					ImportContactProperties.Email3Address
				},
				{
					"יומי משפחה",
					ImportContactProperties.SurnameYomi
				},
				{
					"משתמש 4",
					ImportContactProperties.User4
				},
				{
					"שם מנהל",
					ImportContactProperties.ManagerName
				},
				{
					"מדינה/אזור של העבודה",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"התקשרות חזרה",
					ImportContactProperties.Callback
				},
				{
					"טלפון עיקרי",
					ImportContactProperties.PrimaryPhone
				},
				{
					"כתובת דואר אלקטרוני",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"טלפון בעבודה 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"אזור כתובת הבית",
					ImportContactProperties.HomeState
				},
				{
					"פקס אחר",
					ImportContactProperties.OtherFax
				},
				{
					"רחוב 3 של כתובת העבודה",
					ImportContactProperties.BusinessStreet3
				},
				{
					"עיר כתובת הבית",
					ImportContactProperties.HomeCity
				},
				{
					"אזור אחר",
					ImportContactProperties.OtherState
				},
				{
					"רחוב 2 של כתובת העבודה",
					ImportContactProperties.BusinessStreet2
				},
				{
					"תואר",
					ImportContactProperties.Title
				},
				{
					"רחוב 3 של כתובת הבית",
					ImportContactProperties.HomeStreet3
				},
				{
					"מיקום משרד",
					ImportContactProperties.OfficeLocation
				},
				{
					"יומי פרטי",
					ImportContactProperties.GivenYomi
				},
				{
					"מדינה/אזור של הבית",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"טלפון ברכב",
					ImportContactProperties.CarPhone
				},
				{
					"חברה",
					ImportContactProperties.Company
				},
				{
					"תא דואר של כתובת הבית",
					ImportContactProperties.HomePOBox
				},
				{
					"תא דואר של כתובת אחרת",
					ImportContactProperties.OtherPOBox
				},
				{
					"רחוב כתובת הבית",
					ImportContactProperties.HomeStreet
				},
				{
					"שם תצוגה של דואר אלקטרוני 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"רגישות",
					ImportContactProperties.Sensitivity
				},
				{
					"הערות",
					ImportContactProperties.Notes
				},
				{
					"שם פרטי",
					ImportContactProperties.FirstName
				},
				{
					"רחוב אחר",
					ImportContactProperties.OtherStreet
				},
				{
					"שם פרטי שני",
					ImportContactProperties.MiddleName
				},
				{
					"מרחק",
					ImportContactProperties.Mileage
				},
				{
					"משתמש 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1037));
		}

		private OutlookCsvLanguage CreateLanguageObject_hr_hr()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"Adresa e-pošte 2",
					ImportContactProperties.Email2Address
				},
				{
					"Poslovna titula",
					ImportContactProperties.JobTitle
				},
				{
					"Druga država/regija",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Internetsko objavljivanje zauzetosti",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Druga adresa - ulica 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Zanimanje",
					ImportContactProperties.Profession
				},
				{
					"Vrsta e-pošte",
					ImportContactProperties.EmailType
				},
				{
					"Račun",
					ImportContactProperties.Account
				},
				{
					"Kategorije",
					ImportContactProperties.Categories
				},
				{
					"Rođendan",
					ImportContactProperties.Birthday
				},
				{
					"Službena adresa - poštanski pretinac",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Druga adresa - ulica 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"JMBG",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Prezime",
					ImportContactProperties.LastName
				},
				{
					"Radijski telefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Obljetnica",
					ImportContactProperties.Anniversary
				},
				{
					"Tekstualni telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Vrsta e-pošte 2",
					ImportContactProperties.Email2Type
				},
				{
					"Prikazano ime e-pošte 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Službena adresa - poštanski broj",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Drugi telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritet",
					ImportContactProperties.Priority
				},
				{
					"Inicijali",
					ImportContactProperties.Initials
				},
				{
					"Supružnik",
					ImportContactProperties.Spouse
				},
				{
					"Odjel",
					ImportContactProperties.Department
				},
				{
					"Kućni telefon",
					ImportContactProperties.HomePhone
				},
				{
					"Službeni telefon",
					ImportContactProperties.BusinessPhone
				},
				{
					"Tvrtka Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Javljač",
					ImportContactProperties.Pager
				},
				{
					"Druga adresa - poštanski broj",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Službena adresa - savezna država",
					ImportContactProperties.BusinessState
				},
				{
					"Korisnik 2",
					ImportContactProperties.User2
				},
				{
					"Kućni telefon 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobitel",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufiks",
					ImportContactProperties.Suffix
				},
				{
					"Hobi",
					ImportContactProperties.Hobby
				},
				{
					"Korisnik 3",
					ImportContactProperties.User3
				},
				{
					"Telefonski broj centrale tvrtke",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Djeca",
					ImportContactProperties.Children
				},
				{
					"Službeni faks",
					ImportContactProperties.BusinessFax
				},
				{
					"Kućna adresa - poštanski broj",
					ImportContactProperties.HomePostalCode
				},
				{
					"Mjesto",
					ImportContactProperties.Location
				},
				{
					"Informacija za naplatu",
					ImportContactProperties.BillingInformation
				},
				{
					"Kućni faks",
					ImportContactProperties.HomeFax
				},
				{
					"Web-stranica",
					ImportContactProperties.WebPage
				},
				{
					"Kućna adresa - ulica 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Službena adresa - grad",
					ImportContactProperties.BusinessCity
				},
				{
					"Druga adresa - grad",
					ImportContactProperties.OtherCity
				},
				{
					"Službena adresa - ulica",
					ImportContactProperties.BusinessStreet
				},
				{
					"Vrsta e-pošte 3",
					ImportContactProperties.Email3Type
				},
				{
					"Ime pomoćnika",
					ImportContactProperties.AssistantName
				},
				{
					"Spol",
					ImportContactProperties.Gender
				},
				{
					"Pomoćnikov telefon",
					ImportContactProperties.AssistantPhone
				},
				{
					"Prikazano ime e-pošte",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Adresa e-pošte 3",
					ImportContactProperties.Email3Address
				},
				{
					"Prezime Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Korisnik 4",
					ImportContactProperties.User4
				},
				{
					"Naziv rukovoditelja",
					ImportContactProperties.ManagerName
				},
				{
					"Država/regija poslovanja",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Povratni poziv",
					ImportContactProperties.Callback
				},
				{
					"Primarni telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Adresa e-pošte",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Službeni telefon 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Kućna adresa - savezna država",
					ImportContactProperties.HomeState
				},
				{
					"Drugi faks",
					ImportContactProperties.OtherFax
				},
				{
					"Službena adresa - ulica 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Kućna adresa - grad",
					ImportContactProperties.HomeCity
				},
				{
					"Druga adresa - savezna država",
					ImportContactProperties.OtherState
				},
				{
					"Službena adresa - ulica 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titula",
					ImportContactProperties.Title
				},
				{
					"Kućna adresa - ulica 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Mjesto ureda",
					ImportContactProperties.OfficeLocation
				},
				{
					"Ime Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Matična država/regija",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Automobilski telefon",
					ImportContactProperties.CarPhone
				},
				{
					"Tvrtka",
					ImportContactProperties.Company
				},
				{
					"Kućna adresa - poštanski pretinac",
					ImportContactProperties.HomePOBox
				},
				{
					"Druga adresa - poštanski pretinac",
					ImportContactProperties.OtherPOBox
				},
				{
					"Kućna adresa - ulica",
					ImportContactProperties.HomeStreet
				},
				{
					"Prikazano ime e-pošte 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Osjetljivost",
					ImportContactProperties.Sensitivity
				},
				{
					"Bilješke",
					ImportContactProperties.Notes
				},
				{
					"Ime",
					ImportContactProperties.FirstName
				},
				{
					"Druga adresa - ulica",
					ImportContactProperties.OtherStreet
				},
				{
					"Srednje ime",
					ImportContactProperties.MiddleName
				},
				{
					"Kilometraža",
					ImportContactProperties.Mileage
				},
				{
					"Korisnik 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1050));
		}

		private OutlookCsvLanguage CreateLanguageObject_hu_hu()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"2. e-mail cím",
					ImportContactProperties.Email2Address
				},
				{
					"Beosztás",
					ImportContactProperties.JobTitle
				},
				{
					"Más ország/régió",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Elfoglaltság közzététele az interneten",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"2. utcacím",
					ImportContactProperties.OtherStreet2
				},
				{
					"Foglalkozás",
					ImportContactProperties.Profession
				},
				{
					"E-mail típusa",
					ImportContactProperties.EmailType
				},
				{
					"Számla",
					ImportContactProperties.Account
				},
				{
					"Kategóriák",
					ImportContactProperties.Categories
				},
				{
					"Születésnap",
					ImportContactProperties.Birthday
				},
				{
					"Hivatali cím, postafiók",
					ImportContactProperties.BusinessPOBox
				},
				{
					"3. utcacím",
					ImportContactProperties.OtherStreet3
				},
				{
					"Kormányzati azonosító",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Vezetéknév",
					ImportContactProperties.LastName
				},
				{
					"Rádiótelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Évforduló",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"2. e-mail típusa",
					ImportContactProperties.Email2Type
				},
				{
					"2. e-mailhez megjelenítendő név",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Munkahely irányítószáma",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Egyéb telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritás",
					ImportContactProperties.Priority
				},
				{
					"Monogram",
					ImportContactProperties.Initials
				},
				{
					"Házastárs",
					ImportContactProperties.Spouse
				},
				{
					"Osztály",
					ImportContactProperties.Department
				},
				{
					"Otthoni telefon",
					ImportContactProperties.HomePhone
				},
				{
					"Hivatali telefon",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Személyhívó",
					ImportContactProperties.Pager
				},
				{
					"Más irányítószám",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Megye (hivatal)",
					ImportContactProperties.BusinessState
				},
				{
					"Felhasználói 2",
					ImportContactProperties.User2
				},
				{
					"Másik otthoni telefon",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Utótag",
					ImportContactProperties.Suffix
				},
				{
					"Hobbi",
					ImportContactProperties.Hobby
				},
				{
					"Felhasználói 3",
					ImportContactProperties.User3
				},
				{
					"Cég fővonala",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Gyerekek",
					ImportContactProperties.Children
				},
				{
					"Hivatali fax",
					ImportContactProperties.BusinessFax
				},
				{
					"Irányítószám (lakás)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Hely",
					ImportContactProperties.Location
				},
				{
					"Számlaadatok",
					ImportContactProperties.BillingInformation
				},
				{
					"Otthoni fax",
					ImportContactProperties.HomeFax
				},
				{
					"Weblap",
					ImportContactProperties.WebPage
				},
				{
					"2. otthoni utcacím",
					ImportContactProperties.HomeStreet2
				},
				{
					"Város (hivatal)",
					ImportContactProperties.BusinessCity
				},
				{
					"Más város",
					ImportContactProperties.OtherCity
				},
				{
					"Munkahely címe",
					ImportContactProperties.BusinessStreet
				},
				{
					"3. e-mail típusa",
					ImportContactProperties.Email3Type
				},
				{
					"Asszisztens neve",
					ImportContactProperties.AssistantName
				},
				{
					"Nem",
					ImportContactProperties.Gender
				},
				{
					"Asszisztens telefonszáma",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-mailhez megjelenítendő név",
					ImportContactProperties.EmailDisplayName
				},
				{
					"3. e-mail cím",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Felhasználói 4",
					ImportContactProperties.User4
				},
				{
					"Felettes neve",
					ImportContactProperties.ManagerName
				},
				{
					"Ország/régió (hivatal)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Visszahívás",
					ImportContactProperties.Callback
				},
				{
					"Elsődleges telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mail cím",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Másik hivatali telefon",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Megye (lakás)",
					ImportContactProperties.HomeState
				},
				{
					"Egyéb fax",
					ImportContactProperties.OtherFax
				},
				{
					"3. vállalati utcacím",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Város (lakás)",
					ImportContactProperties.HomeCity
				},
				{
					"Más megye",
					ImportContactProperties.OtherState
				},
				{
					"2. vállalati utcacím",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Cím",
					ImportContactProperties.Title
				},
				{
					"3. otthoni utcacím",
					ImportContactProperties.HomeStreet3
				},
				{
					"Iroda helye",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Ország/régió (lakás)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autótelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Cég",
					ImportContactProperties.Company
				},
				{
					"Otthoni cím, postafiók",
					ImportContactProperties.HomePOBox
				},
				{
					"Egyéb cím, postafiók",
					ImportContactProperties.OtherPOBox
				},
				{
					"Lakcím",
					ImportContactProperties.HomeStreet
				},
				{
					"3. e-mailhez megjelenítendő név",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Tartalom",
					ImportContactProperties.Sensitivity
				},
				{
					"Feljegyzések",
					ImportContactProperties.Notes
				},
				{
					"Utónév",
					ImportContactProperties.FirstName
				},
				{
					"Más utcacím",
					ImportContactProperties.OtherStreet
				},
				{
					"Középső",
					ImportContactProperties.MiddleName
				},
				{
					"Távolság",
					ImportContactProperties.Mileage
				},
				{
					"Felhasználói 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1038));
		}

		private OutlookCsvLanguage CreateLanguageObject_it_it()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Indirizzo posta elettronica 2",
					ImportContactProperties.Email2Address
				},
				{
					"Posizione",
					ImportContactProperties.JobTitle
				},
				{
					"Altro paese",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Disponibilità Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Altra via 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Professione",
					ImportContactProperties.Profession
				},
				{
					"Tipo posta elettronica",
					ImportContactProperties.EmailType
				},
				{
					"Account",
					ImportContactProperties.Account
				},
				{
					"Categorie",
					ImportContactProperties.Categories
				},
				{
					"Compleanno",
					ImportContactProperties.Birthday
				},
				{
					"Indirizzo (uff.) - Casella postale",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Altra via 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Cod. Fisc./P. IVA",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Cognome",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefono",
					ImportContactProperties.RadioPhone
				},
				{
					"Anniversario",
					ImportContactProperties.Anniversary
				},
				{
					"Telefono TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tipo posta elettronica 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nome visualizzato posta elettronica 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"CAP (uff.)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Altro telefono",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorità",
					ImportContactProperties.Priority
				},
				{
					"Iniziali",
					ImportContactProperties.Initials
				},
				{
					"Nome coniuge",
					ImportContactProperties.Spouse
				},
				{
					"Reparto",
					ImportContactProperties.Department
				},
				{
					"Abitazione",
					ImportContactProperties.HomePhone
				},
				{
					"Ufficio",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Cercapersone",
					ImportContactProperties.Pager
				},
				{
					"Altro CAP",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Provincia (uff.)",
					ImportContactProperties.BusinessState
				},
				{
					"Utente 2",
					ImportContactProperties.User2
				},
				{
					"Abitazione 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Cellulare",
					ImportContactProperties.MobilePhone
				},
				{
					"Titolo straniero",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Utente 3",
					ImportContactProperties.User3
				},
				{
					"Telefono principale società",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Figli",
					ImportContactProperties.Children
				},
				{
					"Fax (uff.)",
					ImportContactProperties.BusinessFax
				},
				{
					"CAP (ab.)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Luogo",
					ImportContactProperties.Location
				},
				{
					"Dati fatturazione",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax (ab.)",
					ImportContactProperties.HomeFax
				},
				{
					"Pagina Web",
					ImportContactProperties.WebPage
				},
				{
					"Via (ab.) 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Città (uff.)",
					ImportContactProperties.BusinessCity
				},
				{
					"Altra città",
					ImportContactProperties.OtherCity
				},
				{
					"Via (uff.)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tipo posta elettronica 3",
					ImportContactProperties.Email3Type
				},
				{
					"Nome assistente",
					ImportContactProperties.AssistantName
				},
				{
					"Sesso",
					ImportContactProperties.Gender
				},
				{
					"Telefono assistente",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nome visualizzato posta elettronica",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Indirizzo posta elettronica 3",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Utente 4",
					ImportContactProperties.User4
				},
				{
					"Nome manager",
					ImportContactProperties.ManagerName
				},
				{
					"Paese (uff.)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Richiamata automatica",
					ImportContactProperties.Callback
				},
				{
					"Telefono principale",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Indirizzo posta elettronica",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Ufficio 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Provincia (ab.)",
					ImportContactProperties.HomeState
				},
				{
					"Altro fax",
					ImportContactProperties.OtherFax
				},
				{
					"Via (uff.) 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Città (ab.)",
					ImportContactProperties.HomeCity
				},
				{
					"Altra provincia",
					ImportContactProperties.OtherState
				},
				{
					"Via (uff.) 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titolo",
					ImportContactProperties.Title
				},
				{
					"Via (ab.) 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Ubicazione ufficio",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Paese (ab.)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefono auto",
					ImportContactProperties.CarPhone
				},
				{
					"Società",
					ImportContactProperties.Company
				},
				{
					"Indirizzo (ab.) - Casella postale",
					ImportContactProperties.HomePOBox
				},
				{
					"Altro indirizzo - Casella postale",
					ImportContactProperties.OtherPOBox
				},
				{
					"Via (ab.)",
					ImportContactProperties.HomeStreet
				},
				{
					"Nome visualizzato posta elettronica 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Riservatezza",
					ImportContactProperties.Sensitivity
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"Nome",
					ImportContactProperties.FirstName
				},
				{
					"Altra via",
					ImportContactProperties.OtherStreet
				},
				{
					"Secondo nome",
					ImportContactProperties.MiddleName
				},
				{
					"Indennità trasferta",
					ImportContactProperties.Mileage
				},
				{
					"Utente 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1040));
		}

		private OutlookCsvLanguage CreateLanguageObject_ja_jp()
		{
			return new OutlookCsvLanguage(932, new Dictionary<string, ImportContactProperties>
			{
				{
					"電子メール 2 アドレス",
					ImportContactProperties.Email2Address
				},
				{
					"役職",
					ImportContactProperties.JobTitle
				},
				{
					"国 (その他)/地域",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"テレックス",
					ImportContactProperties.Telex
				},
				{
					"インターネット空き時間情報",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"住所 2 (その他)",
					ImportContactProperties.OtherStreet2
				},
				{
					"職業",
					ImportContactProperties.Profession
				},
				{
					"電子メールの種類",
					ImportContactProperties.EmailType
				},
				{
					"アカウント",
					ImportContactProperties.Account
				},
				{
					"分類",
					ImportContactProperties.Categories
				},
				{
					"誕生日",
					ImportContactProperties.Birthday
				},
				{
					"会社住所私書箱",
					ImportContactProperties.BusinessPOBox
				},
				{
					"住所 3 (その他)",
					ImportContactProperties.OtherStreet3
				},
				{
					"ID 番号",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"姓",
					ImportContactProperties.LastName
				},
				{
					"無線電話",
					ImportContactProperties.RadioPhone
				},
				{
					"記念日",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"電子メール 2 の種類",
					ImportContactProperties.Email2Type
				},
				{
					"電子メール 2 表示名",
					ImportContactProperties.Email2DisplayName
				},
				{
					"郵便番号 (会社)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"その他の電話",
					ImportContactProperties.OtherPhone
				},
				{
					"優先度",
					ImportContactProperties.Priority
				},
				{
					"イニシャル",
					ImportContactProperties.Initials
				},
				{
					"配偶者",
					ImportContactProperties.Spouse
				},
				{
					"部署",
					ImportContactProperties.Department
				},
				{
					"自宅電話",
					ImportContactProperties.HomePhone
				},
				{
					"会社電話",
					ImportContactProperties.BusinessPhone
				},
				{
					"会社名フリガナ",
					ImportContactProperties.CompanyYomi
				},
				{
					"ポケットベル",
					ImportContactProperties.Pager
				},
				{
					"郵便番号 (その他)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"都道府県 (会社)",
					ImportContactProperties.BusinessState
				},
				{
					"ユーザー 2",
					ImportContactProperties.User2
				},
				{
					"自宅電話 2",
					ImportContactProperties.HomePhone2
				},
				{
					"携帯電話",
					ImportContactProperties.MobilePhone
				},
				{
					"敬称",
					ImportContactProperties.Suffix
				},
				{
					"趣味",
					ImportContactProperties.Hobby
				},
				{
					"ユーザー 3",
					ImportContactProperties.User3
				},
				{
					"会社代表電話",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"子供",
					ImportContactProperties.Children
				},
				{
					"会社 FAX",
					ImportContactProperties.BusinessFax
				},
				{
					"郵便番号 (自宅)",
					ImportContactProperties.HomePostalCode
				},
				{
					"場所",
					ImportContactProperties.Location
				},
				{
					"支払い条件",
					ImportContactProperties.BillingInformation
				},
				{
					"自宅 FAX",
					ImportContactProperties.HomeFax
				},
				{
					"Web ページ",
					ImportContactProperties.WebPage
				},
				{
					"住所 2 (自宅)",
					ImportContactProperties.HomeStreet2
				},
				{
					"市町村 (会社)",
					ImportContactProperties.BusinessCity
				},
				{
					"市町村 (その他)",
					ImportContactProperties.OtherCity
				},
				{
					"番地 (会社)",
					ImportContactProperties.BusinessStreet
				},
				{
					"電子メール 3 の種類",
					ImportContactProperties.Email3Type
				},
				{
					"秘書の氏名",
					ImportContactProperties.AssistantName
				},
				{
					"性別",
					ImportContactProperties.Gender
				},
				{
					"秘書の電話",
					ImportContactProperties.AssistantPhone
				},
				{
					"電子メール表示名",
					ImportContactProperties.EmailDisplayName
				},
				{
					"電子メール 3 アドレス",
					ImportContactProperties.Email3Address
				},
				{
					"姓フリガナ",
					ImportContactProperties.SurnameYomi
				},
				{
					"ユーザー 4",
					ImportContactProperties.User4
				},
				{
					"マネージャー",
					ImportContactProperties.ManagerName
				},
				{
					"国 (会社)/地域",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"コールバック",
					ImportContactProperties.Callback
				},
				{
					"通常の電話",
					ImportContactProperties.PrimaryPhone
				},
				{
					"電子メール アドレス",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"会社電話 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"都道府県 (自宅)",
					ImportContactProperties.HomeState
				},
				{
					"その他の FAX",
					ImportContactProperties.OtherFax
				},
				{
					"住所 3 (会社)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"市町村 (自宅)",
					ImportContactProperties.HomeCity
				},
				{
					"都道府県 (その他)",
					ImportContactProperties.OtherState
				},
				{
					"住所 2 (会社)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"肩書き",
					ImportContactProperties.Title
				},
				{
					"住所 3 (自宅)",
					ImportContactProperties.HomeStreet3
				},
				{
					"事業所",
					ImportContactProperties.OfficeLocation
				},
				{
					"名前フリガナ",
					ImportContactProperties.GivenYomi
				},
				{
					"国 (自宅)/地域",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"自動車電話",
					ImportContactProperties.CarPhone
				},
				{
					"会社名",
					ImportContactProperties.Company
				},
				{
					"自宅住所私書箱",
					ImportContactProperties.HomePOBox
				},
				{
					"その他住所私書箱",
					ImportContactProperties.OtherPOBox
				},
				{
					"番地 (自宅)",
					ImportContactProperties.HomeStreet
				},
				{
					"電子メール 3 表示名",
					ImportContactProperties.Email3DisplayName
				},
				{
					"秘密度",
					ImportContactProperties.Sensitivity
				},
				{
					"メモ",
					ImportContactProperties.Notes
				},
				{
					"名",
					ImportContactProperties.FirstName
				},
				{
					"番地 (その他)",
					ImportContactProperties.OtherStreet
				},
				{
					"ミドル ネーム",
					ImportContactProperties.MiddleName
				},
				{
					"経費情報",
					ImportContactProperties.Mileage
				},
				{
					"ユーザー 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1041));
		}

		private OutlookCsvLanguage CreateLanguageObject_ko_kr()
		{
			return new OutlookCsvLanguage(949, new Dictionary<string, ImportContactProperties>
			{
				{
					"전자 메일 2 주소",
					ImportContactProperties.Email2Address
				},
				{
					"직함",
					ImportContactProperties.JobTitle
				},
				{
					"기타 국가/지역",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"텔렉스",
					ImportContactProperties.Telex
				},
				{
					"인터넷 약속 있음/약속 없음",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"기타 번지 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"직업",
					ImportContactProperties.Profession
				},
				{
					"전자 메일 유형",
					ImportContactProperties.EmailType
				},
				{
					"계정",
					ImportContactProperties.Account
				},
				{
					"범주 항목",
					ImportContactProperties.Categories
				},
				{
					"생일",
					ImportContactProperties.Birthday
				},
				{
					"근무처 주소 사서함",
					ImportContactProperties.BusinessPOBox
				},
				{
					"기타 번지 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"주민 등록 번호",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"성",
					ImportContactProperties.LastName
				},
				{
					"무선 전화",
					ImportContactProperties.RadioPhone
				},
				{
					"결혼 기념일",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD 전화",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"전자 메일 2 유형",
					ImportContactProperties.Email2Type
				},
				{
					"전자 메일 2 표시 이름",
					ImportContactProperties.Email2DisplayName
				},
				{
					"근무지 우편 번호",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"기타 전화",
					ImportContactProperties.OtherPhone
				},
				{
					"우선 순위",
					ImportContactProperties.Priority
				},
				{
					"머리글자",
					ImportContactProperties.Initials
				},
				{
					"배우자",
					ImportContactProperties.Spouse
				},
				{
					"부서",
					ImportContactProperties.Department
				},
				{
					"집 전화 번호",
					ImportContactProperties.HomePhone
				},
				{
					"근무처 전화",
					ImportContactProperties.BusinessPhone
				},
				{
					"회사(일본어 요미)",
					ImportContactProperties.CompanyYomi
				},
				{
					"호출기",
					ImportContactProperties.Pager
				},
				{
					"기타 우편 번호",
					ImportContactProperties.OtherPostalCode
				},
				{
					"근무지 시/도",
					ImportContactProperties.BusinessState
				},
				{
					"사용자 2",
					ImportContactProperties.User2
				},
				{
					"집 전화 2",
					ImportContactProperties.HomePhone2
				},
				{
					"휴대폰",
					ImportContactProperties.MobilePhone
				},
				{
					"호칭(한글)",
					ImportContactProperties.Suffix
				},
				{
					"취미",
					ImportContactProperties.Hobby
				},
				{
					"사용자 3",
					ImportContactProperties.User3
				},
				{
					"회사 대표 전화",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"자녀",
					ImportContactProperties.Children
				},
				{
					"근무지 팩스",
					ImportContactProperties.BusinessFax
				},
				{
					"집 주소 우편 번호",
					ImportContactProperties.HomePostalCode
				},
				{
					"국가",
					ImportContactProperties.Location
				},
				{
					"비용 정보",
					ImportContactProperties.BillingInformation
				},
				{
					"집 팩스",
					ImportContactProperties.HomeFax
				},
				{
					"웹 페이지",
					ImportContactProperties.WebPage
				},
				{
					"집 번지 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"근무지 구/군/시",
					ImportContactProperties.BusinessCity
				},
				{
					"기타 구/군/시",
					ImportContactProperties.OtherCity
				},
				{
					"근무지 주소 번지",
					ImportContactProperties.BusinessStreet
				},
				{
					"전자 메일 3 유형",
					ImportContactProperties.Email3Type
				},
				{
					"비서 이름",
					ImportContactProperties.AssistantName
				},
				{
					"성별",
					ImportContactProperties.Gender
				},
				{
					"비서 전화 번호",
					ImportContactProperties.AssistantPhone
				},
				{
					"전자 메일 표시 이름",
					ImportContactProperties.EmailDisplayName
				},
				{
					"전자 메일 3 주소",
					ImportContactProperties.Email3Address
				},
				{
					"성(일본어 요미)",
					ImportContactProperties.SurnameYomi
				},
				{
					"사용자 4",
					ImportContactProperties.User4
				},
				{
					"관리자 이름",
					ImportContactProperties.ManagerName
				},
				{
					"근무지 국가/지역",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"다시 걸 전화",
					ImportContactProperties.Callback
				},
				{
					"기본 전화",
					ImportContactProperties.PrimaryPhone
				},
				{
					"전자 메일 주소",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"근무처 전화 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"집 주소 시/도",
					ImportContactProperties.HomeState
				},
				{
					"기타 팩스",
					ImportContactProperties.OtherFax
				},
				{
					"근무지 번지 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"집 주소 구/군/시",
					ImportContactProperties.HomeCity
				},
				{
					"기타 시/도",
					ImportContactProperties.OtherState
				},
				{
					"근무지 번지 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"호칭(영문)",
					ImportContactProperties.Title
				},
				{
					"집 번지 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"사무실 위치",
					ImportContactProperties.OfficeLocation
				},
				{
					"이름(일본어 요미)",
					ImportContactProperties.GivenYomi
				},
				{
					"집 주소 국가/지역",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"카폰",
					ImportContactProperties.CarPhone
				},
				{
					"회사",
					ImportContactProperties.Company
				},
				{
					"집 주소 사서함",
					ImportContactProperties.HomePOBox
				},
				{
					"기타 주소 사서함",
					ImportContactProperties.OtherPOBox
				},
				{
					"집 번지",
					ImportContactProperties.HomeStreet
				},
				{
					"전자 메일 3 표시 이름",
					ImportContactProperties.Email3DisplayName
				},
				{
					"우편물 종류",
					ImportContactProperties.Sensitivity
				},
				{
					"메모",
					ImportContactProperties.Notes
				},
				{
					"이름",
					ImportContactProperties.FirstName
				},
				{
					"기타 번지",
					ImportContactProperties.OtherStreet
				},
				{
					"중간 이름",
					ImportContactProperties.MiddleName
				},
				{
					"거리",
					ImportContactProperties.Mileage
				},
				{
					"사용자 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1042));
		}

		private OutlookCsvLanguage CreateLanguageObject_lt_lt()
		{
			return new OutlookCsvLanguage(1257, new Dictionary<string, ImportContactProperties>
			{
				{
					"2-as el. pašto adresas",
					ImportContactProperties.Email2Address
				},
				{
					"Pareigos",
					ImportContactProperties.JobTitle
				},
				{
					"Kita šalis/regionas",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleksas",
					ImportContactProperties.Telex
				},
				{
					"Interneto užimtumo tarnyba",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"2-a gatvė",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesija",
					ImportContactProperties.Profession
				},
				{
					"El. pašto tipas",
					ImportContactProperties.EmailType
				},
				{
					"Abonentas",
					ImportContactProperties.Account
				},
				{
					"Kategorijos",
					ImportContactProperties.Categories
				},
				{
					"Gimtadienis",
					ImportContactProperties.Birthday
				},
				{
					"Darbo adreso a/d",
					ImportContactProperties.BusinessPOBox
				},
				{
					"3-ia gatvė",
					ImportContactProperties.OtherStreet3
				},
				{
					"Asmens kodas",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Pavardė",
					ImportContactProperties.LastName
				},
				{
					"Radijo telefonas",
					ImportContactProperties.RadioPhone
				},
				{
					"Sukaktis",
					ImportContactProperties.Anniversary
				},
				{
					"Teletaipas/Įrenginys kurtiesiems",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"2-o el. pašto tipas",
					ImportContactProperties.Email2Type
				},
				{
					"Rodomas 2-o el. pašto vardas",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Pašto indeksas (darbo)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Kitas telefonas",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritetas",
					ImportContactProperties.Priority
				},
				{
					"Inicialai",
					ImportContactProperties.Initials
				},
				{
					"Sutuoktinis",
					ImportContactProperties.Spouse
				},
				{
					"Skyrius",
					ImportContactProperties.Department
				},
				{
					"Namų telefonas",
					ImportContactProperties.HomePhone
				},
				{
					"Darbo telefonas",
					ImportContactProperties.BusinessPhone
				},
				{
					"Yomi įmonė",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pranešimų gaviklis",
					ImportContactProperties.Pager
				},
				{
					"Kitas pašto indeksas",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Rajonas, apskritis (darbo)",
					ImportContactProperties.BusinessState
				},
				{
					"2-as vartotojas",
					ImportContactProperties.User2
				},
				{
					"2-as namų telefonas",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobilusis telefonas",
					ImportContactProperties.MobilePhone
				},
				{
					"Priedas",
					ImportContactProperties.Suffix
				},
				{
					"Pomėgiai",
					ImportContactProperties.Hobby
				},
				{
					"3-ias vartotojas",
					ImportContactProperties.User3
				},
				{
					"Pagrindinis įmonės telefonas",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Vaikai",
					ImportContactProperties.Children
				},
				{
					"Darbo faksograma",
					ImportContactProperties.BusinessFax
				},
				{
					"Namų pašto indeksas",
					ImportContactProperties.HomePostalCode
				},
				{
					"Vieta",
					ImportContactProperties.Location
				},
				{
					"Atsiskaitymo informacija",
					ImportContactProperties.BillingInformation
				},
				{
					"Namų faksas",
					ImportContactProperties.HomeFax
				},
				{
					"Tinklalapis",
					ImportContactProperties.WebPage
				},
				{
					"2-a gyvenamoji gatvė",
					ImportContactProperties.HomeStreet2
				},
				{
					"Miestas (darbo)",
					ImportContactProperties.BusinessCity
				},
				{
					"Kitas miestas",
					ImportContactProperties.OtherCity
				},
				{
					"Gatvė (darbo)",
					ImportContactProperties.BusinessStreet
				},
				{
					"3-io el. pašto tipas",
					ImportContactProperties.Email3Type
				},
				{
					"Padėjėjo vardas, pavardė",
					ImportContactProperties.AssistantName
				},
				{
					"Lytis",
					ImportContactProperties.Gender
				},
				{
					"Padėjėjo telefonas",
					ImportContactProperties.AssistantPhone
				},
				{
					"Rodomas el. pašto vardas",
					ImportContactProperties.EmailDisplayName
				},
				{
					"3-ias el. pašto adresas",
					ImportContactProperties.Email3Address
				},
				{
					"Yomi pavardė",
					ImportContactProperties.SurnameYomi
				},
				{
					"4-as vartotojas",
					ImportContactProperties.User4
				},
				{
					"Vadybininko vardas, pavardė",
					ImportContactProperties.ManagerName
				},
				{
					"Darbo šalis/regionas",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Atgalinis skambinimas",
					ImportContactProperties.Callback
				},
				{
					"Pagrindinis telefonas",
					ImportContactProperties.PrimaryPhone
				},
				{
					"El. pašto adresas",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"2-as darbo telefonas",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Gyvenamasis rajonas, apskritis",
					ImportContactProperties.HomeState
				},
				{
					"Kitas faksas",
					ImportContactProperties.OtherFax
				},
				{
					"3-ia gatvė (darbo)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Gyvenamasis miestas",
					ImportContactProperties.HomeCity
				},
				{
					"Kitas rajonas, apskritis",
					ImportContactProperties.OtherState
				},
				{
					"2-a gatvė (darbo)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titulas",
					ImportContactProperties.Title
				},
				{
					"3-ia gyvenamoji gatvė",
					ImportContactProperties.HomeStreet3
				},
				{
					"Įmonės vieta",
					ImportContactProperties.OfficeLocation
				},
				{
					"Yomi vardas",
					ImportContactProperties.GivenYomi
				},
				{
					"Gyvenamoji šalis/regionas",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Automobilinis telefonas",
					ImportContactProperties.CarPhone
				},
				{
					"Įmonė",
					ImportContactProperties.Company
				},
				{
					"Namų adreso a/d",
					ImportContactProperties.HomePOBox
				},
				{
					"Kito adreso a/d",
					ImportContactProperties.OtherPOBox
				},
				{
					"Gyvenamoji gatvė",
					ImportContactProperties.HomeStreet
				},
				{
					"Rodomas 3-io el. pašto vardas",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Slaptumas",
					ImportContactProperties.Sensitivity
				},
				{
					"Pastabos",
					ImportContactProperties.Notes
				},
				{
					"Vardas",
					ImportContactProperties.FirstName
				},
				{
					"Kita gatvė",
					ImportContactProperties.OtherStreet
				},
				{
					"Antras vardas",
					ImportContactProperties.MiddleName
				},
				{
					"Atstumas",
					ImportContactProperties.Mileage
				},
				{
					"1-as vartotojas",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1063));
		}

		private OutlookCsvLanguage CreateLanguageObject_lv_lv()
		{
			return new OutlookCsvLanguage(1257, new Dictionary<string, ImportContactProperties>
			{
				{
					"2. e-pasta adrese",
					ImportContactProperties.Email2Address
				},
				{
					"Amats",
					ImportContactProperties.JobTitle
				},
				{
					"Cita valsts/reģions",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telekss",
					ImportContactProperties.Telex
				},
				{
					"Interneta brīvs/aizņemts",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Citas ielas 2. rindiņa",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesija",
					ImportContactProperties.Profession
				},
				{
					"E-pasta tips",
					ImportContactProperties.EmailType
				},
				{
					"Konts",
					ImportContactProperties.Account
				},
				{
					"Kategorijas",
					ImportContactProperties.Categories
				},
				{
					"Dzimšanas diena",
					ImportContactProperties.Birthday
				},
				{
					"Darba adreses pastkastīte",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Citas ielas 3. rindiņa",
					ImportContactProperties.OtherStreet3
				},
				{
					"Personas kods",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Uzvārds",
					ImportContactProperties.LastName
				},
				{
					"Radiotālrunis",
					ImportContactProperties.RadioPhone
				},
				{
					"Jubileja",
					ImportContactProperties.Anniversary
				},
				{
					"Teletaips/surdotālrunis",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"2. e-pasta tips",
					ImportContactProperties.Email2Type
				},
				{
					"2. e-pasta parādāmais vārds",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Darbavietas pasta indekss",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Cits tālrunis",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritāte",
					ImportContactProperties.Priority
				},
				{
					"Iniciāļi",
					ImportContactProperties.Initials
				},
				{
					"Dzīvesbiedrs",
					ImportContactProperties.Spouse
				},
				{
					"Nodaļa",
					ImportContactProperties.Department
				},
				{
					"Tālrunis mājās",
					ImportContactProperties.HomePhone
				},
				{
					"Tālrunis darbā",
					ImportContactProperties.BusinessPhone
				},
				{
					"Uzņēmuma Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Peidžeris",
					ImportContactProperties.Pager
				},
				{
					"Cits pasta indekss",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Darbavietas rajons",
					ImportContactProperties.BusinessState
				},
				{
					"2. lietotājs",
					ImportContactProperties.User2
				},
				{
					"2. tālrunis mājās",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobilais tālrunis",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufikss",
					ImportContactProperties.Suffix
				},
				{
					"Hobijs",
					ImportContactProperties.Hobby
				},
				{
					"3. lietotājs",
					ImportContactProperties.User3
				},
				{
					"Uzņēmuma galvenais tālrunis",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Bērni",
					ImportContactProperties.Children
				},
				{
					"Fakss darbā",
					ImportContactProperties.BusinessFax
				},
				{
					"Dzīvesvietas pasta indekss",
					ImportContactProperties.HomePostalCode
				},
				{
					"Atrašanās vieta",
					ImportContactProperties.Location
				},
				{
					"Rēķina informācija",
					ImportContactProperties.BillingInformation
				},
				{
					"Fakss mājās",
					ImportContactProperties.HomeFax
				},
				{
					"Tīmekļa lapa",
					ImportContactProperties.WebPage
				},
				{
					"Dzīvesvietas ielas 2. rindiņa",
					ImportContactProperties.HomeStreet2
				},
				{
					"Darbavietas pilsēta",
					ImportContactProperties.BusinessCity
				},
				{
					"Cita pilsēta",
					ImportContactProperties.OtherCity
				},
				{
					"Darbavietas iela",
					ImportContactProperties.BusinessStreet
				},
				{
					"3. e-pasta tips",
					ImportContactProperties.Email3Type
				},
				{
					"Asistenta vārds",
					ImportContactProperties.AssistantName
				},
				{
					"Dzimums",
					ImportContactProperties.Gender
				},
				{
					"Asistenta tālrunis",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-pasta parādāmais vārds",
					ImportContactProperties.EmailDisplayName
				},
				{
					"3. e-pasta adrese",
					ImportContactProperties.Email3Address
				},
				{
					"Uzvārda Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"4. lietotājs",
					ImportContactProperties.User4
				},
				{
					"Vadītāja vārds",
					ImportContactProperties.ManagerName
				},
				{
					"Darbavietas valsts/reģions",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Atzvanīšana",
					ImportContactProperties.Callback
				},
				{
					"Primārais tālrunis",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-pasta adrese",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"2. tālrunis darbā",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Dzīvesvietas rajons",
					ImportContactProperties.HomeState
				},
				{
					"Cits fakss",
					ImportContactProperties.OtherFax
				},
				{
					"Darbavietas ielas 3. rindiņa",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Dzīvesvietas pilsēta",
					ImportContactProperties.HomeCity
				},
				{
					"Cits rajons",
					ImportContactProperties.OtherState
				},
				{
					"Darbavietas ielas 2. rindiņa",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Dzīvesvietas ielas 3. rindiņa",
					ImportContactProperties.HomeStreet3
				},
				{
					"Biroja atrašanās vieta",
					ImportContactProperties.OfficeLocation
				},
				{
					"Vārda Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Dzīvesvietas valsts/reģions",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Tālrunis automašīnā",
					ImportContactProperties.CarPhone
				},
				{
					"Uzņēmums",
					ImportContactProperties.Company
				},
				{
					"Mājas adreses pastkastīte",
					ImportContactProperties.HomePOBox
				},
				{
					"Citas adreses pastkastīte",
					ImportContactProperties.OtherPOBox
				},
				{
					"Dzīvesvietas iela",
					ImportContactProperties.HomeStreet
				},
				{
					"3. e-pasta parādāmais vārds",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Jutība",
					ImportContactProperties.Sensitivity
				},
				{
					"Piezīmes",
					ImportContactProperties.Notes
				},
				{
					"Vārds",
					ImportContactProperties.FirstName
				},
				{
					"Cita iela",
					ImportContactProperties.OtherStreet
				},
				{
					"Otrais vārds",
					ImportContactProperties.MiddleName
				},
				{
					"Attālums",
					ImportContactProperties.Mileage
				},
				{
					"1. lietotājs",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1062));
		}

		private OutlookCsvLanguage CreateLanguageObject_nl_nl()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-mailadres 2",
					ImportContactProperties.Email2Address
				},
				{
					"Functie",
					ImportContactProperties.JobTitle
				},
				{
					"Land/regio (anders)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Internet Beschikbaarheidsinfo",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Ander adres, straat 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Beroep",
					ImportContactProperties.Profession
				},
				{
					"E-mailtype",
					ImportContactProperties.EmailType
				},
				{
					"Account",
					ImportContactProperties.Account
				},
				{
					"Categorieën",
					ImportContactProperties.Categories
				},
				{
					"Verjaardag",
					ImportContactProperties.Birthday
				},
				{
					"Werkadres, postbusnummer",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Ander adres, straat 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Sofinummer",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Achternaam",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefoon",
					ImportContactProperties.RadioPhone
				},
				{
					"Speciale datum",
					ImportContactProperties.Anniversary
				},
				{
					"Teksttelefoon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-mailtype 2",
					ImportContactProperties.Email2Type
				},
				{
					"E-mail, weergegeven naam 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Werkadres, postcode",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Andere telefoon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioriteit",
					ImportContactProperties.Priority
				},
				{
					"Initialen",
					ImportContactProperties.Initials
				},
				{
					"Partner",
					ImportContactProperties.Spouse
				},
				{
					"Afdeling",
					ImportContactProperties.Department
				},
				{
					"Telefoon thuis",
					ImportContactProperties.HomePhone
				},
				{
					"Telefoon op werk",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Ander adres, postcode",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Werkadres, provincie",
					ImportContactProperties.BusinessState
				},
				{
					"Gebruiker 2",
					ImportContactProperties.User2
				},
				{
					"Telefoon thuis 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiele telefoon",
					ImportContactProperties.MobilePhone
				},
				{
					"Achtervoegsel",
					ImportContactProperties.Suffix
				},
				{
					"Hobby's",
					ImportContactProperties.Hobby
				},
				{
					"Gebruiker 3",
					ImportContactProperties.User3
				},
				{
					"Hoofdtelefoon bedrijf",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Kinderen",
					ImportContactProperties.Children
				},
				{
					"Fax op werk",
					ImportContactProperties.BusinessFax
				},
				{
					"Huisadres, postcode",
					ImportContactProperties.HomePostalCode
				},
				{
					"Locatie",
					ImportContactProperties.Location
				},
				{
					"Factuurinformatie",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax thuis",
					ImportContactProperties.HomeFax
				},
				{
					"Webpagina",
					ImportContactProperties.WebPage
				},
				{
					"Huisadres, straat 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Werkadres, plaats",
					ImportContactProperties.BusinessCity
				},
				{
					"Ander adres, plaats",
					ImportContactProperties.OtherCity
				},
				{
					"Werkadres, straat",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-mailtype 3",
					ImportContactProperties.Email3Type
				},
				{
					"Naam assistent",
					ImportContactProperties.AssistantName
				},
				{
					"Geslacht",
					ImportContactProperties.Gender
				},
				{
					"Telefoon assistent",
					ImportContactProperties.AssistantPhone
				},
				{
					"Weergavenaam voor e-mail",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mailadres 3",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Gebruiker 4",
					ImportContactProperties.User4
				},
				{
					"Naam manager",
					ImportContactProperties.ManagerName
				},
				{
					"Land/regio (werk)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Terugbellen",
					ImportContactProperties.Callback
				},
				{
					"Hoofdtelefoon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mailadres",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefoon op werk 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Huisadres, provincie",
					ImportContactProperties.HomeState
				},
				{
					"Andere fax",
					ImportContactProperties.OtherFax
				},
				{
					"Werkadres 3, straat",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Huisadres, plaats",
					ImportContactProperties.HomeCity
				},
				{
					"Ander adres, provincie",
					ImportContactProperties.OtherState
				},
				{
					"Werkadres 2, straat",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titel",
					ImportContactProperties.Title
				},
				{
					"Huisadres, straat 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Kantoorlocatie",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Land/regio (thuis)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autotelefoon",
					ImportContactProperties.CarPhone
				},
				{
					"Bedrijf",
					ImportContactProperties.Company
				},
				{
					"Huisadres, postbusnummer",
					ImportContactProperties.HomePOBox
				},
				{
					"Ander adres, postbusnummer",
					ImportContactProperties.OtherPOBox
				},
				{
					"Huisadres, straat",
					ImportContactProperties.HomeStreet
				},
				{
					"E-mail, weergegeven naam 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Gevoeligheid",
					ImportContactProperties.Sensitivity
				},
				{
					"Notities",
					ImportContactProperties.Notes
				},
				{
					"Voornaam",
					ImportContactProperties.FirstName
				},
				{
					"Ander adres, straat",
					ImportContactProperties.OtherStreet
				},
				{
					"Middelste naam",
					ImportContactProperties.MiddleName
				},
				{
					"Reisafstand",
					ImportContactProperties.Mileage
				},
				{
					"Gebruiker 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1043));
		}

		private OutlookCsvLanguage CreateLanguageObject_nb_no()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-postadresse 2",
					ImportContactProperties.Email2Address
				},
				{
					"Stilling",
					ImportContactProperties.JobTitle
				},
				{
					"Annet land/område",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Ledige og opptatte tidspunkt på Internett",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Annen adresse 2: Gate/vei",
					ImportContactProperties.OtherStreet2
				},
				{
					"Yrke",
					ImportContactProperties.Profession
				},
				{
					"E-posttype",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategorier",
					ImportContactProperties.Categories
				},
				{
					"Fødselsdag",
					ImportContactProperties.Birthday
				},
				{
					"Adresse, arb.: Postboks",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Annen adresse 3: Gate/vei",
					ImportContactProperties.OtherStreet3
				},
				{
					"Offentlig ID-nummer",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Etternavn",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Merkedag",
					ImportContactProperties.Anniversary
				},
				{
					"Telefon for hørselshemmede",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-posttype 2",
					ImportContactProperties.Email2Type
				},
				{
					"Visningsnavn for e-post 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Adresse, arb.: Postnummer",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Annen telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritet",
					ImportContactProperties.Priority
				},
				{
					"Initialer",
					ImportContactProperties.Initials
				},
				{
					"Ektefelle",
					ImportContactProperties.Spouse
				},
				{
					"Avdeling",
					ImportContactProperties.Department
				},
				{
					"Telefon, priv.",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon, arb.",
					ImportContactProperties.BusinessPhone
				},
				{
					"Firma-Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Personsøker",
					ImportContactProperties.Pager
				},
				{
					"Annen adresse: Postnummer",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Adresse, arb.: Område",
					ImportContactProperties.BusinessState
				},
				{
					"Bruker 2",
					ImportContactProperties.User2
				},
				{
					"Telefon 2, priv.",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Suffiks",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Bruker 3",
					ImportContactProperties.User3
				},
				{
					"Hovedtelefon for firma",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Barn",
					ImportContactProperties.Children
				},
				{
					"Telefaks, arb.",
					ImportContactProperties.BusinessFax
				},
				{
					"Adresse, priv.: Postnummer",
					ImportContactProperties.HomePostalCode
				},
				{
					"Plassering",
					ImportContactProperties.Location
				},
				{
					"Faktureringsinformasjon",
					ImportContactProperties.BillingInformation
				},
				{
					"Telefaks, priv.",
					ImportContactProperties.HomeFax
				},
				{
					"Webside",
					ImportContactProperties.WebPage
				},
				{
					"Adresse 2, priv.: Gate/vei",
					ImportContactProperties.HomeStreet2
				},
				{
					"Adresse, arb.: Poststed",
					ImportContactProperties.BusinessCity
				},
				{
					"Annen adresse: Poststed",
					ImportContactProperties.OtherCity
				},
				{
					"Adresse, arb.: Gate/vei",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-posttype 3",
					ImportContactProperties.Email3Type
				},
				{
					"Assistentens navn",
					ImportContactProperties.AssistantName
				},
				{
					"Kjønn",
					ImportContactProperties.Gender
				},
				{
					"Assistentens telefonnummer",
					ImportContactProperties.AssistantPhone
				},
				{
					"Visningsnavn for e-post",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-postadresse 3",
					ImportContactProperties.Email3Address
				},
				{
					"Etternavn-Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Bruker 4",
					ImportContactProperties.User4
				},
				{
					"Overordnedes navn",
					ImportContactProperties.ManagerName
				},
				{
					"Firmaland/område",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Tilbakeringing",
					ImportContactProperties.Callback
				},
				{
					"Hovedtelefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-postadresse",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon 2, arb.",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Adresse, priv.: Område",
					ImportContactProperties.HomeState
				},
				{
					"Annen telefaks",
					ImportContactProperties.OtherFax
				},
				{
					"Adresse 3, arb.: Gate/vei",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Adresse, priv.: Poststed",
					ImportContactProperties.HomeCity
				},
				{
					"Annen adresse: Område",
					ImportContactProperties.OtherState
				},
				{
					"Adresse 2, arb.: Gate/vei",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Tittel",
					ImportContactProperties.Title
				},
				{
					"Adresse 3, priv.: Gate/vei",
					ImportContactProperties.HomeStreet3
				},
				{
					"Kontor",
					ImportContactProperties.OfficeLocation
				},
				{
					"Angitt Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Hjemland/område",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Biltelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Firma",
					ImportContactProperties.Company
				},
				{
					"Adresse, priv.: Postboks",
					ImportContactProperties.HomePOBox
				},
				{
					"Annen postboksadresse",
					ImportContactProperties.OtherPOBox
				},
				{
					"Adresse, priv.: Gate/vei",
					ImportContactProperties.HomeStreet
				},
				{
					"Visningsnavn for e-post 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Følsomhet",
					ImportContactProperties.Sensitivity
				},
				{
					"Notater",
					ImportContactProperties.Notes
				},
				{
					"Fornavn",
					ImportContactProperties.FirstName
				},
				{
					"Annen adresse: Gate/vei",
					ImportContactProperties.OtherStreet
				},
				{
					"Mellomnavn",
					ImportContactProperties.MiddleName
				},
				{
					"Reisegodtgjørelse",
					ImportContactProperties.Mileage
				},
				{
					"Bruker 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1044));
		}

		private OutlookCsvLanguage CreateLanguageObject_pl_pl()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"Adres e-mail 2",
					ImportContactProperties.Email2Address
				},
				{
					"Stanowisko",
					ImportContactProperties.JobTitle
				},
				{
					"Inny adres - kraj/region",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Internetowe informacje wolny/zajęty",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Inny adres - ulica 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Zawód",
					ImportContactProperties.Profession
				},
				{
					"Typ poczty e-mail",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategorie",
					ImportContactProperties.Categories
				},
				{
					"Urodziny",
					ImportContactProperties.Birthday
				},
				{
					"Służbowa skrzynka pocztowa",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Inny adres - ulica 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"PESEL",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Nazwisko",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Rocznica",
					ImportContactProperties.Anniversary
				},
				{
					"Telefon TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Rodzaj e-mail 2",
					ImportContactProperties.Email2Type
				},
				{
					"Wyświetlana nazwa e-mail 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Adres służbowy - kod pocztowy",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Inny telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorytet",
					ImportContactProperties.Priority
				},
				{
					"Inicjały",
					ImportContactProperties.Initials
				},
				{
					"Współmałżonek",
					ImportContactProperties.Spouse
				},
				{
					"Dział",
					ImportContactProperties.Department
				},
				{
					"Telefon domowy",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon służbowy",
					ImportContactProperties.BusinessPhone
				},
				{
					"Firma Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Inny adres - kod pocztowy",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Adres służbowy - województwo",
					ImportContactProperties.BusinessState
				},
				{
					"Użytkownik 2",
					ImportContactProperties.User2
				},
				{
					"Telefon domowy 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Telefon komórkowy",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufiks",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Użytkownik 3",
					ImportContactProperties.User3
				},
				{
					"Główny telefon do firmy",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Dzieci",
					ImportContactProperties.Children
				},
				{
					"Faks służbowy",
					ImportContactProperties.BusinessFax
				},
				{
					"Adres domowy - kod pocztowy",
					ImportContactProperties.HomePostalCode
				},
				{
					"Lokalizacja",
					ImportContactProperties.Location
				},
				{
					"Informacje rozliczeniowe",
					ImportContactProperties.BillingInformation
				},
				{
					"Faks domowy",
					ImportContactProperties.HomeFax
				},
				{
					"Strona sieci Web",
					ImportContactProperties.WebPage
				},
				{
					"Adres domowy - ulica (2)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Adres służbowy - miejscowość",
					ImportContactProperties.BusinessCity
				},
				{
					"Inny adres - miejscowość",
					ImportContactProperties.OtherCity
				},
				{
					"Adres służbowy - ulica",
					ImportContactProperties.BusinessStreet
				},
				{
					"Rodzaj e-mail 3",
					ImportContactProperties.Email3Type
				},
				{
					"Imię i nazwisko asystenta",
					ImportContactProperties.AssistantName
				},
				{
					"Płeć",
					ImportContactProperties.Gender
				},
				{
					"Telefon asystenta",
					ImportContactProperties.AssistantPhone
				},
				{
					"Wyświetlana nazwa e-mail",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Adres e-mail 3",
					ImportContactProperties.Email3Address
				},
				{
					"Nazwisko Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Użytkownik 4",
					ImportContactProperties.User4
				},
				{
					"Menedżer",
					ImportContactProperties.ManagerName
				},
				{
					"Adres służbowy - kraj/region",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Wywołanie zwrotne",
					ImportContactProperties.Callback
				},
				{
					"Telefon podstawowy",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Adres e-mail",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon służbowy 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Adres domowy - województwo",
					ImportContactProperties.HomeState
				},
				{
					"Inny faks",
					ImportContactProperties.OtherFax
				},
				{
					"Adres służbowy - ulica 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Adres domowy - miejscowość",
					ImportContactProperties.HomeCity
				},
				{
					"Inny adres - województwo",
					ImportContactProperties.OtherState
				},
				{
					"Adres służbowy - ulica 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Tytuł",
					ImportContactProperties.Title
				},
				{
					"Adres domowy - ulica (3)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Lokalizacja biura",
					ImportContactProperties.OfficeLocation
				},
				{
					"Imię Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Adres domowy - kraj/region",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefon w samochodzie",
					ImportContactProperties.CarPhone
				},
				{
					"Firma",
					ImportContactProperties.Company
				},
				{
					"Domowa skrzynka pocztowa",
					ImportContactProperties.HomePOBox
				},
				{
					"Inna skrzynka pocztowa",
					ImportContactProperties.OtherPOBox
				},
				{
					"Adres domowy - ulica",
					ImportContactProperties.HomeStreet
				},
				{
					"Wyświetlana nazwa e-mail 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Charakter",
					ImportContactProperties.Sensitivity
				},
				{
					"Notatki",
					ImportContactProperties.Notes
				},
				{
					"Imię",
					ImportContactProperties.FirstName
				},
				{
					"Inny adres - ulica",
					ImportContactProperties.OtherStreet
				},
				{
					"Drugie imię",
					ImportContactProperties.MiddleName
				},
				{
					"Przebieg",
					ImportContactProperties.Mileage
				},
				{
					"Użytkownik 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1045));
		}

		private OutlookCsvLanguage CreateLanguageObject_pt_br()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Endereço de email 2",
					ImportContactProperties.Email2Address
				},
				{
					"Cargo",
					ImportContactProperties.JobTitle
				},
				{
					"Outro País/Região",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Disponibilidade da Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Outro endereço 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profissão",
					ImportContactProperties.Profession
				},
				{
					"Tipo de email",
					ImportContactProperties.EmailType
				},
				{
					"Conta",
					ImportContactProperties.Account
				},
				{
					"Categorias",
					ImportContactProperties.Categories
				},
				{
					"Birthday",
					ImportContactProperties.Birthday
				},
				{
					"Caixa postal do endereço comercial",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Outro endereço 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Código do governo",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Sobrenome",
					ImportContactProperties.LastName
				},
				{
					"Radiofone",
					ImportContactProperties.RadioPhone
				},
				{
					"Datas especiais",
					ImportContactProperties.Anniversary
				},
				{
					"Telefone TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tipo de email 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nome para exibição do email 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Business Postal Code",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Outro telefone",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioridade",
					ImportContactProperties.Priority
				},
				{
					"Iniciais",
					ImportContactProperties.Initials
				},
				{
					"Spouse",
					ImportContactProperties.Spouse
				},
				{
					"Departamento",
					ImportContactProperties.Department
				},
				{
					"Telefone residencial",
					ImportContactProperties.HomePhone
				},
				{
					"Telefone comercial",
					ImportContactProperties.BusinessPhone
				},
				{
					"Yomi (empresa)",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"CEP",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Business State",
					ImportContactProperties.BusinessState
				},
				{
					"Personalizado 2",
					ImportContactProperties.User2
				},
				{
					"Telefone residencial 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Telefone celular",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufixo",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Personalizado 3",
					ImportContactProperties.User3
				},
				{
					"Telefone principal da empresa",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Filhos",
					ImportContactProperties.Children
				},
				{
					"Fax comercial",
					ImportContactProperties.BusinessFax
				},
				{
					"CEP do endereço residencial",
					ImportContactProperties.HomePostalCode
				},
				{
					"Local",
					ImportContactProperties.Location
				},
				{
					"Informações para cobrança",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax residencial",
					ImportContactProperties.HomeFax
				},
				{
					"Página da Web",
					ImportContactProperties.WebPage
				},
				{
					"Endereço residencial 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Business City",
					ImportContactProperties.BusinessCity
				},
				{
					"Cidade",
					ImportContactProperties.OtherCity
				},
				{
					"Business Street",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tipo de email 3",
					ImportContactProperties.Email3Type
				},
				{
					"Nome do assistente",
					ImportContactProperties.AssistantName
				},
				{
					"Sexo",
					ImportContactProperties.Gender
				},
				{
					"Telefone do assistente",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nome para exibição do email",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Endereço de email 3",
					ImportContactProperties.Email3Address
				},
				{
					"Yomi (sobrenome)",
					ImportContactProperties.SurnameYomi
				},
				{
					"Personalizado 4",
					ImportContactProperties.User4
				},
				{
					"Nome do gerenciador",
					ImportContactProperties.ManagerName
				},
				{
					"País/Região da Empresa",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Retorno de chamada",
					ImportContactProperties.Callback
				},
				{
					"Telefone principal",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mail Address",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefone comercial 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Estado",
					ImportContactProperties.HomeState
				},
				{
					"Outro fax",
					ImportContactProperties.OtherFax
				},
				{
					"Rua do endereço comercial 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Cidade do endereço residencial",
					ImportContactProperties.HomeCity
				},
				{
					"Outro Estado",
					ImportContactProperties.OtherState
				},
				{
					"Rua do endereço comercial 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Tratamento",
					ImportContactProperties.Title
				},
				{
					"Endereço residencial 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Sala",
					ImportContactProperties.OfficeLocation
				},
				{
					"Yomi fornecido",
					ImportContactProperties.GivenYomi
				},
				{
					"País/Região de Residência",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefone do carro",
					ImportContactProperties.CarPhone
				},
				{
					"Empresa",
					ImportContactProperties.Company
				},
				{
					"Caixa postal do endereço residencial",
					ImportContactProperties.HomePOBox
				},
				{
					"Caixa postal de outro endereço",
					ImportContactProperties.OtherPOBox
				},
				{
					"Endereço residencial",
					ImportContactProperties.HomeStreet
				},
				{
					"Nome para exibição do email 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Sensibilidade",
					ImportContactProperties.Sensitivity
				},
				{
					"Anotações",
					ImportContactProperties.Notes
				},
				{
					"Primeiro nome",
					ImportContactProperties.FirstName
				},
				{
					"Outro endereço",
					ImportContactProperties.OtherStreet
				},
				{
					"Segundo nome",
					ImportContactProperties.MiddleName
				},
				{
					"Quilometragem",
					ImportContactProperties.Mileage
				},
				{
					"Personalizado 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1046));
		}

		private OutlookCsvLanguage CreateLanguageObject_pt_pt()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"Endereço do correio electrónico 2",
					ImportContactProperties.Email2Address
				},
				{
					"Cargo",
					ImportContactProperties.JobTitle
				},
				{
					"Outro País/Região",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Disponibilidade da Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Rua do outro endereço 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profissão",
					ImportContactProperties.Profession
				},
				{
					"Tipo de correio electrónico",
					ImportContactProperties.EmailType
				},
				{
					"Conta",
					ImportContactProperties.Account
				},
				{
					"Categorias",
					ImportContactProperties.Categories
				},
				{
					"Data de nascimento",
					ImportContactProperties.Birthday
				},
				{
					"Apartado da empresa",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Rua do outro endereço 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Número do BI",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Apelido",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefone",
					ImportContactProperties.RadioPhone
				},
				{
					"Aniversário",
					ImportContactProperties.Anniversary
				},
				{
					"Telefone TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tipo de correio electrónico 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nome a apresentar para o correio electrónico 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Código postal da empresa",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Outro telefone",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioridade",
					ImportContactProperties.Priority
				},
				{
					"Iniciais",
					ImportContactProperties.Initials
				},
				{
					"Cônjuge",
					ImportContactProperties.Spouse
				},
				{
					"Departamento",
					ImportContactProperties.Department
				},
				{
					"Telefone da residência",
					ImportContactProperties.HomePhone
				},
				{
					"Telefone da empresa",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Código postal do outro endereço",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Distrito da empresa",
					ImportContactProperties.BusinessState
				},
				{
					"Utilizador 2",
					ImportContactProperties.User2
				},
				{
					"2º telefone da residência",
					ImportContactProperties.HomePhone2
				},
				{
					"Telemóvel",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufixo",
					ImportContactProperties.Suffix
				},
				{
					"Passatempo",
					ImportContactProperties.Hobby
				},
				{
					"Utilizador 3",
					ImportContactProperties.User3
				},
				{
					"Telefone principal da empresa",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Filhos",
					ImportContactProperties.Children
				},
				{
					"Fax da empresa",
					ImportContactProperties.BusinessFax
				},
				{
					"Código postal da residência",
					ImportContactProperties.HomePostalCode
				},
				{
					"Localização",
					ImportContactProperties.Location
				},
				{
					"Dados para facturação",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax da residência",
					ImportContactProperties.HomeFax
				},
				{
					"Página na Web",
					ImportContactProperties.WebPage
				},
				{
					"Rua da residência (2)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Localidade da empresa",
					ImportContactProperties.BusinessCity
				},
				{
					"Localidade do outro endereço",
					ImportContactProperties.OtherCity
				},
				{
					"Rua da empresa",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tipo de correio electrónico 3",
					ImportContactProperties.Email3Type
				},
				{
					"Nome do assistente",
					ImportContactProperties.AssistantName
				},
				{
					"Sexo",
					ImportContactProperties.Gender
				},
				{
					"Telefone do assistente",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nome a apresentar para o correio electrónico",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Endereço do correio electrónico 3",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Utilizador 4",
					ImportContactProperties.User4
				},
				{
					"Nome do gestor",
					ImportContactProperties.ManagerName
				},
				{
					"País/Região da Empresa",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Tel. de resposta",
					ImportContactProperties.Callback
				},
				{
					"Telefone principal",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Endereço de correio electrónico",
					ImportContactProperties.EmailAddress
				},
				{
					"RDIS",
					ImportContactProperties.ISDN
				},
				{
					"2º telefone da empresa",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Distrito da residência",
					ImportContactProperties.HomeState
				},
				{
					"Outro fax",
					ImportContactProperties.OtherFax
				},
				{
					"Rua da empresa 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Localidade da residência",
					ImportContactProperties.HomeCity
				},
				{
					"Distrito do outro endereço",
					ImportContactProperties.OtherState
				},
				{
					"Rua da empresa 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Título",
					ImportContactProperties.Title
				},
				{
					"Rua da residência (3)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Gabinete",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"País da Residência/Região",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefone do carro",
					ImportContactProperties.CarPhone
				},
				{
					"Empresa",
					ImportContactProperties.Company
				},
				{
					"Apartado da residência",
					ImportContactProperties.HomePOBox
				},
				{
					"Apartado do outro endereço",
					ImportContactProperties.OtherPOBox
				},
				{
					"Rua da residência",
					ImportContactProperties.HomeStreet
				},
				{
					"Nome a apresentar para o correio electrónico 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Confidencialidade",
					ImportContactProperties.Sensitivity
				},
				{
					"Notas",
					ImportContactProperties.Notes
				},
				{
					"Primeiro nome",
					ImportContactProperties.FirstName
				},
				{
					"Rua do outro endereço",
					ImportContactProperties.OtherStreet
				},
				{
					"Outros nomes/apelidos",
					ImportContactProperties.MiddleName
				},
				{
					"Quilometragem",
					ImportContactProperties.Mileage
				},
				{
					"Utilizador 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(2070));
		}

		private OutlookCsvLanguage CreateLanguageObject_ro_ro()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"Adresă potă electronică 2",
					ImportContactProperties.Email2Address
				},
				{
					"Ocupaţie",
					ImportContactProperties.JobTitle
				},
				{
					"Altă ţară/regiune",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Internet Liber Ocupat",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Altă stradă 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesie",
					ImportContactProperties.Profession
				},
				{
					"Tip de potă electronică",
					ImportContactProperties.EmailType
				},
				{
					"Cont",
					ImportContactProperties.Account
				},
				{
					"Categorii",
					ImportContactProperties.Categories
				},
				{
					"Zi de natere",
					ImportContactProperties.Birthday
				},
				{
					"Căsuţă potală la firmă",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Altă stradă 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Număr IDentificator la guvern",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Nume de familie",
					ImportContactProperties.LastName
				},
				{
					"Radiotelefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Aniversare",
					ImportContactProperties.Anniversary
				},
				{
					"Telefon TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tip potă electronică 2",
					ImportContactProperties.Email2Type
				},
				{
					"Nume afiare potă electronică 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Cod potal Firmă",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Alt telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritate",
					ImportContactProperties.Priority
				},
				{
					"Iniţiale",
					ImportContactProperties.Initials
				},
				{
					"Soţ(ie)",
					ImportContactProperties.Spouse
				},
				{
					"Serviciu",
					ImportContactProperties.Department
				},
				{
					"Telefon domiciliu",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon firmă",
					ImportContactProperties.BusinessPhone
				},
				{
					"Yomi firmă",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pager",
					ImportContactProperties.Pager
				},
				{
					"Alt cod potal",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Judeţ firmă",
					ImportContactProperties.BusinessState
				},
				{
					"Utilizator 2",
					ImportContactProperties.User2
				},
				{
					"Telefon 2 domiciliu",
					ImportContactProperties.HomePhone2
				},
				{
					"Telefon mobil",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufix",
					ImportContactProperties.Suffix
				},
				{
					"Pasiune",
					ImportContactProperties.Hobby
				},
				{
					"Utilizator 3",
					ImportContactProperties.User3
				},
				{
					"Telefon principal firmă",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Copii",
					ImportContactProperties.Children
				},
				{
					"Fax firmă",
					ImportContactProperties.BusinessFax
				},
				{
					"Cod potal domiciliu",
					ImportContactProperties.HomePostalCode
				},
				{
					"Amplasare",
					ImportContactProperties.Location
				},
				{
					"Informații pentru facturare",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax domiciliu",
					ImportContactProperties.HomeFax
				},
				{
					"Pagină Web",
					ImportContactProperties.WebPage
				},
				{
					"Stradă 2 domiciliu",
					ImportContactProperties.HomeStreet2
				},
				{
					"Ora Firmă",
					ImportContactProperties.BusinessCity
				},
				{
					"Alt ora",
					ImportContactProperties.OtherCity
				},
				{
					"Stradă Firmă",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tip potă electronică 3",
					ImportContactProperties.Email3Type
				},
				{
					"Nume asistent",
					ImportContactProperties.AssistantName
				},
				{
					"Sex",
					ImportContactProperties.Gender
				},
				{
					"Număr telefon asistent",
					ImportContactProperties.AssistantPhone
				},
				{
					"Nume afiare potă electronică",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Adresă potă electronică 3",
					ImportContactProperties.Email3Address
				},
				{
					"Nume Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Utilizator 4",
					ImportContactProperties.User4
				},
				{
					"Nume manager",
					ImportContactProperties.ManagerName
				},
				{
					"Ţară/regiune firmă",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Revenire cu apel telefonic",
					ImportContactProperties.Callback
				},
				{
					"Telefon principal",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Adresă de potă electronică",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon 2 firmă",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Judeţ domiciliu",
					ImportContactProperties.HomeState
				},
				{
					"Alt fax",
					ImportContactProperties.OtherFax
				},
				{
					"Strada 3 Firmă",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Ora domiciliu",
					ImportContactProperties.HomeCity
				},
				{
					"Alt judeţ",
					ImportContactProperties.OtherState
				},
				{
					"Strada 2 Firmă",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titlu",
					ImportContactProperties.Title
				},
				{
					"Stradă 3 domiciliu",
					ImportContactProperties.HomeStreet3
				},
				{
					"Amplasare birou",
					ImportContactProperties.OfficeLocation
				},
				{
					"Prenume Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Ţară/regiune domiciliu",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefon pe maină",
					ImportContactProperties.CarPhone
				},
				{
					"Firmă",
					ImportContactProperties.Company
				},
				{
					"Căsuţă potală la domiciliu",
					ImportContactProperties.HomePOBox
				},
				{
					"Căsuţă potală la altă adresă",
					ImportContactProperties.OtherPOBox
				},
				{
					"Stradă domiciliu",
					ImportContactProperties.HomeStreet
				},
				{
					"Nume afiare potă electronică 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Sensibilitate",
					ImportContactProperties.Sensitivity
				},
				{
					"Notes",
					ImportContactProperties.Notes
				},
				{
					"Prenume",
					ImportContactProperties.FirstName
				},
				{
					"Altă stradă",
					ImportContactProperties.OtherStreet
				},
				{
					"Al doilea nume",
					ImportContactProperties.MiddleName
				},
				{
					"Distanţă parcursă",
					ImportContactProperties.Mileage
				},
				{
					"Utilizator 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1048));
		}

		private OutlookCsvLanguage CreateLanguageObject_ru_ru()
		{
			return new OutlookCsvLanguage(1251, new Dictionary<string, ImportContactProperties>
			{
				{
					"Адрес 2 эл. почты",
					ImportContactProperties.Email2Address
				},
				{
					"Должность",
					ImportContactProperties.JobTitle
				},
				{
					"Страна или регион \u00a0(другой адрес)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Телекс",
					ImportContactProperties.Telex
				},
				{
					"Сведения о доступности в Интернете",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Улица \u00a02 (другой адрес)",
					ImportContactProperties.OtherStreet2
				},
				{
					"Профессия",
					ImportContactProperties.Profession
				},
				{
					"Тип эл. почты",
					ImportContactProperties.EmailType
				},
				{
					"Счет",
					ImportContactProperties.Account
				},
				{
					"Категории",
					ImportContactProperties.Categories
				},
				{
					"День рождения",
					ImportContactProperties.Birthday
				},
				{
					"Почтовый ящик (раб. адрес)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Улица \u00a03 (другой адрес)",
					ImportContactProperties.OtherStreet3
				},
				{
					"Личный код",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Фамилия",
					ImportContactProperties.LastName
				},
				{
					"Радиотелефон",
					ImportContactProperties.RadioPhone
				},
				{
					"Годовщина",
					ImportContactProperties.Anniversary
				},
				{
					"Телетайп/телефон с титрами",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Тип 2 эл. почты",
					ImportContactProperties.Email2Type
				},
				{
					"Краткое 2 имя эл. почты",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Индекс (раб. адрес)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Другой телефон",
					ImportContactProperties.OtherPhone
				},
				{
					"Важность",
					ImportContactProperties.Priority
				},
				{
					"Инициалы",
					ImportContactProperties.Initials
				},
				{
					"Супруг(а)",
					ImportContactProperties.Spouse
				},
				{
					"Отдел",
					ImportContactProperties.Department
				},
				{
					"Домашний телефон",
					ImportContactProperties.HomePhone
				},
				{
					"Рабочий телефон",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Пейджер",
					ImportContactProperties.Pager
				},
				{
					"Индекс \u00a0(другой адрес)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Область (раб. адрес)",
					ImportContactProperties.BusinessState
				},
				{
					"Пользователь 2",
					ImportContactProperties.User2
				},
				{
					"Телефон дом. 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Телефон переносной",
					ImportContactProperties.MobilePhone
				},
				{
					"Суффикс",
					ImportContactProperties.Suffix
				},
				{
					"Хобби",
					ImportContactProperties.Hobby
				},
				{
					"Пользователь 3",
					ImportContactProperties.User3
				},
				{
					"Основной телефон организации",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Дети",
					ImportContactProperties.Children
				},
				{
					"Рабочий факс",
					ImportContactProperties.BusinessFax
				},
				{
					"Почтовый код (дом.)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Расположение",
					ImportContactProperties.Location
				},
				{
					"Счета",
					ImportContactProperties.BillingInformation
				},
				{
					"Домашний факс",
					ImportContactProperties.HomeFax
				},
				{
					"Веб-страница",
					ImportContactProperties.WebPage
				},
				{
					"Улица 2 (дом. адрес)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Город (раб. адрес)",
					ImportContactProperties.BusinessCity
				},
				{
					"Город \u00a0(другой адрес)",
					ImportContactProperties.OtherCity
				},
				{
					"Улица (раб. адрес)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Тип 3 эл. почты",
					ImportContactProperties.Email3Type
				},
				{
					"Имя помощника",
					ImportContactProperties.AssistantName
				},
				{
					"Пол",
					ImportContactProperties.Gender
				},
				{
					"Телефон помощника",
					ImportContactProperties.AssistantPhone
				},
				{
					"Краткое имя эл. почты",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Адрес 3 эл. почты",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Пользователь 4",
					ImportContactProperties.User4
				},
				{
					"Руководитель",
					ImportContactProperties.ManagerName
				},
				{
					"Страна или регион (раб. адрес)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Обратный вызов",
					ImportContactProperties.Callback
				},
				{
					"Основной телефон",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Адрес эл. почты",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Телефон раб. 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Область (дом. адрес)",
					ImportContactProperties.HomeState
				},
				{
					"Другой факс",
					ImportContactProperties.OtherFax
				},
				{
					"Улица 3 (раб. адрес)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Город (дом. адрес)",
					ImportContactProperties.HomeCity
				},
				{
					"Область \u00a0(другой адрес)",
					ImportContactProperties.OtherState
				},
				{
					"Улица 2 (раб. адрес)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Обращение",
					ImportContactProperties.Title
				},
				{
					"Улица 3 (дом. адрес)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Расположение комнаты",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Страна или регион (дом. адрес)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Телефон в машине",
					ImportContactProperties.CarPhone
				},
				{
					"Организация",
					ImportContactProperties.Company
				},
				{
					"Почтовый ящик (дом. адрес)",
					ImportContactProperties.HomePOBox
				},
				{
					"Почтовый ящик (другой адрес)",
					ImportContactProperties.OtherPOBox
				},
				{
					"Улица (дом. адрес)",
					ImportContactProperties.HomeStreet
				},
				{
					"Краткое 3 имя эл. почты",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Пометка",
					ImportContactProperties.Sensitivity
				},
				{
					"Заметки",
					ImportContactProperties.Notes
				},
				{
					"Имя",
					ImportContactProperties.FirstName
				},
				{
					"Улица (другой адрес)",
					ImportContactProperties.OtherStreet
				},
				{
					"Отчество",
					ImportContactProperties.MiddleName
				},
				{
					"Расстояние",
					ImportContactProperties.Mileage
				},
				{
					"Пользователь 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1049));
		}

		private OutlookCsvLanguage CreateLanguageObject_sk_sk()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-mailová adresa 2",
					ImportContactProperties.Email2Address
				},
				{
					"Funkcia",
					ImportContactProperties.JobTitle
				},
				{
					"Krajina alebo oblasť (iné)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Informácie o voľnom čase na Internete",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Iná ulica 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesia",
					ImportContactProperties.Profession
				},
				{
					"Typ e-mailu",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategórie",
					ImportContactProperties.Categories
				},
				{
					"Narodeniny",
					ImportContactProperties.Birthday
				},
				{
					"P.O. box pracoviska",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Iná ulica 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Rodné číslo",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Priezvisko",
					ImportContactProperties.LastName
				},
				{
					"Rádiotelefón",
					ImportContactProperties.RadioPhone
				},
				{
					"Výročie",
					ImportContactProperties.Anniversary
				},
				{
					"Telefón TTY/TTD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Typ e-mailu 2",
					ImportContactProperties.Email2Type
				},
				{
					"Zobrazované meno príjemcu e-mailu 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"PSČ (práca)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Iný telefón",
					ImportContactProperties.OtherPhone
				},
				{
					"Priorita",
					ImportContactProperties.Priority
				},
				{
					"Iniciálky",
					ImportContactProperties.Initials
				},
				{
					"Manžel(ka)",
					ImportContactProperties.Spouse
				},
				{
					"Oddelenie",
					ImportContactProperties.Department
				},
				{
					"Telefón domov",
					ImportContactProperties.HomePhone
				},
				{
					"Telefón do práce",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Operátor",
					ImportContactProperties.Pager
				},
				{
					"Iné PSČ",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Okres (práca)",
					ImportContactProperties.BusinessState
				},
				{
					"Používateľ 2",
					ImportContactProperties.User2
				},
				{
					"Telefón domov 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobilný telefón",
					ImportContactProperties.MobilePhone
				},
				{
					"Prípona",
					ImportContactProperties.Suffix
				},
				{
					"Záľuby",
					ImportContactProperties.Hobby
				},
				{
					"Používateľ 3",
					ImportContactProperties.User3
				},
				{
					"Hlavný telefón spoločnosti",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Deti",
					ImportContactProperties.Children
				},
				{
					"Fax do práce",
					ImportContactProperties.BusinessFax
				},
				{
					"PSČ (domov)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Umiestnenie",
					ImportContactProperties.Location
				},
				{
					"Vyúčtovanie",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax domov",
					ImportContactProperties.HomeFax
				},
				{
					"Webová stránka",
					ImportContactProperties.WebPage
				},
				{
					"Ulica 2 (domov)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Mesto (práca)",
					ImportContactProperties.BusinessCity
				},
				{
					"Iné mesto",
					ImportContactProperties.OtherCity
				},
				{
					"Ulica (práca)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Typ e-mailu 3",
					ImportContactProperties.Email3Type
				},
				{
					"Meno asistenta",
					ImportContactProperties.AssistantName
				},
				{
					"Pohlavie",
					ImportContactProperties.Gender
				},
				{
					"Telefón asistenta",
					ImportContactProperties.AssistantPhone
				},
				{
					"Zobrazované meno príjemcu e-mailu",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-mailová adresa 3",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Používateľ 4",
					ImportContactProperties.User4
				},
				{
					"Meno manažéra",
					ImportContactProperties.ManagerName
				},
				{
					"Krajina alebo oblasť (práca)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Spätné volanie",
					ImportContactProperties.Callback
				},
				{
					"Primárny telefón",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-mailová adresa",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefón do práce 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Okres (domov)",
					ImportContactProperties.HomeState
				},
				{
					"Iný fax",
					ImportContactProperties.OtherFax
				},
				{
					"Ulica (práca) 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Mesto (domov)",
					ImportContactProperties.HomeCity
				},
				{
					"Iný kraj",
					ImportContactProperties.OtherState
				},
				{
					"Ulica (práca) 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titul",
					ImportContactProperties.Title
				},
				{
					"Ulica 3 (domov)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Pracovisko",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Krajina alebo oblasť (domov)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Autotelefón",
					ImportContactProperties.CarPhone
				},
				{
					"Spoločnosť",
					ImportContactProperties.Company
				},
				{
					"P.O. box trvalého bydliska",
					ImportContactProperties.HomePOBox
				},
				{
					"P.O. box prechodného bydliska",
					ImportContactProperties.OtherPOBox
				},
				{
					"Ulica (domov)",
					ImportContactProperties.HomeStreet
				},
				{
					"Zobrazované meno príjemcu e-mailu 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Citlivosť",
					ImportContactProperties.Sensitivity
				},
				{
					"Poznámky",
					ImportContactProperties.Notes
				},
				{
					"Krstné meno",
					ImportContactProperties.FirstName
				},
				{
					"Iná ulica",
					ImportContactProperties.OtherStreet
				},
				{
					"Ďalšie meno",
					ImportContactProperties.MiddleName
				},
				{
					"Vzdialenosť",
					ImportContactProperties.Mileage
				},
				{
					"Používateľ 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1051));
		}

		private OutlookCsvLanguage CreateLanguageObject_sl_si()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-poštni naslov 2",
					ImportContactProperties.Email2Address
				},
				{
					"Naziv1",
					ImportContactProperties.JobTitle
				},
				{
					"Drugi naslov – država/regija",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Internetni prosti/zasedeni čas",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Drugi naslov - ulica 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Poklic",
					ImportContactProperties.Profession
				},
				{
					"Vrsta e-pošte",
					ImportContactProperties.EmailType
				},
				{
					"Račun",
					ImportContactProperties.Account
				},
				{
					"Zvrsti",
					ImportContactProperties.Categories
				},
				{
					"Rojstni dan",
					ImportContactProperties.Birthday
				},
				{
					"Službeni naslov - poštni predal",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Drugi naslov - ulica 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Vladna ID številka",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Priimek",
					ImportContactProperties.LastName
				},
				{
					"Radijski telefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Obletnica",
					ImportContactProperties.Anniversary
				},
				{
					"Telefon TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Vrsta e-pošte 2",
					ImportContactProperties.Email2Type
				},
				{
					"E-poštno ime za prikaz 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Službeni naslov - poštna številka",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Drugi telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioriteta",
					ImportContactProperties.Priority
				},
				{
					"Začetnice",
					ImportContactProperties.Initials
				},
				{
					"Zakonec",
					ImportContactProperties.Spouse
				},
				{
					"Oddelek",
					ImportContactProperties.Department
				},
				{
					"Domači telefon",
					ImportContactProperties.HomePhone
				},
				{
					"Službeni telefon",
					ImportContactProperties.BusinessPhone
				},
				{
					"Podjetje Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pozivnik",
					ImportContactProperties.Pager
				},
				{
					"Drugi naslov - poštna številka",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Službeni naslov - zvezna država",
					ImportContactProperties.BusinessState
				},
				{
					"Uporabnik 2",
					ImportContactProperties.User2
				},
				{
					"Domači telefon 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Prenosni telefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Pripona",
					ImportContactProperties.Suffix
				},
				{
					"Konjiček",
					ImportContactProperties.Hobby
				},
				{
					"Uporabnik 3",
					ImportContactProperties.User3
				},
				{
					"Centrala podjetja",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Podrejeni",
					ImportContactProperties.Children
				},
				{
					"Službeni faks",
					ImportContactProperties.BusinessFax
				},
				{
					"Domači naslov - poštna številka",
					ImportContactProperties.HomePostalCode
				},
				{
					"Mesto",
					ImportContactProperties.Location
				},
				{
					"Podatki za obračun",
					ImportContactProperties.BillingInformation
				},
				{
					"Domači faks",
					ImportContactProperties.HomeFax
				},
				{
					"Spletna stran",
					ImportContactProperties.WebPage
				},
				{
					"Domači naslov - ulica 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Službeni naslov - kraj",
					ImportContactProperties.BusinessCity
				},
				{
					"Drugi naslov - kraj",
					ImportContactProperties.OtherCity
				},
				{
					"Službeni naslov - ulica",
					ImportContactProperties.BusinessStreet
				},
				{
					"Vrsta e-pošte 3",
					ImportContactProperties.Email3Type
				},
				{
					"Pomočnik",
					ImportContactProperties.AssistantName
				},
				{
					"Spol",
					ImportContactProperties.Gender
				},
				{
					"Pomočnikov telefon",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-poštno ime za prikaz",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-poštni naslov 3",
					ImportContactProperties.Email3Address
				},
				{
					"Priimek Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Uporabnik 4",
					ImportContactProperties.User4
				},
				{
					"Direktor",
					ImportContactProperties.ManagerName
				},
				{
					"Službeni naslov – država/regija",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Povratni klic",
					ImportContactProperties.Callback
				},
				{
					"Primarni telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-poštni naslov",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Službeni telefon 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Domači naslov - zvezna država",
					ImportContactProperties.HomeState
				},
				{
					"Drugi faks",
					ImportContactProperties.OtherFax
				},
				{
					"Službeni naslov - ulica 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Domači naslov - kraj",
					ImportContactProperties.HomeCity
				},
				{
					"Drugi naslov - zvezna država",
					ImportContactProperties.OtherState
				},
				{
					"Službeni naslov - ulica 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Naziv",
					ImportContactProperties.Title
				},
				{
					"Domači naslov - ulica 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Pisarna",
					ImportContactProperties.OfficeLocation
				},
				{
					"Ime Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Domači naslov – država/regija",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Podjetje",
					ImportContactProperties.Company
				},
				{
					"Domači naslov - poštni predal",
					ImportContactProperties.HomePOBox
				},
				{
					"Drugi naslov - poštni predal",
					ImportContactProperties.OtherPOBox
				},
				{
					"Domači naslov - ulica",
					ImportContactProperties.HomeStreet
				},
				{
					"E-poštno ime za prikaz 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Občutljivost",
					ImportContactProperties.Sensitivity
				},
				{
					"Opombe",
					ImportContactProperties.Notes
				},
				{
					"Ime",
					ImportContactProperties.FirstName
				},
				{
					"Drugi naslov - ulica",
					ImportContactProperties.OtherStreet
				},
				{
					"Drugo ime",
					ImportContactProperties.MiddleName
				},
				{
					"Kilometrina",
					ImportContactProperties.Mileage
				},
				{
					"Uporabnik 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1060));
		}

		private OutlookCsvLanguage CreateLanguageObject_sr_sp()
		{
			return new OutlookCsvLanguage(1250, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-adresa 2",
					ImportContactProperties.Email2Address
				},
				{
					"Radno mesto",
					ImportContactProperties.JobTitle
				},
				{
					"Druga država/region",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Zauzetost na Internetu",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Druga ulica 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Profesija",
					ImportContactProperties.Profession
				},
				{
					"Tip e-poruke",
					ImportContactProperties.EmailType
				},
				{
					"Nalog",
					ImportContactProperties.Account
				},
				{
					"Kategorije",
					ImportContactProperties.Categories
				},
				{
					"Rođendan",
					ImportContactProperties.Birthday
				},
				{
					"Adresa na poslu: poštanski fah",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Druga ulica 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Matični broj",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Prezime",
					ImportContactProperties.LastName
				},
				{
					"Radio telefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Godišnjica",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Tip e-pošte 2",
					ImportContactProperties.Email2Type
				},
				{
					"Ime za prikaz za e-poštu 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Poštanski broj (posao)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Drugi telefon",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritet",
					ImportContactProperties.Priority
				},
				{
					"Inicijali",
					ImportContactProperties.Initials
				},
				{
					"Supružnik",
					ImportContactProperties.Spouse
				},
				{
					"Odeljenje",
					ImportContactProperties.Department
				},
				{
					"Telefon kod kuće",
					ImportContactProperties.HomePhone
				},
				{
					"Telefon na poslu",
					ImportContactProperties.BusinessPhone
				},
				{
					"Kompanija Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Pejdžer",
					ImportContactProperties.Pager
				},
				{
					"Drugi poštanski broj",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Država (posao)",
					ImportContactProperties.BusinessState
				},
				{
					"Korisnik 2",
					ImportContactProperties.User2
				},
				{
					"Telefon kod kuće 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobilni telefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Sufiks",
					ImportContactProperties.Suffix
				},
				{
					"Hobi",
					ImportContactProperties.Hobby
				},
				{
					"Korisnik 3",
					ImportContactProperties.User3
				},
				{
					"Glavni telefon kompanije",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Deca",
					ImportContactProperties.Children
				},
				{
					"Faks na poslu",
					ImportContactProperties.BusinessFax
				},
				{
					"Matični poštanski broj",
					ImportContactProperties.HomePostalCode
				},
				{
					"Lokacija",
					ImportContactProperties.Location
				},
				{
					"Informacija o obračunu",
					ImportContactProperties.BillingInformation
				},
				{
					"Faks kod kuće",
					ImportContactProperties.HomeFax
				},
				{
					"Veb stranica",
					ImportContactProperties.WebPage
				},
				{
					"Kućna adresa 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"Grad (posao)",
					ImportContactProperties.BusinessCity
				},
				{
					"Drugi grad",
					ImportContactProperties.OtherCity
				},
				{
					"Ulica (posao)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Tip e-pošte 3",
					ImportContactProperties.Email3Type
				},
				{
					"Ime pomoćnika",
					ImportContactProperties.AssistantName
				},
				{
					"Pol",
					ImportContactProperties.Gender
				},
				{
					"Telefon pomoćnika",
					ImportContactProperties.AssistantPhone
				},
				{
					"Ime za prikaz za e-poštu",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-adresa 3",
					ImportContactProperties.Email3Address
				},
				{
					"Prezime Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Korisnik 4",
					ImportContactProperties.User4
				},
				{
					"Ime direktora",
					ImportContactProperties.ManagerName
				},
				{
					"Država/region firme",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Povratni poziv",
					ImportContactProperties.Callback
				},
				{
					"Primarni telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-adresa",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon na poslu 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Matična država",
					ImportContactProperties.HomeState
				},
				{
					"Drugi faks",
					ImportContactProperties.OtherFax
				},
				{
					"Ulica (posao) 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Grad",
					ImportContactProperties.HomeCity
				},
				{
					"Druga država",
					ImportContactProperties.OtherState
				},
				{
					"Ulica (posao) 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Zvanje",
					ImportContactProperties.Title
				},
				{
					"Kućna adresa 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"Lokacija kancelarije",
					ImportContactProperties.OfficeLocation
				},
				{
					"Ime Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Matična država/region",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Telefon u automobilu",
					ImportContactProperties.CarPhone
				},
				{
					"Kompanija",
					ImportContactProperties.Company
				},
				{
					"Kućna adresa: poštanski fah",
					ImportContactProperties.HomePOBox
				},
				{
					"Druga adresa: poštanski fah",
					ImportContactProperties.OtherPOBox
				},
				{
					"Kućna adresa",
					ImportContactProperties.HomeStreet
				},
				{
					"Ime za prikaz za e-poštu 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Stepen poverljivosti",
					ImportContactProperties.Sensitivity
				},
				{
					"Beleške",
					ImportContactProperties.Notes
				},
				{
					"Ime",
					ImportContactProperties.FirstName
				},
				{
					"Druga ulica",
					ImportContactProperties.OtherStreet
				},
				{
					"Srednje ime",
					ImportContactProperties.MiddleName
				},
				{
					"Kilometraža",
					ImportContactProperties.Mileage
				},
				{
					"Korisnik 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(2074));
		}

		private OutlookCsvLanguage CreateLanguageObject_sv_se()
		{
			return new OutlookCsvLanguage(1252, new Dictionary<string, ImportContactProperties>
			{
				{
					"E-postadress 2",
					ImportContactProperties.Email2Address
				},
				{
					"Befattning",
					ImportContactProperties.JobTitle
				},
				{
					"Annat land",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telex",
					ImportContactProperties.Telex
				},
				{
					"Ledig/upptagen-information för Internet",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Annan gata 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Yrke",
					ImportContactProperties.Profession
				},
				{
					"E-posttyp",
					ImportContactProperties.EmailType
				},
				{
					"Konto",
					ImportContactProperties.Account
				},
				{
					"Kategorier",
					ImportContactProperties.Categories
				},
				{
					"Födelsedag",
					ImportContactProperties.Birthday
				},
				{
					"Arbetsadress, box",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Annan gata 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Personnummer",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Efternamn",
					ImportContactProperties.LastName
				},
				{
					"Telefon 2",
					ImportContactProperties.RadioPhone
				},
				{
					"Årsdag",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD-telefon",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"E-posttyp 2",
					ImportContactProperties.Email2Type
				},
				{
					"E-postnamn 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Postadress, arbete",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Telefon, annan",
					ImportContactProperties.OtherPhone
				},
				{
					"Prioritet",
					ImportContactProperties.Priority
				},
				{
					"Initialer",
					ImportContactProperties.Initials
				},
				{
					"Make maka",
					ImportContactProperties.Spouse
				},
				{
					"Avdelning",
					ImportContactProperties.Department
				},
				{
					"Hem",
					ImportContactProperties.HomePhone
				},
				{
					"Arbete",
					ImportContactProperties.BusinessPhone
				},
				{
					"Företag Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Personsökare",
					ImportContactProperties.Pager
				},
				{
					"Annat postnummer",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Region, arbete",
					ImportContactProperties.BusinessState
				},
				{
					"Användare 2",
					ImportContactProperties.User2
				},
				{
					"Telefon 2, hem",
					ImportContactProperties.HomePhone2
				},
				{
					"Mobiltelefon",
					ImportContactProperties.MobilePhone
				},
				{
					"Namnsuffix",
					ImportContactProperties.Suffix
				},
				{
					"Hobby",
					ImportContactProperties.Hobby
				},
				{
					"Användare 3",
					ImportContactProperties.User3
				},
				{
					"Företag, växel",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Barn",
					ImportContactProperties.Children
				},
				{
					"Fax arbete",
					ImportContactProperties.BusinessFax
				},
				{
					"Postnr, hem",
					ImportContactProperties.HomePostalCode
				},
				{
					"Plats",
					ImportContactProperties.Location
				},
				{
					"Fakturering",
					ImportContactProperties.BillingInformation
				},
				{
					"Fax, hem",
					ImportContactProperties.HomeFax
				},
				{
					"Webbsida",
					ImportContactProperties.WebPage
				},
				{
					"Gatuadress 2, hem",
					ImportContactProperties.HomeStreet2
				},
				{
					"Ort, arbete",
					ImportContactProperties.BusinessCity
				},
				{
					"Annan ort",
					ImportContactProperties.OtherCity
				},
				{
					"Gatuadress, arbete",
					ImportContactProperties.BusinessStreet
				},
				{
					"E-posttyp 3",
					ImportContactProperties.Email3Type
				},
				{
					"Namn, assistent",
					ImportContactProperties.AssistantName
				},
				{
					"Kön",
					ImportContactProperties.Gender
				},
				{
					"Telefon, assistent",
					ImportContactProperties.AssistantPhone
				},
				{
					"E-postnamn",
					ImportContactProperties.EmailDisplayName
				},
				{
					"E-postadress 3",
					ImportContactProperties.Email3Address
				},
				{
					"Efternamn Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Användare 4",
					ImportContactProperties.User4
				},
				{
					"Namn, chef",
					ImportContactProperties.ManagerName
				},
				{
					"Land, arbete",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Motringning",
					ImportContactProperties.Callback
				},
				{
					"Telefon 1",
					ImportContactProperties.PrimaryPhone
				},
				{
					"E-postadress",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Telefon 2, arbete",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Region, hem",
					ImportContactProperties.HomeState
				},
				{
					"Fax, annan",
					ImportContactProperties.OtherFax
				},
				{
					"Gatuadress 3, arbete",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Hemort",
					ImportContactProperties.HomeCity
				},
				{
					"Annan region",
					ImportContactProperties.OtherState
				},
				{
					"Gatuadress 2, arbete",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Titel",
					ImportContactProperties.Title
				},
				{
					"Gatuadress 3, hem",
					ImportContactProperties.HomeStreet3
				},
				{
					"Arbetsplats",
					ImportContactProperties.OfficeLocation
				},
				{
					"Förnamn Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Land, hem",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Biltelefon",
					ImportContactProperties.CarPhone
				},
				{
					"Företag",
					ImportContactProperties.Company
				},
				{
					"Hemadress, box",
					ImportContactProperties.HomePOBox
				},
				{
					"Annan adress, box",
					ImportContactProperties.OtherPOBox
				},
				{
					"Gatuadress, hem",
					ImportContactProperties.HomeStreet
				},
				{
					"E-postnamn 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Känslighet",
					ImportContactProperties.Sensitivity
				},
				{
					"Anteckningar",
					ImportContactProperties.Notes
				},
				{
					"Förnamn",
					ImportContactProperties.FirstName
				},
				{
					"Annan gata",
					ImportContactProperties.OtherStreet
				},
				{
					"Mellannamn",
					ImportContactProperties.MiddleName
				},
				{
					"Reseersättning (km)",
					ImportContactProperties.Mileage
				},
				{
					"Användare 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1053));
		}

		private OutlookCsvLanguage CreateLanguageObject_th_th()
		{
			return new OutlookCsvLanguage(874, new Dictionary<string, ImportContactProperties>
			{
				{
					"ที่อยู่อีเมล 2",
					ImportContactProperties.Email2Address
				},
				{
					"ตำแหน่งงาน",
					ImportContactProperties.JobTitle
				},
				{
					"ประเทศ/ภูมิภาคอื่น",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"เทเล็กซ์",
					ImportContactProperties.Telex
				},
				{
					"ข้อมูลว่างไม่ว่างบนอินเทอร์เน็ต",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"ถนนอื่น 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"อาชีพ",
					ImportContactProperties.Profession
				},
				{
					"ชนิดอีเมล",
					ImportContactProperties.EmailType
				},
				{
					"บัญชีผู้ใช้",
					ImportContactProperties.Account
				},
				{
					"ประเภท",
					ImportContactProperties.Categories
				},
				{
					"วันเกิด",
					ImportContactProperties.Birthday
				},
				{
					"ตู้ ป.ณ. ของที่อยู่ (ธุรกิจ)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"ถนนอื่น 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"ID ประจำตัวทางราชการ",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"นามสกุล",
					ImportContactProperties.LastName
				},
				{
					"โทรศัพท์วิทยุ",
					ImportContactProperties.RadioPhone
				},
				{
					"วันครบรอบ",
					ImportContactProperties.Anniversary
				},
				{
					"โทรศัพท์ TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"ชนิดของอีเมล 2",
					ImportContactProperties.Email2Type
				},
				{
					"ชื่อที่ใช้แสดงของอีเมล 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"รหัสไปรษณีย์ (ธุรกิจ)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"โทรศัพท์อื่น",
					ImportContactProperties.OtherPhone
				},
				{
					"ลำดับความสำคัญ",
					ImportContactProperties.Priority
				},
				{
					"ชื่อย่อ",
					ImportContactProperties.Initials
				},
				{
					"คู่สมรส",
					ImportContactProperties.Spouse
				},
				{
					"แผนก",
					ImportContactProperties.Department
				},
				{
					"โทรศัพท์ (บ้าน)",
					ImportContactProperties.HomePhone
				},
				{
					"โทรศัพท์ (ธุรกิจ)",
					ImportContactProperties.BusinessPhone
				},
				{
					"บริษัท โยมิ",
					ImportContactProperties.CompanyYomi
				},
				{
					"เพจเจอร์",
					ImportContactProperties.Pager
				},
				{
					"รหัสไปรษณีย์อื่น",
					ImportContactProperties.OtherPostalCode
				},
				{
					"จังหวัด (ธุรกิจ)",
					ImportContactProperties.BusinessState
				},
				{
					"ผู้ใช้ 2",
					ImportContactProperties.User2
				},
				{
					"โทรศัพท์ 2 (บ้าน)",
					ImportContactProperties.HomePhone2
				},
				{
					"โทรศัพท์มือถือ",
					ImportContactProperties.MobilePhone
				},
				{
					"คำต่อท้าย",
					ImportContactProperties.Suffix
				},
				{
					"งานอดิเรก",
					ImportContactProperties.Hobby
				},
				{
					"ผู้ใช้ 3",
					ImportContactProperties.User3
				},
				{
					"โทรศัพท์หลักของบริษัท",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"เด็ก",
					ImportContactProperties.Children
				},
				{
					"โทรสาร (ธุรกิจ)",
					ImportContactProperties.BusinessFax
				},
				{
					"รหัสไปรษณีย์ (บ้าน)",
					ImportContactProperties.HomePostalCode
				},
				{
					"ที่ตั้ง",
					ImportContactProperties.Location
				},
				{
					"ข้อมูลการเรียกเก็บเงิน",
					ImportContactProperties.BillingInformation
				},
				{
					"โทรสาร (บ้าน)",
					ImportContactProperties.HomeFax
				},
				{
					"เว็บเพจ",
					ImportContactProperties.WebPage
				},
				{
					"ถนน 2 (บ้าน)",
					ImportContactProperties.HomeStreet2
				},
				{
					"เมือง (ธุรกิจ)",
					ImportContactProperties.BusinessCity
				},
				{
					"เมืองอื่น",
					ImportContactProperties.OtherCity
				},
				{
					"ถนน (ธุรกิจ)",
					ImportContactProperties.BusinessStreet
				},
				{
					"ชนิดของอีเมล 3",
					ImportContactProperties.Email3Type
				},
				{
					"ชื่อผู้ช่วย",
					ImportContactProperties.AssistantName
				},
				{
					"เพศ",
					ImportContactProperties.Gender
				},
				{
					"โทรศัพท์ของผู้ช่วย",
					ImportContactProperties.AssistantPhone
				},
				{
					"ชื่อที่ใช้แสดงของอีเมล",
					ImportContactProperties.EmailDisplayName
				},
				{
					"ที่อยู่อีเมล 3",
					ImportContactProperties.Email3Address
				},
				{
					"นามสกุล โยมิ",
					ImportContactProperties.SurnameYomi
				},
				{
					"ผู้ใช้ 4",
					ImportContactProperties.User4
				},
				{
					"ชื่อผู้จัดการ",
					ImportContactProperties.ManagerName
				},
				{
					"ประเทศ/ภูมิภาค (ธุรกิจ)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"ติดต่อกลับ",
					ImportContactProperties.Callback
				},
				{
					"โทรศัพท์หลัก",
					ImportContactProperties.PrimaryPhone
				},
				{
					"ที่อยู่อีเมล",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"โทรศัพท์ 2 (ธุรกิจ)",
					ImportContactProperties.BusinessPhone2
				},
				{
					"จังหวัด (บ้าน)",
					ImportContactProperties.HomeState
				},
				{
					"โทรสารอื่น",
					ImportContactProperties.OtherFax
				},
				{
					"ถนน 3 (ธุรกิจ)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"เมือง (บ้าน)",
					ImportContactProperties.HomeCity
				},
				{
					"จังหวัดอื่น",
					ImportContactProperties.OtherState
				},
				{
					"ถนน 2 (ธุรกิจ)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"คำนำหน้า",
					ImportContactProperties.Title
				},
				{
					"ถนน 3 (บ้าน)",
					ImportContactProperties.HomeStreet3
				},
				{
					"ที่ตั้งของสำนักงาน",
					ImportContactProperties.OfficeLocation
				},
				{
					"ชื่อ โยมิ",
					ImportContactProperties.GivenYomi
				},
				{
					"ประเทศ/ภูมิภาค (บ้าน)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"โทรศัพท์ในรถ",
					ImportContactProperties.CarPhone
				},
				{
					"บริษัท",
					ImportContactProperties.Company
				},
				{
					"ตู้ ป.ณ. ของที่อยู่ (บ้าน)",
					ImportContactProperties.HomePOBox
				},
				{
					"ตู้ ป.ณ. ของที่อยู่อื่น",
					ImportContactProperties.OtherPOBox
				},
				{
					"ถนน (บ้าน)",
					ImportContactProperties.HomeStreet
				},
				{
					"ชื่อที่ใช้แสดงของอีเมล 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"ระดับความลับ",
					ImportContactProperties.Sensitivity
				},
				{
					"บันทึกย่อ",
					ImportContactProperties.Notes
				},
				{
					"ชื่อ",
					ImportContactProperties.FirstName
				},
				{
					"ถนนอื่น",
					ImportContactProperties.OtherStreet
				},
				{
					"ชื่อกลาง",
					ImportContactProperties.MiddleName
				},
				{
					"ระยะทางที่ใช้",
					ImportContactProperties.Mileage
				},
				{
					"ผู้ใช้ 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1054));
		}

		private OutlookCsvLanguage CreateLanguageObject_tr_tr()
		{
			return new OutlookCsvLanguage(1254, new Dictionary<string, ImportContactProperties>
			{
				{
					"Elektronik Posta 2 Adres",
					ImportContactProperties.Email2Address
				},
				{
					"İş Unvanı",
					ImportContactProperties.JobTitle
				},
				{
					"Diğer-Ülke/Bölge",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Teleks",
					ImportContactProperties.Telex
				},
				{
					"Internet Serbest Meşgul",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Diğer-Cadde 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"Meslek",
					ImportContactProperties.Profession
				},
				{
					"Elektronik Posta Türü",
					ImportContactProperties.EmailType
				},
				{
					"Hesap",
					ImportContactProperties.Account
				},
				{
					"Kategoriler",
					ImportContactProperties.Categories
				},
				{
					"Doğum günü",
					ImportContactProperties.Birthday
				},
				{
					"İş Adresi Posta Kutusu",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Diğer-Cadde 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"Ulusal Kimlik No",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Soyadı",
					ImportContactProperties.LastName
				},
				{
					"Telsiz Telefon",
					ImportContactProperties.RadioPhone
				},
				{
					"Yıldönümü",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD Telefonu",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Elektronik Posta 2 Türü",
					ImportContactProperties.Email2Type
				},
				{
					"Elektronik Posta 2 Görünen Adı",
					ImportContactProperties.Email2DisplayName
				},
				{
					"İş-Posta Kodu",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Diğer Telefon No",
					ImportContactProperties.OtherPhone
				},
				{
					"Öncelik",
					ImportContactProperties.Priority
				},
				{
					"İlk Harfler",
					ImportContactProperties.Initials
				},
				{
					"Eşi",
					ImportContactProperties.Spouse
				},
				{
					"Bölüm",
					ImportContactProperties.Department
				},
				{
					"Ev Telefonu",
					ImportContactProperties.HomePhone
				},
				{
					"İş Telefon No",
					ImportContactProperties.BusinessPhone
				},
				{
					"Yomi Şirket Adı",
					ImportContactProperties.CompanyYomi
				},
				{
					"Çağrı Cihazı",
					ImportContactProperties.Pager
				},
				{
					"Diğer-Posta Kodu",
					ImportContactProperties.OtherPostalCode
				},
				{
					"İş-Bölge",
					ImportContactProperties.BusinessState
				},
				{
					"Kullanıcı 2",
					ImportContactProperties.User2
				},
				{
					"Ev Telefonu 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Cep Telefonu",
					ImportContactProperties.MobilePhone
				},
				{
					"Sonek",
					ImportContactProperties.Suffix
				},
				{
					"Hobiler",
					ImportContactProperties.Hobby
				},
				{
					"Kullanıcı 3",
					ImportContactProperties.User3
				},
				{
					"Şirket Santral Telefonu",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Çocuklar",
					ImportContactProperties.Children
				},
				{
					"İş Faks No",
					ImportContactProperties.BusinessFax
				},
				{
					"Ev-Posta Kodu",
					ImportContactProperties.HomePostalCode
				},
				{
					"Konum",
					ImportContactProperties.Location
				},
				{
					"Faturalama Bilgisi",
					ImportContactProperties.BillingInformation
				},
				{
					"Ev-Faks No",
					ImportContactProperties.HomeFax
				},
				{
					"Web Sayfası",
					ImportContactProperties.WebPage
				},
				{
					"Ev-Cadde 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"İş-Şehir",
					ImportContactProperties.BusinessCity
				},
				{
					"Diğer-Şehir",
					ImportContactProperties.OtherCity
				},
				{
					"İş-Cadde",
					ImportContactProperties.BusinessStreet
				},
				{
					"Elektronik Posta 3 Türü",
					ImportContactProperties.Email3Type
				},
				{
					"Yardımcının Adı",
					ImportContactProperties.AssistantName
				},
				{
					"Cinsiyet",
					ImportContactProperties.Gender
				},
				{
					"Yardımcının Telefonu",
					ImportContactProperties.AssistantPhone
				},
				{
					"Elektronik Posta Görünen Adı",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Elektronik Posta 3 Adres",
					ImportContactProperties.Email3Address
				},
				{
					"Yomi Soyad",
					ImportContactProperties.SurnameYomi
				},
				{
					"Kullanıcı 4",
					ImportContactProperties.User4
				},
				{
					"Yönetici'nin Adı",
					ImportContactProperties.ManagerName
				},
				{
					"İş-Ülke/Bölge",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Geri Arama",
					ImportContactProperties.Callback
				},
				{
					"Birincil Telefon",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Elektronik Posta Adresi",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"İş Telefonu 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Ev-Bölge",
					ImportContactProperties.HomeState
				},
				{
					"Diğer Faks No",
					ImportContactProperties.OtherFax
				},
				{
					"İş-Cadde 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Ev-Şehir",
					ImportContactProperties.HomeCity
				},
				{
					"Diğer-Bölge",
					ImportContactProperties.OtherState
				},
				{
					"İş-Cadde 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Unvan",
					ImportContactProperties.Title
				},
				{
					"Ev-Cadde 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"İşyeri Konumu",
					ImportContactProperties.OfficeLocation
				},
				{
					"Yomi Ad",
					ImportContactProperties.GivenYomi
				},
				{
					"Ev-Ülke/Bölge",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Araç Telefonu",
					ImportContactProperties.CarPhone
				},
				{
					"Şirket",
					ImportContactProperties.Company
				},
				{
					"Ev Adresi Posta Kutusu",
					ImportContactProperties.HomePOBox
				},
				{
					"Diğer Adres Posta Kutusu",
					ImportContactProperties.OtherPOBox
				},
				{
					"Ev-Cadde",
					ImportContactProperties.HomeStreet
				},
				{
					"Elektronik Posta 3 Görünen Adı",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Duyarlılık",
					ImportContactProperties.Sensitivity
				},
				{
					"Notlar",
					ImportContactProperties.Notes
				},
				{
					"İlk Adı",
					ImportContactProperties.FirstName
				},
				{
					"Diğer-Cadde",
					ImportContactProperties.OtherStreet
				},
				{
					"İkinci Adı",
					ImportContactProperties.MiddleName
				},
				{
					"Dönem",
					ImportContactProperties.Mileage
				},
				{
					"Kullanıcı 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1055));
		}

		private OutlookCsvLanguage CreateLanguageObject_uk_ua()
		{
			return new OutlookCsvLanguage(1251, new Dictionary<string, ImportContactProperties>
			{
				{
					"Адреса 2 ел. пошти",
					ImportContactProperties.Email2Address
				},
				{
					"Посада",
					ImportContactProperties.JobTitle
				},
				{
					"Країна або регіон (інша адреса)",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Телекс",
					ImportContactProperties.Telex
				},
				{
					"Відомості про доступність в Інтернеті",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"Вулиця 2 (інша адреса)",
					ImportContactProperties.OtherStreet2
				},
				{
					"Професія",
					ImportContactProperties.Profession
				},
				{
					"Тип ел. пошти",
					ImportContactProperties.EmailType
				},
				{
					"Рахунок",
					ImportContactProperties.Account
				},
				{
					"Категорії",
					ImportContactProperties.Categories
				},
				{
					"День народження",
					ImportContactProperties.Birthday
				},
				{
					"Поштова скринька (роб. адреса)",
					ImportContactProperties.BusinessPOBox
				},
				{
					"Вулиця 3 (інша адреса)",
					ImportContactProperties.OtherStreet3
				},
				{
					"Особистий код",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"Прізвище",
					ImportContactProperties.LastName
				},
				{
					"Радіотелефон",
					ImportContactProperties.RadioPhone
				},
				{
					"Річниця",
					ImportContactProperties.Anniversary
				},
				{
					"Телефон TTY/TDD",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"Тип 2 ел. пошти",
					ImportContactProperties.Email2Type
				},
				{
					"Коротке ім'я ел. пошти 2",
					ImportContactProperties.Email2DisplayName
				},
				{
					"Індекс (роб. адреса)",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"Інший телефон",
					ImportContactProperties.OtherPhone
				},
				{
					"Важливість",
					ImportContactProperties.Priority
				},
				{
					"Ініціали",
					ImportContactProperties.Initials
				},
				{
					"Дружина",
					ImportContactProperties.Spouse
				},
				{
					"Відділ",
					ImportContactProperties.Department
				},
				{
					"Домашній телефон",
					ImportContactProperties.HomePhone
				},
				{
					"Робочий телефон",
					ImportContactProperties.BusinessPhone
				},
				{
					"Company Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"Пейджер",
					ImportContactProperties.Pager
				},
				{
					"Індекс (інша адреса)",
					ImportContactProperties.OtherPostalCode
				},
				{
					"Область (роб. адреса)",
					ImportContactProperties.BusinessState
				},
				{
					"Користувач 2",
					ImportContactProperties.User2
				},
				{
					"Телефон дом. 2",
					ImportContactProperties.HomePhone2
				},
				{
					"Телефон мобільний",
					ImportContactProperties.MobilePhone
				},
				{
					"Суфікс",
					ImportContactProperties.Suffix
				},
				{
					"Хобі",
					ImportContactProperties.Hobby
				},
				{
					"Користувач 3",
					ImportContactProperties.User3
				},
				{
					"Основний телефон організації",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"Діти",
					ImportContactProperties.Children
				},
				{
					"Робочий факс",
					ImportContactProperties.BusinessFax
				},
				{
					"Поштовий код (дом.)",
					ImportContactProperties.HomePostalCode
				},
				{
					"Розташування",
					ImportContactProperties.Location
				},
				{
					"Рахунки",
					ImportContactProperties.BillingInformation
				},
				{
					"Домашній факс",
					ImportContactProperties.HomeFax
				},
				{
					"Веб-сторінка",
					ImportContactProperties.WebPage
				},
				{
					"Вулиця 2 (дом. адреса)",
					ImportContactProperties.HomeStreet2
				},
				{
					"Місто (роб. адреса)",
					ImportContactProperties.BusinessCity
				},
				{
					"Місто (інша адреса)",
					ImportContactProperties.OtherCity
				},
				{
					"Вулиця (роб. адреса)",
					ImportContactProperties.BusinessStreet
				},
				{
					"Тип 3 ел. пошти",
					ImportContactProperties.Email3Type
				},
				{
					"Ім'я помічника",
					ImportContactProperties.AssistantName
				},
				{
					"Стать",
					ImportContactProperties.Gender
				},
				{
					"Телефон помічника",
					ImportContactProperties.AssistantPhone
				},
				{
					"Коротке ім'я ел. пошти",
					ImportContactProperties.EmailDisplayName
				},
				{
					"Адреса 3 ел. пошти",
					ImportContactProperties.Email3Address
				},
				{
					"Surname Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"Користувач 4",
					ImportContactProperties.User4
				},
				{
					"Керівник",
					ImportContactProperties.ManagerName
				},
				{
					"Країна або регіон (роб. адреса)",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"Зворотний виклик",
					ImportContactProperties.Callback
				},
				{
					"Основний телефон",
					ImportContactProperties.PrimaryPhone
				},
				{
					"Адреса ел. пошти",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"Телефон роб. 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"Область (дом. адреса)",
					ImportContactProperties.HomeState
				},
				{
					"Інший факс",
					ImportContactProperties.OtherFax
				},
				{
					"Вулиця 3 (роб. адреса)",
					ImportContactProperties.BusinessStreet3
				},
				{
					"Місто (дом. адреса)",
					ImportContactProperties.HomeCity
				},
				{
					"Область (інша адреса)",
					ImportContactProperties.OtherState
				},
				{
					"Вулиця 2 (роб. адреса)",
					ImportContactProperties.BusinessStreet2
				},
				{
					"Звернення",
					ImportContactProperties.Title
				},
				{
					"Вулиця 3 (дом. адреса)",
					ImportContactProperties.HomeStreet3
				},
				{
					"Розташування кімнати",
					ImportContactProperties.OfficeLocation
				},
				{
					"Given Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"Країна або регіон (дом. адреса)",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"Телефон у машині",
					ImportContactProperties.CarPhone
				},
				{
					"Організація",
					ImportContactProperties.Company
				},
				{
					"Поштова скринька (дом. адреса)",
					ImportContactProperties.HomePOBox
				},
				{
					"Поштова скринька (інша адреса)",
					ImportContactProperties.OtherPOBox
				},
				{
					"Вулиця (дом. адреса)",
					ImportContactProperties.HomeStreet
				},
				{
					"Коротке ім'я ел. пошти 3",
					ImportContactProperties.Email3DisplayName
				},
				{
					"Приватність",
					ImportContactProperties.Sensitivity
				},
				{
					"Нотатки",
					ImportContactProperties.Notes
				},
				{
					"Ім'я",
					ImportContactProperties.FirstName
				},
				{
					"Вулиця (інша адреса)",
					ImportContactProperties.OtherStreet
				},
				{
					"По батькові",
					ImportContactProperties.MiddleName
				},
				{
					"Відстань",
					ImportContactProperties.Mileage
				},
				{
					"Користувач 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1058));
		}

		private OutlookCsvLanguage CreateLanguageObject_zh_cn()
		{
			return new OutlookCsvLanguage(936, new Dictionary<string, ImportContactProperties>
			{
				{
					"电子邮件 2 地址",
					ImportContactProperties.Email2Address
				},
				{
					"职务",
					ImportContactProperties.JobTitle
				},
				{
					"其他地址 国家/地区",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"电报",
					ImportContactProperties.Telex
				},
				{
					"Internet 忙闲",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"其他地址 街道 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"职业",
					ImportContactProperties.Profession
				},
				{
					"电子邮件类型",
					ImportContactProperties.EmailType
				},
				{
					"帐户",
					ImportContactProperties.Account
				},
				{
					"类别",
					ImportContactProperties.Categories
				},
				{
					"生日",
					ImportContactProperties.Birthday
				},
				{
					"商务地址 - 邮箱",
					ImportContactProperties.BusinessPOBox
				},
				{
					"其他地址 街道 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"身份证编号",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"姓",
					ImportContactProperties.LastName
				},
				{
					"无绳电话",
					ImportContactProperties.RadioPhone
				},
				{
					"纪念日",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD 电话",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"电子邮件 2 类型",
					ImportContactProperties.Email2Type
				},
				{
					"电子邮件 2 显示名",
					ImportContactProperties.Email2DisplayName
				},
				{
					"商务地址 邮政编码",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"其他电话",
					ImportContactProperties.OtherPhone
				},
				{
					"优先级",
					ImportContactProperties.Priority
				},
				{
					"缩写",
					ImportContactProperties.Initials
				},
				{
					"配偶",
					ImportContactProperties.Spouse
				},
				{
					"部门",
					ImportContactProperties.Department
				},
				{
					"住宅电话",
					ImportContactProperties.HomePhone
				},
				{
					"商务电话",
					ImportContactProperties.BusinessPhone
				},
				{
					"公司 Yomi",
					ImportContactProperties.CompanyYomi
				},
				{
					"寻呼机",
					ImportContactProperties.Pager
				},
				{
					"其他地址 邮政编码",
					ImportContactProperties.OtherPostalCode
				},
				{
					"商务地址 省/市/自治区",
					ImportContactProperties.BusinessState
				},
				{
					"用户 2",
					ImportContactProperties.User2
				},
				{
					"住宅电话 2",
					ImportContactProperties.HomePhone2
				},
				{
					"移动电话",
					ImportContactProperties.MobilePhone
				},
				{
					"中文称谓",
					ImportContactProperties.Suffix
				},
				{
					"业余爱好",
					ImportContactProperties.Hobby
				},
				{
					"用户 3",
					ImportContactProperties.User3
				},
				{
					"单位主要电话",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"子女",
					ImportContactProperties.Children
				},
				{
					"商务传真",
					ImportContactProperties.BusinessFax
				},
				{
					"住宅地址 邮政编码",
					ImportContactProperties.HomePostalCode
				},
				{
					"地点",
					ImportContactProperties.Location
				},
				{
					"记帐信息",
					ImportContactProperties.BillingInformation
				},
				{
					"住宅传真",
					ImportContactProperties.HomeFax
				},
				{
					"网页",
					ImportContactProperties.WebPage
				},
				{
					"住宅地址 街道 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"商务地址 市/县",
					ImportContactProperties.BusinessCity
				},
				{
					"其他地址 市/县",
					ImportContactProperties.OtherCity
				},
				{
					"商务地址 街道",
					ImportContactProperties.BusinessStreet
				},
				{
					"电子邮件 3 类型",
					ImportContactProperties.Email3Type
				},
				{
					"助理的姓名",
					ImportContactProperties.AssistantName
				},
				{
					"性别",
					ImportContactProperties.Gender
				},
				{
					"助理的电话",
					ImportContactProperties.AssistantPhone
				},
				{
					"电子邮件显示名称",
					ImportContactProperties.EmailDisplayName
				},
				{
					"电子邮件 3 地址",
					ImportContactProperties.Email3Address
				},
				{
					"姓氏 Yomi",
					ImportContactProperties.SurnameYomi
				},
				{
					"用户 4",
					ImportContactProperties.User4
				},
				{
					"经理姓名",
					ImportContactProperties.ManagerName
				},
				{
					"商务地址 国家/地区",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"回电话",
					ImportContactProperties.Callback
				},
				{
					"主要电话",
					ImportContactProperties.PrimaryPhone
				},
				{
					"电子邮件地址",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"商务电话 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"住宅地址 省/市/自治区",
					ImportContactProperties.HomeState
				},
				{
					"其他传真",
					ImportContactProperties.OtherFax
				},
				{
					"商务地址 街道 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"住宅地址 市/县",
					ImportContactProperties.HomeCity
				},
				{
					"其他地址 省/市/自治区",
					ImportContactProperties.OtherState
				},
				{
					"商务地址 街道 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"英文称谓",
					ImportContactProperties.Title
				},
				{
					"住宅地址 街道 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"办公地点",
					ImportContactProperties.OfficeLocation
				},
				{
					"给定 Yomi",
					ImportContactProperties.GivenYomi
				},
				{
					"住宅地址 国家/地区",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"车载电话",
					ImportContactProperties.CarPhone
				},
				{
					"单位",
					ImportContactProperties.Company
				},
				{
					"住宅地址 - 邮箱",
					ImportContactProperties.HomePOBox
				},
				{
					"其他地址 - 邮箱",
					ImportContactProperties.OtherPOBox
				},
				{
					"住宅地址 街道",
					ImportContactProperties.HomeStreet
				},
				{
					"电子邮件 3 显示名",
					ImportContactProperties.Email3DisplayName
				},
				{
					"敏感度",
					ImportContactProperties.Sensitivity
				},
				{
					"附注",
					ImportContactProperties.Notes
				},
				{
					"名",
					ImportContactProperties.FirstName
				},
				{
					"其他地址 街道",
					ImportContactProperties.OtherStreet
				},
				{
					"中间名",
					ImportContactProperties.MiddleName
				},
				{
					"里程",
					ImportContactProperties.Mileage
				},
				{
					"用户 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(2052));
		}

		private OutlookCsvLanguage CreateLanguageObject_zh_tw()
		{
			return new OutlookCsvLanguage(950, new Dictionary<string, ImportContactProperties>
			{
				{
					"電子郵件 2 地址",
					ImportContactProperties.Email2Address
				},
				{
					"職稱",
					ImportContactProperties.JobTitle
				},
				{
					"其他 - 國家/地區",
					ImportContactProperties.OtherCountryOrRegion
				},
				{
					"Telix",
					ImportContactProperties.Telex
				},
				{
					"網際網路空閒-忙碌中",
					ImportContactProperties.InternetFreeBusy
				},
				{
					"其他 - 街 2",
					ImportContactProperties.OtherStreet2
				},
				{
					"專業",
					ImportContactProperties.Profession
				},
				{
					"電子郵件類型",
					ImportContactProperties.EmailType
				},
				{
					"帳戶",
					ImportContactProperties.Account
				},
				{
					"類別",
					ImportContactProperties.Categories
				},
				{
					"生日",
					ImportContactProperties.Birthday
				},
				{
					"公司地址郵政信箱",
					ImportContactProperties.BusinessPOBox
				},
				{
					"其他 - 街 3",
					ImportContactProperties.OtherStreet3
				},
				{
					"身份證字號",
					ImportContactProperties.GovernmentIDNumber
				},
				{
					"姓氏",
					ImportContactProperties.LastName
				},
				{
					"無線電話",
					ImportContactProperties.RadioPhone
				},
				{
					"紀念日",
					ImportContactProperties.Anniversary
				},
				{
					"TTY/TDD 電話",
					ImportContactProperties.TTYOrTDDPhone
				},
				{
					"電子郵件 2 類型",
					ImportContactProperties.Email2Type
				},
				{
					"電子郵件 2 顯示名稱",
					ImportContactProperties.Email2DisplayName
				},
				{
					"商務 - 郵遞區號",
					ImportContactProperties.BusinessPostalCode
				},
				{
					"其他電話",
					ImportContactProperties.OtherPhone
				},
				{
					"優先順序",
					ImportContactProperties.Priority
				},
				{
					"縮寫",
					ImportContactProperties.Initials
				},
				{
					"配偶",
					ImportContactProperties.Spouse
				},
				{
					"部門",
					ImportContactProperties.Department
				},
				{
					"住家電話",
					ImportContactProperties.HomePhone
				},
				{
					"商務電話",
					ImportContactProperties.BusinessPhone
				},
				{
					"公司拼音",
					ImportContactProperties.CompanyYomi
				},
				{
					"呼叫器",
					ImportContactProperties.Pager
				},
				{
					"其他 - 郵遞區號",
					ImportContactProperties.OtherPostalCode
				},
				{
					"商務 - 縣/市",
					ImportContactProperties.BusinessState
				},
				{
					"使用者 2",
					ImportContactProperties.User2
				},
				{
					"住家電話 2",
					ImportContactProperties.HomePhone2
				},
				{
					"行動電話",
					ImportContactProperties.MobilePhone
				},
				{
					"稱謂",
					ImportContactProperties.Suffix
				},
				{
					"嗜好",
					ImportContactProperties.Hobby
				},
				{
					"使用者 3",
					ImportContactProperties.User3
				},
				{
					"公司代表線",
					ImportContactProperties.CompanyMainPhone
				},
				{
					"子女",
					ImportContactProperties.Children
				},
				{
					"商務傳真",
					ImportContactProperties.BusinessFax
				},
				{
					"住家 - 郵遞區號",
					ImportContactProperties.HomePostalCode
				},
				{
					"地點",
					ImportContactProperties.Location
				},
				{
					"帳目資訊",
					ImportContactProperties.BillingInformation
				},
				{
					"住家傳真",
					ImportContactProperties.HomeFax
				},
				{
					"網頁",
					ImportContactProperties.WebPage
				},
				{
					"住家 - 街 2",
					ImportContactProperties.HomeStreet2
				},
				{
					"商務 - 市/鎮",
					ImportContactProperties.BusinessCity
				},
				{
					"其他 - 市/鎮",
					ImportContactProperties.OtherCity
				},
				{
					"商務 - 街",
					ImportContactProperties.BusinessStreet
				},
				{
					"電子郵件 3 類型",
					ImportContactProperties.Email3Type
				},
				{
					"助理",
					ImportContactProperties.AssistantName
				},
				{
					"性別",
					ImportContactProperties.Gender
				},
				{
					"助理電話",
					ImportContactProperties.AssistantPhone
				},
				{
					"電子郵件顯示名稱",
					ImportContactProperties.EmailDisplayName
				},
				{
					"電子郵件 3 地址",
					ImportContactProperties.Email3Address
				},
				{
					"姓氏拼音",
					ImportContactProperties.SurnameYomi
				},
				{
					"使用者 4",
					ImportContactProperties.User4
				},
				{
					"主管名稱",
					ImportContactProperties.ManagerName
				},
				{
					"商務 - 國家/地區",
					ImportContactProperties.BusinessCountryOrRegion
				},
				{
					"回撥電話",
					ImportContactProperties.Callback
				},
				{
					"代表電話",
					ImportContactProperties.PrimaryPhone
				},
				{
					"電子郵件地址",
					ImportContactProperties.EmailAddress
				},
				{
					"ISDN",
					ImportContactProperties.ISDN
				},
				{
					"商務電話 2",
					ImportContactProperties.BusinessPhone2
				},
				{
					"住家 - 縣/市",
					ImportContactProperties.HomeState
				},
				{
					"其他傳真",
					ImportContactProperties.OtherFax
				},
				{
					"商務 - 街 3",
					ImportContactProperties.BusinessStreet3
				},
				{
					"住家 - 市/鎮",
					ImportContactProperties.HomeCity
				},
				{
					"其他 - 縣/市",
					ImportContactProperties.OtherState
				},
				{
					"商務 - 街 2",
					ImportContactProperties.BusinessStreet2
				},
				{
					"頭銜",
					ImportContactProperties.Title
				},
				{
					"住家 - 街 3",
					ImportContactProperties.HomeStreet3
				},
				{
					"辦公室",
					ImportContactProperties.OfficeLocation
				},
				{
					"名字拼音",
					ImportContactProperties.GivenYomi
				},
				{
					"住家 - 國家/地區",
					ImportContactProperties.HomeCountryOrRegion
				},
				{
					"汽車電話",
					ImportContactProperties.CarPhone
				},
				{
					"公司",
					ImportContactProperties.Company
				},
				{
					"住家地址郵政信箱",
					ImportContactProperties.HomePOBox
				},
				{
					"其他地址郵政信箱",
					ImportContactProperties.OtherPOBox
				},
				{
					"住家 - 街",
					ImportContactProperties.HomeStreet
				},
				{
					"電子郵件 3 顯示名稱",
					ImportContactProperties.Email3DisplayName
				},
				{
					"敏感度",
					ImportContactProperties.Sensitivity
				},
				{
					"記事",
					ImportContactProperties.Notes
				},
				{
					"名字",
					ImportContactProperties.FirstName
				},
				{
					"其他 - 街",
					ImportContactProperties.OtherStreet
				},
				{
					"中間名",
					ImportContactProperties.MiddleName
				},
				{
					"津貼",
					ImportContactProperties.Mileage
				},
				{
					"使用者 1",
					ImportContactProperties.User1
				}
			}, new CultureInfo(1028));
		}

		private OutlookCsvLanguageSelect()
		{
			this.englishUs = this.CreateLanguageObject_en_us();
			this.allLanguages = new List<OutlookCsvLanguage>
			{
				this.CreateLanguageObject_ar_sa(),
				this.CreateLanguageObject_bg_bg(),
				this.CreateLanguageObject_cs_cz(),
				this.CreateLanguageObject_da_dk(),
				this.CreateLanguageObject_de_de(),
				this.CreateLanguageObject_el_gr(),
				this.englishUs,
				this.CreateLanguageObject_es_es(),
				this.CreateLanguageObject_et_ee(),
				this.CreateLanguageObject_fi_fi(),
				this.CreateLanguageObject_fr_fr(),
				this.CreateLanguageObject_he_il(),
				this.CreateLanguageObject_hr_hr(),
				this.CreateLanguageObject_hu_hu(),
				this.CreateLanguageObject_it_it(),
				this.CreateLanguageObject_ja_jp(),
				this.CreateLanguageObject_ko_kr(),
				this.CreateLanguageObject_lt_lt(),
				this.CreateLanguageObject_lv_lv(),
				this.CreateLanguageObject_nl_nl(),
				this.CreateLanguageObject_nb_no(),
				this.CreateLanguageObject_pl_pl(),
				this.CreateLanguageObject_pt_br(),
				this.CreateLanguageObject_pt_pt(),
				this.CreateLanguageObject_ro_ro(),
				this.CreateLanguageObject_ru_ru(),
				this.CreateLanguageObject_sk_sk(),
				this.CreateLanguageObject_sl_si(),
				this.CreateLanguageObject_sr_sp(),
				this.CreateLanguageObject_sv_se(),
				this.CreateLanguageObject_th_th(),
				this.CreateLanguageObject_tr_tr(),
				this.CreateLanguageObject_uk_ua(),
				this.CreateLanguageObject_zh_cn(),
				this.CreateLanguageObject_zh_tw()
			};
		}

		public static OutlookCsvLanguageSelect Instance
		{
			get
			{
				if (OutlookCsvLanguageSelect.instance == null)
				{
					OutlookCsvLanguageSelect.instance = new OutlookCsvLanguageSelect();
				}
				return OutlookCsvLanguageSelect.instance;
			}
		}

		public OutlookCsvLanguage EnglishUS
		{
			get
			{
				return this.englishUs;
			}
		}

		public List<OutlookCsvLanguage> AllLanguages
		{
			get
			{
				return this.allLanguages;
			}
		}

		public List<OutlookCsvLanguage> GetLanguagesForCodepage(int codepage)
		{
			if (codepage == 1200)
			{
				return this.allLanguages;
			}
			int capacity = 10;
			List<OutlookCsvLanguage> list = new List<OutlookCsvLanguage>(capacity);
			foreach (OutlookCsvLanguage outlookCsvLanguage in this.allLanguages)
			{
				if (outlookCsvLanguage.CodePage == codepage)
				{
					list.Add(outlookCsvLanguage);
				}
			}
			return list;
		}

		private const int WindowsUnicodeCodepage = 1200;

		private static OutlookCsvLanguageSelect instance;

		private OutlookCsvLanguage englishUs;

		private List<OutlookCsvLanguage> allLanguages;
	}
}
