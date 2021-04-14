using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	internal class ForwardReplyHeaderOptions
	{
		public int ComposeFontSize
		{
			get
			{
				return 2;
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
				return "Tahoma";
			}
		}

		public bool ComposeFontBold
		{
			get
			{
				return true;
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
				return "";
			}
		}

		private const string ComposeFontNameValue = "Tahoma";

		private const int ComposeFontSizeValue = 2;

		private const string ComposeFontColorValue = "#000000";

		private const bool ComposeFontBoldValue = true;

		private const bool ComposeFontUnderlineValue = false;

		private const bool ComposeFontItalicsValue = false;

		private const bool AutoAddSignatureValue = false;

		private const string SignatureTextValue = "";
	}
}
