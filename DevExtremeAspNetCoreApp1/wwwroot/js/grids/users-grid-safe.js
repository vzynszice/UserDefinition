// Global reference for the grid instance
var userGridInstance = null;

// Grid initialization handler
function onGridInitialized(e) {
    userGridInstance = e.component;
    console.log("User grid initialized");

    // Setup username validation
    setupUsernameValidation();

    // Setup grid event handlers
    addGridEventHandlers(e.component);
}

// Content ready handler
function onGridContentReady(e) {
    console.log("User grid content ready");
}

function setupDeleteHandler(grid) {
    grid.option("onRowRemoving", function (e) {
        e.cancel = true; // Cancel the default deletion

        if (!confirm("Are you sure you want to delete this user?")) {
            return;
        }

        // Simple DELETE request with query parameter
        $.ajax({
            url: "/Home/DeleteUser?key=" + e.key,
            method: "DELETE",
            success: function (response) {
                DevExpress.ui.notify("User deleted successfully", "success", 3000);
                grid.refresh();
            },
            error: function (xhr) {
                DevExpress.ui.notify("Failed to delete user", "error", 3000);
                console.error("Delete error:", xhr.responseText);
            }
        });
    });
}

// Setup username validation
function setupUsernameValidation() {
    // Add custom validation rule for username uniqueness
    DevExpress.validationEngine.registerValidationCallbacks({
        usernameUnique: {
            validationCallback: function (options) {
                var username = options.value;
                var userId = options.data && options.data.id || 0;

                // Create a deferred object for async validation
                var d = $.Deferred();

                // Skip validation if username is empty (other validators will catch this)
                if (!username) {
                    d.resolve(true);
                    return d.promise();
                }

                // Check if username exists on the server
                $.ajax({
                    url: "/Home/CheckUsername",
                    data: {
                        username: username,
                        userId: userId
                    },
                    success: function (result) {
                        console.log("Username check response:", result);
                        d.resolve(result.success !== false);
                    },
                    error: function () {
                        console.error("Error checking username");
                        d.resolve(true); // On error, assume validation passes
                    }
                });

                return d.promise();
            },
            message: "Bu kullanıcı adı zaten kullanımda"
        }
    });
}

// Add event handlers to the grid
function addGridEventHandlers(grid) {
    console.log(2)
    alert("asd");
    // Handle editor preparation
    grid.on("editorPreparing", function (e) {
        if (e.parentType !== "dataRow") return;

        // Handle name field
        if (e.dataField === "name") {
            var originalOnValueChanged = e.editorOptions.onValueChanged;
            e.editorOptions.onValueChanged = function (args) {
                if (originalOnValueChanged) {
                    originalOnValueChanged(args);
                }

                // Generate email when name changes
                generateEmail(grid, e.row);
            };
        }

        // Handle surname field
        if (e.dataField === "surname") {
            var originalOnValueChanged = e.editorOptions.onValueChanged;
            e.editorOptions.onValueChanged = function (args) {
                if (originalOnValueChanged) {
                    originalOnValueChanged(args);
                }

                // Generate email when surname changes
                generateEmail(grid, e.row);
            };
        }

        // Make email field read-only
        if (e.dataField === "email") {
            e.editorOptions.readOnly = true;
        }

        // Handle password field - make it required only for new records
        if (e.dataField === "password") {
            var isNewRecord = !e.key;

            if (!isNewRecord) {
                // For existing records, password is optional (blank keeps existing password)
                e.editorOptions.placeholder = "Leave empty to keep current password";

                // Remove required validation for existing users
                if (e.validationRules) {
                    e.validationRules = e.validationRules.filter(function (rule) {
                        return rule.type !== "required";
                    });
                }
            }
        }

        // Set up username validation
        if (e.dataField === "username") {
            if (!e.validationRules) {
                e.validationRules = [];
            }

            // Add required validation
            e.validationRules.push({
                type: "required",
                message: "Username is required"
            });

            // Add username uniqueness validation
            e.validationRules.push({
                type: "custom",
                validationCallback: DevExpress.validationEngine.getModelValidator("usernameUnique"),
                message: "Bu kullanıcı adı zaten kullanımda"
            });
        }
    });

    // Generate email on new row initialization
    grid.on("initNewRow", function (e) {

        alert(111);
        console.log("Initializing new row");
        debugger

        var popup = grid.option("editing.popup");
        alert(popup);

        if (popup) {
            popup.option("title", "ÜNAL");
        }
        // Default values can be set here if needed
        e.data = {
            id: 0,
            name: "",
            surname: "",
            username: "",
            email: "",
            password: "",
            dealerID: null,
            serviceID: null
        };
    });

    // Handle validation on editing
    grid.on("editingStart", function (e) {

        grid.option("editing.popup.title","edit ulan!")
        console.log("Editing started for row:", e.key);

        // Store original data for later comparison
        //e.component.option("originalRowData", $.extend({}, e.data));
    });

    // Before saving, ensure email is generated
    grid.on("saving", function (e) {
        if (e.changes.length > 0) {
            console.log("Saving changes:", e.changes);

            var change = e.changes[0];

            // Generate email if name and surname are provided
            if (change.type === "insert" || change.type === "update") {
                var data = change.type === "insert"
                    ? change.data
                    : $.extend({},
                        e.component.option("originalRowData"),
                        change.data);

                if (data.name && data.surname) {
                    // Generate and set email
                    var generatedEmail = (data.name + data.surname)
                        .toLowerCase()
                        .replace(/\s+/g, '') + "@hyundai.com.tr";

                    // Always update the email when name or surname changes
                    if (change.type === "insert") {
                        change.data.email = generatedEmail;
                    } else if (change.type === "update") {
                        // For updates, we always set the email based on current name and surname
                        change.data.email = generatedEmail;
                    }

                    console.log("Generated email:", generatedEmail);
                }
            }
        }
    });

    // FIX: Intercept DELETE requests to format them correctly
    // FIX: Intercept DELETE requests to format them correctly
    grid.option("onRowRemoving", function (e) {
        e.cancel = true; // Cancel the default deletion

        if (!confirm("Are you sure you want to delete this user?")) {
            return;
        }

        // Get the user ID
        var userId = e.key;

        // Log information for debugging
        console.log("Attempting to delete user with ID:", userId);

        // Make sure we actually have an ID to send
        if (!userId && userId !== 0) {
            DevExpress.ui.notify("Cannot delete user: Missing user ID", "error", 3000);
            return;
        }

        // Send a properly formatted DELETE request with the ID in the URL
        $.ajax({
            url: "/Home/DeleteUser?key=" + userId,
            method: "DELETE",
            success: function (response) {
                if (response && response.success) {
                    console.log("User deletion successful:", response);
                    DevExpress.ui.notify("User deleted successfully", "success", 3000);
                    // Refresh the grid
                    grid.refresh();
                } else {
                    console.error("User deletion failed:", response);
                    DevExpress.ui.notify(response?.message || "Failed to delete user", "error", 3000);
                }
            },
            error: function (xhr) {
                console.error("Error deleting user:", xhr.status, xhr.statusText);
                console.error("Response:", xhr.responseText);
                // Try to parse error message if possible
                try {
                    var errorResponse = JSON.parse(xhr.responseText);
                    DevExpress.ui.notify(errorResponse.message || "Server error: Failed to delete user", "error", 3000);
                } catch (e) {
                    DevExpress.ui.notify("Server error: Failed to delete user", "error", 3000);
                }
            }
        });
    });

    // FIX: Intercept INSERT/UPDATE requests to format them correctly
    var originalOnSaved = grid.option("onSaved");
    grid.option("onSaved", function (e) {
        // Let original handler run if it exists
        if (originalOnSaved) {
            originalOnSaved(e);
        }
    });

    // FIX: Handle form submission manually
    grid.option("onEditorPreparing", function (e) {
        if (e.parentType === "dataRow") {
            // Log the editor preparation for debugging
            console.log("Editor preparing for field:", e.dataField);
        }
    });

    // FIX: Add custom update handler
    grid.option("onRowUpdating", function (e) {
        e.cancel = $.Deferred();

        // Get the full user data by combining original with changes
        var userData = $.extend({}, e.oldData, e.newData);

        // Ensure email is generated if name or surname changes
        if ((userData.name && userData.surname) &&
            (e.newData.name !== undefined || e.newData.surname !== undefined)) {
            userData.email = (userData.name + userData.surname)
                .toLowerCase()
                .replace(/\s+/g, '') + "@hyundai.com.tr";
        }

        // Send update request
        $.ajax({
            url: "/Home/UpdateUser?key=" + e.key,
            method: "PUT",
            contentType: "application/json",
            data: JSON.stringify({ values: JSON.stringify(userData) }),
            success: function (response) {
                if (response.success) {
                    e.cancel.resolve(false); // Proceed with update
                    grid.refresh();
                } else {
                    e.cancel.resolve(true); // Cancel update
                    DevExpress.ui.notify(response.message || "Failed to update user", "error", 3000);
                }
            },
            error: function (xhr) {
                console.error("Error updating user:", xhr);
                e.cancel.resolve(true); // Cancel update
                DevExpress.ui.notify("Server error: Failed to update user", "error", 3000);
            }
        });
    });
}

// Function to generate email based on name and surname
function generateEmail(grid, row) {
    // Get the current data for the row being edited
    var formData = grid.option("editing.form").option("formData");

    // Only proceed if we have both name and surname
    if (formData.name && formData.surname) {
        var generatedEmail = (formData.name + formData.surname)
            .toLowerCase()
            .replace(/\s+/g, '') + "@hyundai.com.tr";

        // Update the email field in the form
        grid.option("editing.form").updateData("email", generatedEmail);

        console.log("Generated email:", generatedEmail);
    }
}

// Expose functions globally
window.onGridInitialized = onGridInitialized;
window.onGridContentReady = onGridContentReady;