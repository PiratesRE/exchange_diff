using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class JunkEmailCollection : Collection<string>
	{
		private static string MakeSmtpAddressFromDomain(string smtpDomainWithLeadingAt)
		{
			return "user" + smtpDomainWithLeadingAt;
		}

		private static bool IsValidFormat(string value)
		{
			return SmtpAddress.IsValidSmtpAddress(value) || SmtpAddress.IsValidSmtpAddress(JunkEmailCollection.MakeSmtpAddressFromDomain(value));
		}

		private JunkEmailCollection()
		{
		}

		internal static JunkEmailCollection Create(JunkEmailRule junkRule, JunkEmailCollection.ListType listType)
		{
			return JunkEmailCollection.Create(junkRule, listType, null);
		}

		internal static JunkEmailCollection Create(JunkEmailRule junkRule, JunkEmailCollection.ListType listType, ICollection<string> invlidEntriesCollection)
		{
			return new JunkEmailCollection
			{
				junkRule = junkRule,
				listType = listType,
				invlidEntriesCollection = invlidEntriesCollection
			};
		}

		public new int Add(string value)
		{
			base.Add(value);
			return base.Count - 1;
		}

		public void AddRange(string[] value)
		{
			foreach (string value2 in value)
			{
				this.Add(value2);
			}
		}

		public JunkEmailCollection.ValidationProblem TryAdd(string value)
		{
			JunkEmailCollection.ValidationProblem validationProblem = this.validating ? this.CheckValue(value) : JunkEmailCollection.ValidationProblem.NoError;
			if (validationProblem == JunkEmailCollection.ValidationProblem.NoError)
			{
				base.Add(value);
			}
			return validationProblem;
		}

		private void InternalSort(JunkEmailCollection.SortDelegate sortDelegate)
		{
			bool flag = this.validating;
			this.validating = false;
			try
			{
				sortDelegate();
			}
			finally
			{
				this.validating = flag;
			}
		}

		public void Sort()
		{
			this.InternalSort(delegate
			{
				((List<string>)base.Items).Sort();
			});
		}

		public void Sort(Comparison<string> comparison)
		{
			this.InternalSort(delegate
			{
				((List<string>)this.Items).Sort(comparison);
			});
		}

		public void Sort(IComparer<string> comparer)
		{
			this.InternalSort(delegate
			{
				((List<string>)this.Items).Sort(comparer);
			});
		}

		public void Sort(int index, int count, IComparer<string> comparer)
		{
			this.InternalSort(delegate
			{
				((List<string>)this.Items).Sort(index, count, comparer);
			});
		}

		public int MaxNumberOfEntries
		{
			get
			{
				if (this.listType == JunkEmailCollection.ListType.TrustedList)
				{
					return this.MaxNumberOfTrustedEntries;
				}
				return this.MaxNumberOfBlockedEntries;
			}
		}

		public static int MaxEntrySize
		{
			get
			{
				return 512;
			}
		}

		protected override void InsertItem(int index, string newItem)
		{
			this.Validate(newItem);
			base.InsertItem(index, newItem);
		}

		protected override void SetItem(int index, string newItem)
		{
			this.Validate(newItem);
			base.SetItem(index, newItem);
		}

		internal bool Validating
		{
			get
			{
				return this.validating;
			}
			set
			{
				this.validating = value;
			}
		}

		private void Validate(string value)
		{
			if (this.validating)
			{
				JunkEmailCollection.ValidationProblem validationProblem = this.CheckValue(value);
				if (validationProblem != JunkEmailCollection.ValidationProblem.NoError)
				{
					throw new JunkEmailValidationException(value, validationProblem);
				}
			}
		}

		private JunkEmailCollection.ValidationProblem CheckValue(string value)
		{
			if (value == null)
			{
				return JunkEmailCollection.ValidationProblem.Null;
			}
			if (value.Length == 0)
			{
				return JunkEmailCollection.ValidationProblem.Empty;
			}
			if (value.Length > JunkEmailCollection.MaxEntrySize)
			{
				return JunkEmailCollection.ValidationProblem.TooBig;
			}
			if (base.Count >= this.MaxNumberOfEntries)
			{
				return JunkEmailCollection.ValidationProblem.TooManyEntries;
			}
			if (!JunkEmailCollection.IsValidFormat(value))
			{
				return JunkEmailCollection.ValidationProblem.FormatError;
			}
			foreach (string value2 in this)
			{
				if (value.Equals(value2, StringComparison.OrdinalIgnoreCase))
				{
					return JunkEmailCollection.ValidationProblem.Duplicate;
				}
			}
			if (this.invlidEntriesCollection != null && this.invlidEntriesCollection.Contains(value))
			{
				return JunkEmailCollection.ValidationProblem.EntryInInvalidEntriesList;
			}
			return JunkEmailCollection.ValidationProblem.NoError;
		}

		private int MaxNumberOfTrustedEntries
		{
			get
			{
				return this.junkRule.MaxNumberOfTrustedEntries;
			}
		}

		private int MaxNumberOfBlockedEntries
		{
			get
			{
				return this.junkRule.MaxNumberOfBlockedEntries;
			}
		}

		private const int MaximumEntrySize = 512;

		private bool validating = true;

		private JunkEmailCollection.ListType listType;

		private JunkEmailRule junkRule;

		private ICollection<string> invlidEntriesCollection;

		internal enum ListType
		{
			TrustedList = 1,
			BlockedList
		}

		internal enum ValidationProblem
		{
			NoError,
			Null,
			Duplicate,
			FormatError,
			Empty,
			TooBig,
			TooManyEntries,
			EntryInInvalidEntriesList
		}

		private delegate void SortDelegate();
	}
}
