using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	internal static class MigrationWorkflowServiceStrings
	{
		static MigrationWorkflowServiceStrings()
		{
			MigrationWorkflowServiceStrings.stringIDs.Add(2604524602U, "ErrorCmdletPoolExhausted");
			MigrationWorkflowServiceStrings.stringIDs.Add(3276599839U, "ErrorHeatMapNotBuilt");
			MigrationWorkflowServiceStrings.stringIDs.Add(3174005438U, "ErrorInsufficientCapacityProvisioning");
			MigrationWorkflowServiceStrings.stringIDs.Add(334017145U, "ErrorAutomaticMailboxLoadBalancingNotAllowed");
			MigrationWorkflowServiceStrings.stringIDs.Add(2617355097U, "ErrorUnknownProvisioningStatus");
		}

		public static LocalizedString ErrorMultipleRecipientFound(string userId)
		{
			return new LocalizedString("ErrorMultipleRecipientFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString ErrorDatabaseNotFound(string guid)
		{
			return new LocalizedString("ErrorDatabaseNotFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorMissingAnchorMailbox(string capability)
		{
			return new LocalizedString("ErrorMissingAnchorMailbox", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				capability
			});
		}

		public static LocalizedString ErrorCannotRetrieveCapacityData(string objectIdentity)
		{
			return new LocalizedString("ErrorCannotRetrieveCapacityData", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				objectIdentity
			});
		}

		public static LocalizedString UsageText(string processName)
		{
			return new LocalizedString("UsageText", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				processName
			});
		}

		public static LocalizedString ErrorServerNotFound(string guid)
		{
			return new LocalizedString("ErrorServerNotFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorConstraintCouldNotBeSatisfied(string constraintExpression)
		{
			return new LocalizedString("ErrorConstraintCouldNotBeSatisfied", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				constraintExpression
			});
		}

		public static LocalizedString ErrorMissingDatabaseActivationPreference(string databaseName)
		{
			return new LocalizedString("ErrorMissingDatabaseActivationPreference", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				databaseName
			});
		}

		public static LocalizedString ErrorCmdletPoolExhausted
		{
			get
			{
				return new LocalizedString("ErrorCmdletPoolExhausted", MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContainerCannotTakeLoad(string containerGuid)
		{
			return new LocalizedString("ErrorContainerCannotTakeLoad", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				containerGuid
			});
		}

		public static LocalizedString ErrorDatabaseFailedOver(string guid)
		{
			return new LocalizedString("ErrorDatabaseFailedOver", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorHeatMapNotBuilt
		{
			get
			{
				return new LocalizedString("ErrorHeatMapNotBuilt", MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOrganization(string orgName)
		{
			return new LocalizedString("ErrorInvalidOrganization", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				orgName
			});
		}

		public static LocalizedString ErrorInsufficientCapacityProvisioning
		{
			get
			{
				return new LocalizedString("ErrorInsufficientCapacityProvisioning", MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecipientNotFound(string userId)
		{
			return new LocalizedString("ErrorRecipientNotFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				userId
			});
		}

		public static LocalizedString ErrorObjectCannotBeMoved(string objectType, string objectIdentity)
		{
			return new LocalizedString("ErrorObjectCannotBeMoved", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				objectType,
				objectIdentity
			});
		}

		public static LocalizedString ErrorAutomaticMailboxLoadBalancingNotAllowed
		{
			get
			{
				return new LocalizedString("ErrorAutomaticMailboxLoadBalancingNotAllowed", MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOverlappingBandDefinition(string newBand, string existingBand)
		{
			return new LocalizedString("ErrorOverlappingBandDefinition", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				newBand,
				existingBand
			});
		}

		public static LocalizedString ErrorEntityNotMovable(string orgId, string userId)
		{
			return new LocalizedString("ErrorEntityNotMovable", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				orgId,
				userId
			});
		}

		public static LocalizedString ErrorUnknownProvisioningStatus
		{
			get
			{
				return new LocalizedString("ErrorUnknownProvisioningStatus", MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDagNotFound(string guid)
		{
			return new LocalizedString("ErrorDagNotFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString ErrorInvalidExternalOrganizationId(string orgName, string externalId)
		{
			return new LocalizedString("ErrorInvalidExternalOrganizationId", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				orgName,
				externalId
			});
		}

		public static LocalizedString ErrorDatabaseNotLocal(string databaseName, string edbPath)
		{
			return new LocalizedString("ErrorDatabaseNotLocal", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				databaseName,
				edbPath
			});
		}

		public static LocalizedString ErrorNotEnoughDatabaseCapacity(string databaseGuid, string capacityType, long requestedCapacityUnits, long availableCapacityUnits)
		{
			return new LocalizedString("ErrorNotEnoughDatabaseCapacity", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				databaseGuid,
				capacityType,
				requestedCapacityUnits,
				availableCapacityUnits
			});
		}

		public static LocalizedString ErrorBandDefinitionNotFound(string band)
		{
			return new LocalizedString("ErrorBandDefinitionNotFound", MigrationWorkflowServiceStrings.ResourceManager, new object[]
			{
				band
			});
		}

		public static LocalizedString GetLocalizedString(MigrationWorkflowServiceStrings.IDs key)
		{
			return new LocalizedString(MigrationWorkflowServiceStrings.stringIDs[(uint)key], MigrationWorkflowServiceStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.MailboxLoadBalance.Strings", typeof(MigrationWorkflowServiceStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorCmdletPoolExhausted = 2604524602U,
			ErrorHeatMapNotBuilt = 3276599839U,
			ErrorInsufficientCapacityProvisioning = 3174005438U,
			ErrorAutomaticMailboxLoadBalancingNotAllowed = 334017145U,
			ErrorUnknownProvisioningStatus = 2617355097U
		}

		private enum ParamIDs
		{
			ErrorMultipleRecipientFound,
			ErrorDatabaseNotFound,
			ErrorMissingAnchorMailbox,
			ErrorCannotRetrieveCapacityData,
			UsageText,
			ErrorServerNotFound,
			ErrorConstraintCouldNotBeSatisfied,
			ErrorMissingDatabaseActivationPreference,
			ErrorContainerCannotTakeLoad,
			ErrorDatabaseFailedOver,
			ErrorInvalidOrganization,
			ErrorRecipientNotFound,
			ErrorObjectCannotBeMoved,
			ErrorOverlappingBandDefinition,
			ErrorEntityNotMovable,
			ErrorDagNotFound,
			ErrorInvalidExternalOrganizationId,
			ErrorDatabaseNotLocal,
			ErrorNotEnoughDatabaseCapacity,
			ErrorBandDefinitionNotFound
		}
	}
}
