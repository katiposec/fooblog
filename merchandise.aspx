<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="merchandise.aspx.cs" Inherits="FooBlog.merchandise" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Merchandise</h3>
    <asp:DataList ID="merchList" runat="server" DataKeyField="merchid">
        <ItemTemplate>
            <a href='<%# (String.IsNullOrEmpty(Eval("merchid").ToString()) ? "#" : String.Format("view_item.aspx?id={0}", Eval("merchid"))) %>'>
                <h4><%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %></h4>
            </a>
            <p class="grey-text">
                Price: <%#(String.IsNullOrEmpty(Eval("merchprice").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchprice"))) %>
            </p>
            <p><%#(String.IsNullOrEmpty(Eval("merchbrief").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchbrief"))) %></p>
            <br />
            <a href='<%# (String.IsNullOrEmpty(Eval("merchid").ToString()) ? "#" : String.Format("view_item.aspx?id={0}", Eval("merchid"))) %>'>Read more</a>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
</asp:Content>