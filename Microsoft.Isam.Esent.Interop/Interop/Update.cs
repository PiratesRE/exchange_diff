using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class Update : EsentResource
	{
		public Update(JET_SESID sesid, JET_TABLEID tableid, JET_prep prep)
		{
			if (JET_prep.Cancel == prep)
			{
				throw new ArgumentException("Cannot create an Update for JET_prep.Cancel", "prep");
			}
			this.sesid = sesid;
			this.tableid = tableid;
			this.prep = prep;
			Api.JetPrepareUpdate(this.sesid, this.tableid, this.prep);
			base.ResourceWasAllocated();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Update ({0})", new object[]
			{
				this.prep
			});
		}

		public void Save(byte[] bookmark, int bookmarkSize, out int actualBookmarkSize)
		{
			base.CheckObjectIsNotDisposed();
			if (!base.HasResource)
			{
				throw new InvalidOperationException("Not in an update");
			}
			Api.JetUpdate(this.sesid, this.tableid, bookmark, bookmarkSize, out actualBookmarkSize);
			base.ResourceWasReleased();
		}

		public void Save()
		{
			int num;
			this.Save(null, 0, out num);
		}

		public void SaveAndGotoBookmark()
		{
			byte[] array = null;
			try
			{
				array = Caches.BookmarkCache.Allocate();
				int bookmarkSize;
				this.Save(array, array.Length, out bookmarkSize);
				Api.JetGotoBookmark(this.sesid, this.tableid, array, bookmarkSize);
			}
			finally
			{
				if (array != null)
				{
					Caches.BookmarkCache.Free(ref array);
				}
			}
		}

		public void Cancel()
		{
			base.CheckObjectIsNotDisposed();
			if (!base.HasResource)
			{
				throw new InvalidOperationException("Not in an update");
			}
			Api.JetPrepareUpdate(this.sesid, this.tableid, JET_prep.Cancel);
			base.ResourceWasReleased();
		}

		protected override void ReleaseResource()
		{
			this.Cancel();
		}

		private readonly JET_SESID sesid;

		private readonly JET_TABLEID tableid;

		private readonly JET_prep prep;
	}
}
