// employeeService.js
// Async employee operations delegated to the API storage layer.

(function (global) {
  var employeeService = {
    getAll: function (query) {
      return global.storageService.getAll(query);
    },

    getById: function (id) {
      return global.storageService.getById(id);
    },

    add: function (data) {
      return global.storageService.add(data);
    },

    update: function (id, data) {
      return global.storageService.update(id, data);
    },

    remove: function (id) {
      return global.storageService.remove(id);
    },

    getDepartments: function () {
      return global.appConfig.DEPARTMENTS.slice();
    }
  };

  global.employeeService = employeeService;
})(window);
