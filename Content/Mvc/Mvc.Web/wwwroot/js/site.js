// YourApp MVC site scripts.
window.YourApp = window.YourApp || {};

YourApp.confirmAction = function(message) {
    return window.confirm(message || 'Are you sure?');
};