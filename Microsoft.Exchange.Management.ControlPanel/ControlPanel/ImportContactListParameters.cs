using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ImportContactListParameters : SetObjectProperties
	{
		public ImportContactListParameters()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public Stream CSVStream
		{
			get
			{
				return (Stream)base["CSVStream"];
			}
			set
			{
				base["CSVStream"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Import-ContactList";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self|Organization";
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["CSV"] = new SwitchParameter(true);
		}

		private const string CsvSuffix = "&CSV";

		private const string CsvStreamSuffix = "&CSVStream";

		public const string RbacParameters = "?Identity&CSV&CSVStream";
	}
}
