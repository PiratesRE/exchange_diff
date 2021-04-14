using System;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal class DocumentSchema : Schema
	{
		public new static DocumentSchema Instance
		{
			get
			{
				if (DocumentSchema.instance == null)
				{
					DocumentSchema.instance = new DocumentSchema();
				}
				return DocumentSchema.instance;
			}
		}

		public static readonly PropertyDefinition Identity = new SimplePropertyDefinition("Identity", typeof(IIdentity), PropertyFlag.None);

		public static readonly PropertyDefinition Operation = new SimplePropertyDefinition("Operation", typeof(DocumentOperation), PropertyFlag.None);

		public static readonly PropertyDefinition PipelineInstanceName = new SimplePropertyDefinition("PipelineInstanceName", typeof(string), PropertyFlag.None);

		public static readonly PropertyDefinition PipelineVersion = new SimplePropertyDefinition("PipelineVersion", typeof(Version), PropertyFlag.None);

		public static readonly PropertyDefinition MailboxId = new SimplePropertyDefinition("MailboxId", typeof(string), PropertyFlag.None);

		public static readonly PropertyDefinition ParentIdentity = new SimplePropertyDefinition("ParentIdentity", typeof(IIdentity), PropertyFlag.None);

		private static DocumentSchema instance = null;
	}
}
