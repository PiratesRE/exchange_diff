using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecipientToIndex
	{
		internal RecipientToIndex(RecipientItemType recipientType, string displayName, string emailAddress)
		{
			if (recipientType != RecipientItemType.To && recipientType != RecipientItemType.Cc && recipientType != RecipientItemType.Bcc)
			{
				throw new ArgumentException("Only TO, CC, and BCC recipient type is supported.");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (emailAddress == null)
			{
				throw new ArgumentNullException("emailAddress");
			}
			this.RecipientType = recipientType;
			this.DisplayName = RecipientToIndex.SanitizeDisplayName(displayName);
			this.EmailAddress = emailAddress;
		}

		internal RecipientItemType RecipientType { get; private set; }

		public string DisplayName { get; private set; }

		public string EmailAddress { get; private set; }

		public static RecipientToIndex Parse(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException("data");
			}
			string[] array = data.Split(new string[]
			{
				";"
			}, StringSplitOptions.None);
			if (array.Length < 3)
			{
				throw new FormatException(string.Format("Invalid input data: {0}", data));
			}
			RecipientItemType recipientType = RecipientItemType.To;
			Enum.TryParse<RecipientItemType>(array[0], out recipientType);
			string displayName = array[1];
			string emailAddress = array[2];
			return new RecipientToIndex(recipientType, displayName, emailAddress);
		}

		public override string ToString()
		{
			return string.Format("{1}{0}{2}{0}{3}", new object[]
			{
				";",
				this.RecipientType.ToString(),
				this.DisplayName,
				this.EmailAddress
			});
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			RecipientToIndex recipientToIndex = obj as RecipientToIndex;
			return recipientToIndex != null && (recipientToIndex.RecipientType == this.RecipientType && string.Compare(recipientToIndex.DisplayName, this.DisplayName, StringComparison.OrdinalIgnoreCase) == 0) && string.Compare(recipientToIndex.EmailAddress, this.EmailAddress, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static string SanitizeDisplayName(string displayName)
		{
			return displayName.Replace(";", string.Empty).Replace("|", string.Empty);
		}

		internal const string SeparatorToken = ";";
	}
}
