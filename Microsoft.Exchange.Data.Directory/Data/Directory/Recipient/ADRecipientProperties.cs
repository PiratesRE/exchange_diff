using System;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class ADRecipientProperties : ADPropertyUnionSchema
	{
		public static ADRecipientProperties Instance
		{
			get
			{
				if (ADRecipientProperties.instance == null)
				{
					ADRecipientProperties.instance = ObjectSchema.GetInstance<ADRecipientProperties>();
				}
				return ADRecipientProperties.instance;
			}
		}

		public override ReadOnlyCollection<ADObjectSchema> ObjectSchemas
		{
			get
			{
				return ADRecipientProperties.AllRecipientSchemas;
			}
		}

		private static readonly ReadOnlyCollection<ADObjectSchema> AllRecipientSchemas = new ReadOnlyCollection<ADObjectSchema>(new ADRecipientSchema[]
		{
			ObjectSchema.GetInstance<ADContactSchema>(),
			ObjectSchema.GetInstance<ADDynamicGroupSchema>(),
			ObjectSchema.GetInstance<ADGroupSchema>(),
			ObjectSchema.GetInstance<ADPublicDatabaseSchema>(),
			ObjectSchema.GetInstance<ADPublicFolderSchema>(),
			ObjectSchema.GetInstance<ADSystemAttendantMailboxSchema>(),
			ObjectSchema.GetInstance<ADSystemMailboxSchema>(),
			ObjectSchema.GetInstance<ADUserSchema>()
		});

		private static ADRecipientProperties instance;
	}
}
