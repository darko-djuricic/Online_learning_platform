//GLOBAL JSON FOR VALIDATING FIELDS OF REGISTRATION
let check = { username: false, password: false, repassword: false, email: false };
//REGEX FOR PASSWORD VALIDATION
const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$/
const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

$("#startNav").remove();

let username, password, repassword, email;

$(document).ready(function () {
    //CHANGING VIEW OF VALID/INVALID INPUT
    function GoodOrNot(selector, bool) {
        $(selector).attr("class", `form-control is-${bool ? "valid" : "invalid"}`)
    }

    //CHECKING USERNAME INPUT
    function usernameRegister() {
        username = $("#usernameRegister").val()
        let bool = username.length > 4
        check.username = bool
        $("#userInvalid").text("Too short")
        GoodOrNot("#usernameRegister", bool)
    }

    //CHECKING EMAIL INPUT
    function emailRegister() {
        email = $("#emailRegister").val()
        let bool = emailRegex.exec(email) != null
        check.email = bool
        $("#emailInvalid").text("Bad format")
        GoodOrNot("#emailRegister", bool)
    }

    //CHECKING PASSWORD INPUT 
    function passwordRegister() {
        password = $("#password2").val();
        const bool = regex.exec(password) != null;
        check.password = bool
        GoodOrNot("#password2", bool)
        confirmPasswd();
    }

    //CHECKING CONFIRM PASSWORD INPUT WHILE TYPING
    function confirmPasswd() {
        repassword = $("#repassword2").val();
        if (repassword != "") {
            let bool = check.password && (repassword == password);
            check.repassword = bool;
            GoodOrNot("#repassword2", bool)
        }
    }


    $("#usernameRegister")
        .keyup(usernameRegister)
        .keypress(usernameRegister)
        .blur(usernameRegister)
        .change(usernameRegister)
        .focus(usernameRegister);


    $("#emailRegister")
        .keyup(emailRegister)
        .keypress(emailRegister)
        .blur(emailRegister)
        .change(emailRegister);

    $("#password2")
        .keyup(passwordRegister)
        .keypress(passwordRegister)
        .blur(passwordRegister)
        .change(passwordRegister);

    $("#repassword2")
        .keyup(confirmPasswd)
        .keypress(confirmPasswd)
        .blur(confirmPasswd)
        .change(confirmPasswd);


    //REGISTRATION
    $("#btnRegister").click(() => {
        $("#form1").submit();
    })
});