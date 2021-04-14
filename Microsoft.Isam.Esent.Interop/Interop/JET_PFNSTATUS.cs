using System;

namespace Microsoft.Isam.Esent.Interop
{
	public delegate JET_err JET_PFNSTATUS(JET_SESID sesid, JET_SNP snp, JET_SNT snt, object data);
}
