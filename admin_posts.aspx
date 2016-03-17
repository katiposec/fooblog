<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="admin_posts.aspx.cs" Inherits="FooBlog.admin_posts" %>
<%@ Import Namespace="FooBlog" %>
<%@ Import Namespace="Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Content Editor</h3>
    <h4>Categories</h4>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <asp:GridView ID="categoryGrid" runat="server" AutoGenerateColumns="False" ShowFooter="True" DataKeyNames="catid"
                  OnRowDeleting="CategoryGrid_Delete" OnRowUpdating="CategoryGrid_Update" OnRowEditing="CategoryGrid_Edit"
                  OnRowCancelingEdit="CategoryGrid_Cancel" OnRowCommand="CategoryGrid_Command" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="catid" HeaderText="Category ID" InsertVisible="False" ReadOnly="True" SortExpression="catname" />
            <asp:TemplateField HeaderText="Name">
                <EditItemTemplate>
                    <asp:TextBox ID="txtCatName" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("catname"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtCatNameFooter" MaxLength="32" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="nameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("catname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Edit" ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="UpdateButton" CausesValidation="False" CommandName="Update" Text="Update" OnClientClick=" if (!validateCategoryGridUpdate()) return false; " runat="server"></asp:LinkButton>
                    <asp:LinkButton ID="CancelButton" CausesValidation="False" CommandName="Cancel" Text="Cancel" runat="server"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="EditButton" CausesValidation="False" CommandName="Edit" Text="Edit" runat="server"></asp:LinkButton>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="NewButton" CausesValidation="False" CommandName="AddNew" Text="Add New" OnClientClick=" if (!validateCategoryGridInsert()) return false; " runat="server"></asp:LinkButton>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="DeleteButton" CausesValidation="False" CommandName="Delete" Text="Delete" OnClientClick=" if (!window.confirm('Are you sure?')) return false; " runat="server"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <h4>Posts</h4>
    <asp:GridView ID="postGrid" runat="server" AutoGenerateColumns="False" ShowFooter="False" DataKeyNames="postid"
                  OnSelectedIndexChanged="PostGrid_SelectedIndexChanged" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>

        <Columns>
            <asp:BoundField DataField="postid" HeaderText="Post ID" InsertVisible="False" ReadOnly="True" SortExpression="postid" />
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:Label ID="titleLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("posttitle"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Posted">
                <ItemTemplate>
                    <asp:Label ID="timeLabel" runat="server" Text='<%# (String.IsNullOrEmpty(Eval("posttime").ToString()) ? "<i>Undefined</i>" : FooStringHelper.DateTimeToString((DateTime) Eval("posttime"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions" ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="SelectButton" CausesValidation="False" CommandName="Select" Text="Select" runat="server"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <br />
    <asp:DetailsView ID="postView" DataKeyNames="postid" AllowPaging="False" AutoGenerateRows="False" runat="server" OnDataBound="PostView_Databound" OnItemInserting="PostView_ItemInserting" OnModeChanging="PostView_ModeChanging" OnItemDeleting="PostView_ItemDeleting" OnItemUpdating="PostView_ItemUpdating" Width="720px">
        <Fields>
            <asp:BoundField DataField="postid" HeaderText="Post ID" InsertVisible="False" ReadOnly="True" SortExpression="postid" />
            <asp:TemplateField HeaderText="Name">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPostTitle" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("posttitle"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtPostTitle" MaxLength="32" runat="server"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="titleLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("posttitle"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Category">
                <EditItemTemplate>
                    <asp:DropDownList ID="catDropdown" AutoPostBack="False" runat="server" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:DropDownList ID="catDropdown" AutoPostBack="False" runat="server" />
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="catLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("catname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Brief">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPostBrief" MaxLength="1024" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("postbrief").ToString()) ? "<i>Undefined</i>" : Server.HtmlDecode((string) Eval("postbrief"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtPostBrief" MaxLength="1024" runat="server"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="briefLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("postbrief").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("postbrief"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Body">
                <EditItemTemplate>
                    <asp:TextBox ID="txtPostBody" runat="server" ClientIDMode="Static" TextMode="MultiLine" Rows="30" Style="width: 95%" CssClass="tinymce" Text='<%# Server.HtmlDecode((string) Eval("postbody")) %>' />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtPostBody" runat="server" ClientIDMode="Static" TextMode="MultiLine" Rows="30" Style="width: 95%" CssClass="tinymce" />
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Literal ID="bodyOutput" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("postbody").ToString()) ? "<i>Undefined</i>" : Sanitizer.GetSafeHtmlFragment((string) Eval("postbody"))) %>'></asp:Literal>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Enabled">
                <EditItemTemplate>
                    <asp:CheckBox ID="postEnabledCheckbox" runat="server" />
                    <label for="mainContent_postView_postEnabledCheckbox"></label>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="postEnabledCheckbox" runat="server" />
                    <label for="mainContent_postView_postEnabledCheckbox"></label>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="postEnabledLabel" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Editing">
                <EditItemTemplate>
                    <asp:LinkButton ID="updateButton" runat="server" CommandName="update" OnClientClick=" if (!validatePostView()) return false; ">Update</asp:LinkButton>
                    <asp:LinkButton ID="cancelButton" runat="server" CommandName="cancel">Cancel</asp:LinkButton>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:LinkButton ID="insertButton" runat="server" CommandName="insert" OnClientClick=" if (!validatePostView()) return false; ">Save</asp:LinkButton>
                    <asp:LinkButton ID="cancelButton" runat="server" CommandName="cancel">Cancel</asp:LinkButton>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="editButton" runat="server" CommandName="edit">Edit</asp:LinkButton>
                    <asp:LinkButton ID="deleteButton" runat="server" CommandName="delete" OnClientClick=" if (!window.confirm('Are you sure?')) return false; ">Delete</asp:LinkButton>
                    <asp:LinkButton ID="newButton" runat="server" CommandName="new">Add New</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
        <EmptyDataTemplate>
            No results returned.<br />
            <asp:LinkButton ID="newButton" runat="server" CommandName="new">Add New</asp:LinkButton>
        </EmptyDataTemplate>
    </asp:DetailsView>
    <br />
    <h4>Comments</h4>
    <asp:GridView ID="commentGrid" runat="server" AutoGenerateColumns="False" ShowFooter="False" DataKeyNames="commentid"
                  OnRowDeleting="CommentGrid_Delete" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="commentid" HeaderText="Comment ID" InsertVisible="False" ReadOnly="True" SortExpression="commenttime" />
            <asp:TemplateField HeaderText="User">
                <ItemTemplate>
                    <asp:Label ID="commentNameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Post">
                <ItemTemplate>
                    <asp:Label ID="commentTitleLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("posttitle"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Comment">
                <ItemTemplate>
                    <asp:Label ID="commentBodyLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("commentbody").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("commentbody"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="DeleteButton" CausesValidation="False" CommandName="Delete" Text="Delete" OnClientClick=" if (!window.confirm('Are you sure?')) return false; " runat="server"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>