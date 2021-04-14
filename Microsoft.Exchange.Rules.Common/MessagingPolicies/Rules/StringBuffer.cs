using System;
using Microsoft.Exchange.TextMatching;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class StringBuffer : ITextInputBuffer
	{
		public StringBuffer(string text)
		{
			this.text = text;
			this.index = -1;
		}

		public int NextChar
		{
			get
			{
				if (this.index == -1)
				{
					this.index++;
					return 1;
				}
				if (string.IsNullOrEmpty(this.text) || this.index == this.text.Length)
				{
					return -1;
				}
				return (int)char.ToLowerInvariant(this.text[this.index++]);
			}
		}

		private string text;

		private int index;
	}
}
