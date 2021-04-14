using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MailboxCreator : IDataObjectCreator
	{
		public object Create(DataTable table)
		{
			ADUser aduser = new ADUser();
			if (table != null)
			{
				DataColumn dataColumn = table.Columns["ResourceType"];
				if (dataColumn != null)
				{
					if (DBNull.Value.Equals(dataColumn.DefaultValue))
					{
						aduser.ResourceType = null;
					}
					else
					{
						aduser.ResourceType = new ExchangeResourceType?((ExchangeResourceType)dataColumn.DefaultValue);
					}
				}
			}
			return new Mailbox(aduser);
		}

		private const string ResourceType = "ResourceType";
	}
}
