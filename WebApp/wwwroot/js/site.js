// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
  const apiKey = 'AIzaSyCfNvnpm7ji0P4PXMD0Cfjed7PJ1r_BlmM';
  const appName = '[DEFAULT]'
  // sessionSlidingTimeoutMinutes = the number of minutes when the cookie expires 
  // and the number of minutes prior to token expiration that the refresh_token will be used to obtain an new id_token
  const sessionSlidingTimeoutMinutes = 5;
  const refreshTokenUrl = 'https://securetoken.googleapis.com/v1/token?key=';
  const firebaseCookie = 'firebaseAccessToken';
  const logoutUrl = '/logout.html';

  // *****
  // Refresh Token Section
  // *****

  let key = `firebase:authUser:${apiKey}:${appName}`
  let record = JSON.parse(sessionStorage.getItem(key));
  if (record) {
    let refreshToken = record.stsTokenManager.refreshToken;
    let expTime = record.stsTokenManager.expirationTime;
    let expiresInMinutes = (expTime - Date.now()) / 60000;
    if (expiresInMinutes < 60 - sessionSlidingTimeoutMinutes) {
      jQuery.ajax({
        url: refreshTokenUrl + apiKey,
        type: 'POST',
        data: {
          'grant_type': 'refresh_token',
          'refresh_token': refreshToken
        }
      }).done(data => ProcessTokens(data));
    }
    //TODO: Remove once testing is done
    document.getElementById('expirationTime').textContent = 'Expires in: ' + expiresInMinutes.toString();
  }

  function ProcessTokens(data) {
    let idToken = data.id_token;
    let refreshToken = data.refresh_token;
    record.stsTokenManager.refreshToken = refreshToken;
    record.stsTokenManager.idToken = idToken;
    // Firebase tokens expire in 1 hour (3,600,000 milliseconds)
    // The following is approximate but good enough for what we need as our granularity is in minutes
    // So no need to decode the token to get its actual expiration
    record.stsTokenManager.expirationTime = Date.now() + 3600000;
    sessionStorage.setItem(key, JSON.stringify(record));
    document.cookie = `${firebaseCookie}=${idToken}; path=/; samesite=strict; secure=true;`;
  }

  // *****
  // Logout due to Inactivity Section
  // *****
  let timeoutId;

  function startTimer() {
    // window.setTimeout returns an Id that can be used to start and stop a timer
    timeoutId = window.setTimeout(doInactive, sessionSlidingTimeoutMinutes * 60000)
  }

  function resetTimer() {
    window.clearTimeout(timeoutId)
    startTimer();
  }

  function doInactive() {
    window.location.href = logoutUrl;
  }

  function setupTimers() {
    document.addEventListener("mousemove", resetTimer, false);
    document.addEventListener("mousedown", resetTimer, false);
    document.addEventListener("keypress", resetTimer, false);
    document.addEventListener("touchmove", resetTimer, false);

    startTimer();
  }

  setupTimers();

}).call(this)
