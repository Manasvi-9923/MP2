// uiService.js
// DOM rendering and visual feedback for the API-backed frontend.

(function (global, $) {
  var uiService = {
    showLoginCard: function () {
      $("#signupCard").addClass("d-none");
      $("#loginCard").removeClass("d-none");
    },

    showSignupCard: function () {
      $("#loginCard").addClass("d-none");
      $("#signupCard").removeClass("d-none");
    },

    showAuthSection: function () {
      $("#authSection").removeClass("d-none");
      $("#appSection").addClass("d-none");
    },

    showAppSection: function () {
      $("#authSection").addClass("d-none");
      $("#appSection").removeClass("d-none");
    },

    showView: function (viewId) {
      $("#dashboardView, #employeesView").addClass("d-none");
      $("#" + viewId).removeClass("d-none");
      $("a.nav-link[data-view]").removeClass("active");
      $('a.nav-link[data-view="' + viewId + '"]').addClass("active");
    },

    setAuthenticatedUI: function (isAuthenticated) {
      $("#navLinksGroup, #logoutBtn, #roleBadge").toggleClass("d-none", !isAuthenticated);
    },

    applyRoleUI: function (isAdmin, roleLabel) {
      $("#roleBadge").text(roleLabel || "").toggleClass("d-none", !roleLabel);
      $("#addEmployeeNavBtn").toggleClass("d-none", !isAdmin);
      $("#viewerNotice").toggleClass("d-none", isAdmin);
    },

    populateDepartmentOptions: function (departments) {
      var filterOptions = ['<option value="all">All Departments</option>'];
      var formOptions = ['<option value="">Select department</option>'];

      departments.forEach(function (dept) {
        filterOptions.push('<option value="' + dept + '">' + dept + "</option>");
        formOptions.push('<option value="' + dept + '">' + dept + "</option>");
      });

      $("#departmentFilter").html(filterOptions.join(""));
      $("#department").html(formOptions.join(""));
    },

    renderDashboard: function (dashboard) {
      this.renderDashboardCards(dashboard);
      this.renderDepartmentBreakdown(dashboard.departmentBreakdown || []);
      this.renderRecentEmployees(dashboard.recentEmployees || []);
    },

    renderDashboardCards: function (summary) {
      var cards = [
        { title: "Total Employees", value: summary.total, icon: "bi-people-fill", color: "bg-primary" },
        { title: "Active Employees", value: summary.active, icon: "bi-person-check-fill", color: "bg-success" },
        { title: "Inactive Employees", value: summary.inactive, icon: "bi-person-dash-fill", color: "bg-secondary" },
        { title: "Total Departments", value: summary.departments, icon: "bi-building", color: "bg-info" }
      ];

      $("#dashboardCardsRow").html(cards.map(function (card) {
        return '<div class="col-12 col-sm-6 col-lg-3">' +
          '<div class="card card-hover text-white ' + card.color + '">' +
          '<div class="card-body d-flex justify-content-between align-items-center">' +
          '<div><div class="small text-white-50">' + card.title + '</div><div class="fs-3 fw-semibold">' + card.value + '</div></div>' +
          '<i class="bi ' + card.icon + ' fs-1 text-white-50"></i>' +
          '</div></div></div>';
      }).join(""));
    },

    renderDepartmentBreakdown: function (items) {
      var max = items.reduce(function (acc, item) {
        return item.count > acc ? item.count : acc;
      }, 0);

      $("#departmentBreakdownContainer").html(items.map(function (item) {
        var width = max ? Math.round((item.count / max) * 100) : 0;
        return '<div class="dept-row">' +
          '<span class="small fw-semibold" style="min-width: 110px;">' + escapeHtml(item.department) + '</span>' +
          '<div class="dept-bar" style="width:' + width + '%;"></div>' +
          '<span class="badge bg-light text-dark ms-2">' + item.count + '</span>' +
          '</div>';
      }).join("") || '<p class="text-muted mb-0 small">No employees to show.</p>');
    },

    renderRecentEmployees: function (items) {
      $("#recentEmployeesContainer").html(items.map(function (item) {
        return '<div class="recent-item">' +
          '<div><div class="fw-semibold">' + escapeHtml(item.fullName) + '</div><div class="small text-muted">' + escapeHtml(item.designation) + '</div></div>' +
          '<div class="text-end"><span class="badge ' + getDeptBadgeClass(item.department) + ' me-1">' + escapeHtml(item.department) + '</span>' +
          '<span class="badge ' + getStatusClass(item.status) + '">' + escapeHtml(item.status) + '</span></div>' +
          '</div>';
      }).join("") || '<p class="text-muted mb-0 small">No recent employees.</p>');
    },

    renderEmployeeTable: function (pagedResult, isAdmin) {
      var items = pagedResult.items || pagedResult.data || [];
      var $tbody = $("#employeesTableBody");
      $tbody.empty();

      if (!items.length) {
        this.setTableStates(false, true);
        return;
      }

      this.setTableStates(false, false);

      items.forEach(function (employee) {
        var actionButtons = [
          '<button class="btn btn-sm btn-outline-secondary me-1" data-action="view" data-id="' + employee.id + '"><i class="bi bi-eye"></i></button>'
        ];

        if (isAdmin) {
          actionButtons.push('<button class="btn btn-sm btn-outline-primary me-1" data-action="edit" data-id="' + employee.id + '"><i class="bi bi-pencil"></i></button>');
          actionButtons.push('<button class="btn btn-sm btn-outline-danger" data-action="delete" data-id="' + employee.id + '"><i class="bi bi-trash"></i></button>');
        }

        $tbody.append(
          '<tr>' +
          '<td>' + employee.id + '</td>' +
          '<td>' + escapeHtml(employee.fullName || (employee.firstName + " " + employee.lastName)) + '</td>' +
          '<td>' + escapeHtml(employee.email) + '</td>' +
          '<td><span class="badge ' + getDeptBadgeClass(employee.department) + '">' + escapeHtml(employee.department) + '</span></td>' +
          '<td>' + escapeHtml(employee.designation) + '</td>' +
          '<td>' + formatCurrency(employee.salary) + '</td>' +
          '<td>' + formatDate(employee.joinDate) + '</td>' +
          '<td><span class="badge ' + getStatusClass(employee.status) + '">' + escapeHtml(employee.status) + '</span></td>' +
          '<td class="text-center">' + actionButtons.join("") + '</td>' +
          '</tr>'
        );
      });
    },

    renderPagination: function (pagedResult, currentPage) {
      var totalPages = pagedResult.totalPages || 0;
      var $container = $("#paginationContainer");
      $container.empty();

      if (totalPages <= 1) {
        return;
      }

      appendPageItem($container, "Prev", currentPage - 1, currentPage <= 1, false);
      for (var page = 1; page <= totalPages; page += 1) {
        appendPageItem($container, String(page), page, false, page === currentPage);
      }
      appendPageItem($container, "Next", currentPage + 1, currentPage >= totalPages, false);
    },

    renderPaginationSummary: function (pagedResult) {
      var totalCount = pagedResult.totalCount || 0;
      if (!totalCount) {
        $("#employeeCountLabel").text("0 employees");
        $("#paginationSummary").text("");
        return;
      }

      var items = pagedResult.items || [];
      var start = ((pagedResult.page - 1) * pagedResult.pageSize) + 1;
      var end = Math.min(start + items.length - 1, totalCount);

      $("#employeeCountLabel").text("Showing " + start + "-" + end + " of " + totalCount + " employees");
      $("#paginationSummary").text("Page " + pagedResult.page + " of " + pagedResult.totalPages);
    },

    renderViewEmployee: function (employee) {
      if (!employee) {
        $("#viewEmployeeContent").html('<p class="text-muted mb-0 small">Employee not found.</p>');
        return;
      }

      $("#viewEmployeeContent").html(
        '<div class="row g-3">' +
        '<div class="col-md-6"><div class="mb-2"><span class="text-muted small d-block">Full Name</span><span class="fw-semibold">' + escapeHtml(employee.fullName || (employee.firstName + " " + employee.lastName)) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Email</span><span>' + escapeHtml(employee.email) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Phone</span><span>' + escapeHtml(employee.phone) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Join Date</span><span>' + formatDate(employee.joinDate) + '</span></div></div>' +
        '<div class="col-md-6"><div class="mb-2"><span class="text-muted small d-block">Department</span><span class="badge ' + getDeptBadgeClass(employee.department) + '">' + escapeHtml(employee.department) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Designation</span><span>' + escapeHtml(employee.designation) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Salary</span><span>' + formatCurrency(employee.salary) + '</span></div>' +
        '<div class="mb-2"><span class="text-muted small d-block">Status</span><span class="badge ' + getStatusClass(employee.status) + '">' + escapeHtml(employee.status) + '</span></div></div>' +
        '</div>'
      );
    },

    populateForm: function (employee) {
      $("#employeeId").val(employee ? employee.id : "");
      $("#firstName").val(employee ? employee.firstName : "");
      $("#lastName").val(employee ? employee.lastName : "");
      $("#email").val(employee ? employee.email : "");
      $("#phone").val(employee ? employee.phone : "");
      $("#department").val(employee ? employee.department : "");
      $("#designation").val(employee ? employee.designation : "");
      $("#salary").val(employee ? employee.salary : "");
      $("#joinDate").val(employee ? toInputDate(employee.joinDate) : "");
      $("#status").val(employee ? employee.status : "");
    },

    clearForm: function () {
      $("#employeeForm")[0].reset();
      $("#employeeId").val("");
      this.clearInlineErrors();
      $("#employeeFormModalLabel").text("Add Employee");
      $("#employeeFormSubmitBtn").text("Add Employee");
    },

    showModal: function (type, data) {
      if (type === "employeeForm") {
        bootstrap.Modal.getOrCreateInstance($("#employeeFormModal")[0]).show();
      } else if (type === "viewEmployee") {
        this.renderViewEmployee(data);
        bootstrap.Modal.getOrCreateInstance($("#viewEmployeeModal")[0]).show();
      } else if (type === "confirmDelete") {
        $("#confirmDeleteBtn").data("id", data.id);
        $("#deleteConfirmMessage").text("Are you sure you want to delete " + data.fullName + "?");
        bootstrap.Modal.getOrCreateInstance($("#deleteConfirmModal")[0]).show();
      }
    },

    hideModal: function (selector) {
      var modal = bootstrap.Modal.getInstance($(selector)[0]);
      if (modal) {
        modal.hide();
      }
    },

    showToast: function (message, type) {
      var $toast = $("#mainToast");
      $("#mainToastBody").text(message);
      $toast.removeClass("text-bg-primary text-bg-success text-bg-danger");
      $toast.addClass(type === "success" ? "text-bg-success" : type === "error" ? "text-bg-danger" : "text-bg-primary");
      bootstrap.Toast.getOrCreateInstance($toast[0]).show();
    },

    showInlineErrors: function (errors, scope) {
      this.clearInlineErrors();
      var scopedMap = {
        login: { username: ["loginUsername"], password: ["loginPassword"] },
        signup: { username: ["signupUsername"], password: ["signupPassword"], confirmPassword: ["signupConfirmPassword"], signupRole: ["signupRole"] },
        employee: {}
      };
      var inputMap = scopedMap[scope] || scopedMap.employee;

      Object.keys(errors).forEach(function (key) {
        var ids = inputMap[key] || [key];
        ids.forEach(function (id) {
          $("#" + id).addClass("is-invalid");
          $('[data-error-for="' + id + '"]').text(errors[key]);
        });
      });
    },

    clearFieldError: function (fieldId) {
      $("#" + fieldId).removeClass("is-invalid");
      $('[data-error-for="' + fieldId + '"]').text("");
    },

    clearInlineErrors: function () {
      $(".is-invalid").removeClass("is-invalid");
      $("[data-error-for]").text("");
    },

    setTableStates: function (isLoading, isEmpty) {
      $("#tableLoadingState").toggleClass("d-none", !isLoading);
      $("#tableEmptyState").toggleClass("d-none", !isEmpty);
      if (isEmpty) {
        $("#employeesTableBody").empty();
      }
    }
  };

  function appendPageItem($container, label, page, disabled, active) {
    var itemClass = "page-item";
    if (disabled) itemClass += " disabled";
    if (active) itemClass += " active";
    $container.append('<li class="' + itemClass + '"><button class="page-link" type="button" data-page="' + page + '">' + label + "</button></li>");
  }

  function formatCurrency(value) {
    return String.fromCharCode(8377) + (Number(value) || 0).toLocaleString("en-IN");
  }

  function formatDate(value) {
    return toInputDate(value);
  }

  function toInputDate(value) {
    return value ? String(value).split("T")[0] : "";
  }

  function escapeHtml(text) {
    return String(text || "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }

  function getDeptBadgeClass(dept) {
    if (dept === "Engineering") return "badge-dept-engineering";
    if (dept === "Marketing") return "badge-dept-marketing";
    if (dept === "HR") return "badge-dept-hr";
    if (dept === "Finance") return "badge-dept-finance";
    if (dept === "Operations") return "badge-dept-operations";
    return "bg-secondary";
  }

  function getStatusClass(status) {
    return status === "Active" ? "badge-status-active" : "badge-status-inactive";
  }

  global.uiService = uiService;
})(window, jQuery);

