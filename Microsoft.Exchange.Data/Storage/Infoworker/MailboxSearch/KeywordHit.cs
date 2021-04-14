using System;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	public class KeywordHit
	{
		public string Phrase { get; set; }

		public int Count { get; set; }

		public ByteQuantifiedSize Size { get; set; }

		public int MailboxCount { get; set; }

		public static KeywordHit Parse(string value)
		{
			string[] array = value.Split(new char[]
			{
				'\t'
			});
			int count = int.Parse(array[array.Length - 3]);
			ByteQuantifiedSize size = ByteQuantifiedSize.Parse(array[array.Length - 2]);
			int mailboxCount = int.Parse(array[array.Length - 1]);
			return new KeywordHit
			{
				Phrase = string.Join("\t", array, 0, array.Length - 3),
				Count = count,
				Size = size,
				MailboxCount = mailboxCount
			};
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.Phrase,
				'\t',
				this.Count,
				'\t',
				this.Size,
				'\t',
				this.MailboxCount
			});
		}

		public const string UnsearchablePhrase = "652beee2-75f7-4ca0-8a02-0698a3919cb9";
	}
}
