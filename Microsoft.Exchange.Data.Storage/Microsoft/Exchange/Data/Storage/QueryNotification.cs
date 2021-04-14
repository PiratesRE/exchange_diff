using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class QueryNotification : Notification
	{
		internal QueryNotification(QueryNotificationType eventType, int errorCode, byte[] index, byte[] prior, ICollection<PropertyDefinition> propertyDefinitions, object[] row) : base(NotificationType.Query)
		{
			this.propertyDefinitions = propertyDefinitions;
			this.eventType = eventType;
			this.errorCode = errorCode;
			this.index = index;
			this.prior = prior;
			this.row = row;
		}

		public QueryNotificationType EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public byte[] Index
		{
			get
			{
				return this.index;
			}
		}

		public byte[] Prior
		{
			get
			{
				return this.prior;
			}
		}

		public object[] Row
		{
			get
			{
				return this.row;
			}
		}

		public ICollection<PropertyDefinition> PropertyDefinitions
		{
			get
			{
				return this.propertyDefinitions;
			}
		}

		public static QueryNotification CreateQueryResultChangedNotification()
		{
			return new QueryNotification(QueryNotificationType.QueryResultChanged, 0, Array<byte>.Empty, Array<byte>.Empty, Array<UnresolvedPropertyDefinition>.Empty, Array<object>.Empty);
		}

		public QueryNotification CreateRowAddedNotification()
		{
			return new QueryNotification(QueryNotificationType.RowAdded, this.errorCode, this.index, this.prior, this.propertyDefinitions, this.row);
		}

		private readonly ICollection<PropertyDefinition> propertyDefinitions;

		private readonly QueryNotificationType eventType;

		private readonly int errorCode;

		private readonly byte[] index;

		private readonly byte[] prior;

		private readonly object[] row;
	}
}
