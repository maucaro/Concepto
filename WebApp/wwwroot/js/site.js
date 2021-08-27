// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
  // expirationMarginMinutes = the number of minutes prior to expiration that the refresh_token will be used to obtain an new id_token
  const expirationMarginMinutes = 5;
  const refreshTokenUrl = 'https://securetoken.googleapis.com/v1/token?key=';
  const firebaseLocalStorageDb = 'firebaseLocalStorageDb';
  const firebaseObjectStore = 'firebaseLocalStorage';
  const firbaseCookie = 'firebaseAccessToken';
  let openDB = window.indexedDB.open(firebaseLocalStorageDb, 1);
  let db;
  let record;

  openDB.onerror = function (event) {
    console.log('Error opening IndexedDB');
  };
  openDB.onsuccess = function (event) {
    db = event.target.result;
    if (db.objectStoreNames.contains(firebaseObjectStore)) {
      let transaction = db.transaction(firebaseObjectStore, 'readonly');
      let firebaseLocalStorage = transaction.objectStore(firebaseObjectStore);
      firebaseLocalStorage.getAll().onsuccess = function (res) {
        if (res.target.result.length > 0) {
          record = res.target.result[0];
          let refreshToken = record.value.stsTokenManager.refreshToken;
          let expTime = record.value.stsTokenManager.expirationTime;
          let apiKey = record.value.apiKey;
          let expiresInMinutes = (expTime - Date.now()) / 60000;
          if (expiresInMinutes < expirationMarginMinutes) {
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
          //document.getElementById('expirationTime').textContent = 'Expires in: ' + expiresInMinutes.toString();
        }
      }
    }
  }

  function ProcessTokens(data) {
    let transaction = db.transaction(firebaseObjectStore, 'readwrite');
    let firebaseLocalStorage = transaction.objectStore(firebaseObjectStore);

    let idToken = data.id_token;
    let refreshToken = data.refresh_token;
    record.value.stsTokenManager.refreshToken = refreshToken;
    record.value.stsTokenManager.idToken = idToken;
    // Firebase tokens expire in 1 hour (3,600,000 milliseconds)
    // The following is approximate but good enough for what we need as our granularity is in minutes
    // So no need to decode the token to get its actual expiration
    record.value.stsTokenManager.expirationTime = Date.now() + 3600000;
    firebaseLocalStorage.put(record);
    document.cookie = `${firbaseCookie}=${idToken}; path=/; SameSite=Strict`;
  }
}).call(this)
