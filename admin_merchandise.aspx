<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="admin_merchandise.aspx.cs" Inherits="FooBlog.admin_merchandise" %>
<%@ Import Namespace="FooBlog" %>
<%@ Import Namespace="Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Merchandise Management</h3>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <asp:GridView ID="merchGrid" runat="server" AutoGenerateColumns="False" ShowFooter="False" DataKeyNames="merchid"
                  OnSelectedIndexChanged="MerchGrid_SelectedIndexChanged" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="merchid" HeaderText="Post ID" InsertVisible="False" ReadOnly="True" SortExpression="merchid" />
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:Label ID="nameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions" ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="SelectButton" CausesValidation="False" CommandName="Select" Text="Select" runat="server"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br/><br/>
    <asp:DetailsView ID="merchView" DataKeyNames="merchid" AllowPaging="False" AutoGenerateRows="False" runat="server" OnDataBound="MerchView_Databound" OnItemInserting="MerchView_ItemInserting" OnModeChanging="MerchView_ModeChanging" OnItemDeleting="MerchView_ItemDeleting" OnItemUpdating="MerchView_ItemUpdating" Width="720px">
        <Fields>
            <asp:BoundField DataField="merchid" HeaderText="Post ID" InsertVisible="False" ReadOnly="True" SortExpression="merchid" />
            <asp:TemplateField HeaderText="Name">
                <EditItemTemplate>
                    <asp:TextBox ID="txtMerchName" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("merchname"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtMerchName" MaxLength="32" runat="server"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="nameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Price">
                <EditItemTemplate>
                    <asp:TextBox ID="txtMerchPrice" MaxLength="16" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchprice").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("merchprice"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtMerchPrice" MaxLength="16" runat="server"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="priceLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchprice").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchprice"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Image">
                <EditItemTemplate>
                    <div class="file-field input-field">
                        <div class="waves-effect waves-light blue lighten-1 btn">
                            <span>Select</span>
                            <asp:FileUpload ID="imageUploadForm" runat="server" />
                        </div>
                        <div class="file-path-wrapper">
                            <input class="file-path validate" type="text">
                        </div>
                    </div>
                    <small>File must be an image less than 2MB.</small>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <div class="file-field input-field">
                        <div class="waves-effect waves-light blue lighten-1 btn">
                            <span>Select</span>
                            <asp:FileUpload ID="imageUploadForm" runat="server" />
                        </div>
                        <div class="file-path-wrapper">
                            <input class="file-path validate" type="text">
                        </div>
                    </div>
                    <small>File must be an image less than 2MB.</small>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Image ID="merchImg" ImageUrl='<%# (String.IsNullOrEmpty(Eval("merchimg").ToString()) ? FooStringHelper.MakeImageUrl("merch_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("merchimg"))) %>' Width="150px" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Brief">
                <EditItemTemplate>
                    <asp:TextBox ID="txtMerchBrief" MaxLength="1024" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchbrief").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("merchbrief"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtMerchBrief" MaxLength="1024" runat="server"></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="briefLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchbrief").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchbrief"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Body">
                <EditItemTemplate>
                    <asp:TextBox ID="txtMerchBody" runat="server" ClientIDMode="Static" TextMode="MultiLine" Rows="30" Style="width: 95%" CssClass="tinymce" Text='<%# Server.HtmlDecode((string) Eval("merchbody")) %>' />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="txtMerchBody" runat="server" ClientIDMode="Static" TextMode="MultiLine" Rows="30" Style="width: 95%" CssClass="tinymce" />
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Literal ID="bodyOutput" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchbody").ToString()) ? "<i>Undefined</i>" : Sanitizer.GetSafeHtmlFragment((string) Eval("merchbody"))) %>'></asp:Literal>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Enabled">
                <EditItemTemplate>
                    <asp:CheckBox ID="merchEnabledCheckbox" runat="server" />
                    <label for="mainContent_merchView_merchEnabledCheckbox"></label>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="merchEnabledCheckbox" runat="server" />
                    <label for="mainContent_merchView_merchEnabledCheckbox"></label>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="merchEnabledLabel" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Editing">
                <EditItemTemplate>
                    <asp:LinkButton ID="updateButton" runat="server" CommandName="update" OnClientClick=" if (!validateMerchView()) return false; ">Update</asp:LinkButton>
                    <asp:LinkButton ID="cancelButton" runat="server" CommandName="cancel">Cancel</asp:LinkButton>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:LinkButton ID="insertButton" runat="server" CommandName="insert" OnClientClick=" if (!validateMerchView()) return false; ">Save</asp:LinkButton>
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
    <h4>Reviews</h4>
    <asp:GridView ID="reviewGrid" runat="server" AutoGenerateColumns="False" ShowFooter="False" DataKeyNames="reviewid"
                  OnRowDeleting="ReviewGrid_Delete" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>
        <Columns>
            <asp:BoundField DataField="reviewid" HeaderText="Review ID" InsertVisible="False" ReadOnly="True" SortExpression="reviewtime" />
            <asp:TemplateField HeaderText="User">
                <ItemTemplate>
                    <asp:Label ID="reviewNameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Post">
                <ItemTemplate>
                    <asp:Label ID="merchNameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Review">
                <ItemTemplate>
                    <asp:Label ID="reviewBodyLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("reviewbody").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("reviewbody"))) %>'></asp:Label>
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