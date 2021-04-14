using System;

namespace Microsoft.Office.CompliancePolicy
{
	[Serializable]
	public sealed class ServerProtocolViolationException : Exception
	{
		public ServerProtocolViolationException(long size) : base(string.Format("A server protocol violation occurred. The server is sending more data then it actually committed ({0})", size))
		{
		}
	}
}
