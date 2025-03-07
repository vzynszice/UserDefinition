class DealerGrid {
    constructor() {
        this.gridId = "dealersGrid";
        this.gridInstance = null;
        this.initialize();
    }

    initialize() {
        console.log("Dealer grid initialization started");

        $(document).ready(() => {
            this.gridInstance = GridUtils.getGridInstance(this.gridId);
            if (this.gridInstance) {
                this.setupEventHandlers();
            } else {
                console.error("Grid instance could not be initialized.");
            }
        });
    }

    setupEventHandlers() {
        if (!this.gridInstance) return;

        // Kaydetme işlemi sırasında tetiklenen olay
        this.gridInstance.on("saving", this.handleSaving.bind(this));

        // Satır silme işlemi tamamlandığında tetiklenen olay
        this.gridInstance.on("rowRemoved", this.handleRowRemoved.bind(this));

        // Düzenleme başladığında tetiklenen olay
        this.gridInstance.on("editingStart", this.handleEditingStart.bind(this));
    }

    handleSaving(e) {
        console.log("Saving event triggered:", e);
        const change = e.changes[0];
        if (change && change.type !== "remove") {
            const data = change.data;
            if (data && data.name) {
                // Türkçe e-posta adresi oluştur
                data.email = GridUtils.generateTurkishEmail(data.name, "hyundaiassan.com.tr");
                e.changes[0].data = data;
            }
        }
    }

    async handleRowRemoved(e) {
        console.log("Row successfully removed:", e);
        try {
            await GridUtils.refreshGrid(this.gridId);
            GridUtils.showSuccessMessage("Dealer successfully deleted");
        } catch (error) {
            console.error("Error refreshing grid after row removal:", error);
            GridUtils.showErrorMessage("An error occurred while refreshing the grid.");
        }
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
                // Türkçe e-posta adresi oluştur
                formData.email = GridUtils.generateTurkishEmail(formData.name, "hyundai.com.tr");

                // E-posta alanını güncelle
                const emailEditor = form.getEditor("email");
                if (emailEditor) {
                    emailEditor.option("value", formData.email);
                }

                // Form verilerini güncelle
                form.option("formData", formData);

                // Değişiklikleri grid'e yansıt
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

    static async editDealerRecord(id) {
        try {
            var rowIndex1 = GridUtils.getRowIndexByKey("dealersGrid", id); // rowIndex burada tanımlanır
            if (rowIndex1 >= 0) {
                const grid = GridUtils.getGridInstance("dealersGrid");
                grid.editRow(rowIndex1);
            } else {
                console.error("Row index could not be found for ID:", id);
            }
        } catch (error) {
            console.error("Error editing dealer record:", error);
            GridUtils.showErrorMessage("An error occurred while editing the dealer record.");
        }
    }

    static async deleteDealerRecord(id) {
        try {
            const confirmed = await GridUtils.showConfirmDialog("Are you sure you want to delete this dealer?");
            if (confirmed) {
                var rowIndex1 = GridUtils.getRowIndexByKey("dealersGrid", id); // rowIndex burada tanımlanır
                if (rowIndex1 === -1) {
                    console.error("Row index could not be found for ID:", id);
                } else {
                    const grid = GridUtils.getGridInstance("dealersGrid");
                    grid.deleteRow(rowIndex1);
                }
            }
        } catch (error) {
            console.error("Error deleting dealer record:", error);
            GridUtils.showErrorMessage("An error occurred while deleting the dealer record.");
        }
    }
}

// Global fonksiyonlar
window.editDealerRecord = DealerGrid.editDealerRecord;
window.deleteDealerRecord = DealerGrid.deleteDealerRecord;

// Grid'i başlat
new DealerGrid();