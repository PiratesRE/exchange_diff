using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class EdbFilePath : LocalLongFullPath
	{
		private bool IsTemporaryEdbFile
		{
			get
			{
				return 0 == string.Compare(Path.GetFileName(base.PathName), "tmp.edb", StringComparison.OrdinalIgnoreCase);
			}
		}

		public bool IsPathInRootDirectory
		{
			get
			{
				if (!base.IsValid)
				{
					throw new NotSupportedException("IsPathInRootDirectory");
				}
				if (this.isPathInRootDirectory == null)
				{
					string directoryName = Path.GetDirectoryName(base.PathName);
					if (string.IsNullOrEmpty(directoryName) || string.IsNullOrEmpty(Path.GetDirectoryName(directoryName)))
					{
						this.isPathInRootDirectory = new bool?(true);
					}
					else
					{
						this.isPathInRootDirectory = new bool?(false);
					}
				}
				return this.isPathInRootDirectory.Value;
			}
		}

		public new static EdbFilePath Parse(string pathName)
		{
			return (EdbFilePath)LongPath.ParseInternal(pathName, new EdbFilePath());
		}

		public static bool TryParse(string path, out EdbFilePath resultObject)
		{
			resultObject = (EdbFilePath)LongPath.TryParseInternal(path, new EdbFilePath());
			return null != resultObject;
		}

		protected override bool ParseCore(string path, bool nothrow)
		{
			if (base.ParseCore(path, nothrow))
			{
				if (this.IsTemporaryEdbFile)
				{
					base.IsValid = false;
					if (!nothrow)
					{
						throw new ArgumentException(DataStrings.ErrorEdbFileCannotBeTmp(path), "path");
					}
				}
				else
				{
					string fileName = Path.GetFileName(base.PathName);
					try
					{
						LocalLongFullPath.ValidateFilePath(Path.Combine(Path.Combine(EdbFilePath.DefaultEdbFilePath, "LocalCopies"), fileName) + "0000");
					}
					catch (FormatException innerException)
					{
						base.IsValid = false;
						if (!nothrow)
						{
							throw new ArgumentException(DataStrings.ErrorEdbFileNameTooLong(fileName), innerException);
						}
					}
				}
			}
			if (!base.IsValid && !nothrow)
			{
				throw new ArgumentException(DataStrings.ErrorEdbFilePathCannotConvert(path));
			}
			return base.IsValid;
		}

		public new static EdbFilePath ParseFromPathNameAndFileName(string pathName, string fileName)
		{
			return EdbFilePath.Parse(Path.Combine(pathName, fileName));
		}

		public void ValidateEdbFileExtension()
		{
			LocalLongFullPath.ValidatePathWithSpecifiedExtension(this, ".edb");
		}

		private const string EdbFileExtensionString = ".edb";

		public const string TemporaryDatabaseFileName = "tmp.edb";

		public const string DefaultLocalCopyDirectoryName = "LocalCopies";

		public const string MaximumRetrySuffix = "0000";

		private bool? isPathInRootDirectory;

		public static readonly string DefaultEdbFilePath = Path.GetFullPath("X:\\Program Files\\");
	}
}
