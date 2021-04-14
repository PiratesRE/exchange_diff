using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.Agent.Search
{
	internal sealed class EmailMessageHash
	{
		internal EmailMessageHash(EmailMessage emailMessage)
		{
			if (emailMessage == null)
			{
				throw new ArgumentNullException("emailMessage");
			}
			int num = 0;
			if (emailMessage.Attachments != null)
			{
				num = emailMessage.Attachments.Count;
			}
			int capacity = 8 + 9 * num;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			using (Stream contentReadStream = emailMessage.Body.GetContentReadStream())
			{
				stringBuilder.AppendFormat("{0:X8}", EmailMessageHash.ComputeCRC(contentReadStream));
			}
			if (num > 0)
			{
				foreach (Attachment attachment in emailMessage.Attachments)
				{
					using (Stream contentReadStream2 = attachment.GetContentReadStream())
					{
						stringBuilder.AppendFormat(",{0:X8}", EmailMessageHash.ComputeCRC(contentReadStream2));
					}
				}
			}
			this.cachedHashString = stringBuilder.ToString();
		}

		private EmailMessageHash(string hashString)
		{
			this.cachedHashString = hashString;
		}

		public override string ToString()
		{
			return this.cachedHashString;
		}

		internal static bool TryGetFromHeader(HeaderList mimeHeader, out EmailMessageHash result)
		{
			result = null;
			string property = XHeaderUtils.GetProperty(mimeHeader, "X-MS-Exchange-Forest-EmailMessageHash");
			if (string.IsNullOrEmpty(property))
			{
				return false;
			}
			result = new EmailMessageHash(property);
			return true;
		}

		internal void SetToHeader(HeaderList mimeHeader)
		{
			XHeaderUtils.SetProperty(mimeHeader, "X-MS-Exchange-Forest-EmailMessageHash", this.cachedHashString);
		}

		private static uint ComputeCRC(Stream stream)
		{
			uint num = 0U;
			int num2 = 131072;
			byte[] array = new byte[num2];
			for (;;)
			{
				int num3 = stream.Read(array, 0, num2);
				if (num3 == 0)
				{
					break;
				}
				num = Microsoft.Exchange.Data.Storage.ComputeCRC.Compute(num, array, 0, num3);
			}
			return num;
		}

		private const int HexHashLength = 8;

		private const string BodyHashFormat = "{0:X8}";

		private const string AttachmentHashFormat = ",{0:X8}";

		private const string XHeaderName = "X-MS-Exchange-Forest-EmailMessageHash";

		private readonly string cachedHashString;
	}
}
