using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Model;

namespace Microsoft.Exchange.Services.OData
{
	internal abstract class EntityContainersCommand<TRequest, TResponse> : ExchangeServiceCommand<TRequest, TResponse> where TRequest : ODataRequest where TResponse : ODataResponse
	{
		public EntityContainersCommand(TRequest request) : base(request)
		{
		}

		protected virtual IExchangeEntityContainers EntityContainers
		{
			get
			{
				if (this.entityContainers == null)
				{
					TRequest request = base.Request;
					MailboxSession mailboxIdentityMailboxSession = request.ODataContext.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
					this.entityContainers = new ExchangeEntityContainers(mailboxIdentityMailboxSession);
				}
				return this.entityContainers;
			}
		}

		protected CommandContext CreateCommandContext(QueryAdapter queryAdapter = null)
		{
			CommandContext commandContext = new CommandContext();
			CommandContext commandContext2 = commandContext;
			TRequest request = base.Request;
			commandContext2.IfMatchETag = request.IfMatchETag;
			if (queryAdapter != null)
			{
				List<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> list = new List<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>();
				foreach (Microsoft.Exchange.Services.OData.Model.PropertyDefinition key in queryAdapter.RequestedProperties)
				{
					Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition item = null;
					if (Microsoft.Exchange.Services.OData.Model.EventSchema.ODataToEdmPropertyMap.TryGetValue(key, out item))
					{
						list.Add(item);
					}
				}
				commandContext.RequestedProperties = list;
				commandContext.PageSizeOnReread = queryAdapter.GetPageSize();
			}
			return commandContext;
		}

		protected ICalendarReference GetCalendarContainer(string calendarId = null)
		{
			ICalendarReference result;
			if (string.IsNullOrEmpty(calendarId) || string.Equals(calendarId, DistinguishedFolderIdName.calendar.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				result = this.EntityContainers.Calendaring.Calendars.Default;
			}
			else
			{
				result = this.EntityContainers.Calendaring.Calendars[EwsIdConverter.ODataIdToEwsId(calendarId)];
			}
			return result;
		}

		private IExchangeEntityContainers entityContainers;
	}
}
