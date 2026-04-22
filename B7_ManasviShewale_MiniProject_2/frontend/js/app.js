// app.js
// Event orchestration for the API-backed frontend.

(function (global, $) {
  var authService = global.authService;
  var employeeService = global.employeeService;
  var validationService = global.validationService;
  var dashboardService = global.dashboardService;
  var uiService = global.uiService;

  var state = {
    search: "",
    department: "all",
    status: "all",
    sortBy: "name",
    sortDir: "asc",
    page: 1,
    pageSize: global.appConfig.PAGE_SIZE,
    deleteId: null
  };
  var searchTimer = null;

  function init() {
    uiService.populateDepartmentOptions(employeeService.getDepartments());
    bindNav();
    bindAuthForms();
    bindEmployeeEvents();
    bindFieldErrorReset();
    showAuthView();
  }

  function bindNav() {
    $("a.nav-link[data-view]").on("click", function (event) {
      event.preventDefault();
      if (!ensureAuthenticated()) {
        return;
      }

      uiService.showView($(this).data("view"));
    });

    $("#showSignupLink").on("click", function (event) {
      event.preventDefault();
      uiService.showSignupCard();
      uiService.clearInlineErrors();
    });

    $("#showLoginLink").on("click", function (event) {
      event.preventDefault();
      uiService.showLoginCard();
      uiService.clearInlineErrors();
    });

    $("#logoutBtn").on("click", function () {
      authService.logout();
      state.deleteId = null;
      state.page = 1;
      showAuthView();
      uiService.showToast("Logged out.", "info");
    });

    $("#addEmployeeNavBtn").on("click", function () {
      if (!authService.isAdmin()) {
        return;
      }

      uiService.clearForm();
      uiService.showModal("employeeForm");
    });
  }

  function bindAuthForms() {
    $("#signupForm").on("submit", async function (event) {
      event.preventDefault();
      uiService.clearInlineErrors();

      var payload = {
        username: $("#signupUsername").val().trim(),
        password: $("#signupPassword").val(),
        confirmPassword: $("#signupConfirmPassword").val(),
        role: $("#signupRole").val(),
        isSignup: true
      };

      var errors = validationService.validateAuthForm(payload);
      if (Object.keys(errors).length) {
        uiService.showInlineErrors(errors, "signup");
        return;
      }

      try {
        await authService.signup(payload.username, payload.password, payload.role);
        uiService.showToast("Registration successful. Please log in.", "success");
        uiService.showLoginCard();
        $("#loginUsername").val(payload.username);
        $("#loginPassword").val("");
      } catch (error) {
        var signupErrors = validationService.mapServerErrors(error);
        if (Object.keys(signupErrors).length) {
          uiService.showInlineErrors(signupErrors, "signup");
        } else {
          uiService.showToast(error.message, "error");
        }
      }
    });

    $("#loginForm").on("submit", async function (event) {
      event.preventDefault();
      uiService.clearInlineErrors();

      var payload = {
        username: $("#loginUsername").val().trim(),
        password: $("#loginPassword").val()
      };

      var errors = validationService.validateAuthForm(payload);
      if (Object.keys(errors).length) {
        uiService.showInlineErrors(errors, "login");
        return;
      }

      try {
        await authService.login(payload.username, payload.password);
        showAppView();
        await refreshAll();
        uiService.showToast("Login successful.", "success");
      } catch (error) {
        uiService.showInlineErrors({
          username: error.message,
          password: error.message
        }, "login");
      }
    });
  }

  function bindEmployeeEvents() {
    $("#searchInput").on("input", function () {
      state.search = $(this).val().trim();
      state.page = 1;
      clearTimeout(searchTimer);
      searchTimer = setTimeout(refreshEmployeesTable, 350);
    });

    $("#departmentFilter").on("change", function () {
      state.department = $(this).val();
      state.page = 1;
      refreshEmployeesTable();
    });

    $("input[name='statusFilter']").on("change", function () {
      state.status = $(this).val();
      state.page = 1;
      refreshEmployeesTable();
    });

    $("#sortSelect").on("change", function () {
      var parts = String($(this).val()).split("-");
      state.sortBy = parts[0];
      state.sortDir = parts[1] || "asc";
      state.page = 1;
      refreshEmployeesTable();
    });

    $("#paginationContainer").on("click", "button[data-page]", function () {
      var page = Number($(this).data("page"));
      if (page > 0 && page !== state.page) {
        state.page = page;
        refreshEmployeesTable();
      }
    });

    $("#employeesTableBody").on("click", "button[data-action]", async function () {
      var action = $(this).data("action");
      var id = Number($(this).data("id"));

      if (action === "view") {
        uiService.showModal("viewEmployee", await employeeService.getById(id));
        return;
      }

      if (!authService.isAdmin()) {
        return;
      }

      if (action === "edit") {
        openEditEmployee(await employeeService.getById(id));
      } else if (action === "delete") {
        var current = await employeeService.getById(id);
        state.deleteId = id;
        uiService.showModal("confirmDelete", {
          id: id,
          fullName: current.fullName || (current.firstName + " " + current.lastName)
        });
      }
    });

    $("#employeeForm").on("submit", async function (event) {
      event.preventDefault();
      uiService.clearInlineErrors();

      var idValue = $("#employeeId").val();
      var payload = {
        firstName: $("#firstName").val().trim(),
        lastName: $("#lastName").val().trim(),
        email: $("#email").val().trim(),
        phone: $("#phone").val().trim(),
        department: $("#department").val(),
        designation: $("#designation").val().trim(),
        salary: Number($("#salary").val()),
        joinDate: $("#joinDate").val(),
        status: $("#status").val()
      };

      var errors = validationService.validateEmployeeForm(payload);
      if (Object.keys(errors).length) {
        uiService.showInlineErrors(errors, "employee");
        return;
      }

      try {
        if (idValue) {
          await employeeService.update(Number(idValue), payload);
          uiService.showToast("Employee updated.", "success");
        } else {
          await employeeService.add(payload);
          uiService.showToast("Employee added.", "success");
        }

        uiService.hideModal("#employeeFormModal");
        uiService.clearForm();
        await refreshAll();
      } catch (error) {
        var mapped = validationService.mapServerErrors(error);
        if (Object.keys(mapped).length) {
          uiService.showInlineErrors(mapped, "employee");
        } else {
          uiService.showToast(error.message, "error");
        }
      }
    });

    $("#confirmDeleteBtn").on("click", async function () {
      if (!state.deleteId) {
        return;
      }

      try {
        await employeeService.remove(state.deleteId);
        uiService.hideModal("#deleteConfirmModal");
        uiService.showToast("Employee deleted.", "success");
        state.deleteId = null;
        if (state.page > 1) {
          state.page = 1;
        }
        await refreshAll();
      } catch (error) {
        uiService.showToast(error.message, "error");
      }
    });
  }

  function bindFieldErrorReset() {
    $("input, select").on("input change", function () {
      var id = $(this).attr("id");
      if (id) {
        uiService.clearFieldError(id);
      }
    });
  }

  function ensureAuthenticated() {
    if (authService.isLoggedIn()) {
      return true;
    }

    showAuthView();
    uiService.showToast("Please login first.", "info");
    return false;
  }

  function showAuthView() {
    uiService.showAuthSection();
    uiService.setAuthenticatedUI(false);
    uiService.showLoginCard();
    uiService.clearInlineErrors();
  }

  function showAppView() {
    uiService.showAppSection();
    uiService.setAuthenticatedUI(true);
    uiService.applyRoleUI(authService.isAdmin(), authService.getRole());
    uiService.showView("dashboardView");
  }

  async function refreshAll() {
    await Promise.all([refreshDashboard(), refreshEmployeesTable()]);
  }

  async function refreshDashboard() {
    var dashboard = await dashboardService.getDashboard();
    uiService.renderDashboard(dashboard);
  }

  async function refreshEmployeesTable() {
    if (!authService.isLoggedIn()) {
      return;
    }

    uiService.setTableStates(true, false);

    try {
      var result = await employeeService.getAll({
        search: state.search || undefined,
        department: state.department === "all" ? undefined : state.department,
        status: state.status === "all" ? undefined : state.status,
        sortBy: state.sortBy,
        sortDir: state.sortDir,
        page: state.page,
        pageSize: state.pageSize
      });

      var normalized = {
        items: result.items || result.data || [],
        totalCount: result.totalCount || 0,
        page: result.page || state.page,
        pageSize: result.pageSize || state.pageSize,
        totalPages: result.totalPages || 0,
        hasNextPage: typeof result.hasNextPage === "boolean" ? result.hasNextPage : (result.page || state.page) < (result.totalPages || 0),
        hasPrevPage: typeof result.hasPrevPage === "boolean" ? result.hasPrevPage : (result.page || state.page) > 1
      };

      if (normalized.totalPages > 0 && normalized.page > normalized.totalPages) {
        state.page = normalized.totalPages;
        return refreshEmployeesTable();
      }

      state.page = normalized.page;
      uiService.renderEmployeeTable(normalized, authService.isAdmin());
      uiService.renderPagination(normalized, normalized.page);
      uiService.renderPaginationSummary(normalized);
    } catch (error) {
      uiService.setTableStates(false, true);
      uiService.showToast(error.message, "error");
    }
  }

  function openEditEmployee(employee) {
    if (!employee) {
      return;
    }

    uiService.populateForm(employee);
    $("#employeeFormModalLabel").text("Edit Employee");
    $("#employeeFormSubmitBtn").text("Update Employee");
    uiService.showModal("employeeForm");
  }

  $(init);
})(window, jQuery);

