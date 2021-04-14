using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal static class EvaluationErrorsHelper
	{
		public static int MakeRetriableError(int evaluationError)
		{
			return (int)EvaluationErrorsHelper.GetErrorCode(evaluationError);
		}

		public static int MakeRetriableError(EvaluationErrors evaluationError)
		{
			return (int)EvaluationErrorsHelper.GetErrorCode(evaluationError);
		}

		public static int MakePermanentError(int evaluationError)
		{
			return (int)(-(int)EvaluationErrorsHelper.GetErrorCode(evaluationError));
		}

		public static int MakePermanentError(EvaluationErrors evaluationError)
		{
			return (int)(-(int)EvaluationErrorsHelper.GetErrorCode(evaluationError));
		}

		public static bool IsRetriableError(EvaluationErrors evaluationError)
		{
			return EvaluationErrorsHelper.IsRetriableError((int)evaluationError);
		}

		public static bool IsRetriableError(int evaluationError)
		{
			return evaluationError > 0;
		}

		public static bool IsPermanentError(EvaluationErrors evaluationError)
		{
			return EvaluationErrorsHelper.IsPermanentError((int)evaluationError);
		}

		public static bool IsPermanentError(int evaluationError)
		{
			return evaluationError < 0;
		}

		public static EvaluationErrors GetErrorCode(EvaluationErrors evaluationError)
		{
			return EvaluationErrorsHelper.GetErrorCode((int)evaluationError);
		}

		public static EvaluationErrors GetErrorCode(int evaluationError)
		{
			return (EvaluationErrors)Math.Abs(evaluationError);
		}

		public static LocalizedString GetErrorDescription(EvaluationErrors evaluationError)
		{
			evaluationError = EvaluationErrorsHelper.GetErrorCode(evaluationError);
			if (evaluationError == EvaluationErrors.None)
			{
				return LocalizedString.Empty;
			}
			return new LocalizedString(LocalizedDescriptionAttribute.FromEnum(typeof(EvaluationErrors), evaluationError));
		}

		public static bool ShouldMakePermanent(int attemptCount, int errorCode)
		{
			EvaluationErrors errorCode2 = EvaluationErrorsHelper.GetErrorCode(errorCode);
			switch (errorCode2)
			{
			case EvaluationErrors.AttachmentLimitReached:
			case EvaluationErrors.DocumentParserFailure:
				break;
			case EvaluationErrors.MarsWriterTruncation:
			case EvaluationErrors.AnnotationTokenError:
				goto IL_5A;
			case EvaluationErrors.PoisonDocument:
				return attemptCount >= EvaluationErrorsHelper.searchConfig.MaxAttemptCountPoisonBeforePermanent || EvaluationErrorsHelper.IsPermanentError(errorCode);
			default:
				switch (errorCode2)
				{
				case EvaluationErrors.MailboxQuarantined:
				case EvaluationErrors.LoginFailed:
				case EvaluationErrors.TextConversionFailure:
					break;
				case EvaluationErrors.MailboxLocked:
				case EvaluationErrors.MapiNoSupport:
					goto IL_5A;
				default:
					goto IL_5A;
				}
				break;
			}
			return true;
			IL_5A:
			return attemptCount >= EvaluationErrorsHelper.searchConfig.MaxAttemptCountBeforePermanent || EvaluationErrorsHelper.IsPermanentError(errorCode);
		}

		public const int MaxAttemptCountBeforePermanent = 3;

		private static SearchConfig searchConfig = SearchConfig.Instance;
	}
}
