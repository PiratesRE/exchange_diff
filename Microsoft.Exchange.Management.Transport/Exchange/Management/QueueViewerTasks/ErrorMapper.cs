using System;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	internal static class ErrorMapper
	{
		public static LocalizedException GetLocalizedException(int errorCode, ObjectId identity, ServerIdParameter server)
		{
			if (errorCode == QVErrorCode.QV_E_OBJECT_NOT_FOUND)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.ObjectNotFound(ErrorMapper.GetIdentityString(identity)));
			}
			if (errorCode == QVErrorCode.QV_E_MULTIPLE_IDENTITY_MATCH)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.MultipleIdentityMatch(ErrorMapper.GetIdentityString(identity)));
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_OPERATION)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidOperation(ErrorMapper.GetIdentityString(identity)));
			}
			if (errorCode == QVErrorCode.QV_E_INCOMPLETE_IDENTITY)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.IncompleteIdentity(ErrorMapper.GetIdentityString(identity)));
			}
			if (errorCode == QVErrorCode.QV_E_MESSAGE_NOT_SUSPENDED)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.MessageNotSuspended(ErrorMapper.GetIdentityString(identity)));
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_IDENTITY_FOR_EQUALITY)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidIdentityForEquality);
			}
			if (errorCode == QVErrorCode.QV_E_AMBIGUOUS_PARAMETER_SET)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.AmbiguousParameterSet);
			}
			if (errorCode == QVErrorCode.QV_E_FILTER_TYPE_NOT_SUPPORTED)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.FilterTypeNotSupported);
			}
			if (errorCode == QVErrorCode.QV_E_TEXT_MATCHING_NOT_SUPPORTED)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.TextMatchingNotSupported);
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_FIELD_NAME)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidFieldName);
			}
			if (errorCode == QVErrorCode.QV_E_TOO_MANY_RESULTS)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.TooManyResults);
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_IDENTITY_STRING)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidIdentityString);
			}
			if (errorCode == QVErrorCode.QV_E_COMPARISON_NOT_SUPPORTED)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.ComparisonNotSupported);
			}
			if (errorCode == QVErrorCode.QV_E_RPC_SERVER_UNAVAILABLE)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.RpcUnavailable((string)server));
			}
			if (errorCode == QVErrorCode.QV_E_ENDPOINT_NOT_REGISTERED || errorCode == QVErrorCode.QV_E_UNKNOWN_INTERFACE)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.RpcNotRegistered((string)server));
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_CLIENT_DATA)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidClientData);
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_SERVER_DATA)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidServerData);
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_SERVER_VERSION)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidServerVersion);
			}
			if (errorCode == QVErrorCode.QV_E_INVALID_SERVER_COLLECTION)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.InvalidServerCollection);
			}
			if (errorCode == QVErrorCode.QV_E_QUEUE_RESUBMIT_IN_PROGRESS)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.QueueResubmitInProgress);
			}
			if (errorCode == QVErrorCode.QV_E_REDIRECT_MESSAGE_IN_PROGRESS)
			{
				return ErrorMapper.NewLocalizedException(errorCode, QueueViewerStrings.RedirectMessageInProgress);
			}
			Win32Exception ex = new Win32Exception(errorCode);
			return new LocalizedException(QueueViewerStrings.GenericRpcError(ex.Message, (string)server), ex);
		}

		public static string GetIdentityString(ObjectId identity)
		{
			if (identity != null)
			{
				return identity.ToString();
			}
			throw new Exception("Unexpected null identity when creating localized exception");
		}

		private static LocalizedException NewLocalizedException(int errorCode, LocalizedString locString)
		{
			return new LocalizedException(locString)
			{
				ErrorCode = errorCode
			};
		}
	}
}
