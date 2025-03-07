// services-grid.js
class ServiceGrid {
    constructor() {
        this.gridId = "servicesGrid";
        this.gridInstance = null;
        this.initialize();
    }

    // Add this at the top of your ServiceGrid class initialization
    initialize() {
        console.log("Service grid initialization started");
        $(document).ready(() => {
            // Configure jQuery AJAX defaults for all requests
            $.ajaxSetup({
                contentType: "application/json; charset=utf-8",
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("Content-Type", "application/json");
                    xhr.setRequestHeader("Accept", "application/json");
                }
            });

            this.gridInstance = GridUtils.getGridInstance(this.gridId);
            this.setupEventHandlers();
        });
    }

    setupEventHandlers() {
        if (!this.gridInstance) return;
        this.gridInstance.on("saving", this.handleSaving.bind(this));
        this.gridInstance.on("rowRemoved", this.handleRowRemoved.bind(this));
        this.gridInstance.on("editingStart", this.handleEditingStart.bind(this));

        // Add this new handler for initialization
        this.gridInstance.option("onInitNewRow", function (e) {
            // Initialize default values for new records if needed
            e.data = e.data || {};
        });

        // Add this to intercept before the request is sent
        this.gridInstance.option("onSaving", function (e) {
            if (e.changes && e.changes.length > 0 && e.changes[0].type === "insert") {
                // Convert the data to the expected format
                e.cancel = true;

                // Get the data from the changes
                const data = e.changes[0].data;

                // Create a JSON payload manually
                const payload = JSON.stringify(data);

                // Send the request manually
                $.ajax({
                    url: "/Home/AddService",
                    method: "POST",
                    data: payload,
                    contentType: "application/json; charset=utf-8",
                    success: function (response) {
                        if (response.success) {
                            // Refresh the grid after successful insertion
                            e.component.refresh();
                            GridUtils.showSuccessMessage("Service successfully added");
                        } else {
                            GridUtils.showErrorMessage(response.message || "Failed to add service");
                        }
                    },
                    error: function (xhr) {
                        let errorMessage = "Failed to add service";
                        try {
                            const response = JSON.parse(xhr.responseText);
                            if (response && response.message) {
                                errorMessage = response.message;
                            }
                        } catch (e) { }
                        GridUtils.showErrorMessage(errorMessage);
                    }
                });
            }
        });
    }

    // Add this function to your ServiceGrid class
    handleSaving(e) {
        console.log("Saving event triggered:", e);
        console.log("Changes format:", JSON.stringify(e.changes));

        const change = e.changes[0];
        if (change && change.type !== "remove") {
            const data = change.data;
            if (data && data.name) {
                data.email = GridUtils.generateTurkishEmail(data.name, "hyundaiassan.com.tr");
                e.changes[0].data = data;

                // Log the final data being sent
                console.log("Final data being sent:", JSON.stringify(data));
            }
        }
    }

    async handleRowRemoved(e) {
        console.log("Row successfully removed:", e);
        await GridUtils.refreshGrid(this.gridId);
        GridUtils.showSuccessMessage("Service successfully deleted");
    }

    handleEditingStart(e) {
        console.log("Editing started:", e);
        const editForm = e.component.getController('editing')._editForm;
        if (editForm) {
            editForm.option("onFieldDataChanged", this.handleFieldDataChanged.bind(this));
        }
    }

    handleFieldDataChanged(e) {
        try {
            if (e.dataField !== "name") return;

            const popup = this.gridInstance._editPopup;
            const form = popup.$content().find(".dx-form").dxForm("instance");

            if (!form) {
                GridUtils.showErrorMessage("Form instance not found");
                return;
            }

            const formData = form.option("formData");
            if (formData.name) {
                formData.email = GridUtils.generateTurkishEmail(formData.name, "hyundaiassan.com.tr");

                const emailEditor = form.getEditor("email");
                if (emailEditor) {
                    emailEditor.option("value", formData.email);
                }

                form.option("formData", formData);
                const changes = this.gridInstance.option("editing.changes");

                if (changes.length > 0) {
                    changes[0].data = { ...changes[0].data, ...formData };
                    this.gridInstance.option("editing.changes", changes);
                }
            }
        } catch (error) {
            console.error("Error in handleFieldDataChanged:", error);
            GridUtils.showErrorMessage("An error occurred while updating the form");
        }
    }

    static async editServiceRecord(id) {
        const grid = GridUtils.getGridInstance("servicesGrid");
        if (grid) {
            grid.editRow(grid.getRowIndexByKey(id));
        }
    }

    static async deleteServiceRecord(id) {
        const confirmed = await GridUtils.showConfirmDialog("Are you sure you want to delete this service?");
        if (confirmed) {
            const grid = GridUtils.getGridInstance("servicesGrid");
            if (grid) {
                grid.deleteRow(grid.getRowIndexByKey(id));
            }
        }
    }
}

// Global functions for the window object
window.editServiceRecord = ServiceGrid.editServiceRecord;
window.deleteServiceRecord = ServiceGrid.deleteServiceRecord;

// Initialize the grid
new ServiceGrid();