using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class BasicDataContract
	{
		public bool IsValid
		{
			get
			{
				if (this.isValid == null)
				{
					this.RunValidationCheck();
				}
				return this.isValid.Value;
			}
		}

		public List<LocalizedString> ValidationErrors
		{
			get
			{
				if (!this.IsValid)
				{
					return this.validationErrors;
				}
				throw new InvalidOperationException("ValidationErrors are not available when the instance is valid");
			}
		}

		public virtual string ToJson()
		{
			return JsonConverter.Serialize<BasicDataContract>(this, null);
		}

		public string ToFullString()
		{
			if (this.toFullString == null)
			{
				StringBuilder stringBuilder = new StringBuilder().Append("{ ");
				this.InternalToFullString(stringBuilder);
				stringBuilder.Append("}");
				this.toFullString = stringBuilder.ToString();
			}
			return this.toFullString;
		}

		protected virtual void InternalToFullString(StringBuilder sb)
		{
		}

		protected virtual void InternalValidate(List<LocalizedString> errors)
		{
		}

		private void RunValidationCheck()
		{
			List<LocalizedString> list = new List<LocalizedString>();
			this.InternalValidate(list);
			if (list.Count == 0)
			{
				this.isValid = new bool?(true);
				return;
			}
			this.validationErrors = list;
			this.isValid = new bool?(false);
		}

		private string toFullString;

		private bool? isValid;

		private List<LocalizedString> validationErrors;
	}
}
