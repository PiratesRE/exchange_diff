using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SimpleAdObjectLookup<TADWrapperObject> : IFindAdObject<TADWrapperObject> where TADWrapperObject : class, IADObjectCommon
	{
		public static TADWrapperObject FindAdObjectWithQueryStatic(IADToplogyConfigurationSession adSession, QueryFilter queryFilter)
		{
			TADWrapperObject answer = default(TADWrapperObject);
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				TADWrapperObject[] array = adSession.Find<TADWrapperObject>(null, QueryScope.SubTree, queryFilter, null, 2);
				if (array.Length > 1)
				{
					SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<QueryFilter, TADWrapperObject, TADWrapperObject>(0L, "FindAdObjectWithQueryStatic found multiple objects with query '{0}'! The first two are '{1}' and '{2}'.", queryFilter, array[0], array[1]);
					answer = array[(Environment.TickCount & int.MaxValue) % 2];
					return;
				}
				if (array.Length == 0)
				{
					SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<QueryFilter>(0L, "FindAdObjectWithQueryStatic found no objects with query '{0}'!", queryFilter);
					return;
				}
				answer = array[0];
			}, 2);
			if (ex != null)
			{
				SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<Exception>(0L, "FindAdObjectWithQueryStatic got an exception: {0}", ex);
			}
			return answer;
		}

		public SimpleAdObjectLookup(IADToplogyConfigurationSession adSession)
		{
			this.AdSession = adSession;
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		public IADToplogyConfigurationSession AdSession { get; set; }

		public static TADWrapperObject FindAdObjectTypeByGuidStatic(IADToplogyConfigurationSession adSession, Guid objectGuid)
		{
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectTypeByGuidStatic(adSession, objectGuid, NullPerformanceDataLogger.Instance);
		}

		public static TADWrapperObject FindAdObjectTypeByGuidStatic(IADToplogyConfigurationSession adSession, Guid objectGuid, IPerformanceDataLogger perfLogger)
		{
			if (objectGuid == Guid.Empty)
			{
				throw new ArgumentException("objectGuid cannot be Empty.");
			}
			TADWrapperObject adObject = default(TADWrapperObject);
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, objectGuid);
				TADWrapperObject[] array = adSession.Find<TADWrapperObject>(null, QueryScope.SubTree, filter, null, 1);
				adObject = ((array != null && array.Length > 0) ? array[0] : default(TADWrapperObject));
			}, perfLogger, 2);
			if (ex != null)
			{
				SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<Exception>(0L, "FindAdObjectTypeByGuidStatic got an exception: {0}", ex);
			}
			return adObject;
		}

		public static TADWrapperObject FindAdObjectByServerNameStatic(IADToplogyConfigurationSession adSession, string serverName, out Exception exception)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentNullException("serverName");
			}
			exception = null;
			TADWrapperObject adObject = default(TADWrapperObject);
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, serverName);
				TADWrapperObject[] array = adSession.Find<TADWrapperObject>(null, QueryScope.SubTree, filter, null, 1);
				adObject = ((array != null && array.Length > 0) ? array[0] : default(TADWrapperObject));
			}, 2);
			if (ex != null)
			{
				exception = ex;
				SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<Exception>(0L, "FindAdObjectByServerNameStatic got exception: {0}", ex);
			}
			return adObject;
		}

		public void Clear()
		{
		}

		public TADWrapperObject ReadAdObjectByObjectId(ADObjectId objectId)
		{
			Exception ex;
			return this.ReadAdObjectByObjectIdEx(objectId, out ex);
		}

		public TADWrapperObject ReadAdObjectByObjectIdEx(ADObjectId objectId, out Exception ex)
		{
			TADWrapperObject adObject = default(TADWrapperObject);
			ex = ADUtils.RunADOperation(delegate()
			{
				adObject = this.AdSession.ReadADObject<TADWrapperObject>(objectId);
			}, 2);
			if (ex != null)
			{
				SimpleAdObjectLookup<TADWrapperObject>.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SimpleAdObjectLookup.ReadAdObjectByObjectIdEx got an exception: {0}", ex);
			}
			return adObject;
		}

		public TADWrapperObject FindAdObjectByGuid(Guid objectGuid)
		{
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectTypeByGuidStatic(this.AdSession, objectGuid);
		}

		public TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags)
		{
			return this.FindAdObjectByGuidEx(objectGuid, flags, NullPerformanceDataLogger.Instance);
		}

		public TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags, IPerformanceDataLogger perfLogger)
		{
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectTypeByGuidStatic(this.AdSession, objectGuid, perfLogger);
		}

		public TADWrapperObject FindAdObjectByQuery(QueryFilter queryFilter)
		{
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectWithQueryStatic(this.AdSession, queryFilter);
		}

		public TADWrapperObject FindAdObjectByQueryEx(QueryFilter queryFilter, AdObjectLookupFlags flags)
		{
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectWithQueryStatic(this.AdSession, queryFilter);
		}

		public TADWrapperObject FindServerByFqdn(string fqdn)
		{
			Exception ex;
			return this.FindServerByFqdnWithException(fqdn, out ex);
		}

		public TADWrapperObject FindServerByFqdnWithException(string fqdn, out Exception ex)
		{
			ex = null;
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(fqdn);
			return SimpleAdObjectLookup<TADWrapperObject>.FindAdObjectByServerNameStatic(this.AdSession, nodeNameFromFqdn, out ex);
		}
	}
}
