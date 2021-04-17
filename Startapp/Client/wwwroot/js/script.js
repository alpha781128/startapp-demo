/// <reference path="b-wasm-oauth.js" />

(() => { // Syntax of an Immediately-invoked Function Expression (IIFE) 

})()


window.onload = async () => {
    if (window.location.href.includes("ConfirmEmail")) {
        await handleConfirmEmailCallback();
    }
    if (window.location.href.includes("ResetPassword")) {
        await handleRecoverPasswordCallback();
    }
    var provider = GetFromLocalStorege('b_wasm_provider');
    await loginAsync(provider);
};

function RemoveUnusedParams() {
    RemoveFromLocalStorege('b_wasm_provider');
    RemoveFromLocalStorege('b_wasm_auth');
    RemoveFromLocalStorege('auth_step');
    var returnUri = GetFromLocalStorege("returnUri");
    if (returnUri) {
        RemoveFromLocalStorege("returnUri");
        window.location.assign(returnUri);
    }
}

function getTotalItems() {
    var items = GetFromLocalStorege("cart-items");
    if (items === null || items === undefined) {
        $(".cartSpan").text("");
        return;
    };
    var carts = [];
    carts = JSON.parse(items);
    //var Quantity = 0;
    //carts.forEach(function (obj) {
    //    Quantity += obj["Quantity"];
    //});
    //if (Quantity > 0) $(".cartSpan").text(Quantity);
    if (carts.length > 0) $(".cartSpan").text(carts.length);
    else $(".cartSpan").text("");
}

function addToCart(id) {
    var carts = [];
    var local = GetFromLocalStorege("cart-items");
    if (!(local === null || local === undefined)) {
        carts = JSON.parse(local);
    }
    var cart = { Id: id, Quantity: 1 };
    let ex = carts.find(c => c.Id === id);
    if (!(ex === null || ex === undefined)) {
        ex.Quantity += cart.Quantity;
    }
    else {
        carts.unshift(cart);
    }
    SetToLocalStorege("cart-items", JSON.stringify(carts));
    getTotalItems();
}

function storeCartstate(items) {
    SetToLocalStorege("cart-items", items);
    getTotalItems();
}
function removeElement(array, elem) {
    var index = array.indexOf(elem);
    if (index > -1) {
        array.splice(index, 1);
    }
}
function removeBuyingItemsFromCart() {
    var carts = [], cartsv = [];

    var items = GetFromLocalStorege("cart-items");
    if ((items === null || items === undefined)) return;
    carts = JSON.parse(items);

    var itemsv = GetFromLocalStorege("CartDetail");
    if ((itemsv === null || itemsv === undefined)) return;
    cartsv = JSON.parse(itemsv);
    cartsv.Items.forEach(function (item) {
        let ex = carts.find(c => c.Id === item.Id);
        if (!(ex === null || ex === undefined)) {
            removeElement(carts, ex);
        }      
    });
    SetToLocalStorege("cart-items", JSON.stringify(carts));
    getTotalItems();
    RemoveFromLocalStorege("SelectedItems");
    RemoveFromLocalStorege("CartDetail");
}


// Start login using external provider
// more description ...
// when you chose to log with facebook or others social provider
// the process begin here! triggerd by javascript click event on social button
// this calls that function logWithExternalProvider(provider)
// when we obtain token from that provider we need finally to change it with other
// token provided by our own app, for that we need to trigger other proccess automatically...
// but this time the proccess will be triggerd by the initialization of component...
// that component: file Shared/LoginDisplay.razor in client app contain a method ExchangeTokenAsync() called
// after initialization...

/*
 * a social provider use oauth and sometimes oidc on top of oauth...
 * for that we develp a js file "b-wasm-oauth.js" to interact with that...
 * oauth is a framework to exchange a token for use to secure your apps but
 * in our case we use that one to exchange with a token delevred by our own app
 * because our app also use that technology "oauth and oidc" protocole to provide a access token
 * for use internally or to access to others web apps...
*/


const loginAsync = async (provider) => {
    if (!provider) {
        return;
    }
    var isAuthenticated = GetFromLocalStorege("authToken");
    if (!(isAuthenticated === null || isAuthenticated === undefined)) {
        return;
    }
    const query = window.location.search;
    if (query.includes("code=") && query.includes("state=")) {
        await handleRedirectCallback(await options(provider));
    }
};

const login = async (provider) => {
    SetToLocalStorege('b_wasm_provider', provider);
    await loginWithRedirect(await options(provider));
};

function logWithExternalProvider(provider) {
    login(provider);
}

function GetAppStateFromLocalStorege() {
    var confirmEmail = "", recoverCode = "";
    var jsonObj = {};
    var conf = GetFromLocalStorege('ConfirmEmail');
    if (!(conf === null || conf === undefined)) {
        confirmEmail = conf;
    }
    var rec = GetFromLocalStorege('RecoverCode');
    if (!(rec === null || rec === undefined)) {
        recoverCode = rec;
    }
    jsonObj = { confirmEmail, recoverCode };
    return JSON.stringify(jsonObj);
}

function GetAuthParamsFromLocalStorege() {
    var accessToken = "", provider = "", refreshToken = "", expireIn = "";
    var jsonObj = {};
    var auth = GetFromLocalStorege('b_wasm_auth');
    if (!(auth === null || auth === undefined)) {
        auth = JSON.parse(auth);
        accessToken = auth.access_token;
    }
    var p = GetFromLocalStorege('b_wasm_provider');
    if (!(p === null || p === undefined)) {
        provider = p;
    }
    var r = GetFromLocalStorege('refreshToken');
    if (!(r === null || r === undefined)) {
        refreshToken = r;
    }
    var e = GetFromLocalStorege('expireIn');
    if (!(e === null || e === undefined)) {
        expireIn = e;
    }
    jsonObj = { accessToken, provider, refreshToken, expireIn };
    return JSON.stringify(jsonObj);
}
//end login using external provider

// When we click on the link of email message recived on our mail box after subscription 
// the callback gos here
const handleConfirmEmailCallback = async () => {
    var url = window.location.href;
    var queryStringFragments, _a, userId, code, confirmOptions;
    queryStringFragments = url.split('?').slice(1);
    if (queryStringFragments.length === 0) {
        throw new Error('There are no query params available for parsing.');
    }
    _a = parseQueryResult(queryStringFragments.join('')), userId = _a.userId, code = _a.code;

    confirmOptions = {
        userId: userId,
        code: code + "=="
    };

    url = window.location.origin + "/api/account/confirmemail";
    return getConfirmEmail(url, confirmOptions);
};

const getConfirmEmail = async (url, options) => {
    if (url.length === 0) return;
    var query = $.param(options);
    url += '?' + query
    var xhr = new XMLHttpRequest();

    xhr.open('PUT', url, true);
    xhr.withCredentials = false;
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.responseType = 'json';
    xhr.send();

    xhr.onload = function () {
        var type;
        if (xhr.status !== 202) { // analyze HTTP status of the response
            type = "error";
        } else {
            type = "success";
        }
        SetToLocalStorege('ConfirmEmail', type); // We save that param in local storage "ConfirmEmail" 
        //for good user experience "messaging with user" after refresh page called automatically by callback
        window.location.href = window.location.origin;
    };

    xhr.onerror = function () {
        SetToLocalStorege('ConfirmEmail', "error");
    };
};

// When we click on the link of email message recived on our mail box, after demande "recover password" 
// the callback gos here
const handleRecoverPasswordCallback = async () => {
    var url = window.location.href;
    var queryStringFragments, _a, code;
    queryStringFragments = url.split('?').slice(1);
    if (queryStringFragments.length === 0) {
        throw new Error('There are no query params available for parsing.');
    }
    _a = parseQueryResult(queryStringFragments.join('')), code = _a.code;

    SetToLocalStorege('RecoverCode', code); // we save the code of recovery in local storage for later use
    // of course we call remove that code after refresh page immediatly
    // after refresh page called automatically by callback we just face a form with a hidden input "id=code"
    // we complete the form by changing the password, we post the form, we remove code ...
    window.location.href = window.location.origin;
};

function ChangeUrl() {
    var url = window.location.origin;
    window.history.pushState("", "Startapp-pro - Home", url);
}

toastr.options = { //Notification with toaster
    "closeButton": true, // true/false
    "debug": false, // true/false
    "newestOnTop": false, // true/false
    "progressBar": false, // true/false
    "positionClass": "md-toast-bottom-left", // toast-top-right / toast-top-left / toast-bottom-right / toast-bottom-left
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300", // in milliseconds
    "hideDuration": "1000", // in milliseconds
    "timeOut": "5000", // in milliseconds
    "extendedTimeOut": "1000", // in milliseconds
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

function showModalDialog(parameters) { //Notification with Bootstrap Modal
    DotNet.invokeMethodAsync('Startapp.Client', 'ShowModalDialog', parameters);
}

function ShowToaster(type, message) {
    toastr[type](message);
}

function AlertShow(divName, message) {
    var str = '<div class="alert alert-danger alert-div" role="alert">' + message + '</div>';
    ReplaceAfterLoadingData(divName, str);
    $('#' + divName).alert()
    setTimeout(function () {
        $(".alert").alert('close');
    }, 10000);
}


function ConsoleLog(str) {
    console.log(str);
}

function MdbThemeInitialization() {
    // this function initialize component loaded dynamically
    // MDB Lightbox Init
    //$(function () {
    //$("#mdb-lightbox-ui").load("https://localhost:44385/assets/mdb-addons/mdb-lightbox-ui.html");
    //});
    /* WOW.js init */
    //new WOW().init();

    $(document).ready(function () {
        $('.collapsible').collapsible();
        // SideNav Initialization
        $(".button-collapse").sideNav();

        $('.mdb-select').materialSelect();
        // Data Picker Initialization
        //$('.datepicker').datepicker();

        //get total items in my shopping cart
        getTotalItems();

    });
  
}

function MdbSelectInitialization() {
    // Material Select Initialization
    $(document).ready(function () {
        $('.mdb-select').materialSelect();
    });
}

function CollapseNavbar() {
    if ($(".navbar-1 .navbar-collapse.show")) {
        $('.navbar-1 .navbar-collapse').collapse('hide');
    }
}

function ReplaceAfterLoadingData(divName, str) {
    $('#' + divName).empty();
    $('#' + divName).append(str);
}

function DisableButtonOnLoading(divName) {
    var str = '<button id="submitLogin" type="submit" disabled="@formInvalid" class="disabled btn blue-gradient btn-block btn-rounded z-depth-1a"><span class="spinner-border spinner-border-sm mr-2" role="status" aria-hidden="true"></span>Sign in...</button>';
    ReplaceAfterLoadingData(divName, str);
}

function RestoreDefaultAfterLoading(divName) {
    var str = '<button id="submitLogin" type="submit" disabled="@formInvalid" class="btn blue-gradient btn-block btn-rounded z-depth-1a">Sign in</button>';
    ReplaceAfterLoadingData(divName, str);
}

function HideModal(ModalName, collapse) {
    $("#" + ModalName).modal("hide");
    if (collapse) {
        CollapseNavbar();
    }
}

function ShowModal(ModalName) {
    if (ModalName === "loginModal") {
        var returnTo = window.location.href === window.location.origin + '/' ? true : false;
        var returnUri = window.location.href;
        if (!returnTo) {
            SetToLocalStorege("returnUri", returnUri);
        }
    }
    $("#" + ModalName).modal("show");
}

function RefreshDirection(dir) {
    if (dir === "rtl") {
        $("#main-page").removeClass("direction-ltr").addClass("direction-rtl");
    } else {
        $("#main-page").removeClass("direction-rtl").addClass("direction-ltr");
    }
    console.log("Direction: " + dir);
}

$("body").on("click", ".hide-div", (function () {
    $('.hide-div').hide("slow");
}));

$('body').on('click', '.navbar-1 a.nav-collapse', function () { //to collapse navbar when user click on link
    CollapseNavbar();
    $('ul.navbar-nav li.active').removeClass('active');
    $(this).parent().addClass('active');
});

$('body').bind('click', '.main-collapse', function (e) {
    $(e.target).closest(".navbar").length || $(".navbar-collapse.show").length && $(".navbar-collapse.show").collapse("hide");
});

$(document).on("click", "#toggle", function () {
    $('#slide-out').toggleClass("slim");
    if ($('#slide-out').hasClass("slim")) {
        $('#min-slide').removeClass("fa-angle-double-left");
        $('#min-slide').addClass("fa-angle-double-right");
    } else {
        $('#min-slide').removeClass("fa-angle-double-right");
        $('#min-slide').addClass("fa-angle-double-left");
    }
});

// used to load dynamically a big script like PayPal Checkout.js, of courses when needed
// that script "PayPal" weighs more then 1 MB
// loadScript: returns a promise that completes when the script loads
window.loadScript = function (scriptPath) {
    // check list - if already loaded we can ignore
    if (loaded[scriptPath]) {
        console.log(scriptPath + " already loaded");
        // return 'empty' promise
        return new this.Promise(function (resolve, reject) {
            resolve();
        });
    }

    return new Promise(function (resolve, reject) {
        // create JS library script element
        var script = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";
        console.log(scriptPath + " created");

        // flag as loading/loaded
        loaded[scriptPath] = true;

        // if the script returns okay, return resolve
        script.onload = function () {
            console.log(scriptPath + " loaded ok");
            resolve(scriptPath);
        };

        // if it fails, return reject
        script.onerror = function () {
            console.log(scriptPath + " load failed");
            reject(scriptPath);
        }

        // scripts will load at end of body
        document["body"].appendChild(script);
    });
}
// store list of what scripts we've loaded
loaded = [];