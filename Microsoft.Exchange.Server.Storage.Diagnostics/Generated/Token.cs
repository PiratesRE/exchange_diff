using System;

namespace Microsoft.Exchange.Server.Storage.Diagnostics.Generated
{
	public struct Token
	{
		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public string ValueAsString
		{
			get
			{
				return this.value as string;
			}
		}

		public string ValueAsIdentifier
		{
			get
			{
				if (this.ValueAsString != null && this.ValueAsString.StartsWith("[") && this.ValueAsString.EndsWith("]"))
				{
					return this.TrimOuter();
				}
				return this.ValueAsString;
			}
		}

		public string ValueAsSubtractor
		{
			get
			{
				string text = this.ValueAsString;
				if (text == null)
				{
					return null;
				}
				if (text.Length < 1 || !text.StartsWith("-"))
				{
					return text;
				}
				text = text.Substring(1);
				if (text.StartsWith("[") && text.EndsWith("]"))
				{
					return text.Substring(1, text.Length - 2);
				}
				return text;
			}
		}

		public string TrimOuter()
		{
			if (this.ValueAsString != null && this.ValueAsString.Length > 1)
			{
				return this.ValueAsString.Substring(1, this.ValueAsString.Length - 2);
			}
			return this.ValueAsString;
		}

		private const string IdentifierBegin = "[";

		private const string IdentifierFinish = "]";

		private const string SubtractorPrefix = "-";

		private object value;
	}
}
