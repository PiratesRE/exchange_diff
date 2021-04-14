using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Isam.Esent.Interop
{
	internal abstract class TableEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
	{
		protected TableEnumerator(JET_SESID sesid)
		{
			this.Sesid = sesid;
			this.TableidToEnumerate = JET_TABLEID.Nil;
		}

		public T Current { get; private set; }

		object IEnumerator.Current
		{
			[DebuggerStepThrough]
			get
			{
				return this.Current;
			}
		}

		private protected JET_SESID Sesid { protected get; private set; }

		protected JET_TABLEID TableidToEnumerate { get; set; }

		public void Reset()
		{
			this.isAtEnd = false;
			this.moveToFirst = true;
		}

		public void Dispose()
		{
			this.CloseTable();
			GC.SuppressFinalize(this);
		}

		public bool MoveNext()
		{
			if (this.isAtEnd)
			{
				return false;
			}
			if (this.TableidToEnumerate.IsInvalid)
			{
				this.OpenTable();
			}
			bool flag = true;
			if (this.moveToFirst)
			{
				if (!Api.TryMoveFirst(this.Sesid, this.TableidToEnumerate))
				{
					this.isAtEnd = true;
					return false;
				}
				this.moveToFirst = false;
				flag = false;
			}
			while (flag || this.SkipCurrent())
			{
				if (!Api.TryMoveNext(this.Sesid, this.TableidToEnumerate))
				{
					this.isAtEnd = true;
					return false;
				}
				flag = false;
			}
			this.Current = this.GetCurrent();
			return true;
		}

		protected abstract void OpenTable();

		protected abstract T GetCurrent();

		protected virtual bool SkipCurrent()
		{
			return false;
		}

		protected virtual void CloseTable()
		{
			if (!this.TableidToEnumerate.IsInvalid)
			{
				Api.JetCloseTable(this.Sesid, this.TableidToEnumerate);
			}
			this.TableidToEnumerate = JET_TABLEID.Nil;
		}

		private bool isAtEnd;

		private bool moveToFirst = true;
	}
}
