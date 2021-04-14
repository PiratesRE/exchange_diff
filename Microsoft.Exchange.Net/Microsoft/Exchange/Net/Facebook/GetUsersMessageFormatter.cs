using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetUsersMessageFormatter : IClientMessageFormatter
	{
		internal GetUsersMessageFormatter(IClientMessageFormatter previousFormatter)
		{
			ArgumentValidator.ThrowIfNull("previousFormatter", previousFormatter);
			this.previousFormatter = previousFormatter;
		}

		public object DeserializeReply(Message message, object[] parameters)
		{
			if (message.IsEmpty || message.IsFault)
			{
				return this.previousFormatter.DeserializeReply(message, parameters);
			}
			object result;
			using (XmlDictionaryReader readerAtBodyContents = message.GetReaderAtBodyContents())
			{
				List<FacebookUser> list = new List<FacebookUser>();
				if (!"root".Equals(readerAtBodyContents.LocalName, StringComparison.Ordinal) && !readerAtBodyContents.ReadToFollowing("root"))
				{
					result = new FacebookUsersList
					{
						Users = new List<FacebookUser>()
					};
				}
				else
				{
					readerAtBodyContents.Read();
					DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FacebookUser), "item");
					while ("item".Equals(readerAtBodyContents.LocalName, StringComparison.Ordinal))
					{
						list.Add((FacebookUser)dataContractJsonSerializer.ReadObject(readerAtBodyContents, false));
					}
					readerAtBodyContents.ReadEndElement();
					result = new FacebookUsersList
					{
						Users = list
					};
				}
			}
			return result;
		}

		public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
		{
			return this.previousFormatter.SerializeRequest(messageVersion, parameters);
		}

		private IClientMessageFormatter previousFormatter;
	}
}
