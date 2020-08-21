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
    <style type="text/css">
        .dots:after {
            content: '\2807';
            font-size: 20px;       
        }

        .dots {
            /*background-color: #3498DB;
                    color: white;
                    padding: 16px;*/
            font-size: 16px;
            border: none;
            cursor: pointer;
        }

            .dots:hover, .dots:focus {
                /*background-color: #2980B9;*/
            }

        .dropdown {
            position: relative;
            display: inline-block;
        }

        .dropdown-content {
            display: none;
            position: absolute;
            background-color: #f1f1f1;
            min-width: 160px;
            overflow: auto;
            box-shadow: 0px 8px 16px 0px rgba(0,0,0,0.2);
            z-index: 1;
        }

            .dropdown-content a {
                color: black;
                padding: 12px 16px;
                text-decoration: none;
                display: block;
            }

        .dropdown a:hover {
            background-color: #ddd;
        }

        .show {
            display: block;
        }
    </style>
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

        <table id="tblMonitor" class="monitor" runat="server" />

        <table style="margin-top: 24px" border="1">
            <tr>
            <asp:Repeater ID="rptHeader" runat="server">
                <HeaderTemplate>
                <th>
                </th>
                </HeaderTemplate>
                <ItemTemplate>
                <th>
                    <asp:Literal ID="litColumnHeader" runat="server" 
                        Text='<%# Eval("Name") %>'>          
                    </asp:Literal>
                </th>
                </ItemTemplate>
            </asp:Repeater>
            </tr>
            <asp:Repeater ID="rptRowItem" runat="server" 
                OnItemDataBound="rptRowItem_ItemDataBound">
                <ItemTemplate>
                <tr>
                    <td>
                        <asp:Literal ID="litRowHeader" runat="server" 
                            Text='<%# GetHeader(Container.ItemIndex) %>'>          
                        </asp:Literal>                        
                    </td>
                    <asp:Repeater ID="rptCellItem" runat="server"
                         OnItemDataBound="rptCellItem_ItemDataBound">
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
