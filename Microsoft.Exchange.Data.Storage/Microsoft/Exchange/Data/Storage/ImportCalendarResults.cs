using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ImportCalendarResults
	{
		internal IList<LocalizedString> RawErrors
		{
			get
			{
				return this.errors;
			}
		}

		public LocalizedString[] Errors
		{
			get
			{
				return this.errors.ToArray();
			}
		}

		public int CountOfImportedItems
		{
			get
			{
				return this.countOfImportedItems;
			}
		}

		internal int CountOfUnchangedItems
		{
			get
			{
				return this.countOfUnchangedItems;
			}
		}

		public bool TimedOut
		{
			get
			{
				return this.timedOut;
			}
		}

		public ImportCalendarResultType Result
		{
			get
			{
				if (this.Errors.Length == 0 && !this.TimedOut)
				{
					return ImportCalendarResultType.Success;
				}
				if (this.CountOfImportedItems > 0)
				{
					return ImportCalendarResultType.PartiallySuccess;
				}
				return ImportCalendarResultType.Failed;
			}
		}

		public string CalendarName
		{
			get
			{
				return this.calendarName ?? string.Empty;
			}
			internal set
			{
				this.calendarName = value;
			}
		}

		internal ImportCalendarResults()
		{
			this.errors = new List<LocalizedString>();
		}

		internal void Reset()
		{
			this.countOfImportedItems = 0;
			this.countOfUnchangedItems = 0;
			this.errors.Clear();
			this.timedOut = false;
		}

		internal void Increment(bool changed)
		{
			this.countOfImportedItems++;
			if (!changed)
			{
				this.countOfUnchangedItems++;
			}
		}

		public void SetTimeOut()
		{
			this.timedOut = true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("Result: " + this.Result.ToString());
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("--Name of Calendar: {0}", this.CalendarName);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("--Count of Imported Items: {0}", this.CountOfImportedItems);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("--Count of Unchanged Items: {0}", this.CountOfUnchangedItems);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("--Count of Errors: {0}", this.Errors.Length);
			if (this.Errors.Length > 0)
			{
				foreach (LocalizedString localizedString in this.Errors)
				{
					stringBuilder.AppendLine();
					stringBuilder.AppendFormat("----Error: {0}", localizedString);
				}
			}
			return stringBuilder.ToString();
		}

		private readonly List<LocalizedString> errors;

		private int countOfImportedItems;

		private int countOfUnchangedItems;

		private string calendarName;

		private bool timedOut;
	}
}
