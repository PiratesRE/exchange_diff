using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class OrganizationalUnitPickerService
	{
		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			DataView defaultView = dataTable.DefaultView;
			defaultView.Sort = "Identity asc";
			OrganizationalUnitPickerService.OUElement ouelement = OrganizationalUnitPickerService.BuildOUTree(defaultView.ToTable());
			dataTable.Rows.Clear();
			DataRow dataRow = dataTable.NewRow();
			dataRow["ID"] = ouelement.ID;
			dataRow["Name"] = ouelement.Name;
			dataRow["CanNewSubNode"] = ouelement.CanNewSubNode;
			dataRow["Children"] = ouelement.Children;
			dataTable.Rows.Add(dataRow);
			dataTable.EndLoadData();
		}

		private static OrganizationalUnitPickerService.OUElement BuildOUTree(DataTable dataTable)
		{
			Dictionary<string, OrganizationalUnitPickerService.OUElement> dictionary = new Dictionary<string, OrganizationalUnitPickerService.OUElement>();
			OrganizationalUnitPickerService.OUElement ouelement = new OrganizationalUnitPickerService.OUElement();
			ouelement.ID = (ouelement.Name = "root");
			ouelement.CanNewSubNode = false;
			dictionary.Add(ouelement.Name, ouelement);
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ADObjectId adobjectId = (ADObjectId)dataRow["Identity"];
				string text = adobjectId.ToString();
				int num = OrganizationalUnitPickerService.LastSplitCharPosition(text);
				string key = (num == -1) ? ouelement.Name : text.Substring(0, num);
				if (dictionary.ContainsKey(key))
				{
					OrganizationalUnitPickerService.OUElement ouelement2 = new OrganizationalUnitPickerService.OUElement();
					ouelement2.ID = adobjectId.ObjectGuid.ToString();
					ouelement2.CanNewSubNode = false;
					ouelement2.Name = text.Substring(num + 1);
					dictionary[key].Children.Add(ouelement2);
					dictionary.Add(text, ouelement2);
				}
			}
			return ouelement;
		}

		private static int LastSplitCharPosition(string canonicalName)
		{
			int result = -1;
			for (int i = 0; i < canonicalName.Length; i++)
			{
				char c = canonicalName[i];
				if (c == '\\')
				{
					i++;
				}
				else if (c == '/')
				{
					result = i;
				}
			}
			return result;
		}

		[DataContract]
		public class OUElement : NodeInfo
		{
			[DataMember]
			public List<object> Children
			{
				get
				{
					return this.children;
				}
				private set
				{
					throw new NotSupportedException();
				}
			}

			public override int GetHashCode()
			{
				return base.Name.GetHashCode() + base.ID.GetHashCode() + this.Children.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				OrganizationalUnitPickerService.OUElement ouelement = obj as OrganizationalUnitPickerService.OUElement;
				return ouelement != null && (string.Compare(base.Name, ouelement.Name) == 0 && string.Compare(base.ID, ouelement.ID) == 0 && base.CanNewSubNode == ouelement.CanNewSubNode) && this.Children.Count == ouelement.Children.Count;
			}

			private List<object> children = new List<object>();
		}
	}
}
