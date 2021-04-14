using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExMapiProp : IExInterface, IDisposeTrackable, IDisposable
	{
		int SaveChanges(int ulFlags);

		int GetProps(ICollection<PropTag> lpPropTags, int ulFlags, out PropValue[] lppPropArray);

		int GetPropList(int ulFlags, out PropTag[] propList);

		int OpenProperty(int propTag, Guid lpInterface, int interfaceOptions, int ulFlags, out IExInterface iObj);

		int SetProps(ICollection<PropValue> lpPropArray, out PropProblem[] lppProblems);

		int DeleteProps(ICollection<PropTag> lpPropTags, out PropProblem[] lppProblems);

		int CopyTo(int ciidExclude, Guid[] rgiidExclude, PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems);

		int CopyTo_External(int ciidExclude, Guid[] rgiidExclude, PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems);

		int CopyProps(PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, Guid lpInterface, IntPtr iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems);

		int CopyProps_External(PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp iMAPIPropDest, int ulFlags, out PropProblem[] lppProblems);

		int GetNamesFromIDs(ICollection<PropTag> lppPropTagArray, Guid guidPropSet, int ulFlags, out NamedProp[] lppNames);

		int GetIDsFromNames(ICollection<NamedProp> lppPropNames, int ulFlags, out PropTag[] lpPropIDs);
	}
}
