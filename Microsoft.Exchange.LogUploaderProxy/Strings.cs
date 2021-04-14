using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(590305096U, "ErrorPermanentDALException");
			Strings.stringIDs.Add(3093538361U, "ErrorDataStoreUnavailable");
			Strings.stringIDs.Add(797725276U, "ErrorTransientDALExceptionMaxRetries");
			Strings.stringIDs.Add(3879120206U, "LogFileRangeNegativeEndOffset");
			Strings.stringIDs.Add(2833073812U, "ErrorTransientDALExceptionAmbientTransaction");
			Strings.stringIDs.Add(810055759U, "LogFileRangeNegativeStartOffset");
		}

		public static string FailedToCastToRequestedType(Type t, string fieldName)
		{
			return string.Format(Strings.ResourceManager.GetString("FailedToCastToRequestedType"), t, fieldName);
		}

		public static string ErrorPermanentDALException
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorPermanentDALException");
			}
		}

		public static string FailedToParseField(string fieldName, Type type)
		{
			return string.Format(Strings.ResourceManager.GetString("FailedToParseField"), fieldName, type);
		}

		public static string ErrorDataStoreUnavailable
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorDataStoreUnavailable");
			}
		}

		public static string UnknownField(string fieldName)
		{
			return string.Format(Strings.ResourceManager.GetString("UnknownField"), fieldName);
		}

		public static string ErrorTransientDALExceptionMaxRetries
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorTransientDALExceptionMaxRetries");
			}
		}

		public static string MalformedLogRangeLine(string line)
		{
			return string.Format(Strings.ResourceManager.GetString("MalformedLogRangeLine"), line);
		}

		public static string LogFileRangeNegativeEndOffset
		{
			get
			{
				return Strings.ResourceManager.GetString("LogFileRangeNegativeEndOffset");
			}
		}

		public static string ErrorTransientDALExceptionAmbientTransaction
		{
			get
			{
				return Strings.ResourceManager.GetString("ErrorTransientDALExceptionAmbientTransaction");
			}
		}

		public static string FailedToInstantiateLogFileInfoFileNotExist(string file)
		{
			return string.Format(Strings.ResourceManager.GetString("FailedToInstantiateLogFileInfoFileNotExist"), file);
		}

		public static string LogFileRangeNegativeStartOffset
		{
			get
			{
				return Strings.ResourceManager.GetString("LogFileRangeNegativeStartOffset");
			}
		}

		public static string RequestedCustomDataFieldMissing(string fieldName)
		{
			return string.Format(Strings.ResourceManager.GetString("RequestedCustomDataFieldMissing"), fieldName);
		}

		public static string MergeLogRangesFailed(long otherStartOffset, long otherEndOffset, long startOffset, long endOffset)
		{
			return string.Format(Strings.ResourceManager.GetString("MergeLogRangesFailed"), new object[]
			{
				otherStartOffset,
				otherEndOffset,
				startOffset,
				endOffset
			});
		}

		public static string LogFileRangeWrongOffsets(long start, long end)
		{
			return string.Format(Strings.ResourceManager.GetString("LogFileRangeWrongOffsets"), start, end);
		}

		public static string GetLogTimeStampFailed(string fileName)
		{
			return string.Format(Strings.ResourceManager.GetString("GetLogTimeStampFailed"), fileName);
		}

		public static string GetLocalizedString(Strings.IDs key)
		{
			return Strings.ResourceManager.GetString(Strings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(6);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.LogUploaderProxy.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorPermanentDALException = 590305096U,
			ErrorDataStoreUnavailable = 3093538361U,
			ErrorTransientDALExceptionMaxRetries = 797725276U,
			LogFileRangeNegativeEndOffset = 3879120206U,
			ErrorTransientDALExceptionAmbientTransaction = 2833073812U,
			LogFileRangeNegativeStartOffset = 810055759U
		}

		private enum ParamIDs
		{
			FailedToCastToRequestedType,
			FailedToParseField,
			UnknownField,
			MalformedLogRangeLine,
			FailedToInstantiateLogFileInfoFileNotExist,
			RequestedCustomDataFieldMissing,
			MergeLogRangesFailed,
			LogFileRangeWrongOffsets,
			GetLogTimeStampFailed
		}
	}
}
