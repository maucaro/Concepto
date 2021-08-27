// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let openDB = window.indexedDB.open('firebaseLocalStorageDb', 1);
let db;
let record;

openDB.onerror = function (event) {
  console.log('Error opening IndexedDB?!');
};
openDB.onsuccess = function (event) {
  db = event.target.result;
  if (db.objectStoreNames.contains('firebaseLocalStorage')) {
    let transaction = db.transaction('firebaseLocalStorage', 'readonly');
    let firebaseLocalStorage = transaction.objectStore('firebaseLocalStorage');
    firebaseLocalStorage.getAll().onsuccess = function (res) {
      if (res.target.result.length > 0) {
        record = res.target.result[0];
        let refreshToken = record.value.stsTokenManager.refreshToken;
        let expTime = record.value.stsTokenManager.expirationTime;
        let apiKey = record.value.apiKey;
        let expiresInMinutes = (expTime - Date.now()) / 60000;
        if (expiresInMinutes < 59) {
          jQuery.ajax({
            url: 'https://securetoken.googleapis.com/v1/token?key=' + apiKey,
            type: 'POST',
            data: {
              "grant_type": "refresh_token",
              "refresh_token": refreshToken
            }
          }).done(data => ProcessTokens(data));
        }
        document.getElementById('expirationTime').textContent = expiresInMinutes;
      }
    }
  }
}

function ProcessTokens(data) {
  let transaction = db.transaction('firebaseLocalStorage', 'readwrite');
  let firebaseLocalStorage = transaction.objectStore('firebaseLocalStorage');

  let idToken = data.id_token;
  let refreshToken = data.refresh_token;
  record.value.stsTokenManager.refreshToken = refreshToken;
  record.value.stsTokenManager.idToken = idToken;
  // The following is approximate but good enough for what we need as our granularity is minutes
  record.value.stsTokenManager.expirationTime = Date.now() + 3600000;
  firebaseLocalStorage.put(record);
  jQuery.ajax({
    url: '/refreshtoken',
    type: 'POST',
    data: idToken
  });
}

