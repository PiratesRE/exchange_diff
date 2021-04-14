using System;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Sharing;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoEmailSubjectProperty : XsoStringProperty
	{
		public XsoEmailSubjectProperty(PropertyType propertyType = PropertyType.ReadOnly) : base(ItemSchema.Subject, propertyType)
		{
		}

		public override string StringData
		{
			get
			{
				string result = base.StringData;
				if (this.IsItemDelegated())
				{
					MeetingMessage meetingMessage = base.XsoItem as MeetingMessage;
					Participant receivedRepresenting = meetingMessage.ReceivedRepresenting;
					if (receivedRepresenting != null)
					{
						try
						{
							using (CalendarItemBase correlatedItem = meetingMessage.GetCorrelatedItem())
							{
								if (correlatedItem != null && correlatedItem.IsCancelled)
								{
									result = this.BuildDelegatedSubjectLine(Strings.CanceledDelegatedSubjectPrefix(receivedRepresenting.DisplayName).ToString(Command.CurrentCommand.MailboxSession.PreferedCulture), base.StringData);
								}
								else
								{
									result = this.BuildDelegatedSubjectLine(Strings.DelegatedSubjectPrefix(receivedRepresenting.DisplayName).ToString(Command.CurrentCommand.MailboxSession.PreferedCulture), base.StringData);
								}
							}
						}
						catch (StoragePermanentException)
						{
							result = this.BuildDelegatedSubjectLine(Strings.DelegatedSubjectPrefix(receivedRepresenting.DisplayName).ToString(Command.CurrentCommand.MailboxSession.PreferedCulture), base.StringData);
						}
						catch (StorageTransientException)
						{
							result = this.BuildDelegatedSubjectLine(Strings.DelegatedSubjectPrefix(receivedRepresenting.DisplayName).ToString(Command.CurrentCommand.MailboxSession.PreferedCulture), base.StringData);
						}
						catch (ADUserNotFoundException)
						{
							result = this.BuildDelegatedSubjectLine(Strings.DelegatedSubjectPrefix(receivedRepresenting.DisplayName).ToString(Command.CurrentCommand.MailboxSession.PreferedCulture), base.StringData);
						}
					}
				}
				return result;
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			if (Command.CurrentCommand != null && Command.CurrentCommand.Request.Version >= 160)
			{
				base.XsoItem[base.PropertyDef] = string.Empty;
				return;
			}
			base.InternalSetToDefault(srcProperty);
		}

		private string BuildDelegatedSubjectLine(string prefix, string subject)
		{
			StringBuilder stringBuilder = new StringBuilder(prefix);
			stringBuilder.Append(" ");
			stringBuilder.Append(subject);
			return stringBuilder.ToString();
		}
	}
}
