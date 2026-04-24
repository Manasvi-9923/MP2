// validationService.js
// Client-side validation for API-backed forms.

(function (global) {
  var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  var validationService = {
    validateAuthForm: function (data) {
      var errors = {};
      var username = String(data.username || "").trim();
      var password = String(data.password || "");

      if (!username) {
        errors.username = "Username is required.";
      }
      if (!password) {
        errors.password = "Password is required.";
      } else if (data.isSignup && password.length < 6) {
        errors.password = "Password must be at least 6 characters.";
      }

      if (data.isSignup) {
        if (!data.confirmPassword) {
          errors.confirmPassword = "Confirm Password is required.";
        } else if (data.password !== data.confirmPassword) {
          errors.confirmPassword = "Password and Confirm Password must match.";
        }

        if (!data.role) {
          errors.signupRole = "Role is required.";
        }
      }

      return errors;
    },

    validateEmployeeForm: function (data) {
      var errors = {};

      if (!data.firstName) errors.firstName = "First name is required.";
      if (!data.lastName) errors.lastName = "Last name is required.";
      if (!data.email) {
        errors.email = "Email is required.";
      } else if (!emailPattern.test(String(data.email))) {
        errors.email = "Please enter a valid email.";
      }
      if (!data.phone) {
        errors.phone = "Phone is required.";
      } else if (!/^\d{10}$/.test(String(data.phone))) {
        errors.phone = "Phone must be a 10 digit number.";
      }
      if (!data.department) errors.department = "Department is required.";
      if (!data.designation) errors.designation = "Designation is required.";
      if (data.salary === undefined || data.salary === null || data.salary === "") {
        errors.salary = "Salary is required.";
      } else if (!Number.isFinite(Number(data.salary))) {
        errors.salary = "Salary must be a valid number.";
      } else if (Number(data.salary) <= 0) {
        errors.salary = "Salary must be a positive number.";
      }
      if (!data.joinDate) errors.joinDate = "Join date is required.";
      if (!data.status) errors.status = "Status is required.";

      return errors;
    },

    mapServerErrors: function (error) {
      var payload = error && error.payload ? error.payload : {};
      var message = String((payload && payload.message) || (error && error.message) || "");
      var lowered = message.toLowerCase();

      if (lowered.indexOf("username") !== -1) {
        return { username: message || "Username already exists." };
      }

      if (lowered.indexOf("email") !== -1) {
        return { email: message || "An employee with this email already exists." };
      }

      return {};
    }
  };

  global.validationService = validationService;
})(window);

