using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	public static class RMSvcAgentStrings
	{
		static RMSvcAgentStrings()
		{
			RMSvcAgentStrings.stringIDs.Add(2348409902U, "FailedToDetectMultiTenancy");
			RMSvcAgentStrings.stringIDs.Add(670208566U, "RmsAdName");
		}

		public static LocalizedString RmsErrorTextFor401(string tenantId, string componentName, string objectName, string rmsName, string status)
		{
			return new LocalizedString("RmsErrorTextFor401", "Ex9D8436", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				status
			});
		}

		public static LocalizedString RmsErrorTextFor403(string tenantId, string componentName, string objectName, string rmsName, string status)
		{
			return new LocalizedString("RmsErrorTextFor403", "Ex554C43", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				status
			});
		}

		public static LocalizedString RmsErrorTextForNoServerPL(string messageId)
		{
			return new LocalizedString("RmsErrorTextForNoServerPL", "Ex5D5C04", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString RmsErrorTextForDefer(string messageId)
		{
			return new LocalizedString("RmsErrorTextForDefer", "Ex72ECEA", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString RmsErrorTextForNDR(string messageId)
		{
			return new LocalizedString("RmsErrorTextForNDR", "Ex8816EC", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString TemplateDoesNotExist(Guid templateId)
		{
			return new LocalizedString("TemplateDoesNotExist", "Ex028EEA", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				templateId
			});
		}

		public static LocalizedString FailedToDetectMultiTenancy
		{
			get
			{
				return new LocalizedString("FailedToDetectMultiTenancy", "", false, false, RMSvcAgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RmsErrorTextForNoPL(string messageId)
		{
			return new LocalizedString("RmsErrorTextForNoPL", "ExE9F0BF", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString RmsErrorTextForConnectFailure(string tenantId, string componentName, string objectName, string rmsName, string status)
		{
			return new LocalizedString("RmsErrorTextForConnectFailure", "Ex1B2160", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				status
			});
		}

		public static LocalizedString RmsAdName
		{
			get
			{
				return new LocalizedString("RmsAdName", "Ex048E70", false, true, RMSvcAgentStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RmsErrorTextFor404(string tenantId, string componentName, string objectName, string rmsName, string status)
		{
			return new LocalizedString("RmsErrorTextFor404", "ExB39734", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				status
			});
		}

		public static LocalizedString RmsErrorTextForNoRightException(string tenantId, string componentName, string objectName, string rmsName)
		{
			return new LocalizedString("RmsErrorTextForNoRightException", "Ex899654", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName
			});
		}

		public static LocalizedString RmsErrorTextForDeferJR(string messageId)
		{
			return new LocalizedString("RmsErrorTextForDeferJR", "Ex62AD80", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString RmsErrorTextForNoJR(string messageId)
		{
			return new LocalizedString("RmsErrorTextForNoJR", "ExA1F2BE", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				messageId
			});
		}

		public static LocalizedString RmsErrorTextForGeneralException(string tenantId, string componentName, string exceptionName)
		{
			return new LocalizedString("RmsErrorTextForGeneralException", "Ex58C567", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				exceptionName
			});
		}

		public static LocalizedString RmsErrorTextForTrustFailure(string tenantId, string componentName, string objectName, string rmsName, string status)
		{
			return new LocalizedString("RmsErrorTextForTrustFailure", "Ex2C5B99", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				status
			});
		}

		public static LocalizedString RmsErrorTextForSpecialException(string tenantId, string componentName, string objectName, string rmsName, string exceptionName)
		{
			return new LocalizedString("RmsErrorTextForSpecialException", "Ex28AEC3", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId,
				componentName,
				objectName,
				rmsName,
				exceptionName
			});
		}

		public static LocalizedString FailedToAcquireTenantLicenses(Guid tenantId)
		{
			return new LocalizedString("FailedToAcquireTenantLicenses", "ExEEC3BC", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString RmsAdNameWithUrl(string url)
		{
			return new LocalizedString("RmsAdNameWithUrl", "ExE24BDD", false, true, RMSvcAgentStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString GetLocalizedString(RMSvcAgentStrings.IDs key)
		{
			return new LocalizedString(RMSvcAgentStrings.stringIDs[(uint)key], RMSvcAgentStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MessagingPolicies.RmSvcAgent.RMSvcAgentStrings", typeof(RMSvcAgentStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			FailedToDetectMultiTenancy = 2348409902U,
			RmsAdName = 670208566U
		}

		private enum ParamIDs
		{
			RmsErrorTextFor401,
			RmsErrorTextFor403,
			RmsErrorTextForNoServerPL,
			RmsErrorTextForDefer,
			RmsErrorTextForNDR,
			TemplateDoesNotExist,
			RmsErrorTextForNoPL,
			RmsErrorTextForConnectFailure,
			RmsErrorTextFor404,
			RmsErrorTextForNoRightException,
			RmsErrorTextForDeferJR,
			RmsErrorTextForNoJR,
			RmsErrorTextForGeneralException,
			RmsErrorTextForTrustFailure,
			RmsErrorTextForSpecialException,
			FailedToAcquireTenantLicenses,
			RmsAdNameWithUrl
		}
	}
}
