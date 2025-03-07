// employees-grid.js
console.log("employees-grid.js loading");

class EmployeeGrid {
    constructor() {
        this.gridId = "employeesGrid";
        this.gridInstance = null;
        this.initialize();
    }

    initialize() {
        console.log("Employee grid initialization started");
        $(document).ready(() => {
            console.log("Document ready, initializing grid");
            setTimeout(() => {
                this.gridInstance = $("#" + this.gridId).dxDataGrid("instance");
                if (this.gridInstance) {
                    console.log("Grid instance found");
                    this.setupEventHandlers();
                } else {
                    console.error("Grid instance could not be initialized.");
                }
            }, 500);
        });
    }

    setupEventHandlers() {
        if (!this.gridInstance) {
            console.error("Grid instance not available for setting up event handlers");
            return;
        }

        console.log("Setting up event handlers");

        // Add debug event handlers
        this.gridInstance.option("onDataErrorOccurred", (e) => {
            console.error("Data operation error:", e);
            console.error("Error details:", e.error);

            if (e.error && e.error.responseText) {
                try {
                    const response = JSON.parse(e.error.responseText);
                    GridUtils.showErrorMessage(response.message || "An error occurred");
                } catch (ex) {
                    GridUtils.showErrorMessage("An error occurred: " + e.error.statusText);
                }
            } else {
                GridUtils.showErrorMessage("An error occurred during data operation");
            }
            e.handled = true;
        });

        // Intercept toolbar button events for adding
        this.gridInstance.option("onToolbarPreparing", (e) => {
            // Find the add button
            const addButton = e.toolbarOptions.items.find(item => item.name === "addRowButton");
            if (addButton) {
                // Store the original click handler
                const originalClickHandler = addButton.onClick;

                // Replace with our own handler
                addButton.onClick = (clickEvent) => {
                    // Instead of letting the grid handle it, show our own form
                    this.showAddEmployeeForm();

                    // Prevent the default grid add action
                    clickEvent.event.preventDefault();
                    clickEvent.event.stopPropagation();
                };
            }
        });

        // Disable the default add behavior completely
        this.gridInstance.option("editing.allowAdding", false);
    }

    // Helper method to show a custom add employee form
    showAddEmployeeForm() {
        console.log("Sending payload:", JSON.stringify(data));

        // Get dealers and services data
        $.when(
            $.ajax({
                url: "/Home/GetDealersForUsersAndServices",
                method: "GET",
                dataType: "json"
            }),
            $.ajax({
                url: "/Home/GetServicesByID",
                method: "GET",
                dataType: "json"
            })
        ).done((dealersResponse, servicesResponse) => {
            // Verileri al
            const dealers = (dealersResponse[0].data) ? dealersResponse[0].data : dealersResponse[0];
            const services = (servicesResponse[0].data) ? servicesResponse[0].data : servicesResponse[0];

            // Create the form data
            const formData = {
                isTechnician: false,
                createdOn: new Date()
            };

            // Create a popup with form
            let popupElement = $("<div>").attr("id", "employee-add-popup").appendTo("body");

            let popup = popupElement.dxPopup({
                title: "Add Employee",
                width: 700,
                height: 500,
                showTitle: true,
                dragEnabled: true,
                hideOnOutsideClick: false,
                showCloseButton: true,
                visible: true,
                onHiding: function () {
                    popupElement.remove();
                }
            }).dxPopup("instance");

            // Create the form
            let formElement = $("<div>").appendTo(popup.$content());
            let formInstance = formElement.dxForm({
                formData: formData,
                items: [
                    {
                        itemType: "group",
                        colCount: 2,
                        colSpan: 2,
                        items: [
                            {
                                dataField: "name",
                                label: { text: "Name" },
                                validationRules: [{ type: "required" }]
                            },
                            {
                                dataField: "dealerID",
                                label: { text: "Dealer" },
                                editorType: "dxSelectBox",
                                editorOptions: {
                                    dataSource: dealers,
                                    displayExpr: "name",
                                    valueExpr: "id",
                                    placeholder: "Select Dealer"
                                }
                            },
                            {
                                dataField: "serviceID",
                                label: { text: "Service" },
                                editorType: "dxSelectBox",
                                editorOptions: {
                                    dataSource: services,
                                    displayExpr: "name",
                                    valueExpr: "id",
                                    placeholder: "Select Service"
                                }
                            },
                            {
                                dataField: "email",
                                label: { text: "Email" },
                                editorOptions: { readOnly: true }
                            },
                            {
                                dataField: "tcidno",
                                label: { text: "TC ID No" },
                                validationRules: [
                                    { type: "required", message: "TC Kimlik No zorunludur" },
                                    {
                                        type: "pattern",
                                        pattern: "^[0-9]{11}$",
                                        message: "TC Kimlik No 11 haneli rakamlardan oluşmalıdır"
                                    }
                                ]
                            },
                            {
                                dataField: "isTechnician",
                                label: { text: "Is Technician" },
                                editorType: "dxCheckBox"
                            }
                        ]
                    },
                    {
                        itemType: "button",
                        horizontalAlignment: "right",
                        buttonOptions: {
                            text: "Save",
                            type: "success",
                            useSubmitBehavior: false,
                            onClick: () => {
                                // Validate form
                                const validationResult = formInstance.validate();
                                if (!validationResult.isValid) {
                                    return;
                                }

                                // Get form data
                                const data = formInstance.option("formData");

                                // Check TCIDNO
                                if (!data.tcidno || data.tcidno.length !== 11 || !/^\d+$/.test(data.tcidno)) {
                                    GridUtils.showErrorMessage("TC Kimlik No 11 haneli rakamlardan oluşmalıdır");
                                    return;
                                }

                                // Validate TCIDNO on server
                                this.validateTCIDNO(data.tcidno, null)
                                    .then(isValid => {
                                        if (!isValid) return;

                                        // Submit to server
                                        $.ajax({
                                            url: "/Home/AddEmployee",
                                            method: "POST",
                                            data: JSON.stringify(data),
                                            contentType: "application/json",
                                            success: (response) => {
                                                GridUtils.showSuccessMessage("Employee added successfully");
                                                popup.hide();
                                                this.gridInstance.refresh();
                                            },
                                            error: (xhr) => {
                                                console.error("Add employee failed:", xhr);
                                                try {
                                                    if (xhr.responseText) {
                                                        const response = JSON.parse(xhr.responseText);
                                                        GridUtils.showErrorMessage(response.message || "Error adding employee");
                                                    } else {
                                                        GridUtils.showErrorMessage("Error adding employee: " + xhr.statusText);
                                                    }
                                                } catch (ex) {
                                                    GridUtils.showErrorMessage("Error adding employee");
                                                }
                                            }
                                        });
                                    });
                            }
                        }
                    }
                ],
                onFieldDataChanged: function (e) {
                    // Generate email when name changes
                    if (e.dataField === "tcidno") {
                        console.log("TCIDNO changed to:", e.value);
                    }
                }
            }).dxForm("instance");
        }).fail((error) => {
            console.error("Error loading data:", error);
            GridUtils.showErrorMessage("Error loading form data. Please try again.");
        });
    }

    // Helper method to validate TCIDNO
    validateTCIDNO(tcidno, currentId) {
        return new Promise((resolve, reject) => {
            // First validate the format
            if (!tcidno || tcidno.length !== 11 || !/^\d+$/.test(tcidno)) {
                GridUtils.showErrorMessage("TC Kimlik No 11 haneli rakamlardan oluşmalıdır");
                resolve(false);
                return;
            }

            // Call the validation endpoint
            $.ajax({
                url: "/Home/ValidateTCIDNO",
                method: "GET",
                data: {
                    tcidno: tcidno,
                    currentId: currentId || null
                },
                success: function (response) {
                    if (response.isValid) {
                        resolve(true);
                    } else {
                        GridUtils.showErrorMessage(response.message || "Bu TC Kimlik No sistemde zaten var");
                        resolve(false);
                    }
                },
                error: function (xhr) {
                    console.error("TCIDNO validation error:", xhr);
                    GridUtils.showErrorMessage("Server error during validation");
                    resolve(false);
                }
            });
        });
    }
}

// Global utility functions
window.GridUtils = {
    showErrorMessage: function (message) {
        console.error(message);
        DevExpress.ui.notify(message, "error", 3000);
    },
    showSuccessMessage: function (message) {
        console.log(message);
        DevExpress.ui.notify(message, "success", 3000);
    },
    generateTurkishEmail: function (name, domain) {
        if (!name) return "";
        // Simple email generation logic
        const cleanName = name.toLowerCase()
            .replace(/\s+/g, '.')
            .replace(/[öÖ]/g, 'o')
            .replace(/[çÇ]/g, 'c')
            .replace(/[şŞ]/g, 's')
            .replace(/[ıİ]/g, 'i')
            .replace(/[ğĞ]/g, 'g')
            .replace(/[üÜ]/g, 'u');
        return cleanName + "@" + domain;
    }
};

window.editEmployeeRecord = function (id) {
    console.log("Edit button clicked for ID:", id);

    try {
        // DevExtreme ve jQuery sürümlerini kontrol et
        console.log("DevExtreme version:", DevExpress.VERSION);
        console.log("jQuery version:", $.fn.jquery);

        // Grid instance kontrolü
        const grid = $("#employeesGrid").dxDataGrid("instance");
        if (!grid) {
            console.error("Grid instance not found");
            GridUtils.showErrorMessage("Grid instance not found");
            return;
        }

        // Get the employee data
        const dataSource = grid.getDataSource();
        const items = dataSource.items();
        const rowData = items.find(item => item.id === id);

        if (!rowData) {
            console.error("Employee data not found for ID:", id);
            GridUtils.showErrorMessage("Employee data not found for ID: " + id);
            return;
        }

        console.log("Found employee data:", rowData);

        // Temiz veri oluştur
        const cleanData = {};
        Object.keys(rowData).forEach(key => {
            cleanData[key] = rowData[key] === undefined ? null : rowData[key];
        });

        // Original TCIDNO for later validation
        const originalTCIDNO = cleanData.tcidno;

        // Önce dealer ve service verilerini al
        $.when(
            $.ajax({
                url: "/Home/GetDealersForUsersAndServices",
                method: "GET",
                dataType: "json"
            }),
            $.ajax({
                url: "/Home/GetServicesByID",
                method: "GET",
                dataType: "json"
            })
        ).done(function (dealersResponse, servicesResponse) {
            // Verileri al
            const dealers = (dealersResponse[0].data) ? dealersResponse[0].data : dealersResponse[0];
            const services = (servicesResponse[0].data) ? servicesResponse[0].data : servicesResponse[0];

            console.log("Loaded dealers:", dealers);
            console.log("Loaded services:", services);

            // Form referansını global seviyede tanımla (erişim için)
            let formInstance = null;

            // Popup div'ini oluştur
            let popupElement = $("<div>").attr("id", "employee-edit-popup").appendTo("body");
            console.log("Popup element created:", popupElement);

            // DevExtreme 23.1.6 için özel popup konfigürasyonu
            try {
                // Popup oluştur
                let popup = popupElement.dxPopup({
                    title: "Edit Employee",
                    width: 700,
                    height: 500,
                    showTitle: true,
                    dragEnabled: true,
                    hideOnOutsideClick: false,
                    showCloseButton: true,
                    visible: true,
                    onHiding: function () {
                        console.log("Popup hiding");
                        popupElement.remove();
                    },
                    onShown: function () {
                        console.log("Popup shown and visible");
                    },
                    toolbarItems: [{
                        widget: 'dxButton',
                        toolbar: 'bottom',
                        location: 'after',
                        options: {
                            text: 'Cancel',
                            onClick: function () {
                                console.log("Cancel button clicked");
                                popup.hide();
                            }
                        }
                    }, {
                        widget: 'dxButton',
                        toolbar: 'bottom',
                        location: 'after',
                        options: {
                            text: 'Save',
                            type: 'success',
                            onClick: function () {
                                console.log("Save button clicked in toolbar");

                                if (!formInstance) {
                                    console.error("Form instance not found");
                                    return;
                                }

                                // Form doğrulama
                                const validationResult = formInstance.validate();
                                console.log("Validation result:", validationResult);

                                if (!validationResult.isValid) {
                                    console.error("Form validation failed");
                                    return;
                                }

                                const updatedData = formInstance.option("formData");
                                console.log("Form data to be sent:", updatedData);

                                // Check TCIDNO format
                                if (!updatedData.tcidno || updatedData.tcidno.length !== 11 || !/^\d+$/.test(updatedData.tcidno)) {
                                    GridUtils.showErrorMessage("TC Kimlik No 11 haneli rakamlardan oluşmalıdır");
                                    return;
                                }

                                // Create a clean data object with only the fields we want to update
                                const dataToSend = {
                                    id: id,
                                    name: updatedData.name || "",
                                    email: updatedData.email || "",
                                    dealerID: updatedData.dealerID || 0,
                                    serviceID: updatedData.serviceID || 0,
                                    isTechnician: updatedData.isTechnician || false,
                                    tcidno: updatedData.tcidno || ""
                                };

                                // Check if TCIDNO is being modified
                                if (updatedData.tcidno !== originalTCIDNO) {
                                    // Create grid instance for validation
                                    const grid = new EmployeeGrid();

                                    // Validate that the new TCIDNO is not already in use
                                    grid.validateTCIDNO(updatedData.tcidno, id)
                                        .then(isValid => {
                                            if (isValid) {
                                                sendUpdateRequest(dataToSend);
                                            }
                                        });
                                } else {
                                    // If TCIDNO is unchanged, proceed with update
                                    sendUpdateRequest(dataToSend);
                                }

                                function sendUpdateRequest(data) {
                                    console.log("Data to be sent:", data);
                                    const jsonData = JSON.stringify(data);
                                    console.log("JSON data to send:", jsonData);

                                    // Send request directly to controller
                                    $.ajax({
                                        url: "/Home/UpdateEmployee",
                                        method: "PUT",
                                        data: jsonData,
                                        contentType: "application/json",
                                        success: function (response) {
                                            console.log("Update success:", response);
                                            GridUtils.showSuccessMessage("Employee updated successfully");
                                            popup.hide();
                                            grid.refresh();
                                        },
                                        error: function (xhr, status, error) {
                                            console.error("Update failed:", xhr);
                                            console.error("Status:", status);
                                            console.error("Error:", error);
                                            console.error("Response text:", xhr.responseText);

                                            let errorMessage = "Error updating employee";
                                            try {
                                                if (xhr.responseText) {
                                                    const response = JSON.parse(xhr.responseText);
                                                    errorMessage = response.message || errorMessage;
                                                }
                                            } catch (e) {
                                                errorMessage += ": " + xhr.statusText;
                                            }

                                            GridUtils.showErrorMessage(errorMessage);
                                        }
                                    });
                                }
                            }
                        }
                    }]
                }).dxPopup("instance");

                console.log("Popup instance created:", popup);

                // Popup oluşturulduktan sonra içeriği ayarla
                const contentElement = popup.$content();
                console.log("Content element created inside popup");

                // Form elementini oluştur
                const formElement = $("<div>").appendTo(contentElement);

                // Form oluştur - DOĞRU KULLANIM
                formInstance = formElement.dxForm({
                    formData: cleanData,
                    items: [
                        {
                            itemType: "group",
                            colCount: 2,
                            colSpan: 2,
                            items: [
                                {
                                    dataField: "name",
                                    label: { text: "Name" },
                                    validationRules: [{ type: "required" }]
                                },
                                {
                                    dataField: "dealerID",
                                    label: { text: "Dealer" },
                                    editorType: "dxSelectBox",
                                    editorOptions: {
                                        dataSource: dealers,
                                        displayExpr: "name",
                                        valueExpr: "id",
                                        placeholder: "Select Dealer"
                                    }
                                },
                                {
                                    dataField: "serviceID",
                                    label: { text: "Service" },
                                    editorType: "dxSelectBox",
                                    editorOptions: {
                                        dataSource: services,
                                        displayExpr: "name",
                                        valueExpr: "id",
                                        placeholder: "Select Service"
                                    }
                                },
                                {
                                    dataField: "email",
                                    label: { text: "Email" },
                                    editorOptions: { readOnly: true }
                                },
                                {
                                    dataField: "tcidno",
                                    label: { text: "TC ID No" },
                                    validationRules: [
                                        { type: "required", message: "TC Kimlik No zorunludur" },
                                        {
                                            type: "pattern",
                                            pattern: "^[0-9]{11}$",
                                            message: "TC Kimlik No 11 haneli rakamlardan oluşmalıdır"
                                        }
                                    ]
                                },
                                {
                                    dataField: "isTechnician",
                                    label: { text: "Is Technician" },
                                    editorType: "dxCheckBox"
                                }
                            ]
                        }
                    ],
                    onFieldDataChanged: function (e) {
                        console.log("Field changed:", e.dataField, "to", e.value);

                        // İsim değiştiğinde email güncelle
                        if (e.dataField === "name" && e.value && typeof GridUtils !== 'undefined' && typeof GridUtils.generateTurkishEmail === 'function') {
                            const email = GridUtils.generateTurkishEmail(e.value, "hyundaiassan.com.tr");
                            e.component.updateData("email", email);
                        }
                    },
                    onInitialized: function (e) {
                        console.log("Form initialized:", e.component);
                        formInstance = e.component;
                    }
                }).dxForm("instance");

                console.log("Form created inside popup content");

            } catch (popupError) {
                console.error("Error creating popup:", popupError);
                GridUtils.showErrorMessage("Error creating popup: " + popupError.message);

                // Hata oluşursa popup elementini temizle
                popupElement.remove();
            }
        }).fail(function (error) {
            console.error("Error loading dealer or service data:", error);
            GridUtils.showErrorMessage("Error loading dealer or service data. Please try again.");
        });

    } catch (error) {
        console.error("Error editing employee record:", error);
        GridUtils.showErrorMessage("Error editing employee record: " + error.message);
    }
};

// Global delete function - defined directly on window
window.deleteEmployeeRecord = function (id) {
    console.log("Delete button clicked for ID:", id);

    try {
        if (confirm("Are you sure you want to delete this employee?")) {
            const grid = $("#employeesGrid").dxDataGrid("instance");
            if (!grid) {
                console.error("Grid instance not found");
                return;
            }

            // Send AJAX request directly instead of using grid.deleteRow
            $.ajax({
                url: "/Home/DeleteEmployee?key=" + id,
                method: "DELETE",
                success: function (response) {
                    console.log("Delete success:", response);
                    GridUtils.showSuccessMessage("Employee deleted successfully");
                    grid.refresh();
                },
                error: function (xhr) {
                    console.error("Delete failed:", xhr);
                    let message = "Error deleting employee";

                    try {
                        if (xhr.responseText) {
                            const response = JSON.parse(xhr.responseText);
                            message = response.message || message;
                        }
                    } catch (e) {
                        message += ": " + xhr.statusText;
                    }

                    GridUtils.showErrorMessage(message);
                }
            });
        }
    } catch (error) {
        console.error("Error deleting employee record:", error);
        GridUtils.showErrorMessage("Error deleting employee record: " + error.message);
    }
};

// Initialize the grid
console.log("Creating EmployeeGrid instance");
$(function () {
    console.log("DOM ready, initializing employee grid");
    new EmployeeGrid();

    // Add console checks that functions exist
    console.log("editEmployeeRecord function exists:", typeof window.editEmployeeRecord === 'function');
    console.log("deleteEmployeeRecord function exists:", typeof window.deleteEmployeeRecord === 'function');
});