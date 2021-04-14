using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "AirSync", TypeName = "Collection")]
	public class Collection
	{
		public Collection()
		{
			this.Options = new List<Options>();
			this.Commands = new List<Command>();
		}

		[XmlElement(ElementName = "SyncKey")]
		public string SyncKey { get; set; }

		[XmlElement(ElementName = "NotifyGUID")]
		public string NotifyGuid { get; set; }

		[XmlElement(ElementName = "CollectionId")]
		public string CollectionId { get; set; }

		[XmlIgnore]
		public bool? DeletesAsMoves { get; set; }

		[XmlElement(ElementName = "DeletesAsMoves")]
		public string SerializableDeletesAsMoves
		{
			get
			{
				if (!(this.DeletesAsMoves == true))
				{
					return "0";
				}
				return "1";
			}
			set
			{
				this.DeletesAsMoves = new bool?(XmlConvert.ToBoolean(value));
			}
		}

		[XmlIgnore]
		public bool? GetChanges { get; set; }

		[XmlElement(ElementName = "GetChanges")]
		public string SerializableGetChanges
		{
			get
			{
				if (!(this.GetChanges == true))
				{
					return "0";
				}
				return "1";
			}
			set
			{
				this.GetChanges = new bool?(XmlConvert.ToBoolean(value));
			}
		}

		[XmlElement(ElementName = "WindowSize")]
		public int? WindowSize { get; set; }

		[XmlElement(ElementName = "ConversationMode")]
		public object ConversationMode { get; set; }

		[XmlElement(ElementName = "Options")]
		public List<Options> Options { get; set; }

		[XmlArrayItem("Fetch", typeof(FetchCommand))]
		[XmlArrayItem("Delete", typeof(DeleteCommand))]
		[XmlArrayItem("Add", typeof(AddCommand))]
		[XmlArrayItem("Change", typeof(ChangeCommand))]
		public List<Command> Commands { get; set; }

		[XmlIgnore]
		public bool CommandsSpecified
		{
			get
			{
				return this.Commands.Count > 0;
			}
		}

		[XmlIgnore]
		public bool SerializableDeletesAsMovesSpecified
		{
			get
			{
				return this.DeletesAsMoves != null;
			}
		}

		[XmlIgnore]
		public bool SerializableGetChangesSpecified
		{
			get
			{
				return this.GetChanges != null;
			}
		}

		[XmlIgnore]
		public bool WindowSizeSpecified
		{
			get
			{
				return this.WindowSize != null;
			}
		}
	}
}
