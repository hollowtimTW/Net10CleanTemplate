// YourApp Razor site scripts.
// Add interactivity here. Keep it framework-free.

window.YourApp = window.YourApp || {};

YourApp.confirmAction = function(message) {
    return window.confirm(message || 'Are you sure?');
};