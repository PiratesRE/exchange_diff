using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.Provisioning
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1246705638U, "LabelLogMailFooter");
			Strings.stringIDs.Add(1416516349U, "UnknownProvisioningType");
			Strings.stringIDs.Add(2604316530U, "PermissionDenied");
			Strings.stringIDs.Add(2795084079U, "LabelStartTime");
			Strings.stringIDs.Add(3123046798U, "LabelReportMailErrorHeaderCancel");
			Strings.stringIDs.Add(886329592U, "ReportSubject");
			Strings.stringIDs.Add(1921063387U, "ErrorReportFileName");
			Strings.stringIDs.Add(2079790351U, "LabelTotalRowsProcessed");
			Strings.stringIDs.Add(246748372U, "LabelReportMailErrorHeader");
			Strings.stringIDs.Add(283214974U, "LabelNewUsers");
			Strings.stringIDs.Add(345539229U, "LabelErrors");
			Strings.stringIDs.Add(3889683156U, "UnknownProvisioningOwner");
			Strings.stringIDs.Add(3917832382U, "LabelReportMailHeaderCancel");
			Strings.stringIDs.Add(3597640497U, "ReportSubjectCanceled");
			Strings.stringIDs.Add(3346179848U, "LabelStartedBy");
			Strings.stringIDs.Add(2060675488U, "LabelRunTime");
			Strings.stringIDs.Add(1144436407U, "LabelFileName");
			Strings.stringIDs.Add(2937969650U, "HelpText");
			Strings.stringIDs.Add(3449564964U, "LabelReportMailHeader");
			Strings.stringIDs.Add(264350496U, "ErrorSummary");
			Strings.stringIDs.Add(5503809U, "LabelTotalRows");
			Strings.stringIDs.Add(2821624239U, "HelpURLText");
		}

		public static LocalizedString LabelLogMailFooter
		{
			get
			{
				return new LocalizedString("LabelLogMailFooter", "Ex333202", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownProvisioningType
		{
			get
			{
				return new LocalizedString("UnknownProvisioningType", "Ex5FE00C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PermissionDenied
		{
			get
			{
				return new LocalizedString("PermissionDenied", "Ex431543", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelStartTime
		{
			get
			{
				return new LocalizedString("LabelStartTime", "Ex301A0B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UsedEmailAddress(string email, string otherUserID)
		{
			return new LocalizedString("UsedEmailAddress", "ExD52E47", false, true, Strings.ResourceManager, new object[]
			{
				email,
				otherUserID
			});
		}

		public static LocalizedString LabelReportMailErrorHeaderCancel
		{
			get
			{
				return new LocalizedString("LabelReportMailErrorHeaderCancel", "Ex5BDA91", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSubject
		{
			get
			{
				return new LocalizedString("ReportSubject", "ExB47335", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReportFileName
		{
			get
			{
				return new LocalizedString("ErrorReportFileName", "Ex098155", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelTotalRowsProcessed
		{
			get
			{
				return new LocalizedString("LabelTotalRowsProcessed", "Ex1FE283", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelReportMailErrorHeader
		{
			get
			{
				return new LocalizedString("LabelReportMailErrorHeader", "ExAEEA91", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelNewUsers
		{
			get
			{
				return new LocalizedString("LabelNewUsers", "Ex097570", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelErrors
		{
			get
			{
				return new LocalizedString("LabelErrors", "ExF637C8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownProvisioningOwner
		{
			get
			{
				return new LocalizedString("UnknownProvisioningOwner", "Ex6A6645", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelReportMailHeaderCancel
		{
			get
			{
				return new LocalizedString("LabelReportMailHeaderCancel", "Ex6F67EB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReportSubjectCanceled
		{
			get
			{
				return new LocalizedString("ReportSubjectCanceled", "ExAC7874", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelStartedBy
		{
			get
			{
				return new LocalizedString("LabelStartedBy", "ExC5B354", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunTimeFormatMinutes(int minutes)
		{
			return new LocalizedString("RunTimeFormatMinutes", "ExB9F67C", false, true, Strings.ResourceManager, new object[]
			{
				minutes
			});
		}

		public static LocalizedString LabelRunTime
		{
			get
			{
				return new LocalizedString("LabelRunTime", "Ex866C0D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RunTimeFormatDays(int days, int hours, int minutes)
		{
			return new LocalizedString("RunTimeFormatDays", "ExE0E5DB", false, true, Strings.ResourceManager, new object[]
			{
				days,
				hours,
				minutes
			});
		}

		public static LocalizedString FailedToUpdateProperty(string errorDetails)
		{
			return new LocalizedString("FailedToUpdateProperty", "Ex5800C0", false, true, Strings.ResourceManager, new object[]
			{
				errorDetails
			});
		}

		public static LocalizedString FailedToUpdateDistributionGroupMember(string errorDetails)
		{
			return new LocalizedString("FailedToUpdateDistributionGroupMember", "Ex5B2C5D", false, true, Strings.ResourceManager, new object[]
			{
				errorDetails
			});
		}

		public static LocalizedString LabelFileName
		{
			get
			{
				return new LocalizedString("LabelFileName", "Ex9557FE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString HelpText
		{
			get
			{
				return new LocalizedString("HelpText", "Ex7D046E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LabelReportMailHeader
		{
			get
			{
				return new LocalizedString("LabelReportMailHeader", "ExEAE37B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSummary
		{
			get
			{
				return new LocalizedString("ErrorSummary", "Ex5E3554", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessagePlural(int errorCount)
		{
			return new LocalizedString("ErrorMessagePlural", "Ex1CBEAA", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString RunTimeFormatHours(int hours, int minutes)
		{
			return new LocalizedString("RunTimeFormatHours", "ExDDC7DA", false, true, Strings.ResourceManager, new object[]
			{
				hours,
				minutes
			});
		}

		public static LocalizedString LabelTotalRows
		{
			get
			{
				return new LocalizedString("LabelTotalRows", "ExE08087", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageSingular(int errorCount)
		{
			return new LocalizedString("ErrorMessageSingular", "Ex93FA17", false, true, Strings.ResourceManager, new object[]
			{
				errorCount
			});
		}

		public static LocalizedString HelpURLText
		{
			get
			{
				return new LocalizedString("HelpURLText", "Ex0EF214", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(22);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.BulkProvisioning.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			LabelLogMailFooter = 1246705638U,
			UnknownProvisioningType = 1416516349U,
			PermissionDenied = 2604316530U,
			LabelStartTime = 2795084079U,
			LabelReportMailErrorHeaderCancel = 3123046798U,
			ReportSubject = 886329592U,
			ErrorReportFileName = 1921063387U,
			LabelTotalRowsProcessed = 2079790351U,
			LabelReportMailErrorHeader = 246748372U,
			LabelNewUsers = 283214974U,
			LabelErrors = 345539229U,
			UnknownProvisioningOwner = 3889683156U,
			LabelReportMailHeaderCancel = 3917832382U,
			ReportSubjectCanceled = 3597640497U,
			LabelStartedBy = 3346179848U,
			LabelRunTime = 2060675488U,
			LabelFileName = 1144436407U,
			HelpText = 2937969650U,
			LabelReportMailHeader = 3449564964U,
			ErrorSummary = 264350496U,
			LabelTotalRows = 5503809U,
			HelpURLText = 2821624239U
		}

		private enum ParamIDs
		{
			UsedEmailAddress,
			RunTimeFormatMinutes,
			RunTimeFormatDays,
			FailedToUpdateProperty,
			FailedToUpdateDistributionGroupMember,
			ErrorMessagePlural,
			RunTimeFormatHours,
			ErrorMessageSingular
		}
	}
}
