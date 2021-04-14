using System;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class XmlSerializationReaderSetupComponentInfo : XmlSerializationReader
	{
		public object Read13_SetupComponentInfo()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_SetupComponentInfo || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read12_SetupComponentInfo(true, true);
			}
			else
			{
				base.UnknownNode(null, ":SetupComponentInfo");
			}
			return result;
		}

		private DatacenterMode Read14_DatacenterMode(string s)
		{
			if (s != null)
			{
				if (s == "Common")
				{
					return DatacenterMode.Common;
				}
				if (s == "Ffo")
				{
					return DatacenterMode.Ffo;
				}
				if (s == "ExO")
				{
					return DatacenterMode.ExO;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(DatacenterMode));
		}

		private SetupComponentInfo Read12_SetupComponentInfo(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id1_SetupComponentInfo || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			SetupComponentInfo setupComponentInfo = new SetupComponentInfo();
			if (setupComponentInfo.ServerTasks == null)
			{
				setupComponentInfo.ServerTasks = new ServerTaskInfoCollection();
			}
			ServerTaskInfoCollection serverTasks = setupComponentInfo.ServerTasks;
			if (setupComponentInfo.OrgTasks == null)
			{
				setupComponentInfo.OrgTasks = new OrgTaskInfoCollection();
			}
			OrgTaskInfoCollection orgTasks = setupComponentInfo.OrgTasks;
			if (setupComponentInfo.ServicePlanOrgTasks == null)
			{
				setupComponentInfo.ServicePlanOrgTasks = new ServicePlanTaskInfoCollection();
			}
			ServicePlanTaskInfoCollection servicePlanOrgTasks = setupComponentInfo.ServicePlanOrgTasks;
			TaskInfoCollection tasks = setupComponentInfo.Tasks;
			bool[] array = new bool[12];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id3_Name && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.Name = base.Reader.Value;
					array[0] = true;
				}
				else if (!array[1] && base.Reader.LocalName == this.id4_AlwaysExecute && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.AlwaysExecute = XmlConvert.ToBoolean(base.Reader.Value);
					array[1] = true;
				}
				else if (!array[2] && base.Reader.LocalName == this.id5_IsDatacenterOnly && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.IsDatacenterOnly = XmlConvert.ToBoolean(base.Reader.Value);
					array[2] = true;
				}
				else if (!array[3] && base.Reader.LocalName == this.id6_DescriptionId && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.DescriptionId = base.Reader.Value;
					array[3] = true;
				}
				else if (!array[4] && base.Reader.LocalName == this.id50_IsPartnerHostedOnly && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.IsPartnerHostedOnly = XmlConvert.ToBoolean(base.Reader.Value);
					array[4] = true;
				}
				else if (!array[5] && base.Reader.LocalName == this.id22_DatacenterMode && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.DatacenterMode = this.Read14_DatacenterMode(base.Reader.Value);
					array[5] = true;
				}
				else if (!array[6] && base.Reader.LocalName == this.id55_IsDatacenterDedicatedOnly && base.Reader.NamespaceURI == this.id2_Item)
				{
					setupComponentInfo.IsDatacenterDedicatedOnly = XmlConvert.ToBoolean(base.Reader.Value);
					array[6] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(setupComponentInfo, ":Name, :AlwaysExecute, :IsDatacenterOnly, :DescriptionId, :IsPartnerHostedOnly, :IsDatacenterDedicatedOnly");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return setupComponentInfo;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (base.Reader.LocalName == this.id7_ServerTasks && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							if (setupComponentInfo.ServerTasks == null)
							{
								setupComponentInfo.ServerTasks = new ServerTaskInfoCollection();
							}
							ServerTaskInfoCollection serverTasks2 = setupComponentInfo.ServerTasks;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num2 = 0;
								int readerCount2 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id8_ServerTaskInfo && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (serverTasks2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												serverTasks2.Add(this.Read7_ServerTaskInfo(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":ServerTaskInfo");
										}
									}
									else
									{
										base.UnknownNode(null, ":ServerTaskInfo");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num2, ref readerCount2);
								}
								base.ReadEndElement();
							}
						}
					}
					else if (base.Reader.LocalName == this.id9_OrgTasks && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							if (setupComponentInfo.OrgTasks == null)
							{
								setupComponentInfo.OrgTasks = new OrgTaskInfoCollection();
							}
							OrgTaskInfoCollection orgTasks2 = setupComponentInfo.OrgTasks;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num3 = 0;
								int readerCount3 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id10_OrgTaskInfo && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (orgTasks2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												orgTasks2.Add(this.Read10_OrgTaskInfo(true, true));
											}
										}
										else if (base.Reader.LocalName == this.id12_ServicePlanTaskInfo && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (orgTasks2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												orgTasks2.Add(this.Read11_ServicePlanTaskInfo(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":OrgTaskInfo");
										}
									}
									else
									{
										base.UnknownNode(null, ":OrgTaskInfo");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num3, ref readerCount3);
								}
								base.ReadEndElement();
							}
						}
					}
					else if (base.Reader.LocalName == this.id11_ServicePlanOrgTasks && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							if (setupComponentInfo.ServicePlanOrgTasks == null)
							{
								setupComponentInfo.ServicePlanOrgTasks = new ServicePlanTaskInfoCollection();
							}
							ServicePlanTaskInfoCollection servicePlanOrgTasks2 = setupComponentInfo.ServicePlanOrgTasks;
							if (base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num4 = 0;
								int readerCount4 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id12_ServicePlanTaskInfo && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (servicePlanOrgTasks2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												servicePlanOrgTasks2.Add(this.Read11_ServicePlanTaskInfo(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":ServicePlanTaskInfo");
										}
									}
									else
									{
										base.UnknownNode(null, ":ServicePlanTaskInfo");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num4, ref readerCount4);
								}
								base.ReadEndElement();
							}
						}
					}
					else if (base.Reader.LocalName == this.id13_Tasks && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (!base.ReadNull())
						{
							TaskInfoCollection tasks2 = setupComponentInfo.Tasks;
							if (tasks2 == null || base.Reader.IsEmptyElement)
							{
								base.Reader.Skip();
							}
							else
							{
								base.Reader.ReadStartElement();
								base.Reader.MoveToContent();
								int num5 = 0;
								int readerCount5 = base.ReaderCount;
								while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
								{
									if (base.Reader.NodeType == XmlNodeType.Element)
									{
										if (base.Reader.LocalName == this.id14_TaskInfo && base.Reader.NamespaceURI == this.id2_Item)
										{
											if (tasks2 == null)
											{
												base.Reader.Skip();
											}
											else
											{
												tasks2.Add(this.Read2_TaskInfo(true, true));
											}
										}
										else
										{
											base.UnknownNode(null, ":TaskInfo");
										}
									}
									else
									{
										base.UnknownNode(null, ":TaskInfo");
									}
									base.Reader.MoveToContent();
									base.CheckReaderCount(ref num5, ref readerCount5);
								}
								base.ReadEndElement();
							}
						}
					}
					else
					{
						base.UnknownNode(setupComponentInfo, ":ServerTasks, :OrgTasks, :ServicePlanOrgTasks, :Tasks");
					}
				}
				else
				{
					base.UnknownNode(setupComponentInfo, ":ServerTasks, :OrgTasks, :ServicePlanOrgTasks, :Tasks");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return setupComponentInfo;
		}

		private TaskInfo Read2_TaskInfo(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id14_TaskInfo || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id10_OrgTaskInfo && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read10_OrgTaskInfo(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id12_ServicePlanTaskInfo && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read11_ServicePlanTaskInfo(isNullable, false);
				}
				if (xmlQualifiedName.Name == this.id8_ServerTaskInfo && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read7_ServerTaskInfo(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				throw base.CreateAbstractTypeException("TaskInfo", "");
			}
		}

		private ServerTaskInfo Read7_ServerTaskInfo(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id8_ServerTaskInfo || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ServerTaskInfo serverTaskInfo = new ServerTaskInfo();
			bool[] array = new bool[7];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id40_Id && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfo.Id = base.Reader.Value;
					array[0] = true;
				}
				else if (!array[1] && base.Reader.LocalName == this.id41_Component && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfo.Component = base.Reader.Value;
					array[1] = true;
				}
				else if (!array[2] && base.Reader.LocalName == this.id56_ExcludeInDatacenterDedicated && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfo.ExcludeInDatacenterDedicated = XmlConvert.ToBoolean(base.Reader.Value);
					array[2] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(serverTaskInfo);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return serverTaskInfo;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[3] && base.Reader.LocalName == this.id15_Install && base.Reader.NamespaceURI == this.id2_Item)
					{
						serverTaskInfo.Install = this.Read6_ServerTaskInfoBlock(false, true);
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id16_BuildToBuildUpgrade && base.Reader.NamespaceURI == this.id2_Item)
					{
						serverTaskInfo.BuildToBuildUpgrade = this.Read6_ServerTaskInfoBlock(false, true);
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id17_DisasterRecovery && base.Reader.NamespaceURI == this.id2_Item)
					{
						serverTaskInfo.DisasterRecovery = this.Read6_ServerTaskInfoBlock(false, true);
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id18_Uninstall && base.Reader.NamespaceURI == this.id2_Item)
					{
						serverTaskInfo.Uninstall = this.Read6_ServerTaskInfoBlock(false, true);
						array[6] = true;
					}
					else
					{
						base.UnknownNode(serverTaskInfo, ":Install, :BuildToBuildUpgrade, :DisasterRecovery, :Uninstall");
					}
				}
				else
				{
					base.UnknownNode(serverTaskInfo, ":Install, :BuildToBuildUpgrade, :DisasterRecovery, :Uninstall");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return serverTaskInfo;
		}

		private ServerTaskInfoBlock Read6_ServerTaskInfoBlock(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id19_ServerTaskInfoBlock || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ServerTaskInfoBlock serverTaskInfoBlock = new ServerTaskInfoBlock();
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id6_DescriptionId && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfoBlock.DescriptionId = base.Reader.Value;
					array[0] = true;
				}
				else if (!array[1] && base.Reader.LocalName == this.id20_UseInstallTasks && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfoBlock.UseInstallTasks = XmlConvert.ToBoolean(base.Reader.Value);
					array[1] = true;
				}
				else if (!array[2] && base.Reader.LocalName == this.id21_Weight && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfoBlock.Weight = XmlConvert.ToInt32(base.Reader.Value);
					array[2] = true;
				}
				else if (!array[3] && base.Reader.LocalName == this.id22_IsFatal && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfoBlock.IsFatal = XmlConvert.ToBoolean(base.Reader.Value);
					array[3] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(serverTaskInfoBlock, ":DescriptionId, :UseInstallTasks, :Weight, :IsFatal");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return serverTaskInfoBlock;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[4] && base.Reader.LocalName == this.id23_Standalone && base.Reader.NamespaceURI == this.id2_Item)
					{
						serverTaskInfoBlock.Standalone = this.Read5_ServerTaskInfoEntry(false, true);
						array[4] = true;
					}
					else
					{
						base.UnknownNode(serverTaskInfoBlock, ":Standalone");
					}
				}
				else
				{
					base.UnknownNode(serverTaskInfoBlock, ":Standalone");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return serverTaskInfoBlock;
		}

		private ServerTaskInfoEntry Read5_ServerTaskInfoEntry(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id24_ServerTaskInfoEntry || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ServerTaskInfoEntry serverTaskInfoEntry = new ServerTaskInfoEntry();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[1] && base.Reader.LocalName == this.id25_UseStandaloneTask && base.Reader.NamespaceURI == this.id2_Item)
				{
					serverTaskInfoEntry.UseStandaloneTask = XmlConvert.ToBoolean(base.Reader.Value);
					array[1] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(serverTaskInfoEntry, ":UseStandaloneTask");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return serverTaskInfoEntry;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				string text = null;
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					base.UnknownNode(serverTaskInfoEntry, "");
				}
				else if (base.Reader.NodeType == XmlNodeType.Text || base.Reader.NodeType == XmlNodeType.CDATA || base.Reader.NodeType == XmlNodeType.Whitespace || base.Reader.NodeType == XmlNodeType.SignificantWhitespace)
				{
					text = base.ReadString(text, false);
					serverTaskInfoEntry.Task = text;
				}
				else
				{
					base.UnknownNode(serverTaskInfoEntry, "");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return serverTaskInfoEntry;
		}

		private ServicePlanTaskInfo Read11_ServicePlanTaskInfo(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id12_ServicePlanTaskInfo || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ServicePlanTaskInfo servicePlanTaskInfo = new ServicePlanTaskInfo();
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[3] && base.Reader.LocalName == this.id26_FeatureName && base.Reader.NamespaceURI == this.id2_Item)
				{
					servicePlanTaskInfo.FeatureName = base.Reader.Value;
					array[3] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(servicePlanTaskInfo, ":FeatureName");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return servicePlanTaskInfo;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id15_Install && base.Reader.NamespaceURI == this.id2_Item)
					{
						servicePlanTaskInfo.Install = this.Read9_OrgTaskInfoBlock(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id16_BuildToBuildUpgrade && base.Reader.NamespaceURI == this.id2_Item)
					{
						servicePlanTaskInfo.BuildToBuildUpgrade = this.Read9_OrgTaskInfoBlock(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id18_Uninstall && base.Reader.NamespaceURI == this.id2_Item)
					{
						servicePlanTaskInfo.Uninstall = this.Read9_OrgTaskInfoBlock(false, true);
						array[2] = true;
					}
					else
					{
						base.UnknownNode(servicePlanTaskInfo, ":Install, :BuildToBuildUpgrade, :Uninstall");
					}
				}
				else
				{
					base.UnknownNode(servicePlanTaskInfo, ":Install, :BuildToBuildUpgrade, :Uninstall");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return servicePlanTaskInfo;
		}

		private OrgTaskInfoBlock Read9_OrgTaskInfoBlock(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id27_OrgTaskInfoBlock || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			OrgTaskInfoBlock orgTaskInfoBlock = new OrgTaskInfoBlock();
			bool[] array = new bool[6];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id6_DescriptionId && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoBlock.DescriptionId = base.Reader.Value;
					array[0] = true;
				}
				else if (!array[1] && base.Reader.LocalName == this.id20_UseInstallTasks && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoBlock.UseInstallTasks = XmlConvert.ToBoolean(base.Reader.Value);
					array[1] = true;
				}
				else if (!array[2] && base.Reader.LocalName == this.id21_Weight && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoBlock.Weight = XmlConvert.ToInt32(base.Reader.Value);
					array[2] = true;
				}
				else if (!array[3] && base.Reader.LocalName == this.id22_IsFatal && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoBlock.IsFatal = XmlConvert.ToBoolean(base.Reader.Value);
					array[3] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(orgTaskInfoBlock, ":DescriptionId, :UseInstallTasks, :Weight, :IsFatal");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return orgTaskInfoBlock;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[4] && base.Reader.LocalName == this.id28_Global && base.Reader.NamespaceURI == this.id2_Item)
					{
						orgTaskInfoBlock.Global = this.Read8_OrgTaskInfoEntry(false, true);
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id29_Tenant && base.Reader.NamespaceURI == this.id2_Item)
					{
						orgTaskInfoBlock.Tenant = this.Read8_OrgTaskInfoEntry(false, true);
						array[5] = true;
					}
					else
					{
						base.UnknownNode(orgTaskInfoBlock, ":Global, :Tenant");
					}
				}
				else
				{
					base.UnknownNode(orgTaskInfoBlock, ":Global, :Tenant");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return orgTaskInfoBlock;
		}

		private OrgTaskInfoEntry Read8_OrgTaskInfoEntry(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id30_OrgTaskInfoEntry || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			OrgTaskInfoEntry orgTaskInfoEntry = new OrgTaskInfoEntry();
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[1] && base.Reader.LocalName == this.id31_UseGlobalTask && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoEntry.UseGlobalTask = XmlConvert.ToBoolean(base.Reader.Value);
					array[1] = true;
				}
				else if (!array[2] && base.Reader.LocalName == this.id53_UseForReconciliation && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoEntry.UseForReconciliation = XmlConvert.ToBoolean(base.Reader.Value);
					array[2] = true;
				}
				else if (!array[3] && base.Reader.LocalName == this.id54_RecipientOperation && base.Reader.NamespaceURI == this.id2_Item)
				{
					orgTaskInfoEntry.RecipientOperation = XmlConvert.ToBoolean(base.Reader.Value);
					array[3] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(orgTaskInfoEntry, ":UseGlobalTask, :UseForReconciliation, :RecipientOperation");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return orgTaskInfoEntry;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				string text = null;
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					base.UnknownNode(orgTaskInfoEntry, "");
				}
				else if (base.Reader.NodeType == XmlNodeType.Text || base.Reader.NodeType == XmlNodeType.CDATA || base.Reader.NodeType == XmlNodeType.Whitespace || base.Reader.NodeType == XmlNodeType.SignificantWhitespace)
				{
					text = base.ReadString(text, false);
					orgTaskInfoEntry.Task = text;
				}
				else
				{
					base.UnknownNode(orgTaskInfoEntry, "");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return orgTaskInfoEntry;
		}

		private OrgTaskInfo Read10_OrgTaskInfo(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id10_OrgTaskInfo || xmlQualifiedName.Namespace != this.id2_Item))
			{
				if (xmlQualifiedName.Name == this.id12_ServicePlanTaskInfo && xmlQualifiedName.Namespace == this.id2_Item)
				{
					return this.Read11_ServicePlanTaskInfo(isNullable, false);
				}
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			else
			{
				if (flag)
				{
					return null;
				}
				OrgTaskInfo orgTaskInfo = new OrgTaskInfo();
				bool[] array = new bool[6];
				while (base.Reader.MoveToNextAttribute())
				{
					if (!array[0] && base.Reader.LocalName == this.id40_Id && base.Reader.NamespaceURI == this.id2_Item)
					{
						orgTaskInfo.Id = base.Reader.Value;
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id41_Component && base.Reader.NamespaceURI == this.id2_Item)
					{
						orgTaskInfo.Component = base.Reader.Value;
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id56_ExcludeInDatacenterDedicated && base.Reader.NamespaceURI == this.id2_Item)
					{
						orgTaskInfo.ExcludeInDatacenterDedicated = XmlConvert.ToBoolean(base.Reader.Value);
						array[2] = true;
					}
					else if (!base.IsXmlnsAttribute(base.Reader.Name))
					{
						base.UnknownNode(orgTaskInfo);
					}
				}
				base.Reader.MoveToElement();
				if (base.Reader.IsEmptyElement)
				{
					base.Reader.Skip();
					return orgTaskInfo;
				}
				base.Reader.ReadStartElement();
				base.Reader.MoveToContent();
				int num = 0;
				int readerCount = base.ReaderCount;
				while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
				{
					if (base.Reader.NodeType == XmlNodeType.Element)
					{
						if (!array[3] && base.Reader.LocalName == this.id15_Install && base.Reader.NamespaceURI == this.id2_Item)
						{
							orgTaskInfo.Install = this.Read9_OrgTaskInfoBlock(false, true);
							array[3] = true;
						}
						else if (!array[4] && base.Reader.LocalName == this.id16_BuildToBuildUpgrade && base.Reader.NamespaceURI == this.id2_Item)
						{
							orgTaskInfo.BuildToBuildUpgrade = this.Read9_OrgTaskInfoBlock(false, true);
							array[4] = true;
						}
						else if (!array[5] && base.Reader.LocalName == this.id18_Uninstall && base.Reader.NamespaceURI == this.id2_Item)
						{
							orgTaskInfo.Uninstall = this.Read9_OrgTaskInfoBlock(false, true);
							array[5] = true;
						}
						else
						{
							base.UnknownNode(orgTaskInfo, ":Install, :BuildToBuildUpgrade, :Uninstall");
						}
					}
					else
					{
						base.UnknownNode(orgTaskInfo, ":Install, :BuildToBuildUpgrade, :Uninstall");
					}
					base.Reader.MoveToContent();
					base.CheckReaderCount(ref num, ref readerCount);
				}
				base.ReadEndElement();
				return orgTaskInfo;
			}
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id31_UseGlobalTask = base.Reader.NameTable.Add("UseGlobalTask");
			this.id53_UseForReconciliation = base.Reader.NameTable.Add("UseForReconciliation");
			this.id54_RecipientOperation = base.Reader.NameTable.Add("RecipientOperation");
			this.id6_DescriptionId = base.Reader.NameTable.Add("DescriptionId");
			this.id15_Install = base.Reader.NameTable.Add("Install");
			this.id11_ServicePlanOrgTasks = base.Reader.NameTable.Add("ServicePlanOrgTasks");
			this.id19_ServerTaskInfoBlock = base.Reader.NameTable.Add("ServerTaskInfoBlock");
			this.id21_Weight = base.Reader.NameTable.Add("Weight");
			this.id26_FeatureName = base.Reader.NameTable.Add("FeatureName");
			this.id7_ServerTasks = base.Reader.NameTable.Add("ServerTasks");
			this.id12_ServicePlanTaskInfo = base.Reader.NameTable.Add("ServicePlanTaskInfo");
			this.id10_OrgTaskInfo = base.Reader.NameTable.Add("OrgTaskInfo");
			this.id5_IsDatacenterOnly = base.Reader.NameTable.Add("IsDatacenterOnly");
			this.id50_IsPartnerHostedOnly = base.Reader.NameTable.Add("IsPartnerHostedOnly");
			this.id55_IsDatacenterDedicatedOnly = base.Reader.NameTable.Add("IsDatacenterDedicatedOnly");
			this.id56_ExcludeInDatacenterDedicated = base.Reader.NameTable.Add("ExcludeInDatacenterDedicated");
			this.id27_OrgTaskInfoBlock = base.Reader.NameTable.Add("OrgTaskInfoBlock");
			this.id9_OrgTasks = base.Reader.NameTable.Add("OrgTasks");
			this.id23_Standalone = base.Reader.NameTable.Add("Standalone");
			this.id30_OrgTaskInfoEntry = base.Reader.NameTable.Add("OrgTaskInfoEntry");
			this.id20_UseInstallTasks = base.Reader.NameTable.Add("UseInstallTasks");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id14_TaskInfo = base.Reader.NameTable.Add("TaskInfo");
			this.id24_ServerTaskInfoEntry = base.Reader.NameTable.Add("ServerTaskInfoEntry");
			this.id13_Tasks = base.Reader.NameTable.Add("Tasks");
			this.id3_Name = base.Reader.NameTable.Add("Name");
			this.id18_Uninstall = base.Reader.NameTable.Add("Uninstall");
			this.id1_SetupComponentInfo = base.Reader.NameTable.Add("SetupComponentInfo");
			this.id17_DisasterRecovery = base.Reader.NameTable.Add("DisasterRecovery");
			this.id28_Global = base.Reader.NameTable.Add("Global");
			this.id29_Tenant = base.Reader.NameTable.Add("Tenant");
			this.id8_ServerTaskInfo = base.Reader.NameTable.Add("ServerTaskInfo");
			this.id16_BuildToBuildUpgrade = base.Reader.NameTable.Add("BuildToBuildUpgrade");
			this.id25_UseStandaloneTask = base.Reader.NameTable.Add("UseStandaloneTask");
			this.id4_AlwaysExecute = base.Reader.NameTable.Add("AlwaysExecute");
			this.id22_IsFatal = base.Reader.NameTable.Add("IsFatal");
			this.id40_Id = base.Reader.NameTable.Add("Id");
			this.id41_Component = base.Reader.NameTable.Add("Component");
			this.id22_DatacenterMode = base.Reader.NameTable.Add("DatacenterMode");
		}

		private string id31_UseGlobalTask;

		private string id53_UseForReconciliation;

		private string id54_RecipientOperation;

		private string id6_DescriptionId;

		private string id15_Install;

		private string id11_ServicePlanOrgTasks;

		private string id19_ServerTaskInfoBlock;

		private string id21_Weight;

		private string id26_FeatureName;

		private string id7_ServerTasks;

		private string id12_ServicePlanTaskInfo;

		private string id10_OrgTaskInfo;

		private string id5_IsDatacenterOnly;

		private string id50_IsPartnerHostedOnly;

		private string id55_IsDatacenterDedicatedOnly;

		private string id56_ExcludeInDatacenterDedicated;

		private string id27_OrgTaskInfoBlock;

		private string id9_OrgTasks;

		private string id23_Standalone;

		private string id30_OrgTaskInfoEntry;

		private string id20_UseInstallTasks;

		private string id2_Item;

		private string id14_TaskInfo;

		private string id24_ServerTaskInfoEntry;

		private string id13_Tasks;

		private string id3_Name;

		private string id18_Uninstall;

		private string id1_SetupComponentInfo;

		private string id17_DisasterRecovery;

		private string id28_Global;

		private string id29_Tenant;

		private string id8_ServerTaskInfo;

		private string id16_BuildToBuildUpgrade;

		private string id25_UseStandaloneTask;

		private string id4_AlwaysExecute;

		private string id22_IsFatal;

		private string id40_Id;

		private string id41_Component;

		private string id22_DatacenterMode;
	}
}
