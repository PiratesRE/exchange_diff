using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal abstract class SubscriptionSettingsBase : ISubscriptionSettings, IMigrationSerializable
	{
		public ExDateTime LastModifiedTime { get; protected set; }

		public abstract PropertyDefinition[] PropertyDefinitions { get; }

		protected virtual bool IsEmpty
		{
			get
			{
				return this.LastModifiedTime == ExDateTime.MinValue;
			}
		}

		public virtual void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationSubscriptionSettingsLastModifiedTime, (this.LastModifiedTime == ExDateTime.MinValue) ? null : new ExDateTime?(this.LastModifiedTime));
		}

		public virtual bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			ExDateTime? exDateTimePropertyOrNull = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationSubscriptionSettingsLastModifiedTime);
			if (exDateTimePropertyOrNull == null)
			{
				this.LastModifiedTime = ExDateTime.MinValue;
			}
			else
			{
				this.LastModifiedTime = exDateTimePropertyOrNull.Value;
			}
			return !this.IsEmpty;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("SubscriptionSettings");
			if (!argument.HasArgument("verbose"))
			{
				return xelement;
			}
			xelement.Add(new XElement("LastModifiedTime", this.LastModifiedTime));
			this.AddDiagnosticInfoToElement(dataProvider, xelement, argument);
			return xelement;
		}

		protected abstract void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument);

		internal static readonly PropertyDefinition[] SubscriptionSettingsBasePropertyDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationSubscriptionSettingsLastModifiedTime
		};
	}
}
