using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class ApplicationPermissionRoleEntry : RoleEntry, IEquatable<ApplicationPermissionRoleEntry>
	{
		internal ApplicationPermissionRoleEntry(string name, string[] parameters) : base(name, parameters)
		{
			if (parameters != null && parameters.Length > 0)
			{
				throw new FormatException(DataStrings.ApplicationPermissionRoleEntryParameterNotEmptyException(base.Name));
			}
		}

		internal ApplicationPermissionRoleEntry(string entryString)
		{
			int num = base.ExtractAndSetName(entryString);
			if (num > 0)
			{
				throw new FormatException(DataStrings.ApplicationPermissionRoleEntryParameterNotEmptyException(base.Name));
			}
			base.ExtractAndSetParameters(entryString, num);
		}

		internal ApplicationPermissionRoleEntry()
		{
		}

		public override string ToADString()
		{
			return base.ToADString('a', null, null);
		}

		public bool Equals(ApplicationPermissionRoleEntry other)
		{
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ApplicationPermissionRoleEntry);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal const char TypeHint = 'a';
	}
}
