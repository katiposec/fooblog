<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="index.aspx.cs" Inherits="FooBlog.index" %>
<%@ Import Namespace="FooBlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Ramblings</h3>
    <asp:DataList ID="postList" runat="server" DataKeyField="postid">
        <ItemTemplate>
            <a href='<%# (String.IsNullOrEmpty(Eval("postid").ToString()) ? "#" : String.Format("view_post.aspx?id={0}", Eval("postid"))) %>'>
                <h4><%#(String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("posttitle"))) %></h4>
            </a>
            <p class="grey-text">
                <%# (String.IsNullOrEmpty(Eval("posttime").ToString()) ? "" : FooStringHelper.DateTimeToString((DateTime) Eval("posttime"))) %><br />
                Tagged: <a href='<%# (String.IsNullOrEmpty(Eval("catid").ToString()) ? "#" : String.Format("view_category.aspx?id={0}", Eval("catid"))) %>'><%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("catname"))) %></a>
            </p>
            <p><%#(String.IsNullOrEmpty(Eval("postbrief").ToString()) ? "" : Server.HtmlDecode((string) Eval("postbrief"))) %></p>
            <br />
            <a href='<%# (String.IsNullOrEmpty(Eval("postid").ToString()) ? "#" : String.Format("view_post.aspx?id={0}", Eval("postid"))) %>'>Read more</a>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
</asp:Content>