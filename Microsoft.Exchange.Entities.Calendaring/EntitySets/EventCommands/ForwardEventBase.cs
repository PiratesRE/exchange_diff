using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.Calendaring.EntitySets.EventCommands
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ForwardEventBase : KeyedEntityCommand<Events, VoidResult>
	{
		[DataMember]
		public ForwardEventParameters Parameters { get; set; }

		protected override void UpdateCustomLoggingData()
		{
			base.UpdateCustomLoggingData();
			this.SetCustomLoggingData("ForwardEventParameters", this.Parameters);
		}
	}
}
