using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Core
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(1350420100U, "IndexNotEnabled");
			Strings.stringIDs.Add(1862033255U, "EvaluationErrorsNoSupport");
			Strings.stringIDs.Add(2980931297U, "EvaluationErrorsAnnotationTokenError");
			Strings.stringIDs.Add(800591795U, "DocumentFailure");
			Strings.stringIDs.Add(3359953177U, "CatalogExcluded");
			Strings.stringIDs.Add(1803150643U, "AcceptedDomainRetrievalFailure");
			Strings.stringIDs.Add(3613842499U, "CatalogReseed");
			Strings.stringIDs.Add(4204989999U, "EvaluationErrorsMailboxLocked");
			Strings.stringIDs.Add(1780900744U, "CannotProcessDoc");
			Strings.stringIDs.Add(77711424U, "EvaluationErrorsMailboxOffline");
			Strings.stringIDs.Add(1144276406U, "EvaluationErrorsTextConversionFailure");
			Strings.stringIDs.Add(1946519939U, "ComponentFailure");
			Strings.stringIDs.Add(935218638U, "EvaluationErrorsTimeout");
			Strings.stringIDs.Add(2717588701U, "EvaluationErrorsAttachmentLimitReached");
			Strings.stringIDs.Add(2107982464U, "SeedingCatalog");
			Strings.stringIDs.Add(1663626573U, "EvaluationErrorsLoginFailed");
			Strings.stringIDs.Add(4239111926U, "EvaluationErrorsGenericError");
			Strings.stringIDs.Add(427251555U, "MapiNetworkError");
			Strings.stringIDs.Add(485396995U, "RpcEndpointFailedToRegister");
			Strings.stringIDs.Add(3251639695U, "IndexStatusTimestampTooOld");
			Strings.stringIDs.Add(498323155U, "EvaluationErrorsMailboxQuarantined");
			Strings.stringIDs.Add(1798012184U, "IndexStatusRegistryNotFound");
			Strings.stringIDs.Add(3214313948U, "CatalogCorruption");
			Strings.stringIDs.Add(3880225324U, "EvaluationErrorsStaleEvent");
			Strings.stringIDs.Add(2285368354U, "CatalogSuspended");
			Strings.stringIDs.Add(3376753831U, "ActivationPreferenceSkipped");
			Strings.stringIDs.Add(3459498499U, "LagCopySkipped");
			Strings.stringIDs.Add(3063759980U, "RecoveryDatabaseSkipped");
			Strings.stringIDs.Add(1388966758U, "ComponentCriticalFailure");
			Strings.stringIDs.Add(3194642979U, "InternalError");
			Strings.stringIDs.Add(1686554670U, "EvaluationErrorsMarsWriterTruncation");
			Strings.stringIDs.Add(4083887973U, "OperationFailure");
			Strings.stringIDs.Add(2780255030U, "CrawlingDatabase");
			Strings.stringIDs.Add(3710103617U, "EvaluationErrorsSessionUnavailable");
			Strings.stringIDs.Add(3704567193U, "EvaluationErrorsRightsManagementFailure");
			Strings.stringIDs.Add(584722949U, "EvaluationErrorsDocumentParserFailure");
			Strings.stringIDs.Add(1400615166U, "EvaluationErrorsPoisonDocument");
			Strings.stringIDs.Add(3608246202U, "DatabaseOffline");
			Strings.stringIDs.Add(2735780538U, "DocProcessCanceled");
		}

		public static LocalizedString IndexNotEnabled
		{
			get
			{
				return new LocalizedString("IndexNotEnabled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FastServiceNotRunning(string server)
		{
			return new LocalizedString("FastServiceNotRunning", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString EvaluationErrorsNoSupport
		{
			get
			{
				return new LocalizedString("EvaluationErrorsNoSupport", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsAnnotationTokenError
		{
			get
			{
				return new LocalizedString("EvaluationErrorsAnnotationTokenError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DocumentFailure
		{
			get
			{
				return new LocalizedString("DocumentFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CatalogExcluded
		{
			get
			{
				return new LocalizedString("CatalogExcluded", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AcceptedDomainRetrievalFailure
		{
			get
			{
				return new LocalizedString("AcceptedDomainRetrievalFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CatalogReseed
		{
			get
			{
				return new LocalizedString("CatalogReseed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsMailboxLocked
		{
			get
			{
				return new LocalizedString("EvaluationErrorsMailboxLocked", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusException(string error)
		{
			return new LocalizedString("IndexStatusException", Strings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString CannotProcessDoc
		{
			get
			{
				return new LocalizedString("CannotProcessDoc", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsMailboxOffline
		{
			get
			{
				return new LocalizedString("EvaluationErrorsMailboxOffline", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsTextConversionFailure
		{
			get
			{
				return new LocalizedString("EvaluationErrorsTextConversionFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentFailure
		{
			get
			{
				return new LocalizedString("ComponentFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsTimeout
		{
			get
			{
				return new LocalizedString("EvaluationErrorsTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DocumentValidationFailure(string msg)
		{
			return new LocalizedString("DocumentValidationFailure", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString EvaluationErrorsAttachmentLimitReached
		{
			get
			{
				return new LocalizedString("EvaluationErrorsAttachmentLimitReached", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SeedingCatalog
		{
			get
			{
				return new LocalizedString("SeedingCatalog", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsLoginFailed
		{
			get
			{
				return new LocalizedString("EvaluationErrorsLoginFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsGenericError
		{
			get
			{
				return new LocalizedString("EvaluationErrorsGenericError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiNetworkError
		{
			get
			{
				return new LocalizedString("MapiNetworkError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServiceNotRunning(string server)
		{
			return new LocalizedString("SearchServiceNotRunning", Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString RpcEndpointFailedToRegister
		{
			get
			{
				return new LocalizedString("RpcEndpointFailedToRegister", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusTimestampTooOld
		{
			get
			{
				return new LocalizedString("IndexStatusTimestampTooOld", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsMailboxQuarantined
		{
			get
			{
				return new LocalizedString("EvaluationErrorsMailboxQuarantined", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusRegistryNotFound
		{
			get
			{
				return new LocalizedString("IndexStatusRegistryNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusInvalidProperty(string property, string value)
		{
			return new LocalizedString("IndexStatusInvalidProperty", Strings.ResourceManager, new object[]
			{
				property,
				value
			});
		}

		public static LocalizedString CatalogCorruption
		{
			get
			{
				return new LocalizedString("CatalogCorruption", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsStaleEvent
		{
			get
			{
				return new LocalizedString("EvaluationErrorsStaleEvent", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CatalogSuspended
		{
			get
			{
				return new LocalizedString("CatalogSuspended", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActivationPreferenceSkipped
		{
			get
			{
				return new LocalizedString("ActivationPreferenceSkipped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LagCopySkipped
		{
			get
			{
				return new LocalizedString("LagCopySkipped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecoveryDatabaseSkipped
		{
			get
			{
				return new LocalizedString("RecoveryDatabaseSkipped", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ComponentCriticalFailure
		{
			get
			{
				return new LocalizedString("ComponentCriticalFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalError
		{
			get
			{
				return new LocalizedString("InternalError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusInvalid(string value)
		{
			return new LocalizedString("IndexStatusInvalid", Strings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString EvaluationErrorsMarsWriterTruncation
		{
			get
			{
				return new LocalizedString("EvaluationErrorsMarsWriterTruncation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationFailure
		{
			get
			{
				return new LocalizedString("OperationFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CrawlingDatabase
		{
			get
			{
				return new LocalizedString("CrawlingDatabase", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsSessionUnavailable
		{
			get
			{
				return new LocalizedString("EvaluationErrorsSessionUnavailable", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsRightsManagementFailure
		{
			get
			{
				return new LocalizedString("EvaluationErrorsRightsManagementFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsDocumentParserFailure
		{
			get
			{
				return new LocalizedString("EvaluationErrorsDocumentParserFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EvaluationErrorsPoisonDocument
		{
			get
			{
				return new LocalizedString("EvaluationErrorsPoisonDocument", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IndexStatusNotFound(string database)
		{
			return new LocalizedString("IndexStatusNotFound", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString PropertyTypeError(string property)
		{
			return new LocalizedString("PropertyTypeError", Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString DatabaseOffline
		{
			get
			{
				return new LocalizedString("DatabaseOffline", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DocProcessCanceled
		{
			get
			{
				return new LocalizedString("DocProcessCanceled", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyError(string property)
		{
			return new LocalizedString("PropertyError", Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(39);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Search.Core.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			IndexNotEnabled = 1350420100U,
			EvaluationErrorsNoSupport = 1862033255U,
			EvaluationErrorsAnnotationTokenError = 2980931297U,
			DocumentFailure = 800591795U,
			CatalogExcluded = 3359953177U,
			AcceptedDomainRetrievalFailure = 1803150643U,
			CatalogReseed = 3613842499U,
			EvaluationErrorsMailboxLocked = 4204989999U,
			CannotProcessDoc = 1780900744U,
			EvaluationErrorsMailboxOffline = 77711424U,
			EvaluationErrorsTextConversionFailure = 1144276406U,
			ComponentFailure = 1946519939U,
			EvaluationErrorsTimeout = 935218638U,
			EvaluationErrorsAttachmentLimitReached = 2717588701U,
			SeedingCatalog = 2107982464U,
			EvaluationErrorsLoginFailed = 1663626573U,
			EvaluationErrorsGenericError = 4239111926U,
			MapiNetworkError = 427251555U,
			RpcEndpointFailedToRegister = 485396995U,
			IndexStatusTimestampTooOld = 3251639695U,
			EvaluationErrorsMailboxQuarantined = 498323155U,
			IndexStatusRegistryNotFound = 1798012184U,
			CatalogCorruption = 3214313948U,
			EvaluationErrorsStaleEvent = 3880225324U,
			CatalogSuspended = 2285368354U,
			ActivationPreferenceSkipped = 3376753831U,
			LagCopySkipped = 3459498499U,
			RecoveryDatabaseSkipped = 3063759980U,
			ComponentCriticalFailure = 1388966758U,
			InternalError = 3194642979U,
			EvaluationErrorsMarsWriterTruncation = 1686554670U,
			OperationFailure = 4083887973U,
			CrawlingDatabase = 2780255030U,
			EvaluationErrorsSessionUnavailable = 3710103617U,
			EvaluationErrorsRightsManagementFailure = 3704567193U,
			EvaluationErrorsDocumentParserFailure = 584722949U,
			EvaluationErrorsPoisonDocument = 1400615166U,
			DatabaseOffline = 3608246202U,
			DocProcessCanceled = 2735780538U
		}

		private enum ParamIDs
		{
			FastServiceNotRunning,
			IndexStatusException,
			DocumentValidationFailure,
			SearchServiceNotRunning,
			IndexStatusInvalidProperty,
			IndexStatusInvalid,
			IndexStatusNotFound,
			PropertyTypeError,
			PropertyError
		}
	}
}
