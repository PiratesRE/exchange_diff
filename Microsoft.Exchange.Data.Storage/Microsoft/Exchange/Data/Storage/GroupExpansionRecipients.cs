using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GroupExpansionRecipients
	{
		public List<RecipientToIndex> Recipients
		{
			get
			{
				return this.recipients;
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

		internal static GroupExpansionRecipients RetrieveFromStore(MessageItem messageItem, StorePropertyDefinition propertyDefinition)
		{
			if (messageItem == null)
			{
				return null;
			}
			try
			{
				object propertyValue = messageItem.TryGetProperty(propertyDefinition);
				if (PropertyError.IsPropertyNotFound(propertyValue))
				{
					return null;
				}
				if (PropertyError.IsPropertyValueTooBig(propertyValue))
				{
					using (Stream stream = messageItem.OpenPropertyStream(propertyDefinition, PropertyOpenMode.ReadOnly))
					{
						Encoding encoding = new UnicodeEncoding(false, false);
						using (StreamReader streamReader = new StreamReader(stream, encoding))
						{
							string data = streamReader.ReadToEnd();
							return GroupExpansionRecipients.Parse(data);
						}
					}
				}
				if (PropertyError.IsPropertyError(propertyValue))
				{
					return null;
				}
				string text = messageItem[propertyDefinition] as string;
				if (!string.IsNullOrEmpty(text))
				{
					return GroupExpansionRecipients.Parse(text);
				}
			}
			catch (PropertyErrorException)
			{
				return null;
			}
			return null;
		}

		internal void SaveToStore(MessageItem messageItem, StorePropertyDefinition propertyDefinition)
		{
			string value = this.ToString();
			if (string.IsNullOrEmpty(value))
			{
				messageItem[propertyDefinition] = null;
				return;
			}
			using (Stream stream = messageItem.OpenPropertyStream(propertyDefinition, PropertyOpenMode.Create))
			{
				Encoding encoding = new UnicodeEncoding(false, false);
				using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
				{
					streamWriter.Write(value);
					streamWriter.Flush();
				}
			}
		}

		internal const string ItemSeparatorToken = "|";

		private List<RecipientToIndex> recipients = new List<RecipientToIndex>();
	}
}
