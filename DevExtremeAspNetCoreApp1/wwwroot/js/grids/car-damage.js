/**
 * Car Damage Assessment JavaScript
 * Dependencies: jQuery, DevExtreme
 */


// Define our car object to store the state
let currentCar = {
    id: 0,
    plate: "",
    partDamages: {}
};

// Initialize the car data
function initializeCar() {
    console.log('Initializing car data...'); // Debug log

    // Get the car data from the hidden input
    const carDataString = document.getElementById('carData').value;
    console.log('Raw car data:', carDataString); // Debug log

    const carData = JSON.parse(carDataString);
    console.log('Parsed car data:', carData); // Debug log

    currentCar = {
        id: carData.id,
        plate: carData.plate,
        partDamages: {}
    };

    // Process the damages from database
    if (carData.partDamages && Array.isArray(carData.partDamages)) {
        carData.partDamages.forEach(damage => {
            if (!currentCar.partDamages[damage.partTypeId]) {
                currentCar.partDamages[damage.partTypeId] = [];
            }
            currentCar.partDamages[damage.partTypeId].push(damage.damageTypeId);
        });
    }

    console.log('Processed car data:', currentCar); // Debug log

    // Update SVG colors based on damages
    document.querySelectorAll('.car-part').forEach(part => {
        const partType = parseInt(part.getAttribute('data-part-type'));
        const damages = currentCar.partDamages[partType];

        // If no damage is recorded, set it as Original
        if (!damages || damages.length === 0) {
            currentCar.partDamages[partType] = [0]; // 0 = Original
        }

        // Get the latest damage type and update the color
        const lastDamage = currentCar.partDamages[partType][currentCar.partDamages[partType].length - 1];
        console.log(`Setting color for part ${partType}: damage ${lastDamage}`); // Debug log
        updateSvgColor(partType, lastDamage);
    });

    // Reload the grid with current data
    reloadGridData();
}
function reloadGridData() {
    console.log('Reloading grid data...'); // Debug log

    // Create grid data from car parts
    const gridData = Array.from(document.querySelectorAll('.car-part'))
        .map(part => {
            const partType = parseInt(part.getAttribute('data-part-type'));
            const damages = currentCar.partDamages[partType] || [0];

            const row = {
                partTypeId: partType,
                partType: partTypeNames[partType],
                damage_0: damages.includes(0), // Original
                damage_1: damages.includes(1), // Crushed
                damage_2: damages.includes(2), // Scratch
                damage_3: damages.includes(3), // Partialrepaint
                damage_4: damages.includes(4), // Repainted
                damage_5: damages.includes(5)  // Replaced
            };

            console.log(`Grid row for part ${partType}:`, row); // Debug log
            return row;
        });

    console.log('Final grid data:', gridData); // Debug log

    // Get the grid instance and update it
    const grid = $("#damageGrid").dxDataGrid("instance");

    grid.option({
        dataSource: gridData,
        loadPanel: {
            enabled: true
        },
        scrolling: {
            mode: 'standard',
            showScrollbar: 'always'
        }
    });

    // Force grid to repaint
    grid.repaint();
}
// Grid data loading function
function onGridLoad() {
    return Array.from(document.querySelectorAll('.car-part')).map(part => {
        const partType = parseInt(part.getAttribute('data-part-type'));
        const damages = currentCar.partDamages[partType] || [0];

        return {
            partTypeId: partType,
            partType: partTypeNames[partType],
            damage_0: damages.includes(0),
            damage_1: damages.includes(1),
            damage_2: damages.includes(2),
            damage_3: damages.includes(3),
            damage_4: damages.includes(4),
            damage_5: damages.includes(5)
        };
    });
}

// Grid cell click handler
function onGridCellClick(e) {
    if (!e.column.dataField?.startsWith('damage_')) return;

    const damageType = parseInt(e.column.dataField.split('_')[1]);
    const partType = e.data.partTypeId;
    const isCurrentlySelected = currentCar.partDamages[partType]?.includes(damageType);

    // Eğer Original seçildiyse ve zaten seçili değilse
    if (damageType === 0 && !isCurrentlySelected) {
        currentCar.partDamages[partType] = [0];
    }
    // Original dışında bir damage seçildiyse
    else if (damageType !== 0) {
        // Eğer seçili değilse ekle
        if (!isCurrentlySelected) {
            addDamage(partType, damageType);
        } else {
            // Seçili ise kaldır
            removeDamage(partType, damageType);
        }
    }

    // Grid ve SVG'yi güncelle
    updateSvgColor(partType, getCurrentDamageType(partType));
    reloadGridData();
}

// Add damage to a part
function addDamage(partType, damageType) {
    if (!currentCar.partDamages[partType]) {
        currentCar.partDamages[partType] = [];
    }

    const damages = currentCar.partDamages[partType];

    // Original (0) seçildiyse diğer tüm hasarları temizle
    if (damageType === 0) {
        currentCar.partDamages[partType] = [0];
    } else {
        // Original dışında bir hasar seçildiyse
        if (damages.includes(0)) {
            // Original'i kaldır
            damages.splice(damages.indexOf(0), 1);
        }
        if (!damages.includes(damageType)) {
            damages.push(damageType);
        }
    }

    // Grid ve SVG'yi güncelle
    updateSvgColor(partType, getCurrentDamageType(partType));
    reloadGridData();
}

// Remove damage from a part
function removeDamage(partType, damageType) {
    if (!currentCar.partDamages[partType]) return;

    const damages = currentCar.partDamages[partType];
    const index = damages.indexOf(damageType);

    if (index > -1) {
        damages.splice(index, 1);
    }

    // Hiç hasar kalmadıysa Original'e dön
    if (damages.length === 0) {
        damages.push(0);
    }

    // Grid ve SVG'yi güncelle
    updateSvgColor(partType, getCurrentDamageType(partType));
    reloadGridData();
}

function refreshGridAfterDamageChange(partType) {
    const grid = $("#damageGrid").dxDataGrid("instance");
    if (grid) {
        // Tüm grid verilerini yeniden yükle
        grid.refresh().done(function () {
            // Seçili satırı koru
            grid.selectRows([partType], false);
        });
    }
}
// Get current damage type for a part
function getCurrentDamageType(partType) {
    const damages = currentCar.partDamages[partType];
    return damages ? damages[damages.length - 1] : 0;
}

// Update SVG color based on damage type
function updateSvgColor(partType, damageType) {
    const part = document.querySelector(`.car-part[data-part-type="${partType}"]`);
    if (!part) return;

    const color = DAMAGE_COLORS[damageType] || DAMAGE_COLORS[0];
    const paths = part.querySelectorAll('path');
    paths.forEach(path => {
        path.style.fill = color;
    });
}

// Handle damage selection from selector
function handleDamageSelection(damageType) {
    const selectedPart = document.querySelector('.car-part.selected');
    if (!selectedPart) return;

    const partType = parseInt(selectedPart.getAttribute('data-part-type'));
    const isCurrentlySelected = currentCar.partDamages[partType]?.includes(damageType);

    // Original seçildiyse
    if (damageType === 0) {
        currentCar.partDamages[partType] = [0];
    }
    // Original dışında bir damage seçildiyse
    else {
        if (isCurrentlySelected) {
            removeDamage(partType, damageType);
        } else {
            addDamage(partType, damageType);
        }
    }

    // Grid, SVG ve selector'ı güncelle
    updateSvgColor(partType, getCurrentDamageType(partType));
    updateDamageSelectorVisuals(partType);
    reloadGridData();
}

// Set up SVG click handlers
function setupSvgPartClickHandlers() {
    const carParts = document.querySelectorAll('.car-part');

    carParts.forEach(part => {
        part.addEventListener('click', function (e) {
            document.querySelectorAll('.car-part').forEach(p =>
                p.classList.remove('selected'));

            this.classList.add('selected');

            const selector = document.getElementById('damageSelector');
            const rect = this.getBoundingClientRect();
            const containerRect = document.querySelector('.car-svg-container').getBoundingClientRect();

            // Get the part type and name
            const partType = parseInt(this.getAttribute('data-part-type'));
            const partName = partTypeNames[partType];

            // Update the part name in the selector
            const titleElement = selector.querySelector('.selected-part-name');
            if (titleElement) {
                titleElement.textContent = partName;
            }

            selector.style.display = 'block';
            selector.style.left = `${rect.right - containerRect.left + 10}px`;
            selector.style.top = `${rect.top - containerRect.top}px`;

            updateDamageSelectorVisuals(partType);
        });
    });
}
// Update damage selector visual state
function updateDamageSelectorVisuals(partType) {
    const damages = currentCar.partDamages[partType] || [0];
    const selector = document.getElementById('damageSelector');
    const options = selector.querySelectorAll('.damage-option');

    options.forEach(option => {
        const damageType = parseInt(option.getAttribute('data-damage-type'));
        option.classList.toggle('active', damages.includes(damageType));
    });
}

// Initialize everything when document is ready
$(document).ready(function () {
    setupSvgPartClickHandlers();
    initializeCar();
    const grid = $("#damageGrid").dxDataGrid("instance");
    if (grid) {
        grid.option({
            loadPanel: {
                enabled: false
            },
            scrolling: {
                mode: 'standard',
                showScrollbar: 'always'
            }
        });
    }

    reloadGridData(); // İlk yükleme

    // Damage selector click handlers
    const options = document.querySelectorAll('.damage-option');
    options.forEach(option => {
        option.addEventListener('click', function (e) {
            e.stopPropagation();
            const damageType = parseInt(this.getAttribute('data-damage-type'));
            handleDamageSelection(damageType);
        });
    });

    // Dışarı tıklama handler'ı
    document.addEventListener('click', function (e) {
        const selector = document.getElementById('damageSelector');
        const carPart = e.target.closest('.car-part');
        const damageOption = e.target.closest('.damage-option');

        if (!carPart && !damageOption) {
            selector.style.display = 'none';
            document.querySelectorAll('.car-part').forEach(part =>
                part.classList.remove('selected'));
        }
    });
});

// Grid selection changed handler
function onGridSelectionChanged(e) {
    const selectedPartType = e.selectedRowKeys[0];
    if (selectedPartType !== undefined) {
        document.querySelectorAll('.car-part').forEach(part => {
            const partType = parseInt(part.getAttribute('data-part-type'));
            part.classList.toggle('selected', partType === selectedPartType);
        });
    }
}

// Clean all damages from selected part and set it to Original
function cleanPartDamages() {
    // Get the currently selected part
    const selectedPart = document.querySelector('.car-part.selected');
    if (!selectedPart) return;  // If no part is selected, exit

    // Get the part type from the selected part
    const partType = parseInt(selectedPart.getAttribute('data-part-type'));

    // Reset the part's damages to only Original (0)
    currentCar.partDamages[partType] = [0];

    // Update the visuals
    updateSvgColor(partType, 0);  // Update the SVG color to Original
    updateDamageSelectorVisuals(partType);  // Update the damage selector buttons
    reloadGridData();  // Refresh the grid to show the changes
}

// Save damages to database
async function saveDamages() {
    const damages = [];

    Object.entries(currentCar.partDamages).forEach(([partTypeId, damageTypes]) => {
        if (damageTypes.length === 1 && damageTypes[0] === 0) {
            return;
        }

        damageTypes.forEach(damageType => {
            damages.push({
                partTypeId: parseInt(partTypeId),
                damageTypeId: damageType
            });
        });
    });

    const response = await fetch(`/api/CarDamage/SaveDamages/${currentCar.id}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(damages)
    });

    if (response.status === 204) { // No Content durumu için
        DevExpress.ui.notify({
            message: "Changes saved successfully",
            type: "success",
            displayTime: 3000
        });
        return;
    }

    const data = await response.text(); // önce text olarak alalım
    const jsonData = data ? JSON.parse(data) : {}; // boş değilse parse edelim

    if (response.ok) {
        DevExpress.ui.notify({
            message: "Changes saved successfully",
            type: "success",
            displayTime: 3000
        });
    } else {
        DevExpress.ui.notify({
            message: jsonData.message || 'Failed to save damages',
            type: "error",
            displayTime: 3000
        });
    }
}