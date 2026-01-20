/**
 * Results Charts JavaScript
 * Handles rendering of pie charts and bar graphs for election results using Chart.js
 */

// Global variables to store chart instances
let pieChartInstance = null;
let barChartInstance = null;
let partyPieChartInstance = null;
let partyBarChartInstance = null;

/**
 * Initialize both pie chart and bar chart with election data
 * @param {Object} data - Election data containing candidates array and totalVotes
 */
function initializeCharts(data) {
    if (!data || !data.candidates || data.candidates.length === 0) {
        console.warn('No candidate data available for charts');
        return;
    }

    // Destroy existing charts if they exist
    if (pieChartInstance) {
        pieChartInstance.destroy();
    }
    if (barChartInstance) {
        barChartInstance.destroy();
    }

    // Prepare data for charts
    const candidateNames = data.candidates.map(c => c.name);
    const voteCounts = data.candidates.map(c => c.votes);
    const partyNames = data.candidates.map(c => c.party);
    
    // Generate colors dynamically
    const colors = generateColors(data.candidates.length);

    // Initialize Pie Chart
    initializePieChart(candidateNames, voteCounts, partyNames, colors);

    // Initialize Bar Chart
    initializeBarChart(candidateNames, voteCounts, partyNames, colors);
}

/**
 * Initialize and render the pie chart
 */
function initializePieChart(candidateNames, voteCounts, partyNames, colors) {
    const ctx = document.getElementById('pieChart');
    
    if (!ctx) {
        console.error('Pie chart canvas element not found');
        return;
    }

    // Prepare labels with party information
    const labels = candidateNames.map((name, index) => 
        `${name} (${partyNames[index]})`
    );

    pieChartInstance = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                label: 'Votes',
                data: voteCounts,
                backgroundColor: colors,
                borderColor: colors.map(c => darkenColor(c, 0.2)),
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : 0;
                            return `${label}: ${value} votes (${percentage}%)`;
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Vote Distribution by Candidate',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });
}

/**
 * Initialize and render the bar chart
 */
function initializeBarChart(candidateNames, voteCounts, partyNames, colors) {
    const ctx = document.getElementById('barChart');
    
    if (!ctx) {
        console.error('Bar chart canvas element not found');
        return;
    }

    // Prepare labels with party information
    const labels = candidateNames.map((name, index) => 
        `${name}\n(${partyNames[index]})`
    );

    barChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Votes',
                data: voteCounts,
                backgroundColor: colors,
                borderColor: colors.map(c => darkenColor(c, 0.2)),
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        precision: 0
                    },
                    title: {
                        display: true,
                        text: 'Number of Votes',
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                },
                x: {
                    ticks: {
                        maxRotation: 45,
                        minRotation: 45,
                        font: {
                            size: 10
                        }
                    },
                    title: {
                        display: true,
                        text: 'Candidates',
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const value = context.parsed.y;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : 0;
                            return `Votes: ${value} (${percentage}%)`;
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Vote Count by Candidate',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });
}

/**
 * Generate an array of distinct colors for the charts
 * @param {number} count - Number of colors needed
 * @returns {Array} Array of color strings
 */
function generateColors(count) {
    const colorPalette = [
        '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF',
        '#FF9F40', '#FF6384', '#C9CBCF', '#4BC0C0', '#FF6384',
        '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40',
        '#FF6384', '#C9CBCF', '#4BC0C0', '#FF6384', '#36A2EB'
    ];

    // If we need more colors than available, generate random colors
    if (count > colorPalette.length) {
        const colors = [...colorPalette];
        for (let i = colorPalette.length; i < count; i++) {
            colors.push(generateRandomColor());
        }
        return colors;
    }

    return colorPalette.slice(0, count);
}

/**
 * Generate a random color
 * @returns {string} Hex color string
 */
function generateRandomColor() {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

/**
 * Darken a color by a certain percentage
 * @param {string} color - Hex color string
 * @param {number} percent - Percentage to darken (0-1)
 * @returns {string} Darkened hex color string
 */
function darkenColor(color, percent) {
    const num = parseInt(color.replace("#", ""), 16);
    const amt = Math.round(2.55 * percent * 100);
    const R = Math.max(0, Math.min(255, (num >> 16) - amt));
    const G = Math.max(0, Math.min(255, (num >> 8 & 0x00FF) - amt));
    const B = Math.max(0, Math.min(255, (num & 0x0000FF) - amt));
    return "#" + (0x1000000 + R * 0x10000 + G * 0x100 + B).toString(16).slice(1);
}

/**
 * Initialize party-wise charts (pie and bar)
 * @param {Object} data - Election data containing parties array
 */
function initializePartyCharts(data) {
    if (!data || !data.parties || data.parties.length === 0) {
        console.warn('No party data available for charts');
        return;
    }

    // Destroy existing charts if they exist
    if (partyPieChartInstance) {
        partyPieChartInstance.destroy();
    }
    if (partyBarChartInstance) {
        partyBarChartInstance.destroy();
    }

    // Prepare data for charts
    const partyNames = data.parties.map(p => p.name);
    const partyVoteCounts = data.parties.map(p => p.votes);
    
    // Generate colors dynamically
    const colors = generateColors(data.parties.length);

    // Initialize Party Pie Chart
    initializePartyPieChart(partyNames, partyVoteCounts, colors);

    // Initialize Party Bar Chart
    initializePartyBarChart(partyNames, partyVoteCounts, colors);
}

/**
 * Initialize and render the party pie chart
 */
function initializePartyPieChart(partyNames, voteCounts, colors) {
    const ctx = document.getElementById('partyPieChart');
    
    if (!ctx) {
        console.error('Party pie chart canvas element not found');
        return;
    }

    partyPieChartInstance = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: partyNames,
            datasets: [{
                label: 'Party Votes',
                data: voteCounts,
                backgroundColor: colors,
                borderColor: colors.map(c => darkenColor(c, 0.2)),
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        padding: 15,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : 0;
                            return `${label}: ${value} votes (${percentage}%)`;
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Party Vote Distribution',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });
}

/**
 * Initialize and render the party bar chart
 */
function initializePartyBarChart(partyNames, voteCounts, colors) {
    const ctx = document.getElementById('partyBarChart');
    
    if (!ctx) {
        console.error('Party bar chart canvas element not found');
        return;
    }

    partyBarChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: partyNames,
            datasets: [{
                label: 'Party Votes',
                data: voteCounts,
                backgroundColor: colors,
                borderColor: colors.map(c => darkenColor(c, 0.2)),
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        precision: 0
                    },
                    title: {
                        display: true,
                        text: 'Number of Votes',
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                },
                x: {
                    ticks: {
                        maxRotation: 45,
                        minRotation: 45,
                        font: {
                            size: 10
                        }
                    },
                    title: {
                        display: true,
                        text: 'Parties',
                        font: {
                            size: 12,
                            weight: 'bold'
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const value = context.parsed.y;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : 0;
                            return `Votes: ${value} (${percentage}%)`;
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Party Vote Count',
                    font: {
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });
}

/**
 * Clean up chart instances (useful for page navigation)
 */
function destroyCharts() {
    if (pieChartInstance) {
        pieChartInstance.destroy();
        pieChartInstance = null;
    }
    if (barChartInstance) {
        barChartInstance.destroy();
        barChartInstance = null;
    }
    if (partyPieChartInstance) {
        partyPieChartInstance.destroy();
        partyPieChartInstance = null;
    }
    if (partyBarChartInstance) {
        partyBarChartInstance.destroy();
        partyBarChartInstance = null;
    }
}

// Clean up charts when page is unloaded
window.addEventListener('beforeunload', function() {
    destroyCharts();
});

