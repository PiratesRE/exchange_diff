using System;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ExchangeSecurityDescriptorHelper
	{
		internal static RawSecurityDescriptor RemoveInheritedACEs(RawSecurityDescriptor sd)
		{
			if (sd == null)
			{
				return null;
			}
			RawAcl discretionaryAcl = sd.DiscretionaryAcl;
			bool flag = false;
			foreach (GenericAce genericAce in discretionaryAcl)
			{
				if ((byte)(genericAce.AceFlags & AceFlags.Inherited) == 16)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return sd;
			}
			RawAcl rawAcl = new RawAcl(discretionaryAcl.Revision, 0);
			foreach (GenericAce genericAce2 in discretionaryAcl)
			{
				if ((byte)(genericAce2.AceFlags & AceFlags.Inherited) != 16)
				{
					rawAcl.InsertAce(rawAcl.Count, genericAce2);
				}
			}
			return new RawSecurityDescriptor(sd.ControlFlags, sd.Owner, sd.Group, sd.SystemAcl, rawAcl);
		}
	}
}
