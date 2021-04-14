using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UpgradeHandlerStrings
	{
		static UpgradeHandlerStrings()
		{
			UpgradeHandlerStrings.stringIDs.Add(3297992468U, "AnchorServiceInstanceNotActive");
			UpgradeHandlerStrings.stringIDs.Add(3742916171U, "TooManyPilotMailboxes");
			UpgradeHandlerStrings.stringIDs.Add(3847819607U, "InvalidE14Mailboxes");
			UpgradeHandlerStrings.stringIDs.Add(31377678U, "ErrorQueryingWorkItem");
			UpgradeHandlerStrings.stringIDs.Add(2176982493U, "ErrorNoE14ServersFound");
			UpgradeHandlerStrings.stringIDs.Add(683139314U, "InvalidE15Mailboxes");
		}

		public static LocalizedString OrganizationHasConstraints(UpgradeRequestTypes requestedType, string orgId, string orgName, string constraints)
		{
			return new LocalizedString("OrganizationHasConstraints", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				requestedType,
				orgId,
				orgName,
				constraints
			});
		}

		public static LocalizedString InvalidRequestedType(string orgId, UpgradeRequestTypes currentType, string requestedType)
		{
			return new LocalizedString("InvalidRequestedType", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				orgId,
				currentType,
				requestedType
			});
		}

		public static LocalizedString UnsupportedUpgradeRequestType(UpgradeRequestTypes upgradeRequest)
		{
			return new LocalizedString("UnsupportedUpgradeRequestType", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				upgradeRequest
			});
		}

		public static LocalizedString InvalidOrganizationVersion(string org, ExchangeObjectVersion version)
		{
			return new LocalizedString("InvalidOrganizationVersion", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				org,
				version
			});
		}

		public static LocalizedString SymphonyFault(string faultMessage)
		{
			return new LocalizedString("SymphonyFault", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				faultMessage
			});
		}

		public static LocalizedString InvalidOrganizationState(string org, string servicePlan, ExchangeObjectVersion version, bool isUpgrading, bool isPiloting, bool isUpgradeInProgress)
		{
			return new LocalizedString("InvalidOrganizationState", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				org,
				servicePlan,
				version,
				isUpgrading,
				isPiloting,
				isUpgradeInProgress
			});
		}

		public static LocalizedString OrganizationInDryRunMode(string tenant, string requestedType)
		{
			return new LocalizedString("OrganizationInDryRunMode", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				tenant,
				requestedType
			});
		}

		public static LocalizedString SymphonyInvalidOperationFault(string faultMessage)
		{
			return new LocalizedString("SymphonyInvalidOperationFault", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				faultMessage
			});
		}

		public static LocalizedString InvalidUpgradeStatus(string id, UpgradeStatusTypes currentStatus)
		{
			return new LocalizedString("InvalidUpgradeStatus", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				id,
				currentStatus
			});
		}

		public static LocalizedString SymphonyArgumentFault(string faultMessage)
		{
			return new LocalizedString("SymphonyArgumentFault", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				faultMessage
			});
		}

		public static LocalizedString ErrorGettingFiles(string directory)
		{
			return new LocalizedString("ErrorGettingFiles", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				directory
			});
		}

		public static LocalizedString AnchorServiceInstanceNotActive
		{
			get
			{
				return new LocalizedString("AnchorServiceInstanceNotActive", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OrganizationNotFound(string org)
		{
			return new LocalizedString("OrganizationNotFound", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString TooManyPilotMailboxes
		{
			get
			{
				return new LocalizedString("TooManyPilotMailboxes", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedBatchType(string batchType)
		{
			return new LocalizedString("UnsupportedBatchType", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				batchType
			});
		}

		public static LocalizedString InvalidE14Mailboxes
		{
			get
			{
				return new LocalizedString("InvalidE14Mailboxes", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SymphonyCancelNotAllowedFault(string faultMessage)
		{
			return new LocalizedString("SymphonyCancelNotAllowedFault", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				faultMessage
			});
		}

		public static LocalizedString ErrorQueryingWorkItem
		{
			get
			{
				return new LocalizedString("ErrorQueryingWorkItem", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SymphonyAccessDeniedFault(string faultMessage)
		{
			return new LocalizedString("SymphonyAccessDeniedFault", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				faultMessage
			});
		}

		public static LocalizedString ErrorNoE14ServersFound
		{
			get
			{
				return new LocalizedString("ErrorNoE14ServersFound", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReadingFile(string file)
		{
			return new LocalizedString("ErrorReadingFile", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				file
			});
		}

		public static LocalizedString InvalidE15Mailboxes
		{
			get
			{
				return new LocalizedString("InvalidE15Mailboxes", UpgradeHandlerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserNotFound(string org)
		{
			return new LocalizedString("UserNotFound", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString ErrorGettingDatabases(string server)
		{
			return new LocalizedString("ErrorGettingDatabases", UpgradeHandlerStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString GetLocalizedString(UpgradeHandlerStrings.IDs key)
		{
			return new LocalizedString(UpgradeHandlerStrings.stringIDs[(uint)key], UpgradeHandlerStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.Strings", typeof(UpgradeHandlerStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			AnchorServiceInstanceNotActive = 3297992468U,
			TooManyPilotMailboxes = 3742916171U,
			InvalidE14Mailboxes = 3847819607U,
			ErrorQueryingWorkItem = 31377678U,
			ErrorNoE14ServersFound = 2176982493U,
			InvalidE15Mailboxes = 683139314U
		}

		private enum ParamIDs
		{
			OrganizationHasConstraints,
			InvalidRequestedType,
			UnsupportedUpgradeRequestType,
			InvalidOrganizationVersion,
			SymphonyFault,
			InvalidOrganizationState,
			OrganizationInDryRunMode,
			SymphonyInvalidOperationFault,
			InvalidUpgradeStatus,
			SymphonyArgumentFault,
			ErrorGettingFiles,
			OrganizationNotFound,
			UnsupportedBatchType,
			SymphonyCancelNotAllowedFault,
			SymphonyAccessDeniedFault,
			ErrorReadingFile,
			UserNotFound,
			ErrorGettingDatabases
		}
	}
}
