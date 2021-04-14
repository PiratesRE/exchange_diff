using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal class OrgEmptyMasterTableCache
	{
		internal static OrgEmptyMasterTableCache Singleton
		{
			get
			{
				return OrgEmptyMasterTableCache.singleton;
			}
		}

		internal bool IsEmpty(OrganizationId organizationId)
		{
			bool flag2;
			bool flag = this.emptyMasterTableDictionary.TryGetValue(organizationId.GetHashCode(), out flag2);
			OrgEmptyMasterTableCache.Tracer.TraceDebug<OrganizationId, bool>(0L, "OrgEmptyMasterTableCache.IsEmpty: Org: {0} IsEmpty: {1}", organizationId, flag);
			return flag;
		}

		internal void Update(OrganizationId organizationId, bool isEmpty)
		{
			OrgEmptyMasterTableCache.Tracer.TraceDebug<OrganizationId, bool>(0L, "OrgEmptyMasterTableCache.Update: Org: {0} IsEmpty: {1}", organizationId, isEmpty);
			if (isEmpty)
			{
				this.emptyMasterTableDictionary.Add(organizationId.GetHashCode(), isEmpty);
				return;
			}
			this.emptyMasterTableDictionary.Remove(organizationId.GetHashCode());
		}

		private const int MaxEntryCount = 20000;

		private static OrgEmptyMasterTableCache singleton = new OrgEmptyMasterTableCache();

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private MruDictionary<int, bool> emptyMasterTableDictionary = new MruDictionary<int, bool>(20000, new OrgEmptyMasterTableCache.IntComparer(), null);

		private sealed class IntComparer : IComparer<int>
		{
			int IComparer<int>.Compare(int x, int y)
			{
				return x.CompareTo(y);
			}
		}
	}
}
