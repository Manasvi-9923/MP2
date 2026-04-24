// dashboardService.js
// Dashboard data fetches for API-backed frontend.

(function (global) {
  var dashboardService = {
    getDashboard: function () {
      return global.storageService.getDashboard();
    }
  };

  global.dashboardService = dashboardService;
})(window);

