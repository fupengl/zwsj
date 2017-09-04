<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
<%--    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />--%>    <meta http-equiv="content-script-type" content="text/javascript">
    <title>智地数据库</title>
    <script src="../../Content/mainFramework.js" type="text/javascript"></script>
</head>
<body>
    <div>
        <ext:ResourceManager ID="ResourceManager1" runat="server" ScriptMode="Debug" Theme="Default" />
             <form id="Form1" runat="server">
                 <ext:Window ID="Window2" runat="server" Closable="true" Resizable="false" Height="150"
            Icon="Lock" Title="密码修改" Draggable="false" Width="350" BodyPadding="5"  Modal="true"
            AutoShow="False" Layout="FormLayout" >
            <Items>
                <ext:TextField ID="oldPassword" runat="server" InputType="Password" FieldLabel="原始密码" AllowBlank="false"
                    BlankText="请输入原始密码" Text="" />
                <ext:TextField ID="newPassword" runat="server" InputType="Password" FieldLabel="新密码" AllowBlank="false"
                    BlankText="请输入新密码" Text="" />
                <ext:TextField ID="comfirmPassword" runat="server" InputType="Password" FieldLabel="确认密码"
                    AllowBlank="false" BlankText="请输入确认密码" Text="" />
            </Items>
            <Buttons>
                <ext:Button ID="Button1" runat="server" Text="修改密码" Icon="Accept">
                    <Listeners>
                        <Click Handler="
                            if (!#{oldPassword}.validate() || !#{newPassword}.validate() ||!#{comfirmPassword}.validate()) {
                                Ext.Msg.alert('错误','输入信息有空！');
                                // return false to prevent the btnLogin_Click Ajax Click event from firing.
                                return false; 
                            }
                            if(#{newPassword}.getValue() != #{comfirmPassword}.getValue())
                            {
                                Ext.Msg.alert('错误','新密码与确认密码不一致！');
                                return false;
                            }
                            " />
                    </Listeners>
                    <DirectEvents>
                        <Click Url="SysMenu" Action="Modify">
                            <ExtraParams>
                                <ext:Parameter Name="oldPassword" Value="#{oldPassword}.getValue()" Mode="Raw" />
                                <ext:Parameter Name="newPassword" Value="#{newPassword}.getValue()" Mode="Raw" />
                            </ExtraParams>
                            <EventMask ShowMask="true" Msg="正在修改..." MinDelay="50" />
                        </Click>
                    </DirectEvents>
                </ext:Button>
            </Buttons>
        </ext:Window>
             </form>
        <ext:Viewport ID="Viewport1" runat="server" Layout="border">
            <Items>
                <ext:Panel runat="server" Height="67" Html="<img src='/Images/logo.png' height='40px'><span style='position:absolute;width:300px; padding:0px; top:0px;  border:0px solid red;  font-size:24px;font-family:微软雅黑;font-weight:bolder;'>智地数据库管理系统</span>" Region="North" Split="True" BodyStyle="line-height : 40px;padding-left:0px;font-size:22px;font-family:黑体;font-weight:bolder;background-color:#dfe8f6;">
                    <BottomBar>
                        <ext:Toolbar runat="server">
                            <Items>
                                <ext:Button runat="server" Text="管理员" Icon="User" ID="userName">
                                </ext:Button>
                                <ext:ToolbarSeparator>
                                </ext:ToolbarSeparator>
                                <ext:Button runat="server" Text='<%# DateTime.Today.ToString("yyyy年MM月dd日") %>' AutoDataBind="True">
                                </ext:Button>
                                <ext:ToolbarFill runat="server" />
                                <ext:Button ID="Button2" runat="server" Text="修改密码" Icon="Lock">
                                    <Listeners>
                                        <Click Handler = "#{Window2}.show();">
                                        </Click>
                                    </Listeners>
                                </ext:Button>
                                <ext:Button runat="server" Text="退出" Icon="DoorOut">
                                    <DirectEvents>
                                        <Click Url="Account" Action="Logout">
                                            <EventMask ShowMask="True"></EventMask>
                                        </Click>
                                    </DirectEvents>
                                </ext:Button>
                            </Items>
                        </ext:Toolbar>
                    </BottomBar>
                </ext:Panel>
                <ext:Panel runat="server" ID="tree" Title="系统菜单" Width="250" Region="West" Icon="ApplicationSideTree" AutoScroll="False" Layout="accordion" Collapsible="True" LayoutConfig="{animate: true}" Split="True">
                    
                </ext:Panel>
                <ext:TabPanel ID="tab" runat="server" Split="True" Region="Center" AutoScroll="True" Border="true" AnimScroll="true" ActiveTabIndex="0">
                    <Items>
                        <ext:Panel  runat="server" Icon="ApplicationDouble" Title="平台首页"></ext:Panel>
                    </Items>
                    <Plugins>
                        <ext:BoxReorderer runat="server"></ext:BoxReorderer>
                        <ext:TabCloseMenu CloseTabText="关闭面板" CloseOthersTabsText="关闭其他" CloseAllTabsText="关闭所有"></ext:TabCloseMenu>
                    </Plugins>
                </ext:TabPanel>
               <%-- <ext:StatusBar runat="server" Region="South" StatusAlign="Left" Items="测试">
                </ext:StatusBar>--%>
            </Items>
            <Listeners>
                <AfterRender Fn="loadMenu"></AfterRender>
            </Listeners>
        </ext:Viewport>
    </div>
</body>
</html>
