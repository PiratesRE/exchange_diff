using System;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop.Unpublished;

namespace Microsoft.Isam.Esent.Interop
{
	internal static class CallbackDataConverter
	{
		public static object GetManagedData(IntPtr nativeData, JET_SNP snp, JET_SNT snt)
		{
			if (IntPtr.Zero != nativeData)
			{
				if ((JET_SNP)18 == snp)
				{
					if (snt != JET_SNT.Progress)
					{
						switch (snt)
						{
						case (JET_SNT)1001:
						{
							NATIVE_SNOPENLOG native_SNOPENLOG = (NATIVE_SNOPENLOG)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNOPENLOG));
							JET_SNOPENLOG jet_SNOPENLOG = new JET_SNOPENLOG();
							jet_SNOPENLOG.SetFromNativeSnopenlog(ref native_SNOPENLOG);
							return jet_SNOPENLOG;
						}
						case (JET_SNT)1002:
						{
							NATIVE_SNOPENCHECKPOINT native_SNOPENCHECKPOINT = (NATIVE_SNOPENCHECKPOINT)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNOPENCHECKPOINT));
							JET_SNOPENCHECKPOINT jet_SNOPENCHECKPOINT = new JET_SNOPENCHECKPOINT();
							jet_SNOPENCHECKPOINT.SetFromNativeSnopencheckpoint(ref native_SNOPENCHECKPOINT);
							return jet_SNOPENCHECKPOINT;
						}
						case (JET_SNT)1004:
						{
							NATIVE_SNMISSINGLOG native_SNMISSINGLOG = (NATIVE_SNMISSINGLOG)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNMISSINGLOG));
							JET_SNMISSINGLOG jet_SNMISSINGLOG = new JET_SNMISSINGLOG();
							jet_SNMISSINGLOG.SetFromNativeSnmissinglog(ref native_SNMISSINGLOG);
							return jet_SNMISSINGLOG;
						}
						case (JET_SNT)1005:
						{
							NATIVE_SNBEGINUNDO native_SNBEGINUNDO = (NATIVE_SNBEGINUNDO)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNBEGINUNDO));
							JET_SNBEGINUNDO jet_SNBEGINUNDO = new JET_SNBEGINUNDO();
							jet_SNBEGINUNDO.SetFromNativeSnbeginundo(ref native_SNBEGINUNDO);
							return jet_SNBEGINUNDO;
						}
						case (JET_SNT)1006:
						{
							NATIVE_SNNOTIFICATIONEVENT native_SNNOTIFICATIONEVENT = (NATIVE_SNNOTIFICATIONEVENT)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNNOTIFICATIONEVENT));
							JET_SNNOTIFICATIONEVENT jet_SNNOTIFICATIONEVENT = new JET_SNNOTIFICATIONEVENT();
							jet_SNNOTIFICATIONEVENT.SetFromNativeSnnotificationevent(ref native_SNNOTIFICATIONEVENT);
							return jet_SNNOTIFICATIONEVENT;
						}
						case (JET_SNT)1007:
						{
							NATIVE_SNSIGNALERROR native_SNSIGNALERROR = (NATIVE_SNSIGNALERROR)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNSIGNALERROR));
							JET_SNSIGNALERROR jet_SNSIGNALERROR = new JET_SNSIGNALERROR();
							jet_SNSIGNALERROR.SetFromNativeSnsignalerror(ref native_SNSIGNALERROR);
							return jet_SNSIGNALERROR;
						}
						case (JET_SNT)1008:
						{
							NATIVE_SNDBATTACHED native_SNDBATTACHED = (NATIVE_SNDBATTACHED)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNDBATTACHED));
							JET_SNDBATTACHED jet_SNDBATTACHED = new JET_SNDBATTACHED();
							jet_SNDBATTACHED.SetFromNativeSndbattached(ref native_SNDBATTACHED);
							return jet_SNDBATTACHED;
						}
						case (JET_SNT)1009:
						{
							NATIVE_SNDBDETACHED native_SNDBDETACHED = (NATIVE_SNDBDETACHED)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNDBDETACHED));
							JET_SNDBDETACHED jet_SNDBDETACHED = new JET_SNDBDETACHED();
							jet_SNDBDETACHED.SetFromNativeSndbdetached(ref native_SNDBDETACHED);
							return jet_SNDBDETACHED;
						}
						}
						throw new ArgumentOutOfRangeException("snt", "Unknown snt value used with SNP=RecoveryControl: {0}", snt.ToString());
					}
					NATIVE_SNPROG fromNative = (NATIVE_SNPROG)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNPROG));
					JET_SNPROG jet_SNPROG = new JET_SNPROG();
					jet_SNPROG.SetFromNative(fromNative);
					return jet_SNPROG;
				}
				else if ((JET_SNP)19 == snp)
				{
					if ((JET_SNT)1101 == snt)
					{
						NATIVE_SNPATCHREQUEST native_SNPATCHREQUEST = (NATIVE_SNPATCHREQUEST)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNPATCHREQUEST));
						JET_SNPATCHREQUEST jet_SNPATCHREQUEST = new JET_SNPATCHREQUEST();
						jet_SNPATCHREQUEST.SetFromNativeSnpatchrequest(ref native_SNPATCHREQUEST);
						return jet_SNPATCHREQUEST;
					}
					if ((JET_SNT)1102 == snt)
					{
						NATIVE_SNCORRUPTEDPAGE native_SNCORRUPTEDPAGE = (NATIVE_SNCORRUPTEDPAGE)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNCORRUPTEDPAGE));
						JET_SNCORRUPTEDPAGE jet_SNCORRUPTEDPAGE = new JET_SNCORRUPTEDPAGE();
						jet_SNCORRUPTEDPAGE.SetFromNativeSncorruptedpage(ref native_SNCORRUPTEDPAGE);
						return jet_SNCORRUPTEDPAGE;
					}
				}
				else
				{
					if (JET_SNP.Compact == snp && snt == JET_SNT.Progress)
					{
						NATIVE_SNPROG fromNative2 = (NATIVE_SNPROG)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNPROG));
						JET_SNPROG jet_SNPROG2 = new JET_SNPROG();
						jet_SNPROG2.SetFromNative(fromNative2);
						return jet_SNPROG2;
					}
					if (JET_SNP.Restore == snp && snt == JET_SNT.Progress)
					{
						NATIVE_SNPROG fromNative3 = (NATIVE_SNPROG)Marshal.PtrToStructure(nativeData, typeof(NATIVE_SNPROG));
						JET_SNPROG jet_SNPROG3 = new JET_SNPROG();
						jet_SNPROG3.SetFromNative(fromNative3);
						return jet_SNPROG3;
					}
				}
			}
			return null;
		}
	}
}
