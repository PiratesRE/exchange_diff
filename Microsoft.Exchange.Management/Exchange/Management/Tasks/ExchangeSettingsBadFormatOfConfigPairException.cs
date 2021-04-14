using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeSettingsBadFormatOfConfigPairException : ExchangeSettingsException
	{
		public ExchangeSettingsBadFormatOfConfigPairException(string pair) : base(Strings.ExchangeSettingsBadFormatOfConfigPair(pair))
		{
			this.pair = pair;
		}

		public ExchangeSettingsBadFormatOfConfigPairException(string pair, Exception innerException) : base(Strings.ExchangeSettingsBadFormatOfConfigPair(pair), innerException)
		{
			this.pair = pair;
		}

		protected ExchangeSettingsBadFormatOfConfigPairException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pair = (string)info.GetValue("pair", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pair", this.pair);
		}

		public string Pair
		{
			get
			{
				return this.pair;
			}
		}

		private readonly string pair;
	}
}
