using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public class CalendarLog : IConfigurable, IComparer<CalendarLog>
	{
		public CalendarLog()
		{
		}

		internal CalendarLog(Item item, string user)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			this.id = new CalendarLogId(item, user);
			this.OriginalLastModifiedTime = item.LastModifiedTime.ToUtc().UniversalTime;
			this.ItemVersion = item.GetProperty(CalendarItemBaseSchema.ItemVersion);
			this.InternalGlobalObjectId = item.GetProperty(CalendarItemBaseSchema.CleanGlobalObjectId);
			this.NormalizedSubject = item.GetValueOrDefault<string>(ItemSchema.NormalizedSubject, string.Empty);
		}

		internal CalendarLog(Item item, FileInfo log, string user) : this(item, user)
		{
			if (log == null)
			{
				throw new ArgumentNullException("item");
			}
			this.id = new CalendarLogId(log.FullName);
		}

		internal static CalendarLog[] Parse(string identity)
		{
			string[] array = identity.Split(new char[]
			{
				'|'
			}, StringSplitOptions.RemoveEmptyEntries);
			List<CalendarLog> list = new List<CalendarLog>();
			CalendarLog calendarLog = null;
			foreach (string text in array)
			{
				if (Directory.Exists(text))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text);
					FileInfo[] files = directoryInfo.GetFiles("*.msg");
					foreach (FileInfo logFile in files)
					{
						if (CalendarLog.TryParse(logFile, out calendarLog))
						{
							list.Add(calendarLog);
						}
					}
				}
				else if (File.Exists(text))
				{
					if (CalendarLog.TryParse(new FileInfo(text), out calendarLog))
					{
						list.Add(calendarLog);
					}
				}
				else
				{
					string text2 = text.Substring(0, text.LastIndexOf('\\'));
					string value = text.Substring(text2.Length + 1);
					if (Directory.Exists(text2))
					{
						DirectoryInfo directoryInfo2 = new DirectoryInfo(text2);
						FileInfo[] files2 = directoryInfo2.GetFiles("*.msg");
						foreach (FileInfo logFile2 in files2)
						{
							if (CalendarLog.TryParse(logFile2, out calendarLog) && calendarLog.CleanGlobalObjectId.Equals(value))
							{
								list.Add(calendarLog);
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		internal static CalendarLog FromFile(FileInfo file)
		{
			if (file.Exists)
			{
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					using (FileStream fileStream = file.OpenRead())
					{
						ItemConversion.ConvertMsgStorageToItem(fileStream, messageItem, new InboundConversionOptions(new EmptyRecipientCache(), null));
						return new CalendarLog(messageItem, file, null);
					}
				}
			}
			return null;
		}

		public bool IsFileLink
		{
			get
			{
				return new UriHandler(this.id.Uri).IsFileLink;
			}
		}

		public ObjectId Identity
		{
			get
			{
				return this.id;
			}
			private set
			{
				this.id = (value as CalendarLogId);
			}
		}

		public string LogDate
		{
			get
			{
				return this.OriginalLastModifiedTime.ToString("yyyy-MM-dd, h:mm:ss tt");
			}
		}

		internal DateTime OriginalLastModifiedTime { get; private set; }

		internal int ItemVersion { get; private set; }

		public string NormalizedSubject { get; private set; }

		public string CleanGlobalObjectId
		{
			get
			{
				return this.InternalGlobalObjectId.To64BitString();
			}
		}

		internal byte[] InternalGlobalObjectId { get; private set; }

		internal static bool TryParse(FileInfo logFile, out CalendarLog log)
		{
			log = null;
			try
			{
				log = CalendarLog.FromFile(logFile);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		internal static IComparer<CalendarLog> GetComparer()
		{
			return new CalendarLog();
		}

		internal int CompareTo(CalendarLog other)
		{
			return this.Compare(this, other);
		}

		public int Compare(CalendarLog c0, CalendarLog c1)
		{
			int num = c0.ItemVersion.CompareTo(c1.ItemVersion);
			if (num == 0)
			{
				num = c0.OriginalLastModifiedTime.CompareTo(c1.OriginalLastModifiedTime);
			}
			return num;
		}

		public ValidationError[] Validate()
		{
			if (this.validationErrors == null)
			{
				List<ValidationError> list = new List<ValidationError>();
				if (this.Identity == null)
				{
					list.Add(new ObjectValidationError(Strings.CalendarLogIdentityNotSpecified, this.Identity, "Identity"));
				}
				else
				{
					UriHandler uriHandler = new UriHandler(this.id.Uri);
					if (!uriHandler.IsValidLink)
					{
						list.Add(new ObjectValidationError(Strings.InvalidLogIdentityFormat(uriHandler.Uri.ToString()), this.Identity, "Identity"));
					}
					if (uriHandler.IsFileLink)
					{
						FileInfo fileInfo = new FileInfo(uriHandler.Uri.LocalPath);
						if (!fileInfo.Exists)
						{
							list.Add(new ObjectValidationError(Strings.CalendarLogFileDoesNotExist(uriHandler.Uri.ToString()), this.Identity, "Identity"));
						}
					}
				}
				if (list.Count == 0)
				{
					this.validationErrors = ValidationError.None;
				}
			}
			return this.validationErrors;
		}

		public bool IsValid
		{
			get
			{
				return new UriHandler(this.id.Uri).IsValidLink && this.Validate().Length == 0;
			}
		}

		public ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		public void CopyChangesFrom(IConfigurable source)
		{
		}

		public void ResetChangeTracking()
		{
		}

		public override string ToString()
		{
			return this.id.ToString();
		}

		private CalendarLogId id;

		private ValidationError[] validationErrors;
	}
}
