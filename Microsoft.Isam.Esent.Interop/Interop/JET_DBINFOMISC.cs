using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	public sealed class JET_DBINFOMISC : IEquatable<JET_DBINFOMISC>
	{
		public int ulIncrementalReseedCount
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulIncrementalReseedCount;
			}
			internal set
			{
				this._ulIncrementalReseedCount = value;
			}
		}

		public JET_LOGTIME logtimeIncrementalReseed
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeIncrementalReseed;
			}
			internal set
			{
				this._logtimeIncrementalReseed = value;
			}
		}

		public int ulIncrementalReseedCountOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulIncrementalReseedCountOld;
			}
			internal set
			{
				this._ulIncrementalReseedCountOld = value;
			}
		}

		public int ulPagePatchCount
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulPagePatchCount;
			}
			internal set
			{
				this._ulPagePatchCount = value;
			}
		}

		public JET_LOGTIME logtimePagePatch
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimePagePatch;
			}
			internal set
			{
				this._logtimePagePatch = value;
			}
		}

		public int ulPagePatchCountOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulPagePatchCountOld;
			}
			internal set
			{
				this._ulPagePatchCountOld = value;
			}
		}

		public JET_LOGTIME logtimeChecksumPrev
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeChecksumPrev;
			}
			internal set
			{
				this._logtimeChecksumPrev = value;
			}
		}

		public JET_LOGTIME logtimeChecksumStart
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeChecksumStart;
			}
			internal set
			{
				this._logtimeChecksumStart = value;
			}
		}

		public int cpgDatabaseChecked
		{
			[DebuggerStepThrough]
			get
			{
				return this._cpgDatabaseChecked;
			}
			internal set
			{
				this._cpgDatabaseChecked = value;
			}
		}

		internal void SetFromNativeDbinfoMisc6(ref NATIVE_DBINFOMISC6 native)
		{
			this.SetFromNativeDbinfoMisc(ref native.dbinfo4);
			this.cpgDatabaseChecked = (int)native.cpgDatabaseChecked;
			this.ulIncrementalReseedCount = (int)native.ulIncrementalReseedCount;
			this.ulIncrementalReseedCountOld = (int)native.ulIncrementalReseedCountOld;
			this.ulPagePatchCount = (int)native.ulPagePatchCount;
			this.ulPagePatchCountOld = (int)native.ulPagePatchCountOld;
			this.logtimeChecksumPrev = native.logtimeChecksumPrev;
			this.logtimeChecksumStart = native.logtimeChecksumStart;
			this.logtimeIncrementalReseed = native.logtimeIncrementalReseed;
			this.logtimePagePatch = native.logtimePagePatch;
		}

		internal NATIVE_DBINFOMISC6 GetNativeDbinfomisc6()
		{
			return new NATIVE_DBINFOMISC6
			{
				dbinfo4 = this.GetNativeDbinfomisc4(),
				cpgDatabaseChecked = (uint)this._cpgDatabaseChecked,
				ulIncrementalReseedCount = (uint)this._ulIncrementalReseedCount,
				ulIncrementalReseedCountOld = (uint)this._ulIncrementalReseedCountOld,
				ulPagePatchCount = (uint)this._ulPagePatchCount,
				ulPagePatchCountOld = (uint)this._ulPagePatchCountOld,
				logtimeChecksumPrev = this._logtimeChecksumPrev,
				logtimeChecksumStart = this._logtimeChecksumStart,
				logtimeIncrementalReseed = this._logtimeIncrementalReseed,
				logtimePagePatch = this._logtimePagePatch
			};
		}

		public JET_LOGTIME logtimeLastReAttach
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeLastReAttach;
			}
			internal set
			{
				this._logtimeLastReAttach = value;
			}
		}

		public JET_LGPOS lgposLastReAttach
		{
			[DebuggerStepThrough]
			get
			{
				return this._lgposLastReAttach;
			}
			internal set
			{
				this._lgposLastReAttach = value;
			}
		}

		internal void SetFromNativeDbinfoMisc(ref NATIVE_DBINFOMISC7 native)
		{
			this.SetFromNativeDbinfoMisc6(ref native.dbinfo6);
			this.logtimeLastReAttach = native.logtimeLastReAttach;
			this.lgposLastReAttach = native.lgposLastReAttach;
		}

		internal NATIVE_DBINFOMISC7 GetNativeDbinfomisc7()
		{
			return new NATIVE_DBINFOMISC7
			{
				dbinfo6 = this.GetNativeDbinfomisc6(),
				logtimeLastReAttach = this._logtimeLastReAttach,
				lgposLastReAttach = this._lgposLastReAttach
			};
		}

		private void NotYetPublishedEquals(JET_DBINFOMISC other, ref bool notYetPublishedEquals)
		{
			notYetPublishedEquals = (this._cpgDatabaseChecked == other._cpgDatabaseChecked && this._ulIncrementalReseedCount == other._ulIncrementalReseedCount && this._ulIncrementalReseedCountOld == other._ulIncrementalReseedCountOld && this._ulPagePatchCount == other._ulPagePatchCount && this._ulPagePatchCountOld == other._ulPagePatchCountOld && this._logtimeChecksumPrev == other._logtimeChecksumPrev && this._logtimeChecksumStart == other._logtimeChecksumStart && this._logtimeIncrementalReseed == other._logtimeIncrementalReseed && this._logtimePagePatch == other._logtimePagePatch && this._logtimeLastReAttach == other._logtimeLastReAttach && this._lgposLastReAttach == other._lgposLastReAttach);
		}

		private void AddNotYetPublishedHashCodes(IList<int> hashCodes)
		{
			hashCodes.Add(this._cpgDatabaseChecked);
			hashCodes.Add(this._ulIncrementalReseedCount);
			hashCodes.Add(this._ulIncrementalReseedCountOld);
			hashCodes.Add(this._ulPagePatchCount);
			hashCodes.Add(this._ulPagePatchCountOld);
			hashCodes.Add(this._logtimeChecksumPrev.GetHashCode());
			hashCodes.Add(this._logtimeChecksumStart.GetHashCode());
			hashCodes.Add(this._logtimeIncrementalReseed.GetHashCode());
			hashCodes.Add(this._logtimePagePatch.GetHashCode());
			hashCodes.Add(this._logtimeLastReAttach.GetHashCode());
			hashCodes.Add(this._lgposLastReAttach.GetHashCode());
		}

		public int ulVersion
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulVersion;
			}
			internal set
			{
				this._ulVersion = value;
			}
		}

		public int ulUpdate
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulUpdate;
			}
			internal set
			{
				this._ulUpdate = value;
			}
		}

		public JET_SIGNATURE signDb
		{
			[DebuggerStepThrough]
			get
			{
				return this._signDb;
			}
			internal set
			{
				this._signDb = value;
			}
		}

		public JET_dbstate dbstate
		{
			[DebuggerStepThrough]
			get
			{
				return this._dbstate;
			}
			internal set
			{
				this._dbstate = value;
			}
		}

		public JET_LGPOS lgposConsistent
		{
			[DebuggerStepThrough]
			get
			{
				return this._lgposConsistent;
			}
			internal set
			{
				this._lgposConsistent = value;
			}
		}

		public JET_LOGTIME logtimeConsistent
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeConsistent;
			}
			internal set
			{
				this._logtimeConsistent = value;
			}
		}

		public JET_LOGTIME logtimeAttach
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeAttach;
			}
			internal set
			{
				this._logtimeAttach = value;
			}
		}

		public JET_LGPOS lgposAttach
		{
			[DebuggerStepThrough]
			get
			{
				return this._lgposAttach;
			}
			internal set
			{
				this._lgposAttach = value;
			}
		}

		public JET_LOGTIME logtimeDetach
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeDetach;
			}
			internal set
			{
				this._logtimeDetach = value;
			}
		}

		public JET_LGPOS lgposDetach
		{
			[DebuggerStepThrough]
			get
			{
				return this._lgposDetach;
			}
			internal set
			{
				this._lgposDetach = value;
			}
		}

		public JET_SIGNATURE signLog
		{
			[DebuggerStepThrough]
			get
			{
				return this._signLog;
			}
			internal set
			{
				this._signLog = value;
			}
		}

		public JET_BKINFO bkinfoFullPrev
		{
			[DebuggerStepThrough]
			get
			{
				return this._bkinfoFullPrev;
			}
			internal set
			{
				this._bkinfoFullPrev = value;
			}
		}

		public JET_BKINFO bkinfoIncPrev
		{
			[DebuggerStepThrough]
			get
			{
				return this._bkinfoIncPrev;
			}
			internal set
			{
				this._bkinfoIncPrev = value;
			}
		}

		public JET_BKINFO bkinfoFullCur
		{
			[DebuggerStepThrough]
			get
			{
				return this._bkinfoFullCur;
			}
			internal set
			{
				this._bkinfoFullCur = value;
			}
		}

		public bool fShadowingDisabled
		{
			[DebuggerStepThrough]
			get
			{
				return this._fShadowingDisabled;
			}
			internal set
			{
				this._fShadowingDisabled = value;
			}
		}

		public bool fUpgradeDb
		{
			[DebuggerStepThrough]
			get
			{
				return this._fUpgradeDb;
			}
			internal set
			{
				this._fUpgradeDb = value;
			}
		}

		public int dwMajorVersion
		{
			[DebuggerStepThrough]
			get
			{
				return this._dwMajorVersion;
			}
			internal set
			{
				this._dwMajorVersion = value;
			}
		}

		public int dwMinorVersion
		{
			[DebuggerStepThrough]
			get
			{
				return this._dwMinorVersion;
			}
			internal set
			{
				this._dwMinorVersion = value;
			}
		}

		public int dwBuildNumber
		{
			[DebuggerStepThrough]
			get
			{
				return this._dwBuildNumber;
			}
			internal set
			{
				this._dwBuildNumber = value;
			}
		}

		public int lSPNumber
		{
			[DebuggerStepThrough]
			get
			{
				return this._lSPNumber;
			}
			internal set
			{
				this._lSPNumber = value;
			}
		}

		public int cbPageSize
		{
			[DebuggerStepThrough]
			get
			{
				return this._cbPageSize;
			}
			internal set
			{
				this._cbPageSize = value;
			}
		}

		public int genMinRequired
		{
			[DebuggerStepThrough]
			get
			{
				return this._genMinRequired;
			}
			internal set
			{
				this._genMinRequired = value;
			}
		}

		public int genMaxRequired
		{
			[DebuggerStepThrough]
			get
			{
				return this._genMaxRequired;
			}
			internal set
			{
				this._genMaxRequired = value;
			}
		}

		public JET_LOGTIME logtimeGenMaxCreate
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeGenMaxCreate;
			}
			internal set
			{
				this._logtimeGenMaxCreate = value;
			}
		}

		public int ulRepairCount
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulRepairCount;
			}
			internal set
			{
				this._ulRepairCount = value;
			}
		}

		public JET_LOGTIME logtimeRepair
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeRepair;
			}
			internal set
			{
				this._logtimeRepair = value;
			}
		}

		public int ulRepairCountOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulRepairCountOld;
			}
			internal set
			{
				this._ulRepairCountOld = value;
			}
		}

		public int ulECCFixSuccess
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulECCFixSuccess;
			}
			internal set
			{
				this._ulECCFixSuccess = value;
			}
		}

		public JET_LOGTIME logtimeECCFixSuccess
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeECCFixSuccess;
			}
			internal set
			{
				this._logtimeECCFixSuccess = value;
			}
		}

		public int ulECCFixSuccessOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulECCFixSuccessOld;
			}
			internal set
			{
				this._ulECCFixSuccessOld = value;
			}
		}

		public int ulECCFixFail
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulECCFixFail;
			}
			internal set
			{
				this._ulECCFixFail = value;
			}
		}

		public JET_LOGTIME logtimeECCFixFail
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeECCFixFail;
			}
			internal set
			{
				this._logtimeECCFixFail = value;
			}
		}

		public int ulECCFixFailOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulECCFixFailOld;
			}
			internal set
			{
				this._ulECCFixFailOld = value;
			}
		}

		public int ulBadChecksum
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulBadChecksum;
			}
			internal set
			{
				this._ulBadChecksum = value;
			}
		}

		public JET_LOGTIME logtimeBadChecksum
		{
			[DebuggerStepThrough]
			get
			{
				return this._logtimeBadChecksum;
			}
			internal set
			{
				this._logtimeBadChecksum = value;
			}
		}

		public int ulBadChecksumOld
		{
			[DebuggerStepThrough]
			get
			{
				return this._ulBadChecksumOld;
			}
			internal set
			{
				this._ulBadChecksumOld = value;
			}
		}

		public int genCommitted
		{
			[DebuggerStepThrough]
			get
			{
				return this._genCommitted;
			}
			internal set
			{
				this._genCommitted = value;
			}
		}

		public JET_BKINFO bkinfoCopyPrev
		{
			[DebuggerStepThrough]
			get
			{
				return this._bkinfoCopyPrev;
			}
			internal set
			{
				this._bkinfoCopyPrev = value;
			}
		}

		public JET_BKINFO bkinfoDiffPrev
		{
			[DebuggerStepThrough]
			get
			{
				return this._bkinfoDiffPrev;
			}
			internal set
			{
				this._bkinfoDiffPrev = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_DBINFOMISC({0})", new object[]
			{
				this._signDb
			});
		}

		public override int GetHashCode()
		{
			int[] collection = new int[]
			{
				this._ulVersion,
				this._ulUpdate,
				this._signDb.GetHashCode(),
				this._dbstate.GetHashCode(),
				this._lgposConsistent.GetHashCode(),
				this._logtimeConsistent.GetHashCode(),
				this._logtimeAttach.GetHashCode(),
				this._lgposAttach.GetHashCode(),
				this._logtimeDetach.GetHashCode(),
				this._lgposDetach.GetHashCode(),
				this._signLog.GetHashCode(),
				this._bkinfoFullPrev.GetHashCode(),
				this._bkinfoIncPrev.GetHashCode(),
				this._bkinfoFullCur.GetHashCode(),
				this._fShadowingDisabled.GetHashCode(),
				this._fUpgradeDb.GetHashCode(),
				this._dwMajorVersion,
				this._dwMinorVersion,
				this._dwBuildNumber,
				this._lSPNumber,
				this._cbPageSize,
				this._genMinRequired,
				this._genMaxRequired,
				this._logtimeGenMaxCreate.GetHashCode(),
				this._ulRepairCount,
				this._logtimeRepair.GetHashCode(),
				this._ulRepairCountOld,
				this._ulECCFixSuccess,
				this._logtimeECCFixSuccess.GetHashCode(),
				this._ulECCFixSuccessOld,
				this._ulECCFixFail,
				this._logtimeECCFixFail.GetHashCode(),
				this._ulECCFixFailOld,
				this._ulBadChecksum,
				this._logtimeBadChecksum.GetHashCode(),
				this._ulBadChecksumOld,
				this._genCommitted,
				this._bkinfoCopyPrev.GetHashCode(),
				this._bkinfoDiffPrev.GetHashCode()
			};
			List<int> list = new List<int>(collection);
			this.AddNotYetPublishedHashCodes(list);
			return Util.CalculateHashCode(list);
		}

		public override bool Equals(object obj)
		{
			return obj != null && !(base.GetType() != obj.GetType()) && this.Equals((JET_DBINFOMISC)obj);
		}

		public bool Equals(JET_DBINFOMISC other)
		{
			if (other == null)
			{
				return false;
			}
			bool flag = true;
			this.NotYetPublishedEquals(other, ref flag);
			return flag && this._ulVersion == other._ulVersion && this._ulUpdate == other._ulUpdate && this._signDb == other._signDb && this._dbstate == other._dbstate && this._lgposConsistent == other._lgposConsistent && this._logtimeConsistent == other._logtimeConsistent && this._logtimeAttach == other._logtimeAttach && this._lgposAttach == other._lgposAttach && this._logtimeDetach == other._logtimeDetach && this._lgposDetach == other._lgposDetach && this._signLog == other._signLog && this._bkinfoFullPrev == other._bkinfoFullPrev && this._bkinfoIncPrev == other._bkinfoIncPrev && this._bkinfoFullCur == other._bkinfoFullCur && this._fShadowingDisabled == other._fShadowingDisabled && this._fUpgradeDb == other._fUpgradeDb && this._dwMajorVersion == other._dwMajorVersion && this._dwMinorVersion == other._dwMinorVersion && this._dwBuildNumber == other._dwBuildNumber && this._lSPNumber == other._lSPNumber && this._cbPageSize == other._cbPageSize && this._genMinRequired == other._genMinRequired && this._genMaxRequired == other._genMaxRequired && this._logtimeGenMaxCreate == other._logtimeGenMaxCreate && this._ulRepairCount == other._ulRepairCount && this._logtimeRepair == other._logtimeRepair && this._ulRepairCountOld == other._ulRepairCountOld && this._ulECCFixSuccess == other._ulECCFixSuccess && this._logtimeECCFixSuccess == other._logtimeECCFixSuccess && this._ulECCFixSuccessOld == other._ulECCFixSuccessOld && this._ulECCFixFail == other._ulECCFixFail && this._logtimeECCFixFail == other._logtimeECCFixFail && this._ulECCFixFailOld == other._ulECCFixFailOld && this._ulBadChecksum == other._ulBadChecksum && this._logtimeBadChecksum == other._logtimeBadChecksum && this._ulBadChecksumOld == other._ulBadChecksumOld && this._genCommitted == other._genCommitted && this._bkinfoCopyPrev == other._bkinfoCopyPrev && this._bkinfoDiffPrev == other._bkinfoDiffPrev;
		}

		internal void SetFromNativeDbinfoMisc(ref NATIVE_DBINFOMISC native)
		{
			this._ulVersion = (int)native.ulVersion;
			this._ulUpdate = (int)native.ulUpdate;
			this._signDb = new JET_SIGNATURE(native.signDb);
			this._dbstate = (JET_dbstate)native.dbstate;
			this._lgposConsistent = native.lgposConsistent;
			this._logtimeConsistent = native.logtimeConsistent;
			this._logtimeAttach = native.logtimeAttach;
			this._lgposAttach = native.lgposAttach;
			this._logtimeDetach = native.logtimeDetach;
			this._lgposDetach = native.lgposDetach;
			this._signLog = new JET_SIGNATURE(native.signLog);
			this._bkinfoFullPrev = native.bkinfoFullPrev;
			this._bkinfoIncPrev = native.bkinfoIncPrev;
			this._bkinfoFullCur = native.bkinfoFullCur;
			this._fShadowingDisabled = (0U != native.fShadowingDisabled);
			this._fUpgradeDb = (0U != native.fUpgradeDb);
			this._dwMajorVersion = (int)native.dwMajorVersion;
			this._dwMinorVersion = (int)native.dwMinorVersion;
			this._dwBuildNumber = (int)native.dwBuildNumber;
			this._lSPNumber = (int)native.lSPNumber;
			this._cbPageSize = (int)native.cbPageSize;
		}

		internal void SetFromNativeDbinfoMisc(ref NATIVE_DBINFOMISC4 native)
		{
			this.SetFromNativeDbinfoMisc(ref native.dbinfo);
			this._genMinRequired = (int)native.genMinRequired;
			this._genMaxRequired = (int)native.genMaxRequired;
			this._logtimeGenMaxCreate = native.logtimeGenMaxCreate;
			this._ulRepairCount = (int)native.ulRepairCount;
			this._logtimeRepair = native.logtimeRepair;
			this._ulRepairCountOld = (int)native.ulRepairCountOld;
			this._ulECCFixSuccess = (int)native.ulECCFixSuccess;
			this._logtimeECCFixSuccess = native.logtimeECCFixSuccess;
			this._ulECCFixSuccessOld = (int)native.ulECCFixSuccessOld;
			this._ulECCFixFail = (int)native.ulECCFixFail;
			this._logtimeECCFixFail = native.logtimeECCFixFail;
			this._ulECCFixFailOld = (int)native.ulECCFixFailOld;
			this._ulBadChecksum = (int)native.ulBadChecksum;
			this._logtimeBadChecksum = native.logtimeBadChecksum;
			this._ulBadChecksumOld = (int)native.ulBadChecksumOld;
			this._genCommitted = (int)native.genCommitted;
			this._bkinfoCopyPrev = native.bkinfoCopyPrev;
			this._bkinfoDiffPrev = native.bkinfoDiffPrev;
		}

		internal NATIVE_DBINFOMISC GetNativeDbinfomisc()
		{
			return new NATIVE_DBINFOMISC
			{
				ulVersion = (uint)this._ulVersion,
				ulUpdate = (uint)this._ulUpdate,
				signDb = this._signDb.GetNativeSignature(),
				dbstate = (uint)this._dbstate,
				lgposConsistent = this._lgposConsistent,
				logtimeConsistent = this._logtimeConsistent,
				logtimeAttach = this._logtimeAttach,
				lgposAttach = this._lgposAttach,
				logtimeDetach = this._logtimeDetach,
				lgposDetach = this._lgposDetach,
				signLog = this._signLog.GetNativeSignature(),
				bkinfoFullPrev = this._bkinfoFullPrev,
				bkinfoIncPrev = this._bkinfoIncPrev,
				bkinfoFullCur = this._bkinfoFullCur,
				fShadowingDisabled = (this._fShadowingDisabled ? 1U : 0U),
				fUpgradeDb = (this._fUpgradeDb ? 1U : 0U),
				dwMajorVersion = (uint)this._dwMajorVersion,
				dwMinorVersion = (uint)this._dwMinorVersion,
				dwBuildNumber = (uint)this._dwBuildNumber,
				lSPNumber = (uint)this._lSPNumber,
				cbPageSize = (uint)this._cbPageSize
			};
		}

		internal NATIVE_DBINFOMISC4 GetNativeDbinfomisc4()
		{
			return new NATIVE_DBINFOMISC4
			{
				dbinfo = this.GetNativeDbinfomisc(),
				genMinRequired = (uint)this._genMinRequired,
				genMaxRequired = (uint)this._genMaxRequired,
				logtimeGenMaxCreate = this._logtimeGenMaxCreate,
				ulRepairCount = (uint)this._ulRepairCount,
				logtimeRepair = this._logtimeRepair,
				ulRepairCountOld = (uint)this._ulRepairCountOld,
				ulECCFixSuccess = (uint)this._ulECCFixSuccess,
				logtimeECCFixSuccess = this._logtimeECCFixSuccess,
				ulECCFixSuccessOld = (uint)this._ulECCFixSuccessOld,
				ulECCFixFail = (uint)this._ulECCFixFail,
				logtimeECCFixFail = this._logtimeECCFixFail,
				ulECCFixFailOld = (uint)this._ulECCFixFailOld,
				ulBadChecksum = (uint)this._ulBadChecksum,
				logtimeBadChecksum = this._logtimeBadChecksum,
				ulBadChecksumOld = (uint)this._ulBadChecksumOld,
				genCommitted = (uint)this._genCommitted,
				bkinfoCopyPrev = this._bkinfoCopyPrev,
				bkinfoDiffPrev = this._bkinfoDiffPrev
			};
		}

		private int _ulIncrementalReseedCount;

		private JET_LOGTIME _logtimeIncrementalReseed;

		private int _ulIncrementalReseedCountOld;

		private int _ulPagePatchCount;

		private JET_LOGTIME _logtimePagePatch;

		private int _ulPagePatchCountOld;

		private JET_LOGTIME _logtimeChecksumPrev;

		private JET_LOGTIME _logtimeChecksumStart;

		private int _cpgDatabaseChecked;

		private JET_LOGTIME _logtimeLastReAttach;

		private JET_LGPOS _lgposLastReAttach;

		private int _ulVersion;

		private int _ulUpdate;

		private JET_SIGNATURE _signDb;

		private JET_dbstate _dbstate;

		private JET_LGPOS _lgposConsistent;

		private JET_LOGTIME _logtimeConsistent;

		private JET_LOGTIME _logtimeAttach;

		private JET_LGPOS _lgposAttach;

		private JET_LOGTIME _logtimeDetach;

		private JET_LGPOS _lgposDetach;

		private JET_SIGNATURE _signLog;

		private JET_BKINFO _bkinfoFullPrev;

		private JET_BKINFO _bkinfoIncPrev;

		private JET_BKINFO _bkinfoFullCur;

		private bool _fShadowingDisabled;

		private bool _fUpgradeDb;

		private int _dwMajorVersion;

		private int _dwMinorVersion;

		private int _dwBuildNumber;

		private int _lSPNumber;

		private int _cbPageSize;

		private int _genMinRequired;

		private int _genMaxRequired;

		private JET_LOGTIME _logtimeGenMaxCreate;

		private int _ulRepairCount;

		private JET_LOGTIME _logtimeRepair;

		private int _ulRepairCountOld;

		private int _ulECCFixSuccess;

		private JET_LOGTIME _logtimeECCFixSuccess;

		private int _ulECCFixSuccessOld;

		private int _ulECCFixFail;

		private JET_LOGTIME _logtimeECCFixFail;

		private int _ulECCFixFailOld;

		private int _ulBadChecksum;

		private JET_LOGTIME _logtimeBadChecksum;

		private int _ulBadChecksumOld;

		private int _genCommitted;

		private JET_BKINFO _bkinfoCopyPrev;

		private JET_BKINFO _bkinfoDiffPrev;
	}
}
