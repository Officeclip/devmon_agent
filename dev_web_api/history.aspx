<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="dev_web_api.history" %>

<%@ Register TagPrefix="ocTag" TagName="graphControl" Src="~/Graphcontrol.ascx" %>
<html>

<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.13.0/moment.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.6.0/Chart.min.js"></script>
    <%--    <script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div>
            <ocTag:graphControl ID="graphControl" runat="server"></ocTag:graphControl>
        </div>
    </form>

</body>
</html>
