using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.Dsn
{
	[Cmdlet("Get", "SystemMessage", DefaultParameterSetName = "Identity")]
	public sealed class GetSystemMessage : GetSystemConfigurationObjectTask<SystemMessageIdParameter, SystemMessage>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Original")]
		public SwitchParameter Original
		{
			get
			{
				return (SwitchParameter)(base.Fields["Original"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Original"] = value;
			}
		}

		private static SystemMessage[] GetAllResourceSystemMessages(ADObjectId orgContainer)
		{
			CultureInfo[] allSupportedDsnLanguages = ClientCultures.GetAllSupportedDsnLanguages();
			string[] array;
			LocalizedString[] array2;
			DsnDefaultMessages.GetAllDsnCodesAndMessages(out array, out array2);
			QuotaMessageType[] array3 = (QuotaMessageType[])Enum.GetValues(typeof(QuotaMessageType));
			SystemMessage[] array4 = new SystemMessage[(array.Length + array3.Length) * allSupportedDsnLanguages.Length];
			int num = 0;
			foreach (CultureInfo cultureInfo in allSupportedDsnLanguages)
			{
				ADObjectId dsnCustomizationContainer = SystemMessage.GetDsnCustomizationContainer(orgContainer);
				ADObjectId childId = dsnCustomizationContainer.GetChildId(cultureInfo.LCID.ToString(NumberFormatInfo.InvariantInfo));
				ADObjectId childId2 = childId.GetChildId("internal");
				for (int j = 0; j < array.Length; j++)
				{
					array4[num] = new SystemMessage();
					array4[num].SetId(childId2.GetChildId(array[j]));
					array4[num].Text = array2[j].ToString(cultureInfo);
					num++;
				}
				foreach (QuotaMessageType quotaMessageType in array3)
				{
					QuotaLocalizedTexts quotaLocalizedTexts = QuotaLocalizedTexts.GetQuotaLocalizedTexts(SetSystemMessage.ConvertToInternal(quotaMessageType), string.Empty, string.Empty, true);
					array4[num] = new SystemMessage();
					array4[num].SetId(childId.GetChildId(quotaMessageType.ToString()));
					array4[num].Text = quotaLocalizedTexts.Details.ToString(cultureInfo);
					num++;
				}
			}
			return array4;
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ADObjectId currentOrgContainerId = base.CurrentOrgContainerId;
				return SystemMessage.GetDsnCustomizationContainer(currentOrgContainerId);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (this.Original)
			{
				ADObjectId orgContainerId = (base.DataSession as IConfigurationSession).GetOrgContainerId();
				this.WriteResult<SystemMessage>(GetSystemMessage.GetAllResourceSystemMessages(orgContainerId));
				return;
			}
			base.InternalProcessRecord();
		}
	}
}
