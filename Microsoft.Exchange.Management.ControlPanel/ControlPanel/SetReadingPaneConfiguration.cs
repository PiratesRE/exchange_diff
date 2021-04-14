using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetReadingPaneConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public string PreviewMarkAsReadBehavior
		{
			get
			{
				return (string)base["PreviewMarkAsReadBehavior"];
			}
			set
			{
				base["PreviewMarkAsReadBehavior"] = value;
			}
		}

		[DataMember]
		public string EmailComposeMode
		{
			get
			{
				return (string)base["EmailComposeMode"];
			}
			set
			{
				base["EmailComposeMode"] = value;
			}
		}

		[DataMember]
		public string PreviewMarkAsReadDelaytime { get; set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.PreviewMarkAsReadDelaytime != null)
			{
				int num;
				if (int.TryParse(this.PreviewMarkAsReadDelaytime, out num) && num >= 0 && num <= 30)
				{
					base["PreviewMarkAsReadDelaytime"] = num;
					return;
				}
				if (this.PreviewMarkAsReadBehavior == Microsoft.Exchange.Data.Storage.Management.PreviewMarkAsReadBehavior.Delayed.ToString())
				{
					throw new FaultException(OwaOptionStrings.PreviewMarkAsReadDelaytimeErrorMessage);
				}
			}
		}
	}
}
