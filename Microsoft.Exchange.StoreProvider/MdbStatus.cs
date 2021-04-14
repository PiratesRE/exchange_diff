using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbStatus
	{
		internal MdbStatus(MDBSTATUSRAW pMdbStatus, IntPtr pStartAddress)
		{
			this.mdbGuid = pMdbStatus.guidMdb;
			this.status = (MdbStatusFlags)pMdbStatus.ulStatus;
			if (pMdbStatus.ibMdbName != 0U)
			{
				IntPtr ptr = (IntPtr)((long)pStartAddress + (long)((ulong)pMdbStatus.ibMdbName));
				if (Marshal.ReadByte(ptr, (int)(pMdbStatus.cbMdbName - 1U)) == 0)
				{
					this.mdbName = Marshal.PtrToStringAnsi(ptr, (int)(pMdbStatus.cbMdbName - 1U));
				}
			}
			if (pMdbStatus.ibVServerName != 0U)
			{
				IntPtr ptr2 = (IntPtr)((long)pStartAddress + (long)((ulong)pMdbStatus.ibVServerName));
				if (Marshal.ReadByte(ptr2, (int)(pMdbStatus.cbVServerName - 1U)) == 0)
				{
					this.vServerName = Marshal.PtrToStringAnsi(ptr2, (int)(pMdbStatus.cbVServerName - 1U));
				}
			}
			if (pMdbStatus.ibMdbLegacyDN != 0U)
			{
				IntPtr ptr3 = (IntPtr)((long)pStartAddress + (long)((ulong)pMdbStatus.ibMdbLegacyDN));
				if (Marshal.ReadByte(ptr3, (int)(pMdbStatus.cbMdbLegacyDN - 1U)) == 0)
				{
					this.mdbLegacyDN = Marshal.PtrToStringAnsi(ptr3, (int)(pMdbStatus.cbMdbLegacyDN - 1U));
				}
			}
		}

		internal MdbStatus(Guid _mdbGuid, uint _status)
		{
			this.mdbGuid = _mdbGuid;
			this.status = (MdbStatusFlags)_status;
		}

		public Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		public MdbStatusFlags Status
		{
			get
			{
				return this.status;
			}
		}

		public string MdbName
		{
			get
			{
				return this.mdbName;
			}
		}

		public string VServerName
		{
			get
			{
				return this.vServerName;
			}
		}

		public string MdbLegacyDN
		{
			get
			{
				return this.mdbLegacyDN;
			}
		}

		public override string ToString()
		{
			string str = string.Empty;
			if (this.vServerName != string.Empty)
			{
				str = string.Format("Virtual Server {0} ", this.vServerName);
			}
			if (this.mdbName != string.Empty)
			{
				str += string.Format("MDB {0} ", this.mdbName);
			}
			return str + string.Format("({0}): {1}\n", this.mdbGuid, this.status);
		}

		private Guid mdbGuid;

		private MdbStatusFlags status;

		private string mdbName = string.Empty;

		private string vServerName = string.Empty;

		private string mdbLegacyDN = string.Empty;
	}
}
