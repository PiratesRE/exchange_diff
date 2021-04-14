using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.ProvisionRequest
{
	internal class XmlSerializationReaderProvision : XmlSerializationReader
	{
		public object Read5_Provision()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_Provision || base.Reader.NamespaceURI != this.id2_DeltaSyncV2)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read4_Provision(false, true);
			}
			else
			{
				base.UnknownNode(null, "DeltaSyncV2::Provision");
			}
			return result;
		}

		private Provision Read4_Provision(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id3_Item || xmlQualifiedName.Namespace != this.id2_DeltaSyncV2))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Provision provision = new Provision();
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(provision);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return provision;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id4_Add && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						if (!base.ReadNull())
						{
							AccountType[] array = null;
							int num2 = 0;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num3 = 0;
								int readerCount2 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id5_Account && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
										{
											array = (AccountType[])base.EnsureArrayIndex(array, num2, typeof(AccountType));
											array[num2++] = this.Read3_AccountType(false, true);
										}
										else
										{
											base.UnknownNode(null, "DeltaSyncV2::Account");
										}
									}
									else
									{
										base.UnknownNode(null, "DeltaSyncV2::Account");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num3, ref readerCount2);
								}
								base.ReadEndElement();
							}
							provision.Add = (AccountType[])base.ShrinkArray(array, num2, typeof(AccountType), false);
						}
					}
					else if (base.Reader.LocalName == this.id6_Delete && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						if (!base.ReadNull())
						{
							AccountType[] array2 = null;
							int num4 = 0;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num5 = 0;
								int readerCount3 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id5_Account && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
										{
											array2 = (AccountType[])base.EnsureArrayIndex(array2, num4, typeof(AccountType));
											array2[num4++] = this.Read3_AccountType(false, true);
										}
										else
										{
											base.UnknownNode(null, "DeltaSyncV2::Account");
										}
									}
									else
									{
										base.UnknownNode(null, "DeltaSyncV2::Account");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num5, ref readerCount3);
								}
								base.ReadEndElement();
							}
							provision.Delete = (AccountType[])base.ShrinkArray(array2, num4, typeof(AccountType), false);
						}
					}
					else if (base.Reader.LocalName == this.id7_Read && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						if (!base.ReadNull())
						{
							AccountType[] array3 = null;
							int num6 = 0;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num7 = 0;
								int readerCount4 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id5_Account && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
										{
											array3 = (AccountType[])base.EnsureArrayIndex(array3, num6, typeof(AccountType));
											array3[num6++] = this.Read3_AccountType(false, true);
										}
										else
										{
											base.UnknownNode(null, "DeltaSyncV2::Account");
										}
									}
									else
									{
										base.UnknownNode(null, "DeltaSyncV2::Account");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num7, ref readerCount4);
								}
								base.ReadEndElement();
							}
							provision.Read = (AccountType[])base.ShrinkArray(array3, num6, typeof(AccountType), false);
						}
					}
					else
					{
						base.UnknownNode(provision, "DeltaSyncV2::Add, DeltaSyncV2::Delete, DeltaSyncV2::Read");
					}
				}
				else
				{
					base.UnknownNode(provision, "DeltaSyncV2::Add, DeltaSyncV2::Delete, DeltaSyncV2::Read");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return provision;
		}

		private AccountType Read3_AccountType(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id8_AccountType || xmlQualifiedName.Namespace != this.id2_DeltaSyncV2))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			AccountType accountType = new AccountType();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(accountType);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return accountType;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_Name && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						accountType.Name = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id10_Type && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						accountType.Type = this.Read1_AccountTypeType(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id11_PartnerID && base.Reader.NamespaceURI == this.id2_DeltaSyncV2)
					{
						accountType.PartnerID = base.Reader.ReadElementString();
						array[2] = true;
					}
					else
					{
						base.UnknownNode(accountType, "DeltaSyncV2::Name, DeltaSyncV2::Type, DeltaSyncV2::PartnerID");
					}
				}
				else
				{
					base.UnknownNode(accountType, "DeltaSyncV2::Name, DeltaSyncV2::Type, DeltaSyncV2::PartnerID");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return accountType;
		}

		private AccountTypeType Read1_AccountTypeType(string s)
		{
			if (s != null && s == "MailRelay")
			{
				return AccountTypeType.MailRelay;
			}
			throw base.CreateUnknownConstantException(s, typeof(AccountTypeType));
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id9_Name = base.Reader.NameTable.Add("Name");
			this.id8_AccountType = base.Reader.NameTable.Add("AccountType");
			this.id1_Provision = base.Reader.NameTable.Add("Provision");
			this.id7_Read = base.Reader.NameTable.Add("Read");
			this.id3_Item = base.Reader.NameTable.Add("");
			this.id5_Account = base.Reader.NameTable.Add("Account");
			this.id6_Delete = base.Reader.NameTable.Add("Delete");
			this.id11_PartnerID = base.Reader.NameTable.Add("PartnerID");
			this.id2_DeltaSyncV2 = base.Reader.NameTable.Add("DeltaSyncV2:");
			this.id10_Type = base.Reader.NameTable.Add("Type");
			this.id4_Add = base.Reader.NameTable.Add("Add");
		}

		private string id9_Name;

		private string id8_AccountType;

		private string id1_Provision;

		private string id7_Read;

		private string id3_Item;

		private string id5_Account;

		private string id6_Delete;

		private string id11_PartnerID;

		private string id2_DeltaSyncV2;

		private string id10_Type;

		private string id4_Add;
	}
}
