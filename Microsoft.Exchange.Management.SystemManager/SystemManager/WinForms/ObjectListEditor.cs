using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectListEditor : ListEditorBase<ADObjectId>
	{
		public ObjectListEditor()
		{
			base.Name = "ObjectListEditor";
		}

		protected override void InsertChangedObject(DataRow dataRow)
		{
			base.ChangedObjects.Add(dataRow["Identity"] as ADObjectId, (dataRow["ObjectClass"] ?? string.Empty).ToString());
		}
	}
}
