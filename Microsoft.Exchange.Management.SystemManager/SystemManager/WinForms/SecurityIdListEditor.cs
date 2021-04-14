using System;
using System.Data;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SecurityIdListEditor : ListEditorBase<SecurityIdentifier>
	{
		public SecurityIdListEditor()
		{
			base.ObjectFilterProperty = IADSecurityPrincipalSchema.Sid;
			base.Name = "SecurityIdListEditor";
		}

		protected override void InsertChangedObject(DataRow dataRow)
		{
			base.ChangedObjects.Add(dataRow["Sid"] as SecurityIdentifier, (dataRow["UserFriendlyName"] ?? string.Empty).ToString());
		}
	}
}
