// authService.js
// API-backed authentication with in-memory session storage.

(function (global) {
  var session = null;

  var authService = {
    signup: async function (username, password, role) {
      return global.storageService.register({
        username: username,
        password: password,
        role: role || "Viewer"
      });
    },

    login: async function (username, password) {
      var result = await global.storageService.login({
        username: username,
        password: password
      });

      session = {
        username: result.username,
        role: result.role,
        token: result.token
      };

      return result;
    },

    logout: function () {
      session = null;
    },

    isLoggedIn: function () {
      return !!(session && session.token);
    },

    isAdmin: function () {
      return !!(session && session.role === "Admin");
    },

    getRole: function () {
      return session ? session.role : "";
    },

    getToken: function () {
      return session ? session.token : "";
    },

    getCurrentUser: function () {
      return session ? { username: session.username, role: session.role } : null;
    }
  };

  global.authService = authService;
})(window);

