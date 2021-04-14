using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class IMAPUtils
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
				if (decodedString[i] <= '\u007f' && decodedString[i] != IMAPUtils.cShiftUTF7)
				{
					int num = i;
					while (i < decodedString.Length && decodedString[i] <= '\u007f' && decodedString[i] != IMAPUtils.cShiftUTF7)
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
				else if (decodedString[i] == IMAPUtils.cShiftUTF7)
				{
					stringBuilder.Append(IMAPUtils.cShiftUTF7);
					stringBuilder.Append(IMAPUtils.cShiftASCII);
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
					bytes[0] = (byte)IMAPUtils.cShiftUTF7;
					for (int j = 0; j < bytes.Length; j++)
					{
						if (bytes[j] == IMAPUtils.bSlash)
						{
							stringBuilder.Append(IMAPUtils.cComma);
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
			int i = encodedString.IndexOf(IMAPUtils.cShiftUTF7);
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
				i = encodedString.IndexOf(IMAPUtils.cShiftASCII, num);
				if (i == -1)
				{
					return false;
				}
				if (num + 1 == i)
				{
					stringBuilder.Append(IMAPUtils.cShiftUTF7);
					num = i + 1;
					i = encodedString.IndexOf(IMAPUtils.cShiftUTF7, num);
				}
				else
				{
					if (array == null)
					{
						array = Encoding.ASCII.GetBytes(encodedString);
					}
					array[num] = (byte)IMAPUtils.cOriginalShiftUTF7;
					for (int j = num; j < i; j++)
					{
						if (array[j] == IMAPUtils.bComma)
						{
							array[j] = IMAPUtils.bSlash;
						}
					}
					string @string = Encoding.UTF7.GetString(array, num, i - num);
					if (@string.Length == 0)
					{
						return false;
					}
					stringBuilder.Append(@string);
					num = i + 1;
					i = encodedString.IndexOf(IMAPUtils.cShiftUTF7, num);
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

		internal static string CreateEmailCloudId(IMAPFolder folder, string messageId)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				folder.Uniqueness,
				messageId
			});
		}

		internal static string CreateEmailCloudVersion(IMAPFolder folder, string uid, IMAPMailFlags flags)
		{
			flags = IMAPUtils.FilterFlagsAgainstSupported(flags, folder.Mailbox);
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				uid,
				(uint)flags
			});
		}

		internal static bool GetMessageIdFromCloudId(string cloudId, out string messageId)
		{
			int num = cloudId.IndexOf(' ');
			if (num >= 0)
			{
				messageId = cloudId.Substring(num + 1);
				return true;
			}
			messageId = cloudId;
			return false;
		}

		internal static bool GetUidFromEmailCloudVersion(string cloudVersion, out string uid)
		{
			if (cloudVersion == null)
			{
				uid = null;
				return false;
			}
			int num = cloudVersion.IndexOf(' ');
			if (num < 0)
			{
				uid = null;
				return false;
			}
			string s = cloudVersion.Substring(0, num);
			uint num2 = 0U;
			if (!uint.TryParse(s, out num2))
			{
				uid = null;
				return false;
			}
			uid = num2.ToString(CultureInfo.InvariantCulture);
			return true;
		}

		internal static bool GetFlagsFromCloudVersion(string cloudVersion, IMAPMailbox mailbox, out IMAPMailFlags flags)
		{
			int num = cloudVersion.IndexOf(' ');
			if (num < 0)
			{
				flags = IMAPMailFlags.None;
				return false;
			}
			string s = cloudVersion.Substring(num + 1);
			uint incomingFlags = 0U;
			if (!uint.TryParse(s, out incomingFlags))
			{
				flags = IMAPMailFlags.None;
				return false;
			}
			flags = IMAPUtils.FilterFlagsAgainstSupported((IMAPMailFlags)incomingFlags, mailbox);
			return true;
		}

		internal static void UpdateUidInCloudVersion(string uid, ref string cloudVersion)
		{
			int num = cloudVersion.IndexOf(' ');
			if (num < 0)
			{
				cloudVersion = uid + " 0";
				return;
			}
			string str = cloudVersion.Substring(num);
			cloudVersion = uid + str;
		}

		internal static bool TryUpdateFlagsInCloudVersion(IMAPMailFlags flags, IMAPMailbox mailbox, ref string cloudVersion)
		{
			int num = cloudVersion.IndexOf(' ');
			if (num < 0)
			{
				return false;
			}
			IMAPMailFlags imapmailFlags = IMAPUtils.FilterFlagsAgainstSupported(flags, mailbox);
			string text = cloudVersion.Substring(0, num + 1);
			string str = text;
			int num2 = (int)imapmailFlags;
			cloudVersion = str + num2.ToString(CultureInfo.InvariantCulture);
			return true;
		}

		internal static void AppendStringBuilderIMAPFlags(IMAPMailFlags flags, StringBuilder builderToUse)
		{
			builderToUse.Append('(');
			string value = string.Empty;
			if ((flags & IMAPMailFlags.Answered) == IMAPMailFlags.Answered)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Answered");
				value = " ";
			}
			if ((flags & IMAPMailFlags.Deleted) == IMAPMailFlags.Deleted)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Deleted");
				value = " ";
			}
			if ((flags & IMAPMailFlags.Draft) == IMAPMailFlags.Draft)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Draft");
				value = " ";
			}
			if ((flags & IMAPMailFlags.Flagged) == IMAPMailFlags.Flagged)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Flagged");
				value = " ";
			}
			if ((flags & IMAPMailFlags.Seen) == IMAPMailFlags.Seen)
			{
				builderToUse.Append(value);
				builderToUse.Append("\\Seen");
			}
			builderToUse.Append(')');
		}

		internal static IMAPMailFlags ConvertStringFormToFlags(string stringForm)
		{
			IMAPMailFlags imapmailFlags = IMAPMailFlags.None;
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
											imapmailFlags |= IMAPMailFlags.Seen;
										}
									}
									else
									{
										imapmailFlags |= IMAPMailFlags.Flagged;
									}
								}
								else
								{
									imapmailFlags |= IMAPMailFlags.Draft;
								}
							}
							else
							{
								imapmailFlags |= IMAPMailFlags.Deleted;
							}
						}
						else
						{
							imapmailFlags |= IMAPMailFlags.Answered;
						}
					}
				}
			}
			return imapmailFlags;
		}

		internal static IMAPMailFlags FilterFlagsAgainstSupported(IMAPMailFlags incomingFlags, IMAPMailbox mailbox)
		{
			IMAPMailFlags imapmailFlags = IMAPMailFlags.All;
			if (mailbox != null)
			{
				imapmailFlags = mailbox.PermanentFlags;
			}
			return incomingFlags & imapmailFlags;
		}

		internal static void LogExceptionDetails(SyncLogSession log, Trace tracer, IMAPCommand failingCommand, Exception failure)
		{
			Exception ex = failure;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			log.LogError((TSLID)884UL, tracer, "While executing [{0}]: {1}", new object[]
			{
				failingCommand.ToPiiCleanString(),
				ex.Message
			});
			string stackTrace = ex.StackTrace;
			if (stackTrace != null && stackTrace.Length > 0)
			{
				log.LogError((TSLID)885UL, tracer, "Stack [{0}]", new object[]
				{
					stackTrace
				});
			}
		}

		internal static void LogExceptionDetails(SyncLogSession log, Trace tracer, string errPrefix, Exception failure)
		{
			Exception ex = failure;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			log.LogError((TSLID)886UL, tracer, "{0}: {1}", new object[]
			{
				errPrefix,
				ex.Message
			});
			string stackTrace = ex.StackTrace;
			if (stackTrace != null && stackTrace.Length > 0)
			{
				log.LogError((TSLID)887UL, tracer, "{0}: Stack [{1}]", new object[]
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

		private static byte bSlash = (byte)IMAPUtils.cSlash;

		private static char cComma = ',';

		private static byte bComma = (byte)IMAPUtils.cComma;
	}
}
