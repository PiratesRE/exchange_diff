using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal abstract class BasicNotification : BasicDataContract
	{
		public BasicNotification(string identifier = null)
		{
			this.Identifier = (identifier ?? BasicNotification.GetNextId());
		}

		[DataMember(Name = "id", EmitDefaultValue = false)]
		public string Identifier { get; private set; }

		public sealed override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "id:" + this.Identifier;
			}
			return this.toString;
		}

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			if (this.Identifier == null)
			{
				this.Identifier = BasicNotification.GetNextId();
			}
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("id:").Append(this.Identifier).Append("; ");
			sb.Append("type:").Append(base.GetType().ToString()).Append("; ");
		}

		private static string GetNextId()
		{
			int num = Interlocked.Increment(ref BasicNotification.idCounter);
			return string.Format(BasicNotification.IdTemplate, num);
		}

		private static readonly string IdTemplate = ExDateTime.UtcNow.ToString("yyyyMMdd-HHmmss-{0}");

		private static int idCounter;

		private string toString;
	}
}
