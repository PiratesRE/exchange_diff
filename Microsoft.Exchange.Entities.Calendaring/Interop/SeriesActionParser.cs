using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities.Calendaring;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.Serialization;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal class SeriesActionParser : ISeriesActionParser
	{
		public SeriesActionParser(ICalendarInteropLog logger, IStoreSession session)
		{
			this.logger = logger;
			this.session = session;
		}

		private ITracer Trace
		{
			get
			{
				return ExTraceGlobals.SeriesActionParserTracer;
			}
		}

		public ICalendarInteropSeriesAction DeserializeCommand(ActionInfo action, Event contextEntity)
		{
			Exception deserializeException;
			try
			{
				if (action.CommandType == typeof(UpdateSeries).Name)
				{
					return EntitySerializer.Deserialize<UpdateSeries>(action.RawData);
				}
				if (action.CommandType == typeof(CancelSeries).Name)
				{
					return EntitySerializer.Deserialize<CancelSeries>(action.RawData);
				}
				if (action.CommandType == typeof(RespondToSeries).Name)
				{
					return EntitySerializer.Deserialize<RespondToSeries>(action.RawData);
				}
				if (action.CommandType == typeof(ForwardSeries).Name)
				{
					return EntitySerializer.Deserialize<ForwardSeries>(action.RawData);
				}
				if (action.CommandType == typeof(DeleteSeries).Name)
				{
					return EntitySerializer.Deserialize<DeleteSeries>(action.RawData);
				}
				return this.GetNoopActionForUnknownDataData(action, contextEntity);
			}
			catch (SerializationException ex)
			{
				deserializeException = ex;
			}
			catch (InvalidDataContractException ex2)
			{
				deserializeException = ex2;
			}
			catch (CorruptDataException ex3)
			{
				deserializeException = ex3;
			}
			return this.GetNoOpActionForCorruptedData(action, contextEntity, deserializeException);
		}

		private ICalendarInteropSeriesAction GetNoopActionForUnknownDataData(ActionInfo action, Event contextEntity)
		{
			this.Trace.TraceError<string, Guid>((long)this.GetHashCode(), "Unknown serialized command type: '{0}' for action '{1}'. Command will be replace with noop command to get it removed from the queue.", action.CommandType, action.Id);
			this.logger.LogEntry(this.session, "Unknown serialized command type: '{0}' for action '{1}'. Command will be replace with noop command to get it removed from the queue.", new object[]
			{
				action.CommandType,
				action.Id
			});
			return new NoOpSeriesRecoveryCommand(action.Id)
			{
				EntityKey = contextEntity.Id,
				Entity = contextEntity
			};
		}

		private ICalendarInteropSeriesAction GetNoOpActionForCorruptedData(ActionInfo action, Event contextEntity, Exception deserializeException)
		{
			this.Trace.TraceError<Guid, Exception>((long)this.GetHashCode(), "Error deserializing persisted command with id = {0}. Command will be replace with noop command to get it removed from the queue. Error is: {1}", action.Id, deserializeException);
			this.logger.LogEntry(this.session, deserializeException, true, "Error deserializing persisted command with id = {0}. Command will be replace with noop command to get it removed from the queue. Error is: {1}", new object[]
			{
				action.Id,
				deserializeException
			});
			return new NoOpSeriesRecoveryCommand(action.Id)
			{
				EntityKey = contextEntity.Id,
				Entity = contextEntity
			};
		}

		private readonly ICalendarInteropLog logger;

		private readonly IStoreSession session;
	}
}
