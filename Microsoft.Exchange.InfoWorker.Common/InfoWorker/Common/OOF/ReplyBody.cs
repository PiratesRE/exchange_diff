using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class ReplyBody
	{
		[XmlElement]
		public string Message
		{
			get
			{
				if (this.SetByLegacyClient)
				{
					return TextUtil.ConvertPlainTextToHtml(this.message);
				}
				return this.message;
			}
			set
			{
				if (value != null && value.Length > ReplyBody.MaxMessageLength)
				{
					value = value.Substring(0, ReplyBody.MaxMessageLength);
				}
				this.message = value;
			}
		}

		[XmlIgnore]
		internal string RawMessage
		{
			get
			{
				return this.message;
			}
			set
			{
				if (value != null && value.Length > ReplyBody.MaxMessageLength)
				{
					value = value.Substring(0, ReplyBody.MaxMessageLength);
				}
				this.message = value;
			}
		}

		[XmlIgnore]
		internal bool SetByLegacyClient
		{
			get
			{
				return this.setByLegacyClient;
			}
			set
			{
				this.setByLegacyClient = value;
			}
		}

		[XmlAttribute("xml:lang")]
		public string LanguageTag
		{
			get
			{
				return this.languageTag;
			}
			set
			{
				this.languageTag = value;
			}
		}

		internal static ReplyBody Create()
		{
			return new ReplyBody();
		}

		internal static ReplyBody CreateDefault()
		{
			return new ReplyBody
			{
				RawMessage = string.Empty,
				SetByLegacyClient = false
			};
		}

		private ReplyBody()
		{
		}

		private string message;

		private string languageTag;

		private bool setByLegacyClient;

		public static int MaxMessageLength = 128000;
	}
}
