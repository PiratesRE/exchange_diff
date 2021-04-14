using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class MailboxFolderId : ObjectId
	{
		public MailboxFolderId(string mailboxName, string url)
		{
			if (string.IsNullOrEmpty(mailboxName))
			{
				throw new ArgumentNullException("mailboxName");
			}
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.mailboxName = mailboxName;
			this.path = url.Replace('/', '\\');
		}

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.ToString());
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.mailboxName);
			stringBuilder.Append(this.path);
			return stringBuilder.ToString();
		}

		public static MailboxFolderId Parse(string folderIdentity)
		{
			if (string.IsNullOrEmpty(folderIdentity))
			{
				throw new ArgumentNullException("folderIdentity");
			}
			int num = folderIdentity.IndexOf('\\');
			if (num <= 0 || num >= folderIdentity.Length - 1)
			{
				throw new ArgumentException(Strings.InvalidMailboxFolderIdentity(folderIdentity), "folderIdentity");
			}
			string text = folderIdentity.Substring(0, num);
			string url = folderIdentity.Substring(num);
			return new MailboxFolderId(text, url);
		}

		private readonly string mailboxName;

		private readonly string path;
	}
}
