using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class DialGroupEntryComment : DialGroupEntryField
	{
		public DialGroupEntryComment(string comment) : base(comment, "Comment")
		{
		}

		public static DialGroupEntryComment Parse(string comment)
		{
			return new DialGroupEntryComment(comment);
		}

		protected override void Validate()
		{
			if (!string.IsNullOrEmpty(base.Data))
			{
				base.ValidateMaxLength(96);
			}
		}

		public const int MaxLength = 96;
	}
}
