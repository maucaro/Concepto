// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
  // *****
  // Logout due to Inactivity Section
  // *****
  const sessionSlidingTimeoutMinutes = 5;
  const logoutUrl = '/logout.html';

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
