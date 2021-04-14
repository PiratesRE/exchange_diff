using System;
using System.Globalization;
using Microsoft.Isam.Esent.Interop.Windows8;

namespace Microsoft.Isam.Esent.Interop
{
	public class Transaction : EsentResource
	{
		public Transaction(JET_SESID sesid)
		{
			this.sesid = sesid;
			this.Begin();
		}

		public bool IsInTransaction
		{
			get
			{
				base.CheckObjectIsNotDisposed();
				return base.HasResource;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Transaction (0x{0:x})", new object[]
			{
				this.sesid.Value
			});
		}

		public void Begin()
		{
			base.CheckObjectIsNotDisposed();
			if (this.IsInTransaction)
			{
				throw new InvalidOperationException("Already in a transaction");
			}
			Api.JetBeginTransaction(this.sesid);
			base.ResourceWasAllocated();
		}

		public void Commit(CommitTransactionGrbit grbit)
		{
			base.CheckObjectIsNotDisposed();
			if (!this.IsInTransaction)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			Api.JetCommitTransaction(this.sesid, grbit);
			base.ResourceWasReleased();
		}

		public void Commit(CommitTransactionGrbit grbit, TimeSpan durableCommit, out JET_COMMIT_ID commitId)
		{
			base.CheckObjectIsNotDisposed();
			if (!this.IsInTransaction)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			Windows8Api.JetCommitTransaction2(this.sesid, grbit, durableCommit, out commitId);
			base.ResourceWasReleased();
		}

		public void Rollback()
		{
			base.CheckObjectIsNotDisposed();
			if (!this.IsInTransaction)
			{
				throw new InvalidOperationException("Not in a transaction");
			}
			Api.JetRollback(this.sesid, RollbackTransactionGrbit.None);
			base.ResourceWasReleased();
		}

		protected override void ReleaseResource()
		{
			this.Rollback();
		}

		private readonly JET_SESID sesid;
	}
}
