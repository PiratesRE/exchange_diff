using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal static class HResult
	{
		public static bool IsHandled(uint errorCode)
		{
			return (errorCode & 3221225472U) == 0U;
		}

		public static bool IsRetryableAtCurrentHub(uint errorCode)
		{
			return (errorCode & 3221225472U) == 1073741824U;
		}

		public static bool IsRetryableAtOtherHub(uint errorCode)
		{
			return (errorCode & 3221225472U) == 2147483648U;
		}

		public static bool IsUnexpectedError(uint errorCode)
		{
			return (errorCode & 3221225472U) == 3221225472U;
		}

		public static bool IsRetryMailbox(uint errorCode)
		{
			return (errorCode & 117440512U) == 16777216U;
		}

		public static bool IsRetryMailboxDatabase(uint errorCode)
		{
			return (errorCode & 117440512U) == 33554432U;
		}

		public static bool IsRetryMailboxServer(uint errorCode)
		{
			return (errorCode & 117440512U) == 67108864U;
		}

		public static bool IsNonActionable(uint errorCode)
		{
			return errorCode == 6U || errorCode == 16U || errorCode == 8U || errorCode == 9U;
		}

		public static bool IsMessageSubmittedOrHasNoRcpts(uint errorCode)
		{
			return errorCode == 0U || errorCode == 1U || errorCode == 15U;
		}

		public static string GetStringForErrorCode(uint errorCode)
		{
			uint num = errorCode;
			if (num <= 1107296262U)
			{
				switch (num)
				{
				case 0U:
					return "Success";
				case 1U:
					return "NDRGenerated";
				case 2U:
					return "NoMessageForNDR";
				case 3U:
					return "PoisonMessage";
				case 4U:
				case 13U:
				case 14U:
					break;
				case 5U:
					return "PermanentNDRGenerationFailure";
				case 6U:
					return "NoMessageItem";
				case 7U:
					return "InvalidSender";
				case 8U:
					return "VirusMessage";
				case 9U:
					return "EventWithoutSubmitFlag";
				case 10U:
					return "InvalidQuotaWarningMessage";
				case 11U:
					return "MessageThrottled";
				case 12U:
					return "ResubmissionAbortedDueToContentChange";
				case 15U:
					return "NoRecipients";
				case 16U:
					return "Skip";
				case 17U:
					return "InvalidSenderNullProxy";
				case 18U:
					return "InvalidSenderInvalidProxy";
				case 19U:
					return "InvalidSenderLookupFailed";
				case 20U:
					return "InvalidSenderCouldNotResolveRecipient";
				case 21U:
					return "InvalidSenderCouldNotResolveUser";
				case 22U:
					return "PoisonMessageHandledAndPoisonNdrSuccess";
				case 23U:
					return "PoisonMessageHandledButPoisonNdrPermanentFailure";
				case 24U:
					return "PoisonMessageHandledButPoisonNdrRetry";
				default:
					switch (num)
					{
					case 1090519040U:
						return "RetryMailboxError";
					case 1090519041U:
						return "TemporaryNDRGenerationFailureMailbox";
					case 1090519042U:
						return "TooManySubmissions";
					case 1090519043U:
						return "RetryableRpcError";
					default:
						switch (num)
						{
						case 1107296260U:
							return "RetryMailboxDatabaseError";
						case 1107296262U:
							return "TemporaryNDRGenerationFailureMailboxDatabase";
						}
						break;
					}
					break;
				}
			}
			else if (num <= 2214592517U)
			{
				switch (num)
				{
				case 1140850693U:
					return "RetryMailboxServerError";
				case 1140850694U:
					break;
				case 1140850695U:
					return "TemporaryNDRGenerationFailureMailboxServer";
				case 1140850696U:
					return "ResourceQuarantined";
				default:
					switch (num)
					{
					case 2214592513U:
						return "SubmissionPaused";
					case 2214592514U:
						return "ServerRetired";
					case 2214592515U:
						return "ServerNotAvailable";
					case 2214592516U:
						return "RetryOtherHubError";
					case 2214592517U:
						return "TemporaryNDRGenerationFailureOtherHub";
					}
					break;
				}
			}
			else
			{
				if (num == 2684354560U)
				{
					return "RetrySmtp";
				}
				if (num == 3221225472U)
				{
					return "UnexpectedError";
				}
			}
			return errorCode.ToString("X");
		}

		public const uint EventHandledBit = 0U;

		public const uint RetryCurrentHubBit = 1073741824U;

		public const uint RetryOtherHubBit = 2147483648U;

		public const uint RetrySmtp = 2684354560U;

		public const uint UnexpectedErrorBit = 3221225472U;

		public const uint CategoryBit = 3221225472U;

		public const uint RetryMailboxBit = 16777216U;

		public const uint RetryMailboxDatabaseBit = 33554432U;

		public const uint RetryMailboxServerBit = 67108864U;

		public const uint ScopeBit = 117440512U;

		public const uint Success = 0U;

		public const uint NDRGenerated = 1U;

		public const uint NoMessageForNDR = 2U;

		public const uint PoisonMessage = 3U;

		public const uint PermanentNDRGenerationFailure = 5U;

		public const uint NoMessageItem = 6U;

		public const uint InvalidSender = 7U;

		public const uint VirusMessage = 8U;

		public const uint EventWithoutSubmitFlag = 9U;

		public const uint InvalidQuotaWarningMessage = 10U;

		public const uint MessageThrottled = 11U;

		public const uint ResubmissionAbortedDueToContentChange = 12U;

		public const uint NoRecipients = 15U;

		public const uint Skip = 16U;

		public const uint InvalidSenderNullProxy = 17U;

		public const uint InvalidSenderInvalidProxy = 18U;

		public const uint InvalidSenderLookupFailed = 19U;

		public const uint InvalidSenderCouldNotResolveRecipient = 20U;

		public const uint InvalidSenderCouldNotResolveUser = 21U;

		public const uint PoisonMessageHandledAndPoisonNdrSuccess = 22U;

		public const uint PoisonMessageHandledButPoisonNdrPermanentFailure = 23U;

		public const uint PoisonMessageHandledButPoisonNdrRetry = 24U;

		public const uint RetryMailboxError = 1090519040U;

		public const uint TemporaryNDRGenerationFailureMailbox = 1090519041U;

		public const uint TooManySubmissions = 1090519042U;

		public const uint RetryableRpcError = 1090519043U;

		public const uint RetryMailboxDatabaseError = 1107296260U;

		public const uint RetryMailboxServerError = 1140850693U;

		public const uint TemporaryNDRGenerationFailureMailboxDatabase = 1107296262U;

		public const uint TemporaryNDRGenerationFailureMailboxServer = 1140850695U;

		public const uint ResourceQuarantined = 1140850696U;

		public const uint SubmissionPaused = 2214592513U;

		public const uint ServerRetired = 2214592514U;

		public const uint ServerNotAvailable = 2214592515U;

		public const uint RetryOtherHubError = 2214592516U;

		public const uint TemporaryNDRGenerationFailureOtherHub = 2214592517U;

		public const uint UnexpectedError = 3221225472U;
	}
}
