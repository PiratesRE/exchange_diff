using System;

namespace Microsoft.Exchange.Services.Core
{
	internal class ForwardReplyHeaderOptions
	{
		public int ComposeFontSize
		{
			get
			{
				return 3;
			}
		}

		public string ComposeFontColor
		{
			get
			{
				return "#000000";
			}
		}

		public string ComposeFontName
		{
			get
			{
				return "Calibri";
			}
		}

		public bool ComposeFontBold
		{
			get
			{
				return false;
			}
		}

		public bool ComposeFontUnderline
		{
			get
			{
				return false;
			}
		}

		public bool ComposeFontItalics
		{
			get
			{
				return false;
			}
		}

		public bool AutoAddSignature
		{
			get
			{
				return false;
			}
		}

		public string SignatureText
		{
			get
			{
				return string.Empty;
			}
		}

		private const string ComposeFontNameValue = "Calibri";

		private const int ComposeFontSizeValue = 3;

		private const string ComposeFontColorValue = "#000000";

		private const bool ComposeFontBoldValue = false;

		private const bool ComposeFontUnderlineValue = false;

		private const bool ComposeFontItalicsValue = false;

		private const bool AutoAddSignatureValue = false;
	}
}
