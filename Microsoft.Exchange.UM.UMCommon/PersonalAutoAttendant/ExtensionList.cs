using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class ExtensionList : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable, IDataItem
	{
		internal ExtensionList()
		{
			this.extensions = new List<string>();
		}

		public int Count
		{
			get
			{
				return this.extensions.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public PAAValidationResult ValidationResult
		{
			get
			{
				return this.validationResult;
			}
			set
			{
				this.validationResult = value;
			}
		}

		public string this[int index]
		{
			get
			{
				return this.extensions[index];
			}
			set
			{
				this.extensions[index] = value;
			}
		}

		public int IndexOf(string item)
		{
			return this.extensions.IndexOf(item);
		}

		public void Insert(int index, string item)
		{
			this.extensions.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.extensions.RemoveAt(index);
		}

		public void Add(string item)
		{
			this.extensions.Add(item);
		}

		public void Clear()
		{
			this.extensions.Clear();
		}

		public bool Contains(string item)
		{
			return this.extensions.Contains(item);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			this.extensions.CopyTo(array, arrayIndex);
		}

		public bool Remove(string item)
		{
			return this.extensions.Remove(item);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this.extensions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.extensions.GetEnumerator();
		}

		public bool Validate(IDataValidator dataValidator)
		{
			string text;
			return dataValidator.ValidateExtensions(this.extensions, out this.validationResult, out text);
		}

		private List<string> extensions;

		private PAAValidationResult validationResult;
	}
}
