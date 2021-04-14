using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	public class ReplyBodySerializer
	{
		internal static ReplyBodySerializer Serialize(ReplyBody replyBody)
		{
			return new ReplyBodySerializer
			{
				Message = replyBody.RawMessage,
				LanguageTag = replyBody.LanguageTag
			};
		}

		internal ReplyBody Deserialize()
		{
			ReplyBody replyBody = ReplyBody.Create();
			replyBody.RawMessage = this.Message;
			replyBody.LanguageTag = this.LanguageTag;
			return replyBody;
		}

		[XmlElement]
		public string Message = string.Empty;

		[XmlElement]
		public string LanguageTag;
	}
}
