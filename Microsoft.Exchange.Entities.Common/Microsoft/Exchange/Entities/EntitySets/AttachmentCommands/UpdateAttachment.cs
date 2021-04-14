using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.Commands;

namespace Microsoft.Exchange.Entities.EntitySets.AttachmentCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UpdateAttachment : UpdateEntityCommand<Attachments, IAttachment>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.UpdateAttachmentTracer;
			}
		}

		protected override IAttachment OnExecute()
		{
			throw new NotSupportedException(Strings.ErrorUnsupportedOperation("Update"));
		}

		protected override void SetETag(string eTag)
		{
		}
	}
}
