using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class CmdletRoleEntry : RoleEntry, IEquatable<CmdletRoleEntry>
	{
		internal CmdletRoleEntry(string name, string snapinName, string[] parameters) : base(name, parameters)
		{
			if (string.IsNullOrEmpty(snapinName))
			{
				throw new FormatException(DataStrings.SnapinNameTooShort);
			}
			if (RoleEntry.ContainsInvalidChars(snapinName))
			{
				throw new FormatException(DataStrings.SnapinNameInvalidCharException(snapinName));
			}
			this.snapinName = snapinName;
			this.fullName = this.snapinName + "\\" + base.Name;
		}

		internal CmdletRoleEntry(string entryString)
		{
			int num = base.ExtractAndSetName(entryString);
			if (num <= 0 || num == entryString.Length)
			{
				throw new FormatException(DataStrings.SnapinNameTooShort);
			}
			num = this.ParseVersion(entryString, num);
			int num2 = entryString.IndexOf(',', num) + 1;
			int num3 = ((num2 <= 0) ? entryString.Length : (num2 - 1)) - num;
			if (num3 < 1)
			{
				throw new FormatException(DataStrings.SnapinNameTooShort);
			}
			base.ExtractAndSetParameters(entryString, num2);
			string text = entryString.Substring(num, num3);
			if (RoleEntry.ContainsInvalidChars(text))
			{
				throw new FormatException(DataStrings.SnapinNameInvalidCharException(text));
			}
			this.snapinName = text;
			this.fullName = this.snapinName + "\\" + base.Name;
		}

		private int ParseVersion(string entryString, int snapinIndex)
		{
			if (snapinIndex + 8 >= entryString.Length || entryString.IndexOf('v', snapinIndex, 1) != snapinIndex || entryString.IndexOf('v', snapinIndex + 4, 1) != snapinIndex + 4 || !char.IsDigit(entryString, snapinIndex + 1) || !char.IsDigit(entryString, snapinIndex + 2) || !char.IsDigit(entryString, snapinIndex + 5) || !char.IsDigit(entryString, snapinIndex + 6) || entryString.IndexOf(',', snapinIndex + 3, 1) != snapinIndex + 3 || entryString.IndexOf(',', snapinIndex + 7, 1) != snapinIndex + 7)
			{
				return snapinIndex;
			}
			this.versionInfo = entryString.Substring(snapinIndex, 7);
			return snapinIndex + 8;
		}

		internal CmdletRoleEntry(CmdletRoleEntry entryToClone)
		{
			this.snapinName = entryToClone.snapinName;
			this.fullName = entryToClone.fullName;
		}

		internal CmdletRoleEntry(CmdletRoleEntry entryToClone, string newName) : this(entryToClone)
		{
			if (!string.IsNullOrWhiteSpace(newName))
			{
				this.fullName = entryToClone.snapinName + "\\" + newName;
			}
		}

		public string PSSnapinName
		{
			get
			{
				return this.snapinName;
			}
		}

		internal string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		public override string ToString()
		{
			return base.ToString(this.PSSnapinName);
		}

		public override string ToADString()
		{
			return base.ToADString('c', this.PSSnapinName, this.versionInfo);
		}

		internal new static void ValidateName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new FormatException(DataStrings.CmdletFullNameFormatException(name ?? string.Empty));
			}
			int num = name.IndexOf(',');
			if (-1 == num)
			{
				if (RoleEntry.ContainsInvalidChars(name))
				{
					throw new FormatException(DataStrings.CmdletFullNameFormatException(name));
				}
			}
			else
			{
				if (num == 0 || name.Length - 1 == num)
				{
					throw new FormatException(DataStrings.CmdletFullNameFormatException(name));
				}
				if (RoleEntry.ContainsInvalidChars(name, 0, num))
				{
					throw new FormatException(DataStrings.CmdletFullNameFormatException(name));
				}
				if (RoleEntry.ContainsInvalidChars(name, 1 + num, name.Length - num - 1))
				{
					throw new FormatException(DataStrings.CmdletFullNameFormatException(name));
				}
			}
		}

		public bool Equals(CmdletRoleEntry other)
		{
			return other != null && this.PSSnapinName.Equals(other.PSSnapinName, StringComparison.OrdinalIgnoreCase) && base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CmdletRoleEntry);
		}

		public override int GetHashCode()
		{
			return base.Name.GetHashCode();
		}

		internal const char TypeHint = 'c';

		private string snapinName;

		private string fullName;

		private string versionInfo;
	}
}
