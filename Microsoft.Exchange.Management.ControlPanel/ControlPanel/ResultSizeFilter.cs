using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class ResultSizeFilter : WebServiceParameters
	{
		protected ResultSizeFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.ResultSize = 500;
		}

		[DataMember]
		public int ResultSize
		{
			get
			{
				return (int)base["ResultSize"];
			}
			set
			{
				base["ResultSize"] = value;
			}
		}

		public const int ResultSizeLimit = 500;

		public const string RbacParameters = "?ResultSize";
	}
}
