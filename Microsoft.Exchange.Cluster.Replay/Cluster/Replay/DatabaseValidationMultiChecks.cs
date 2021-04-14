using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class DatabaseValidationMultiChecks : IEnumerable<DatabaseValidationCheck>, IEnumerable
	{
		protected DatabaseValidationMultiChecks()
		{
			this.DefineChecks();
		}

		protected virtual void DefineChecks()
		{
			this.AddCheck(new DatabaseCheckDatabaseIsReplicated());
			this.AddCheck(new DatabaseCheckCopyStatusNotStale());
			this.AddCheck(new DatabaseCheckCopyStatusRpcSuccessful());
			this.AddCheck(new DatabaseCheckServerInMaintenanceMode());
			this.AddCheck(new DatabaseCheckActivationDisfavored());
		}

		protected void AddCheck(DatabaseValidationCheck check)
		{
			this.m_checks.Add(check);
		}

		public IEnumerator<DatabaseValidationCheck> GetEnumerator()
		{
			return this.m_checks.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_checks).GetEnumerator();
		}

		private List<DatabaseValidationCheck> m_checks = new List<DatabaseValidationCheck>(10);
	}
}
