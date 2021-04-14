using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class GroupExpansionRecipients
	{
		public List<RecipientToIndex> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public string ToRecipients
		{
			get
			{
				return GroupExpansionRecipients.GetRecipientsByType(RecipientItemType.To, this.recipients);
			}
		}

		public string CcRecipients
		{
			get
			{
				return GroupExpansionRecipients.GetRecipientsByType(RecipientItemType.Cc, this.recipients);
			}
		}

		public string BccRecipients
		{
			get
			{
				return GroupExpansionRecipients.GetRecipientsByType(RecipientItemType.Bcc, this.recipients);
			}
		}

		public static GroupExpansionRecipients Parse(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			GroupExpansionRecipients groupExpansionRecipients = new GroupExpansionRecipients();
			string[] array = data.Split(new string[]
			{
				"|"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string data2 in array)
			{
				groupExpansionRecipients.Recipients.Add(RecipientToIndex.Parse(data2));
			}
			return groupExpansionRecipients;
		}

		public override string ToString()
		{
			if (this.recipients.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (RecipientToIndex recipientToIndex in this.recipients)
			{
				stringBuilder.Append(recipientToIndex.ToString());
				stringBuilder.Append("|");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= "|".Length;
			}
			return stringBuilder.ToString();
		}

		private static string GetRecipientsByType(RecipientItemType itemType, List<RecipientToIndex> recipients)
		{
			string result = string.Empty;
			if (recipients != null && recipients.Count != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int count = recipients.Count;
				for (int i = 0; i < count; i++)
				{
					if (recipients[i].RecipientType.Equals(itemType))
					{
						stringBuilder.Append(string.Format("{1}{0}{2}", ";", recipients[i].DisplayName, recipients[i].EmailAddress));
						if (i < count - 1)
						{
							stringBuilder.Append(",");
						}
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public const string RecipientsSeparatorToken = ",";

		internal const string ItemSeparatorToken = "|";

		private List<RecipientToIndex> recipients = new List<RecipientToIndex>();
	}
}
