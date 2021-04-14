using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DialGroupEntryName : DialGroupEntryField
	{
		public DialGroupEntryName(string name) : base((name == null) ? null : name.Trim(), "Name")
		{
		}

		public static DialGroupEntryName Parse(string name)
		{
			return new DialGroupEntryName(name);
		}

		protected override void Validate()
		{
			base.ValidateNullOrEmpty();
			base.ValidateMaxLength(32);
		}

		public const int MaxLength = 32;
	}
}
