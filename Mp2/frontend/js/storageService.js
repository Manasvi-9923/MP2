// storageService.js
// Fetch wrapper for API-backed auth and employee data.

(function (global) {
  var config = global.appConfig;

  function buildHeaders(withAuth) {
    var headers = {
      "Content-Type": "application/json"
    };

    if (withAuth !== false && global.authService && global.authService.getToken) {
      var token = global.authService.getToken();
      if (token) {
        headers.Authorization = "Bearer " + token;
      }
    }

    return headers;
  }

  function buildQuery(params) {
    var searchParams = new URLSearchParams();
    Object.keys(params || {}).forEach(function (key) {
      var value = params[key];
      if (value !== undefined && value !== null && value !== "") {
        searchParams.append(key, value);
      }
    });
    var query = searchParams.toString();
    return query ? "?" + query : "";
  }

  async function request(path, options) {
    var response = await fetch(config.API_BASE_URL + path, options);
    var payload = null;

    try {
      payload = await response.json();
    } catch (error) {
      payload = null;
    }

    if (!response.ok) {
      var message = payload && payload.message ? payload.message : "Request failed.";
      var requestError = new Error(message);
      requestError.status = response.status;
      requestError.payload = payload;
      throw requestError;
    }

    return payload;
  }

  var storageService = {
    login: function (credentials) {
      return request("/auth/login", {
        method: "POST",
        headers: buildHeaders(false),
        body: JSON.stringify(credentials)
      });
    },

    register: function (payload) {
      return request("/auth/register", {
        method: "POST",
        headers: buildHeaders(false),
        body: JSON.stringify(payload)
      });
    },

    getAll: function (query) {
      return request("/employees" + buildQuery(query), {
        method: "GET",
        headers: buildHeaders(true)
      });
    },

    getById: function (id) {
      return request("/employees/" + id, {
        method: "GET",
        headers: buildHeaders(true)
      });
    },

    add: function (employee) {
      return request("/employees", {
        method: "POST",
        headers: buildHeaders(true),
        body: JSON.stringify(employee)
      });
    },

    update: function (id, employee) {
      return request("/employees/" + id, {
        method: "PUT",
        headers: buildHeaders(true),
        body: JSON.stringify(employee)
      });
    },

    remove: function (id) {
      return request("/employees/" + id, {
        method: "DELETE",
        headers: buildHeaders(true)
      });
    },

    getDashboard: function () {
      return request("/employees/dashboard", {
        method: "GET",
        headers: buildHeaders(true)
      });
    }
  };

  global.storageService = storageService;
})(window);

