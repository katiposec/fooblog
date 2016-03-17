$(document).ready(function() {
    $('select').material_select();
    $(".dropdown-button").dropdown();
    $('.materialboxed').materialbox();
});

tinymce.init({
    selector: ".tinymce",
    theme: "modern",
    menubar: false,
    resize: false,
    statusbar: false,
    plugins: ["advlist autolink lists charmap preview hr anchor",
        "pagebreak code nonbreaking table contextmenu directionality paste"],
    toolbar1: "styleselect | bold italic underline | pagebreak code preview | undo redo",
    toolbar2: "alignleft aligncenter alignright alignjustify | bullist numlist outdent indent"
});

function isEmail(address) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(address);
}

function stripTags(input) {
    var tmp = document.implementation.createHTMLDocument("New").body;
    tmp.innerHTML = input;
    return tmp.textContent || tmp.innerText || "";
}

function validateSearch() {
    var isValid = true;

    $("input[id^='searchText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Search Empty', 4000);
            isValid = false;
        } else {
            el.value = stripTags(el.value);
        }
    });

    return isValid;
}

function validateMerchView() {
    var isValid = true;

    $("input[id^='mainContent_merchView_imageUploadForm']").each(function(i, el) {
        if (el.value) {
            if (el.files[0].type.indexOf("image") != 0 || el.files[0].size > 2097152) {
                Materialize.toast('Invalid Image', 4000);
                isValid = false;
            }
        }
    });

    $("input[id^='mainContent_merchView_txtMerchName']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Name Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_merchView_txtMerchPrice']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Price Empty', 4000);
            isValid = false;
        } else if (!el.value.trim().match(/^\$?[0-9]+(\.[0-9][0-9])?$/)) {
            Materialize.toast('Invalid Price', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_merchView_txtMerchBrief']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Brief Empty', 4000);
            isValid = false;
        }
    });

    $("textarea[id^='txtMerchBody']").each(function(i, el) {
        tinyMCE.triggerSave();

        if (el.value.length < 1) {
            Materialize.toast('Body Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateCategoryGridInsert() {
    var isValid = true;

    $("input[id^='mainContent_categoryGrid_txtCatNameFooter']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Name Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateCategoryGridUpdate() {
    var isValid = true;

    $("input[id^='mainContent_categoryGrid_txtCatName_']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Name Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validatePostView() {
    var isValid = true;

    $("input[id^='mainContent_postView_txtPostTitle']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Title Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_postView_txtPostBrief']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Brief Empty', 4000);
            isValid = false;
        }
    });

    $("textarea[id^='txtPostBody']").each(function(i, el) {
        tinyMCE.triggerSave();

        if (el.value.length < 1) {
            Materialize.toast('Body Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateUserGridUpdate() {
    var isValid = true;

    $("input[id^='mainContent_userGrid_txtUserName_']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Username Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userGrid_txtUserAlias_']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Alias Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userGrid_txtEmail_']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Email Empty', 4000);
            isValid = false;
        } else if (!isEmail(el.value.trim())) {
            Materialize.toast('Invalid Email', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateUserGridInsert() {
    var isValid = true;

    $("input[id^='mainContent_userGrid_txtUserNameFooter']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Username Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userGrid_txtUserAliasFooter']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Alias Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userGrid_txtEmailFooter']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Email Empty', 4000);
            isValid = false;
        } else if (!isEmail(el.value.trim())) {
            Materialize.toast('Invalid Email', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userGrid_txtUserPasswordFooter']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Password Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function resetPassword() {
    var isValid = true;
    var txtPass = $("#passText").val();
    var txtConfirm = $("#passText_confirm").val();

    if (!txtPass || !txtConfirm) {
        Materialize.toast('Password Empty', 4000);
        isValid = false;
    } else if (!txtPass.trim() || !txtConfirm.trim()) {
        Materialize.toast('Password Empty', 4000);
        isValid = false;
    }

    if (txtPass != txtConfirm) {
        Materialize.toast('Values Must Match', 4000);
        isValid = false;
    }

    if (isValid) {
        var jsonData = JSON.stringify({ Password: txtPass.trim(), Confirmation: txtConfirm.trim() });

        $.ajax({
            url: 'reset_handler.ashx',
            type: 'POST',
            data: jsonData,
            xhrFields: {
                withCredentials: true
            },
            success: function(data) {
                Materialize.toast(data, 4000);
                $("input#passText,input#passText_confirm").val("");
                $("input#passText,input#passText_confirm").blur();
            },
            error: function(errorText) {
                Materialize.toast(errorText, 4000);
                ;
            }
        });
    }
}

function validateUserView() {
    var isValid = true;

    $("input[id^='mainContent_userView_imageUploadForm']").each(function(i, el) {
        if (el.value) {
            var validExt = ['.bmp', '.jpeg', '.jpg', '.gif', '.png', '.tiff'];
            var isValidFileExt = (new RegExp('(' + validExt.join('|').replace(/\./g, '\\.') + ')$')).test(el.value);

            if (!isValidFileExt || el.files[0].size > 2097152) {
                Materialize.toast('Invalid Image', 4000);
                isValid = false;
            }
        }
    });

    $("input[id^='mainContent_userView_txtUserAlias']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Alias Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userView_txtUserEmail']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Email Empty', 4000);
            isValid = false;
        } else if (!isEmail(el.value.trim())) {
            Materialize.toast('Invalid Email', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userView_txtUserAddress']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Address Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userView_txtUserCity']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('City Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userView_txtUserCountry']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Country Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_userView_txtUserBody']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Bio Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function doLogin() {
    var isValid = true;

    $("input[id^='mainContent_usernameText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Username Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_passText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Password Empty', 4000);
            isValid = false;
        }
    });

    if (isValid) {
        var fd = new FormData();
        fd.append("username", $("input[id^='mainContent_usernameText']").val());
        fd.append("password", $("input[id^='mainContent_passText']").val());

        $.ajax({
            url: 'login_handler.ashx',
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            success: function (data) {
                $("input#passText,input#passText_confirm").val("");
                $("input#passText,input#passText_confirm").blur();
                if (data === "OK") {
                    document.location = "index.aspx";
                } else {
                    Materialize.toast(data, 4000);
                }
            },
            error: function (errorText) {
                Materialize.toast(errorText, 4000);
            }
        });
    }
}

function validateRegistration() {
    var isValid = true;

    $("input[id^='mainContent_aliasText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Alias Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_emailText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Email Empty', 4000);
            isValid = false;
        } else if (!isEmail(el.value.trim())) {
            Materialize.toast('Invalid Email', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_addressText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Address Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_cityText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('City Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_countryText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Country Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_usernameText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Username Empty', 4000);
            isValid = false;
        }
    });

    $("input[id^='mainContent_passText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Password Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateReset() {
    var isValid = true;

    $("input[id^='mainContent_emailText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Email Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}

function validateComment() {
    var isValid = true;

    $("input[id^='mainContent_commentText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Comment Empty', 4000);
            isValid = false;
        } else {
            el.value = stripTags(el.value);
        }
    });

    return isValid;
}

function validateReview() {
    var isValid = true;

    $("input[id^='mainContent_reviewText']").each(function(i, el) {
        if (!el.value.trim()) {
            Materialize.toast('Review Empty', 4000);
            isValid = false;
        }
    });

    return isValid;
}