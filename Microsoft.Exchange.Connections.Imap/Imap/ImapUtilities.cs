using System;
using System.Text;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Imap
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ImapUtilities
	{
		internal static string ToModifiedUTF7(string decodedString)
		{
			if (string.IsNullOrEmpty(decodedString))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(decodedString.Length * 3);
			char[] array = null;
			for (int i = 0; i < decodedString.Length; i++)
			{
				if (decodedString[i] <= '\u007f' && decodedString[i] != ImapUtilities.cShiftUTF7)
				{
					int num = i;
					while (i < decodedString.Length && decodedString[i] <= '\u007f' && decodedString[i] != ImapUtilities.cShiftUTF7)
					{
						i++;
					}
					if (num == 0 && i == decodedString.Length)
					{
						return decodedString;
					}
					stringBuilder.Append(decodedString.Substring(num, i - num));
					i--;
				}
				else if (decodedString[i] == ImapUtilities.cShiftUTF7)
				{
					stringBuilder.Append(ImapUtilities.cShiftUTF7);
					stringBuilder.Append(ImapUtilities.cShiftASCII);
				}
				else
				{
					if (array == null)
					{
						array = decodedString.ToCharArray();
					}
					int num = i;
					while (i < decodedString.Length && decodedString[i] > '\u007f')
					{
						i++;
					}
					byte[] bytes = Encoding.UTF7.GetBytes(array, num, i - num);
					bytes[0] = (byte)ImapUtilities.cShiftUTF7;
					for (int j = 0; j < bytes.Length; j++)
					{
						if (bytes[j] == ImapUtilities.bSlash)
						{
							stringBuilder.Append(ImapUtilities.cComma);
						}
						else
						{
							stringBuilder.Append((char)bytes[j]);
						}
					}
					i--;
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool FromModifiedUTF7(string encodedString, out string decodedString)
		{
			decodedString = string.Empty;
			if (string.IsNullOrEmpty(encodedString))
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder(encodedString.Length);
			byte[] array = null;
			int num = 0;
			int i = encodedString.IndexOf(ImapUtilities.cShiftUTF7);
			while (i > -1)
			{
				if (i > encodedString.Length - 1)
				{
					return false;
				}
				if (num < i)
				{
					stringBuilder.Append(encodedString.Substring(num, i - num));
				}
				num = i;
				i = encodedString.IndexOf(ImapUtilities.cShiftASCII, num);
				if (i == -1)
				{
					return false;
				}
				if (num + 1 == i)
				{
					stringBuilder.Append(ImapUtilities.cShiftUTF7);
					num = i + 1;
					i = encodedString.IndexOf(ImapUtilities.cShiftUTF7, num);
				}
				else
				{
					if (array == null)
					{
						array = Encoding.ASCII.GetBytes(encodedString);
					}
					array[num] = (byte)ImapUtilities.cOriginalShiftUTF7;
					for (int j = num; j < i; j++)
					{
						if (array[j] == ImapUtilities.bComma)
						{
							array[j] = ImapUtilities.bSlash;
						}
					}
					string @string = Encoding.UTF7.GetString(array, num, i - num);
					if (@string.Length == 0)
					{
						return false;
					}
					stringBuilder.Append(@string);
					num = i + 1;
					i = encodedString.IndexOf(ImapUtilities.cShiftUTF7, num);
				}
			}
			if (num == 0)
			{
				decodedString = encodedString;
				return true;
			}
			if (num < encodedString.Length)
			{
				stringBuilder.Append(encodedString.Substring(num));
			}
			decodedString = stringBuilder.ToString();
			return true;
		}

		internal static void AppendStringBuilderImapFlags(ImapMailFlags flags, StringBuilder builderToUse)
		{
			builderToUse.Append('(');
			string value = string.Empty;
			if ((flags & ImapMailFlags.Answered) == ImapMailFlags.Answered)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Answered");
				value = " ";
			}
			if ((flags & ImapMailFlags.Deleted) == ImapMailFlags.Deleted)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Deleted");
				value = " ";
			}
			if ((flags & ImapMailFlags.Draft) == ImapMailFlags.Draft)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Draft");
				value = " ";
			}
			if ((flags & ImapMailFlags.Flagged) == ImapMailFlags.Flagged)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Flagged");
				value = " ";
			}
			if ((flags & ImapMailFlags.Seen) == ImapMailFlags.Seen)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Seen");
			}
			builderToUse.Append(')');
		}

		internal static ImapMailFlags ConvertStringFormToFlags(string stringForm)
		{
			ImapMailFlags imapMailFlags = ImapMailFlags.None;
			if (!string.IsNullOrEmpty(stringForm))
			{
				string[] array = stringForm.ToUpperInvariant().Trim(new char[]
				{
					'(',
					')'
				}).Split(new char[]
				{
					' '
				});
				foreach (string text in array)
				{
					string a;
					if ((a = text) != null)
					{
						if (!(a == "\\ANSWERED"))
						{
							if (!(a == "\\DELETED"))
							{
								if (!(a == "\\DRAFT"))
								{
									if (!(a == "\\FLAGGED"))
									{
										if (a == "\\SEEN")
										{
											imapMailFlags |= ImapMailFlags.Seen;
										}
									}
									else
									{
										imapMailFlags |= ImapMailFlags.Flagged;
									}
								}
								else
								{
									imapMailFlags |= ImapMailFlags.Draft;
								}
							}
							else
							{
								imapMailFlags |= ImapMailFlags.Deleted;
							}
						}
						else
						{
							imapMailFlags |= ImapMailFlags.Answered;
						}
					}
				}
			}
			return imapMailFlags;
		}

		internal static ImapMailFlags FilterFlagsAgainstSupported(ImapMailFlags incomingFlags, ImapMailbox mailbox)
		{
			ImapMailFlags imapMailFlags = ImapMailFlags.All;
			if (mailbox != null)
			{
				imapMailFlags = mailbox.PermanentFlags;
			}
			return incomingFlags & imapMailFlags;
		}

		internal static void LogExceptionDetails(ILog log, ImapCommand failingCommand, Exception failure)
		{
			Exception ex = failure;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			log.Error("While executing [{0}]: {1}", new object[]
			{
				failingCommand.ToPiiCleanString(),
				ex.Message
			});
			string stackTrace = ex.StackTrace;
			if (stackTrace != null && stackTrace.Length > 0)
			{
				log.Error("Stack [{0}]", new object[]
				{
					stackTrace
				});
			}
		}

		internal static void LogExceptionDetails(ILog log, string errPrefix, Exception failure)
		{
			Exception ex = failure;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			log.Error("{0}: {1}", new object[]
			{
				errPrefix,
				ex.Message
			});
			string stackTrace = ex.StackTrace;
			if (stackTrace != null && stackTrace.Length > 0)
			{
				log.Error("{0}: Stack [{1}]", new object[]
				{
					errPrefix,
					stackTrace
				});
			}
		}

		private const string AnsweredFlag = "\\Answered";

		private const string AnswerFlagUpper = "\\ANSWERED";

		private const string DeletedFlag = "\\Deleted";

		private const string DeletedFlagUpper = "\\DELETED";

		private const string DraftFlag = "\\Draft";

		private const string DraftFlagUpper = "\\DRAFT";

		private const string FlaggedFlag = "\\Flagged";

		private const string FlaggedFlagUpper = "\\FLAGGED";

		private const string SeenFlag = "\\Seen";

		private const string SeenFlagUpper = "\\SEEN";

		private static char cShiftUTF7 = '&';

		private static char cOriginalShiftUTF7 = '+';

		private static char cShiftASCII = '-';

		private static char cSlash = '/';

		private static byte bSlash = (byte)ImapUtilities.cSlash;

		private static char cComma = ',';

		private static byte bComma = (byte)ImapUtilities.cComma;
	}
}
