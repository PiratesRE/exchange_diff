using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public interface IItem : IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned
	{
		[NotMapped]
		List<IAttachment> Attachments { get; set; }

		ItemBody Body { get; set; }

		List<string> Categories { get; set; }

		ExDateTime DateTimeCreated { get; set; }

		bool HasAttachments { get; set; }

		Importance Importance { get; set; }

		ExDateTime LastModifiedTime { get; set; }

		string Preview { get; set; }

		ExDateTime ReceivedTime { get; set; }

		Sensitivity Sensitivity { get; set; }

		string Subject { get; set; }
	}
}
