﻿Type.registerNamespace("ECP");function DateChooser(n){this.$$d_$T_4=Function.createDelegate(this,this.$T_4);DateChooser.initializeBase(this,[n])}DateChooser.prototype={$2_4:null,$1_4:null,$0_4:null,$3_4:null,$W_4:null,initialize:function(){EcpControl.prototype.initialize.call(this);this.$2_4&&$addHandler(this.$2_4,"change",this.$$d_$T_4);this.$1_4&&$addHandler(this.$1_4,"change",this.$$d_$T_4);this.$T_4(null)},$T_4:function(n){this.$b_4();this.$0_4.disabled||this.$g_4()},$b_4:function(){this.$1_4.disabled=this.$2_4.disabled||this.$2_4.value==="0";this.$0_4.disabled=this.$2_4.disabled||this.$2_4.value==="0";if(!this.$1_4.disabled){if(this.$0_4.options[0].value==="0"){this.$0_4.remove(0);this.$0_4.value==="0"&&(this.$0_4.value="1")}if(this.$1_4.options[0].value==="0"){this.$1_4.remove(0);this.$1_4.value==="0"&&(this.$1_4.value="1")}}},$e_4:function(n){var t=!1;n%4||!(n%100)&&n%400||(t=!0);return t},$g_4:function(){var f=parseInt(this.$2_4.value);var e=parseInt(this.$1_4.value);var n=0;switch(e){case 0:n=0;break;case 2:n=this.$e_4(f)?29:28;break;case 4:case 6:case 9:case 11:n=30;break;default:n=31;break}var i=this.$0_4.options.length;if(i>n)for(var r=i;r>n;r--)this.$0_4.remove(r-1);else for(var t=i+1;t<=n;t++){var u=document.createElement("OPTION");u.value=t.toString();u.text=t.toString();EcpUtil.appendOption(this.$0_4,u)}},get_$Z_4:function(){var n=!1;this.$2_4&&this.$1_4&&this.$0_4&&(n=this.$2_4.selectedIndex>0&&!!parseInt(this.$0_4.value)&&!!parseInt(this.$1_4.value));return n},get_UserDateFormat:function(){return this.$W_4},set_UserDateFormat:function(n){this.$W_4=n;return n},get_rawValue:function(){return this.get_$Z_4()?new Date(parseInt(this.$2_4.value),parseInt(this.$1_4.value)-1,parseInt(this.$0_4.value)):null},get_value:function(){var n=this.get_rawValue();return n?n.format("yyyy/MM/dd"):null},set_value:function(n){if(this.get_value()!==n){this.$3_4=n;if(this.$3_4){var i=Date.parse(this.$3_4);var t=new Date(i);if(t){this.$2_4&&EcpUtil.selectOption(this.$2_4,t.getFullYear());this.$1_4&&(this.$1_4.value=(t.getMonth()+1).toString());this.$0_4&&(this.$0_4.value=t.getDate().toString())}}else{this.$2_4&&(this.$2_4.selectedIndex=0);this.$1_4&&(this.$1_4.selectedIndex=0);this.$0_4&&(this.$0_4.selectedIndex=0)}this.$T_4(null)}return n},get_valueDisplay:function(){return this.get_rawValue()?this.get_rawValue().format(this.$W_4||_String.empty):null},addAriaAttribute:function(){this.$2_4.setAttribute(AriaUtil.ariaRole,AriaUtil.ariaCombobox);this.$1_4.setAttribute(AriaUtil.ariaRole,AriaUtil.ariaCombobox);this.$0_4.setAttribute(AriaUtil.ariaRole,AriaUtil.ariaCombobox);var f=$get(this.get_element().id+AriaUtil.associatedLabelSuffix);var n=EcpUtil.getInnerText(f);var t=$get(this.$2_4.id+AriaUtil.associatedLabelSuffix);var i=$get(this.$1_4.id+AriaUtil.associatedLabelSuffix);var r=$get(this.$0_4.id+AriaUtil.associatedLabelSuffix);var e=Strings.DateChooserListName(EcpUtil.getInnerText(t),n);var o=Strings.DateChooserListName(EcpUtil.getInnerText(i),n);var s=Strings.DateChooserListName(EcpUtil.getInnerText(r),n);EcpUtil.setInnerText(t,e);EcpUtil.setInnerText(i,o);EcpUtil.setInnerText(r,s);var u=AriaUtil.getFvaReaderSpanId(this.get_element());this.$2_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(t.id,u));this.$1_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(i.id,u));this.$0_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(r.id,u))},get_DdlYear:function(){return this.$2_4},set_DdlYear:function(n){this.$2_4=n;return n},get_DdlMonth:function(){return this.$1_4},set_DdlMonth:function(n){this.$1_4=n;return n},get_DdlDay:function(){return this.$0_4},set_DdlDay:function(n){this.$0_4=n;return n},dispose:function(){$clearHandlers(this.get_element());EcpControl.prototype.dispose.call(this)},setEnabled:function(n){if(n){this.$2_4&&(this.$2_4.disabled=!1);this.$b_4()}else{this.$2_4&&(this.$2_4.disabled=!0);this.$1_4&&(this.$1_4.disabled=!0);this.$0_4&&(this.$0_4.disabled=!0)}}};function DaysOfWeekSelector(n){DaysOfWeekSelector.initializeBase(this,[n])}DaysOfWeekSelector.prototype={$A_4:null,$8_4:null,$C_4:null,$D_4:null,$B_4:null,$7_4:null,$9_4:null,$3_4:0,initialize:function(){EcpControl.prototype.initialize.call(this);for(var n=EcpUtil.findChildrenMatchTag(this.get_element(),"DIV"),u=0,t=n.length,i=0;i<t;i++)n[i].offsetWidth>u&&(u=n[i].offsetWidth);for(var r=0;r<t;r++)n[r].style.minWidth=EcpUtil.pX2EM(n[r],u)+"em";EcpUtil.isIE()?n[t-1].style.styleFloat="none":n[t-1].style.cssFloat="none"},addAriaAttribute:function(){EcpControl.prototype.addAriaAttribute.call(this);var n=AriaUtil.findAssociatedLabel(this.get_element(),!0);this.$8_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$8_4.id));this.$C_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$C_4.id));this.$D_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$D_4.id));this.$B_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$B_4.id));this.$7_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$7_4.id));this.$9_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$9_4.id));this.$A_4.setAttribute(AriaUtil.ariaLabelledby,AriaUtil.joinIDs(n.id,this.$A_4.id))},$d_4:function(){var n=0;var t=this.$A_4.checked?1:0;var i=this.$8_4.checked?1:0;var r=this.$C_4.checked?1:0;var u=this.$D_4.checked?1:0;var f=this.$B_4.checked?1:0;var e=this.$7_4.checked?1:0;var o=this.$9_4.checked?1:0;n=t+i*2+r*4+u*8+f*16+e*32+o*64;return n},get_value:function(){return this.$d_4()},set_value:function(n){if(this.get_value()!==n){this.$3_4=n;this.$A_4.checked=(this.$3_4&1)>0;this.$8_4.checked=(this.$3_4&2)>0;this.$C_4.checked=(this.$3_4&4)>0;this.$D_4.checked=(this.$3_4&8)>0;this.$B_4.checked=(this.$3_4&16)>0;this.$7_4.checked=(this.$3_4&32)>0;this.$9_4.checked=(this.$3_4&64)>0}return n},get_ChkSunday:function(){return this.$A_4},set_ChkSunday:function(n){this.$A_4=n;return n},get_ChkMonday:function(){return this.$8_4},set_ChkMonday:function(n){this.$8_4=n;return n},get_ChkTuesday:function(){return this.$C_4},set_ChkTuesday:function(n){this.$C_4=n;return n},get_ChkWednesday:function(){return this.$D_4},set_ChkWednesday:function(n){this.$D_4=n;return n},get_ChkThursday:function(){return this.$B_4},set_ChkThursday:function(n){this.$B_4=n;return n},get_ChkFriday:function(){return this.$7_4},set_ChkFriday:function(n){this.$7_4=n;return n},get_ChkSaturday:function(){return this.$9_4},set_ChkSaturday:function(n){this.$9_4=n;return n}};function MailboxSearchUtil(){}MailboxSearchUtil.StartFullStatsMailboxSearchHandler=function(n,t,i,r){var o=$get(n);o&&(o.style.display="none");var s=$get(t);s&&EcpUtil.setInnerText(s,Strings.get_RetrievingStatistics());var u=new WebServiceMethod;u.set_MethodName("SetObject");u.set_ServiceUrl(EcpUrl.get_ecpFullUrl()+"Reporting/MailboxSearches.svc");u.set_ParameterNames(4);var h=new Identity(i,r);var f={};f.IncludeKeywordStatistics=!0;var e=[h,f];u.invokeWithArgs(e,null);u=new WebServiceMethod;u.set_MethodName("StartSearch");u.set_ServiceUrl(EcpUrl.get_ecpFullUrl()+"Reporting/MailboxSearches.svc");u.set_ParameterNames(2);u.add_Succeeded(MailboxSearchUtil.$Y);f={};var c=[h];e=[c,f];u.invokeWithArgs(e,null)};MailboxSearchUtil.KeywordStatisticsPaginationSearch=function(n,t,i,r,u,f){var o=$get(n);o&&(o.style.display="none");var s=$get(t);s&&(s.style.display="none");var h=$get(i);h&&EcpUtil.setInnerText(h,Strings.get_RetrievingStatistics());var e=new WebServiceMethod;e.set_MethodName("StartSearch");e.set_ServiceUrl(EcpUrl.get_ecpFullUrl()+"Reporting/MailboxSearches.svc");e.set_ParameterNames(2);e.add_Succeeded(MailboxSearchUtil.$Y);var l=new Identity(r,u);var c={};c.StatisticsStartIndex=f;var a=[l];var v=[a,c];e.invokeWithArgs(v,null)};MailboxSearchUtil.$Y=function(n,t){var i=t.get_results();i.ErrorRecords&&i.ErrorRecords.length>0||i.Warnings&&i.Warnings.length>0?ErrorHandling.showPowerShellErrorsWithCallback(i,function(n,t){RefreshWindow(window.self)}):window.setTimeout(function(){RefreshWindow(window.self)},2e4)};function EditDiscoveryHoldViewModel(){this.$$d_$c_4=Function.createDelegate(this,this.$c_4);this.$$d_saveCommandPreCheck=Function.createDelegate(this,this.saveCommandPreCheck);EditDiscoveryHoldViewModel.initializeBase(this)}EditDiscoveryHoldViewModel.prototype={$J_4:!0,$L_4:!0,$K_4:!0,$M_4:!0,$N_4:!0,$I_4:!0,$H_4:!0,$E_4:!1,$6_4:!1,$5_4:!1,$F_4:!1,$G_4:!1,get_InPlaceHoldEnabled:function(){return this.getValue("InPlaceHoldEnabled")},set_InPlaceHoldEnabled:function(n){if(n!==this.getValue("InPlaceHoldEnabled")){this.setValue("InPlaceHoldEnabled",n);this.set_HoldPeriodSectionEnabled(n);this.set_SelectAllMailboxesEnabled(!n);this.set_SelectAllPublicFoldersEnabled(!n)}return n},get_InPlaceHoldCheckBoxEnabled:function(){return this.$H_4},set_InPlaceHoldCheckBoxEnabled:function(n){this.$H_4=this.get_IsFlightingEnabled()?this.get_SearchNoPublicFolders()&&!this.get_SearchNoMailboxes()&&!this.get_SearchAllMailboxes():!this.get_SearchAllMailboxes();this.raisePropertyChanged("InPlaceHoldCheckBoxEnabled");return n},get_SearchMailboxesSelection:function(){return this.getValue("SearchMailboxesSelection")},set_SearchMailboxesSelection:function(n){this.setValue("SearchMailboxesSelection",n);if(n==="All"){this.set_SearchAllMailboxes(!0);this.set_SearchNoMailboxes(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}if(n==="Select"){this.set_SearchAllMailboxes(!1);this.set_SearchNoMailboxes(!1);this.set_InPlaceHoldCheckBoxEnabled(!0)}return n},get_SearchPublicFoldersSelection:function(){return this.getValue("SearchPublicFoldersSelection")},set_SearchPublicFoldersSelection:function(n){this.setValue("SearchPublicFoldersSelection",n);if(n==="All"){this.set_SearchAllPublicFolders(!0);this.set_SearchNoPublicFolders(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}if(n==="Select"){this.set_SearchAllPublicFolders(!1);this.set_SearchNoPublicFolders(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}return n},get_SearchAllMailboxes:function(){return this.getValue("SearchAllMailboxes")},set_SearchAllMailboxes:function(n){n!==this.getValue("SearchAllMailboxes")&&this.setValue("SearchAllMailboxes",n);this.get_IsFlightingEnabled()&&n!==this.getValue("AllSourceMailboxes")&&this.setValue("AllSourceMailboxes",n);this.get_IsFlightingEnabled()?this.set_SourceMailboxesEnabled(!n&&!this.get_SearchNoMailboxes()):this.set_SourceMailboxesEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchAllPublicFolders:function(){return this.getValue("SearchAllPublicFolders")},set_SearchAllPublicFolders:function(n){if(n!==this.getValue("SearchAllPublicFolders")){this.setValue("SearchAllPublicFolders",n);this.setValue("AllPublicFolderSources",n)}this.set_PublicFolderSourcesEnabled(!n&&!this.get_SearchNoPublicFolders());this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchNoMailboxes:function(){var n=this.getValue("SourceMailboxes");return n&&n.length?!1||this.get_SearchAllMailboxes():!0&&!this.get_SearchAllMailboxes()},set_SearchNoMailboxes:function(n){n!==this.getValue("SearchNoMailboxes")&&this.setValue("SearchNoMailboxes",n);this.set_SourceMailboxesEnabled(!n&&!this.get_SearchAllMailboxes());this.set_SelectNoPublicFoldersEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchNoPublicFolders:function(){var n=this.getValue("PublicFolderSources");return n&&n.length?!1||this.get_SearchAllPublicFolders():!0&&!this.get_SearchAllPublicFolders()},set_SearchNoPublicFolders:function(n){n!==this.getValue("SearchNoPublicFolders")&&this.setValue("SearchNoPublicFolders",n);this.set_PublicFolderSourcesEnabled(!n&&!this.get_SearchAllPublicFolders());this.set_SelectNoMailboxesEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(n);return n},get_IsEmptyPublicFolders:function(){var n=this.getValue("PublicFolderSources");return n&&n.length?!1:!0},get_IsEmptySourceMailboxes:function(){var n=this.getValue("SourceMailboxes");return n&&n.length?!1:!0},get_OriginalSearchObject:function(){return this.getValue("OriginalSearchObject")},set_OriginalSearchObject:function(n){this.setValue("OriginalSearchObject",n);return n},get_IsFlightingEnabled:function(){return this.getValue("IsFlightingEnabled")},set_IsFlightingEnabled:function(n){this.setValue("IsFlightingEnabled",n);return n},get_SelectAllMailboxesEnabled:function(){return this.$J_4},set_SelectAllMailboxesEnabled:function(n){if(this.$J_4!==n){this.$J_4=n;this.raisePropertyChanged("SelectAllMailboxesEnabled")}return n},get_SelectNoMailboxesEnabled:function(){return this.$L_4},set_SelectNoMailboxesEnabled:function(n){if(this.$L_4!==n){this.$L_4=n;this.raisePropertyChanged("SelectNoMailboxesEnabled")}return n},get_SourceMailboxesEnabled:function(){return this.$N_4},set_SourceMailboxesEnabled:function(n){if(this.$N_4!==n){this.$N_4=n;this.raisePropertyChanged("SourceMailboxesEnabled")}return n},get_SelectAllPublicFoldersEnabled:function(){return this.$K_4},set_SelectAllPublicFoldersEnabled:function(n){if(this.$K_4!==n){this.$K_4=n;this.raisePropertyChanged("SelectAllPublicFoldersEnabled")}return n},get_SelectNoPublicFoldersEnabled:function(){return this.$M_4},set_SelectNoPublicFoldersEnabled:function(n){if(this.$M_4!==n){this.$M_4=n;this.raisePropertyChanged("SelectNoPublicFoldersEnabled")}return n},get_PublicFolderSourcesEnabled:function(){return this.$I_4},set_PublicFolderSourcesEnabled:function(n){if(this.$I_4!==n){this.$I_4=n;this.raisePropertyChanged("PublicFolderSourcesEnabled")}return n},get_SearchContent:function(){return this.getValue("SearchContent")},set_SearchContent:function(n){if(n!==this.getValue("SearchContent")){this.setValue("SearchContent",n);this.set_ContentFilterSectionEnabled(n)}return n},get_ContentFilterSectionEnabled:function(){return this.$E_4},set_ContentFilterSectionEnabled:function(n){if(this.$E_4!==n){this.$E_4=n;this.raisePropertyChanged("ContentFilterSectionEnabled")}return n},get_StartDateEnabled:function(){return this.getValue("StartDateEnabled")},set_StartDateEnabled:function(n){if(n!==this.getValue("StartDateEnabled")){this.setValue("StartDateEnabled",n);this.set_StartDatePickerEnabled(n)}return n},get_StartDatePickerEnabled:function(){return this.$6_4},set_StartDatePickerEnabled:function(n){if(this.$6_4!==n){this.$6_4=n;this.raisePropertyChanged("StartDatePickerEnabled")}this.$6_4&&this.makeDirty("SearchStartDate");return n},get_EndDateEnabled:function(){return this.getValue("EndDateEnabled")},set_EndDateEnabled:function(n){if(n!==this.getValue("EndDateEnabled")){this.setValue("EndDateEnabled",n);this.set_EndDatePickerEnabled(n)}return n},get_EndDatePickerEnabled:function(){return this.$5_4},set_EndDatePickerEnabled:function(n){if(this.$5_4!==n){this.$5_4=n;this.raisePropertyChanged("EndDatePickerEnabled")}this.$5_4&&this.makeDirty("SearchEndDate");return n},get_HoldIndefinitely:function(){return this.getValue("HoldIndefinitely")},set_HoldIndefinitely:function(n){if(n!==this.getValue("HoldIndefinitely")){this.setValue("HoldIndefinitely",n);this.set_HoldPeriodTextBoxEnabled(!n)}return n},get_HoldPeriodTextBoxEnabled:function(){return this.$G_4},set_HoldPeriodTextBoxEnabled:function(n){if(this.$G_4!==n){this.$G_4=n;this.raisePropertyChanged("HoldPeriodTextBoxEnabled")}return n},get_HoldPeriodSectionEnabled:function(){return this.$F_4},set_HoldPeriodSectionEnabled:function(n){if(this.$F_4!==n){this.$F_4=n;this.raisePropertyChanged("HoldPeriodSectionEnabled")}return n},initialize:function(){PropertyPageViewModel.prototype.initialize.call(this);this.get_SaveCommand().set_preCheck(this.$$d_saveCommandPreCheck);this.set_SelectAllMailboxesEnabled(!this.get_InPlaceHoldEnabled());if(this.get_IsFlightingEnabled()){this.set_SourceMailboxesEnabled(!this.get_SearchAllMailboxes()&&!this.get_SearchNoMailboxes());this.set_SelectAllPublicFoldersEnabled(!this.get_InPlaceHoldEnabled());this.set_PublicFolderSourcesEnabled(!this.get_SearchAllPublicFolders()&&!this.get_SearchNoPublicFolders());this.set_SearchMailboxesSelection(this.get_SearchAllMailboxes()?"All":"Select");this.set_SearchPublicFoldersSelection(this.get_SearchAllPublicFolders()?"All":"Select");this.set_SelectNoMailboxesEnabled(!this.get_SearchNoPublicFolders());this.set_SelectNoPublicFoldersEnabled(!this.get_SearchNoMailboxes());if(this.get_OriginalSearchObject()){this.set_SearchAllMailboxes(this.get_SearchNoMailboxes());this.set_SearchNoMailboxes(!1);this.set_SearchNoPublicFolders(!1)}}else this.set_SourceMailboxesEnabled(!this.get_SearchAllMailboxes());this.set_InPlaceHoldCheckBoxEnabled(!this.get_SearchAllMailboxes());this.set_ContentFilterSectionEnabled(this.get_SearchContent());this.set_StartDatePickerEnabled(this.get_StartDateEnabled());this.set_EndDatePickerEnabled(this.get_EndDateEnabled());this.set_HoldPeriodSectionEnabled(this.get_InPlaceHoldEnabled());this.set_HoldPeriodTextBoxEnabled(!this.get_HoldIndefinitely())},saveCommandPreCheck:function(){this.set_MessageHandler(this.$$d_$c_4);return!0},$c_4:function(n,t,i){if(t){n.ErrorRecords=null;i()}}};function MessageTypeControl(n){this.$$d_$f_5=Function.createDelegate(this,this.$f_5);MessageTypeControl.initializeBase(this,[n])}MessageTypeControl.prototype={$Q_5:null,$P_5:null,$S_5:null,$4_5:null,$R_5:null,initialize:function(){EcpSelect.prototype.initialize.call(this);$addHandler(this.$Q_5,"click",this.$$d_$f_5);this.$a_5()},$a_5:function(){var n=new Sys.StringBuilder;if(this.$4_5&&this.$4_5.length)for(var t=0;t<this.$4_5.length;t++){n.append(this.$R_5[this.$4_5[t]]);t!==this.$4_5.length-1&&n.append(", ")}else n.append(Strings.get_MessageTypeAll());this.$P_5&&(this.$P_5.innerHTML=EcpUtil.htmlEncode(n.toString()))},onOK:function(n){this.set_ValueArray(n)},$f_5:function(n){if(!this.get_element().disabled){var t=this.$S_5;t+="?types="+this.get_value();this.invokePicker(t)}},get_ValueArray:function(){return this.$4_5},set_ValueArray:function(n){if(this.$4_5!==n){this.$4_5=n;this.$a_5()}return n},get_value:function(){return this.$4_5?this.$4_5.join(","):_String.empty},set_value:function(n){this.set_ValueArray(Sys.Serialization.JavaScriptSerializer.deserialize(n));return n},get_LaunchButton:function(){return this.$Q_5},set_LaunchButton:function(n){this.$Q_5=n;return n},get_ValueLabel:function(){return this.$P_5},set_ValueLabel:function(n){this.$P_5=n;return n},get_MessageTypePickerUrl:function(){return this.$S_5},set_MessageTypePickerUrl:function(n){this.$S_5=n;return n},get_LocStringMapping:function(){return this.$R_5},set_LocStringMapping:function(n){this.$R_5=n;return n}};function MessageTypePicker(n){MessageTypePicker.initializeBase(this,[n])}MessageTypePicker.prototype={$O_6:null,$U_6:null,$V_6:null,$Z_6:function(){return this.$O_6.checked||this.$U_6.checked&&this.get_value().length>0},onCommit:function(n){n.preventDefault();this.$Z_6()?this.addSelectResult(this.get_value()):MessageBox.showMessage(Strings.get_MessageTypePickerInvalid(),MessageBoxType.error)},get_value:function(){var n=[];if(this.$O_6.checked)return n;var t=this.$V_6;for(var i in t){var r={key:i,value:t[i]};var u=$get(r.value);u.checked&&Array.add(n,r.key)}return n},get_AllTypesRadioButton:function(){return this.$O_6},set_AllTypesRadioButton:function(n){this.$O_6=n;return n},get_SpecificTypesRadioButton:function(){return this.$U_6},set_SpecificTypesRadioButton:function(n){this.$U_6=n;return n},get_TypeEnumToControlMapping:function(){return this.$V_6},set_TypeEnumToControlMapping:function(n){this.$V_6=n;return n}};function NewDiscoveryHoldViewModel(){this.$$d_$c_5=Function.createDelegate(this,this.$c_5);this.$$d_saveCommandPreCheck=Function.createDelegate(this,this.saveCommandPreCheck);NewDiscoveryHoldViewModel.initializeBase(this)}NewDiscoveryHoldViewModel.prototype={$J_5:!0,$L_5:!0,$K_5:!0,$M_5:!0,$N_5:!0,$I_5:!0,$H_5:!0,$E_5:!1,$6_5:!1,$5_5:!1,$F_5:!1,$G_5:!1,get_InPlaceHoldEnabled:function(){return this.getValue("InPlaceHoldEnabled")},set_InPlaceHoldEnabled:function(n){if(n!==this.getValue("InPlaceHoldEnabled")){this.setValue("InPlaceHoldEnabled",n);this.set_HoldPeriodSectionEnabled(n);this.set_SelectAllMailboxesEnabled(!n);this.set_SelectAllPublicFoldersEnabled(!n)}return n},get_InPlaceHoldCheckBoxEnabled:function(){return this.$H_5},set_InPlaceHoldCheckBoxEnabled:function(n){this.$H_5=this.get_IsFlightingEnabled()?this.get_SearchNoPublicFolders()&&!this.get_SearchNoMailboxes()&&!this.get_SearchAllMailboxes():!this.get_SearchAllMailboxes();this.raisePropertyChanged("InPlaceHoldCheckBoxEnabled");return n},get_SearchMailboxesSelection:function(){return this.getValue("SearchMailboxesSelection")},set_SearchMailboxesSelection:function(n){this.setValue("SearchMailboxesSelection",n);if(n==="All"){this.set_SearchAllMailboxes(!0);this.set_SearchNoMailboxes(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}if(n==="Select"){this.set_SearchAllMailboxes(!1);this.set_SearchNoMailboxes(!1);this.set_InPlaceHoldCheckBoxEnabled(!0)}return n},get_SearchPublicFoldersSelection:function(){return this.getValue("SearchPublicFoldersSelection")},set_SearchPublicFoldersSelection:function(n){this.setValue("SearchPublicFoldersSelection",n);if(n==="All"){this.set_SearchAllPublicFolders(!0);this.set_SearchNoPublicFolders(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}if(n==="Select"){this.set_SearchAllPublicFolders(!1);this.set_SearchNoPublicFolders(!1);this.set_InPlaceHoldCheckBoxEnabled(!1)}return n},get_SearchAllMailboxes:function(){return this.getValue("SearchAllMailboxes")},set_SearchAllMailboxes:function(n){n!==this.getValue("SearchAllMailboxes")&&this.setValue("SearchAllMailboxes",n);this.get_IsFlightingEnabled()&&n!==this.getValue("AllSourceMailboxes")&&this.setValue("AllSourceMailboxes",n);this.get_IsFlightingEnabled()?this.set_SourceMailboxesEnabled(!n&&!this.get_SearchNoMailboxes()):this.set_SourceMailboxesEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchAllPublicFolders:function(){return this.getValue("SearchAllPublicFolders")},set_SearchAllPublicFolders:function(n){if(n!==this.getValue("SearchAllPublicFolders")){this.setValue("SearchAllPublicFolders",n);this.setValue("AllPublicFolderSources",n)}this.set_PublicFolderSourcesEnabled(!n&&!this.get_SearchNoPublicFolders());this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchNoMailboxes:function(){var n=this.getValue("SourceMailboxes");return n&&n.length?!1||this.get_SearchAllMailboxes():!0&&!this.get_SearchAllMailboxes()},set_SearchNoMailboxes:function(n){n!==this.getValue("SearchNoMailboxes")&&this.setValue("SearchNoMailboxes",n);this.set_SourceMailboxesEnabled(!n&&!this.get_SearchAllMailboxes());this.set_SelectNoPublicFoldersEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(!n);return n},get_SearchNoPublicFolders:function(){var n=this.getValue("PublicFolderSources");return n&&n.length?!1||this.get_SearchAllPublicFolders():!0&&!this.get_SearchAllPublicFolders()},set_SearchNoPublicFolders:function(n){n!==this.getValue("SearchNoPublicFolders")&&this.setValue("SearchNoPublicFolders",n);this.set_PublicFolderSourcesEnabled(!n&&!this.get_SearchAllPublicFolders());this.set_SelectNoMailboxesEnabled(!n);this.set_InPlaceHoldCheckBoxEnabled(n);return n},get_IsEmptyPublicFolders:function(){var n=this.getValue("PublicFolderSources");return n&&n.length?!1:!0},get_IsEmptySourceMailboxes:function(){var n=this.getValue("SourceMailboxes");return n&&n.length?!1:!0},get_SelectAllMailboxesEnabled:function(){return this.$J_5},set_SelectAllMailboxesEnabled:function(n){if(this.$J_5!==n){this.$J_5=n;this.raisePropertyChanged("SelectAllMailboxesEnabled")}return n},get_SelectNoMailboxesEnabled:function(){return this.$L_5},set_SelectNoMailboxesEnabled:function(n){if(this.$L_5!==n){this.$L_5=n;this.raisePropertyChanged("SelectNoMailboxesEnabled")}return n},get_SourceMailboxesEnabled:function(){return this.$N_5},set_SourceMailboxesEnabled:function(n){if(this.$N_5!==n){this.$N_5=n;this.raisePropertyChanged("SourceMailboxesEnabled")}return n},get_SelectAllPublicFoldersEnabled:function(){return this.$K_5},set_SelectAllPublicFoldersEnabled:function(n){if(this.$K_5!==n){this.$K_5=n;this.raisePropertyChanged("SelectAllPublicFoldersEnabled")}return n},get_SelectNoPublicFoldersEnabled:function(){return this.$M_5},set_SelectNoPublicFoldersEnabled:function(n){if(this.$M_5!==n){this.$M_5=n;this.raisePropertyChanged("SelectNoPublicFoldersEnabled")}return n},get_PublicFolderSourcesEnabled:function(){return this.$I_5},set_PublicFolderSourcesEnabled:function(n){if(this.$I_5!==n){this.$I_5=n;this.raisePropertyChanged("PublicFolderSourcesEnabled")}return n},get_IsFlightingEnabled:function(){return this.getValue("IsFlightingEnabled")},set_IsFlightingEnabled:function(n){this.setValue("IsFlightingEnabled",n);return n},get_SearchContent:function(){return this.getValue("SearchContent")},set_SearchContent:function(n){if(n!==this.getValue("SearchContent")){this.setValue("SearchContent",n);this.set_ContentFilterSectionEnabled(n)}return n},get_ContentFilterSectionEnabled:function(){return this.$E_5},set_ContentFilterSectionEnabled:function(n){if(this.$E_5!==n){this.$E_5=n;this.raisePropertyChanged("ContentFilterSectionEnabled")}return n},get_StartDateEnabled:function(){return this.getValue("StartDateEnabled")},set_StartDateEnabled:function(n){if(n!==this.getValue("StartDateEnabled")){this.setValue("StartDateEnabled",n);this.set_StartDatePickerEnabled(n)}return n},get_StartDatePickerEnabled:function(){return this.$6_5},set_StartDatePickerEnabled:function(n){if(this.$6_5!==n){this.$6_5=n;this.raisePropertyChanged("StartDatePickerEnabled")}return n},get_EndDateEnabled:function(){return this.getValue("EndDateEnabled")},set_EndDateEnabled:function(n){if(n!==this.getValue("EndDateEnabled")){this.setValue("EndDateEnabled",n);this.set_EndDatePickerEnabled(n)}return n},get_EndDatePickerEnabled:function(){return this.$5_5},set_EndDatePickerEnabled:function(n){if(this.$5_5!==n){this.$5_5=n;this.raisePropertyChanged("EndDatePickerEnabled")}return n},get_HoldIndefinitely:function(){return this.getValue("HoldIndefinitely")},set_HoldIndefinitely:function(n){if(n!==this.getValue("HoldIndefinitely")){this.setValue("HoldIndefinitely",n);this.set_HoldPeriodTextBoxEnabled(!n)}return n},get_HoldPeriodTextBoxEnabled:function(){return this.$G_5},set_HoldPeriodTextBoxEnabled:function(n){if(this.$G_5!==n){this.$G_5=n;this.raisePropertyChanged("HoldPeriodTextBoxEnabled")}return n},get_HoldPeriodSectionEnabled:function(){return this.$F_5},set_HoldPeriodSectionEnabled:function(n){if(this.$F_5!==n){this.$F_5=n;this.raisePropertyChanged("HoldPeriodSectionEnabled")}return n},initialize:function(){WizardViewModel.prototype.initialize.call(this);this.get_SaveCommand().set_preCheck(this.$$d_saveCommandPreCheck);this.set_SearchMailboxesSelection("Select");this.set_SearchPublicFoldersSelection("Select");if(!this.get_AdhocValues().getProperty("HasHoldRole")){var n=this.findViewModelWithID("DiscoveryHoldContent");n.set_NextViewModelID(null)}},handleMessage:function(){this.handleMessageInternal(!1)},saveCommandPreCheck:function(){this.set_MessageHandler(this.$$d_$c_5);return!0},$c_5:function(n,t,i){if(t){n.ErrorRecords=null;i()}}};DateChooser.registerClass("DateChooser",ContainerControl);DaysOfWeekSelector.registerClass("DaysOfWeekSelector",ContainerControl);MailboxSearchUtil.registerClass("MailboxSearchUtil");EditDiscoveryHoldViewModel.registerClass("EditDiscoveryHoldViewModel",PropertyPageViewModel);MessageTypeControl.registerClass("MessageTypeControl",EcpSelect);MessageTypePicker.registerClass("MessageTypePicker",PickerFormBase);NewDiscoveryHoldViewModel.registerClass("NewDiscoveryHoldViewModel",WizardViewModel);NewDiscoveryHoldViewModel.contentFilterStepId="DiscoveryHoldContent";NewDiscoveryHoldViewModel.mailboxesStepId="DiscoveryHoldMailboxes"