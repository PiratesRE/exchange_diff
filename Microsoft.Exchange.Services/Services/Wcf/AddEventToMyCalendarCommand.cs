using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class AddEventToMyCalendarCommand : ServiceCommand<CalendarActionResponse>
	{
		public AddEventToMyCalendarCommand(CallContext callContext, AddEventToMyCalendarRequest request) : base(callContext)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.Request = request;
		}

		protected override CalendarActionResponse InternalExecute()
		{
			IdAndSession idAndSession = base.IdConverter.ConvertItemIdToIdAndSessionReadWrite(this.Request.ItemId);
			if (idAndSession != null)
			{
				try
				{
					using (Item rootXsoItem = idAndSession.GetRootXsoItem(AddEventToMyCalendarCommand.forwardReplyPropertyDefinitionArray))
					{
						CalendarActionResponse calendarActionResponse = new AddEventToMyCalendarCommandAction(base.MailboxIdentityMailboxSession, base.CallContext.AccessingPrincipal, base.CallContext.ClientCulture, rootXsoItem).Execute();
						if (!calendarActionResponse.WasSuccessful)
						{
							throw new ServiceInvalidOperationException((CoreResources.IDs)4004906780U);
						}
						return calendarActionResponse;
					}
				}
				catch (ObjectNotFoundException)
				{
					AddEventToMyCalendarCommand.TraceError(this.GetHashCode(), "AddEventToMyCalendarCommand.InternalExecute(): ObjectNotFoundException: forwardingItem is not found for the IdAndSession {0}", new object[]
					{
						idAndSession
					});
					throw new ServiceInvalidOperationException((CoreResources.IDs)4005418156U);
				}
			}
			AddEventToMyCalendarCommand.TraceError(this.GetHashCode(), "AddEventToMyCalendarCommand.InternalExecute(): The IdAndSession is null for ItemId: {0}", new object[]
			{
				this.Request.ItemId
			});
			throw new ServiceInvalidOperationException((CoreResources.IDs)4005418156U);
		}

		internal static void TraceDebug(int hashCode, string messageFormat, params object[] args)
		{
			ExTraceGlobals.AddEventToMyCalendarTracer.TraceDebug((long)hashCode, messageFormat, args);
		}

		internal static void TraceError(int hashCode, string messageFormat, params object[] args)
		{
			ExTraceGlobals.AddEventToMyCalendarTracer.TraceError((long)hashCode, messageFormat, args);
		}

		private readonly AddEventToMyCalendarRequest Request;

		private static readonly PropertyDefinition[] forwardReplyPropertyDefinitionArray = new PropertyDefinition[]
		{
			ItemSchema.SentTime,
			ItemSchema.IsClassified,
			ItemSchema.Classification,
			ItemSchema.ClassificationGuid,
			ItemSchema.ClassificationDescription,
			ItemSchema.ClassificationKeep
		};
	}
}
