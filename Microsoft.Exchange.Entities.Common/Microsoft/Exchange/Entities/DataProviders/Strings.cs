using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.DataProviders
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3579904699U, "ErrorAccessDenied");
			Strings.stringIDs.Add(2352838554U, "ErrorAllButLastNestedAttachmentMustBeItemAttachment");
			Strings.stringIDs.Add(3410698111U, "RightsManagementMailboxOnlySupport");
			Strings.stringIDs.Add(2624402344U, "ErrorItemCorrupt");
			Strings.stringIDs.Add(1702622873U, "RightsManagementInternalLicensingDisabled");
			Strings.stringIDs.Add(1408093181U, "ErrorInvalidComplianceId");
			Strings.stringIDs.Add(1426071079U, "ErrorNestedAttachmentsCannotBeRemoved");
		}

		public static LocalizedString ErrorAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorAccessDenied", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRightsManagementDuplicateTemplateId(string id)
		{
			return new LocalizedString("ErrorRightsManagementDuplicateTemplateId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorAllButLastNestedAttachmentMustBeItemAttachment
		{
			get
			{
				return new LocalizedString("ErrorAllButLastNestedAttachmentMustBeItemAttachment", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrresolvableConflict(ConflictResolutionResult conflictResolutionResult)
		{
			return new LocalizedString("IrresolvableConflict", Strings.ResourceManager, new object[]
			{
				conflictResolutionResult
			});
		}

		public static LocalizedString RightsManagementMailboxOnlySupport
		{
			get
			{
				return new LocalizedString("RightsManagementMailboxOnlySupport", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemCorrupt
		{
			get
			{
				return new LocalizedString("ErrorItemCorrupt", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidRequest(LocalizedString violation)
		{
			return new LocalizedString("InvalidRequest", Strings.ResourceManager, new object[]
			{
				violation
			});
		}

		public static LocalizedString CanNotUseFolderIdForItem(string id)
		{
			return new LocalizedString("CanNotUseFolderIdForItem", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorMissingRequiredParameter(string name)
		{
			return new LocalizedString("ErrorMissingRequiredParameter", Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorUnsupportedOperation(string operationName)
		{
			return new LocalizedString("ErrorUnsupportedOperation", Strings.ResourceManager, new object[]
			{
				operationName
			});
		}

		public static LocalizedString RightsManagementInternalLicensingDisabled
		{
			get
			{
				return new LocalizedString("RightsManagementInternalLicensingDisabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ItemWithGivenIdNotFound(string id)
		{
			return new LocalizedString("ItemWithGivenIdNotFound", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorInvalidTimeZoneId(string id)
		{
			return new LocalizedString("ErrorInvalidTimeZoneId", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorInvalidComplianceId
		{
			get
			{
				return new LocalizedString("ErrorInvalidComplianceId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNestedAttachmentsCannotBeRemoved
		{
			get
			{
				return new LocalizedString("ErrorNestedAttachmentsCannotBeRemoved", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanNotUseItemIdForFolder(string id)
		{
			return new LocalizedString("CanNotUseItemIdForFolder", Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(7);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Entities.DataProviders.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorAccessDenied = 3579904699U,
			ErrorAllButLastNestedAttachmentMustBeItemAttachment = 2352838554U,
			RightsManagementMailboxOnlySupport = 3410698111U,
			ErrorItemCorrupt = 2624402344U,
			RightsManagementInternalLicensingDisabled = 1702622873U,
			ErrorInvalidComplianceId = 1408093181U,
			ErrorNestedAttachmentsCannotBeRemoved = 1426071079U
		}

		private enum ParamIDs
		{
			ErrorRightsManagementDuplicateTemplateId,
			IrresolvableConflict,
			InvalidRequest,
			CanNotUseFolderIdForItem,
			ErrorMissingRequiredParameter,
			ErrorUnsupportedOperation,
			ItemWithGivenIdNotFound,
			ErrorInvalidTimeZoneId,
			CanNotUseItemIdForFolder
		}
	}
}
