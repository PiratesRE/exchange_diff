using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IParseLogonTracker
	{
		void ParseBegin(ServerObjectHandleTable serverObjectHandleTable);

		void ParseEnd();

		void ParseRecordLogon(byte logonIndex, byte handleTableIndex, LogonFlags logonFlags);

		void ParseRecordRelease(byte handleTableIndex);

		void ParseRecordInputOutput(byte handleTableIndex);

		bool ParseIsValidLogon(byte logonIndex);

		bool ParseIsPublicLogon(byte logonIndex);
	}
}
