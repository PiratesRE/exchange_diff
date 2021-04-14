using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.EntitySets.AttachmentCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReadAttachment : ReadEntityCommand<Attachments, IAttachment>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.ReadAttachmentTracer;
			}
		}

		protected override IAttachment OnExecute()
		{
			return this.Scope.AttachmentDataProvider.Read(base.EntityKey);
		}
	}
}
