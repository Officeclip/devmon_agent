<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    Title="Home"
    AutoEventWireup="true"
    CodeBehind="default.aspx.cs"
    Inherits="dev_web_api._default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/countdown.css" />

    <script type="text/javascript">
        function myFunction(id) {
            document.getElementById(id).classList.toggle("show");
        }

        $(document).ready(function () {
            $(document).on('mouseenter', '.outer', function () {
                $(this).find(".dots").css("visibility", "visible");
            }).on('mouseleave', '.outer', function () {
                $(this).find(".dots").css("visibility", "hidden");
            });
        });

        // Close the dropdown if the user clicks outside of it
        window.onclick = function (event) {
            if (!event.target.matches('.dots')) {
                var dropdowns = document.getElementsByClassName("dropdown-content");
                var i;
                for (i = 0; i < dropdowns.length; i++) {
                    var openDropdown = dropdowns[i];
                    if (openDropdown.classList.contains('show')) {
                        openDropdown.classList.remove('show');
                    }
                }
            }
        }


    </script>
    <div>
        <div>
            <div id="app" style="float: left"></div>
            <div style="margin: 5px 0 0 20px; float: left">
                <span>Agent Group:</span>
                <asp:DropDownList ID="ddlAgentGroups"
                    runat="server"
                    OnSelectedIndexChanged="ddlAgentGroups_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:DropDownList>
                <asp:CheckBox ID="chkEmailOpt" runat="server"
                    Style="padding-left: 40px"
                    OnCheckedChanged="chkEmailOpt_CheckedChanged"
                    AutoPostBack="true" />
                <span>Send Email if Agent fails</span> &nbsp&nbsp
                <span style="font-size: small; padding-left: 70px">Last Updated: <%= DateTime.Now %>
                </span>

            </div>
            <div style="clear: both" />

        </div>

        <table id="tblMonitor" class="monitor" runat="server" visible="false" />

        <table class="monitor">
            <tr>
                <asp:Repeater ID="rptHeader" runat="server">
                    <HeaderTemplate>
                        <th></th>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <th id="thLimit" title='<%# GetMonitorCommandWarningLimit(Container.ItemIndex)%>'>
                            <span style="border-bottom: 1px dashed black">
                                <asp:Literal ID="litColumnHeader" runat="server"
                                    Text='<%#Eval("Name") %>'>          
                                </asp:Literal>
                            </span>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
            <asp:Repeater ID="rptRowItem" runat="server"
                OnItemDataBound="rptRowItem_ItemDataBound">
                <ItemTemplate>
                    <tr>
                        <td id="tdTooltip" title='<%# GetToolTipInfo(Container.ItemIndex) %>' class="headerTitle" runat="server">
                            <%# GetHeader(Container.ItemIndex) %>                        
                        </td>
                        <asp:Repeater ID="rptCellItem" runat="server">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <%# GetData(Container.ItemIndex, Container.DataItem) %>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <div style="margin-top: 50px">
            <span class="heading">Agent Installation:</span>
            <ul>
                <li>Download and Install:
                             
                            <ol>
                                <li>Download 
                                    <a
                                        href="https://github.com/Officeclip/devmon_agent/releases/download/Agent-0.5/AgentSetup.zip"
                                        style="font-weight: bold">AgentSetup.zip</a></li>
                                <li>Unzip AgentSetup.zip</li>
                                <li>Run <b>Setup.exe</b></li>
                                <li>Enter Server Url: <b><%= GetWebUri() %></b></li>
                                <li>Enter Server Key: <b><%= serverGuid %></b></li>
                            </ol>

                </li>


                <li>Uninstall:
                    <ul>
                        <li>Go to the <i>Control Panel > Programs and Features</i>
                            and uninstall the Agent Monitor
                        </li>
                    </ul>

                </li>
            </ul>
        </div>
    </div>

    <script src="js/countdown.js"></script>

</asp:Content>
