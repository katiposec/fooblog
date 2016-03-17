<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="view_profile.aspx.cs" Inherits="FooBlog.view_profile" %>
<%@ Import Namespace="FooBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <asp:DataList ID="userList" runat="server" DataKeyField="userid">
        <ItemTemplate>
            <div class="left-align" style="width: 30%">
                <img class="responsive-img materialboxed" style="border-radius: 2px" src='<%# (String.IsNullOrEmpty(Eval("profileimg").ToString()) ? FooStringHelper.MakeImageUrl("profile_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("profileimg"))) %>' />
            </div>
            <p>
                <span style="font-size: 24px"><%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %></span><br />
                <span class="grey-text"><%#(String.IsNullOrEmpty(Eval("country").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("city"))) %>, <%#(String.IsNullOrEmpty(Eval("country").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("country"))) %></span>
            </p>
            <p><%#(String.IsNullOrEmpty(Eval("profilebody").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("profilebody"))) %></p>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
</asp:Content>