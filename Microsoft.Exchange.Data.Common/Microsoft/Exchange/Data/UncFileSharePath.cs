using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class UncFileSharePath : LongPath
	{
		public string ShareName
		{
			get
			{
				return this.shareName;
			}
		}

		public new static UncFileSharePath Parse(string pathName)
		{
			return (UncFileSharePath)LongPath.ParseInternal(pathName, new UncFileSharePath());
		}

		public static bool TryParse(string path, out UncFileSharePath resultObject)
		{
			resultObject = (UncFileSharePath)LongPath.TryParseInternal(path, new UncFileSharePath());
			return null != resultObject;
		}

		protected override bool ParseCore(string path, bool nothrow)
		{
			if (base.ParseCore(path, nothrow))
			{
				if (!base.IsUnc)
				{
					base.IsValid = false;
					if (!nothrow)
					{
						throw new ArgumentException(DataStrings.ErrorUncPathMustBeUncPath(path), "path");
					}
				}
				else
				{
					if (path.Length >= 260)
					{
						throw new PathTooLongException(DataStrings.ErrorUncPathTooLong(path));
					}
					Match match = UncFileSharePath.s_regexUncShare.Match(base.PathName);
					if (!match.Success)
					{
						base.IsValid = false;
						if (!nothrow)
						{
							throw new ArgumentException(DataStrings.ErrorUncPathMustBeUncPathOnly(path), "path");
						}
					}
					else
					{
						IPAddress ipaddress;
						if (IPAddress.TryParse(match.Groups[1].ToString(), out ipaddress))
						{
							base.IsValid = false;
							if (!nothrow)
							{
								throw new ArgumentException(DataStrings.ErrorUncPathMustUseServerName(path), "path");
							}
						}
						this.shareName = match.Groups[2].ToString();
					}
					match = UncFileSharePath.s_regexLeadingWhitespace.Match(base.PathName);
					bool success = match.Success;
					match = UncFileSharePath.s_regexTrailingWhitespace.Match(base.PathName);
					if (match.Success || success)
					{
						base.IsValid = false;
						if (!nothrow)
						{
							throw new ArgumentException(DataStrings.ConstraintViolationNoLeadingOrTrailingWhitespace, "path");
						}
					}
				}
			}
			return base.IsValid;
		}

		private const int MaxPath = 260;

		private static Regex s_regexUncShare = new Regex("^\\\\\\\\([^\\\\]+)\\\\([^\\\\]+)$", RegexOptions.CultureInvariant);

		private static Regex s_regexLeadingWhitespace = new Regex("^\\s+", RegexOptions.CultureInvariant);

		private static Regex s_regexTrailingWhitespace = new Regex("\\s+$", RegexOptions.CultureInvariant);

		private string shareName;
	}
}
