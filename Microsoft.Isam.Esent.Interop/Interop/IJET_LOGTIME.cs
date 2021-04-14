using System;

namespace Microsoft.Isam.Esent.Interop
{
	public interface IJET_LOGTIME : INullableJetStruct
	{
		DateTime? ToDateTime();
	}
}
