using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class MembersProperty : ComplexPropertyBase, IPregatherParticipants, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IAppendUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public MembersProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		protected static bool? RenderMembersCollection
		{
			get
			{
				return MembersProperty.renderMembersCollection;
			}
			set
			{
				MembersProperty.renderMembersCollection = value;
			}
		}

		public static MembersProperty CreateMembersCommand(CommandContext commandContext)
		{
			MembersProperty result = new MembersProperty(commandContext);
			MembersProperty.renderMembersCollection = new bool?(true);
			return result;
		}

		void IPregatherParticipants.Pregather(StoreObject storeObject, List<Participant> participants)
		{
			DistributionList distributionList = (DistributionList)storeObject;
			if (distributionList.Count > 0)
			{
				for (int i = 0; i < distributionList.Count; i++)
				{
					participants.Add(distributionList[i].Participant);
				}
			}
		}

		public virtual void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			DistributionListType distributionListType = commandSettings.ServiceObject as DistributionListType;
			DistributionList distributionList = (DistributionList)commandSettings.StoreObject;
			if (distributionList.Count > 0)
			{
				bool flag = true;
				foreach (DistributionListMember distributionListMember in distributionList)
				{
					if (distributionListMember.Participant != null)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					List<MemberType> list = new List<MemberType>();
					for (int i = 0; i < distributionList.Count; i++)
					{
						if (distributionList[i].Participant != null)
						{
							MemberType item = this.MemberToServiceObject(distributionList, i);
							list.Add(item);
						}
					}
					distributionListType.Members = list.ToArray();
				}
			}
			MembersProperty.RenderMembersCollection = null;
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(serviceObject, storeObject, false);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			this.SetProperty(serviceObject, storeObject, false);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			DistributionList distributionList = (DistributionList)storeObject;
			distributionList.Clear();
		}

		public override void AppendUpdate(AppendPropertyUpdate appendPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			ServiceObject serviceObject = appendPropertyUpdate.ServiceObject;
			this.SetProperty(serviceObject, storeObject, true);
		}

		protected static int FindMemberIndex(DistributionList distributionList, string memberKey)
		{
			for (int i = 0; i < distributionList.Count; i++)
			{
				if (distributionList[i] != null)
				{
					ParticipantEntryId mainEntryId = distributionList[i].MainEntryId;
					string text = MembersProperty.MemberKeyToString(mainEntryId.ToByteArray());
					if (text != null && text == memberKey)
					{
						return i;
					}
				}
			}
			return -1;
		}

		protected MemberType MemberToServiceObject(DistributionList distributionList, int i)
		{
			MemberType memberType = new MemberType();
			DistributionListMember distributionListMember = distributionList[i];
			ParticipantInformation participant = EWSSettings.ParticipantInformation.GetParticipant(distributionListMember.Participant);
			memberType.Key = MembersProperty.MemberKeyToString(distributionListMember.MainEntryId.ToByteArray());
			memberType.StatusString = ((participant.Demoted != null && participant.Demoted.Value) ? MemberStatus.Demoted.ToString() : distributionListMember.MemberStatus.ToString());
			memberType.Mailbox = base.CreateRecipientFromParticipant(participant, distributionList).Mailbox;
			return memberType;
		}

		protected void SetProperty(ServiceObject serviceObject, StoreObject storeObject, bool appendMode)
		{
			DistributionList distributionList = (DistributionList)storeObject;
			if (!appendMode)
			{
				distributionList.Clear();
			}
			MemberType[] valueOrDefault = serviceObject.GetValueOrDefault<MemberType[]>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				for (int i = 0; i < valueOrDefault.Length; i++)
				{
					MemberType memberType = valueOrDefault[i];
					if (memberType.Mailbox != null)
					{
						try
						{
							Participant participant2;
							Participant participant = MailboxHelper.ParseMailbox(this.commandContext.PropertyInformation.PropertyPath, distributionList, memberType.Mailbox, this.commandContext.IdConverter, out participant2, false);
							this.AddMember(distributionList, participant);
						}
						catch (ServicePermanentExceptionWithPropertyPath servicePermanentExceptionWithPropertyPath)
						{
							servicePermanentExceptionWithPropertyPath.ConstantValues.Add("MemberIndex", i.ToString());
							throw servicePermanentExceptionWithPropertyPath;
						}
						catch (LocalizedException innerException)
						{
							throw new InvalidMailboxException(i, this.commandContext.PropertyInformation.PropertyPath, innerException, CoreResources.IDs.ErrorInvalidMailbox);
						}
					}
				}
			}
		}

		protected void AddMember(DistributionList distributionList, Participant participant)
		{
			int count = distributionList.Count;
			distributionList.Add(participant);
			ParticipantEntryId mainEntryId = distributionList[count].MainEntryId;
			string memberKey = MembersProperty.MemberKeyToString(mainEntryId.ToByteArray());
			int num = MembersProperty.FindMemberIndex(distributionList, memberKey);
			if (num < count)
			{
				distributionList.RemoveAt(count);
			}
		}

		private static string MemberKeyToString(byte[] memberKey)
		{
			return Convert.ToBase64String(memberKey);
		}

		public virtual void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			DistributionList distributionList = (DistributionList)commandSettings.StoreObject;
			if (distributionList.Count > 0)
			{
				bool flag = true;
				foreach (DistributionListMember distributionListMember in distributionList)
				{
					if (distributionListMember.Participant != null)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					XmlElement xmlParent = base.CreateXmlElement(commandSettings.ServiceItem, "Members");
					for (int i = 0; i < distributionList.Count; i++)
					{
						if (distributionList[i].Participant != null)
						{
							this.MemberToXml(distributionList, i, xmlParent);
						}
					}
				}
			}
			MembersProperty.RenderMembersCollection = null;
		}

		protected void MemberToXml(DistributionList distributionList, int index, XmlElement xmlParent)
		{
			DistributionListMember distributionListMember = distributionList[index];
			XmlElement parentElement = base.CreateXmlElement(xmlParent, "Member");
			ParticipantEntryId mainEntryId = distributionListMember.MainEntryId;
			string attributeValue = MembersProperty.MemberKeyToString(mainEntryId.ToByteArray());
			PropertyCommand.CreateXmlAttribute(parentElement, "Key", attributeValue);
			ParticipantInformation participant = EWSSettings.ParticipantInformation.GetParticipant(distributionListMember.Participant);
			base.CreateParticipantOrDLXml(parentElement, participant, distributionList);
			string textValue = (participant.Demoted != null && participant.Demoted.Value) ? MemberStatus.Demoted.ToString() : distributionListMember.MemberStatus.ToString();
			base.CreateXmlTextElement(parentElement, "Status", textValue);
		}

		protected void SetProperty(XmlElement xmlMembers, StoreObject storeObject, bool appendMode)
		{
			throw new InvalidOperationException("MembersProperty.SetProperty for XML should not be called.");
		}

		protected const string XmlElementNameMembers = "Members";

		private const string XmlElementNameMember = "Member";

		private const string XmlAttributeNameKey = "Key";

		private const string XmlElementNameMemberStatus = "Status";

		private const string XmlElementNameMailbox = "Mailbox";

		[ThreadStatic]
		private static bool? renderMembersCollection = null;
	}
}
