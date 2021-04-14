using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.HostingData.System
{
	[HelpKeyword("vs.data.DataSet")]
	[XmlSchemaProvider("GetTypedDataSetSchema")]
	[DesignerCategory("code")]
	[XmlRoot("TextMessagingHostingData")]
	[ToolboxItem(true)]
	[Serializable]
	internal class TextMessagingHostingData : DataSet
	{
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public TextMessagingHostingData()
		{
			base.BeginInit();
			this.InitClass();
			CollectionChangeEventHandler value = new CollectionChangeEventHandler(this.SchemaChanged);
			base.Tables.CollectionChanged += value;
			base.Relations.CollectionChanged += value;
			base.EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected TextMessagingHostingData(SerializationInfo info, StreamingContext context) : base(info, context, false)
		{
			if (base.IsBinarySerialized(info, context))
			{
				this.InitVars(false);
				CollectionChangeEventHandler value = new CollectionChangeEventHandler(this.SchemaChanged);
				this.Tables.CollectionChanged += value;
				this.Relations.CollectionChanged += value;
				return;
			}
			string s = (string)info.GetValue("XmlSchema", typeof(string));
			if (base.DetermineSchemaSerializationMode(info, context) == SchemaSerializationMode.IncludeSchema)
			{
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
				if (dataSet.Tables["_locDefinition"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData._locDefinitionDataTable(dataSet.Tables["_locDefinition"]));
				}
				if (dataSet.Tables["Regions"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RegionsDataTable(dataSet.Tables["Regions"]));
				}
				if (dataSet.Tables["Region"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RegionDataTable(dataSet.Tables["Region"]));
				}
				if (dataSet.Tables["Carriers"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CarriersDataTable(dataSet.Tables["Carriers"]));
				}
				if (dataSet.Tables["Carrier"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CarrierDataTable(dataSet.Tables["Carrier"]));
				}
				if (dataSet.Tables["LocalizedInfo"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.LocalizedInfoDataTable(dataSet.Tables["LocalizedInfo"]));
				}
				if (dataSet.Tables["Services"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.ServicesDataTable(dataSet.Tables["Services"]));
				}
				if (dataSet.Tables["Service"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.ServiceDataTable(dataSet.Tables["Service"]));
				}
				if (dataSet.Tables["VoiceCallForwarding"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.VoiceCallForwardingDataTable(dataSet.Tables["VoiceCallForwarding"]));
				}
				if (dataSet.Tables["SmtpToSmsGateway"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.SmtpToSmsGatewayDataTable(dataSet.Tables["SmtpToSmsGateway"]));
				}
				if (dataSet.Tables["RecipientAddressing"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RecipientAddressingDataTable(dataSet.Tables["RecipientAddressing"]));
				}
				if (dataSet.Tables["MessageRendering"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.MessageRenderingDataTable(dataSet.Tables["MessageRendering"]));
				}
				if (dataSet.Tables["Capacity"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CapacityDataTable(dataSet.Tables["Capacity"]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				base.Merge(dataSet, false, MissingSchemaAction.Add);
				this.InitVars();
			}
			else
			{
				base.ReadXmlSchema(new XmlTextReader(new StringReader(s)));
			}
			base.GetSerializationData(info, context);
			CollectionChangeEventHandler value2 = new CollectionChangeEventHandler(this.SchemaChanged);
			base.Tables.CollectionChanged += value2;
			this.Relations.CollectionChanged += value2;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public TextMessagingHostingData._locDefinitionDataTable _locDefinition
		{
			get
			{
				return this.table_locDefinition;
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		public TextMessagingHostingData.RegionsDataTable Regions
		{
			get
			{
				return this.tableRegions;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public TextMessagingHostingData.RegionDataTable Region
		{
			get
			{
				return this.tableRegion;
			}
		}

		[Browsable(false)]
		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public TextMessagingHostingData.CarriersDataTable Carriers
		{
			get
			{
				return this.tableCarriers;
			}
		}

		[Browsable(false)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DebuggerNonUserCode]
		public TextMessagingHostingData.CarrierDataTable Carrier
		{
			get
			{
				return this.tableCarrier;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public TextMessagingHostingData.LocalizedInfoDataTable LocalizedInfo
		{
			get
			{
				return this.tableLocalizedInfo;
			}
		}

		[Browsable(false)]
		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public TextMessagingHostingData.ServicesDataTable Services
		{
			get
			{
				return this.tableServices;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public TextMessagingHostingData.ServiceDataTable Service
		{
			get
			{
				return this.tableService;
			}
		}

		[Browsable(false)]
		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public TextMessagingHostingData.VoiceCallForwardingDataTable VoiceCallForwarding
		{
			get
			{
				return this.tableVoiceCallForwarding;
			}
		}

		[DebuggerNonUserCode]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public TextMessagingHostingData.SmtpToSmsGatewayDataTable SmtpToSmsGateway
		{
			get
			{
				return this.tableSmtpToSmsGateway;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public TextMessagingHostingData.RecipientAddressingDataTable RecipientAddressing
		{
			get
			{
				return this.tableRecipientAddressing;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public TextMessagingHostingData.MessageRenderingDataTable MessageRendering
		{
			get
			{
				return this.tableMessageRendering;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[Browsable(false)]
		public TextMessagingHostingData.CapacityDataTable Capacity
		{
			get
			{
				return this.tableCapacity;
			}
		}

		[DebuggerNonUserCode]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public override SchemaSerializationMode SchemaSerializationMode
		{
			get
			{
				return this._schemaSerializationMode;
			}
			set
			{
				this._schemaSerializationMode = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public new DataTableCollection Tables
		{
			get
			{
				return base.Tables;
			}
		}

		[DebuggerNonUserCode]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public new DataRelationCollection Relations
		{
			get
			{
				return base.Relations;
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override void InitializeDerivedDataSet()
		{
			base.BeginInit();
			this.InitClass();
			base.EndInit();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public override DataSet Clone()
		{
			TextMessagingHostingData textMessagingHostingData = (TextMessagingHostingData)base.Clone();
			textMessagingHostingData.InitVars();
			textMessagingHostingData.SchemaSerializationMode = this.SchemaSerializationMode;
			return textMessagingHostingData;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override bool ShouldSerializeTables()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		protected override bool ShouldSerializeRelations()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override void ReadXmlSerializable(XmlReader reader)
		{
			if (base.DetermineSchemaSerializationMode(reader) == SchemaSerializationMode.IncludeSchema)
			{
				this.Reset();
				DataSet dataSet = new DataSet();
				dataSet.ReadXml(reader);
				if (dataSet.Tables["_locDefinition"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData._locDefinitionDataTable(dataSet.Tables["_locDefinition"]));
				}
				if (dataSet.Tables["Regions"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RegionsDataTable(dataSet.Tables["Regions"]));
				}
				if (dataSet.Tables["Region"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RegionDataTable(dataSet.Tables["Region"]));
				}
				if (dataSet.Tables["Carriers"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CarriersDataTable(dataSet.Tables["Carriers"]));
				}
				if (dataSet.Tables["Carrier"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CarrierDataTable(dataSet.Tables["Carrier"]));
				}
				if (dataSet.Tables["LocalizedInfo"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.LocalizedInfoDataTable(dataSet.Tables["LocalizedInfo"]));
				}
				if (dataSet.Tables["Services"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.ServicesDataTable(dataSet.Tables["Services"]));
				}
				if (dataSet.Tables["Service"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.ServiceDataTable(dataSet.Tables["Service"]));
				}
				if (dataSet.Tables["VoiceCallForwarding"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.VoiceCallForwardingDataTable(dataSet.Tables["VoiceCallForwarding"]));
				}
				if (dataSet.Tables["SmtpToSmsGateway"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.SmtpToSmsGatewayDataTable(dataSet.Tables["SmtpToSmsGateway"]));
				}
				if (dataSet.Tables["RecipientAddressing"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.RecipientAddressingDataTable(dataSet.Tables["RecipientAddressing"]));
				}
				if (dataSet.Tables["MessageRendering"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.MessageRenderingDataTable(dataSet.Tables["MessageRendering"]));
				}
				if (dataSet.Tables["Capacity"] != null)
				{
					base.Tables.Add(new TextMessagingHostingData.CapacityDataTable(dataSet.Tables["Capacity"]));
				}
				base.DataSetName = dataSet.DataSetName;
				base.Prefix = dataSet.Prefix;
				base.Namespace = dataSet.Namespace;
				base.Locale = dataSet.Locale;
				base.CaseSensitive = dataSet.CaseSensitive;
				base.EnforceConstraints = dataSet.EnforceConstraints;
				base.Merge(dataSet, false, MissingSchemaAction.Add);
				this.InitVars();
				return;
			}
			base.ReadXml(reader);
			this.InitVars();
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		protected override XmlSchema GetSchemaSerializable()
		{
			MemoryStream memoryStream = new MemoryStream();
			base.WriteXmlSchema(new XmlTextWriter(memoryStream, null));
			memoryStream.Position = 0L;
			return XmlSchema.Read(new XmlTextReader(memoryStream), null);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars()
		{
			this.InitVars(true);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		internal void InitVars(bool initTable)
		{
			this.table_locDefinition = (TextMessagingHostingData._locDefinitionDataTable)base.Tables["_locDefinition"];
			if (initTable && this.table_locDefinition != null)
			{
				this.table_locDefinition.InitVars();
			}
			this.tableRegions = (TextMessagingHostingData.RegionsDataTable)base.Tables["Regions"];
			if (initTable && this.tableRegions != null)
			{
				this.tableRegions.InitVars();
			}
			this.tableRegion = (TextMessagingHostingData.RegionDataTable)base.Tables["Region"];
			if (initTable && this.tableRegion != null)
			{
				this.tableRegion.InitVars();
			}
			this.tableCarriers = (TextMessagingHostingData.CarriersDataTable)base.Tables["Carriers"];
			if (initTable && this.tableCarriers != null)
			{
				this.tableCarriers.InitVars();
			}
			this.tableCarrier = (TextMessagingHostingData.CarrierDataTable)base.Tables["Carrier"];
			if (initTable && this.tableCarrier != null)
			{
				this.tableCarrier.InitVars();
			}
			this.tableLocalizedInfo = (TextMessagingHostingData.LocalizedInfoDataTable)base.Tables["LocalizedInfo"];
			if (initTable && this.tableLocalizedInfo != null)
			{
				this.tableLocalizedInfo.InitVars();
			}
			this.tableServices = (TextMessagingHostingData.ServicesDataTable)base.Tables["Services"];
			if (initTable && this.tableServices != null)
			{
				this.tableServices.InitVars();
			}
			this.tableService = (TextMessagingHostingData.ServiceDataTable)base.Tables["Service"];
			if (initTable && this.tableService != null)
			{
				this.tableService.InitVars();
			}
			this.tableVoiceCallForwarding = (TextMessagingHostingData.VoiceCallForwardingDataTable)base.Tables["VoiceCallForwarding"];
			if (initTable && this.tableVoiceCallForwarding != null)
			{
				this.tableVoiceCallForwarding.InitVars();
			}
			this.tableSmtpToSmsGateway = (TextMessagingHostingData.SmtpToSmsGatewayDataTable)base.Tables["SmtpToSmsGateway"];
			if (initTable && this.tableSmtpToSmsGateway != null)
			{
				this.tableSmtpToSmsGateway.InitVars();
			}
			this.tableRecipientAddressing = (TextMessagingHostingData.RecipientAddressingDataTable)base.Tables["RecipientAddressing"];
			if (initTable && this.tableRecipientAddressing != null)
			{
				this.tableRecipientAddressing.InitVars();
			}
			this.tableMessageRendering = (TextMessagingHostingData.MessageRenderingDataTable)base.Tables["MessageRendering"];
			if (initTable && this.tableMessageRendering != null)
			{
				this.tableMessageRendering.InitVars();
			}
			this.tableCapacity = (TextMessagingHostingData.CapacityDataTable)base.Tables["Capacity"];
			if (initTable && this.tableCapacity != null)
			{
				this.tableCapacity.InitVars();
			}
			this.relationRegions_Region = this.Relations["Regions_Region"];
			this.relationCarriers_Carrier = this.Relations["Carriers_Carrier"];
			this.relationCarrier_LocalizedInfo = this.Relations["Carrier_LocalizedInfo"];
			this.relationFK_Services_Service = this.Relations["FK_Services_Service"];
			this.relationFK_Carrier_Service = this.Relations["FK_Carrier_Service"];
			this.relationFK_Region_Service = this.Relations["FK_Region_Service"];
			this.relationFK_Service_VoiceCallForwarding = this.Relations["FK_Service_VoiceCallForwarding"];
			this.relationFK_Service_SmtpToSmsGateway = this.Relations["FK_Service_SmtpToSmsGateway"];
			this.relationFK_SmtpToSmsGateway_RecipientAddressing = this.Relations["FK_SmtpToSmsGateway_RecipientAddressing"];
			this.relationFK_SmtpToSmsGateway_MessageRendering = this.Relations["FK_SmtpToSmsGateway_MessageRendering"];
			this.relationFK_MessageRendering_Capacity = this.Relations["FK_MessageRendering_Capacity"];
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private void InitClass()
		{
			base.DataSetName = "TextMessagingHostingData";
			base.Prefix = "";
			base.Locale = new CultureInfo("");
			base.EnforceConstraints = true;
			this.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;
			this.table_locDefinition = new TextMessagingHostingData._locDefinitionDataTable();
			base.Tables.Add(this.table_locDefinition);
			this.tableRegions = new TextMessagingHostingData.RegionsDataTable();
			base.Tables.Add(this.tableRegions);
			this.tableRegion = new TextMessagingHostingData.RegionDataTable();
			base.Tables.Add(this.tableRegion);
			this.tableCarriers = new TextMessagingHostingData.CarriersDataTable();
			base.Tables.Add(this.tableCarriers);
			this.tableCarrier = new TextMessagingHostingData.CarrierDataTable();
			base.Tables.Add(this.tableCarrier);
			this.tableLocalizedInfo = new TextMessagingHostingData.LocalizedInfoDataTable();
			base.Tables.Add(this.tableLocalizedInfo);
			this.tableServices = new TextMessagingHostingData.ServicesDataTable();
			base.Tables.Add(this.tableServices);
			this.tableService = new TextMessagingHostingData.ServiceDataTable();
			base.Tables.Add(this.tableService);
			this.tableVoiceCallForwarding = new TextMessagingHostingData.VoiceCallForwardingDataTable();
			base.Tables.Add(this.tableVoiceCallForwarding);
			this.tableSmtpToSmsGateway = new TextMessagingHostingData.SmtpToSmsGatewayDataTable();
			base.Tables.Add(this.tableSmtpToSmsGateway);
			this.tableRecipientAddressing = new TextMessagingHostingData.RecipientAddressingDataTable();
			base.Tables.Add(this.tableRecipientAddressing);
			this.tableMessageRendering = new TextMessagingHostingData.MessageRenderingDataTable();
			base.Tables.Add(this.tableMessageRendering);
			this.tableCapacity = new TextMessagingHostingData.CapacityDataTable();
			base.Tables.Add(this.tableCapacity);
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint("Regions_Region", new DataColumn[]
			{
				this.tableRegions.Regions_IdColumn
			}, new DataColumn[]
			{
				this.tableRegion.Regions_IdColumn
			});
			this.tableRegion.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("Carriers_Carrier", new DataColumn[]
			{
				this.tableCarriers.Carriers_IdColumn
			}, new DataColumn[]
			{
				this.tableCarrier.Carriers_IdColumn
			});
			this.tableCarrier.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("Carrier_LocalizedInfo", new DataColumn[]
			{
				this.tableCarrier.IdentityColumn
			}, new DataColumn[]
			{
				this.tableLocalizedInfo.CarrierIdentityColumn
			});
			this.tableLocalizedInfo.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_Services_Service", new DataColumn[]
			{
				this.tableServices.Services_IdColumn
			}, new DataColumn[]
			{
				this.tableService.Services_IdColumn
			});
			this.tableService.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_Carrier_Service", new DataColumn[]
			{
				this.tableCarrier.IdentityColumn
			}, new DataColumn[]
			{
				this.tableService.CarrierIdentityColumn
			});
			this.tableService.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_Region_Service", new DataColumn[]
			{
				this.tableRegion.Iso2Column
			}, new DataColumn[]
			{
				this.tableService.RegionIso2Column
			});
			this.tableService.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_Service_VoiceCallForwarding", new DataColumn[]
			{
				this.tableService.RegionIso2Column,
				this.tableService.CarrierIdentityColumn,
				this.tableService.TypeColumn
			}, new DataColumn[]
			{
				this.tableVoiceCallForwarding.RegionIso2Column,
				this.tableVoiceCallForwarding.CarrierIdentityColumn,
				this.tableVoiceCallForwarding.ServiceTypeColumn
			});
			this.tableVoiceCallForwarding.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_Service_SmtpToSmsGateway", new DataColumn[]
			{
				this.tableService.RegionIso2Column,
				this.tableService.CarrierIdentityColumn,
				this.tableService.TypeColumn
			}, new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			});
			this.tableSmtpToSmsGateway.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_SmtpToSmsGateway_RecipientAddressing", new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableRecipientAddressing.RegionIso2Column,
				this.tableRecipientAddressing.CarrierIdentityColumn,
				this.tableRecipientAddressing.ServiceTypeColumn
			});
			this.tableRecipientAddressing.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_SmtpToSmsGateway_MessageRendering", new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableMessageRendering.RegionIso2Column,
				this.tableMessageRendering.CarrierIdentityColumn,
				this.tableMessageRendering.ServiceTypeColumn
			});
			this.tableMessageRendering.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			foreignKeyConstraint = new ForeignKeyConstraint("FK_MessageRendering_Capacity", new DataColumn[]
			{
				this.tableMessageRendering.RegionIso2Column,
				this.tableMessageRendering.CarrierIdentityColumn,
				this.tableMessageRendering.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableCapacity.RegionIso2Column,
				this.tableCapacity.CarrierIdentityColumn,
				this.tableCapacity.ServiceTypeColumn
			});
			this.tableCapacity.Constraints.Add(foreignKeyConstraint);
			foreignKeyConstraint.AcceptRejectRule = AcceptRejectRule.Cascade;
			foreignKeyConstraint.DeleteRule = Rule.Cascade;
			foreignKeyConstraint.UpdateRule = Rule.Cascade;
			this.relationRegions_Region = new DataRelation("Regions_Region", new DataColumn[]
			{
				this.tableRegions.Regions_IdColumn
			}, new DataColumn[]
			{
				this.tableRegion.Regions_IdColumn
			}, false);
			this.relationRegions_Region.Nested = true;
			this.Relations.Add(this.relationRegions_Region);
			this.relationCarriers_Carrier = new DataRelation("Carriers_Carrier", new DataColumn[]
			{
				this.tableCarriers.Carriers_IdColumn
			}, new DataColumn[]
			{
				this.tableCarrier.Carriers_IdColumn
			}, false);
			this.relationCarriers_Carrier.Nested = true;
			this.Relations.Add(this.relationCarriers_Carrier);
			this.relationCarrier_LocalizedInfo = new DataRelation("Carrier_LocalizedInfo", new DataColumn[]
			{
				this.tableCarrier.IdentityColumn
			}, new DataColumn[]
			{
				this.tableLocalizedInfo.CarrierIdentityColumn
			}, false);
			this.relationCarrier_LocalizedInfo.Nested = true;
			this.Relations.Add(this.relationCarrier_LocalizedInfo);
			this.relationFK_Services_Service = new DataRelation("FK_Services_Service", new DataColumn[]
			{
				this.tableServices.Services_IdColumn
			}, new DataColumn[]
			{
				this.tableService.Services_IdColumn
			}, false);
			this.relationFK_Services_Service.Nested = true;
			this.Relations.Add(this.relationFK_Services_Service);
			this.relationFK_Carrier_Service = new DataRelation("FK_Carrier_Service", new DataColumn[]
			{
				this.tableCarrier.IdentityColumn
			}, new DataColumn[]
			{
				this.tableService.CarrierIdentityColumn
			}, false);
			this.Relations.Add(this.relationFK_Carrier_Service);
			this.relationFK_Region_Service = new DataRelation("FK_Region_Service", new DataColumn[]
			{
				this.tableRegion.Iso2Column
			}, new DataColumn[]
			{
				this.tableService.RegionIso2Column
			}, false);
			this.Relations.Add(this.relationFK_Region_Service);
			this.relationFK_Service_VoiceCallForwarding = new DataRelation("FK_Service_VoiceCallForwarding", new DataColumn[]
			{
				this.tableService.RegionIso2Column,
				this.tableService.CarrierIdentityColumn,
				this.tableService.TypeColumn
			}, new DataColumn[]
			{
				this.tableVoiceCallForwarding.RegionIso2Column,
				this.tableVoiceCallForwarding.CarrierIdentityColumn,
				this.tableVoiceCallForwarding.ServiceTypeColumn
			}, false);
			this.relationFK_Service_VoiceCallForwarding.Nested = true;
			this.Relations.Add(this.relationFK_Service_VoiceCallForwarding);
			this.relationFK_Service_SmtpToSmsGateway = new DataRelation("FK_Service_SmtpToSmsGateway", new DataColumn[]
			{
				this.tableService.RegionIso2Column,
				this.tableService.CarrierIdentityColumn,
				this.tableService.TypeColumn
			}, new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			}, false);
			this.relationFK_Service_SmtpToSmsGateway.Nested = true;
			this.Relations.Add(this.relationFK_Service_SmtpToSmsGateway);
			this.relationFK_SmtpToSmsGateway_RecipientAddressing = new DataRelation("FK_SmtpToSmsGateway_RecipientAddressing", new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableRecipientAddressing.RegionIso2Column,
				this.tableRecipientAddressing.CarrierIdentityColumn,
				this.tableRecipientAddressing.ServiceTypeColumn
			}, false);
			this.relationFK_SmtpToSmsGateway_RecipientAddressing.Nested = true;
			this.Relations.Add(this.relationFK_SmtpToSmsGateway_RecipientAddressing);
			this.relationFK_SmtpToSmsGateway_MessageRendering = new DataRelation("FK_SmtpToSmsGateway_MessageRendering", new DataColumn[]
			{
				this.tableSmtpToSmsGateway.RegionIso2Column,
				this.tableSmtpToSmsGateway.CarrierIdentityColumn,
				this.tableSmtpToSmsGateway.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableMessageRendering.RegionIso2Column,
				this.tableMessageRendering.CarrierIdentityColumn,
				this.tableMessageRendering.ServiceTypeColumn
			}, false);
			this.relationFK_SmtpToSmsGateway_MessageRendering.Nested = true;
			this.Relations.Add(this.relationFK_SmtpToSmsGateway_MessageRendering);
			this.relationFK_MessageRendering_Capacity = new DataRelation("FK_MessageRendering_Capacity", new DataColumn[]
			{
				this.tableMessageRendering.RegionIso2Column,
				this.tableMessageRendering.CarrierIdentityColumn,
				this.tableMessageRendering.ServiceTypeColumn
			}, new DataColumn[]
			{
				this.tableCapacity.RegionIso2Column,
				this.tableCapacity.CarrierIdentityColumn,
				this.tableCapacity.ServiceTypeColumn
			}, false);
			this.relationFK_MessageRendering_Capacity.Nested = true;
			this.Relations.Add(this.relationFK_MessageRendering_Capacity);
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerialize_locDefinition()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeRegions()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeRegion()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeCarriers()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeCarrier()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeLocalizedInfo()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeServices()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeService()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeVoiceCallForwarding()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeSmtpToSmsGateway()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeRecipientAddressing()
		{
			return false;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		private bool ShouldSerializeMessageRendering()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private bool ShouldSerializeCapacity()
		{
			return false;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		private void SchemaChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				this.InitVars();
			}
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		[DebuggerNonUserCode]
		public static XmlSchemaComplexType GetTypedDataSetSchema(XmlSchemaSet xs)
		{
			TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.Namespace = textMessagingHostingData.Namespace;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
			if (xs.Contains(schemaSerializable.TargetNamespace))
			{
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				try
				{
					schemaSerializable.Write(memoryStream);
					foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
					{
						XmlSchema xmlSchema = (XmlSchema)obj;
						memoryStream2.SetLength(0L);
						xmlSchema.Write(memoryStream2);
						if (memoryStream.Length == memoryStream2.Length)
						{
							memoryStream.Position = 0L;
							memoryStream2.Position = 0L;
							while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
							{
							}
							if (memoryStream.Position == memoryStream.Length)
							{
								return xmlSchemaComplexType;
							}
						}
					}
				}
				finally
				{
					if (memoryStream != null)
					{
						memoryStream.Close();
					}
					if (memoryStream2 != null)
					{
						memoryStream2.Close();
					}
				}
			}
			xs.Add(schemaSerializable);
			return xmlSchemaComplexType;
		}

		private TextMessagingHostingData._locDefinitionDataTable table_locDefinition;

		private TextMessagingHostingData.RegionsDataTable tableRegions;

		private TextMessagingHostingData.RegionDataTable tableRegion;

		private TextMessagingHostingData.CarriersDataTable tableCarriers;

		private TextMessagingHostingData.CarrierDataTable tableCarrier;

		private TextMessagingHostingData.LocalizedInfoDataTable tableLocalizedInfo;

		private TextMessagingHostingData.ServicesDataTable tableServices;

		private TextMessagingHostingData.ServiceDataTable tableService;

		private TextMessagingHostingData.VoiceCallForwardingDataTable tableVoiceCallForwarding;

		private TextMessagingHostingData.SmtpToSmsGatewayDataTable tableSmtpToSmsGateway;

		private TextMessagingHostingData.RecipientAddressingDataTable tableRecipientAddressing;

		private TextMessagingHostingData.MessageRenderingDataTable tableMessageRendering;

		private TextMessagingHostingData.CapacityDataTable tableCapacity;

		private DataRelation relationRegions_Region;

		private DataRelation relationCarriers_Carrier;

		private DataRelation relationCarrier_LocalizedInfo;

		private DataRelation relationFK_Services_Service;

		private DataRelation relationFK_Carrier_Service;

		private DataRelation relationFK_Region_Service;

		private DataRelation relationFK_Service_VoiceCallForwarding;

		private DataRelation relationFK_Service_SmtpToSmsGateway;

		private DataRelation relationFK_SmtpToSmsGateway_RecipientAddressing;

		private DataRelation relationFK_SmtpToSmsGateway_MessageRendering;

		private DataRelation relationFK_MessageRendering_Capacity;

		private SchemaSerializationMode _schemaSerializationMode = SchemaSerializationMode.IncludeSchema;

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void _locDefinitionRowChangeEventHandler(object sender, TextMessagingHostingData._locDefinitionRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void RegionsRowChangeEventHandler(object sender, TextMessagingHostingData.RegionsRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void RegionRowChangeEventHandler(object sender, TextMessagingHostingData.RegionRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void CarriersRowChangeEventHandler(object sender, TextMessagingHostingData.CarriersRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void CarrierRowChangeEventHandler(object sender, TextMessagingHostingData.CarrierRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void LocalizedInfoRowChangeEventHandler(object sender, TextMessagingHostingData.LocalizedInfoRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void ServicesRowChangeEventHandler(object sender, TextMessagingHostingData.ServicesRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void ServiceRowChangeEventHandler(object sender, TextMessagingHostingData.ServiceRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void VoiceCallForwardingRowChangeEventHandler(object sender, TextMessagingHostingData.VoiceCallForwardingRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void SmtpToSmsGatewayRowChangeEventHandler(object sender, TextMessagingHostingData.SmtpToSmsGatewayRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void RecipientAddressingRowChangeEventHandler(object sender, TextMessagingHostingData.RecipientAddressingRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void MessageRenderingRowChangeEventHandler(object sender, TextMessagingHostingData.MessageRenderingRowChangeEvent e);

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public delegate void CapacityRowChangeEventHandler(object sender, TextMessagingHostingData.CapacityRowChangeEvent e);

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class _locDefinitionDataTable : TypedTableBase<TextMessagingHostingData._locDefinitionRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public _locDefinitionDataTable()
			{
				base.TableName = "_locDefinition";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal _locDefinitionDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected _locDefinitionDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn _locDefinition_IdColumn
			{
				get
				{
					return this.column_locDefinition_Id;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn _locDefault_locColumn
			{
				get
				{
					return this.column_locDefault_loc;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			[Browsable(false)]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData._locDefinitionRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData._locDefinitionRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData._locDefinitionRowChangeEventHandler _locDefinitionRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData._locDefinitionRowChangeEventHandler _locDefinitionRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData._locDefinitionRowChangeEventHandler _locDefinitionRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData._locDefinitionRowChangeEventHandler _locDefinitionRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void Add_locDefinitionRow(TextMessagingHostingData._locDefinitionRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData._locDefinitionRow Add_locDefinitionRow(string _locDefault_loc)
			{
				TextMessagingHostingData._locDefinitionRow locDefinitionRow = (TextMessagingHostingData._locDefinitionRow)base.NewRow();
				object[] itemArray = new object[]
				{
					null,
					_locDefault_loc
				};
				locDefinitionRow.ItemArray = itemArray;
				base.Rows.Add(locDefinitionRow);
				return locDefinitionRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData._locDefinitionDataTable locDefinitionDataTable = (TextMessagingHostingData._locDefinitionDataTable)base.Clone();
				locDefinitionDataTable.InitVars();
				return locDefinitionDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData._locDefinitionDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.column_locDefinition_Id = base.Columns["_locDefinition_Id"];
				this.column_locDefault_loc = base.Columns["_locDefault_loc"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.column_locDefinition_Id = new DataColumn("_locDefinition_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.column_locDefinition_Id);
				this.column_locDefault_loc = new DataColumn("_locDefault_loc", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.column_locDefault_loc);
				base.Constraints.Add(new UniqueConstraint("_locDefinitionConstraint1", new DataColumn[]
				{
					this.column_locDefinition_Id
				}, true));
				this.column_locDefinition_Id.AutoIncrement = true;
				this.column_locDefinition_Id.AllowDBNull = false;
				this.column_locDefinition_Id.Unique = true;
				this.column_locDefinition_Id.Namespace = "";
				this.column_locDefault_loc.AllowDBNull = false;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData._locDefinitionRow New_locDefinitionRow()
			{
				return (TextMessagingHostingData._locDefinitionRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData._locDefinitionRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData._locDefinitionRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this._locDefinitionRowChanged != null)
				{
					this._locDefinitionRowChanged(this, new TextMessagingHostingData._locDefinitionRowChangeEvent((TextMessagingHostingData._locDefinitionRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this._locDefinitionRowChanging != null)
				{
					this._locDefinitionRowChanging(this, new TextMessagingHostingData._locDefinitionRowChangeEvent((TextMessagingHostingData._locDefinitionRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this._locDefinitionRowDeleted != null)
				{
					this._locDefinitionRowDeleted(this, new TextMessagingHostingData._locDefinitionRowChangeEvent((TextMessagingHostingData._locDefinitionRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this._locDefinitionRowDeleting != null)
				{
					this._locDefinitionRowDeleting(this, new TextMessagingHostingData._locDefinitionRowChangeEvent((TextMessagingHostingData._locDefinitionRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void Remove_locDefinitionRow(TextMessagingHostingData._locDefinitionRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "_locDefinitionDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn column_locDefinition_Id;

			private DataColumn column_locDefault_loc;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class RegionsDataTable : TypedTableBase<TextMessagingHostingData.RegionsRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public RegionsDataTable()
			{
				base.TableName = "Regions";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal RegionsDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected RegionsDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn Regions_IdColumn
			{
				get
				{
					return this.columnRegions_Id;
				}
			}

			[DebuggerNonUserCode]
			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionsRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.RegionsRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionsRowChangeEventHandler RegionsRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionsRowChangeEventHandler RegionsRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionsRowChangeEventHandler RegionsRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionsRowChangeEventHandler RegionsRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddRegionsRow(TextMessagingHostingData.RegionsRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionsRow AddRegionsRow()
			{
				TextMessagingHostingData.RegionsRow regionsRow = (TextMessagingHostingData.RegionsRow)base.NewRow();
				object[] array = new object[1];
				object[] itemArray = array;
				regionsRow.ItemArray = itemArray;
				base.Rows.Add(regionsRow);
				return regionsRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.RegionsDataTable regionsDataTable = (TextMessagingHostingData.RegionsDataTable)base.Clone();
				regionsDataTable.InitVars();
				return regionsDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.RegionsDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnRegions_Id = base.Columns["Regions_Id"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnRegions_Id = new DataColumn("Regions_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegions_Id);
				base.Constraints.Add(new UniqueConstraint("Constraint1", new DataColumn[]
				{
					this.columnRegions_Id
				}, true));
				this.columnRegions_Id.AutoIncrement = true;
				this.columnRegions_Id.AllowDBNull = false;
				this.columnRegions_Id.Unique = true;
				this.columnRegions_Id.Namespace = "";
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionsRow NewRegionsRow()
			{
				return (TextMessagingHostingData.RegionsRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.RegionsRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.RegionsRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.RegionsRowChanged != null)
				{
					this.RegionsRowChanged(this, new TextMessagingHostingData.RegionsRowChangeEvent((TextMessagingHostingData.RegionsRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.RegionsRowChanging != null)
				{
					this.RegionsRowChanging(this, new TextMessagingHostingData.RegionsRowChangeEvent((TextMessagingHostingData.RegionsRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.RegionsRowDeleted != null)
				{
					this.RegionsRowDeleted(this, new TextMessagingHostingData.RegionsRowChangeEvent((TextMessagingHostingData.RegionsRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.RegionsRowDeleting != null)
				{
					this.RegionsRowDeleting(this, new TextMessagingHostingData.RegionsRowChangeEvent((TextMessagingHostingData.RegionsRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveRegionsRow(TextMessagingHostingData.RegionsRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "RegionsDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnRegions_Id;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class RegionDataTable : TypedTableBase<TextMessagingHostingData.RegionRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public RegionDataTable()
			{
				base.TableName = "Region";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal RegionDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected RegionDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CountryCodeColumn
			{
				get
				{
					return this.columnCountryCode;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn PhoneNumberExampleColumn
			{
				get
				{
					return this.columnPhoneNumberExample;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn Iso2Column
			{
				get
				{
					return this.columnIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn Regions_IdColumn
			{
				get
				{
					return this.columnRegions_Id;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[Browsable(false)]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RegionRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.RegionRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionRowChangeEventHandler RegionRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionRowChangeEventHandler RegionRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionRowChangeEventHandler RegionRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RegionRowChangeEventHandler RegionRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddRegionRow(TextMessagingHostingData.RegionRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionRow AddRegionRow(string CountryCode, string PhoneNumberExample, string Iso2, TextMessagingHostingData.RegionsRow parentRegionsRowByRegions_Region)
			{
				TextMessagingHostingData.RegionRow regionRow = (TextMessagingHostingData.RegionRow)base.NewRow();
				object[] array = new object[4];
				array[0] = CountryCode;
				array[1] = PhoneNumberExample;
				array[2] = Iso2;
				object[] array2 = array;
				if (parentRegionsRowByRegions_Region != null)
				{
					array2[3] = parentRegionsRowByRegions_Region[0];
				}
				regionRow.ItemArray = array2;
				base.Rows.Add(regionRow);
				return regionRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RegionRow FindByIso2(string Iso2)
			{
				return (TextMessagingHostingData.RegionRow)base.Rows.Find(new object[]
				{
					Iso2
				});
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.RegionDataTable regionDataTable = (TextMessagingHostingData.RegionDataTable)base.Clone();
				regionDataTable.InitVars();
				return regionDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.RegionDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnCountryCode = base.Columns["CountryCode"];
				this.columnPhoneNumberExample = base.Columns["PhoneNumberExample"];
				this.columnIso2 = base.Columns["Iso2"];
				this.columnRegions_Id = base.Columns["Regions_Id"];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnCountryCode = new DataColumn("CountryCode", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnCountryCode);
				this.columnPhoneNumberExample = new DataColumn("PhoneNumberExample", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnPhoneNumberExample);
				this.columnIso2 = new DataColumn("Iso2", typeof(string), null, MappingType.Attribute);
				base.Columns.Add(this.columnIso2);
				this.columnRegions_Id = new DataColumn("Regions_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegions_Id);
				base.Constraints.Add(new UniqueConstraint("RegionKey1", new DataColumn[]
				{
					this.columnIso2
				}, true));
				this.columnCountryCode.AllowDBNull = false;
				this.columnIso2.AllowDBNull = false;
				this.columnIso2.Unique = true;
				this.columnIso2.Namespace = "";
				this.columnRegions_Id.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RegionRow NewRegionRow()
			{
				return (TextMessagingHostingData.RegionRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.RegionRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.RegionRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.RegionRowChanged != null)
				{
					this.RegionRowChanged(this, new TextMessagingHostingData.RegionRowChangeEvent((TextMessagingHostingData.RegionRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.RegionRowChanging != null)
				{
					this.RegionRowChanging(this, new TextMessagingHostingData.RegionRowChangeEvent((TextMessagingHostingData.RegionRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.RegionRowDeleted != null)
				{
					this.RegionRowDeleted(this, new TextMessagingHostingData.RegionRowChangeEvent((TextMessagingHostingData.RegionRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.RegionRowDeleting != null)
				{
					this.RegionRowDeleting(this, new TextMessagingHostingData.RegionRowChangeEvent((TextMessagingHostingData.RegionRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveRegionRow(TextMessagingHostingData.RegionRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "RegionDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnCountryCode;

			private DataColumn columnPhoneNumberExample;

			private DataColumn columnIso2;

			private DataColumn columnRegions_Id;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class CarriersDataTable : TypedTableBase<TextMessagingHostingData.CarriersRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public CarriersDataTable()
			{
				base.TableName = "Carriers";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal CarriersDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected CarriersDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn Carriers_IdColumn
			{
				get
				{
					return this.columnCarriers_Id;
				}
			}

			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarriersRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.CarriersRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarriersRowChangeEventHandler CarriersRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarriersRowChangeEventHandler CarriersRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarriersRowChangeEventHandler CarriersRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarriersRowChangeEventHandler CarriersRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void AddCarriersRow(TextMessagingHostingData.CarriersRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarriersRow AddCarriersRow()
			{
				TextMessagingHostingData.CarriersRow carriersRow = (TextMessagingHostingData.CarriersRow)base.NewRow();
				object[] array = new object[1];
				object[] itemArray = array;
				carriersRow.ItemArray = itemArray;
				base.Rows.Add(carriersRow);
				return carriersRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.CarriersDataTable carriersDataTable = (TextMessagingHostingData.CarriersDataTable)base.Clone();
				carriersDataTable.InitVars();
				return carriersDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.CarriersDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal void InitVars()
			{
				this.columnCarriers_Id = base.Columns["Carriers_Id"];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnCarriers_Id = new DataColumn("Carriers_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarriers_Id);
				base.Constraints.Add(new UniqueConstraint("Constraint1", new DataColumn[]
				{
					this.columnCarriers_Id
				}, true));
				this.columnCarriers_Id.AutoIncrement = true;
				this.columnCarriers_Id.AllowDBNull = false;
				this.columnCarriers_Id.Unique = true;
				this.columnCarriers_Id.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarriersRow NewCarriersRow()
			{
				return (TextMessagingHostingData.CarriersRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.CarriersRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.CarriersRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.CarriersRowChanged != null)
				{
					this.CarriersRowChanged(this, new TextMessagingHostingData.CarriersRowChangeEvent((TextMessagingHostingData.CarriersRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.CarriersRowChanging != null)
				{
					this.CarriersRowChanging(this, new TextMessagingHostingData.CarriersRowChangeEvent((TextMessagingHostingData.CarriersRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.CarriersRowDeleted != null)
				{
					this.CarriersRowDeleted(this, new TextMessagingHostingData.CarriersRowChangeEvent((TextMessagingHostingData.CarriersRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.CarriersRowDeleting != null)
				{
					this.CarriersRowDeleting(this, new TextMessagingHostingData.CarriersRowChangeEvent((TextMessagingHostingData.CarriersRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveCarriersRow(TextMessagingHostingData.CarriersRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "CarriersDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnCarriers_Id;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class CarrierDataTable : TypedTableBase<TextMessagingHostingData.CarrierRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public CarrierDataTable()
			{
				base.TableName = "Carrier";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal CarrierDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected CarrierDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn IdentityColumn
			{
				get
				{
					return this.columnIdentity;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn Carriers_IdColumn
			{
				get
				{
					return this.columnCarriers_Id;
				}
			}

			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarrierRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.CarrierRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarrierRowChangeEventHandler CarrierRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarrierRowChangeEventHandler CarrierRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarrierRowChangeEventHandler CarrierRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CarrierRowChangeEventHandler CarrierRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void AddCarrierRow(TextMessagingHostingData.CarrierRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarrierRow AddCarrierRow(int Identity, TextMessagingHostingData.CarriersRow parentCarriersRowByCarriers_Carrier)
			{
				TextMessagingHostingData.CarrierRow carrierRow = (TextMessagingHostingData.CarrierRow)base.NewRow();
				object[] array = new object[2];
				array[0] = Identity;
				object[] array2 = array;
				if (parentCarriersRowByCarriers_Carrier != null)
				{
					array2[1] = parentCarriersRowByCarriers_Carrier[0];
				}
				carrierRow.ItemArray = array2;
				base.Rows.Add(carrierRow);
				return carrierRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarrierRow FindByIdentity(int Identity)
			{
				return (TextMessagingHostingData.CarrierRow)base.Rows.Find(new object[]
				{
					Identity
				});
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.CarrierDataTable carrierDataTable = (TextMessagingHostingData.CarrierDataTable)base.Clone();
				carrierDataTable.InitVars();
				return carrierDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.CarrierDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnIdentity = base.Columns["Identity"];
				this.columnCarriers_Id = base.Columns["Carriers_Id"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnIdentity = new DataColumn("Identity", typeof(int), null, MappingType.Attribute);
				base.Columns.Add(this.columnIdentity);
				this.columnCarriers_Id = new DataColumn("Carriers_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarriers_Id);
				base.Constraints.Add(new UniqueConstraint("CarrierKey1", new DataColumn[]
				{
					this.columnIdentity
				}, true));
				this.columnIdentity.AllowDBNull = false;
				this.columnIdentity.Unique = true;
				this.columnIdentity.Namespace = "";
				this.columnCarriers_Id.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarrierRow NewCarrierRow()
			{
				return (TextMessagingHostingData.CarrierRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.CarrierRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.CarrierRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.CarrierRowChanged != null)
				{
					this.CarrierRowChanged(this, new TextMessagingHostingData.CarrierRowChangeEvent((TextMessagingHostingData.CarrierRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.CarrierRowChanging != null)
				{
					this.CarrierRowChanging(this, new TextMessagingHostingData.CarrierRowChangeEvent((TextMessagingHostingData.CarrierRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.CarrierRowDeleted != null)
				{
					this.CarrierRowDeleted(this, new TextMessagingHostingData.CarrierRowChangeEvent((TextMessagingHostingData.CarrierRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.CarrierRowDeleting != null)
				{
					this.CarrierRowDeleting(this, new TextMessagingHostingData.CarrierRowChangeEvent((TextMessagingHostingData.CarrierRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveCarrierRow(TextMessagingHostingData.CarrierRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "CarrierDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnIdentity;

			private DataColumn columnCarriers_Id;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class LocalizedInfoDataTable : TypedTableBase<TextMessagingHostingData.LocalizedInfoRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public LocalizedInfoDataTable()
			{
				base.TableName = "LocalizedInfo";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal LocalizedInfoDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected LocalizedInfoDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn DisplayNameColumn
			{
				get
				{
					return this.columnDisplayName;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CultureColumn
			{
				get
				{
					return this.columnCulture;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[Browsable(false)]
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.LocalizedInfoRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.LocalizedInfoRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.LocalizedInfoRowChangeEventHandler LocalizedInfoRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.LocalizedInfoRowChangeEventHandler LocalizedInfoRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.LocalizedInfoRowChangeEventHandler LocalizedInfoRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.LocalizedInfoRowChangeEventHandler LocalizedInfoRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddLocalizedInfoRow(TextMessagingHostingData.LocalizedInfoRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.LocalizedInfoRow AddLocalizedInfoRow(string DisplayName, string Culture, TextMessagingHostingData.CarrierRow parentCarrierRowByCarrier_LocalizedInfo)
			{
				TextMessagingHostingData.LocalizedInfoRow localizedInfoRow = (TextMessagingHostingData.LocalizedInfoRow)base.NewRow();
				object[] array = new object[3];
				array[0] = DisplayName;
				array[1] = Culture;
				object[] array2 = array;
				if (parentCarrierRowByCarrier_LocalizedInfo != null)
				{
					array2[2] = parentCarrierRowByCarrier_LocalizedInfo[0];
				}
				localizedInfoRow.ItemArray = array2;
				base.Rows.Add(localizedInfoRow);
				return localizedInfoRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.LocalizedInfoRow FindByCultureCarrierIdentity(string Culture, int CarrierIdentity)
			{
				return (TextMessagingHostingData.LocalizedInfoRow)base.Rows.Find(new object[]
				{
					Culture,
					CarrierIdentity
				});
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.LocalizedInfoDataTable localizedInfoDataTable = (TextMessagingHostingData.LocalizedInfoDataTable)base.Clone();
				localizedInfoDataTable.InitVars();
				return localizedInfoDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.LocalizedInfoDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnDisplayName = base.Columns["DisplayName"];
				this.columnCulture = base.Columns["Culture"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnDisplayName = new DataColumn("DisplayName", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnDisplayName);
				this.columnCulture = new DataColumn("Culture", typeof(string), null, MappingType.Attribute);
				base.Columns.Add(this.columnCulture);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				base.Constraints.Add(new UniqueConstraint("LocalizedInfoKey1", new DataColumn[]
				{
					this.columnCulture,
					this.columnCarrierIdentity
				}, true));
				this.columnCulture.AllowDBNull = false;
				this.columnCulture.Namespace = "";
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.LocalizedInfoRow NewLocalizedInfoRow()
			{
				return (TextMessagingHostingData.LocalizedInfoRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.LocalizedInfoRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.LocalizedInfoRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.LocalizedInfoRowChanged != null)
				{
					this.LocalizedInfoRowChanged(this, new TextMessagingHostingData.LocalizedInfoRowChangeEvent((TextMessagingHostingData.LocalizedInfoRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.LocalizedInfoRowChanging != null)
				{
					this.LocalizedInfoRowChanging(this, new TextMessagingHostingData.LocalizedInfoRowChangeEvent((TextMessagingHostingData.LocalizedInfoRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.LocalizedInfoRowDeleted != null)
				{
					this.LocalizedInfoRowDeleted(this, new TextMessagingHostingData.LocalizedInfoRowChangeEvent((TextMessagingHostingData.LocalizedInfoRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.LocalizedInfoRowDeleting != null)
				{
					this.LocalizedInfoRowDeleting(this, new TextMessagingHostingData.LocalizedInfoRowChangeEvent((TextMessagingHostingData.LocalizedInfoRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveLocalizedInfoRow(TextMessagingHostingData.LocalizedInfoRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "LocalizedInfoDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnDisplayName;

			private DataColumn columnCulture;

			private DataColumn columnCarrierIdentity;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class ServicesDataTable : TypedTableBase<TextMessagingHostingData.ServicesRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public ServicesDataTable()
			{
				base.TableName = "Services";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal ServicesDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected ServicesDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn Services_IdColumn
			{
				get
				{
					return this.columnServices_Id;
				}
			}

			[Browsable(false)]
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServicesRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.ServicesRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServicesRowChangeEventHandler ServicesRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServicesRowChangeEventHandler ServicesRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServicesRowChangeEventHandler ServicesRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServicesRowChangeEventHandler ServicesRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddServicesRow(TextMessagingHostingData.ServicesRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServicesRow AddServicesRow()
			{
				TextMessagingHostingData.ServicesRow servicesRow = (TextMessagingHostingData.ServicesRow)base.NewRow();
				object[] array = new object[1];
				object[] itemArray = array;
				servicesRow.ItemArray = itemArray;
				base.Rows.Add(servicesRow);
				return servicesRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.ServicesDataTable servicesDataTable = (TextMessagingHostingData.ServicesDataTable)base.Clone();
				servicesDataTable.InitVars();
				return servicesDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.ServicesDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnServices_Id = base.Columns["Services_Id"];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnServices_Id = new DataColumn("Services_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnServices_Id);
				base.Constraints.Add(new UniqueConstraint("ServicesKey1", new DataColumn[]
				{
					this.columnServices_Id
				}, true));
				this.columnServices_Id.AutoIncrement = true;
				this.columnServices_Id.AllowDBNull = false;
				this.columnServices_Id.Unique = true;
				this.columnServices_Id.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServicesRow NewServicesRow()
			{
				return (TextMessagingHostingData.ServicesRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.ServicesRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.ServicesRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.ServicesRowChanged != null)
				{
					this.ServicesRowChanged(this, new TextMessagingHostingData.ServicesRowChangeEvent((TextMessagingHostingData.ServicesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.ServicesRowChanging != null)
				{
					this.ServicesRowChanging(this, new TextMessagingHostingData.ServicesRowChangeEvent((TextMessagingHostingData.ServicesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.ServicesRowDeleted != null)
				{
					this.ServicesRowDeleted(this, new TextMessagingHostingData.ServicesRowChangeEvent((TextMessagingHostingData.ServicesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.ServicesRowDeleting != null)
				{
					this.ServicesRowDeleting(this, new TextMessagingHostingData.ServicesRowChangeEvent((TextMessagingHostingData.ServicesRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveServicesRow(TextMessagingHostingData.ServicesRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "ServicesDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnServices_Id;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class ServiceDataTable : TypedTableBase<TextMessagingHostingData.ServiceRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public ServiceDataTable()
			{
				base.TableName = "Service";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal ServiceDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected ServiceDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn Services_IdColumn
			{
				get
				{
					return this.columnServices_Id;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn TypeColumn
			{
				get
				{
					return this.columnType;
				}
			}

			[Browsable(false)]
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.ServiceRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServiceRowChangeEventHandler ServiceRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServiceRowChangeEventHandler ServiceRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServiceRowChangeEventHandler ServiceRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.ServiceRowChangeEventHandler ServiceRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddServiceRow(TextMessagingHostingData.ServiceRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow AddServiceRow(TextMessagingHostingData.ServicesRow parentServicesRowByFK_Services_Service, TextMessagingHostingData.RegionRow parentRegionRowByFK_Region_Service, TextMessagingHostingData.CarrierRow parentCarrierRowByFK_Carrier_Service, string Type)
			{
				TextMessagingHostingData.ServiceRow serviceRow = (TextMessagingHostingData.ServiceRow)base.NewRow();
				object[] array = new object[]
				{
					null,
					null,
					null,
					Type
				};
				if (parentServicesRowByFK_Services_Service != null)
				{
					array[0] = parentServicesRowByFK_Services_Service[0];
				}
				if (parentRegionRowByFK_Region_Service != null)
				{
					array[1] = parentRegionRowByFK_Region_Service[2];
				}
				if (parentCarrierRowByFK_Carrier_Service != null)
				{
					array[2] = parentCarrierRowByFK_Carrier_Service[0];
				}
				serviceRow.ItemArray = array;
				base.Rows.Add(serviceRow);
				return serviceRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow FindByRegionIso2CarrierIdentityType(string RegionIso2, int CarrierIdentity, string Type)
			{
				return (TextMessagingHostingData.ServiceRow)base.Rows.Find(new object[]
				{
					RegionIso2,
					CarrierIdentity,
					Type
				});
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.ServiceDataTable serviceDataTable = (TextMessagingHostingData.ServiceDataTable)base.Clone();
				serviceDataTable.InitVars();
				return serviceDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.ServiceDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnServices_Id = base.Columns["Services_Id"];
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnType = base.Columns["Type"];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnServices_Id = new DataColumn("Services_Id", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnServices_Id);
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnRegionIso2);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Element);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnType = new DataColumn("Type", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnType);
				base.Constraints.Add(new UniqueConstraint("ServiceKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnType
				}, true));
				this.columnServices_Id.Namespace = "";
				this.columnRegionIso2.AllowDBNull = false;
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnType.AllowDBNull = false;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow NewServiceRow()
			{
				return (TextMessagingHostingData.ServiceRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.ServiceRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.ServiceRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.ServiceRowChanged != null)
				{
					this.ServiceRowChanged(this, new TextMessagingHostingData.ServiceRowChangeEvent((TextMessagingHostingData.ServiceRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.ServiceRowChanging != null)
				{
					this.ServiceRowChanging(this, new TextMessagingHostingData.ServiceRowChangeEvent((TextMessagingHostingData.ServiceRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.ServiceRowDeleted != null)
				{
					this.ServiceRowDeleted(this, new TextMessagingHostingData.ServiceRowChangeEvent((TextMessagingHostingData.ServiceRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.ServiceRowDeleting != null)
				{
					this.ServiceRowDeleting(this, new TextMessagingHostingData.ServiceRowChangeEvent((TextMessagingHostingData.ServiceRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveServiceRow(TextMessagingHostingData.ServiceRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "ServiceDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnServices_Id;

			private DataColumn columnRegionIso2;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnType;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class VoiceCallForwardingDataTable : TypedTableBase<TextMessagingHostingData.VoiceCallForwardingRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public VoiceCallForwardingDataTable()
			{
				base.TableName = "VoiceCallForwarding";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal VoiceCallForwardingDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected VoiceCallForwardingDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn EnableColumn
			{
				get
				{
					return this.columnEnable;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn DisableColumn
			{
				get
				{
					return this.columnDisable;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn TypeColumn
			{
				get
				{
					return this.columnType;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn ServiceTypeColumn
			{
				get
				{
					return this.columnServiceType;
				}
			}

			[Browsable(false)]
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.VoiceCallForwardingRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.VoiceCallForwardingRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.VoiceCallForwardingRowChangeEventHandler VoiceCallForwardingRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.VoiceCallForwardingRowChangeEventHandler VoiceCallForwardingRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.VoiceCallForwardingRowChangeEventHandler VoiceCallForwardingRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.VoiceCallForwardingRowChangeEventHandler VoiceCallForwardingRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddVoiceCallForwardingRow(TextMessagingHostingData.VoiceCallForwardingRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.VoiceCallForwardingRow AddVoiceCallForwardingRow(string Enable, string Disable, string Type, string RegionIso2, int CarrierIdentity, string ServiceType)
			{
				TextMessagingHostingData.VoiceCallForwardingRow voiceCallForwardingRow = (TextMessagingHostingData.VoiceCallForwardingRow)base.NewRow();
				object[] itemArray = new object[]
				{
					Enable,
					Disable,
					Type,
					RegionIso2,
					CarrierIdentity,
					ServiceType
				};
				voiceCallForwardingRow.ItemArray = itemArray;
				base.Rows.Add(voiceCallForwardingRow);
				return voiceCallForwardingRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.VoiceCallForwardingDataTable voiceCallForwardingDataTable = (TextMessagingHostingData.VoiceCallForwardingDataTable)base.Clone();
				voiceCallForwardingDataTable.InitVars();
				return voiceCallForwardingDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.VoiceCallForwardingDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnEnable = base.Columns["Enable"];
				this.columnDisable = base.Columns["Disable"];
				this.columnType = base.Columns["Type"];
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnServiceType = base.Columns["ServiceType"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnEnable = new DataColumn("Enable", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnEnable);
				this.columnDisable = new DataColumn("Disable", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnDisable);
				this.columnType = new DataColumn("Type", typeof(string), null, MappingType.Attribute);
				base.Columns.Add(this.columnType);
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegionIso2);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnServiceType = new DataColumn("ServiceType", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnServiceType);
				base.Constraints.Add(new UniqueConstraint("VoiceCallForwardingKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnServiceType
				}, true));
				this.columnType.Namespace = "";
				this.columnRegionIso2.AllowDBNull = false;
				this.columnRegionIso2.Namespace = "";
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
				this.columnServiceType.AllowDBNull = false;
				this.columnServiceType.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.VoiceCallForwardingRow NewVoiceCallForwardingRow()
			{
				return (TextMessagingHostingData.VoiceCallForwardingRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.VoiceCallForwardingRow(builder);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.VoiceCallForwardingRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.VoiceCallForwardingRowChanged != null)
				{
					this.VoiceCallForwardingRowChanged(this, new TextMessagingHostingData.VoiceCallForwardingRowChangeEvent((TextMessagingHostingData.VoiceCallForwardingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.VoiceCallForwardingRowChanging != null)
				{
					this.VoiceCallForwardingRowChanging(this, new TextMessagingHostingData.VoiceCallForwardingRowChangeEvent((TextMessagingHostingData.VoiceCallForwardingRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.VoiceCallForwardingRowDeleted != null)
				{
					this.VoiceCallForwardingRowDeleted(this, new TextMessagingHostingData.VoiceCallForwardingRowChangeEvent((TextMessagingHostingData.VoiceCallForwardingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.VoiceCallForwardingRowDeleting != null)
				{
					this.VoiceCallForwardingRowDeleting(this, new TextMessagingHostingData.VoiceCallForwardingRowChangeEvent((TextMessagingHostingData.VoiceCallForwardingRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveVoiceCallForwardingRow(TextMessagingHostingData.VoiceCallForwardingRow row)
			{
				base.Rows.Remove(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "VoiceCallForwardingDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnEnable;

			private DataColumn columnDisable;

			private DataColumn columnType;

			private DataColumn columnRegionIso2;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnServiceType;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class SmtpToSmsGatewayDataTable : TypedTableBase<TextMessagingHostingData.SmtpToSmsGatewayRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public SmtpToSmsGatewayDataTable()
			{
				base.TableName = "SmtpToSmsGateway";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal SmtpToSmsGatewayDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected SmtpToSmsGatewayDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn ServiceTypeColumn
			{
				get
				{
					return this.columnServiceType;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[Browsable(false)]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.SmtpToSmsGatewayRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.SmtpToSmsGatewayRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.SmtpToSmsGatewayRowChangeEventHandler SmtpToSmsGatewayRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.SmtpToSmsGatewayRowChangeEventHandler SmtpToSmsGatewayRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.SmtpToSmsGatewayRowChangeEventHandler SmtpToSmsGatewayRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.SmtpToSmsGatewayRowChangeEventHandler SmtpToSmsGatewayRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddSmtpToSmsGatewayRow(TextMessagingHostingData.SmtpToSmsGatewayRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.SmtpToSmsGatewayRow AddSmtpToSmsGatewayRow(string RegionIso2, int CarrierIdentity, string ServiceType)
			{
				TextMessagingHostingData.SmtpToSmsGatewayRow smtpToSmsGatewayRow = (TextMessagingHostingData.SmtpToSmsGatewayRow)base.NewRow();
				object[] itemArray = new object[]
				{
					RegionIso2,
					CarrierIdentity,
					ServiceType
				};
				smtpToSmsGatewayRow.ItemArray = itemArray;
				base.Rows.Add(smtpToSmsGatewayRow);
				return smtpToSmsGatewayRow;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.SmtpToSmsGatewayDataTable smtpToSmsGatewayDataTable = (TextMessagingHostingData.SmtpToSmsGatewayDataTable)base.Clone();
				smtpToSmsGatewayDataTable.InitVars();
				return smtpToSmsGatewayDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.SmtpToSmsGatewayDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnServiceType = base.Columns["ServiceType"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegionIso2);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnServiceType = new DataColumn("ServiceType", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnServiceType);
				base.Constraints.Add(new UniqueConstraint("SmtpToSmsGatewayKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnServiceType
				}, true));
				this.columnRegionIso2.AllowDBNull = false;
				this.columnRegionIso2.Namespace = "";
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
				this.columnServiceType.AllowDBNull = false;
				this.columnServiceType.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.SmtpToSmsGatewayRow NewSmtpToSmsGatewayRow()
			{
				return (TextMessagingHostingData.SmtpToSmsGatewayRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.SmtpToSmsGatewayRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.SmtpToSmsGatewayRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.SmtpToSmsGatewayRowChanged != null)
				{
					this.SmtpToSmsGatewayRowChanged(this, new TextMessagingHostingData.SmtpToSmsGatewayRowChangeEvent((TextMessagingHostingData.SmtpToSmsGatewayRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.SmtpToSmsGatewayRowChanging != null)
				{
					this.SmtpToSmsGatewayRowChanging(this, new TextMessagingHostingData.SmtpToSmsGatewayRowChangeEvent((TextMessagingHostingData.SmtpToSmsGatewayRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.SmtpToSmsGatewayRowDeleted != null)
				{
					this.SmtpToSmsGatewayRowDeleted(this, new TextMessagingHostingData.SmtpToSmsGatewayRowChangeEvent((TextMessagingHostingData.SmtpToSmsGatewayRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.SmtpToSmsGatewayRowDeleting != null)
				{
					this.SmtpToSmsGatewayRowDeleting(this, new TextMessagingHostingData.SmtpToSmsGatewayRowChangeEvent((TextMessagingHostingData.SmtpToSmsGatewayRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void RemoveSmtpToSmsGatewayRow(TextMessagingHostingData.SmtpToSmsGatewayRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "SmtpToSmsGatewayDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnRegionIso2;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnServiceType;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class RecipientAddressingDataTable : TypedTableBase<TextMessagingHostingData.RecipientAddressingRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public RecipientAddressingDataTable()
			{
				base.TableName = "RecipientAddressing";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal RecipientAddressingDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected RecipientAddressingDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn SmtpAddressColumn
			{
				get
				{
					return this.columnSmtpAddress;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn ServiceTypeColumn
			{
				get
				{
					return this.columnServiceType;
				}
			}

			[DebuggerNonUserCode]
			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RecipientAddressingRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.RecipientAddressingRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RecipientAddressingRowChangeEventHandler RecipientAddressingRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RecipientAddressingRowChangeEventHandler RecipientAddressingRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RecipientAddressingRowChangeEventHandler RecipientAddressingRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.RecipientAddressingRowChangeEventHandler RecipientAddressingRowDeleted;

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void AddRecipientAddressingRow(TextMessagingHostingData.RecipientAddressingRow row)
			{
				base.Rows.Add(row);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RecipientAddressingRow AddRecipientAddressingRow(string RegionIso2, string SmtpAddress, int CarrierIdentity, string ServiceType)
			{
				TextMessagingHostingData.RecipientAddressingRow recipientAddressingRow = (TextMessagingHostingData.RecipientAddressingRow)base.NewRow();
				object[] itemArray = new object[]
				{
					RegionIso2,
					SmtpAddress,
					CarrierIdentity,
					ServiceType
				};
				recipientAddressingRow.ItemArray = itemArray;
				base.Rows.Add(recipientAddressingRow);
				return recipientAddressingRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.RecipientAddressingDataTable recipientAddressingDataTable = (TextMessagingHostingData.RecipientAddressingDataTable)base.Clone();
				recipientAddressingDataTable.InitVars();
				return recipientAddressingDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.RecipientAddressingDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnSmtpAddress = base.Columns["SmtpAddress"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnServiceType = base.Columns["ServiceType"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegionIso2);
				this.columnSmtpAddress = new DataColumn("SmtpAddress", typeof(string), null, MappingType.Element);
				base.Columns.Add(this.columnSmtpAddress);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnServiceType = new DataColumn("ServiceType", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnServiceType);
				base.Constraints.Add(new UniqueConstraint("RecipientAddressingKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnServiceType
				}, true));
				this.columnRegionIso2.AllowDBNull = false;
				this.columnRegionIso2.Namespace = "";
				this.columnSmtpAddress.MaxLength = 1000;
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
				this.columnServiceType.AllowDBNull = false;
				this.columnServiceType.Namespace = "";
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RecipientAddressingRow NewRecipientAddressingRow()
			{
				return (TextMessagingHostingData.RecipientAddressingRow)base.NewRow();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.RecipientAddressingRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.RecipientAddressingRow);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.RecipientAddressingRowChanged != null)
				{
					this.RecipientAddressingRowChanged(this, new TextMessagingHostingData.RecipientAddressingRowChangeEvent((TextMessagingHostingData.RecipientAddressingRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.RecipientAddressingRowChanging != null)
				{
					this.RecipientAddressingRowChanging(this, new TextMessagingHostingData.RecipientAddressingRowChangeEvent((TextMessagingHostingData.RecipientAddressingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.RecipientAddressingRowDeleted != null)
				{
					this.RecipientAddressingRowDeleted(this, new TextMessagingHostingData.RecipientAddressingRowChangeEvent((TextMessagingHostingData.RecipientAddressingRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.RecipientAddressingRowDeleting != null)
				{
					this.RecipientAddressingRowDeleting(this, new TextMessagingHostingData.RecipientAddressingRowChangeEvent((TextMessagingHostingData.RecipientAddressingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveRecipientAddressingRow(TextMessagingHostingData.RecipientAddressingRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "RecipientAddressingDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnRegionIso2;

			private DataColumn columnSmtpAddress;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnServiceType;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class MessageRenderingDataTable : TypedTableBase<TextMessagingHostingData.MessageRenderingRow>
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public MessageRenderingDataTable()
			{
				base.TableName = "MessageRendering";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal MessageRenderingDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected MessageRenderingDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn ContainerColumn
			{
				get
				{
					return this.columnContainer;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn ServiceTypeColumn
			{
				get
				{
					return this.columnServiceType;
				}
			}

			[DebuggerNonUserCode]
			[Browsable(false)]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.MessageRenderingRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.MessageRenderingRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.MessageRenderingRowChangeEventHandler MessageRenderingRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.MessageRenderingRowChangeEventHandler MessageRenderingRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.MessageRenderingRowChangeEventHandler MessageRenderingRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.MessageRenderingRowChangeEventHandler MessageRenderingRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddMessageRenderingRow(TextMessagingHostingData.MessageRenderingRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.MessageRenderingRow AddMessageRenderingRow(string Container, string RegionIso2, int CarrierIdentity, string ServiceType)
			{
				TextMessagingHostingData.MessageRenderingRow messageRenderingRow = (TextMessagingHostingData.MessageRenderingRow)base.NewRow();
				object[] itemArray = new object[]
				{
					Container,
					RegionIso2,
					CarrierIdentity,
					ServiceType
				};
				messageRenderingRow.ItemArray = itemArray;
				base.Rows.Add(messageRenderingRow);
				return messageRenderingRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public override DataTable Clone()
			{
				TextMessagingHostingData.MessageRenderingDataTable messageRenderingDataTable = (TextMessagingHostingData.MessageRenderingDataTable)base.Clone();
				messageRenderingDataTable.InitVars();
				return messageRenderingDataTable;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.MessageRenderingDataTable();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal void InitVars()
			{
				this.columnContainer = base.Columns["Container"];
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnServiceType = base.Columns["ServiceType"];
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			private void InitClass()
			{
				this.columnContainer = new DataColumn("Container", typeof(string), null, MappingType.Attribute);
				base.Columns.Add(this.columnContainer);
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegionIso2);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnServiceType = new DataColumn("ServiceType", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnServiceType);
				base.Constraints.Add(new UniqueConstraint("MessageRenderingKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnServiceType
				}, true));
				this.columnContainer.Namespace = "";
				this.columnRegionIso2.AllowDBNull = false;
				this.columnRegionIso2.Namespace = "";
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
				this.columnServiceType.AllowDBNull = false;
				this.columnServiceType.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.MessageRenderingRow NewMessageRenderingRow()
			{
				return (TextMessagingHostingData.MessageRenderingRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.MessageRenderingRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.MessageRenderingRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.MessageRenderingRowChanged != null)
				{
					this.MessageRenderingRowChanged(this, new TextMessagingHostingData.MessageRenderingRowChangeEvent((TextMessagingHostingData.MessageRenderingRow)e.Row, e.Action));
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.MessageRenderingRowChanging != null)
				{
					this.MessageRenderingRowChanging(this, new TextMessagingHostingData.MessageRenderingRowChangeEvent((TextMessagingHostingData.MessageRenderingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.MessageRenderingRowDeleted != null)
				{
					this.MessageRenderingRowDeleted(this, new TextMessagingHostingData.MessageRenderingRowChangeEvent((TextMessagingHostingData.MessageRenderingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.MessageRenderingRowDeleting != null)
				{
					this.MessageRenderingRowDeleting(this, new TextMessagingHostingData.MessageRenderingRowChangeEvent((TextMessagingHostingData.MessageRenderingRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveMessageRenderingRow(TextMessagingHostingData.MessageRenderingRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "MessageRenderingDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnContainer;

			private DataColumn columnRegionIso2;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnServiceType;
		}

		[XmlSchemaProvider("GetTypedTableSchema")]
		[Serializable]
		public class CapacityDataTable : TypedTableBase<TextMessagingHostingData.CapacityRow>
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public CapacityDataTable()
			{
				base.TableName = "Capacity";
				this.BeginInit();
				this.InitClass();
				this.EndInit();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal CapacityDataTable(DataTable table)
			{
				base.TableName = table.TableName;
				if (table.CaseSensitive != table.DataSet.CaseSensitive)
				{
					base.CaseSensitive = table.CaseSensitive;
				}
				if (table.Locale.ToString() != table.DataSet.Locale.ToString())
				{
					base.Locale = table.Locale;
				}
				if (table.Namespace != table.DataSet.Namespace)
				{
					base.Namespace = table.Namespace;
				}
				base.Prefix = table.Prefix;
				base.MinimumCapacity = table.MinimumCapacity;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected CapacityDataTable(SerializationInfo info, StreamingContext context) : base(info, context)
			{
				this.InitVars();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CodingSchemeColumn
			{
				get
				{
					return this.columnCodingScheme;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn Capacity_ValueColumn
			{
				get
				{
					return this.columnCapacity_Value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn RegionIso2Column
			{
				get
				{
					return this.columnRegionIso2;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataColumn CarrierIdentityColumn
			{
				get
				{
					return this.columnCarrierIdentity;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataColumn ServiceTypeColumn
			{
				get
				{
					return this.columnServiceType;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[Browsable(false)]
			[DebuggerNonUserCode]
			public int Count
			{
				get
				{
					return base.Rows.Count;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CapacityRow this[int index]
			{
				get
				{
					return (TextMessagingHostingData.CapacityRow)base.Rows[index];
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CapacityRowChangeEventHandler CapacityRowChanging;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CapacityRowChangeEventHandler CapacityRowChanged;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CapacityRowChangeEventHandler CapacityRowDeleting;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public event TextMessagingHostingData.CapacityRowChangeEventHandler CapacityRowDeleted;

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void AddCapacityRow(TextMessagingHostingData.CapacityRow row)
			{
				base.Rows.Add(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CapacityRow AddCapacityRow(string CodingScheme, int Capacity_Value, string RegionIso2, int CarrierIdentity, string ServiceType)
			{
				TextMessagingHostingData.CapacityRow capacityRow = (TextMessagingHostingData.CapacityRow)base.NewRow();
				object[] itemArray = new object[]
				{
					CodingScheme,
					Capacity_Value,
					RegionIso2,
					CarrierIdentity,
					ServiceType
				};
				capacityRow.ItemArray = itemArray;
				base.Rows.Add(capacityRow);
				return capacityRow;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CapacityRow FindByRegionIso2CarrierIdentityServiceTypeCodingScheme(string RegionIso2, int CarrierIdentity, string ServiceType, string CodingScheme)
			{
				return (TextMessagingHostingData.CapacityRow)base.Rows.Find(new object[]
				{
					RegionIso2,
					CarrierIdentity,
					ServiceType,
					CodingScheme
				});
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public override DataTable Clone()
			{
				TextMessagingHostingData.CapacityDataTable capacityDataTable = (TextMessagingHostingData.CapacityDataTable)base.Clone();
				capacityDataTable.InitVars();
				return capacityDataTable;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override DataTable CreateInstance()
			{
				return new TextMessagingHostingData.CapacityDataTable();
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal void InitVars()
			{
				this.columnCodingScheme = base.Columns["CodingScheme"];
				this.columnCapacity_Value = base.Columns["Capacity_Value"];
				this.columnRegionIso2 = base.Columns["RegionIso2"];
				this.columnCarrierIdentity = base.Columns["CarrierIdentity"];
				this.columnServiceType = base.Columns["ServiceType"];
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			private void InitClass()
			{
				this.columnCodingScheme = new DataColumn("CodingScheme", typeof(string), null, MappingType.Attribute);
				base.Columns.Add(this.columnCodingScheme);
				this.columnCapacity_Value = new DataColumn("Capacity_Value", typeof(int), null, MappingType.SimpleContent);
				base.Columns.Add(this.columnCapacity_Value);
				this.columnRegionIso2 = new DataColumn("RegionIso2", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnRegionIso2);
				this.columnCarrierIdentity = new DataColumn("CarrierIdentity", typeof(int), null, MappingType.Hidden);
				base.Columns.Add(this.columnCarrierIdentity);
				this.columnServiceType = new DataColumn("ServiceType", typeof(string), null, MappingType.Hidden);
				base.Columns.Add(this.columnServiceType);
				base.Constraints.Add(new UniqueConstraint("CapacityKey1", new DataColumn[]
				{
					this.columnRegionIso2,
					this.columnCarrierIdentity,
					this.columnServiceType,
					this.columnCodingScheme
				}, true));
				this.columnCodingScheme.AllowDBNull = false;
				this.columnCodingScheme.Namespace = "";
				this.columnRegionIso2.AllowDBNull = false;
				this.columnRegionIso2.Namespace = "";
				this.columnCarrierIdentity.AllowDBNull = false;
				this.columnCarrierIdentity.Namespace = "";
				this.columnServiceType.AllowDBNull = false;
				this.columnServiceType.Namespace = "";
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CapacityRow NewCapacityRow()
			{
				return (TextMessagingHostingData.CapacityRow)base.NewRow();
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
			{
				return new TextMessagingHostingData.CapacityRow(builder);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			protected override Type GetRowType()
			{
				return typeof(TextMessagingHostingData.CapacityRow);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanged(DataRowChangeEventArgs e)
			{
				base.OnRowChanged(e);
				if (this.CapacityRowChanged != null)
				{
					this.CapacityRowChanged(this, new TextMessagingHostingData.CapacityRowChangeEvent((TextMessagingHostingData.CapacityRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowChanging(DataRowChangeEventArgs e)
			{
				base.OnRowChanging(e);
				if (this.CapacityRowChanging != null)
				{
					this.CapacityRowChanging(this, new TextMessagingHostingData.CapacityRowChangeEvent((TextMessagingHostingData.CapacityRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleted(DataRowChangeEventArgs e)
			{
				base.OnRowDeleted(e);
				if (this.CapacityRowDeleted != null)
				{
					this.CapacityRowDeleted(this, new TextMessagingHostingData.CapacityRowChangeEvent((TextMessagingHostingData.CapacityRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			protected override void OnRowDeleting(DataRowChangeEventArgs e)
			{
				base.OnRowDeleting(e);
				if (this.CapacityRowDeleting != null)
				{
					this.CapacityRowDeleting(this, new TextMessagingHostingData.CapacityRowChangeEvent((TextMessagingHostingData.CapacityRow)e.Row, e.Action));
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void RemoveCapacityRow(TextMessagingHostingData.CapacityRow row)
			{
				base.Rows.Remove(row);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public static XmlSchemaComplexType GetTypedTableSchema(XmlSchemaSet xs)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				TextMessagingHostingData textMessagingHostingData = new TextMessagingHostingData();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.Namespace = "http://www.w3.org/2001/XMLSchema";
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				xmlSchemaAny.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				xmlSchemaAny2.Namespace = "urn:schemas-microsoft-com:xml-diffgram-v1";
				xmlSchemaAny2.MinOccurs = 1m;
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				xmlSchemaSequence.Items.Add(xmlSchemaAny2);
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Name = "namespace";
				xmlSchemaAttribute.FixedValue = textMessagingHostingData.Namespace;
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute);
				XmlSchemaAttribute xmlSchemaAttribute2 = new XmlSchemaAttribute();
				xmlSchemaAttribute2.Name = "tableTypeName";
				xmlSchemaAttribute2.FixedValue = "CapacityDataTable";
				xmlSchemaComplexType.Attributes.Add(xmlSchemaAttribute2);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				XmlSchema schemaSerializable = textMessagingHostingData.GetSchemaSerializable();
				if (xs.Contains(schemaSerializable.TargetNamespace))
				{
					MemoryStream memoryStream = new MemoryStream();
					MemoryStream memoryStream2 = new MemoryStream();
					try
					{
						schemaSerializable.Write(memoryStream);
						foreach (object obj in xs.Schemas(schemaSerializable.TargetNamespace))
						{
							XmlSchema xmlSchema = (XmlSchema)obj;
							memoryStream2.SetLength(0L);
							xmlSchema.Write(memoryStream2);
							if (memoryStream.Length == memoryStream2.Length)
							{
								memoryStream.Position = 0L;
								memoryStream2.Position = 0L;
								while (memoryStream.Position != memoryStream.Length && memoryStream.ReadByte() == memoryStream2.ReadByte())
								{
								}
								if (memoryStream.Position == memoryStream.Length)
								{
									return xmlSchemaComplexType;
								}
							}
						}
					}
					finally
					{
						if (memoryStream != null)
						{
							memoryStream.Close();
						}
						if (memoryStream2 != null)
						{
							memoryStream2.Close();
						}
					}
				}
				xs.Add(schemaSerializable);
				return xmlSchemaComplexType;
			}

			private DataColumn columnCodingScheme;

			private DataColumn columnCapacity_Value;

			private DataColumn columnRegionIso2;

			private DataColumn columnCarrierIdentity;

			private DataColumn columnServiceType;
		}

		public class _locDefinitionRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal _locDefinitionRow(DataRowBuilder rb) : base(rb)
			{
				this.table_locDefinition = (TextMessagingHostingData._locDefinitionDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int _locDefinition_Id
			{
				get
				{
					return (int)base[this.table_locDefinition._locDefinition_IdColumn];
				}
				set
				{
					base[this.table_locDefinition._locDefinition_IdColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string _locDefault_loc
			{
				get
				{
					return (string)base[this.table_locDefinition._locDefault_locColumn];
				}
				set
				{
					base[this.table_locDefinition._locDefault_locColumn] = value;
				}
			}

			private TextMessagingHostingData._locDefinitionDataTable table_locDefinition;
		}

		public class RegionsRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal RegionsRow(DataRowBuilder rb) : base(rb)
			{
				this.tableRegions = (TextMessagingHostingData.RegionsDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Regions_Id
			{
				get
				{
					return (int)base[this.tableRegions.Regions_IdColumn];
				}
				set
				{
					base[this.tableRegions.Regions_IdColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionRow[] GetRegionRows()
			{
				if (base.Table.ChildRelations["Regions_Region"] == null)
				{
					return new TextMessagingHostingData.RegionRow[0];
				}
				return (TextMessagingHostingData.RegionRow[])base.GetChildRows(base.Table.ChildRelations["Regions_Region"]);
			}

			private TextMessagingHostingData.RegionsDataTable tableRegions;
		}

		public class RegionRow : DataRow
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal RegionRow(DataRowBuilder rb) : base(rb)
			{
				this.tableRegion = (TextMessagingHostingData.RegionDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string CountryCode
			{
				get
				{
					return (string)base[this.tableRegion.CountryCodeColumn];
				}
				set
				{
					base[this.tableRegion.CountryCodeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string PhoneNumberExample
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableRegion.PhoneNumberExampleColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'PhoneNumberExample' in table 'Region' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableRegion.PhoneNumberExampleColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string Iso2
			{
				get
				{
					return (string)base[this.tableRegion.Iso2Column];
				}
				set
				{
					base[this.tableRegion.Iso2Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Regions_Id
			{
				get
				{
					int result;
					try
					{
						result = (int)base[this.tableRegion.Regions_IdColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Regions_Id' in table 'Region' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableRegion.Regions_IdColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionsRow RegionsRow
			{
				get
				{
					return (TextMessagingHostingData.RegionsRow)base.GetParentRow(base.Table.ParentRelations["Regions_Region"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["Regions_Region"]);
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public bool IsPhoneNumberExampleNull()
			{
				return base.IsNull(this.tableRegion.PhoneNumberExampleColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetPhoneNumberExampleNull()
			{
				base[this.tableRegion.PhoneNumberExampleColumn] = Convert.DBNull;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsRegions_IdNull()
			{
				return base.IsNull(this.tableRegion.Regions_IdColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void SetRegions_IdNull()
			{
				base[this.tableRegion.Regions_IdColumn] = Convert.DBNull;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow[] GetServiceRows()
			{
				if (base.Table.ChildRelations["FK_Region_Service"] == null)
				{
					return new TextMessagingHostingData.ServiceRow[0];
				}
				return (TextMessagingHostingData.ServiceRow[])base.GetChildRows(base.Table.ChildRelations["FK_Region_Service"]);
			}

			private TextMessagingHostingData.RegionDataTable tableRegion;
		}

		public class CarriersRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal CarriersRow(DataRowBuilder rb) : base(rb)
			{
				this.tableCarriers = (TextMessagingHostingData.CarriersDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Carriers_Id
			{
				get
				{
					return (int)base[this.tableCarriers.Carriers_IdColumn];
				}
				set
				{
					base[this.tableCarriers.Carriers_IdColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarrierRow[] GetCarrierRows()
			{
				if (base.Table.ChildRelations["Carriers_Carrier"] == null)
				{
					return new TextMessagingHostingData.CarrierRow[0];
				}
				return (TextMessagingHostingData.CarrierRow[])base.GetChildRows(base.Table.ChildRelations["Carriers_Carrier"]);
			}

			private TextMessagingHostingData.CarriersDataTable tableCarriers;
		}

		public class CarrierRow : DataRow
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal CarrierRow(DataRowBuilder rb) : base(rb)
			{
				this.tableCarrier = (TextMessagingHostingData.CarrierDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Identity
			{
				get
				{
					return (int)base[this.tableCarrier.IdentityColumn];
				}
				set
				{
					base[this.tableCarrier.IdentityColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Carriers_Id
			{
				get
				{
					int result;
					try
					{
						result = (int)base[this.tableCarrier.Carriers_IdColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Carriers_Id' in table 'Carrier' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableCarrier.Carriers_IdColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarriersRow CarriersRow
			{
				get
				{
					return (TextMessagingHostingData.CarriersRow)base.GetParentRow(base.Table.ParentRelations["Carriers_Carrier"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["Carriers_Carrier"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCarriers_IdNull()
			{
				return base.IsNull(this.tableCarrier.Carriers_IdColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void SetCarriers_IdNull()
			{
				base[this.tableCarrier.Carriers_IdColumn] = Convert.DBNull;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.LocalizedInfoRow[] GetLocalizedInfoRows()
			{
				if (base.Table.ChildRelations["Carrier_LocalizedInfo"] == null)
				{
					return new TextMessagingHostingData.LocalizedInfoRow[0];
				}
				return (TextMessagingHostingData.LocalizedInfoRow[])base.GetChildRows(base.Table.ChildRelations["Carrier_LocalizedInfo"]);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServiceRow[] GetServiceRows()
			{
				if (base.Table.ChildRelations["FK_Carrier_Service"] == null)
				{
					return new TextMessagingHostingData.ServiceRow[0];
				}
				return (TextMessagingHostingData.ServiceRow[])base.GetChildRows(base.Table.ChildRelations["FK_Carrier_Service"]);
			}

			private TextMessagingHostingData.CarrierDataTable tableCarrier;
		}

		public class LocalizedInfoRow : DataRow
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal LocalizedInfoRow(DataRowBuilder rb) : base(rb)
			{
				this.tableLocalizedInfo = (TextMessagingHostingData.LocalizedInfoDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string DisplayName
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableLocalizedInfo.DisplayNameColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'DisplayName' in table 'LocalizedInfo' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableLocalizedInfo.DisplayNameColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string Culture
			{
				get
				{
					return (string)base[this.tableLocalizedInfo.CultureColumn];
				}
				set
				{
					base[this.tableLocalizedInfo.CultureColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableLocalizedInfo.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableLocalizedInfo.CarrierIdentityColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarrierRow CarrierRow
			{
				get
				{
					return (TextMessagingHostingData.CarrierRow)base.GetParentRow(base.Table.ParentRelations["Carrier_LocalizedInfo"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["Carrier_LocalizedInfo"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsDisplayNameNull()
			{
				return base.IsNull(this.tableLocalizedInfo.DisplayNameColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDisplayNameNull()
			{
				base[this.tableLocalizedInfo.DisplayNameColumn] = Convert.DBNull;
			}

			private TextMessagingHostingData.LocalizedInfoDataTable tableLocalizedInfo;
		}

		public class ServicesRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal ServicesRow(DataRowBuilder rb) : base(rb)
			{
				this.tableServices = (TextMessagingHostingData.ServicesDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int Services_Id
			{
				get
				{
					return (int)base[this.tableServices.Services_IdColumn];
				}
				set
				{
					base[this.tableServices.Services_IdColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServiceRow[] GetServiceRows()
			{
				if (base.Table.ChildRelations["FK_Services_Service"] == null)
				{
					return new TextMessagingHostingData.ServiceRow[0];
				}
				return (TextMessagingHostingData.ServiceRow[])base.GetChildRows(base.Table.ChildRelations["FK_Services_Service"]);
			}

			private TextMessagingHostingData.ServicesDataTable tableServices;
		}

		public class ServiceRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal ServiceRow(DataRowBuilder rb) : base(rb)
			{
				this.tableService = (TextMessagingHostingData.ServiceDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Services_Id
			{
				get
				{
					int result;
					try
					{
						result = (int)base[this.tableService.Services_IdColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Services_Id' in table 'Service' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableService.Services_IdColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableService.RegionIso2Column];
				}
				set
				{
					base[this.tableService.RegionIso2Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableService.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableService.CarrierIdentityColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string Type
			{
				get
				{
					return (string)base[this.tableService.TypeColumn];
				}
				set
				{
					base[this.tableService.TypeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServicesRow ServicesRow
			{
				get
				{
					return (TextMessagingHostingData.ServicesRow)base.GetParentRow(base.Table.ParentRelations["FK_Services_Service"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_Services_Service"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarrierRow CarrierRow
			{
				get
				{
					return (TextMessagingHostingData.CarrierRow)base.GetParentRow(base.Table.ParentRelations["FK_Carrier_Service"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_Carrier_Service"]);
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.RegionRow RegionRow
			{
				get
				{
					return (TextMessagingHostingData.RegionRow)base.GetParentRow(base.Table.ParentRelations["FK_Region_Service"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_Region_Service"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsServices_IdNull()
			{
				return base.IsNull(this.tableService.Services_IdColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void SetServices_IdNull()
			{
				base[this.tableService.Services_IdColumn] = Convert.DBNull;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.VoiceCallForwardingRow[] GetVoiceCallForwardingRows()
			{
				if (base.Table.ChildRelations["FK_Service_VoiceCallForwarding"] == null)
				{
					return new TextMessagingHostingData.VoiceCallForwardingRow[0];
				}
				return (TextMessagingHostingData.VoiceCallForwardingRow[])base.GetChildRows(base.Table.ChildRelations["FK_Service_VoiceCallForwarding"]);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.SmtpToSmsGatewayRow[] GetSmtpToSmsGatewayRows()
			{
				if (base.Table.ChildRelations["FK_Service_SmtpToSmsGateway"] == null)
				{
					return new TextMessagingHostingData.SmtpToSmsGatewayRow[0];
				}
				return (TextMessagingHostingData.SmtpToSmsGatewayRow[])base.GetChildRows(base.Table.ChildRelations["FK_Service_SmtpToSmsGateway"]);
			}

			private TextMessagingHostingData.ServiceDataTable tableService;
		}

		public class VoiceCallForwardingRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal VoiceCallForwardingRow(DataRowBuilder rb) : base(rb)
			{
				this.tableVoiceCallForwarding = (TextMessagingHostingData.VoiceCallForwardingDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string Enable
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableVoiceCallForwarding.EnableColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Enable' in table 'VoiceCallForwarding' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableVoiceCallForwarding.EnableColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string Disable
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableVoiceCallForwarding.DisableColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Disable' in table 'VoiceCallForwarding' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableVoiceCallForwarding.DisableColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string Type
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableVoiceCallForwarding.TypeColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Type' in table 'VoiceCallForwarding' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableVoiceCallForwarding.TypeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableVoiceCallForwarding.RegionIso2Column];
				}
				set
				{
					base[this.tableVoiceCallForwarding.RegionIso2Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableVoiceCallForwarding.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableVoiceCallForwarding.CarrierIdentityColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string ServiceType
			{
				get
				{
					return (string)base[this.tableVoiceCallForwarding.ServiceTypeColumn];
				}
				set
				{
					base[this.tableVoiceCallForwarding.ServiceTypeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow ServiceRowParent
			{
				get
				{
					return (TextMessagingHostingData.ServiceRow)base.GetParentRow(base.Table.ParentRelations["FK_Service_VoiceCallForwarding"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_Service_VoiceCallForwarding"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsEnableNull()
			{
				return base.IsNull(this.tableVoiceCallForwarding.EnableColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetEnableNull()
			{
				base[this.tableVoiceCallForwarding.EnableColumn] = Convert.DBNull;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public bool IsDisableNull()
			{
				return base.IsNull(this.tableVoiceCallForwarding.DisableColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetDisableNull()
			{
				base[this.tableVoiceCallForwarding.DisableColumn] = Convert.DBNull;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public bool IsTypeNull()
			{
				return base.IsNull(this.tableVoiceCallForwarding.TypeColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetTypeNull()
			{
				base[this.tableVoiceCallForwarding.TypeColumn] = Convert.DBNull;
			}

			private TextMessagingHostingData.VoiceCallForwardingDataTable tableVoiceCallForwarding;
		}

		public class SmtpToSmsGatewayRow : DataRow
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal SmtpToSmsGatewayRow(DataRowBuilder rb) : base(rb)
			{
				this.tableSmtpToSmsGateway = (TextMessagingHostingData.SmtpToSmsGatewayDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableSmtpToSmsGateway.RegionIso2Column];
				}
				set
				{
					base[this.tableSmtpToSmsGateway.RegionIso2Column] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableSmtpToSmsGateway.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableSmtpToSmsGateway.CarrierIdentityColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string ServiceType
			{
				get
				{
					return (string)base[this.tableSmtpToSmsGateway.ServiceTypeColumn];
				}
				set
				{
					base[this.tableSmtpToSmsGateway.ServiceTypeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServiceRow ServiceRowParent
			{
				get
				{
					return (TextMessagingHostingData.ServiceRow)base.GetParentRow(base.Table.ParentRelations["FK_Service_SmtpToSmsGateway"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_Service_SmtpToSmsGateway"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RecipientAddressingRow[] GetRecipientAddressingRows()
			{
				if (base.Table.ChildRelations["FK_SmtpToSmsGateway_RecipientAddressing"] == null)
				{
					return new TextMessagingHostingData.RecipientAddressingRow[0];
				}
				return (TextMessagingHostingData.RecipientAddressingRow[])base.GetChildRows(base.Table.ChildRelations["FK_SmtpToSmsGateway_RecipientAddressing"]);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.MessageRenderingRow[] GetMessageRenderingRows()
			{
				if (base.Table.ChildRelations["FK_SmtpToSmsGateway_MessageRendering"] == null)
				{
					return new TextMessagingHostingData.MessageRenderingRow[0];
				}
				return (TextMessagingHostingData.MessageRenderingRow[])base.GetChildRows(base.Table.ChildRelations["FK_SmtpToSmsGateway_MessageRendering"]);
			}

			private TextMessagingHostingData.SmtpToSmsGatewayDataTable tableSmtpToSmsGateway;
		}

		public class RecipientAddressingRow : DataRow
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			internal RecipientAddressingRow(DataRowBuilder rb) : base(rb)
			{
				this.tableRecipientAddressing = (TextMessagingHostingData.RecipientAddressingDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableRecipientAddressing.RegionIso2Column];
				}
				set
				{
					base[this.tableRecipientAddressing.RegionIso2Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string SmtpAddress
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableRecipientAddressing.SmtpAddressColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'SmtpAddress' in table 'RecipientAddressing' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableRecipientAddressing.SmtpAddressColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableRecipientAddressing.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableRecipientAddressing.CarrierIdentityColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string ServiceType
			{
				get
				{
					return (string)base[this.tableRecipientAddressing.ServiceTypeColumn];
				}
				set
				{
					base[this.tableRecipientAddressing.ServiceTypeColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.SmtpToSmsGatewayRow SmtpToSmsGatewayRowParent
			{
				get
				{
					return (TextMessagingHostingData.SmtpToSmsGatewayRow)base.GetParentRow(base.Table.ParentRelations["FK_SmtpToSmsGateway_RecipientAddressing"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_SmtpToSmsGateway_RecipientAddressing"]);
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public bool IsSmtpAddressNull()
			{
				return base.IsNull(this.tableRecipientAddressing.SmtpAddressColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetSmtpAddressNull()
			{
				base[this.tableRecipientAddressing.SmtpAddressColumn] = Convert.DBNull;
			}

			private TextMessagingHostingData.RecipientAddressingDataTable tableRecipientAddressing;
		}

		public class MessageRenderingRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal MessageRenderingRow(DataRowBuilder rb) : base(rb)
			{
				this.tableMessageRendering = (TextMessagingHostingData.MessageRenderingDataTable)base.Table;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string Container
			{
				get
				{
					string result;
					try
					{
						result = (string)base[this.tableMessageRendering.ContainerColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Container' in table 'MessageRendering' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableMessageRendering.ContainerColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableMessageRendering.RegionIso2Column];
				}
				set
				{
					base[this.tableMessageRendering.RegionIso2Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableMessageRendering.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableMessageRendering.CarrierIdentityColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string ServiceType
			{
				get
				{
					return (string)base[this.tableMessageRendering.ServiceTypeColumn];
				}
				set
				{
					base[this.tableMessageRendering.ServiceTypeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.SmtpToSmsGatewayRow SmtpToSmsGatewayRowParent
			{
				get
				{
					return (TextMessagingHostingData.SmtpToSmsGatewayRow)base.GetParentRow(base.Table.ParentRelations["FK_SmtpToSmsGateway_MessageRendering"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_SmtpToSmsGateway_MessageRendering"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsContainerNull()
			{
				return base.IsNull(this.tableMessageRendering.ContainerColumn);
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public void SetContainerNull()
			{
				base[this.tableMessageRendering.ContainerColumn] = Convert.DBNull;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CapacityRow[] GetCapacityRows()
			{
				if (base.Table.ChildRelations["FK_MessageRendering_Capacity"] == null)
				{
					return new TextMessagingHostingData.CapacityRow[0];
				}
				return (TextMessagingHostingData.CapacityRow[])base.GetChildRows(base.Table.ChildRelations["FK_MessageRendering_Capacity"]);
			}

			private TextMessagingHostingData.MessageRenderingDataTable tableMessageRendering;
		}

		public class CapacityRow : DataRow
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			internal CapacityRow(DataRowBuilder rb) : base(rb)
			{
				this.tableCapacity = (TextMessagingHostingData.CapacityDataTable)base.Table;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public string CodingScheme
			{
				get
				{
					return (string)base[this.tableCapacity.CodingSchemeColumn];
				}
				set
				{
					base[this.tableCapacity.CodingSchemeColumn] = value;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public int Capacity_Value
			{
				get
				{
					int result;
					try
					{
						result = (int)base[this.tableCapacity.Capacity_ValueColumn];
					}
					catch (InvalidCastException innerException)
					{
						throw new StrongTypingException("The value for column 'Capacity_Value' in table 'Capacity' is DBNull.", innerException);
					}
					return result;
				}
				set
				{
					base[this.tableCapacity.Capacity_ValueColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string RegionIso2
			{
				get
				{
					return (string)base[this.tableCapacity.RegionIso2Column];
				}
				set
				{
					base[this.tableCapacity.RegionIso2Column] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public int CarrierIdentity
			{
				get
				{
					return (int)base[this.tableCapacity.CarrierIdentityColumn];
				}
				set
				{
					base[this.tableCapacity.CarrierIdentityColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public string ServiceType
			{
				get
				{
					return (string)base[this.tableCapacity.ServiceTypeColumn];
				}
				set
				{
					base[this.tableCapacity.ServiceTypeColumn] = value;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.MessageRenderingRow MessageRenderingRowParent
			{
				get
				{
					return (TextMessagingHostingData.MessageRenderingRow)base.GetParentRow(base.Table.ParentRelations["FK_MessageRendering_Capacity"]);
				}
				set
				{
					base.SetParentRow(value, base.Table.ParentRelations["FK_MessageRendering_Capacity"]);
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public bool IsCapacity_ValueNull()
			{
				return base.IsNull(this.tableCapacity.Capacity_ValueColumn);
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public void SetCapacity_ValueNull()
			{
				base[this.tableCapacity.Capacity_ValueColumn] = Convert.DBNull;
			}

			private TextMessagingHostingData.CapacityDataTable tableCapacity;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class _locDefinitionRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public _locDefinitionRowChangeEvent(TextMessagingHostingData._locDefinitionRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData._locDefinitionRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData._locDefinitionRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class RegionsRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public RegionsRowChangeEvent(TextMessagingHostingData.RegionsRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RegionsRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.RegionsRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class RegionRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public RegionRowChangeEvent(TextMessagingHostingData.RegionRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RegionRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.RegionRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class CarriersRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public CarriersRowChangeEvent(TextMessagingHostingData.CarriersRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.CarriersRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.CarriersRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class CarrierRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public CarrierRowChangeEvent(TextMessagingHostingData.CarrierRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CarrierRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.CarrierRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class LocalizedInfoRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public LocalizedInfoRowChangeEvent(TextMessagingHostingData.LocalizedInfoRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.LocalizedInfoRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.LocalizedInfoRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class ServicesRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public ServicesRowChangeEvent(TextMessagingHostingData.ServicesRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.ServicesRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.ServicesRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class ServiceRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public ServiceRowChangeEvent(TextMessagingHostingData.ServiceRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.ServiceRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.ServiceRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class VoiceCallForwardingRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public VoiceCallForwardingRowChangeEvent(TextMessagingHostingData.VoiceCallForwardingRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.VoiceCallForwardingRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.VoiceCallForwardingRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class SmtpToSmsGatewayRowChangeEvent : EventArgs
		{
			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public SmtpToSmsGatewayRowChangeEvent(TextMessagingHostingData.SmtpToSmsGatewayRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.SmtpToSmsGatewayRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.SmtpToSmsGatewayRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class RecipientAddressingRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public RecipientAddressingRowChangeEvent(TextMessagingHostingData.RecipientAddressingRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public TextMessagingHostingData.RecipientAddressingRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.RecipientAddressingRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class MessageRenderingRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public MessageRenderingRowChangeEvent(TextMessagingHostingData.MessageRenderingRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.MessageRenderingRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.MessageRenderingRow eventRow;

			private DataRowAction eventAction;
		}

		[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
		public class CapacityRowChangeEvent : EventArgs
		{
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			[DebuggerNonUserCode]
			public CapacityRowChangeEvent(TextMessagingHostingData.CapacityRow row, DataRowAction action)
			{
				this.eventRow = row;
				this.eventAction = action;
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public TextMessagingHostingData.CapacityRow Row
			{
				get
				{
					return this.eventRow;
				}
			}

			[DebuggerNonUserCode]
			[GeneratedCode("System.Data.Design.TypedDataSetGenerator", "4.0.0.0")]
			public DataRowAction Action
			{
				get
				{
					return this.eventAction;
				}
			}

			private TextMessagingHostingData.CapacityRow eventRow;

			private DataRowAction eventAction;
		}
	}
}
