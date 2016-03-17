<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="view_post.aspx.cs" Inherits="FooBlog.view_post" %>
<%@ Import Namespace="FooBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <asp:DataList ID="postList" runat="server" DataKeyField="postid">
        <ItemTemplate>
            <h3><%#(String.IsNullOrEmpty((string) Eval("posttitle")) ? "" : Eval("posttitle")) %></h3>
            <p class="grey-text" style="font-size: 13px">
                <%# (String.IsNullOrEmpty(Eval("posttime").ToString()) ? "" : FooStringHelper.DateTimeToString((DateTime) Eval("posttime"))) %><br />
                Tagged: <a href='<%# (String.IsNullOrEmpty(Eval("catid").ToString()) ? "#" : String.Format("view_category.aspx?id={0}", Eval("catid"))) %>'><%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "Undefined" : Eval("catname")) %></a>
            </p>
            <p><%#(String.IsNullOrEmpty(Eval("postbody").ToString()) ? "" : Eval("postbody")) %></p>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <br />
    <asp:Panel ID="commentPanel" runat="server" Visible="False">
        <h5>Comments</h5>
        <asp:DataList ID="commentList" runat="server" DataKeyField="commentid">
            <ItemTemplate>
                <div class="row z-depth-1" style="padding-bottom: 5px; padding-top: 10px;">
                    <div class="col s1">
                        <a href='<%# (String.IsNullOrEmpty(Eval("userid").ToString()) ? "#" : String.Format("view_profile.aspx?id={0}", Eval("userid"))) %>'>
                            <img class="responsive-img center-align" style="border-radius: 2px" src='<%# (String.IsNullOrEmpty(Eval("profileimg").ToString()) ? FooStringHelper.MakeImageUrl("profile_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("profileimg"))) %>' />
                        </a>
                    </div>
                    <div class="col s11" style="padding-bottom: 5px">
                        <p>
                            <%#(String.IsNullOrEmpty(Eval("commentbody").ToString()) ? "" : Eval("commentbody")) %>
                        </p>
                        <p style="font-size: 12px">
                            <a href='<%# (String.IsNullOrEmpty(Eval("userid").ToString()) ? "#" : String.Format("view_profile.aspx?id={0}", Eval("userid"))) %>'><%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "Undefined" : Eval("useralias")) %></a>
                            <span class="grey-text">&nbsp;@&nbsp;<%# (String.IsNullOrEmpty(Eval("commenttime").ToString()) ? "" : FooStringHelper.DateTimeToString((DateTime) Eval("commenttime"))) %></span>
                        </p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:DataList>
        <asp:Label ID="commentLabel" runat="server"></asp:Label>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:TextBox ID="commentText" MaxLength="1024" runat="server"></asp:TextBox>
                <label for="mainContent_commentText">Leave a Comment</label>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:LinkButton ID="submitButton" runat="server" CausesValidation="False" CssClass="waves-effect waves-light blue lighten-1 btn-large" OnClientClick=" if (!validateComment()) return false; " OnClick="submitButton_Click">Submit</asp:LinkButton>
            </div>
        </div>
        <asp:HiddenField ID="RequestToken" runat="server" />
        <asp:Label ID="commentErrorLabel" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>