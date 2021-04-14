using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class WbxmlSchema30 : WbxmlSchema
	{
		public WbxmlSchema30() : base(30)
		{
			WbxmlSchema30.CodePage30[] array = new WbxmlSchema30.CodePage30[]
			{
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.AirSync, "AirSync:", 0),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Contacts, "Contacts:", 1),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Email, "Email:", 2),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.AirNotify, "AirNotify:", 3),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Cal, "Calendar:", 4),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Move, "Move:", 5),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.ItemEstimate, "GetItemEstimate:", 6),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.FolderHierarchy, "FolderHierarchy:", 7),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.MeetingResponse, "MeetingResponse:", 8),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Tasks, "Tasks:", 9),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.ResolveRecipients, "ResolveRecipients:", 10),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.ValidateCert, "ValidateCert:", 11),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Contacts2, "Contacts2:", 12),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Ping, "Ping:", 13),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Provision, "Provision:", 14),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Search, "Search:", 15),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Gal, "Gal:", 16),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.AirsyncBase, "AirSyncBase:", 17),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Settings, "Settings:", 18),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.DocumentLibrary, "DocumentLibrary:", 19),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.ItemOperations, "ItemOperations:", 20),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.ComposeMail, "ComposeMail:", 21),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Email2, "Email2:", 22),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.Notes, "Notes:", 23),
				new WbxmlSchema30.CodePage30(AirSyncStringArrays.RightsManagement, "RightsManagement:", 24)
			};
			this.codeSpace30 = new Dictionary<int, WbxmlSchema30.CodePage30>();
			this.stringSpace30 = new Hashtable();
			for (int i = 0; i < array.Length; i++)
			{
				this.codeSpace30.Add(i, array[i]);
				this.stringSpace30.Add(array[i].NameSpace, array[i]);
			}
			WbxmlSchema30.CodePage30 value = new WbxmlSchema30.CodePage30(AirSyncStringArrays.WindowsLive, "WindowsLive:", 254);
			this.codeSpace30.Add(254, value);
			this.stringSpace30.Add("WindowsLive:", value);
		}

		public override string GetName(int tag)
		{
			int num = (tag & 65280) >> 8;
			string text = this.codeSpace30[num].NameFromCode(tag);
			if (text == null || num != this.codeSpace30[num].PageNumber)
			{
				throw new WbxmlException("Invalid tag code: 0x" + tag.ToString("X", CultureInfo.InvariantCulture));
			}
			return text;
		}

		public override string GetNameSpace(int tag)
		{
			int key = (tag & 65280) >> 8;
			string nameSpace = this.codeSpace30[key].NameSpace;
			if (nameSpace == null)
			{
				throw new WbxmlException("Invalid tag pagecode: 0x" + tag.ToString("X", CultureInfo.InvariantCulture));
			}
			return nameSpace;
		}

		public override int GetTag(string nameSpace, string name)
		{
			if (nameSpace == null || name == null)
			{
				throw new WbxmlException("nameSpace and name must both be non-null");
			}
			WbxmlSchema30.CodePage30 codePage = (WbxmlSchema30.CodePage30)this.stringSpace30[nameSpace];
			if (codePage == null)
			{
				throw new WbxmlException("The namespace " + nameSpace + " is invalid in this schema");
			}
			int num = codePage.CodeFromName(name);
			if (num == 0)
			{
				throw new WbxmlException(string.Concat(new string[]
				{
					"The name ",
					nameSpace,
					" ",
					name,
					" could not be found in schema"
				}));
			}
			return num;
		}

		public override bool IsTagSecure(int tag)
		{
			return tag == 3871 || tag == 5141;
		}

		public override bool IsTagAnOpaqueBlob(int tag)
		{
			return tag == 5392 || tag == 5144 || tag == 3872 || tag == 5641 || tag == 4383;
		}

		private const int WindowsLiveCodePageNumber = 254;

		private Dictionary<int, WbxmlSchema30.CodePage30> codeSpace30;

		private Hashtable stringSpace30;

		protected class CodePage30
		{
			public CodePage30(string[] tags, string nameSpace, int page)
			{
				this.nameSpace = nameSpace;
				this.pageNumber = page;
				this.stringTable = new Hashtable();
				this.codeTable = new string[tags.Length];
				int num = 5;
				foreach (string text in tags)
				{
					int num2 = page << 8;
					this.stringTable.Add(text, num | num2);
					this.codeTable[num - 5] = text;
					num++;
				}
			}

			public string NameSpace
			{
				get
				{
					return this.nameSpace;
				}
			}

			public int PageNumber
			{
				get
				{
					return this.pageNumber;
				}
			}

			public int CodeFromName(string name)
			{
				object obj = this.stringTable[name];
				if (obj == null)
				{
					return 0;
				}
				return (int)obj;
			}

			public string NameFromCode(int code)
			{
				int num = (code & 255) - 5;
				return this.codeTable[num];
			}

			private string[] codeTable;

			private string nameSpace;

			private int pageNumber;

			private Hashtable stringTable;
		}
	}
}
