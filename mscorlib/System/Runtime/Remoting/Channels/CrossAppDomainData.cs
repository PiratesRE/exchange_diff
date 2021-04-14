using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	internal class CrossAppDomainData
	{
		internal virtual IntPtr ContextID
		{
			get
			{
				return new IntPtr((long)this._ContextID);
			}
		}

		internal virtual int DomainID
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this._DomainID;
			}
		}

		internal virtual string ProcessGuid
		{
			get
			{
				return this._processGuid;
			}
		}

		internal CrossAppDomainData(IntPtr ctxId, int domainID, string processGuid)
		{
			this._DomainID = domainID;
			this._processGuid = processGuid;
			this._ContextID = ctxId.ToInt64();
		}

		internal bool IsFromThisProcess()
		{
			return Identity.ProcessGuid.Equals(this._processGuid);
		}

		[SecurityCritical]
		internal bool IsFromThisAppDomain()
		{
			return this.IsFromThisProcess() && Thread.GetDomain().GetId() == this._DomainID;
		}

		private object _ContextID = 0;

		private int _DomainID;

		private string _processGuid;
	}
}
