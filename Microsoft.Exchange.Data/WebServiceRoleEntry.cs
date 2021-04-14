using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class WebServiceRoleEntry : RoleEntry, IEquatable<WebServiceRoleEntry>
	{
		internal WebServiceRoleEntry(string name, string[] parameters) : base(name, parameters)
		{
			if (parameters != null && parameters.Length > 0)
			{
				throw new FormatException(DataStrings.WebServiceRoleEntryParameterNotEmptyException(base.Name));
			}
		}

		internal WebServiceRoleEntry(string entryString)
		{
			int num = base.ExtractAndSetName(entryString);
			if (num > 0)
			{
				throw new FormatException(DataStrings.WebServiceRoleEntryParameterNotEmptyException(base.Name));
			}
			base.ExtractAndSetParameters(entryString, num);
		}

		internal WebServiceRoleEntry()
		{
		}

		public override string ToADString()
		{
			return base.ToADString('w', null, null);
		}

		public bool Equals(WebServiceRoleEntry other)
		{
			return base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as WebServiceRoleEntry);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal const char TypeHint = 'w';
	}
}
