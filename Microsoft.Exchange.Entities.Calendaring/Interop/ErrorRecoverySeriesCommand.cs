using System;
using Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.Entities.Calendaring.Interop
{
	internal abstract class ErrorRecoverySeriesCommand : UpdateSeries
	{
		protected ErrorRecoverySeriesCommand(Guid originalActionId)
		{
			this.originalId = originalActionId;
		}

		public sealed override Guid CommandId
		{
			get
			{
				return this.originalId;
			}
		}

		protected sealed override void PrepareDataForInstance(Event instanceDelta)
		{
			this.PrepareRecoveryData(instanceDelta);
		}

		protected virtual void PrepareRecoveryData(Event instanceDelta)
		{
			((IEventInternal)instanceDelta).SeriesToInstancePropagation = true;
		}

		private readonly Guid originalId;
	}
}
