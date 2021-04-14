using System;
using System.Collections;

namespace System.IO.IsolatedStorage
{
	internal sealed class TwoLevelFileEnumerator : IEnumerator
	{
		public TwoLevelFileEnumerator(string root)
		{
			this.m_Root = root;
			this.Reset();
		}

		public bool MoveNext()
		{
			lock (this)
			{
				if (this.m_fReset)
				{
					this.m_fReset = false;
					return this.AdvanceRootDir();
				}
				if (this.m_RootDir.Length == 0)
				{
					return false;
				}
				this.m_nSubDir++;
				if (this.m_nSubDir >= this.m_SubDir.Length)
				{
					this.m_nSubDir = this.m_SubDir.Length;
					return this.AdvanceRootDir();
				}
				this.UpdateCurrent();
			}
			return true;
		}

		private bool AdvanceRootDir()
		{
			this.m_nRootDir++;
			if (this.m_nRootDir >= this.m_RootDir.Length)
			{
				this.m_nRootDir = this.m_RootDir.Length;
				return false;
			}
			this.m_SubDir = Directory.GetDirectories(this.m_RootDir[this.m_nRootDir]);
			if (this.m_SubDir.Length == 0)
			{
				return this.AdvanceRootDir();
			}
			this.m_nSubDir = 0;
			this.UpdateCurrent();
			return true;
		}

		private void UpdateCurrent()
		{
			this.m_Current.Path1 = Path.GetFileName(this.m_RootDir[this.m_nRootDir]);
			this.m_Current.Path2 = Path.GetFileName(this.m_SubDir[this.m_nSubDir]);
		}

		public object Current
		{
			get
			{
				if (this.m_fReset)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
				}
				if (this.m_nRootDir >= this.m_RootDir.Length)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
				}
				return this.m_Current;
			}
		}

		public void Reset()
		{
			this.m_RootDir = null;
			this.m_nRootDir = -1;
			this.m_SubDir = null;
			this.m_nSubDir = -1;
			this.m_Current = new TwoPaths();
			this.m_fReset = true;
			this.m_RootDir = Directory.GetDirectories(this.m_Root);
		}

		private string m_Root;

		private TwoPaths m_Current;

		private bool m_fReset;

		private string[] m_RootDir;

		private int m_nRootDir;

		private string[] m_SubDir;

		private int m_nSubDir;
	}
}
