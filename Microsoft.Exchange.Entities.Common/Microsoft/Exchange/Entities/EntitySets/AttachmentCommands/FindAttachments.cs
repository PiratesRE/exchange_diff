using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.EntitySets.Commands;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods;

namespace Microsoft.Exchange.Entities.EntitySets.AttachmentCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FindAttachments : FindEntitiesCommand<Attachments, IAttachment>
	{
		protected override ITracer Trace
		{
			get
			{
				return ExTraceGlobals.FindAttachmentsTracer;
			}
		}

		protected override IEnumerable<IAttachment> OnExecute()
		{
			IEnumerable<IAttachment> allAttachments = this.Scope.AttachmentDataProvider.GetAllAttachments();
			return base.QueryOptions.ApplyTo(allAttachments.AsQueryable<IAttachment>());
		}
	}
}
