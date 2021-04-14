﻿Type.registerNamespace("ECP");function CalendarNotificationSlabProperties(n){this.$$d_$Q_6=Function.createDelegate(this,this.$Q_6);CalendarNotificationSlabProperties.initializeBase(this,[n])}CalendarNotificationSlabProperties.prototype={initialize:function(){SmsSlabProperties.prototype.initialize.call(this);var n=this.getContentElementByServerId("lnkEditNotification");n.href="#";$addHandler(n,"click",this.onClickTurnOnButtonHandler);var t=$get("spnNextDays");var i=this.getContentElementByServerId("ddlCalendarUpdateNextDays");t.appendChild(i);$addHandler(t,"click",this.$$d_$Q_6)},$Q_6:function(n){n.preventDefault()},dispose:function(){$removeHandler($get("spnNextDays"),"click",this.$$d_$Q_6);this.onClickTurnOnButtonHandler&&$removeHandler(this.getContentElementByServerId("lnkEditNotification"),"click",this.onClickTurnOnButtonHandler);SmsSlabProperties.prototype.dispose.call(this)},showElements:function(){var i=this.get_Results().Output[0];var n=this.get_isNotificationEnabled();var t=this.get_isEasAccountEnabled();this.showElement("SmsNotificationNotSetUp",!n&&!t);this.showElement("lnkEditNotification",n&&!t);if(!n||t)WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("SmsNotificationEnabled"),!0);else{WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("SmsNotificationEnabled"),!1);this.$H_6()}},refreshObject:function(n){this.refresh()},$H_6:function(){WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("CalendarUpdateDuringWorkHour"),!this.getContentElementByServerId("chkCalendarUpdate").checked);WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("ddlCalendarUpdateNextDays"),!this.getContentElementByServerId("chkCalendarUpdate").checked);WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("MeetingReminderDuringWorkHour"),!this.getContentElementByServerId("chkMeetingReminder").checked);WizardPropertyUtil.toggleChildrenDisabled(this.getContentElementByServerId("DailyAgendaNotificationSendTime"),!this.getContentElementByServerId("chkDailyAgenda").checked)}};function EnterNumberStep(n){EnterNumberStep.initializeBase(this,[n])}EnterNumberStep.prototype={$B_7:null,getNextStep:function(n){var t=this.get_PropertyPage();if(this.$B_7!==t.get_numberInput().value){var i=this;WebServiceWizardStep.prototype.getNextStep.call(this,function(r){r===i.get_NextStepID()&&(i.$B_7=t.get_numberInput().value);n(r)})}else n(this.get_NextStepID())},show:function(){var n=this.get_PropertyPage();var t="+"+n.getRegionData(n.get_regionList().value).CountryCode;EcpUtil.setInnerText(n.get_countryCodeLabel(),t);n.get_countryCodeHiddenInput().value=t;n.get_passcodeInput().value="";FieldValidationAssistant.get_instance().setTextboxErrorProperties(n.get_numberInput(),null);FieldValidationAssistant.get_instance().setTextboxErrorProperties(n.get_passcodeInput(),null);WizardStepBase.prototype.show.call(this)},internalWebServiceMethodSucceeded:function(n,t){WebServiceWizardStep.prototype.internalWebServiceMethodSucceeded.call(this,n,t);this.get_PropertyPage().set_Results(t.get_results())}};function RegionCarrierStep(n){RegionCarrierStep.initializeBase(this,[n])}RegionCarrierStep.prototype={$E_6:null,get_Properties:function(){return this.$E_6},set_Properties:function(n){this.$E_6=n;return n},isFinalStep:function(){return!1}};function SmsCommunicationSlabProperties(n){this.$$d_onLayout=Function.createDelegate(this,this.onLayout);this.$$d_$R_6=Function.createDelegate(this,this.$R_6);this.$$d_$P_6=Function.createDelegate(this,this.$P_6);this.$$d_$T_6=Function.createDelegate(this,this.$T_6);this.$$d_$K_6=Function.createDelegate(this,this.$K_6);this.$$d_$M_6=Function.createDelegate(this,this.$M_6);this.$$d_$L_6=Function.createDelegate(this,this.$L_6);SmsCommunicationSlabProperties.initializeBase(this,[n])}SmsCommunicationSlabProperties.prototype={$4_6:null,get_RedirectionUrl:function(){return this.$4_6},set_RedirectionUrl:function(n){this.$4_6=n;return n},get_calendarLink:function(){return $get("lnkCalendar",this.get_element())},get_setupCalendarLink:function(){return $get("lnkSetupCalendar",this.get_element())},get_voiceMailLink:function(){return $get("lnkVoiceMail",this.get_element())},get_setupVoiceMailLink:function(){return $get("lnkSetupVoiceMail",this.get_element())},get_inboxRuleLink:function(){return $get("lnkInboxRule",this.get_element())},get_setupInboxRuleLink:function(){return $get("lnkSetupInboxRule",this.get_element())},get_hasOKButton:function(){return!!this.$4_6},initialize:function(){SmsSlabProperties.prototype.initialize.call(this);$addHandler(this.getContentElementByServerId("lnkChangeNumberSettings"),"click",this.$$d_$L_6);$addHandler(this.getContentElementByServerId("lnkChangePhoneSettings"),"click",this.$$d_$M_6);this.get_calendarLink()&&$addHandler(this.get_calendarLink(),"click",this.$$d_$K_6);this.get_setupCalendarLink()&&$addHandler(this.get_setupCalendarLink(),"click",this.$$d_$K_6);this.get_voiceMailLink()&&$addHandler(this.get_voiceMailLink(),"click",this.$$d_$T_6);this.get_setupVoiceMailLink()&&$addHandler(this.get_setupVoiceMailLink(),"click",this.$$d_$T_6);this.get_inboxRuleLink()&&$addHandler(this.get_inboxRuleLink(),"click",this.$$d_$P_6);this.get_setupInboxRuleLink()&&$addHandler(this.get_setupInboxRuleLink(),"click",this.$$d_$P_6);if(this.get_hasOKButton()){$addHandler(this.getContentElementByServerId("btnOK"),"click",this.$$d_$R_6);$addHandler(window,"load",this.$$d_onLayout);$addHandler(window,"resize",this.$$d_onLayout)}},dispose:function(){if(this.get_hasOKButton()){$removeHandler(this.getContentElementByServerId("btnOK"),"click",this.$$d_$R_6);$removeHandler(window,"load",this.$$d_onLayout);$removeHandler(window,"resize",this.$$d_onLayout)}this.get_setupInboxRuleLink()&&$removeHandler(this.get_setupInboxRuleLink(),"click",this.$$d_$P_6);this.get_inboxRuleLink()&&$removeHandler(this.get_inboxRuleLink(),"click",this.$$d_$P_6);this.get_setupVoiceMailLink()&&$removeHandler(this.get_setupVoiceMailLink(),"click",this.$$d_$T_6);this.get_voiceMailLink()&&$removeHandler(this.get_voiceMailLink(),"click",this.$$d_$T_6);this.get_setupCalendarLink()&&$removeHandler(this.get_setupCalendarLink(),"click",this.$$d_$K_6);this.get_calendarLink()&&$removeHandler(this.get_calendarLink(),"click",this.$$d_$K_6);$removeHandler(this.getContentElementByServerId("lnkChangeNumberSettings"),"click",this.$$d_$L_6);$removeHandler(this.getContentElementByServerId("lnkChangePhoneSettings"),"click",this.$$d_$M_6);SmsSlabProperties.prototype.dispose.call(this)},onLayout:function(n){if(this.get_hasOKButton()&&this.get_isEasAccountEnabled()){var t=this.get_element().parentNode.clientHeight-16;var i=this.getContentElementByServerId("PropertyPanel");var r=this.getContentElementByServerId("SeparatorPanel");var u=this.getContentElementByServerId("ButtonPanel");var f=Math.max(0,t-i.scrollHeight-u.scrollHeight);r.style.height=f+"px"}},$R_6:function(n){this.get_isEasAccountEnabled()&&PopupWindowManager.OpenWindow(this.$4_6,"_self","")},$K_6:function(n){this.shouldContinueToHandleEvent(n)&&this.openWizardDialog("CalendarNotification.slab?CAS=1")},$T_6:function(n){this.shouldContinueToHandleEvent(n)&&this.$D_6("../Customize/Voicemail.aspx","Voicemail")},$M_6:function(n){this.shouldContinueToHandleEvent(n)&&this.$D_6("../TroubleShooting/MobileDevices.slab","MobileDevices")},$P_6:function(n){this.shouldContinueToHandleEvent(n)&&this.openWizardDialog("../RulesEditor/NewInboxRule.aspx?tplNames=SendTextMessageNotificationTo")},$D_6:function(n,t){this.get_hasOKButton()?this.openWizardDialog(n):JumpTo(t)},$L_6:function(n){this.shouldContinueToHandleEvent(n)&&this.openWizardDialog("EditNotification.aspx")},refreshObject:function(n){this.refresh()},showElements:function(){if(this.get_Results()&&this.get_Results().Output){var n=this.get_Results().Output[0];this.showElement("lnkChangeNumberSettings",!this.get_isEasAccountEnabled()&&this.get_isNotificationEnabled());this.showElement("lnkChangePhoneSettings",this.get_isEasAccountEnabled());this.showElement("btnTurnOn",!this.get_isEasAccountEnabled()&&!this.get_isNotificationEnabled());this.showElement("btnTurnOff",!this.get_isEasAccountEnabled()&&this.get_isNotificationEnabled());this.showElement("lblNotifDesc",!this.get_isEasAccountEnabled()&&!this.get_isNotificationEnabled());this.get_calendarLink()&&EcpUtil.showElement(this.get_calendarLink(),!this.get_isEasAccountEnabled()&&!this.get_isNotificationEnabled());this.get_setupCalendarLink()&&EcpUtil.showElement(this.get_setupCalendarLink(),!this.get_isEasAccountEnabled()&&this.get_isNotificationEnabled());this.get_voiceMailLink()&&EcpUtil.showElement(this.get_voiceMailLink(),!this.get_isEasAccountEnabled()&&!this.get_isNotificationEnabled());this.get_setupVoiceMailLink()&&EcpUtil.showElement(this.get_setupVoiceMailLink(),!this.get_isEasAccountEnabled()&&this.get_isNotificationEnabled());this.get_inboxRuleLink()&&EcpUtil.showElement(this.get_inboxRuleLink(),!this.get_isEasAccountEnabled()&&!this.get_isNotificationEnabled());this.get_setupInboxRuleLink()&&EcpUtil.showElement(this.get_setupInboxRuleLink(),!this.get_isEasAccountEnabled()&&this.get_isNotificationEnabled());if(this.get_hasOKButton()){this.showElement("ButtonPanel",this.get_isEasAccountEnabled());this.showElement("btnOK",this.get_isEasAccountEnabled());this.onLayout(null)}}}};function SmsOptions(){}SmsOptions.prototype={EasEnabled:!1,NotificationEnabled:!1,NotificationPhoneNumber:null,CountryRegionId:null,MobileOperatorId:null,Description:null,StatusPrefix:null,StatusDetails:null};function TextMessagingData(){}TextMessagingData.prototype={ID:null,Name:null};function RegionData(){RegionData.initializeBase(this)}RegionData.prototype={CountryCode:null,CarrierIDs:null};function CarrierData(){CarrierData.initializeBase(this)}CarrierData.prototype={HasSmtpGateway:!1,UnifiedMessagingInfo:null};function SmsServiceProviders(){}SmsServiceProviders.prototype={RegionList:null,CarrierDictionary:null};function UnifiedMessagingInfo(){}UnifiedMessagingInfo.prototype={EnableTemplate:null,DisableTemplate:null,CallForwardingType:null};function SmsSlabProperties(n){this.$$d_$C_5=Function.createDelegate(this,this.$C_5);this.$$d_$O_5=Function.createDelegate(this,this.$O_5);this.$$d_$N_5=Function.createDelegate(this,this.$N_5);SmsSlabProperties.initializeBase(this,[n])}SmsSlabProperties.prototype={get_DisableWebServiceMethod:function(){return this.$1_5},set_DisableWebServiceMethod:function(n){this.$1_5&&this.$1_5.remove_Succeeded(this.webServiceMethod_Succeeded);this.$1_5=n;this.$1_5&&this.$1_5.add_Succeeded(this.webServiceMethod_Succeeded);return n},$1_5:null,get_EditSettingPage:function(){return this.$5_5},set_EditSettingPage:function(n){this.$5_5=n;return n},$5_5:null,get_result:function(){return this.get_Results().Output[0]},get_isEasAccountEnabled:function(){return this.get_result().EasEnabled},get_isNotificationEnabled:function(){return this.get_result().NotificationEnabled},onClickTurnOnButtonHandler:null,onClickTurnOffButtonHandler:null,initialize:function(){PropertyPage.prototype.initialize.call(this);var n=[this.get_identityBinding()];if(this.$1_5){this.$1_5.set_Parameters(n);this.onClickTurnOffButtonHandler=this.$$d_$N_5;$addHandler(this.getContentElementByServerId("btnTurnOff"),"click",this.onClickTurnOffButtonHandler)}this.onClickTurnOnButtonHandler=this.$$d_$O_5;$addHandler(this.getContentElementByServerId("btnTurnOn"),"click",this.onClickTurnOnButtonHandler);this.add_propertyChanged(this.$$d_$C_5);this.showElements()},dispose:function(){if(this.onClickTurnOnButtonHandler){$removeHandler(this.getContentElementByServerId("btnTurnOn"),"click",this.onClickTurnOnButtonHandler);this.onClickTurnOnButtonHandler=null}if(this.$1_5&&this.onClickTurnOffButtonHandler){$removeHandler(this.getContentElementByServerId("btnTurnOff"),"click",this.onClickTurnOffButtonHandler);this.onClickTurnOffButtonHandler=null}PropertyPage.prototype.dispose.call(this)},shouldContinueToHandleEvent:function(n){n.preventDefault();return!n.target.disabled},$O_5:function(n){this.shouldContinueToHandleEvent(n)&&this.openWizardDialog(this.$5_5)},openWizardDialog:function(n){PopupWindowManager.showPropertiesDialog(n,this,BuildCenteredWindowFeatureString(450,700,GlobalVariables.FeaturesForPopups),null)},$I_5:0,$N_5:function(n){if(this.shouldContinueToHandleEvent(n)){var t=new Info;t.Message=OwaOptionStrings.get_ClearOmsAccountPrompt();t.MessageType=MessageBoxType.warning;var i=this;MessageBox.show(t,2,function(n,t){n===ModalDialogResult.yes&&i.$1_5.invoke(++i.$I_5)},null)}},$C_5:function(n,t){t.get_propertyName()==="Results"&&this.showElements()},showElements:function(){},showElement:function(n,t){EcpUtil.showElement(this.getContentElementByServerId(n),t)}};function SmsWizardProperties(n){this.$$d_$C_5=Function.createDelegate(this,this.$C_5);this.$$d_$S_5=Function.createDelegate(this,this.$S_5);this.$$d_onOperatorChange=Function.createDelegate(this,this.onOperatorChange);this.$$d_onRegionChange=Function.createDelegate(this,this.onRegionChange);this.$$d_$U_5=Function.createDelegate(this,this.$U_5);SmsWizardProperties.initializeBase(this,[n])}SmsWizardProperties.prototype={$2_5:null,$A_5:!1,$0_5:null,$J_5:0,get_SendVerificationCodeWebServiceMethod:function(){return this.$0_5},set_SendVerificationCodeWebServiceMethod:function(n){if(this.$0_5){this.$0_5.remove_Succeeded(this.webServiceMethod_Succeeded);this.$0_5.remove_Succeeded(this.$$d_$U_5)}this.$0_5=n;if(this.$0_5){this.$0_5.add_Succeeded(this.webServiceMethod_Succeeded);this.$0_5.add_Succeeded(this.$$d_$U_5)}return n},get_ServiceProviders:function(){return this.$2_5},set_ServiceProviders:function(n){this.$2_5=n;return n},get_IsNotificationWizard:function(){return this.$A_5},set_IsNotificationWizard:function(n){this.$A_5=n;return n},get_regionList:function(){return this.getContentElementByServerId("RegionList")},get_operatorList:function(){return this.getContentElementByServerId("OperatorList")},get_regionIDHiddenInput:function(){return this.getContentElementByServerId("txtRegion")},get_numberInput:function(){return this.getContentElementByServerId("txtNumber")},get_passcodeInput:function(){return this.getContentElementByServerId("txtPasscode")},get_sendPasscodeLink:function(){return this.getContentElementByServerId("lnkSendPasscode")},get_countryCodeHiddenInput:function(){return this.getContentElementByServerId("txtCountryCode")},get_countryCodeLabel:function(){return this.getContentElementByServerId("lblCountryCode")},getRegionData:function(n){for(var i=this.$2_5.RegionList,t=0;t<i.length;t++)if(i[t].ID===n)return i[t];return null},getCarrierData:function(n){return this.$2_5.CarrierDictionary[n]},initialize:function(){PropertyPage.prototype.initialize.call(this);this.$0_5&&this.$0_5.set_Parameters([this.get_identityBinding(),new ChangedValuesBinding(this.get_Bindings())]);for(var n=this.get_regionList(),i=this.get_operatorList(),r=this.$2_5.RegionList,t=0;t<r.length;t++)this.$7_5(n,RtlUtil.convertToDecodedBidiString(r[t].Name),r[t].ID);this.$8_5(this.get_operatorList());if(this.get_Results()&&this.get_Results().Output.length>0){this.$F_5(n,"txtRegion",this.$$d_onRegionChange);this.$F_5(i,"txtOperator",this.$$d_onOperatorChange);this.$3_5(n,"txtRegion");this.$3_5(i,"txtOperator")}$addHandler(n,"change",this.$$d_onRegionChange);$addHandler(i,"change",this.$$d_onOperatorChange);this.get_sendPasscodeLink()&&$addHandler(this.get_sendPasscodeLink(),"click",this.$$d_$S_5);this.add_propertyChanged(this.$$d_$C_5)},dispose:function(){$removeHandler(this.get_regionList(),"change",this.$$d_onRegionChange);$removeHandler(this.get_operatorList(),"change",this.$$d_onOperatorChange);this.get_sendPasscodeLink()&&$removeHandler(this.get_sendPasscodeLink(),"click",this.$$d_$S_5);PropertyPage.prototype.dispose.call(this)},onRegionChange:function(n){var t=this.get_regionList();n&&this.$3_5(t,"txtRegion");this.$G_5(t,this.get_operatorList(),t.selectedIndex>0?this.getRegionData(t.value).CarrierIDs:null,this.$2_5.CarrierDictionary);this.onOperatorChange(n)},onOperatorChange:function(n){var t=this.get_operatorList();if(n){this.$3_5(t,"txtOperator");this.get_numberInput()&&(this.get_numberInput().value="")}},$S_5:function(n){this.$0_5.invoke(++this.$J_5)},$F_5:function(n,t,i){var u=this.getContentElementByServerId(t);if(u.value.length>0)for(var r=0;r<n.options.length;r++){var f=n.options[r];if(f.value===u.value){n.selectedIndex=r;i(null);break}}},$3_5:function(n,t){var i=this.getContentElementByServerId(t);i.value=n.selectedIndex>0?n.value:""},$G_5:function(n,t,i,r){this.$8_5(t);if(i){t.disabled=!1;this.$9_5(t.parentNode,!1);for(var u=0;u<i.length;u++){var f=i[u];this.$7_5(t,r[f].Name,f)}i.length===1&&(t.selectedIndex=1)}t.selectedIndex>0&&FieldValidationAssistant.hasErrorMessage(t)&&FieldValidationAssistant.get_instance().setTextboxErrorProperties(t,null)},$9_5:function(n,t){t?n.className.endsWith("disabled")||(n.className+=" disabled"):n.className.endsWith("disabled")&&(n.className=n.className.replace("disabled",""))},$7_5:function(n,t,i){var r=document.createElement("option");r.text=t;r.value=i;EcpUtil.appendOption(n,r)},$8_5:function(n){for(var t=n.options.length-1;t>0;t--)n.remove(t);n.disabled=!0;this.$9_5(n.parentNode,!0)},$U_5:function(n,t){var i=t.get_results();(i.ErrorRecords&&i.ErrorRecords.length||i.Warnings)&&i.Warnings.length||MessageBox.showMessage(OwaOptionStrings.SendPasscodeSucceededFormat(this.get_numberInput().value),MessageBoxType.information)},$C_5:function(n,t){if(t.get_propertyName()==="Results"){this.get_passcodeInput().value="";this.get_Bindings().resetIsDirty();var i=_Window.getOpener();if(i){PopupWindowManager.tryRefreshOpener();PopupWindowManager.tryRefreshOpener(i,null)}}}};SmsSlabProperties.registerClass("SmsSlabProperties",PropertyPage);CalendarNotificationSlabProperties.registerClass("CalendarNotificationSlabProperties",SmsSlabProperties);EnterNumberStep.registerClass("EnterNumberStep",WebServiceWizardStep);RegionCarrierStep.registerClass("RegionCarrierStep",WizardStep);SmsCommunicationSlabProperties.registerClass("SmsCommunicationSlabProperties",SmsSlabProperties);SmsOptions.registerClass("SmsOptions");TextMessagingData.registerClass("TextMessagingData");RegionData.registerClass("RegionData",TextMessagingData);CarrierData.registerClass("CarrierData",TextMessagingData);SmsServiceProviders.registerClass("SmsServiceProviders");UnifiedMessagingInfo.registerClass("UnifiedMessagingInfo");SmsWizardProperties.registerClass("SmsWizardProperties",PropertyPage)