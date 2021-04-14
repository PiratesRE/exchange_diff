using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

namespace Microsoft.Exchange.CtsResources
{
	internal static class DataStrings
	{
		static DataStrings()
		{
			DataStrings.stringIDs.Add(1256740561U, "ErrorPathCanNotBeRoot");
			DataStrings.stringIDs.Add(2058499689U, "ConstraintViolationNoLeadingOrTrailingWhitespace");
		}

		public static string ErrorInvalidFullyQualifiedFileName(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorInvalidFullyQualifiedFileName"), path);
		}

		public static string ErrorFilePathMismatchExpectedExtension(string path, string extension)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorFilePathMismatchExpectedExtension"), path, extension);
		}

		public static string ErrorEdbFileCannotBeUncPath(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileCannotBeUncPath"), pathName);
		}

		public static string ErrorUncPathMustUseServerName(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustUseServerName"), path);
		}

		public static string ErrorEdbFilePathCannotConvert(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFilePathCannotConvert"), pathName);
		}

		public static string ErrorEdbFileNameTooLong(string fileName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileNameTooLong"), fileName);
		}

		public static string ErrorUncPathMustBeUncPath(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustBeUncPath"), path);
		}

		public static string ErrorEdbFileCannotBeTmp(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorEdbFileCannotBeTmp"), pathName);
		}

		public static string ErrorLocalLongFullPathTooLong(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullPathTooLong"), path);
		}

		public static string ErrorUncPathMustBeUncPathOnly(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathMustBeUncPathOnly"), path);
		}

		public static string ErrorPathCanNotBeRoot
		{
			get
			{
				return DataStrings.ResourceManager.GetString("ErrorPathCanNotBeRoot");
			}
		}

		public static string ErrorLocalLongFullPathCannotConvert(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullPathCannotConvert"), pathName);
		}

		public static string ConstraintViolationNoLeadingOrTrailingWhitespace
		{
			get
			{
				return DataStrings.ResourceManager.GetString("ConstraintViolationNoLeadingOrTrailingWhitespace");
			}
		}

		public static string ErrorUncPathTooLong(string path)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorUncPathTooLong"), path);
		}

		public static string ErrorLongPathCannotConvert(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorLongPathCannotConvert"), pathName);
		}

		public static string ErrorInvalidExtension(string extension)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorInvalidExtension"), extension);
		}

		public static string ErrorLocalLongFullAsciiPathCannotConvert(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorLocalLongFullAsciiPathCannotConvert"), pathName);
		}

		public static string ErrorStmFilePathCannotConvert(string pathName)
		{
			return string.Format(DataStrings.ResourceManager.GetString("ErrorStmFilePathCannotConvert"), pathName);
		}

		public static string GetLocalizedString(DataStrings.IDs key)
		{
			return DataStrings.ResourceManager.GetString(DataStrings.stringIDs[(uint)key]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ResourceManager ResourceManager = new ResourceManager("Microsoft.Exchange.CtsResources.DataStrings", typeof(DataStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorPathCanNotBeRoot = 1256740561U,
			ConstraintViolationNoLeadingOrTrailingWhitespace = 2058499689U
		}

		private enum ParamIDs
		{
			ErrorInvalidFullyQualifiedFileName,
			ErrorFilePathMismatchExpectedExtension,
			ErrorEdbFileCannotBeUncPath,
			ErrorUncPathMustUseServerName,
			ErrorEdbFilePathCannotConvert,
			ErrorEdbFileNameTooLong,
			ErrorUncPathMustBeUncPath,
			ErrorEdbFileCannotBeTmp,
			ErrorLocalLongFullPathTooLong,
			ErrorUncPathMustBeUncPathOnly,
			ErrorLocalLongFullPathCannotConvert,
			ErrorUncPathTooLong,
			ErrorLongPathCannotConvert,
			ErrorInvalidExtension,
			ErrorLocalLongFullAsciiPathCannotConvert,
			ErrorStmFilePathCannotConvert
		}
	}
}
