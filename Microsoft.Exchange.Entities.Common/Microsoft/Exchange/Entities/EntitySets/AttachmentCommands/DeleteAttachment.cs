using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.EntitySets.AttachmentCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeleteAttachment : DeleteEntityCommand<Attachments>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.DeleteAttachmentTracer;
			}
		}

		protected override VoidResult OnExecute()
		{
			this.Scope.AttachmentDataProvider.Delete(base.EntityKey, DeleteItemFlags.None);
			return VoidResult.Value;
		}
	}
}
