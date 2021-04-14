using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class Session : EsentResource
	{
		public Session(JET_INSTANCE instance)
		{
			Api.JetBeginSession(instance, out this.sesid, null, null);
			base.ResourceWasAllocated();
		}

		public JET_SESID JetSesid
		{
			get
			{
				base.CheckObjectIsNotDisposed();
				return this.sesid;
			}
		}

		public static implicit operator JET_SESID(Session session)
		{
			return session.JetSesid;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Session (0x{0:x})", new object[]
			{
				this.sesid.Value
			});
		}

		public void End()
		{
			base.CheckObjectIsNotDisposed();
			this.ReleaseResource();
		}

		protected override void ReleaseResource()
		{
			if (!this.sesid.IsInvalid)
			{
				Api.JetEndSession(this.JetSesid, EndSessionGrbit.None);
			}
			this.sesid = JET_SESID.Nil;
			base.ResourceWasReleased();
		}

		private JET_SESID sesid;
	}
}
