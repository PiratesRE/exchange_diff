using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	[Serializable]
	internal sealed class AmDatabaseMoveResult
	{
		private void BuildDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendFormat("AmDatabaseMoveResult: [ DbGuid='{0}', ", this.m_dbGuid);
			stringBuilder.AppendFormat("DbName='{0}', ", this.m_dbName);
			stringBuilder.AppendFormat("FromServer='{0}', ", this.m_fromServerFqdn);
			stringBuilder.AppendFormat("FinalActiveServer='{0}', ", this.m_finalActiveServerFqdn);
			stringBuilder.AppendFormat("MoveStatus='{0}', ", this.m_dbMoveStatus);
			stringBuilder.AppendFormat("MountStatusAtMoveStart='{0}', ", this.m_dbMountStatusAtStart);
			stringBuilder.AppendFormat("MountStatusAtMoveEnd='{0}', ", this.m_dbMountStatusAtEnd);
			stringBuilder.AppendFormat("ErrorInfo='{0}', ", this.m_errorInfo);
			int num = 0;
			List<AmDbRpcOperationSubStatus> attemptedServerSubStatuses = this.m_attemptedServerSubStatuses;
			if (attemptedServerSubStatuses != null && attemptedServerSubStatuses.Count > 0)
			{
				num = this.m_attemptedServerSubStatuses.Count;
			}
			stringBuilder.AppendFormat("AttemptedServerSubStatuses: [Count='{0}'] ", num);
			stringBuilder.Append("]");
			this.m_debugString = stringBuilder.ToString();
		}

		public AmDatabaseMoveResult(Guid dbGuid, string dbName, string fromServerFqdn, string finalActiveServerFqdn, AmDbMoveStatus dbMoveStatus, AmDbMountStatus dbMountStatusAtStart, AmDbMountStatus dbMountStatusAtEnd, RpcErrorExceptionInfo errorInfo, List<AmDbRpcOperationSubStatus> attemptedServerSubStatuses)
		{
			this.m_isLegacy = false;
			this.BuildDebugString();
		}

		public sealed override string ToString()
		{
			return this.m_debugString;
		}

		public bool IsLegacy
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_isLegacy;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_isLegacy = value;
			}
		}

		public Guid DbGuid
		{
			get
			{
				return this.m_dbGuid;
			}
		}

		public string DbName
		{
			get
			{
				return this.m_dbName;
			}
		}

		public string FromServerFqdn
		{
			get
			{
				return this.m_fromServerFqdn;
			}
		}

		public string FinalActiveServerFqdn
		{
			get
			{
				return this.m_finalActiveServerFqdn;
			}
		}

		public AmDbMoveStatus MoveStatus
		{
			get
			{
				return this.m_dbMoveStatus;
			}
		}

		public AmDbMountStatus MountStatusAtStart
		{
			get
			{
				return this.m_dbMountStatusAtStart;
			}
		}

		public AmDbMountStatus MountStatusAtEnd
		{
			get
			{
				return this.m_dbMountStatusAtEnd;
			}
		}

		public RpcErrorExceptionInfo ErrorInfo
		{
			get
			{
				return this.m_errorInfo;
			}
		}

		public List<AmDbRpcOperationSubStatus> AttemptedServerSubStatuses
		{
			get
			{
				return this.m_attemptedServerSubStatuses;
			}
		}

		private readonly Guid m_dbGuid = dbGuid;

		private readonly string m_dbName = dbName;

		private readonly string m_fromServerFqdn = fromServerFqdn;

		private readonly string m_finalActiveServerFqdn = finalActiveServerFqdn;

		private readonly AmDbMoveStatus m_dbMoveStatus = dbMoveStatus;

		private readonly AmDbMountStatus m_dbMountStatusAtStart = dbMountStatusAtStart;

		private readonly AmDbMountStatus m_dbMountStatusAtEnd = dbMountStatusAtEnd;

		private readonly RpcErrorExceptionInfo m_errorInfo = errorInfo;

		private readonly List<AmDbRpcOperationSubStatus> m_attemptedServerSubStatuses = attemptedServerSubStatuses;

		private bool m_isLegacy;

		private string m_debugString;
	}
}
