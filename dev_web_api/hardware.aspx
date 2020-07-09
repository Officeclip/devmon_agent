<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="hardware.aspx.cs"
    Inherits="dev_web_api.hardware" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="margin: 20px">
        <div>
            <div style="display: inline">
                Agents:
                    <asp:DropDownList ID="ddlAgents" runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlAgents_SelectedIndexChanged" />
            </div>
            <div style="display: inline; margin-left: 20px">
                <asp:Literal ID="litDate" runat="server" />
            </div>
            <div style="display: inline; margin-left: 20px">
                <asp:Button ID="btnBack" runat="server"
                    Text="Back to Monitor"
                    Font-Bold="true"
                    OnClick="btnBack_Click" />
            </div>
        </div>
        <div style="margin-top: 10px">
            <asp:Label ID="lblEmptyData" runat="server"
                Text="Data for this agent is not available"
                ForeColor="Red" />
            <asp:TreeView ID="treeView1" runat="server">
            </asp:TreeView>
        </div>
    </div>
</asp:Content>
