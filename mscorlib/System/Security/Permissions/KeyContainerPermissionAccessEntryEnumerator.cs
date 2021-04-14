﻿using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class KeyContainerPermissionAccessEntryEnumerator : IEnumerator
	{
		private KeyContainerPermissionAccessEntryEnumerator()
		{
		}

		internal KeyContainerPermissionAccessEntryEnumerator(KeyContainerPermissionAccessEntryCollection entries)
		{
			this.m_entries = entries;
			this.m_current = -1;
		}

		public KeyContainerPermissionAccessEntry Current
		{
			get
			{
				return this.m_entries[this.m_current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.m_entries[this.m_current];
			}
		}

		public bool MoveNext()
		{
			if (this.m_current == this.m_entries.Count - 1)
			{
				return false;
			}
			this.m_current++;
			return true;
		}

		public void Reset()
		{
			this.m_current = -1;
		}

		private KeyContainerPermissionAccessEntryCollection m_entries;

		private int m_current;
	}
}
