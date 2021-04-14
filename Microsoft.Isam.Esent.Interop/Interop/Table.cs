using System;

namespace Microsoft.Isam.Esent.Interop
{
	public class Table : EsentResource
	{
		public Table(JET_SESID sesid, JET_DBID dbid, string name, OpenTableGrbit grbit)
		{
			this.sesid = sesid;
			this.name = name;
			Api.JetOpenTable(this.sesid, dbid, this.name, null, 0, grbit, out this.tableid);
			base.ResourceWasAllocated();
		}

		public string Name
		{
			get
			{
				base.CheckObjectIsNotDisposed();
				return this.name;
			}
		}

		public JET_TABLEID JetTableid
		{
			get
			{
				base.CheckObjectIsNotDisposed();
				return this.tableid;
			}
		}

		public static implicit operator JET_TABLEID(Table table)
		{
			return table.JetTableid;
		}

		public override string ToString()
		{
			return this.name;
		}

		public void Close()
		{
			base.CheckObjectIsNotDisposed();
			this.ReleaseResource();
		}

		protected override void ReleaseResource()
		{
			Api.JetCloseTable(this.sesid, this.tableid);
			this.sesid = JET_SESID.Nil;
			this.tableid = JET_TABLEID.Nil;
			this.name = null;
			base.ResourceWasReleased();
		}

		private JET_SESID sesid;

		private JET_TABLEID tableid;

		private string name;
	}
}
