using System;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class Breadcrumb
	{
		public Breadcrumb(string message)
		{
			if (message.Length > 128)
			{
				this.message = message.Substring(0, 128);
				return;
			}
			this.message = message;
		}

		public override string ToString()
		{
			return this.timeDropped + " " + this.message;
		}

		internal const int MaxCharactersPerLine = 128;

		private DateTime timeDropped = DateTime.UtcNow;

		private string message;
	}
}
