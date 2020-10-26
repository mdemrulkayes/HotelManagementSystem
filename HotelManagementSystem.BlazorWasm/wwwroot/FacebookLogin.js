//window.fbAsyncInit = function() {
//        FB.init({
//            appId: '795201797944860',
//            cookie: true,
//            xfbml: true,
//            version: 'v3.2'
//        });

//        //FB.AppEvents.logPageView();

//        FB.getLoginStatus(function (response) {
//            if (response.status === 'connected') {
//                //display user data
//                getFbUserData();
//            }
//        });

//    };

//(function(d, s, id){
//    var js, fjs = d.getElementsByTagName(s)[0];
//    if (d.getElementById(id)) {return;}
//    js = d.createElement(s); js.id = id;
//    js.src = "https://connect.facebook.net/en_US/sdk.js";
//    fjs.parentNode.insertBefore(js, fjs);
//}(document, 'script', 'facebook-jssdk'));


function fbLogin() {
    FB.login(function (response) {
        if (response.authResponse) {
            // Get and display the user profile data
            getFbUserData();
        } else {
            return null;
            //document.getElementById('status').innerHTML = 'User cancelled login or did not fully authorize.';
        }
    }, { scope: 'email' });
}

function getFbUserData() {
    FB.api('/me', { locale: 'en_US', fields: 'id,first_name,last_name,email,link,gender,locale,picture' },
        function (response) {
            DotNet.invokeMethodAsync('HotelManagementSystem.BlazorWasm', 'FbLoginProcessCallback', response);
        });
}

// Logout from facebook
function fbLogout() {
    FB.logout(function () {
        DotNet.invokeMethodAsync('HotelManagementSystem.BlazorWasm', 'FbLogOutCallback');
    });
}

//window.fbLogin = function () {
//    window.FB.login(function (response) {
//        if (response.status === 'connected') {
//            //console.log(response)            
//        } else {
//           // console.log("No se logeo");
//        }

//        DotNet.invokeMethodAsync('HotelManagementSystem.BlazorWasm', 'FbLoginProcessCallback', response);

//    }, { scope: 'public_profile, email' });
//}