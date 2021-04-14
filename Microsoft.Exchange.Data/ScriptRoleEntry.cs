using System;
using System.IO;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class ScriptRoleEntry : RoleEntry, IEquatable<ScriptRoleEntry>
	{
		internal ScriptRoleEntry(string name, string[] parameters) : base(name, parameters)
		{
		}

		internal ScriptRoleEntry(string entryString)
		{
			int paramIndex = base.ExtractAndSetName(entryString);
			if (-1 != base.Name.IndexOfAny(ScriptRoleEntry.invalidFileNameChars))
			{
				throw new FormatException(DataStrings.ScriptRoleEntryNameInvalidException(base.Name));
			}
			base.ExtractAndSetParameters(entryString, paramIndex);
		}

		internal ScriptRoleEntry()
		{
		}

		public override string ToADString()
		{
			return base.ToADString('s', null, null);
		}

		public bool Equals(ScriptRoleEntry other)
		{
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ScriptRoleEntry);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal const char TypeHint = 's';

		private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
	}
}
