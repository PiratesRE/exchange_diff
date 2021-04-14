using System;
using System.Collections;
using System.Globalization;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBXmlSchema30 : WBXmlSchema
	{
		internal WBXmlSchema30() : base(30)
		{
			WBXmlSchema30.CodePage30[] array = new WBXmlSchema30.CodePage30[]
			{
				new WBXmlSchema30.CodePage30(EasStringArrays.AirSync, "AirSync", 0),
				new WBXmlSchema30.CodePage30(EasStringArrays.Contacts, "Contacts", 1),
				new WBXmlSchema30.CodePage30(EasStringArrays.Email, "Email", 2),
				new WBXmlSchema30.CodePage30(EasStringArrays.AirNotify, "AirNotify", 3),
				new WBXmlSchema30.CodePage30(EasStringArrays.Cal, "Calendar", 4),
				new WBXmlSchema30.CodePage30(EasStringArrays.Move, "Move", 5),
				new WBXmlSchema30.CodePage30(EasStringArrays.ItemEstimate, "GetItemEstimate", 6),
				new WBXmlSchema30.CodePage30(EasStringArrays.FolderHierarchy, "FolderHierarchy", 7),
				new WBXmlSchema30.CodePage30(EasStringArrays.MeetingResponse, "MeetingResponse", 8),
				new WBXmlSchema30.CodePage30(EasStringArrays.Tasks, "Tasks", 9),
				new WBXmlSchema30.CodePage30(EasStringArrays.ResolveRecipients, "ResolveRecipientsRequest", 10),
				new WBXmlSchema30.CodePage30(EasStringArrays.ValidateCert, "ValidateCertRequest", 11),
				new WBXmlSchema30.CodePage30(EasStringArrays.Contacts2, "Contacts2", 12),
				new WBXmlSchema30.CodePage30(EasStringArrays.Ping, "PingRequest", 13),
				new WBXmlSchema30.CodePage30(EasStringArrays.Provision, "ProvisionRequest", 14),
				new WBXmlSchema30.CodePage30(EasStringArrays.Search, "SearchRequest", 15),
				new WBXmlSchema30.CodePage30(EasStringArrays.Gal, "Gal", 16),
				new WBXmlSchema30.CodePage30(EasStringArrays.AirsyncBase, "AirSyncBase", 17),
				new WBXmlSchema30.CodePage30(EasStringArrays.Settings, "Settings", 18),
				new WBXmlSchema30.CodePage30(EasStringArrays.DocumentLibrary, "DocumentLibrary", 19),
				new WBXmlSchema30.CodePage30(EasStringArrays.ItemOperations, "ItemOperations", 20),
				new WBXmlSchema30.CodePage30(EasStringArrays.ComposeMail, "ComposeMail", 21),
				new WBXmlSchema30.CodePage30(EasStringArrays.Email2, "Email2", 22),
				new WBXmlSchema30.CodePage30(EasStringArrays.Notes, "Notes", 23),
				new WBXmlSchema30.CodePage30(EasStringArrays.RightsManagement, "RightsManagement", 24),
				new WBXmlSchema30.CodePage30(EasStringArrays.WindowsLive, "WindowsLive", 254)
			};
			this.codeSpace30 = new Hashtable();
			this.stringSpace30 = new Hashtable();
			foreach (WBXmlSchema30.CodePage30 codePage in array)
			{
				this.codeSpace30.Add(codePage.PageNumber, codePage);
				this.stringSpace30.Add(codePage.NameSpace, codePage);
			}
		}

		internal override string GetName(int tag)
		{
			int num = (tag & 65280) >> 8;
			string text = ((WBXmlSchema30.CodePage30)this.codeSpace30[num]).NameFromCode(tag);
			if (text == null || num != ((WBXmlSchema30.CodePage30)this.codeSpace30[num]).PageNumber)
			{
				throw new EasWBXmlTransientException("Invalid tag code: 0x" + tag.ToString("X", CultureInfo.InvariantCulture));
			}
			return text;
		}

		internal override string GetNameSpace(int tag)
		{
			int num = (tag & 65280) >> 8;
			string nameSpace = ((WBXmlSchema30.CodePage30)this.codeSpace30[num]).NameSpace;
			if (nameSpace == null)
			{
				throw new EasWBXmlTransientException("Invalid tag pagecode: 0x" + tag.ToString("X", CultureInfo.InvariantCulture));
			}
			return nameSpace;
		}

		internal override int GetTag(string nameSpace, string name)
		{
			if (nameSpace == null || name == null)
			{
				throw new EasWBXmlTransientException("nameSpace and name must both be non-null");
			}
			WBXmlSchema30.CodePage30 codePage = (WBXmlSchema30.CodePage30)this.stringSpace30[nameSpace];
			if (codePage == null)
			{
				throw new EasWBXmlTransientException("The namespace " + nameSpace + " is invalid in this schema");
			}
			int num = codePage.CodeFromName(name);
			if (num == 0)
			{
				throw new EasWBXmlTransientException(string.Concat(new string[]
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

		internal override bool IsTagSecure(int tag)
		{
			return tag == 3871 || tag == 5141;
		}

		internal override bool IsTagAnOpaqueBlob(int tag)
		{
			return tag == 5392 || tag == 5144 || tag == 3872 || tag == 5641 || tag == 5642;
		}

		private Hashtable codeSpace30;

		private Hashtable stringSpace30;

		protected class CodePage30
		{
			internal CodePage30(string[] tags, string nameSpace, int page)
			{
				this.NameSpace = nameSpace;
				this.PageNumber = page;
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

			internal string NameSpace { get; private set; }

			internal int PageNumber { get; private set; }

			internal int CodeFromName(string name)
			{
				object obj = this.stringTable[name];
				if (obj == null)
				{
					return 0;
				}
				return (int)obj;
			}

			internal string NameFromCode(int code)
			{
				int num = (code & 255) - 5;
				return this.codeTable[num];
			}

			private readonly string[] codeTable;

			private readonly Hashtable stringTable;
		}
	}
}
