// grid-utils.js
class GridUtils {
    // Get Grid Instance
    static getGridInstance(gridId) {
        const grid = $(`#${gridId}`).dxDataGrid("instance");
        if (!grid) {
            console.error(`Grid instance not found for ID: ${gridId}`);
        }
        return grid;
    }

    // Refresh Grid
    static async refreshGrid(gridId) {
        const grid = this.getGridInstance(gridId);
        if (grid) {
            try {
                await grid.refresh();
                console.log(`Grid ${gridId} refreshed successfully`);
            } catch (error) {
                console.error(`Error refreshing grid ${gridId}:`, error);
                this.showErrorMessage("Failed to refresh grid");
            }
        }
    }

    // Find row index by key
    static getRowIndexByKey(gridId, key) {
        const grid = this.getGridInstance(gridId);
        if (!grid) return -1;
        return grid.getRowIndexByKey(key);
    }

    // Format Turkish text and create email
    static generateTurkishEmail(text, domain) {
        if (!text || !domain) return "";

        return (text + "@" + domain)
            .toLowerCase()
            .replace(/\s+/g, '')
            .replace(/[ğ]/g, 'g')
            .replace(/[ü]/g, 'u')
            .replace(/[ş]/g, 's')
            .replace(/[ı]/g, 'i')
            .replace(/[ö]/g, 'o')
            .replace(/[ç]/g, 'c');
    }

    // Form validation
    static validateForm(form) {
        if (!form) return false;
        const validationGroup = form.option("validationGroup");
        if (!validationGroup) return true;

        const result = validationGroup.validate();
        return result.isValid;
    }

    // Notification functions
    static showSuccessMessage(message, displayTime = 2000) {
        DevExpress.ui.notify({
            message: message,
            type: "success",
            displayTime: displayTime,
            position: {
                my: "center top",
                at: "center top"
            }
        });
    }

    static showErrorMessage(message, displayTime = 3000) {
        DevExpress.ui.notify({
            message: message,
            type: "error",
            displayTime: displayTime,
            position: {
                my: "center top",
                at: "center top"
            }
        });
    }

    static showWarningMessage(message, displayTime = 3000) {
        DevExpress.ui.notify({
            message: message,
            type: "warning",
            displayTime: displayTime,
            position: {
                my: "center top",
                at: "center top"
            }
        });
    }

    // Confirmation dialog
    static showConfirmDialog(message, title = "Confirmation") {
        return new Promise((resolve) => {
            DevExpress.ui.dialog.confirm(message, title)
                .done(function (result) {
                    resolve(result);
                });
        });
    }

    // Form helper functions
    static getFormInstance(formElement) {
        return formElement.dxForm("instance");
    }

    static updateFormData(form, newData) {
        if (!form) return;
        const currentData = form.option("formData");
        form.option("formData", { ...currentData, ...newData });
    }

    // Grid helper functions
    static setGridEditingOptions(grid, options) {
        if (!grid) return;
        grid.option("editing", { ...grid.option("editing"), ...options });
    }

    static setGridTitle(grid, title) {

        text = "aaaaaaaaaa";
        if (!grid) return;
        //grid.option("editing.popup.title", title);
    }

    // Data manipulation
    static sanitizeData(data) {
        const sanitized = {};
        for (let key in data) {
            if (data[key] !== null && data[key] !== undefined) {
                sanitized[key] = data[key];
            }
        }
        return sanitized;
    }

    // Error handling
    static handleError(error, context = "") {
        console.error(`Error in ${context}:`, error);
        this.showErrorMessage(error.message || "An unexpected error occurred");
    }

    // Grid event helpers
    static attachGridEvents(grid, events) {
        if (!grid) return;

        for (const [eventName, handler] of Object.entries(events)) {
            grid.on(eventName, handler);
        }
    }

    // Common validation rules
    static getCommonValidationRules() {
        return {
            required: { type: "required", message: "This field is required" },
            email: { type: "email", message: "Invalid email address" },
            phone: { type: "pattern", pattern: "^[0-9]{10}$", message: "Invalid phone number" },
            numeric: { type: "numeric", message: "Must be a number" },
            date: { type: "required", message: "Date is required" }
        };
    }


    static onEditingStart: function(e){
    grid.option("editing.popup.title","eeee")
    }
    
}

// Make accessible globally
window.GridUtils = GridUtils;