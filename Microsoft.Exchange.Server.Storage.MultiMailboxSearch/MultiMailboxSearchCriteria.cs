using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MultiMailboxSearch;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.MultiMailboxSearch
{
	internal sealed class MultiMailboxSearchCriteria
	{
		internal MultiMailboxSearchCriteria(Guid queryCorrelationId, SearchCriteria criteria, Guid mailboxGuid, int mailboxNumber, string query) : this(queryCorrelationId, criteria, mailboxGuid, mailboxNumber, query, 0, string.Empty, string.Empty)
		{
			MultiMailboxSearchCriteria.TraceFunction("Entering MultiMailboxSearchCriteria.ctor(searchCriteria, query)");
			MultiMailboxSearchCriteria.TraceFunction("Exiting MultiMailboxSearchCriteria.ctor(searchCriteria, query)");
		}

		internal MultiMailboxSearchCriteria(Guid queryCorrelationId, SearchCriteria criteria, Guid mailboxGuid, int mailboxNumber, string query, int pageSize, string sortSpecification, string paginationClause)
		{
			MultiMailboxSearchCriteria.TraceFunction("Entering MultiMailboxSearchCriteria.ctor");
			this.queryCorrelationId = queryCorrelationId;
			this.searchCriteria = criteria;
			this.mailboxGuid = mailboxGuid;
			this.query = query;
			this.pageSize = pageSize;
			this.sortSpecification = sortSpecification;
			this.paginationClause = paginationClause;
			this.mailboxNumber = mailboxNumber;
			MultiMailboxSearchCriteria.TraceFunction("Exiting MultiMailboxSearchCriteria.ctor");
		}

		internal string PaginationClause
		{
			get
			{
				return this.paginationClause;
			}
		}

		internal int PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		internal string SortSpecification
		{
			get
			{
				return this.sortSpecification;
			}
		}

		internal SearchCriteria SearchCriteria
		{
			get
			{
				return this.searchCriteria;
			}
		}

		internal string Query
		{
			get
			{
				return this.query;
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal Guid QueryCorrelationId
		{
			get
			{
				return this.queryCorrelationId;
			}
		}

		internal int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		private static void TraceFunction(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}
			if (ExTraceGlobals.SearchTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				ExTraceGlobals.SearchTracer.TraceFunction(35728, 0L, message);
			}
		}

		private readonly SearchCriteria searchCriteria;

		private readonly Guid queryCorrelationId;

		private readonly Guid mailboxGuid;

		private readonly string query;

		private readonly int pageSize;

		private readonly string sortSpecification;

		private readonly string paginationClause;

		private readonly int mailboxNumber;
	}
}
