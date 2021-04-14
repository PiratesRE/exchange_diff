using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal interface IJetSimpleQueryOperator : IJetRecordCounter, ITWIR
	{
		byte[] GetColumnValueAsBytes(Column column);

		byte[] GetPhysicalColumnValueAsBytes(PhysicalColumn column);

		bool MoveFirst(out int rowsSkipped);

		bool MoveNext();

		bool Interrupted { get; }

		void RequestResume();

		bool CanMoveBack { get; }

		void MoveBackAndInterrupt(int rows);
	}
}
