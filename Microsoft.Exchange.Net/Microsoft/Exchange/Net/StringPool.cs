using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Net
{
	internal sealed class StringPool
	{
		public StringPool(IEqualityComparer<string> equalityComparer) : this(null, equalityComparer)
		{
		}

		public StringPool(ICollection<string> initialStrings, IEqualityComparer<string> equalityComparer)
		{
			if (equalityComparer == null)
			{
				throw new ArgumentNullException("equalityComparer");
			}
			if (initialStrings != null)
			{
				this.stringDictionary = new Dictionary<string, string>(initialStrings.Count, equalityComparer);
				using (IEnumerator<string> enumerator = initialStrings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						this.stringDictionary.Add(text, text);
					}
					return;
				}
			}
			this.stringDictionary = new Dictionary<string, string>(equalityComparer);
		}

		public string Intern(string stringToIntern)
		{
			if (stringToIntern == null)
			{
				return null;
			}
			return this.IsInterned(stringToIntern) ?? this.AddStringToDictionary(stringToIntern);
		}

		public string IsInterned(string stringToIntern)
		{
			if (stringToIntern == null)
			{
				return null;
			}
			if (stringToIntern.Length == 0)
			{
				return string.Empty;
			}
			try
			{
				this.readWriterLock.EnterReadLock();
				string result;
				if (this.stringDictionary.TryGetValue(stringToIntern, out result))
				{
					return result;
				}
			}
			finally
			{
				try
				{
					this.readWriterLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return null;
		}

		private string AddStringToDictionary(string stringToIntern)
		{
			string result;
			try
			{
				this.readWriterLock.EnterWriteLock();
				string text;
				if (this.stringDictionary.TryGetValue(stringToIntern, out text))
				{
					result = text;
				}
				else
				{
					this.stringDictionary.Add(stringToIntern, stringToIntern);
					result = stringToIntern;
				}
			}
			finally
			{
				try
				{
					this.readWriterLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		private Dictionary<string, string> stringDictionary;

		private ReaderWriterLockSlim readWriterLock = new ReaderWriterLockSlim();
	}
}
