using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AcePermissionRecipientRow : BaseRow
	{
		public AcePermissionRecipientRow(Identity identity) : base(identity, null)
		{
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return base.Identity.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			AcePermissionRecipientRow acePermissionRecipientRow = obj as AcePermissionRecipientRow;
			return acePermissionRecipientRow != null && base.Identity.Equals(acePermissionRecipientRow.Identity);
		}

		public override int GetHashCode()
		{
			return base.Identity.GetHashCode();
		}

		public static PropertyDefinition[] Properties = new List<PropertyDefinition>
		{
			ADObjectSchema.Guid,
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.MasterAccountSid
		}.ToArray();
	}
}
