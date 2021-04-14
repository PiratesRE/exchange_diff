using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class UnknownRoleEntry : RoleEntry, IEquatable<UnknownRoleEntry>
	{
		internal UnknownRoleEntry(string entryString)
		{
			this.typeHint = entryString[0];
			int paramIndex = base.ExtractAndSetName(entryString);
			base.ExtractAndSetParameters(entryString, paramIndex);
		}

		internal UnknownRoleEntry()
		{
		}

		public override string ToADString()
		{
			return base.ToADString(this.typeHint, null, null);
		}

		public bool Equals(UnknownRoleEntry other)
		{
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as UnknownRoleEntry);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private readonly char typeHint;
	}
}
