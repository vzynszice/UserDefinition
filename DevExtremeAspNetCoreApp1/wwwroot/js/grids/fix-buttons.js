// This script fixes the edit and delete buttons that aren't working
// It directly adds event handlers to the buttons in the grid

// Main function to add event handlers to grid buttons
function addEventHandlersToGridButtons() {
    console.log("Adding event handlers to grid buttons");

    // First, find all buttons in the grid
    const editButtons = document.querySelectorAll('.edit-btn');
    const deleteButtons = document.querySelectorAll('.delete-btn');

    console.log(`Found ${editButtons.length} edit buttons and ${deleteButtons.length} delete buttons`);

    // Add click handlers to edit buttons
    editButtons.forEach(button => {
        // Remove existing event listeners first (if any)
        const newButton = button.cloneNode(true);
        button.parentNode.replaceChild(newButton, button);

        // Add a new event listener
        newButton.addEventListener('click', async function (e) {
            e.preventDefault();
            e.stopPropagation();

            try {
                const userId = this.getAttribute('data-id');
                console.log("Edit button clicked for user ID:", userId);

                // Get user data and open edit form
                const userData = await getUserById(userId);
                openEditUserForm(userData);
            } catch (error) {
                console.error("Error handling edit click:", error);
                if (typeof GridUtils !== 'undefined') {
                    GridUtils.showErrorMessage("Kullanıcı düzenleme işleminde hata oluştu");
                } else {
                    alert("Kullanıcı düzenleme işleminde hata oluştu");
                }
            }
        });
    });

    // Add click handlers to delete buttons
    deleteButtons.forEach(button => {
        // Remove existing event listeners first (if any)
        const newButton = button.cloneNode(true);
        button.parentNode.replaceChild(newButton, button);

        // Add a new event listener
        newButton.addEventListener('click', async function (e) {
            e.preventDefault();
            e.stopPropagation();

            try {
                const userId = this.getAttribute('data-id');
                console.log("Delete button clicked for user ID:", userId);

                // Confirm deletion
                const confirmMessage = "Bu kullanıcıyı silmek istediğinize emin misiniz?";
                let confirmed = false;

                if (typeof GridUtils !== 'undefined' && typeof GridUtils.showConfirmDialog === 'function') {
                    confirmed = await GridUtils.showConfirmDialog(confirmMessage);
                } else {
                    confirmed = confirm(confirmMessage);
                }

                if (!confirmed) {
                    return;
                }

                // Delete the user
                await deleteUser(userId);
            } catch (error) {
                console.error("Error handling delete click:", error);
                if (typeof GridUtils !== 'undefined') {
                    GridUtils.showErrorMessage("Kullanıcı silme işleminde hata oluştu");
                } else {
                    alert("Kullanıcı silme işleminde hata oluştu");
                }
            }
        });
    });
}

// Function to run after the grid is rendered
function onContentRendered() {
    setTimeout(addEventHandlersToGridButtons, 500); // Add a slight delay to ensure DOM is ready
}

// Override the existing content ready function
if (typeof onGridContentReady === 'function') {
    const originalContentReady = onGridContentReady;
    window.onGridContentReady = function (e) {
        originalContentReady(e);
        onContentRendered();
    };
} else {
    window.onGridContentReady = function (e) {
        console.log("Grid content ready");
        onContentRendered();
    };
}

// For manual testing
function manuallyAddEventHandlers() {
    console.log("Manually adding event handlers");
    addEventHandlersToGridButtons();
}

// Make this function available globally
window.fixGridButtons = manuallyAddEventHandlers;

// Also run once when this script loads, in case the grid is already rendered
setTimeout(addEventHandlersToGridButtons, 1000);