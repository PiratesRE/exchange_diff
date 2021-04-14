using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class LogonResult : RopResult
	{
		protected LogonResult(ErrorCode errorCode, IServerObject returnObject) : base(RopId.Logon, errorCode, returnObject)
		{
		}

		protected LogonResult(Reader reader) : base(reader)
		{
		}

		public static RopResult Parse(Reader reader)
		{
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			ErrorCode errorCode2 = errorCode;
			if (errorCode2 != ErrorCode.None)
			{
				if (errorCode2 == ErrorCode.WrongServer)
				{
					return new RedirectLogonResult(reader);
				}
				return StandardRopResult.ParseFailResult(reader);
			}
			else
			{
				LogonFlags logonFlags = (LogonFlags)reader.PeekByte(6L);
				if ((byte)(logonFlags & LogonFlags.Private) == 1)
				{
					return new SuccessfulPrivateLogonResult(reader);
				}
				return new SuccessfulPublicLogonResult(reader);
			}
		}
	}
}
