using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMCoreADUtil
	{
		internal static void GetDisambiguatedNameForRecipient(IADRecipient r, string displayNameToUse, bool disambiguate, DisambiguationFieldEnum disambiguationField, out string displayName, out string disambiguationText)
		{
			disambiguationText = null;
			displayName = displayNameToUse;
			string phoneticDisplayName = r.PhoneticDisplayName;
			PIIMessage data = PIIMessage.Create(PIIType._UserDisplayName, displayName);
			if (!string.IsNullOrEmpty(phoneticDisplayName))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "For User _UserDisplayName, PhoneticDisplayName: {0}.", new object[]
				{
					phoneticDisplayName
				});
				displayName = phoneticDisplayName;
			}
			IADOrgPerson iadorgPerson = r as IADOrgPerson;
			if (!disambiguate || iadorgPerson == null || disambiguationField == DisambiguationFieldEnum.None)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "For User '_UserDisplayName', no disambiguation text will be retrieved - disambiguate = {0}, recipient is ADOrgPerson = {1}, disambiguationField = {2}.", new object[]
				{
					disambiguate,
					iadorgPerson != null,
					disambiguationField
				});
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "GetDisambiguatedNameForRecipient(_UserDisplayName, {0}).", new object[]
			{
				disambiguationField
			});
			switch (disambiguationField)
			{
			case DisambiguationFieldEnum.Title:
				disambiguationText = iadorgPerson.Title;
				break;
			case DisambiguationFieldEnum.Department:
				disambiguationText = iadorgPerson.Department;
				break;
			case DisambiguationFieldEnum.Location:
				disambiguationText = iadorgPerson.Office;
				Regex.Replace(disambiguationText ?? string.Empty, "(?<building>\\d\\d+)/(?<room>\\d\\d\\d\\d+)", "${building} / ${room}");
				break;
			}
			if (string.IsNullOrEmpty(disambiguationText))
			{
				disambiguationText = null;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "For User '_UserDisplayName', returning DisambiguationText = '{0}'.", new object[]
			{
				(disambiguationText == null) ? "<null>" : disambiguationText
			});
		}
	}
}
