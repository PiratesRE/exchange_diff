using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal static class Constants
	{
		public const string RecipientListToGeneratePL = "Microsoft.Exchange.RMSEncryptionAgent.RecipientListForPL";

		public const string CacheKeySeparator = "|";

		public const string HtmlBegin = "<HTML><HEAD></HEAD><BODY>";

		public const string HtmlBR = "<BR>";

		public const string HtmlEnd = "</BODY></HTML>";

		public const string NDRStatusCode = "550";

		public const string NDRStatusEnhancedCode = "5.7.1";

		public const string RetryStatusCode = "451";

		public const string RetryStatusEnhancedCode = "4.3.2";

		public const string ActiveAgentsCapReached = "Already processing maximum number of messages.";

		public const string ContactSystemAdministrator = "Please contact your system administrator for more information.";

		public const string DeliveryNotAuthorized = "Delivery not authorized, message refused.";

		public const string EncryptionDisabled = "Cannot RMS protect the message because Encryption is disabled in Microsoft Exchange Transport.";

		public const string EncryptionDisabledOME = "Cannot OME protect the message because Encryption is disabled in Microsoft Exchange Transport.";

		public const string NDRGenericOME = "Cannot OME protect the message. Error Code: {0}.";

		public const string NDRPublishLicenseLimitExceededOME = "Cannot OME protect the message because there are too many recipients.";

		public const string Enterprise = "Enterprise";

		public const string FailedToFindAddressEntry = "Failed to find AddressEntry.";

		public const string FailedToFindTemplate = "A failure occurred when trying to look up Rights Management Server template '{0}'.";

		public const string FailedToReadOrganizationConfiguration = "A transient error occurred when reading configuration for {0} from AD.";

		public const string FailedToReadUserConfiguration = "A transient error occurred when reading user configuration from AD.";

		public const string JournalReportDecryptionTransientError = "A transient error occurred during journal decryption when communicating with RMS server {0}.";

		public const string NoValidRecipients = "No valid recipients.";

		public const string PrelicenseTransientError = "A transient error occurred during prelicensing when communicating with RMS server {0}.";

		public const string SenderNotAuthorized = "The sender is not authorized to send e-mail messages to this e-mail address.";

		public const string Tenant = "Tenant '{0}'";

		public const string UnableToObtainPLForDLUnderLiveRMS = "Cannot RMS protect a recipient that is a distribution group under Live RMS.";

		public const string UnableToPipelineDecrypt = "Microsoft Exchange Transport cannot RMS decrypt the message.";

		public const string UnableToPipelineReEncryptNoPL = "Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR1).";

		public const string UnableToPipelineReEncryptNoUL = "Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR2).";

		public const string UnableToPipelineReEncryptNoLicenseUri = "Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR4).";

		public const string FailedToPipelineReEncrypt = "Microsoft Exchange Transport cannot RMS re-encrypt the message (ATTR3).";

		public const string PrelicenseTransientErrorWithFailureCodesSame = "A transient error occurred during prelicensing when communicating with RMS server {0}. Failure Code {1}";

		public const string PrelicenseTransientErrorWithFailureCodesDifferent = "A transient error occurred during prelicensing when communicating with RMS server {0}. First recipient {1} failure code is {2}";

		public const string ExceptionType = "Exception encountered: {0}.";

		public const string FailureCode = "Failure Code: {0}.";

		public const int DefaultANSICodePage = 1252;

		public static readonly string SupportedMapiMessageClassForDrm = "IPM.Note";

		public static readonly SmtpResponse NDRResponse = new SmtpResponse("550", "5.7.1", Constants.NDRTexts);

		private static readonly string[] NDRTexts = new string[]
		{
			"Delivery not authorized, message refused.",
			"The sender is not authorized to send e-mail messages to this e-mail address.",
			"Please contact your system administrator for more information."
		};
	}
}
