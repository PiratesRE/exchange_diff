﻿Type.registerNamespace("ECP");function ReadingPaneProperties(n){this.$$d_$3_5=Function.createDelegate(this,this.$3_5);ReadingPaneProperties.initializeBase(this,[n])}ReadingPaneProperties.prototype={$0_5:null,$1_5:null,initialize:function(){PropertyPage.prototype.initialize.call(this);var t=this.getContentElementByServerId("rbPreviewMarkAsReadBehavior");this.$1_5=this.getContentElementByServerId("divPreviewMarkAsReadDelaytime");this.$0_5=t.getElementsByTagName("input");var n=this.getContentElementByServerId("tbxPreviewMarkAsReadDelaytime");var i=this.getContentElementByServerId("tbxPreviewMarkAsReadDelaytime_PreLabel");var r=this.getContentElementByServerId("tbxPreviewMarkAsReadDelaytime_PostLabel");EcpUtil.isIE()&&Sys.UI.DomElement.addCssClass(n,"vaM");n.setAttribute(AriaUtil.ariaLabelledby,String.format("{0} {1} {2}",i.id,n.id,r.id));var u=t.insertRow(1);var f=u.insertCell(0);f.appendChild(this.$1_5);$addHandler(this.$0_5[0],"click",this.$$d_$3_5);$addHandler(this.$0_5[2],"click",this.$$d_$3_5);$addHandler(this.$0_5[3],"click",this.$$d_$3_5);this.$2_5()},dispose:function(){$removeHandler(this.$0_5[0],"click",this.$$d_$3_5);$removeHandler(this.$0_5[2],"click",this.$$d_$3_5);$removeHandler(this.$0_5[3],"click",this.$$d_$3_5);PropertyPage.prototype.dispose.call(this)},$3_5:function(n){this.$2_5()},$2_5:function(){var n=this.$0_5[0].checked;WizardPropertyUtil.toggleChildrenDisabled(this.$1_5,!n)}};ReadingPaneProperties.registerClass("ReadingPaneProperties",PropertyPage)