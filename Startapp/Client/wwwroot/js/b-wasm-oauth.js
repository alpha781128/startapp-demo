function GetFromLocalStorege(key) {
    return localStorage.getItem(key);
}
function SetToLocalStorege(key, value) {
    var oldValue = GetFromLocalStorege(key);
    if (oldValue !== null) {
        localStorage.removeItem(key);
    }
    localStorage.setItem(key, value);
}
function RemoveFromLocalStorege(key) {
    var oldValue = GetFromLocalStorege(key);
    if (oldValue !== null) {
        localStorage.removeItem(key);
    }
}

function generateRandomString(length) {
    var text = "";
    var possible = "ABCDukrvgvGHIimiudctIUDSkoRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
    for (var i = 0; i < length; i++) {
        text += possible.charAt(Math.floor(Math.random() * possible.length));
    }
    return text;
}
function base64URL(string) {
    return string.toString(CryptoJS.enc.Base64).replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_')
}
var __assign = function () {
    __assign = Object.assign || function __assign(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p)) t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var getCrypto = function () {
    return (window.crypto || window.msCrypto);
};
var encode = function (value) { return btoa(value); };
var createRandomString = function () {
    var charset = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_~.';
    var random = '';
    var randomValues = Array.from(getCrypto().getRandomValues(new Uint8Array(43)));
    randomValues.forEach(function (v) { return (random += charset[v % charset.length]); });
    return random;
};
var parseQueryResult = function (queryString) {
    if (queryString.indexOf('#') > -1) {
        queryString = queryString.substr(0, queryString.indexOf('#'));
    }
    var queryParams = queryString.split('&');
    var parsedQuery = {};
    queryParams.forEach(function (qp) {
        var _a = qp.split('='), key = _a[0], val = _a[1];
        parsedQuery[key] = decodeURIComponent(val);
    });
    return __assign(__assign({}, parsedQuery), { expires_in: parseInt(parsedQuery.expires_in) });
};

const getToken = async (url, options) => {
    if (url.length === 0) return;
    var query = $.param(options);
    url += '?' + query
    var xhr = new XMLHttpRequest();

    xhr.open('GET', url, true);
    xhr.withCredentials = false;
    //xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.responseType = 'json';
    xhr.send();

    xhr.onload = function () {
        if (xhr.status !== 200) {
            alert(`Error ${xhr.status}: ${xhr.statusText}`); // e.g. 404: Not Found
        } else {
            SetToLocalStorege('b_wasm_auth', JSON.stringify(xhr.response));
        }
    };
    xhr.onerror = function () {
        alert("Request failed");
    };
};

const getTokenOidc = async (url, options) => {
    if (url.length === 0) return;
    var xhr = new XMLHttpRequest();
    var body = JSON.stringify(options);

    xhr.open('POST', url, true);
    xhr.withCredentials = false;
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.responseType = 'json';
    xhr.send(body);

    xhr.onload = function () {
        if (xhr.status !== 200) { // analyze HTTP status of the response
            alert(`Error ${xhr.status}: ${xhr.statusText}`); // e.g. 404: Not Found
        } else {
            SetToLocalStorege('b_wasm_auth', JSON.stringify(xhr.response));
        }
    };

    xhr.onerror = function () {
        alert("Request failed");
    };
};

const fetchAuthConfig = () => fetch("/auth_config.json");
const auth0Params = async (bool) => {
    var params;
    //const query = window.location.search;
    if (!bool) {
        var stateIn = encode(createRandomString());
        var nonceIn = encode(createRandomString());
        var codeVerifier = generateRandomString(128);
        var codeChallenge = base64URL(CryptoJS.SHA256(codeVerifier));
        params = {
            state: stateIn,
            nonce: nonceIn,
            code_verifier: codeVerifier,
            code_challenge: codeChallenge
        }
        SetToLocalStorege("auth_step", JSON.stringify(params));
    } else {
        params = JSON.parse(GetFromLocalStorege("auth_step"));
    }
    return params;
};
const options = async (provider) => {
    const response = await fetchAuthConfig();
    const config = await response.json();
    const params = config[provider];
    const query = window.location.search;
    var auth_step = query.includes("code=") ? true : false;
    const authParams = await auth0Params(auth_step);
    var opts;
    if (!auth_step) {
        opts = {
            domain: params.authorization_endpoint,
            redirect_uri: params.redirect_uri,
            client_id: params.client_id,
            code_challenge: authParams.code_challenge,
            nonce: authParams.nonce,
            scope: params.scope,
            sessionChecksEnabled: false,
            state: authParams.state
        };
    } else {
        opts = {
            domain: params.token_endpoint,
            redirect_uri: params.redirect_uri,
            client_id: params.client_id,
            client_secret: params.client_secret,
            code_verifier: authParams.code_verifier,
            scope: params.scope,
            response_type: params.response_type,
            oidc: params.oidc
        };
    };
    return opts;
};
const loginWithRedirect = async (options) => {
    authOptions = {
        redirect_uri: options.redirect_uri,
        client_id: options.client_id,
        code_challenge: options.code_challenge,
        code_challenge_method: "S256",
        nonce: options.nonce,
        scope: options.scope,
        response_mode: "query",
        response_type: "code",
        sessionChecksEnabled: options.sessionChecksEnabled,
        state: options.state
    };
    var query = $.param(authOptions);
    var url = options.domain + '?' + query
    window.location.assign(url);
};
const handleRedirectCallback = async (options) => {
    var url = window.location.href;
    var queryStringFragments, _a, code, tokenOptions;
    queryStringFragments = url.split('?').slice(1);
    if (queryStringFragments.length === 0) {
        throw new Error('There are no query params available for parsing.');
    }
    _a = parseQueryResult(queryStringFragments.join('')), code = _a.code;

    tokenOptions = {
        redirect_uri: options.redirect_uri,
        client_id: options.client_id,
        client_secret: options.client_secret,
        code_verifier: options.code_verifier,
        oidc: options.oidc,
        grant_type: 'authorization_code',
        code: code
    };
    // some old versions of the SDK might not have added redirect_uri to the
    // transaction, we dont want the key to be set to undefined.                          
    ChangeUrl();
    if (options.oidc === false) {
        return getToken(options.domain, tokenOptions);
    } else {
        return getTokenOidc(options.domain, tokenOptions);
    }
};

