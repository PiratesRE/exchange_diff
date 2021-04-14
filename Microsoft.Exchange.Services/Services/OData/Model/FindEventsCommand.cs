using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FindEventsCommand : EntityContainersCommand<FindEventsRequest, FindEventsResponse>
	{
		public FindEventsCommand(FindEventsRequest request) : base(request)
		{
		}

		protected override FindEventsResponse InternalExecute()
		{
			IEvents events = base.GetCalendarContainer(base.Request.CalendarId).Events;
			DataEntityQueryAdpater dataEntityQueryAdpater = new DataEntityQueryAdpater(EventSchema.SchemaInstance, base.Request.ODataQueryOptions);
			IEntityQueryOptions entityQueryOptions = dataEntityQueryAdpater.GetEntityQueryOptions();
			CommandContext context = base.CreateCommandContext(dataEntityQueryAdpater);
			ExDateTime value;
			ExDateTime value2;
			bool startEndQueryFilter = this.GetStartEndQueryFilter(dataEntityQueryAdpater.ODataQueryOptions.Filter, out value, out value2);
			IEnumerable<Event> source;
			if (startEndQueryFilter)
			{
				CalendarViewParameters parameters = new CalendarViewParameters(new ExDateTime?(value), new ExDateTime?(value2));
				IEnumerable<Event> calendarView = events.GetCalendarView(parameters, context);
				source = entityQueryOptions.ApplyTo(calendarView.AsQueryable<Event>());
			}
			else
			{
				source = events.Find(entityQueryOptions, context);
			}
			IEnumerable<Event> entities = (from x in source
			select GetEventCommand.DataEntityEventToEntity(x, base.Request.ODataQueryOptions, base.ExchangeService)).ToList<Event>();
			return new FindEventsResponse(base.Request)
			{
				Result = new FindEntitiesResult<Event>(entities, -1)
			};
		}

		private bool GetStartEndQueryFilter(FilterClause filter, out ExDateTime start, out ExDateTime end)
		{
			start = ExDateTime.UtcNow;
			end = ExDateTime.UtcNow;
			if (filter != null && filter.Expression.Kind == 4)
			{
				BinaryOperatorNode binaryOperatorNode = filter.Expression as BinaryOperatorNode;
				if (binaryOperatorNode != null && binaryOperatorNode.OperatorKind == 1 && binaryOperatorNode.Left.Kind == 4 && binaryOperatorNode.Right.Kind == 4)
				{
					BinaryOperatorNode binaryOperatorNode2 = binaryOperatorNode.Left as BinaryOperatorNode;
					BinaryOperatorNode binaryOperatorNode3 = binaryOperatorNode.Right as BinaryOperatorNode;
					if (binaryOperatorNode2 != null && binaryOperatorNode3 != null && binaryOperatorNode2.OperatorKind == 5 && binaryOperatorNode3.OperatorKind == 7 && binaryOperatorNode2.Left.Kind == 6 && binaryOperatorNode2.Right.Kind == 2 && binaryOperatorNode3.Left.Kind == 6 && binaryOperatorNode2.Right.Kind == 2)
					{
						SingleValuePropertyAccessNode singleValuePropertyAccessNode = binaryOperatorNode2.Left as SingleValuePropertyAccessNode;
						SingleValuePropertyAccessNode singleValuePropertyAccessNode2 = binaryOperatorNode3.Left as SingleValuePropertyAccessNode;
						ConvertNode convertNode = binaryOperatorNode2.Right as ConvertNode;
						ConvertNode convertNode2 = binaryOperatorNode3.Right as ConvertNode;
						if (singleValuePropertyAccessNode != null && singleValuePropertyAccessNode2 != null && singleValuePropertyAccessNode.Property.Name == "End" && singleValuePropertyAccessNode2.Property.Name == "Start" && convertNode2 != null && convertNode != null)
						{
							ConstantNode constantNode = convertNode.Source as ConstantNode;
							ConstantNode constantNode2 = convertNode2.Source as ConstantNode;
							if (constantNode != null && constantNode2 != null)
							{
								start = ExDateTime.Parse(constantNode.LiteralText);
								end = ExDateTime.Parse(constantNode2.LiteralText);
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}
