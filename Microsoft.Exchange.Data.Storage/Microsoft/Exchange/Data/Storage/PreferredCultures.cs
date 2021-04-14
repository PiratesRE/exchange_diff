using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PreferredCultures : IEnumerable<CultureInfo>, IEnumerable
	{
		public PreferredCultures()
		{
		}

		public PreferredCultures(IEnumerable<CultureInfo> cultures)
		{
			if (cultures == null)
			{
				throw new ArgumentNullException();
			}
			this.cultures.AddRange(cultures);
		}

		public IEnumerator<CultureInfo> GetEnumerator()
		{
			return this.cultures.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.cultures.Count;
			}
		}

		public void Add(CultureInfo newCulture)
		{
			if (newCulture == null)
			{
				throw new ArgumentNullException();
			}
			if (!this.CultureWillFit(newCulture))
			{
				throw new PreferredCulturesException(ServerStrings.TooManyCultures);
			}
			if (!this.cultures.Contains(newCulture))
			{
				this.cultures.Add(newCulture);
			}
		}

		public void Remove(CultureInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException();
			}
			this.cultures.Remove(info);
		}

		public void Clear()
		{
			this.cultures.Clear();
		}

		public CultureInfo this[int index]
		{
			get
			{
				return this.cultures[index];
			}
		}

		public void InsertBefore(CultureInfo cultureAtInsertPoint, CultureInfo cultureToInsert)
		{
			if (!this.CultureWillFit(cultureToInsert))
			{
				throw new PreferredCulturesException(ServerStrings.TooManyCultures);
			}
			if (cultureAtInsertPoint == null || cultureToInsert == null)
			{
				throw new ArgumentNullException();
			}
			if (this.cultures.Contains(cultureToInsert))
			{
				if (LocaleMap.GetLcidFromCulture(cultureToInsert) == LocaleMap.GetLcidFromCulture(cultureAtInsertPoint))
				{
					return;
				}
				this.Remove(cultureToInsert);
			}
			if (!this.cultures.Contains(cultureAtInsertPoint))
			{
				this.Add(cultureToInsert);
				return;
			}
			int index = this.cultures.IndexOf(cultureAtInsertPoint);
			this.cultures.Insert(index, cultureToInsert);
		}

		public void AddSupportedCulture(CultureInfo newCulture, Predicate<CultureInfo> isSupported)
		{
			if (newCulture == null)
			{
				throw new ArgumentNullException("newCulture");
			}
			if (isSupported == null)
			{
				throw new ArgumentNullException("isSupported");
			}
			int num = -1;
			int i = 0;
			while (i < this.cultures.Count)
			{
				CultureInfo cultureInfo = this.cultures[i];
				if (isSupported(cultureInfo))
				{
					if (cultureInfo.Equals(newCulture))
					{
						return;
					}
					num = i;
					break;
				}
				else
				{
					i++;
				}
			}
			if (this.cultures.Contains(newCulture))
			{
				this.cultures.Remove(newCulture);
			}
			while (!this.CultureWillFit(newCulture) && this.cultures.Count > 0)
			{
				this.cultures.RemoveAt(this.cultures.Count - 1);
			}
			if (num < 0)
			{
				this.cultures.Add(newCulture);
				return;
			}
			if (num < this.cultures.Count)
			{
				this.cultures.Insert(num, newCulture);
				return;
			}
			this.cultures.Add(newCulture);
		}

		private bool CultureWillFit(CultureInfo newCulture)
		{
			if (this.cultures.Contains(newCulture))
			{
				return true;
			}
			ADUser aduser = new ADUser();
			try
			{
				Util.AddRange<CultureInfo, CultureInfo>(aduser.Languages, this.cultures);
				aduser.Languages.Add(newCulture);
			}
			catch (DataValidationException ex)
			{
				if (ex.Error is PropertyConstraintViolationError)
				{
					return false;
				}
			}
			return true;
		}

		private readonly List<CultureInfo> cultures = new List<CultureInfo>();
	}
}
