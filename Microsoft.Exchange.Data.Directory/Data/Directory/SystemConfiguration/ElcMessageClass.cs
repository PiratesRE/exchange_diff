using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ElcMessageClass
	{
		static ElcMessageClass()
		{
			ElcMessageClass.standardList = new Dictionary<string, string>(13, StringComparer.OrdinalIgnoreCase);
			ElcMessageClass.standardList.Add(ElcMessageClass.AllMailboxContent, DirectoryStrings.AllMailboxContentMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.AllEmail, DirectoryStrings.AllEmailMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.VoiceMail, DirectoryStrings.ExchangeVoicemailMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.MissedCall, DirectoryStrings.ExchangeMissedcallMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Fax, DirectoryStrings.ExchangeFaxMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.CalItems, DirectoryStrings.CalendarItemMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.MeetingRequest, DirectoryStrings.MeetingRequestMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Contacts, DirectoryStrings.ContactItemsMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Documents, DirectoryStrings.DocumentMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Tasks, DirectoryStrings.TaskItemsMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Journal, DirectoryStrings.JournalItemsMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Notes, DirectoryStrings.NotesMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.Posts, DirectoryStrings.PostMC);
			ElcMessageClass.standardList.Add(ElcMessageClass.RssSubscriptions, DirectoryStrings.RssSubscriptionMC);
		}

		public ElcMessageClass(string messageClass) : this(messageClass, ElcMessageClass.GetDisplayName(messageClass))
		{
		}

		public ElcMessageClass(string messageClass, string displayName)
		{
			if (messageClass != null)
			{
				this.messageClass = messageClass.Trim();
			}
			this.displayName = displayName;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public static ElcMessageClass Parse(string expression)
		{
			return ElcMessageClass.GetMessageClass(expression);
		}

		public static bool operator ==(ElcMessageClass a, ElcMessageClass b)
		{
			return a == b || (a != null && b != null && a.MessageClass == b.MessageClass);
		}

		public static bool operator !=(ElcMessageClass a, ElcMessageClass b)
		{
			return !(a == b);
		}

		public static ElcMessageClass GetMessageClass(string messageClass)
		{
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			string text;
			ElcMessageClass result;
			if (!ElcMessageClass.standardList.TryGetValue(messageClass, out text))
			{
				result = new ElcMessageClass(messageClass, null);
			}
			else
			{
				result = new ElcMessageClass(messageClass, text);
			}
			return result;
		}

		public static ElcMessageClass[] GetStandardMessageClasses()
		{
			ElcMessageClass[] array = new ElcMessageClass[ElcMessageClass.standardList.Count];
			int num = 0;
			foreach (KeyValuePair<string, string> keyValuePair in ElcMessageClass.standardList)
			{
				array[num] = new ElcMessageClass(keyValuePair.Key, keyValuePair.Value);
				num++;
			}
			return array;
		}

		internal static string GetDisplayName(string messageClass)
		{
			string text = null;
			if (messageClass != null)
			{
				ElcMessageClass.standardList.TryGetValue(messageClass, out text);
				if (text == null)
				{
					if (string.Compare(messageClass, ElcMessageClass.AllEmailList, StringComparison.OrdinalIgnoreCase) == 0)
					{
						text = ElcMessageClass.AllEmail;
					}
					else
					{
						text = messageClass;
					}
				}
			}
			return text;
		}

		internal static bool IsAllEmail(string messageClass)
		{
			return !string.IsNullOrEmpty(messageClass) && string.Compare(messageClass, ElcMessageClass.AllEmail, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static bool IsMultiMessageClass(string messageClass)
		{
			return !string.IsNullOrEmpty(messageClass) && messageClass.IndexOfAny(ElcMessageClass.MessageClassDelims) != -1;
		}

		internal static bool IsMultiMessageClassDeputy(string messageClass)
		{
			return !string.IsNullOrEmpty(messageClass) && string.Compare(messageClass, ElcMessageClass.MultiMessageClassDeputy, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public sealed override string ToString()
		{
			return this.DisplayName;
		}

		public sealed override int GetHashCode()
		{
			return this.MessageClass.GetHashCode();
		}

		public sealed override bool Equals(object obj)
		{
			return this == obj as ElcMessageClass;
		}

		private readonly string displayName;

		private string messageClass;

		private static readonly Dictionary<string, string> standardList;

		public static readonly string VoiceMail = "IPM.Note.Microsoft.Voicemail*";

		public static readonly string Fax = "IPM.Note.Microsoft.Fax*";

		public static readonly string MissedCall = "IPM.Note.Microsoft.Missed.Voice*";

		public static readonly string CalItems = "IPM.Appointment*";

		public static readonly string Contacts = "IPM.Contact*";

		public static readonly string Tasks = "IPM.Task*";

		public static readonly string Journal = "IPM.Activity*";

		public static readonly string Notes = "IPM.StickyNote*";

		public static readonly string MeetingRequest = "IPM.Schedule*";

		public static readonly string AllMailboxContent = "*";

		public static readonly string Documents = "IPM.Document*";

		public static readonly string Posts = "IPM.Post";

		public static readonly string RssSubscriptions = "IPM.Post.RSS";

		public static readonly string AllEmail = "E-mail";

		internal static readonly string AllEmailList = "IPM.Note;IPM.Note.AS/400 Move Notification Form v1.0;IPM.Note.Delayed;IPM.Note.Exchange.ActiveSync.Report;IPM.Note.JournalReport.Msg;IPM.Note.JournalReport.Tnef;IPM.Note.Microsoft.Missed.Voice;IPM.Note.Rules.OofTemplate.Microsoft;IPM.Note.Rules.ReplyTemplate.Microsoft;IPM.Note.Secure.Sign;IPM.Note.SMIME;IPM.Note.SMIME.MultipartSigned;IPM.Note.StorageQuotaWarning;IPM.Note.StorageQuotaWarning.Warning;IPM.Notification.Meeting.Forward;IPM.Outlook.Recall;IPM.Recall.Report.Success;IPM.Schedule.Meeting.*;REPORT.IPM.Note.NDR";

		internal static string MultiMessageClassDeputy = "{5C8E4D3F-96BD-4a97-BEB5-764F032A8ECD}(MultiMessageClassDeputy Exchange 2007 sp1 or later)";

		internal static readonly char[] MessageClassDelims = new char[]
		{
			';'
		};
	}
}
