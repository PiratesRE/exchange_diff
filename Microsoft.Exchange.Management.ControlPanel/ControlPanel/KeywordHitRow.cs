using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class KeywordHitRow
	{
		private KeywordHit KeywordHit { get; set; }

		public KeywordHitRow(KeywordHit kwh)
		{
			this.KeywordHit = kwh;
		}

		public string Phrase
		{
			get
			{
				return this.KeywordHit.Phrase;
			}
		}

		public int Count
		{
			get
			{
				return this.KeywordHit.Count;
			}
		}

		public int MailboxCount
		{
			get
			{
				return this.KeywordHit.MailboxCount;
			}
		}
	}
}
