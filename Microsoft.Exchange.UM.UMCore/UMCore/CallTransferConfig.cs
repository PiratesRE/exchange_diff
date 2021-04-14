using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallTransferConfig : ActivityConfig
	{
		internal CallTransferConfig(ActivityManagerConfig manager) : base(manager)
		{
		}

		internal CallTransferType TransferType
		{
			get
			{
				return this.transferType;
			}
		}

		internal string PhoneNumber
		{
			get
			{
				return this.phoneNumber;
			}
		}

		internal string PhoneNumberType
		{
			get
			{
				return this.phoneNumberType;
			}
		}

		internal override ActivityBase CreateActivity(ActivityManager manager)
		{
			return new CallTransfer(manager, this);
		}

		internal PhoneNumber GetPhoneNumberVariable(ActivityManager manager)
		{
			ExAssert.RetailAssert(this.fsmVariable != null, "Fsm variable is null. We only initialize it when phoneNumberType is Variable");
			return this.fsmVariable.GetValue(manager);
		}

		protected override void LoadAttributes(XmlNode rootNode)
		{
			base.LoadAttributes(rootNode);
			this.transferType = (CallTransferType)Enum.Parse(typeof(CallTransferType), rootNode.Attributes["type"].Value, true);
			XmlNode xmlNode = rootNode.Attributes["numberType"];
			if (xmlNode == null)
			{
				this.phoneNumberType = "literal";
			}
			else
			{
				this.phoneNumberType = xmlNode.Value;
			}
			if (string.Equals(this.phoneNumberType, "variable", StringComparison.OrdinalIgnoreCase))
			{
				QualifiedName variableName = new QualifiedName(rootNode.Attributes["number"].Value, base.ManagerConfig);
				this.fsmVariable = FsmVariable<Microsoft.Exchange.UM.UMCommon.PhoneNumber>.Create(variableName, base.ManagerConfig);
				return;
			}
			this.phoneNumber = rootNode.Attributes["number"].Value;
		}

		private CallTransferType transferType;

		private string phoneNumber;

		private FsmVariable<PhoneNumber> fsmVariable;

		private string phoneNumberType;
	}
}
