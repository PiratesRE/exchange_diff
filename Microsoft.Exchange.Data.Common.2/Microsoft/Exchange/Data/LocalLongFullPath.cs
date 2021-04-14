using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class LocalLongFullPath : LongPath
	{
		protected LocalLongFullPath()
		{
		}

		public new static LocalLongFullPath Parse(string path)
		{
			return (LocalLongFullPath)LongPath.ParseInternal(path, new LocalLongFullPath());
		}

		public static bool TryParse(string path, out LocalLongFullPath resultObject)
		{
			resultObject = (LocalLongFullPath)LongPath.TryParseInternal(path, new LocalLongFullPath());
			return null != resultObject;
		}

		public static LocalLongFullPath ParseFromPathNameAndFileName(string pathName, string fileName)
		{
			LocalLongFullPath localLongFullPath = LocalLongFullPath.Parse(Path.Combine(pathName, fileName));
			try
			{
				localLongFullPath.ValidateFilePathLength();
			}
			catch (FormatException ex)
			{
				throw new ArgumentException(ex.Message, ex);
			}
			return localLongFullPath;
		}

		public static string ConvertInvalidCharactersInPathName(string fileName)
		{
			return LocalLongFullPath.ConvertInvalidCharactersInternal(fileName, Path.GetInvalidPathChars());
		}

		public static string ConvertInvalidCharactersInFileName(string fileName)
		{
			return LocalLongFullPath.ConvertInvalidCharactersInternal(fileName, Path.GetInvalidFileNameChars());
		}

		protected static string ConvertInvalidCharactersInternal(string fileName, char[] invalidChars)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			fileName = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(fileName));
			Array.Sort<char>(invalidChars);
			StringBuilder stringBuilder = new StringBuilder(fileName.Length + 1);
			foreach (char c in fileName)
			{
				if (Array.BinarySearch<char>(invalidChars, c) < 0 && c != '~')
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append('_');
				}
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				' ',
				'.'
			});
		}

		protected static void ValidatePathWithSpecifiedExtension(LocalLongFullPath path, string specifiedExtension)
		{
			if (string.IsNullOrEmpty(specifiedExtension))
			{
				specifiedExtension = string.Empty;
			}
			else if (specifiedExtension[0] != '.' || specifiedExtension.Length == 1 || -1 != specifiedExtension.IndexOfAny(("." + new string(Path.GetInvalidFileNameChars())).ToCharArray(), 1))
			{
				throw new FormatException(DataStrings.ErrorInvalidExtension(specifiedExtension));
			}
			if (specifiedExtension != null && string.Compare(Path.GetExtension(path.PathName), specifiedExtension, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new FormatException(DataStrings.ErrorFilePathMismatchExpectedExtension(path.PathName, specifiedExtension));
			}
		}

		public void ValidateDirectoryPathLength()
		{
			LocalLongFullPath.ValidateDirectoryPath(base.PathName);
		}

		protected static void ValidateDirectoryPath(string input)
		{
			try
			{
				string text = Path.GetFullPath(input);
				if (Path.DirectorySeparatorChar == text[text.Length - 1] || Path.AltDirectorySeparatorChar == text[text.Length - 1])
				{
					text = Path.GetDirectoryName(text);
				}
				if (!string.IsNullOrEmpty(text) && 248 <= text.Length)
				{
					throw new PathTooLongException(DataStrings.ErrorLocalLongFullPathTooLong(input));
				}
			}
			catch (IOException ex)
			{
				throw new FormatException(ex.Message, ex);
			}
		}

		protected static void ValidateFilePath(string input)
		{
			try
			{
				string text = Path.GetFullPath(input);
				if (Path.DirectorySeparatorChar == text[text.Length - 1] || Path.AltDirectorySeparatorChar == text[text.Length - 1])
				{
					throw new FormatException(DataStrings.ErrorInvalidFullyQualifiedFileName(input));
				}
				if (Path.IsPathRooted(text) && 259 <= text.Length)
				{
					throw new PathTooLongException(DataStrings.ErrorLocalLongFullPathTooLong(input));
				}
				text = Path.GetDirectoryName(text);
				if (!string.IsNullOrEmpty(text) && 248 <= text.Length)
				{
					throw new PathTooLongException(DataStrings.ErrorLocalLongFullPathTooLong(input));
				}
			}
			catch (IOException ex)
			{
				throw new FormatException(ex.Message, ex);
			}
		}

		protected override bool ParseCore(string path, bool nothrow)
		{
			if (base.ParseCore(path, nothrow))
			{
				if (!base.IsLocalFull)
				{
					base.IsValid = false;
				}
				else
				{
					try
					{
						Path.GetFullPath(base.PathName);
					}
					catch (IOException ex)
					{
						base.IsValid = false;
						if (!nothrow)
						{
							throw new ArgumentException(ex.Message, ex);
						}
					}
				}
			}
			if (!base.IsValid && !nothrow)
			{
				throw new ArgumentException(DataStrings.ErrorLocalLongFullPathCannotConvert(path), "path");
			}
			return base.IsValid;
		}

		public void ValidateFilePathLength()
		{
			LocalLongFullPath.ValidateFilePath(base.PathName);
		}

		private const int MaxDirectoryPath = 248;

		private const int MaxPath = 260;
	}
}
