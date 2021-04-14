using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EdgeSync.Common
{
	internal class XmlSerializationReaderStatus : XmlSerializationReader
	{
		public object Read5_Status()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_Status || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read4_Status(true, true);
			}
			else
			{
				base.UnknownNode(null, ":Status");
			}
			return result;
		}

		private Status Read4_Status(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_Status || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Status status = new Status();
			bool[] array = new bool[11];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(status);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return status;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id3_Result && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Result = this.Read1_StatusResult(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id4_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Type = this.Read2_SyncTreeType(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id5_Name && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Name = base.Reader.ReadElementString();
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id6_FailureDetails && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.FailureDetails = base.Reader.ReadElementString();
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id7_StartUTC && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.StartUTC = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id8_EndUTC && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.EndUTC = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id9_Added && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Added = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[6] = true;
					}
					else if (!array[7] && base.Reader.LocalName == this.id10_Deleted && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Deleted = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[7] = true;
					}
					else if (!array[8] && base.Reader.LocalName == this.id11_Updated && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Updated = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[8] = true;
					}
					else if (!array[9] && base.Reader.LocalName == this.id12_Scanned && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.Scanned = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[9] = true;
					}
					else if (!array[10] && base.Reader.LocalName == this.id13_TargetScanned && base.Reader.NamespaceURI == this.id2_Item)
					{
						status.TargetScanned = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[10] = true;
					}
					else
					{
						base.UnknownNode(status, ":Result, :Type, :Name, :FailureDetails, :StartUTC, :EndUTC, :Added, :Deleted, :Updated, :Scanned, :TargetScanned");
					}
				}
				else
				{
					base.UnknownNode(status, ":Result, :Type, :Name, :FailureDetails, :StartUTC, :EndUTC, :Added, :Deleted, :Updated, :Scanned, :TargetScanned");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return status;
		}

		internal Hashtable SyncTreeTypeValues
		{
			get
			{
				if (this._SyncTreeTypeValues == null)
				{
					this._SyncTreeTypeValues = new Hashtable
					{
						{
							"Configuration",
							1L
						},
						{
							"Recipients",
							2L
						},
						{
							"General",
							4L
						}
					};
				}
				return this._SyncTreeTypeValues;
			}
		}

		private SyncTreeType Read2_SyncTreeType(string s)
		{
			return (SyncTreeType)XmlSerializationReader.ToEnum(s, this.SyncTreeTypeValues, "global::Microsoft.Exchange.EdgeSync.Common.SyncTreeType");
		}

		private StatusResult Read1_StatusResult(string s)
		{
			switch (s)
			{
			case "InProgress":
				return StatusResult.InProgress;
			case "Success":
				return StatusResult.Success;
			case "Aborted":
				return StatusResult.Aborted;
			case "CouldNotConnect":
				return StatusResult.CouldNotConnect;
			case "CouldNotLease":
				return StatusResult.CouldNotLease;
			case "LostLease":
				return StatusResult.LostLease;
			case "Incomplete":
				return StatusResult.Incomplete;
			case "NoSubscriptions":
				return StatusResult.NoSubscriptions;
			case "SyncDisabled":
				return StatusResult.SyncDisabled;
			}
			throw base.CreateUnknownConstantException(s, typeof(StatusResult));
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id7_StartUTC = base.Reader.NameTable.Add("StartUTC");
			this.id5_Name = base.Reader.NameTable.Add("Name");
			this.id1_Status = base.Reader.NameTable.Add("Status");
			this.id12_Scanned = base.Reader.NameTable.Add("Scanned");
			this.id6_FailureDetails = base.Reader.NameTable.Add("FailureDetails");
			this.id13_TargetScanned = base.Reader.NameTable.Add("TargetScanned");
			this.id9_Added = base.Reader.NameTable.Add("Added");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id8_EndUTC = base.Reader.NameTable.Add("EndUTC");
			this.id3_Result = base.Reader.NameTable.Add("Result");
			this.id10_Deleted = base.Reader.NameTable.Add("Deleted");
			this.id4_Type = base.Reader.NameTable.Add("Type");
			this.id11_Updated = base.Reader.NameTable.Add("Updated");
		}

		private Hashtable _SyncTreeTypeValues;

		private string id7_StartUTC;

		private string id5_Name;

		private string id1_Status;

		private string id12_Scanned;

		private string id6_FailureDetails;

		private string id13_TargetScanned;

		private string id9_Added;

		private string id2_Item;

		private string id8_EndUTC;

		private string id3_Result;

		private string id10_Deleted;

		private string id4_Type;

		private string id11_Updated;
	}
}
