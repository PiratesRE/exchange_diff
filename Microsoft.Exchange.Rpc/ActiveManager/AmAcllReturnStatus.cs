using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.ActiveManager
{
	[Serializable]
	internal sealed class AmAcllReturnStatus
	{
		public AmAcllReturnStatus()
		{
			this.m_noLoss = false;
			this.m_mountAllowed = false;
			this.m_mountDialOverrideUsed = false;
			this.m_lastLogGenShipped = -1L;
			this.m_lastLogGenNotified = -1L;
			this.m_numLogsLost = -1L;
			this.m_lastInspectedLogTime = DateTime.MinValue;
			this.m_lastErrorMsg = null;
		}

		public sealed override string ToString()
		{
			object[] array = new object[8];
			array[0] = this.m_noLoss;
			array[1] = this.m_mountAllowed;
			array[2] = this.m_mountDialOverrideUsed;
			array[3] = this.m_lastLogGenShipped;
			array[4] = this.m_lastLogGenNotified;
			array[5] = this.m_numLogsLost;
			array[6] = this.m_lastInspectedLogTime;
			string lastErrorMsg;
			if (this.m_lastErrorMsg == null)
			{
				lastErrorMsg = AmAcllReturnStatus.s_StringNone;
			}
			else
			{
				lastErrorMsg = this.m_lastErrorMsg;
			}
			array[7] = lastErrorMsg;
			return string.Format(AmAcllReturnStatus.s_ToStringFormat, array);
		}

		public bool NoLoss
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_noLoss;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_noLoss = value;
			}
		}

		public bool MountAllowed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_mountAllowed;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_mountAllowed = value;
			}
		}

		public bool MountDialOverrideUsed
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.m_mountDialOverrideUsed;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.m_mountDialOverrideUsed = value;
			}
		}

		public long LastLogGenerationShipped
		{
			get
			{
				return this.m_lastLogGenShipped;
			}
			set
			{
				this.m_lastLogGenShipped = value;
			}
		}

		public long LastLogGenerationNotified
		{
			get
			{
				return this.m_lastLogGenNotified;
			}
			set
			{
				this.m_lastLogGenNotified = value;
			}
		}

		public long NumberOfLogsLost
		{
			get
			{
				return this.m_numLogsLost;
			}
			set
			{
				this.m_numLogsLost = value;
			}
		}

		public DateTime LastInspectedLogTime
		{
			get
			{
				return this.m_lastInspectedLogTime;
			}
			set
			{
				this.m_lastInspectedLogTime = value;
			}
		}

		public string LastError
		{
			get
			{
				return this.m_lastErrorMsg;
			}
			set
			{
				this.m_lastErrorMsg = value;
			}
		}

		private bool m_noLoss;

		private bool m_mountAllowed;

		private bool m_mountDialOverrideUsed;

		private long m_lastLogGenShipped;

		private long m_lastLogGenNotified;

		private long m_numLogsLost;

		private DateTime m_lastInspectedLogTime;

		private string m_lastErrorMsg;

		private static string s_StringNone = "<none>";

		private static string s_ToStringFormat = "AmAcllReturnStatus: [NoLoss={0}, MountAllowed={1}, MountDialOverrideUsed={2}, LastLogGenerationShipped={3}, LastLogGenerationNotified={4}, NumberOfLogsLost={5}, LastInspectedLogTime={6}, LastError={7}]";

		public const long InvalidLogGeneration = -1L;
	}
}
