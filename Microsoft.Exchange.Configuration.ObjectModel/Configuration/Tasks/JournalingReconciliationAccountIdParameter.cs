using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class JournalingReconciliationAccountIdParameter : ADIdParameter
	{
		public JournalingReconciliationAccountIdParameter()
		{
		}

		public JournalingReconciliationAccountIdParameter(string identity) : base(identity)
		{
		}

		public JournalingReconciliationAccountIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public JournalingReconciliationAccountIdParameter(JournalingReconciliationAccount connector) : base(connector.Id)
		{
		}

		public JournalingReconciliationAccountIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static JournalingReconciliationAccountIdParameter Parse(string identity)
		{
			return new JournalingReconciliationAccountIdParameter(identity);
		}

		public JournalingReconciliationAccount GetObject(IConfigDataProvider session)
		{
			IEnumerable<JournalingReconciliationAccount> objects = base.GetObjects<JournalingReconciliationAccount>(null, session);
			IEnumerator<JournalingReconciliationAccount> enumerator = objects.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(this.ToString()));
			}
			JournalingReconciliationAccount result = enumerator.Current;
			if (enumerator.MoveNext())
			{
				throw new ManagementObjectAmbiguousException(Strings.ErrorManagementObjectAmbiguous(this.ToString()));
			}
			return result;
		}
	}
}
