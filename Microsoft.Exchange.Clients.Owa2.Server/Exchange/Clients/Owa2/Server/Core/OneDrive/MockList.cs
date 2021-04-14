using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockList : MockClientObject<List>, IList, IClientObject<List>
	{
		public MockList(MockClientContext context) : this(context, "Documents")
		{
		}

		public MockList(MockClientContext context, string title)
		{
			if (title == null)
			{
				throw new ArgumentNullException("title");
			}
			this.context = context;
			this.title = title;
		}

		public IFolder RootFolder
		{
			get
			{
				MockFolder result;
				if ((result = this.rootFolder) == null)
				{
					result = (this.rootFolder = new MockFolder(this.title, this.context));
				}
				return result;
			}
		}

		public override void LoadMockData()
		{
		}

		public IListItemCollection GetItems(CamlQuery query)
		{
			bool flag = false;
			bool flag2 = false;
			List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
			List<string> list2 = new List<string>();
			int rowLimit = -1;
			using (StringReader stringReader = new StringReader(query.ViewXml))
			{
				using (XmlReader xmlReader = XmlReader.Create(stringReader))
				{
					while (xmlReader.Read())
					{
						if (xmlReader.IsStartElement("OrderBy"))
						{
							flag = true;
							flag2 = false;
						}
						else if (xmlReader.IsStartElement("ViewFields"))
						{
							flag = false;
							flag2 = true;
						}
						else if (xmlReader.IsStartElement("FieldRef"))
						{
							if (flag)
							{
								string attribute = xmlReader.GetAttribute("Name");
								string attribute2 = xmlReader.GetAttribute("Ascending");
								bool value;
								if (!string.IsNullOrEmpty(attribute) && bool.TryParse(attribute2, out value))
								{
									list.Add(new KeyValuePair<string, bool>(attribute, value));
								}
							}
							else if (flag2)
							{
								string attribute3 = xmlReader.GetAttribute("Name");
								if (!string.IsNullOrEmpty(attribute3))
								{
									list2.Add(attribute3);
								}
							}
						}
						else if (xmlReader.IsStartElement("RowLimit"))
						{
							rowLimit = xmlReader.ReadElementContentAsInt();
						}
					}
				}
			}
			return new MockListItemCollection(this.context, this.title, query.FolderServerRelativeUrl, list, list2, rowLimit, query.ListItemCollectionPosition);
		}

		public IListItem GetItemById(string id)
		{
			if (new DirectoryInfo(id).Exists)
			{
				return new MockListItem(new DirectoryInfo(id), Path.GetDirectoryName(id), this.context);
			}
			if (new FileInfo(id).Exists)
			{
				return new MockListItem(new FileInfo(id), Path.GetDirectoryName(id), this.context);
			}
			throw new MockServerException();
		}

		private const string RootDocumentLibrary = "Documents";

		private const string OrderByElementName = "OrderBy";

		private const string FieldRefElementName = "FieldRef";

		private const string ViewFieldsElementName = "ViewFields";

		private const string RowLimitElementName = "RowLimit";

		private const string NameAttributeName = "Name";

		private const string AscendingAttributeName = "Ascending";

		private readonly string title;

		private MockClientContext context;

		private MockFolder rootFolder;
	}
}
