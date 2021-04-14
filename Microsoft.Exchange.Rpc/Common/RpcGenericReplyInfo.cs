using System;
using System.Text;

namespace Microsoft.Exchange.Rpc.Common
{
	internal sealed class RpcGenericReplyInfo
	{
		private void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			stringBuilder.AppendFormat("RpcGenericReplyInfo: [ServerVersion='{0}', ", this.m_serverVersion);
			int commandId = this.m_commandId;
			stringBuilder.AppendFormat("CommandId='{0}', ", commandId.ToString());
			int commandMajorVersion = this.m_commandMajorVersion;
			stringBuilder.AppendFormat("MajorVersion='{0}', ", commandMajorVersion.ToString());
			int commandMinorVersion = this.m_commandMinorVersion;
			stringBuilder.AppendFormat("MinorVersion='{0}', ", commandMinorVersion.ToString());
			byte[] attachedData = this.m_attachedData;
			string arg;
			if (attachedData == null)
			{
				arg = "<null>";
			}
			else
			{
				arg = attachedData.Length.ToString();
			}
			stringBuilder.AppendFormat("AttachedDataSize='{0}', ", arg);
			stringBuilder.Append("]");
			this.m_debugString = stringBuilder.ToString();
		}

		public RpcGenericReplyInfo(int serverVersion, int commandId, int commandMajorVersion, int commandMinorVersion, byte[] attachedData)
		{
			this.m_attachedData = attachedData;
			base..ctor();
			this.BuildDebugString();
		}

		public RpcGenericReplyInfo(int serverVersion, int commandId, int commandMajorVersion, int commandMinorVersion)
		{
			this.m_attachedData = null;
			base..ctor();
		}

		public sealed override string ToString()
		{
			return this.m_debugString;
		}

		public int ServerVersion
		{
			get
			{
				return this.m_serverVersion;
			}
		}

		public int CommandId
		{
			get
			{
				return this.m_commandId;
			}
		}

		public int CommandMajorVersion
		{
			get
			{
				return this.m_commandMajorVersion;
			}
		}

		public int CommandMinorVersion
		{
			get
			{
				return this.m_commandMinorVersion;
			}
		}

		public byte[] AttachedData
		{
			get
			{
				return this.m_attachedData;
			}
		}

		private int m_serverVersion = serverVersion;

		private int m_commandId = commandId;

		private int m_commandMajorVersion = commandMajorVersion;

		private int m_commandMinorVersion = commandMinorVersion;

		private byte[] m_attachedData;

		private string m_debugString;
	}
}
