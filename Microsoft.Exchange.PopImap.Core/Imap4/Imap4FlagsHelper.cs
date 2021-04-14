using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Imap4
{
	internal sealed class Imap4FlagsHelper
	{
		public static bool TryParse(string flagString, out Imap4Flags flags, out bool containsKeywords)
		{
			flags = Imap4Flags.None;
			containsKeywords = false;
			if (!string.IsNullOrEmpty(flagString))
			{
				if (flagString[0] == '(' && flagString[flagString.Length - 1] == ')')
				{
					flagString = flagString.Substring(1, flagString.Length - 2);
					if (string.IsNullOrEmpty(flagString))
					{
						return true;
					}
				}
				string[] array = flagString.Split(Imap4FlagsHelper.wordDelimiter);
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Compare(array[i], "\\Recent", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Recent;
					}
					else if (string.Compare(array[i], "\\Seen", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Seen;
					}
					else if (string.Compare(array[i], "\\Deleted", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Deleted;
					}
					else if (string.Compare(array[i], "\\Answered", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Answered;
					}
					else if (string.Compare(array[i], "\\Draft", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Draft;
					}
					else if (string.Compare(array[i], "\\Flagged", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Flagged;
					}
					else if (string.Compare(array[i], "$MDNSent", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.MdnSent;
					}
					else if (string.Compare(array[i], "\\Wildcard", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flags |= Imap4Flags.Wildcard;
					}
					else
					{
						if (array[i].Length <= 0 || array[i].IndexOfAny(Imap4FlagsHelper.specialCharacters) != -1)
						{
							return false;
						}
						containsKeywords = true;
					}
				}
			}
			return true;
		}

		internal static Imap4Flags Parse(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item is null");
			}
			object delMarked = item.TryGetProperty(MessageItemSchema.MessageDelMarked);
			object answered = item.TryGetProperty(MessageItemSchema.MessageAnswered);
			object tagged = item.TryGetProperty(MessageItemSchema.MessageTagged);
			object notificationSent = item.TryGetProperty(MessageItemSchema.MessageDeliveryNotificationSent);
			object conversionFailed = item.TryGetProperty(MessageItemSchema.MimeConversionFailed);
			object isDraft = item.TryGetProperty(MessageItemSchema.IsDraft);
			object isRead = item.TryGetProperty(MessageItemSchema.IsRead);
			return Imap4FlagsHelper.Parse(delMarked, answered, tagged, notificationSent, conversionFailed, isDraft, isRead);
		}

		internal static Imap4Flags Parse(object delMarked, object answered, object tagged, object notificationSent, object conversionFailed, object isDraft, object isRead)
		{
			Imap4Flags result = Imap4Flags.None;
			Imap4FlagsHelper.SetFlagBit(ref result, delMarked, Imap4Flags.Deleted);
			Imap4FlagsHelper.SetFlagBit(ref result, answered, Imap4Flags.Answered);
			Imap4FlagsHelper.SetFlagBit(ref result, tagged, Imap4Flags.Flagged);
			Imap4FlagsHelper.SetFlagBit(ref result, notificationSent, Imap4Flags.MdnSent);
			Imap4FlagsHelper.SetFlagBit(ref result, conversionFailed, Imap4Flags.MimeFailed);
			Imap4FlagsHelper.SetFlagBit(ref result, isDraft, Imap4Flags.Draft);
			Imap4FlagsHelper.SetFlagBit(ref result, isRead, Imap4Flags.Seen);
			return result;
		}

		internal static void Apply(Item item, Imap4Flags flags)
		{
			item[MessageItemSchema.MessageDelMarked] = ((flags & Imap4Flags.Deleted) != Imap4Flags.None);
			item[MessageItemSchema.MessageAnswered] = ((flags & Imap4Flags.Answered) != Imap4Flags.None);
			item[MessageItemSchema.MessageTagged] = ((flags & Imap4Flags.Flagged) != Imap4Flags.None);
			item[MessageItemSchema.MessageDeliveryNotificationSent] = ((flags & Imap4Flags.MdnSent) != Imap4Flags.None);
			item[MessageItemSchema.MimeConversionFailed] = ((flags & Imap4Flags.MimeFailed) != Imap4Flags.None);
			item[MessageItemSchema.IsDraft] = ((flags & Imap4Flags.Draft) != Imap4Flags.None);
			item[MessageItemSchema.IsRead] = ((flags & Imap4Flags.Seen) != Imap4Flags.None);
		}

		internal static string ToString(Imap4Flags flags)
		{
			int num = (int)(flags & (Imap4Flags.Recent | Imap4Flags.Seen | Imap4Flags.Deleted | Imap4Flags.Answered | Imap4Flags.Draft | Imap4Flags.Flagged | Imap4Flags.MdnSent | Imap4Flags.Wildcard));
			string text = Imap4FlagsHelper.flagStrings[num];
			if (text != null)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append('(');
			if ((flags & Imap4Flags.Seen) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Seen");
			}
			if ((flags & Imap4Flags.Answered) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Answered");
			}
			if ((flags & Imap4Flags.Flagged) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Flagged");
			}
			if ((flags & Imap4Flags.Deleted) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Deleted");
			}
			if ((flags & Imap4Flags.Draft) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Draft");
			}
			if ((flags & Imap4Flags.MdnSent) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "$MDNSent");
			}
			if ((flags & Imap4Flags.Wildcard) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Wildcard");
			}
			if ((flags & Imap4Flags.Recent) != Imap4Flags.None)
			{
				Imap4FlagsHelper.AddString(stringBuilder, "\\Recent");
			}
			stringBuilder.Append(')');
			text = stringBuilder.ToString();
			Imap4FlagsHelper.flagStrings[num] = text;
			return text;
		}

		private static void AddString(StringBuilder stringBuilder, string str)
		{
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(str);
		}

		private static void SetFlagBit(ref Imap4Flags flags, object bitObject, Imap4Flags flagBit)
		{
			if (!(bitObject is PropertyError) && (bool)bitObject)
			{
				flags |= flagBit;
			}
		}

		internal const string Recent = "\\Recent";

		internal const string Seen = "\\Seen";

		internal const string Deleted = "\\Deleted";

		internal const string Answered = "\\Answered";

		internal const string Draft = "\\Draft";

		internal const string Flagged = "\\Flagged";

		internal const string MdnSent = "$MDNSent";

		internal const string Wildcard = "\\Wildcard";

		private static readonly char[] wordDelimiter = new char[]
		{
			' '
		};

		private static char[] specialCharacters = new char[]
		{
			'\\',
			'{',
			'(',
			')',
			'*',
			'%',
			']',
			'"'
		};

		private static string[] flagStrings = new string[256];
	}
}
