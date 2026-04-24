// authService.js
// API-backed authentication with in-memory session storage.

(function (global) {
  var session = null;

  function decodeJwtRole(token) {
    if (!token) {
      return "";
    }

    try {
      var payloadPart = token.split(".")[1];
      if (!payloadPart) {
        return "";
      }

      var base64 = payloadPart.replace(/-/g, "+").replace(/_/g, "/");
      while (base64.length % 4 !== 0) {
        base64 += "=";
      }

      var payload = JSON.parse(atob(base64));
      return payload.role ||
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        "";
    } catch (error) {
      return "";
    }
  }

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

      var decodedRole = decodeJwtRole(result.token);
      session = {
        username: result.username,
        role: decodedRole || result.role,
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

