<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnalisadorLexico.aspx.cs" Inherits="Analisador_Léxico.AnalisadorLexico" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br/><br/>
            <asp:TextBox ID="TextBox1" runat="server" OnTextChanged="Texto_TextChanged" AutoPostBack="true" Width="334px"></asp:TextBox><br/>
           
            <br/>
            <asp:GridView ID="ListPadroes" runat="server" AutoGenerateColumns="true" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical" Width="343px">
                <AlternatingRowStyle BackColor="#CCCCCC" />
               
                <FooterStyle BackColor="#CCCCCC" />
                <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#808080" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#383838" />
            </asp:GridView>
            


        </div>
    </form>
</body>
</html>
